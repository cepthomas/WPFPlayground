using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFPlayground
{
    /// <summary>
    /// User settings editor.
    /// </summary>
    public partial class Editor : Window
    {
        public UserSettings Settings { get; set; } = new();

        UserSettings _settingsTemp = new();

        public Editor()
        {
            InitializeComponent();

            // Make a copy for editing. Probably should use binding?
            _settingsTemp.CopyFrom(Settings);
            myPropGrid.SelectedObject = _settingsTemp;
        }

        void Window_Loaded(object sender, EventArgs e)
        {
        }

        private void OnOkButton_Clicked(object sender, RoutedEventArgs e)
        {
            _settingsTemp.CopyTo(Settings);
            DialogResult = true;
            Close();
        }

        private void OnCancelButton_Clicked(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            // Restore
            Close();
        }
    }
}
