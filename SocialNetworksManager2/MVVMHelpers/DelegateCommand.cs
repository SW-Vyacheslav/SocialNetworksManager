using System;
using System.Windows.Input;

namespace SocialNetworksManager2.MVVMHelpers
{
    public class DelegateCommand : ICommand
    {
        readonly Action<object> _execute;
        readonly Predicate<object> _can_execute;

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public DelegateCommand(Action<object> execute) : this(execute, null) { }

        public DelegateCommand(Action<object> execute, Predicate<object> can_execute)
        {
            if (execute == null) throw new ArgumentNullException("execute");

            _execute = execute;
            _can_execute = can_execute;
        }

        public bool CanExecute(object parameter)
        {
            return _can_execute?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
