namespace TaskManager.Models;

public class Proyecto
{
    public int IdProyecto { get; set; }
    public required string NombreProyecto { get; set; }
    public string? Descripcion { get; set; }
    public required string FechaInicio { get; set; }
    public int Activo { get; set; } = 1;
    public int TieneDaily { get; set; } = 0;

    // Navigation properties
    public virtual ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
    public virtual ICollection<TareaDaily> TareasDaily { get; set; } = new List<TareaDaily>();
    public virtual ICollection<ImpedimentoDaily> ImpedimentosDaily { get; set; } = new List<ImpedimentoDaily>();
    public virtual ICollection<RecursoProyecto> RecursosProyecto { get; set; } = new List<RecursoProyecto>();
}

public class Recurso
{
    public int IdRecurso { get; set; }
    public required string NombreRecurso { get; set; }
    public int Activo { get; set; } = 1;
    public required string FechaCreacion { get; set; }

    // Navigation properties
    public virtual ICollection<RecursoProyecto> RecursosProyecto { get; set; } = new List<RecursoProyecto>();
    public virtual ICollection<RecursoTarea> RecursosTarea { get; set; } = new List<RecursoTarea>();
    public virtual ICollection<TareaDaily> TareasDaily { get; set; } = new List<TareaDaily>();
    public virtual ICollection<ImpedimentoDaily> ImpedimentosDaily { get; set; } = new List<ImpedimentoDaily>();
}

public class RecursoProyecto
{
    public int IdRecursoProyecto { get; set; }
    public int IdProyecto { get; set; }
    public int IdRecurso { get; set; }
    public required string FechaAsignacion { get; set; }

    // Foreign keys
    public virtual Proyecto? Proyecto { get; set; }
    public virtual Recurso? Recurso { get; set; }
}

public class Tarea
{
    public int IdTarea { get; set; }
    public int IdProyecto { get; set; }
    public required string Titulo { get; set; }
    public string? Detalle { get; set; }
    public required string FechaCreacion { get; set; }
    public string? FechaFIN { get; set; }
    public required string Prioridad { get; set; } // Alta, Media, Baja
    public int Activo { get; set; } = 1;

    // Foreign keys
    public virtual Proyecto? Proyecto { get; set; }
    public virtual ICollection<RecursoTarea> RecursosTarea { get; set; } = new List<RecursoTarea>();
}

public class RecursoTarea
{
    public int IdRecursoTarea { get; set; }
    public int IdTarea { get; set; }
    public int IdRecurso { get; set; }
    public required string FechaAsignacion { get; set; }

    // Foreign keys
    public virtual Tarea? Tarea { get; set; }
    public virtual Recurso? Recurso { get; set; }
}

public class TareaDaily
{
    public int IdTareaDaily { get; set; }
    public int IdProyecto { get; set; }
    public int IdRecurso { get; set; }
    public required string Titulo { get; set; }
    public required string FechaCreacion { get; set; }
    public string? FechaFIN { get; set; }
    public int Activo { get; set; } = 1;

    // Foreign keys
    public virtual Proyecto? Proyecto { get; set; }
    public virtual Recurso? Recurso { get; set; }
}

public class ImpedimentoDaily
{
    public int IdImpedimentoDaily { get; set; }
    public int IdProyecto { get; set; }
    public int IdRecurso { get; set; }
    public required string Impedimento { get; set; }
    public required string Explicacion { get; set; }
    public required string FechaCreacion { get; set; }
    public string? FechaFIN { get; set; }
    public int Activo { get; set; } = 1;

    // Foreign keys
    public virtual Proyecto? Proyecto { get; set; }
    public virtual Recurso? Recurso { get; set; }
}
