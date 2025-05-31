using System.Windows;
using ParamChecker.ViewModels.Dialogs;
using RPToolsUI.Services;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace ParamChecker.Views.Dialogs;

public partial class Rename : FluentWindow
{
    public RenameViewModel ViewModel { get; }

    public Rename(string currentTitle)
    {
        InitializeComponent();
        ThemeWatcherService.Watch(this);

        ViewModel = new RenameViewModel(currentTitle);
        DataContext = ViewModel;

        ViewModel.CloseAction = result =>
        {
            this.DialogResult = result;
            this.Close(); 
        };  
    }

    public string Result => ViewModel.NewTitle;
}
