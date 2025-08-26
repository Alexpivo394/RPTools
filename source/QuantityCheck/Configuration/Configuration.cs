using System.IO;
using System.Reflection;
using Autodesk.Revit.UI;
using Newtonsoft.Json;
using Formatting = System.Xml.Formatting;

namespace QuantityCheck.Configuration;

public class Configuration
{
    private const string DirectoryName = "QuantityCheckSettings";

    private static readonly string DllPath = Assembly.GetExecutingAssembly().Location;
    private static readonly string? DllDirectory = Path.GetDirectoryName(DllPath);

    private readonly string _configFilePath = Path.Combine(DllDirectory ?? string.Empty, DirectoryName, "config.json");


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
        if (dllDir != null)
        {
            var pathCfg = Path.Combine(dllDir, directoryName, configName);

            // path to cfg`s

            var dirCfg = Path.Combine(dllDir, directoryName);

            if (!File.Exists(pathCfg))
            {
                CreateEmptyJsonFile(dirCfg, configName);
            }
        }
    }
    

    private static void CreateDir(string dllPath, string directoryName)
    {
        var dllDirectory = Path.GetDirectoryName(dllPath);
        if (dllDirectory != null)
        {
            var configDirectoryPath = Path.Combine(dllDirectory, directoryName);
            if (!Directory.Exists(configDirectoryPath))
            {
                var x = Directory.CreateDirectory(configDirectoryPath);
            }
        }
    }

    private static void CreateEmptyJsonFile(string directoryPath, string fileName)
    {
        var filePath = Path.Combine(directoryPath, fileName);

        if (!File.Exists(filePath)) File.WriteAllText(filePath, "{}");
    }

    public Settings? LoadSettings()
    {
        try
        {
            if (!File.Exists(_configFilePath)) return new Settings(); // default

            var json = File.ReadAllText(_configFilePath);
            return JsonConvert.DeserializeObject<Settings>(json);
        }
        catch (Exception ex)
        {
            TaskDialog.Show("Error", $"Ошибка при загрузке настроек: {ex.Message}");
            return null;
        }
    }

    public void SaveSettings(Settings settings)
    {
        try
        {
            var json = JsonConvert.SerializeObject(settings, (Newtonsoft.Json.Formatting)Formatting.Indented);
            File.WriteAllText(_configFilePath, json);
        }
        catch (Exception ex)
        {
            TaskDialog.Show("Error", $"Ошибка при сохранении настроек: {ex.Message}");
        }
    }
}