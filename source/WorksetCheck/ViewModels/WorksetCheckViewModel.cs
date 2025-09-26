using System.Collections.ObjectModel;
using RPToolsUI.Services;
using WorksetCheck.Models;
using WorksetCheck.Services;
using Wpf.Ui.Appearance;
using Settings = WorksetCheck.Configuration.Settings;

namespace WorksetCheck.ViewModels;

public sealed partial class WorksetCheckViewModel : ObservableObject
{
    [ObservableProperty] private bool _darkTheme = true;
    [ObservableProperty] private ObservableCollection<ExportModel> _models = new();
    
    partial void OnDarkThemeChanged(bool value)
    {
        var newTheme = value ? ApplicationTheme.Dark : ApplicationTheme.Light;
        ThemeWatcherService.ApplyTheme(newTheme);
    }
    
    [RelayCommand]
    private void AddModel()
    {
        Models.Add(new ExportModel());
    }

    [RelayCommand]
    private void RemoveModel(ExportModel model)
    {
        Models.Remove(model);
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
    public void Start()
    {

    }
}