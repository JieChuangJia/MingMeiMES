namespace DeviceAssist
{
    partial class PlcTestForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlcTestForm));
            this.label34 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.textBoxPlcPort = new System.Windows.Forms.TextBox();
            this.textBoxPlcIP = new System.Windows.Forms.TextBox();
            this.buttonClosePlc = new System.Windows.Forms.Button();
            this.buttonConnectPlc = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.buttonReadPlc = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.buttonWritePlc = new System.Windows.Forms.Button();
            this.label19 = new System.Windows.Forms.Label();
            this.textBoxPlcAddr = new System.Windows.Forms.TextBox();
            this.textBoxPlcVal = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.label2 = new System.Windows.Forms.Label();
            this.richTextBoxLog = new System.Windows.Forms.RichTextBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(379, 24);
            this.label34.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(37, 15);
            this.label34.TabIndex = 16;
            this.label34.Text = "端口";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(55, 21);
            this.label33.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(85, 15);
            this.label33.TabIndex = 17;
            this.label33.Text = "PLC ip地址";
            // 
            // textBoxPlcPort
            // 
            this.textBoxPlcPort.Location = new System.Drawing.Point(425, 12);
            this.textBoxPlcPort.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxPlcPort.Name = "textBoxPlcPort";
            this.textBoxPlcPort.Size = new System.Drawing.Size(80, 25);
            this.textBoxPlcPort.TabIndex = 14;
            this.textBoxPlcPort.Text = "5000";
            // 
            // textBoxPlcIP
            // 
            this.textBoxPlcIP.Location = new System.Drawing.Point(152, 14);
            this.textBoxPlcIP.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxPlcIP.Name = "textBoxPlcIP";
            this.textBoxPlcIP.Size = new System.Drawing.Size(189, 25);
            this.textBoxPlcIP.TabIndex = 15;
            this.textBoxPlcIP.Text = "192.168.1.100";
            // 
            // buttonClosePlc
            // 
            this.buttonClosePlc.Location = new System.Drawing.Point(263, 66);
            this.buttonClosePlc.Margin = new System.Windows.Forms.Padding(4);
            this.buttonClosePlc.Name = "buttonClosePlc";
            this.buttonClosePlc.Size = new System.Drawing.Size(183, 45);
            this.buttonClosePlc.TabIndex = 18;
            this.buttonClosePlc.Text = "断开PLC";
            this.buttonClosePlc.UseVisualStyleBackColor = true;
            this.buttonClosePlc.Click += new System.EventHandler(this.buttonClosePlc_Click);
            // 
            // buttonConnectPlc
            // 
            this.buttonConnectPlc.Location = new System.Drawing.Point(57, 66);
            this.buttonConnectPlc.Margin = new System.Windows.Forms.Padding(4);
            this.buttonConnectPlc.Name = "buttonConnectPlc";
            this.buttonConnectPlc.Size = new System.Drawing.Size(183, 45);
            this.buttonConnectPlc.TabIndex = 19;
            this.buttonConnectPlc.Text = "连接PLC";
            this.buttonConnectPlc.UseVisualStyleBackColor = true;
            this.buttonConnectPlc.Click += new System.EventHandler(this.buttonConnectPlc_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.buttonReadPlc);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.buttonWritePlc);
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Controls.Add(this.textBoxPlcAddr);
            this.groupBox1.Controls.Add(this.textBoxPlcVal);
            this.groupBox1.Location = new System.Drawing.Point(57, 134);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(768, 155);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "单块读写";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(203, 21);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 15);
            this.label1.TabIndex = 14;
            this.label1.Text = "执行次数：";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(9, 32);
            this.label18.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(37, 15);
            this.label18.TabIndex = 13;
            this.label18.Text = "地址";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(8, 105);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(311, 42);
            this.button1.TabIndex = 7;
            this.button1.Text = "停止";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(477, 43);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(120, 42);
            this.button2.TabIndex = 7;
            this.button2.Text = "手动读";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // buttonReadPlc
            // 
            this.buttonReadPlc.Location = new System.Drawing.Point(199, 41);
            this.buttonReadPlc.Margin = new System.Windows.Forms.Padding(4);
            this.buttonReadPlc.Name = "buttonReadPlc";
            this.buttonReadPlc.Size = new System.Drawing.Size(120, 42);
            this.buttonReadPlc.TabIndex = 7;
            this.buttonReadPlc.Text = "自动读";
            this.buttonReadPlc.UseVisualStyleBackColor = true;
            this.buttonReadPlc.Click += new System.EventHandler(this.buttonReadPlc_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(622, 46);
            this.button3.Margin = new System.Windows.Forms.Padding(4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(120, 39);
            this.button3.TabIndex = 10;
            this.button3.Text = "手动写";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // buttonWritePlc
            // 
            this.buttonWritePlc.Location = new System.Drawing.Point(327, 45);
            this.buttonWritePlc.Margin = new System.Windows.Forms.Padding(4);
            this.buttonWritePlc.Name = "buttonWritePlc";
            this.buttonWritePlc.Size = new System.Drawing.Size(120, 39);
            this.buttonWritePlc.TabIndex = 10;
            this.buttonWritePlc.Text = "自动写";
            this.buttonWritePlc.UseVisualStyleBackColor = true;
            this.buttonWritePlc.Click += new System.EventHandler(this.buttonWritePlc_Click);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(9, 66);
            this.label19.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(37, 15);
            this.label19.TabIndex = 13;
            this.label19.Text = "内容";
            // 
            // textBoxPlcAddr
            // 
            this.textBoxPlcAddr.Location = new System.Drawing.Point(100, 21);
            this.textBoxPlcAddr.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxPlcAddr.Name = "textBoxPlcAddr";
            this.textBoxPlcAddr.Size = new System.Drawing.Size(75, 25);
            this.textBoxPlcAddr.TabIndex = 12;
            this.textBoxPlcAddr.Text = "D0100";
            // 
            // textBoxPlcVal
            // 
            this.textBoxPlcVal.Location = new System.Drawing.Point(100, 59);
            this.textBoxPlcVal.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxPlcVal.Name = "textBoxPlcVal";
            this.textBoxPlcVal.Size = new System.Drawing.Size(75, 25);
            this.textBoxPlcVal.TabIndex = 11;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel3);
            this.splitContainer1.Size = new System.Drawing.Size(1072, 546);
            this.splitContainer1.SplitterDistance = 297;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 21;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label33);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.textBoxPlcIP);
            this.panel1.Controls.Add(this.buttonClosePlc);
            this.panel1.Controls.Add(this.textBoxPlcPort);
            this.panel1.Controls.Add(this.buttonConnectPlc);
            this.panel1.Controls.Add(this.label34);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1072, 297);
            this.panel1.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.Highlight;
            this.panel3.Controls.Add(this.toolStrip1);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.richTextBoxLog);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Margin = new System.Windows.Forms.Padding(4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1072, 244);
            this.panel3.TabIndex = 2;
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripSeparator1});
            this.toolStrip1.Location = new System.Drawing.Point(100, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(91, 27);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(73, 24);
            this.toolStripButton1.Text = "清空日志";
            this.toolStripButton1.ToolTipText = "清空显示";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 11);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "调试日志";
            // 
            // richTextBoxLog
            // 
            this.richTextBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxLog.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.richTextBoxLog.Location = new System.Drawing.Point(0, 30);
            this.richTextBoxLog.Margin = new System.Windows.Forms.Padding(4);
            this.richTextBoxLog.Name = "richTextBoxLog";
            this.richTextBoxLog.Size = new System.Drawing.Size(1071, 209);
            this.richTextBoxLog.TabIndex = 0;
            this.richTextBoxLog.Text = "";
            // 
            // PlcTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1072, 546);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "PlcTestForm";
            this.Text = "PlcTestForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PlcTestForm_FormClosed);
            this.Load += new System.EventHandler(this.PlcTestForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.TextBox textBoxPlcPort;
        private System.Windows.Forms.TextBox textBoxPlcIP;
        private System.Windows.Forms.Button buttonClosePlc;
        private System.Windows.Forms.Button buttonConnectPlc;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Button buttonReadPlc;
        private System.Windows.Forms.Button buttonWritePlc;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox textBoxPlcAddr;
        private System.Windows.Forms.TextBox textBoxPlcVal;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox richTextBoxLog;
        public System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}