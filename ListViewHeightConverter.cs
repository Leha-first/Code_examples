using System;
using System.Globalization;
using System.Windows.Data;

namespace ASCRV.SearchByRequest
{
    /// <summary>
    /// Мультиконвертер для вычесления высоты строки после ее раскрытия
    /// </summary>
    public class ListViewHeightConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return (double) values[0] + (double) values[1] + 10D;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] {value};
        }
    }
}
