using System;
using System.Collections.Generic;
using System.Drawing;

namespace ComputerGadget.View
{
    class DotView : IDataViwer
    {
        private int dotSize = 1;
        private float padding = 4;
        private Font messageFont;

        public DotView(float fontSize)
        {
            messageFont = new Font(SystemFonts.DefaultFont.FontFamily, fontSize);
        }

        public void Draw(Graphics graphics, Rectangle clip, IReadOnlyList<double>[] data, string msg)
        {
            float width = (float)clip.Width / data.Length;

            int id = 0;
            foreach (var dat in data)
                DrawSingleData(graphics, clip, dat, width, id++);

            DrawSeprator(graphics, clip, width, data.Length);

            DrawMessage(graphics, msg, width);
        }

        private void DrawSingleData(Graphics graphics, Rectangle clip, IReadOnlyList<double> data, float width, int id)
        {
            for (int i = data.Count - 1; i >= 0; i--)
            {
                float x = (id + 1) * width - (data.Count - i) * dotSize;
                float h = (float)(1 - data[i]) * (clip.Height - padding) - dotSize / 2.0f + padding / 2.0f;
                if (x < id * width)
                    break;
                RectangleF dot = new RectangleF((float)Math.Round(x), (float)Math.Round(h), dotSize, dotSize);
                graphics.FillRectangle(Brushes.White, dot);
            }
        }

        private void DrawSeprator(Graphics graphics, Rectangle clip, float width, int count)
        {
            for (int i = 1; i < count; i++)
            {
                float x = width * i;
                PointF top = new PointF(x, padding / 2.0f);
                PointF bottom = new PointF(x, clip.Bottom - padding / 2.0f);
                graphics.DrawLine(Pens.White, top, bottom);
            }
        }

        private void DrawMessage(Graphics graphics, string msg, float width)
        {
            string[] msgs = msg.Split('|');
            for (int i = 0; i < msgs.Length; i++)
                graphics.DrawString(msgs[i], messageFont, Brushes.White, new PointF(width * i + 1, 1));
        }
    }
}
