using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Autodesk.Revit.UI;
using WorkingSet.Models;
using Wpf.Ui.Appearance;
using RPToolsUI.Services;

namespace WorkingSet.ViewModels;

public class WorkingSetViewModel : BaseViewModel
{
    private readonly WorkingSetModel _model;
    private readonly ExternalCommandData _commandData;
    private readonly CreateWorksetsHandler _createWorksetsHandler;
    private readonly ExternalEvent _externalEvent;

    private string _selectedSection;
    private List<string> _sections;
    private bool _darkTheme;
    public bool DarkTheme
    {
        get => _darkTheme;
        set
        {
            var newTheme = value ? ApplicationTheme.Dark : ApplicationTheme.Light;
            ThemeWatcherService.ApplyTheme(newTheme);
            SetField(ref _darkTheme, value);
        }
    }

    public WorkingSetViewModel(ExternalCommandData commandData)
    {
        _commandData = commandData;
        _model = new WorkingSetModel(@"Y:\13-BIM (разработка)\06_Плагины\Рабочие наборы.xlsx");
        _createWorksetsHandler = new CreateWorksetsHandler();
        _externalEvent = ExternalEvent.Create(_createWorksetsHandler);

        LoadSections();
        CreateWorksetsCommand = new RelayCommand(CreateWorksets);
    }

    public List<string> Sections
    {
        get => _sections;
        set
        {
            _sections = value;
            OnPropertyChanged();
        }
    }

    public string SelectedSection
    {
        get => _selectedSection;
        set
        {
            _selectedSection = value;
            OnPropertyChanged();
        }
    }

    public ICommand CreateWorksetsCommand { get; }

    private void LoadSections()
    {
        Sections = _model.GetSections();
    }

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
}