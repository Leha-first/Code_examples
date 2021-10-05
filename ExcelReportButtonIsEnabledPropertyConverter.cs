using System;
using System.Windows.Data;
using System.Globalization;

namespace ASCRV.SearchByRequest.Converters
{
    /// <summary>
    /// Конвертер режима активации кнопки "Сформировать Excel-отчет"
    /// </summary>
    public class ExcelReportButtonIsEnabledPropertyConverter : IValueConverter
    {
        /// <summary>
        /// Конвертер для преобразования режима активации элемента в пользовательском интерфейсе
        /// на основе заполненности коллекции с данными
        /// </summary>
        /// <param name="value"> Количество элементов DanData.Count </param>
        /// <param name="targetType"> Целевой тип </param>
        /// <param name="parameter"> Параметр </param>
        /// <param name="culture"> Экземпляр культуры </param>
        /// <returns> Логическое значение - привязка на свойство зависимостей IsEnabled кнопки </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;
            var danDataCountElements = (int)value;
            return danDataCountElements > 0;
        }

        /// <summary>
        /// Обратная конвертация
        /// </summary>
        /// <param name="value"> Количество элементов DanData.Count </param>
        /// <param name="targetType"> Целевой тип </param>
        /// <param name="parameter"> Параметр </param>
        /// <param name="culture"> Экземпляр культуры </param>
        /// <returns> Значение по умолчанию </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }
}
