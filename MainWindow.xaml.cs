using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    ///   Window_Loaded (Occurs when the element is laid out, rendered, and ready for interaction.)
    ///   Window_ContentRendered (Occurs after a window's content has been rendered.)
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            AddInfoLine($"MainWindow constructor");

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            dispatcherTimer.Start();
        }


        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            myCanvas.Children.Clear();
            DrawStuff();
        }

        int _loc = 10;
        private void DrawStuff()
        {
            Rectangle aRectangle = new Rectangle();
            aRectangle.Width = 100;
            aRectangle.Height = 50;
            aRectangle.Stroke = Brushes.Red;
            Canvas.SetLeft(aRectangle, _loc);
            Canvas.SetTop(aRectangle, _loc);
            myCanvas.Children.Add(aRectangle);
            _loc++;


            //Line line = new Line();
            //Thickness thickness = new Thickness(101, -11, 362, 250);
            //line.Margin = thickness;
            //line.Visibility = System.Windows.Visibility.Visible;
            //line.StrokeThickness = 4;
            //line.Stroke = System.Windows.Media.Brushes.Black;
            //line.X1 = 10;
            //line.X2 = 40;
            //line.Y1 = 70;
            //line.Y2 = 70;
            //myCanvas.Children.Add(line);


            //QuadraticBezierSegment qbs = new QuadraticBezierSegment(new Point(X2, Y1), new Point(X2, Y2), true);
            //PathSegmentCollection pscollection = new PathSegmentCollection();
            //pscollection.Add(qbs);
            //PathFigure pf = new PathFigure();
            //pf.Segments = pscollection;
            //pf.StartPoint = new Point(X1, Y1);
            //PathFigureCollection pfcollection = new PathFigureCollection();
            //pfcollection.Add(pf);
            //PathGeometry pathGeometry = new PathGeometry();
            //pathGeometry.Figures = pfcollection;
            //Path path = new Path();
            //path.Data = pathGeometry;
            //path.Stroke = new SolidColorBrush(color);
            //path.StrokeThickness = 2;
            //Canvas.SetZIndex(path, (int)Layer.Line);
            //canvas.Children.Add(path);
            //return pathGeometry;
        }


        private void OnUiReady(object sender, EventArgs e)
        {
            AddInfoLine($"N/A? OnUiReady");
            //LinePane.Width = ((StackPanel)LinePane.Parent).ActualWidth;
            //LinePane.Height = ((StackPanel)LinePane.Parent).ActualHeight;
            //DesignerPane.MouseLeave += DesignerPane_MouseLeave;
            //SizeChanged += Window1_SizeChanged;
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            AddInfoLine($"OnRender");
            base.OnRender(drawingContext);
            //Rect rect = new Rect(new System.Windows.Point(50, 50), new System.Windows.Size(200, 100));
            //drawingContext.DrawRectangle(System.Windows.Media.Brushes.Aqua, (System.Windows.Media.Pen)null, rect);
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            AddInfoLine($"Window_Initialized");

        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AddInfoLine($"Window_SizeChanged");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AddInfoLine($"Window_Loaded");
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            AddInfoLine($"Window_ContentRendered");
        }

        private void ellipse1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MyViewModel vm = DataContext as MyViewModel;

            AddInfoLine($"ellipse1_MouseDown:{vm.MyVal}");

            if (vm.MyVal > 1)
            {
                vm.MyVal--;
            }
        }

        private void ShowEditor()
        {
            var editor = new Editor { Owner = this };

            editor.SetPropertiesFromObject(textBox);
            editor.PreviewSampleText = textBox.SelectedText;

            var dlg = editor.ShowDialog();
            if (dlg != null && dlg.Value)
            {
//                editor.ApplyPropertiesToObject(textBox);
            }
        }

        void AddInfoLine(string s)
        {
            infobox.AppendText($"{s}{Environment.NewLine}");
            infobox.ScrollToEnd();
        }
    }
}
