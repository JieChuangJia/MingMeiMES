using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using DevInterface;
using System.Threading;
namespace DevAccess
{
    public enum EnumTriggerMode
    {
        手动按钮触发,
        程序命令触发
    }
    public class BarcodeRWHonevor:IBarcodeRW
    {
        private const byte CR = 0x0D;
        private const byte SYN = 0x16;
        private const byte T = 0x54;
        private const byte U = 0x55;
        private int readerID = 1;
        private string recvBarcode = "";
        private bool isConnect = false;
        private Thread recvThread;
        private bool recvExit = false;
        private bool pauseFlag = false;
        public int recvInterval = 10;

        private List<byte> saveBuf = new List<byte>();
        private object barcodeBufLock = new object();
        public SerialPort ComPortObj { get; set; }
        public int ReaderID { get { return readerID; } }

        public EnumTriggerMode TriggerMode { get; set; } //触发读码模式
        private List<string> recvBarcodesBuf;
        public BarcodeRWHonevor(int id) 
        {
            this.readerID = id;
            recvExit = false;
          
            recvBarcodesBuf = new List<string>();
            TriggerMode = EnumTriggerMode.程序命令触发;
            ComPortObj = new SerialPort();
            ComPortObj.BaudRate = 115200;
            ComPortObj.DataBits = 8;
            ComPortObj.StopBits = StopBits.One;
            ComPortObj.Parity = Parity.None;
        }
        public List<string> GetBarcodesBuf()
        {
            lock(barcodeBufLock)
            {
                List<string> barcodeBuf = new List<string>();
                if(recvBarcodesBuf.Count()>0)
                {
                    barcodeBuf.AddRange(recvBarcodesBuf.ToArray());
                }
                return barcodeBuf;
               
            }
        }
        public void Remove(string barcode)
        {
            lock(barcodeBufLock)
            {
                if(this.recvBarcodesBuf.Contains(barcode))
                {
                    this.recvBarcodesBuf.Remove(barcode);
                }
            }
        }
        public void ClearBarcodesBuf()
        {
             lock(barcodeBufLock)
             {
                 recvBarcodesBuf.Clear();
             }
        }
        public bool StartMonitor(ref  string reStr)
        {
            try
            {
                if (this.ComPortObj != null)
                {
                    //string[] ports = System.IO.Ports.SerialPort.GetPortNames();
                    //if (!ports.Contains(this.ComPortObj.PortName))
                    //{
                    //    reStr = string.Format("{0} 口不存在", this.ComPortObj.PortName);
                    //    return false;
                    //}
                    if (!this.ComPortObj.IsOpen)
                    {
                        this.ComPortObj.Close();
                    }
                    this.ComPortObj.Open();
                    this.pauseFlag = false;
                    recvExit = false;
                   // if(this.recvThread == null)
                    {
                        recvThread = new Thread(new ThreadStart(ComRecvProc));
                        recvThread.IsBackground = true;
                        recvThread.Priority = ThreadPriority.Highest;
                        recvThread.Name = string.Format("条码枪{0}接收线程", this.readerID);
                    }
                  //  if (this.recvThread.ThreadState == (ThreadState.Background | ThreadState.Unstarted))
                    {
                        recvThread.Start();
                        isConnect = true;
                       // Console.WriteLine("条码枪启动监听线程");
                    }

                    return true;
                }
                else
                {
                    reStr = "对象未创建";
                    return false;
                }
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return false;
            }
           
        }
        public bool StopMonitor()
        {
            try
            {
                recvExit = true;
                Thread.Sleep(300);
                this.pauseFlag = true;
               // Console.WriteLine(recvThread.ThreadState.ToString());
                //if (recvThread != null && recvThread.ThreadState == (ThreadState.Running | ThreadState.Background))
                //{
                //    if (!recvThread.Join(300))
                //    {
                //        recvThread.Abort();

                //    }
                //    recvThread = null;
                //    Console.WriteLine("条码枪接收线程关闭");
                //}
                if (this.ComPortObj.IsOpen)
                {
                    this.ComPortObj.Close();
                    Console.WriteLine(ComPortObj.PortName + "已关闭");
                }
                isConnect = false;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
       
        }
        void SetScanTimeout(int timeOutMax)
        {

        }
      
        public string ReadBarcode()
        {
            if(TriggerMode == EnumTriggerMode.手动按钮触发)
            {
                lock (barcodeBufLock)
                {
                    if (this.recvBarcodesBuf.Count() > 0)
                    {
                        string barcode = this.recvBarcodesBuf[0];
                        this.recvBarcodesBuf.RemoveAt(0);
                        this.recvBarcodesBuf.Clear();
                        return barcode;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
               
            }
            else
            {

                saveBuf.Clear();
                this.recvBarcodesBuf.Clear();
                //先清空上次条码
                recvBarcode = string.Empty;
                byte[] sndBuf = new byte[3] { SYN, T, CR };
                ComPortObj.Write(sndBuf, 0, 3);
               // Console.WriteLine("发送命令:0x16 0x54 0x0d");
                int timeCounter = 0;
                int reTryInterval = 100;
                int timeOut = 2000;
                while (timeCounter < timeOut)
                {
                    if (!string.IsNullOrEmpty(recvBarcode))
                    {
                        this.recvBarcodesBuf.Clear();
                        return recvBarcode;
                    }
                    Thread.Sleep(reTryInterval);

                    timeCounter += reTryInterval;
                }
                
                return string.Empty;
            }
            
        }
        private void ComRecvProc()
        {
            //string strRecv = serialPort.ReadExisting();
         
            byte[] buf = new byte[128];
            while (!recvExit)
            {
                Thread.Sleep(recvInterval);
                if(pauseFlag)
                {
                    continue;
                }
              
                if (!ComPortObj.IsOpen || (!isConnect))
                {
                    continue;
                }
                try
                {
                    int readNum = this.ComPortObj.Read(buf, 0, 128);
                    if (readNum > 0)
                    {

                        for (int i = 0; i < readNum; i++)
                        {
                            // Console.WriteLine(string.Format("Recv:0x{0}",buf[i].ToString("X2")));
                            if (buf[i] == CR)
                            {

                                this.recvBarcode = System.Text.Encoding.UTF8.GetString(saveBuf.ToArray());
                                // if(!this.RecvBarcodesBuf.Exists((string s) => s== this.recvBarcode ? true : false))
                                lock (barcodeBufLock)
                                {
                                    if (!this.recvBarcodesBuf.Contains(this.recvBarcode))
                                    {
                                        this.recvBarcodesBuf.Add(this.recvBarcode);
                                    }
                                }
                                saveBuf.Clear();
                                break;
                            }
                            if (saveBuf.Count() > 1024)
                            {
                                saveBuf.Clear();
                                break;
                            }
                            saveBuf.Add(buf[i]);
                        }
                      
                    }
                }
                catch (Exception ex)
                {
                    ComPortObj.Close();
                    isConnect = false;
                   
                }
                
                
            }
        }
    }
}
