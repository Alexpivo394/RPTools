using System.Collections.ObjectModel;
using System.Windows;
using ParamChecker.Models.ExportProfiles;
using ParamChecker.ViewModels.Windows;
using ParamChecker.Views.Windows;
using Newtonsoft.Json;
using ParamChecker.Models.Filters;
using ParamChecker.Services;

namespace ParamChecker.ViewModels.PagesViewModels;

    public partial class ExportProfilesViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _profileName;
        
        [ObservableProperty]
        private ObservableCollection<ExportModel> _models = new();

        [ObservableProperty]
        private ObservableCollection<ExportRule> _rules = new();
        
        private readonly CategoryService _categoryService;

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
    {
        try
        {
            var config = JsonConvert.DeserializeObject<FilterConfigModel>(
                rule.FilterConfigJson
            );

            if (config is not null)
            {
                filterConfigViewModel.LoadFromModel(config);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при загрузке фильтра: {ex.Message}");
        }
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
            {
                rule.ParameterConfigJson = vm.GetResultJson();
            }
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

