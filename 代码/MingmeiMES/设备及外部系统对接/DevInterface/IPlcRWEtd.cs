using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevInterface
{
    /// <summary>
    /// 增加32位地址的读写
    /// </summary>
    public interface IPlcRWEtd
    {
        bool ReadMultiDB(string addr, int blockNum, ref int[] vals);

      
        bool WriteMultiDB(string addr, int blockNum, int[] vals);
    }
}
