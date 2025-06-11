using System.Collections.ObjectModel;
using System.Windows;
using ParamChecker.Models.ExportProfiles;
using ParamChecker.Services;
using ParamChecker.ViewModels.Windows;
using ParamChecker.Views.Windows;

namespace ParamChecker.ViewModels.PagesViewModels;

public partial class ExportProfilesViewModel : ObservableObject
{
    private readonly CategoryService _categoryService;

    [ObservableProperty] private ObservableCollection<ExportModel> _models = new();

    [ObservableProperty] private string _profileName;

    [ObservableProperty] private ObservableCollection<ExportRule> _rules = new();

    public ExportProfilesViewModel(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [RelayCommand]
    private void AddModel()
    {
        Models.Add(new ExportModel());
    }

    [RelayCommand]
    private void RemoveModel(ExportModel model)
    {
        Models.Remove(model);
    }

    [RelayCommand]
    private void AddRule()
    {
        Rules.Add(new ExportRule());
    }

    [RelayCommand]
    private void RemoveRule(ExportRule rule)
    {
        Rules.Remove(rule);
    }

    [RelayCommand]
    private void OpenFilterConfig(ExportRule rule)
    {
        var filterConfigViewModel = new FilterConfigViewModel(_categoryService);
        filterConfigViewModel.Initialize(); // загружаем категории


        if (!string.IsNullOrWhiteSpace(rule.FilterConfigJson))
            try
            {
                var config = filterConfigViewModel.ParseConfig(rule.FilterConfigJson);
                filterConfigViewModel.LoadFromModel(config);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке фильтра: {ex.Message}");
            }

        var filterWindow = new FilterConfig(filterConfigViewModel);

        filterConfigViewModel.OnApplyRequested = json =>
        {
            rule.FilterConfigJson = json;
            filterWindow.Close();
        };

        filterWindow.ShowDialog();
    }


    [RelayCommand]
    private void OpenParameterConfig(ExportRule rule)
    {
        var paramViewModel = new ParameterConfigViewModel();
        var paramWindow = new ParameterConfig(paramViewModel);
        if (paramWindow.ShowDialog() == true && paramWindow.DataContext is ParameterConfigViewModel vm)
            rule.ParameterConfigJson = vm.GetResultJson();
    }

    public ExportProfile GetProfile()
    {
        return new ExportProfile
        {
            Models = Models.ToList(),
            Rules = Rules.ToList()
        };
    }
}