using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace sh01Zahorulko
{
    public class Command<T> : ICommand
    {

        private readonly Action<T?> execute;
        private readonly Predicate<T?>? canExecute;

        public Command(Action<T?> execute, Predicate<T?>? canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            return canExecute?.Invoke((T?)parameter) ?? true;
        }

        public void Execute(object? parameter)
        {
            Task.Run( () => execute((T?)parameter));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(T? parameter)
        { 
            Execute(parameter);
        }

    }
}
