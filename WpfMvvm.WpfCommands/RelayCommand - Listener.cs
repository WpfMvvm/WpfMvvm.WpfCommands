using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfMvvm.ViewModel;

namespace WpfMvvm.WpfCommands
{
    // Структура для слыбых ссыло на методы прослушек
    public partial class RelayCommand : WpfMvvm.Commands.RelayCommand
    {
        /// <summary>Структура-обёртка для слабых ссылок на слушателей.</summary>
        public struct Listener : IEquatable<Listener>
        {
            /// <summary>Создаёт экземпляр для делегата слушателя.</summary>
            /// <param name="handler"></param>
            public Listener(EventHandler handler)
                : this()
            {
                if (handler.Target == null)
                {
                    StaticHandler = handler;
                }
                else
                {
                    Method = handler.Method;

                    target = new WeakReference(handler.Target);
                }

            }

            /// <summary>Объект слушатель. Для статических методов null.</summary>
            public object Target => target?.Target;

            /// <summary>Метод слушатель. Для статических методов null.</summary>
            public MethodInfo Method { get; private set; }
            WeakReference target;

            /// <summary>Делегат для статических методов.</summary>
            public EventHandler StaticHandler { get; }

            /// <summary>Сравнение с любым объектом.</summary>
            /// <param name="obj">Сравниваемый объект.</param>
            /// <returns>true если объекты эквивалентны.</returns>
            public override bool Equals(object obj)
            {
                return obj is Listener listener && Equals(listener);
            }

            /// <summary>Сравнение с другим <see cref="Listener"/>.</summary>
            /// <param name="other">Другой <see cref="Listener"/>.</param>
            /// <returns>true если объекты эквивалентны.</returns>
            public bool Equals(Listener other)
            {
                if (target == null)
                    return other.target == null && EqualityComparer<MethodInfo>.Default.Equals(Method, other.Method);

                return other.target != null &&
                        EqualityComparer<object>.Default.Equals(Target, other.Target) &&
                        EqualityComparer<MethodInfo>.Default.Equals(Method, other.Method);
            }

            /// <inheritdoc cref="object.GetHashCode"/>
            public override int GetHashCode()
            {
                int hashCode = 616744909;

                if (target == null)
                    return hashCode * -1521134295 + EqualityComparer<EventHandler>.Default.GetHashCode(StaticHandler);


                hashCode = hashCode * -1521134295 + EqualityComparer<object>.Default.GetHashCode(Target);
                hashCode = hashCode * -1521134295 + EqualityComparer<MethodInfo>.Default.GetHashCode(Method);
                return hashCode;
            }
        }
    }
}
