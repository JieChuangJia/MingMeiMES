using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevInterface
{
    public class AirlossDetectModel
    {
        public int SeriNO { get; set; }
        public int GroupNO { get; set; }
        public int DLY { get; set; }
        public int CHG { get; set; }
        public int BAL { get; set; }
        public int DET { get; set; }
        public int EXH { get; set; } 
        public float DetectVal { get; set; } //检测值
        public string UnitDesc { get; set; } //单位描述
        public string DetectResult { get; set; } //检测结果
    }
    /// <summary>
    /// 气密检测仪接口
    /// </summary>
    public interface IAirlossDetectDev
    {
        int ReaderID { get; }
        bool StartDetect(ref string reStr);
        AirlossDetectModel QueryResultData(ref string reStr);
        bool StartMonitor(ref string reStr);
    }
    
}
