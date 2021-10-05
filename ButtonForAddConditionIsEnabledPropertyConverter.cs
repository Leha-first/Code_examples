using System;
using System.Linq;
using System.Windows.Data;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;

namespace ASCRV.SearchByRequest.Converters
{
    public class ButtonForAddConditionIsEnabledPropertyConverter: IValueConverter
    {
        /// <summary>
        /// Конвертер преобразования наличия выбранных графострок в состояние включенности
        /// </summary>
        /// <param name="value"> Значение </param>
        /// <param name="targetType"> Целевой тип </param>
        /// <param name="parameter"> Параметр </param>
        /// <param name="culture"> Экземпляр культуры </param>
        /// <returns> Значение активации элемента в пользовательском интерфейсе </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;
            var rowGraphsCount = (int)value;
            return rowGraphsCount > 0;
        }

        /// <summary>
        /// Обратная конвертация
        /// </summary>
        /// <param name="value"> Значение </param>
        /// <param name="targetType"> Целевой тип </param>
        /// <param name="parameter"> Параметр </param>
        /// <param name="culture"> Экезмпляр культуры </param>
        /// <returns> Значение активации элемента в пользовательском интерфейсе </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
