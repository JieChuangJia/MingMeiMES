using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevInterface
{
    /// <summary>
    /// 杭可系统对接接口
    /// </summary>
    public interface IHKAccess
    {
        int HkAccessID { get; set; }
        bool Conn(ref string reStr);
        bool Disconn(ref string reStr);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stationID">stationID装载工委序号，0:1次装载，1:2次装载</param>
        /// <param name="palletID"></param>
        /// <param name="batteryIDs"></param>
        /// <param name="reStr"></param>
        /// <returns></returns>
        bool BatteryFill(int stationID, string palletID, List<string> batteryIDs,ref string sndStr,ref string reStr);
    }
}
