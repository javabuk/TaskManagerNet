using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Services;

public class MarkdownService : IMarkdownService
{
    private readonly ILoggerService _loggerService;

    public MarkdownService(ILoggerService loggerService)
    {
        _loggerService = loggerService;
    }

    public async Task<string> SaveSuggestionsAsync(string suggestions)
    {
        try
        {
            var fileName = GetSuggestionsFilePath();
            var directory = Path.GetDirectoryName(fileName);

            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            _loggerService.LogInfo($"[MarkdownService] Guardando sugerencias en archivo: {fileName}");

            await File.WriteAllTextAsync(fileName, suggestions, Encoding.UTF8);

            _loggerService.LogInfo($"[MarkdownService] Archivo guardado exitosamente");
            return fileName;
        }
        catch (Exception ex)
        {
            _loggerService.LogError($"[MarkdownService] Error al guardar archivo de sugerencias: {ex.Message}", ex);
            throw;
        }
    }

    public string GetSuggestionsFilePath()
    {
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        return $"{timestamp}_Sugerencias.md";
    }
}
