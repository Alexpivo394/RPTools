using SSPlan.ViewModels;
using RPToolsUI.Services;
using Wpf.Ui.Appearance;
using Settings = SSPlan.Configuration.Settings;

namespace SSPlan.Views;

public sealed partial class SSPlanView
{
    public SSPlanViewModel _viewModel;
    public Configuration.Configuration Config { get; } = new();
    
    public SSPlanView(SSPlanViewModel viewModel)
    {
        InitializeComponent();
        
        ThemeWatcherService.Initialize();
        ThemeWatcherService.ApplyTheme(ApplicationTheme.Dark);
        ThemeWatcherService.Watch(this);
        
        _viewModel = viewModel;
        var settings = Config.LoadSettings();
        if (settings != null)
            viewModel.LoadFromSettings(settings);
        DataContext = viewModel;
        
        
        Closing += OnClosing;
        
    }
    
    private void OnClosing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        Settings setting = _viewModel.ToSettings();
        Config.SaveSettings(setting);
    }
}