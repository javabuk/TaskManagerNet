using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TaskManager.Configuration;
using TaskManager.Data;
using TaskManager.Repositories;
using TaskManager.Services;
using TaskManager.Commands;
using Spectre.Console;

namespace TaskManager;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            var services = ConfigureServices();
            var logger = services.GetRequiredService<ILoggerService>();
            
            logger.LogInfo("=== TaskManager iniciado ===");
            logger.LogCommand(args.Length > 0 ? args[0] : "[sin comando]", args.Length > 1 ? args.Skip(1).ToArray() : Array.Empty<string>());
            
            if (args.Length == 0 || args[0] == "--help" || args[0] == "-h" || args[0] == "help")
            {
                ShowHelp();
                logger.LogInfo("Ayuda mostrada");
                return;
            }

            var commandHandler = services.GetRequiredService<ICommandHandler>();
            await commandHandler.HandleAsync(args);
            
            logger.LogInfo("=== Ejecución completada exitosamente ===");
        }
        catch (Exception ex)
        {
            var services = ConfigureServices();
            var logger = services.GetRequiredService<ILoggerService>();
            logger.LogError($"Error fatal: {ex.Message}", ex);
            
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            Environment.Exit(1);
        }
    }

    static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Configuration from appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .Build();

        var config = configuration.GetSection("AppConfiguration").Get<AppConfiguration>() 
            ?? new AppConfiguration();
        services.AddSingleton(config);

        // Database
        services.AddDbContext<TaskManagerDbContext>(options =>
            options.UseSqlite($"Data Source={config.DatabasePath}"));

        // Repositories
        services.AddScoped<IProyectoRepository, ProyectoRepository>();
        services.AddScoped<IRecursoRepository, RecursoRepository>();
        services.AddScoped<IRecursoProyectoRepository, RecursoProyectoRepository>();
        services.AddScoped<ITareaRepository, TareaRepository>();
        services.AddScoped<IRecursoTareaRepository, RecursoTareaRepository>();
        services.AddScoped<ITareaDailyRepository, TareaDailyRepository>();
        services.AddScoped<IImpedimentoDailyRepository, ImpedimentoDailyRepository>();

        // Services
        services.AddScoped<IProyectoService, ProyectoService>();
        services.AddScoped<IRecursoService, RecursoService>();
        services.AddScoped<IRecursoProyectoService, RecursoProyectoService>();
        services.AddScoped<ITareaService, TareaService>();
        services.AddScoped<IRecursoTareaService, RecursoTareaService>();
        services.AddScoped<ITareaDailyService, TareaDailyService>();
        services.AddScoped<IImpedimentoDailyService, ImpedimentoDailyService>();
        services.AddScoped<IReportService, ReportService>();

        // Logger Service
        services.AddSingleton<ILoggerService>(new LoggerService(config.LogFilePath));

        // Command Handler
        services.AddScoped<ICommandHandler, CommandHandler>();

        return services.BuildServiceProvider();
    }

    static void ShowHelp()
    {
        var helpText = @"
╔════════════════════════════════════════════════════════════════════════════╗
║                        TaskManager - Gestor de Tareas                      ║
║                          Referencia de Comandos                            ║
╚════════════════════════════════════════════════════════════════════════════╝

INFORMACIÓN:
  help, --help, -h              Muestra esta ayuda

PROYECTOS:
  proyecto crear                Crear un nuevo proyecto
    --nombre <texto>            Nombre del proyecto (REQUERIDO)
    --descripcion <texto>       Descripción (opcional)
    --fecha-inicio <dd/MM/yyyy> Fecha de inicio (opcional)
    --activo <0|1>              Estado activo (opcional, default: 1)
    --tiene-daily <0|1>         Tiene Daily tracking (opcional, default: 0)
  
  proyecto listar               Listar todos los proyectos
  
  proyecto modificar            Modificar un proyecto
    --id <número>               ID del proyecto (REQUERIDO)
    --nombre <texto>            Nuevo nombre (opcional)
    --descripcion <texto>       Nueva descripción (opcional)
    --activo <0|1>              Nuevo estado (opcional)

RECURSOS:
  recurso crear                 Crear un nuevo recurso
    --nombre <texto>            Nombre del recurso (REQUERIDO)
    --activo <0|1>              Estado activo (opcional, default: 1)
  
  recurso listar                Listar todos los recursos
  
  recurso modificar             Modificar un recurso
    --id <número>               ID del recurso (REQUERIDO)
    --nombre <texto>            Nuevo nombre (opcional)
    --activo <0|1>              Nuevo estado (opcional)

RECURSOS EN PROYECTO:
  recurso-proyecto crear        Asignar recurso a proyecto
    --id-proyecto <número>      ID del proyecto (REQUERIDO)
    --id-recurso <número>       ID del recurso (REQUERIDO)
  
  recurso-proyecto listar       Listar asignaciones
    --id-proyecto <número>      Filtrar por proyecto (opcional)
    --id-recurso <número>       Filtrar por recurso (opcional)
  
  recurso-proyecto modificar    Modificar asignación
    --id <número>               ID de la asignación (REQUERIDO)

TAREAS:
  tarea crear                   Crear una nueva tarea
    --id-proyecto <número>      ID del proyecto (REQUERIDO)
    --titulo <texto>            Título de la tarea (REQUERIDO)
    --detalle <texto>           Detalle adicional (opcional)
    --prioridad <Alta|Media|Baja> Prioridad (opcional, default: Media)
  
  tarea listar                  Listar tareas (con filtros opcionales)
    --id-proyecto <número>      Filtrar por proyecto
    --titulo <texto>            Filtrar por título (búsqueda parcial)
    --prioridad <Alta|Media|Baja> Filtrar por prioridad
    --activo <0|1>              Filtrar por estado
  
  tarea modificar               Modificar una tarea
    --id <número>               ID de la tarea (REQUERIDO)
    --titulo <texto>            Nuevo título (opcional)
    --fecha-fin <dd/MM/yyyy>    Fecha de finalización (opcional)
    --prioridad <prioridad>     Nueva prioridad (opcional)
    --activo <0|1>              Nuevo estado (opcional)

RECURSOS EN TAREA:
  recurso-tarea crear           Asignar recurso a tarea
    --id-tarea <número>         ID de la tarea (REQUERIDO)
    --id-recurso <número>       ID del recurso (REQUERIDO)
  
  recurso-tarea listar          Listar asignaciones
    --id-tarea <número>         Filtrar por tarea
    --id-recurso <número>       Filtrar por recurso
  
  recurso-tarea modificar       Modificar asignación
    --id <número>               ID de la asignación (REQUERIDO)

TAREAS DIARIAS:
  tarea-daily crear             Crear tarea diaria
    --id-proyecto <número>      ID del proyecto (REQUERIDO)
    --id-recurso <número>       ID del recurso (REQUERIDO)
    --titulo <texto>            Título de la tarea (REQUERIDO)
  
  tarea-daily listar            Listar tareas diarias
    --id-proyecto <número>      Filtrar por proyecto
    --id-recurso <número>       Filtrar por recurso
    --titulo <texto>            Filtrar por título
  
  tarea-daily modificar         Modificar tarea diaria
    --id <número>               ID de la tarea (REQUERIDO)
    --titulo <texto>            Nuevo título (opcional)
    --fecha-fin <dd/MM/yyyy>    Fecha de finalización (opcional)

IMPEDIMENTOS DIARIOS:
  impedimento-daily crear       Crear impedimento diario
    --id-proyecto <número>      ID del proyecto (REQUERIDO)
    --id-recurso <número>       ID del recurso (REQUERIDO)
    --impedimento <texto>       Descripción del impedimento (REQUERIDO)
    --explicacion <texto>       Explicación adicional (REQUERIDO)
  
  impedimento-daily listar      Listar impedimentos
    --id-proyecto <número>      Filtrar por proyecto
    --id-recurso <número>       Filtrar por recurso
    --activo <0|1>              Filtrar por estado
  
  impedimento-daily modificar   Modificar impedimento
    --id <número>               ID del impedimento (REQUERIDO)
    --activo <0|1>              Nuevo estado (opcional)

REPORTES:
  reporte generar               Generar reporte en Markdown
    --fecha <dd/MM/yyyy>        Fecha específica (opcional)
    --id-proyecto <número>      Filtrar por proyecto (opcional)
    --nombre-proyecto <texto>   Filtrar por nombre (opcional)

EJEMPLOS DE USO:
  
  # Crear un proyecto
  TaskManager.exe proyecto crear --nombre ""Mi Proyecto"" --tiene-daily 1

  # Listar proyectos
  TaskManager.exe proyecto listar

  # Crear una tarea en el proyecto 1
  TaskManager.exe tarea crear --id-proyecto 1 --titulo ""Mi Tarea"" --prioridad ""Alta""

  # Listar tareas de alta prioridad
  TaskManager.exe tarea listar --prioridad ""Alta""

  # Generar reporte
  TaskManager.exe reporte generar

  # Generar reporte de un proyecto
  TaskManager.exe reporte generar --id-proyecto 1

NOTAS:
  - Las fechas deben estar en formato dd/MM/yyyy
  - Los valores 0/1 para booleanos: 0=No/Inactivo, 1=Sí/Activo
  - Los textos con espacios deben ir entre comillas
  - Consulta el archivo COMANDOS.md para más información

";
        AnsiConsole.Write(new Spectre.Console.Panel(helpText)
        {
            Border = Spectre.Console.BoxBorder.Double
        });
    }
}
