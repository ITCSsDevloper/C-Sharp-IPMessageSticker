﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace C_Sharp_IPMessageSticker
{
    public partial class FormTestFunction : Form
    {
        private bool isActive = false;

        private int screenWidth;
        private int screenHeight;

        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool turnon);


        private string PathPreviewImage = "";

        public FormTestFunction()
        {
            InitializeComponent();

            screenWidth = Screen.PrimaryScreen.Bounds.Width;
            screenHeight = Screen.PrimaryScreen.Bounds.Height;


            cSticker.GetStickers();


            fPreview = new TestPreview();
           fPreview.Show();
            // Thread.Sleep(100);
            // this.Hide();
            //backgroundWorker1.RunWorkerAsync();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Image image = cMain.LoadImageFormPath(@"D:\f1.gif");
            cMain.Clipbroad_AddImage(image);
            SwitchToThisWindow(cMain.GetProcessIPMSG().MainWindowHandle, true);

            Thread.Sleep(50);
            SendKeys.Send("^V");
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                Thread.Sleep(200);

                var process = cMain.GetProcessIPMSG();
                if (process != null)
                {
                    if (process.MainWindowTitle.Contains("Send Message"))
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            if (!isActive)
                            {
                                this.Location = new Point(screenWidth - this.Width + 10, screenHeight - (this.Height + 30));
                                this.Show();
                            }

                            isActive = true;
                        });
                    }
                    else
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            if (isActive)
                            {
                                this.Hide();
                            }

                            isActive = false;
                        });
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = null;

            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;

                //string[] files = Directory.GetFiles(folderBrowserDialog1.SelectedPath);
                //MessageBox.Show("Files found: " + files.Length.ToString(), "Message");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            cSticker.ImportStickers(txtStickName.Text.Trim(), textBox1.Text.Trim());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            cSticker.DeleteSticker(textBox2.Text.Trim());
      
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var st = cSticker.GetStickers().First(x => x.NameHeader == "Basic");
            cSticker.AddToRecent(st.Stickers.First());
        }

        private void button6_Click(object sender, EventArgs e)
        {
            cSticker.ClearRecent();
        }

        private void FormTestFunction_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.ApplicationExitCall)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
                if (this.WindowState == FormWindowState.Minimized)
                {
                    Hide();
                    notifyIcon.Visible = true;
                    notifyIcon.ShowBalloonTip(500);
                }
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show(@"You want to close the program ?", @"Confirm ?", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.OK)
            {
                Application.Exit();
            }
        }


        public TestPreview fPreview { get; set; }
        private void button8_Click(object sender, EventArgs e)
        {
            fPreview.PathImage = @"D:\bokeh6.jpg";
        }
    }
}