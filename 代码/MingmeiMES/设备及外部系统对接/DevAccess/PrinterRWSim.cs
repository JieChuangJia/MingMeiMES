using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevInterface;
namespace DevAccess
{
    public class PrinterRWSim:IPrinterInfoDev
    {
        public bool IsConnect { get { return true; } }
        private int readerID = 1;
        public int ReaderID { get { return readerID; } }
        public PrinterRWSim(int id)
        {
            this.readerID = id;
        }
        public bool SndBarcode(string code,ref string reStr)
        {
            reStr = "OK";
            return true;
        }
        public bool Connect(ref string reStr) { return true; }
        public bool Disconnect(ref string reStr) { return true; }
    }
}
