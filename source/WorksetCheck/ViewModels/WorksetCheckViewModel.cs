using System.Collections.ObjectModel;
using RPToolsUI.Services;
using WorksetCheck.Models;
using Wpf.Ui.Appearance;

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
}