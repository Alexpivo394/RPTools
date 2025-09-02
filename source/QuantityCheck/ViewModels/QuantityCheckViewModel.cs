using QuantityCheck.Configuration;
using QuantityCheck.Models;
using QuantityCheck.Services;
using RPToolsUI.Models;
using RPToolsUI.Services;
using Wpf.Ui.Appearance;
using Settings = QuantityCheck.Configuration.Settings;

namespace QuantityCheck.ViewModels;

public partial class QuantityCheckViewModel(Logger? logger, Document doc) : ObservableObject
{
    [ObservableProperty] private bool _darkTheme = true;
    [ObservableProperty] private string? _parameterName = "";
    // private readonly Logger _logger = logger;
    // private readonly Document _doc = doc;
    private readonly QuantityProcessor _processor = new QuantityProcessor(doc, logger);

    partial void OnDarkThemeChanged(bool value)
    {
        var newTheme = value ? ApplicationTheme.Dark : ApplicationTheme.Light;
        ThemeWatcherService.ApplyTheme(newTheme);
    }
    
    public void LoadFromSettings(Settings settings)
    {
        DarkTheme = settings.DarkTheme;
    }

    public Settings ToSettings()
    {
        return new Settings
        {
            DarkTheme = DarkTheme
        };
    }

    [RelayCommand]
    private void WriteQuantity()
    {
        _processor.Process(ParameterName);
        
        
        string dildo = ToadDialogService.Show(
            "Готово",
            "Длина заполнена",
            DialogButtons.OK,
            DialogIcon.Info
        );
    }
} 