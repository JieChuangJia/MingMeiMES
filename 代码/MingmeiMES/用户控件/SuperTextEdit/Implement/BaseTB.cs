using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperTextEdit
{
    public class BaseTB : IGrapTB
    {
        /// <summary>
        /// 控件名称做成唯一
        /// </summary>
        private string name = "";
        public string Name { get { return this.name; } set { this.name = value; } }
        public string Desc { get; set; }
        /// <summary>
        /// 单元格框
        /// </summary>
        private Rectangle posRect;
        public Rectangle PosRect
        {
            get { return this.posRect; }
            set { this.posRect = value; }
        }
        private int index=0;   
        public int Index 
        {
            get{return this.index;}
            set{this.index=value;}
        }
        private bool visible = true;
        public bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }
        public int Colth { get; set; }
        private Color backColor = Color.SteelBlue;
        public Color BackColor 
        {
            get { return this.backColor; }
            set { this.backColor = value; }
        }
        public int Rowth { get; set; }
        private Point location = new Point();
        public Point Location { get { return this.location; } set { this.location = value; } }
        public string Value { get; set; }
        private DATATYPE valueDataType = DATATYPE.INT;
        public DATATYPE ValueDataType { get { return this.valueDataType; } set { this.valueDataType = value; } }
        private Size size = new Size(320, 25);
        public Size Size { get { return this.size; } set { this.size = value; } }
        private STYLE style = STYLE.DEFAULT;
        public STYLE Style
        {
            get { return this.style; }
            set
            {
                this.style = value;
            }
        }

        public void SetValue(string value,Graphics graphics)
        {
            this.Value = value;
            //this.Draw(graphics);
            
        }
        public virtual void Draw(Graphics graphics)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias; //SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;

          
        }
        public BaseTB()
        { }
    }
}
