using TaskManager.Models;

namespace TaskManager.Services;

public interface IProyectoService
{
    Task<Proyecto> CreateProyectoAsync(string nombreProyecto, string? descripcion, string? fechaInicio, int? activo, int? tieneDaily);
    Task<IEnumerable<Proyecto>> GetAllProyectosAsync();
    Task<Proyecto?> GetProyectoByIdAsync(int id);
    Task<Proyecto> UpdateProyectoAsync(int id, Dictionary<string, object> updates);
    Task<bool> DeleteProyectoAsync(int id);
}

public interface IRecursoService
{
    Task<Recurso> CreateRecursoAsync(string nombreRecurso, int? activo, string? fechaCreacion);
    Task<IEnumerable<Recurso>> GetAllRecursosAsync();
    Task<IEnumerable<Recurso>> GetActivosAsync();
    Task<Recurso?> GetRecursoByIdAsync(int id);
    Task<Recurso> UpdateRecursoAsync(int id, Dictionary<string, object> updates);
    Task<bool> DeleteRecursoAsync(int id);
}

public interface IRecursoProyectoService
{
    Task<RecursoProyecto> CreateRecursoProyectoAsync(int idProyecto, int idRecurso, string? fechaAsignacion);
    Task<IEnumerable<RecursoProyecto>> GetAllRecursosProyectoAsync();
    Task<IEnumerable<RecursoProyecto>> GetByProyectoAsync(int idProyecto);
    Task<IEnumerable<RecursoProyecto>> GetByRecursoAsync(int idRecurso);
    Task<RecursoProyecto?> GetRecursoProyectoByIdAsync(int id);
    Task<RecursoProyecto> UpdateRecursoProyectoAsync(int id, Dictionary<string, object> updates);
    Task<bool> DeleteRecursoProyectoAsync(int id);
}

public interface ITareaService
{
    Task<Tarea> CreateTareaAsync(int idProyecto, string titulo, string? detalle, string? fechaCreacion, string? fechaFIN, string? prioridad, int? activo);
    Task<IEnumerable<Tarea>> GetAllTareasAsync();
    Task<IEnumerable<Tarea>> GetTareasByProyectoAsync(int idProyecto);
    Task<IEnumerable<Tarea>> GetTareasByEstadoAsync(int activo);
    Task<IEnumerable<Tarea>> GetTareasByPrioridadAsync(string prioridad);
    Task<IEnumerable<Tarea>> GetTareasByTituloAsync(string titulo);
    Task<Tarea?> GetTareaByIdAsync(int id);
    Task<Tarea> UpdateTareaAsync(int id, Dictionary<string, object> updates);
    Task<bool> DeleteTareaAsync(int id);
    Task<IEnumerable<Tarea>> FilterTareasAsync(int? idTarea, int? activo, int? idProyecto, string? titulo, string? prioridad);
}

public interface IRecursoTareaService
{
    Task<RecursoTarea> CreateRecursoTareaAsync(int idTarea, int idRecurso, string? fechaAsignacion);
    Task<IEnumerable<RecursoTarea>> GetAllRecursosTareaAsync();
    Task<IEnumerable<RecursoTarea>> GetByTareaAsync(int idTarea);
    Task<IEnumerable<RecursoTarea>> GetByRecursoAsync(int idRecurso);
    Task<RecursoTarea?> GetRecursoTareaByIdAsync(int id);
    Task<RecursoTarea> UpdateRecursoTareaAsync(int id, Dictionary<string, object> updates);
    Task<bool> DeleteRecursoTareaAsync(int id);
}

public interface ITareaDailyService
{
    Task<TareaDaily> CreateTareaDailyAsync(int idProyecto, int idRecurso, string titulo, string? fechaCreacion, string? fechaFIN, int? activo);
    Task<IEnumerable<TareaDaily>> GetAllTareasDailyAsync();
    Task<IEnumerable<TareaDaily>> GetByProyectoAsync(int idProyecto);
    Task<IEnumerable<TareaDaily>> GetByProyectoAndRecursoAsync(int idProyecto, int idRecurso);
    Task<IEnumerable<TareaDaily>> GetByEstadoAsync(int activo);
    Task<IEnumerable<TareaDaily>> GetByTituloAsync(string titulo);
    Task<TareaDaily?> GetTareaDailyByIdAsync(int id);
    Task<TareaDaily> UpdateTareaDailyAsync(int id, Dictionary<string, object> updates);
    Task<bool> DeleteTareaDailyAsync(int id);
    Task<IEnumerable<TareaDaily>> FilterTareasDaily(int? idTareaDaily, int? activo, int? idProyecto, string? titulo, int? idRecurso);
}

public interface IImpedimentoDailyService
{
    Task<ImpedimentoDaily> CreateImpedimentoDailyAsync(int idProyecto, int idRecurso, string impedimento, string explicacion, string? fechaCreacion, string? fechaFIN, int? activo);
    Task<IEnumerable<ImpedimentoDaily>> GetAllImpedimentosDailyAsync();
    Task<IEnumerable<ImpedimentoDaily>> GetByProyectoAsync(int idProyecto);
    Task<IEnumerable<ImpedimentoDaily>> GetByProyectoAndRecursoAsync(int idProyecto, int idRecurso);
    Task<IEnumerable<ImpedimentoDaily>> GetActivosAsync();
    Task<IEnumerable<ImpedimentoDaily>> GetByTextoAsync(string texto);
    Task<ImpedimentoDaily?> GetImpedimentoDailyByIdAsync(int id);
    Task<ImpedimentoDaily> UpdateImpedimentoDailyAsync(int id, Dictionary<string, object> updates);
    Task<bool> DeleteImpedimentoDailyAsync(int id);
    Task<IEnumerable<ImpedimentoDaily>> FilterImpedimentosDaily(int? idImpedimentoDaily, int? activo, int? idProyecto, string? impedimento, int? idRecurso);
}
public interface IAIService
{
    /// <summary>
    /// Obtiene sugerencias de IA usando el modelo especificado
    /// </summary>
    /// <param name="prompt">El prompt a enviar a la IA</param>
    /// <param name="modelName">Nombre del modelo a usar (ej: "moonshotai/kimi-k2-instruct-0905"). Si es null, usa el modelo por defecto</param>
    /// <returns>Respuesta de la IA</returns>
    Task<string> GetSuggestionsAsync(string prompt, string? modelName = null);
}

public interface IDataCollectionService
{
    Task<Dictionary<string, ProjectDataCollection>> CollectActiveTasksAsync(int? idProyecto = null);
}

public interface IMarkdownService
{
    Task<string> SaveSuggestionsAsync(string suggestions);
    string GetSuggestionsFilePath();
}