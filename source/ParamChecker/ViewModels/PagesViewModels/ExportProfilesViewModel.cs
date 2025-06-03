using System.Collections.ObjectModel;
using ParamChecker.Models.ExportProfiles;
using ParamChecker.ViewModels.Windows;
using ParamChecker.Views.Windows;
using Newtonsoft.Json;
using ParamChecker.Services;

namespace ParamChecker.ViewModels.PagesViewModels
{
    public partial class ExportProfilesViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _profileName;
        
        [ObservableProperty]
        private ObservableCollection<ExportModel> _models = new();

        [ObservableProperty]
        private ObservableCollection<ExportRule> _rules = new();

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
            var categoryService = new CategoryService();
            var filterConfigViewModel = new FilterConfigViewModel(categoryService);
            var filterWindow = new FilterConfig(filterConfigViewModel);
            if (filterWindow.ShowDialog() == true && filterWindow.DataContext is FilterConfigViewModel vm)
            {
                rule.FilterConfigJson = vm.GetResultJson();
            }
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
}
