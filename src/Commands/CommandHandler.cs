using Spectre.Console;
using TaskManager.Services;

namespace TaskManager.Commands;

public interface ICommandHandler
{
    Task HandleAsync(string[] args);
}

public class CommandHandler : ICommandHandler
{
    private readonly IProyectoService _proyectoService;
    private readonly IRecursoService _recursoService;
    private readonly IRecursoProyectoService _recursoProyectoService;
    private readonly ITareaService _tareaService;
    private readonly IRecursoTareaService _recursoTareaService;
    private readonly ITareaDailyService _tareaDailyService;
    private readonly IImpedimentoDailyService _impedimentoService;
    private readonly IReportService _reportService;
    private readonly ILoggerService _loggerService;
    private readonly IAIService _aiService;
    private readonly IDataCollectionService _dataCollectionService;
    private readonly IMarkdownService _markdownService;

    public CommandHandler(
        IProyectoService proyectoService,
        IRecursoService recursoService,
        IRecursoProyectoService recursoProyectoService,
        ITareaService tareaService,
        IRecursoTareaService recursoTareaService,
        ITareaDailyService tareaDailyService,
        IImpedimentoDailyService impedimentoService,
        IReportService reportService,
        ILoggerService loggerService,
        IAIService aiService,
        IDataCollectionService dataCollectionService,
        IMarkdownService markdownService)
    {
        _proyectoService = proyectoService;
        _recursoService = recursoService;
        _recursoProyectoService = recursoProyectoService;
        _tareaService = tareaService;
        _recursoTareaService = recursoTareaService;
        _tareaDailyService = tareaDailyService;
        _impedimentoService = impedimentoService;
        _reportService = reportService;
        _loggerService = loggerService;
        _aiService = aiService;
        _dataCollectionService = dataCollectionService;
        _markdownService = markdownService;
    }

    public async Task HandleAsync(string[] args)
    {
        var command = args[0].ToLower();

        try
        {
            switch (command)
            {
                case "proyecto":
                    await HandleProyectoAsync(args.Skip(1).ToArray());
                    break;
                case "recurso":
                    await HandleRecursoAsync(args.Skip(1).ToArray());
                    break;
                case "recurso-proyecto":
                    await HandleRecursoProyectoAsync(args.Skip(1).ToArray());
                    break;
                case "tarea":
                    await HandleTareaAsync(args.Skip(1).ToArray());
                    break;
                case "recurso-tarea":
                    await HandleRecursoTareaAsync(args.Skip(1).ToArray());
                    break;
                case "tarea-daily":
                    await HandleTareaDailyAsync(args.Skip(1).ToArray());
                    break;
                case "impedimento-daily":
                    await HandleImpedimentoDailyAsync(args.Skip(1).ToArray());
                    break;
                case "reporte":
                    await HandleReporteAsync(args.Skip(1).ToArray());
                    break;
                case "sugerencia":
                    await HandleSugerenciaAsync(args.Skip(1).ToArray());
                    break;
                default:
                    AnsiConsole.MarkupLine("[red]Comando no reconocido.[/]");
                    _loggerService.LogWarning($"Comando no reconocido: {command}");
                    break;
            }
        }
        catch (Exception ex)
        {
            _loggerService.LogError($"Error en comando {command}", ex);
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
    }

    private async Task HandleProyectoAsync(string[] args)
    {
        var subcommand = args.Length > 0 ? args[0].ToLower() : "";

        switch (subcommand)
        {
            case "crear":
                await CrearProyectoAsync(args.Skip(1).ToArray());
                break;
            case "listar":
                await ListarProyectosAsync();
                break;
            case "modificar":
                await ModificarProyectoAsync(args.Skip(1).ToArray());
                break;
            default:
                AnsiConsole.MarkupLine("[red]Subcomando no válido para proyecto[/]");
                break;
        }
    }

    private async Task CrearProyectoAsync(string[] args)
    {
        var nombre = GetArgumentValue(args, "--nombre");
        var descripcion = GetArgumentValue(args, "--descripcion");
        var fechaInicio = GetArgumentValue(args, "--fecha-inicio");
        var activo = int.TryParse(GetArgumentValue(args, "--activo"), out var a) ? a : (int?)null;
        var tieneDaily = int.TryParse(GetArgumentValue(args, "--tiene-daily"), out var td) ? td : (int?)null;

        if (string.IsNullOrEmpty(nombre))
        {
            AnsiConsole.MarkupLine("[red]El nombre del proyecto es obligatorio[/]");
            return;
        }

        try
        {
            var proyecto = await _proyectoService.CreateProyectoAsync(nombre, descripcion, fechaInicio, activo, tieneDaily);
            MostrarProyecto(proyecto);
            AnsiConsole.MarkupLine("[green]✓ Proyecto creado exitosamente[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error al crear proyecto: {ex.Message}[/]");
        }
    }

    private async Task ListarProyectosAsync()
    {
        var proyectos = await _proyectoService.GetAllProyectosAsync();

        if (!proyectos.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No hay proyectos registrados[/]");
            return;
        }

        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Nombre");
        table.AddColumn("Descripción");
        table.AddColumn("Fecha Inicio");
        table.AddColumn("Activo");
        table.AddColumn("Daily");

        foreach (var proyecto in proyectos)
        {
            table.AddRow(
                proyecto.IdProyecto.ToString(),
                proyecto.NombreProyecto,
                proyecto.Descripcion ?? "N/A",
                proyecto.FechaInicio,
                proyecto.Activo == 1 ? "Sí" : "No",
                proyecto.TieneDaily == 1 ? "Sí" : "No"
            );
        }

        AnsiConsole.Write(table);
    }

    private async Task ModificarProyectoAsync(string[] args)
    {
        if (!int.TryParse(GetArgumentValue(args, "--id"), out var id))
        {
            AnsiConsole.MarkupLine("[red]ID de proyecto inválido[/]");
            return;
        }

        var updates = new Dictionary<string, object>();

        var nombre = GetArgumentValue(args, "--nombre");
        if (!string.IsNullOrEmpty(nombre))
            updates["NombreProyecto"] = nombre;

        var descripcion = GetArgumentValue(args, "--descripcion");
        if (descripcion != null)
            updates["Descripcion"] = descripcion;

        if (int.TryParse(GetArgumentValue(args, "--activo"), out var activo))
            updates["Activo"] = activo;

        if (updates.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No se especificaron campos a modificar[/]");
            return;
        }

        try
        {
            var proyecto = await _proyectoService.UpdateProyectoAsync(id, updates);
            MostrarProyecto(proyecto);
            AnsiConsole.MarkupLine("[green]✓ Proyecto modificado exitosamente[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
    }

    private async Task HandleRecursoAsync(string[] args)
    {
        var subcommand = args.Length > 0 ? args[0].ToLower() : "";

        switch (subcommand)
        {
            case "crear":
                await CrearRecursoAsync(args.Skip(1).ToArray());
                break;
            case "listar":
                await ListarRecursosAsync();
                break;
            case "modificar":
                await ModificarRecursoAsync(args.Skip(1).ToArray());
                break;
            default:
                AnsiConsole.MarkupLine("[red]Subcomando no válido para recurso[/]");
                break;
        }
    }

    private async Task CrearRecursoAsync(string[] args)
    {
        var nombre = GetArgumentValue(args, "--nombre");
        var activo = int.TryParse(GetArgumentValue(args, "--activo"), out var a) ? a : (int?)null;

        if (string.IsNullOrEmpty(nombre))
        {
            AnsiConsole.MarkupLine("[red]El nombre del recurso es obligatorio[/]");
            return;
        }

        try
        {
            var recurso = await _recursoService.CreateRecursoAsync(nombre, activo, null);
            MostrarRecurso(recurso);
            AnsiConsole.MarkupLine("[green]✓ Recurso creado exitosamente[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
    }

    private async Task ListarRecursosAsync()
    {
        var recursos = await _recursoService.GetAllRecursosAsync();

        if (!recursos.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No hay recursos registrados[/]");
            return;
        }

        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Nombre");
        table.AddColumn("Activo");
        table.AddColumn("Fecha Creación");

        foreach (var recurso in recursos)
        {
            table.AddRow(
                recurso.IdRecurso.ToString(),
                recurso.NombreRecurso,
                recurso.Activo == 1 ? "Sí" : "No",
                recurso.FechaCreacion
            );
        }

        AnsiConsole.Write(table);
    }

    private async Task ModificarRecursoAsync(string[] args)
    {
        if (!int.TryParse(GetArgumentValue(args, "--id"), out var id))
        {
            AnsiConsole.MarkupLine("[red]ID de recurso inválido[/]");
            return;
        }

        var updates = new Dictionary<string, object>();

        var nombre = GetArgumentValue(args, "--nombre");
        if (!string.IsNullOrEmpty(nombre))
            updates["NombreRecurso"] = nombre;

        if (int.TryParse(GetArgumentValue(args, "--activo"), out var activo))
            updates["Activo"] = activo;

        if (updates.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No se especificaron campos a modificar[/]");
            return;
        }

        try
        {
            var recurso = await _recursoService.UpdateRecursoAsync(id, updates);
            MostrarRecurso(recurso);
            AnsiConsole.MarkupLine("[green]✓ Recurso modificado exitosamente[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
    }

    private async Task HandleRecursoProyectoAsync(string[] args)
    {
        var subcommand = args.Length > 0 ? args[0].ToLower() : "";

        switch (subcommand)
        {
            case "crear":
                await CrearRecursoProyectoAsync(args.Skip(1).ToArray());
                break;
            case "listar":
                await ListarRecursosProyectoAsync(args.Skip(1).ToArray());
                break;
            case "modificar":
                await ModificarRecursoProyectoAsync(args.Skip(1).ToArray());
                break;
            default:
                AnsiConsole.MarkupLine("[red]Subcomando no válido[/]");
                break;
        }
    }

    private async Task CrearRecursoProyectoAsync(string[] args)
    {
        if (!int.TryParse(GetArgumentValue(args, "--id-proyecto"), out var idProyecto))
        {
            AnsiConsole.MarkupLine("[red]ID de proyecto inválido[/]");
            return;
        }

        if (!int.TryParse(GetArgumentValue(args, "--id-recurso"), out var idRecurso))
        {
            AnsiConsole.MarkupLine("[red]ID de recurso inválido[/]");
            return;
        }

        try
        {
            var recursoProyecto = await _recursoProyectoService.CreateRecursoProyectoAsync(idProyecto, idRecurso, null);
            AnsiConsole.MarkupLine($"[green]✓ Recurso asignado a proyecto - ID: {recursoProyecto.IdRecursoProyecto}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
    }

    private async Task ListarRecursosProyectoAsync(string[] args)
    {
        var idProyecto = int.TryParse(GetArgumentValue(args, "--id-proyecto"), out var p) ? p : (int?)null;
        var idRecurso = int.TryParse(GetArgumentValue(args, "--id-recurso"), out var r) ? r : (int?)null;

        IEnumerable<Models.RecursoProyecto> recursosProyecto;

        if (idProyecto.HasValue)
            recursosProyecto = await _recursoProyectoService.GetByProyectoAsync(idProyecto.Value);
        else if (idRecurso.HasValue)
            recursosProyecto = await _recursoProyectoService.GetByRecursoAsync(idRecurso.Value);
        else
            recursosProyecto = await _recursoProyectoService.GetAllRecursosProyectoAsync();

        if (!recursosProyecto.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No hay asignaciones registradas[/]");
            return;
        }

        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Proyecto");
        table.AddColumn("Recurso");
        table.AddColumn("Fecha Asignación");

        foreach (var rp in recursosProyecto)
        {
            table.AddRow(
                rp.IdRecursoProyecto.ToString(),
                rp.Proyecto?.NombreProyecto ?? "N/A",
                rp.Recurso?.NombreRecurso ?? "N/A",
                rp.FechaAsignacion
            );
        }

        AnsiConsole.Write(table);
    }

    private async Task ModificarRecursoProyectoAsync(string[] args)
    {
        if (!int.TryParse(GetArgumentValue(args, "--id"), out var id))
        {
            AnsiConsole.MarkupLine("[red]ID inválido[/]");
            return;
        }

        var updates = new Dictionary<string, object>();

        var fechaAsignacion = GetArgumentValue(args, "--fecha-asignacion");
        if (!string.IsNullOrEmpty(fechaAsignacion))
            updates["FechaAsignacion"] = fechaAsignacion;

        if (updates.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No se especificaron campos a modificar[/]");
            return;
        }

        try
        {
            var recursoProyecto = await _recursoProyectoService.UpdateRecursoProyectoAsync(id, updates);
            AnsiConsole.MarkupLine("[green]✓ Asignación modificada[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
    }

    private async Task HandleTareaAsync(string[] args)
    {
        var subcommand = args.Length > 0 ? args[0].ToLower() : "";

        switch (subcommand)
        {
            case "crear":
                await CrearTareaAsync(args.Skip(1).ToArray());
                break;
            case "listar":
                await ListarTareasAsync(args.Skip(1).ToArray());
                break;
            case "modificar":
                await ModificarTareaAsync(args.Skip(1).ToArray());
                break;
            default:
                AnsiConsole.MarkupLine("[red]Subcomando no válido[/]");
                break;
        }
    }

    private async Task CrearTareaAsync(string[] args)
    {
        if (!int.TryParse(GetArgumentValue(args, "--id-proyecto"), out var idProyecto))
        {
            AnsiConsole.MarkupLine("[red]ID de proyecto inválido[/]");
            return;
        }

        var titulo = GetArgumentValue(args, "--titulo");
        if (string.IsNullOrEmpty(titulo))
        {
            AnsiConsole.MarkupLine("[red]El título es obligatorio[/]");
            return;
        }

        var detalle = GetArgumentValue(args, "--detalle");
        var prioridad = GetArgumentValue(args, "--prioridad") ?? "Media";

        try
        {
            var tarea = await _tareaService.CreateTareaAsync(idProyecto, titulo, detalle, null, null, prioridad, null);
            MostrarTarea(tarea);
            AnsiConsole.MarkupLine("[green]✓ Tarea creada[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
    }

    private async Task ListarTareasAsync(string[] args)
    {
        var idProyecto = int.TryParse(GetArgumentValue(args, "--id-proyecto"), out var p) ? p : (int?)null;
        var titulo = GetArgumentValue(args, "--titulo");
        var prioridad = GetArgumentValue(args, "--prioridad");
        var activo = int.TryParse(GetArgumentValue(args, "--activo"), out var a) ? a : (int?)null;

        var tareas = await _tareaService.FilterTareasAsync(null, activo, idProyecto, titulo, prioridad);

        if (!tareas.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No hay tareas registradas[/]");
            return;
        }

        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Proyecto");
        table.AddColumn("Título");
        table.AddColumn("Prioridad");
        table.AddColumn("Activo");
        table.AddColumn("Fecha Fin");

        foreach (var tarea in tareas)
        {
            table.AddRow(
                tarea.IdTarea.ToString(),
                tarea.Proyecto?.NombreProyecto ?? "N/A",
                tarea.Titulo,
                tarea.Prioridad,
                tarea.Activo == 1 ? "Sí" : "No",
                tarea.FechaFIN ?? "N/A"
            );
        }

        AnsiConsole.Write(table);
    }

    private async Task ModificarTareaAsync(string[] args)
    {
        if (!int.TryParse(GetArgumentValue(args, "--id"), out var id))
        {
            AnsiConsole.MarkupLine("[red]ID inválido[/]");
            return;
        }

        var updates = new Dictionary<string, object>();

        var titulo = GetArgumentValue(args, "--titulo");
        if (!string.IsNullOrEmpty(titulo))
            updates["Titulo"] = titulo;

        var fechaFin = GetArgumentValue(args, "--fecha-fin");
        if (fechaFin != null)
            updates["FechaFIN"] = fechaFin;

        var prioridad = GetArgumentValue(args, "--prioridad");
        if (!string.IsNullOrEmpty(prioridad))
            updates["Prioridad"] = prioridad;

        var activo = int.TryParse(GetArgumentValue(args, "--activo"), out var a) ? (int?)a : null;
        if (activo.HasValue)
            updates["Activo"] = activo.Value;

        if (updates.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No se especificaron campos a modificar[/]");
            return;
        }

        try
        {
            var tarea = await _tareaService.UpdateTareaAsync(id, updates);
            MostrarTarea(tarea);
            AnsiConsole.MarkupLine("[green]✓ Tarea modificada[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
    }

    private async Task HandleRecursoTareaAsync(string[] args)
    {
        var subcommand = args.Length > 0 ? args[0].ToLower() : "";

        switch (subcommand)
        {
            case "crear":
                await CrearRecursoTareaAsync(args.Skip(1).ToArray());
                break;
            case "listar":
                await ListarRecursosTareaAsync(args.Skip(1).ToArray());
                break;
            case "modificar":
                await ModificarRecursoTareaAsync(args.Skip(1).ToArray());
                break;
            default:
                AnsiConsole.MarkupLine("[red]Subcomando no válido[/]");
                break;
        }
    }

    private async Task CrearRecursoTareaAsync(string[] args)
    {
        if (!int.TryParse(GetArgumentValue(args, "--id-tarea"), out var idTarea))
        {
            AnsiConsole.MarkupLine("[red]ID de tarea inválido[/]");
            return;
        }

        if (!int.TryParse(GetArgumentValue(args, "--id-recurso"), out var idRecurso))
        {
            AnsiConsole.MarkupLine("[red]ID de recurso inválido[/]");
            return;
        }

        try
        {
            var recursoTarea = await _recursoTareaService.CreateRecursoTareaAsync(idTarea, idRecurso, null);
            AnsiConsole.MarkupLine($"[green]✓ Recurso asignado a tarea - ID: {recursoTarea.IdRecursoTarea}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
    }

    private async Task ListarRecursosTareaAsync(string[] args)
    {
        var idTarea = int.TryParse(GetArgumentValue(args, "--id-tarea"), out var t) ? t : (int?)null;
        var idRecurso = int.TryParse(GetArgumentValue(args, "--id-recurso"), out var r) ? r : (int?)null;

        IEnumerable<Models.RecursoTarea> recursosTarea;

        if (idTarea.HasValue)
            recursosTarea = await _recursoTareaService.GetByTareaAsync(idTarea.Value);
        else if (idRecurso.HasValue)
            recursosTarea = await _recursoTareaService.GetByRecursoAsync(idRecurso.Value);
        else
            recursosTarea = await _recursoTareaService.GetAllRecursosTareaAsync();

        if (!recursosTarea.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No hay asignaciones de recursos a tareas[/]");
            return;
        }

        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Tarea");
        table.AddColumn("Recurso");
        table.AddColumn("Fecha Asignación");

        foreach (var rt in recursosTarea)
        {
            table.AddRow(
                rt.IdRecursoTarea.ToString(),
                rt.Tarea?.Titulo ?? "N/A",
                rt.Recurso?.NombreRecurso ?? "N/A",
                rt.FechaAsignacion
            );
        }

        AnsiConsole.Write(table);
    }

    private async Task ModificarRecursoTareaAsync(string[] args)
    {
        if (!int.TryParse(GetArgumentValue(args, "--id"), out var id))
        {
            AnsiConsole.MarkupLine("[red]ID inválido[/]");
            return;
        }

        var updates = new Dictionary<string, object>();
        var fechaAsignacion = GetArgumentValue(args, "--fecha-asignacion");
        if (!string.IsNullOrEmpty(fechaAsignacion))
            updates["FechaAsignacion"] = fechaAsignacion;

        if (updates.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No se especificaron campos a modificar[/]");
            return;
        }

        try
        {
            var recursoTarea = await _recursoTareaService.UpdateRecursoTareaAsync(id, updates);
            AnsiConsole.MarkupLine("[green]✓ Asignación modificada[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
    }

    private async Task HandleTareaDailyAsync(string[] args)
    {
        var subcommand = args.Length > 0 ? args[0].ToLower() : "";

        switch (subcommand)
        {
            case "crear":
                await CrearTareaDailyAsync(args.Skip(1).ToArray());
                break;
            case "listar":
                await ListarTareasDaily(args.Skip(1).ToArray());
                break;
            case "modificar":
                await ModificarTareaDailyAsync(args.Skip(1).ToArray());
                break;
            default:
                AnsiConsole.MarkupLine("[red]Subcomando no válido[/]");
                break;
        }
    }

    private async Task CrearTareaDailyAsync(string[] args)
    {
        if (!int.TryParse(GetArgumentValue(args, "--id-proyecto"), out var idProyecto))
        {
            AnsiConsole.MarkupLine("[red]ID de proyecto inválido[/]");
            return;
        }

        if (!int.TryParse(GetArgumentValue(args, "--id-recurso"), out var idRecurso))
        {
            AnsiConsole.MarkupLine("[red]ID de recurso inválido[/]");
            return;
        }

        var titulo = GetArgumentValue(args, "--titulo");
        if (string.IsNullOrEmpty(titulo))
        {
            AnsiConsole.MarkupLine("[red]El título es obligatorio[/]");
            return;
        }

        try
        {
            var tareaDaily = await _tareaDailyService.CreateTareaDailyAsync(idProyecto, idRecurso, titulo, null, null, null);
            AnsiConsole.MarkupLine($"[green]✓ Tarea Daily creada - ID: {tareaDaily.IdTareaDaily}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
    }

    private async Task ListarTareasDaily(string[] args)
    {
        var idProyecto = int.TryParse(GetArgumentValue(args, "--id-proyecto"), out var p) ? p : (int?)null;
        var idRecurso = int.TryParse(GetArgumentValue(args, "--id-recurso"), out var r) ? r : (int?)null;
        var titulo = GetArgumentValue(args, "--titulo");

        var tareas = await _tareaDailyService.FilterTareasDaily(null, null, idProyecto, titulo, idRecurso);

        if (!tareas.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No hay tareas daily registradas[/]");
            return;
        }

        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Proyecto");
        table.AddColumn("Recurso");
        table.AddColumn("Título");
        table.AddColumn("Fecha Fin");

        foreach (var tarea in tareas)
        {
            table.AddRow(
                tarea.IdTareaDaily.ToString(),
                tarea.Proyecto?.NombreProyecto ?? "N/A",
                tarea.Recurso?.NombreRecurso ?? "N/A",
                tarea.Titulo,
                tarea.FechaFIN ?? "N/A"
            );
        }

        AnsiConsole.Write(table);
    }

    private async Task ModificarTareaDailyAsync(string[] args)
    {
        if (!int.TryParse(GetArgumentValue(args, "--id"), out var id))
        {
            AnsiConsole.MarkupLine("[red]ID inválido[/]");
            return;
        }

        var updates = new Dictionary<string, object>();

        var titulo = GetArgumentValue(args, "--titulo");
        if (!string.IsNullOrEmpty(titulo))
            updates["Titulo"] = titulo;

        var fechaFin = GetArgumentValue(args, "--fecha-fin");
        if (fechaFin != null)
            updates["FechaFIN"] = fechaFin;

        var activo = int.TryParse(GetArgumentValue(args, "--activo"), out var a) ? (int?)a : null;
        if (activo.HasValue)
            updates["Activo"] = activo.Value;

        if (updates.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No se especificaron campos a modificar[/]");
            return;
        }

        try
        {
            var tareaDaily = await _tareaDailyService.UpdateTareaDailyAsync(id, updates);
            AnsiConsole.MarkupLine("[green]✓ Tarea Daily modificada[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
    }

    private async Task HandleImpedimentoDailyAsync(string[] args)
    {
        var subcommand = args.Length > 0 ? args[0].ToLower() : "";

        switch (subcommand)
        {
            case "crear":
                await CrearImpedimentoDailyAsync(args.Skip(1).ToArray());
                break;
            case "listar":
                await ListarImpedimentosDaily(args.Skip(1).ToArray());
                break;
            case "modificar":
                await ModificarImpedimentoDailyAsync(args.Skip(1).ToArray());
                break;
            default:
                AnsiConsole.MarkupLine("[red]Subcomando no válido[/]");
                break;
        }
    }

    private async Task CrearImpedimentoDailyAsync(string[] args)
    {
        if (!int.TryParse(GetArgumentValue(args, "--id-proyecto"), out var idProyecto))
        {
            AnsiConsole.MarkupLine("[red]ID de proyecto inválido[/]");
            return;
        }

        if (!int.TryParse(GetArgumentValue(args, "--id-recurso"), out var idRecurso))
        {
            AnsiConsole.MarkupLine("[red]ID de recurso inválido[/]");
            return;
        }

        var impedimento = GetArgumentValue(args, "--impedimento");
        var explicacion = GetArgumentValue(args, "--explicacion");

        if (string.IsNullOrEmpty(impedimento) || string.IsNullOrEmpty(explicacion))
        {
            AnsiConsole.MarkupLine("[red]Impedimento y explicación son obligatorios[/]");
            return;
        }

        try
        {
            var impedimentoDaily = await _impedimentoService.CreateImpedimentoDailyAsync(idProyecto, idRecurso, impedimento, explicacion, null, null, null);
            AnsiConsole.MarkupLine($"[green]✓ Impedimento creado - ID: {impedimentoDaily.IdImpedimentoDaily}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
    }

    private async Task ListarImpedimentosDaily(string[] args)
    {
        var idProyecto = int.TryParse(GetArgumentValue(args, "--id-proyecto"), out var p) ? p : (int?)null;
        var idRecurso = int.TryParse(GetArgumentValue(args, "--id-recurso"), out var r) ? r : (int?)null;
        var impedimento = GetArgumentValue(args, "--impedimento");

        var impedimentos = await _impedimentoService.FilterImpedimentosDaily(null, null, idProyecto, impedimento, idRecurso);

        if (!impedimentos.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No hay impedimentos daily registrados[/]");
            return;
        }

        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Proyecto");
        table.AddColumn("Recurso");
        table.AddColumn("Impedimento");
        table.AddColumn("Explicación");
        table.AddColumn("Activo");

        foreach (var imp in impedimentos)
        {
            table.AddRow(
                imp.IdImpedimentoDaily.ToString(),
                imp.Proyecto?.NombreProyecto ?? "N/A",
                imp.Recurso?.NombreRecurso ?? "N/A",
                imp.Impedimento,
                imp.Explicacion,
                imp.Activo == 1 ? "Sí" : "No"
            );
        }

        AnsiConsole.Write(table);
    }

    private async Task ModificarImpedimentoDailyAsync(string[] args)
    {
        if (!int.TryParse(GetArgumentValue(args, "--id"), out var id))
        {
            AnsiConsole.MarkupLine("[red]ID inválido[/]");
            return;
        }

        var updates = new Dictionary<string, object>();

        var impedimento = GetArgumentValue(args, "--impedimento");
        if (!string.IsNullOrEmpty(impedimento))
            updates["Impedimento"] = impedimento;

        var explicacion = GetArgumentValue(args, "--explicacion");
        if (!string.IsNullOrEmpty(explicacion))
            updates["Explicacion"] = explicacion;

        var fechaFin = GetArgumentValue(args, "--fecha-fin");
        if (fechaFin != null)
            updates["FechaFIN"] = fechaFin;

        var activo = int.TryParse(GetArgumentValue(args, "--activo"), out var a) ? (int?)a : null;
        if (activo.HasValue)
            updates["Activo"] = activo.Value;

        if (updates.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No se especificaron campos a modificar[/]");
            return;
        }

        try
        {
            var impedimentoDaily = await _impedimentoService.UpdateImpedimentoDailyAsync(id, updates);
            AnsiConsole.MarkupLine("[green]✓ Impedimento modificado[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
    }

    private async Task HandleReporteAsync(string[] args)
    {
        var subcommand = args.Length > 0 ? args[0].ToLower() : "";

        if (subcommand != "generar")
        {
            AnsiConsole.MarkupLine("[red]Subcomando no válido[/]");
            return;
        }

        var fechaStr = GetArgumentValue(args, "--fecha");
        var idProyecto = int.TryParse(GetArgumentValue(args, "--id-proyecto"), out var p) ? (int?)p : null;
        var nombreProyecto = GetArgumentValue(args, "--nombre-proyecto");

        DateTime? fecha = null;
        if (!string.IsNullOrEmpty(fechaStr))
        {
            if (DateTime.TryParseExact(fechaStr, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var d))
                fecha = d;
            else
                AnsiConsole.MarkupLine("[yellow]Formato de fecha inválido. Usando hoy.[/]");
        }

        try
        {
            var report = await _reportService.GenerateDailyReportAsync(fecha, idProyecto, nombreProyecto);
            AnsiConsole.Write(report);

            // Guardar reporte en archivo
            var nombreArchivo = fecha?.ToString("yyyy-MM-dd") ?? DateTime.Now.ToString("yyyy-MM-dd");
            var rutaArchivo = $"reporte_{nombreArchivo}.md";
            File.WriteAllText(rutaArchivo, report);
            AnsiConsole.MarkupLine($"[green]✓ Reporte guardado en: {rutaArchivo}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
    }

    private string? GetArgumentValue(string[] args, string argumentName)
    {
        var index = Array.FindIndex(args, a => a.Equals(argumentName, StringComparison.OrdinalIgnoreCase));
        return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
    }

    private void MostrarProyecto(Models.Proyecto proyecto)
    {
        var table = new Table();
        table.AddColumn("Propiedad");
        table.AddColumn("Valor");

        table.AddRow("ID", proyecto.IdProyecto.ToString());
        table.AddRow("Nombre", proyecto.NombreProyecto);
        table.AddRow("Descripción", proyecto.Descripcion ?? "N/A");
        table.AddRow("Fecha Inicio", proyecto.FechaInicio);
        table.AddRow("Activo", proyecto.Activo == 1 ? "Sí" : "No");
        table.AddRow("Daily", proyecto.TieneDaily == 1 ? "Sí" : "No");

        AnsiConsole.Write(table);
    }

    private void MostrarRecurso(Models.Recurso recurso)
    {
        var table = new Table();
        table.AddColumn("Propiedad");
        table.AddColumn("Valor");

        table.AddRow("ID", recurso.IdRecurso.ToString());
        table.AddRow("Nombre", recurso.NombreRecurso);
        table.AddRow("Activo", recurso.Activo == 1 ? "Sí" : "No");
        table.AddRow("Fecha Creación", recurso.FechaCreacion);

        AnsiConsole.Write(table);
    }

    private void MostrarTarea(Models.Tarea tarea)
    {
        var table = new Table();
        table.AddColumn("Propiedad");
        table.AddColumn("Valor");

        table.AddRow("ID", tarea.IdTarea.ToString());
        table.AddRow("Título", tarea.Titulo);
        table.AddRow("Detalle", tarea.Detalle ?? "N/A");
        table.AddRow("Prioridad", tarea.Prioridad);
        table.AddRow("Fecha Creación", tarea.FechaCreacion);
        table.AddRow("Fecha Fin", tarea.FechaFIN ?? "N/A");
        table.AddRow("Activo", tarea.Activo == 1 ? "Sí" : "No");

        AnsiConsole.Write(table);
    }

    private async Task HandleSugerenciaAsync(string[] args)
    {
        try
        {
            _loggerService.LogInfo("=== Iniciando comando SUGERENCIA ===");
            
            // Verificar si se solicita guardar en archivo (parámetro P)
            var saveToFile = args.Contains("P", StringComparer.OrdinalIgnoreCase);

            // Mostrar estado
            AnsiConsole.MarkupLine("[cyan]Recopilando tareas activas de todos los proyectos...[/]");
            _loggerService.LogInfo("Recopilando datos de tareas activas");

            // Recopilar datos
            var projectData = await _dataCollectionService.CollectActiveTasksAsync();

            if (projectData.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No hay tareas activas para analizar.[/]");
                _loggerService.LogInfo("No se encontraron tareas activas");
                return;
            }

            AnsiConsole.MarkupLine($"[cyan]Se encontraron {projectData.Count} proyecto(s) con tareas activas.[/]");

            // Construir el prompt para la IA
            var prompt = ConstructAIPrompt(projectData);

            AnsiConsole.MarkupLine("[cyan]Enviando información a la IA para obtener sugerencias...[/]");
            _loggerService.LogInfo("Llamando a servicio de IA");

            // Obtener sugerencias de la IA
            var suggestions = await _aiService.GetSuggestionsAsync(prompt);

            // Mostrar resultado
            AnsiConsole.MarkupLine("[green]✓ Sugerencias obtenidas exitosamente[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[bold cyan]=== SUGERENCIAS DEL EQUIPO ===[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine(suggestions);
            AnsiConsole.WriteLine();

            // Guardar en archivo si se solicita
            if (saveToFile)
            {
                AnsiConsole.MarkupLine("[cyan]Guardando sugerencias en archivo...[/]");
                var filePath = await _markdownService.SaveSuggestionsAsync(suggestions);
                AnsiConsole.MarkupLine($"[green]✓ Sugerencias guardadas en: [bold]{filePath}[/][/]");
                _loggerService.LogInfo($"Sugerencias guardadas en archivo: {filePath}");
            }

            _loggerService.LogInfo("=== Comando SUGERENCIA completado exitosamente ===");
        }
        catch (InvalidOperationException ex)
        {
            AnsiConsole.MarkupLine($"[red]✗ Error de configuración: {ex.Message}[/]");
            _loggerService.LogError($"Error de configuración en comando sugerencia: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            AnsiConsole.MarkupLine($"[red]✗ Error de conexión con la IA: {ex.Message}[/]");
            _loggerService.LogError($"Error de conexión en comando sugerencia: {ex.Message}");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]✗ Error: {ex.Message}[/]");
            _loggerService.LogError($"Error en comando sugerencia: {ex.Message}", ex);
        }
    }

    private string ConstructAIPrompt(Dictionary<string, ProjectDataCollection> projectData)
    {
        var sb = new System.Text.StringBuilder();

        sb.AppendLine("Como experto project manager de un equipo de desarrollo de software, basándote en la siguiente información:");
        sb.AppendLine();

        foreach (var projectEntry in projectData)
        {
            var projectName = projectEntry.Key;
            var collection = projectEntry.Value;

            sb.AppendLine($"## Proyecto: {projectName}");
            sb.AppendLine();

            // Tareas generales
            if (collection.Tareas.Count > 0)
            {
                sb.AppendLine("### Tareas Generales:");
                foreach (var tarea in collection.Tareas)
                {
                    sb.AppendLine($"- **{tarea.Titulo}** (Prioridad: {tarea.Prioridad})");
                    if (!string.IsNullOrEmpty(tarea.Detalle))
                        sb.AppendLine($"  Detalle: {tarea.Detalle}");
                    if (!string.IsNullOrEmpty(tarea.FechaFIN))
                        sb.AppendLine($"  Fecha límite: {tarea.FechaFIN}");
                }
                sb.AppendLine();
            }

            // Tareas diarias
            if (collection.TareasDaily.Count > 0)
            {
                sb.AppendLine("### Tareas Diarias del Equipo:");
                foreach (var tareaDaily in collection.TareasDaily)
                {
                    sb.AppendLine($"- {tareaDaily.Titulo}");
                    if (!string.IsNullOrEmpty(tareaDaily.FechaFIN))
                        sb.AppendLine($"  Fecha límite: {tareaDaily.FechaFIN}");
                }
                sb.AppendLine();
            }

            // Impedimentos
            if (collection.Impedimentos.Count > 0)
            {
                sb.AppendLine("### Impedimentos del Equipo:");
                foreach (var impedimento in collection.Impedimentos)
                {
                    sb.AppendLine($"- **{impedimento.Impedimento}**: {impedimento.Explicacion}");
                    if (!string.IsNullOrEmpty(impedimento.FechaFIN))
                        sb.AppendLine($"  Fecha límite: {impedimento.FechaFIN}");
                }
                sb.AppendLine();
            }
        }

        sb.AppendLine("Elabora una serie de consejos, basándote en tu experiencia de años de gestión, agrupados por proyecto, para cada tarea, tareaDaily e ImpedimentDaily.");
        sb.AppendLine("Incluye también:");
        sb.AppendLine("1. Recordatorios de buenas prácticas en gestión de proyectos");
        sb.AppendLine("2. Avisos de fechas importantes:");
        sb.AppendLine("   - Final de mes: Necesario enviar las horas imputadas al superior");
        sb.AppendLine("   - Final de año: Completar las evaluaciones por desempeño (EVA)");
        sb.AppendLine();
        sb.AppendLine("Por favor, proporciona los consejos de manera clara, estructurada y práctica.");

        return sb.ToString();
    }
}
