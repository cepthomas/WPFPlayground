using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;


namespace WPFPlayground
{
    public class Utils
    {
        public static string GetSourcePath([CallerFilePath] string path = "")
        {
            return System.IO.Path.GetDirectoryName(path)!;
        }

        /// Both the visual tree and logical tree are synchronized with the current set of application elements,
        /// reflecting any addition, deletion, or modification of elements. However, the trees present different
        /// views of the application. Unlike the visual tree, the logical tree does not expand a control's
        /// ContentPresenter element. This means there is not a direct one-to-one correspondence between a
        /// logical tree and a visual tree for the same set of objects. In fact, invoking the LogicalTreeHelper
        /// object's GetChildren method and the VisualTreeHelper object's GetChild method using the same element
        /// as a parameter yields differing results.
        static public void RetrieveDrawing(Visual v)
        {
            DrawingGroup drawingGroup = VisualTreeHelper.GetDrawing(v);
            EnumDrawingGroup(drawingGroup);
        }

        /// Enumerate the drawings in the DrawingGroup.
        static public void EnumDrawingGroup(DrawingGroup drawingGroup)
        {
            DrawingCollection dc = drawingGroup.Children;

            // Enumerate the drawings in the DrawingCollection.
            foreach (Drawing drawing in dc)
            {
                // If the drawing is a DrawingGroup, call the function recursively.
                if (drawing is DrawingGroup group)
                {
                    EnumDrawingGroup(group);
                }
                else if (drawing is GeometryDrawing)
                {
                    // Perform action based on drawing type.  
                }
                else if (drawing is ImageDrawing)
                {
                    // Perform action based on drawing type.
                }
                else if (drawing is GlyphRunDrawing)
                {
                    // Perform action based on drawing type.
                }
                else if (drawing is VideoDrawing)
                {
                    // Perform action based on drawing type.
                }
            }
        }

        /// Enumerate all the descendants of the visual object.
        static public void EnumVisual(Visual myVisual)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(myVisual); i++)
            {
                // Retrieve child visual at specified index value.
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(myVisual, i);

                // Do processing of the child visual object.

                // Enumerate children of the child visual object.
                EnumVisual(childVisual);
            }
        }

    }
}
