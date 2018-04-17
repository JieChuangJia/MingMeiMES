using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace GDICommon
{
    public class DrawTool
    {
        public Pen DrawPen= new Pen(Brushes.Orange, 4);
        public DrawTool()
        { }
        /// <summary>
        /// 在长方框中写字符串
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="rect"></param>
        /// <param name="posPoint"></param>
        /// <param name="drawStr"></param>
        public void DrawStrRect(Graphics graphics, string txt, Rectangle pos)
        {
            Font drawFont = new Font("宋体", 10, FontStyle.Bold);
            SizeF strzSize = graphics.MeasureString(txt, drawFont);
            int pointX = (pos.Width - (int)strzSize.Width) / 2 + pos.Location.X;
            int pointY = (pos.Height - (int)strzSize.Height) / 2 + pos.Location.Y + 2;

            graphics.DrawString(txt, drawFont, Brushes.White, new Point(pointX, pointY));
        }

        public SizeF GetStringSize(Graphics graphics, string str, Font font)
        {

            SizeF strzSize = graphics.MeasureString(str, font);
            return strzSize;
        }
        public void DrawStr(Graphics graphics,string txt,Point location)
        {
            Font drawFont = new Font("宋体", 10, FontStyle.Bold);
           
            graphics.DrawString(txt, drawFont, Brushes.White, location);
        }
        public void DrawStr(Graphics graphics, int frontSize, string txt, Point location)
        {
            Font drawFont = new Font("宋体", frontSize, FontStyle.Bold);

            graphics.DrawString(txt, drawFont, Brushes.Yellow, location);
        }
        public void DrawStr(Graphics graphics, Font font, Brush brush, string txt, Point location)
        {

            graphics.DrawString(txt, font, brush, location);
        }
        /// <summary>
        /// 在长方框中设置颜色
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="rect"></param>
        /// <param name="brush"></param>
        public void FillRect(Graphics graphics, Rectangle rect, Color color)
        {
            SolidBrush brush = new SolidBrush(color);
            graphics.FillRectangle(brush, rect);
        }
        public void DrawRect(Graphics graphics, Pen pen, Rectangle rect)
        {
            graphics.DrawRectangle(pen, rect);
        }
        public void DrawRect(Graphics graphics, Rectangle rect)
        {
            graphics.DrawRectangle(this.DrawPen, rect);
        }
        /// <summary>
        /// 绘制矩形中间带交叉线
        /// </summary>
        /// <param name="pen"></param>
        /// <param name="rect"></param>
        public void DrawRectStyle(Graphics graphics, Pen pen,Rectangle rect)
        {
            graphics.DrawRectangle(pen, rect);

            Pen linePen = new System.Drawing.Pen(Brushes.Silver, 1);
            Point endPoint1 = new Point(rect.Location.X + rect.Width, rect.Location.Y + rect.Height);
            Point startPoint2 = new Point(rect.Location.X + rect.Width, rect.Location.Y);
            Point endPoint2 = new Point(rect.Location.X, rect.Location.Y + rect.Height);

            graphics.DrawLine(linePen, rect.Location, endPoint1);
            graphics.DrawLine(linePen, startPoint2, endPoint2);
            linePen.Dispose();
        }
        /// <summary>
        /// 清空矩形区域
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="pos"></param>
        public void ClearRect(Graphics graphics, Rectangle rect)
        {
            Pen pen = new System.Drawing.Pen(Brushes.Transparent, 4);
            graphics.DrawRectangle(pen, rect);

            Pen linePen = new System.Drawing.Pen(Brushes.Transparent, 1);
            Point endPoint1 = new Point(rect.Location.X + rect.Width, rect.Location.Y + rect.Height);
            Point startPoint2 = new Point(rect.Location.X + rect.Width, rect.Location.Y);
            Point endPoint2 = new Point(rect.Location.X, rect.Location.Y + rect.Height);

            graphics.DrawLine(linePen, rect.Location, endPoint1);
            graphics.DrawLine(linePen, startPoint2, endPoint2);

            linePen.Dispose();

        }

        
    }
}
