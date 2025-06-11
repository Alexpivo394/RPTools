using System.ComponentModel;

namespace ParamChecker.Models.Filters;

public enum FilterParameterLogic
{
    [Description("И")] And,

    [Description("ИЛИ")] Or
}