using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFPlayground
{
    /// <summary>
    /// Interaction logic for MyAnimation.xaml.
    /// From concentric circles sample.
    /// </summary>
    public partial class MyAnimation : Window
    {
        readonly Random _rand = new();
        //int _lastTick = Environment.TickCount;

        public MyAnimation()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CreateCircles();
        }

        void CreateCircles()
        {
            var centerX = canvas.ActualWidth / 2.0;
            var centerY = canvas.ActualHeight / 2.0;

            for (var i = 0; i < 24; ++i)
            {
                var clr = Color.FromRgb((byte)_rand.Next(0, 255), (byte)_rand.Next(0, 255), (byte)_rand.Next(0, 255));
                var circle = new Ellipse
                {
                    Stroke = new SolidColorBrush(clr),
                    StrokeThickness = _rand.Next(1, 4),
                    Width = 0.0,
                    Height = 0.0
                };

                double offsetX = 16 - _rand.Next(32);
                double offsetY = 16 - _rand.Next(32);
                circle.SetValue(Canvas.LeftProperty, centerX + offsetX);
                circle.SetValue(Canvas.TopProperty, centerY + offsetY);
                canvas.Children.Add(circle);

                // How long from min to max.
                var duration = 6.0 + 10.0 * _rand.NextDouble();
                var andur = new Duration(TimeSpan.FromSeconds(duration));
                // Wait to start.
                var delay = 16.0 * _rand.NextDouble();

                ////////// X position ////////////
                var anX = new DoubleAnimation(0.0, -canvas.ActualWidth / 2, andur)
                {
                    BeginTime = TimeSpan.FromSeconds(delay),
                    RepeatBehavior = RepeatBehavior.Forever
                };
                var offsetTransform = new TranslateTransform();
                offsetTransform.BeginAnimation(TranslateTransform.XProperty, anX);
                offsetTransform.BeginAnimation(TranslateTransform.YProperty, anX);
                circle.RenderTransform = offsetTransform;

                ////////// Y position ////////////
                var anSize = new DoubleAnimation(0.0, canvas.ActualHeight, andur)
                {
                    BeginTime = TimeSpan.FromSeconds(delay),
                    RepeatBehavior = RepeatBehavior.Forever
                };
                circle.BeginAnimation(WidthProperty, anSize);
                circle.BeginAnimation(HeightProperty, anSize);

                ////////// fading ////////////
                var anOpac = new DoubleAnimation(duration - 1.0, 0.0, andur)
                {
                    BeginTime = TimeSpan.FromSeconds(delay),
                    RepeatBehavior = RepeatBehavior.Forever
                };
                circle.BeginAnimation(OpacityProperty, anOpac);
            }
        }
    }
}
