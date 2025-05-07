using ModelTransplanter.ViewModels;
using RPToolsUI.Services;
using Wpf.Ui.Appearance;

namespace ModelTransplanter.Views
{
    public sealed partial class ModelTransplanterView
    {
        public ModelTransplanterView(ModelTransplanterViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
            ThemeWatcherService.Initialize();
            ThemeWatcherService.Watch(this);
            ThemeWatcherService.ApplyTheme(ApplicationTheme.Light);
        }
    }
}