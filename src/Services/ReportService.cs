using TaskManager.Models;
using TaskManager.Configuration;

namespace TaskManager.Services;

public interface IReportService
{
    Task<string> GenerateDailyReportAsync(DateTime? reportDate = null, int? idProyecto = null, string? nombreProyecto = null);
}

public class ReportService : IReportService
{
    private readonly ITareaService _tareaService;
    private readonly ITareaDailyService _tareaDailyService;
    private readonly IImpedimentoDailyService _impedimentoService;
    private readonly IProyectoService _proyectoService;
    private readonly IRecursoService _recursoService;
    private readonly IRecursoProyectoService _recursoProyectoService;
    private readonly AppConfiguration _config;

    public ReportService(
        ITareaService tareaService,
        ITareaDailyService tareaDailyService,
        IImpedimentoDailyService impedimentoService,
        IProyectoService proyectoService,
        IRecursoService recursoService,
        IRecursoProyectoService recursoProyectoService,
        AppConfiguration config)
    {
        _tareaService = tareaService;
        _tareaDailyService = tareaDailyService;
        _impedimentoService = impedimentoService;
        _proyectoService = proyectoService;
        _recursoService = recursoService;
        _recursoProyectoService = recursoProyectoService;
        _config = config;
    }

    public async Task<string> GenerateDailyReportAsync(DateTime? reportDate = null, int? idProyecto = null, string? nombreProyecto = null)
    {
        var fecha = reportDate ?? DateTime.Now;
        var fechaFormato = fecha.ToString("dd/MM/yyyy");
        
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"# Diario {fechaFormato}");
        sb.AppendLine("---");
        sb.AppendLine();

        var proyectos = await _proyectoService.GetAllProyectosAsync();

        if (idProyecto.HasValue)
        {
            proyectos = proyectos.Where(p => p.IdProyecto == idProyecto.Value);
        }
        else if (!string.IsNullOrWhiteSpace(nombreProyecto))
        {
            proyectos = proyectos.Where(p => p.NombreProyecto.Contains(nombreProyecto, StringComparison.OrdinalIgnoreCase));
        }

        foreach (var proyecto in proyectos.Where(p => p.Activo == 1))
        {
            sb.AppendLine($"## {proyecto.NombreProyecto}");
            sb.AppendLine();

            // Tareas finalizadas hace 3 días
            await AppendCompletedTasksAsync(sb, proyecto, fecha);

            // Tareas para hoy
            await AppendTodayTasksAsync(sb, proyecto, fecha);

            // Tareas a futuro
            await AppendFutureTasksAsync(sb, proyecto, fecha);

            // Si tiene Daily
            if (proyecto.TieneDaily == 1)
            {
                await AppendDailyAsync(sb, proyecto, fecha);
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }

    private async Task AppendCompletedTasksAsync(System.Text.StringBuilder sb, Proyecto proyecto, DateTime fecha)
    {
        sb.AppendLine("### Tareas finalizadas ayer");

        var tareas = await _tareaService.GetTareasByProyectoAsync(proyecto.IdProyecto);
        var diasAtras = _config.PreviousDaysForReport;
        var fechaInicio = fecha.AddDays(-diasAtras).ToString("dd/MM/yyyy");
        var fechaFin = fecha.AddDays(-1).ToString("dd/MM/yyyy");

        var tareasFinalizadas = tareas
            .Where(t => !string.IsNullOrEmpty(t.FechaFIN) && IsDateInRange(t.FechaFIN, fechaInicio, fechaFin))
            .ToList();

        if (tareasFinalizadas.Any())
        {
            foreach (var tarea in tareasFinalizadas)
            {
                sb.AppendLine($"- **{tarea.Titulo}** (Prioridad: {tarea.Prioridad}, Finalizada: {tarea.FechaFIN})");
                if (!string.IsNullOrEmpty(tarea.Detalle))
                    sb.AppendLine($"  - Detalle: {tarea.Detalle}");
            }
        }
        else
        {
            sb.AppendLine("- No hay tareas finalizadas en el período especificado.");
        }

        sb.AppendLine();
    }

    private async Task AppendTodayTasksAsync(System.Text.StringBuilder sb, Proyecto proyecto, DateTime fecha)
    {
        sb.AppendLine("### Tareas para hoy");

        var tareas = await _tareaService.GetTareasByProyectoAsync(proyecto.IdProyecto);
        var fechaHoy = fecha.ToString("dd/MM/yyyy");

        var tareaasHoy = tareas
            .Where(t => t.Activo == 1 && t.FechaFIN == fechaHoy)
            .ToList();

        if (tareaasHoy.Any())
        {
            foreach (var tarea in tareaasHoy)
            {
                sb.AppendLine($"- **{tarea.Titulo}** (Prioridad: {tarea.Prioridad})");
                if (!string.IsNullOrEmpty(tarea.Detalle))
                    sb.AppendLine($"  - Detalle: {tarea.Detalle}");
            }
        }
        else
        {
            sb.AppendLine("- No hay tareas para hoy.");
        }

        sb.AppendLine();
    }

    private async Task AppendFutureTasksAsync(System.Text.StringBuilder sb, Proyecto proyecto, DateTime fecha)
    {
        sb.AppendLine("### Tareas a futuro");

        var tareas = await _tareaService.GetTareasByProyectoAsync(proyecto.IdProyecto);
        var fechaHoy = fecha.ToString("dd/MM/yyyy");

        var tareasFuturo = tareas
            .Where(t => t.Activo == 1 && (string.IsNullOrEmpty(t.FechaFIN) || CompareDate(t.FechaFIN, fechaHoy) > 0))
            .ToList();

        if (tareasFuturo.Any())
        {
            foreach (var tarea in tareasFuturo)
            {
                var fechaFin = string.IsNullOrEmpty(tarea.FechaFIN) ? "Sin fecha" : tarea.FechaFIN;
                sb.AppendLine($"- **{tarea.Titulo}** (Prioridad: {tarea.Prioridad}, Fecha fin: {fechaFin})");
                if (!string.IsNullOrEmpty(tarea.Detalle))
                    sb.AppendLine($"  - Detalle: {tarea.Detalle}");
            }
        }
        else
        {
            sb.AppendLine("- No hay tareas a futuro.");
        }

        sb.AppendLine();
    }

    private async Task AppendDailyAsync(System.Text.StringBuilder sb, Proyecto proyecto, DateTime fecha)
    {
        sb.AppendLine("### Daily");
        sb.AppendLine();

        var recursosProyecto = await _recursoProyectoService.GetByProyectoAsync(proyecto.IdProyecto);
        var recursosActivos = (await _recursoService.GetActivosAsync()).ToList();

        foreach (var recursoProyecto in recursosProyecto)
        {
            var recurso = recursosActivos.FirstOrDefault(r => r.IdRecurso == recursoProyecto.IdRecurso);
            if (recurso == null) continue;

            sb.AppendLine($"#### {recurso.NombreRecurso}");
            sb.AppendLine();

            // Qué hice ayer
            await AppendYesterdayTasksAsync(sb, proyecto, recurso, fecha);

            // Qué voy a hacer hoy
            sb.AppendLine("##### Qué voy a hacer hoy");
            sb.AppendLine("- (Por completar)");
            sb.AppendLine();

            // Impedimentos
            await AppendImpedimentosAsync(sb, proyecto, recurso);

            sb.AppendLine();
        }
    }

    private async Task AppendYesterdayTasksAsync(System.Text.StringBuilder sb, Proyecto proyecto, Recurso recurso, DateTime fecha)
    {
        sb.AppendLine("##### Qué hice ayer");

        var tareasDailyAyer = await _tareaDailyService.GetByProyectoAndRecursoAsync(proyecto.IdProyecto, recurso.IdRecurso);
        var fechaAyer = fecha.AddDays(-1).ToString("dd/MM/yyyy");

        var tareasAyer = tareasDailyAyer
            .Where(t => t.Activo == 1 && (string.IsNullOrEmpty(t.FechaFIN) || t.FechaFIN == fechaAyer))
            .ToList();

        if (tareasAyer.Any())
        {
            foreach (var tarea in tareasAyer)
            {
                sb.AppendLine($"- {tarea.Titulo}");
            }
        }
        else
        {
            sb.AppendLine("- No hay tareas registradas.");
        }

        sb.AppendLine();
    }

    private async Task AppendImpedimentosAsync(System.Text.StringBuilder sb, Proyecto proyecto, Recurso recurso)
    {
        sb.AppendLine("##### Impedimentos");

        var impedimentos = await _impedimentoService.GetByProyectoAndRecursoAsync(proyecto.IdProyecto, recurso.IdRecurso);
        var impedimentosActivos = impedimentos.Where(i => i.Activo == 1).ToList();

        if (impedimentosActivos.Any())
        {
            foreach (var impedimento in impedimentosActivos)
            {
                sb.AppendLine($"- **{impedimento.Impedimento}**: {impedimento.Explicacion}");
            }
        }
        else
        {
            sb.AppendLine("- Sin impedimentos registrados.");
        }

        sb.AppendLine();
    }

    private bool IsDateInRange(string fecha, string fechaInicio, string fechaFin)
    {
        if (!DateTime.TryParseExact(fecha, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var date))
            return false;

        if (!DateTime.TryParseExact(fechaInicio, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var inicio))
            return false;

        if (!DateTime.TryParseExact(fechaFin, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var fin))
            return false;

        return date >= inicio && date <= fin;
    }

    private int CompareDate(string fecha1, string fecha2)
    {
        if (!DateTime.TryParseExact(fecha1, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var d1))
            return 0;

        if (!DateTime.TryParseExact(fecha2, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var d2))
            return 0;

        return d1.CompareTo(d2);
    }
}
