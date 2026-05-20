using RPToolsUI.Services;
using Wpf.Ui.Appearance;
using WriteDash.ViewModels;

namespace WriteDash.Views;

public sealed partial class WriteDashView
{
    public WriteDashView(WriteDashViewModel viewModel)
    {
        InitializeComponent();
        
        ThemeWatcherService.Initialize();
        ThemeWatcherService.Watch(this);
        ThemeWatcherService.ApplyTheme(ApplicationTheme.Dark);
        
        DataContext = viewModel;
    }
}