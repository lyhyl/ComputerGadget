﻿using System;
using System.Drawing;

namespace ComputerGadget.View
{
    class DotView : SimpleView
    {
        public DotView(float fontSize) : base(fontSize)
        {
        }

        protected override void DrawSingleData(Graphics graphics, Rectangle clip, float x, float h)
        {
            float l = (float)Math.Round(x - DotSize / 2.0f);
            float t = (float)Math.Round(h - DotSize / 2.0f);
            RectangleF dot = new RectangleF(l, t, DotSize, DotSize);
            graphics.FillRectangle(Brushes.White, dot);
        }
    }
}
