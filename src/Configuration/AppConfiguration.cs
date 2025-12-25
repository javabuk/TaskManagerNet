namespace TaskManager.Configuration;

public class AppConfiguration
{
    public string DatabasePath { get; set; } = "taskmanager.db";
    public int PreviousDaysForReport { get; set; } = 3;
}
