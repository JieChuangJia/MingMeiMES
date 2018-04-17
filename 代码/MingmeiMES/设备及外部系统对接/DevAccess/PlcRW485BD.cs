using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using DevInterface;
namespace DevAccess
{
    /// <summary>
    /// 接收数据包的类型定义
    /// </summary>
    public enum RecvPackageType
    {
        RECV_STX = 1, //属于针对read类型命令的响应，plc返回数据
        RECV_ACK  // 属于针对write类型命令的响应，plc返回肯定应答
    }
    public class ComRwBytesEventArgs : EventArgs
    {
        public byte[] buf { get; set; }
    }
    /// <summary>
    /// 针对三菱plc fx3u型号的串口通信类，1C帧，
    /// </summary>
    public class PlcRW485BD:IPlcRW
    {
        private delegate void delegteWriteCom(byte[] sndBuf, int offset, int dataLen);
        private object sendLock = new object();
        private AutoResetEvent recvAutoEvent = new AutoResetEvent(true);
        private Thread recvThread;
        private bool recvExit = false;
        public int recvInterval = 10;
        private Int64 plcStatCounter = 0;
       
        private const int recvMax = 1024;
        private const int recvTimeOut = 2000;//发送出去之后，等待接收完毕，之间的最大时间间隔

        private RecvPackageType currentRecvType = RecvPackageType.RECV_STX;
       
        /// <summary>
        /// 报文等待时间，转换成十六进制字符,*10ms
        /// </summary>
        public byte datagrmWaitTime = 0x33; //'6'*10=60ms
        public EnumPlcCata plcCata = EnumPlcCata.FX3U;
        private int plcStationNumber = 1;
        private byte pcStationNumber = 0xff;
        private int db1Len = 1000;
        private int db2Len = 1000;
        private Int16[] db1Vals = null;
        private Int16[] db2Vals = null;
        public SerialPort serialPort;
        private byte[] recvBuffer = new byte[recvMax];
        private int recvBytes = 0;
        private bool responseOk = false;
        private bool recvBegin = false;
        private const byte STX = 0x02;
        private const byte ETX = 0x03;
        private const byte EOT = 0x04;
        private const byte ENQ = 0x05;
        private const byte ACK = 0x06;
        private const byte LF = 0x0A;
        private const byte CL = 0x0C;
        private const byte CR = 0x0D;
        private const byte NAK = 0x15;
        private const byte DLE = 0x10;
        private bool isConnect = false;
        private string comportName = "";
        public string PlcRole { get; set; }
        public Int64 PlcStatCounter { get { return 0; } }
        public string ComPort { get { return comportName; } set { comportName = value; } }
        public Int16[] Db1Vals { get { return db1Vals; } set { db1Vals = value; } }
        public Int16[] Db2Vals { get { return db2Vals; } set { db2Vals = value; } }
     //   public Int64 PlcStatCounter { get { return plcStatCounter; } }
        public PlcRW485BD(EnumPlcCata plcEnum)
        {
            this.plcCata = plcEnum;
            serialPort = new SerialPort();
            //serialPort.DataReceived += RecvEventHandler;
            this.comportName = "com2";
            serialPort.PortName = comportName;
            serialPort.DataBits = 8;
            serialPort.StopBits = StopBits.One;
            serialPort.Parity = Parity.None;
            serialPort.ReceivedBytesThreshold = 1;
            serialPort.BaudRate = 9600;
            //serialPort.WriteTimeout = 500;
            //serialPort.Handshake = Handshake.XOnXOff; //握手协议,软件XON/XOFF
            if (this.db1Len < 10)
            {
                this.db1Len = 10;
            }
            if (this.db2Len < 10)
            {
                this.db2Len = 10;
            }
            db1Vals = new Int16[this.db1Len];
            db2Vals = new Int16[this.db2Len];
        }
        #region 接口实现

        /// <summary>
        /// com 接收到数据的事件触发
        /// </summary>
        public event EventHandler<ComRwBytesEventArgs> comRecvHandler;

        /// <summary>
        /// com发送数据的触发
        /// </summary>
        public event EventHandler<ComRwBytesEventArgs> comSendHandler;

        /// <summary>
        /// 连接断开事件
        /// </summary>
        public event EventHandler<PlcReLinkArgs> eventLinkLost;
      
        public int StationNumber
        {

            get
            {
                return this.plcStationNumber;
            }
            set
            {
                this.plcStationNumber = value;
            }
        }
        public int ComBaudRate
        {
            get
            {
                return serialPort.BaudRate;
            }
            set
            {
                serialPort.BaudRate = value;
            }
        }
        public string ComPortName 
        {
            get
            {
                return serialPort.PortName;
            }
            set
            {
                serialPort.PortName = value;
            }
        }
        public int ComDataBits 
        {
            get
            {
                return serialPort.DataBits;
            }
            set
            {
                serialPort.DataBits = value;
            }
        }
        public System.IO.Ports.StopBits ComStopBits
        {
            get
            {
                return serialPort.StopBits;
            }
            set
            {
 
                serialPort.StopBits = value;
            }
        }
        public int PlcStationNumber 
        {
            get
            {
                return plcStationNumber;
            }
            set
            {
                plcStationNumber = value;
            }
        }
        public System.IO.Ports.Parity ComParity 
        {
            get
            {
                return serialPort.Parity;
            }
            set
            {
                serialPort.Parity = value;
            }
            
        }
        public int PlcID { get; set; }
        public bool IsConnect
        {
            get { return isConnect; }
        }
        public bool IsPortOpened 
        {
            get
            {
                return serialPort.IsOpen;
            }
        }

        public void Init()
        {

        }
        public void Exit()
        {

        }
        public void Stop()
        {
            recvThread.Suspend();
        }
        public  bool ConnectPLC(ref string reStr)
        {
            
            try
            {
               
                if (serialPort.IsOpen)
                {

                    serialPort.Close();
                }
               
                serialPort.Open();
                recvAutoEvent = new AutoResetEvent(false);
                recvExit = false;
                if (plcCata == EnumPlcCata.FX5U)
                {
                    recvThread = new Thread(new ThreadStart(ComRecvProc2));
                }
                else
                {
                    recvThread = new Thread(new ThreadStart(ComRecvProc));
                }
                
                recvThread.IsBackground = true;
                recvThread.Priority = ThreadPriority.Highest;
                recvThread.Name = "PLC串口接收线程";
                isConnect = true;
                recvThread.Start();
                reStr = "FX plc通信端口已打开";
                string logStr = string.Format("打开串口：{0},{1},{2},{3},{4}", serialPort.PortName, serialPort.BaudRate.ToString(), serialPort.DataBits.ToString(), serialPort.StopBits.ToString(), serialPort.Parity.ToString());
                Console.WriteLine(logStr);
                return true;
            }
            catch (System.Exception ex)
            {
               // AddLog("PLC 485通信打开端口" + comPort+"失败：" + ex.ToString(), EnumLogType.错误);
                reStr = "FX plc通信端口打开失败:" + ex.ToString();
                isConnect = false;
                return false;
            }
        }

        public bool CloseConnect()
        {
            isConnect = false;
            recvExit = true;
            Console.WriteLine(recvThread.ThreadState.ToString());
            Thread.Sleep(300);
            //if (recvThread != null && recvThread.ThreadState == (ThreadState.Running | ThreadState.Background))
            //{
            //   // Console.WriteLine("fx485 接收线程停止");
            //    if (!recvThread.Join(300))
            //    {
            //        recvThread.Abort();
                   
            //    }
            //    recvThread = null;
            //}
            if (serialPort.IsOpen)
            {
                Console.WriteLine(serialPort.PortName + "串口关闭");
                serialPort.Close();
            }
           
            return true;
        }

        public bool ExeBitRead(string addr, ref int val)
        {
            try
            {
                lock (sendLock)
                {
                    val = 0;
                    if (!serialPort.IsOpen)
                    {
                        return false;
                    }

                    addr.Trim();
                    if (addr.Count() != 5)
                    {
                        return false;
                    }
                    byte[] bufferSnd = new byte[256];
                    int sndByteCount = 0;
                    bufferSnd[0] = ENQ;

                    //站号
                    bufferSnd[1] = (byte)((plcStationNumber >> 4) + 0x30);
                    bufferSnd[2] = (byte)((plcStationNumber & 0x0f) + 0x30);
                    //pc号
                    bufferSnd[3] = 0x46;
                    bufferSnd[4] = 0x46;

                    if (plcCata == EnumPlcCata.FX3U || plcCata == EnumPlcCata.ACPU)
                    {
                        //BR指令
                        bufferSnd[5] = 0x42;
                        bufferSnd[6] = 0x52;
                    }
                    else
                    {
                        //JR
                        bufferSnd[5] = 0x4A;
                        bufferSnd[6] = 0x52;
                    }
                    

                    //报文等待
                    bufferSnd[7] = datagrmWaitTime;

                    sndByteCount = 8;
                    //地址
                    byte[] addrBytes = System.Text.UTF8Encoding.Default.GetBytes(addr);
                    if (addrBytes.Count() != 5)
                    {
                        return false;
                    }
                    addrBytes.CopyTo(bufferSnd, 8);
                    sndByteCount = 13;
                    //软件元点数,01
                    bufferSnd[13] = 0x30;
                    bufferSnd[14] = 0x31;

                    sndByteCount = 15;
                    //发送之前清空接收缓冲区
                    Array.Clear(recvBuffer, 0, recvBuffer.Count());
                    recvBytes = 0;
                    recvBegin = false;
                    responseOk = false;
                    //delegteWriteCom writeComHandler = new delegteWriteCom(WriteComportHandler);
                    //writeComHandler.Invoke(bufferSnd, 0, sndByteCount);
                    serialPort.Write(bufferSnd, 0, sndByteCount);
                    // Thread.Sleep(recvTimeOut);
                    recvAutoEvent.WaitOne(recvTimeOut);
                    //接收
                    if (!responseOk)
                    {
                        return false;
                    }
                    //解析
                    if (!CheckRecvAddr(recvBuffer))
                    {
                        return false;
                    }
                    byte[] dataBytes = GetRecvDataBytes(recvBuffer, 1);
                    if (dataBytes == null)
                    {
                        return false;
                    }
                    string strData = System.Text.Encoding.ASCII.GetString(dataBytes);
                    val = Convert.ToInt32(strData, 16);
                    //若解析成功，则应答
                    SendAck();
                    //  Thread.Sleep(recvTimeOut);
                    //Thread.Sleep(50);
                    return true;
                }
            }
            catch (System.Exception ex)
            {
               
                return false;
            }

        }
        public bool ReadDB(string addr, ref int val)
        {
            if (string.IsNullOrWhiteSpace(addr)|| addr.Length<2)
            {
                return false;
            }
            addr = addr[0] + int.Parse(addr.Substring(1, addr.Length - 1)).ToString().PadLeft(4, '0');
            short[] vals = null;
            if(!ReadMultiDB(addr,1,ref vals))
            {
                return false;
            }
            val = vals[0];
            return true;
           /* try
            {
                
                
                lock (sendLock)
                {
                    val = 0;
                    if (!serialPort.IsOpen)
                    {
                        return false;
                    }

                    addr.Trim();
                    if (addr.Count() != 5)
                    {
                        return false;
                    }
                    byte[] bufferSnd = new byte[256];
                    int sndByteCount = 0;
                    bufferSnd[0] = ENQ;

                    //站号
                    bufferSnd[1] = (byte)((plcStationNumber >> 4) + 0x30);
                    bufferSnd[2] = (byte)((plcStationNumber & 0x0f) + 0x30);
                    //pc号
                    bufferSnd[3] = 0x46;
                    bufferSnd[4] = 0x46;

                    //WR指令
                    bufferSnd[5] = 0x57;
                    bufferSnd[6] = 0x52;

                    //报文等待
                    bufferSnd[7] = datagrmWaitTime;

                    sndByteCount = 8;
                    //地址
                    byte[] addrBytes = System.Text.UTF8Encoding.Default.GetBytes(addr);
                    if (addrBytes.Count() != 5)
                    {
                        return false;
                    }
                    addrBytes.CopyTo(bufferSnd, 8);
                    sndByteCount = 13;
                    //软件元点数,01
                    bufferSnd[13] = 0x30;
                    bufferSnd[14] = 0x31;

                    sndByteCount = 15;
                    //发送之前清空接收缓冲区
                    Array.Clear(recvBuffer, 0, recvBuffer.Count());
                    recvBytes = 0;
                    recvBegin = false;
                    responseOk = false;
                    //发送前清空接收、发送缓冲区
                   // serialPort.DiscardInBuffer();
                    //serialPort.DiscardOutBuffer();
                    serialPort.Write(bufferSnd, 0, sndByteCount);
                    serialPort.BaseStream.Flush();
                    string sndStr = "";
                    for (int i = 0; i < sndByteCount;i++ )
                    {
                        sndStr += string.Format("{0} ", bufferSnd[i].ToString("X").PadLeft(2, '0'));
                    }
                    Console.WriteLine("Snd:" + sndStr);
                    //Thread.Sleep(recvTimeOut);
                    recvAutoEvent.WaitOne(recvTimeOut);
                    //接收
                    if (!responseOk)
                    {
                    //    string log = "通信错误：FX PLC 站号：" + plcStationNumber + ",通信超时\n";
                    //    log += ("单寄存器读取，收到字节数:" + recvBytes.ToString() + "\n");
                    //    if (recvBytes > 0)
                    //    {
                    //        log += ("recvBuffer[" + (recvBytes - 1).ToString() + "]=" + recvBuffer[recvBytes - 1].ToString());
                    //    }
                    //    AddLog(log, EnumLogType.错误);
                        //isConnect = false;
                        Console.WriteLine("应答超时");
                        return false;
                    }
                    //解析
                    if (!CheckRecvAddr(recvBuffer))
                    {
                        string log = "通信错误：FX PLC 站号：" + plcStationNumber + ",应答地址解析错误";
                        //AddLog(log, EnumLogType.错误);
                       // isConnect = false;
                        Console.WriteLine(log);
                        return false;
                    }
                    byte[] dataBytes = GetRecvDataBytes(recvBuffer, 4);
                    if (dataBytes == null)
                    {
                       // isConnect = false;
                        return false;
                    }
                    string strData = System.Text.Encoding.ASCII.GetString(dataBytes);
                    val = Convert.ToInt32(strData, 16);
                    //若解析成功，则应答
                    SendAck();
                
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                isConnect = false;
                //AddLog("FX PLC 读取单寄存器异常：" + ex.ToString(), EnumLogType.错误);
                return false;
            }*/
        }
        public bool ReadMultiDB(string addr, int blockNum, ref short[] vals)
        {
            if (string.IsNullOrWhiteSpace(addr) || addr.Length < 2)
            {
                return false;
            }
           
            try
            {
                if(plcCata == EnumPlcCata.FX5U)
                {
                    if(blockNum>960)
                    {
                        Console.WriteLine("PLC批量读取，一次最多960个字");
                        return false;
                    }
                    //4c帧,报文格式5，,二进制传输
                    lock (sendLock)
                    {
                        if (!serialPort.IsOpen)
                        {
                            return false;
                        }
                        addr.Trim();
                        int blockSt = int.Parse(addr.Substring(1));
                        List<byte> bufferSnd = new List<byte>();
                        bufferSnd.AddRange(new byte[] { 0x10, 0x02 }); //控制代码DLE，STX
                        bufferSnd.Add(0x12);//字节数
                        bufferSnd.Add(0xF8);//帧识别号
                        //访问路径
                        bufferSnd.Add((byte)this.plcStationNumber); //站号
                        bufferSnd.Add(0x00);//网络号
                        bufferSnd.Add(0xFF); //PLC编号
                        bufferSnd.AddRange(new byte[] { 0xFF, 0x03 }); //请求目标模块IO编号
                        bufferSnd.Add(0x00); //请求目标模块站号
                        bufferSnd.Add(0x00);//本站站号（PC）
                        //请求数据
                        bufferSnd.AddRange(new byte[] { 0x01, 0x04 }); //0x0401,指令：批量读取
                        bufferSnd.AddRange(new byte[] { 0x00, 0x00 });//子指令，字为单位
                        bufferSnd.Add((byte)(blockSt&0xff)); //起始软元件编号，三个字节，从低到高
                        bufferSnd.Add((byte)((blockSt>>8)&0xFF));
                        bufferSnd.Add((byte)((blockSt >> 16) & 0xFF));
                        bufferSnd.Add(0xA8); //软元件代码，D区
                        bufferSnd.Add((byte)(blockNum & 0xff)); //软元件点数，两字节
                        bufferSnd.Add((byte)((blockNum >> 8) & 0xff));

                        bufferSnd.AddRange(new byte[] { 0x10, 0x03 });//控制代码，DLE ，ETX

                        //发送之前清空接收缓冲区
                        Array.Clear(recvBuffer, 0, recvBuffer.Count());
                        serialPort.Write(bufferSnd.ToArray(), 0, bufferSnd.Count());
                        serialPort.BaseStream.Flush();
                        //Thread.Sleep(recvTimeOut);
                        recvAutoEvent.WaitOne(recvTimeOut);
                        if (!responseOk)
                        {
                            return false;
                        }

                        //检查错误码
                        if(recvBuffer[11] != 0 && recvBuffer[12] != 0)
                        {
                            Console.WriteLine(string.Format("plc,站号:{0},错误码：{1}{2} ",this.plcStationNumber,recvBuffer[12],recvBuffer[11]));
                            return false;
                        }
                    }
                    return true;
                }
                else
                {
                    
                    if (blockNum > 64)
                    {
                        return false;
                        // blockNum = 64;
                    }
                    lock (sendLock)
                    {

                        if (!serialPort.IsOpen)
                        {
                            return false;
                        }

                        addr.Trim();
                        if (addr.Count() != 5)
                        {
                            return false;
                        }
                       // byte[] bufferSnd = new byte[256];
                        List<byte> bufferSnd = new List<byte>();
                    
                        bufferSnd.Add(ENQ);

                        //站号
                        bufferSnd.Add((byte)((plcStationNumber >> 4) + 0x30));
                        bufferSnd.Add((byte)((plcStationNumber & 0x0f) + 0x30));
                        //pc号
                        bufferSnd.Add(0x46);
                        bufferSnd.Add(0x46);


                        if (plcCata == EnumPlcCata.FX3U || plcCata == EnumPlcCata.ACPU)
                        {

                            addr = addr[0] + int.Parse(addr.Substring(1, addr.Length - 1)).ToString().PadLeft(4, '0');
                            //WR指令
                            bufferSnd.Add(0x57);
                            bufferSnd.Add(0x52);
                            //报文等待
                            bufferSnd.Add(datagrmWaitTime);

                            //sndByteCount = 8;
                            //地址
                            byte[] addrBytes = System.Text.UTF8Encoding.Default.GetBytes(addr);
                            bufferSnd.AddRange(addrBytes);
                        
                        }
                        else
                        {
                            addr = addr[0] + int.Parse(addr.Substring(1, addr.Length - 1)).ToString().PadLeft(6, '0');
                            //QR
                            bufferSnd.AddRange(new byte[]{ 0x51,0x52});
                            //报文等待
                            bufferSnd.Add(datagrmWaitTime);
                            byte[] addrBytes = System.Text.UTF8Encoding.Default.GetBytes(addr);
                            bufferSnd.AddRange(addrBytes);
                           
                        }

                        //软件元点数,
                        string strBlockNum = blockNum.ToString().PadLeft(2, '0');
                        byte[] blockNumBytes = System.Text.Encoding.UTF8.GetBytes(strBlockNum);
                        bufferSnd.AddRange(blockNumBytes);
                    
                        //发送之前清空接收缓冲区
                        Array.Clear(recvBuffer, 0, recvBuffer.Count());
                        recvBytes = 0;
                        recvBegin = false;
                        responseOk = false;
                        //serialPort.DiscardInBuffer();
                        //serialPort.DiscardOutBuffer();
                        serialPort.Write(bufferSnd.ToArray(), 0, bufferSnd.Count());
                        serialPort.BaseStream.Flush();
                        //Thread.Sleep(recvTimeOut);
                        recvAutoEvent.WaitOne(recvTimeOut);

                        //接收
                        if (!responseOk)
                        {
                            //string log = "通信错误：FX PLC 站号：" + plcStationNumber + ",通信超时\n";
                            //log += ("批量读取，收到字节数:" + recvBytes.ToString() + "\n");
                            //if (recvBytes > 0)
                            //{
                            //    log += ("recvBuffer[" + (recvBytes - 1).ToString() + "]=" + recvBuffer[recvBytes - 1].ToString());
                            //}

                            //AddLog(log, EnumLogType.错误);
                            //isConnect = false;
                            return false;
                        }
                        //解析
                        if (!CheckRecvAddr(recvBuffer))
                        {
                            string log = "通信错误：FX PLC 站号：" + plcStationNumber + ",应答地址解析错误";
                            //AddLog(log, EnumLogType.错误);
                            //isConnect = false;
                            return false;
                        }
                        byte[] dataBytes = GetRecvDataBytes(recvBuffer, 4 * blockNum);
                        if (dataBytes == null)
                        {
                            // isConnect = false;
                            return false;
                        }
                        vals = new short[blockNum];
                        for (int i = 0; i < blockNum; i++)
                        {
                            byte[] valBytes = new byte[4];
                            Array.Copy(dataBytes, i * 4, valBytes, 0, 4);
                            string strData = System.Text.Encoding.ASCII.GetString(valBytes);
                            vals[i] = Convert.ToInt16(strData, 16);
                        }

                        //若解析成功，则应答
                        SendAck();

                        return true;
                    }
                }
               
            }
            catch (System.Exception ex)
            {
                //AddLog("FX PLC 批量读取异常：" + ex.ToString(), EnumLogType.错误);
              //  isConnect = false;
                return false;
            }

        }
        public bool ExeBitWrite(string addr, short val)
        {
            if (string.IsNullOrWhiteSpace(addr) || addr.Length < 2)
            {
                return false;
            }
            addr = addr[0] + int.Parse(addr.Substring(1, addr.Length - 1)).ToString().PadLeft(4, '0');
            try
            {
                lock (sendLock)
                {
                    if (!serialPort.IsOpen)
                    {
                        return false;
                    }

                    addr.Trim();
                    if (addr.Count() != 5)
                    {
                        return false;
                    }
                    byte[] bufferSnd = new byte[256];
                    int sndByteCount = 0;
                    bufferSnd[0] = ENQ;

                    //站号
                    bufferSnd[1] = (byte)((plcStationNumber >> 4) + 0x30);
                    bufferSnd[2] = (byte)((plcStationNumber & 0x0f) + 0x30);
                    //pc号
                    bufferSnd[3] = 0x46;
                    bufferSnd[4] = 0x46;

                    if (plcCata == EnumPlcCata.FX3U || plcCata == EnumPlcCata.ACPU)
                    {
                        //BW令
                        bufferSnd[5] = 0x42;
                        bufferSnd[6] = 0x57;
                    }
                    else
                    {
                        //JW
                        bufferSnd[5] = 0x4A;
                        bufferSnd[6] = 0x57;
                    }
                    

                    //报文等待
                    bufferSnd[7] = datagrmWaitTime;

                    sndByteCount = 8;
                    //地址
                    byte[] addrBytes = System.Text.UTF8Encoding.Default.GetBytes(addr);
                    if (addrBytes.Count() != 5)
                    {
                        return false;
                    }
                    addrBytes.CopyTo(bufferSnd, 8);
                    sndByteCount = 13;
                    //软件元点数,01
                    bufferSnd[13] = 0x30;
                    bufferSnd[14] = 0x31;

                    sndByteCount = 15;

                    //数据区,1个字节
                    if (val > 0)
                    {
                        bufferSnd[15] = 0x31; // '1'
                    }
                    else
                    {
                        bufferSnd[15] = 0x30;//'0'
                    }
                    sndByteCount = 16;
                    //发送之前清空接收缓冲区
                    Array.Clear(recvBuffer, 0, recvBuffer.Count());
                    recvBytes = 0;
                    recvBegin = false;
                    responseOk = false;
                   // serialPort.DiscardOutBuffer();
                   // serialPort.DiscardInBuffer();
                    serialPort.Write(bufferSnd, 0, sndByteCount);
                    serialPort.BaseStream.Flush();
                    //delegteWriteCom writeComHandler = new delegteWriteCom(WriteComportHandler);
                    //writeComHandler.Invoke(bufferSnd, 0, sndByteCount);
                    //Thread.Sleep(recvTimeOut);
                    recvAutoEvent.WaitOne(recvTimeOut);
                    //接收
                    if (!responseOk)
                    {
                        return false;
                    }
                    //解析
                    if (recvBuffer[0] != ACK)
                    {
                        return false;
                    }
                    if (!CheckRecvAddr(recvBuffer))
                    {
                        return false;
                    }

                    return false;
                }
            }
            catch (System.Exception ex)
            {
                //AddLog("FX PLC 位寄存器写入异常：" + ex.ToString(), EnumLogType.错误);
                return false;
            }

        }
        public bool WriteDB(string addr, int val)
        {
            if (string.IsNullOrWhiteSpace(addr) || addr.Length < 2)
            {
                return false;
            }
            addr = addr[0] + int.Parse(addr.Substring(1, addr.Length - 1)).ToString().PadLeft(4, '0');
            return WriteMultiDB(addr, 1, new short[] { (short)val });
            /*
            try
            {
                lock (sendLock)
                {
                    if (!serialPort.IsOpen)
                    {
                        return false;
                    }

                    addr.Trim();
                    if (addr.Count() != 5)
                    {
                        return false;
                    }
                    byte[] bufferSnd = new byte[256];
                    int sndByteCount = 0;
                    bufferSnd[0] = ENQ;

                    //站号
                    bufferSnd[1] = (byte)((plcStationNumber >> 4) + 0x30);
                    bufferSnd[2] = (byte)((plcStationNumber & 0x0f) + 0x30);
                    //pc号
                    bufferSnd[3] = 0x46;
                    bufferSnd[4] = 0x46;

                    //WW令
                    bufferSnd[5] = 0x57;
                    bufferSnd[6] = 0x57;

                    //报文等待
                    bufferSnd[7] = datagrmWaitTime;

                    sndByteCount = 8;
                    //地址
                    byte[] addrBytes = System.Text.UTF8Encoding.Default.GetBytes(addr);
                    if (addrBytes.Count() != 5)
                    {
                        return false;
                    }
                    addrBytes.CopyTo(bufferSnd, 8);
                    sndByteCount = 13;
                    //软件元点数,01
                    bufferSnd[13] = 0x30;
                    bufferSnd[14] = 0x31;

                    sndByteCount = 15;

                    //数据区4个字节,将数值转换成16进制表示法的ASCII码

                    string cStr = Convert.ToString(val, 16);
                    string cStr1 = cStr.PadLeft(4, '0').ToUpper();
                    byte[] dataBytes = System.Text.UTF8Encoding.Default.GetBytes(cStr1);
                    dataBytes.CopyTo(bufferSnd, 15);
                    sndByteCount += 4;

                    //发送之前清空接收缓冲区
                    Array.Clear(recvBuffer, 0, recvBuffer.Count());
                    recvBytes = 0;
                    recvBegin = false;
                    responseOk = false;
                   // serialPort.DiscardInBuffer();
                   // serialPort.DiscardOutBuffer();
                    serialPort.Write(bufferSnd, 0, sndByteCount);
                    serialPort.BaseStream.Flush();
                    // Thread.Sleep(recvTimeOut);
                    recvAutoEvent.WaitOne(recvTimeOut);
                    //接收
                    if (!responseOk)
                    {
                       // isConnect = false;
                        return false;
                    }
                    //解析
                    if (recvBuffer[0] != ACK)
                    {
                        //isConnect = false;
                        return false;
                    }
                    if (!CheckRecvAddr(recvBuffer))
                    {
                       // isConnect = false;
                        return false;
                    }
                    return true;
                }
            }
            catch (System.Exception ex)
            {
               // AddLog("FX PLC 单字寄存器写入异常：" + ex.ToString(), EnumLogType.错误);
                return false;
            }*/

        }
        public bool WriteMultiDB(string addr, int blockNum, short[] vals)
        {
            if (string.IsNullOrWhiteSpace(addr) || addr.Length < 2)
            {
                return false;
            }
           
            if (plcCata == EnumPlcCata.FX5U)
            {
                if (blockNum > 960)
                {
                    Console.WriteLine("PLC批量写入，一次最多960个字");
                    return false;
                }
                 //4c帧,报文格式5，,二进制传输
                lock (sendLock)
                {
                    if (!serialPort.IsOpen)
                    {
                        return false;
                    }
                    addr.Trim();
                    int blockSt = int.Parse(addr.Substring(1));
                    List<byte> bufferSnd = new List<byte>();
                    bufferSnd.AddRange(new byte[] { 0x10, 0x02 }); //控制代码DLE，STX
                    bufferSnd.Add((byte)(0x12+2*blockNum));//字节数
                    bufferSnd.Add(0xF8);//帧识别号
                    //访问路径
                    bufferSnd.Add((byte)this.plcStationNumber); //站号
                    bufferSnd.Add(0x00);//网络号
                    bufferSnd.Add(0xFF); //PLC编号
                    bufferSnd.AddRange(new byte[] { 0xFF, 0x03 }); //请求目标模块IO编号
                    bufferSnd.Add(0x00); //请求目标模块站号
                    bufferSnd.Add(0x00);//本站站号（PC）

                    //请求数据
                    bufferSnd.AddRange(new byte[] { 0x01, 0x14 }); //0x1401,指令：批量写入
                    bufferSnd.AddRange(new byte[] { 0x00, 0x00 });//子指令，字为单位
                    bufferSnd.Add((byte)(blockSt & 0xff)); //起始软元件编号，三个字节，从低到高
                    bufferSnd.Add((byte)((blockSt >> 8) & 0xFF));
                    bufferSnd.Add((byte)((blockSt >> 16) & 0xFF));
                    bufferSnd.Add(0xA8); //软元件代码，D区
                    bufferSnd.Add((byte)(blockNum & 0xff)); //软元件点数，两字节
                    bufferSnd.Add((byte)((blockNum >> 8) & 0xff));
                    for (int i = 0; i < blockNum;i++ )
                    {
                        bufferSnd.AddRange(BitConverter.GetBytes(vals[i]));
                    }
                    bufferSnd.AddRange(new byte[] { 0x10, 0x03 });//控制代码，DLE ，ETX
                    //发送之前清空接收缓冲区
                    Array.Clear(recvBuffer, 0, recvBuffer.Count());
                    serialPort.Write(bufferSnd.ToArray(), 0, bufferSnd.Count());
                    serialPort.BaseStream.Flush();
                    //Thread.Sleep(recvTimeOut);
                    recvAutoEvent.WaitOne(recvTimeOut);
                    if (!responseOk)
                    {
                        return false;
                    }

                    //检查错误码
                    if (recvBuffer[11] != 0 && recvBuffer[12] != 0)
                    {
                        Console.WriteLine(string.Format("plc,站号:{0},错误码：{1}{2} ", this.plcStationNumber, recvBuffer[12], recvBuffer[11]));
                        return false;
                    }
                    return true;
                }   
            }
            else
            {
                if (blockNum > 64)
                {
                    return false;
                    //blockNum = 64;
                }
                try
                {
                    lock (sendLock)
                    {
                        if (!serialPort.IsOpen)
                        {
                            return false;
                        }

                        addr.Trim();
                        if (addr.Count() != 5)
                        {
                            return false;
                        }
                        List<byte> bufferSnd = new List<byte>();

                        bufferSnd.Add(ENQ);

                        //站号
                        bufferSnd.Add((byte)((plcStationNumber >> 4) + 0x30));
                        bufferSnd.Add((byte)((plcStationNumber & 0x0f) + 0x30));
                        //pc号
                        bufferSnd.Add(0x46);
                        bufferSnd.Add(0x46);


                        //WW令
                        if (plcCata == EnumPlcCata.FX3U || plcCata == EnumPlcCata.ACPU)
                        {
                            addr = addr[0] + int.Parse(addr.Substring(1, addr.Length - 1)).ToString().PadLeft(4, '0');
                            bufferSnd.AddRange(new byte[]{0x57,0x57});
                            //报文等待
                            bufferSnd.Add(datagrmWaitTime);

                            //sndByteCount = 8;
                            //地址
                            byte[] addrBytes = System.Text.UTF8Encoding.Default.GetBytes(addr);
                            bufferSnd.AddRange(addrBytes);
                        }
                        else
                        {
                            addr = addr[0] + int.Parse(addr.Substring(1, addr.Length - 1)).ToString().PadLeft(6, '0');
                            bufferSnd.AddRange(new byte[] { 0x51, 0x57 });
                            //报文等待
                            bufferSnd.Add(datagrmWaitTime);

                            //sndByteCount = 8;
                            //地址
                            byte[] addrBytes = System.Text.UTF8Encoding.Default.GetBytes(addr);
                            bufferSnd.AddRange(addrBytes);
                        }


                        //软件元点数,
                        string strBlockNum = blockNum.ToString().PadLeft(2, '0');
                        byte[] blockNumBytes = System.Text.Encoding.UTF8.GetBytes(strBlockNum);
                        bufferSnd.AddRange(blockNumBytes);

                      
                        //数据区,将数值转换成16进制表示法的ASCII码
                        for (int i = 0; i < blockNum; i++)
                        {
                            string cStr = Convert.ToString(vals[i], 16);
                            string cStr1 = cStr.PadLeft(4, '0').ToUpper();
                            byte[] dataBytes = System.Text.UTF8Encoding.Default.GetBytes(cStr1);
                            bufferSnd.AddRange(dataBytes);
                        }


                        //发送之前清空接收缓冲区
                        Array.Clear(recvBuffer, 0, recvBuffer.Count());
                        recvBytes = 0;
                        recvBegin = false;
                        responseOk = false;
                        // serialPort.DiscardInBuffer();
                        //serialPort.DiscardOutBuffer();
                        serialPort.Write(bufferSnd.ToArray(), 0, bufferSnd.Count());
                        serialPort.BaseStream.Flush();
                        // Thread.Sleep(recvTimeOut);
                        recvAutoEvent.WaitOne(recvTimeOut);
                        //接收
                        if (!responseOk)
                        {
                            // isConnect = false;
                            return false;
                        }
                        //解析
                        if (recvBuffer[0] != ACK)
                        {
                            // isConnect = false;
                            return false;
                        }
                        if (!CheckRecvAddr(recvBuffer))
                        {
                            isConnect = false;
                            return false;
                        }
                        return true;
                    }
                }
                catch (System.Exception ex)
                {
                    // isConnect = false;
                    return false;
                }
            }
            
        }
        public void PlcRWStatUpdate()
        {
            this.plcStatCounter++;
            if (this.plcStatCounter > long.MaxValue - 10)
            {
                this.plcStatCounter = 1;
            }
        }
        #endregion

        /// <summary>
        /// 4C帧，报文格式5的接收处理
        /// </summary>
        private void ComRecvProc2()
        {
            byte[] buf = new byte[recvMax];
            while (!recvExit)
            {
                Thread.Sleep(recvInterval);
                if (!serialPort.IsOpen || (!isConnect))
                {
                    continue;
                }
                int recvLen = 0;
                try
                {
                    recvLen = serialPort.Read(buf, 0, 128);
                    for (int i = 0; i < recvLen; i++)
                    {
                        if (recvBytes >= recvMax)
                        {
                            break;

                        }
                        switch (buf[i])
                        {
                            case STX:
                                {
                                    Array.Clear(recvBuffer, 0, recvBuffer.Count());
                                    recvBytes = 0;
                                    responseOk = false;
                                    recvBegin = true;
                                    this.currentRecvType = RecvPackageType.RECV_STX;
                                    recvBuffer[recvBytes++] = buf[i];
                                    break;
                                }
                            case ETX:
                                {
                                    responseOk = true;
                                    recvBegin = false;
                                    recvBuffer[recvBytes++] = buf[i];
                                    recvAutoEvent.Set();
                                    serialPort.DiscardInBuffer();
                                    break;
                                }
                            default:
                                {
                                    if (recvBegin)
                                    {
                                        recvBuffer[recvBytes++] = buf[i];
                                    }
                                    break;
                                }
                        }
                    }


                    if (recvLen > 0 && comRecvHandler != null)
                    {
                        byte[] recvBytesReal = new byte[recvLen];
                        Array.Copy(buf, recvBytesReal, recvLen);
                        ComRwBytesEventArgs args = new ComRwBytesEventArgs();
                        args.buf = recvBytesReal;
                        comRecvHandler.Invoke(this, args);
                    }
                }
                catch (Exception)
                {
                    serialPort.Close();
                    isConnect = false;
                    throw;
                }
            }
        }
        //1C帧的接收处理
        private void ComRecvProc()
        {
            //string strRecv = serialPort.ReadExisting();
            byte[] buf = new byte[recvMax];
            while (!recvExit)
            {
                Thread.Sleep(recvInterval);
           
                if (!serialPort.IsOpen || (!isConnect))
                {
                    continue;
                }
                int recvLen = 0;
                try
                {
                    //Console.WriteLine("begin recv:");
                    recvLen = serialPort.Read(buf, 0, 128);
                   
                    for (int i = 0; i < recvLen; i++)
                    {
                     //   Console.WriteLine("recv:0x{0}",buf[i].ToString("X2"));
                        if (recvBytes >= recvMax)
                        {
                            break;

                        }
                        switch (buf[i])
                        {
                            case STX:
                                {
                                    Array.Clear(recvBuffer, 0, recvBuffer.Count());
                                    recvBytes = 0;
                                    recvBegin = true;
                                    this.currentRecvType = RecvPackageType.RECV_STX;
                                    recvBuffer[recvBytes++] = buf[i];
                                    break;
                                }
                            case ACK:
                                {
                                    Array.Clear(recvBuffer, 0, recvBuffer.Count());
                                    recvBytes = 0;
                                    recvBegin = true;
                                    this.currentRecvType = RecvPackageType.RECV_ACK;
                                    recvBuffer[recvBytes++] = buf[i];
                                    break;
                                }
                            case ETX:
                                {
                                    responseOk = true;
                                    recvBegin = false;
                                    recvBuffer[recvBytes++] = buf[i];
                                    recvAutoEvent.Set();
                                    serialPort.DiscardInBuffer();
                                    break;
                                }
                            case NAK:
                                {
                                    //Console.Write("recv from plc: NAK " + recvLen.ToString());
                                    responseOk = false;
                                    recvAutoEvent.Set();
                                   // AddLog("FX PLC 站号:" + plcStationNumber + ",收到NAK 错误应答", EnumLogType.错误);
                                    //if (eventLinkLost != null)
                                    //{
                                    //    eventLinkLost.Invoke(this, null);
                                    //}
                                    break;
                                }
                            default:
                                {
                                    if (recvBegin)
                                    {
                                        recvBuffer[recvBytes++] = buf[i];
                                    }
                                    else
                                    {
                                        string log = string.Format("FX PLC 站号{0},通信错误,未收到STX或ACK起始标志符，收到无效数据：buf[{1}]={2}", plcStationNumber,i, buf[i]);
                                        //AddLog(log, EnumLogType.错误);
                                        serialPort.DiscardInBuffer();
                                        //if (eventLinkLost != null)
                                        //{
                                        //    eventLinkLost.Invoke(this, null);
                                        //}
                                    }
                                    if ((this.currentRecvType == RecvPackageType.RECV_ACK) && (recvBytes == 5))
                                    {
                                        //应答包结束 
                                        responseOk = true;
                                        recvBegin = false;
                                        recvAutoEvent.Set();
                                        serialPort.DiscardInBuffer();
                                    }
                                    break;
                                }
                        }

                    }
                }
                catch (System.Exception ex)
                {
                    serialPort.Close();
                    isConnect = false;
                    //AddLog("FX PLC 站号:"+plcStationNumber+",接收数据遇到异常："+ex.ToString(), EnumLogType.错误);
                    //if (eventLinkLost != null)
                    //{
                    //    eventLinkLost.Invoke(this, null);
                    //}
                }
             
                if (recvLen > 0 && comRecvHandler != null)
                {
                    byte[] recvBytesReal = new byte[recvLen];
                    Array.Copy(buf, recvBytesReal, recvLen);
                    ComRwBytesEventArgs args = new ComRwBytesEventArgs();
                    args.buf = recvBytesReal;
                    comRecvHandler.Invoke(this, args);
                }
            }
            
        }
       
        /// <summary>
        /// 验证地址是否匹配
        /// </summary>
        /// <param name="buf"></param>
        /// <returns></returns>
        private bool CheckRecvAddr(byte[] buf)
        {
            byte[] plcAddrBytes = null;
            byte[] pcAddrBytes = null;

            if (buf[0] != ACK && buf[0] != STX && buf[0] != NAK)
            {
                return false;
            }
            plcAddrBytes = new byte[2];
            plcAddrBytes[0] = buf[1];
            plcAddrBytes[1] = buf[2];
            string strPlcAddr = System.Text.Encoding.ASCII.GetString(plcAddrBytes);
            byte plcAddr = Convert.ToByte(strPlcAddr, 16);
            pcAddrBytes = new byte[2];
            pcAddrBytes[0] = buf[3];
            pcAddrBytes[1] = buf[4];
            string strPcAddr = System.Text.Encoding.ASCII.GetString(pcAddrBytes);
            byte pcAddr = Convert.ToByte(strPcAddr, 16);
            if (plcAddr != plcStationNumber || pcAddr != this.pcStationNumber)
            {
                return false;
            }
            return true;
           
            
           
        }
        private byte[] GetRecvDataBytes(byte[] buf,int strLen)
        {
            if(recvBytes< (5+strLen))
            {
                return null;
            }
            byte[] dataBytes = new byte[strLen];
            for (int i = 0; i < strLen; i++)
            {
                dataBytes[i] = buf[5 + i];
            }
            return dataBytes;
        }
        private void SendAck()
        {
            byte[] bufferSnd = new byte[5];
            bufferSnd[0] = ACK;
            bufferSnd[1] = (byte)((plcStationNumber >> 4) + 0x30);
            bufferSnd[2] = (byte)((plcStationNumber & 0x0f) + 0x30);
            bufferSnd[3] = 0x46;
            bufferSnd[4] = 0x46;
            serialPort.Write(bufferSnd, 0, 5);
           
        }
        private void WriteComportHandler(byte[] sndBuf, int offset, int DataLen)
        {
            serialPort.Write(sndBuf, offset, DataLen);
        }
        //protected void AddLog(SysLogModel log, EnumLogType logType)
        //{
        //    //if (eventLogDisp != null)
        //    //{
        //    //    LogEventArgs arg = new LogEventArgs();
        //    //    // arg.happenTime = System.DateTime.Now;
        //    //    // arg.logMes = log.logContent;
        //    //    arg.LogTime = System.DateTime.Now;
        //    //    arg.LogCate = EnumLogCategory.控制层日志;
        //    //    arg.LogContent = log.logContent;
        //    //    arg.LogType = logType;
        //    //    eventLogDisp.Invoke(this, arg);
        //    //}

        //}
        //protected void AddLog(string content, EnumLogType logType)
        //{
        //    //LogModel log = new LogModel();
        //    //log.logCategory = EnumLogCategory.控制层日志.ToString();
        //    //log.logContent = content;
        //    //log.logType = logType.ToString();
        //    //log.logTime = System.DateTime.Now;
        //    //AddLog(log, logType);
        //}
    }
}
