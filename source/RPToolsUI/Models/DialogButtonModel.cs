using System.Windows.Input;
using Wpf.Ui.Controls;

namespace RPToolsUI.Models;


public class DialogButtonModel
{
    public string Text { get; set; } = "";
    public SymbolRegular Icon { get; set; }
    public ICommand Command { get; set; }
}