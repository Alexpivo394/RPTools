using System.Globalization;
using System.Windows.Data;
using ParamChecker.Models.Filters;
using Visibility = System.Windows.Visibility;

namespace ParamChecker.Views.Converters;

public class ConditionTypeToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is FilterConditionType type && parameter is string param)
        {
            return type == (param == "Group" ? FilterConditionType.Group : FilterConditionType.Simple) 
                ? Visibility.Visible 
                : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}