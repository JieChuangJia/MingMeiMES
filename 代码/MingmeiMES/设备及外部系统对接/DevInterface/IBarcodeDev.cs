using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevInterface
{
    /// <summary>
    /// 条码读取设备接口
    /// </summary>
    public interface IBarcodeRW
    {
        int ReaderID { get; }
      //  List<string> RecvBarcodesBuf { get; set; }
        List<string> GetBarcodesBuf();
        void ClearBarcodesBuf();
       // void SetScanTimeout(int timeOutMax); //允许最大延迟，单位:秒
        bool StartMonitor(ref  string reStr);
        bool StopMonitor();
        string ReadBarcode();

    }
}
