using System.Windows;
using Wpf.Ui.Controls;
using RPToolsUI.Models;
using RPToolsUI.Services;
using Wpf.Ui.Appearance;

namespace RPToolsUI.Views;

public partial class ToadDialog : FluentWindow
{
 public string Result { get; private set; }

        public ToadDialog(string title, string message, DialogButtons buttons, DialogIcon icon)
        {
            InitializeComponent();
            TitleBlock.Text = title;
            MessageBlock.Text = message;
            LoadIcon(icon);
            CreateButtons(buttons);
            ThemeWatcherService.Initialize();
            ThemeWatcherService.ApplyTheme(ApplicationTheme.Dark);
            ThemeWatcherService.Watch(this);
        }

        private void LoadIcon(DialogIcon icon)
        {
            SymbolRegular symbol = icon switch
            {
                DialogIcon.Info => SymbolRegular.Info24,
                DialogIcon.Warning => SymbolRegular.Warning24,
                DialogIcon.Error => SymbolRegular.DismissCircle24,
                _ => SymbolRegular.QuestionCircle24
            };
            DialogIconControl.Symbol = symbol;
        }

        private void CreateButtons(DialogButtons buttons)
        {
            void AddBtn(string text, SymbolRegular icon = SymbolRegular.Checkmark24)
            {
                var btn = new Button
                {
                    Content = text,
                    Icon = new SymbolIcon { Symbol = icon, Margin = new Thickness(0,0,5,0) },
                    Tag = text,
                    Width = 90,
                    Margin = new Thickness(5)
                };
                btn.Click += (s, e) =>
                {
                    Result = text;
                    DialogResult = true;
                };
                ButtonsPanel.Children.Add(btn);
            }

            switch (buttons)
            {
                case DialogButtons.OK:
                    AddBtn("OK");
                    break;
                case DialogButtons.OKCancel:
                    AddBtn("OK");
                    AddBtn("Cancel", SymbolRegular.Dismiss24);
                    break;
                case DialogButtons.YesNo:
                    AddBtn("Yes", SymbolRegular.Checkmark24);
                    AddBtn("No", SymbolRegular.Dismiss24);
                    break;
                case DialogButtons.YesNoCancel:
                    AddBtn("Yes", SymbolRegular.Checkmark24);
                    AddBtn("No", SymbolRegular.Dismiss24);
                    AddBtn("Cancel", SymbolRegular.ArrowHookDownLeft24);
                    break;
                case DialogButtons.RetryCancel:
                    AddBtn("Retry", SymbolRegular.ArrowClockwise24);
                    AddBtn("Cancel", SymbolRegular.Dismiss24);
                    break;
                case DialogButtons.RetryAbort:
                    AddBtn("Retry", SymbolRegular.ArrowClockwise24);
                    AddBtn("Abort", SymbolRegular.DismissCircle24);
                    break;
            }
        }
}