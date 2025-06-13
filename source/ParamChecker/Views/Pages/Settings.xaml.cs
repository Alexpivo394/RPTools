using System.Windows.Controls;
using ParamChecker.ViewModels.PagesViewModels;
using RPToolsUI.Services;

namespace ParamChecker.Views.Pages;

/// <summary>
///     Логика взаимодействия для Settings.xaml
/// </summary>
public partial class Settings : Page
{
    public SettingsViewModel ViewModel { get; set; }
    public Settings(SettingsViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = viewModel;
        InitializeComponent();
        ThemeWatcherService.Watch(this);
    }

    
}