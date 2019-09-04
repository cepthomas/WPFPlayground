using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using Newtonsoft.Json;
using System.Windows.Media;
using System.Windows;


namespace WPFPlayground
{
    [Serializable]
    public class UserSettings
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

        #region Persisted non-editable properties
        [Browsable(false)]
        public WindowInfo MainWindowInfo { get; set; } = new WindowInfo();

        [Browsable(false)]
        public bool SomeFlag { get; set; } = false;
        #endregion

        #region Classes
        /// <summary>
        /// General purpose container for persistence.
        /// </summary>
        [Serializable]
        public class WindowInfo
        {
            public double X { get; set; } = 50;
            public double Y { get; set; } = 50;
            public double Width { get; set; } = 1000;
            public double Height { get; set; } = 700;

            public void FromWindow(Window f)
            {
                X = f.Left;
                Y = f.Top;
                Width = f.Width;
                Height = f.Height;
            }
        }
        #endregion

        #region Fields
        /// <summary>The file name.</summary>
        string _fn = "???";
        #endregion

        #region Persistence
        /// <summary>Save object to file.</summary>
        public void Save()
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(_fn, json);
        }

        /// <summary>Create object from file.</summary>
        public static UserSettings Load(string fn)
        {
            UserSettings settings = null;

            if(File.Exists(fn))
            {
                string json = File.ReadAllText(fn);
                settings = JsonConvert.DeserializeObject<UserSettings>(json);
                settings._fn = fn;
            }
            else
            {
                // Doesn't exist, create a new one.
                settings = new UserSettings
                {
                    _fn = fn
                };
            }

            return settings;
        }

        /// <summary>Copy values for UI editing.</summary>
        public void CopyTo(UserSettings to)
        {
            to.EditorFont = EditorFont;
            to.EditorFontSize = EditorFontSize;
            to.SelectedColor = SelectedColor;
            to.BackColor = BackColor;
        }

        /// <summary>Copy values for UI editing.</summary>
        public void CopyFrom(UserSettings from)
        {
            EditorFont = from.EditorFont;
            EditorFontSize = from.EditorFontSize;
            SelectedColor = from.SelectedColor;
            BackColor = from.BackColor;
        }
        #endregion
    }
}
