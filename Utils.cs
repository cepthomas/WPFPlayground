using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;


namespace Ephemera.WPFPlayground
{
    public class Utils
    {
        public static string GetSourcePath([CallerFilePath] string path = "")
        {
            return System.IO.Path.GetDirectoryName(path)!;
        }
    }
}
