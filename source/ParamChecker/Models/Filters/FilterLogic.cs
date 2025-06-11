using System.ComponentModel;

namespace ParamChecker.Models.Filters;

public enum FilterLogic
{
    [Description("Равно")] Equals,

    [Description("Не равно")] NotEquals,

    [Description("Больше")] GreaterThan,

    [Description("Больше или равно")] GreaterThanOrEquals,

    [Description("Меньше")] LessThan,

    [Description("Меньше или равно")] LessThanOrEquals,

    [Description("Содержит")] Contains,

    [Description("Не содержит")] NotContains,

    [Description("Существует")] Exists,

    [Description("Не существует")] NotExists
}