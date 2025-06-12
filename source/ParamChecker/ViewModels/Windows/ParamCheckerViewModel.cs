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

namespace ParamChecker.ViewModels.Windows;

public sealed partial class ParamCheckerViewModel : ObservableObject
{
    private readonly CategoryService _categoryService;

    [ObservableProperty] private bool _isChecked;


    [ObservableProperty] private string _title;

    public ParamCheckerViewModel(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public ObservableCollection<CustomNavItem> CustomNavItems { get; set; } = new();

    public Action<Page> NavigateAction { get; set; }

    [RelayCommand]
    private void AddCustomNavItem()
    {
        var vm = new ExportProfilesViewModel(_categoryService);
        vm.ProfileName = $"Профиль {CustomNavItems.Count + 1}";

        var page = new ExportProfiles
        {
            DataContext = vm
        };

        var item = new CustomNavItem(vm.ProfileName, page, vm, RemoveNavItem);
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

                var item = new CustomNavItem(vm.ProfileName, page, vm, RemoveNavItem)
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

}