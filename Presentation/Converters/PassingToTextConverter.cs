using System;
using System.Globalization;
using System.Windows.Data;

namespace StudentGradesPortal.Converters;

public sealed class PassingToTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is true ? "Зачтено" : "Пересдача";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
