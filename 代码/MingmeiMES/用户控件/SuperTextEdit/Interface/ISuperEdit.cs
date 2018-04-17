using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperTextEdit
{
    public interface ISuperEdit
    {
        string Name { get; set; }
        int RowInterval { get; set; }
        int ColumnInterval { get; set; }

        int ColumnNum { get; set; }

        int RowNum { get; set; }
      
        List<IGrapTB> TextBoxList { get; set; }

        Point StartPos { get; set; }

        Size GraphSize { get; set; }
      
        /// <summary>
        /// xml数据，包括Textbox的数据
        /// </summary>
        string DataSource { get; set; }
        void Draw(Graphics graphics);
        void SetValue(string name, string value, Graphics graphics);


    }
}
