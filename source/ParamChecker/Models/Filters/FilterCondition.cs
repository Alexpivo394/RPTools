using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace ParamChecker.Models.Filters;

public class FilterCondition : INotifyPropertyChanged
{
    public FilterConditionType Type { get; set; }
    public ObservableCollection<ParameterCondition> Conditions { get; } = new();
    public ObservableCollection<FilterCondition> GroupConditions { get; } = new();

    private ICommand _removeConditionCommand;
    private ICommand _addSimpleConditionCommand;

    public ICommand RemoveConditionCommand => _removeConditionCommand ??= new RelayCommand(RemoveCondition);
    public ICommand AddSimpleConditionCommand => _addSimpleConditionCommand ??= new RelayCommand(AddSimpleCondition);

    private void RemoveCondition()
    {
        // This will be implemented in ViewModel
    }

    private void AddSimpleCondition()
    {
        // This will be implemented in ViewModel
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}