using ParamChecker.ViewModels.Windows;
using RPToolsUI.Services;
using Wpf.Ui.Controls;

namespace ParamChecker.Views.Windows;

/// <summary>
///     Логика взаимодействия для ParameterConfig.xaml
/// </summary>
public partial class ParameterConfig : FluentWindow
{
    public ParameterConfig(ParameterConfigViewModel viewModel)
    {
        InitializeComponent();
        viewModel.OnApplyRequested = json =>
        {
            DialogResult = true;
            Close(); // Закрываем окно
        };
        
        DataContext = viewModel;
        ThemeWatcherService.Watch(this);
    }
}