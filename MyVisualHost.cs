using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WPFPlayground
{
    /// <summary>
    /// 
    /// </summary>
    public class MyVisualHostDynamic : FrameworkElement
    {
        // Create a collection of child visual objects.
        readonly VisualCollection _children;
        Random _rand = new Random();

        public MyVisualHostDynamic()
        {
            _children = new VisualCollection(this)
            {
            };

            // Add the event handler for MouseLeftButtonUp.
            MouseLeftButtonUp += MyVisualHost_MouseLeftButtonUp;
        }

        public void Update()
        {
            Canvas c = Parent as Canvas;
            Color clr = Color.FromRgb((byte)_rand.Next(0, 255), (byte)_rand.Next(0, 255), (byte)_rand.Next(0, 255));
            int x = _rand.Next(0, (int)c.ActualWidth);
            int y = _rand.Next(0, (int)c.ActualHeight);

            DrawingVisual drawingVisual = new DrawingVisual();

            // Retrieve the DrawingContext in order to create new drawing content.
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            drawingContext.DrawEllipse(new SolidColorBrush(clr), null, new Point(x, y), 3.0, 3.0);

            // Close the DrawingContext to persist changes to the DrawingVisual.
            drawingContext.Close();

            _children.Add(drawingVisual);
        }

        // Capture the mouse event and hit test the coordinate point value against
        // the child visual objects.
        private void MyVisualHost_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Retreive the coordinates of the mouse button event.
            Point pt = e.GetPosition((UIElement)sender);

            // Initiate the hit test by setting up a hit test result callback method.
            //VisualTreeHelper.HitTest(this, null, DoHitTest, new PointHitTestParameters(pt));
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

    /// <summary>
    /// 
    /// </summary>
    public class MyVisualHostStatic : FrameworkElement
    {
        // Create a collection of child visual objects.
        VisualCollection _children;
        double _width = 0;
        double _height = 0;

        public MyVisualHostStatic()
        {
            _children = new VisualCollection(this);

            // Add the event handlers.
            MouseLeftButtonUp += MyVisualHost_MouseLeftButtonUp;
            Loaded += MyVisualHostStatic_Loaded;
        }

        private void MyVisualHostStatic_Loaded(object sender, RoutedEventArgs e)
        {
            Canvas c = Parent as Canvas;
            _width = c.ActualWidth;
            _height = c.ActualHeight;

            _children.Add(CreateDrawingVisualRectangle());
            _children.Add(CreateDrawingVisualText());
            _children.Add(CreateDrawingVisualEllipses());
        }

        // Capture the mouse event and hit test the coordinate point value against
        // the child visual objects.
        private void MyVisualHost_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Retreive the coordinates of the mouse button event.
            Point pt = e.GetPosition((UIElement) sender);

            // Initiate the hit test by setting up a hit test result callback method.
            VisualTreeHelper.HitTest(this, null, DoHitTest, new PointHitTestParameters(pt));
        }

        // If a child visual object is hit, toggle its opacity to visually indicate a hit.
        public HitTestResultBehavior DoHitTest(HitTestResult result)
        {
            if (result.VisualHit.GetType() == typeof (DrawingVisual))
            {
                ((DrawingVisual)result.VisualHit).Opacity = ((DrawingVisual)result.VisualHit).Opacity == 1.0 ? 0.4 : 1.0;
            }

            // Stop the hit test enumeration of objects in the visual tree.
            return HitTestResultBehavior.Stop;
        }

        // Create a DrawingVisual that contains a rectangle.
        private DrawingVisual CreateDrawingVisualRectangle()
        {
            DrawingVisual drawingVisual = new DrawingVisual();

            // Retrieve the DrawingContext in order to create new drawing content.
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            // Create a rectangle and draw it in the DrawingContext.
            Rect rect = new Rect(new Point(10, 10), new Size(_width - 50, _height - 80));
            drawingContext.DrawRectangle(Brushes.LightBlue, null, rect);

            // Close the DrawingContext to persist changes to the DrawingVisual.
            drawingContext.Close();

            return drawingVisual;
        }

        // Create a DrawingVisual that contains text.
        private DrawingVisual CreateDrawingVisualText()
        {
            // Create an instance of a DrawingVisual.
            DrawingVisual drawingVisual = new DrawingVisual();

            // Retrieve the DrawingContext from the DrawingVisual.
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            // Draw a formatted text string into the DrawingContext.
            drawingContext.DrawText(
                new FormattedText("Click Me!", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, 
                    new Typeface("Verdana"), 36, Brushes.Black, 1.0),
                new Point(10, 10));

            // Close the DrawingContext to persist changes to the DrawingVisual.
            drawingContext.Close();

            return drawingVisual;
        }

        // Create a DrawingVisual that contains an ellipse.
        private DrawingVisual CreateDrawingVisualEllipses()
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            drawingContext.DrawEllipse(Brushes.Maroon, null, new Point(_width - 40, _height / 2), 20, 20);

            // Close the DrawingContext to persist changes to the DrawingVisual.
            drawingContext.Close();

            return drawingVisual;
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