using System.Windows.Input;
using WpfMvvm.Commands;
using WpfMvvm.ViewModel;

namespace WpfMvvm.WpfCommands
{
    /// <summary>Интерфейс команд для свойств ViewModel предназначенных для привязок в View.<br/>
    /// Объединяет в один интерфейс <see cref="Commands.IRelayCommand"/> и <see cref="IDispatcher"/>.</summary>
    public interface IRelayCommand : Commands.IRelayCommand, ICommandRaise, ICommand, IDispatcher
    { }
}
