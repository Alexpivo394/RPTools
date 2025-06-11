using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ParamChecker.ViewModels.PagesViewModels;
using ParamChecker.ViewModels.Windows;
using RPToolsUI.Services;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Settings = ParamChecker.Views.Pages.Settings;

namespace ParamChecker.Views.Windows;

public sealed partial class ParamChecker : FluentWindow
{
    private readonly ParamCheckerViewModel _viewModel;

    public ParamChecker(ParamCheckerViewModel viewModel)
    {
        InitializeComponent();
        ThemeWatcherService.Initialize();
        ThemeWatcherService.Watch(this);
        ThemeWatcherService.ApplyTheme(ApplicationTheme.Dark);

        _viewModel = viewModel;
        DataContext = _viewModel;

        _viewModel.NavigateAction = NavigateToPage;
    }

    public SettingsViewModel SettingsVm { get; } = new();

    private void NavigateToPage(Page page)
    {
        MainFrame.Navigate(page);
    }

    private void OnProfileClicked(object sender, MouseButtonEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.DataContext is CustomNavItem item) item.SelectCommand.Execute(null);
    }

    private void OnSettingsClicked(object sender, MouseButtonEventArgs e)
    {
        var page = new Settings(SettingsVm);

        MainFrame.Navigate(page);
    }
}