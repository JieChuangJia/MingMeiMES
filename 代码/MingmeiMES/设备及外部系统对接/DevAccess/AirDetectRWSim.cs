using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevInterface;
namespace DevAccess
{
    public class AirDetectRWSim:IAirlossDetectDev
    {
        private int readerID = 1;
        public int ReaderID { get { return readerID; } }
        public string DetectRe = "OK";
        public AirDetectRWSim(int id)
        {
            readerID = id;
        }
        public bool StartDetect(ref string reStr)
        {
            return true;
        }
        public AirlossDetectModel QueryResultData(ref string reStr)
        {
            AirlossDetectModel model = new AirlossDetectModel();
            model.DetectResult = DetectRe;
            return model;
        }
        public bool StartMonitor(ref string reStr)
        {
            return true;
        }
    }
}
