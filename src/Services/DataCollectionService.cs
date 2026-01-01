using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Models;

namespace TaskManager.Services;

public class ProjectDataCollection
{
    public Proyecto? Proyecto { get; set; }
    public List<Tarea> Tareas { get; set; } = new();
    public List<TareaDaily> TareasDaily { get; set; } = new();
    public List<ImpedimentoDaily> Impedimentos { get; set; } = new();
}

public class DataCollectionService : IDataCollectionService
{
    private readonly ITareaService _tareaService;
    private readonly ITareaDailyService _tareaDailyService;
    private readonly IImpedimentoDailyService _impedimentoService;
    private readonly IProyectoService _proyectoService;
    private readonly ILoggerService _loggerService;

    public DataCollectionService(
        ITareaService tareaService,
        ITareaDailyService tareaDailyService,
        IImpedimentoDailyService impedimentoService,
        IProyectoService proyectoService,
        ILoggerService loggerService)
    {
        _tareaService = tareaService;
        _tareaDailyService = tareaDailyService;
        _impedimentoService = impedimentoService;
        _proyectoService = proyectoService;
        _loggerService = loggerService;
    }

    public async Task<Dictionary<string, ProjectDataCollection>> CollectActiveTasksAsync()
    {
        var today = DateTime.Now.ToString("yyyy-MM-dd");
        var result = new Dictionary<string, ProjectDataCollection>();

        try
        {
            _loggerService.LogInfo("[DataCollectionService] Iniciando recopilación de datos activos");

            // Obtener todos los proyectos
            var allProjects = await _proyectoService.GetAllProyectosAsync();
            _loggerService.LogInfo($"[DataCollectionService] Total de proyectos encontrados: {allProjects.Count()}");

            foreach (var proyecto in allProjects)
            {
                if (proyecto == null) continue;

                _loggerService.LogInfo($"[DataCollectionService] Procesando proyecto: {proyecto.NombreProyecto} (ID: {proyecto.IdProyecto})");

                var collection = new ProjectDataCollection { Proyecto = proyecto };

                // Obtener tareas activas del proyecto
                var tareas = await _tareaService.GetTareasByProyectoAsync(proyecto.IdProyecto);
                var activeTasksWithoutEnd = tareas.Where(t => 
                    t.Activo == 1 && (string.IsNullOrEmpty(t.FechaFIN) || string.Compare(t.FechaFIN, today) >= 0)
                ).ToList();

                collection.Tareas = activeTasksWithoutEnd;
                _loggerService.LogInfo($"[DataCollectionService] Tareas activas en {proyecto.NombreProyecto}: {activeTasksWithoutEnd.Count()}");

                // Obtener tareas diarias del proyecto
                var tareasDaily = await _tareaDailyService.GetByProyectoAsync(proyecto.IdProyecto);
                var activeDailyTasks = tareasDaily.Where(t => 
                    t.Activo == 1 && (string.IsNullOrEmpty(t.FechaFIN) || string.Compare(t.FechaFIN, today) >= 0)
                ).ToList();

                collection.TareasDaily = activeDailyTasks;
                _loggerService.LogInfo($"[DataCollectionService] Tareas diarias activas en {proyecto.NombreProyecto}: {activeDailyTasks.Count()}");

                // Obtener impedimentos diarios del proyecto
                var impedimentos = await _impedimentoService.GetByProyectoAsync(proyecto.IdProyecto);
                var activeImpedimentos = impedimentos.Where(i => 
                    i.Activo == 1 && (string.IsNullOrEmpty(i.FechaFIN) || string.Compare(i.FechaFIN, today) >= 0)
                ).ToList();

                collection.Impedimentos = activeImpedimentos;
                _loggerService.LogInfo($"[DataCollectionService] Impedimentos activos en {proyecto.NombreProyecto}: {activeImpedimentos.Count()}");

                // Solo agregar al resultado si hay algo que reportar
                if (collection.Tareas.Count > 0 || collection.TareasDaily.Count > 0 || collection.Impedimentos.Count > 0)
                {
                    result[proyecto.NombreProyecto] = collection;
                }
            }

            _loggerService.LogInfo($"[DataCollectionService] Recopilación completada. Total de proyectos con datos activos: {result.Count()}");
            return result;
        }
        catch (Exception ex)
        {
            _loggerService.LogError($"[DataCollectionService] Error durante la recopilación de datos: {ex.Message}", ex);
            throw;
        }
    }
}
