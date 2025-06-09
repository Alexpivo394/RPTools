using Wpf.Ui.Controls;
using ParamChecker.ViewModels.Windows;
using RPToolsUI.Services;

namespace ParamChecker.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для FilterConfig.xaml
    /// </summary>
    public partial class FilterConfig : FluentWindow
    {
        public FilterConfig(FilterConfigViewModel viewModel)
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
}
