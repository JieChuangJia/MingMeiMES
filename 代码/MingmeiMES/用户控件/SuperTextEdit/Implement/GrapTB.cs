
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDICommon;

namespace SuperTextEdit
{
    public class GrapTB:BaseTB
    {
        private DrawTool drawTool = new DrawTool();
        public GrapTB()
        { }
        public override void Draw(Graphics graphics)
        {
            base.Draw(graphics);
            drawTool.DrawRect(graphics, this.PosRect);
            drawTool.FillRect(graphics, this.PosRect, this.BackColor);
            drawTool.DrawStrRect(graphics, this.Desc + ":" + this.Name , this.PosRect);
             
        }
    }
}
