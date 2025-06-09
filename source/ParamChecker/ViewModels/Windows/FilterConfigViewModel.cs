#nullable enable
using ParamChecker.Models.Filters;
using ParamChecker.Services;
using System.Collections.ObjectModel;
using ParamChecker.ViewModels.Conditions;
using Newtonsoft.Json;

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
        private FilterParameterLogic _selectedItemParOrPar;
        
        [ObservableProperty]
        private ObservableCollection<ConditionViewModelBase> filterConditions = new();
        
        [ObservableProperty]
        private FilterConfigModel configModel = new();
        
        public Action<string>? OnApplyRequested { get; set; }

        
        public IEnumerable<CategoryParameterLogic> ItemsCatOrPar => 
            Enum.GetValues(typeof(CategoryParameterLogic)).Cast<CategoryParameterLogic>();

        public IEnumerable<FilterParameterLogic> ItemsParOrPar => 
            Enum.GetValues(typeof(FilterParameterLogic)).Cast<FilterParameterLogic>();
        

        public FilterConfigViewModel(CategoryService categoryService)
        {
            _categoryService = categoryService;
            
            SelectedItemCatOrPar = CategoryParameterLogic.CategoriesAndParameters;
            SelectedItemParOrPar = FilterParameterLogic.And;

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
                .Where(c => c.CatName != null && c.CatName.Contains(CategoryFilter, StringComparison.OrdinalIgnoreCase))
                .ToList();

            FilteredCategories = new ObservableCollection<CategoryFilterItem>(filtered);
        }

        public string GetResultJson()
        {
            var model = new FilterConfigModel
            {
                SelectedCategories = Categories
                    .Where(c => c.CatIsSelected)
                    .Select(c => c.BuiltInCategory)
                    .ToList(),
                CategoryParameterLogic = SelectedItemCatOrPar,
                ParameterLogic = SelectedItemParOrPar,
                Conditions = FilterConditions.Select(ToConditionModel).ToList()
            };

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };

            return JsonConvert.SerializeObject(model, settings);
        }

        private ConditionModelBase ToConditionModel(ConditionViewModelBase vm)
        {
            switch (vm)
            {
                case SimpleConditionViewModel simple:
                    return new SimpleConditionModel
                    {
                        Type = "Simple",
                        ParameterName = simple.ParameterName,
                        Value = simple.Value,
                        SelectedLogic = simple.SelectedItemLogic
                    };

                case GroupConditionViewModel group:
                    return new GroupConditionModel
                    {
                        Type = "Group",
                        Children = group.Children.Select(ToConditionModel).ToList()
                    };

                default:
                    throw new NotSupportedException($"Unknown condition VM: {vm.GetType().Name}");
            }
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
        
        [RelayCommand]
        private void Apply()
        {
            var json = GetResultJson();
            OnApplyRequested?.Invoke(json);
        }
        
        public void LoadFromModel(FilterConfigModel model)
        {
            // 🧠 Восстановить выбранные категории
            foreach (var cat in Categories)
                cat.CatIsSelected = model.SelectedCategories.Contains(cat.BuiltInCategory);

            // 🧠 Восстановить логику фильтрации
            SelectedItemCatOrPar = model.CategoryParameterLogic;
            SelectedItemParOrPar = model.ParameterLogic;

            // 🧠 Восстановить условия
            FilterConditions.Clear();
            foreach (var condition in model.Conditions)
            {
                var vm = FromConditionModel(condition);
                if (vm != null)
                    FilterConditions.Add(vm);
            }
        }

// 👇 Распаковывает обратно из Model в ViewModel
        private ConditionViewModelBase? FromConditionModel(ConditionModelBase model)
        {
            switch (model)
            {
                case SimpleConditionModel simple:
                    return new SimpleConditionViewModel
                    {
                        ParameterName = simple.ParameterName,
                        Value = simple.Value,
                        SelectedItemLogic = simple.SelectedLogic,
                        RemoveSimpleRequested = vm => FilterConditions.Remove(vm)
                    };

                case GroupConditionModel group:
                    var groupVM = new GroupConditionViewModel
                    {
                        RemoveGroupRequested = vm => FilterConditions.Remove(vm)
                    };

                    foreach (var child in group.Children)
                    {
                        var childVM = FromConditionModel(child);
                        if (childVM != null)
                            groupVM.Children.Add(childVM);
                    }

                    return groupVM;

                default:
                    return null;
            }
        }

    }
}
