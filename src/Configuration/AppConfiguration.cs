namespace TaskManager.Configuration;

public class AppConfiguration
{
    public string DatabasePath { get; set; } = "taskmanager.db";
    public int PreviousDaysForReport { get; set; } = 3;
    public string LogFilePath { get; set; } = "taskmanager.log";
}
