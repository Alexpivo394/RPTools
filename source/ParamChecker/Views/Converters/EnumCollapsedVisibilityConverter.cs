#nullable enable
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using Visibility = System.Windows.Visibility;

namespace ParamChecker.Views.Converters;

public class EnumCollapsedVisibilityConverter<TEnum> : MarkupExtension, IValueConverter where TEnum : Enum
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not TEnum valueEnum) throw new ArgumentException($"{nameof(value)} is not type: {typeof(TEnum)}");

        if (parameter is not TEnum parameterEnum)
            throw new ArgumentException($"{nameof(parameter)} is not type: {typeof(TEnum)}");

        return EqualityComparer<TEnum>.Default.Equals(valueEnum, parameterEnum)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}