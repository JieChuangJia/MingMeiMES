using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDICommon;
using System.Xml;
using System.Drawing;

namespace SuperTextEdit
{
    public class SuperEdit : ISuperEdit
    {

        public string Name { get; set; }
        private int rowInterval = 15;
        private Point startPos = new Point(20, 20);
        DrawTool drawTool = new DrawTool();
       
        public Point StartPos
        {
            get { return this.startPos; }
            set { this.startPos = value; }
        }
        private int columnNum = 2;
        public int ColumnNum
        {
            get { return this.columnNum; }
            set { this.columnNum = value; }
        }
        public int RowNum
        { get; set; }

        public Size GraphSize { get; set; }
        public int RowInterval { get { return this.rowInterval; } set { this.rowInterval = value; } }
        private int columnInterval = 15;
        public int ColumnInterval { get { return this.columnInterval; } set { this.columnInterval = value; } }
        private List<IGrapTB> textboxList = new List<IGrapTB>();
        public List<IGrapTB> TextBoxList { get { return this.textboxList; } set { this.textboxList = value; } }
        /// <summary>
        /// xml数据，包括Textbox的数据
        /// </summary>
        private string dataSource = "";
        public string DataSource
        {
            get { return this.dataSource; }
            set
            {
                this.dataSource = value;
                this.textboxList.Clear();
               
             
                AnalysisData();
                if (this.textboxList != null && this.textboxList.Count > 0)
                {
                    double rowthTemp = (this.textboxList.Count) / this.columnNum;
                    this.RowNum = (int)Math.Ceiling(rowthTemp);
                    int graWidth = this.ColumnNum * this.textboxList[0].Size.Width + (this.ColumnNum - 1) * this.columnInterval + startPos.X+20;
                    int graHight = this.RowNum * (this.textboxList[0].Size.Height + this.rowInterval) + startPos.Y+20;

                    this.GraphSize = new Size(graWidth, graHight);
                }

            }
        }

        private XMLOperater xmlOperater = new XMLOperater();

      
        public SuperEdit()
        {
           
        }
       
        private bool IsExistNameInEditor(string name)
        {

            if (this.textboxList == null)
            {
                return false;
            }
            for (int i = 0; i < this.textboxList.Count; i++)
            {
                IGrapTB grapTb = this.textboxList[i];
                if (grapTb == null)
                {
                    return false;
                }
                if (grapTb.Name == name)
                {
                    return true;
                }
            }
            return false;
        }
        private bool CreateTbox(string name, DATATYPE type, string value, STYLE style, string desc, int index, ref IGrapTB grapTb)
        {
            if (IsExistNameInEditor(name) == true)
            {
                return false;
            }
            if (style == STYLE.EDITOR)
            {
                grapTb = new GrapTBEditor();
            }
            else if (style == STYLE.DEFAULT)
            {
                grapTb = new GrapTB();
            }
            else
            {
                return false;
            }

            grapTb.Name = name;
            grapTb.ValueDataType = type;
            grapTb.Value = value;
            grapTb.Style = style;
            grapTb.Index = index;
            grapTb.Desc = desc;
            double rowthTemp = (1.0 +index)/ this.columnNum;
            
            grapTb.Rowth = (int)Math.Ceiling(rowthTemp);
            int colthTemp = (index + 1) % this.columnNum;
            if (colthTemp == 0)
            {
                grapTb.Colth = this.columnNum;
            }
            else
            {
                grapTb.Colth = colthTemp;
            }
            int x = startPos.X + grapTb.Size.Width * (grapTb.Colth-1) + (grapTb.Colth - 1) * this.columnInterval;
            int y = startPos.Y + grapTb.Size.Height *( grapTb.Rowth -1)+ (grapTb.Rowth - 1) * this.rowInterval;
            grapTb.Location = new Point(x, y);
            grapTb.PosRect = new Rectangle(grapTb.Location, grapTb.Size);

            return true;
        }

        private bool AnalysisData()
        {
            if (string.IsNullOrEmpty(this.DataSource))
            {
                return false;
            }
            if (xmlOperater.LoadXmlContent(this.DataSource) == false)
            {
                return false;
            }
            XmlNodeList nodeList = xmlOperater.GetNodesByName("Item");
            if (nodeList == null)
            {
                return false;
            }
            for (int i = 0; i < nodeList.Count; i++)
            {
                XmlNode node = nodeList[i];
                if (node == null)
                {
                    continue;
                }
                XmlAttribute nameAttr = xmlOperater.GetProperCollection(node, "name");
                if (nameAttr == null)
                {
                    continue;
                }
                XmlAttribute typeAttr = xmlOperater.GetProperCollection(node, "type");
                if (typeAttr == null)
                {
                    continue;
                }
                XmlAttribute descAttr = xmlOperater.GetProperCollection(node, "desc");
                if (descAttr == null)
                {
                    continue;
                }
                XmlAttribute styleAttr = xmlOperater.GetProperCollection(node, "style");
                if (styleAttr == null)
                {
                    continue;
                }
 
                string name = nameAttr.Value;
                DATATYPE type = (DATATYPE)Enum.Parse(typeof(DATATYPE), typeAttr.Value);
                string desc = descAttr.Value;
                STYLE style = (STYLE)Enum.Parse(typeof(STYLE), styleAttr.Value);
                IGrapTB grapTb = null;
                if (CreateTbox(name, type, node.InnerXml, style, desc, i, ref grapTb) == false)
                {
                    continue;
                }
                this.textboxList.Add(grapTb);
            }
            return true;
        }

        private bool GetGrapTB(string name ,ref IGrapTB iGrapTb)
        {
          
            if (this.textboxList == null)
            {
                return false;
            }
            for (int i = 0; i < this.textboxList.Count; i++)
            {
                if(textboxList[i].Name == name)
                {
                    iGrapTb = textboxList[i];
                    break;
                }
            }
            return true;
        }

        public void SetValue(string name, string value, Graphics graphics)
        {
            IGrapTB grapTb = null;
            if (GetGrapTB(name, ref grapTb) == false)
            {
                return;
            }
            grapTb.SetValue(value, graphics);
        }
        public void Draw(Graphics graphics)
        {
            
            foreach (IGrapTB tb in this.textboxList)//排
            {
                if (tb.Visible == true)
                {
                    tb.Draw(graphics);
                }
                else
                {
                    drawTool.ClearRect(graphics, tb.PosRect);
                }
            }
        }
    }
}
