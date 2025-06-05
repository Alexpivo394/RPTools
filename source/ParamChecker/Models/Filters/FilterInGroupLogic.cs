using System.ComponentModel;

namespace ParamChecker.Models.Filters;

public enum FilterInGroupLogic
{
    [Description("И")]
    And,

    [Description("ИЛИ")]
    Or
}