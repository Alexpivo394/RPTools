using System.Windows.Controls;
using ParamChecker.ViewModels.PagesViewModels;
using RPToolsUI.Services;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Appearance;

namespace ParamChecker.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        public SettingsViewModel ViewModel { get; }
        
        public Settings(SettingsViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
            ThemeWatcherService.Watch(this);
        }
    }
}
