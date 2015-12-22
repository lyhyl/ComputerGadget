using ComputerGadget.View;
using System;

namespace ComputerGadget
{
    public partial class ComputerGadget
    {
        private const int highUpdateTime = 500;
        private const int normalUpdataTime = 1000;
        private const int lowUpdateTime = 2000;

        private void ExtendDispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var counter in counters)
                    (counters as IDisposable)?.Dispose();
                (viewer as IDisposable)?.Dispose();
            }
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
            updateTimer.Interval = lowUpdateTime;
        }

        private void normalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            updateTimer.Interval = normalUpdataTime;
        }

        private void highToolStripMenuItem_Click(object sender, EventArgs e)
        {
            updateTimer.Interval = highUpdateTime;
        }

        private void dotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (viewer is DotView)
                return;
            viewer = new DotView(config.FontSize);
        }

        private void stripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (viewer is StripView)
                return;
            viewer = new StripView(config.FontSize);
        }

        private void darkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewer.Theme = Theme.DarkTheme;
        }

        private void lightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewer.Theme = Theme.LightTheme;
        }
    }
}
