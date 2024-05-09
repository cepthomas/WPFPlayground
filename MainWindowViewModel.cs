using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace .WPFPlayground
{
    public class MainWindowViewModel : ViewModelBase
    {
        /// Commands
        public ICommand ChangeText { get; }

        public ICommand ChangeColor { get; }

        /// Properties.
        string _color = "LightGreen";
        public string MyColor { get => _color; set => SetField(ref _color, value); }

        string _string = "Nada";
        public string MyString { get => _string; set => SetField(ref _string, value); }

        int _val = 9;
        public int MyVal { get => _val; set => SetField(ref _val, value); }

        public MainWindowViewModel()
        {
            /////// Internal fields.
            int _stringIndex = 0;
            int _colorIndex = 0;
            string[] colors = { "LightSalmon", "LightBlue", "Yellow", "LightGreen" };

            ////// Init command handlers.

            ChangeText = new RelayCommand(
                canExecute =>
                {
                    return true;
                },
                execute =>
                {
                    _stringIndex++;
                    MyString = $"Hey, you Clicked me {++_stringIndex}";
                });

            ChangeColor = new RelayCommand(
                canExecute =>
                {
                    return true;
                },
                execute =>
                {
                    MyColor = colors[_colorIndex % colors.Length];
                    _colorIndex++;
                });
        }
    }
}
