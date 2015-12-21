using ComputerGadget.Counter;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ComputerGadget.View
{
    abstract class SimpleView : IDataViwer
    {
        protected int DotSize { private set; get; } = 1;
        protected float Padding { private set; get; } = 4;
        private Font messageFont;

        public SimpleView(float fontSize)
        {
            messageFont = new Font(SystemFonts.DefaultFont.FontFamily, fontSize);
        }

        public void Draw(Graphics graphics, Rectangle clip, ICounter counter)
        {
            int sampleSize = (int)Math.Ceiling((double)clip.Width / DotSize);
            IReadOnlyList<double>[] data = counter.UpdateAndGetData(sampleSize);
            float width = (float)clip.Width / data.Length;

            DrawData(graphics, clip, data, width);
            DrawSeprator(graphics, clip, width, data.Length);
            DrawMessage(graphics, counter.Message, width);
        }

        protected abstract void DrawSingleData(Graphics graphics, Rectangle clip, float x, float h);

        private void DrawData(Graphics graphics, Rectangle clip, IReadOnlyList<double>[] data, float width)
        {
            int id = 0;
            foreach (var dat in data)
                DrawSingleSet(graphics, clip, dat, width, id++);
        }

        private void DrawSingleSet(Graphics graphics, Rectangle clip, IReadOnlyList<double> data, float width, int id)
        {
            for (int i = data.Count - 1; i >= 0; i--)
            {
                float x = (id + 1) * width - (data.Count - i - 0.5f) * DotSize;
                float h = (float)(1 - data[i]) * (clip.Height - Padding) + Padding / 2.0f;
                if (x < id * width)
                    break;
                DrawSingleData(graphics, clip, x, h);
            }
        }

        private void DrawSeprator(Graphics graphics, Rectangle clip, float width, int count)
        {
            for (int i = 1; i < count; i++)
            {
                float x = width * i;
                PointF top = new PointF(x, Padding / 2.0f);
                PointF bottom = new PointF(x, clip.Bottom - Padding / 2.0f);
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
