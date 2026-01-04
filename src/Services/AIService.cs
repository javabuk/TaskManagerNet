using System;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TaskManager.Configuration;

namespace TaskManager.Services;

/// <summary>
/// Servicio de IA mejorado para soportar múltiples proveedores y modelos configurables
/// Permite seleccionar el modelo a utilizar en cada solicitud
/// </summary>
public class AIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly List<AIServiceConfiguration> _aiConfigurations;
    private readonly string _defaultModel;
    private readonly ILoggerService _loggerService;

    public AIService(IConfiguration configuration, ILoggerService loggerService)
    {
        _loggerService = loggerService;
        _httpClient = new HttpClient();

        // Cargar configuración de múltiples proveedores IA
        var aiProvidersSection = configuration.GetSection("AIServiceProviders");
        _aiConfigurations = new List<AIServiceConfiguration>();

        if (aiProvidersSection.Exists())
        {
            aiProvidersSection.Bind(_aiConfigurations);
        }

        if (_aiConfigurations.Count == 0)
        {
            _loggerService.LogWarning("[AIService] No se encontró configuración de AIServiceProviders. Usando valores por defecto.");
            // Configuración por defecto para compatibilidad backwards
            _aiConfigurations.Add(new AIServiceConfiguration
            {
                Id = "kimi-default",
                Model = "moonshotai/kimi-k2-instruct-0905",
                ApiUrl = "https://api.groq.com/openai/v1/chat/completions",
                ApiKey = "",
                Temperature = 0.6m,
                MaxCompletionTokens = 4096,
                TopP = 1.0m
            });
        }

        _defaultModel = configuration["DefaultAIModel"] ?? "moonshotai/kimi-k2-instruct-0905";
    }

    /// <summary>
    /// Obtiene sugerencias usando el modelo especificado (o el modelo por defecto si no se especifica)
    /// </summary>
    public async Task<string> GetSuggestionsAsync(string prompt, string? modelName = null)
    {
        var selectedModel = modelName ?? _defaultModel;
        var config = GetAIConfiguration(selectedModel);

        if (config == null)
        {
            _loggerService.LogError($"[AIService] Modelo '{selectedModel}' no encontrado en la configuración");
            throw new InvalidOperationException($"El modelo '{selectedModel}' no está configurado. Modelos disponibles: {string.Join(", ", _aiConfigurations.Select(c => c.Model))}");
        }

        if (string.IsNullOrWhiteSpace(config.ApiKey))
        {
            _loggerService.LogError($"[AIService] API Key no configurada para modelo '{selectedModel}' (Id: {config.Id})");
            throw new InvalidOperationException($"API Key no está configurada para el modelo '{selectedModel}'. Por favor, configure 'AIServiceProviders[].ApiKey' en appsettings.local.json");
        }

        try
        {
            _loggerService.LogInfo($"[AIService] Preparando solicitud para modelo: {config.Model}");
            var requestBody = BuildRequestBody(prompt, config);
            var jsonContent = JsonSerializer.Serialize(requestBody);

            _loggerService.LogInfo($"[AIService] Enviando solicitud a API");
            _loggerService.LogInfo($"[AIService] URL: {config.ApiUrl}");
            _loggerService.LogInfo($"[AIService] Modelo: {config.Model}");
            _loggerService.LogInfo($"[AIService] Prompt preview: {prompt.Substring(0, Math.Min(200, prompt.Length))}...");

            var request = new HttpRequestMessage(HttpMethod.Post, config.ApiUrl)
            {
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
            };

            request.Headers.Add("Authorization", $"Bearer {config.ApiKey}");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _loggerService.LogError($"[AIService] Error en solicitud API: {response.StatusCode}");
                _loggerService.LogError($"[AIService] Respuesta: {errorContent}");
                throw new HttpRequestException($"Error en la solicitud a API: {response.StatusCode} - {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            _loggerService.LogInfo($"[AIService] Respuesta recibida exitosamente (longitud: {responseContent.Length} caracteres)");

            return ParseApiResponse(responseContent);
        }
        catch (HttpRequestException ex)
        {
            _loggerService.LogError($"[AIService] Error de conexión: {ex.Message}", ex);
            throw;
        }
        catch (Exception ex)
        {
            _loggerService.LogError($"[AIService] Error inesperado: {ex.Message}", ex);
            throw;
        }
    }

    /// <summary>
    /// Obtiene la configuración de un modelo por su nombre
    /// </summary>
    private AIServiceConfiguration? GetAIConfiguration(string modelName)
    {
        return _aiConfigurations.FirstOrDefault(c => c.Model == modelName);
    }

    /// <summary>
    /// Construye el cuerpo de la solicitud HTTP incluyendo parámetros base y personalizados
    /// </summary>
    private object BuildRequestBody(string prompt, AIServiceConfiguration config)
    {
        var baseRequest = new
        {
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = prompt
                }
            },
            model = config.Model,
            temperature = (double)config.Temperature,
            max_completion_tokens = config.MaxCompletionTokens,
            top_p = (double)config.TopP,
            stream = false,
            stop = config.Stop
        };

        // Si hay parámetros adicionales específicos del modelo, incluirlos
        if (config.CustomParams.Count == 0)
        {
            return baseRequest;
        }

        // Para modelos con parámetros personalizados, usar un diccionario dinámico
        var requestDict = new Dictionary<string, object?>
        {
            { "messages", baseRequest.GetType().GetProperty("messages")!.GetValue(baseRequest)! },
            { "model", config.Model },
            { "temperature", (double)config.Temperature },
            { "max_completion_tokens", config.MaxCompletionTokens },
            { "top_p", (double)config.TopP },
            { "stream", false },
            { "stop", config.Stop }
        };

        // Agregar parámetros personalizados
        foreach (var kvp in config.CustomParams)
        {
            requestDict[kvp.Key] = kvp.Value;
        }

        return requestDict;
    }

    /// <summary>
    /// Parsea la respuesta JSON de la API extrayendo el contenido del mensaje
    /// </summary>
    private string ParseApiResponse(string responseContent)
    {
        var jsonDocument = JsonDocument.Parse(responseContent);
        var root = jsonDocument.RootElement;

        if (root.TryGetProperty("choices", out var choicesElement) && choicesElement.GetArrayLength() > 0)
        {
            var firstChoice = choicesElement[0];
            if (firstChoice.TryGetProperty("message", out var messageElement) &&
                messageElement.TryGetProperty("content", out var contentElement))
            {
                var content = contentElement.GetString() ?? "";
                _loggerService.LogInfo($"[AIService] Contenido extraído exitosamente (longitud: {content.Length} caracteres)");
                return content;
            }
        }

        _loggerService.LogError("[AIService] No se pudo extraer el contenido de la respuesta");
        throw new InvalidOperationException("No se pudo extraer el contenido de la respuesta de la API");
    }

    /// <summary>
    /// Obtiene la lista de modelos disponibles configurados
    /// </summary>
    public List<string> GetAvailableModels()
    {
        return _aiConfigurations.Select(c => c.Model).ToList();    }
}