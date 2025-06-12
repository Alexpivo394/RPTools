using System.ComponentModel;

namespace ParamChecker.Models.Filters;

public enum CategoryParameterLogic
{
    [Description("Только категории")] CategoriesOnly,

    [Description("Только параметры")] ParametersOnly,

    [Description("Категории ИЛИ Параметры")] CategoriesOrParameters,

    [Description("Категории И Параметры")] CategoriesAndParameters
}