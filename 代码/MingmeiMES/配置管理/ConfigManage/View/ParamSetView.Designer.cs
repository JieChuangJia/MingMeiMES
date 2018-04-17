namespace ConfigManage
{
    partial class ParamSetView
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.nud_ParamValue = new System.Windows.Forms.NumericUpDown();
            this.lb_ParamName = new System.Windows.Forms.Label();
            this.bt_ParamSet = new System.Windows.Forms.Button();
            this.bt_ParamCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_ParamValue)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.nud_ParamValue);
            this.groupBox1.Controls.Add(this.lb_ParamName);
            this.groupBox1.Location = new System.Drawing.Point(4, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(329, 100);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "参数";
            // 
            // nud_ParamValue
            // 
            this.nud_ParamValue.Location = new System.Drawing.Point(79, 47);
            this.nud_ParamValue.Maximum = new decimal(new int[] {
            1215752191,
            23,
            0,
            0});
            this.nud_ParamValue.Name = "nud_ParamValue";
            this.nud_ParamValue.Size = new System.Drawing.Size(184, 21);
            this.nud_ParamValue.TabIndex = 2;
            // 
            // lb_ParamName
            // 
            this.lb_ParamName.AutoSize = true;
            this.lb_ParamName.Location = new System.Drawing.Point(21, 49);
            this.lb_ParamName.Name = "lb_ParamName";
            this.lb_ParamName.Size = new System.Drawing.Size(29, 12);
            this.lb_ParamName.TabIndex = 0;
            this.lb_ParamName.Text = "参数";
            // 
            // bt_ParamSet
            // 
            this.bt_ParamSet.Location = new System.Drawing.Point(175, 118);
            this.bt_ParamSet.Name = "bt_ParamSet";
            this.bt_ParamSet.Size = new System.Drawing.Size(75, 23);
            this.bt_ParamSet.TabIndex = 1;
            this.bt_ParamSet.Text = "设置";
            this.bt_ParamSet.UseVisualStyleBackColor = true;
            this.bt_ParamSet.Click += new System.EventHandler(this.bt_ParamSet_Click);
            // 
            // bt_ParamCancel
            // 
            this.bt_ParamCancel.Location = new System.Drawing.Point(256, 118);
            this.bt_ParamCancel.Name = "bt_ParamCancel";
            this.bt_ParamCancel.Size = new System.Drawing.Size(75, 23);
            this.bt_ParamCancel.TabIndex = 2;
            this.bt_ParamCancel.Text = "关闭";
            this.bt_ParamCancel.UseVisualStyleBackColor = true;
            this.bt_ParamCancel.Click += new System.EventHandler(this.bt_ParamCancel_Click);
            // 
            // ParamSetView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(339, 153);
            this.Controls.Add(this.bt_ParamCancel);
            this.Controls.Add(this.bt_ParamSet);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ParamSetView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "参数设置";
            this.Load += new System.EventHandler(this.ParamSetView_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_ParamValue)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button bt_ParamSet;
        private System.Windows.Forms.Button bt_ParamCancel;
        private System.Windows.Forms.NumericUpDown nud_ParamValue;
        private System.Windows.Forms.Label lb_ParamName;
    }
}