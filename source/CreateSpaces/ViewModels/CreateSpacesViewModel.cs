using System.Collections.ObjectModel;
using Autodesk.Revit.DB.Architecture;
using CreateSpaces.Models;
using CreateSpaces.Services;
using RPToolsUI.Services;
using Wpf.Ui.Appearance;

namespace CreateSpaces.ViewModels;

public partial class CreateSpacesViewModel : ObservableObject
{
    [ObservableProperty] private bool _darkTheme = true;
    [ObservableProperty] private bool _createSpaces = true;
    [ObservableProperty] private ObservableCollection<ParameterMappingModel> _models = new();
    public ObservableCollection<LinkDescriptor> LinkedModels { get; set; }
    [ObservableProperty] private LinkDescriptor _selectedLink;
    private GetParameterService _getParameterService;
    private RevitRoomProvider _roomProvider;
    
    public CreateSpacesViewModel(IReadOnlyList<LinkDescriptor> linkedModels, GetParameterService getParameterService,  RevitRoomProvider roomProvider)
    {
        LinkedModels = new ObservableCollection<LinkDescriptor>(linkedModels);
        Models = new ObservableCollection<ParameterMappingModel>();
        _getParameterService = getParameterService;
        _roomProvider = roomProvider;
    }

    partial void OnDarkThemeChanged(bool value)
    {
        var newTheme = value ? ApplicationTheme.Dark : ApplicationTheme.Light;
        ThemeWatcherService.ApplyTheme(newTheme);
    }

    partial void OnSelectedLinkChanged(LinkDescriptor? value)
    {
        if (value == null)
            return;

        LoadRoomParameters(value);
    }
    
    private void LoadRoomParameters(LinkDescriptor link)
    {
        var roomParameters = _getParameterService.GetFromRoom(_roomProvider.GetRoomsFromLink(link).FirstOrDefault());
        var spaceParameters = _getParameterService.GetFromTemporarySpace();
        
        UpdateModels(spaceParameters, roomParameters);
    }
    
    public void UpdateModels(
        IEnumerable<ParameterDescriptor> spaceParameters,
        IEnumerable<ParameterDescriptor> roomParameters)
    {
        Models = new ObservableCollection<ParameterMappingModel>(
            spaceParameters.Select(sp =>
                new ParameterMappingModel(sp, roomParameters))
        );
    }
    
}