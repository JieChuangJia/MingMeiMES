using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DevInterface;
using System.Threading;
using System.Net.NetworkInformation;
namespace DevAccess
{
    public class LogEventArgs:EventArgs
    {
        public string LogStr { get; set; }
    }
    public enum EnumAddrArea
    {
        D,
        W
    }
    public class OmlPlcRW : IPlcRW,IPlcRWEtd
    {
        public string PlcRole { get; set; }
        private   object lockObj = new object();
        public EnumPLCType PlcType { get; set; }
        public string PLCIP { get; set; }
        public int PLCPort { get; set; }
       public string PCIP { get; set; }
       
        private Int64 plcStatCounter = 0;
        private ISocket mySocket { get; set; }

        private const int MAXREWORKNUM = 5;//最大操作次数
        
        private IOMLFins omlFinsPtl = null;

        private AutoResetEvent recvAutoEvent = new AutoResetEvent(false);

        private const int TIMEWAITOUT = 5000;//5秒
        private const int WAITTIME = 50;
        private List<byte> recBuffer = new List<byte>();
        private Action<string> printLog;
        #region 实现IPLCRW接口
        public EventHandler<LogEventArgs> eventLog;
        public Int64 PlcStatCounter { get{return this.plcStatCounter;} }
        /// <summary>
        /// PLC id，自定义
        /// </summary>
        public int PlcID { get; set; }
        /// <summary>
        /// 是否处于连接状态
        /// </summary>
        public bool IsConnect { get { return this.mySocket.IsConnected; } }

        public int StationNumber { get; set; }
        public Int16[] Db1Vals { get; set; }
        public Int16[] Db2Vals { get; set; }

        public OmlPlcRW(EnumPLCType plcType, string plcIP, int plcPort, string pcIP,Action<string> printLog)
        {
            this.PlcType = plcType;
            this.PLCIP = plcIP;
            this.PCIP = pcIP;
            this.PLCPort = plcPort;
            if(this.PlcType== EnumPLCType.OML_TCP)
            {
                this.mySocket = new STcpClient();
                this.omlFinsPtl = new OMLFinsTCPProtocol(this.PCIP, this.PLCIP);
            }
            else
            {
                this.mySocket = new SUdpClient();
                this.omlFinsPtl = new OMLFinsUDPProtocol(this.PCIP, this.PLCIP);
            }
            this.mySocket.ReceiveCompleted += ReciveEventHandler;
            this.mySocket.LostLink += LostLinkEventHandler;
            this.printLog = printLog;
        }

        /// <summary>
        /// 连接PLC
        /// </summary>
        /// <param name="plcAddr">plc地址</param>
        /// <param name="reStr"></param>
        /// <returns></returns>
        public bool ConnectPLC(ref string reStr)
        {
            try
            {


                lock (lockObj)
                {
                    int reworkNum = 0;
                    if (this.mySocket == null)
                    {
                        reStr = "通讯对象为空！连接失败！";
                        return false;
                    }
                    if (this.PlcType == EnumPLCType.OML_TCP)
                    {
                        bool connStatus = this.mySocket.Connect(this.PLCIP, this.PLCPort, ref reStr);
                        if (connStatus == false)
                        {
                            return false;
                        }
                        byte[] handCmd = null;
                        //发送fins握手协议
                        bool handCmdSta = this.omlFinsPtl.HandCmd(ref handCmd, ref reStr);
                        if (handCmdSta == false)
                        {
                            reStr += "：获取握手协议失败！";

                            return false;
                        }
                        this.OnLog("发送握手协议：" + DataConvert.ByteToHexStr(handCmd));
                        this.recBuffer.Clear();//每次发送前要清空缓存
                        bool sendSta = this.mySocket.Send(handCmd, ref reStr);
                        if (sendSta == false)
                        {
                            reStr += ":数据发送失败！";
                            return false;
                        }
                        if (this.recvAutoEvent.WaitOne(TIMEWAITOUT) == true)
                        {
                            while (reworkNum < MAXREWORKNUM)
                            {
                                if (this.omlFinsPtl.HandCmdResponse(this.recBuffer.ToArray(), ref reStr) == false)
                                {
                                    reworkNum++;
                                    OnLog("接收握手数据：" + DataConvert.ByteToHexStr(this.recBuffer.ToArray()));
                                    System.Threading.Thread.Sleep(WAITTIME);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            if (reworkNum >= MAXREWORKNUM)
                            {
                                OnLog("握手协议反馈数据错误！");
                                return false;
                            }
                            return true;
                        }
                        else
                        {
                            reStr = "FINS 握手反馈超时！";
                            return false;
                        }
                    }
                    else if (this.PlcType == EnumPLCType.OML_UDP)
                    {
                        bool connStatus = this.mySocket.Connect(this.PLCIP, this.PLCPort, ref reStr);//不需要握手了
                        if (connStatus == false)
                        {
                            return false;
                        }
                        reStr = "连接成功！";
                        return true;
                    }
                    else
                    {
                        reStr = "plc类型错误！";
                        return false;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("PLC底层数据连接接口错误：" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <returns></returns>
        public bool CloseConnect()
        {
            bool re = this.mySocket.Disconnect();
            return re;
        }

        /// <summary>
        /// 读PLC 数据
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public bool ReadDB(string addr, ref int val)
        {
            short[] value= null;
            bool status = ReadMultiDB(addr, 1, ref value);
            if(status == false)
            {
                return false;
            }
            if(value==null)
            {
                return false;
            }
            if(value.Count() ==0)
            {
                return false;
            }
            val = value[0];
            return true;
        }
        public bool PingIp(string strIpOrDName)
        {
            try
            {
                Ping objPingSender = new Ping();
                PingOptions objPinOptions = new PingOptions();
                objPinOptions.DontFragment = true;
                string data = "";
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                int intTimeout = 120;
                PingReply objPinReply = objPingSender.Send(strIpOrDName, intTimeout, buffer, objPinOptions);
                string strInfo = objPinReply.Status.ToString();
                if (strInfo == "Success")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool ReadMultiDB(string addr, int blockNum, ref short[] vals)
        {
            try
            {


                lock (lockObj)
                {
                    if (addr.Length < 1)
                    {
                        return false;
                    }
                    string addrArea = addr.Substring(0, 1).ToUpper();
                    addr = addr.Substring(1, addr.Length - 1);
                    string restr = "";
                    byte[] cmdBytes = null;
                    int reworkNum = 0;
                    if (addrArea == EnumAddrArea.D.ToString())
                    {
                        //获取读取地址指令
                        bool readCmd = this.omlFinsPtl.ReadDMAddrCmd(short.Parse(addr), (short)blockNum, ref cmdBytes, ref restr);
                        if (readCmd == false)
                        {
                            return false;
                        }
                        this.recBuffer.Clear();
                        bool sendStatus = this.mySocket.Send(cmdBytes, ref restr);

                        if (sendStatus == false)
                        {
                            return false;
                        }
                        this.OnLog("协议：" + DataConvert.ByteToHexStr(cmdBytes));
                        this.recvAutoEvent.Reset();
                        if (this.recvAutoEvent.WaitOne(TIMEWAITOUT) == true)
                        {

                            while (reworkNum < MAXREWORKNUM)
                            {
                                if (this.omlFinsPtl.ReadDMAddrCmdResponse(this.recBuffer.ToArray(), (short)blockNum, ref vals, ref restr) == false)
                                {
                                    reworkNum++;
                                    OnLog("接收反馈数据：" + DataConvert.ByteToHexStr(this.recBuffer.ToArray()));
                                    System.Threading.Thread.Sleep(WAITTIME);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            if (reworkNum >= MAXREWORKNUM)
                            {
                                OnLog("写入数据反馈数据错误！");
                                return false;
                            }
                            return true;

                        }
                        else
                        {
                            OnLog("读取超时！");
                            //if(!PingIp(this.PLCIP) )//在外部控制断开连接
                            //{
                            //    Console.WriteLine("PLC:{0}网络不通，将尝试重连", this.PLCIP);
                            //}
                            //string reStr = "";
                            //if(!ConnectPLC(ref reStr))
                            //{
                            //    Console.WriteLine("连接PLC{0}失败", this.PLCIP);
                            //}
                            //else
                            //{
                            //    Console.WriteLine("连接PLC{0}成功", this.PLCIP);
                            //}
                            return false;
                        }
                        //指令发出后要等待接收可用信号
                    }
                    else if (addrArea == EnumAddrArea.W.ToString())
                    {
                        //获取读取地址指令
                        bool readCmd = this.omlFinsPtl.ReadCIOAddrCmd(float.Parse(addr), (short)blockNum, ref cmdBytes, ref restr);
                        if (readCmd == false)
                        {
                            return false;
                        }
                        this.recBuffer.Clear();
                        bool sendStatus = this.mySocket.Send(cmdBytes, ref restr);

                        if (sendStatus == false)
                        {
                            return false;
                        }
                        this.OnLog("协议：" + DataConvert.ByteToHexStr(cmdBytes));
                        this.recvAutoEvent.Reset();
                        if (this.recvAutoEvent.WaitOne(TIMEWAITOUT) == true)
                        {

                            while (reworkNum < MAXREWORKNUM)
                            {
                                if (this.omlFinsPtl.ReadCIOAddrCmdResponse(this.recBuffer.ToArray(), (short)blockNum, ref vals, ref restr) == false)
                                {
                                    reworkNum++;
                                    OnLog("接收反馈数据：" + DataConvert.ByteToHexStr(this.recBuffer.ToArray()));
                                    System.Threading.Thread.Sleep(WAITTIME);
                                }
                                else
                                {
                                    return true;
                                }
                            }
                            if (reworkNum >= MAXREWORKNUM)
                            {
                                OnLog("写入数据反馈数据错误！");
                                return false;
                            }
                        }
                        else
                        {
                            OnLog("读取超时！");
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                    return true;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("底层PLC读接口错误：" + ex.Message);
                return false;
                     
            }
        }

        /// <summary>
        /// 写PLC数据
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="?"></param>
        /// <returns></returns>
       public bool WriteDB(string addr, int val)
       {
           short[] value=new short[1];
           value[0] =(short)val;
           return WriteMultiDB(addr,1,value);
       }
       public bool WriteMultiDB(string addr, int blockNum, short[] vals)
       {
           try
           {


               lock (lockObj)
               {
                   //int reworkNum = 0;
                   //EnumAddrArea addrArea = EnumAddrArea.DM;
                   //string restr = "";
                   //bool isAddr = AddrCheck(addr, ref addrArea);
                   //byte[] cmdBytes = null;
                   //if (isAddr == false)
                   //{
                   //    return false;
                   //}

                   string addrArea = addr.Substring(0, 1).ToUpper();
                   addr = addr.Substring(1, addr.Length - 1);
                   string restr = "";
                   byte[] cmdBytes = null;
                   int reworkNum = 0;
                   if (addrArea == EnumAddrArea.D.ToString())
                   {
                       //获取读取地址指令
                       bool readCmd = this.omlFinsPtl.WriteDMAddrCmd(short.Parse(addr), vals, ref cmdBytes, ref restr);
                       if (readCmd == false)
                       {
                           return false;
                       }
                       this.recBuffer.Clear();
                       bool sendStatus = this.mySocket.Send(cmdBytes, ref restr);

                       if (sendStatus == false)
                       {
                           return false;
                       }
                       this.OnLog("协议：" + DataConvert.ByteToHexStr(cmdBytes));
                       this.recvAutoEvent.Reset();
                       if (this.recvAutoEvent.WaitOne(TIMEWAITOUT) == true)
                       {
                           while (reworkNum < MAXREWORKNUM)
                           {
                               if (this.omlFinsPtl.WriteDMAddrCmdResponse(this.recBuffer.ToArray(), ref restr) == false)
                               {
                                   reworkNum++;
                                   OnLog("接收反馈数据：" + DataConvert.ByteToHexStr(this.recBuffer.ToArray()));
                                   System.Threading.Thread.Sleep(WAITTIME);
                               }
                               else
                               {

                                   return true;
                               }
                           }
                           if (reworkNum >= MAXREWORKNUM)
                           {
                               OnLog("写入数据反馈数据错误！");
                               return false;
                           }

                       }
                       else
                       {
                           OnLog("写入超时！");
                           return false;
                       }
                       //指令发出后要等待接收可用信号
                   }
                   else if (addrArea == EnumAddrArea.W.ToString())
                   {
                       //获取读取地址指令

                       bool readCmd = this.omlFinsPtl.WriteCIOAddrCmd(float.Parse(addr), vals, ref cmdBytes, ref restr);
                       if (readCmd == false)
                       {
                           return false;
                       }
                       this.recBuffer.Clear();
                       bool sendStatus = this.mySocket.Send(cmdBytes, ref restr);
                       if (sendStatus == false)
                       {
                           return false;
                       }
                       this.OnLog("协议：" + DataConvert.ByteToHexStr(cmdBytes));
                       this.recvAutoEvent.Reset();
                       if (this.recvAutoEvent.WaitOne(TIMEWAITOUT) == true)
                       {
                           while (reworkNum < MAXREWORKNUM)
                           {
                               if (this.omlFinsPtl.WriteDMAddrCmdResponse(this.recBuffer.ToArray(), ref restr) == false)
                               {
                                   reworkNum++;
                                   OnLog("接收反馈数据：" + DataConvert.ByteToHexStr(this.recBuffer.ToArray()));
                                   System.Threading.Thread.Sleep(WAITTIME);
                               }
                               else
                               {
                                   return true;
                               }
                           }
                           if (reworkNum >= MAXREWORKNUM)
                           {
                               OnLog("写入数据反馈数据错误！");
                               return false;
                           }

                       }
                       else
                       {
                           OnLog("接收超时！");
                           return false;
                       }
                   }
                   return true;
               }
           }
           catch(Exception ex)
           {
               Console.WriteLine("底层PLC写接口错误：" + ex.Message);
               return false;
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
       #region 实现IPlcEd扩展接口
       public bool ReadMultiDB(string addr, int blockNum, ref int[] vals)
       {
           try
           {


               lock (lockObj)
               {
                   if (addr.Length < 1)
                   {
                       return false;
                   }
                   string addrArea = addr.Substring(0, 1).ToUpper();
                   addr = addr.Substring(1, addr.Length - 1);
                   string restr = "";
                   byte[] cmdBytes = null;
                   int reworkNum = 0;
                   if (addrArea == EnumAddrArea.D.ToString())
                   {
                       //获取读取地址指令,32位读取4个字节

                       bool readCmd = this.omlFinsPtl.ReadDMAddrCmd(short.Parse(addr), (short)(blockNum * 2), ref cmdBytes, ref restr);
                       if (readCmd == false)
                       {
                           return false;
                       }
                       this.recBuffer.Clear();
                       bool sendStatus = this.mySocket.Send(cmdBytes, ref restr);

                       if (sendStatus == false)
                       {
                           return false;
                       }
                       this.OnLog("协议：" + DataConvert.ByteToHexStr(cmdBytes));
                       this.recvAutoEvent.Reset();
                       if (this.recvAutoEvent.WaitOne(TIMEWAITOUT) == true)
                       {

                           while (reworkNum < MAXREWORKNUM)
                           {
                               if (this.omlFinsPtl.ReadDMAddrCmdResponse(this.recBuffer.ToArray(), (short)blockNum, ref vals, ref restr) == false)
                               {
                                   reworkNum++;
                                   OnLog("接收反馈数据：" + DataConvert.ByteToHexStr(this.recBuffer.ToArray()));
                                   System.Threading.Thread.Sleep(WAITTIME);
                               }
                               else
                               {
                                   break;
                               }
                           }
                           if (reworkNum >= MAXREWORKNUM)
                           {
                               OnLog("写入数据反馈数据错误！");
                               return false;
                           }
                           return true;

                       }
                       else
                       {
                           OnLog("读取超时！");
                           return false;
                       }
                       //指令发出后要等待接收可用信号
                   }
                   else if (addrArea == EnumAddrArea.W.ToString())
                   {
                       return false;//目前针对明美项目只实现了D数据的读写
                   }
                   else
                   {
                       return false;
                   }

               }
           }
           catch(Exception ex)
           {
               Console.WriteLine("底层PLC扩展读接口错误：" + ex.Message);
               return false;
           }
       }


       public bool WriteMultiDB(string addr, int blockNum, int[] vals)
       {
           try
           {
               lock (lockObj)
               {

                   string addrArea = addr.Substring(0, 1).ToUpper();
                   addr = addr.Substring(1, addr.Length - 1);
                   string restr = "";
                   byte[] cmdBytes = null;
                   int reworkNum = 0;
                   if (addrArea == EnumAddrArea.D.ToString())
                   {
                       //获取读取地址指令
                       bool readCmd = this.omlFinsPtl.WriteDMAddrCmd(short.Parse(addr), vals, ref cmdBytes, ref restr);
                       if (readCmd == false)
                       {
                           return false;
                       }
                       this.recBuffer.Clear();
                       bool sendStatus = this.mySocket.Send(cmdBytes, ref restr);

                       if (sendStatus == false)
                       {
                           return false;
                       }
                       this.OnLog("协议：" + DataConvert.ByteToHexStr(cmdBytes));
                       this.recvAutoEvent.Reset();
                       if (this.recvAutoEvent.WaitOne(TIMEWAITOUT) == true)
                       {
                           while (reworkNum < MAXREWORKNUM)
                           {
                               if (this.omlFinsPtl.WriteDMAddrCmdResponse(this.recBuffer.ToArray(), ref restr) == false)
                               {
                                   reworkNum++;
                                   OnLog("接收反馈数据：" + DataConvert.ByteToHexStr(this.recBuffer.ToArray()));
                                   System.Threading.Thread.Sleep(WAITTIME);
                               }
                               else
                               {

                                   return true;
                               }
                           }
                           if (reworkNum >= MAXREWORKNUM)
                           {
                               OnLog("写入数据反馈数据错误！");
                               return false;
                           }

                       }
                       else
                       {
                           OnLog("写入超时！");
                           return false;
                       }
                       //指令发出后要等待接收可用信号
                   }
                   else if (addrArea == EnumAddrArea.W.ToString())
                   {

                       return true;
                       //暂未实现
                   }
                   else
                   {
                       return false;
                   }
                   return true;
               }
           }
           catch(Exception ex)
           {
               Console.WriteLine("底层PLC扩展写接口错误：" + ex.Message);
               return false;
           }
       }
      
       #endregion
       #region 私有函数
       private void OnLog(string logStr)
       {
            if(this.printLog!=null)
            {
                this.printLog(logStr);
            }
       }
        private void ReciveEventHandler(object sender,SocketEventArgs e)
        {
            this.recBuffer.AddRange(e.RecBytes);
            this.recvAutoEvent.Set();
        }
        private void LostLinkEventHandler(object sender,    LostLinkEventArgs e)
        {
            string str ="";
            OnLog("网络断开!");
           //if( this.ConnectPLC(ref str)==false)
           //{
           //    OnLog("重新连接失败！");
           //}
           //else
           //{
           //    OnLog("重新连接成功！");
           //}
        }
        //private bool AddrCheck(string addr,ref EnumAddrArea addrArea)
        //{
        //    string regularInt = "^[0-9]*[1-9][0-9]*$";
        //    string regularFloat = "^(([0-9]+\\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\\.[0-9]+)|([0-9]*[1-9][0-9]*))$";
        //    Regex regexInt = new Regex(regularInt);
        //    Regex regexFloat = new Regex(regularFloat);

        //    bool isInt = regexInt.IsMatch(addr);
        //    if(isInt == true)
        //    {
        //        addrArea = EnumAddrArea.DM;
        //        return true;
        //    }
           
        //    bool isFloat = regexFloat.IsMatch(addr);
        //    if(isFloat == true)
        //    {
        //        addrArea = EnumAddrArea.CIO;
        //        return true;
        //    }
        //    if(isInt==false&&isInt==false)
        //    {
        //        return false;
        //    }
        //    return false;
        //}
       #endregion
    }
}
