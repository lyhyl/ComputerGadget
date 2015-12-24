using ComputerGadget.Counter;
using ComputerGadget.View;
using System.Collections.Generic;
using System.Drawing;
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
            viewer = new DotView(config.FontSize);
        }
    }
}
