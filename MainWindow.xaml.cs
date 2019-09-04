using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace WPFPlayground
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// These happen in this order.
    ///   Window_Initialized
    ///   MainWindow
    ///   OnRender
    ///   OnRender
    ///   Window_SizeChanged
    ///   Window_Loaded
    ///   Window_ContentRendered
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer _dispTimer = new DispatcherTimer();
        UserSettings _settings = null;
        Random _rand = new Random();

        #region Lifecycle
        public MainWindow()
        {
            InitializeComponent();
            AddInfoLine($"MainWindow constructor");

            _settings = UserSettings.Load(@"..\..\settings.json");

            Left = _settings.MainWindowInfo.X;
            Top = _settings.MainWindowInfo.Y;
            Width = _settings.MainWindowInfo.Width;
            Height = _settings.MainWindowInfo.Height;

            _dispTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            _dispTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            _dispTimer.Start();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            AddInfoLine($"Window_Initialized");
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AddInfoLine($"Window_SizeChanged");

            _settings.MainWindowInfo.FromWindow(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AddInfoLine($"Window_Loaded");
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            AddInfoLine($"Window_ContentRendered");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _settings.Save();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {

        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            AddInfoLine($"OnRender");
            base.OnRender(drawingContext);
            // Could draw stuff like GDI here. But shouldn't.
        }
        #endregion

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            //myCanvas.Children.Clear();
            DrawStuff();
        }

        private void DrawStuff()
        {
            Rectangle rect = new Rectangle
            {
                Width = 100,
                Height = 50,
                Stroke = Brushes.Red,
                StrokeThickness = 5
            };

            Canvas.SetLeft(rect, _rand.Next(10, (int)myCanvas.ActualWidth - 120));
            Canvas.SetTop(rect, _rand.Next(10, (int)myCanvas.ActualHeight - 60));
            myCanvas.Children.Add(rect);
        }

        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MyViewModel vm = DataContext as MyViewModel;

            AddInfoLine($"ellipse1_MouseDown:{vm.MyVal}");

            if (vm.MyVal > 1)
            {
                vm.MyVal--;
            }
        }

        void AddInfoLine(string s)
        {
            infobox.AppendText($"{s}{Environment.NewLine}");
            infobox.ScrollToEnd();
        }

        private void OnSettingsClicked(object sender, RoutedEventArgs e)
        {
            var dlg = new Editor { Owner = this, Settings = _settings };

            if (dlg.ShowDialog().Value)
            {
                // Changes made - apply.

                _settings = dlg.Settings;
                _settings.Save();
            }
        }
    }
}
