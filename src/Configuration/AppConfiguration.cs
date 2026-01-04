namespace TaskManager.Configuration;

public class AppConfiguration
{
    public string DatabasePath { get; set; } = "taskmanager.db";
    public int PreviousDaysForReport { get; set; } = 3;
    public string LogFilePath { get; set; } = "taskmanager.log";
    public string IndicacionesPath { get; set; } = "Indicaciones.md";
}

/// <summary>
/// Configuración de un proveedor de IA individual
/// </summary>
public class AIServiceConfiguration
{
    public required string Id { get; set; }
    public required string Model { get; set; }
    public required string ApiUrl { get; set; }
    public required string ApiKey { get; set; }
    public decimal Temperature { get; set; } = 1.0m;
    public int MaxCompletionTokens { get; set; } = 1024;
    public decimal TopP { get; set; } = 1.0m;
    public string? ReasoningEffort { get; set; }
    public string? Stop { get; set; }
    
    /// <summary>
    /// Parámetros personalizados específicos de cada modelo/proveedor
    /// Ejemplo: para groq/compound, puede contener "compound_custom" con herramientas
    /// </summary>
    public Dictionary<string, object> CustomParams { get; set; } = new();
}
