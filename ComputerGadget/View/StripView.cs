using System;
using System.Drawing;

namespace ComputerGadget.View
{
    class StripView : SimpleView
    {
        public StripView(float fontSize):base(fontSize)
        {
        }

        protected override void DrawSingleData(Graphics graphics, Rectangle clip, float x, float h)
        {
            float l = (float)Math.Round(x - DotSize / 2.0f);
            float t = (float)Math.Round(h - DotSize / 2.0f);
            RectangleF dot = new RectangleF(l, t, DotSize, clip.Bottom - Padding - h);
            dot.Height = (float)Math.Ceiling(dot.Bottom) - h;
            graphics.FillRectangle(Brushes.White, dot);
        }
    }
}
