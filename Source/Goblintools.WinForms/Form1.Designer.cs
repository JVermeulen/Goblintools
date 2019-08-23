namespace Goblintools.WinForms
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuWatchCharacters = new System.Windows.Forms.ToolStripMenuItem();
            this.SystemTray = new System.Windows.Forms.NotifyIcon(this.components);
            this.SystemTrayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.cboAccounts = new System.Windows.Forms.ComboBox();
            this.cboCharacters = new System.Windows.Forms.ComboBox();
            this.Panel = new System.Windows.Forms.Panel();
            this.updIndex = new System.Windows.Forms.NumericUpDown();
            this.MainMenu.SuspendLayout();
            this.SystemTrayMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updIndex)).BeginInit();
            this.SuspendLayout();
            // 
            // MainMenu
            // 
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuSettings});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Size = new System.Drawing.Size(1026, 24);
            this.MainMenu.TabIndex = 0;
            this.MainMenu.Tag = "Menu";
            this.MainMenu.Text = "menuStrip1";
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuExit});
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(37, 20);
            this.mnuFile.Text = "&File";
            // 
            // mnuExit
            // 
            this.mnuExit.Name = "mnuExit";
            this.mnuExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.mnuExit.Size = new System.Drawing.Size(134, 22);
            this.mnuExit.Text = "E&xit";
            this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
            // 
            // mnuSettings
            // 
            this.mnuSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuWatchCharacters});
            this.mnuSettings.Name = "mnuSettings";
            this.mnuSettings.Size = new System.Drawing.Size(61, 20);
            this.mnuSettings.Text = "&Settings";
            // 
            // mnuWatchCharacters
            // 
            this.mnuWatchCharacters.Checked = true;
            this.mnuWatchCharacters.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuWatchCharacters.Name = "mnuWatchCharacters";
            this.mnuWatchCharacters.Size = new System.Drawing.Size(167, 22);
            this.mnuWatchCharacters.Text = "Watch Characters";
            this.mnuWatchCharacters.Click += new System.EventHandler(this.mnuWatchClassicCharacters_Click);
            // 
            // SystemTray
            // 
            this.SystemTray.ContextMenuStrip = this.SystemTrayMenu;
            this.SystemTray.Icon = ((System.Drawing.Icon)(resources.GetObject("SystemTray.Icon")));
            this.SystemTray.Text = "Goblintools";
            this.SystemTray.Visible = true;
            // 
            // SystemTrayMenu
            // 
            this.SystemTrayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem1});
            this.SystemTrayMenu.Name = "SystemTrayMenu";
            this.SystemTrayMenu.Size = new System.Drawing.Size(104, 54);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.showToolStripMenuItem.Text = "Show";
            this.showToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(100, 6);
            // 
            // exitToolStripMenuItem1
            // 
            this.exitToolStripMenuItem1.Name = "exitToolStripMenuItem1";
            this.exitToolStripMenuItem1.Size = new System.Drawing.Size(103, 22);
            this.exitToolStripMenuItem1.Text = "E&xit";
            this.exitToolStripMenuItem1.Click += new System.EventHandler(this.exitToolStripMenuItem1_Click);
            // 
            // cboAccounts
            // 
            this.cboAccounts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAccounts.FormattingEnabled = true;
            this.cboAccounts.Location = new System.Drawing.Point(12, 27);
            this.cboAccounts.Name = "cboAccounts";
            this.cboAccounts.Size = new System.Drawing.Size(150, 21);
            this.cboAccounts.TabIndex = 5;
            this.cboAccounts.SelectedIndexChanged += new System.EventHandler(this.cboAccounts_SelectedIndexChanged);
            // 
            // cboCharacters
            // 
            this.cboCharacters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCharacters.FormattingEnabled = true;
            this.cboCharacters.Location = new System.Drawing.Point(168, 27);
            this.cboCharacters.Name = "cboCharacters";
            this.cboCharacters.Size = new System.Drawing.Size(150, 21);
            this.cboCharacters.TabIndex = 5;
            this.cboCharacters.SelectedIndexChanged += new System.EventHandler(this.cboCharacters_SelectedIndexChanged);
            // 
            // Panel
            // 
            this.Panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel.Location = new System.Drawing.Point(12, 54);
            this.Panel.Name = "Panel";
            this.Panel.Size = new System.Drawing.Size(1002, 668);
            this.Panel.TabIndex = 6;
            // 
            // updIndex
            // 
            this.updIndex.Location = new System.Drawing.Point(324, 27);
            this.updIndex.Name = "updIndex";
            this.updIndex.Size = new System.Drawing.Size(120, 20);
            this.updIndex.TabIndex = 7;
            this.updIndex.ValueChanged += new System.EventHandler(this.updIndex_ValueChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1026, 763);
            this.Controls.Add(this.updIndex);
            this.Controls.Add(this.Panel);
            this.Controls.Add(this.cboCharacters);
            this.Controls.Add(this.cboAccounts);
            this.Controls.Add(this.MainMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MainMenu;
            this.MinimumSize = new System.Drawing.Size(1042, 802);
            this.Name = "MainForm";
            this.Text = "Goblintools";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.SystemTrayMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.updIndex)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuExit;
        private System.Windows.Forms.NotifyIcon SystemTray;
        private System.Windows.Forms.ContextMenuStrip SystemTrayMenu;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuSettings;
        private System.Windows.Forms.ToolStripMenuItem mnuWatchCharacters;
        private System.Windows.Forms.ComboBox cboAccounts;
        private System.Windows.Forms.ComboBox cboCharacters;
        private System.Windows.Forms.Panel Panel;
        private System.Windows.Forms.NumericUpDown updIndex;
    }
}

