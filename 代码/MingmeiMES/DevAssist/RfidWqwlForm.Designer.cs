namespace DeviceAssist
{
    partial class RfidWqwlForm
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnClearEPClist = new System.Windows.Forms.Panel();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.buttonReadID = new System.Windows.Forms.Button();
            this.buttonWriteID = new System.Windows.Forms.Button();
            this.label30 = new System.Windows.Forms.Label();
            this.textBoxReadData = new System.Windows.Forms.TextBox();
            this.textBoxWriteData = new System.Windows.Forms.TextBox();
            this.label29 = new System.Windows.Forms.Label();
            this.btnClearEpcs = new System.Windows.Forms.Button();
            this.buttonReadUID = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.textBoxUID = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.buttonDisconn = new System.Windows.Forms.Button();
            this.buttonConn = new System.Windows.Forms.Button();
            this.label33 = new System.Windows.Forms.Label();
            this.textBoxIP = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.textBoxHostIP = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.btnClearEPClist.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.btnClearEPClist, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30.42328F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 69.57672F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(643, 378);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // btnClearEPClist
            // 
            this.btnClearEPClist.Controls.Add(this.radioButton2);
            this.btnClearEPClist.Controls.Add(this.radioButton1);
            this.btnClearEPClist.Controls.Add(this.listBox1);
            this.btnClearEPClist.Controls.Add(this.buttonReadID);
            this.btnClearEPClist.Controls.Add(this.buttonWriteID);
            this.btnClearEPClist.Controls.Add(this.label30);
            this.btnClearEPClist.Controls.Add(this.textBoxReadData);
            this.btnClearEPClist.Controls.Add(this.textBoxWriteData);
            this.btnClearEPClist.Controls.Add(this.label29);
            this.btnClearEPClist.Controls.Add(this.btnClearEpcs);
            this.btnClearEPClist.Controls.Add(this.buttonReadUID);
            this.btnClearEPClist.Controls.Add(this.label14);
            this.btnClearEPClist.Controls.Add(this.textBoxUID);
            this.btnClearEPClist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnClearEPClist.Location = new System.Drawing.Point(3, 118);
            this.btnClearEPClist.Name = "btnClearEPClist";
            this.btnClearEPClist.Size = new System.Drawing.Size(637, 257);
            this.btnClearEPClist.TabIndex = 0;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(112, 79);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(95, 16);
            this.radioButton2.TabIndex = 19;
            this.radioButton2.Text = "字符串型数据";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(11, 79);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(71, 16);
            this.radioButton1.TabIndex = 19;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "整形数据";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(472, 49);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(162, 124);
            this.listBox1.TabIndex = 18;
            // 
            // buttonReadID
            // 
            this.buttonReadID.Location = new System.Drawing.Point(356, 101);
            this.buttonReadID.Name = "buttonReadID";
            this.buttonReadID.Size = new System.Drawing.Size(104, 30);
            this.buttonReadID.TabIndex = 17;
            this.buttonReadID.Text = "回读数据";
            this.buttonReadID.UseVisualStyleBackColor = true;
            this.buttonReadID.Click += new System.EventHandler(this.buttonReadID_Click);
            // 
            // buttonWriteID
            // 
            this.buttonWriteID.Location = new System.Drawing.Point(356, 144);
            this.buttonWriteID.Name = "buttonWriteID";
            this.buttonWriteID.Size = new System.Drawing.Size(104, 30);
            this.buttonWriteID.TabIndex = 16;
            this.buttonWriteID.Text = "写入数据";
            this.buttonWriteID.UseVisualStyleBackColor = true;
            this.buttonWriteID.Click += new System.EventHandler(this.buttonWriteID_Click);
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(11, 115);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(41, 12);
            this.label30.TabIndex = 14;
            this.label30.Tag = "如未知读卡器id，则输入0,";
            this.label30.Text = "回读ID";
            // 
            // textBoxReadData
            // 
            this.textBoxReadData.Location = new System.Drawing.Point(88, 112);
            this.textBoxReadData.Name = "textBoxReadData";
            this.textBoxReadData.Size = new System.Drawing.Size(242, 21);
            this.textBoxReadData.TabIndex = 12;
            // 
            // textBoxWriteData
            // 
            this.textBoxWriteData.Location = new System.Drawing.Point(88, 150);
            this.textBoxWriteData.Name = "textBoxWriteData";
            this.textBoxWriteData.Size = new System.Drawing.Size(242, 21);
            this.textBoxWriteData.TabIndex = 13;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(10, 153);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(41, 12);
            this.label29.TabIndex = 15;
            this.label29.Tag = "如未知读卡器id，则输入0,";
            this.label29.Text = "写入ID";
            // 
            // btnClearEpcs
            // 
            this.btnClearEpcs.Location = new System.Drawing.Point(472, 16);
            this.btnClearEpcs.Name = "btnClearEpcs";
            this.btnClearEpcs.Size = new System.Drawing.Size(104, 30);
            this.btnClearEpcs.TabIndex = 10;
            this.btnClearEpcs.Text = "清空UID列表";
            this.btnClearEpcs.UseVisualStyleBackColor = true;
            this.btnClearEpcs.Click += new System.EventHandler(this.btnClearEpcs_Click);
            // 
            // buttonReadUID
            // 
            this.buttonReadUID.Location = new System.Drawing.Point(362, 16);
            this.buttonReadUID.Name = "buttonReadUID";
            this.buttonReadUID.Size = new System.Drawing.Size(104, 30);
            this.buttonReadUID.TabIndex = 10;
            this.buttonReadUID.Text = "读标签UID";
            this.buttonReadUID.UseVisualStyleBackColor = true;
            this.buttonReadUID.Click += new System.EventHandler(this.buttonReadUID_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(11, 34);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(23, 12);
            this.label14.TabIndex = 11;
            this.label14.Tag = "如未知读卡器id，则输入0,";
            this.label14.Text = "UID";
            // 
            // textBoxUID
            // 
            this.textBoxUID.Location = new System.Drawing.Point(89, 25);
            this.textBoxUID.Name = "textBoxUID";
            this.textBoxUID.Size = new System.Drawing.Size(242, 21);
            this.textBoxUID.TabIndex = 9;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.buttonDisconn);
            this.panel2.Controls.Add(this.buttonConn);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label33);
            this.panel2.Controls.Add(this.textBoxHostIP);
            this.panel2.Controls.Add(this.textBoxIP);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.textBoxPort);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(637, 109);
            this.panel2.TabIndex = 1;
            // 
            // buttonDisconn
            // 
            this.buttonDisconn.Location = new System.Drawing.Point(384, 8);
            this.buttonDisconn.Name = "buttonDisconn";
            this.buttonDisconn.Size = new System.Drawing.Size(97, 30);
            this.buttonDisconn.TabIndex = 20;
            this.buttonDisconn.Text = "断开";
            this.buttonDisconn.UseVisualStyleBackColor = true;
            this.buttonDisconn.Click += new System.EventHandler(this.buttonDisconn_Click);
            // 
            // buttonConn
            // 
            this.buttonConn.Location = new System.Drawing.Point(281, 8);
            this.buttonConn.Name = "buttonConn";
            this.buttonConn.Size = new System.Drawing.Size(97, 30);
            this.buttonConn.TabIndex = 20;
            this.buttonConn.Text = "连接";
            this.buttonConn.UseVisualStyleBackColor = true;
            this.buttonConn.Click += new System.EventHandler(this.buttonConn_Click);
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(9, 16);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(77, 12);
            this.label33.TabIndex = 19;
            this.label33.Text = "读卡器ip地址";
            // 
            // textBoxIP
            // 
            this.textBoxIP.Location = new System.Drawing.Point(103, 10);
            this.textBoxIP.Name = "textBoxIP";
            this.textBoxIP.Size = new System.Drawing.Size(143, 21);
            this.textBoxIP.TabIndex = 18;
            this.textBoxIP.Text = "192.168.1.100";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 11;
            this.label1.Tag = "如未知读卡器id，则输入0,";
            this.label1.Text = "端口";
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(103, 40);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(92, 21);
            this.textBoxPort.TabIndex = 9;
            // 
            // textBoxHostIP
            // 
            this.textBoxHostIP.Location = new System.Drawing.Point(103, 67);
            this.textBoxHostIP.Name = "textBoxHostIP";
            this.textBoxHostIP.Size = new System.Drawing.Size(143, 21);
            this.textBoxHostIP.TabIndex = 18;
            this.textBoxHostIP.Text = "192.168.1.200";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 19;
            this.label2.Text = "电脑IP地址";
            // 
            // RfidWqwlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(643, 378);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "RfidWqwlForm";
            this.Text = "完全物联";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.btnClearEPClist.ResumeLayout(false);
            this.btnClearEPClist.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel btnClearEPClist;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.TextBox textBoxIP;
        private System.Windows.Forms.Button buttonReadUID;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBoxUID;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.TextBox textBoxReadData;
        private System.Windows.Forms.TextBox textBoxWriteData;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Button buttonReadID;
        private System.Windows.Forms.Button buttonWriteID;
        private System.Windows.Forms.Button buttonDisconn;
        private System.Windows.Forms.Button buttonConn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button btnClearEpcs;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxHostIP;
    }
}