using System;
using System.Globalization;
using System.Windows.Data;
using StudentGradesPortal.Models;

namespace StudentGradesPortal.Converters;

public sealed class GradeStatusToTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            GradeStatusFilter.Excellent => "Отличники",
            GradeStatusFilter.Passing => "Без долгов",
            GradeStatusFilter.Failed => "Пересдача",
            GradeStatusFilter.Final => "Итоговые",
            _ => "Все записи"
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
