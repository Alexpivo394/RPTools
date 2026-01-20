using CreateCover.ViewModels;
using RPToolsUI.Services;
using Wpf.Ui.Appearance;

namespace CreateCover.Views;

public sealed partial class CreateCoverView
{
    public CreateCoverView(CreateCoverViewModel viewModel)
    {
        InitializeComponent();
        
        ThemeWatcherService.Initialize();
        ThemeWatcherService.Watch(this);
        ThemeWatcherService.ApplyTheme(ApplicationTheme.Dark);
        
        DataContext = viewModel;
    }
}