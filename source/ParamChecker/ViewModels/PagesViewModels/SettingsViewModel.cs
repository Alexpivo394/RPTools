using System.Windows;
using RPToolsUI.Services;
using Wpf.Ui.Appearance;

namespace ParamChecker.ViewModels.PagesViewModels
{
    public sealed partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isDarkTheme;
        
        partial void OnIsDarkThemeChanged(bool value)
        {
            var newTheme = value ? ApplicationTheme.Dark : ApplicationTheme.Light;
            ThemeWatcherService.ApplyTheme(newTheme);
        }



    }
}
     