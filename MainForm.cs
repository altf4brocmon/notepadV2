using NotepadV2.Forms;
using NotepadV2.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NotepadV2
{
    public partial class MainForm : Form
    {
        private int lastThemeIndex = -1;
        private int lastFontColorIndex = -1;
        private bool settingsChanged = false;

        public MainForm()
        {
            InitializeComponent();
            this.FormClosing += MainForm_FormClosing;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (File.Exists("settings.config"))
            {
                string[] settings = File.ReadAllText("settings.config").Split(',');
                if (settings.Length == 2 &&
                    int.TryParse(settings[0], out int themeIndex) &&
                    int.TryParse(settings[1], out int fontIndex))
                {
                    comboBox1.SelectedIndex = themeIndex;
                    comboBox2.SelectedIndex = fontIndex;
                }
                else
                {
                    comboBox1.SelectedIndex = 1;
                    comboBox2.SelectedIndex = 2;
                }
            }
            else
            {
                comboBox1.SelectedIndex = 1;
                comboBox2.SelectedIndex = 2;
            }

            lastThemeIndex = comboBox1.SelectedIndex;
            lastFontColorIndex = comboBox2.SelectedIndex;
            AddNewTab("Document 1");
            ApplyThemeToAllControls();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (settingsChanged)
            {
                DialogResult result = MessageBox.Show(
                    "Would you like to save your settings?\nChanges were made since the last save\n\nIf you save make sure to put the .exe inside a folder",
                    "Save Settings",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    lastThemeIndex = comboBox1.SelectedIndex;
                    lastFontColorIndex = comboBox2.SelectedIndex;
                    settingsChanged = false;
                    File.WriteAllText("settings.config", $"{lastThemeIndex},{lastFontColorIndex}");
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start("https://discord.gg/skNbN4PU8Z");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var rtb = GetCurrentRichTextBox();
            if (rtb != null)
            {
                bool success = NativeInterop.SaveTextViaDialog(rtb.Text);
                if (!success)
                    MessageBox.Show("Failed to save file or operation was canceled.");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string loadedText = NativeInterop.LoadTextViaDialog();
            if (loadedText != null)
            {
                var rtb = GetCurrentRichTextBox();
                if (rtb != null)
                    rtb.Text = loadedText;
            }
            else
            {
                MessageBox.Show("Failed to load file or operation was canceled.");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            AddNewTab("Document " + (tabControl1.TabPages.Count + 1));
            ApplyThemeToAllControls();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (tabControl1.TabPages.Count > 0)
                tabControl1.TabPages.Remove(tabControl1.SelectedTab);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != lastThemeIndex)
                settingsChanged = true;

            ApplyThemeToAllControls();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex != lastFontColorIndex)
                settingsChanged = true;

            var rtb = GetCurrentRichTextBox();
            if (rtb == null) return;

            rtb.ForeColor = comboBox2.SelectedIndex switch
            {
                0 => Color.Black,
                1 => Color.Gray,
                2 => Color.White,
                3 => Color.Red,
                4 => Color.Blue,
                5 => Color.Green,
                _ => Color.Black
            };
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e) { }

        private void AddNewTab(string title = "New Tab")
        {
            TabPage tabPage = new TabPage(title);
            RichTextBox rtb = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                BackColor = comboBox1.SelectedIndex == 1 ? Color.Black : Color.White,
                ForeColor = comboBox2.SelectedIndex switch
                {
                    0 => Color.Black,
                    1 => Color.Gray,
                    2 => Color.White,
                    3 => Color.Red,
                    4 => Color.Blue,
                    5 => Color.Green,
                    _ => Color.Black
                }
            };
            tabPage.BackColor = rtb.BackColor;
            tabPage.Controls.Add(rtb);
            tabControl1.TabPages.Add(tabPage);
            tabControl1.SelectedTab = tabPage;
        }

        private RichTextBox GetCurrentRichTextBox()
        {
            if (tabControl1.SelectedTab?.Controls.Count > 0)
                return tabControl1.SelectedTab.Controls[0] as RichTextBox;
            return null;
        }

        private void ApplyThemeToAllControls()
        {
            Color back = comboBox1.SelectedIndex == 1 ? Color.Black : Color.White;
            Color fore = comboBox1.SelectedIndex == 1 ? Color.White : Color.Black;

            this.BackColor = back;
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Label || ctrl is Button || ctrl is ComboBox || ctrl is TabControl)
                {
                    ctrl.BackColor = back;
                    ctrl.ForeColor = fore;
                }
            }

            foreach (TabPage tab in tabControl1.TabPages)
            {
                tab.BackColor = back;
                if (tab.Controls.Count > 0 && tab.Controls[0] is RichTextBox rtb)
                {
                    rtb.BackColor = back;
                    rtb.ForeColor = comboBox2.SelectedIndex switch
                    {
                        0 => Color.Black,
                        1 => Color.Gray,
                        2 => Color.White,
                        3 => Color.Red,
                        4 => Color.Blue,
                        5 => Color.Green,
                        _ => Color.Black
                    };
                }
            }

            label1.ForeColor = label2.ForeColor = label3.ForeColor = fore;
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form form = new Settings();
            form.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Form form = new Tools();
            form.Show();
        }
    }
}