using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using WpfMvvm.Commands;
using WpfMvvm.Converters;
using WpfMvvm.ViewModel;

namespace WpfMvvm.WpfCommands
{
    /// <summary>Производный от <see cref="RelayCommand"/> класс
    /// с обобщённым параметром.</summary>
    /// <typeparam name="T">Тип параметра методов команды.</typeparam>
    public class RelayCommand<T> : RelayCommand, IRelayCommand, Commands.IRelayCommand, ICommandRaise, ICommand, IDispatcher
    {

        /// <summary>Метод, который определяет, может ли данная команда выполняться в ее текущем состоянии.</summary>
        /// <param name="parameter">Команда требует параметр в типе конвертируемом в тип <typeparamref name="T"/>.<br/>
        /// Если он не конвертируется - всегда будет возвращаться <see langword="false"/>.<br/>
        /// Если передан <see langword="null"/>, то проверяется тип <see cref=" Commands.RelayCommand.ParameterType"/> на принадлежность к значимым типам.<br/>
        /// Если принадлежит - возвращается <see langword="false"/>; иначе - результат выполнения метода переданного по делегату с default параметром.</param>
        /// <returns> Значение <see langword="true"/>, если эту команду можно выполнить; в противном случае — значение <see langword="false"/>.</returns>
        /// <remarks> Cначала <paramref name="parameter"/> приводится по шаблону к <typeparamref name="T"/>.<br/>
        /// Если приведение не удалось, то производится его попытка конвертации с использованием <see cref="DefaultValueConverter"/>.<para/>
        /// После конвертации вызывается метод из переданного делегата.</remarks>
        public override bool CanExecute(object parameter)
        {
            if (parameter == null)
                return isNullableParameterType && canExecute(default);

            if (parameter is T t)
                return canExecute(t);

            DefaultValueConverter converter = StaticMethodsOfConverters.GetValueTypeConverter(parameter.GetType(), ParameterType);

            if (converter.Convert(parameter, ParameterType, null, CultureInfo.InvariantCulture) is T tt)
                return canExecute(tt);

            return false;
        }

        private readonly bool isNullableParameterType;

        /// <summary>Метод, вызываемый при исполнении данной команды.</summary>
        /// <param name="parameter">Команда требует параметр в типе конвертируемом в тип <typeparamref name="T"/>.
        /// Если он не конвертируется - команда не исполняется.</param>
        /// <remarks> Cначала <paramref name="parameter"/> приводится по шаблону к <typeparamref name="T"/>.<br/>
        /// Если приведение не удалось, то производится его попытка конвертации с использованием <see cref="DefaultValueConverter"/>.<para/>
        /// После конвертации вызывается метод из переданного делегата.</remarks>
        public override void Execute(object parameter)
        {
            if (parameter == null)
            {
                if (isNullableParameterType)
                    execute(default);
            }

            else if (parameter is T t)
            {
                execute(t);
            }
            else
            {
                DefaultValueConverter converter = StaticMethodsOfConverters.GetValueTypeConverter(parameter.GetType(), ParameterType);

                if (converter.Convert(parameter, ParameterType, null, CultureInfo.InvariantCulture) is T tt)
                    execute(tt);
            }
        }

        /// <summary>Делегат метода исполняющего команду.</summary>
        private readonly ExecuteCommandHandler<T> execute;

        /// <summary>Делегат метода проверяющего состояние команды.</summary>
        private readonly CanExecuteCommandHandler<T> canExecute;

        /// <summary>Всегда возвращает <see langword="true"/>.</summary>
        /// <param name="parameter">Параметр команды. Не используется.</param>
        /// <returns>Всегда <see langword="true"/>.</returns>
        /// <remarks>Используется как заглушка, если не передан делегат для метода проверяющего состояние команды.</remarks>
        public static bool AlwaysTrue(T parameter) => true;

        /// <summary>Создаёт экземпляр команды.</summary>
        /// <param name="executeHandler">Делегат метода исполняющего команду.</param>
        /// <param name="canExecuteHandler">Делегат метода проверяющего состояние команды.</param>
        /// <remarks><see cref="Dispatcher"/>=<see cref="Application.Current"/>.<see cref="System.Windows.Threading.Dispatcher"/>.</remarks>
        public RelayCommand(ExecuteCommandHandler<T> executeHandler, CanExecuteCommandHandler<T> canExecuteHandler)
            : this(executeHandler, canExecuteHandler, Application.Current.Dispatcher)
        {
            execute = executeHandler ?? throw ExecuteHandlerNullException;
            canExecute = canExecuteHandler ?? throw CanExecuteHandlerNullException;

            isNullableParameterType = default(T) == null;
        }

        /// <summary>Создаёт экземпляр команды.</summary>
        /// <param name="executeHandler">Делегат метода исполняющего команду.</param>
        /// <param name="canExecuteHandler">Делегат метода проверяющего состояние команды.</param>
        /// <param name="dispatcher">Диспетчер. Не может быть <see langword="null"/>.</param>
        public RelayCommand(ExecuteCommandHandler<T> executeHandler, CanExecuteCommandHandler<T> canExecuteHandler, Dispatcher dispatcher)
            : base(typeof(T), dispatcher)
        {
            execute = executeHandler ?? throw ExecuteHandlerNullException;
            canExecute = canExecuteHandler ?? throw CanExecuteHandlerNullException;

            isNullableParameterType = default(T) == null;
        }

        /// <summary>Создаёт экземпляр команды.</summary>
        /// <param name="executeHandler">Делегат метода исполняющего команду.</param>
        /// <remarks>Метод проверяющий состояние команды замещается методом <see cref="AlwaysTrue(T)"/>.<br/>
        /// <see cref="Dispatcher"/>=<see cref="Application.Current"/>.<see cref="System.Windows.Threading.Dispatcher"/>.</remarks>
        public RelayCommand(ExecuteCommandHandler<T> executeHandler)
            : this(executeHandler, AlwaysTrue)
        { }

        /// <summary>Создаёт экземпляр команды.</summary>
        /// <param name="executeHandler">Делегат метода исполняющего команду.</param>
        /// <remarks>Метод проверяющий состояние команды замещается методом <see cref="AlwaysTrue(T)"/>.</remarks>
        /// <param name="dispatcher">Диспетчер. Не может быть <see langword="null"/>.</param>
        public RelayCommand(ExecuteCommandHandler<T> executeHandler, Dispatcher dispatcher)
            : this(executeHandler, AlwaysTrue, dispatcher)
        { }

    }
}
