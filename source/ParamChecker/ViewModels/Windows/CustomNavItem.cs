using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using ParamChecker.Messaging;
using ParamChecker.ViewModels.Windows;
using ParamChecker.Views.Dialogs;

namespace ParamChecker.ViewModels.Windows;

public partial class CustomNavItem : ObservableObject
{
    [ObservableProperty]
    private string title;
    public Page Page { get; }
    public object ViewModel { get; }
    
    public object ViewModelInstance { get; }
    
    private readonly Action<CustomNavItem> _removeCallback;
    public Action<Page> OnNavigate;

    public IRelayCommand RemoveCommand { get; }
    public IRelayCommand RenameCommand { get; }
    public IRelayCommand SelectCommand { get; }
    
    public bool IsChecked { get; set; }
    public CustomNavItem(string title, Page page, object viewModel, Action<CustomNavItem> removeCallback)
    {
        Title = title;
        Page = page;
        ViewModelInstance = viewModel;
        _removeCallback = removeCallback;

        RemoveCommand = new RelayCommand(() => removeCallback?.Invoke(this));
        RenameCommand = new RelayCommand(() =>
        {
            var dialog = new Rename(Title);
            var result = dialog.ShowDialog();
            if (result == true)
            {
                Title =  dialog.Result;
            }
        });
        SelectCommand = new RelayCommand(() =>
        {
            OnNavigate?.Invoke(Page);
        });
    }
}
