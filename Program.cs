using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
            var commandHandler = services.GetRequiredService<ICommandHandler>();
            
            if (args.Length == 0)
            {
                ShowHelp();
                return;
            }

            await commandHandler.HandleAsync(args);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            Environment.Exit(1);
        }
    }

    static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Configuration
        var config = new AppConfiguration
        {
            DatabasePath = "taskmanager.db",
            PreviousDaysForReport = 3
        };
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

        // Command Handler
        services.AddScoped<ICommandHandler, CommandHandler>();

        return services.BuildServiceProvider();
    }

    static void ShowHelp()
    {
        Console.WriteLine("\n=== TaskManager - Gestor de Tareas ===\n");
        Console.WriteLine("PROYECTOS:");
        Console.WriteLine("  proyecto crear --nombre <name> [--descripcion <desc>] [--fecha-inicio <date>] [--activo <0|1>] [--tiene-daily <0|1>]");
        Console.WriteLine("  proyecto listar");
        Console.WriteLine("  proyecto modificar --id <id> [--nombre <name>] [--descripcion <desc>] [--activo <0|1>]\n");
        
        Console.WriteLine("RECURSOS:");
        Console.WriteLine("  recurso crear --nombre <name> [--activo <0|1>]");
        Console.WriteLine("  recurso listar");
        Console.WriteLine("  recurso modificar --id <id> [--nombre <name>] [--activo <0|1>]\n");
        
        Console.WriteLine("RECURSOS PROYECTO:");
        Console.WriteLine("  recurso-proyecto crear --id-proyecto <id> --id-recurso <id>");
        Console.WriteLine("  recurso-proyecto listar [--id-proyecto <id>] [--id-recurso <id>]");
        Console.WriteLine("  recurso-proyecto modificar --id <id>\n");
        
        Console.WriteLine("TAREAS:");
        Console.WriteLine("  tarea crear --id-proyecto <id> --titulo <title> [--detalle <detail>] [--prioridad <Alta|Media|Baja>]");
        Console.WriteLine("  tarea listar [--id-proyecto <id>] [--titulo <title>] [--prioridad <Alta|Media|Baja>] [--activo <0|1>]");
        Console.WriteLine("  tarea modificar --id <id> [--titulo <title>] [--fecha-fin <date>] [--prioridad <priority>]\n");
        
        Console.WriteLine("RECURSOS TAREA:");
        Console.WriteLine("  recurso-tarea crear --id-tarea <id> --id-recurso <id>");
        Console.WriteLine("  recurso-tarea listar [--id-tarea <id>] [--id-recurso <id>]");
        Console.WriteLine("  recurso-tarea modificar --id <id>\n");
        
        Console.WriteLine("TAREAS DAILY:");
        Console.WriteLine("  tarea-daily crear --id-proyecto <id> --id-recurso <id> --titulo <title>");
        Console.WriteLine("  tarea-daily listar [--id-proyecto <id>] [--id-recurso <id>] [--titulo <title>]");
        Console.WriteLine("  tarea-daily modificar --id <id> [--titulo <title>] [--fecha-fin <date>]\n");
        
        Console.WriteLine("IMPEDIMENTOS DAILY:");
        Console.WriteLine("  impedimento-daily crear --id-proyecto <id> --id-recurso <id> --impedimento <text> --explicacion <text>");
        Console.WriteLine("  impedimento-daily listar [--id-proyecto <id>] [--id-recurso <id>]");
        Console.WriteLine("  impedimento-daily modificar --id <id> [--activo <0|1>]\n");
        
        Console.WriteLine("REPORTES:");
        Console.WriteLine("  reporte generar [--fecha <dd/MM/yyyy>] [--id-proyecto <id>] [--nombre-proyecto <name>]\n");
    }
}
