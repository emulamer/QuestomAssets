namespace Assplorer
{
    partial class ExploreTree
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExploreTree));
            this.tvExplorer = new System.Windows.Forms.TreeView();
            this.imgTreeIcons = new System.Windows.Forms.ImageList(this.components);
            this.txtFind = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnFind = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvExplorer
            // 
            this.tvExplorer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvExplorer.ImageIndex = 0;
            this.tvExplorer.ImageList = this.imgTreeIcons;
            this.tvExplorer.Location = new System.Drawing.Point(0, 0);
            this.tvExplorer.Name = "tvExplorer";
            this.tvExplorer.SelectedImageIndex = 0;
            this.tvExplorer.Size = new System.Drawing.Size(450, 339);
            this.tvExplorer.TabIndex = 1;
            this.tvExplorer.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.TvExplorer_BeforeExpand);
            this.tvExplorer.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TvExplorer_AfterSelect);
            this.tvExplorer.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TvExplorer_NodeMouseClick);
            // 
            // imgTreeIcons
            // 
            this.imgTreeIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgTreeIcons.ImageStream")));
            this.imgTreeIcons.TransparentColor = System.Drawing.Color.White;
            this.imgTreeIcons.Images.SetKeyName(0, "blank.png");
            this.imgTreeIcons.Images.SetKeyName(1, "Link.png");
            // 
            // txtFind
            // 
            this.txtFind.Location = new System.Drawing.Point(96, 8);
            this.txtFind.Name = "txtFind";
            this.txtFind.Size = new System.Drawing.Size(187, 26);
            this.txtFind.TabIndex = 3;
            this.txtFind.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtFind_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Search for:";
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(289, 3);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(75, 39);
            this.btnFind.TabIndex = 5;
            this.btnFind.Text = "Find";
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.BtnFind_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.btnFind);
            this.splitContainer1.Panel1.Controls.Add(this.txtFind);
            this.splitContainer1.Panel1MinSize = 28;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tvExplorer);
            this.splitContainer1.Size = new System.Drawing.Size(450, 371);
            this.splitContainer1.SplitterDistance = 28;
            this.splitContainer1.TabIndex = 6;
            // 
            // ExploreTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.splitContainer1);
            this.Name = "ExploreTree";
            this.Size = new System.Drawing.Size(450, 371);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TreeView tvExplorer;
        private System.Windows.Forms.ImageList imgTreeIcons;
        private System.Windows.Forms.TextBox txtFind;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}
