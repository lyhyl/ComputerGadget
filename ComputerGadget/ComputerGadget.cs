using ComputerGadget.Counter;
using ComputerGadget.View;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace ComputerGadget
{
    public partial class ComputerGadget : Form
    {
        private const int highUpdateTime = 500;
        private const int normalUpdataTime = 1000;
        private const int lowUpdateTime = 2000;

        private const int itemWidth = 200;
        private const int itemHeight = 50;
        private const int padding = 5;

        private const int easeFPS = 50;
        private const int easeTimePerTick = 1000 / easeFPS;

        private const double focusOpacity = .7;
        private const double leaveOpacity = .3;

        private Color transparencyKey = Color.FromArgb(0, 255, 0);

        private Config config = new Config();

        private Timer timer = new Timer();
        private ICounter[] counters = null;
        private IDataViwer viewer = null;

        private double targetOpacity = .7;
        private Timer easeOpacityTimer = new Timer();
        private double easePerTick = (1.0 / easeFPS) * 2;

        public ComputerGadget()
        {
            InitializeComponent();

            MinimumSize = new Size(0, 0);
            TransparencyKey = transparencyKey;
            Opacity = leaveOpacity;

            timer.Interval = normalUpdataTime;
            timer.Tick += (s, e) => { Invalidate(); };
            timer.Start();

            easeOpacityTimer.Interval = easeTimePerTick;
            easeOpacityTimer.Tick += EaseOpacityTimer_Tick;

            counters = new ICounter[]
            {
                new CPUPerformanceCounter() { DataSize = 1000 },
                new RAMPerformanceCounter() { DataSize = 1000 },
                new NetPerformanceCounter() { DataSize = 1000 },
            };
            viewer = new DotView(config.FontSize);

            Width = itemWidth;
            Height = itemHeight * counters.Length + padding * (counters.Length - 1);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_TOOLWINDOW = 0x00000080;
                const int WS_EX_NOACTIVATE = 0x08000000;
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_TOOLWINDOW;
                cp.ExStyle |= WS_EX_NOACTIVATE;
                return cp;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Point position = GetDisplayPosition();
            SetDesktopLocation(position.X, position.Y);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseHover(e);
            EaseOpacityTo(focusOpacity);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            EaseOpacityTo(leaveOpacity);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.Graphics.PageUnit = GraphicsUnit.Pixel;

            e.Graphics.FillRectangle(new SolidBrush(transparencyKey), e.ClipRectangle);
            Rectangle clip = new Rectangle(0, 0, itemWidth, itemHeight);

            //GraphicsPath path = CreateRoundedRectanglePath(clip, 5);
            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(clip);
            
            foreach (var counter in counters)
            {
                e.Graphics.FillPath(Brushes.Black, path);
                viewer.Draw(e.Graphics, clip, counter.UpdateAndGetData(), counter.Message);
                e.Graphics.TranslateTransform(0, (itemHeight + padding));
            }
        }

        private void EaseOpacityTimer_Tick(object sender, EventArgs e)
        {
            if (Math.Abs(Opacity - targetOpacity) < easePerTick)
            {
                Opacity = targetOpacity;
                easeOpacityTimer.Stop();
            }
            else if (Opacity < targetOpacity)
                Opacity += easePerTick;
            else
                Opacity -= easePerTick;
        }

        private void EaseOpacityTo(double opacity)
        {
            targetOpacity = opacity;
            if (!easeOpacityTimer.Enabled)
                easeOpacityTimer.Start();
        }

        private Point GetDisplayPosition()
        {
            Rectangle area = Screen.PrimaryScreen.WorkingArea;
            int x, y;
            x = (area.Width - Width) / 2;
            y = (area.Height - Height) / 2;
            switch (config.Position)
            {
                case Config.Side.Top:
                    y = padding;
                    break;
                case Config.Side.Bottom:
                    y = area.Bottom - padding - Height;
                    break;
                case Config.Side.Left:
                    x = padding;
                    break;
                case Config.Side.Right:
                    x = area.Right - padding - Width;
                    break;
                case Config.Side.Top | Config.Side.Left:
                    x = padding;
                    y = padding;
                    break;
                case Config.Side.Bottom | Config.Side.Left:
                    y = area.Bottom - padding - Height;
                    x = padding;
                    break;
                case Config.Side.Top | Config.Side.Right:
                    x = area.Right - padding - Width;
                    y = padding;
                    break;
                case Config.Side.Bottom | Config.Side.Right:
                default:
                    x = area.Right - padding - Width;
                    y = area.Bottom - padding - Height;
                    break;
            }
            return new Point(x, y);
        }

        private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int cornerRadius)
        {
            GraphicsPath roundedRect = new GraphicsPath();
            roundedRect.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
            roundedRect.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
            roundedRect.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            roundedRect.AddLine(rect.Right - cornerRadius * 2, rect.Bottom, rect.X + cornerRadius * 2, rect.Bottom);
            roundedRect.AddArc(rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            roundedRect.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
            roundedRect.CloseFigure();
            return roundedRect;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void topToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TopMost = !TopMost;
            topToolStripMenuItem.Checked = TopMost;
        }

        private void lowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Interval = lowUpdateTime;
        }

        private void normalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Interval = normalUpdataTime;
        }

        private void highToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Interval = highUpdateTime;
        }
    }
}
