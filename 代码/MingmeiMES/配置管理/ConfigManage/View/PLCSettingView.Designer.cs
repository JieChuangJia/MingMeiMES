namespace ConfigManage
{
    partial class PLCSettingView
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.dgv_DevList = new System.Windows.Forms.DataGridView();
            this.col_DeviceID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_DeviceName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.superTextEdit1 = new SuperTextEdit.SuperTextEdit();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.刷新数据ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_DevList)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.groupBox3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer2.Size = new System.Drawing.Size(907, 437);
            this.splitContainer2.SplitterDistance = 191;
            this.splitContainer2.TabIndex = 4;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.dgv_DevList);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(191, 437);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "设备列表";
            // 
            // dgv_DevList
            // 
            this.dgv_DevList.AllowUserToAddRows = false;
            this.dgv_DevList.AllowUserToDeleteRows = false;
            this.dgv_DevList.AllowUserToResizeRows = false;
            this.dgv_DevList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv_DevList.BackgroundColor = System.Drawing.SystemColors.ScrollBar;
            this.dgv_DevList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_DevList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_DeviceID,
            this.col_DeviceName});
            this.dgv_DevList.ContextMenuStrip = this.contextMenuStrip1;
            this.dgv_DevList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_DevList.Location = new System.Drawing.Point(3, 17);
            this.dgv_DevList.MultiSelect = false;
            this.dgv_DevList.Name = "dgv_DevList";
            this.dgv_DevList.ReadOnly = true;
            this.dgv_DevList.RowHeadersVisible = false;
            this.dgv_DevList.RowTemplate.Height = 23;
            this.dgv_DevList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_DevList.Size = new System.Drawing.Size(185, 417);
            this.dgv_DevList.TabIndex = 0;
            this.dgv_DevList.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_DevList_CellClick);
            this.dgv_DevList.SelectionChanged += new System.EventHandler(this.dgv_DevList_SelectionChanged);
            // 
            // col_DeviceID
            // 
            this.col_DeviceID.HeaderText = "设备编号";
            this.col_DeviceID.Name = "col_DeviceID";
            this.col_DeviceID.ReadOnly = true;
            // 
            // col_DeviceName
            // 
            this.col_DeviceName.HeaderText = "设备名称";
            this.col_DeviceName.Name = "col_DeviceName";
            this.col_DeviceName.ReadOnly = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.superTextEdit1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(712, 437);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "参数列表";
            // 
            // superTextEdit1
            // 
            this.superTextEdit1.AutoScroll = true;
            this.superTextEdit1.AutoScrollMinSize = new System.Drawing.Size(706, 434);
            this.superTextEdit1.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.superTextEdit1.ColumnNum = 3;
            this.superTextEdit1.DataSource = "";
            this.superTextEdit1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.superTextEdit1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.superTextEdit1.Location = new System.Drawing.Point(3, 17);
            this.superTextEdit1.Name = "superTextEdit1";
            this.superTextEdit1.Size = new System.Drawing.Size(706, 417);
            this.superTextEdit1.TabIndex = 0;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.刷新数据ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(125, 26);
            // 
            // 刷新数据ToolStripMenuItem
            // 
            this.刷新数据ToolStripMenuItem.Name = "刷新数据ToolStripMenuItem";
            this.刷新数据ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.刷新数据ToolStripMenuItem.Text = "刷新数据";
            this.刷新数据ToolStripMenuItem.Click += new System.EventHandler(this.刷新数据ToolStripMenuItem_Click);
            // 
            // PLCSettingView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(907, 437);
            this.Controls.Add(this.splitContainer2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PLCSettingView";
            this.Text = "参数设置";
            this.Load += new System.EventHandler(this.PLCSettingView_Load);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_DevList)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgv_DevList;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_DeviceID;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_DeviceName;
        private SuperTextEdit.SuperTextEdit superTextEdit1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 刷新数据ToolStripMenuItem;
    }
}