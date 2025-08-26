using System.Windows;
using System.Windows.Input;
using Wpf.Ui.Controls;
using RPToolsUI.Models;
using RPToolsUI.Services;
using Wpf.Ui.Appearance;

namespace RPToolsUI.Views;

public partial class ToadDialog : FluentWindow
{
        public ToadDialog ()
        {
            InitializeComponent();
            
            
            Loaded += (_, __) =>
            {
                if (DataContext is ViewModels.ToadDialogViewModel vm)
                {
                    vm.RequestClose += (_, __) =>
                    {
                        DialogResult = true;
                        Close();
                    };
                }
            };
            
            // ThemeWatcherService.Initialize();
            // ThemeWatcherService.ApplyTheme(ApplicationTheme.Dark);
            ThemeWatcherService.Watch(this);
        }
        
        private void RootGrid_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }
}