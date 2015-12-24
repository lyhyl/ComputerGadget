using ComputerGadget.Counter;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ComputerGadget.View
{
    public abstract class SimpleView : IDataViwer, IDisposable
    {
        private bool disposed = false;
        private Font messageFont;

        protected int DotSize { private set; get; } = 1;
        protected float Padding { private set; get; } = 1;

        private Theme theme = Theme.DarkTheme;
        private Brush bgbrush;
        public Theme Theme
        {
            set
            {
                theme = value;
                if (theme == null)
                    throw new ArgumentNullException(nameof(Theme));
                OnThemeChanged();
            }
            get { return theme; }
        }

        public SimpleView(float fontSize)
        {
            messageFont = new Font(SystemFonts.DefaultFont.FontFamily, fontSize);
            bgbrush = new SolidBrush(Theme.BackgroundColor);
        }

        public void Draw(Graphics graphics, Rectangle clip, IPerformanceDataCollector counter)
        {
            int sampleSize = (int)Math.Ceiling((double)clip.Width / DotSize);
            IReadOnlyList<double>[] data = counter.UpdateAndGetData(sampleSize);

            graphics.FillRectangle(bgbrush, clip);

            if (data.Length > 0)
            {
                float width = (float)clip.Width / data.Length;
                DrawData(graphics, clip, data, width);
                DrawSeprator(graphics, clip, width, data.Length);
                DrawMessage(graphics, counter.BriefMessage, width);
            }
            else
                DrawMessage(graphics, counter.BriefMessage, clip.Width);
        }

        protected abstract void DrawSingleData(Graphics graphics, Rectangle clip, float x, float h);

        protected virtual void OnThemeChanged() => UpdatePenAndBrush();

        private void UpdatePenAndBrush()
        {
            bgbrush?.Dispose();
            bgbrush = new SolidBrush(Theme.BackgroundColor);
        }

        private void DrawData(Graphics graphics, Rectangle clip, IReadOnlyList<double>[] data, float width)
        {
            for (int i = 0; i < data.Length; i++)
            {
                var dat = data[i];
                for (int j = data[i].Count - 1; j >= 0; j--)
                {
                    float x = (i + 1) * width - (dat.Count - j - 0.5f) * DotSize;
                    float h = (float)(1 - dat[j]) * (clip.Height - Padding * 2 - 1) + Padding;
                    if (x < i * width)
                        break;
                    DrawSingleData(graphics, clip, x, h);
                }
            }
        }

        private void DrawSeprator(Graphics graphics, Rectangle clip, float width, int count)
        {
            using (Pen fpen = new Pen(Theme.ForegroundColor), bpen = new Pen(Theme.BackgroundColor))
            {
                for (int i = 1; i < count; i++)
                {
                    float x = (float)Math.Round(width * i);
                    PointF top = new PointF(x, Padding);
                    PointF bottom = new PointF(x, clip.Bottom - Padding - 1);
                    graphics.DrawLine(fpen, top, bottom);
                    PointF topl = new PointF(x - 1, 0);
                    PointF bottoml = new PointF(x - 1, clip.Bottom - 1);
                    graphics.DrawLine(bpen, topl, bottoml);
                    PointF topr = new PointF(x + 1, 0);
                    PointF bottomr = new PointF(x + 1, clip.Bottom - 1);
                    graphics.DrawLine(bpen, topr, bottomr);
                }
                PointF t0 = new PointF(0, 0);
                PointF b0 = new PointF(0, clip.Bottom - 1);
                graphics.DrawLine(bpen, t0, b0);
                PointF t1 = new PointF(clip.Right - 1, 0);
                PointF b1 = new PointF(clip.Right - 1, clip.Bottom - 1);
                graphics.DrawLine(bpen, t1, b1);
            }
        }

        private void DrawMessage(Graphics graphics, string msg, float width)
        {
            string[] msgs = msg.Split('|');
            using (Brush brush = new SolidBrush(Theme.ForegroundColor))
                for (int i = 0; i < msgs.Length; i++)
                    graphics.DrawString(msgs[i], messageFont, brush, new PointF(width * i + 1, 1));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                messageFont.Dispose();
                bgbrush?.Dispose();
            }
            
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
