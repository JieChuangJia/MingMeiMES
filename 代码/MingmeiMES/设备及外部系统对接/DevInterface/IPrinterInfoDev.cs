using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevInterface
{
    public class PrinterReStat
    {
        public string errCode = "000";
        public string errInfo = "";
    }
    /// <summary>
    /// 贴标机设备接口
    /// </summary>
    public interface IPrinterInfoDev
    {
        bool IsConnect { get; }
        int ReaderID { get; }
        bool SndBarcode(string code,ref string reStr);
        bool Connect(ref string reStr);
        bool Disconnect(ref string reStr);
    }
}
