using System.Collections.ObjectModel;
using CreateSpaces.Models;

namespace CreateSpaces.ViewModels;

public partial class ParameterMappingModel : ObservableObject
{
    // Параметр ПРОСТРАНСТВА (цель)
    public ParameterDescriptor SpaceParameter { get; }

    // Параметры ПОМЕЩЕНИЯ (источники)
    public ObservableCollection<ParameterDescriptor> RoomParameters { get; }

    [ObservableProperty]
    private ParameterDescriptor? _selectedRoomParameter;

    public ParameterMappingModel(
        ParameterDescriptor spaceParameter,
        IEnumerable<ParameterDescriptor> roomParameters)
    {
        SpaceParameter = spaceParameter;
        RoomParameters = new ObservableCollection<ParameterDescriptor>(roomParameters);
    }
}