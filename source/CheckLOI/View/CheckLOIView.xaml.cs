using Wpf.Ui.Appearance;
using CheckLOI.ViewModels;
using Wpf.Ui.Controls;
using RPToolsUI.Services;

namespace CheckLOI.View
{
    public partial class CheckLoiView : FluentWindow
    {
        public CheckLoiView(CheckLoiViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            ThemeWatcherService.Initialize();
            ThemeWatcherService.Watch(this);
            ThemeWatcherService.ApplyTheme(ApplicationTheme.Light);
        }

    }
}