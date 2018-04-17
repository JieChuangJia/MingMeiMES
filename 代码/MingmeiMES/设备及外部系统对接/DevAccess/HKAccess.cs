using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using DevInterface;
namespace DevAccess
{
    public class HKAccess:IHKAccess
    {
       
        private int hkAccessID = 1;
        private string hkSvcIP = "";
        private int hkSvcPort = 13535;
        private bool isConnected = false;
        private TcpClient tcpClient = null;
       // private IPEndPoint udpRemotePoint = null;
        private object lockObj = new object();// 多线程锁
        private object lockRecvBuf = new object(); //接收缓存区锁
       // private object lockRecvObj = new object();
        private const int connTimeOut = 5;  //连接超时，单位：秒
       // private int netFailTimes = 0;//读取或写入次数超过5次自动重新连接，解决断网或者拔网线无法通讯问题
        private NetworkStream workStream = null;
        private ManualResetEvent connectDone = new ManualResetEvent(false);
        private List<byte> recBuffer = new List<byte>();  //接收缓存
        private  int recvTimeOut = 2000;     
        public const int palletCharNum = 9; //托盘字符数
        public const int batteryCharNum = 35;//电池条码字符数
        public string HkSvcIP { get { return hkSvcIP; } set { hkSvcIP = value; } }
        public int HkSvcPort { get { return hkSvcPort; } set { hkSvcPort = value; } }
        public int RecvTimeOut { get { return recvTimeOut; } set { recvTimeOut = value; } }
        public int HkAccessID { get { return hkAccessID; } set { hkAccessID = value; } }
        public HKAccess(int id,string svcIP,int port)
        {
            hkAccessID = id;
            this.hkSvcIP = svcIP;
            this.hkSvcPort = port;
        }
        public HKAccess()
        {

        }
        public bool Conn(ref string reStr)
        {
            try
            {
                lock(lockObj)
                {
                    if (string.IsNullOrEmpty(this.hkSvcIP))
                    {
                        reStr = "服务器IP地址为空!";
                        return false;
                    }
                    if (this.tcpClient != null)//如果不为空先关闭
                    {
                        this.tcpClient.Close();
                    }
                    tcpClient = new TcpClient();

                    tcpClient.ReceiveTimeout = connTimeOut;
                    connectDone.Reset();
                    tcpClient.BeginConnect(hkSvcIP, hkSvcPort, new AsyncCallback(ConnectCallback), tcpClient);

                    connectDone.WaitOne();
                    if ((tcpClient != null) && (this.isConnected))
                    {
                        workStream = tcpClient.GetStream();
                        StateObject state = new StateObject();
                        state.client = tcpClient;
                        if (workStream.CanRead)
                        {
                            IAsyncResult ar = workStream.BeginRead(state.buffer, 0, StateObject.BufferSize,
                                    new AsyncCallback(TCPReadCallBack), state);
                        }

                        isConnected = true;
                     //   this.netFailTimes = 0;
                    }
                    else
                    {
                        isConnected = false;
                    }
                } 
                if (isConnected)
                {
                    reStr = "杭可服务器连接成功！";
                  //  this.netFailTimes = 0; //读写失败次数清零

                }
                else
                {
                    reStr = "杭可服务器连接失败!";
                }
                return isConnected;
              
            }
            catch (Exception ex)
            {

                reStr = ex.ToString();
                return false;
            }
           
        }
        public bool Disconn(ref string reStr)
        {
            try
            {
                lock (lockObj)
                {
                    if ((tcpClient != null) && (tcpClient.Connected))
                    {
                        workStream.Close();
                        tcpClient.Close();
                        this.isConnected = false;
                    }
                }
               
                return true;
            }
            catch(Exception ex)
            {
                reStr = ex.ToString();
                return false;
            }
        }

        public bool BatteryFill(int stationID, string palletID, List<string> batteryIDs, ref string sndStr,ref string reStr)
        {
            try
            {
                Disconn(ref reStr);
                if (!Conn(ref reStr))
                {
                    return false;
                }
                //1 连接

                if (palletID.Trim().Length != palletCharNum)
                {
                    reStr = string.Format("托盘长度跟设置的不符，设置托盘长度：{0},实际长度：{1}", palletCharNum, palletID.Length);
                    return false;
                }
             
                lock(lockObj)
                {
                    sndStr = @"#1";
                    string cmdID = @"002";
                    string objectID = @"01";
                    string strBody = "";
                    //托盘号 
                    strBody = strBody + palletID + stationID.ToString();
                    //电池条码
                    for (int i = 0; i < batteryIDs.Count(); i++)
                    {
                        if (string.IsNullOrWhiteSpace(batteryIDs[i]))
                        {
                            strBody += "0";
                        }
                        else if (batteryIDs[i].Trim().Length != batteryCharNum)
                        {
                            //if (batteryIDs[i].Trim().Length != batteryCharNum)
                            //{
                            //    reStr = string.Format("电池条码长度跟设置的不符，设置电池条码长度：{0},实际长度：{1}", batteryCharNum, batteryIDs[i].Length);
                            //    return false;
                            //}
                            strBody += "0";

                        }
                        else
                        {
                            strBody += batteryIDs[i].Trim();
                        }
                    }
                    string strEnd = @"#0";
                    sndStr += (objectID + cmdID + strBody + strEnd);

                    byte[] bytesToSnd = System.Text.ASCIIEncoding.UTF8.GetBytes(sndStr);

                    //2 发送装载信息
                    if (!SendData(bytesToSnd))
                    {
                        reStr = "装载数据发送失败";
                        return false;
                    }
                    //等待命令接收成功的应答

                    if (!WaitFillCmdRes(this.recvTimeOut, ref reStr))
                    {
                        return false;
                    }
                    else
                    {
                        //移除002命令应答数据#10100201#0
                        lock(lockRecvBuf)
                        {
                            for (int i = 0; i < 11; i++)
                            {
                                this.recBuffer.RemoveAt(0);
                            }
                        }
                       
                    }
                    //3 等待
                    int re = WaitFillOK(this.recvTimeOut, ref reStr);
                    bool fillRe = false;
                    if (re == 0)
                    {
                        bytesToSnd = System.Text.ASCIIEncoding.UTF8.GetBytes("#10100301#0");
                        SendData(bytesToSnd);

                        fillRe = true;
                    }
                    else if (re == 2)
                    {
                        bytesToSnd = System.Text.ASCIIEncoding.UTF8.GetBytes("#10100301#0");
                        SendData(bytesToSnd);
                        fillRe = false;
                    }
                    string dscnRestr = "";
                    Disconn(ref dscnRestr);
                    return fillRe;
                }
                    
            }
            catch (Exception ex)
            {
                reStr = "发送失败，发生异常：" + ex.ToString();
                return false;
            }
        
   
        }
        private bool SendData(byte[] sendData)
        {
            try
            {
                if (workStream != null)
                {
                    lock (lockRecvBuf)
                    {
                        this.recBuffer.Clear();//发送请求之前清缓存
                    }
                   
                    workStream.Write(sendData, 0, sendData.Length);
                    return true;
                }
                else
                {
                    this.isConnected = false;
                    return false;
                }
            }
            catch
            {
                this.isConnected = false;
                return false;
            }
        }

        private bool WaitFillCmdRes(int timeOut,ref string reStr)
        {
            DateTime st = System.DateTime.Now;
            //if (timeOut > 5000)
            //{
            //    timeOut = 5000;
            //}
            while(true)
            {
                Thread.Sleep(5);
                string recvStr = "";
                lock(lockRecvBuf)
                {
                    recvStr = System.Text.ASCIIEncoding.UTF8.GetString(this.recBuffer.ToArray());
                }
                
                if(recvStr.Length>10)
                {
                    if (recvStr.Contains("#10100201#0"))
                    {
                        return true;
                    }
                    else
                    {

                        reStr = "装载命令应答错误，收到的应答信息：" + recvStr;
                        return false;
                    }
                }
                DateTime cur = System.DateTime.Now;
                TimeSpan ts = cur - st;
                if (ts.TotalMilliseconds > timeOut)
                {
                   
                    Disconn(ref reStr);
                    reStr = string.Format("命令接收应答超时,{0}毫秒，自动断开Socket连接",timeOut);
                    return false;
                }
            }
        }
        private int WaitFillOK(int timeOut,ref string reStr)
        {
            DateTime st = System.DateTime.Now;
            //if (timeOut > 5000)
            //{
            //    timeOut = 5000;
            //}
            while(true)
            {
                Thread.Sleep(5);
                string recvStr = "";
                lock(lockRecvBuf)
                {
                    recvStr = Encoding.Default.GetString(this.recBuffer.ToArray());//System.Text.ASCIIEncoding.UTF8.GetString(this.recBuffer.ToArray());
                }
               
                if(recvStr.Length>9)
                {
                    if(recvStr.ToUpper().Contains("#101003COMPLETE#0"))
                    {
                        return 0;
                    }
                    else
                    {

                        reStr = "装载错误，返回失败信息：" + recvStr.Substring(7, recvStr.Length - 9);
                        return 2;
                    }
                    
                }
                DateTime cur = System.DateTime.Now;
                TimeSpan ts = cur - st;
                bool re = false;
                if (ts.TotalMilliseconds > timeOut)
                {
                    reStr = string.Format("装载成功应答超时,{0}毫秒，自动断开Socket连接", timeOut);
                    return 1;
                }
              
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                TcpClient tc = (TcpClient)ar.AsyncState;
                if (tc.Connected)
                {
                    this.isConnected = true;
                    tc.EndConnect(ar);
                }
                else
                {
                    this.isConnected = false;
                    tc.EndConnect(ar);
                }
            }
            catch (SocketException se)
            {
                this.isConnected = false;
            }
            finally
            {
                connectDone.Set();
            }
        }
        private void TCPReadCallBack(IAsyncResult ar)
        {
            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                if ((state.client == null) || (!state.client.Connected))  //主动断开时
                {
                    this.isConnected = false;
                    return;
                }
                int numberOfBytesRead;
                NetworkStream netStream = state.client.GetStream();
                numberOfBytesRead = netStream.EndRead(ar);
                state.totalBytesRead += numberOfBytesRead;
                if (numberOfBytesRead > 0)
                {
                    byte[] recBytes = new byte[numberOfBytesRead];
                    Array.Copy(state.buffer, 0, recBytes, 0, numberOfBytesRead);
                    lock(lockRecvBuf)
                    {
                        this.recBuffer.AddRange(recBytes);
                    }
                    
                    
                    netStream.BeginRead(state.buffer, 0, StateObject.BufferSize, new AsyncCallback(TCPReadCallBack), state);
                }
                else
                {
                    //被动断开时 
                    netStream.Close();
                    state.client.Close();
                    netStream = null;
                    state = null;
                    this.isConnected = false;
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("接收杭可服务器数据出现异常:" + ex.Message);
                isConnected = false;
            }

        }
    }
}
