using Goblintools.Types;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Goblintools.WinForms
{
    public partial class MainForm : Form
    {
        private SavedVariableProcessor SavedVariable { get; set; }
        private GTGoblinDBManager Manager { get; set; }
        private ImageCreator Creator { get; set; }

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Manager = new GTGoblinDBManager();
            Creator = new ImageCreator();

            SavedVariable = new SavedVariableProcessor();
            SavedVariable.OnGoblinDBChanged.OnReceive.Subscribe(Work);
            SetSavedVariable();
            SetAutoStart(true);
        }

        private void VerifySettings()
        {
            if (!Directory.Exists(Goblintools.Properties.Settings.Default.WowPath))
                throw new DirectoryNotFoundException($"Unable to start Goblintools. WowPath '{Goblintools.Properties.Settings.Default.WowPath}' does not exist.");
        }

        private void Work(GTGoblinDB goblinDB)
        {
            if (InvokeRequired)
                Invoke(new Action<GTGoblinDB>(Work), goblinDB);
            else
                AddGoblinDB(goblinDB);
        }

        private void AddGoblinDB(GTGoblinDB goblinDB)
        {
            Manager.Add(goblinDB);

            UpdateComboboxes();
        }

        private void UpdateComboboxes()
        {
            var selected = cboAccounts.SelectedValue;

            cboAccounts.DataSource = Manager.GetAccounts();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                SystemTray.Visible = true;
                SystemTray.BalloonTipText = "Goblintools is running in background.";
                SystemTray.ShowBalloonTip(500);

                Hide();
            }
            else if (WindowState == FormWindowState.Normal)
            {
                SystemTray.Visible = false;
            }
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();

            WindowState = FormWindowState.Normal;
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            CloseApplication();
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CloseApplication();
        }

        private void CloseApplication()
        {
            Close();
        }

        private void mnuWatchClassicCharacters_Click(object sender, EventArgs e)
        {
            mnuWatchCharacters.Checked = !mnuWatchCharacters.Checked;

            SetSavedVariable();
        }

        private void mnuAutoStart_Click(object sender, EventArgs e)
        {
            mnuAutoStart.Checked = !mnuAutoStart.Checked;

            SetAutoStart();
        }

        private void SetSavedVariable()
        {
            if (mnuWatchCharacters.Checked)
                SavedVariable.Start();
            else
                SavedVariable.Stop();
        }

        private void SetAutoStart(bool initialize = false)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                if (initialize)
                {
                    mnuAutoStart.Checked = (key.GetValue(Text) != null);
                }
                else
                {
                    if (mnuAutoStart.Checked)
                        key.SetValue(Text, Application.ExecutablePath);
                    else if (key.GetValue(Text) != null)
                        key.DeleteValue(Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedAccount = (string)cboAccounts.SelectedItem;
            var selectedCharacter = (string)cboCharacters.SelectedItem;

            var availableCharacters = Manager.GetCharacterNames(selectedAccount);

            if (string.IsNullOrWhiteSpace(selectedAccount))
                cboCharacters.DataSource = null;
            else
                cboCharacters.DataSource = availableCharacters;

            if (selectedCharacter == null && availableCharacters.Length > 0)
                cboCharacters.SelectedIndex = 0;
            else
                cboCharacters.SelectedItem = selectedCharacter;
        }

        private void cboCharacters_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedAccount = (string)cboAccounts.SelectedItem;
            var selectedCharacter = (string)cboCharacters.SelectedItem;

            if (!string.IsNullOrWhiteSpace(selectedCharacter))
            {
                var list = Manager.GetCharacter(selectedAccount, selectedCharacter);
                var goblinDB = list.FirstOrDefault();

                if (goblinDB != null)
                    updIndex.Maximum = goblinDB.Records.Count - 1;

                updIndex.Value = 0;
                updIndex.Enabled = (goblinDB != null);

                DrawImage();
            }
        }

        private void updIndex_ValueChanged(object sender, EventArgs e)
        {
            Panel.Visible = updIndex.Enabled;

            DrawImage();
        }

        private void DrawImage()
        {
            if (Panel.Visible)
            {
                var selectedAccount = (string)cboAccounts.SelectedItem;
                var selectedCharacter = (string)cboCharacters.SelectedItem;
                var index = (int)updIndex.Value;

                if (!string.IsNullOrWhiteSpace(selectedCharacter))
                {
                    var list = Manager.GetCharacter(selectedAccount, selectedCharacter);
                    var goblinDB = list.FirstOrDefault();

                    if (goblinDB != null && index < goblinDB.Records.Count)
                    {
                        using (var graphics = Panel.CreateGraphics())
                        {
                            var bitmap = Creator.CreateImage(goblinDB.Records.Format, goblinDB.Records.Data[index]);

                            graphics.DrawImage(bitmap, 0, 0);
                        }
                    }
                }
            }
        }
    }
}
