using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            ApplicationThemeManager.Apply(newTheme);
        }
    }
}
     