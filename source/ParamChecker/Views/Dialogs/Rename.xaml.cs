using ParamChecker.ViewModels.Dialogs;
using RPToolsUI.Services;
using Wpf.Ui.Controls;

namespace ParamChecker.Views.Dialogs;

public partial class Rename : FluentWindow
{
    public Rename(string currentTitle)
    {
        InitializeComponent();
        ThemeWatcherService.Watch(this);

        ViewModel = new RenameViewModel(currentTitle);
        DataContext = ViewModel;

        ViewModel.CloseAction = result =>
        {
            DialogResult = result;
            Close();
        };
    }

    public RenameViewModel ViewModel { get; }

    public string Result => ViewModel.NewTitle;
}