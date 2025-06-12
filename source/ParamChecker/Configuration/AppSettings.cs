namespace ParamChecker.Configuration;

public class AppSettings
{
    public bool IsDarkTheme { get; set; } = true;
    public string LogFilePath { get; set; } = "";
    public string ReportFilePath { get; set; } = "";
    public bool UpdateGeneralReport { get; set; } = false;
}
