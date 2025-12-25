using TaskManager.Models;
using TaskManager.Repositories;

namespace TaskManager.Services;

public class ProyectoService : IProyectoService
{
    private readonly IProyectoRepository _repository;

    public ProyectoService(IProyectoRepository repository)
    {
        _repository = repository;
    }

    public async Task<Proyecto> CreateProyectoAsync(string nombreProyecto, string? descripcion, string? fechaInicio, int? activo, int? tieneDaily)
    {
        if (string.IsNullOrWhiteSpace(nombreProyecto))
            throw new ArgumentException("El nombre del proyecto es obligatorio.");

        var proyecto = new Proyecto
        {
            NombreProyecto = nombreProyecto,
            Descripcion = descripcion,
            FechaInicio = fechaInicio ?? DateTime.Now.ToString("dd/MM/yyyy"),
            Activo = activo ?? 1,
            TieneDaily = tieneDaily ?? 0
        };

        return await _repository.CreateAsync(proyecto);
    }

    public async Task<IEnumerable<Proyecto>> GetAllProyectosAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Proyecto?> GetProyectoByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Proyecto> UpdateProyectoAsync(int id, Dictionary<string, object> updates)
    {
        var proyecto = await _repository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"Proyecto con ID {id} no encontrado.");

        foreach (var update in updates)
        {
            var propertyInfo = proyecto.GetType().GetProperty(update.Key);
            if (propertyInfo != null && propertyInfo.CanWrite)
                propertyInfo.SetValue(proyecto, update.Value);
        }

        return await _repository.UpdateAsync(proyecto);
    }

    public async Task<bool> DeleteProyectoAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }
}

public class RecursoService : IRecursoService
{
    private readonly IRecursoRepository _repository;

    public RecursoService(IRecursoRepository repository)
    {
        _repository = repository;
    }

    public async Task<Recurso> CreateRecursoAsync(string nombreRecurso, int? activo, string? fechaCreacion)
    {
        if (string.IsNullOrWhiteSpace(nombreRecurso))
            throw new ArgumentException("El nombre del recurso es obligatorio.");

        var recurso = new Recurso
        {
            NombreRecurso = nombreRecurso,
            Activo = activo ?? 1,
            FechaCreacion = fechaCreacion ?? DateTime.Now.ToString("dd/MM/yyyy")
        };

        return await _repository.CreateAsync(recurso);
    }

    public async Task<IEnumerable<Recurso>> GetAllRecursosAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<IEnumerable<Recurso>> GetActivosAsync()
    {
        return await _repository.GetActivosAsync();
    }

    public async Task<Recurso?> GetRecursoByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Recurso> UpdateRecursoAsync(int id, Dictionary<string, object> updates)
    {
        var recurso = await _repository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"Recurso con ID {id} no encontrado.");

        foreach (var update in updates)
        {
            var propertyInfo = recurso.GetType().GetProperty(update.Key);
            if (propertyInfo != null && propertyInfo.CanWrite)
                propertyInfo.SetValue(recurso, update.Value);
        }

        return await _repository.UpdateAsync(recurso);
    }

    public async Task<bool> DeleteRecursoAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }
}

public class RecursoProyectoService : IRecursoProyectoService
{
    private readonly IRecursoProyectoRepository _repository;

    public RecursoProyectoService(IRecursoProyectoRepository repository)
    {
        _repository = repository;
    }

    public async Task<RecursoProyecto> CreateRecursoProyectoAsync(int idProyecto, int idRecurso, string? fechaAsignacion)
    {
        var recursoProyecto = new RecursoProyecto
        {
            IdProyecto = idProyecto,
            IdRecurso = idRecurso,
            FechaAsignacion = fechaAsignacion ?? DateTime.Now.ToString("dd/MM/yyyy")
        };

        return await _repository.CreateAsync(recursoProyecto);
    }

    public async Task<IEnumerable<RecursoProyecto>> GetAllRecursosProyectoAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<IEnumerable<RecursoProyecto>> GetByProyectoAsync(int idProyecto)
    {
        return await _repository.GetByProyectoAsync(idProyecto);
    }

    public async Task<IEnumerable<RecursoProyecto>> GetByRecursoAsync(int idRecurso)
    {
        return await _repository.GetByRecursoAsync(idRecurso);
    }

    public async Task<RecursoProyecto?> GetRecursoProyectoByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<RecursoProyecto> UpdateRecursoProyectoAsync(int id, Dictionary<string, object> updates)
    {
        var recursoProyecto = await _repository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"RecursoProyecto con ID {id} no encontrado.");

        foreach (var update in updates)
        {
            var propertyInfo = recursoProyecto.GetType().GetProperty(update.Key);
            if (propertyInfo != null && propertyInfo.CanWrite)
                propertyInfo.SetValue(recursoProyecto, update.Value);
        }

        return await _repository.UpdateAsync(recursoProyecto);
    }

    public async Task<bool> DeleteRecursoProyectoAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }
}

public class TareaService : ITareaService
{
    private readonly ITareaRepository _repository;

    public TareaService(ITareaRepository repository)
    {
        _repository = repository;
    }

    public async Task<Tarea> CreateTareaAsync(int idProyecto, string titulo, string? detalle, string? fechaCreacion, string? fechaFIN, string? prioridad, int? activo)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("El título de la tarea es obligatorio.");

        if (string.IsNullOrWhiteSpace(prioridad))
            prioridad = "Media";

        var tarea = new Tarea
        {
            IdProyecto = idProyecto,
            Titulo = titulo,
            Detalle = detalle,
            FechaCreacion = fechaCreacion ?? DateTime.Now.ToString("dd/MM/yyyy"),
            FechaFIN = fechaFIN,
            Prioridad = prioridad,
            Activo = activo ?? 1
        };

        return await _repository.CreateAsync(tarea);
    }

    public async Task<IEnumerable<Tarea>> GetAllTareasAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<IEnumerable<Tarea>> GetTareasByProyectoAsync(int idProyecto)
    {
        return await _repository.GetByProyectoAsync(idProyecto);
    }

    public async Task<IEnumerable<Tarea>> GetTareasByEstadoAsync(int activo)
    {
        return await _repository.GetByEstadoAsync(activo);
    }

    public async Task<IEnumerable<Tarea>> GetTareasByPrioridadAsync(string prioridad)
    {
        return await _repository.GetByPrioridadAsync(prioridad);
    }

    public async Task<IEnumerable<Tarea>> GetTareasByTituloAsync(string titulo)
    {
        return await _repository.GetByTituloAsync(titulo);
    }

    public async Task<Tarea?> GetTareaByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Tarea> UpdateTareaAsync(int id, Dictionary<string, object> updates)
    {
        var tarea = await _repository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"Tarea con ID {id} no encontrada.");

        foreach (var update in updates)
        {
            var propertyInfo = tarea.GetType().GetProperty(update.Key);
            if (propertyInfo != null && propertyInfo.CanWrite)
                propertyInfo.SetValue(tarea, update.Value);
        }

        return await _repository.UpdateAsync(tarea);
    }

    public async Task<bool> DeleteTareaAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<Tarea>> FilterTareasAsync(int? idTarea, int? activo, int? idProyecto, string? titulo, string? prioridad)
    {
        var allTareas = await _repository.GetAllAsync();
        var filtered = allTareas.AsEnumerable();

        if (idTarea.HasValue)
            filtered = filtered.Where(t => t.IdTarea == idTarea.Value);

        if (activo.HasValue)
            filtered = filtered.Where(t => t.Activo == activo.Value);

        if (idProyecto.HasValue)
            filtered = filtered.Where(t => t.IdProyecto == idProyecto.Value);

        if (!string.IsNullOrWhiteSpace(titulo))
            filtered = filtered.Where(t => t.Titulo.Contains(titulo, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(prioridad))
            filtered = filtered.Where(t => t.Prioridad == prioridad);

        return filtered.ToList();
    }
}

public class RecursoTareaService : IRecursoTareaService
{
    private readonly IRecursoTareaRepository _repository;

    public RecursoTareaService(IRecursoTareaRepository repository)
    {
        _repository = repository;
    }

    public async Task<RecursoTarea> CreateRecursoTareaAsync(int idTarea, int idRecurso, string? fechaAsignacion)
    {
        var recursoTarea = new RecursoTarea
        {
            IdTarea = idTarea,
            IdRecurso = idRecurso,
            FechaAsignacion = fechaAsignacion ?? DateTime.Now.ToString("dd/MM/yyyy")
        };

        return await _repository.CreateAsync(recursoTarea);
    }

    public async Task<IEnumerable<RecursoTarea>> GetAllRecursosTareaAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<IEnumerable<RecursoTarea>> GetByTareaAsync(int idTarea)
    {
        return await _repository.GetByTareaAsync(idTarea);
    }

    public async Task<IEnumerable<RecursoTarea>> GetByRecursoAsync(int idRecurso)
    {
        return await _repository.GetByRecursoAsync(idRecurso);
    }

    public async Task<RecursoTarea?> GetRecursoTareaByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<RecursoTarea> UpdateRecursoTareaAsync(int id, Dictionary<string, object> updates)
    {
        var recursoTarea = await _repository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"RecursoTarea con ID {id} no encontrado.");

        foreach (var update in updates)
        {
            var propertyInfo = recursoTarea.GetType().GetProperty(update.Key);
            if (propertyInfo != null && propertyInfo.CanWrite)
                propertyInfo.SetValue(recursoTarea, update.Value);
        }

        return await _repository.UpdateAsync(recursoTarea);
    }

    public async Task<bool> DeleteRecursoTareaAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }
}

public class TareaDailyService : ITareaDailyService
{
    private readonly ITareaDailyRepository _repository;

    public TareaDailyService(ITareaDailyRepository repository)
    {
        _repository = repository;
    }

    public async Task<TareaDaily> CreateTareaDailyAsync(int idProyecto, int idRecurso, string titulo, string? fechaCreacion, string? fechaFIN, int? activo)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("El título de la tarea daily es obligatorio.");

        var tareaDaily = new TareaDaily
        {
            IdProyecto = idProyecto,
            IdRecurso = idRecurso,
            Titulo = titulo,
            FechaCreacion = fechaCreacion ?? DateTime.Now.ToString("dd/MM/yyyy"),
            FechaFIN = fechaFIN,
            Activo = activo ?? 1
        };

        return await _repository.CreateAsync(tareaDaily);
    }

    public async Task<IEnumerable<TareaDaily>> GetAllTareasDailyAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<IEnumerable<TareaDaily>> GetByProyectoAsync(int idProyecto)
    {
        return await _repository.GetByProyectoAsync(idProyecto);
    }

    public async Task<IEnumerable<TareaDaily>> GetByProyectoAndRecursoAsync(int idProyecto, int idRecurso)
    {
        return await _repository.GetByProyectoAndRecursoAsync(idProyecto, idRecurso);
    }

    public async Task<IEnumerable<TareaDaily>> GetByEstadoAsync(int activo)
    {
        return await _repository.GetByEstadoAsync(activo);
    }

    public async Task<IEnumerable<TareaDaily>> GetByTituloAsync(string titulo)
    {
        return await _repository.GetByTituloAsync(titulo);
    }

    public async Task<TareaDaily?> GetTareaDailyByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<TareaDaily> UpdateTareaDailyAsync(int id, Dictionary<string, object> updates)
    {
        var tareaDaily = await _repository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"TareaDaily con ID {id} no encontrada.");

        foreach (var update in updates)
        {
            var propertyInfo = tareaDaily.GetType().GetProperty(update.Key);
            if (propertyInfo != null && propertyInfo.CanWrite)
                propertyInfo.SetValue(tareaDaily, update.Value);
        }

        return await _repository.UpdateAsync(tareaDaily);
    }

    public async Task<bool> DeleteTareaDailyAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<TareaDaily>> FilterTareasDaily(int? idTareaDaily, int? activo, int? idProyecto, string? titulo, int? idRecurso)
    {
        var allTareas = await _repository.GetAllAsync();
        var filtered = allTareas.AsEnumerable();

        if (idTareaDaily.HasValue)
            filtered = filtered.Where(t => t.IdTareaDaily == idTareaDaily.Value);

        if (activo.HasValue)
            filtered = filtered.Where(t => t.Activo == activo.Value);

        if (idProyecto.HasValue)
            filtered = filtered.Where(t => t.IdProyecto == idProyecto.Value);

        if (idRecurso.HasValue)
            filtered = filtered.Where(t => t.IdRecurso == idRecurso.Value);

        if (!string.IsNullOrWhiteSpace(titulo))
            filtered = filtered.Where(t => t.Titulo.Contains(titulo, StringComparison.OrdinalIgnoreCase));

        return filtered.ToList();
    }
}

public class ImpedimentoDailyService : IImpedimentoDailyService
{
    private readonly IImpedimentoDailyRepository _repository;

    public ImpedimentoDailyService(IImpedimentoDailyRepository repository)
    {
        _repository = repository;
    }

    public async Task<ImpedimentoDaily> CreateImpedimentoDailyAsync(int idProyecto, int idRecurso, string impedimento, string explicacion, string? fechaCreacion, string? fechaFIN, int? activo)
    {
        if (string.IsNullOrWhiteSpace(impedimento))
            throw new ArgumentException("El impedimento es obligatorio.");

        if (string.IsNullOrWhiteSpace(explicacion))
            throw new ArgumentException("La explicación es obligatoria.");

        var impedimentoDaily = new ImpedimentoDaily
        {
            IdProyecto = idProyecto,
            IdRecurso = idRecurso,
            Impedimento = impedimento,
            Explicacion = explicacion,
            FechaCreacion = fechaCreacion ?? DateTime.Now.ToString("dd/MM/yyyy"),
            FechaFIN = fechaFIN,
            Activo = activo ?? 1
        };

        return await _repository.CreateAsync(impedimentoDaily);
    }

    public async Task<IEnumerable<ImpedimentoDaily>> GetAllImpedimentosDailyAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<IEnumerable<ImpedimentoDaily>> GetByProyectoAsync(int idProyecto)
    {
        return await _repository.GetByProyectoAsync(idProyecto);
    }

    public async Task<IEnumerable<ImpedimentoDaily>> GetByProyectoAndRecursoAsync(int idProyecto, int idRecurso)
    {
        return await _repository.GetByProyectoAndRecursoAsync(idProyecto, idRecurso);
    }

    public async Task<IEnumerable<ImpedimentoDaily>> GetActivosAsync()
    {
        return await _repository.GetActivosAsync();
    }

    public async Task<IEnumerable<ImpedimentoDaily>> GetByTextoAsync(string texto)
    {
        return await _repository.GetByTextoAsync(texto);
    }

    public async Task<ImpedimentoDaily?> GetImpedimentoDailyByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<ImpedimentoDaily> UpdateImpedimentoDailyAsync(int id, Dictionary<string, object> updates)
    {
        var impedimentoDaily = await _repository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"ImpedimentoDaily con ID {id} no encontrado.");

        foreach (var update in updates)
        {
            var propertyInfo = impedimentoDaily.GetType().GetProperty(update.Key);
            if (propertyInfo != null && propertyInfo.CanWrite)
                propertyInfo.SetValue(impedimentoDaily, update.Value);
        }

        return await _repository.UpdateAsync(impedimentoDaily);
    }

    public async Task<bool> DeleteImpedimentoDailyAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<ImpedimentoDaily>> FilterImpedimentosDaily(int? idImpedimentoDaily, int? activo, int? idProyecto, string? impedimento, int? idRecurso)
    {
        var allImpedimentos = await _repository.GetAllAsync();
        var filtered = allImpedimentos.AsEnumerable();

        if (idImpedimentoDaily.HasValue)
            filtered = filtered.Where(i => i.IdImpedimentoDaily == idImpedimentoDaily.Value);

        if (activo.HasValue)
            filtered = filtered.Where(i => i.Activo == activo.Value);

        if (idProyecto.HasValue)
            filtered = filtered.Where(i => i.IdProyecto == idProyecto.Value);

        if (idRecurso.HasValue)
            filtered = filtered.Where(i => i.IdRecurso == idRecurso.Value);

        if (!string.IsNullOrWhiteSpace(impedimento))
            filtered = filtered.Where(i => i.Impedimento.Contains(impedimento, StringComparison.OrdinalIgnoreCase));

        return filtered.ToList();
    }
}
