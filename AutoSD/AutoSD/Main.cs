using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace AutoSD
{
    public partial class Main : Form
    {
        DateTime nowTime = DateTime.Now,sdTime;
        readonly string setFilePath = Application.StartupPath + @"\Set.txt";
        const string likPath = @"C:\Users\baron.ren\Documents\Lik";

        public Main()
        {
            InitializeComponent();
            sdTime = Convert.ToDateTime(ReadSet());
            label3.Text = sdTime.ToString("H时m分s秒");
            Directory.GetFiles(likPath).ToList().ForEach(a =>
            {
                ToolStripItem toolStripItem=contextMenuStrip1.Items.Add(Path.GetFileName(a), null, contextMenuStrip_Click);
                toolStripItem.ForeColor = Color.White;
            });
            ShowAppStartForm();
        }

        private void contextMenuStrip_Click(object sender, EventArgs e)
        {
            if(sender is ToolStripItem)
            {
                try { Process.Start(likPath + @"\" + (sender as ToolStripItem).Text); } catch(Exception exp) { new MsgBox(exp.Message).ShowDialog(this); }
            }
        }

        private string ReadSet(string result="")
        {
            foreach(string i in File.ReadAllLines(setFilePath))
            {
                bool flag = false;
                i.Split('|').ToList().ForEach(a =>
                {
                    if (flag) result = a;
                    flag = a.Contains(nowTime.ToString("yyyy-MM-dd"));
                });
                if (flag) break;
            }
            return result==""? "18:00":result;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            notifyIcon1.Dispose();
            Environment.Exit(0);
        }

        private void button3_Click(object sender, EventArgs e)=> this.Hide();

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            if (e is MouseEventArgs)
            {
                if ((e as MouseEventArgs).Button == MouseButtons.Left) this.Show();
                else if ((e as MouseEventArgs).Button == MouseButtons.Right) new Node().Show();
            }
        }

        private void button1_Click(object sender, EventArgs e)=> Process.Start(setFilePath);

        private void Main_Shown(object sender, EventArgs e)=> this.Hide();

        private void NotifyIcon1_Click(object sender, EventArgs e)
        {
            if(e is MouseEventArgs)if((e as MouseEventArgs).Button == MouseButtons.Left) ShowAppStartForm();
        }

        private void ShowAppStartForm()
        {
            if (Program.appStart is null) Program.appStart = new AppStart();
            Program.appStart.Show();
            Program.appStart.BringToFront();
            Program.appStart.Focus();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text =(nowTime = nowTime.AddSeconds(1)).ToString("yyyy-MM-dd_HH:mm:ss");
            decimal diffHour=0, diffMin=0, diffSec=0,remSec=0,allSec=32400;
            if (DateTime.Compare(nowTime, sdTime) == -1)
                label2.Text = (diffHour = nowTime.Hour == sdTime.Hour ? 0 : (diffHour = sdTime.Hour - nowTime.Hour - 1)).ToString() + "时"
                 + (nowTime.Minute >= sdTime.Minute ? (diffMin = 59 - nowTime.Minute + sdTime.Minute).ToString() : (diffMin = sdTime.Minute - nowTime.Minute - 1).ToString()) + "分"
                 + (nowTime.Second >= sdTime.Second ? (diffSec = 59 - nowTime.Second + sdTime.Second).ToString() : (diffSec = sdTime.Second - nowTime.Second - 1).ToString()) + "秒";
            else if (label2.Text == "0时0分0秒")
            {
                timer1.Enabled = false;
                Process.Start(@"C:\Windows\System32\shutdown.exe", "-s -t 0 -hybrid");
                notifyIcon1.Dispose();
                Environment.Exit(0);
            }
            remSec = diffHour * 60 * 60 + diffMin * 60 + diffSec;
            notifyIcon1.Text = "AutoSD\n" + label2.Text + "\n" + label3.Text+"\n"+(Math.Round((remSec/allSec)*100,2)).ToString()+"%";
        }
    }
}
