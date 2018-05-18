using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevInterface;
using ReaderB;
using System.Threading;

namespace DevAccess
{
    public class RfidRW_CFRH390_Tcp : IrfidRW
    {
        public string readerIP = "";
        public int readerPort = 3001;
        public byte comAddr = 0;
        public int readHandle = 0;
        private byte readerID = 0xFF;
        private const byte constReaderID = 0xFF;
        public byte ReaderID
        {
            get { return this.readerID; }
            set { this.readerID = value; }
        }
        

        private const int MAXREWORKNUM = 5;//最大操作次数
        private object lockObj = new object();
        private ISocket mySocket { get; set; }
        private AutoResetEvent recvAutoEvent = new AutoResetEvent(false);
        private const int TIMEWAITOUT = 5000;//5秒
        private const int WAITTIME = 50;
        private List<byte> recBuffer = new List<byte>();
        private Action<string> printLog;
        private bool isConnect = false;
        private byte[] SNR = new byte[4];
        public RfidRW_CFRH390_Tcp(byte id, string ip, int port, Action<string> printLog)
        {
            this.ReaderID = id;
            this.readerIP = ip;
            this.readerPort = port;
            this.mySocket = new STcpClient();
            this.mySocket.ReceiveCompleted += ReciveEventHandler;
            this.printLog = printLog;
        }

        public bool IsOpened { get { return this.isConnect; } }
        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            lock (lockObj)
            {
                string str = "";
                bool connStatus = this.mySocket.Connect(this.readerIP, this.readerPort, ref str);
                if (connStatus == false)
                {
                    return false;
                }
                return true;
            }
                 
        }
        private void LostLinkEventHandler(object sender, LostLinkEventArgs e)
        {
            
        }
        private void ReciveEventHandler(object sender, SocketEventArgs e)
        {
            this.recBuffer.AddRange(e.RecBytes);
            this.recvAutoEvent.Set();
        }
        /// <summary>
        /// 断开
        /// </summary>
        /// <returns></returns>
        public bool Disconnect()
        {
            bool re = this.mySocket.Disconnect();
            return re;
        }

        /// <summary>
        /// 读托盘号
        /// </summary>
        /// <returns></returns>
        public int ReadData()
        {
            return 0;
        }

        public byte[] ReadBytesData()
        {
            throw new NotImplementedException();
        }

        public Int64 ReadDataInt64()
        {
            throw new NotImplementedException();
        }
        public bool WriteData(int palletID)
        {
            throw new NotImplementedException();
        }
        public bool WriteDataInt64(Int64 palletID)
        {
            throw new NotImplementedException();
        }
        public bool WriteBytesData(byte[] bytesData)
        {
            return false;
        }

        public string ReadUID()//读电子标签的label ID
        {
            string barCode = "";
            try
            {
                lock (lockObj)
                {
                    string restr = "";
                    if (this.mySocket.IsConnected == false)
                    {
                        Disconnect();
                        Connect();
                    }

                    if (Request() == false)
                    {

                        return string.Empty;
                    }
                    byte[] cmdBytes = GetAnticollCmd().ToArray();
                    int reworkNum = 0;
                    this.recBuffer.Clear();
                    bool sendStatus = this.mySocket.Send(cmdBytes, ref restr);

                    if (sendStatus == false)
                    {
                        return string.Empty;
                    }
                    //this.OnLog("协议：" + DataConvert.ByteToHexStr(cmdBytes));
                    this.recvAutoEvent.Reset();
                    if (this.recvAutoEvent.WaitOne(TIMEWAITOUT) == true)
                    {
                        while (reworkNum < MAXREWORKNUM)
                        {
                            if (CheckAnticollCmd(this.recBuffer, ref barCode) == false)
                            {
                                reworkNum++;
                                OnLog("读卡接受" + reworkNum + "次，接收反馈数据：" + DataConvert.ByteToHexStr(this.recBuffer.ToArray()));
                                System.Threading.Thread.Sleep(WAITTIME);
                            }
                            else
                            {
                                OnLog("读卡成功！");
                                return barCode;
                            }

                        }
                        if (reworkNum >= MAXREWORKNUM)
                        {
                            OnLog("读卡超时，写入请求读卡反馈数据错误！");
                            return string.Empty;
                        }

                    }
                    else
                    {
                        OnLog("读卡超时！");
                        return string.Empty;
                    }
                }
                return barCode;
            }
            catch
            {
                Disconnect();
                Connect();
                return string.Empty;

            }
        }
        public string ReadStrData()
        {
            return "";
        }

        private string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
            return sb.ToString().ToUpper();
        }
        private bool GetReaderInfor()
        {
            byte[] verInfo = new byte[2];
            byte[] trType = new byte[2];
            byte readerType = 0;
            byte inventoryScanTime = 0;
            int fCmdRet = 0x30;
      
        
            fCmdRet = StaticClassReaderB.GetReaderInformation(ref this.comAddr, verInfo,
                                                    ref readerType, trType,
                                                    ref inventoryScanTime, this.readHandle);
            if(fCmdRet == 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        private bool ChangeProtocol()
        {
           // GetReaderInfor();//切换协议前先是读取地址
            int fCmdRet = 0x30;

            fCmdRet = StaticClassReaderB.ChangeTo14443A(ref this.comAddr, this.readHandle);
            if (fCmdRet == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private List<byte> GetAnticollCmd()
        {
            List<byte> anticollCmd = new List<byte>();
            byte len = 0x06;
            byte commAdr = constReaderID;
            byte cmd = 0x42;
            byte state = 0x10;
            byte data = 0;
            byte lsbCrc = 0;
            byte msbCrc = 0;
            anticollCmd.Add(len);
            anticollCmd.Add(commAdr);
            anticollCmd.Add(cmd);
            anticollCmd.Add(state);
            anticollCmd.Add(data);
            CalcuCRC(anticollCmd, ref lsbCrc, ref msbCrc);
           
            anticollCmd.Add(lsbCrc);
            anticollCmd.Add(msbCrc);
            return anticollCmd;
        }
        /// <summary>
        /// 打开磁场
        /// </summary>
        /// <returns></returns>
        private List<byte> GetOpenMagnetic()
        {
            List<byte> openMagneticCmd = new List<byte>();

            byte len = 0x05;
            byte commAdr = constReaderID;
            byte cmd = 0x00;
            byte state = 0x02;
          
            byte lsbCrc = 0;
            byte msbCrc = 0;
            openMagneticCmd.Add(len);
            openMagneticCmd.Add(commAdr);
            openMagneticCmd.Add(cmd);
            openMagneticCmd.Add(state);
           
            CalcuCRC(openMagneticCmd, ref lsbCrc, ref msbCrc);

            openMagneticCmd.Add(lsbCrc);
            openMagneticCmd.Add(msbCrc);
            return openMagneticCmd; 
        }
        private List<byte> GetRequestCmd()
        {
            List<byte> crcSource = new List<byte>();
            lock (this.lockObj)
            {
                byte len = 0x06;
                byte com_adr = constReaderID;
                byte requestCmd = 0x41;
                byte state = 0x10;
                byte mode = 1;
                byte lsbCrc = 0;
                byte msbCrc = 0;

                crcSource.Add(len);
                crcSource.Add(com_adr);
                crcSource.Add(requestCmd);
                crcSource.Add(state);
                crcSource.Add(mode);

                CalcuCRC(crcSource, ref lsbCrc, ref msbCrc);
                crcSource.Add(lsbCrc);
                crcSource.Add(msbCrc);
            }
            return crcSource;
        }
        private bool CheckAnticollCmd(List<byte> checkQeqCmd,ref string barCode)
        {
            try
            {
                if (checkQeqCmd == null || checkQeqCmd.Count < 3)
                {

                    return false;
                }
                int startIndex = 0;
                if (checkQeqCmd.Count > 9)
                {
                    for (int i = 0; i < checkQeqCmd.Count; i++)
                    {
                        if (checkQeqCmd[i] == 8 && checkQeqCmd.Count >= (i + 9))
                        {
                            startIndex += i;
                            break;
                        }
                    }
                }

                if (checkQeqCmd[startIndex] == 0x08 && checkQeqCmd[startIndex + 2] == 0x00 && checkQeqCmd.Count >= startIndex + 8)
                {
                    List<byte> crcList = checkQeqCmd.GetRange(startIndex, 7);
                    if (CRCCheck(crcList, checkQeqCmd[startIndex + 7], checkQeqCmd[startIndex + 8]) == false)
                    {
                        this.OnLog("防冲突CRC校验失败！");
                        return false;
                    }
                    //3-6序列号
                    List<byte> codeList = new List<byte>();
                    codeList.Add(checkQeqCmd[startIndex + 3]);
                    codeList.Add(checkQeqCmd[startIndex + 4]);
                    codeList.Add(checkQeqCmd[startIndex + 5]);
                    codeList.Add(checkQeqCmd[startIndex + 6]);
                    barCode = DataConvert.ByteToHexStr(codeList.ToArray());
                    return true;
                }
                else
                {
                    this.OnLog("读卡错误，接受数据：" + checkQeqCmd[0].ToString() + "-" + checkQeqCmd[1].ToString() + "-" + checkQeqCmd[2].ToString());
                    return false;
                }
            }
            catch
            {
                Disconnect();
                Connect();
                return false;
            }
            
        }
        private bool CRCCheck(List<byte> crcSouce, byte lsb, byte msb)
        {
            byte lsb_temp = 0;
            byte msb_temp = 0;
            CalcuCRC(crcSouce, ref lsb_temp,ref msb_temp);
            if(lsb== lsb_temp&&msb == msb_temp)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool CheckRequestCmd(List<byte> checkQeqCmd)
        {
            if(checkQeqCmd == null||checkQeqCmd.Count<7)
            {
                this.OnLog("结束数据长度为空或者小于1");
                return false;
            }
            if(checkQeqCmd[0] == 0x06&&checkQeqCmd[2] == 0x00)
            {
                List<byte> crcList = checkQeqCmd.GetRange(0, 5);
                if (CRCCheck(crcList, checkQeqCmd[5], checkQeqCmd[6]) == false)
                {
                    this.OnLog("请求CRC校验失败！");
                    return false;
                }
                return true;
            }
            else
            {
                this.OnLog("接受数据：" + checkQeqCmd[0].ToString() + "-" + checkQeqCmd[1].ToString() + "-" + checkQeqCmd[2].ToString());
                return false;
            }

        }
             


        private bool Request()
        {
            try
            {
               
                string restr = "";
                int reworkNum = 0;
                this.recBuffer.Clear();
                List<byte> requseCmd = GetRequestCmd();
                bool sendStatus = this.mySocket.Send(requseCmd.ToArray(), ref restr);

                if (sendStatus == false)
                {
                   
                    return false;
                }
                //this.OnLog("协议：" + DataConvert.ByteToHexStr(cmdBytes));
                this.recvAutoEvent.Reset();
                if (this.recvAutoEvent.WaitOne(TIMEWAITOUT) == true)
                {
                    while (reworkNum < MAXREWORKNUM)
                    {
                        if (CheckRequestCmd(this.recBuffer) == false)
                        {
                            reworkNum++;
                            OnLog("请求读卡"+reworkNum+"次，接收反馈数据：" + DataConvert.ByteToHexStr(this.recBuffer.ToArray()));
                            System.Threading.Thread.Sleep(WAITTIME);
                        }
                        else
                        {
                            OnLog("请求成功！");
                            return true;
                        }
                       
                    }
                    if (reworkNum >= MAXREWORKNUM)
                    {
                        OnLog("写入请求读卡反馈数据错误！");
                        List<byte> openMagnetic = GetOpenMagnetic();
                        bool sendopenMagneticStatus = this.mySocket.Send(openMagnetic.ToArray(), ref restr);//打开磁场，这里不做校验了直接下发
                        if(sendopenMagneticStatus == true)
                        {
                            OnLog("发送打开磁场指令成功！");
                        }
                        else
                        {
                            OnLog("发送打开磁场指令失败！");
                        }
                        return false;
                    }
                    
                }
                else
                {
                    OnLog("请求读卡反馈数据超时！");
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }

        }
     
        private void CalcuCRC(List<byte> crcSouce,ref byte lsbCRC,ref byte msbCRC)
        {
            UInt32 ployNomal = 0x8408;
            UInt32 currentCrcValue = 0xffff;
            for( int i=0;i<crcSouce.Count();i++)
            {
                currentCrcValue = currentCrcValue ^ ((UInt32)crcSouce[i]);
                for(int j=0;j<8;j++)
                {
                    if ((currentCrcValue&0x0001)==1)
                    {
                        currentCrcValue = (currentCrcValue >> 1) ^ ployNomal;
                    }
                    else
                    {
                        currentCrcValue = (currentCrcValue >> 1);
                    }

                }
            }

            lsbCRC = (byte)(currentCrcValue & 0x00ff);
            msbCRC = (byte)((currentCrcValue >> 8) & 0x00ff);

        }


        private void OnLog(string logStr)
        {
            if (this.printLog != null)
            {
                this.printLog(logStr);
            }
        }
    }
}
