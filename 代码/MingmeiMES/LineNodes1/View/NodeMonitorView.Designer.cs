namespace LineNodes
{
    partial class NodeMonitorView
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.timerNodeStatus = new System.Windows.Forms.Timer(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tableLayoutMonitor = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPicSample = new System.Windows.Forms.FlowLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label10 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label11 = new System.Windows.Forms.Label();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.label13 = new System.Windows.Forms.Label();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.label14 = new System.Windows.Forms.Label();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.label15 = new System.Windows.Forms.Label();
            this.labelCommInfo = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.userControlLine1 = new LineNodes.View.UserControlLine();
            this.userControlLine2 = new LineNodes.View.UserControlLine();
            this.userControlLine3 = new LineNodes.View.UserControlLine();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.checkBoxAutorefresh = new System.Windows.Forms.CheckBox();
            this.groupBoxCtlSim = new System.Windows.Forms.GroupBox();
            this.comboBoxAirlossRe = new System.Windows.Forms.ComboBox();
            this.comboBoxBarcodeGun = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.textBoxBarcode = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.comboBoxRfid = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.buttonDB2Reset = new System.Windows.Forms.Button();
            this.comboBoxDB2 = new System.Windows.Forms.ComboBox();
            this.textBoxRfidVal = new System.Windows.Forms.TextBox();
            this.textBoxDB2ItemVal = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.buttonRfidSimWrite = new System.Windows.Forms.Button();
            this.buttonDB2SimSet = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxDevList = new System.Windows.Forms.ComboBox();
            this.buttonClearDevCmd = new System.Windows.Forms.Button();
            this.buttonRefreshDevStatus = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridViewDevDB1 = new System.Windows.Forms.DataGridView();
            this.dataGridViewDevDB2 = new System.Windows.Forms.DataGridView();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.richTextBoxTaskInfo = new System.Windows.Forms.RichTextBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tableLayoutMonitor.SuspendLayout();
            this.flowLayoutPicSample.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBoxCtlSim.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDevDB1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDevDB2)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerNodeStatus
            // 
            this.timerNodeStatus.Interval = 200;
            this.timerNodeStatus.Tick += new System.EventHandler(this.timerNodeStatus_Tick);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.ItemSize = new System.Drawing.Size(84, 26);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(924, 477);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.tabPage1.Controls.Add(this.tableLayoutMonitor);
            this.tabPage1.Location = new System.Drawing.Point(4, 30);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(0);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(916, 443);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "工位状态监控";
            // 
            // tableLayoutMonitor
            // 
            this.tableLayoutMonitor.BackColor = System.Drawing.Color.Gray;
            this.tableLayoutMonitor.ColumnCount = 1;
            this.tableLayoutMonitor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutMonitor.Controls.Add(this.flowLayoutPicSample, 0, 0);
            this.tableLayoutMonitor.Controls.Add(this.tableLayoutPanel1, 0, 1);
            this.tableLayoutMonitor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutMonitor.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutMonitor.Name = "tableLayoutMonitor";
            this.tableLayoutMonitor.RowCount = 2;
            this.tableLayoutMonitor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutMonitor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutMonitor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 13F));
            this.tableLayoutMonitor.Size = new System.Drawing.Size(916, 443);
            this.tableLayoutMonitor.TabIndex = 5;
            // 
            // flowLayoutPicSample
            // 
            this.flowLayoutPicSample.BackColor = System.Drawing.SystemColors.Control;
            this.flowLayoutPicSample.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowLayoutPicSample.Controls.Add(this.pictureBox1);
            this.flowLayoutPicSample.Controls.Add(this.label10);
            this.flowLayoutPicSample.Controls.Add(this.pictureBox2);
            this.flowLayoutPicSample.Controls.Add(this.label11);
            this.flowLayoutPicSample.Controls.Add(this.pictureBox3);
            this.flowLayoutPicSample.Controls.Add(this.label13);
            this.flowLayoutPicSample.Controls.Add(this.pictureBox4);
            this.flowLayoutPicSample.Controls.Add(this.label14);
            this.flowLayoutPicSample.Controls.Add(this.pictureBox5);
            this.flowLayoutPicSample.Controls.Add(this.label15);
            this.flowLayoutPicSample.Controls.Add(this.labelCommInfo);
            this.flowLayoutPicSample.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPicSample.Font = new System.Drawing.Font("方正姚体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.flowLayoutPicSample.Location = new System.Drawing.Point(3, 2);
            this.flowLayoutPicSample.Margin = new System.Windows.Forms.Padding(3, 2, 3, 1);
            this.flowLayoutPicSample.Name = "flowLayoutPicSample";
            this.flowLayoutPicSample.Size = new System.Drawing.Size(910, 25);
            this.flowLayoutPicSample.TabIndex = 4;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Red;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(29, 13);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(38, 2);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(76, 18);
            this.label10.TabIndex = 1;
            this.label10.Text = "设备故障";
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Green;
            this.pictureBox2.Location = new System.Drawing.Point(147, 3);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(30, 3, 3, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(29, 13);
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            // 
            // label11
            // 
            this.label11.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(182, 2);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(76, 18);
            this.label11.TabIndex = 1;
            this.label11.Text = "设备空闲";
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.Color.Yellow;
            this.pictureBox3.Location = new System.Drawing.Point(291, 3);
            this.pictureBox3.Margin = new System.Windows.Forms.Padding(30, 3, 3, 3);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(29, 13);
            this.pictureBox3.TabIndex = 0;
            this.pictureBox3.TabStop = false;
            // 
            // label13
            // 
            this.label13.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(326, 2);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(93, 18);
            this.label13.TabIndex = 1;
            this.label13.Text = "设备使用中";
            // 
            // pictureBox4
            // 
            this.pictureBox4.BackColor = System.Drawing.Color.LightSeaGreen;
            this.pictureBox4.Location = new System.Drawing.Point(452, 3);
            this.pictureBox4.Margin = new System.Windows.Forms.Padding(30, 3, 3, 3);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(29, 13);
            this.pictureBox4.TabIndex = 0;
            this.pictureBox4.TabStop = false;
            this.pictureBox4.Visible = false;
            // 
            // label14
            // 
            this.label14.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(487, 2);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(76, 18);
            this.label14.TabIndex = 1;
            this.label14.Text = "工位有板";
            this.label14.Visible = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.BackColor = System.Drawing.Color.PaleVioletRed;
            this.pictureBox5.Location = new System.Drawing.Point(596, 3);
            this.pictureBox5.Margin = new System.Windows.Forms.Padding(30, 3, 3, 3);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(29, 13);
            this.pictureBox5.TabIndex = 0;
            this.pictureBox5.TabStop = false;
            // 
            // label15
            // 
            this.label15.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(631, 2);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(76, 18);
            this.label15.TabIndex = 1;
            this.label15.Text = "无法识别";
            // 
            // labelCommInfo
            // 
            this.labelCommInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCommInfo.BackColor = System.Drawing.SystemColors.HotTrack;
            this.labelCommInfo.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelCommInfo.ForeColor = System.Drawing.Color.Yellow;
            this.labelCommInfo.Location = new System.Drawing.Point(713, 0);
            this.labelCommInfo.Name = "labelCommInfo";
            this.labelCommInfo.Size = new System.Drawing.Size(139, 22);
            this.labelCommInfo.TabIndex = 9;
            this.labelCommInfo.Text = "通信周期：";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.userControlLine1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.userControlLine2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.userControlLine3, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 30);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(912, 411);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // userControlLine1
            // 
            this.userControlLine1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userControlLine1.LineName = "产线名称";
            this.userControlLine1.Location = new System.Drawing.Point(1, 1);
            this.userControlLine1.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.userControlLine1.Name = "userControlLine1";
            this.userControlLine1.Size = new System.Drawing.Size(910, 135);
            this.userControlLine1.TabIndex = 0;
            // 
            // userControlLine2
            // 
            this.userControlLine2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userControlLine2.LineName = "产线名称";
            this.userControlLine2.Location = new System.Drawing.Point(1, 138);
            this.userControlLine2.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.userControlLine2.Name = "userControlLine2";
            this.userControlLine2.Size = new System.Drawing.Size(910, 135);
            this.userControlLine2.TabIndex = 1;
            // 
            // userControlLine3
            // 
            this.userControlLine3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userControlLine3.LineName = "产线名称";
            this.userControlLine3.Location = new System.Drawing.Point(1, 275);
            this.userControlLine3.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.userControlLine3.Name = "userControlLine3";
            this.userControlLine3.Size = new System.Drawing.Size(910, 135);
            this.userControlLine3.TabIndex = 2;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.tabPage2.Controls.Add(this.splitContainer1);
            this.tabPage2.Location = new System.Drawing.Point(4, 30);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPage2.Size = new System.Drawing.Size(916, 443);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "数据通信监控";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.splitContainer1.Panel1.Controls.Add(this.checkBoxAutorefresh);
            this.splitContainer1.Panel1.Controls.Add(this.groupBoxCtlSim);
            this.splitContainer1.Panel1.Controls.Add(this.label5);
            this.splitContainer1.Panel1.Controls.Add(this.comboBoxDevList);
            this.splitContainer1.Panel1.Controls.Add(this.buttonClearDevCmd);
            this.splitContainer1.Panel1.Controls.Add(this.buttonRefreshDevStatus);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel2);
            this.splitContainer1.Size = new System.Drawing.Size(910, 437);
            this.splitContainer1.SplitterDistance = 222;
            this.splitContainer1.TabIndex = 0;
            // 
            // checkBoxAutorefresh
            // 
            this.checkBoxAutorefresh.AutoSize = true;
            this.checkBoxAutorefresh.Location = new System.Drawing.Point(9, 53);
            this.checkBoxAutorefresh.Name = "checkBoxAutorefresh";
            this.checkBoxAutorefresh.Size = new System.Drawing.Size(72, 16);
            this.checkBoxAutorefresh.TabIndex = 9;
            this.checkBoxAutorefresh.Text = "自动刷新";
            this.checkBoxAutorefresh.UseVisualStyleBackColor = true;
            this.checkBoxAutorefresh.CheckedChanged += new System.EventHandler(this.checkBoxAutorefresh_CheckedChanged);
            // 
            // groupBoxCtlSim
            // 
            this.groupBoxCtlSim.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxCtlSim.Controls.Add(this.comboBoxAirlossRe);
            this.groupBoxCtlSim.Controls.Add(this.comboBoxBarcodeGun);
            this.groupBoxCtlSim.Controls.Add(this.label16);
            this.groupBoxCtlSim.Controls.Add(this.textBoxBarcode);
            this.groupBoxCtlSim.Controls.Add(this.label17);
            this.groupBoxCtlSim.Controls.Add(this.comboBoxRfid);
            this.groupBoxCtlSim.Controls.Add(this.label18);
            this.groupBoxCtlSim.Controls.Add(this.label9);
            this.groupBoxCtlSim.Controls.Add(this.buttonDB2Reset);
            this.groupBoxCtlSim.Controls.Add(this.comboBoxDB2);
            this.groupBoxCtlSim.Controls.Add(this.textBoxRfidVal);
            this.groupBoxCtlSim.Controls.Add(this.textBoxDB2ItemVal);
            this.groupBoxCtlSim.Controls.Add(this.label23);
            this.groupBoxCtlSim.Controls.Add(this.buttonRfidSimWrite);
            this.groupBoxCtlSim.Controls.Add(this.buttonDB2SimSet);
            this.groupBoxCtlSim.Controls.Add(this.label7);
            this.groupBoxCtlSim.Controls.Add(this.label25);
            this.groupBoxCtlSim.Location = new System.Drawing.Point(7, 109);
            this.groupBoxCtlSim.Name = "groupBoxCtlSim";
            this.groupBoxCtlSim.Size = new System.Drawing.Size(205, 287);
            this.groupBoxCtlSim.TabIndex = 8;
            this.groupBoxCtlSim.TabStop = false;
            this.groupBoxCtlSim.Text = "仿真模拟";
            // 
            // comboBoxAirlossRe
            // 
            this.comboBoxAirlossRe.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxAirlossRe.FormattingEnabled = true;
            this.comboBoxAirlossRe.Items.AddRange(new object[] {
            "OK",
            "NG"});
            this.comboBoxAirlossRe.Location = new System.Drawing.Point(90, 107);
            this.comboBoxAirlossRe.Name = "comboBoxAirlossRe";
            this.comboBoxAirlossRe.Size = new System.Drawing.Size(81, 20);
            this.comboBoxAirlossRe.TabIndex = 14;
            this.comboBoxAirlossRe.Visible = false;
            // 
            // comboBoxBarcodeGun
            // 
            this.comboBoxBarcodeGun.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxBarcodeGun.FormattingEnabled = true;
            this.comboBoxBarcodeGun.Location = new System.Drawing.Point(90, 194);
            this.comboBoxBarcodeGun.Name = "comboBoxBarcodeGun";
            this.comboBoxBarcodeGun.Size = new System.Drawing.Size(73, 20);
            this.comboBoxBarcodeGun.TabIndex = 12;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(9, 197);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(41, 12);
            this.label16.TabIndex = 13;
            this.label16.Text = "条码枪";
            // 
            // textBoxBarcode
            // 
            this.textBoxBarcode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxBarcode.Location = new System.Drawing.Point(60, 220);
            this.textBoxBarcode.Name = "textBoxBarcode";
            this.textBoxBarcode.Size = new System.Drawing.Size(107, 21);
            this.textBoxBarcode.TabIndex = 11;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(9, 229);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(41, 12);
            this.label17.TabIndex = 10;
            this.label17.Text = "条码值";
            // 
            // comboBoxRfid
            // 
            this.comboBoxRfid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxRfid.FormattingEnabled = true;
            this.comboBoxRfid.Location = new System.Drawing.Point(90, 133);
            this.comboBoxRfid.Name = "comboBoxRfid";
            this.comboBoxRfid.Size = new System.Drawing.Size(81, 20);
            this.comboBoxRfid.TabIndex = 8;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(7, 115);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(77, 12);
            this.label18.TabIndex = 9;
            this.label18.Text = "气密检测结果";
            this.label18.Visible = false;
            this.label18.Click += new System.EventHandler(this.label18_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 136);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(71, 12);
            this.label9.TabIndex = 9;
            this.label9.Text = "RFID读/写卡";
            // 
            // buttonDB2Reset
            // 
            this.buttonDB2Reset.Location = new System.Drawing.Point(3, 74);
            this.buttonDB2Reset.Name = "buttonDB2Reset";
            this.buttonDB2Reset.Size = new System.Drawing.Size(89, 24);
            this.buttonDB2Reset.TabIndex = 7;
            this.buttonDB2Reset.Text = "DB2复位";
            this.buttonDB2Reset.UseVisualStyleBackColor = true;
            // 
            // comboBoxDB2
            // 
            this.comboBoxDB2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDB2.FormattingEnabled = true;
            this.comboBoxDB2.Location = new System.Drawing.Point(84, 18);
            this.comboBoxDB2.Name = "comboBoxDB2";
            this.comboBoxDB2.Size = new System.Drawing.Size(86, 20);
            this.comboBoxDB2.TabIndex = 1;
            // 
            // textBoxRfidVal
            // 
            this.textBoxRfidVal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRfidVal.Location = new System.Drawing.Point(60, 159);
            this.textBoxRfidVal.Name = "textBoxRfidVal";
            this.textBoxRfidVal.Size = new System.Drawing.Size(107, 21);
            this.textBoxRfidVal.TabIndex = 5;
            // 
            // textBoxDB2ItemVal
            // 
            this.textBoxDB2ItemVal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDB2ItemVal.Location = new System.Drawing.Point(85, 49);
            this.textBoxDB2ItemVal.Name = "textBoxDB2ItemVal";
            this.textBoxDB2ItemVal.Size = new System.Drawing.Size(85, 21);
            this.textBoxDB2ItemVal.TabIndex = 5;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(7, 21);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(71, 24);
            this.label23.TabIndex = 2;
            this.label23.Text = "DB2索引\r\n（从1开始）";
            // 
            // buttonRfidSimWrite
            // 
            this.buttonRfidSimWrite.Location = new System.Drawing.Point(2, 244);
            this.buttonRfidSimWrite.Name = "buttonRfidSimWrite";
            this.buttonRfidSimWrite.Size = new System.Drawing.Size(176, 27);
            this.buttonRfidSimWrite.TabIndex = 0;
            this.buttonRfidSimWrite.Text = "模拟写入";
            this.buttonRfidSimWrite.UseVisualStyleBackColor = true;
            this.buttonRfidSimWrite.Click += new System.EventHandler(this.buttonRfidSimWrite_Click);
            // 
            // buttonDB2SimSet
            // 
            this.buttonDB2SimSet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDB2SimSet.Location = new System.Drawing.Point(98, 74);
            this.buttonDB2SimSet.Name = "buttonDB2SimSet";
            this.buttonDB2SimSet.Size = new System.Drawing.Size(72, 24);
            this.buttonDB2SimSet.TabIndex = 0;
            this.buttonDB2SimSet.Text = "模拟写入";
            this.buttonDB2SimSet.UseVisualStyleBackColor = true;
            this.buttonDB2SimSet.Click += new System.EventHandler(this.buttonDB2SimSet_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 168);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 2;
            this.label7.Text = "RFID数值";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(9, 56);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(29, 12);
            this.label25.TabIndex = 2;
            this.label25.Text = "数值";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 6;
            this.label5.Text = "选择工位";
            // 
            // comboBoxDevList
            // 
            this.comboBoxDevList.FormattingEnabled = true;
            this.comboBoxDevList.Location = new System.Drawing.Point(5, 27);
            this.comboBoxDevList.Name = "comboBoxDevList";
            this.comboBoxDevList.Size = new System.Drawing.Size(185, 20);
            this.comboBoxDevList.TabIndex = 5;
            // 
            // buttonClearDevCmd
            // 
            this.buttonClearDevCmd.Location = new System.Drawing.Point(100, 75);
            this.buttonClearDevCmd.Name = "buttonClearDevCmd";
            this.buttonClearDevCmd.Size = new System.Drawing.Size(90, 27);
            this.buttonClearDevCmd.TabIndex = 3;
            this.buttonClearDevCmd.Text = "复位";
            this.buttonClearDevCmd.UseVisualStyleBackColor = true;
            this.buttonClearDevCmd.Visible = false;
            this.buttonClearDevCmd.Click += new System.EventHandler(this.buttonClearDevCmd_Click_1);
            // 
            // buttonRefreshDevStatus
            // 
            this.buttonRefreshDevStatus.Location = new System.Drawing.Point(7, 75);
            this.buttonRefreshDevStatus.Name = "buttonRefreshDevStatus";
            this.buttonRefreshDevStatus.Size = new System.Drawing.Size(87, 27);
            this.buttonRefreshDevStatus.TabIndex = 4;
            this.buttonRefreshDevStatus.Text = "手动刷新";
            this.buttonRefreshDevStatus.UseVisualStyleBackColor = true;
            this.buttonRefreshDevStatus.Click += new System.EventHandler(this.buttonRefreshDevStatus_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Controls.Add(this.dataGridViewDevDB1, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.dataGridViewDevDB2, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.label6, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label8, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.groupBox1, 0, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 96F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(684, 437);
            this.tableLayoutPanel2.TabIndex = 9;
            // 
            // dataGridViewDevDB1
            // 
            this.dataGridViewDevDB1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewDevDB1.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewDevDB1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewDevDB1.Location = new System.Drawing.Point(3, 26);
            this.dataGridViewDevDB1.Name = "dataGridViewDevDB1";
            this.dataGridViewDevDB1.RowTemplate.Height = 23;
            this.dataGridViewDevDB1.Size = new System.Drawing.Size(336, 312);
            this.dataGridViewDevDB1.TabIndex = 3;
            // 
            // dataGridViewDevDB2
            // 
            this.dataGridViewDevDB2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewDevDB2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewDevDB2.Location = new System.Drawing.Point(345, 26);
            this.dataGridViewDevDB2.Name = "dataGridViewDevDB2";
            this.dataGridViewDevDB2.RowTemplate.Height = 23;
            this.dataGridViewDevDB2.Size = new System.Drawing.Size(336, 312);
            this.dataGridViewDevDB2.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Orange;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(336, 23);
            this.label6.TabIndex = 5;
            this.label6.Text = "DB1";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Orange;
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(345, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(336, 23);
            this.label8.TabIndex = 6;
            this.label8.Text = "DB2";
            // 
            // groupBox1
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.groupBox1, 2);
            this.groupBox1.Controls.Add(this.richTextBoxTaskInfo);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 344);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(678, 90);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "流程详细";
            // 
            // richTextBoxTaskInfo
            // 
            this.richTextBoxTaskInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxTaskInfo.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.richTextBoxTaskInfo.Location = new System.Drawing.Point(3, 17);
            this.richTextBoxTaskInfo.Name = "richTextBoxTaskInfo";
            this.richTextBoxTaskInfo.Size = new System.Drawing.Size(672, 70);
            this.richTextBoxTaskInfo.TabIndex = 0;
            this.richTextBoxTaskInfo.Text = "";
            // 
            // NodeMonitorView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(924, 477);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "NodeMonitorView";
            this.Text = "NodeMonitorView";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.NodeMonitorView_FormClosed);
            this.Load += new System.EventHandler(this.NodeMonitorView_Load);
            this.SizeChanged += new System.EventHandler(this.NodeMonitorView_SizeChanged);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tableLayoutMonitor.ResumeLayout(false);
            this.flowLayoutPicSample.ResumeLayout(false);
            this.flowLayoutPicSample.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBoxCtlSim.ResumeLayout(false);
            this.groupBoxCtlSim.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDevDB1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDevDB2)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timerNodeStatus;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutMonitor;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPicSample;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.CheckBox checkBoxAutorefresh;
        private System.Windows.Forms.GroupBox groupBoxCtlSim;
        private System.Windows.Forms.ComboBox comboBoxRfid;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button buttonDB2Reset;
        private System.Windows.Forms.ComboBox comboBoxDB2;
        private System.Windows.Forms.TextBox textBoxRfidVal;
        private System.Windows.Forms.TextBox textBoxDB2ItemVal;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Button buttonRfidSimWrite;
        private System.Windows.Forms.Button buttonDB2SimSet;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxDevList;
        private System.Windows.Forms.Button buttonClearDevCmd;
        private System.Windows.Forms.Button buttonRefreshDevStatus;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.DataGridView dataGridViewDevDB1;
        private System.Windows.Forms.DataGridView dataGridViewDevDB2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox comboBoxBarcodeGun;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox textBoxBarcode;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.ComboBox comboBoxAirlossRe;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label labelCommInfo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox richTextBoxTaskInfo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private View.UserControlLine userControlLine1;
        private View.UserControlLine userControlLine2;
        private View.UserControlLine userControlLine3;
       
    }
}