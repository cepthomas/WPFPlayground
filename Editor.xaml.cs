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
    /// Interaction logic for Editor.xaml
    /// </summary>
    public partial class Editor : Window
    {
        public UserSettings Settings { get; set; } = null;

        UserSettings _settingsTemp = new UserSettings();

        public Editor()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Make a copy for editing. Probably should use binding?
            _settingsTemp.CopyFrom(Settings);
            myPropGrid.SelectedObject = _settingsTemp;
        }

        private void OnOkButtonClicked(object sender, RoutedEventArgs e)
        {
            _settingsTemp.CopyTo(Settings);
            DialogResult = true;
            Close();
        }

        private void OnCancelButtonClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            // Restore
            Close();
        }
    }
}
