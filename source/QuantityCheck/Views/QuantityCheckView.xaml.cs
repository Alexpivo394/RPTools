using QuantityCheck.ViewModels;
using Wpf.Ui.Appearance;
using QuantityCheck.Configuration;
using RPToolsUI.Services;

namespace QuantityCheck.Views;

public sealed partial class QuantityCheckView
{
    public QuantityCheckViewModel _viewModel;
    public Configuration.Configuration Config { get; } = new();
    public QuantityCheckView(QuantityCheckViewModel viewModel)
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