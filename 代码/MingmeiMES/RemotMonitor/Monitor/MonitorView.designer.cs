namespace RemotMonitor
{
    partial class MonitorView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MonitorView));
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.bt_StartSystem = new System.Windows.Forms.Button();
            this.bt_StopSystem = new System.Windows.Forms.Button();
            this.bt_ExitSys = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
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
            this.panel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
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
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.ItemSize = new System.Drawing.Size(84, 26);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(967, 477);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.tabPage1.Controls.Add(this.tableLayoutMonitor);
            this.tabPage1.Location = new System.Drawing.Point(4, 30);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(0);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(959, 443);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "工位状态监控";
            // 
            // tableLayoutMonitor
            // 
            this.tableLayoutMonitor.ColumnCount = 1;
            this.tableLayoutMonitor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutMonitor.Controls.Add(this.panel3, 0, 3);
            this.tableLayoutMonitor.Controls.Add(this.flowLayoutPicSample, 0, 0);
            this.tableLayoutMonitor.Controls.Add(this.flowLayoutPanel1, 0, 2);
            this.tableLayoutMonitor.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutMonitor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutMonitor.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutMonitor.Name = "tableLayoutMonitor";
            this.tableLayoutMonitor.RowCount = 4;
            this.tableLayoutMonitor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutMonitor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.tableLayoutMonitor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutMonitor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 199F));
            this.tableLayoutMonitor.Size = new System.Drawing.Size(959, 443);
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
            this.panel3.Location = new System.Drawing.Point(3, 247);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(953, 193);
            this.panel3.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(33, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(163, 29);
            this.label2.TabIndex = 1;
            this.label2.Text = "投产型号：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.Color.Green;
            this.label4.Location = new System.Drawing.Point(202, 56);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 29);
            this.label4.TabIndex = 0;
            this.label4.Text = "型号";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.Green;
            this.label3.Location = new System.Drawing.Point(202, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(163, 29);
            this.label3.TabIndex = 0;
            this.label3.Text = "投产条码：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(33, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(163, 29);
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
            this.flowLayoutPicSample.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPicSample.Font = new System.Drawing.Font("方正姚体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.flowLayoutPicSample.Location = new System.Drawing.Point(3, 2);
            this.flowLayoutPicSample.Margin = new System.Windows.Forms.Padding(3, 2, 3, 1);
            this.flowLayoutPicSample.Name = "flowLayoutPicSample";
            this.flowLayoutPicSample.Size = new System.Drawing.Size(953, 25);
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
            this.label10.Location = new System.Drawing.Point(38, 0);
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
            this.label11.Location = new System.Drawing.Point(182, 0);
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
            this.label13.Location = new System.Drawing.Point(326, 0);
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
            this.label14.Location = new System.Drawing.Point(487, 0);
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
            this.label15.Location = new System.Drawing.Point(631, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(76, 18);
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
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 73);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(953, 168);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.flowLayoutPanel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 31);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(953, 36);
            this.panel1.TabIndex = 0;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.flowLayoutPanel2.Controls.Add(this.label12);
            this.flowLayoutPanel2.Controls.Add(this.bt_StartSystem);
            this.flowLayoutPanel2.Controls.Add(this.bt_StopSystem);
            this.flowLayoutPanel2.Controls.Add(this.bt_ExitSys);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(-4, 4);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(397, 34);
            this.flowLayoutPanel2.TabIndex = 8;
            // 
            // bt_StartSystem
            // 
            this.bt_StartSystem.BackColor = System.Drawing.Color.Khaki;
            this.bt_StartSystem.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bt_StartSystem.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.bt_StartSystem.Image = ((System.Drawing.Image)(resources.GetObject("bt_StartSystem.Image")));
            this.bt_StartSystem.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_StartSystem.Location = new System.Drawing.Point(3, 22);
            this.bt_StartSystem.Name = "bt_StartSystem";
            this.bt_StartSystem.Size = new System.Drawing.Size(128, 50);
            this.bt_StartSystem.TabIndex = 38;
            this.bt_StartSystem.Text = "启动系统";
            this.bt_StartSystem.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bt_StartSystem.UseVisualStyleBackColor = false;
            this.bt_StartSystem.Visible = false;
            // 
            // bt_StopSystem
            // 
            this.bt_StopSystem.BackColor = System.Drawing.Color.Khaki;
            this.bt_StopSystem.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bt_StopSystem.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.bt_StopSystem.Image = ((System.Drawing.Image)(resources.GetObject("bt_StopSystem.Image")));
            this.bt_StopSystem.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_StopSystem.Location = new System.Drawing.Point(137, 22);
            this.bt_StopSystem.Name = "bt_StopSystem";
            this.bt_StopSystem.Size = new System.Drawing.Size(133, 49);
            this.bt_StopSystem.TabIndex = 39;
            this.bt_StopSystem.Text = "停止系统";
            this.bt_StopSystem.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bt_StopSystem.UseVisualStyleBackColor = false;
            this.bt_StopSystem.Visible = false;
            // 
            // bt_ExitSys
            // 
            this.bt_ExitSys.BackColor = System.Drawing.Color.Khaki;
            this.bt_ExitSys.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bt_ExitSys.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.bt_ExitSys.Image = ((System.Drawing.Image)(resources.GetObject("bt_ExitSys.Image")));
            this.bt_ExitSys.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_ExitSys.Location = new System.Drawing.Point(276, 22);
            this.bt_ExitSys.Name = "bt_ExitSys";
            this.bt_ExitSys.Size = new System.Drawing.Size(115, 49);
            this.bt_ExitSys.TabIndex = 40;
            this.bt_ExitSys.Text = "退出";
            this.bt_ExitSys.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bt_ExitSys.UseVisualStyleBackColor = false;
            this.bt_ExitSys.Visible = false;
            // 
            // label12
            // 
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label12.ForeColor = System.Drawing.Color.SteelBlue;
            this.label12.Location = new System.Drawing.Point(3, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(260, 19);
            this.label12.TabIndex = 7;
            this.label12.Text = "当前正在检测的设备数量：0";
            // 
            // MonitorView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(967, 477);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MonitorView";
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
            this.panel1.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timerNodeStatus;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutMonitor;
        private System.Windows.Forms.Panel panel1;
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
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button bt_StartSystem;
        private System.Windows.Forms.Button bt_StopSystem;
        private System.Windows.Forms.Button bt_ExitSys;
       
    }
}