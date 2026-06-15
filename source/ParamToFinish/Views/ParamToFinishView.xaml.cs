using ParamToFinish.ViewModels;
using RPToolsUI.Services;
using Wpf.Ui.Appearance;

namespace ParamToFinish.Views;

public sealed partial class ParamToFinishView
{
    public ParamToFinishView(ParamToFinishViewModel viewModel)
    {
        InitializeComponent();
        
        ThemeWatcherService.Initialize();
        ThemeWatcherService.Watch(this);
        ThemeWatcherService.ApplyTheme(ApplicationTheme.Dark);
        
        DataContext = viewModel;
        
        viewModel.CloseRequested += (_, _) => Close();
    }
}