namespace RemotMonitor
{
    partial class UserControlCtlNode
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new MyPanel();
            this.labelTimer = new System.Windows.Forms.Label();
            this.labelDetail = new System.Windows.Forms.Label();
            this.labelNodename = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.labelTimer);
            this.panel1.Controls.Add(this.labelDetail);
            this.panel1.Controls.Add(this.labelNodename);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(259, 266);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint_1);
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            // 
            // labelTimer
            // 
            this.labelTimer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTimer.BackColor = System.Drawing.Color.Transparent;
            this.labelTimer.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelTimer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.labelTimer.Location = new System.Drawing.Point(160, 15);
            this.labelTimer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTimer.Name = "labelTimer";
            this.labelTimer.Size = new System.Drawing.Size(88, 28);
            this.labelTimer.TabIndex = 0;
            this.labelTimer.Text = "0";
            this.labelTimer.Visible = false;
            // 
            // labelDetail
            // 
            this.labelDetail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDetail.BackColor = System.Drawing.Color.Transparent;
            this.labelDetail.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelDetail.Location = new System.Drawing.Point(8, 125);
            this.labelDetail.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelDetail.Name = "labelDetail";
            this.labelDetail.Size = new System.Drawing.Size(240, 31);
            this.labelDetail.TabIndex = 0;
            this.labelDetail.Text = "状态描述";
            // 
            // labelNodename
            // 
            this.labelNodename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelNodename.BackColor = System.Drawing.Color.Transparent;
            this.labelNodename.Font = new System.Drawing.Font("黑体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelNodename.ForeColor = System.Drawing.Color.Black;
            this.labelNodename.Location = new System.Drawing.Point(-3, 239);
            this.labelNodename.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelNodename.Name = "labelNodename";
            this.labelNodename.Size = new System.Drawing.Size(251, 28);
            this.labelNodename.TabIndex = 0;
            this.labelNodename.Text = "节点名称";
            // 
            // UserControlCtlNode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "UserControlCtlNode";
            this.Size = new System.Drawing.Size(259, 266);
            this.Load += new System.EventHandler(this.UserControlCtlNode_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MyPanel panel1;
        private System.Windows.Forms.Label labelNodename;
        private System.Windows.Forms.Label labelDetail;
        private System.Windows.Forms.Label labelTimer;
    }
}
