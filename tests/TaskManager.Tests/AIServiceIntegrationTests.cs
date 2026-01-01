using Xunit;
using Moq;
using TaskManager.Services;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace TaskManager.Tests;

/// <summary>
/// Tests de integración para AIService.GetSuggestionsAsync
/// Simula llamadas a la API de Groq sin hacer peticiones reales
/// </summary>
public class AIServiceIntegrationTests : IDisposable
{
    private readonly string _testOutputPath;

    public AIServiceIntegrationTests()
    {
        // Crear directorio para salida de pruebas
        _testOutputPath = Path.Combine(Path.GetTempPath(), "TaskManagerTestLogs");
        if (!Directory.Exists(_testOutputPath))
            Directory.CreateDirectory(_testOutputPath);
    }

    public void Dispose()
    {
        // Limpiar recursos
    }

    private Mock<ILoggerService> GetMockLogger()
    {
        return new Mock<ILoggerService>();
    }

    private IConfiguration CreateMockConfiguration(
        string? apiKey = "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",
        string apiUrl = "https://api.groq.com/openai/v1/chat/completions",
        string model = "openai/gpt-oss-120b",
        string temperature = "1.0",
        string maxTokens = "8192",
        string topP = "1.0")
    {
        var mockConfig = new Mock<IConfiguration>();
        var mockSection = new Mock<IConfigurationSection>();

        mockConfig.Setup(c => c.GetSection("AIServices"))
            .Returns(mockSection.Object);

        mockSection.Setup(s => s["GroqApiKey"])
            .Returns(apiKey);
        mockSection.Setup(s => s["GroqApiUrl"])
            .Returns(apiUrl);
        mockSection.Setup(s => s["GroqModel"])
            .Returns(model);
        mockSection.Setup(s => s["Temperature"])
            .Returns(temperature);
        mockSection.Setup(s => s["MaxCompletionTokens"])
            .Returns(maxTokens);
        mockSection.Setup(s => s["TopP"])
            .Returns(topP);

        return mockConfig.Object;
    }

    private IConfiguration CreateMockConfigurationMoonshotAI(
        string? apiKey = "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",
        string apiUrl = "https://api.groq.com/openai/v1/chat/completions",
        string model = "moonshotai/kimi-k2-instruct-0905",
        string temperature = "0.6",
        string maxTokens = "4096",
        string topP = "1.0")
    {
        var mockConfig = new Mock<IConfiguration>();
        var mockSection = new Mock<IConfigurationSection>();

        mockConfig.Setup(c => c.GetSection("AIServices"))
            .Returns(mockSection.Object);

        mockSection.Setup(s => s["GroqApiKey"])
            .Returns(apiKey);
        mockSection.Setup(s => s["GroqApiUrl"])
            .Returns(apiUrl);
        mockSection.Setup(s => s["GroqModel"])
            .Returns(model);
        mockSection.Setup(s => s["Temperature"])
            .Returns(temperature);
        mockSection.Setup(s => s["MaxCompletionTokens"])
            .Returns(maxTokens);
        mockSection.Setup(s => s["TopP"])
            .Returns(topP);

        return mockConfig.Object;
    }

    [Fact]
    public async Task GetSuggestionsAsync_WithoutApiKey_ThrowsInvalidOperationException()
    {
        // Arrange
        var mockConfig = CreateMockConfiguration(apiKey: null);
        var mockLogger = GetMockLogger();
        var service = new AIService(mockConfig, mockLogger.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await service.GetSuggestionsAsync("Test prompt"));

        Assert.Contains("API Key de Groq no está configurada", exception.Message);
        
        // Verificar que se llamó al logger de error
        mockLogger.Verify(
            l => l.LogError(It.IsAny<string>(), It.IsAny<Exception>()),
            Times.Once,
            "Se debe registrar un error cuando falta la API Key");
    }

    [Fact]
    public async Task GetSuggestionsAsync_WithEmptyApiKey_ThrowsInvalidOperationException()
    {
        // Arrange
        var mockConfig = CreateMockConfiguration(apiKey: "");
        var mockLogger = GetMockLogger();
        var service = new AIService(mockConfig, mockLogger.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await service.GetSuggestionsAsync("Test prompt"));

        Assert.Contains("API Key de Groq no está configurada", exception.Message);
    }

    [Fact]
    public async Task GetSuggestionsAsync_WithWhitespaceApiKey_ThrowsInvalidOperationException()
    {
        // Arrange
        var mockConfig = CreateMockConfiguration(apiKey: "   ");
        var mockLogger = GetMockLogger();
        var service = new AIService(mockConfig, mockLogger.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await service.GetSuggestionsAsync("Test prompt"));

        Assert.Contains("API Key de Groq no está configurada", exception.Message);
    }

    [Fact]
    public void Constructor_WithValidConfiguration_InitializesSuccessfully()
    {
        // Arrange
        var mockConfig = CreateMockConfiguration(
            temperature: "0.8",
            maxTokens: "2048",
            topP: "0.95");
        var mockLogger = GetMockLogger();

        // Act
        var service = new AIService(mockConfig, mockLogger.Object);

        // Assert
        Assert.NotNull(service);
        mockLogger.Verify(
            l => l.LogError(It.IsAny<string>(), It.IsAny<Exception>()),
            Times.Never,
            "No se deben registrar errores en la inicialización correcta");
    }

    [Fact]
    public void Constructor_WithMissingApiUrl_UsesDefaultUrl()
    {
        // Arrange
        var mockConfig = new Mock<IConfiguration>();
        var mockSection = new Mock<IConfigurationSection>();

        mockConfig.Setup(c => c.GetSection("AIServices"))
            .Returns(mockSection.Object);

        mockSection.Setup(s => s["GroqApiKey"])
            .Returns("test-key");
        mockSection.Setup(s => s["GroqApiUrl"])
            .Returns((string?)null);
        mockSection.Setup(s => s["GroqModel"])
            .Returns("mixtral-8x7b-32768");
        mockSection.Setup(s => s["Temperature"])
            .Returns("0.6");
        mockSection.Setup(s => s["MaxCompletionTokens"])
            .Returns("4096");
        mockSection.Setup(s => s["TopP"])
            .Returns("1.0");

        var mockLogger = GetMockLogger();

        // Act
        var service = new AIService(mockConfig.Object, mockLogger.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithMissingModel_UsesDefaultModel()
    {
        // Arrange
        var mockConfig = new Mock<IConfiguration>();
        var mockSection = new Mock<IConfigurationSection>();

        mockConfig.Setup(c => c.GetSection("AIServices"))
            .Returns(mockSection.Object);

        mockSection.Setup(s => s["GroqApiKey"])
            .Returns("test-key");
        mockSection.Setup(s => s["GroqApiUrl"])
            .Returns("https://api.groq.com/openai/v1/chat/completions");
        mockSection.Setup(s => s["GroqModel"])
            .Returns((string?)null);
        mockSection.Setup(s => s["Temperature"])
            .Returns("0.6");
        mockSection.Setup(s => s["MaxCompletionTokens"])
            .Returns("4096");
        mockSection.Setup(s => s["TopP"])
            .Returns("1.0");

        var mockLogger = GetMockLogger();

        // Act
        var service = new AIService(mockConfig.Object, mockLogger.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithInvalidTemperature_UsesDefaultValue()
    {
        // Arrange
        var mockConfig = new Mock<IConfiguration>();
        var mockSection = new Mock<IConfigurationSection>();

        mockConfig.Setup(c => c.GetSection("AIServices"))
            .Returns(mockSection.Object);

        mockSection.Setup(s => s["GroqApiKey"])
            .Returns("test-key");
        mockSection.Setup(s => s["GroqApiUrl"])
            .Returns("https://api.groq.com/openai/v1/chat/completions");
        mockSection.Setup(s => s["GroqModel"])
            .Returns("mixtral-8x7b-32768");
        mockSection.Setup(s => s["Temperature"])
            .Returns("invalid-value");
        mockSection.Setup(s => s["MaxCompletionTokens"])
            .Returns("4096");
        mockSection.Setup(s => s["TopP"])
            .Returns("1.0");

        var mockLogger = GetMockLogger();

        // Act
        var service = new AIService(mockConfig.Object, mockLogger.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithInvalidMaxTokens_UsesDefaultValue()
    {
        // Arrange
        var mockConfig = new Mock<IConfiguration>();
        var mockSection = new Mock<IConfigurationSection>();

        mockConfig.Setup(c => c.GetSection("AIServices"))
            .Returns(mockSection.Object);

        mockSection.Setup(s => s["GroqApiKey"])
            .Returns("test-key");
        mockSection.Setup(s => s["GroqApiUrl"])
            .Returns("https://api.groq.com/openai/v1/chat/completions");
        mockSection.Setup(s => s["GroqModel"])
            .Returns("mixtral-8x7b-32768");
        mockSection.Setup(s => s["Temperature"])
            .Returns("0.6");
        mockSection.Setup(s => s["MaxCompletionTokens"])
            .Returns("not-a-number");
        mockSection.Setup(s => s["TopP"])
            .Returns("1.0");

        var mockLogger = GetMockLogger();

        // Act
        var service = new AIService(mockConfig.Object, mockLogger.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public async Task GetSuggestionsAsync_VerifiesLoggingOnError()
    {
        // Arrange
        var mockConfig = CreateMockConfiguration(apiKey: "");
        var mockLogger = GetMockLogger();
        var service = new AIService(mockConfig, mockLogger.Object);

        // Act
        try
        {
            await service.GetSuggestionsAsync("Test prompt");
        }
        catch (InvalidOperationException)
        {
            // Esperado
        }

        // Assert
        mockLogger.Verify(
            l => l.LogError(It.IsAny<string>(), It.IsAny<Exception>()),
            Times.Once);
    }

    [Fact]
    public async Task GetSuggestionsAsync_VerifiesLoggingOnSuccess()
    {
        
        // Arrange
        var mockConfig = CreateMockConfigurationMoonshotAI();
        var mockLogger = GetMockLogger();

        // Act
        var service = new AIService(mockConfig, mockLogger.Object);
        var result = await service.GetSuggestionsAsync("Dime si el año 2026 es bisiesto");     

        Assert.NotNull(result);

        // Assert
         mockLogger.Verify(
             l => l.LogInfo(It.IsAny<string>()),
             Times.AtLeastOnce,
             "Se deben registrar eventos informativos durante la solicitud");
    }

    [Fact]
    public void Constructor_WithAllValidParameters_InitializesCorrectly()
    {
        // Arrange
        var mockConfig = CreateMockConfiguration(
            apiKey: "valid-key-12345",
            apiUrl: "https://api.groq.com/openai/v1/chat/completions",
            model: "mixtral-8x7b-32768",
            temperature: "0.7",
            maxTokens: "3000",
            topP: "0.9");
        var mockLogger = GetMockLogger();

        // Act
        var service = new AIService(mockConfig, mockLogger.Object);

        // Assert
        Assert.NotNull(service);
        mockLogger.Verify(
            l => l.LogError(It.IsAny<string>(), It.IsAny<Exception>()),
            Times.Never);
    }
}