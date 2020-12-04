using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfMvvm.ViewModel;

namespace WpfMvvm.WpfCommands
{
    // Реализация слабого события
    public partial class RelayCommand : WpfMvvm.Commands.RelayCommand
    {
        // Количество слушатлей события CanExecuteChanged;
        private int ListenersCount = 0;


        /// <inheritdoc cref="WpfMvvm.Commands.RelayCommand.CanExecuteChanged"/>
        public override event EventHandler CanExecuteChanged
        {
            add => listeners.Add(new Listener(value));

            remove => listeners.Remove(new Listener(value));

        }

        // По событию CommandManager.RequerySuggested создаёт событие CanExecuteChanged
        private void CommandManagerRequerySuggested(object sender, EventArgs e)
            => RaiseCanExecuteChanged();

        private readonly HashSet<Listener> listeners = new HashSet<Listener>();

        private readonly object[] args;

        /// <summary>Создаёт экземпляр команды для заданного типа.</summary>
        /// <param name="type">Тип параметра команды.
        /// Для команд без параметра - <see langword="null"/>.</param>
        protected RelayCommand(Type type)
            : base(type)
        {
            args = new object[] { this, EventArgs.Empty };
            CommandManager.RequerySuggested += CommandManagerRequerySuggested;
        }

        /// <summary>Создание события для всех слушателей.</summary>
        public override void RaiseCanExecuteChanged()
        {
            // Список включённых слушателей слушателей.
            List<(Listener lstr, object strongLink)> enabled
                = new List<(Listener lstr, object strongLink)>();

            // Список отвалившихся слушателей.
            List<Listener> toRemove = new List<Listener>();

            // Временная переменая для хранения объекта слушателя.
            (Listener lstr, object strongLink) temp;

            // Блокировка источника события.
            lock (this)
            {
                //  Перебор всех слушателей.
                foreach (Listener listener in listeners)
                {
                    temp = (listener, listener.Target);

                    // Статические слушатели сразу запоминаются.
                    if (temp.lstr.StaticHandler != null)
                    {
                        enabled.Add(temp);
                    }

                    // Для динамических слушателей провверяется объект из слабой ссылки.
                    else
                    {
                        // Если ссылка null - значит слушателя уже нет.
                        if (temp.strongLink == null)
                        {
                            toRemove.Add(listener);
                        }

                        // Запоминание включённых слушателей
                        else
                        {
                            enabled.Add(temp);
                        }
                    }
                }
                temp = default;

                // Удаление из списка несуществующих слушателей. 
                toRemove.ForEach(lstr => listeners.Remove(lstr));

                // Создание события для статический слушатлей и существующих динамических.
                RaiseCanExecuteChanged(enabled);
            }
        }

        private void RaiseCanExecuteChanged(List<(Listener lstr, object strongLink)> enabledListeners)
        {
            if (Dispatcher.CheckAccess())
            {
                (Listener lstr, object strongLink) listener;

                for (int index = 0; index < enabledListeners.Count; index++)
                {
                    listener = enabledListeners[index];
                    if (listener.lstr.StaticHandler != null)
                        listener.lstr.StaticHandler(this, EventArgs.Empty);
                    else
                        listener.lstr.Method.Invoke(listener.strongLink, args);

                    enabledListeners[index] = default;
                }

                listener = default;
            }
            else
                Dispatcher.BeginInvoke((RaiseCanExecuteChangedYandler)RaiseCanExecuteChanged, enabledListeners);
        }

        private delegate void RaiseCanExecuteChangedYandler(List<(Listener lstr, object strongLink)> enabledListeners);
    }
}
