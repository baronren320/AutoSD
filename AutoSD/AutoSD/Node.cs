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
using Newtonsoft.Json;
using System.Diagnostics;

namespace AutoSD
{
    public partial class Node : Form
    {
        readonly string mapFilePath = Application.StartupPath + @"\Map";
        string mapName = "";
        List<Tuple<string, List<string>, string, List<string>>> nodes = new List<Tuple<string, List<string>, string, List<string>>>();
        List<string> history = new List<string>();

        public Node()=> InitializeComponent();

        private void Node_Load(object sender, EventArgs e)
        {
            mapToolStripMenuItem.DropDownItems.Clear();
            foreach (string i in Directory.GetDirectories(mapFilePath))
            {
                mapToolStripMenuItem.DropDownItems.Add(i.Substring(i.LastIndexOf('\\')+1),null, mapToolStripMenuClick);
            }
        }

        private void mapToolStripMenuClick(object sender, EventArgs e)
        {
            LoadMap(mapName = (sender as ToolStripItem).Text);
            historyToolStripMenuItem.DropDownItems.Clear();
        }

        private void jsonToolStripMenuClick(object sender, EventArgs e)
        {
            if (checkedListBox1.SelectedIndex != -1)
            {
                if (sender is ToolStripItem)
                {
                    if ((sender as ToolStripItem).Text == "Down") Json(FillModel(nodes[checkedListBox1.SelectedIndex]));
                    else Json(FillModelUp(nodes[checkedListBox1.SelectedIndex]));
                }
            }
        }

        private void LoadMap(string mapName, Tuple<string, List<string>, string, List<string>> node = null)
        {
            checkedListBox1.Items.Clear();
            nodes.Clear();
            if (node is null)
            {
                foreach (string i in Directory.GetFiles(mapFilePath+@"\"+mapName))
                {
                    string[] item = File.ReadAllLines(i);
                    nodes.Add(new Tuple<string, List<string>, string, List<string>>(Path.GetFileNameWithoutExtension(i), item[0].Split('|').ToList(), item[1], item[2].Split('|').ToList()));
                }
            }
            else
            {
                foreach (string i in node.Item4)
                {
                    if (i != "NULL")
                    {
                        string[] item = File.ReadAllLines(mapFilePath + @"\" + mapName + @"\" + i + ".txt");
                        nodes.Add(new Tuple<string, List<string>, string, List<string>>(Path.GetFileNameWithoutExtension(i), item[0].Split('|').ToList(), item[1], item[2].Split('|').ToList()));
                    }
                }
            }
            foreach (var j in nodes) if (j.Item3.Length <= 10) checkedListBox1.Items.Add(j.Item3); else checkedListBox1.Items.Add(j.Item3.Substring(0, 10));
        }

        private void Json(object model)
        {
            File.WriteAllText(AppContext.BaseDirectory+@"\Json.json", JsonConvert.SerializeObject(model));
            Process.Start(AppContext.BaseDirectory + @"\Json.json");
        }

        private NodeModel FillModel(Tuple<string, List<string>, string, List<string>> node)
        {
            NodeModel nodeModel = new NodeModel();
            nodeModel.NodeId = node.Item1;
            nodeModel.Desc = node.Item3;
            foreach (string i in node.Item4)
            {
                if (i != "NULL")
                {
                    string[] item = File.ReadAllLines(mapFilePath + @"\" + mapName + @"\" + i + ".txt");
                    Tuple<string, List<string>, string, List<string>> child = new Tuple<string, List<string>, string, List<string>>(Path.GetFileNameWithoutExtension(i), item[0].Split('|').ToList(), item[1], item[2].Split('|').ToList());
                    NodeModel _nodeModel = FillModel(child);
                    _nodeModel.NodeId = i;
                    _nodeModel.Desc = child.Item3;
                    if (nodeModel.Child is null) nodeModel.Child = new List<NodeModel>();
                    nodeModel.Child.Add(_nodeModel);
                }
            }
            return nodeModel;
        }

        private NodeModelUp FillModelUp(Tuple<string, List<string>, string, List<string>> node)
        {
            NodeModelUp nodeModel = new NodeModelUp();
            nodeModel.NodeId = node.Item1;
            nodeModel.Desc = node.Item3;
            foreach (string i in node.Item2)
            {
                if (i != "NULL")
                {
                    string[] item = File.ReadAllLines(mapFilePath + @"\" + mapName + @"\" + i + ".txt");
                    Tuple<string, List<string>, string, List<string>> parent = new Tuple<string, List<string>, string, List<string>>(Path.GetFileNameWithoutExtension(i), item[0].Split('|').ToList(), item[1], item[2].Split('|').ToList());
                    NodeModelUp _nodeModel = FillModelUp(parent);
                    _nodeModel.NodeId = i;
                    _nodeModel.Desc = parent.Item3;
                    if (nodeModel.Parent is null) nodeModel.Parent = new List<NodeModelUp>();
                    nodeModel.Parent.Add(_nodeModel);
                }
            }
            return nodeModel;
        }

        private void CheckedListBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (checkedListBox1.SelectedIndex != -1) 
            {
                if (!history.Contains(nodes[checkedListBox1.SelectedIndex].Item1))
                {
                    history.Add(nodes[checkedListBox1.SelectedIndex].Item1);
                    if (nodes[checkedListBox1.SelectedIndex].Item3.Length <= 10)historyToolStripMenuItem.DropDownItems.Add(history.Count.ToString() + ":" + nodes[checkedListBox1.SelectedIndex].Item3, null, HistoryClick);
                    else historyToolStripMenuItem.DropDownItems.Add(history.Count.ToString()+":"+nodes[checkedListBox1.SelectedIndex].Item3.Substring(0,10), null, HistoryClick);
                }
                LoadMap(mapName, nodes[checkedListBox1.SelectedIndex]);
            }
        }

        private void HistoryClick(object sender, EventArgs e) => LoadMap(mapName, nodes.Where(a => a.Item1 == history[Convert.ToInt32((sender as ToolStripItem).Text.Split(':')[0]) - 1]).FirstOrDefault());

        private void CheckedListBox1_Click(object sender, EventArgs e)
        {
            if (checkedListBox1.SelectedIndex != -1) textBox1.Text = nodes[checkedListBox1.SelectedIndex].Item3;
        }

        private void 存储ToolStripMenuItem_Click(object sender, EventArgs e)=> Process.Start(mapFilePath + @"\" + mapName);

        private void ToolStripTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar=='\r')
            {
                
            }
        }
    }
}
