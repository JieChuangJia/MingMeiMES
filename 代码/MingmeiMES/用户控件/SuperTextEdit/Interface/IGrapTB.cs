using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperTextEdit
{
    public enum DATATYPE
    {
        INT,
        DINT,
        REAL,
        UINT
    }
    public enum STYLE
    {
        EDITOR,
        DEFAULT
    }
    public interface IGrapTB
    {
        string Name { get; set; }
        Rectangle PosRect { get; set; }

        Color BackColor { get; set; }
        string Desc { get; set; }
        Point Location { get; set; }
        string Value { get; set; }
        STYLE Style { get; set; }
        DATATYPE ValueDataType { get; set; }

        int Index { get; set; }

        int Colth { get; set; }
        int Rowth { get; set; }
        Size Size { get; set; }
        bool Visible { get; set; }

        void Draw(Graphics graphics);
        void SetValue(string value, Graphics graphics);

    }
}
