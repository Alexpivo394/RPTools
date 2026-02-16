using CreateSpaces.ViewModels;
using RPToolsUI.Services;
using Wpf.Ui.Appearance;

namespace CreateSpaces.Views;

public sealed partial class CreateSpacesView
{
    public CreateSpacesView(CreateSpacesViewModel viewModel)
    {
        InitializeComponent();
        
        ThemeWatcherService.Initialize();
        ThemeWatcherService.Watch(this);
        ThemeWatcherService.ApplyTheme(ApplicationTheme.Dark);
        
        DataContext = viewModel;
    }
}