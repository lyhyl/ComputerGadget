using ComputerGadget.Counter;
using ComputerGadget.View;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Windows.Forms;

namespace ComputerGadget
{
    public partial class ComputerGadget : Form
    {
        private const int itemWidth = 200;
        private const int itemHeight = 50;
        private const int padding = 5;
        private readonly Color transparencyKey = Color.FromArgb(0, 255, 0);

        private Config config = new Config();
        private Timer updateTimer = new Timer();

        private List<IPerformanceDataCollector> counters = null;

        private Theme theme = Theme.Light;
        private IDataViwer viewer = null;

        public ComputerGadget()
        {
            InitializeComponent();

            CreateTimer();
            CreateCounterView();
            SetVisualStyle();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.Graphics.PageUnit = GraphicsUnit.Pixel;

            e.Graphics.FillRectangle(new SolidBrush(transparencyKey), e.ClipRectangle);
            Rectangle clip = new Rectangle(0, 0, itemWidth, itemHeight);

            foreach (var counter in counters)
            {
                viewer.Draw(e.Graphics, clip, counter);
                e.Graphics.TranslateTransform(0, itemHeight + padding);
            }

            if (theme.BackgroundImage != null)
                DrawImage(e, clip, .25f);
        }

        private void DrawImage(PaintEventArgs e, Rectangle clip, float alpha)
        {
            e.Graphics.Transform = new Matrix();
            float[][] rawMat = {
                new float[] {1, 0, 0, 0, 0},
                new float[] {0, 1, 0, 0, 0},
                new float[] {0, 0, 1, 0, 0},
                new float[] {0, 0, 0, alpha, 0},
                new float[] {0, 0, 0, 0, 1}
            };
            ColorMatrix alphaMat = new ColorMatrix(rawMat);
            ImageAttributes imgAttributes = new ImageAttributes();
            imgAttributes.SetColorMatrix(alphaMat, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            int x = (Width - theme.BackgroundImage.Width) / 2;
            int y = (Height - theme.BackgroundImage.Height) / 2;
            Point[] coners = new Point[] {
                new Point(x, y),
                new Point(x + theme.BackgroundImage.Width, y),
                new Point(x, y + theme.BackgroundImage.Height)
            };
            Rectangle srcRect = new Rectangle(Point.Empty, theme.BackgroundImage.Size);
            for (int i = 0; i < counters.Count; i++)
            {
                e.Graphics.Clip = new Region(clip);
                e.Graphics.DrawImage(theme.BackgroundImage, coners, srcRect, GraphicsUnit.Pixel, imgAttributes);
                clip.Offset(0, itemHeight + padding);
            }
        }

        private void SetVisualStyle()
        {
            MinimumSize = new Size(0, 0);
            TransparencyKey = transparencyKey;
            Opacity = leaveOpacity;
            Width = itemWidth;
            Height = itemHeight * counters.Count + padding * (counters.Count - 1);
        }

        private void CreateTimer()
        {
            delayActiveTimer.Interval = 1000;
            delayActiveTimer.Tick += DelayActiveTimer_Tick;

            easeOpacityTimer.Interval = easeOpacityInterval;
            easeOpacityTimer.Tick += EaseOpacityTimer_Tick;

            easeLocationTimer.Interval = easeLocationInterval;
            easeLocationTimer.Tick += EaseLocationTimer_Tick;

            updateTimer.Interval = normalUpdataTime;
            updateTimer.Tick += (s, e) => { Invalidate(); };
            updateTimer.Start();
        }

        private void CreateCounterView()
        {
            counters = new List<IPerformanceDataCollector>()
            {
                new CPUPerformanceCounter(),
                new RAMPerformanceCounter(),
                new DiskPerformanceCounter(),
                new NetPerformanceCounter(),
            };
            viewer = new DotView(config.FontSize) { Theme = theme };
        }

        private void ExtendDispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var counter in counters)
                    (counters as IDisposable)?.Dispose();
                (viewer as IDisposable)?.Dispose();
                theme?.Dispose();
            }
        }
    }
}
