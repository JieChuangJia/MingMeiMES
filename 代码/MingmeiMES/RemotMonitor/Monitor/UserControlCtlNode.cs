using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
namespace RemotMonitor
{
    public partial class UserControlCtlNode : UserControl
    {
        #region 私有数据
        private Color bkgColor = Color.Transparent; //背景色
        private Color borderColor = Color.Black;
        private bool bkgFill = false; //是否填充
        private string title = "控制节点";
        private string dispDetail = "详细信息";
        private string popupInfo = "";
        private string timerInfo = "40";//计时信息
      
       // private Size canvasSize = new Size(100, 100);//大小
        private bool popdlgShowed = false;
        #endregion
        #region 属性
        public Color BkgColor { get { return bkgColor; } set { bkgColor = value; } }
        public Color BorderColor { get { return borderColor; } set { borderColor = value; } }
        public bool BkgFill { get { return bkgFill; } set { bkgFill = value; } }
        public string Title { get { return title; } set { title = value; } }
        public string DispDetail { get { return dispDetail; } set { dispDetail = value; } }
        public string TimerInfo { get { return timerInfo; } set { timerInfo = value; } }
        public string DispPopupInfo { get { return popupInfo; } set { popupInfo = value; } }
        #endregion
        public UserControlCtlNode()
        {
            //this.canvasSize = canvasSize;
            InitializeComponent();
            
        }
        public void RefreshDisp()
        {
            this.labelNodename.Text = title;
            this.labelDetail.Text = DispDetail;
            this.Invalidate();
        }
      

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            //this.panel1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
           
        }

        private void panel1_Paint_1(object sender, PaintEventArgs e)
        {
            Graphics curG = e.Graphics;
            BufferedGraphicsContext currentContext = BufferedGraphicsManager.Current;
            currentContext.MaximumBuffer = new Size(this.panel1.Width + 1, this.panel1.Height + 1);
            Rectangle drawClientRect = new Rectangle(new Point(0,0),new Size(this.panel1.Width,this.panel1.Height));
            BufferedGraphics myBuffer = currentContext.Allocate(e.Graphics, e.ClipRectangle);
            Graphics g = myBuffer.Graphics;
            g.PageUnit = GraphicsUnit.Pixel;
            g.SmoothingMode = SmoothingMode.AntiAlias; //SmoothingMode.HighQuality;//
            g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            LinearGradientBrush bkgBrush = new LinearGradientBrush(new Point(drawClientRect.Left, drawClientRect.Top), new Point(drawClientRect.Right, drawClientRect.Bottom), this.bkgColor, Color.WhiteSmoke);
            g.FillRectangle(bkgBrush, drawClientRect);
           
            myBuffer.Render(curG);
            g.Dispose();
            myBuffer.Dispose();//释放资源
        }

        private void panel1_MouseEnter(object sender, EventArgs e)
        {
            OnMouseEnter();
        }
        private void OnMouseEnter()
        {
          
           
        }

        private void UserControlCtlNode_Load(object sender, EventArgs e)
        {
           
          //  popupDlg.Parent = this;
            
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            // MessageBox.Show(this.dispDetail);
            //if (popdlgShowed)
            //{
            //    this.popupDlg.Hide();
            //    this.popdlgShowed = false;
            //}
            //else
            //{
            //    popupDlg.Location = this.PointToScreen(e.Location);
            //    popupDlg.RefreshDisp(this.popupInfo);
            //    this.popupDlg.Show();
            //    this.popdlgShowed = true;
            //}
        }

    }
}
