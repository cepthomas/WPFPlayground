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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Wpf3dLib;

namespace WPFPlayground
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// These happen in this order.
    ///   Window_Initialized
    ///   MainWindow
    ///   OnRender
    ///   Window_SizeChanged
    ///   Window_Loaded
    ///   Window_ContentRendered
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer _slowTimer = new DispatcherTimer();
        DispatcherTimer _fastTimer = new DispatcherTimer();
        UserSettings _settings = null;
        Random _rand = new Random();

        MyVisualHostDynamic _vhd = new MyVisualHostDynamic();

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

            _slowTimer.Tick += new EventHandler(SlowTimer_Tick);
            _slowTimer.Interval = TimeSpan.FromSeconds(1.0);
            _slowTimer.Start();

            _fastTimer.Tick += new EventHandler(FastTimer_Tick);
            _fastTimer.Interval = TimeSpan.FromSeconds(1.0 / 60.0);
            _fastTimer.Start();

            // Raw drawing.
            var vhs = new MyVisualHostStatic();
            myCanvasDrawingStatic.Children.Add(vhs);

            myCanvasDrawingDynamic.Children.Add(_vhd);
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

            // Window_Loaded3D(sender, e);
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

        protected override void OnRender(DrawingContext drawingContext)
        {
            AddInfoLine($"OnRender");
            base.OnRender(drawingContext);
            // Could draw stuff like GDI here. But shouldn't.
        }
        #endregion


        private void FastTimer_Tick(object sender, EventArgs e)
        {
            _vhd.Update();
        }

        private void SlowTimer_Tick(object sender, EventArgs e)
        {
            //myCanvasShape.Children.Clear();

            Color clr = Color.FromRgb((byte)_rand.Next(0, 255), (byte)_rand.Next(0, 255), (byte)_rand.Next(0, 255));

            Rectangle rect = new Rectangle
            {
                Width = 100,
                Height = 50,
                Stroke = new SolidColorBrush(clr),
                StrokeThickness = 5
            };

            Canvas.SetLeft(rect, _rand.Next(10, (int)myCanvasShape.ActualWidth - 120));
            Canvas.SetTop(rect, _rand.Next(10, (int)myCanvasShape.ActualHeight - 60));
            myCanvasShape.Children.Add(rect);
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

        private void OnFun3DClicked(object sender, RoutedEventArgs e)
        {
            var fun = new Fun3D { Owner = this };

            fun.Show();
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
