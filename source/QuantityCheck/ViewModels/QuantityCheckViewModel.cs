using QuantityCheck.Configuration;
using RPToolsUI.Models;
using RPToolsUI.Services;
using Wpf.Ui.Appearance;
using Settings = QuantityCheck.Configuration.Settings;

namespace QuantityCheck.ViewModels;

public partial class QuantityCheckViewModel : ObservableObject
{
    [ObservableProperty] private bool _darkTheme = true;
    [ObservableProperty] private string _parameterName = "";
    
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
        string? dildo = ToadDialogService.Show(
            "Диалог",
            "Тестовое диалоговое окно?",
            DialogButtons.RetryAbort,
            DialogIcon.Warning
        );
    }
} 