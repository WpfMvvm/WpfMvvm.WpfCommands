using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using WpfMvvm.Commands;
using WpfMvvm.ViewModel;

namespace WpfMvvm.WpfCommands
{

    /// <summary>Реализация интерфейса <see cref="IRelayCommand"/> в базовом типе <see cref="Commands.RelayCommand"/>.<br/>
    /// Отличается от него автоматической подпиской на событие <see cref="CommandManager.RequerySuggested"/>
    /// метода <see cref="RaiseCanExecuteChanged()"/> и его выполнением в потоке диспетчера.</summary>
    public class RelayCommand : Commands.RelayCommand, IRelayCommand, IDispatcher, Commands.IRelayCommand, ICommandRaise, ICommand
    {

        /// <summary>Диспетчер в потоке которого должно создаваться событие <see cref="Commands.RelayCommand.RaiseCanExecuteChanged"/></summary>
        public Dispatcher Dispatcher { get; }

        /// <summary>Создаёт экземпляр команды для заданного типа.</summary>
        /// <param name="type">Тип параметра команды.
        /// Для команд без параметра - <see langword="null"/>.</param>
        /// <remarks><see cref="Dispatcher"/>=<see cref="Application.Current"/>.<see cref="System.Windows.Threading.Dispatcher"/>.</remarks>
        protected RelayCommand(Type type)
            : this(type, Application.Current.Dispatcher)
        { }

        /// <summary>Создаёт экземпляр команды для заданного типа и диспетчера.</summary>
        /// <param name="type">Тип параметра команды.
        /// Для команд без параметра - <see langword="null"/>.</param>
        /// <param name="dispatcher">Диспетчер. Не может быть <see langword="null"/>.</param>
        /// <exception cref="DispatcherNullException">Если <paramref name="dispatcher"/>=<see langword="null"/>.</exception>
        protected RelayCommand(Type type, Dispatcher dispatcher)
              : base(type)
        {
            Dispatcher = dispatcher ?? throw DispatcherNullException;
            requerySuggested = RaiseCanExecuteChanged;
            CommandManager.RequerySuggested += requerySuggested;
        }


        // Поле для сильной ссылки на делегат к методу RequerySuggested
        private readonly EventHandler requerySuggested;

        // Обработчик CommandManager.RequerySuggested.
        // Выполняется в том же потоке, что и собыие RequerySuggested.
        private void RaiseCanExecuteChanged(object sender, EventArgs e)
        {
            base.RaiseCanExecuteChanged();
        }

        /// <summary>Создаёт экземпляр команды для методов без параметра.</summary>
        /// <param name="executeHandler">Делегат метода исполняющего команду.</param>
        /// <param name="canExecuteHandler">Делегат метода проверяющего состояние команды.</param>
        /// <param name="dispatcher">Диспетчер. Не может быть <see langword="null"/>.</param>
        public RelayCommand(ExecuteCommandHandler executeHandler, CanExecuteCommandHandler canExecuteHandler, Dispatcher dispatcher)
            : base(executeHandler, canExecuteHandler)
        {
            Dispatcher = dispatcher ?? throw DispatcherNullException;
            requerySuggested = RaiseCanExecuteChanged;
            CommandManager.RequerySuggested += requerySuggested;
        }

        /// <summary>Создаёт экземпляр команды для методов без параметра.</summary>
        /// <param name="executeHandler">Делегат метода исполняющего команду.</param>
        /// <param name="dispatcher">Диспетчер. Не может быть <see langword="null"/>.</param>
        /// <remarks><see cref="Commands.RelayCommand.CanExecute"/>=<see cref="Commands.RelayCommand.AlwaysTrue(object)"/></remarks>
        public RelayCommand(ExecuteCommandHandler executeHandler, Dispatcher dispatcher)
            : this(executeHandler, AlwaysTrue, dispatcher)
        { }

        /// <summary>Создаёт экземпляр команды для методов без параметра.</summary>
        /// <param name="executeHandler">Делегат метода исполняющего команду.</param>
        /// <param name="canExecuteHandler">Делегат метода проверяющего состояние команды.</param>
        /// <remarks><see cref="Dispatcher"/> = <see cref="Application.Current"/>.<see cref="System.Windows.Threading.Dispatcher"/>.</remarks>
        public RelayCommand(ExecuteCommandHandler executeHandler, CanExecuteCommandHandler canExecuteHandler)
             : this(executeHandler, canExecuteHandler, Application.Current.Dispatcher)
        { }

        /// <summary>Создаёт экземпляр команды для методов без параметра.</summary>
        /// <param name="executeHandler">Делегат метода исполняющего команду.</param>
        /// <remarks><see cref="Dispatcher"/>=<see cref="Application.Current"/>.<see cref="System.Windows.Threading.Dispatcher"/>.<br/>
        /// <see cref="Commands.RelayCommand.CanExecute"/> = <see cref="Commands.RelayCommand.AlwaysTrue(object)"/></remarks>
        public RelayCommand(ExecuteCommandHandler executeHandler)
             : this(executeHandler, AlwaysTrue)
        { }

        /// <summary>Создаёт экземпляр команды для методов без параметра.</summary>
        /// <param name="executeHandler">Делегат метода исполняющего команду.</param>
        /// <param name="canExecuteHandler">Делегат метода проверяющего состояние команды.</param>
        /// <param name="dispatcher">Диспетчер. Не может быть <see langword="null"/>.</param>
        public RelayCommand(Action executeHandler, Func<bool> canExecuteHandler, Dispatcher dispatcher)
            : base(executeHandler, canExecuteHandler)
        {
            Dispatcher = dispatcher ?? throw DispatcherNullException;
            requerySuggested = RaiseCanExecuteChanged;
            CommandManager.RequerySuggested += requerySuggested;
        }

        /// <summary>Создаёт экземпляр команды для методов без параметра.</summary>
        /// <param name="executeHandler">Делегат метода исполняющего команду.</param>
        /// <param name="dispatcher">Диспетчер. Не может быть <see langword="null"/>.</param>
        /// <remarks><see cref="Commands.RelayCommand.CanExecute"/>=<see cref="Commands.RelayCommand.AlwaysTrue()"/></remarks>
        public RelayCommand(Action executeHandler, Dispatcher dispatcher)
            : this(executeHandler, AlwaysTrue, dispatcher)
        { }

        /// <summary>Создаёт экземпляр команды для методов без параметра.</summary>
        /// <param name="executeHandler">Делегат метода исполняющего команду.</param>
        /// <param name="canExecuteHandler">Делегат метода проверяющего состояние команды.</param>
        /// <remarks><see cref="Dispatcher"/> = <see cref="Application.Current"/>.<see cref="System.Windows.Threading.Dispatcher"/>.</remarks>
        public RelayCommand(Action executeHandler, Func<bool> canExecuteHandler)
             : this(executeHandler, canExecuteHandler, Application.Current.Dispatcher)
        { }

        /// <summary>Создаёт экземпляр команды для методов без параметра.</summary>
        /// <param name="executeHandler">Делегат метода исполняющего команду.</param>
        /// <remarks><see cref="Dispatcher"/>=<see cref="Application.Current"/>.<see cref="System.Windows.Threading.Dispatcher"/>.<br/>
        /// <see cref="Commands.RelayCommand.CanExecute"/> = <see cref="Commands.RelayCommand.AlwaysTrue()"/></remarks>
        public RelayCommand(Action executeHandler)
             : this(executeHandler, AlwaysTrue)
        { }

        /// <inheritdoc cref="Commands.RelayCommand.RaiseCanExecuteChanged"/>
        public override void RaiseCanExecuteChanged()
        {
            // Если текущий поток не поток Диспетчера, 
            // то передача создания события в очередь диспетчера.

            if (Dispatcher.CheckAccess())
                base.RaiseCanExecuteChanged();
            else
                Dispatcher.BeginInvoke((Action)base.RaiseCanExecuteChanged);
        }

        /// <summary>Ошибка возникающая при передаче в конструктор <see cref="RelayCommand"/> параметра dispatcher = <see langword="null"/>.</summary>
        public static ArgumentNullException DispatcherNullException = new ArgumentNullException("dispatcher");
    }
}
