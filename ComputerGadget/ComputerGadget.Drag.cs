using System;
using System.Drawing;
using System.Windows.Forms;

namespace ComputerGadget
{
    public partial class ComputerGadget
    {
        private const int easeLocationFPS = 50;
        private const int easeLocationInterval = 1000 / easeLocationFPS;

        private bool dragging = false;
        private Point prvLocation;

        private Point targetLocation = new Point();
        private Timer easeLocationTimer = new Timer();
        private double easeLocationPercentagePerTick = .75;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (easeLocationTimer.Enabled)
                easeLocationTimer.Stop();
            switch (e.Button)
            {
                case MouseButtons.Left:
                    dragging = true;
                    prvLocation = e.Location;
                    break;
                case MouseButtons.Right:
                    iconMenu.Show(PointToScreen(e.Location));
                    break;
                default:
                    break;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if(dragging)
            {
                int dx = e.X - prvLocation.X;
                int dy = e.Y - prvLocation.Y;
                Location = new Point(Location.X + dx, Location.Y + dy);
                prvLocation = new Point(e.X - dx, e.Y - dy);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            dragging = false;
            DockToClosestEdge();
        }

        private void DockToClosestEdge()
        {
            Point center = PointToScreen(new Point(Width / 2, Height / 2));
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            if (screen.Width < Width || screen.Height < Height)
                return;
            screen.X += Width / 2 + padding;
            screen.Y += Height / 2 + padding;
            screen.Width -= Width + padding * 2;
            screen.Height -= Height + padding * 2;
            center.X = Clamp(screen.Left, screen.Right, center.X);
            center.Y = Clamp(screen.Top, screen.Bottom, center.Y);
            int[] d = new int[4] {
                center.X - screen.Left, screen.Right - center.X,
                center.Y - screen.Top, screen.Bottom - center.Y
            };
            int mind = Math.Min(Math.Min(d[0], d[1]), Math.Min(d[2], d[3]));
            if (mind == d[0])
                center.X = screen.Left;
            else if (mind == d[1])
                center.X = screen.Right;
            else if (mind == d[2])
                center.Y = screen.Top;
            else
                center.Y = screen.Bottom;
            EaseLocationTo(new Point(center.X - Width / 2, center.Y - Height / 2));
        }
        
        private void EaseLocationTo(Point location)
        {
            targetLocation = location;
            if (!easeLocationTimer.Enabled)
                easeLocationTimer.Start();
        }

        private void EaseLocationTimer_Tick(object sender, EventArgs e)
        {
            int dx = targetLocation.X - Location.X;
            int dy = targetLocation.Y - Location.Y;
            int lensq = dx * dx + dy * dy;
            if (lensq < 2)
            {
                Location = targetLocation;
                easeLocationTimer.Stop();
            }
            else
            {
                int nx = (int)Math.Round(Location.X + dx * easeLocationPercentagePerTick);
                int ny = (int)Math.Round(Location.Y + dy * easeLocationPercentagePerTick);
                Location = new Point(nx, ny);
            }
        }

        private int Clamp(int min,int max,int v)
        {
            return Math.Min(Math.Max(min, v), max);
        }
    }
}
