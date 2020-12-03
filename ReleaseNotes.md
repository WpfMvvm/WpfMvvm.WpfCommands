# V.0.0.0.1 [Ещё не опубликован]
Версия первой публикации для темы [Библиотека элементов для реализации WPF MVVM Решений](https://www.cyberforum.ru/wpf-silverlight/thread2738784.html).

В составе пакета:
1. ExecuteCommandHandler - Делегат исполнительного метода команды без параметра.
2. CanExecuteCommandHandler - Делегат метода проверяющего состояние команды без параметра.
3. ExecuteCommandHandler<T> - Делегат исполнительного метода команды с обобщённым параметром.
4. CanExecuteCommandHandler<T> - Делегат метода проверяющего состояние команды с обобщённым параметром.
5. ICommandRaise - Интерфейс добавляющий в интерфейс ICommand метод RaiseCanExecuteChanged().
7. IRelayCommand - Интерфейс добавляющий в интерфейс ICommandRaise свойство с типом параметра.
8. RelayCommand - Базовая реализация команды с интерфейсом IRelayCommand для методов без параметра.
9. RelayCommand<T> - Производная от RelayCommand реализация команды для методов с обобщённым параметром.

