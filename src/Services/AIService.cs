using System;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace TaskManager.Services;

public class AIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _apiUrl;
    private readonly string _model;
    private readonly decimal _temperature;
    private readonly int _maxCompletionTokens;
    private readonly decimal _topP;
    private readonly ILoggerService _loggerService;

    public AIService(IConfiguration configuration, ILoggerService loggerService)
    {
        _loggerService = loggerService;

        var aiConfig = configuration.GetSection("AIServices");
        _apiKey = aiConfig["GroqApiKey"] ?? "";
        _apiUrl = aiConfig["GroqApiUrl"] ?? "https://api.groq.com/openai/v1/chat/completions";
        _model = aiConfig["GroqModel"] ?? "openai/gpt-oss-120b";
        _temperature = decimal.TryParse(aiConfig["Temperature"], NumberStyles.Any, CultureInfo.InvariantCulture, out var temp) ? temp : 1.0m;
        _maxCompletionTokens = int.TryParse(aiConfig["MaxCompletionTokens"], out var maxTokens) ? maxTokens : 8192;
        _topP = decimal.TryParse(aiConfig["TopP"], NumberStyles.Any, CultureInfo.InvariantCulture, out var topP) ? topP : 1.0m;

        _httpClient = new HttpClient();
    }

    public async Task<string> GetSuggestionsAsync(string prompt)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            _loggerService.LogError("GROQ_API_KEY no está configurada en appsettings.local.json");
            throw new InvalidOperationException("API Key de Groq no está configurada. Por favor, configure 'AIServices:GroqApiKey' en appsettings.local.json");
        }

        try
        {
            var requestBody = new
            {
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                },
                model = _model,
                temperature = (double)_temperature,
                max_completion_tokens = _maxCompletionTokens,
                top_p = (double)_topP,
                stream = false,
                stop = (string?)null
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            _loggerService.LogInfo($"[AIService] Enviando solicitud a Groq API");
            _loggerService.LogInfo($"[AIService] URL: {_apiUrl}");
            _loggerService.LogInfo($"[AIService] Modelo: {_model}");
            _loggerService.LogInfo($"[AIService] Prompt preview: {prompt.Substring(0, Math.Min(200, prompt.Length))}...");

            var request = new HttpRequestMessage(HttpMethod.Post, _apiUrl)
            {
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
            };

            request.Headers.Add("Authorization", $"Bearer {_apiKey}");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _loggerService.LogError($"[AIService] Error en solicitud a Groq API: {response.StatusCode}");
                _loggerService.LogError($"[AIService] Respuesta: {errorContent}");
                throw new HttpRequestException($"Error en la solicitud a Groq API: {response.StatusCode} - {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            _loggerService.LogInfo($"[AIService] Respuesta recibida exitosamente (longitud: {responseContent.Length} caracteres)");

            // Parsear la respuesta JSON
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

            _loggerService.LogError("[AIService] No se pudo extraer el contenido de la respuesta de Groq");
            throw new InvalidOperationException("No se pudo extraer el contenido de la respuesta de Groq API");
        }
        catch (HttpRequestException ex)
        {
            _loggerService.LogError($"[AIService] Error de conexión con Groq API: {ex.Message}", ex);
            throw;
        }
        catch (Exception ex)
        {
            _loggerService.LogError($"[AIService] Error inesperado al llamar a Groq API: {ex.Message}", ex);
            throw;
        }
    }
}
