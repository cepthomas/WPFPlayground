using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WPFPlayground
{
    /// <summary>
    /// A dynamic drawing space.
    /// </summary>
    public class MyVisualHost : FrameworkElement
    {
        // Create a collection of child visual objects.
        readonly VisualCollection _children;
        Random _rand = new Random();

        public MyVisualHost()
        {
            _children = new VisualCollection(this);

            // Add the event handler for MouseLeftButtonUp.
            MouseLeftButtonUp += MyVisualHost_MouseLeftButtonUp;
        }

        public void Update()
        {
            const int NUM_DOTS = 100;

            // Add a dot in a random color/location.
            Canvas c = Parent as Canvas;
            Color clr = Color.FromRgb((byte)_rand.Next(0, 255), (byte)_rand.Next(0, 255), (byte)_rand.Next(0, 255));
            int x = _rand.Next(0, (int)c.ActualWidth);
            int y = _rand.Next(0, (int)c.ActualHeight);

            // Draw it.
            DrawingVisual vis = new DrawingVisual();
            DrawingContext context = vis.RenderOpen();
            context.DrawEllipse(new SolidColorBrush(clr), null, new Point(x, y), 5.0, 5.0);
            // Close the DrawingContext to persist changes to the DrawingVisual.
            context.Close();

            _children.Add(vis);
            if(_children.Count > NUM_DOTS)
            {
                _children.RemoveRange(0, NUM_DOTS / 20);
            }
        }

        // Capture the mouse event and hit test the coordinate point value against
        // the child visual objects.
        void MyVisualHost_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Retreive the coordinates of the mouse button event.
            Point pt = e.GetPosition((UIElement)sender);
        }

        // Provide a required override for the VisualChildrenCount property.
        protected override int VisualChildrenCount => _children.Count;

        // Provide a required override for the GetVisualChild method.
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _children[index];
        }
    }
}