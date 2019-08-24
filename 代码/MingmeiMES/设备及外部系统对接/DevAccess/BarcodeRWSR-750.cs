using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevInterface;
using System.Threading;

namespace DevAccess
{
    public class SR_750Protocol
    {
        private const string READCMD = "LON";   //LON  cmd
        private const string STOPREADCMD = "LOFF"; //LOFF cmd
        private const string TAILCODE = "0D";
        private const string ERROR = "ERROR";
        private const int PROLENTH = 24; // 条码长度
        public SR_750Protocol()
        {

        }

        public bool ReadCodeCmd(ref byte[] cmdBytes)
        {
            string readCmdHexStr = DataConvert.AcciiStringToHexStr(READCMD);
            readCmdHexStr += TAILCODE;
            cmdBytes = DataConvert.StrToToHexByte(readCmdHexStr);
            return true;
        }

        public bool ReadCodeCmdReponse(byte[] recBytes, ref string barcode, ref string reStr)
        {

            barcode = DataConvert.AsciiBytesToHex(recBytes).Trim();

            //if (barcode.Length < PROLENTH)
            //{
            //    reStr = "接收数据协议长度错误！";
            //    return false;
            //}
            barcode = barcode.Substring(0, PROLENTH);
            return true;
        }
        public bool StopReadCodeCmd(ref byte[] cmdBytes)
        {
            string stopReadHexStr = DataConvert.AcciiStringToHexStr(STOPREADCMD);
            stopReadHexStr += TAILCODE;
            cmdBytes = DataConvert.StrToToHexByte(stopReadHexStr);
            return true;
        }
        public bool StopReadCodeCmdResponse(byte[] recBytes,ref string reStr)
        {

            string recStr = DataConvert.AsciiBytesToHex(recBytes).ToUpper();
            if(recStr != ERROR)
            {
                return false;
            }
            return true;
        }


    }
    public class BarcodeRWSR_750:IBarcodeRW
    {
        private ISocket mySocket = null;
        private SR_750Protocol sr750Dev = null;
        private int readerID = 1;
        private  int TIMEWAITOUT = 5000;//5秒
        private const int WAITTIME = 50;
        private const int MAXREWORKNUM = 5;//最大操作次数
        private string DevIP { get; set; }
        private int DevPort { get; set; }
        public string Barcode = "EA1233";
        public int ReaderID { get { return readerID; } }
        private List<string> recvBarcodesBuf = new List<string>();
        private List<byte> recBuffer = new List<byte>();
        private AutoResetEvent recvAutoEvent = new AutoResetEvent(false);
        private  Action<string> printLog = null;
        private object lockObj = new object();
        public List<string> RecvBarcodesBuf { get { return this.recvBarcodesBuf; } set { this.recvBarcodesBuf = value; } }
        public BarcodeRWSR_750(int id, string devIP, int devPort, Action<string> addLog)//端口默认为9004,为tcp通讯
        {
            this.readerID = id;
            this.mySocket = new STcpClient();
            this.DevIP = devIP;
            this.DevPort = devPort;
            this.printLog= addLog;
            sr750Dev = new SR_750Protocol();
        }
        public void ClearBarcodesBuf()
        {
            this.RecvBarcodesBuf.Clear();
        }
        public bool StartMonitor(ref  string reStr)
        {
            if(this.mySocket!= null)
            {
                this.mySocket.Disconnect();
            }
          
            this.mySocket = new STcpClient();
            this.mySocket.ReceiveCompleted += ReciveEventHandler;
            this.mySocket.LostLink += LostLinkEventHandler;
            return mySocket.Connect(this.DevIP, this.DevPort, ref reStr);
        } 
        public List<string> GetBarcodesBuf()
        {
            return RecvBarcodesBuf;
        }
        public bool StopMonitor()
        {
            if (this.mySocket != null)
            {
                this.mySocket.Disconnect();
            }
            return true;
            //lock (this.lockObj)
            //{
            //    string reStr = "";
            //    int reworkNum = 0;
            //    this.recBuffer.Clear();
            //    byte[] stopCodeCmd = null;
            //    bool stopCodeCmdStatus = sr750Dev.StopReadCodeCmd(ref stopCodeCmd);
            //    if (stopCodeCmdStatus == false)
            //    {
            //        return false ;
            //    }
            //    if (this.mySocket.Send(stopCodeCmd, ref reStr) == false)
            //    {
            //        ShowLog("发送读取条码命令失败！");
            //    }
            //    ShowLog("发送读取条码命令：" + DataConvert.ByteToHexStr(stopCodeCmd));
            //    this.recvAutoEvent.Reset();
            //    if (this.recvAutoEvent.WaitOne(TIMEWAITOUT) == true)
            //    {
            //        while (reworkNum < MAXREWORKNUM)
            //        {
            //            if (this.sr750Dev.StopReadCodeCmdResponse(this.recBuffer.ToArray(), ref reStr) == false)
            //            {
            //                reworkNum++;
            //                ShowLog("接收停止数据数据：" + DataConvert.AsciiBytesToHex(this.recBuffer.ToArray()));
            //                System.Threading.Thread.Sleep(WAITTIME);
            //            }
            //            else
            //            {
            //                break;
            //            }
            //        }
            //        if (reworkNum >= MAXREWORKNUM)
            //        {
            //            ShowLog("接收停止数据数据错误！");
            //            return false;
            //        }

                    
            //    }
            //    else
            //    {
            //        reStr = "接收条码数据超时！";
            //        if (this.mySocket != null)
            //        {
            //            this.mySocket.Disconnect();
            //        }
            //        return false;
            //    }

              
            //}      
        }
        public void SetScanTimeout(int timeOutMax)
        {
            this.TIMEWAITOUT = timeOutMax;
        }
        public string ReadBarcode()
        {
            lock (this.lockObj)
            {
                try
                {
                    string reStr = "";
                    string barCode = "";
                    int reworkNum = 0;
                    this.recBuffer.Clear();
                    byte[] readCodeCmd = null;
                    bool readCodeCmdStatus = sr750Dev.ReadCodeCmd(ref readCodeCmd);
                    if (readCodeCmdStatus == false)
                    {
                        return string.Empty;
                    }
                    this.recvAutoEvent.Reset();
                    if (this.mySocket.Send(readCodeCmd, ref reStr) == false)
                    {
                        ShowLog("发送读取条码命令失败！");
                    }
                    ShowLog("发送读取条码命令：" + DataConvert.ByteToHexStr(readCodeCmd));
                   
                    if (this.recvAutoEvent.WaitOne(TIMEWAITOUT) == true)
                    {
                        while (reworkNum < MAXREWORKNUM)
                        {
                            if (this.sr750Dev.ReadCodeCmdReponse(this.recBuffer.ToArray(), ref barCode, ref reStr) == false)
                            {
                                reworkNum++;
                                ShowLog("接收条码数据数据：" + DataConvert.AsciiBytesToHex(this.recBuffer.ToArray()));
                                System.Threading.Thread.Sleep(WAITTIME);
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (reworkNum >= MAXREWORKNUM)
                        {
                            ShowLog("接收条码数据数据错误！");
                            return string.Empty;
                        }
                        CloseScan();
                        return barCode;
                    }
                    else
                    {
                        CloseScan();
                        reStr = "接收条码数据超时！";
                        return string.Empty;
                    }
                }
                catch(Exception ex)
                {
                    CloseScan();
                    Console.WriteLine(ex.StackTrace.ToString());
                    return string.Empty;
                }
            }
        }
        private void CloseScan()
        {
            string reStr = "";
            //关闭扫码
            byte[] stopCodeCmd = null;
            bool stopCodeCmdStatus = sr750Dev.StopReadCodeCmd(ref stopCodeCmd);
            if (stopCodeCmdStatus == false)
            {
                ShowLog("获取关闭条码指令失败！");
            }
            if (this.mySocket.Send(stopCodeCmd, ref reStr) == false)
            {
                ShowLog("发送关闭条码指令失败！");
            }
        }
        private void ReciveEventHandler(object sender, SocketEventArgs e)
        {
            this.recBuffer.AddRange(e.RecBytes);
            this.recvAutoEvent.Set();
        }
        private void ShowLog(string logStr)
        {
            if(this.printLog!= null)
            {
                this.printLog(logStr);
            }
        }
        private void LostLinkEventHandler(object sender, LostLinkEventArgs e)
        {
            string str = "";
            this.ShowLog("网络断开!");
            //if( this.ConnectPLC(ref str)==false)
            //{
            //    OnLog("重新连接失败！");
            //}
            //else
            //{
            //    OnLog("重新连接成功！");
            //}
        }
         
    }
}
