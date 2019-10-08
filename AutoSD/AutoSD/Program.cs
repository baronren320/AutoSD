using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoSD
{
    static class Program
    {
        public static bool hasArg = false;
        public static readonly string setFilePath = Application.StartupPath + @"\AppStart.txt";
        public const string autoStart= "AUTOSTART";
        public static AppStart appStart;

        [STAThread]
        static void Main(string[] arg)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (arg.Length==0)
            {
                AutoStart();
                Main main = new Main();
                Application.Run(main);
            }
            else if (arg[0] != "-a")
            {
                Main main = new Main();
                Application.Run(main);
            }
            else
            {
                hasArg = true;
                Application.Run(new AppStart());
            }
        }

        static void AutoStart()
        {
            int index = 0;
            string[] appstart = File.ReadAllLines(setFilePath);
            foreach (string i in appstart)
            {
                if (i.StartsWith("=>") || i == autoStart) index++;
                else
                {
                    List<string> item = i.Split('|').ToList();
                    if (!appstart[index].StartsWith("=>") && appstart[index] != autoStart)
                    {
                        if (index < appstart.Length - 1)
                        {
                            if (appstart[index + 1] == autoStart) Process.Start(item[item.Count - 1]);
                            else if (index < appstart.Length - 2)
                            {
                                if (appstart[index + 2] == autoStart && appstart[index + 1].StartsWith("=>")) Process.Start(item[item.Count - 1]);
                            }
                        }
                    }
                    index++;
                }
            }
        }
    }

    public class NodeModel
    {
        //[JsonIgnore]
        public string NodeId { get; set; }
        public string Desc { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<NodeModel> Child { get; set; }
    }

    public class NodeModelUp
    {
        [JsonIgnore]
        public string NodeId { get; set; }
        public string Desc { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<NodeModelUp> Parent { get; set; } 
    }
}
