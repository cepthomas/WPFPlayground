using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace Ephemera.WPFPlayground
{
    /// <summary>
    /// Common stuff for MV.
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// Makes command neat and clean.
    /// </summary>
    public class RelayCommand : ICommand
    {
        readonly Predicate<object?> _canExecute;
        readonly Action<object?> _execute;

        public RelayCommand(Predicate<object?> canExecute, Action<object?> execute)
        {
            _canExecute = canExecute;
            _execute = execute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute is null || _canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }
    }


    ///////////////////////////////////// from naudio //////////////////////////////////
    /*

    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }


public class RelayCommand_NA : ICommand
{
    private readonly Action<object> execute;
    private readonly Predicate<object> canExecute;

    public RelayCommand_NA(Action<object> execute)
        : this(execute, null)
    {
    }

    public RelayCommand_NA(Action execute, Func<bool> canExecute)
        : this((s) => execute(), (s) => canExecute())
    {

    }

    public RelayCommand_NA(Action execute)
        : this((s) => execute(), null)
    {

    }

    public RelayCommand_NA(Action<object> execute, Predicate<object> canExecute)
    {
        this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
        this.canExecute = canExecute;
    }

    [DebuggerStepThrough]
    public bool CanExecute(object parameter)
    {
        return canExecute == null || canExecute(parameter);
    }

    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public void Execute(object parameter)
    {
        execute(parameter);
    }
}


class DelegateCommand : ICommand
{
    private readonly Action action;
    private bool isEnabled;

    public DelegateCommand(Action action)
    {
        this.action = action;
        isEnabled = true;
    }

    public void Execute(object parameter)
    {
        action();
    }

    public bool CanExecute(object parameter)
    {
        return isEnabled;
    }

    public bool IsEnabled
    {
        get
        {
            return isEnabled;
        }
        set
        {
            if (isEnabled != value)
            {
                isEnabled = value;
                OnCanExecuteChanged();
            }
        }
    }

    public event EventHandler CanExecuteChanged;

    protected virtual void OnCanExecuteChanged()
    {
        EventHandler handler = CanExecuteChanged;
        if (handler != null) handler(this, EventArgs.Empty);
    }
}

////// Use Like this:
class AudioPlaybackViewModel : ViewModelBase//, IDisposable
{
    public ICommand PlayCommand { get; }

    public AudioPlaybackViewModel(IEnumerable<IVisualizationPlugin> visualizations)
    {
        PlayCommand = new DelegateCommand(Play);

        this.PropertyChanged += OnPropertyChanged;
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
        equalizer?.Update();
    }

    private void Play()
    {
        if (this.selectedFile == null)
        {
            OpenFile();
        }
        if (this.selectedFile != null)
        {
            audioPlayback.Play();
        }
    }

}

    public IVisualizationPlugin SelectedVisualization
    {
        get
        {
            return this.selectedVisualization;
        }
        set
        {
            if (this.selectedVisualization != value)
            {
                this.selectedVisualization = value;
                OnPropertyChanged("SelectedVisualization");
                OnPropertyChanged("Visualization");
            }
        }
    }

        public class RelayCommand_NAXXX : ICommand
    {
        private readonly Action<object> execute;
        private readonly Predicate<object> canExecute;

        public RelayCommand_NAXXX(Action<object> execute)
            : this(execute, null)
        {
        }

        public RelayCommand_NAXXX(Action execute, Func<bool> canExecute)
            : this((s) => execute(), (s) => canExecute())
        {

        }

        public RelayCommand_NAXXX(Action execute)
            : this((s) => execute(), null)
        {

        }

        public RelayCommand_NAXXX(Action<object> execute, Predicate<object> canExecute)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        //[DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }

*/


}
