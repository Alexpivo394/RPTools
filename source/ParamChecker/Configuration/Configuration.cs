using System.IO;
using System.Reflection;
using Autodesk.Revit.UI;
using Newtonsoft.Json;
using ParamChecker.Models.Filters;
using ParamChecker.Services;
using ParamChecker.ViewModels.Windows;

namespace ParamChecker.Configuration;

public class Configuration
{
    
    private readonly CategoryService _categoryService;
    
    private static readonly string DllPath = Assembly.GetExecutingAssembly().Location;
    private static readonly string DllDirectory = Path.GetDirectoryName(DllPath);

    private const string DirectoryName = "ПапкаСНастройками потом переименуй";
    private string _configFilePath = Path.Combine(DllDirectory, DirectoryName, "config.json");
    
    
    public FilterConfigViewModel FilterConfigViewModel { get; set; }
   
    
    public Configuration(CategoryService categoryService)
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
            FilterConfigViewModel = new FilterConfigViewModel(categoryService);
            SaveConfig();
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
        
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "{}");
        }
    }
    
    public static T LoadConfig<T>(string filePath) where T : class
    {
        try
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Файл конфигурации не найден", filePath);
            }
            
            var json = File.ReadAllText(filePath);
            var config = JsonConvert.DeserializeObject<T>(json);

            return config;
        }
        catch (FileNotFoundException ex)
        {
            TaskDialog.Show("File Not Found", ex.Message);
            return null;
        }
        catch (JsonException ex)
        {
            TaskDialog.Show("JSON Error", $"An error occurred while deserializing the JSON: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            TaskDialog.Show("Error", $"An unexpected error occurred: {ex.Message}");
            return null;
        }
    }
    
    public void SaveConfig()
    {
        try
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(_configFilePath, json);
        }
        catch (Exception ex)
        {
            TaskDialog.Show("Error", $"Ошибка с сохранние конфигурации: {ex.Message}");
        }
    }
}