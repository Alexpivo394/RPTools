using System.Windows;
using RPToolsUI.Models;
using RPToolsUI.Views;

namespace RPToolsUI.Services;

public class ToadDialogService
{
    public static string Show(string title, string message, DialogButtons buttons, DialogIcon icon = DialogIcon.None, Window owner = null)
    {
        var dlg = new ToadDialog(title, message, buttons, icon)
        {
            Owner = owner ?? Application.Current?.MainWindow
        };
        return dlg.ShowDialog() == true ? dlg.Result : null;
    }
}