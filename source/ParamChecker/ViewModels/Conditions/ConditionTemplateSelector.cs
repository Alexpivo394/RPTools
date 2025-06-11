using System.Windows;
using System.Windows.Controls;

namespace ParamChecker.ViewModels.Conditions;

public class ConditionTemplateSelector : DataTemplateSelector
{
    public DataTemplate SimpleTemplate { get; set; }
    public DataTemplate GroupTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        return item switch
        {
            SimpleConditionViewModel => SimpleTemplate,
            GroupConditionViewModel => GroupTemplate,
            _ => base.SelectTemplate(item, container)
        };
    }
}