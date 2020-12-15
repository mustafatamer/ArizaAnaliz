using System;

namespace ArizaAnaliz.ViewModels
{
    public class DelegateCommand : System.Windows.Input.ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action _execute;
        public DelegateCommand(Action execute = null, bool v = false)
            : this(execute, null)
        {
        }
        public DelegateCommand(Action execute,
                               Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        #region ICommand Members

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            return _canExecute(parameter);
        }

        public void Execute(object parameter = null)
        {
            _execute();
            //_execute(parameter);
        }

        #endregion

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }

}
