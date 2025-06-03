using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ModelTransplanter.Models;
using ModelTransplanter.Services;
using ModelTransplanter.Commands;
using Wpf.Ui.Appearance;
using RPToolsUI.Services;
using RelayCommand = ModelTransplanter.Commands.RelayCommand;

namespace ModelTransplanter.ViewModels
{
    public class ModelTransplanterViewModel : BaseViewModel
    {
        private readonly UIApplication _uiApp;
        private Logger _logger;

        private Document _selectedSourceDoc;
        private Document _selectedTargetDoc;
        private string _logFilePath;
        private int _progressValue;
        private bool _isBusy;
        private bool _darkTheme;
        public bool DarkTheme
        {
            get => _darkTheme;
            set
            {
                var newTheme = value ? ApplicationTheme.Dark : ApplicationTheme.Light;
                ThemeWatcherService.ApplyTheme(newTheme);
                SetField(ref _darkTheme, value);
            }
        }

        public List<Document> OpenDocuments { get; }
        public ICommand SelectLogPathCommand { get; }
        public ICommand TransferElementsCommand { get; }

        public Document SelectedSourceDoc
        {
            get => _selectedSourceDoc;
            set
            {
                _selectedSourceDoc = value;
                OnPropertyChanged();
            }
        }

        public Document SelectedTargetDoc
        {
            get => _selectedTargetDoc;
            set
            {
                _selectedTargetDoc = value;
                OnPropertyChanged();
            }
        }

        public string LogFilePath
        {
            get => _logFilePath;
            set
            {
                _logFilePath = value;
                OnPropertyChanged();
            }
        }

        public int ProgressValue
        {
            get => _progressValue;
            set
            {
                _progressValue = value;
                OnPropertyChanged();
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public ModelTransplanterViewModel(UIApplication uiApp)
        {
            _uiApp = uiApp;
            OpenDocuments = uiApp.Application.Documents.Cast<Document>().ToList();

            _logger = new Logger(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TransferElementsLog.txt");
            LogFilePath = _logger.ToString();

            SelectLogPathCommand = new RelayCommand(SelectLogPath);
            TransferElementsCommand = new RelayCommand(TransferElements, CanTransferElements);
        }

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

        private bool CanTransferElements(object parameter)
        {
            return SelectedSourceDoc != null &&
                   SelectedTargetDoc != null &&
                   SelectedSourceDoc != SelectedTargetDoc &&
                   !IsBusy;
        }

        private void TransferElements(object parameter)
        {
            IsBusy = true;
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

                // Создаем контекст выполнения в UI потоке
                var uiContext = SynchronizationContext.Current;

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
            finally
            {
                IsBusy = false;
            }
        }

    }
}