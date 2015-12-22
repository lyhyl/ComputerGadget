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
            PointF pd = new PointF(x, h);
            PointF pp = new PointF(x, clip.Bottom - Padding - 1);
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
            pen = new Pen(Theme.ForegroundColor, DotSize);
        }
    }
}
