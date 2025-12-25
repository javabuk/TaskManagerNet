using TaskManager.Models;

namespace TaskManager.Repositories;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(int id);
}

public interface IProyectoRepository : IRepository<Proyecto>
{
    Task<Proyecto?> GetByNameAsync(string nombre);
}

public interface IRecursoRepository : IRepository<Recurso>
{
    Task<IEnumerable<Recurso>> GetActivosAsync();
}

public interface IRecursoProyectoRepository : IRepository<RecursoProyecto>
{
    Task<IEnumerable<RecursoProyecto>> GetByProyectoAsync(int idProyecto);
    Task<IEnumerable<RecursoProyecto>> GetByRecursoAsync(int idRecurso);
    Task<RecursoProyecto?> GetByProyectoAndRecursoAsync(int idProyecto, int idRecurso);
}

public interface ITareaRepository : IRepository<Tarea>
{
    Task<IEnumerable<Tarea>> GetByProyectoAsync(int idProyecto);
    Task<IEnumerable<Tarea>> GetByEstadoAsync(int activo);
    Task<IEnumerable<Tarea>> GetByPrioridadAsync(string prioridad);
    Task<IEnumerable<Tarea>> GetByTituloAsync(string titulo);
}

public interface IRecursoTareaRepository : IRepository<RecursoTarea>
{
    Task<IEnumerable<RecursoTarea>> GetByTareaAsync(int idTarea);
    Task<IEnumerable<RecursoTarea>> GetByRecursoAsync(int idRecurso);
    Task<RecursoTarea?> GetByTareaAndRecursoAsync(int idTarea, int idRecurso);
}

public interface ITareaDailyRepository : IRepository<TareaDaily>
{
    Task<IEnumerable<TareaDaily>> GetByProyectoAsync(int idProyecto);
    Task<IEnumerable<TareaDaily>> GetByProyectoAndRecursoAsync(int idProyecto, int idRecurso);
    Task<IEnumerable<TareaDaily>> GetByEstadoAsync(int activo);
    Task<IEnumerable<TareaDaily>> GetByTituloAsync(string titulo);
}

public interface IImpedimentoDailyRepository : IRepository<ImpedimentoDaily>
{
    Task<IEnumerable<ImpedimentoDaily>> GetByProyectoAsync(int idProyecto);
    Task<IEnumerable<ImpedimentoDaily>> GetByProyectoAndRecursoAsync(int idProyecto, int idRecurso);
    Task<IEnumerable<ImpedimentoDaily>> GetActivosAsync();
    Task<IEnumerable<ImpedimentoDaily>> GetByTextoAsync(string texto);
}
