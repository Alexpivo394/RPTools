using System.Collections.ObjectModel;
using Nice3point.Revit.Extensions.Runtime;
using ParamToFinish.Models;
using ParamToFinish.Services;
using RPToolsUI.Models;
using RPToolsUI.Services;
using Wpf.Ui.Appearance;

namespace ParamToFinish.ViewModels;

public sealed partial class ParamToFinishViewModel : ObservableObject
{
    [ObservableProperty] private bool _darkTheme = true;
    [ObservableProperty] private bool _allModel = false;
    [ObservableProperty] private string _filter = "";
    public event EventHandler? CloseRequested;

    [ObservableProperty] private ObservableCollection<ParameterDescriptor>? _wallParameters;
    [ObservableProperty] private ParameterDescriptor? _selectedWallParameter;
    [ObservableProperty] private ParameterDescriptor? _selectedFinishParameter;
    private readonly IFinishParameterTransferService _transferService;

    public ParamToFinishViewModel(List<ParameterDescriptor>  wallParameters, IFinishParameterTransferService transferService)
    {
        WallParameters = new ObservableCollection<ParameterDescriptor>(wallParameters);
        _transferService = transferService;
    }
    
    partial void OnDarkThemeChanged(bool value)
    {
        var newTheme = value
            ? ApplicationTheme.Dark
            : ApplicationTheme.Light;

        ThemeWatcherService.ApplyTheme(newTheme);
    }

    [RelayCommand]
    private void Start()
    {
        if (SelectedWallParameter == null || SelectedFinishParameter == null)
        {
            var dial1 = ToadDialogService.Show(
                "Ошибка!",
                "Выберите параметры",
                DialogButtons.OK,
                DialogIcon.Error
            );
            
            return;
        }

        if (Filter.IsNullOrWhiteSpace())
        {
            var dial1 = ToadDialogService.Show(
                "Ошибка!",
                "Введите фильтр для поиска отделки",
                DialogButtons.OK,
                DialogIcon.Error
            );
            
            return;
        }
        
        _transferService.Transfer(SelectedWallParameter, SelectedFinishParameter, AllModel, Filter);
        
        var dial = ToadDialogService.Show(
            "Успех!",
            "Основание записано в отделку",
            DialogButtons.OK,
            DialogIcon.Info
        );

        if (dial == "OK")
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
