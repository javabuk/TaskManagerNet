using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly TaskManagerDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(TaskManagerDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<T> CreateAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null)
            return false;

        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}

public class ProyectoRepository : Repository<Proyecto>, IProyectoRepository
{
    public ProyectoRepository(TaskManagerDbContext context) : base(context)
    {
    }

    public async Task<Proyecto?> GetByNameAsync(string nombre)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.NombreProyecto == nombre);
    }
}

public class RecursoRepository : Repository<Recurso>, IRecursoRepository
{
    public RecursoRepository(TaskManagerDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Recurso>> GetActivosAsync()
    {
        return await _dbSet.Where(r => r.Activo == 1).ToListAsync();
    }
}

public class RecursoProyectoRepository : Repository<RecursoProyecto>, IRecursoProyectoRepository
{
    public RecursoProyectoRepository(TaskManagerDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<RecursoProyecto>> GetByProyectoAsync(int idProyecto)
    {
        return await _dbSet
            .Include(rp => rp.Proyecto)
            .Include(rp => rp.Recurso)
            .Where(rp => rp.IdProyecto == idProyecto)
            .ToListAsync();
    }

    public async Task<IEnumerable<RecursoProyecto>> GetByRecursoAsync(int idRecurso)
    {
        return await _dbSet
            .Include(rp => rp.Proyecto)
            .Include(rp => rp.Recurso)
            .Where(rp => rp.IdRecurso == idRecurso)
            .ToListAsync();
    }

    public async Task<RecursoProyecto?> GetByProyectoAndRecursoAsync(int idProyecto, int idRecurso)
    {
        return await _dbSet
            .FirstOrDefaultAsync(rp => rp.IdProyecto == idProyecto && rp.IdRecurso == idRecurso);
    }
}

public class TareaRepository : Repository<Tarea>, ITareaRepository
{
    public TareaRepository(TaskManagerDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Tarea>> GetByProyectoAsync(int idProyecto)
    {
        return await _dbSet
            .Include(t => t.Proyecto)
            .Where(t => t.IdProyecto == idProyecto)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tarea>> GetByEstadoAsync(int activo)
    {
        return await _dbSet
            .Include(t => t.Proyecto)
            .Where(t => t.Activo == activo)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tarea>> GetByPrioridadAsync(string prioridad)
    {
        return await _dbSet
            .Include(t => t.Proyecto)
            .Where(t => t.Prioridad == prioridad)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tarea>> GetByTituloAsync(string titulo)
    {
        return await _dbSet
            .Include(t => t.Proyecto)
            .Where(t => t.Titulo.Contains(titulo))
            .ToListAsync();
    }
}

public class RecursoTareaRepository : Repository<RecursoTarea>, IRecursoTareaRepository
{
    public RecursoTareaRepository(TaskManagerDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<RecursoTarea>> GetByTareaAsync(int idTarea)
    {
        return await _dbSet
            .Include(rt => rt.Tarea)
            .Include(rt => rt.Recurso)
            .Where(rt => rt.IdTarea == idTarea)
            .ToListAsync();
    }

    public async Task<IEnumerable<RecursoTarea>> GetByRecursoAsync(int idRecurso)
    {
        return await _dbSet
            .Include(rt => rt.Tarea)
            .Include(rt => rt.Recurso)
            .Where(rt => rt.IdRecurso == idRecurso)
            .ToListAsync();
    }

    public async Task<RecursoTarea?> GetByTareaAndRecursoAsync(int idTarea, int idRecurso)
    {
        return await _dbSet
            .FirstOrDefaultAsync(rt => rt.IdTarea == idTarea && rt.IdRecurso == idRecurso);
    }
}

public class TareaDailyRepository : Repository<TareaDaily>, ITareaDailyRepository
{
    public TareaDailyRepository(TaskManagerDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TareaDaily>> GetByProyectoAsync(int idProyecto)
    {
        return await _dbSet
            .Include(td => td.Proyecto)
            .Include(td => td.Recurso)
            .Where(td => td.IdProyecto == idProyecto)
            .ToListAsync();
    }

    public async Task<IEnumerable<TareaDaily>> GetByProyectoAndRecursoAsync(int idProyecto, int idRecurso)
    {
        return await _dbSet
            .Where(td => td.IdProyecto == idProyecto && td.IdRecurso == idRecurso)
            .ToListAsync();
    }

    public async Task<IEnumerable<TareaDaily>> GetByEstadoAsync(int activo)
    {
        return await _dbSet
            .Include(td => td.Proyecto)
            .Include(td => td.Recurso)
            .Where(td => td.Activo == activo)
            .ToListAsync();
    }

    public async Task<IEnumerable<TareaDaily>> GetByTituloAsync(string titulo)
    {
        return await _dbSet
            .Include(td => td.Proyecto)
            .Include(td => td.Recurso)
            .Where(td => td.Titulo.Contains(titulo))
            .ToListAsync();
    }
}

public class ImpedimentoDailyRepository : Repository<ImpedimentoDaily>, IImpedimentoDailyRepository
{
    public ImpedimentoDailyRepository(TaskManagerDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ImpedimentoDaily>> GetByProyectoAsync(int idProyecto)
    {
        return await _dbSet
            .Include(id => id.Proyecto)
            .Include(id => id.Recurso)
            .Where(id => id.IdProyecto == idProyecto)
            .ToListAsync();
    }

    public async Task<IEnumerable<ImpedimentoDaily>> GetByProyectoAndRecursoAsync(int idProyecto, int idRecurso)
    {
        return await _dbSet
            .Where(id => id.IdProyecto == idProyecto && id.IdRecurso == idRecurso)
            .ToListAsync();
    }

    public async Task<IEnumerable<ImpedimentoDaily>> GetActivosAsync()
    {
        return await _dbSet
            .Include(id => id.Proyecto)
            .Include(id => id.Recurso)
            .Where(id => id.Activo == 1)
            .ToListAsync();
    }

    public async Task<IEnumerable<ImpedimentoDaily>> GetByTextoAsync(string texto)
    {
        return await _dbSet
            .Include(id => id.Proyecto)
            .Include(id => id.Recurso)
            .Where(id => id.Impedimento.Contains(texto))
            .ToListAsync();
    }
}
