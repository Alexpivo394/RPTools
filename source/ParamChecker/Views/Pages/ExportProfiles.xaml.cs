using System.Windows.Controls;
using ParamChecker.ViewModels.PagesViewModels;
using RPToolsUI.Services;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace ParamChecker.Views.Pages
{

    public partial class ExportProfiles : Page

    {
        public ExportProfiles()
        {
            InitializeComponent();
            ThemeWatcherService.Watch(this);

        }
    }
}
