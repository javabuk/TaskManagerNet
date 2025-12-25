using Xunit;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Repositories;
using TaskManager.Services;

namespace TaskManager.Tests;

public class IntegrationTests : IDisposable
{
    private readonly TaskManagerDbContext _context;
    private readonly IProyectoRepository _proyectoRepository;
    private readonly IRecursoRepository _recursoRepository;
    private readonly ITareaRepository _tareaRepository;
    private readonly ITareaDailyRepository _tareaDailyRepository;
    private readonly IImpedimentoDailyRepository _impedimentoRepository;
    private readonly IRecursoProyectoRepository _recursoProyectoRepository;
    private readonly IRecursoTareaRepository _recursoTareaRepository;

    public IntegrationTests()
    {
        var options = new DbContextOptionsBuilder<TaskManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new TaskManagerDbContext(options);
        _context.Database.EnsureCreated();

        _proyectoRepository = new ProyectoRepository(_context);
        _recursoRepository = new RecursoRepository(_context);
        _tareaRepository = new TareaRepository(_context);
        _tareaDailyRepository = new TareaDailyRepository(_context);
        _impedimentoRepository = new ImpedimentoDailyRepository(_context);
        _recursoProyectoRepository = new RecursoProyectoRepository(_context);
        _recursoTareaRepository = new RecursoTareaRepository(_context);
    }

    [Fact]
    public async Task CreateAndRetrieveProyecto_Success()
    {
        // Arrange
        var proyecto = new Proyecto
        {
            NombreProyecto = "Test Project",
            Descripcion = "Test Description",
            FechaInicio = "25/12/2025",
            Activo = 1,
            TieneDaily = 1
        };

        // Act
        var created = await _proyectoRepository.CreateAsync(proyecto);
        var retrieved = await _proyectoRepository.GetByIdAsync(created.IdProyecto);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal("Test Project", retrieved.NombreProyecto);
    }

    [Fact]
    public async Task CreateAndUpdateProyecto_Success()
    {
        // Arrange
        var proyecto = new Proyecto
        {
            NombreProyecto = "Original Name",
            FechaInicio = "25/12/2025",
            Activo = 1,
            TieneDaily = 0
        };

        var created = await _proyectoRepository.CreateAsync(proyecto);

        // Act
        created.NombreProyecto = "Updated Name";
        await _proyectoRepository.UpdateAsync(created);
        var updated = await _proyectoRepository.GetByIdAsync(created.IdProyecto);

        // Assert
        Assert.Equal("Updated Name", updated.NombreProyecto);
    }

    [Fact]
    public async Task CreateAndDeleteProyecto_Success()
    {
        // Arrange
        var proyecto = new Proyecto
        {
            NombreProyecto = "Delete Test",
            FechaInicio = "25/12/2025",
            Activo = 1,
            TieneDaily = 0
        };

        var created = await _proyectoRepository.CreateAsync(proyecto);

        // Act
        await _proyectoRepository.DeleteAsync(created.IdProyecto);
        var retrieved = await _proyectoRepository.GetByIdAsync(created.IdProyecto);

        // Assert
        Assert.Null(retrieved);
    }

    [Fact]
    public async Task CreateTarea_WithProyecto_Success()
    {
        // Arrange
        var proyecto = new Proyecto
        {
            NombreProyecto = "Project A",
            FechaInicio = "25/12/2025",
            Activo = 1,
            TieneDaily = 0
        };

        var createdProyecto = await _proyectoRepository.CreateAsync(proyecto);

        var tarea = new Tarea
        {
            IdProyecto = createdProyecto.IdProyecto,
            Titulo = "Task 1",
            Detalle = "Task Details",
            FechaCreacion = "25/12/2025",
            Prioridad = "Alta",
            Activo = 1
        };

        // Act
        var createdTarea = await _tareaRepository.CreateAsync(tarea);
        var retrieved = await _tareaRepository.GetByIdAsync(createdTarea.IdTarea);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal("Task 1", retrieved.Titulo);
        Assert.Equal(createdProyecto.IdProyecto, retrieved.IdProyecto);
    }

    [Fact]
    public async Task FilterTareasByProyecto_Success()
    {
        // Arrange
        var proyecto1 = new Proyecto { NombreProyecto = "P1", FechaInicio = "25/12/2025", Activo = 1, TieneDaily = 0 };
        var proyecto2 = new Proyecto { NombreProyecto = "P2", FechaInicio = "25/12/2025", Activo = 1, TieneDaily = 0 };

        var p1 = await _proyectoRepository.CreateAsync(proyecto1);
        var p2 = await _proyectoRepository.CreateAsync(proyecto2);

        var tarea1 = new Tarea { IdProyecto = p1.IdProyecto, Titulo = "T1", FechaCreacion = "25/12/2025", Prioridad = "Media", Activo = 1 };
        var tarea2 = new Tarea { IdProyecto = p2.IdProyecto, Titulo = "T2", FechaCreacion = "25/12/2025", Prioridad = "Media", Activo = 1 };

        await _tareaRepository.CreateAsync(tarea1);
        await _tareaRepository.CreateAsync(tarea2);

        // Act
        var result = await _tareaRepository.GetByProyectoAsync(p1.IdProyecto);

        // Assert
        Assert.Single(result);
        Assert.Equal("T1", result.First().Titulo);
    }

    [Fact]
    public async Task CreateRecursoProyecto_WithConstraint_Success()
    {
        // Arrange
        var proyecto = new Proyecto { NombreProyecto = "P1", FechaInicio = "25/12/2025", Activo = 1, TieneDaily = 0 };
        var recurso = new Recurso { NombreRecurso = "R1", FechaCreacion = "25/12/2025", Activo = 1 };

        var p = await _proyectoRepository.CreateAsync(proyecto);
        var r = await _recursoRepository.CreateAsync(recurso);

        var recursoProyecto = new RecursoProyecto
        {
            IdProyecto = p.IdProyecto,
            IdRecurso = r.IdRecurso,
            FechaAsignacion = "25/12/2025"
        };

        // Act
        var created = await _recursoProyectoRepository.CreateAsync(recursoProyecto);
        var retrieved = await _recursoProyectoRepository.GetByProyectoAndRecursoAsync(p.IdProyecto, r.IdRecurso);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(p.IdProyecto, retrieved.IdProyecto);
        Assert.Equal(r.IdRecurso, retrieved.IdRecurso);
    }

    [Fact]
    public async Task CreateTareaDailyAndImpedimento_Success()
    {
        // Arrange
        var proyecto = new Proyecto { NombreProyecto = "P1", FechaInicio = "25/12/2025", Activo = 1, TieneDaily = 1 };
        var recurso = new Recurso { NombreRecurso = "R1", FechaCreacion = "25/12/2025", Activo = 1 };

        var p = await _proyectoRepository.CreateAsync(proyecto);
        var r = await _recursoRepository.CreateAsync(recurso);

        var tareaDaily = new TareaDaily
        {
            IdProyecto = p.IdProyecto,
            IdRecurso = r.IdRecurso,
            Titulo = "Daily Task",
            FechaCreacion = "25/12/2025",
            Activo = 1
        };

        var impedimento = new ImpedimentoDaily
        {
            IdProyecto = p.IdProyecto,
            IdRecurso = r.IdRecurso,
            Impedimento = "Blocker",
            Explicacion = "Explanation",
            FechaCreacion = "25/12/2025",
            Activo = 1
        };

        // Act
        var createdTarea = await _tareaDailyRepository.CreateAsync(tareaDaily);
        var createdImpedimento = await _impedimentoRepository.CreateAsync(impedimento);

        var retrievedTarea = await _tareaDailyRepository.GetByIdAsync(createdTarea.IdTareaDaily);
        var retrievedImpedimento = await _impedimentoRepository.GetByIdAsync(createdImpedimento.IdImpedimentoDaily);

        // Assert
        Assert.NotNull(retrievedTarea);
        Assert.NotNull(retrievedImpedimento);
        Assert.Equal("Daily Task", retrievedTarea.Titulo);
        Assert.Equal("Blocker", retrievedImpedimento.Impedimento);
    }

    [Fact]
    public async Task GetActivosRecursos_FilterCorrectly()
    {
        // Arrange
        var r1 = new Recurso { NombreRecurso = "R1", FechaCreacion = "25/12/2025", Activo = 1 };
        var r2 = new Recurso { NombreRecurso = "R2", FechaCreacion = "25/12/2025", Activo = 0 };

        await _recursoRepository.CreateAsync(r1);
        await _recursoRepository.CreateAsync(r2);

        // Act
        var result = await _recursoRepository.GetActivosAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("R1", result.First().NombreRecurso);
    }

    [Fact]
    public async Task ComplexScenario_MultipleProjectsAndTasks()
    {
        // Arrange
        var proyecto = new Proyecto { NombreProyecto = "Complex Project", FechaInicio = "25/12/2025", Activo = 1, TieneDaily = 1 };
        var recurso1 = new Recurso { NombreRecurso = "Developer 1", FechaCreacion = "25/12/2025", Activo = 1 };
        var recurso2 = new Recurso { NombreRecurso = "Developer 2", FechaCreacion = "25/12/2025", Activo = 1 };

        var p = await _proyectoRepository.CreateAsync(proyecto);
        var r1 = await _recursoRepository.CreateAsync(recurso1);
        var r2 = await _recursoRepository.CreateAsync(recurso2);

        // Asignar recursos al proyecto
        await _recursoProyectoRepository.CreateAsync(new RecursoProyecto { IdProyecto = p.IdProyecto, IdRecurso = r1.IdRecurso, FechaAsignacion = "25/12/2025" });
        await _recursoProyectoRepository.CreateAsync(new RecursoProyecto { IdProyecto = p.IdProyecto, IdRecurso = r2.IdRecurso, FechaAsignacion = "25/12/2025" });

        // Crear tareas
        var t1 = new Tarea { IdProyecto = p.IdProyecto, Titulo = "Feature 1", FechaCreacion = "25/12/2025", Prioridad = "Alta", Activo = 1 };
        var t2 = new Tarea { IdProyecto = p.IdProyecto, Titulo = "Bug Fix", FechaCreacion = "25/12/2025", Prioridad = "Media", Activo = 1 };

        var tarea1 = await _tareaRepository.CreateAsync(t1);
        var tarea2 = await _tareaRepository.CreateAsync(t2);

        // Asignar recursos a tareas
        await _recursoTareaRepository.CreateAsync(new RecursoTarea { IdTarea = tarea1.IdTarea, IdRecurso = r1.IdRecurso, FechaAsignacion = "25/12/2025" });
        await _recursoTareaRepository.CreateAsync(new RecursoTarea { IdTarea = tarea2.IdTarea, IdRecurso = r2.IdRecurso, FechaAsignacion = "25/12/2025" });

        // Crear daily tasks
        await _tareaDailyRepository.CreateAsync(new TareaDaily { IdProyecto = p.IdProyecto, IdRecurso = r1.IdRecurso, Titulo = "Daily 1", FechaCreacion = "25/12/2025", Activo = 1 });
        await _tareaDailyRepository.CreateAsync(new TareaDaily { IdProyecto = p.IdProyecto, IdRecurso = r2.IdRecurso, Titulo = "Daily 2", FechaCreacion = "25/12/2025", Activo = 1 });

        // Act
        var tareas = await _tareaRepository.GetByProyectoAsync(p.IdProyecto);
        var recursosTarea = await _recursoTareaRepository.GetByRecursoAsync(r1.IdRecurso);
        var tareasDaily = await _tareaDailyRepository.GetByProyectoAsync(p.IdProyecto);
        var recursosProyecto = await _recursoProyectoRepository.GetByProyectoAsync(p.IdProyecto);

        // Assert
        Assert.Equal(2, tareas.Count());
        Assert.Single(recursosTarea);
        Assert.Equal(2, tareasDaily.Count());
        Assert.Equal(2, recursosProyecto.Count());
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

public class RepositoryTests : IDisposable
{
    private readonly TaskManagerDbContext _context;
    private readonly ITareaRepository _tareaRepository;

    public RepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TaskManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new TaskManagerDbContext(options);
        _context.Database.EnsureCreated();

        _tareaRepository = new TareaRepository(_context);
    }

    [Fact]
    public async Task GetByTituloAsync_FindsTasksByTitle()
    {
        // Arrange
        var proyecto = new Proyecto { NombreProyecto = "P1", FechaInicio = "25/12/2025", Activo = 1, TieneDaily = 0 };
        var proyectoRepo = new ProyectoRepository(_context);
        var p = await proyectoRepo.CreateAsync(proyecto);

        var tarea1 = new Tarea { IdProyecto = p.IdProyecto, Titulo = "Search Task", FechaCreacion = "25/12/2025", Prioridad = "Media", Activo = 1 };
        var tarea2 = new Tarea { IdProyecto = p.IdProyecto, Titulo = "Other Task", FechaCreacion = "25/12/2025", Prioridad = "Media", Activo = 1 };

        await _tareaRepository.CreateAsync(tarea1);
        await _tareaRepository.CreateAsync(tarea2);

        // Act
        var result = await _tareaRepository.GetByTituloAsync("Search");

        // Assert
        Assert.Single(result);
        Assert.Contains("Search", result.First().Titulo);
    }

    [Fact]
    public async Task GetByPrioridadAsync_FiltersByPriority()
    {
        // Arrange
        var proyecto = new Proyecto { NombreProyecto = "P1", FechaInicio = "25/12/2025", Activo = 1, TieneDaily = 0 };
        var proyectoRepo = new ProyectoRepository(_context);
        var p = await proyectoRepo.CreateAsync(proyecto);

        var tarea1 = new Tarea { IdProyecto = p.IdProyecto, Titulo = "T1", FechaCreacion = "25/12/2025", Prioridad = "Alta", Activo = 1 };
        var tarea2 = new Tarea { IdProyecto = p.IdProyecto, Titulo = "T2", FechaCreacion = "25/12/2025", Prioridad = "Baja", Activo = 1 };

        await _tareaRepository.CreateAsync(tarea1);
        await _tareaRepository.CreateAsync(tarea2);

        // Act
        var result = await _tareaRepository.GetByPrioridadAsync("Alta");

        // Assert
        Assert.Single(result);
        Assert.Equal("Alta", result.First().Prioridad);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
