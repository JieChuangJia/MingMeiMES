using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevInterface;
namespace DevAccess
{
    public class BarcodeRWSim : IBarcodeRW
    {
        private int readerID = 1;
        public string Barcode = "EA1233";
        public int ReaderID { get { return readerID; } }
        public List<string> RecvBarcodesBuf { get; set; }
        public BarcodeRWSim(int id)
        {
            this.readerID = id;
        }
        public void ClearBarcodesBuf()
        {
            this.RecvBarcodesBuf.Clear();
        }
        public bool StartMonitor(ref  string reStr)
        {
            return true;
        } 
        public List<string> GetBarcodesBuf()
        {
            return RecvBarcodesBuf;
        }
        public bool StopMonitor()
        {
            return true;
        }
        public void SetScanTimeout(int timeOutMax)
        {

        }
        public string ReadBarcode()
        {
          //  Random rand = new Random(1000);
          //  string barCode = "BARCODE2016" + rand.Next().ToString().PadLeft(4);
           // return barCode;
            return Barcode;
        }
    }
}
