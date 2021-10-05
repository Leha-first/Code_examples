using System;
using System.Windows;
using System.Windows.Controls;

namespace ASCRV.SearchByRequest
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Делегат при редактировании сохраненного запроса
        /// </summary>
        /// <param name="queryId"> Идентификатор запроса </param>
        /// <param name="typeQuery"> Тип запроса (P - персональный, C - общий) </param>
        public delegate void ActionsWithQueryButtonDelegate(Guid queryId, string typeQuery);
        /// <summary>
        /// Событие нажатия кнопки "Редактировать" на запросе
        /// </summary>
        public event ActionsWithQueryButtonDelegate ClickOnEditQueryButtonDelegateEventHandler;
        /// <summary>
        /// Событие нажатия кнопки "Редактировать" на запросе
        /// </summary>
        public event ActionsWithQueryButtonDelegate ClickOnRemoveQueryButtonDelegateEventHandler;

        /// <summary>
        /// Конструктор представления
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработка нажатия на кнопку "Редактировать" на общем запросе
        /// </summary>
        /// <param name="sender"> Dynamically's created button's </param>
        /// <param name="e"> Данные о событии RoutedEventArgs </param>
        private void ClickOnEditCommonQueryButton(object sender, RoutedEventArgs e) =>
            ClickOnEditQueryButtonDelegateEventHandler?.Invoke((Guid)((Button) sender).Tag, "C");

        /// <summary>
        /// Обработка нажатия на кнопку "Редактировать" на персоанльном запросе
        /// </summary>
        /// <param name="sender"> Dynamically's created button's </param>
        /// <param name="e"> Данные о событии RoutedEventArgs </param>
        private void ClickOnEditPersonalQueryButton(object sender, RoutedEventArgs e) =>
            ClickOnEditQueryButtonDelegateEventHandler?.Invoke((Guid) ((Button) sender).Tag, "P");

        /// <summary>
        /// Обработка нажатия на кнопку "Удалить" на общем запросе
        /// </summary>
        /// <param name="sender"> Dynamically's created button's </param>
        /// <param name="e"> Данные о событии RoutedEventArgs </param>
        private void ClickOnRemoveCommonQueryButton(object sender, RoutedEventArgs e) =>
            ClickOnRemoveQueryButtonDelegateEventHandler?.Invoke((Guid) ((Button) sender).Tag, "C");

        /// <summary>
        /// Обработка нажатия на кнопку "Редактировать" на персоанльном запросе
        /// </summary>
        /// <param name="sender"> Dynamically's created button's </param>
        /// <param name="e"> Данные о событии RoutedEventArgs </param>
        private void ClickOnRemovePersonalQueryButton(object sender, RoutedEventArgs e) =>
            ClickOnRemoveQueryButtonDelegateEventHandler?.Invoke((Guid) ((Button) sender).Tag, "P");
    }
}

