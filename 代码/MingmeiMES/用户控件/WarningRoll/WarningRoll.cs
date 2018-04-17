using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using GDICommon;
namespace WarningRoll
{
    public partial class WarningRoll: UserControl,IWarnningRoll
    {
        private object lockObj = new object();
        private DrawTool drawTool = new DrawTool();
        private Font drawFont = null;
        private Point drawPostion = new Point();
        private Thread drawThread = null;
        private bool exitThread = false;
        private bool stopThread = false;
        private int threadInterval = 100;
        private Brush fontColor = Brushes.Red;
        private delegate void StrAnimationDelegate();
        private string WarnningStr { get; set; }
        private int fronSize = 15;
        private int speed = 5;
        public WarningRoll()
        {
            InitializeComponent();
        }

        private void WarningRoll_Load(object sender, EventArgs e)
        {
            Init();
        }
        public void SetFontSize(int fontSize)
        {
            this.fronSize = fontSize;
            this.drawFont = new Font("宋体", this.fronSize, FontStyle.Bold);
        }
        /// <summary>
        /// 设置滚动速度1-10
        /// </summary>
        /// <param name="speed"></param>
        public void SetSpeed(int speed)
        {
            if (speed == 0)
            {
                this.speed = 1;
            }
            else
            {
                this.speed = speed;
            }
           
           
        }
        public void SetWarnningStr(string warnStr)
        {
            this.WarnningStr = warnStr;
           
        }

        public void SetFontColor(Brush color)
        {
            this.fontColor = color;
        }
        public void StartRoll()
        {
            this.stopThread = false;
            if(this.drawThread.ThreadState == (ThreadState.Background|ThreadState.Running))
            {
                return;
            }
            this.drawThread.Start();
        }
        public void StopRoll()
        {
            this.stopThread = true;
        }
        private void RefreshData()
        {
            while(!exitThread)
            {
                int speedTemp = this.threadInterval / this.speed;
                if(this.stopThread == true)
                {
                    continue;
                }
                System.Threading.Thread.Sleep(speedTemp);
                StrAnimation();
            }
        }

        private void StrAnimation()
        {
            if (this.InvokeRequired)
            {
                StrAnimationDelegate sad = new StrAnimationDelegate(StrAnimation);
                this.Invoke(sad);
            }
            else
            {
                this.drawPostion.X -= 1;
                SizeF fontSize=this.drawTool.GetStringSize(this.CreateGraphics(), this.WarnningStr, this.drawFont);
                float strLength =fontSize .Width;
                if (this.drawPostion.X <= -strLength)
                {
                    this.drawPostion.X =  this.Size.Width;
                }
                this.drawPostion.Y =(int) (this.Location.Y + (this.Size.Height - fontSize.Height) / 2);
                this.Refresh();
            }
      
        }
        private void Init()
        {
            drawThread = new Thread(RefreshData);
            drawThread.IsBackground = true;

          
            this.drawPostion.X = this.Size.Width;
            this.drawPostion.Y = this.Location.Y / 2;

            this.drawFont = new Font("宋体", this.fronSize, FontStyle.Bold);
        }
        private void WarningRoll_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; //SmoothingMode.HighQuality;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;

            e.Graphics.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);
            //superEditor.Draw(e.Graphics);//可以考虑只刷新当前可见区域
            this.drawTool.DrawStr(e.Graphics, this.drawFont,this.fontColor, this.WarnningStr, this.drawPostion );
        
            e.Dispose();
        }

    }
}
