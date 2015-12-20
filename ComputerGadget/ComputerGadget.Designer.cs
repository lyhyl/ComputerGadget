namespace ComputerGadget
{
    partial class ComputerGadget
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ComputerGadget));
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.iconMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.topToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.speedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.highToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.normalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iconMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.iconMenu;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Visible = true;
            // 
            // iconMenu
            // 
            this.iconMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.speedToolStripMenuItem,
            this.topToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.iconMenu.Name = "iconMenu";
            this.iconMenu.Size = new System.Drawing.Size(153, 92);
            // 
            // topToolStripMenuItem
            // 
            this.topToolStripMenuItem.Name = "topToolStripMenuItem";
            this.topToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.topToolStripMenuItem.Text = "&Top";
            this.topToolStripMenuItem.Click += new System.EventHandler(this.topToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // speedToolStripMenuItem
            // 
            this.speedToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.highToolStripMenuItem,
            this.normalToolStripMenuItem,
            this.lowToolStripMenuItem});
            this.speedToolStripMenuItem.Name = "speedToolStripMenuItem";
            this.speedToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.speedToolStripMenuItem.Text = "&Speed";
            // 
            // highToolStripMenuItem
            // 
            this.highToolStripMenuItem.Name = "highToolStripMenuItem";
            this.highToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.highToolStripMenuItem.Text = "&High";
            this.highToolStripMenuItem.Click += new System.EventHandler(this.highToolStripMenuItem_Click);
            // 
            // normalToolStripMenuItem
            // 
            this.normalToolStripMenuItem.Name = "normalToolStripMenuItem";
            this.normalToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.normalToolStripMenuItem.Text = "&Normal";
            this.normalToolStripMenuItem.Click += new System.EventHandler(this.normalToolStripMenuItem_Click);
            // 
            // lowToolStripMenuItem
            // 
            this.lowToolStripMenuItem.Name = "lowToolStripMenuItem";
            this.lowToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.lowToolStripMenuItem.Text = "&Low";
            this.lowToolStripMenuItem.Click += new System.EventHandler(this.lowToolStripMenuItem_Click);
            // 
            // ComputerGadget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.ControlBox = false;
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ComputerGadget";
            this.Opacity = 0.7D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Gadget";
            this.iconMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip iconMenu;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem topToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem speedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem highToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem normalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lowToolStripMenuItem;
    }
}

