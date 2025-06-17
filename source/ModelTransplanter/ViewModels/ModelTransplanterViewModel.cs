using Autodesk.Revit.UI;
using System.Windows.Input;
using ModelTransplanter.Models;
using ModelTransplanter.Services;
using Wpf.Ui.Appearance;
using RPToolsUI.Services;
using Settings = ModelTransplanter.Configuration.Settings;

namespace ModelTransplanter.ViewModels;

public partial class ModelTransplanterViewModel : ObservableObject
{
    private readonly UIApplication _uiApp;
    private Logger _logger;

    [ObservableProperty] private Document _selectedSourceDoc;
    [ObservableProperty] private Document _selectedTargetDoc;
    [ObservableProperty] private string _logFilePath;
    [ObservableProperty] private int _progressValue;
    [ObservableProperty] private bool _darkTheme = true;

    partial void OnDarkThemeChanged(bool value)
    {
        var newTheme = value ? ApplicationTheme.Dark : ApplicationTheme.Light;
        ThemeWatcherService.ApplyTheme(newTheme);
    }

    public List<Document> OpenDocuments { get; }
    public ModelTransplanterViewModel(UIApplication uiApp)
    {
        _uiApp = uiApp;
        OpenDocuments = uiApp.Application.Documents.Cast<Document>().ToList();

        _logger = new Logger(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TransferElementsLog.txt");
        LogFilePath = _logger.ToString();
    }

    [RelayCommand]
    private void SelectLogPath(object parameter)
    {
        var saveFileDialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "Текстовые файлы (*.txt)|*.txt",
            DefaultExt = ".txt",
            FileName = "TransferElementsLog.txt"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            LogFilePath = saveFileDialog.FileName;
            _logger = new Logger(LogFilePath);
        }
    }
    
    [RelayCommand]
    private void TransferElements(object parameter)
    { 
        ProgressValue = 0;

        try
        {
            // Проверка документов
            if (SelectedSourceDoc == null || SelectedTargetDoc == null)
            {
                TaskDialog.Show("Ошибка", "Не выбраны исходный или целевой документ");
                return;
            }

            if (SelectedSourceDoc.IsReadOnly || SelectedTargetDoc.IsReadOnly)
            {
                TaskDialog.Show("Ошибка", "Один из документов открыт в режиме только для чтения");
                return;
            }

            try
            {
                var progress = new Progress<int>(value =>
                {
                    ProgressValue = value;
                });

                var transferService = new ElementTransferService(_logger);
                transferService.TransferElements(SelectedSourceDoc, SelectedTargetDoc, progress);

                TaskDialog.Show("Готово", "Перенос элементов завершен успешно!");
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ошибка", $"Ошибка: {ex.Message}");
                _logger.LogError("Ошибка при переносе", ex);
            }
        }
        catch
        {
            // ignored
        }
    }

    public void LoadFromSettings(Settings settings)
    {
        DarkTheme = settings.IsDarkTheme;
        LogFilePath = settings.LogFilePath;
    }

    public Settings ToSettings()
    {
        return new Settings
        {
            IsDarkTheme = DarkTheme,
            LogFilePath = LogFilePath
        };
    }
}