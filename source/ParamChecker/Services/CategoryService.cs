namespace ParamChecker.Services
{
    public class CategoryService
    {
        private readonly Dictionary<BuiltInCategory, string> _localizedCategories = new();

        public void Initialize(Document doc)
        {
            _localizedCategories.Clear();

            foreach (Category category in doc.Settings.Categories)
            {
                if (Enum.IsDefined(typeof(BuiltInCategory), category.Id.IntegerValue))
                {
                    var builtInCategory = (BuiltInCategory)category.Id.IntegerValue;
                    _localizedCategories[builtInCategory] = category.Name;
                }
            }
        }

        public string GetCategoryName(BuiltInCategory category)
        {
            return _localizedCategories.TryGetValue(category, out var name) ? name : category.ToString();
        }

        public IEnumerable<BuiltInCategory> GetAllCategories()
        {
            return _localizedCategories.Keys.ToList();
        }
    }
}