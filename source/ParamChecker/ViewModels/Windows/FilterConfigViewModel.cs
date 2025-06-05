using ParamChecker.Models.Filters;
using ParamChecker.Services;
using System.Collections.ObjectModel;
using ParamChecker.ViewModels.Conditions;

namespace ParamChecker.ViewModels.Windows
{
    public partial class FilterConfigViewModel : ObservableObject
    {
        private readonly CategoryService _categoryService;

        [ObservableProperty]
        private string _categoryFilter = "";

        [ObservableProperty]
        private ObservableCollection<CategoryFilterItem> _categories = new();

        [ObservableProperty]
        private ObservableCollection<CategoryFilterItem> _filteredCategories = new();
        
        [ObservableProperty]
        private CategoryParameterLogic _selectedItemCatOrPar;

        [ObservableProperty]
        private FilterInGroupLogic _selectedItemParOrPar;
        
        [ObservableProperty]
        private ObservableCollection<ConditionViewModelBase> filterConditions = new();

        
        public IEnumerable<CategoryParameterLogic> ItemsCatOrPar => 
            Enum.GetValues(typeof(CategoryParameterLogic)).Cast<CategoryParameterLogic>();

        public IEnumerable<FilterInGroupLogic> ItemsParOrPar => 
            Enum.GetValues(typeof(FilterInGroupLogic)).Cast<FilterInGroupLogic>();
        

        public FilterConfigViewModel(CategoryService categoryService)
        {
            _categoryService = categoryService;
            
            SelectedItemCatOrPar = CategoryParameterLogic.CategoriesAndParameters;
            SelectedItemParOrPar = FilterInGroupLogic.And;

            PropertyChanged += OnPropertyChanged;
            
        }

        public void Initialize()
        {
            LoadCategories();
        }

        private void LoadCategories()
        {
            Categories.Clear();
            foreach (var category in _categoryService.GetAllCategories())
            {
                Categories.Add(new CategoryFilterItem
                {
                    CatName = _categoryService.GetCategoryName(category),
                    BuiltInCategory = category,
                    CatIsSelected = false
                });
            }
            UpdateFilteredCategories();
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CategoryFilter))
            {
                UpdateFilteredCategories();
            }
        }

        private void UpdateFilteredCategories()
        {
            if (string.IsNullOrWhiteSpace(CategoryFilter))
            {
                FilteredCategories = new ObservableCollection<CategoryFilterItem>(Categories);
                return;
            }

            var filtered = Categories
                .Where(c => c.CatName.Contains(CategoryFilter, StringComparison.OrdinalIgnoreCase))
                .ToList();

            FilteredCategories = new ObservableCollection<CategoryFilterItem>(filtered);
        }

        public string GetResultJson()
        {
            throw new NotImplementedException();
        }

        [RelayCommand]
        private void AddSimpleCondition()
        {
            var simple = new SimpleConditionViewModel
            {
                RemoveSimpleRequested = (vm) => FilterConditions.Remove(vm)
            };
            FilterConditions.Add(simple);
        }

        [RelayCommand]
        private void AddGroupCondition()
        {
            var group = new GroupConditionViewModel
            {
                RemoveGroupRequested = (vm) => FilterConditions.Remove(vm)
            };
            FilterConditions.Add(group);
        }
    }
}
