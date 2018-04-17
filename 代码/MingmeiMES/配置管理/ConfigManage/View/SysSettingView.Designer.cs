namespace ConfigManage
{
    partial class SysSettingView
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnDel = new System.Windows.Forms.Button();
            this.btnDispBuf = new System.Windows.Forms.Button();
            this.btnModifyMod = new System.Windows.Forms.Button();
            this.btnAddMod = new System.Windows.Forms.Button();
            this.textBoxMod = new System.Windows.Forms.TextBox();
            this.listBoxMod = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbxSwitchLine = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnParamCancel = new System.Windows.Forms.Button();
            this.btnParamSave = new System.Windows.Forms.Button();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonCancelSet = new System.Windows.Forms.Button();
            this.buttonCfgApply = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.buttonCancelSet);
            this.panel1.Controls.Add(this.buttonCfgApply);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1050, 591);
            this.panel1.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnDel);
            this.groupBox2.Controls.Add(this.btnDispBuf);
            this.groupBox2.Controls.Add(this.btnModifyMod);
            this.groupBox2.Controls.Add(this.btnAddMod);
            this.groupBox2.Controls.Add(this.textBoxMod);
            this.groupBox2.Controls.Add(this.listBoxMod);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.cbxSwitchLine);
            this.groupBox2.Font = new System.Drawing.Font("宋体", 12F);
            this.groupBox2.Location = new System.Drawing.Point(12, 210);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1026, 369);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "分档线缓存";
            // 
            // btnDel
            // 
            this.btnDel.Font = new System.Drawing.Font("宋体", 13F);
            this.btnDel.Location = new System.Drawing.Point(24, 147);
            this.btnDel.Margin = new System.Windows.Forms.Padding(4);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(221, 63);
            this.btnDel.TabIndex = 16;
            this.btnDel.Text = "移除";
            this.btnDel.UseVisualStyleBackColor = true;
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // btnDispBuf
            // 
            this.btnDispBuf.Font = new System.Drawing.Font("宋体", 13F);
            this.btnDispBuf.Location = new System.Drawing.Point(24, 77);
            this.btnDispBuf.Margin = new System.Windows.Forms.Padding(4);
            this.btnDispBuf.Name = "btnDispBuf";
            this.btnDispBuf.Size = new System.Drawing.Size(221, 63);
            this.btnDispBuf.TabIndex = 17;
            this.btnDispBuf.Text = "刷新";
            this.btnDispBuf.UseVisualStyleBackColor = true;
            this.btnDispBuf.Click += new System.EventHandler(this.btnDispBuf_Click);
            // 
            // btnModifyMod
            // 
            this.btnModifyMod.Location = new System.Drawing.Point(911, 23);
            this.btnModifyMod.Name = "btnModifyMod";
            this.btnModifyMod.Size = new System.Drawing.Size(109, 43);
            this.btnModifyMod.TabIndex = 15;
            this.btnModifyMod.Text = "修改";
            this.btnModifyMod.UseVisualStyleBackColor = true;
            this.btnModifyMod.Visible = false;
            this.btnModifyMod.Click += new System.EventHandler(this.btnModifyMod_Click);
            // 
            // btnAddMod
            // 
            this.btnAddMod.Location = new System.Drawing.Point(799, 24);
            this.btnAddMod.Name = "btnAddMod";
            this.btnAddMod.Size = new System.Drawing.Size(109, 43);
            this.btnAddMod.TabIndex = 15;
            this.btnAddMod.Text = "增加";
            this.btnAddMod.UseVisualStyleBackColor = true;
            this.btnAddMod.Click += new System.EventHandler(this.btnAddMod_Click);
            // 
            // textBoxMod
            // 
            this.textBoxMod.Font = new System.Drawing.Font("宋体", 12F);
            this.textBoxMod.Location = new System.Drawing.Point(289, 31);
            this.textBoxMod.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxMod.Name = "textBoxMod";
            this.textBoxMod.Size = new System.Drawing.Size(490, 35);
            this.textBoxMod.TabIndex = 14;
            // 
            // listBoxMod
            // 
            this.listBoxMod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBoxMod.FormattingEnabled = true;
            this.listBoxMod.ItemHeight = 24;
            this.listBoxMod.Location = new System.Drawing.Point(289, 104);
            this.listBoxMod.Name = "listBoxMod";
            this.listBoxMod.Size = new System.Drawing.Size(490, 244);
            this.listBoxMod.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(285, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(154, 24);
            this.label2.TabIndex = 6;
            this.label2.Text = "模块条码列表";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 24);
            this.label1.TabIndex = 6;
            this.label1.Text = "选择分档线";
            // 
            // cbxSwitchLine
            // 
            this.cbxSwitchLine.FormattingEnabled = true;
            this.cbxSwitchLine.Location = new System.Drawing.Point(155, 34);
            this.cbxSwitchLine.Name = "cbxSwitchLine";
            this.cbxSwitchLine.Size = new System.Drawing.Size(90, 32);
            this.cbxSwitchLine.TabIndex = 5;
            this.cbxSwitchLine.SelectedIndexChanged += new System.EventHandler(this.cbxSwitchLine_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnParamCancel);
            this.groupBox1.Controls.Add(this.btnParamSave);
            this.groupBox1.Controls.Add(this.flowLayoutPanel2);
            this.groupBox1.Font = new System.Drawing.Font("宋体", 12F);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1035, 164);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "系统设置";
            // 
            // btnParamCancel
            // 
            this.btnParamCancel.Location = new System.Drawing.Point(369, 18);
            this.btnParamCancel.Name = "btnParamCancel";
            this.btnParamCancel.Size = new System.Drawing.Size(199, 47);
            this.btnParamCancel.TabIndex = 8;
            this.btnParamCancel.Text = "取消";
            this.btnParamCancel.UseVisualStyleBackColor = true;
            this.btnParamCancel.Click += new System.EventHandler(this.btnParamCancel_Click);
            // 
            // btnParamSave
            // 
            this.btnParamSave.Location = new System.Drawing.Point(125, 18);
            this.btnParamSave.Name = "btnParamSave";
            this.btnParamSave.Size = new System.Drawing.Size(199, 47);
            this.btnParamSave.TabIndex = 8;
            this.btnParamSave.Text = "保存";
            this.btnParamSave.UseVisualStyleBackColor = true;
            this.btnParamSave.Click += new System.EventHandler(this.btnParamSave_Click);
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel2.AutoScroll = true;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(6, 71);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(1023, 87);
            this.flowLayoutPanel2.TabIndex = 7;
            // 
            // buttonCancelSet
            // 
            this.buttonCancelSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancelSet.Font = new System.Drawing.Font("宋体", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonCancelSet.Location = new System.Drawing.Point(544, -202);
            this.buttonCancelSet.Name = "buttonCancelSet";
            this.buttonCancelSet.Size = new System.Drawing.Size(186, 47);
            this.buttonCancelSet.TabIndex = 1;
            this.buttonCancelSet.Text = "取消";
            this.buttonCancelSet.UseVisualStyleBackColor = true;
            this.buttonCancelSet.Click += new System.EventHandler(this.buttonCancelSet_Click);
            // 
            // buttonCfgApply
            // 
            this.buttonCfgApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCfgApply.Font = new System.Drawing.Font("宋体", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonCfgApply.Location = new System.Drawing.Point(330, -201);
            this.buttonCfgApply.Name = "buttonCfgApply";
            this.buttonCfgApply.Size = new System.Drawing.Size(186, 46);
            this.buttonCfgApply.TabIndex = 1;
            this.buttonCfgApply.Text = "应用";
            this.buttonCfgApply.UseVisualStyleBackColor = true;
            this.buttonCfgApply.Click += new System.EventHandler(this.buttonCfgApply_Click);
            // 
            // SysSettingView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1050, 591);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "SysSettingView";
            this.Text = "系统设置";
            this.Load += new System.EventHandler(this.SysSettingView_Load);
            this.panel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonCfgApply;
        private System.Windows.Forms.Button buttonCancelSet;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox listBoxMod;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbxSwitchLine;
        private System.Windows.Forms.Button btnModifyMod;
        private System.Windows.Forms.Button btnAddMod;
        private System.Windows.Forms.TextBox textBoxMod;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnDel;
        private System.Windows.Forms.Button btnDispBuf;
        private System.Windows.Forms.Button btnParamSave;
        private System.Windows.Forms.Button btnParamCancel;
    }
}