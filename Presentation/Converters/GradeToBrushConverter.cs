using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace StudentGradesPortal.Converters;

public sealed class GradeToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            5 => new SolidColorBrush(Color.FromRgb(20, 128, 105)),
            4 => new SolidColorBrush(Color.FromRgb(45, 112, 179)),
            3 => new SolidColorBrush(Color.FromRgb(190, 132, 35)),
            _ => new SolidColorBrush(Color.FromRgb(184, 68, 59))
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
