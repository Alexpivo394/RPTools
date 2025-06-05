using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Autodesk.Revit.UI;
using Microsoft.Win32;
using Newtonsoft.Json;
using CheckLOI.Models;
using CheckLOI.Services;
using Wpf.Ui.Appearance;
using RPToolsUI.Services;

namespace CheckLOI.ViewModels
{
    public sealed partial class CheckLoiViewModel : BaseViewModel
    {
        private readonly ExternalCommandData _commandData;
        private readonly CheckLoiModel _model;
        private readonly CategoryService _categoryService;
        private int _tabCounter = 1;
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

        public CheckLoiViewModel(ExternalCommandData commandData, CategoryService categoryService)
        {
            _commandData = commandData;
            _categoryService = categoryService;
            _model = new CheckLoiModel();            

            // Инициализация команд
            AddTabCommand = new RelayCommand(AddTab);
            RemoveTabCommand = new RelayCommand<ExportConfiguration>(RemoveTab);
            StartExportCommand = new RelayCommand(StartExport);
            ExportSettingsCommand = new RelayCommand(ExportSettings);
            ImportSettingsCommand = new RelayCommand(ImportSettings);

            // Инициализация коллекций
            ExportConfigurations = new ObservableCollection<ExportConfiguration>();

            // Добавление первой вкладки по умолчанию
            AddTab();
        }

        public ObservableCollection<ExportConfiguration> ExportConfigurations { get; }

        public ICommand AddTabCommand { get; }
        public ICommand RemoveTabCommand { get; }
        public ICommand StartExportCommand { get; }
        public ICommand ExportSettingsCommand { get; }
        public ICommand ImportSettingsCommand { get; }

        private void AddTab()
        {
            var config = new ExportConfiguration
            {
                TabName = $"Конфигурация {_tabCounter++}",
                Parameters = new ObservableCollection<ParameterItem> { new ParameterItem() },
                ModelsToExport = new ObservableCollection<ModelExportItem> { new ModelExportItem() }
            };

            config.AddParameterCommand = new RelayCommand(() => AddParameter(config));
            config.AddModelCommand = new RelayCommand(() => AddModel(config));
            config.RemoveModelCommand = new RelayCommand<ModelExportItem>(model => RemoveModel(config, model));
            config.EditTabNameCommand = new RelayCommand(() => ToggleTabNameEditing(config));

            // Инициализация категорий
            var categories = Enum.GetValues(typeof(BuiltInCategory))
                .Cast<BuiltInCategory>()
                .Where(c => c != BuiltInCategory.INVALID)
                .Select(c => new CategoryItem
                {
                    Name = _categoryService.GetCategoryName(c),
                    Category = c,
                    IsSelected = false
                })
                .OrderBy(c => c.Name)
                .ToList();

            config.Categories = new ObservableCollection<CategoryItem>(categories);
            config.FilteredCategories = CollectionViewSource.GetDefaultView(config.Categories);
            config.FilteredCategories.Filter = item =>
            {
                if (string.IsNullOrWhiteSpace(config.CategoryFilter)) return true;
                var category = (CategoryItem)item;
                return category.Name.IndexOf(config.CategoryFilter, StringComparison.OrdinalIgnoreCase) >= 0;
            };

            ExportConfigurations.Add(config);
        }

        private void RemoveTab(ExportConfiguration config)
        {
            if (ExportConfigurations.Count > 1)
            {
                ExportConfigurations.Remove(config);
            }
            else
            {
                MessageBox.Show("Должна остаться хотя бы одна конфигурация.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AddParameter(ExportConfiguration config)
        {
            config.Parameters.Add(new ParameterItem());
        }

        private void AddModel(ExportConfiguration config)
        {
            config.ModelsToExport.Add(new ModelExportItem
            {
                ViewName = "Navisworks",
                WorksetKeyword = "00"
            });
        }

        private void RemoveModel(ExportConfiguration config, ModelExportItem model)
        {
            config.ModelsToExport.Remove(model);
        }

        private void ToggleTabNameEditing(ExportConfiguration config)
        {
            config.IsTabNameEditing = !config.IsTabNameEditing;
        }

        private void StartExport()
        {
            try
            {
                foreach (var config in ExportConfigurations)
                {
                    if (config.ModelsToExport.Count == 0)
                    {
                        MessageBox.Show($"Добавьте хотя бы одну модель для экспорта в '{config.TabName}'.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var parameters = config.Parameters
                        .Where(p => !string.IsNullOrWhiteSpace(p.Value))
                        .Select(p => p.Value)
                        .ToList();

                    if (parameters.Count == 0)
                    {
                        MessageBox.Show($"Укажите хотя бы один параметр для экспорта в '{config.TabName}'.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var selectedCategories = config.Categories
                        .Where(c => c.IsSelected)
                        .Select(c => c.Category)
                        .ToList();

                    if (selectedCategories.Count == 0)
                    {
                        MessageBox.Show($"Выберите хотя бы одну категорию для экспорта в '{config.TabName}'.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    foreach (var model in config.ModelsToExport)
                    {
                        if (string.IsNullOrWhiteSpace(model.ModelPath))
                        {
                            MessageBox.Show($"Укажите путь к модели в '{config.TabName}'.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        var result = _model.ProcessModelAsync(
                            _commandData,
                            parameters,
                            selectedCategories,
                            config.ExportToExistingFile,
                            model.ModelPath,
                            model.ViewName,
                            model.WorksetKeyword).Result;

                        if (!result.IsSuccess)
                        {
                            MessageBox.Show($"Ошибка экспорта {model.ModelPath}: {result.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }

                MessageBox.Show("Экспорт успешно завершен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportSettings()
        {
            var settings = new ExportSettingsData
            {
                Configurations = ExportConfigurations.Select(c => new ExportConfigData
                {
                    TabName = c.TabName,
                    Parameters = c.Parameters.Select(p => p.Value).ToList(),
                    SelectedCategories = c.Categories.Where(cat => cat.IsSelected).Select(cat => cat.Category).ToList(),
                    ExportToExistingFile = c.ExportToExistingFile,
                    Models = c.ModelsToExport.Select(m => new ModelExportData
                    {
                        ModelPath = m.ModelPath,
                        ViewName = m.ViewName,
                        WorksetKeyword = m.WorksetKeyword
                    }).ToList()
                }).ToList(),
            };

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON файлы (*.json)|*.json",
                DefaultExt = "json",
                Title = "Экспорт настроек"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                    File.WriteAllText(saveFileDialog.FileName, json);
                    MessageBox.Show("Настройки успешно экспортированы!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка экспорта настроек: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ImportSettings()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "JSON файлы (*.json)|*.json",
                DefaultExt = "json",
                Title = "Импорт настроек"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var json = File.ReadAllText(openFileDialog.FileName);
                    var settings = JsonConvert.DeserializeObject<ExportSettingsData>(json);

                    if (settings == null)
                    {
                        MessageBox.Show("Некорректный формат файла настроек.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Очистка текущих настроек
                    ExportConfigurations.Clear();

                    // Импорт конфигураций
                    foreach (var configData in settings.Configurations)
                    {
                        var config = new ExportConfiguration
                        {
                            TabName = configData.TabName,
                            ExportToExistingFile = configData.ExportToExistingFile,
                            Parameters = new ObservableCollection<ParameterItem>(
                                configData.Parameters.Select(p => new ParameterItem { Value = p })),
                            ModelsToExport = new ObservableCollection<ModelExportItem>(
                                configData.Models.Select(m => new ModelExportItem
                                {
                                    ModelPath = m.ModelPath,
                                    ViewName = m.ViewName,
                                    WorksetKeyword = m.WorksetKeyword
                                }))
                        };

                        config.AddParameterCommand = new RelayCommand(() => AddParameter(config));
                        config.AddModelCommand = new RelayCommand(() => AddModel(config));
                        config.RemoveModelCommand = new RelayCommand<ModelExportItem>(model => RemoveModel(config, model));
                        config.EditTabNameCommand = new RelayCommand(() => ToggleTabNameEditing(config));

                        // Инициализация категорий
                        var categories = Enum.GetValues(typeof(BuiltInCategory))
                            .Cast<BuiltInCategory>()
                            .Where(c => c != BuiltInCategory.INVALID)
                            .Select(c => new CategoryItem
                            {
                                Name = _categoryService.GetCategoryName(c),
                                Category = c,
                                IsSelected = configData.SelectedCategories.Contains(c)
                            })
                            .OrderBy(c => c.Name)
                            .ToList();

                        config.Categories = new ObservableCollection<CategoryItem>(categories);
                        config.FilteredCategories = CollectionViewSource.GetDefaultView(config.Categories);
                        config.FilteredCategories.Filter = item =>
                        {
                            if (string.IsNullOrWhiteSpace(config.CategoryFilter)) return true;
                            var category = (CategoryItem)item;
                            return category.Name.IndexOf(config.CategoryFilter, StringComparison.OrdinalIgnoreCase) >= 0;
                        };

                        ExportConfigurations.Add(config);
                    }

                    MessageBox.Show("Настройки успешно импортированы!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка импорта настроек: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    public class ExportConfiguration : BaseViewModel
    {
        private string _tabName;
        private bool _isTabNameEditing;
        private bool _exportToExistingFile;
        private string _viewName = "Navisworks";
        private string _worksetKeyword = "00";
        private string _categoryFilter;

        public string TabName
        {
            get => _tabName;
            set => SetField(ref _tabName, value);
        }

        public bool IsTabNameEditing
        {
            get => _isTabNameEditing;
            set => SetField(ref _isTabNameEditing, value);
        }

        public bool ExportToExistingFile
        {
            get => _exportToExistingFile;
            set => SetField(ref _exportToExistingFile, value);
        }

        public string ViewName
        {
            get => _viewName;
            set => SetField(ref _viewName, value);
        }

        public string WorksetKeyword
        {
            get => _worksetKeyword;
            set => SetField(ref _worksetKeyword, value);
        }

        public string CategoryFilter
        {
            get => _categoryFilter;
            set
            {
                SetField(ref _categoryFilter, value);
                FilteredCategories?.Refresh();
            }
        }

        public ObservableCollection<ParameterItem> Parameters { get; set; }
        public ObservableCollection<CategoryItem> Categories { get; set; }
        public ObservableCollection<ModelExportItem> ModelsToExport { get; set; }
        public ICollectionView FilteredCategories { get; set; }
        public ICommand AddParameterCommand { get; set; }
        public ICommand AddModelCommand { get; set; }
        public ICommand RemoveModelCommand { get; set; }
        public ICommand EditTabNameCommand { get; set; }
    }

    public class ModelExportItem : BaseViewModel
    {
        private string _modelPath;
        private string _viewName;
        private string _worksetKeyword;

        public string ModelPath
        {
            get => _modelPath;
            set => SetField(ref _modelPath, value);
        }

        public string ViewName
        {
            get => _viewName;
            set => SetField(ref _viewName, value);
        }

        public string WorksetKeyword
        {
            get => _worksetKeyword;
            set => SetField(ref _worksetKeyword, value);
        }
    }

    public class ParameterItem : BaseViewModel
    {
        private string _value;

        public string Value
        {
            get => _value;
            set => SetField(ref _value, value);
        }
    }

    public class CategoryItem : BaseViewModel
    {
        private bool _isSelected;

        public string Name { get; set; }
        public BuiltInCategory Category { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetField(ref _isSelected, value);
        }
    }

    public class ExportSettingsData
    {
        public List<ExportConfigData> Configurations { get; set; }
        public bool? IsDarkTheme { get; set; }
    }

    public class ExportConfigData
    {
        public string TabName { get; set; }
        public List<string> Parameters { get; set; }
        public List<BuiltInCategory> SelectedCategories { get; set; }
        public bool ExportToExistingFile { get; set; }
        public List<ModelExportData> Models { get; set; }
    }

    public class ModelExportData
    {
        public string ModelPath { get; set; }
        public string ViewName { get; set; }
        public string WorksetKeyword { get; set; }
    }

}