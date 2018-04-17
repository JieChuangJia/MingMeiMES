using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using DevInterface;
namespace DevAccess
{
    
    public class PrinterRW : IPrinterInfoDev
    {
        private string printerAddr = "192.168.3.100";
        private short printerPort = 10002;
        private bool isConnected = false;
        private TcpClient tcpClient = null;
        private NetworkStream workStream = null;
       // private ManualResetEvent connectDone = new ManualResetEvent(false);
      //  private AutoResetEvent recvAutoEvent = new AutoResetEvent(false);
        private string recvStr = "";
        private List<byte> saveBuf = new List<byte>();
        private PrinterReStat reStat = null;
        #region IPrinterInfoDev接口实现
         private int readerID = 1;
        public int ReaderID { get { return readerID; } }
        public string PrinterAddr { get { return PrinterAddr; } set { printerAddr = value; } }
        public short PrinterPort { get { return printerPort; } set { printerPort = value; } }
        public  bool IsConnect { get { return isConnected; } }
        public PrinterRW(int id,string ipAddr,short port)
        {
          
            this.readerID = id;
            this.printerAddr = ipAddr;
            this.printerPort = port;
            tcpClient = new TcpClient();
            tcpClient.ReceiveTimeout = 2000; //
        }
        public bool Connect(ref string reStr)
        {
            try
            {
                if(tcpClient.Connected)
                {
                    reStr = "贴标机已经处于连接状态";
                    return false;
                }
                tcpClient = new TcpClient();
                tcpClient.ReceiveTimeout = 2000; //
              
                tcpClient.Connect(this.printerAddr, this.printerPort);
                
                if(tcpClient.Connected)
                {
                    this.isConnected = true;
                    reStr = "连接成功";
                    StateObject state = new StateObject();
                    state.client = tcpClient;
                    workStream = tcpClient.GetStream();
                    if (workStream.CanRead)
                    {
                        IAsyncResult ar = workStream.BeginRead(state.buffer, 0, StateObject.BufferSize,
                                   new AsyncCallback(TCPReadCallBack), state);
                    }
                    return true;
                }
                else
                {
                    reStr = "连接超时";
                    this.isConnected = false;
                    return false;
                }
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return false; 
              
            }
        }
        public bool Disconnect(ref string reStr)
        {
            try
            {
                workStream.Close();
                tcpClient.Close();
                this.isConnected = false;
                reStr = "贴标机连接断开";
                return true;
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                this.isConnected = false;
                return false;
            }
            
        }
        public bool SndBarcode(string code,ref string reStr)
        {
            if(!isConnected)
            {
               
                if(!Connect(ref reStr))
                {
                    reStr = "贴标机未连接:"+reStr;
                    return false;
                }
                
            }
            try
            {
                string sndStr = "test " + code + "\n";
                byte[] sndBuf = System.Text.Encoding.GetEncoding(936).GetBytes(sndStr);
                reStr = string.Empty; //发送之前先清空接收到的字符
                saveBuf.Clear();
                if(!SendBuf(sndBuf,ref reStr))
                {
                    reStr = "发送：" + sndStr + "接收：" + reStr;
                    return false;
                }
                
                //返回错误,超时，发送条码格式错误，
                bool re = false;
                if(reStr.Length>3)
                {
                    if(reStr.Substring(0,3) == "000")
                    {
                        re = true;
                    }
                }
                else
                {
                    re = false;
                }
                reStr = "发送：" + sndStr + "接收：" + reStr;                
                return re;
            }
            catch (Exception ex)
            {
                
                Disconnect(ref reStr);
                Connect(ref reStr);
               
                string sndStr = "test " + code + "\n";
                byte[] sndBuf = System.Text.Encoding.GetEncoding(936).GetBytes(sndStr);
                reStr = string.Empty; //发送之前先清空接收到的字符
                saveBuf.Clear();
                if (!SendBuf(sndBuf, ref reStr))
                {
                    reStr = "发送：" + sndStr + "接收：" + reStr;
                    return false;
                }

                //返回错误,超时，发送条码格式错误，
                bool re = false;
                if (reStr.Length > 3)
                {
                    if (reStr.Substring(0, 3) == "000")
                    {
                        re = true;
                    }
                }
                else
                {
                    re = false;
                }
                reStr = "发送：" + sndStr + "接收：" + reStr;
                return re;
            }  
        }

        //查询条码是否打印完成
        public bool PrintFinished(string code,ref string reStr)
        {
            if (!isConnected)
            {
                reStr = "贴标机未连接";
                return false;
            }
            try
            {
                string sndStr = "result " + code + "\n";
                byte[] sndBuf = System.Text.Encoding.GetEncoding(936).GetBytes(sndStr);
               
                if(!SendBuf(sndBuf,ref reStr))
                {
                    reStr = "发送：" + sndStr + "接收：" + reStr;
                    return false;
                }
                bool re = false;
                if (reStr.Length > 3)
                {
                    if (reStr.Substring(0, 3) == "000")
                    {
                        re = true;
                    }
                }
                else
                {
                    re = false;
                }
                reStr = "发送：" + sndStr + "接收：" + reStr;
                return re;
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return false;

            }  
        }
        public bool PrinterStat(ref PrinterReStat stat, ref string reStr)
        {
            if (!isConnected)
            {
                reStr = "贴标机未连接";
                return false;
            }
            try
            {
                string sndStr = "stat\n";
                byte[] sndBuf = System.Text.Encoding.GetEncoding(936).GetBytes(sndStr);
                if (!SendBuf(sndBuf, ref reStr))
                {
                    reStr = "发送：" + sndStr + "接收：" + reStr;
                    return false;
                }
                //返回信息,应答超时，001 错误信息，000 OK
                stat = new PrinterReStat();
                bool re = false;
                if (reStr.Length > 3)
                {
                    if (reStr.Substring(0, 3) == "000")
                    {
                        re = true;
                    }
                }
                else
                {
                    re = false;
                }
                reStr = "发送：" + sndStr + " 接收：" + reStr;         
                return re;
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return false;

            }  
        }
        public bool SndHelloInfo(ref string reStr)
        {
            if (!isConnected)
            {
                reStr = "贴标机未连接";
                if (!Connect(ref reStr))
                {
                    reStr = "贴标机未连接:" + reStr;
                    return false;
                }
                
            }
            try
            {
                string sndStr = "hello\n";
                byte[] sndBuf = System.Text.Encoding.GetEncoding(936).GetBytes(sndStr);
                if (!SendBuf(sndBuf, ref reStr))
                {
                    reStr = "发送：" + sndStr + "接收:" + reStr;
                    return false;
                }
                reStr = "发送：" + sndStr + "接收：" + reStr;         
                return true;
            }
            catch (Exception)
            {
                Disconnect(ref reStr);
                Connect(ref reStr);
                string sndStr = "hello\n";
                byte[] sndBuf = System.Text.Encoding.GetEncoding(936).GetBytes(sndStr);
                if (!SendBuf(sndBuf, ref reStr))
                {
                    reStr = "发送：" + sndStr + "接收:" + reStr;
                    return false;
                }
                reStr = "发送：" + sndStr + "接收：" + reStr;
                return true;

            }  
        }

        #endregion
        #region 内部接口
        private bool SendBuf(byte[] buf, ref string reStr)
        {
            recvStr = string.Empty; //发送之前先清空接收到的字符
            saveBuf.Clear();
            workStream.Write(buf, 0, buf.Count());
            int timeCounter = 0;
            int reTryInterval = 100;
            int timeOut = 2000;
            while (timeCounter < timeOut)
            {
                if (!string.IsNullOrEmpty(recvStr))
                {
                    reStr = recvStr;
                    return true;

                }
                Thread.Sleep(reTryInterval);

                timeCounter += reTryInterval;
            }
            reStr = "应答超时,收到：" + System.Text.Encoding.GetEncoding(936).GetString(saveBuf.ToArray()).Trim(); 
            return false;
        }
        /// <summary>
        /// 作者:np
        /// 时间:2014年6月2日
        /// 内容:TCP读数据的回调函数
        /// </summary>
        /// <param name="ar"></param>
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

                    for (int i = 0; i < numberOfBytesRead;i++ )
                    {
                       // if(recBytes[i] == 0x0D)
                        if (recBytes[i] == 0x0A)
                        {
                            recvStr = System.Text.Encoding.GetEncoding(936).GetString(saveBuf.ToArray()).Trim();
                           // recvStr = System.Text.Encoding.UTF8.GetString(saveBuf.ToArray()).Trim();
                            string[] strArry = recvStr.Split(new string[] { ",", " ", ":", "-", "|" }, StringSplitOptions.RemoveEmptyEntries);
                           
                            if(strArry.Count()>1)
                            {
                                reStat = new PrinterReStat();
                                reStat.errCode = strArry[0];
                                reStat.errInfo = strArry[1];
                            }
                          
                            break;
                        }
                        else
                        {
                            this.saveBuf.Add(recBytes[i]);
                        }
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
                isConnected = false;
            }

        }
        ///// <summary>
        ///// 作者:np
        ///// 时间:2014年6月3日
        ///// 内容:异步连接的回调函数
        ///// </summary>
        ///// <param name="ar"></param>
        //private void ConnectCallback(IAsyncResult ar)
        //{

        //    TcpClient tc = (TcpClient)ar.AsyncState;
        //    try
        //    {
        //        if (tc.Connected)
        //        {
        //            this.isConnected = true;
        //            tc.EndConnect(ar);
        //        }
        //        else
        //        {
        //            this.isConnected = false;
        //            tc.EndConnect(ar);
        //        }
        //    }
        //    catch (SocketException se)
        //    {
        //        this.isConnected = false;
        //    }
        //    finally
        //    {
        //        connectDone.Set();
        //    }
        //}
        #endregion
       
    }
}
