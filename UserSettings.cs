using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Media;
using System.Windows;


namespace Ephemera.WPFPlayground
{
    [Serializable]
    public sealed class UserSettings
    {
        #region Persisted editable properties
        [DisplayName("Editor Font"), Description("The font to use for editors etc."), Browsable(true)]
        public FontFamily EditorFont { get; set; } = new FontFamily("Consolas");

        [DisplayName("Editor Font Size"), Description("The font to use for editors etc."), Browsable(true)]
        public double EditorFontSize { get; set; } = 10;

        [DisplayName("Selected Color"), Description("The color used for selections."), Browsable(true)]
        public Color SelectedColor { get; set; } = Colors.Violet;

        [DisplayName("Background Color"), Description("The color used for overall background."), Browsable(true)]
        public Color BackColor { get; set; } = Colors.AliceBlue;
        #endregion
    }
}
