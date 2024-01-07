﻿namespace FilesFinder
{
    partial class FrmFilesFinder
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmFilesFinder));
            this.btnOpenFileBrowser = new System.Windows.Forms.Button();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.lbRegEx = new System.Windows.Forms.Label();
            this.lbStartDir = new System.Windows.Forms.Label();
            this.tvFiles = new System.Windows.Forms.TreeView();
            this.tbRegEx = new System.Windows.Forms.TextBox();
            this.tbStartDir = new System.Windows.Forms.TextBox();
            this.dlgBrowseStartDir = new System.Windows.Forms.FolderBrowserDialog();
            this.imgListFiles = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // btnOpenFileBrowser
            // 
            this.btnOpenFileBrowser.Location = new System.Drawing.Point(307, 17);
            this.btnOpenFileBrowser.Name = "btnOpenFileBrowser";
            this.btnOpenFileBrowser.Size = new System.Drawing.Size(27, 20);
            this.btnOpenFileBrowser.TabIndex = 15;
            this.btnOpenFileBrowser.Text = "...";
            this.btnOpenFileBrowser.UseVisualStyleBackColor = true;
            this.btnOpenFileBrowser.Click += new System.EventHandler(this.btnOpenFileBrowser_Click);
            // 
            // tbLog
            // 
            this.tbLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tbLog.Location = new System.Drawing.Point(14, 124);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.Size = new System.Drawing.Size(320, 238);
            this.tbLog.TabIndex = 14;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(15, 82);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(319, 23);
            this.btnSearch.TabIndex = 13;
            this.btnSearch.Text = "Начать поиск";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // lbRegEx
            // 
            this.lbRegEx.AutoSize = true;
            this.lbRegEx.Location = new System.Drawing.Point(12, 48);
            this.lbRegEx.Name = "lbRegEx";
            this.lbRegEx.Size = new System.Drawing.Size(116, 13);
            this.lbRegEx.TabIndex = 12;
            this.lbRegEx.Text = "Шаблон имени файла";
            // 
            // lbStartDir
            // 
            this.lbStartDir.AutoSize = true;
            this.lbStartDir.Location = new System.Drawing.Point(12, 20);
            this.lbStartDir.Name = "lbStartDir";
            this.lbStartDir.Size = new System.Drawing.Size(122, 13);
            this.lbStartDir.TabIndex = 11;
            this.lbStartDir.Text = "Стартовая директория";
            // 
            // tvFiles
            // 
            this.tvFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvFiles.ImageIndex = 0;
            this.tvFiles.ImageList = this.imgListFiles;
            this.tvFiles.Location = new System.Drawing.Point(350, 17);
            this.tvFiles.Name = "tvFiles";
            this.tvFiles.SelectedImageIndex = 0;
            this.tvFiles.Size = new System.Drawing.Size(381, 345);
            this.tvFiles.TabIndex = 10;
            this.tvFiles.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvFiles_BeforeExpand);
            this.tvFiles.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.tvFiles_AfterExpand);
            // 
            // tbRegEx
            // 
            this.tbRegEx.Location = new System.Drawing.Point(143, 45);
            this.tbRegEx.Name = "tbRegEx";
            this.tbRegEx.Size = new System.Drawing.Size(192, 20);
            this.tbRegEx.TabIndex = 9;
            this.tbRegEx.Text = "*.*";
            // 
            // tbStartDir
            // 
            this.tbStartDir.Location = new System.Drawing.Point(143, 17);
            this.tbStartDir.Name = "tbStartDir";
            this.tbStartDir.Size = new System.Drawing.Size(163, 20);
            this.tbStartDir.TabIndex = 8;
            // 
            // imgListFiles
            // 
            this.imgListFiles.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgListFiles.ImageStream")));
            this.imgListFiles.TransparentColor = System.Drawing.Color.Transparent;
            this.imgListFiles.Images.SetKeyName(0, "imgFolder.png");
            this.imgListFiles.Images.SetKeyName(1, "imgFile.png");
            // 
            // FrmFilesFinder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(743, 374);
            this.Controls.Add(this.btnOpenFileBrowser);
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.lbRegEx);
            this.Controls.Add(this.lbStartDir);
            this.Controls.Add(this.tvFiles);
            this.Controls.Add(this.tbRegEx);
            this.Controls.Add(this.tbStartDir);
            this.Name = "FrmFilesFinder";
            this.Text = "Поиск файлов";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmFilesFinder_FormClosing);
            this.Load += new System.EventHandler(this.FrmFilesFinder_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOpenFileBrowser;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label lbRegEx;
        private System.Windows.Forms.Label lbStartDir;
        private System.Windows.Forms.TreeView tvFiles;
        private System.Windows.Forms.TextBox tbRegEx;
        private System.Windows.Forms.TextBox tbStartDir;
        private System.Windows.Forms.FolderBrowserDialog dlgBrowseStartDir;
        private System.Windows.Forms.ImageList imgListFiles;
    }
}

