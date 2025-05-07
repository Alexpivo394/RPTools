using System.Windows;
using Wpf.Ui.Appearance;
using CheckLOI.ViewModels;
using Wpf.Ui.Controls;
using RPToolsUI.Services;

namespace CheckLOI.View
{
    public partial class CheckLOIView : FluentWindow
    {
        public CheckLOIView(CheckLOIViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            ThemeWatcherService.Initialize();
            ThemeWatcherService.Watch(this);
            ThemeWatcherService.ApplyTheme(ApplicationTheme.Light);
        }

    }
}