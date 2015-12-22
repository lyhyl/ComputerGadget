using System.Drawing;

namespace ComputerGadget.View
{
    public class Theme
    {
        public Color ForegroundColor { set; get; }
        public Color BackgroundColor { set; get; }
        public Color WarningColor { set; get; }

        public static Theme DarkTheme { get; } = new Theme(Color.White, Color.Black, Color.DarkRed);
        public static Theme LightTheme { get; } = new Theme(Color.Black, Color.White, Color.DarkRed);

        public Theme(Color fore, Color back, Color warn)
        {
            ForegroundColor = fore;
            BackgroundColor = back;
            WarningColor = warn;
        }
    }
}
