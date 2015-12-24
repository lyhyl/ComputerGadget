using System;
using System.Drawing;
using System.Windows.Forms;

namespace ComputerGadget
{
    public partial class ComputerGadget
    {
        private const int easeOpacityFPS = 50;
        private const int easeOpacityInterval = 1000 / easeOpacityFPS;
        private const double focusOpacity = .8;
        private const double leaveOpacity = .2;

        private double targetOpacity = leaveOpacity;
        private Timer easeOpacityTimer = new Timer();
        private double easeOpacityPerTick = (1.0 / easeOpacityFPS) * 1.5;

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseHover(e);
            EaseOpacityTo(focusOpacity);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            EaseOpacityTo(leaveOpacity);
            SetHookState(true);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Location = GetDisplayPosition();
        }

        private Point GetDisplayPosition()
        {
            Rectangle area = Screen.PrimaryScreen.WorkingArea;
            int x = (area.Width - Width) / 2;
            int y = (area.Height - Height) / 2;
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

        private void EaseOpacityTimer_Tick(object sender, EventArgs e)
        {
            if (Math.Abs(Opacity - targetOpacity) < easeOpacityPerTick)
            {
                Opacity = targetOpacity;
                easeOpacityTimer.Stop();
            }
            else if (Opacity < targetOpacity)
                Opacity += easeOpacityPerTick;
            else
                Opacity -= easeOpacityPerTick;
        }

        private void EaseOpacityTo(double opacity)
        {
            targetOpacity = opacity;
            if (!easeOpacityTimer.Enabled)
                easeOpacityTimer.Start();
        }
    }
}
