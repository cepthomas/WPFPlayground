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
        readonly Random _rand = new Random();
        int _lastTick = Environment.TickCount;

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
                // Wait to start.
                var delay = 16.0 * _rand.NextDouble();

                //////////////////////
                var offsetXAnimation = new DoubleAnimation(0.0, -canvas.ActualWidth / 2, new Duration(TimeSpan.FromSeconds(duration)))
                {
                    RepeatBehavior = RepeatBehavior.Forever,
                    BeginTime = TimeSpan.FromSeconds(delay)
                };
                var offsetTransform = new TranslateTransform();
                offsetTransform.BeginAnimation(TranslateTransform.XProperty, offsetXAnimation);
                offsetTransform.BeginAnimation(TranslateTransform.YProperty, offsetXAnimation);
                circle.RenderTransform = offsetTransform;

                //////////////////////
                var sizeAnimation = new DoubleAnimation(0.0, canvas.ActualHeight, new Duration(TimeSpan.FromSeconds(duration)))
                {
                    RepeatBehavior = RepeatBehavior.Forever,
                    BeginTime = TimeSpan.FromSeconds(delay)
                };
                circle.BeginAnimation(WidthProperty, sizeAnimation);
                circle.BeginAnimation(HeightProperty, sizeAnimation);

                //////////////////////
                var opacityAnimation = new DoubleAnimation(duration - 1.0, 0.0, new Duration(TimeSpan.FromSeconds(duration)))
                {
                    BeginTime = TimeSpan.FromSeconds(delay),
                    RepeatBehavior = RepeatBehavior.Forever
                };
                circle.BeginAnimation(OpacityProperty, opacityAnimation);
            }
        }
    }
}
