using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FilesFinder
{
    public partial class FrmFilesFinder : Form
    {
        string c_InitFilePath = @".\init.txt";

        Node v_Root = null;
        Thread v_TSearchFiles, v_TShowResult;
        ManualResetEvent v_ResEvent = new ManualResetEvent(false);
        Stopwatch v_SW = new Stopwatch();

        String c_StartText = "Начать поиск";
        String c_СontinueText = "Возобновить поиск";
        Regex v_RegEx;

        enum EnumProcessState {Started = 'S', Continued = 'C', Paused = 'P', Stopped = 'N'};
        EnumProcessState v_State = EnumProcessState.Stopped;

        private int v_Found;
        private int v_Count;
        private string v_CurrentDirName;

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

        private void SetState(EnumProcessState p_State)
        {
            v_State = p_State;
            
            switch (v_State)
            {
                case EnumProcessState.Started:

                    var v_Dir = tbStartDir.Text.Trim();
                    var v_RegExStr = tbRegEx.Text.Trim();

                    try
                    {
                        v_RegEx = new Regex(v_RegExStr);
                    }
                    catch 
                    {
                        MessageBox.Show("Регулярное выражение введено с ошибкой!");
                        tbRegEx.Focus();
                        v_State=EnumProcessState.Stopped;
                        break;
                    }
                    v_Root = new Node { FullPath = v_Dir };

                    v_ResEvent.Set();

                    v_TSearchFiles = new Thread(() => BuildTree(v_Root)) { IsBackground = true };
                    v_TSearchFiles.Start();

                    v_TShowResult = new Thread(() => ShowResult(v_Root)) { IsBackground = true };
                    v_TShowResult.Start();
                    
                    v_SW.Reset();
                    v_SW.Start();

                    btnSearch.Enabled = false;
                    btnPause.Enabled = true;
                    btnStop.Enabled = true;
                    break;                    
                case EnumProcessState.Continued:
                    v_ResEvent.Set();
                    v_SW.Start();

                    btnSearch.Enabled = false;
                    btnPause.Enabled = true;
                    btnStop.Enabled = true;

                    break;
                case EnumProcessState.Paused:
                    v_ResEvent.Reset();
                    v_SW.Stop();
                    
                    if (btnSearch.InvokeRequired)
                    {
                        btnSearch.Invoke(new Action(() => { 
                            btnSearch.Text = c_СontinueText;
                            btnSearch.Enabled = true;
                        }));
                        btnPause.Invoke(new Action(() => {
                            btnPause.Enabled = false;
                        }));
                        btnStop.Invoke(new Action(() => {
                            btnStop.Enabled = true;
                        }));
                    }
                    else
                    {
                        btnSearch.Text = c_СontinueText;
                        btnSearch.Enabled = true;
                        btnPause.Enabled = false;
                        btnStop.Enabled = true;
                    }                 

                    break;
                case EnumProcessState.Stopped:                
                    if (btnSearch.InvokeRequired)
                    {
                        btnSearch.Invoke(new Action(() => { 
                            btnSearch.Text = c_StartText; 
                            btnSearch.Enabled = true; 
                        }));
                        btnPause.Invoke(new Action(() => {
                            btnPause.Enabled = false;
                        }));
                        btnStop.Invoke(new Action(() => {
                                btnStop.Enabled = false;
                        }));
                    }
                    else
                    {
                        btnSearch.Text = c_StartText;
                        btnSearch.Enabled = true;
                        btnPause.Enabled = false;
                        btnStop.Enabled = false;
                    }
                    v_ResEvent.Reset();
                    v_TSearchFiles.Abort();
                    v_TShowResult.Abort();

                    break;
            }            
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (v_State == EnumProcessState.Stopped) 
            {
                SetState(EnumProcessState.Started);
            }
            else 
            {
                SetState(EnumProcessState.Continued);
            }
        
        }

        void ShowResult(Node v_Root) 
        {
            tvFiles.Invoke(new Action(() => { tvFiles.Nodes.Clear(); }));            
            int i = 0;
            while (v_TSearchFiles.IsAlive)
            {
                if (v_SW.ElapsedMilliseconds > i * 3000)
                {
                    tvFiles.Invoke(new Action(() => { tvFiles_BuildTree(v_Root); }));
                    i++;
                }
                tbLog.Invoke(new Action(() => {
                    tbLog.Text = "Текущая директория: " + Environment.NewLine + v_CurrentDirName + Environment.NewLine
                + "Просмотрено файлов:  " + v_Count + Environment.NewLine
                + "Найдено файлов:   " + v_Found + Environment.NewLine
                + "Затраченное время:   " + v_SW.Elapsed.TotalSeconds.ToString("0.##") + " секунд";
                }));

                Thread.Sleep(100);

                v_ResEvent.WaitOne();
            }
            tvFiles.Invoke(new Action(() => { tvFiles_BuildTree(v_Root); }));

            SetState(EnumProcessState.Stopped);
                        
        }

        async void tvFiles_BuildTree(Node p_Node) {
            TreeNode v_tvNewNode = tvFiles.Nodes[p_Node.Id.ToString()];
            if (v_tvNewNode == null)
            {
                v_tvNewNode = tvFiles.Nodes.Add(p_Node.Id.ToString(), p_Node.ToString());
                v_tvNewNode.Tag = p_Node;
            }
            await tvFiles_BuildNode(v_tvNewNode, p_Node, false);
        }

        public async Task tvFiles_BuildNode(TreeNode p_tvNode, Node p_Node, bool p_BuildNextLevel)
        {
            if (p_tvNode.FirstNode != null && p_tvNode.FirstNode.Tag == null)
            {
                p_tvNode.Nodes.Clear();
            }

            if (p_tvNode.Nodes.Count == p_Node.Count()) return;            

            foreach (var v_Node in p_Node)
            {
                TreeNode v_tvNewNode = p_tvNode.Nodes[v_Node.Id.ToString()];
                if (v_tvNewNode == null)
                {
                    v_tvNewNode=p_tvNode.Nodes.Add(v_Node.Id.ToString(), v_Node.Name);
                    v_tvNewNode.Tag = v_Node;
                    v_tvNewNode.ImageIndex = v_Node.IsFile ? 1 : 0;
                    v_tvNewNode.SelectedImageIndex = v_Node.IsFile ? 1 : 0;
                    if (!v_Node.IsFile) {
                        v_tvNewNode.Nodes.Add("Loading...");
                    }
                }
                if (!v_Node.IsFile && (p_BuildNextLevel || v_tvNewNode.IsExpanded))
                {
                    await tvFiles_BuildNode(v_tvNewNode, v_Node, false);
                }
            }            
        }
       
        private async void tvFiles_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            await tvFiles_BuildNode(e.Node, e.Node.Tag as Node, false);
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            SetState(EnumProcessState.Paused);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            SetState(EnumProcessState.Stopped);                    
        }

        private void BuildTree(Node root)
        {
            v_Found=0;
            v_Count=0;
            
            var v_NodeStack = new Stack<Node>();
            v_NodeStack.Push(root);

            while (v_NodeStack.Count > 0)
            {
                var v_Node = v_NodeStack.Pop();
                try
                {
                    v_CurrentDirName = v_Node.FullPath;

                    foreach (var dir in Directory.GetDirectories(v_Node.FullPath))
                    {
                        var v_NewNode = new Node { FullPath = dir };
                        v_Node.Add(v_NewNode);
                        v_NodeStack.Push(v_NewNode);
                    }

                    v_Count += Directory.GetFiles(v_Node.FullPath).Length;

                    var v_Files = Directory.GetFiles(v_Node.FullPath).Where(p_Path => v_RegEx.IsMatch(Path.GetFileName(p_Path)));
                    
                    foreach (var v_File in v_Files)
                    {
                        var v_NewNode = new Node { FullPath = v_File, IsFile = true };
                        v_Node.Add(v_NewNode);
                        v_Found++;                        
                    }
                }
                catch (UnauthorizedAccessException)
                {
                }
                v_ResEvent.WaitOne();
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
        public int Count() 
        { 
            return Nodes.Where(n => n.HasFile).Count();
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
