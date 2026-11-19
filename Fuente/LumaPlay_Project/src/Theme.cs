using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LumaPlay
{
    internal static class Theme
    {
        public static Color Background = Color.FromArgb(13, 15, 27);
        public static Color Sidebar = Color.FromArgb(10, 12, 22);
        public static Color Card = Color.FromArgb(23, 27, 44);
        public static Color CardHover = Color.FromArgb(35, 40, 62);
        public static Color Active = Color.FromArgb(46, 40, 78);
        public static Color Border = Color.FromArgb(49, 54, 77);
        public static Color Text = Color.FromArgb(241, 243, 255);
        public static Color Muted = Color.FromArgb(157, 166, 193);
        public static Color Input = Color.FromArgb(17, 21, 36);
        public static Color Accent = Color.FromArgb(145, 113, 255);
        public static Color AccentHover = Color.FromArgb(165, 139, 255);

        public static void Apply(string name)
        {
            if (name == "Cian")
            {
                Accent = Color.FromArgb(61, 191, 220);
                AccentHover = Color.FromArgb(91, 211, 235);
            }
            else if (name == "Verde")
            {
                Accent = Color.FromArgb(83, 204, 153);
                AccentHover = Color.FromArgb(113, 222, 175);
            }
            else
            {
                Accent = Color.FromArgb(145, 113, 255);
                AccentHover = Color.FromArgb(165, 139, 255);
            }
        }
    }

    internal sealed class GradientPanel : Panel
    {
        public Color StartColor { get; set; }
        public Color EndColor { get; set; }

        public GradientPanel()
        {
            StartColor = Color.FromArgb(51, 42, 105);
            EndColor = Color.FromArgb(22, 70, 100);
            DoubleBuffered = true;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (ClientRectangle.Width < 1 || ClientRectangle.Height < 1) return;
            using (LinearGradientBrush brush = new LinearGradientBrush(ClientRectangle, StartColor, EndColor, 25F))
                e.Graphics.FillRectangle(brush, ClientRectangle);
            using (Pen line = new Pen(Color.FromArgb(24, 255, 255, 255), 1F))
            {
                for (int i = 0; i < 7; i++)
                {
                    int y = ClientSize.Height - 18 - i * 18;
                    e.Graphics.DrawLine(line, ClientSize.Width - 190 + i * 20, y, ClientSize.Width + 30, y - 80);
                }
            }
        }
    }
}
