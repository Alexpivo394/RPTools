using System.Windows;
using Autodesk.Revit.UI;
using Microsoft.Win32;
using WorkingSet.Models;
using Wpf.Ui.Appearance;
using RPToolsUI.Services;
using Settings = WorkingSet.Configuration.Settings;

namespace WorkingSet.ViewModels;

public partial class WorkingSetViewModel : ObservableObject
{
    private WorkingSetModel _model;
    private readonly ExternalCommandData _commandData;
    CreateWorksetsHandler _createWorksetsHandler;
    private ExternalEvent _externalEvent;

    [ObservableProperty] private string _selectedSection;
    [ObservableProperty] private List<string> _sections;
    [ObservableProperty] private bool _darkTheme = true;
    [ObservableProperty] private string _excelFilePath;
    
    partial void OnDarkThemeChanged(bool value)
    {
        var newTheme = value ? ApplicationTheme.Dark : ApplicationTheme.Light;
        ThemeWatcherService.ApplyTheme(newTheme);
    }

    partial void OnExcelFilePathChanged(string value)
    {
        _model = new WorkingSetModel(value);
        _createWorksetsHandler = new CreateWorksetsHandler();
        _externalEvent = ExternalEvent.Create(_createWorksetsHandler);

        LoadSections();
    }

    public WorkingSetViewModel(ExternalCommandData commandData)
    {
        _commandData = commandData;
    }
    private void LoadSections()
    {
        Sections = _model.GetSections();
    }

    [RelayCommand]
    private void CreateWorksets()
    {
        if (string.IsNullOrEmpty(SelectedSection))
        {
            MessageBox.Show("Пожалуйста, выберите раздел.");
            return;
        }

        var worksets = _model.GetWorksetsFromSection(SelectedSection);

        _createWorksetsHandler.Worksets = worksets;
        _createWorksetsHandler.CommandData = _commandData;
        _externalEvent.Raise();

        MessageBox.Show("Рабочие наборы созданы успешно!");
    }
    
    [RelayCommand]
    private void SelectExcelFilePath(object parameter)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Excel Files (*.xlsx; *.xls)|*.xlsx;*.xls|All files (*.*)|*.*",
            Title = "Выберите Excel файл",
            DefaultExt = ".xlsx",
            CheckFileExists = true,
            CheckPathExists = true,
            Multiselect = false,
            RestoreDirectory = true
        };

        if (openFileDialog.ShowDialog() == true) ExcelFilePath = openFileDialog.FileName;
    }

    public void LoadFromSettings(Settings settings)
    {
        DarkTheme = settings.DarkTheme;
        ExcelFilePath = settings.ExcelFilePath;
    }

    public Settings ToSettings()
    {
        return new Settings
        {
            DarkTheme = DarkTheme,
            ExcelFilePath = ExcelFilePath
        };
    }
}