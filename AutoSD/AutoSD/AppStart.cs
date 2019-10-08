using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace AutoSD
{
    public partial class AppStart : Form
    {
        List<string> key = new List<string>();
        int flag = 0;
        string[] appstart = File.ReadAllLines(Program.setFilePath);

        public AppStart()
        {
            InitializeComponent();
            this.HelpButtonClicked += AppStart_HelpButtonClicked;
        }

        private void AppStart_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            Process.Start(AppContext.BaseDirectory + @"\AppStart.txt");
        }

        private void AppStart_KeyDown(object sender, KeyEventArgs e)
        {
            key.Add(e.KeyData.ToString());
            label1.Text += key[key.Count - 1];
            if (!timer1.Enabled) timer1.Enabled = true;
        }

        private void SearchKey()
        {
            int index = 0;
            foreach (string i in appstart)
            {
                flag = 0;
                List<string> item = i.Split('|').ToList();
                item.ForEach(a =>
                {
                    if (key.Contains(a)) flag++;
                    if (flag == key.Count && item.Count==key.Count+1 && !appstart[index].StartsWith("=>") && appstart[index]!= Program.autoStart)
                    {
                        if(index<appstart.Length-1)
                        {
                            if (appstart[index+1].StartsWith("=>")) Clipboard.SetText(appstart[index+1].Substring("=>".Length));
                        }
                        Process.Start(item[item.Count - 1]);
                        flag = 0;
                        key.Clear();
                        if (Program.hasArg) this.Close();
                    }
                });
                index++;
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            SearchKey();
            flag = 0;
            key.Clear();
            label1.Text = "快捷键：";
        }

        private void AppStart_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.appStart = null;
        }

        private void AppStart_Load(object sender, EventArgs e)
        {
            
        }

        private void AppStart_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(Color.White, 3), label1.Location.X - 10, label1.Location.Y - 10,this.Width - (label1.Location.X - 10) * 2-15, this.Height - (label1.Location.Y - 10) * 2-40);
        }
    }
}
