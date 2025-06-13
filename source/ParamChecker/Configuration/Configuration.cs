#nullable enable
using System.IO;
using System.Reflection;
using Autodesk.Revit.UI;
using Newtonsoft.Json;
using ParamChecker.Models.Filters;

namespace ParamChecker.Configuration;

public class Configuration
{
    private const string DirectoryName = "ParamCheckerSettings";

    private static readonly string DllPath = Assembly.GetExecutingAssembly().Location;
    private static readonly string DllDirectory = Path.GetDirectoryName(DllPath);

    private readonly string _configFilePath = Path.Combine(DllDirectory, DirectoryName, "config.json");


    public Configuration()
    {
        var directoryName = DirectoryName;
        var configName = "config.json";

        // path to dll
        var dllPath = Assembly.GetExecutingAssembly().Location;
        CreateDir(dllPath, directoryName);

        // path to dll`s directory
        var dllDir = Path.GetDirectoryName(dllPath);

        // path to cfg`s directory
        var pathCfg = Path.Combine(dllDir, directoryName, configName);

        // path to cfg`s

        var dirCfg = Path.Combine(dllDir, directoryName);

        if (!File.Exists(pathCfg))
        {
            CreateEmptyJsonFile(dirCfg, configName);
        }
    }
    

    private static void CreateDir(string dllPath, string directoryName)
    {
        var dllDirectory = Path.GetDirectoryName(dllPath);
        var configDirectoryPath = Path.Combine(dllDirectory, directoryName);
        if (!Directory.Exists(configDirectoryPath))
        {
            var x = Directory.CreateDirectory(configDirectoryPath);
        }
    }

    private static void CreateEmptyJsonFile(string directoryPath, string fileName)
    {
        var filePath = Path.Combine(directoryPath, fileName);

        if (!File.Exists(filePath)) File.WriteAllText(filePath, "{}");
    }

    public AppSettings? LoadSettings()
    {
        try
        {
            if (!File.Exists(_configFilePath)) return new AppSettings(); // default

            var json = File.ReadAllText(_configFilePath);
            return JsonConvert.DeserializeObject<AppSettings>(json);
        }
        catch (Exception ex)
        {
            TaskDialog.Show("Error", $"Ошибка при загрузке настроек: {ex.Message}");
            return null;
        }
    }

    public void SaveSettings(AppSettings settings)
    {
        try
        {
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(_configFilePath, json);
        }
        catch (Exception ex)
        {
            TaskDialog.Show("Error", $"Ошибка при сохранении настроек: {ex.Message}");
        }
    }
}