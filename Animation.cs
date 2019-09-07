using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WPFPlayground
{
    class Animation : Window
    {
        // A Canvas contains a collection of UIElement objects, which are in the Children property.
        Canvas _canvas = new Canvas();
        private readonly Random _rand;
        private int _lastTick;

        public Animation() // MainWindow() from concentric circles sample.
        {
            //InitializeComponent();

            // WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;

            // Not used.
            //var frameTimer = new DispatcherTimer();
            //frameTimer.Tick += OnFrame;
            //frameTimer.Interval = TimeSpan.FromSeconds(1.0 / 60.0);
            //frameTimer.Start();

            _lastTick = Environment.TickCount;

            _rand = new Random(GetHashCode());

            Show();

           // KeyDown += Window1_KeyDown;

            CreateCircles();
        }

        //private void Window1_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Escape)
        //    {
        //        Close();
        //    }
        //}

        private void OnFrame(object sender, EventArgs e)
        {
        }

        private void CreateCircles()
        {
            var centerX = _canvas.ActualWidth / 2.0;
            var centerY = _canvas.ActualHeight / 2.0;

            Color[] colors = { Colors.White, Colors.Green, Colors.Green, Colors.Lime };

            for (var i = 0; i < 24; ++i)
            {
                var circle = new Ellipse();
                var alpha = (byte)_rand.Next(96, 192);
                var colorIndex = _rand.Next(4);
                circle.Stroke = new SolidColorBrush(Color.FromArgb(alpha, colors[colorIndex].R, colors[colorIndex].G, colors[colorIndex].B));
                circle.StrokeThickness = _rand.Next(1, 4);
                circle.Width = 0.0;
                circle.Height = 0.0;
                double offsetX = 16 - _rand.Next(32);
                double offsetY = 16 - _rand.Next(32);

                _canvas.Children.Add(circle);

                circle.SetValue(Canvas.LeftProperty, centerX + offsetX);
                circle.SetValue(Canvas.TopProperty, centerY + offsetY);

                var duration = 6.0 + 10.0 * _rand.NextDouble();
                var delay = 16.0 * _rand.NextDouble();

                //////////////////////
                var offsetXAnimation = new DoubleAnimation(0.0, -256.0, new Duration(TimeSpan.FromSeconds(duration)))
                {
                    RepeatBehavior = RepeatBehavior.Forever,
                    BeginTime = TimeSpan.FromSeconds(delay)
                };
                var offsetTransform = new TranslateTransform();
                offsetTransform.BeginAnimation(TranslateTransform.XProperty, offsetXAnimation);
                offsetTransform.BeginAnimation(TranslateTransform.YProperty, offsetXAnimation);
                circle.RenderTransform = offsetTransform;

                //////////////////////
                var sizeAnimation = new DoubleAnimation(0.0, 512.0, new Duration(TimeSpan.FromSeconds(duration)))
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
