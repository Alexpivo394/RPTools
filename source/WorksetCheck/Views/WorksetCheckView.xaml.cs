using RPToolsUI.Services;
using WorksetCheck.ViewModels;
using Wpf.Ui.Appearance;

namespace WorksetCheck.Views;

public sealed partial class WorksetCheckView
{
    public WorksetCheckView(WorksetCheckViewModel viewModel)
    {
        InitializeComponent();
        
        ThemeWatcherService.Initialize();
        ThemeWatcherService.Watch(this);
        ThemeWatcherService.ApplyTheme(ApplicationTheme.Dark);
        
        DataContext = viewModel;
    }
}