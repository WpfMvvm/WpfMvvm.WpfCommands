using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using WpfMvvm.ViewModel;

namespace WpfMvvm.WpfCommands
{
    public class RelayCommand : Commands.RelayCommand, IDispatcher
    {
        /// <summary>Диспетчер в потоке которого должно создаваться событие <see cref="Commands.RelayCommand.RaiseCanExecuteChanged"/></summary>
        public Dispatcher Dispatcher { get; }
    }
}
