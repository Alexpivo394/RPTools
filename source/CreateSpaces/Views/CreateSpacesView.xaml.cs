using CreateSpaces.ViewModels;
using ToadTools.UI.Services;
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