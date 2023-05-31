using System;
using System.Windows.Input;
using System.Windows.Threading;

namespace CuSharp.MandelbrotExample;

public class RelayCommand : ICommand
{
    private readonly Action<object> _execute;
    private readonly Func<object, bool> _canExecute;
    private Dispatcher _dispatcher;
    public RelayCommand(Action<object> execute, Func<object, bool> canExecute, Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => _canExecute(parameter);

    public void Execute(object? parameter) => _execute(parameter);

    public event EventHandler CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
        _dispatcher.Invoke(() => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
    }
}