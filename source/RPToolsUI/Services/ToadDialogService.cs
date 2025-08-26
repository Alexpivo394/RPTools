#nullable enable
using System.Windows;
using RPToolsUI.Models;
using RPToolsUI.ViewModels;
using RPToolsUI.Views;

namespace RPToolsUI.Services;

public class ToadDialogService
{
    public static string? Show(string title, string message, DialogButtons buttons, DialogIcon icon = DialogIcon.None, Window? owner = null)
    {
        var vm = new ToadDialogViewModel
        {
            Title = title,
            Message = message,
            Icon = icon switch
            {
                DialogIcon.Info => Wpf.Ui.Controls.SymbolRegular.Info24,
                DialogIcon.Warning => Wpf.Ui.Controls.SymbolRegular.Warning24,
                DialogIcon.Error => Wpf.Ui.Controls.SymbolRegular.DismissCircle24,
                _ => Wpf.Ui.Controls.SymbolRegular.QuestionCircle24
            }
        };
        vm.BuildButtons(buttons);

        var dlg = new ToadDialog
        {
            DataContext = vm,
            Owner = owner ?? Application.Current?.MainWindow
        };

        if (dlg.ShowDialog() == true)
            return vm.Result;

        return null;
    }
}