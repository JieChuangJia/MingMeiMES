using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLProcessModel
{
    public enum EnumAddrDataType
    {
        DINT,
        INT,
        REAL,
        UINT
    }
    public class DevCfgItemModel
    {
        public string PlcAddr { get; set; }
        public EnumAddrDataType AddrDataType { get; set; }
      
        public string Desc { get; set; }
        public DevCfgItemModel()
        { }
    }
}
