using System;
using System.Drawing;

namespace ComputerGadget.View
{
    public sealed class DotView : SimpleView
    {
        private Brush brush;

        public DotView(float fontSize) : base(fontSize)
        {
            UpdateBrushes();
        }

        protected override void DrawSingleData(Graphics graphics, Rectangle clip, float x, float h)
        {
            float l = (float)Math.Round(x - DotSize / 2.0f);
            float t = (float)Math.Round(h - DotSize / 2.0f);
            RectangleF dot = new RectangleF(l, t, DotSize, DotSize);
            graphics.FillRectangle(brush, dot);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
                brush?.Dispose();
        }

        protected override void OnThemeChanged()
        {
            base.OnThemeChanged();
            UpdateBrushes();
        }

        private void UpdateBrushes()
        {
            brush = new SolidBrush(Theme.ForegroundColor);
        }
    }
}
