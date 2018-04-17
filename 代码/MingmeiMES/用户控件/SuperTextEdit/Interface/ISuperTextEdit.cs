using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperTextEdit
{
    public interface ISuperTextEdit
    {
        void SetValue(string name, string value);
        string DataSource { get; set; }

        int ColumnNum { get; set; }
    }
}
