namespace ConfigManage
{
    partial class MesOfflineView
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cb_StationNum = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.bt_ChangeMode = new System.Windows.Forms.Button();
            this.cb_OfflineDataStatus = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.rb_OfflineMode = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.dtp_StartDate = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.rb_OnlineMode = new System.Windows.Forms.RadioButton();
            this.bt_QueryOffline = new System.Windows.Forms.Button();
            this.dtp_EndDate = new System.Windows.Forms.DateTimePicker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgv_OfflineData = new System.Windows.Forms.DataGridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cb_QrStatus = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.bt_QueryQR = new System.Windows.Forms.Button();
            this.bt_DeleteQR = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.cb_QrType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tb_QrCode = new System.Windows.Forms.TextBox();
            this.bt_AddQR = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgv_CodeList = new System.Windows.Forms.DataGridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.bt_SetStandard = new System.Windows.Forms.Button();
            this.tb_TestStandardData = new System.Windows.Forms.TextBox();
            this.bt_GetTestStandardData = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_OfflineData)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_CodeList)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(919, 488);
            this.tabControl1.TabIndex = 49;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tableLayoutPanel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(911, 462);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "模式切换";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(905, 456);
            this.tableLayoutPanel1.TabIndex = 55;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cb_StationNum);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.bt_ChangeMode);
            this.panel1.Controls.Add(this.cb_OfflineDataStatus);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.rb_OfflineMode);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.dtp_StartDate);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.rb_OnlineMode);
            this.panel1.Controls.Add(this.bt_QueryOffline);
            this.panel1.Controls.Add(this.dtp_EndDate);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(899, 74);
            this.panel1.TabIndex = 50;
            // 
            // cb_StationNum
            // 
            this.cb_StationNum.FormattingEnabled = true;
            this.cb_StationNum.Location = new System.Drawing.Point(646, 43);
            this.cb_StationNum.Name = "cb_StationNum";
            this.cb_StationNum.Size = new System.Drawing.Size(160, 20);
            this.cb_StationNum.TabIndex = 59;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(584, 47);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 58;
            this.label7.Text = "工作中心";
            // 
            // bt_ChangeMode
            // 
            this.bt_ChangeMode.Location = new System.Drawing.Point(206, 11);
            this.bt_ChangeMode.Name = "bt_ChangeMode";
            this.bt_ChangeMode.Size = new System.Drawing.Size(75, 23);
            this.bt_ChangeMode.TabIndex = 57;
            this.bt_ChangeMode.Text = "应用";
            this.bt_ChangeMode.UseVisualStyleBackColor = true;
            this.bt_ChangeMode.Click += new System.EventHandler(this.bt_ChangeMode_Click);
            // 
            // cb_OfflineDataStatus
            // 
            this.cb_OfflineDataStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_OfflineDataStatus.FormattingEnabled = true;
            this.cb_OfflineDataStatus.Items.AddRange(new object[] {
            "待上传",
            "已上传",
            "用户拒绝上传"});
            this.cb_OfflineDataStatus.Location = new System.Drawing.Point(454, 43);
            this.cb_OfflineDataStatus.Name = "cb_OfflineDataStatus";
            this.cb_OfflineDataStatus.Size = new System.Drawing.Size(121, 20);
            this.cb_OfflineDataStatus.TabIndex = 56;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(368, 47);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 12);
            this.label6.TabIndex = 55;
            this.label6.Text = "离线数据状态";
            // 
            // rb_OfflineMode
            // 
            this.rb_OfflineMode.AutoSize = true;
            this.rb_OfflineMode.Location = new System.Drawing.Point(110, 14);
            this.rb_OfflineMode.Name = "rb_OfflineMode";
            this.rb_OfflineMode.Size = new System.Drawing.Size(71, 16);
            this.rb_OfflineMode.TabIndex = 48;
            this.rb_OfflineMode.TabStop = true;
            this.rb_OfflineMode.Text = "离线模式";
            this.rb_OfflineMode.UseVisualStyleBackColor = true;
            this.rb_OfflineMode.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rb_OfflineMode_MouseDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 52;
            this.label1.Text = "开始时间";
            // 
            // dtp_StartDate
            // 
            this.dtp_StartDate.Location = new System.Drawing.Point(72, 43);
            this.dtp_StartDate.Name = "dtp_StartDate";
            this.dtp_StartDate.Size = new System.Drawing.Size(108, 21);
            this.dtp_StartDate.TabIndex = 53;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(189, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 54;
            this.label2.Text = "结束时间";
            // 
            // rb_OnlineMode
            // 
            this.rb_OnlineMode.AutoSize = true;
            this.rb_OnlineMode.Location = new System.Drawing.Point(10, 14);
            this.rb_OnlineMode.Name = "rb_OnlineMode";
            this.rb_OnlineMode.Size = new System.Drawing.Size(71, 16);
            this.rb_OnlineMode.TabIndex = 47;
            this.rb_OnlineMode.TabStop = true;
            this.rb_OnlineMode.Text = "在线模式";
            this.rb_OnlineMode.UseVisualStyleBackColor = true;
            this.rb_OnlineMode.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rb_OnlineMode_MouseDown);
            // 
            // bt_QueryOffline
            // 
            this.bt_QueryOffline.Location = new System.Drawing.Point(815, 42);
            this.bt_QueryOffline.Name = "bt_QueryOffline";
            this.bt_QueryOffline.Size = new System.Drawing.Size(75, 23);
            this.bt_QueryOffline.TabIndex = 50;
            this.bt_QueryOffline.Text = "查询";
            this.bt_QueryOffline.UseVisualStyleBackColor = true;
            this.bt_QueryOffline.Click += new System.EventHandler(this.bt_QueryOffline_Click);
            // 
            // dtp_EndDate
            // 
            this.dtp_EndDate.Location = new System.Drawing.Point(251, 43);
            this.dtp_EndDate.Name = "dtp_EndDate";
            this.dtp_EndDate.Size = new System.Drawing.Size(108, 21);
            this.dtp_EndDate.TabIndex = 51;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dgv_OfflineData);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 83);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(899, 370);
            this.groupBox1.TabIndex = 51;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "离线数据";
            // 
            // dgv_OfflineData
            // 
            this.dgv_OfflineData.AllowUserToAddRows = false;
            this.dgv_OfflineData.AllowUserToDeleteRows = false;
            this.dgv_OfflineData.AllowUserToResizeColumns = false;
            this.dgv_OfflineData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_OfflineData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_OfflineData.Location = new System.Drawing.Point(3, 17);
            this.dgv_OfflineData.Name = "dgv_OfflineData";
            this.dgv_OfflineData.RowTemplate.Height = 23;
            this.dgv_OfflineData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_OfflineData.Size = new System.Drawing.Size(893, 350);
            this.dgv_OfflineData.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tableLayoutPanel2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(911, 462);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "离线条码管理";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.groupBox2, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(905, 456);
            this.tableLayoutPanel2.TabIndex = 56;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.cb_QrStatus);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.bt_QueryQR);
            this.panel2.Controls.Add(this.bt_DeleteQR);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.cb_QrType);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.tb_QrCode);
            this.panel2.Controls.Add(this.bt_AddQR);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(899, 74);
            this.panel2.TabIndex = 50;
            // 
            // cb_QrStatus
            // 
            this.cb_QrStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_QrStatus.FormattingEnabled = true;
            this.cb_QrStatus.Items.AddRange(new object[] {
            "待申请",
            "已申请"});
            this.cb_QrStatus.Location = new System.Drawing.Point(312, 37);
            this.cb_QrStatus.Name = "cb_QrStatus";
            this.cb_QrStatus.Size = new System.Drawing.Size(121, 20);
            this.cb_QrStatus.TabIndex = 58;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(217, 41);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 12);
            this.label5.TabIndex = 57;
            this.label5.Text = "二维码申请状态";
            // 
            // bt_QueryQR
            // 
            this.bt_QueryQR.Location = new System.Drawing.Point(449, 36);
            this.bt_QueryQR.Name = "bt_QueryQR";
            this.bt_QueryQR.Size = new System.Drawing.Size(75, 23);
            this.bt_QueryQR.TabIndex = 56;
            this.bt_QueryQR.Text = "查询";
            this.bt_QueryQR.UseVisualStyleBackColor = true;
            this.bt_QueryQR.Click += new System.EventHandler(this.bt_QueryQR_Click);
            // 
            // bt_DeleteQR
            // 
            this.bt_DeleteQR.Location = new System.Drawing.Point(531, 36);
            this.bt_DeleteQR.Name = "bt_DeleteQR";
            this.bt_DeleteQR.Size = new System.Drawing.Size(75, 23);
            this.bt_DeleteQR.TabIndex = 55;
            this.bt_DeleteQR.Text = "删除";
            this.bt_DeleteQR.UseVisualStyleBackColor = true;
            this.bt_DeleteQR.Click += new System.EventHandler(this.bt_DeleteQR_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 41);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 54;
            this.label4.Text = "二维码类型";
            // 
            // cb_QrType
            // 
            this.cb_QrType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_QrType.FormattingEnabled = true;
            this.cb_QrType.Items.AddRange(new object[] {
            "模块",
            "模组"});
            this.cb_QrType.Location = new System.Drawing.Point(89, 37);
            this.cb_QrType.Name = "cb_QrType";
            this.cb_QrType.Size = new System.Drawing.Size(108, 20);
            this.cb_QrType.TabIndex = 53;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 52;
            this.label3.Text = "二维码";
            // 
            // tb_QrCode
            // 
            this.tb_QrCode.Location = new System.Drawing.Point(89, 9);
            this.tb_QrCode.Name = "tb_QrCode";
            this.tb_QrCode.Size = new System.Drawing.Size(344, 21);
            this.tb_QrCode.TabIndex = 51;
            // 
            // bt_AddQR
            // 
            this.bt_AddQR.Location = new System.Drawing.Point(448, 7);
            this.bt_AddQR.Name = "bt_AddQR";
            this.bt_AddQR.Size = new System.Drawing.Size(158, 23);
            this.bt_AddQR.TabIndex = 50;
            this.bt_AddQR.Text = "添加";
            this.bt_AddQR.UseVisualStyleBackColor = true;
            this.bt_AddQR.Click += new System.EventHandler(this.bt_AddQR_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dgv_CodeList);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 83);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(899, 370);
            this.groupBox2.TabIndex = 51;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "条码列表";
            // 
            // dgv_CodeList
            // 
            this.dgv_CodeList.AllowUserToAddRows = false;
            this.dgv_CodeList.AllowUserToDeleteRows = false;
            this.dgv_CodeList.AllowUserToResizeColumns = false;
            this.dgv_CodeList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_CodeList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_CodeList.Location = new System.Drawing.Point(3, 17);
            this.dgv_CodeList.MultiSelect = false;
            this.dgv_CodeList.Name = "dgv_CodeList";
            this.dgv_CodeList.RowTemplate.Height = 23;
            this.dgv_CodeList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_CodeList.Size = new System.Drawing.Size(893, 350);
            this.dgv_CodeList.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.bt_SetStandard);
            this.tabPage3.Controls.Add(this.tb_TestStandardData);
            this.tabPage3.Controls.Add(this.bt_GetTestStandardData);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(911, 462);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "获取测试标准";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // bt_SetStandard
            // 
            this.bt_SetStandard.Location = new System.Drawing.Point(182, 15);
            this.bt_SetStandard.Name = "bt_SetStandard";
            this.bt_SetStandard.Size = new System.Drawing.Size(158, 23);
            this.bt_SetStandard.TabIndex = 53;
            this.bt_SetStandard.Text = "设定测试标准";
            this.bt_SetStandard.UseVisualStyleBackColor = true;
            this.bt_SetStandard.Click += new System.EventHandler(this.bt_SetStandard_Click);
            // 
            // tb_TestStandardData
            // 
            this.tb_TestStandardData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_TestStandardData.Location = new System.Drawing.Point(3, 44);
            this.tb_TestStandardData.Multiline = true;
            this.tb_TestStandardData.Name = "tb_TestStandardData";
            this.tb_TestStandardData.Size = new System.Drawing.Size(905, 412);
            this.tb_TestStandardData.TabIndex = 52;
            // 
            // bt_GetTestStandardData
            // 
            this.bt_GetTestStandardData.Location = new System.Drawing.Point(8, 15);
            this.bt_GetTestStandardData.Name = "bt_GetTestStandardData";
            this.bt_GetTestStandardData.Size = new System.Drawing.Size(158, 23);
            this.bt_GetTestStandardData.TabIndex = 51;
            this.bt_GetTestStandardData.Text = "从MES获取测试标准";
            this.bt_GetTestStandardData.UseVisualStyleBackColor = true;
            this.bt_GetTestStandardData.Click += new System.EventHandler(this.bt_GetTestStandardData_Click);
            // 
            // MesOfflineView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(919, 488);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MesOfflineView";
            this.Text = "MesOfflineView";
            this.Load += new System.EventHandler(this.MesOfflineView_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_OfflineData)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_CodeList)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rb_OfflineMode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtp_StartDate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton rb_OnlineMode;
        private System.Windows.Forms.Button bt_QueryOffline;
        private System.Windows.Forms.DateTimePicker dtp_EndDate;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgv_OfflineData;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button bt_AddQR;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgv_CodeList;
        private System.Windows.Forms.Button bt_DeleteQR;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cb_QrType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tb_QrCode;
        private System.Windows.Forms.Button bt_QueryQR;
        private System.Windows.Forms.ComboBox cb_QrStatus;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cb_OfflineDataStatus;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button bt_ChangeMode;
        private System.Windows.Forms.ComboBox cb_StationNum;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TextBox tb_TestStandardData;
        private System.Windows.Forms.Button bt_GetTestStandardData;
        private System.Windows.Forms.Button bt_SetStandard;

    }
}