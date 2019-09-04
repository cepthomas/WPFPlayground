using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFPlayground
{
    public class MyViewModel : INotifyPropertyChanged
    {
        // TODO improve boilerplate.

        protected void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        string _color = "LightGreen";
        public string MyColor
        {
            get => _color;
            set => SetField(ref _color, value);
        }

        string _string = "Nada";
        public string MyString
        {
            get => _string;
            set => SetField(ref _string, value);
        }

        int _val = 9;
        public int MyVal
        {
            get => _val;
            set => SetField(ref _val, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand ChangeText { get; set; }
        public ICommand ChangeColor { get; set; }

        public MyViewModel()
        {
            // Init command handlers.
            ChangeText = new ChangeTextCommand { mwv = this };
            ChangeColor = new ChangeColorCommand { mwv = this };
        }
    }


    public class ChangeColorCommand : ICommand
    {
        public MyViewModel mwv;
        string[] colors = { "LightSalmon", "LightBlue", "LightYellow", "LightGreen" };
        private int i = 0;

        public bool CanExecute(object parameter)
        {
            
            return true;
        }

        // Occurs when changes occur that affect whether or not the command should execute.
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            mwv.MyColor = colors[i % colors.Count()];
            i++;
        }
    }

    public class ChangeTextCommand : ICommand
    {
        public MyViewModel mwv;
        private int i = 0;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        // Occurs when changes occur that affect whether or not the command should execute.
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            i++;
            mwv.MyString = "Hey, you Clicked me " + i.ToString();
        }
    }
}




/*

public class RelayCommand : ICommand
{
    private Predicate<object> _canExecute;
    private Action<object> _execute;

    public RelayCommand(Predicate<object> canExecute, Action<object> execute)
    {
        this._canExecute = canExecute;
        this._execute = execute;
    }

    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public bool CanExecute(object parameter)
    {
        return _canExecute(parameter);
    }

    public void Execute(object parameter)
    {
        _execute(parameter);
    }
}

This could then be used as...

public class MyViewModel
{
    private ICommand _doSomething;
    public ICommand DoSomethingCommand
    {
        get
        {
            if (_doSomething == null)
            {
                _doSomething = new RelayCommand(
                    p => this.CanDoSomething,
                    p => this.DoSomeImportantMethod());
            }
            return _doSomething;
        }
    }
}

*/
