using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using FastTreeNS;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace FilesFinder
{
    public partial class FrmFilesFinder : Form
    {
        string c_InitFilePath = @".\init.txt";

        public FrmFilesFinder()
        {
            InitializeComponent();
        }

        private void btnOpenFileBrowser_Click(object sender, EventArgs e)
        {
            dlgBrowseStartDir.SelectedPath = tbStartDir.Text.Trim();
            DialogResult result = dlgBrowseStartDir.ShowDialog();
            if (result == DialogResult.OK)
            {
                tbStartDir.Text = dlgBrowseStartDir.SelectedPath.Trim();
            }
        }

        private void FrmFilesFinder_FormClosing(object sender, FormClosingEventArgs e)
        {
            FileInfo v_FileInfo = new FileInfo(c_InitFilePath);
            if (!v_FileInfo.Exists)
            {
                v_FileInfo.Create().Close();
            }
            File.WriteAllLines(c_InitFilePath, new[] { tbStartDir.Text, tbRegEx.Text });
        }

        private void FrmFilesFinder_Load(object sender, EventArgs e)
        {
            FileInfo v_FileInfo = new FileInfo(c_InitFilePath);
            if (v_FileInfo.Exists)
            {
                String[] v_Lines = File.ReadAllLines(c_InitFilePath);
                if (v_Lines.Length >= 2)
                {
                    tbStartDir.Text = v_Lines[0];
                    tbRegEx.Text = v_Lines[1];
                }
            }

        }

        Node v_Root = null;
        bool v_Started = false;
        Thread thread, thread2, thread3;
        String c_StartText = "Начать поиск";
        String c_StopText = "Остановить поиск";
        Stopwatch v_SW;

        private int v_Found;
        private int v_Count;
        private string v_CurrentDirName;

        private void SetStarted(bool p_Started) 
        {
            v_Started = p_Started;

            if (btnSearch.InvokeRequired)
            {
                btnSearch.Invoke(new Action(() => { btnSearch.Text = p_Started ? c_StopText : c_StartText; }));
            }
            else
            {
                btnSearch.Text = p_Started ? c_StopText : c_StartText;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (!v_Started) {
                SetStarted(true);
            //Regex reg = new Regex(@"^FA[0-9]{4}\.xml$");
            //var files = Directory.GetFiles(yourPath, "*.xml").Where(path => reg.IsMatch(path));

            //InitializeComponent();

            var dir = tbStartDir.Text.Trim();// @"с:";
            var fileMask = tbRegEx.Text.Trim();// "*.*";

            tvFiles.Nodes.Clear();
                //корневая директория
                v_Root = new Node { FullPath = dir };
                v_SW = new Stopwatch();

                //поток
                thread = new Thread(() => BuildTree(v_Root, fileMask)) { IsBackground = true };
                thread.Start();
                v_SW.Start();

                //thread2 = new Thread(() => ShowTree(v_Root)) { IsBackground = true };
                //thread2.Start();

                thread3 = new Thread(() => ShowResults(v_Root)) { IsBackground = true };
                thread3.Start();
                //дерево
                //var ft = new FastTree() { 
                //    Parent = this, Dock = DockStyle.Fill, ShowIcons = true, ShowRootNode = true };
                //ft.NodeIconNeeded += ft_NodeIconNeeded;

            //    //обновляем дерево, пока работает поток
            //    Application.Idle += delegate
            //{
            //    if (thread.IsAlive)
            //    {
            //        //ft.Build(root);
            //        Thread.Sleep(300);
            //        tv_BuildTree(v_Root);
            //        //Text = "Found files: " + found;
            //        //.ElapsedMilliseconds;

            //        tbLog.Text = "Текущая директория: " + Environment.NewLine + v_CurrentDirName +Environment.NewLine
            //        + "Просмотрено файлов:  " + v_Count + Environment.NewLine
            //        + "Найдено файлов:   " + v_Found + Environment.NewLine
            //        + "Затраченное время:   " + v_SW.Elapsed.TotalSeconds.ToString() + " секунд";

            //    } else
            //    {
            //        SetStarted(false);
            //        v_SW.Reset();
            //    }
            //};
        } else {
                SetStarted(false);
                v_SW.Reset();
                thread.Abort();
               // thread2.Abort();
                thread3.Abort();
            }
        }

        void ShowTree(Node v_Root)
        {
            while (thread.IsAlive)
            {
                //ft.Build(root);
                Thread.Sleep(1000);
                tvFiles.Invoke(new Action(() => { tv_BuildTree(v_Root); }));

                //tv_BuildTree(v_Root);
                //Text = "Found files: " + found;
                //.ElapsedMilliseconds;
                //tbLog.Invoke(new Action(() => {
                //    tbLog.Text = "Текущая директория: " + Environment.NewLine + v_CurrentDirName + Environment.NewLine
                //+ "Просмотрено файлов:  " + v_Count + Environment.NewLine
                //+ "Найдено файлов:   " + v_Found + Environment.NewLine
                //+ "Затраченное время:   " + v_SW.Elapsed.TotalSeconds.ToString() + " секунд";
                //}));
            }

            //SetStarted(false);
           // v_SW.Reset();

        }
        void ShowResults(Node v_Root) {
            tvFiles.Invoke(new Action(() => { tv_BuildTree(v_Root); }));

            while (thread.IsAlive)
            {
                //ft.Build(root);
                Thread.Sleep(1000);
                //tvFiles.Invoke(new Action(() => { tv_BuildTree(v_Root); }));

                //tv_BuildTree(v_Root);
                //Text = "Found files: " + found;
                //.ElapsedMilliseconds;
                tbLog.Invoke(new Action(() => {
                    tbLog.Text = "Текущая директория: " + Environment.NewLine + v_CurrentDirName + Environment.NewLine
                + "Просмотрено файлов:  " + v_Count + Environment.NewLine
                + "Найдено файлов:   " + v_Found + Environment.NewLine
                + "Затраченное время:   " + v_SW.Elapsed.TotalSeconds.ToString() + " секунд";
                }));
            }
            
                SetStarted(false);
                v_SW.Reset();
            
        }

        void tv_BuildTree(Node p_Node) {
            TreeNode v_tvNewNode = tvFiles.Nodes[p_Node.Id.ToString()] ??
                tvFiles.Nodes.Add(p_Node.Id.ToString(), p_Node.ToString());
           // TreeNode v_tvNewNode = tvFiles.TopNode ??
             //   tvFiles.Nodes.Add(p_Node.Id.ToString(), p_Node.ToString());
            v_tvNewNode.Tag = p_Node;
            v_tvNewNode.Expand();
            tv_BuildNode(v_tvNewNode, p_Node, true);
        }

        //void tv_BuildNode(TreeNode p_tvNode, Guid p_NodeID, bool p_BuildNextLevel)
        //{
        //    v_Root
        //}
        void tv_BuildNode(TreeNode p_tvNode, Node p_Node, bool p_BuildNextLevel) 
        {
            foreach (var v_Node in p_Node)
            {
                TreeNode v_tvNewNode = p_tvNode.Nodes[v_Node.Id.ToString()];
                if (v_tvNewNode == null)
                {
                    v_tvNewNode=p_tvNode.Nodes.Add(v_Node.Id.ToString(), v_Node.Name);
                    v_tvNewNode.Tag = v_Node;
                    v_tvNewNode.ImageIndex = v_Node.IsFile ? 1 : 0;                    
                }
                if (p_BuildNextLevel || v_tvNewNode.IsExpanded) tv_BuildNode(v_tvNewNode, v_Node, false);
            }
        }

        private void tvFiles_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            tv_BuildNode(e.Node, e.Node.Tag as Node, true);
        }
        private void tvFiles_AfterExpand(object sender, TreeViewEventArgs e)
        {
            
        }

        void NodeIconNeeded(object sender, ImageNodeEventArgs e)
        {
            //if (!(e.Node as Node).IsFile)
            //    e.Result = Properties.Resources.folder;
        }
         
        

        private void BuildTree(Node root, string fileMask)
        {
            v_Found=0;
            v_Count=0;
            
            var toProcess = new Stack<Node>();
            toProcess.Push(root);

            while (toProcess.Count > 0)
            {
                var node = toProcess.Pop();
                try
                {
                    v_Count += Directory.GetFiles(node.FullPath).Length;
                    v_CurrentDirName = node.FullPath;

                    foreach (var dir in Directory.GetDirectories(node.FullPath))
                    {
                        var n = new Node { FullPath = dir };
                        node.Add(n);
                        toProcess.Push(n);
                    }

                    foreach (var file in Directory.GetFiles(node.FullPath, fileMask))
                    {
                        var n = new Node { FullPath = file, IsFile = true };
                        node.Add(n);
                        v_Found++;
                    }
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
        }
    }

    public class Node : IEnumerable<Node>
    {
        private List<Node> nodes;
        
        public bool IsFile { get; set; }
        
        public string FullPath { get; set; }
        
        public string Name { get { return Path.GetFileName(FullPath); } }

        public Guid Id { get; set; }

        public bool HasFile
        {
            get
            {
                return IsFile || Nodes.Any(n => n.HasFile);
            }
        }

        public Node()
        {
            Id = Guid.NewGuid();
            nodes = new List<Node>();
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Name) ? FullPath : Name;
        }

        public void Add(Node node)
        {
            nodes.Add(node);
        }

        IEnumerable<Node> Nodes
        {
            get
            {
                for (int i = 0; i < nodes.Count; i++)
                    yield return nodes[i];
            }
        }

        public IEnumerator<Node> GetEnumerator()
        {
            return Nodes.Where(n => n.HasFile).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    
    }
}
