using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        private void btnSearch_Click(object sender, EventArgs e)
        {

        }
    }
}
