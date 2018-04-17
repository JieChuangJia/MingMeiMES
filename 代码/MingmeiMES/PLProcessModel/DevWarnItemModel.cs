using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLProcessModel
{
    public class DevWarnItemModel
    {
        public string PlcAddr { get; set; }
        public int WarnStat { get; set; }
        public string WarnInfo { get; set; }
        public DateTime RecordTime { get; set; }
    }
}
