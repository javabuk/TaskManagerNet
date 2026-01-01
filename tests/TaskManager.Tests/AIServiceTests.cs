using Xunit;
using Moq;
using TaskManager.Models;
using TaskManager.Services;
using Microsoft.Extensions.Configuration;

namespace TaskManager.Tests;

public class AIServiceTests
{
    private Mock<ILoggerService> GetMockLogger()
    {
        return new Mock<ILoggerService>();
    }

    [Fact]
    public async Task GetSuggestionsAsync_WithValidKey_CallsAPI()
    {
        // Arrange
        var mockConfig = new Mock<IConfiguration>();
        var mockSection = new Mock<IConfigurationSection>();
        var mockLogger = GetMockLogger();

        mockConfig.Setup(c => c.GetSection("AIServices"))
            .Returns(mockSection.Object);

        mockSection.Setup(s => s["GroqApiKey"])
            .Returns("test-api-key");
        mockSection.Setup(s => s["GroqApiUrl"])
            .Returns("https://api.groq.com/openai/v1/chat/completions");
        mockSection.Setup(s => s["GroqModel"])
            .Returns("mixtral-8x7b-32768");
        mockSection.Setup(s => s["Temperature"])
            .Returns("0.6");
        mockSection.Setup(s => s["MaxCompletionTokens"])
            .Returns("4096");
        mockSection.Setup(s => s["TopP"])
            .Returns("1.0");

        var service = new AIService(mockConfig.Object, mockLogger.Object);

        // Note: Esta prueba validaría la estructura, pero requiere conexión real
        // En un test real usarías HttpClientFactory con mock
        // Este test verifica que el constructor no falla
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithMissingApiKey_ReturnsService()
    {
        // Arrange
        var mockConfig = new Mock<IConfiguration>();
        var mockSection = new Mock<IConfigurationSection>();
        var mockLogger = GetMockLogger();

        mockConfig.Setup(c => c.GetSection("AIServices"))
            .Returns(mockSection.Object);

        mockSection.Setup(s => s["GroqApiKey"])
            .Returns((string?)null);
        mockSection.Setup(s => s["GroqApiUrl"])
            .Returns("https://api.groq.com/openai/v1/chat/completions");
        mockSection.Setup(s => s["GroqModel"])
            .Returns("mixtral-8x7b-32768");
        mockSection.Setup(s => s["Temperature"])
            .Returns("0.6");
        mockSection.Setup(s => s["MaxCompletionTokens"])
            .Returns("4096");
        mockSection.Setup(s => s["TopP"])
            .Returns("1.0");

        // Act & Assert
        var service = new AIService(mockConfig.Object, mockLogger.Object);
        Assert.NotNull(service);
    }
}

public class DataCollectionServiceTests
{
    [Fact]
    public async Task CollectActiveTasksAsync_WithActiveTasks_ReturnsGroupedByProject()
    {
        // Arrange
        var mockTareaService = new Mock<ITareaService>();
        var mockTareaDailyService = new Mock<ITareaDailyService>();
        var mockImpedimentoService = new Mock<IImpedimentoDailyService>();
        var mockProyectoService = new Mock<IProyectoService>();
        var mockLogger = new Mock<ILoggerService>();

        var proyecto = new Proyecto 
        { 
            IdProyecto = 1, 
            NombreProyecto = "Test Project", 
            FechaInicio = DateTime.Now.ToString("yyyy-MM-dd"),
            Activo = 1
        };

        var tarea = new Tarea
        {
            IdTarea = 1,
            IdProyecto = 1,
            Titulo = "Test Task",
            Detalle = "Test Detail",
            FechaCreacion = DateTime.Now.ToString("yyyy-MM-dd"),
            FechaFIN = null,
            Prioridad = "Alta",
            Activo = 1
        };

        var tareaDaily = new TareaDaily
        {
            IdTareaDaily = 1,
            IdProyecto = 1,
            IdRecurso = 1,
            Titulo = "Daily Task",
            FechaCreacion = DateTime.Now.ToString("yyyy-MM-dd"),
            FechaFIN = null,
            Activo = 1
        };

        var impedimento = new ImpedimentoDaily
        {
            IdImpedimentoDaily = 1,
            IdProyecto = 1,
            IdRecurso = 1,
            Impedimento = "Bug in module",
            Explicacion = "Module X is broken",
            FechaCreacion = DateTime.Now.ToString("yyyy-MM-dd"),
            FechaFIN = null,
            Activo = 1
        };

        mockProyectoService.Setup(s => s.GetAllProyectosAsync())
            .ReturnsAsync(new List<Proyecto> { proyecto });

        mockTareaService.Setup(s => s.GetTareasByProyectoAsync(1))
            .ReturnsAsync(new List<Tarea> { tarea });

        mockTareaDailyService.Setup(s => s.GetByProyectoAsync(1))
            .ReturnsAsync(new List<TareaDaily> { tareaDaily });

        mockImpedimentoService.Setup(s => s.GetByProyectoAsync(1))
            .ReturnsAsync(new List<ImpedimentoDaily> { impedimento });

        var service = new DataCollectionService(
            mockTareaService.Object,
            mockTareaDailyService.Object,
            mockImpedimentoService.Object,
            mockProyectoService.Object,
            mockLogger.Object
        );

        // Act
        var result = await service.CollectActiveTasksAsync();

        // Assert
        Assert.Single(result);
        Assert.True(result.ContainsKey("Test Project"));
        
        var projectData = result["Test Project"];
        Assert.NotNull(projectData.Proyecto);
        Assert.Single(projectData.Tareas);
        Assert.Single(projectData.TareasDaily);
        Assert.Single(projectData.Impedimentos);
    }

    [Fact]
    public async Task CollectActiveTasksAsync_WithNoActiveTasks_ReturnsEmpty()
    {
        // Arrange
        var mockTareaService = new Mock<ITareaService>();
        var mockTareaDailyService = new Mock<ITareaDailyService>();
        var mockImpedimentoService = new Mock<IImpedimentoDailyService>();
        var mockProyectoService = new Mock<IProyectoService>();
        var mockLogger = new Mock<ILoggerService>();

        mockProyectoService.Setup(s => s.GetAllProyectosAsync())
            .ReturnsAsync(new List<Proyecto>());

        var service = new DataCollectionService(
            mockTareaService.Object,
            mockTareaDailyService.Object,
            mockImpedimentoService.Object,
            mockProyectoService.Object,
            mockLogger.Object
        );

        // Act
        var result = await service.CollectActiveTasksAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task CollectActiveTasksAsync_ExcludesCompletedTasks()
    {
        // Arrange
        var mockTareaService = new Mock<ITareaService>();
        var mockTareaDailyService = new Mock<ITareaDailyService>();
        var mockImpedimentoService = new Mock<IImpedimentoDailyService>();
        var mockProyectoService = new Mock<IProyectoService>();
        var mockLogger = new Mock<ILoggerService>();

        var proyecto = new Proyecto 
        { 
            IdProyecto = 1, 
            NombreProyecto = "Test Project", 
            FechaInicio = DateTime.Now.ToString("yyyy-MM-dd"),
            Activo = 1
        };

        // Tarea completada (fecha fin en el pasado)
        var tareaCompletada = new Tarea
        {
            IdTarea = 1,
            IdProyecto = 1,
            Titulo = "Completed Task",
            FechaCreacion = DateTime.Now.AddDays(-10).ToString("yyyy-MM-dd"),
            FechaFIN = DateTime.Now.AddDays(-5).ToString("yyyy-MM-dd"),
            Prioridad = "Alta",
            Activo = 1
        };

        mockProyectoService.Setup(s => s.GetAllProyectosAsync())
            .ReturnsAsync(new List<Proyecto> { proyecto });

        mockTareaService.Setup(s => s.GetTareasByProyectoAsync(1))
            .ReturnsAsync(new List<Tarea> { tareaCompletada });

        mockTareaDailyService.Setup(s => s.GetByProyectoAsync(1))
            .ReturnsAsync(new List<TareaDaily>());

        mockImpedimentoService.Setup(s => s.GetByProyectoAsync(1))
            .ReturnsAsync(new List<ImpedimentoDaily>());

        var service = new DataCollectionService(
            mockTareaService.Object,
            mockTareaDailyService.Object,
            mockImpedimentoService.Object,
            mockProyectoService.Object,
            mockLogger.Object
        );

        // Act
        var result = await service.CollectActiveTasksAsync();

        // Assert
        // No debería incluir el proyecto si todas las tareas están completadas
        Assert.Empty(result);
    }
}

public class MarkdownServiceTests
{
    [Fact]
    public async Task SaveSuggestionsAsync_CreatesFileWithTimestamp()
    {
        // Arrange
        var mockLogger = new Mock<ILoggerService>();
        var service = new MarkdownService(mockLogger.Object);
        
        var testContent = "# Test Suggestions\n\nThis is a test.";
        var tempDir = Path.Combine(Path.GetTempPath(), "TaskManagerTests");
        
        if (!Directory.Exists(tempDir))
            Directory.CreateDirectory(tempDir);

        var currentDir = Directory.GetCurrentDirectory();
        try
        {
            Directory.SetCurrentDirectory(tempDir);

            // Act
            var filePath = await service.SaveSuggestionsAsync(testContent);

            // Assert
            Assert.NotNull(filePath);
            Assert.True(File.Exists(filePath));
            Assert.Contains("_Sugerencias.md", filePath);
            
            var content = File.ReadAllText(filePath);
            Assert.Equal(testContent, content);

            // Cleanup
            File.Delete(filePath);
        }
        finally
        {
            Directory.SetCurrentDirectory(currentDir);
        }
    }

    [Fact]
    public void GetSuggestionsFilePath_ReturnsProperFormat()
    {
        // Arrange
        var mockLogger = new Mock<ILoggerService>();
        var service = new MarkdownService(mockLogger.Object);

        // Act
        var filePath = service.GetSuggestionsFilePath();

        // Assert
        Assert.Contains("_Sugerencias.md", filePath);
        Assert.Matches(@"^\d{14}_Sugerencias\.md$", filePath);
    }
}
