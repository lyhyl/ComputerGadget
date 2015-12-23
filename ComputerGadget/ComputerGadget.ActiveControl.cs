using Gma.UserActivityMonitor;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ComputerGadget
{
    public partial class ComputerGadget
    {
        private Timer delayActiveTimer = new Timer();
        private bool globalHooks = true;

        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_TRANSPARENT = 0x00000020;
                const int WS_EX_TOOLWINDOW = 0x00000080;
                const int WS_EX_NOACTIVATE = 0x08000000;
                CreateParams cp = base.CreateParams;
                if (globalHooks)
                    cp.ExStyle |= WS_EX_TRANSPARENT;
                cp.ExStyle |= WS_EX_TOOLWINDOW;
                cp.ExStyle |= WS_EX_NOACTIVATE;
                return cp;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            HookManager.MouseMove += HookManager_MouseMove;
        }

        private void DelayActiveTimer_Tick(object sender, EventArgs e)
        {
            SetHookState(false);
            delayActiveTimer.Stop();
        }

        private void HookManager_MouseMove(object sender, MouseEventArgs e)
        {
            long now = DateTime.Now.Ticks;
            if (globalHooks && DesktopBounds.Contains(e.Location) && InsideDataView(PointToClient(e.Location)))
            {
                if (!delayActiveTimer.Enabled)
                    delayActiveTimer.Start();
                //Debug.WriteLine("Enter");
            }
            else if (delayActiveTimer.Enabled)
            {
                delayActiveTimer.Stop();
                //Debug.WriteLine("Leave");
            }
        }

        private void SetHookState(bool state)
        {
            globalHooks = state;
            UpdateStyles();
        }

        private bool InsideDataView(Point location)
        {
            Rectangle clip = new Rectangle(0, 0, itemWidth, itemHeight);
            for (int i = 0; i < counters.Count; i++)
            {
                if (clip.Contains(location))
                    return true;
                clip.Y += itemHeight + padding;
            }
            return false;
        }
    }
}
