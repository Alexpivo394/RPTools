using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckLOI.Services
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
                    _localizedCategories[builtInCategory] = category.Name; // Должно быть русское название

                }
            }
        }

        public string GetCategoryName(BuiltInCategory category)
        {
            return _localizedCategories.TryGetValue(category, out var name) ? name : category.ToString();
        }
    }


}
