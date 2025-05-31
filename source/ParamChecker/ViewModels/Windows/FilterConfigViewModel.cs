using ParamChecker.Models.Filters;
using ParamChecker.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
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
        private ObservableCollection<FilterCondition> _filterConditions = new();

        [ObservableProperty]
        private CategoryParameterLogic _selectedItemCatOrPar;

        [ObservableProperty]
        private FilterLogic _selectedItemParOrPar;

        public IEnumerable<CategoryParameterLogic> ItemsCatOrPar => 
            Enum.GetValues(typeof(CategoryParameterLogic)).Cast<CategoryParameterLogic>();

        public IEnumerable<FilterLogic> ItemsParOrPar => 
            Enum.GetValues(typeof(FilterLogic)).Cast<FilterLogic>();

        public ICommand AddSimpleConditionCommand { get; }
        public ICommand AddGroupConditionCommand { get; }
        public ICommand ApllyFiltersCommand { get; }

        public FilterConfigViewModel(CategoryService categoryService)
        {
            _categoryService = categoryService;

            AddSimpleConditionCommand = new RelayCommand(AddSimpleCondition);
            AddGroupConditionCommand = new RelayCommand(AddGroupCondition);
            ApllyFiltersCommand = new RelayCommand(ApplyFilters);

            SelectedItemCatOrPar = CategoryParameterLogic.CategoriesAndParameters;
            SelectedItemParOrPar = FilterLogic.And;

            PropertyChanged += OnPropertyChanged;
        }

        public void Initialize(Document doc)
        {
            _categoryService.Initialize(doc);
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

        private void AddSimpleCondition()
        {
            var condition = new FilterCondition
            {
                Type = FilterConditionType.Simple
            };
            condition.Conditions.Add(new ParameterCondition());
            FilterConditions.Add(condition);
        }

        private void AddGroupCondition()
        {
            var condition = new FilterCondition
            {
                Type = FilterConditionType.Group
            };
            condition.Conditions.Add(new ParameterCondition());
            FilterConditions.Add(condition);
        }
        
        [RelayCommand]
        private void RemoveCondition(ParameterCondition condition)
        {
            foreach (var filterCondition in FilterConditions)
            {
                if (filterCondition.Conditions.Contains(condition))
                {
                    filterCondition.Conditions.Remove(condition);
                    break;
                }
            }
        }

        private void ApplyFilters()
        {
            var result = new FilterConfigResult
            {
                SelectedCategories = Categories
                    .Where(c => c.CatIsSelected)
                    .Select(c => c.BuiltInCategory)
                    .ToList(),
                Conditions = FilterConditions.ToList(),
                CategoryParameterLogic = SelectedItemCatOrPar,
                ParametersLogic = SelectedItemParOrPar
            };

            // Здесь нужно вернуть результат в вызывающее окно
            // Например, через событие или callback
        }
        
        public string GetResultJson()
        {
            var result = new FilterConfigResult
            {
                // заполните данные
            };
            return result.ToJson();
        }

        // Методы для удаления условий будут вызываться из самих условий
        public void RemoveCondition(FilterCondition condition)
        {
            FilterConditions.Remove(condition);
        }

        public void AddConditionToGroup(FilterCondition groupCondition)
        {
            if (groupCondition.Type == FilterConditionType.Group)
            {
                groupCondition.Conditions.Add(new ParameterCondition());
            }
        }
    }
}
