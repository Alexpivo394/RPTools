using System.IO;
using Newtonsoft.Json;

namespace ParamToFinish.Services;

public sealed class ParamToFinishSettingsService
{
    private const string SettingsFileName = "settings.json";

    private static readonly JsonSerializerSettings SerializerSettings = new()
    {
        Formatting = Formatting.Indented
    };

    private readonly string _settingsPath;

    public ParamToFinishSettingsService()
    {
        var settingsDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "RPTools",
            "ParamToFinish");

        _settingsPath = Path.Combine(settingsDirectory, SettingsFileName);
    }

    public ParamToFinishSettings Load()
    {
        try
        {
            if (!File.Exists(_settingsPath))
                return new ParamToFinishSettings();

            var json = File.ReadAllText(_settingsPath);

            if (string.IsNullOrWhiteSpace(json))
                return new ParamToFinishSettings();

            return JsonConvert.DeserializeObject<ParamToFinishSettings>(json)
                   ?? new ParamToFinishSettings();
        }
        catch
        {
            return new ParamToFinishSettings();
        }
    }

    public void Save(ParamToFinishSettings settings)
    {
        try
        {
            var settingsDirectory = Path.GetDirectoryName(_settingsPath);

            if (!string.IsNullOrWhiteSpace(settingsDirectory))
                Directory.CreateDirectory(settingsDirectory);

            var json = JsonConvert.SerializeObject(settings, SerializerSettings);

            File.WriteAllText(_settingsPath, json);
        }
        catch
        {
            // Settings persistence should never interrupt the Revit command.
        }
    }
}
