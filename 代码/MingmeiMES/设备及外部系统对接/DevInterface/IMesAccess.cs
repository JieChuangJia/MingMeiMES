using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevInterface
{
    /// <summary>
    /// MES系统对接接口
    /// </summary>
    public interface IMesAccess
    {
        string MesdbConnstr { get; set; }
        bool ConnDB(ref string reStr);
        bool DisconnDB(ref string reStr);
        int MesAssemAuto(string[] paramArray, ref string reStr);
        int MesAssemDown(string[] paramArray, ref string reStr);
       
        /// <summary>
        /// 维修审核是否完成
        /// </summary>
        /// <param name="paramArray"></param>
        /// <param name="reStr"></param>
        /// <returns></returns>
        int MesReAssemEnabled(string[] paramArray, ref string reStr);
        int MesDownEnabled(string mesLinID, string barcode, string downQueryMesID, ref string reStr);
    }
}
