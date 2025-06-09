using System.Collections.ObjectModel;
using ParamChecker.Models.Filters;

namespace ParamChecker.ViewModels.Conditions;

public partial class GroupConditionViewModel : ConditionViewModelBase
{
    [ObservableProperty]
    private FilterParameterLogic groupLogic;

    [ObservableProperty]
    private ObservableCollection<ConditionViewModelBase> children = new();

    public Action<GroupConditionViewModel> RemoveGroupRequested { get; set; }
    public GroupConditionViewModel()
    {

    }

    [RelayCommand]
    private void Add()
    {
        var simple = new SimpleConditionViewModel
        {
            RemoveSimpleRequested = (vm) => Children.Remove(vm)
        };
        Children.Add(simple);
    }

    [RelayCommand]
    private void Remove()
    {
        RemoveGroupRequested?.Invoke(this);
    }
    
}
