using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DevAccess
{
    public class SUdpClient : ISocket
    {
        private string ServerIP { get; set; }
        private int ServerPort { get; set; }
        private int LocalPort { get; set; }
        private const int threadInterval = 30;
        private bool exitThread = false;

        //private const int localPort = 3008;
        private const int TIMEOUT = 5000;
       
        private UdpClient myUdpClient;
        private Thread recThread = null;
        private bool isConected = false;
        private bool threadExit = false;
        //============================================//
        private UdpClient udpClient = null;
        private UdpState udpReceiveState = null;
                  
        private object lockObj = new object();// 多线程锁
        private IPEndPoint udpRemotePoint = null;
        private bool udpClientEnabled = false;//当前客户端是否可用
        private static int localPcPortMachine = 2000;
        private static List<int> currUsePort = new List<int>();
       //===============================================//
        /// <summary>
        /// 接收完成时引发事件。
        /// </summary>
        public event EventHandler<SocketEventArgs> ReceiveCompleted;

        /// <summary>
        /// 断开连接引发事件
        /// </summary>
        public event EventHandler<LostLinkEventArgs> LostLink;


        public SUdpClient()
        {
            //recThread = new Thread(ReciveMsg);//开启接收消息线程
            //recThread.IsBackground = true;
        }

        private void InitUdpClient()
        {
            recThread = new Thread(ReciveMsg);//开启接收消息线程
            recThread.IsBackground = true;
            this.threadExit = false;
            this.myUdpClient = new UdpClient();
            const long IOC_IN = 0x80000000;
            const long IOC_VENDOR = 0x18000000;
            const long SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
            byte[] optionInValue = { Convert.ToByte(false) };
            byte[] optionOutValue = new byte[4];

            this.myUdpClient.Client.IOControl((IOControlCode)SIO_UDP_CONNRESET, optionInValue, optionOutValue);

           
        }
        /// <summary>
        /// 获取是否已连接。
        /// </summary>
        public bool IsConnected { get { return this.isConected; } }
        /// <summary>
        /// 连接至服务器。
        /// </summary>
        /// <param name="endpoint">服务器终结点。</param>
        //public bool Connect(string ip, int port, ref string reStr)
        //{         
        //    try
        //    {
        //        if(this.IsConnected ==true)//UDP通讯连接一次即可
        //        {
        //            return true;
        //        }
        //        //Disconnect();
        //        InitUdpClient();
        //        this.myUdpClient.Connect(ip, port);
        //        this.exitThread = false;
        //        this.ServerIP = ip;
        //        this.ServerPort = port;
                
        //        this.exitThread = false;
        //        this.isConected = true;
        //        StartRec();//开始接收
             
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.ToString());
        //        return false;
        //    }
        //}

        /// <summary>
        /// 发送数据。
        /// </summary>
        /// <param name="data">要发送的数据。</param>
        public bool Send(byte[] data, ref string reStr)
        {
            lock(this.lockObj)
            {
                if (this.isConected == false)
                {
                    reStr = "请连接服务器！";
                    return false;
                }
                if (this.udpClient == null)
                {
                    reStr = "请连接服务器！";
                    return false;
                }

                int re = this.udpClient.Send(data, data.Count(), this.udpRemotePoint);
                if (re > 0)
                {
                    reStr = "发送成功！";
                    return true;
                }
                else
                {
                    reStr = "发送失败！";
                    return false;
                }
            }
           
            //return false;
            //int sendLength = myUdpClient.Send(data, data.Length);
            //if (sendLength > 0)
            //{
            //    reStr = "发送成功！";
            //    return true;
            //}
            //else
            //{
            //    reStr = "发送失败！";
            //    return false;
            //}
        }

        /// <summary>
        /// 断开连接。
        /// </summary>
        public bool Disconnect()
        {
            //if(recThread == null)
            //{
            //    return true;
            //}
            //this.exitThread = true;
           
            //if (recThread.ThreadState == (ThreadState.Running | ThreadState.Background))
            //{
            //    if (!recThread.Join(500))
            //    {
            //        recThread.Abort();
            //        recThread = null;
            //    }
            //}
            //if (this.myUdpClient != null)
            //{
            //    this.myUdpClient.Close();
            //}
            this.isConected = false;
          
            return true;
        }

        private void StartRec()
        {
           
            if (recThread.ThreadState == (ThreadState.Running | ThreadState.Background))
            {
                return;
            }
            recThread.Start();
        }

        public bool  Connect(string ip, int port, ref string reStr)
        {
            try
            {
                lock (lockObj)
                {
                    if (string.IsNullOrEmpty(ip))
                    {
                        reStr = "PLC通信地址为空!";
                        return false;
                    }
                    
                    if(this.udpClientEnabled == false)
                    {
                        this.udpClientEnabled = true;
                        this.LocalPort  = GetLocalPort();
                        udpClient = new UdpClient(this.LocalPort);
                        currUsePort.Add(this.LocalPort);
                        this.udpRemotePoint = new IPEndPoint(IPAddress.Parse(ip), port);

                        udpReceiveState = new UdpState();
                        udpReceiveState.ipEndPoint = this.udpRemotePoint;
                        udpReceiveState.udpClient = udpClient;
                    }
                    
                    this.ServerIP = ip;
                    this.ServerPort = port;


                    udpClient.BeginReceive(UdpRecvCallback, udpReceiveState);
                    
                    reStr = "PLC连接成功";
                    this.isConected = true;
                 
                    return true;
                }
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                this.isConected = false;
             
                this.udpClientEnabled = false;
                currUsePort.Remove(this.LocalPort);
                 
                return false;

            }
        }

        private int GetLocalPort()
        {
           
            while(true)
            {
                localPcPortMachine++;
                if (localPcPortMachine > 5000)
                {
                    localPcPortMachine = 2000;
                }
                if (currUsePort.Contains(localPcPortMachine) == true)
                {
                    continue;
                }
                else
                {
                    
                    break;
                }
            }

            return localPcPortMachine;
        }
        private void UdpRecvCallback(IAsyncResult iar)
        {
            try
            {
                UdpState udpState = iar.AsyncState as UdpState;
                if (iar.IsCompleted)
                {
                    Byte[] recBytes = udpState.udpClient.EndReceive(iar, ref udpState.ipEndPoint);
                    if (recBytes != null && recBytes.Count() > 0)
                    {
                        OnRecComplete(recBytes);


                    }

                }
                udpClient.BeginReceive(UdpRecvCallback, udpState);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SUdpClient类UdpRecvCallback错误：" + ex.StackTrace.ToString());
            }
        }

        private void OnRecComplete(byte[] recBytes)
        {
            if (this.ReceiveCompleted != null)
            {
                SocketEventArgs sea = new SocketEventArgs();
                sea.RecBytes = recBytes;
                this.ReceiveCompleted.Invoke(this, sea);
            }
        }
        private void ReciveMsg()//接收数据做服务
        {

            byte[] recBytes = null;

            while (exitThread == false)
            {
                try
                {
                   
                    System.Threading.Thread.Sleep(threadInterval);
                    IPEndPoint remoteIPE = new IPEndPoint(IPAddress.Any, 0);
                    if(this.myUdpClient == null)
                    {
                        continue;
                    }
                    recBytes = this.myUdpClient.Receive(ref remoteIPE);//UDP接收数据
                    if (recBytes.Length > 0 && remoteIPE.Address.ToString() == this.ServerIP)//只处理特定的服务端的数据
                    {
                        OnRecComplete(recBytes);
                    }
                    
                }
                catch
                {
                    
                }
            }
        }

    }
}
