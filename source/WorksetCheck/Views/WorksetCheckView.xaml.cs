using RPToolsUI.Services;
using WorksetCheck.ViewModels;
using Wpf.Ui.Appearance;

namespace WorksetCheck.Views;

public sealed partial class WorksetCheckView
{
    public WorksetCheckViewModel _viewModel;

    public Configuration.Configuration Config { get; } = new();
    public WorksetCheckView(WorksetCheckViewModel viewModel)
    {
        InitializeComponent();
        
        ThemeWatcherService.Initialize();
        ThemeWatcherService.Watch(this);
        ThemeWatcherService.ApplyTheme(ApplicationTheme.Dark);
        
        _viewModel = viewModel;
        var settings = Config.LoadSettings();
        if (settings != null)
            viewModel.LoadFromSettings(settings);
        DataContext = viewModel;
        
        Closing += OnClosing;
    }
    
    private void OnClosing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        var setting = _viewModel.ToSettings();
        Config.SaveSettings(setting);
    }
}