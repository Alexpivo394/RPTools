using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using Newtonsoft.Json;
using RPToolsUI.Models;
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
    private WorksetCheckModel _model;

    public WorksetCheckViewModel(WorksetCheckModel model)
    {
        _model = model;
        
        Models.CollectionChanged += (_, _) =>
        {
            if (Models.Count == 50)
            {
                ToadDialogService.Show(
                    "Пиздец!",
                    "Ты че еблан?",
                    DialogButtons.OK,
                    DialogIcon.Error
                );
            }
        };
    }

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
        foreach (var model in Models)
        {
            try
            {
                _model.CheckWorksets(model.ServerPath, model.WorksetForWorkSetsName, model.WorksetForAxesName, model.WorksetForHolesName);
            }
            catch (Exception e)
            { 
                Debug.WriteLine(e);
                throw;
            }
        }
        var dialog = ToadDialogService.Show(
            "Успех!",
            "Отчеты успешно сохранены на рабочий стол.",
            DialogButtons.OK,
            DialogIcon.Info
        );
        
    }
    
    [RelayCommand]
    private void ExportModels()
    {
        var dlg = new SaveFileDialog
        {
            Filter = "JSON files (*.json)|*.json",
            FileName = "models.json"
        };

        if (dlg.ShowDialog() == true)
        {
            var json = JsonConvert.SerializeObject(Models, Formatting.Indented);
            File.WriteAllText(dlg.FileName, json);
        }
    }

    [RelayCommand]
    private void ImportModels()
    {
        var dlg = new OpenFileDialog
        {
            Filter = "JSON files (*.json)|*.json"
        };

        if (dlg.ShowDialog() == true)
        {
            var json = File.ReadAllText(dlg.FileName);
            var imported = JsonConvert.DeserializeObject<List<ExportModel>>(json);

            if (imported != null)
            {
                Models.Clear();
                foreach (var model in imported)
                    Models.Add(model);
            }
        }
    }
}