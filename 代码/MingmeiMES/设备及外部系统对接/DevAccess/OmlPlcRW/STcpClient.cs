using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace DevAccess
{
    public class STcpClient:ISocket
    {
        private TcpClient myTcpClient;
        
        private Stream myNetStream;
        private int netFailTimes = 0;//读取或写入次数超过5次自动重新连接，解决断网或者拔网线无法通讯问题
        private bool exitConnMonitor = false;
        private string currIp = "";
        private int currPort = 0;
        private object threadObj = new object();
        private Thread connMonitorThread = null;
        /// <summary>
        /// 接收完成时引发事件。
        /// </summary>
        public  event EventHandler<SocketEventArgs> ReceiveCompleted;
        /// <summary>
        /// 发送完成时引发事件。
        /// </summary>
        public  event EventHandler<SocketEventArgs> SendCompleted;
        /// <summary>
        /// 断开连接引发事件
        /// </summary>
        public event EventHandler<LostLinkEventArgs> LostLink;
        public STcpClient()
        {
            this.myTcpClient = new TcpClient();
          //  InitMonitorLosfLink();//暂时先不让自动连接
        }
       
        private bool isConnected = false;
        public bool IsConnected { get { return isConnected; } set { this.isConnected = value; } }
        public bool Connect(string ip, int port,ref string reStr)
        {
            try
            {
                if (string.IsNullOrEmpty(ip))
                {
                    reStr = "地址为空!";
                    return false;
                }
                
                if (this.IsConnected == true)//如果不为空先关闭
                {
                   
                    reStr = "网络已经连接！";
                    return true;
                }
                this.currIp = ip;
                this.currPort = port;
                Disconnect();
                this.myTcpClient = new TcpClient();
               
                this.myTcpClient.ReceiveTimeout = 5;
                SocketAsyncState state = new SocketAsyncState();
                state.client = this.myTcpClient;

                this.myTcpClient.BeginConnect(ip, port, new AsyncCallback(ConnectCallback), state);
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                 
                TimeSpan ts2 = sw.Elapsed;
                //等待异步全部处理完成
                while (!state.IsCompleted) 
                {
                    if (sw.Elapsed.TotalSeconds >=5)
                    {
                        reStr = "网络连接失败！";
                        sw.Stop();
                        return false;
                    }
                  
                }
                this.isConnected = true;

                if (this.IsConnected == false)
                {
                    return false;
                }

                myNetStream = myTcpClient.GetStream();
                if (myNetStream.CanRead == false)
                {
                    reStr = "网络流不可写！";
                    return false;
                }


                IAsyncResult ar = myNetStream.BeginRead(state.buffer, 0, SocketAsyncState.BufferSize,
                            new AsyncCallback(ReadCallBack), state);
               
                this.netFailTimes = 0;
             
                return true;

            }
            catch (Exception ex)
            {
                //isConnected = false;
                reStr = "连接错误，" + ex.Message;
                return false;
            }
             
        }
        public bool Disconnect()
        {
            try
            {
                if (this.myTcpClient != null )
                {
                    myTcpClient.Close();
                  
                    this.isConnected = false;
                    this.exitConnMonitor = true;
                  
                }
                if(this.myNetStream!= null)
                {
                    myNetStream.Close();
                }
                return true;
            }
            catch
            {
                return false;
            }

        }
        public bool Send(byte[] data, ref string reStr)
        {
            lock (this.threadObj)
            {
                try
                {

                    if (myNetStream != null)
                    {
                        //this.recBuffer.Clear();//发送请求之前清缓存
                        myNetStream.Write(data, 0, data.Length);
                        return true;
                    }
                    else
                    {
                        this.netFailTimes++;
                        return false;
                    }
                }
                catch
                {
                    this.netFailTimes++;
                    if (this.netFailTimes >= 5)
                    {
                        OnLostLink();
                    }

                    this.isConnected = false;
                    return false;
                }

            }
        }
       
        /// <summary>
        /// 作者:np
        /// 时间:2014年6月2日
        /// 内容:TCP读数据的回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void ReadCallBack(IAsyncResult ar)
        {
            try
            {
                SocketAsyncState state = (SocketAsyncState)ar.AsyncState;
                if ((state.client == null) || (!state.client.Connected))  //主动断开时
                {
                    this.isConnected = false;
                    return;
                }
                int numberOfBytesRead;
                NetworkStream netStream = state.client.GetStream();
                numberOfBytesRead = netStream.EndRead(ar);
                //state.totalBytesRead += numberOfBytesRead;

                if (numberOfBytesRead > 0)
                {
                    byte[] recBytes = new byte[numberOfBytesRead];
                    netStream.BeginRead(state.buffer, 0, SocketAsyncState.BufferSize, new AsyncCallback(ReadCallBack), state);
                    Array.Copy(state.buffer, 0, recBytes, 0, numberOfBytesRead);
                    //state.AddBuffer(recBytes);
                    OnRecCompleted(state,recBytes);
                }
                else
                {
                    //被动断开时 
                    netStream.Close();
                    state.client.Close();
                    netStream = null;
                    state = null;
                    this.isConnected = false;
                    OnLostLink();
                    
                }
            }
            catch  
            {
               this. isConnected = false;
               string reStr = "";
               Connect(this.currIp, this.currPort, ref reStr);
            }

        }
        private void OnRecCompleted(SocketAsyncState state,byte [] recBytes)
        {
            SocketEventArgs sockArgs = new SocketEventArgs();
            sockArgs.RecBytes = recBytes;
            sockArgs.SockState = state;
            if(this.ReceiveCompleted!= null)
            {
                this.ReceiveCompleted.Invoke(this, sockArgs);
             
            }
        }
        private void ConnectCallback(IAsyncResult ar)
        {
            SocketAsyncState state = (SocketAsyncState)ar.AsyncState;
            try
            {
              
                if (state.client.Connected)
                {
                    state.client.EndConnect(ar);
                }
                else
                {
                    state.client.EndConnect(ar);
                }
                state.IsCompleted = true;
            }
            catch 
            {
                state.IsCompleted = false;
            }
           
        }
        private void OnLostLink()
        {
            if(this.LostLink!= null)
            {
                LostLinkEventArgs linkArgs = new LostLinkEventArgs();
                linkArgs.IP= this.currIp;
                linkArgs.Port = this.currPort;
                this.LostLink.Invoke(this, linkArgs);
            }
        }
        private void InitMonitorLosfLink()
        {
            this.exitConnMonitor = false;
            connMonitorThread = new Thread(new ThreadStart(ConnMonitorProc));
            connMonitorThread.IsBackground = true;
            connMonitorThread.Name = "PLC tcp连接监控线程";
            connMonitorThread.Start();
        }

        /// <summary>
        /// 网络连接监控线程
        /// </summary>
        private void ConnMonitorProc()
        {
            string reStr="";
            while (!exitConnMonitor)
            {
                Thread.Sleep(1000);
                if (this.isConnected==false&&this.netFailTimes>=5)
                {
                    Connect(this.currIp, this.currPort, ref reStr);
                }
            }
        }
    }
}
