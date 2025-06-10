using Microsoft.Win32;
using RPToolsUI.Services;
using Wpf.Ui.Appearance;

namespace ParamChecker.ViewModels.PagesViewModels;

public sealed partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty] private bool _isDarkTheme = true;

    [ObservableProperty] private string _logFilePath;

    [ObservableProperty] private string _reportFilePath;

    [ObservableProperty] private bool _updateGeneralReport;

    partial void OnIsDarkThemeChanged(bool value)
    {
        var newTheme = value ? ApplicationTheme.Dark : ApplicationTheme.Light;
        ThemeWatcherService.ApplyTheme(newTheme);
    }


    [RelayCommand]
    private void SelectLogPath(object parameter)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "Текстовые файлы (*.txt)|*.txt",
            DefaultExt = ".txt",
            FileName = "ParamCheckerLog.txt"
        };

        if (saveFileDialog.ShowDialog() == true) LogFilePath = saveFileDialog.FileName;
    }

    [RelayCommand]
    private void SelectReportFilePath(object parameter)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Excel Files (*.xlsx; *.xls)|*.xlsx;*.xls|All files (*.*)|*.*",
            Title = "Выберите Excel файл",
            DefaultExt = ".xlsx",
            CheckFileExists = true,
            CheckPathExists = true,
            Multiselect = false,
            RestoreDirectory = true
        };

        if (openFileDialog.ShowDialog() == true) ReportFilePath = openFileDialog.FileName;
    }
}