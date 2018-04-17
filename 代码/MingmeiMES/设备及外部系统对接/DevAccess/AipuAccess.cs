using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevInterface;
using System.IO.Ports;
using System.Threading;
namespace DevAccess
{
    /// <summary>
    /// 艾诺安规设备访问
    /// </summary>
    public class AipuAccess
    {
        private string devName = "";
        private int readerID = 1;
        private Thread recvThread;
        private bool recvExit = false;
        private bool detectBegin = false;
        private bool detectDataOK = false;
        private List<byte> detectData = null;
        private SerialPort comportObj = null;
        public int recvInterval = 10;
        private bool pauseFlag = false;
        private object recvBufLock = new object();
        private string strCmdRes = ""; //命令应答
      //  private int recvPhase = 0;//接收起止标识，recvPhase = 1:收到'{'，有效字符串开始，2：收到'}',有效字符结束
        //private AinuoDetectModel detectModel = new AinuoDetectModel();
        public int ReaderID { get { return readerID; } }
        //public SerialPort ComPortObj { get; set; }
       
        public string ComportName { get; set; }
        public AipuAccess(int id, string port)
        {
            this.ComportName = port;
            readerID = id;
            devName = "艾普检测仪" + readerID.ToString();
            detectData = new List<byte>();
            recvExit = false;
            recvThread = new Thread(new ThreadStart(ComRecvProc));
            recvThread.IsBackground = true;
            recvThread.Priority = ThreadPriority.Highest;
            recvThread.Name = string.Format("艾普检测仪{0}接收线程", this.readerID);
        }
        public bool CommPortOpen(ref string reStr)
        {
            try
            {
                string[] ports = System.IO.Ports.SerialPort.GetPortNames();
                if (!ports.Contains(this.ComportName))
                {
                    reStr = string.Format("{0} 口不存在", this.ComportName);
                    return false;
                }
                if(this.comportObj == null)
                {
                    this.comportObj = new SerialPort(this.ComportName, 9600, Parity.None, 8, StopBits.One);

                }
                if(this.comportObj != null && this.comportObj.IsOpen)
                {
                    this.comportObj.Close();
                }
                this.comportObj.Open();
                return true;
            }
            catch (Exception ex)
            {
                reStr = ex.Message;
                return false;
            }
        }
        public bool StartMonitor(ref string reStr)
        {
            this.pauseFlag = false;
            if (this.recvThread.ThreadState == (ThreadState.Background | ThreadState.Unstarted))
            {
                recvThread.Start();
            }
            return true;
        }
        public void StopMonitor()
        {
            this.pauseFlag = false;


        }
        public bool CommPortClose(ref string reStr)
        {
            try
            {
                if(this.comportObj == null)
                {
                    reStr = string.Format("串口:{0}未打开", this.ComportName);
                    return false;
                }
                this.comportObj.Close();
                return true;
            }
            catch (Exception ex)
            {
                reStr = ex.Message;
                return false;
            }
        }
        /// <summary>
        /// 打开设备，电源输出
        /// </summary>
        /// <param name="reStr"></param>
        /// <returns></returns>
        public bool OpenDev(ref string reStr)
        {
            try
            {
                if(this.comportObj == null || (!this.comportObj.IsOpen))
                {
                    reStr = "串口未打卡";
                    return false;
                }
                string strCmd = "#G";
               // recvPhase = 0;
                strCmdRes = string.Empty;
                this.comportObj.Write(strCmd);
                int timeCounter = 0;
                int reTryInterval = 100;
                int timeOut = 2000;
                while (timeCounter < timeOut)
                {

                    lock(recvBufLock)
                    {
                        strCmdRes = strCmdRes.Trim().ToUpper();
                        if (strCmdRes.Contains("RECEIVED"))
                        {
                            return true;
                        }
                        else if (strCmdRes.Trim().ToUpper().Contains("ERROR"))
                        {
                            reStr = "设备返回错误‘Error',电源不在待机态";
                            return false;
                        }
                    }
                  
   
                    Thread.Sleep(reTryInterval);

                    timeCounter += reTryInterval;
                }
                reStr = "应答超时:2秒";
                return false;
            }
            catch (Exception ex)
            {
                reStr = ex.Message;
                return false;
            }
        }
        /// <summary>
        /// 关闭设备，停止检测
        /// </summary>
        /// <param name="reStr"></param>
        /// <returns></returns>
        public bool CloseDev(ref string reStr)
        {
            try
            {
                if (this.comportObj == null || (!this.comportObj.IsOpen))
                {
                    reStr = "串口未打卡";
                    return false;
                }
                string strCmd = "#U";
             
                strCmdRes = string.Empty;
                this.comportObj.Write(strCmd);
                int timeCounter = 0;
                int reTryInterval = 100;
                int timeOut = 2000;
                while (timeCounter < timeOut)
                {
                    lock(recvBufLock)
                    {
                        strCmdRes = strCmdRes.Trim().ToUpper();
                        if (strCmdRes.Contains("RECEIVED"))
                        {
                            return true;
                        }
                        else if (strCmdRes.Trim().ToUpper().Contains("ERROR"))
                        {
                            reStr = "设备返回错误‘Error',电源不在启动态";
                            return false;
                        }
                    }
                    
                    Thread.Sleep(reTryInterval);

                    timeCounter += reTryInterval;
                }
                reStr = "应答超时:2秒";
                return false;
            }
            catch (Exception ex)
            {
                reStr = ex.Message;
                return false;
            }
        }
        public bool SetRunningParam(float fraze,float volt,ref string reStr)
        {
            try
            {
                if (this.comportObj == null || (!this.comportObj.IsOpen))
                {
                    reStr = "串口未打卡";
                    return false;
                }
                string strFraze = ((int)(fraze*10)).ToString();
                if(strFraze.Length>=4)
                {
                    strFraze = strFraze.Substring(0,4);
                }
                else
                {
                    strFraze = strFraze.PadLeft(4,'0');
                }
                string strVol = ((int)(volt*10)).ToString();
                if(strVol.Length>=4)
                {
                    strVol = strVol.Substring(0,4);
                }
                else
                {
                    strVol = strVol.PadLeft(4,'0');
                }
                string strCmd = string.Format("#S{0}{1}",strFraze,strVol);

                strCmdRes = string.Empty;
                this.comportObj.Write(strCmd);
                int timeCounter = 0;
                int reTryInterval = 100;
                int timeOut = 2000;
                while (timeCounter < timeOut)
                {
                    lock(recvBufLock)
                    {
                        strCmdRes = strCmdRes.Trim().ToUpper();
                        if (strCmdRes.Trim().ToUpper().Contains("RECEIVED"))
                        {
                            return true;
                        }
                        else if (strCmdRes.Trim().ToUpper().Contains("ERROR"))
                        {
                            reStr = "设备返回错误‘Error',电源不在待机态或者是设置参数超过设置量程";
                            return false;
                        }
                    }
                   
                    Thread.Sleep(reTryInterval);

                    timeCounter += reTryInterval;
                }
                reStr = "应答超时:2秒";
                return false;
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return false;
               
            }
        }
     
        private void ComRecvProc()
        {
            //string strRecv = serialPort.ReadExisting();
            byte[] buf = new byte[256];
            while (!recvExit)
            {
                Thread.Sleep(recvInterval);
                if (pauseFlag)
                {
                    continue;
                }
                if (!comportObj.IsOpen)
                {
                    continue;
                }
                try
                {
                    int readNum = comportObj.Read(buf, 0, 256);
                    if (readNum <= 0)
                    {
                        continue;
                    }
                    lock(recvBufLock)
                    {
                        for (int i = 0; i < readNum; i++)
                        {
                            // Console.WriteLine(string.Format("Recv:0x{0}",buf[i].ToString("X2")));
                            strCmdRes = strCmdRes + System.Text.Encoding.UTF8.GetString(buf, i, 1);
                        }
                      //  Console.WriteLine("收到：" + strCmdRes);
                    }
                   
                }
                catch (Exception ex)
                {
                     comportObj.Close();
                   
                }
              
             
            }
        }
    }
}
