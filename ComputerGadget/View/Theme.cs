using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ComputerGadget.View
{
    public class Theme : IDisposable
    {
        private bool disposed = false;

        public Color ForegroundColor { set; get; }
        public Color BackgroundColor { set; get; }
        public Color WarningColor { set; get; }
        public Bitmap BackgroundImage { protected set; get; }

        public static Theme Dark { get; } = new Theme(Color.White, Color.Black);
        public static Theme Light { get; } = new Theme(Color.Black, Color.White);
        public static Theme Doraemon { get; } = new Theme(Color.White, Color.Red, "Doraemon.jpg");

        public Theme(Color fore, Color back) : this(fore, back, Color.Red, string.Empty)
        {
        }

        public Theme(Color fore, Color back, Color warn, string bgimage)
        {
            ForegroundColor = fore;
            BackgroundColor = back;
            WarningColor = warn;
            ReadInImage(bgimage);
        }

        public Theme(Color fore, Color warn, string bgimage)
        {
            ForegroundColor = fore;
            WarningColor = warn;
            ReadInImage(bgimage);
            BackgroundColor = GetBackgroundImageAvgColor();
        }

        private Color GetBackgroundImageAvgColor()
        {
            if (BackgroundImage == null)
                return Color.Black;
            Color[] colors = GetRawColorData();
            int[] rStatistic, gStatistic, bStatistic;
            GenerateStatistics(colors, out rStatistic, out gStatistic, out bStatistic);
            int MaxDiff = 8;
            double rtc = GetComponentTheme(rStatistic, MaxDiff);
            double gtc = GetComponentTheme(gStatistic, MaxDiff);
            double btc = GetComponentTheme(bStatistic, MaxDiff);
            return GetAvgColorByFilter(colors, MaxDiff, rtc, gtc, btc);
        }

        private Color[] GetRawColorData()
        {
            Color[] colors = new Color[BackgroundImage.Width * BackgroundImage.Height];
            int idx = 0;
            for (int x = 0; x < BackgroundImage.Width; x++)
                for (int y = 0; y < BackgroundImage.Height; y++)
                    colors[idx++] = BackgroundImage.GetPixel(x, y);
            return colors;
        }

        private void GenerateStatistics(Color[] colors, out int[] rstat, out int[] gstat, out int[] bstat)
        {
            const int Size = 256;
            rstat = new int[Size];
            for (int i = 0; i < colors.Length; i++)
                rstat[colors[i].R]++;
            gstat = new int[Size];
            for (int i = 0; i < colors.Length; i++)
                gstat[colors[i].G]++;
            bstat = new int[Size];
            for (int i = 0; i < colors.Length; i++)
                bstat[colors[i].B]++;
        }

        private double GetComponentTheme(int[] data, int maxDiff)
        {
            int curCount = 0;
            for (int i = 0; i < maxDiff; i++)
                curCount += data[i];
            int maxCount = curCount, maxIdx = 0;
            for (int i = maxDiff; i < data.Length; i++)
            {
                curCount += data[i];
                curCount -= data[i - maxDiff];
                if (maxCount < curCount)
                {
                    maxCount = curCount;
                    maxIdx = i + 1 - maxDiff;
                }
            }
            double u = 0;
            for (int i = 0; i < maxDiff; i++)
                u += data[maxIdx + i] * (maxIdx + i);
            return u / maxCount;
        }

        private Color GetAvgColorByFilter(Color[] colors, int MaxDiff, double rtc, double gtc, double btc)
        {
            double r = 0, g = 0, b = 0;
            int cnt = 0;
            foreach (var color in colors)
                if (Math.Abs(rtc - color.R) < MaxDiff / 2.0 &&
                    Math.Abs(gtc - color.G) < MaxDiff / 2.0 &&
                    Math.Abs(btc - color.B) < MaxDiff / 2.0)
                {
                    r += color.R;
                    g += color.G;
                    b += color.B;
                    cnt++;
                }
            if (cnt > 0)
            {
                r = Math.Round(r / cnt);
                g = Math.Round(g / cnt);
                b = Math.Round(b / cnt);
            }
            return Color.FromArgb((int)r, (int)g, (int)b);
        }

        private void ReadInImage(string bgimage)
        {
            if (!string.IsNullOrEmpty(bgimage))
                try
                {
                    BackgroundImage = Image.FromFile(".\\Res\\" + bgimage) as Bitmap;
                }
                catch (Exception)
                {

                }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
                BackgroundImage?.Dispose();
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
