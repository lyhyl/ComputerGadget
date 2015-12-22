using System;
using System.Drawing;

namespace ComputerGadget.View
{
    public sealed class StripView : SimpleView
    {
        private Pen pen;

        public StripView(float fontSize) : base(fontSize)
        {
            UpdatePens();
        }

        protected override void DrawSingleData(Graphics graphics, Rectangle clip, float x, float h)
        {
            float l = (float)Math.Round(x - DotSize / 2.0f);
            float t = (float)Math.Round(h - DotSize / 2.0f);
            PointF pd = new PointF(x, h);
            PointF pp = new PointF(x, clip.Bottom - Padding);
            graphics.DrawLine(pen, pd, pp);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
                pen?.Dispose();
        }

        protected override void OnThemeChanged()
        {
            base.OnThemeChanged();
            UpdatePens();
        }

        private void UpdatePens()
        {
            pen = new Pen(Theme.ForegroundColor);
        }
    }
}
