#nullable enable
using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RPToolsUI.Models;
using Wpf.Ui.Controls;

namespace RPToolsUI.ViewModels;

public partial class ToadDialogViewModel : ObservableObject
{
    private string _title = "";
    private string _message = "";
    private SymbolRegular _icon = SymbolRegular.QuestionCircle24;

    public string Title { get => _title; set => SetProperty(ref _title, value); }
    public string Message { get => _message; set => SetProperty(ref _message, value); }
    public SymbolRegular Icon { get => _icon; set => SetProperty(ref _icon, value); }

    public string? Result { get; private set; }

    public ObservableCollection<DialogButtonModel> Buttons { get; } = new();

    /// <summary>Сигнал вьюхе «закрой окно»</summary>
    public event EventHandler? RequestClose;

    private void Close(string result)
    {
        Result = result;
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    public void BuildButtons(DialogButtons buttons)
    {
        void AddBtn(string text, SymbolRegular icon)
        {
            Buttons.Add(new DialogButtonModel
            {
                Text = text,
                Icon = icon,
                Command = new RelayCommand(() => Close(text))
            });
        }

        switch (buttons)
        {
            case DialogButtons.OK:
                AddBtn("OK", SymbolRegular.Checkmark24);
                break;
            case DialogButtons.OKCancel:
                AddBtn("OK", SymbolRegular.Checkmark24);
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
