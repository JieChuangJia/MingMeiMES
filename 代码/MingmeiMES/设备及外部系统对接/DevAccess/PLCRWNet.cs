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
    /// <summary>
    /// 重新实现三菱PLC mc协议的接口类，基于TCP
    /// </summary>
    public class PLCRWNet : IPlcRW
    {
        #region 全局变量
        
        private bool isConnected = false;
        private TcpClient tcpClient = null;
        private NetworkStream workStream = null;
        private ManualResetEvent connectDone = new ManualResetEvent(false);
        private AutoResetEvent recvAutoEvent = new AutoResetEvent(false);

        private List<byte> recBuffer = new List<byte>();  //接收缓存
        private byte[] constantByte = new byte[7]; //固定发送字段
        private byte orderReadH = 0x04;             //字单位成批读取
        private byte orderReadL = 0x01;             //字单位成批读取

        private byte orderWriteH = 0x14;            //子单位成批写入
        private byte orderWriteL = 0x01;            //子单位成批写入
        private byte dataLengthH = 0x00;            //读取、写入字节长度
        private byte dataLengthL = 0x0c;            //读取、写入字节长度

        private byte cpuMonitorTimeH = 0x00;           //CPU监控时间
        private byte cpuMonitorTimeL = 0x04;           //CPU监控时间
        private byte childROderCodeH = 0x00;             //读子命令码
        private byte childROderCodeL = 0x00;             //读子命令码

        private byte childWOderCodeH = 0x00;             //子命令码
        private byte childWOderCodeL = 0x00;             //子命令码
        private byte writeTypeCode = 0xA8;              // 批量写软元件代码
        private byte readTypeCode = 0xA8;               // 批量读软元件代码

        private const int recvTimeOut = 3000;          //发送出去之后，等待接收完毕，之间的最大时间间隔
        private const int connTimeOut = 5;  //连接超时，单位：秒
        private EnumRequireType enumRequireType = EnumRequireType.成批读取;//默认值
        private object lockObj = new object();// 多线程锁
        private string connStr = "";
        private int netFailTimes = 0;//读取或写入次数超过5次自动重新连接，解决断网或者拔网线无法通讯问题

        private bool exitConnMonitor = false;
        private Thread connMonitorThread = null;
        public string ConnStr
        { get { return connStr; } set { this.connStr = value; } }
        #endregion

        #region 初始化
        public PLCRWNet()
        {
            constantByte[0] = 0x50;//副标题
            constantByte[1] = 0x00;
            constantByte[2] = 0x00;//网络编号
            constantByte[3] = 0xFF;//PLC编号

            constantByte[4] = 0xFF;//请求目标模块IO编号
            constantByte[5] = 0x03;
            constantByte[6] = 0x00;//请求目标站模块编号
           

        }
        public void Init()
        {
            connMonitorThread = new Thread(new ThreadStart(ConnMonitorProc));
            connMonitorThread.IsBackground = true;
            connMonitorThread.Name = "PLC tcp连接监控线程";
            connMonitorThread.Start();
            
        }
        public void Exit()
        {
            this.exitConnMonitor = true;
          
        }

        #endregion
        public Int64 PlcStatCounter { get { return 0; } }
        #region  接口实现
        public int StationNumber { get; set; }
        /// <summary>
        /// 作者:np
        /// 时间:2014年6月3日
        /// 内容:连接断开事件
        /// </summary>
        public event EventHandler<PlcReLinkArgs> eventLinkLost;
      //  public event EventHandler<LogEventArgs> eventLogDisp;

        //add by zwx,2014-08-04
        public int PlcID { get; set; }
        /// <summary>
        /// 作者:np
        /// 时间:2014年6月3日
        /// 内容:连接状态
        /// </summary>
        public bool IsConnect
        {
            get
            {
                return isConnected;
            }

        }

        /// <summary>
        /// 作者:np
        /// 时间:2014年6月3日
        /// 内容:断开连接
        /// </summary>
        public bool CloseConnect()
        {
            try
            {
                if ((tcpClient != null) && (tcpClient.Connected))
                {
                    workStream.Close();
                    tcpClient.Close();
                    this.isConnected = false;
                }
                return true;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// 作者:np
        /// 时间:2014年6月2日
        /// 内容:连接服务器
        /// </summary>
        /// <param name="plcAddr"></param>
        /// <param name="reStr"></param>
        /// <returns></returns>
        public bool ConnectPLC(ref string reStr)
        {
            try
            {
                if(string.IsNullOrEmpty(this.connStr))
                {
                    reStr = "PLC通信地址为空!";
                    return false;
                }
                //this.connStr = plcAddr;
                string[] splitStr = new string[] { ",", ";", ":", "-", "|" };
                string[] strArray = this.connStr.Split(splitStr, StringSplitOptions.RemoveEmptyEntries);
                if (strArray.Count() < 2)
                {
                    isConnected = false;
                }
                if ((tcpClient == null) || (!this.isConnected))
                {
                    //try
                    //{
                    if (this.tcpClient != null)//如果不为空先关闭
                    {
                        this.tcpClient.Close();
                    }
                    tcpClient = new TcpClient();
                    tcpClient.ReceiveTimeout = connTimeOut;
                    connectDone.Reset();
                    tcpClient.BeginConnect(strArray[0], int.Parse(strArray[1]), new AsyncCallback(ConnectCallback), tcpClient);
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
                        this.netFailTimes = 0;
                    }
                    else
                    {
                        isConnected = false;
                    }
                    //}
                    //catch (Exception se)
                    //{
                    //    isConnected = false;
                    //    reStr = "Q PLC连接失败";
                    //    return false;
                    //}
                }
            }
            catch
            {
                isConnected = false;
                reStr = "Q PLC连接失败";
            }
            if (isConnected)
            {
                reStr = "连接PLC成功！";
                this.netFailTimes = 0; //读写失败次数清零
                
            }
            else {
                reStr = "连接PLC失败!";
            }
            return isConnected;
        }

        /// <summary>
        /// 作者:np
        /// 时间:2014年6月3日
        /// 内容:读单个地址
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool ReadDB(string addr, ref int val)
        {
            try
            {
                lock (lockObj)
                {
                    short[] valueArr = new short[1];
                    bool readStatus = ReadMultiDB(addr, 1, ref valueArr);
                    if (readStatus)
                    {
                        val = valueArr[0];
                        return true;

                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 作者:np
        /// 时间:2014年6月3日
        /// 内容:写单个地址
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool WriteDB(string addr, int val)
        {
            try
            {
                
                lock (lockObj)
                {
                    //if (!isConnected)
                    //{
                    //    PlcReLinkArgs args = new PlcReLinkArgs();
                    //    args.PlcID = this.PlcID;
                    //    args.StrConn = this.connStr;
                     
                    //    if (eventLinkLost != null)//连接断开
                    //    {
                    //        eventLinkLost.Invoke(this, args);
                    //    }
                    //    return false;
                    //}
                    short[] valueArr = new short[1];
                    valueArr[0] = (short)val;
                    bool writeStatus = WriteMultiDB(addr, 1, valueArr);
                    if (writeStatus)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 作者:np
        /// 时间:2014年6月3日
        /// 内容:成批读取地址值
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="blockNum"></param>
        /// <param name="vals"></param>
        /// <returns></returns>
        public bool ReadMultiDB(string addr, int blockNum, ref short[] vals)
        {
            try
            {
                lock (lockObj)
                {
                    if (this.netFailTimes > 2 || (!isConnected))//超过2次就触发断开事件
                    {
                        isConnected = false;
                       
                        return false;
                    }
                   
                    List<byte> sendBuffer = new List<byte>();//发送缓存
                    sendBuffer.AddRange(constantByte);
                    sendBuffer.Add(dataLengthL);
                    sendBuffer.Add(dataLengthH);

                    sendBuffer.Add(cpuMonitorTimeL);
                    sendBuffer.Add(cpuMonitorTimeH);
                    sendBuffer.Add(orderReadL);
                    sendBuffer.Add(orderReadH);
                    sendBuffer.Add(childROderCodeL);
                    sendBuffer.Add(childROderCodeH);

                    short addrValue = Convert.ToInt16(addr.Substring(1));
                    byte[] addrBytes = BitConverter.GetBytes(addrValue);
                    byte addrHigh = 0;
                    sendBuffer.AddRange(addrBytes);
                    sendBuffer.Add(addrHigh);

                    sendBuffer.Add(readTypeCode);
                    sendBuffer.AddRange(BitConverter.GetBytes((short)blockNum));
                    SendData(sendBuffer.ToArray());
                    enumRequireType = EnumRequireType.成批读取;
                    if (recvAutoEvent.WaitOne(recvTimeOut, false))
                    {
                        bool analysisStatus = GetRecShortValues(this.recBuffer.ToArray(), blockNum, ref vals);
                        if (analysisStatus)
                        {
                            this.netFailTimes = 0;
                            return true;
                        }
                        else
                        {
                            this.netFailTimes++;
                           
                            return false;
                        }
                    }
                    else
                    {
                        this.netFailTimes++;
                        return false;
                    }
                    
                    
                }
            }
            catch
            {
                this.netFailTimes++;
                return false;
            }
        }

        /// <summary>
        /// 作者:np
        /// 时间:2014年6月3日
        /// 内容:成批写值
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="blockNum"></param>
        /// <param name="vals"></param>
        /// <returns></returns>
        public bool WriteMultiDB(string addr, int blockNum, short[] vals)
        {
            try
            {
                lock (lockObj)
                {
                    if (this.netFailTimes > 2 || (!isConnected))//超过2次就触发断开事件
                    {
                        isConnected = false;
                       // OnLinkLost();
                        
                        return false;
                    }
                    List<byte> sendBuffer = new List<byte>();//发送缓存
                    sendBuffer.AddRange(constantByte);
                    short sendLength = (short)(0x0c + blockNum * 2);
                    sendBuffer.AddRange(BitConverter.GetBytes(sendLength));

                    sendBuffer.Add(cpuMonitorTimeL);
                    sendBuffer.Add(cpuMonitorTimeH);
                    sendBuffer.Add(orderWriteL);
                    sendBuffer.Add(orderWriteH);
                    sendBuffer.Add(childWOderCodeL);
                    sendBuffer.Add(childWOderCodeH);

                    short addrValue = Convert.ToInt16(addr.Substring(1));
                    byte[] addrBytes = BitConverter.GetBytes(addrValue);
                    byte addrHigh = 0;
                    sendBuffer.AddRange(addrBytes);
                    sendBuffer.Add(addrHigh);

                    sendBuffer.Add(writeTypeCode);
                    sendBuffer.AddRange(BitConverter.GetBytes((short)blockNum));

                    for (int i = 0; i < vals.Count(); i++)
                    {
                        byte[] sendValue = BitConverter.GetBytes(vals[i]);
                        sendBuffer.AddRange(sendValue);
                    }

                    SendData(sendBuffer.ToArray());
                    enumRequireType = EnumRequireType.成批写入;
                    if (recvAutoEvent.WaitOne(recvTimeOut, false))
                    {
                        if (CheckWriteStatus(this.recBuffer.ToArray()))
                        {
                            this.netFailTimes = 0;
                            return true;
                        }
                        else
                        {
                            this.netFailTimes++;
                            
                            return false;
                        }
                    }
                    else
                    {
                        this.netFailTimes++;
                        return false;
                    }
                }
            }
            catch
            {
                this.netFailTimes++;
                return false;
            }

        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 网络连接监控线程
        /// </summary>
        private void ConnMonitorProc()
        {
            while (!exitConnMonitor)
            {
                Thread.Sleep(500);
                if (!this.isConnected)
                {
                    OnLinkLost();
                }
            }
        }
        /// <summary>
        /// 作者:np
        /// 时间:2014年8月23日
        /// 内容:触发连接断开事件函数
        /// </summary>
        private void OnLinkLost()
        {
            if (this.eventLinkLost != null)
            {
                PlcReLinkArgs args = new PlcReLinkArgs();
                args.PlcID = this.PlcID;
                args.StrConn = this.connStr;
                eventLinkLost.Invoke(this, args);
            }
        }
        /// <summary>
        /// 作者:np
        /// 时间:2014年6月3日
        /// 内容:异步连接的回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void ConnectCallback(IAsyncResult ar)
        {
            
            TcpClient tc = (TcpClient)ar.AsyncState;
            try
            {
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
        /// <summary>
        /// 作者:np
        /// 时间:2014年6月2日
        /// 内容:
        /// </summary>
        /// <param name="recBuffer"></param>
        /// <param name="readAddrCount"></param>
        /// <param name="addrValues"></param>
        /// <returns></returns>
        private bool GetRecShortValues(byte[] recBuffer, int readAddrCount, ref short[] addrValues)
        {
            try
            {
                short[] addrValuesTemp = new short[readAddrCount];
                int valueIndex = 11;
                int addrValuesIndex = 0;
                for (int i = 0; i < readAddrCount*2; i += 2)
                {
                    List<byte> recValueList = new List<byte>();
                    recValueList.Add(recBuffer[valueIndex + i]);
                    recValueList.Add(recBuffer[valueIndex + i+1]);//高位在后,转换一下
                    addrValuesTemp[addrValuesIndex] = BitConverter.ToInt16(recValueList.ToArray(), 0);
                    addrValuesIndex++;
                }
                addrValues = addrValuesTemp;
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 作者:np
        /// 时间:2014年6月2日
        /// 内容:写入返回校验
        /// </summary>
        /// <param name="recBuffer"></param>
        /// <returns></returns>
        private bool CheckWriteStatus(byte[] recBuffer)
        {
            try
            {
                if (recBuffer[0] == 0xD0 && recBuffer[1] == 0x00 && recBuffer[9] == 0x00 && recBuffer[10] == 0x00)//判断头尾是否正确
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///作者:np
        ///时间:2014年6月2日
        ///内容:发送数据
        /// </summary>
        private bool SendData(byte[] sendData)
        {
            try
            {
                if (workStream != null)
                {
                    this.recBuffer.Clear();//发送请求之前清缓存
                    workStream.Write(sendData, 0, sendData.Length);
                    return true;
                }
                else
                {
                    //PlcReLinkArgs args = new PlcReLinkArgs();
                    //args.PlcID = this.PlcID;
                    //args.StrConn = this.connStr;
          
                    //if (eventLinkLost != null)//连接断开
                    //{
                    //    eventLinkLost.Invoke(this, args);
                    //}
                    this.isConnected = false;
                    return false;
                }
            }
            catch
            {

                //PlcReLinkArgs args = new PlcReLinkArgs();
                //args.PlcID = this.PlcID;
                //args.StrConn = this.connStr;

                //if (eventLinkLost != null)//连接断开
                //{
                //    eventLinkLost.Invoke(this, args);
                //}
                this.isConnected = false;
                return false;
            }
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
                    netStream.BeginRead(state.buffer, 0, StateObject.BufferSize, new AsyncCallback(TCPReadCallBack), state);
                    this.recBuffer.AddRange(recBytes);
                    if (IsReceiveComplete())
                    {
                        recvAutoEvent.Set();
                    }
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
        /// <summary>
        /// 作者:np
        /// 时间:2014年6月3日
        /// 内容:解析数据是否接收完成
        /// </summary>
        /// <returns></returns>
        private bool IsReceiveComplete()
        {
            bool isComplete = false;
            if (enumRequireType == EnumRequireType.成批读取)
            {
                if (this.recBuffer.Count > 10)
                {
                    if (recBuffer[0] == 208 && recBuffer[1] == 0x00 && recBuffer[9] == 0x00 && recBuffer[10] == 0x00)//判断头尾是否正确
                    {
                        byte[] lengthByte = recBuffer.GetRange(7, 2).ToArray();
                        short datalength = BitConverter.ToInt16(lengthByte, 0);
                        short allDataLength = (short)(datalength + 9 );

                        if (this.recBuffer.Count == allDataLength)
                        {
                            //数据接收正确
                            isComplete = true;
                        }

                    }
                }
            }
            else if (enumRequireType == EnumRequireType.成批写入)
            {
                if (this.recBuffer.Count > 10)
                {
                    if (recBuffer[0] == 208 && recBuffer[1] == 0x00 && recBuffer[9] == 0x00 && recBuffer[10] == 0x00)//判断头尾是否正确
                    {
                        isComplete = true;
                    }
                }
            }
            return isComplete;
        }
        #endregion
    }

    /// <summary>
    /// 作者:np
    /// 时间:2014年6月2日
    /// 内容:异步接收缓存类
    /// </summary>
    internal class StateObject
    {
        public TcpClient client = null;
        public int totalBytesRead = 0;
        public const int BufferSize = 1024;
        public string readType = null;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder messageBuffer = new StringBuilder();
    }

   
}
