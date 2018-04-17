using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using GDICommon;
namespace SuperTextEdit
{
    public partial class SuperTextEdit: UserControl,ISuperTextEdit
    {
        ISuperEdit superEditor = new SuperEdit();
        private IGrapTB currSelectedTB = null;
        DrawTool drawTool = new DrawTool();
        private int columnNum = 2;
        public int ColumnNum { get { return this.columnNum; } set { this.columnNum = value;
        this.superEditor.ColumnNum = this.columnNum;
        } }

        public string DataSource { get { return this.superEditor.DataSource; }
            set 
            { 
                this.superEditor.DataSource = value;
                SetScrollSize();
                this.Invalidate();
            }
        }

        public SuperTextEdit()
        {
            InitializeComponent();
           
        }

        public void SetValue(string name, string value)
        {
            this.superEditor.SetValue(name, value,this.CreateGraphics());
            this.Invalidate();//调用刷新函数
     
        }
        /// <summary>
        /// 任务锁定时绘制
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="pos"></param>
        public void DrawSelect(Graphics graphics)
        {
            SelectButton(graphics);
            //Pen linePen = new System.Drawing.Pen(Brushes.SkyBlue, 4);

            //if (this.currSelectedTB.Visible == true)
            //{
            //    this.drawTool.DrawRect(graphics, linePen, this.currSelectedTB.PosRect);
            //}
        }
        public void SelectButton(Graphics graphics)
        {
            Pen linePen = new System.Drawing.Pen(Brushes.SkyBlue, 4);
            GrapTBEditor graptb = this.currSelectedTB as GrapTBEditor;
            if (graptb== null)
            {
                return;
            }
            if (graptb.Visible == true)
            {
                this.drawTool.DrawRect(graphics, linePen, graptb.ButtonArea);
            }
        }
        private void SuperTextEdit_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; //SmoothingMode.HighQuality;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            
            e.Graphics.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);
            superEditor.Draw(e.Graphics);//可以考虑只刷新当前可见区域
            
            if (this.currSelectedTB != null)
            {
                this.DrawSelect(e.Graphics);
            }
            e.Dispose();
        }
        private IGrapTB GetPostionByPt(Point pt)
        {
            IGrapTB pos = null;
            foreach (IGrapTB tb in this.superEditor.TextBoxList)
            {
                if (tb == null)
                {
                    continue;
                }
                 
                 if (tb.PosRect.Contains(pt) == true)
                {
                    pos = tb;
                    break;
                }
            }
            return pos;
        }
        private IGrapTB GetBttonByPt(Point pt)
        {
            IGrapTB pos = null;
            foreach (IGrapTB tb in this.superEditor.TextBoxList)
            {
                if (tb == null)
                {
                    continue;
                }

                if (tb.Style == STYLE.EDITOR)
                {
                    GrapTBEditor tbEditor = tb as GrapTBEditor;
                    if (tbEditor.ButtonArea.Contains(pt) == true)
                    {
                        pos = tb;
                        break;
                    }
                }
            }
            return pos;
        }
        #region 自定义事件
        public delegate void ClickPositionsEventHandler(object sender, ClickPositionsEventArgs e);
        public class ClickPositionsEventArgs : EventArgs
        {
            public IGrapTB GrapTbox { get; set; }
        }
        [Description("货架单元格的鼠标事件:单击鼠标事件"), Category("Mouse")]
        public event ClickPositionsEventHandler PositionsClick;
        #endregion

        private void SuperTextEdit_MouseClick(object sender, MouseEventArgs e)
        {
            this.Invalidate();//重绘一次
            Point pt = new Point(e.X - this.AutoScrollPosition.X, e.Y - this.AutoScrollPosition.Y);
            IGrapTB pos = GetBttonByPt(pt);
            
            if (pos != null)
            {
                if (this.PositionsClick != null)
                {
                    this.currSelectedTB = pos;
                    ClickPositionsEventArgs positionsArgs = new ClickPositionsEventArgs();
                    positionsArgs.GrapTbox = pos;
                    PositionsClick.Invoke(this, positionsArgs);
                }
            }
        }
        /// <summary>
        /// 设置滚动范围根据列数和层数
        /// </summary>
        private void SetScrollSize()
        {
            int widthTemp = this.Size.Width;
            int heightTemp = this.Size.Height;
            if (widthTemp < this.superEditor.GraphSize.Width)
            {
                widthTemp = this.superEditor.GraphSize.Width;
            }

            if (heightTemp < this.superEditor.GraphSize.Height)
            {
                heightTemp = this.superEditor.GraphSize.Height;
            }
            this.AutoScrollMinSize = new Size(widthTemp, heightTemp+20);
        }
       
    }
}
