using Xunit;
using Moq;
using TaskManager.Models;
using TaskManager.Repositories;
using TaskManager.Services;

namespace TaskManager.Tests;

public class ProyectoServiceTests
{
    [Fact]
    public async Task CreateProyectoAsync_WithValidData_ReturnsProyecto()
    {
        // Arrange
        var mockRepo = new Mock<IProyectoRepository>();
        var service = new ProyectoService(mockRepo.Object);

        var nombre = "Test Project";
        var descripcion = "Test Description";

        mockRepo.Setup(r => r.CreateAsync(It.IsAny<Proyecto>()))
            .ReturnsAsync((Proyecto p) => p);

        // Act
        var result = await service.CreateProyectoAsync(nombre, descripcion, null, null, null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(nombre, result.NombreProyecto);
        Assert.Equal(descripcion, result.Descripcion);
        Assert.Equal(1, result.Activo);
        Assert.Equal(0, result.TieneDaily);
    }

    [Fact]
    public async Task CreateProyectoAsync_WithoutNombre_ThrowsException()
    {
        // Arrange
        var mockRepo = new Mock<IProyectoRepository>();
        var service = new ProyectoService(mockRepo.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateProyectoAsync("", null, null, null, null));
    }

    [Fact]
    public async Task GetAllProyectosAsync_ReturnsAll()
    {
        // Arrange
        var mockRepo = new Mock<IProyectoRepository>();
        var service = new ProyectoService(mockRepo.Object);

        var proyectos = new List<Proyecto>
        {
            new Proyecto { IdProyecto = 1, NombreProyecto = "P1", FechaInicio = DateTime.Now.ToString("dd/MM/yyyy") },
            new Proyecto { IdProyecto = 2, NombreProyecto = "P2", FechaInicio = DateTime.Now.ToString("dd/MM/yyyy") }
        };

        mockRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(proyectos);

        // Act
        var result = await service.GetAllProyectosAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task UpdateProyectoAsync_WithValidData_UpdatesProyecto()
    {
        // Arrange
        var mockRepo = new Mock<IProyectoRepository>();
        var service = new ProyectoService(mockRepo.Object);

        var proyecto = new Proyecto 
        { 
            IdProyecto = 1, 
            NombreProyecto = "Old Name", 
            FechaInicio = DateTime.Now.ToString("dd/MM/yyyy") 
        };

        var updates = new Dictionary<string, object> { { "NombreProyecto", "New Name" } };

        mockRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(proyecto);

        mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Proyecto>()))
            .ReturnsAsync((Proyecto p) => p);

        // Act
        var result = await service.UpdateProyectoAsync(1, updates);

        // Assert
        Assert.Equal("New Name", result.NombreProyecto);
    }

    [Fact]
    public async Task GetProyectoByIdAsync_RetornsProyecto()
    {
        // Arrange
        var mockRepo = new Mock<IProyectoRepository>();
        var service = new ProyectoService(mockRepo.Object);

        var proyecto = new Proyecto 
        { 
            IdProyecto = 1, 
            NombreProyecto = "Test", 
            FechaInicio = DateTime.Now.ToString("dd/MM/yyyy") 
        };

        mockRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(proyecto);

        // Act
        var result = await service.GetProyectoByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.IdProyecto);
    }

    [Fact]
    public async Task DeleteProyectoAsync_ReturnsTrueOnSuccess()
    {
        // Arrange
        var mockRepo = new Mock<IProyectoRepository>();
        var service = new ProyectoService(mockRepo.Object);

        mockRepo.Setup(r => r.DeleteAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await service.DeleteProyectoAsync(1);

        // Assert
        Assert.True(result);
    }
}

public class RecursoServiceTests
{
    [Fact]
    public async Task CreateRecursoAsync_WithValidData_ReturnsRecurso()
    {
        // Arrange
        var mockRepo = new Mock<IRecursoRepository>();
        var service = new RecursoService(mockRepo.Object);

        var nombre = "Test Resource";

        mockRepo.Setup(r => r.CreateAsync(It.IsAny<Recurso>()))
            .ReturnsAsync((Recurso r) => r);

        // Act
        var result = await service.CreateRecursoAsync(nombre, null, null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(nombre, result.NombreRecurso);
        Assert.Equal(1, result.Activo);
    }

    [Fact]
    public async Task CreateRecursoAsync_WithoutNombre_ThrowsException()
    {
        // Arrange
        var mockRepo = new Mock<IRecursoRepository>();
        var service = new RecursoService(mockRepo.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateRecursoAsync("", null, null));
    }

    [Fact]
    public async Task GetActivosAsync_ReturnsOnlyActive()
    {
        // Arrange
        var mockRepo = new Mock<IRecursoRepository>();
        var service = new RecursoService(mockRepo.Object);

        var recursos = new List<Recurso>
        {
            new Recurso { IdRecurso = 1, NombreRecurso = "R1", Activo = 1, FechaCreacion = DateTime.Now.ToString("dd/MM/yyyy") },
            new Recurso { IdRecurso = 2, NombreRecurso = "R2", Activo = 1, FechaCreacion = DateTime.Now.ToString("dd/MM/yyyy") }
        };

        mockRepo.Setup(r => r.GetActivosAsync())
            .ReturnsAsync(recursos);

        // Act
        var result = await service.GetActivosAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }
}

public class TareaServiceTests
{
    [Fact]
    public async Task CreateTareaAsync_WithValidData_ReturnsTarea()
    {
        // Arrange
        var mockRepo = new Mock<ITareaRepository>();
        var service = new TareaService(mockRepo.Object);

        var titulo = "Test Task";

        mockRepo.Setup(r => r.CreateAsync(It.IsAny<Tarea>()))
            .ReturnsAsync((Tarea t) => t);

        // Act
        var result = await service.CreateTareaAsync(1, titulo, null, null, null, null, null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(titulo, result.Titulo);
        Assert.Equal("Media", result.Prioridad);
    }

    [Fact]
    public async Task CreateTareaAsync_WithoutTitulo_ThrowsException()
    {
        // Arrange
        var mockRepo = new Mock<ITareaRepository>();
        var service = new TareaService(mockRepo.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateTareaAsync(1, "", null, null, null, null, null));
    }

    [Fact]
    public async Task FilterTareasAsync_FiltersByProyecto()
    {
        // Arrange
        var mockRepo = new Mock<ITareaRepository>();
        var service = new TareaService(mockRepo.Object);

        var tareas = new List<Tarea>
        {
            new Tarea { IdTarea = 1, IdProyecto = 1, Titulo = "T1", FechaCreacion = DateTime.Now.ToString("dd/MM/yyyy"), Prioridad = "Alta" },
            new Tarea { IdTarea = 2, IdProyecto = 2, Titulo = "T2", FechaCreacion = DateTime.Now.ToString("dd/MM/yyyy"), Prioridad = "Media" }
        };

        mockRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(tareas);

        // Act
        var result = await service.FilterTareasAsync(null, null, 1, null, null);

        // Assert
        Assert.Single(result);
        Assert.Equal(1, result.First().IdProyecto);
    }

    [Fact]
    public async Task FilterTareasAsync_FiltersByPrioridad()
    {
        // Arrange
        var mockRepo = new Mock<ITareaRepository>();
        var service = new TareaService(mockRepo.Object);

        var tareas = new List<Tarea>
        {
            new Tarea { IdTarea = 1, IdProyecto = 1, Titulo = "T1", FechaCreacion = DateTime.Now.ToString("dd/MM/yyyy"), Prioridad = "Alta" },
            new Tarea { IdTarea = 2, IdProyecto = 1, Titulo = "T2", FechaCreacion = DateTime.Now.ToString("dd/MM/yyyy"), Prioridad = "Media" }
        };

        mockRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(tareas);

        // Act
        var result = await service.FilterTareasAsync(null, null, null, null, "Alta");

        // Assert
        Assert.Single(result);
        Assert.Equal("Alta", result.First().Prioridad);
    }

    [Fact]
    public async Task GetTareaByIdAsync_ReturnsTarea()
    {
        // Arrange
        var mockRepo = new Mock<ITareaRepository>();
        var service = new TareaService(mockRepo.Object);

        var tarea = new Tarea 
        { 
            IdTarea = 1, 
            IdProyecto = 1, 
            Titulo = "Test", 
            FechaCreacion = DateTime.Now.ToString("dd/MM/yyyy"), 
            Prioridad = "Media" 
        };

        mockRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(tarea);

        // Act
        var result = await service.GetTareaByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.IdTarea);
    }
}

public class TareaDailyServiceTests
{
    [Fact]
    public async Task CreateTareaDailyAsync_WithValidData_ReturnsTareaDaily()
    {
        // Arrange
        var mockRepo = new Mock<ITareaDailyRepository>();
        var service = new TareaDailyService(mockRepo.Object);

        var titulo = "Daily Task";

        mockRepo.Setup(r => r.CreateAsync(It.IsAny<TareaDaily>()))
            .ReturnsAsync((TareaDaily t) => t);

        // Act
        var result = await service.CreateTareaDailyAsync(1, 1, titulo, null, null, null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(titulo, result.Titulo);
    }

    [Fact]
    public async Task CreateTareaDailyAsync_WithoutTitulo_ThrowsException()
    {
        // Arrange
        var mockRepo = new Mock<ITareaDailyRepository>();
        var service = new TareaDailyService(mockRepo.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateTareaDailyAsync(1, 1, "", null, null, null));
    }

    [Fact]
    public async Task GetByProyectoAsync_ReturnsFiltered()
    {
        // Arrange
        var mockRepo = new Mock<ITareaDailyRepository>();
        var service = new TareaDailyService(mockRepo.Object);

        var tareas = new List<TareaDaily>
        {
            new TareaDaily { IdTareaDaily = 1, IdProyecto = 1, IdRecurso = 1, Titulo = "T1", FechaCreacion = DateTime.Now.ToString("dd/MM/yyyy") },
            new TareaDaily { IdTareaDaily = 2, IdProyecto = 2, IdRecurso = 1, Titulo = "T2", FechaCreacion = DateTime.Now.ToString("dd/MM/yyyy") }
        };

        mockRepo.Setup(r => r.GetByProyectoAsync(1))
            .ReturnsAsync(tareas.Where(t => t.IdProyecto == 1));

        // Act
        var result = await service.GetByProyectoAsync(1);

        // Assert
        Assert.Single(result);
    }
}

public class ImpedimentoDailyServiceTests
{
    [Fact]
    public async Task CreateImpedimentoDailyAsync_WithValidData_ReturnsImpedimento()
    {
        // Arrange
        var mockRepo = new Mock<IImpedimentoDailyRepository>();
        var service = new ImpedimentoDailyService(mockRepo.Object);

        var impedimento = "Test Blocker";
        var explicacion = "Test Explanation";

        mockRepo.Setup(r => r.CreateAsync(It.IsAny<ImpedimentoDaily>()))
            .ReturnsAsync((ImpedimentoDaily i) => i);

        // Act
        var result = await service.CreateImpedimentoDailyAsync(1, 1, impedimento, explicacion, null, null, null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(impedimento, result.Impedimento);
        Assert.Equal(explicacion, result.Explicacion);
    }

    [Fact]
    public async Task CreateImpedimentoDailyAsync_WithoutImpedimento_ThrowsException()
    {
        // Arrange
        var mockRepo = new Mock<IImpedimentoDailyRepository>();
        var service = new ImpedimentoDailyService(mockRepo.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            service.CreateImpedimentoDailyAsync(1, 1, "", "explicacion", null, null, null));
    }

    [Fact]
    public async Task CreateImpedimentoDailyAsync_WithoutExplicacion_ThrowsException()
    {
        // Arrange
        var mockRepo = new Mock<IImpedimentoDailyRepository>();
        var service = new ImpedimentoDailyService(mockRepo.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            service.CreateImpedimentoDailyAsync(1, 1, "impedimento", "", null, null, null));
    }

    [Fact]
    public async Task GetActivosAsync_ReturnsOnlyActive()
    {
        // Arrange
        var mockRepo = new Mock<IImpedimentoDailyRepository>();
        var service = new ImpedimentoDailyService(mockRepo.Object);

        var impedimentos = new List<ImpedimentoDaily>
        {
            new ImpedimentoDaily { IdImpedimentoDaily = 1, IdProyecto = 1, IdRecurso = 1, Impedimento = "I1", Explicacion = "E1", FechaCreacion = DateTime.Now.ToString("dd/MM/yyyy"), Activo = 1 },
            new ImpedimentoDaily { IdImpedimentoDaily = 2, IdProyecto = 1, IdRecurso = 1, Impedimento = "I2", Explicacion = "E2", FechaCreacion = DateTime.Now.ToString("dd/MM/yyyy"), Activo = 1 }
        };

        mockRepo.Setup(r => r.GetActivosAsync())
            .ReturnsAsync(impedimentos);

        // Act
        var result = await service.GetActivosAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task FilterImpedimentosDaily_FiltersByTexto()
    {
        // Arrange
        var mockRepo = new Mock<IImpedimentoDailyRepository>();
        var service = new ImpedimentoDailyService(mockRepo.Object);

        var impedimentos = new List<ImpedimentoDaily>
        {
            new ImpedimentoDaily { IdImpedimentoDaily = 1, IdProyecto = 1, IdRecurso = 1, Impedimento = "Sin servidor", Explicacion = "E1", FechaCreacion = DateTime.Now.ToString("dd/MM/yyyy") },
            new ImpedimentoDaily { IdImpedimentoDaily = 2, IdProyecto = 1, IdRecurso = 1, Impedimento = "Sin acceso BD", Explicacion = "E2", FechaCreacion = DateTime.Now.ToString("dd/MM/yyyy") }
        };

        mockRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(impedimentos);

        // Act
        var result = await service.FilterImpedimentosDaily(null, null, null, "servidor", null);

        // Assert
        Assert.Single(result);
    }
}
