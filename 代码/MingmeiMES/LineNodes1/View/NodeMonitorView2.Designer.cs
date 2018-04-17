namespace LineNodes
{
    partial class NodeMonitorView2
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.timerNodeStatus = new System.Windows.Forms.Timer(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tableLayoutMonitor = new System.Windows.Forms.TableLayoutPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.labelCommInfo = new System.Windows.Forms.Label();
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
            this.panel3.SuspendLayout();
            this.flowLayoutPicSample.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
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
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1450, 716);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.tabPage1.Controls.Add(this.tableLayoutMonitor);
            this.tabPage1.Location = new System.Drawing.Point(4, 30);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(0);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(1442, 682);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "工位状态监控";
            // 
            // tableLayoutMonitor
            // 
            this.tableLayoutMonitor.ColumnCount = 1;
            this.tableLayoutMonitor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutMonitor.Controls.Add(this.panel3, 0, 2);
            this.tableLayoutMonitor.Controls.Add(this.flowLayoutPicSample, 0, 0);
            this.tableLayoutMonitor.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.tableLayoutMonitor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutMonitor.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutMonitor.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutMonitor.Name = "tableLayoutMonitor";
            this.tableLayoutMonitor.RowCount = 3;
            this.tableLayoutMonitor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.tableLayoutMonitor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutMonitor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 171F));
            this.tableLayoutMonitor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutMonitor.Size = new System.Drawing.Size(1442, 682);
            this.tableLayoutMonitor.TabIndex = 5;
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Location = new System.Drawing.Point(4, 517);
            this.panel3.Margin = new System.Windows.Forms.Padding(4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1434, 161);
            this.panel3.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(50, 78);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(245, 44);
            this.label2.TabIndex = 1;
            this.label2.Text = "投产型号：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.Color.Green;
            this.label4.Location = new System.Drawing.Point(303, 84);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(110, 44);
            this.label4.TabIndex = 0;
            this.label4.Text = "型号";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.Green;
            this.label3.Location = new System.Drawing.Point(303, 15);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(245, 44);
            this.label3.TabIndex = 0;
            this.label3.Text = "投产条码：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(50, 15);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(245, 44);
            this.label1.TabIndex = 0;
            this.label1.Text = "投产条码：";
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
            this.flowLayoutPicSample.Location = new System.Drawing.Point(4, 3);
            this.flowLayoutPicSample.Margin = new System.Windows.Forms.Padding(4, 3, 4, 2);
            this.flowLayoutPicSample.Name = "flowLayoutPicSample";
            this.flowLayoutPicSample.Size = new System.Drawing.Size(1434, 37);
            this.flowLayoutPicSample.TabIndex = 4;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Red;
            this.pictureBox1.Location = new System.Drawing.Point(4, 4);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(44, 20);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(56, 3);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(112, 27);
            this.label10.TabIndex = 1;
            this.label10.Text = "设备故障";
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Green;
            this.pictureBox2.Location = new System.Drawing.Point(217, 4);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(45, 4, 4, 4);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(44, 20);
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            // 
            // label11
            // 
            this.label11.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(269, 3);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(112, 27);
            this.label11.TabIndex = 1;
            this.label11.Text = "设备空闲";
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.Color.Yellow;
            this.pictureBox3.Location = new System.Drawing.Point(430, 4);
            this.pictureBox3.Margin = new System.Windows.Forms.Padding(45, 4, 4, 4);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(44, 20);
            this.pictureBox3.TabIndex = 0;
            this.pictureBox3.TabStop = false;
            // 
            // label13
            // 
            this.label13.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(482, 3);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(137, 27);
            this.label13.TabIndex = 1;
            this.label13.Text = "设备使用中";
            // 
            // pictureBox4
            // 
            this.pictureBox4.BackColor = System.Drawing.Color.LightSeaGreen;
            this.pictureBox4.Location = new System.Drawing.Point(668, 4);
            this.pictureBox4.Margin = new System.Windows.Forms.Padding(45, 4, 4, 4);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(44, 20);
            this.pictureBox4.TabIndex = 0;
            this.pictureBox4.TabStop = false;
            this.pictureBox4.Visible = false;
            // 
            // label14
            // 
            this.label14.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(720, 3);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(112, 27);
            this.label14.TabIndex = 1;
            this.label14.Text = "工位有板";
            this.label14.Visible = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.BackColor = System.Drawing.Color.PaleVioletRed;
            this.pictureBox5.Location = new System.Drawing.Point(881, 4);
            this.pictureBox5.Margin = new System.Windows.Forms.Padding(45, 4, 4, 4);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(44, 20);
            this.pictureBox5.TabIndex = 0;
            this.pictureBox5.TabStop = false;
            // 
            // label15
            // 
            this.label15.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(933, 3);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(112, 27);
            this.label15.TabIndex = 1;
            this.label15.Text = "无法识别";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.flowLayoutPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(4, 46);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1434, 461);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // labelCommInfo
            // 
            this.labelCommInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCommInfo.BackColor = System.Drawing.SystemColors.HotTrack;
            this.labelCommInfo.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelCommInfo.ForeColor = System.Drawing.Color.Yellow;
            this.labelCommInfo.Location = new System.Drawing.Point(1053, 0);
            this.labelCommInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelCommInfo.Name = "labelCommInfo";
            this.labelCommInfo.Size = new System.Drawing.Size(208, 33);
            this.labelCommInfo.TabIndex = 9;
            this.labelCommInfo.Text = "通信周期：";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.tabPage2.Controls.Add(this.splitContainer1);
            this.tabPage2.Location = new System.Drawing.Point(4, 30);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage2.Size = new System.Drawing.Size(1442, 682);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "数据通信监控";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(4, 4);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
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
            this.splitContainer1.Size = new System.Drawing.Size(1434, 674);
            this.splitContainer1.SplitterDistance = 416;
            this.splitContainer1.SplitterWidth = 6;
            this.splitContainer1.TabIndex = 0;
            // 
            // checkBoxAutorefresh
            // 
            this.checkBoxAutorefresh.AutoSize = true;
            this.checkBoxAutorefresh.Location = new System.Drawing.Point(14, 80);
            this.checkBoxAutorefresh.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxAutorefresh.Name = "checkBoxAutorefresh";
            this.checkBoxAutorefresh.Size = new System.Drawing.Size(106, 22);
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
            this.groupBoxCtlSim.Location = new System.Drawing.Point(10, 164);
            this.groupBoxCtlSim.Margin = new System.Windows.Forms.Padding(4);
            this.groupBoxCtlSim.Name = "groupBoxCtlSim";
            this.groupBoxCtlSim.Padding = new System.Windows.Forms.Padding(4);
            this.groupBoxCtlSim.Size = new System.Drawing.Size(442, 430);
            this.groupBoxCtlSim.TabIndex = 8;
            this.groupBoxCtlSim.TabStop = false;
            this.groupBoxCtlSim.Text = "仿真模拟";
            // 
            // comboBoxAirlossRe
            // 
            this.comboBoxAirlossRe.FormattingEnabled = true;
            this.comboBoxAirlossRe.Items.AddRange(new object[] {
            "OK",
            "NG"});
            this.comboBoxAirlossRe.Location = new System.Drawing.Point(135, 160);
            this.comboBoxAirlossRe.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxAirlossRe.Name = "comboBoxAirlossRe";
            this.comboBoxAirlossRe.Size = new System.Drawing.Size(138, 26);
            this.comboBoxAirlossRe.TabIndex = 14;
            this.comboBoxAirlossRe.Visible = false;
            // 
            // comboBoxBarcodeGun
            // 
            this.comboBoxBarcodeGun.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxBarcodeGun.FormattingEnabled = true;
            this.comboBoxBarcodeGun.Location = new System.Drawing.Point(135, 291);
            this.comboBoxBarcodeGun.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxBarcodeGun.Name = "comboBoxBarcodeGun";
            this.comboBoxBarcodeGun.Size = new System.Drawing.Size(196, 26);
            this.comboBoxBarcodeGun.TabIndex = 12;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(14, 296);
            this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(62, 18);
            this.label16.TabIndex = 13;
            this.label16.Text = "条码枪";
            // 
            // textBoxBarcode
            // 
            this.textBoxBarcode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxBarcode.Location = new System.Drawing.Point(90, 330);
            this.textBoxBarcode.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxBarcode.Name = "textBoxBarcode";
            this.textBoxBarcode.Size = new System.Drawing.Size(247, 28);
            this.textBoxBarcode.TabIndex = 11;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(14, 344);
            this.label17.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(62, 18);
            this.label17.TabIndex = 10;
            this.label17.Text = "条码值";
            // 
            // comboBoxRfid
            // 
            this.comboBoxRfid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxRfid.FormattingEnabled = true;
            this.comboBoxRfid.Location = new System.Drawing.Point(135, 200);
            this.comboBoxRfid.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxRfid.Name = "comboBoxRfid";
            this.comboBoxRfid.Size = new System.Drawing.Size(196, 26);
            this.comboBoxRfid.TabIndex = 8;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(10, 172);
            this.label18.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(116, 18);
            this.label18.TabIndex = 9;
            this.label18.Text = "气密检测结果";
            this.label18.Visible = false;
            this.label18.Click += new System.EventHandler(this.label18_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(14, 204);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(107, 18);
            this.label9.TabIndex = 9;
            this.label9.Text = "RFID读/写卡";
            // 
            // buttonDB2Reset
            // 
            this.buttonDB2Reset.Location = new System.Drawing.Point(4, 111);
            this.buttonDB2Reset.Margin = new System.Windows.Forms.Padding(4);
            this.buttonDB2Reset.Name = "buttonDB2Reset";
            this.buttonDB2Reset.Size = new System.Drawing.Size(134, 36);
            this.buttonDB2Reset.TabIndex = 7;
            this.buttonDB2Reset.Text = "DB2复位";
            this.buttonDB2Reset.UseVisualStyleBackColor = true;
            // 
            // comboBoxDB2
            // 
            this.comboBoxDB2.FormattingEnabled = true;
            this.comboBoxDB2.Location = new System.Drawing.Point(126, 27);
            this.comboBoxDB2.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxDB2.Name = "comboBoxDB2";
            this.comboBoxDB2.Size = new System.Drawing.Size(145, 26);
            this.comboBoxDB2.TabIndex = 1;
            // 
            // textBoxRfidVal
            // 
            this.textBoxRfidVal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxRfidVal.Location = new System.Drawing.Point(90, 238);
            this.textBoxRfidVal.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxRfidVal.Name = "textBoxRfidVal";
            this.textBoxRfidVal.Size = new System.Drawing.Size(247, 28);
            this.textBoxRfidVal.TabIndex = 5;
            // 
            // textBoxDB2ItemVal
            // 
            this.textBoxDB2ItemVal.Location = new System.Drawing.Point(128, 74);
            this.textBoxDB2ItemVal.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxDB2ItemVal.Name = "textBoxDB2ItemVal";
            this.textBoxDB2ItemVal.Size = new System.Drawing.Size(144, 28);
            this.textBoxDB2ItemVal.TabIndex = 5;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(10, 32);
            this.label23.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(107, 36);
            this.label23.TabIndex = 2;
            this.label23.Text = "DB2索引\r\n（从1开始）";
            // 
            // buttonRfidSimWrite
            // 
            this.buttonRfidSimWrite.Location = new System.Drawing.Point(3, 366);
            this.buttonRfidSimWrite.Margin = new System.Windows.Forms.Padding(4);
            this.buttonRfidSimWrite.Name = "buttonRfidSimWrite";
            this.buttonRfidSimWrite.Size = new System.Drawing.Size(264, 40);
            this.buttonRfidSimWrite.TabIndex = 0;
            this.buttonRfidSimWrite.Text = "模拟写入";
            this.buttonRfidSimWrite.UseVisualStyleBackColor = true;
            this.buttonRfidSimWrite.Click += new System.EventHandler(this.buttonRfidSimWrite_Click);
            // 
            // buttonDB2SimSet
            // 
            this.buttonDB2SimSet.Location = new System.Drawing.Point(147, 111);
            this.buttonDB2SimSet.Margin = new System.Windows.Forms.Padding(4);
            this.buttonDB2SimSet.Name = "buttonDB2SimSet";
            this.buttonDB2SimSet.Size = new System.Drawing.Size(126, 36);
            this.buttonDB2SimSet.TabIndex = 0;
            this.buttonDB2SimSet.Text = "模拟写入";
            this.buttonDB2SimSet.UseVisualStyleBackColor = true;
            this.buttonDB2SimSet.Click += new System.EventHandler(this.buttonDB2SimSet_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(14, 252);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 18);
            this.label7.TabIndex = 2;
            this.label7.Text = "RFID数值";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(14, 84);
            this.label25.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(44, 18);
            this.label25.TabIndex = 2;
            this.label25.Text = "数值";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 18);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 18);
            this.label5.TabIndex = 6;
            this.label5.Text = "选择工位";
            // 
            // comboBoxDevList
            // 
            this.comboBoxDevList.FormattingEnabled = true;
            this.comboBoxDevList.Location = new System.Drawing.Point(8, 40);
            this.comboBoxDevList.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxDevList.Name = "comboBoxDevList";
            this.comboBoxDevList.Size = new System.Drawing.Size(276, 26);
            this.comboBoxDevList.TabIndex = 5;
            // 
            // buttonClearDevCmd
            // 
            this.buttonClearDevCmd.Location = new System.Drawing.Point(150, 112);
            this.buttonClearDevCmd.Margin = new System.Windows.Forms.Padding(4);
            this.buttonClearDevCmd.Name = "buttonClearDevCmd";
            this.buttonClearDevCmd.Size = new System.Drawing.Size(135, 40);
            this.buttonClearDevCmd.TabIndex = 3;
            this.buttonClearDevCmd.Text = "复位";
            this.buttonClearDevCmd.UseVisualStyleBackColor = true;
            this.buttonClearDevCmd.Visible = false;
            // 
            // buttonRefreshDevStatus
            // 
            this.buttonRefreshDevStatus.Location = new System.Drawing.Point(10, 112);
            this.buttonRefreshDevStatus.Margin = new System.Windows.Forms.Padding(4);
            this.buttonRefreshDevStatus.Name = "buttonRefreshDevStatus";
            this.buttonRefreshDevStatus.Size = new System.Drawing.Size(130, 40);
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
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.Controls.Add(this.dataGridViewDevDB1, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.dataGridViewDevDB2, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.label6, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label8, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.groupBox1, 0, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 144F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1012, 674);
            this.tableLayoutPanel2.TabIndex = 9;
            // 
            // dataGridViewDevDB1
            // 
            this.dataGridViewDevDB1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewDevDB1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewDevDB1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewDevDB1.Location = new System.Drawing.Point(4, 38);
            this.dataGridViewDevDB1.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridViewDevDB1.Name = "dataGridViewDevDB1";
            this.dataGridViewDevDB1.RowTemplate.Height = 23;
            this.dataGridViewDevDB1.Size = new System.Drawing.Size(498, 488);
            this.dataGridViewDevDB1.TabIndex = 3;
            // 
            // dataGridViewDevDB2
            // 
            this.dataGridViewDevDB2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewDevDB2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewDevDB2.Location = new System.Drawing.Point(510, 38);
            this.dataGridViewDevDB2.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridViewDevDB2.Name = "dataGridViewDevDB2";
            this.dataGridViewDevDB2.RowTemplate.Height = 23;
            this.dataGridViewDevDB2.Size = new System.Drawing.Size(498, 488);
            this.dataGridViewDevDB2.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Orange;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(4, 0);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(498, 34);
            this.label6.TabIndex = 5;
            this.label6.Text = "DB1";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Orange;
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(510, 0);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(498, 34);
            this.label8.TabIndex = 6;
            this.label8.Text = "DB2";
            // 
            // groupBox1
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.groupBox1, 2);
            this.groupBox1.Controls.Add(this.richTextBoxTaskInfo);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(4, 534);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(1004, 136);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "流程详细";
            // 
            // richTextBoxTaskInfo
            // 
            this.richTextBoxTaskInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxTaskInfo.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.richTextBoxTaskInfo.Location = new System.Drawing.Point(4, 25);
            this.richTextBoxTaskInfo.Margin = new System.Windows.Forms.Padding(4);
            this.richTextBoxTaskInfo.Name = "richTextBoxTaskInfo";
            this.richTextBoxTaskInfo.Size = new System.Drawing.Size(996, 107);
            this.richTextBoxTaskInfo.TabIndex = 0;
            this.richTextBoxTaskInfo.Text = "";
            // 
            // NodeMonitorView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1450, 716);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "NodeMonitorView";
            this.Text = "NodeMonitorView";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.NodeMonitorView_FormClosed);
            this.Load += new System.EventHandler(this.NodeMonitorView_Load);
            this.SizeChanged += new System.EventHandler(this.NodeMonitorView_SizeChanged);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tableLayoutMonitor.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.flowLayoutPicSample.ResumeLayout(false);
            this.flowLayoutPicSample.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
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
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPicSample;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
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
       
    }
}