namespace ParamChecker.ViewModels.Dialogs;

public partial class RenameViewModel : ObservableObject
{
    [ObservableProperty] private string _newTitle;

    public RenameViewModel(string currentTitle)
    {
        NewTitle = currentTitle;
    }

    public Action<bool> CloseAction { get; set; }

    [RelayCommand]
    private void Confirm()
    {
        CloseAction?.Invoke(true); // подтвердить
    }

    [RelayCommand]
    private void Cancel()
    {
        CloseAction?.Invoke(false); // отменить
    }
}