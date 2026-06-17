using System;
using System.Windows;
using System.Windows.Input;
using Wpf.Ui.Controls;
using RPToolsUI.Services;
using Wpf.Ui.Appearance;

namespace RPToolsUI.Views;

public partial class ToadDialog : FluentWindow
{
        private const double MaxDialogWidthRatio = 0.7;
        private const double MaxDialogHeightRatio = 0.8;
        private const double ContentWidthPadding = 100;
        private const double ContentHeightPadding = 160;

        public ToadDialog ()
        {
            InitializeComponent();
            
            ApplySizeLimits();
            
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

        private void ApplySizeLimits()
        {
            var workArea = SystemParameters.WorkArea;

            MaxWidth = Math.Max(MinWidth, workArea.Width * MaxDialogWidthRatio);
            MaxHeight = Math.Max(MinHeight, workArea.Height * MaxDialogHeightRatio);

            var maxContentWidth = Math.Max(250, MaxWidth - ContentWidthPadding);
            TitleTextBlock.MaxWidth = maxContentWidth;
            MessageTextBlock.MaxWidth = maxContentWidth;
            MessageScrollViewer.MaxHeight = Math.Max(80, MaxHeight - ContentHeightPadding);
        }
        
        private void RootGrid_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }
}
