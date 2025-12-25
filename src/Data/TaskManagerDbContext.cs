using Microsoft.EntityFrameworkCore;
using TaskManager.Models;

namespace TaskManager.Data;

public class TaskManagerDbContext : DbContext
{
    public DbSet<Proyecto> Proyectos { get; set; }
    public DbSet<Recurso> Recursos { get; set; }
    public DbSet<RecursoProyecto> RecursosProyecto { get; set; }
    public DbSet<Tarea> Tareas { get; set; }
    public DbSet<RecursoTarea> RecursosTarea { get; set; }
    public DbSet<TareaDaily> TareasDaily { get; set; }
    public DbSet<ImpedimentoDaily> ImpedimentosDaily { get; set; }

    public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options) : base(options)
    {
        // Crear la BD si no existe
        try
        {
            Database.EnsureCreated();
        }
        catch
        {
            // Ignorar si falla en tests
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar tabla Proyectos
        modelBuilder.Entity<Proyecto>(entity =>
        {
            entity.HasKey(e => e.IdProyecto);
            entity.Property(e => e.IdProyecto).ValueGeneratedOnAdd();
            entity.Property(e => e.NombreProyecto).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Descripcion).HasMaxLength(1000);
            entity.Property(e => e.FechaInicio).IsRequired();
            entity.Property(e => e.Activo).IsRequired();
            entity.Property(e => e.TieneDaily).IsRequired();
        });

        // Configurar tabla Recursos
        modelBuilder.Entity<Recurso>(entity =>
        {
            entity.HasKey(e => e.IdRecurso);
            entity.Property(e => e.IdRecurso).ValueGeneratedOnAdd();
            entity.Property(e => e.NombreRecurso).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Activo).IsRequired();
            entity.Property(e => e.FechaCreacion).IsRequired();
        });

        // Configurar tabla RecursoProyecto
        modelBuilder.Entity<RecursoProyecto>(entity =>
        {
            entity.HasKey(e => e.IdRecursoProyecto);
            entity.Property(e => e.IdRecursoProyecto).ValueGeneratedOnAdd();
            entity.HasOne(e => e.Proyecto).WithMany(p => p.RecursosProyecto)
                .HasForeignKey(e => e.IdProyecto).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Recurso).WithMany(r => r.RecursosProyecto)
                .HasForeignKey(e => e.IdRecurso).OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.IdProyecto, e.IdRecurso }).IsUnique();
            entity.Property(e => e.FechaAsignacion).IsRequired();
        });

        // Configurar tabla Tareas
        modelBuilder.Entity<Tarea>(entity =>
        {
            entity.HasKey(e => e.IdTarea);
            entity.Property(e => e.IdTarea).ValueGeneratedOnAdd();
            entity.HasOne(e => e.Proyecto).WithMany(p => p.Tareas)
                .HasForeignKey(e => e.IdProyecto).OnDelete(DeleteBehavior.Cascade);
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Detalle).HasMaxLength(2000);
            entity.Property(e => e.FechaCreacion).IsRequired();
            entity.Property(e => e.Prioridad).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Activo).IsRequired();
        });

        // Configurar tabla RecursoTarea
        modelBuilder.Entity<RecursoTarea>(entity =>
        {
            entity.HasKey(e => e.IdRecursoTarea);
            entity.Property(e => e.IdRecursoTarea).ValueGeneratedOnAdd();
            entity.HasOne(e => e.Tarea).WithMany(t => t.RecursosTarea)
                .HasForeignKey(e => e.IdTarea).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Recurso).WithMany(r => r.RecursosTarea)
                .HasForeignKey(e => e.IdRecurso).OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.IdTarea, e.IdRecurso }).IsUnique();
            entity.Property(e => e.FechaAsignacion).IsRequired();
        });

        // Configurar tabla TareasDaily
        modelBuilder.Entity<TareaDaily>(entity =>
        {
            entity.HasKey(e => e.IdTareaDaily);
            entity.Property(e => e.IdTareaDaily).ValueGeneratedOnAdd();
            entity.HasOne(e => e.Proyecto).WithMany(p => p.TareasDaily)
                .HasForeignKey(e => e.IdProyecto).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Recurso).WithMany(r => r.TareasDaily)
                .HasForeignKey(e => e.IdRecurso).OnDelete(DeleteBehavior.Cascade);
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(500);
            entity.Property(e => e.FechaCreacion).IsRequired();
            entity.Property(e => e.Activo).IsRequired();
        });

        // Configurar tabla ImpedimentosDaily
        modelBuilder.Entity<ImpedimentoDaily>(entity =>
        {
            entity.HasKey(e => e.IdImpedimentoDaily);
            entity.Property(e => e.IdImpedimentoDaily).ValueGeneratedOnAdd();
            entity.HasOne(e => e.Proyecto).WithMany(p => p.ImpedimentosDaily)
                .HasForeignKey(e => e.IdProyecto).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Recurso).WithMany(r => r.ImpedimentosDaily)
                .HasForeignKey(e => e.IdRecurso).OnDelete(DeleteBehavior.Cascade);
            entity.Property(e => e.Impedimento).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Explicacion).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.FechaCreacion).IsRequired();
            entity.Property(e => e.Activo).IsRequired();
        });
    }
}
