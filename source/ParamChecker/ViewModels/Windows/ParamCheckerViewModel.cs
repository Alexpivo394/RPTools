using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using ParamChecker.Models.ExportProfiles;
using ParamChecker.Services;
using ParamChecker.ViewModels.PagesViewModels;
using ParamChecker.Views.Dialogs;
using ParamChecker.Views.Pages;
using ParamChecker.Models;

namespace ParamChecker.ViewModels.Windows;

public sealed partial class ParamCheckerViewModel : ObservableObject
{
    private readonly CategoryService _categoryService;
    private readonly ExportService _exportService;
    private readonly Logger _logger;
    private readonly SettingsViewModel _settingsViewModel;

    [ObservableProperty] private bool _isChecked;
    [ObservableProperty] private string _title;

    public ParamCheckerViewModel(CategoryService categoryService,  ExportService exportService,  SettingsViewModel settingsViewModel, Logger logger)
    {
        _logger = logger;
        _settingsViewModel = settingsViewModel;
        _categoryService = categoryService;
        _exportService = exportService;
    }

    public ObservableCollection<CustomNavItem> CustomNavItems { get; set; } = new();

    public Action<Page> NavigateAction { get; set; }

    [RelayCommand]
    private void AddCustomNavItem()
    {
        var vm = new ExportProfilesViewModel(_categoryService);
        vm.ProfileName = $"Профиль {CustomNavItems.Count + 1}";
        vm.IsChecked = IsChecked;

        var page = new ExportProfiles
        {
            DataContext = vm
        };

        var item = new CustomNavItem(IsChecked, vm.ProfileName, page, vm, RemoveNavItem);
        item.OnNavigate = NavigateAction;
        CustomNavItems.Add(item);
    }

    [RelayCommand]
    private void RemoveNavItem(CustomNavItem item)
    {
        if (CustomNavItems.Contains(item))
            CustomNavItems.Remove(item);
    }

    [RelayCommand]
    private void RenameNavItem()
    {
        var dialog = new Rename(Title);
        var result = dialog.ShowDialog();
        if (result == true) Title = dialog.Title;
    }
    
    [RelayCommand]
    private void ExportProfiles()
    {
        var profiles = new List<ExportProfile>();

        foreach (var item in CustomNavItems)
        {
            if (item.ViewModelInstance is ExportProfilesViewModel vm)
            {
                var profile = vm.GetProfile();
                profile.ProfileName = vm.ProfileName;
                profiles.Add(profile);
            }
        }

        var json = JsonConvert.SerializeObject(profiles, Formatting.Indented);

        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "JSON files (*.json)|*.json",
            FileName = "export_profiles.json"
        };

        if (dialog.ShowDialog() == true)
        {
            File.WriteAllText(dialog.FileName, json);
        }
    }

    [RelayCommand]
    private void ImportProfiles()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "JSON files (*.json)|*.json"
        };

        if (dialog.ShowDialog() != true) return;

        try
        {
            var json = File.ReadAllText(dialog.FileName);
            var profiles = JsonConvert.DeserializeObject<List<ExportProfile>>(json);

            if (profiles == null) return;

            CustomNavItems.Clear();

            foreach (var profile in profiles)
            {
                var vm = new ExportProfilesViewModel(_categoryService);
                vm.LoadFromProfile(profile);

                var page = new ExportProfiles
                {
                    DataContext = vm
                };

                var item = new CustomNavItem(vm.IsChecked, vm.ProfileName, page, vm, RemoveNavItem)
                {
                    OnNavigate = NavigateAction
                };

                CustomNavItems.Add(item);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при импорте: {ex.Message}");
        }
    }

    [RelayCommand]
    private void StartExport()
    {

        if (!CustomNavItems.Any()) MessageBox.Show("Нет профилей для экспорта!", "Ошибка!");
        else
        {
            _logger.StartLog(_settingsViewModel.LogFilePath);
            foreach (var item in CustomNavItems)
            {
                if (item.ViewModelInstance is not ExportProfilesViewModel vm) continue;

                var profile = vm.GetProfile();
                if (profile == null) continue;
                if (profile.IsChecked == true)
                {
                    if (!profile.Models.Any()) MessageBox.Show($"Добавьте хотябы одну модель в профиль {profile.ProfileName}!", "Ошибка!");
                    else
                    {
                        if (!profile.Rules.Any()) MessageBox.Show($"Добавьте хотябы одно правило в профиль {profile.ProfileName}!", "Ошибка!");
                        else
                        {
                            _logger.Log($"Обрабатываем профиль {profile.ProfileName}");
                            _exportService.ExportProfile(profile);
                        }
                    }
                }
                else
                {
                    _logger.Log($"Профиль {profile.ProfileName} не выбран для экспорта");
                }
            }

            MessageBox.Show("Экспорт завершен!", "Успех!");
        }
    }

}