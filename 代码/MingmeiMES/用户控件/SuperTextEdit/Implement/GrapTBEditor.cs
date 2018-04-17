using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDICommon;

namespace SuperTextEdit
{
    public class GrapTBEditor : BaseTB
    {
        private string buttonTxt = "修改";
        int buttonwidth = 40;
        public string ButtonTxt { get { return this.buttonTxt; } set { this.buttonTxt = value; } }
        public Color ButtonColor = Color.DimGray;
        private Rectangle buttonArea = new Rectangle();
        private Rectangle textArea = new Rectangle();

        private DrawTool drawTool = new DrawTool();
        public override void Draw(Graphics graphics)
        {
            base.Draw(graphics);
            drawTool.DrawRect(graphics, this.PosRect);
            drawTool.FillRect(graphics, this.PosRect, this.BackColor);

            drawTool.DrawStrRect(graphics, this.Desc + ":" + this.Name + ":" + this.Value, this.TextArea);

          
            drawTool.DrawRect(graphics, this.ButtonArea);
            drawTool.FillRect(graphics, this.ButtonArea, this.ButtonColor);
            drawTool.DrawStrRect(graphics, "修改", this.ButtonArea);
            
        }
        public Rectangle ButtonArea 
        {
            get 
            {
                int x = this.PosRect.Location.X + (this.PosRect.Width - buttonwidth);
                int y = this.PosRect.Location.Y;
                buttonArea.Location = new Point(x, y);
                buttonArea.Width = buttonwidth;
                buttonArea.Height = this.PosRect.Height;
                return buttonArea;
            }
        }

        public Rectangle TextArea
        {
            get 
            {
                int x = this.PosRect.Location.X ;
                int y = this.PosRect.Location.Y;
                textArea.Location = new Point(x, y);
                textArea.Width = this.PosRect.Width-buttonwidth;
                textArea.Height = this.PosRect.Height;
                return this.textArea;
            }
        }
        public GrapTBEditor()
        { 
        
        }

    }
}
