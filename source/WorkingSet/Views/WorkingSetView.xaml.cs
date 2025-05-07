using RPToolsUI.Services;
using WorkingSet.ViewModels;
using Wpf.Ui.Appearance;

namespace WorkingSet.Views
{
    public sealed partial class WorkingSetView
    {
        public WorkingSetView(WorkingSetViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
            ThemeWatcherService.Initialize();
            ThemeWatcherService.Watch(this);
            ThemeWatcherService.ApplyTheme(ApplicationTheme.Light);
        }
    }
}