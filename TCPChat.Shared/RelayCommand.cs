using System.Windows.Input;

namespace TCPChat.Shared
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;
        private bool _isExecuting;
        private EventHandler _canExecuteChanged;

        public event EventHandler CanExecuteChanged
        {
            add
            { 
                _canExecuteChanged += value; 
            }
            remove 
            { 
                _canExecuteChanged -= value; 
            }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return !_isExecuting && (_canExecute == null || _canExecute(parameter));
        }

        public void Execute(object parameter)
        {
            if (_isExecuting)
                return;

            try
            {
                _isExecuting = true;
                _canExecuteChanged?.Invoke(this, EventArgs.Empty);

                _execute(parameter);
            }
            finally
            {
                _isExecuting = false;
                _canExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
