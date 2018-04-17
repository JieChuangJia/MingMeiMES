namespace CreateKey
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.bt_CreateKey = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tb_KeyTxt = new System.Windows.Forms.TextBox();
            this.bt_Cancel = new System.Windows.Forms.Button();
            this.bt_createLicenceFile = new System.Windows.Forms.Button();
            this.bt_readEndDate = new System.Windows.Forms.Button();
            this.textBoxTime = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(104, 8);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(155, 21);
            this.dateTimePicker1.TabIndex = 0;
            // 
            // bt_CreateKey
            // 
            this.bt_CreateKey.Location = new System.Drawing.Point(284, 143);
            this.bt_CreateKey.Name = "bt_CreateKey";
            this.bt_CreateKey.Size = new System.Drawing.Size(75, 20);
            this.bt_CreateKey.TabIndex = 1;
            this.bt_CreateKey.Text = "生成激活码";
            this.bt_CreateKey.UseVisualStyleBackColor = true;
            this.bt_CreateKey.Click += new System.EventHandler(this.bt_CreateKey_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "授权截止日期：";
            // 
            // tb_KeyTxt
            // 
            this.tb_KeyTxt.Location = new System.Drawing.Point(8, 34);
            this.tb_KeyTxt.Multiline = true;
            this.tb_KeyTxt.Name = "tb_KeyTxt";
            this.tb_KeyTxt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tb_KeyTxt.Size = new System.Drawing.Size(467, 99);
            this.tb_KeyTxt.TabIndex = 3;
            // 
            // bt_Cancel
            // 
            this.bt_Cancel.Location = new System.Drawing.Point(368, 142);
            this.bt_Cancel.Name = "bt_Cancel";
            this.bt_Cancel.Size = new System.Drawing.Size(75, 23);
            this.bt_Cancel.TabIndex = 4;
            this.bt_Cancel.Text = "取消";
            this.bt_Cancel.UseVisualStyleBackColor = true;
            this.bt_Cancel.Click += new System.EventHandler(this.bt_Cancel_Click);
            // 
            // bt_createLicenceFile
            // 
            this.bt_createLicenceFile.Location = new System.Drawing.Point(159, 142);
            this.bt_createLicenceFile.Name = "bt_createLicenceFile";
            this.bt_createLicenceFile.Size = new System.Drawing.Size(116, 23);
            this.bt_createLicenceFile.TabIndex = 5;
            this.bt_createLicenceFile.Text = "生成License文件";
            this.bt_createLicenceFile.UseVisualStyleBackColor = true;
            this.bt_createLicenceFile.Click += new System.EventHandler(this.bt_createLicenceFile_Click);
            // 
            // bt_readEndDate
            // 
            this.bt_readEndDate.Location = new System.Drawing.Point(34, 142);
            this.bt_readEndDate.Name = "bt_readEndDate";
            this.bt_readEndDate.Size = new System.Drawing.Size(116, 23);
            this.bt_readEndDate.TabIndex = 6;
            this.bt_readEndDate.Text = "读license文件";
            this.bt_readEndDate.UseVisualStyleBackColor = true;
            this.bt_readEndDate.Click += new System.EventHandler(this.bt_readEndDate_Click);
            // 
            // textBoxTime
            // 
            this.textBoxTime.Location = new System.Drawing.Point(328, 7);
            this.textBoxTime.Name = "textBoxTime";
            this.textBoxTime.Size = new System.Drawing.Size(148, 21);
            this.textBoxTime.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(265, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "HH(24):MM";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(487, 171);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxTime);
            this.Controls.Add(this.bt_readEndDate);
            this.Controls.Add(this.bt_createLicenceFile);
            this.Controls.Add(this.bt_Cancel);
            this.Controls.Add(this.tb_KeyTxt);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bt_CreateKey);
            this.Controls.Add(this.dateTimePicker1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "生成激活码";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Button bt_CreateKey;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_KeyTxt;
        private System.Windows.Forms.Button bt_Cancel;
        private System.Windows.Forms.Button bt_createLicenceFile;
        private System.Windows.Forms.Button bt_readEndDate;
        private System.Windows.Forms.TextBox textBoxTime;
        private System.Windows.Forms.Label label2;
    }
}

