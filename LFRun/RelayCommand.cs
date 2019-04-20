using System;
using System.Windows.Input;

namespace LFRun
{
    /// <summary>
    /// A generic command that can be wired into the MVVM pattern.
    /// </summary>
    /// <typeparam name="T">The type of data used by the command</typeparam>
    public class RelayCommand<T> : ICommand
        where T : class
    {
        #region Fields

        private readonly Action<T> _execute = null;
        private readonly Predicate<T> _canExecute = null;

        #endregion

        #region  Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="RelayCommand{T}"/>.
        /// </summary>
        /// <param name="execute">Delegate to execute when Execute is called on the command. This can be null just to hook up a CanExecute delegate.</param>
        /// <remarks><seealso cref="CanExecute"/> will always return true.</remarks>
        public RelayCommand(Action<T> execute)
            :this(execute, null)
        { }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        #endregion

        #region ICommand Members

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does nto require data to be passed, this object can be set to null.</param>
        /// <returns>True if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke((T)parameter) ?? true;
        }

        /// <summary>
        /// Occurs when conditions change that affect whether or not the command should execute
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameter)
        {
            _execute?.Invoke((T) parameter);
        }

        #endregion
    }

    public class RelayCommand : RelayCommand<object>
    {
        #region  Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="RelayCommand{object}"/>.
        /// </summary>
        /// <param name="execute">Delegate to execute when Execute is called on the command. This can be null just to hook up a CanExecute delegate.</param>
        /// <remarks><seealso cref="CanExecute"/> will always return true.</remarks>
        public RelayCommand(Action<object> execute)
            : base(execute, null)
        { }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
            :base(execute, canExecute)
        {

        }

        #endregion
    }
}
