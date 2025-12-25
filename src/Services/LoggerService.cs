using System;
using System.IO;

namespace TaskManager.Services;

/// <summary>
/// Servicio de logging que registra todas las operaciones y errores
/// </summary>
public interface ILoggerService
{
    void LogInfo(string message);
    void LogWarning(string message);
    void LogError(string message, Exception? ex = null);
    void LogCommand(string command, string[] args);
    void LogSuccess(string message);
}

public class LoggerService : ILoggerService
{
    private readonly string _logFilePath;
    private readonly object _lockObject = new object();

    public LoggerService(string logFilePath)
    {
        _logFilePath = logFilePath;
        
        // Asegurar que el directorio existe
        var directory = Path.GetDirectoryName(_logFilePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public void LogInfo(string message)
    {
        WriteLog("INFO", message);
    }

    public void LogWarning(string message)
    {
        WriteLog("WARNING", message);
    }

    public void LogError(string message, Exception? ex = null)
    {
        var fullMessage = ex != null 
            ? $"{message} | Exception: {ex.GetType().Name} - {ex.Message} | StackTrace: {ex.StackTrace}"
            : message;
        WriteLog("ERROR", fullMessage);
    }

    public void LogCommand(string command, string[] args)
    {
        var argsStr = args.Length > 0 ? string.Join(" ", args) : "[sin argumentos]";
        WriteLog("COMMAND", $"{command} {argsStr}");
    }

    public void LogSuccess(string message)
    {
        WriteLog("SUCCESS", message);
    }

    private void WriteLog(string level, string message)
    {
        lock (_lockObject)
        {
            try
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                var logEntry = $"[{timestamp}] [{level}] {message}";

                File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                // Si falla el logging, intentamos escribir al menos en consola
                Console.Error.WriteLine($"Error al escribir log: {ex.Message}");
            }
        }
    }
}
