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
    public class RelayCommand : WpfMvvm.Commands.RelayCommand, IDispatcher
    {
        // Класс обёртка для слабых ссылок на слушателей.
        private class WeakDelegate
        {
            public WeakReference Target;
            public MethodInfo Method;
        }

        // Количество слушатлей события CanExecuteChanged;
        private int ListenersCount = 0;

        private readonly List<WeakDelegate> listeners = new List<WeakDelegate>();

        /// <inheritdoc cref="WpfMvvm.Commands.RelayCommand.CanExecuteChanged"/>
        public override event EventHandler CanExecuteChanged
        {
            add
            {
                listeners.RemoveAll(wd => !wd.Target.IsAlive);
                if (!listeners.Any(wd => wd.Target == value.Target && wd.Method == value.Method))
                {
                    if (ListenersCount == 0)
                    {
                        CommandManager.RequerySuggested += CommandManagerRequerySuggested;
                    }
                    listeners.Add(new WeakDelegate
                    {
                        Target = new WeakReference(value.Target),
                        Method = value.Method
                    });
                }
            }
            remove
            {
                listeners.RemoveAll(wd => !wd.Target.IsAlive);
                int index = listeners.FindIndex(wd => wd.Target == value.Target && wd.Method == value.Method);
                if (index >= 0)
                {
                    listeners.RemoveAt(index);

                    if (listeners.Count == 0)
                    {
                        CommandManager.RequerySuggested -= CommandManagerRequerySuggested;
                    }
                }

            }
        }

        // По событию CommandManager.RequerySuggested создаёт событие CanExecuteChanged
        private void CommandManagerRequerySuggested(object sender, EventArgs e)
            => RaiseCanExecuteChanged();


        /// <inheritdoc cref="WpfMvvm.Commands.RelayCommand.RaiseCanExecuteChanged"/>
        public override void RaiseCanExecuteChanged()
        {
            lock (listeners)
            {
                object target;
                object[] args = { this, EventArgs.Empty };
                foreach (var wd in listeners)
                {
                    target = wd.Target.Target;
                    if (target == null)
                    {
                        wd.Target = null;
                    }
                    else
                    {
                        wd.Method.Invoke(target, args);
                    }
                }
                listeners.RemoveAll(wd => wd.Target == null);
            }
        }

        private readonly List<WeakDelegate> clickSubscribers = new List<WeakDelegate>();

        public void FireClick()
        {
            // List<WeakDelegate> toRemove = new List<WeakDelegate>(); лишнее

            foreach (WeakDelegate subscriber in clickSubscribers)
            {
                object target = subscriber.Target.Target;
                if (target == null)
                {
                    //  toRemove.Add(subscriber);
                    subscriber.Target = null;
                }
                else
                {
                    subscriber.Method.Invoke(target, new object[] { this, EventArgs.Empty });
                }
            }

            clickSubscribers.RemoveAll(weak => weak.Target == null);
        }
    }
}
