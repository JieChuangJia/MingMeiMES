using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;
using DevInterface;
namespace DevAccess
{
    // 定义 UdpState类
    internal class WqrfidUdpState
    {
        public UdpClient udpClient;
        public IPEndPoint ipEndPoint;
        //public const int BufferSize = 1024;
        //public byte[] buffer = new byte[BufferSize];
        //public int counter = 0;
    }
    internal class WqrfidCmdPackage
    {
        public byte BootCode = 0;
        public byte Length = 0;
        public byte Cmd = 0;
        public List<byte> data;
        public byte checkSum = 0;
        public WqrfidCmdPackage()
        {
            data = new List<byte>();
        }
    }
    public class WqRfidUdp:IrfidRW
    {
        #region 数据区 
        private EnumTag tagType = EnumTag.TagEPCC1G2;
        private string readerIP = "";
        private uint readerPort = 0;
        private uint hostPort = 6890;
        private byte readerID = 0x00; //读写器ID
        private UdpClient udpClient = null;
        private IPEndPoint udpRemotePoint = null;
        private NetworkStream workStream = null;
        private List<byte> recBuffer = new List<byte>();  //接收缓存
        private const int recvTimeOut = 2000;          //发送出去之后，等待接收完毕，之间的最大时间间隔
        private const int connTimeOut = 5;  //连接超时，单位：秒
        private object lockObj = new object();// 多线程锁
        private object bufLock = new object();
        public byte[] AccPaswd { get; set; }
        public byte ReaderID
        {
            get
            {
                return readerID;
            }
            set
            {
                readerID = value;
            }
        }
        public string ReaderIP { get { return readerIP; } set { readerIP = value; } }
        public uint ReaderPort { get { return readerPort; } set { readerPort = value; } }
        public string readerNmae { get; set; }
        public bool IsOpened { get; set; }
        #endregion
        public WqRfidUdp(EnumTag tagType, byte readerID, string ip, uint port,uint hostPort)
        {
            this.readerID = readerID;
            this.tagType = tagType;
            this.readerIP = ip;
            this.readerPort = port;
            AccPaswd = new byte[4] { 0, 0, 0, 0 };
            IsOpened = false;
            this.hostPort = hostPort;
        }
        public bool Connect()
        {
            try
            {
                int tryMax = 20;
                int counter = 0;
                while (PortInUse((int)hostPort))
                {
                    if (hostPort > 10000)
                    {
                        hostPort = 5000;
                    }
                    hostPort += 1;
                    counter++;
                    if (counter > tryMax)
                    {
                        Console.WriteLine("{0}RFID连接失败，端口占用，自动分配方案失败", readerIP);
                        return false;
                    }
                }
                if (udpClient == null)
                {
                    udpClient = new UdpClient((int)hostPort);
                }
                this.udpRemotePoint = new IPEndPoint(IPAddress.Parse(readerIP), (int)readerPort);
                WqrfidUdpState udpReceiveState = new WqrfidUdpState();
                udpReceiveState.ipEndPoint = this.udpRemotePoint;
                udpReceiveState.udpClient = udpClient;
               // udpClient.Connect(this.udpRemotePoint);
                udpClient.BeginReceive(UdpRecvCallback, udpReceiveState);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            
        }
        public bool Disconnect()
        {
            udpClient.Close();
            udpClient = null;
            return true;
        }
        public bool PortInUse(int port)
        {

            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();
            foreach (IPEndPoint endPoint in ipEndPoints)
            {
                if (endPoint.Port == port)
                {
                    return true;

                }
            }
            ipEndPoints = ipProperties.GetActiveUdpListeners();
            foreach (IPEndPoint endPoint in ipEndPoints)
            {
                if (endPoint.Port == port)
                {
                    return true;

                }
            }
            return false;
        }
        public byte[] ReadEPC()
        {
            try
            {
                lock (lockObj)
                {

                    byte[] cmdBytes = new byte[] { 0x40, 0x06, 0xEE, 0x01, 0x00, 0x00, 0x00, 0xCB };
                    if (!SendData(cmdBytes))
                    {
                        return null;
                    }
                    WqrfidCmdPackage package = WaitRecvPackage(recvTimeOut);
                    if (package == null)
                    {

                        Console.WriteLine("{0}读EPC失败,接收到的数据：{1}",this.readerIP,BytesToString(this.recBuffer.ToArray()));
                        return null;
                    }
                    if (package.BootCode != 0xF0)
                    {
                        Console.WriteLine("{0}读EPC失败,数据错误：{1}", this.readerIP, BytesToString(this.recBuffer.ToArray()));
                        return null;
                    }
                    if (package.Cmd != 0xEE)
                    {
                        Console.WriteLine("{0}读EPC失败,功能码错误,理论上功能码{1},实际：{2}",this.readerIP, "EE", package.Cmd.ToString("X2"));
                        return null;
                    }
                    List<byte> epcList = new List<byte>();
                    int epcLen = package.data[1] * 2;
                    if (package.data.Count() < epcLen + 2)
                    {
                        Console.WriteLine("{0}读EPC失败，EPC理论长度{1}，实际：{2}", this.readerIP,epcLen, package.data.Count() - 2);
                        return null;
                    }
                    for (int i = 0; i < epcLen+1; i++)
                    {
                        epcList.Add(package.data[1 + i]);
                    }
                    return epcList.ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public string ReadStrData()
        {
            byte[] epcBytes = ReadEPC();
            if(epcBytes == null || epcBytes.Count()<1)
            {
                return string.Empty;
            }
            int epcLen = epcBytes[0] * 2;
            byte[] reBytes = new byte[epcLen];
            Array.Copy(epcBytes, 1, reBytes, 0, epcLen);
            string str = System.Text.Encoding.UTF8.GetString(reBytes);
            str = str.Trim();
            return str;
        }
        public string ReadUID()
        {
            //throw new NotImplementedException();
            try
            {
                lock(lockObj)
                {
                    string strUID = "";
                    byte[] epcBytes = ReadEPC();
                    if (epcBytes == null || epcBytes.Count() < 2)
                    {
                        return string.Empty;
                    }
                    byte epcWordLen=epcBytes[0];
                    byte mem = 2;
                    byte addr = 0;
                    byte len = 6; //word长度
                    List<byte> cmdBytes = new List<byte>();
                    cmdBytes.Add(0x40);
                    cmdBytes.Add((byte)(10 + epcWordLen*2));
                    cmdBytes.Add(0xEC);
                    cmdBytes.AddRange(epcBytes);
                    cmdBytes.Add(mem);
                    cmdBytes.Add(addr);
                    cmdBytes.Add(len);
                    cmdBytes.AddRange(AccPaswd);
                    byte check = CheckSum(cmdBytes.ToArray());
                    cmdBytes.Add(check);
                    if (!SendData(cmdBytes.ToArray()))
                    {
                        return null;
                    }
                    WqrfidCmdPackage package = WaitRecvPackage(recvTimeOut);
                    if (package == null)
                    {

                        Console.WriteLine("{0}读EPC失败,接收到的数据：{1}", this.readerIP, BytesToString(this.recBuffer.ToArray()));
                        return null;
                    }
                    if (package.BootCode != 0xF0)
                    {
                        Console.WriteLine("{0}读EPC失败,数据错误：{1}", this.readerIP, BytesToString(this.recBuffer.ToArray()));
                        return null;
                    }
                    if (package.Cmd != 0xEC)
                    {
                        Console.WriteLine("{0}读EPC失败,功能码错误,理论上功能码{1},实际：{2}", this.readerIP, "EE", package.Cmd.ToString("X2"));
                        return null;
                    }
                    strUID = "";
                    foreach(byte b in package.data)
                    {
                        strUID += b.ToString("X2");
                    }
                    return strUID;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
           
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
            throw new NotImplementedException();
        }
        public int ReadData()//ref byte[] recvByteArray
        {
            throw new NotImplementedException();
        }
        public byte[] ReadBytesData()
        {
            throw new NotImplementedException();
        }
        public string BytesToString(byte[] byteStream)
        {
            string str = "";
            for (int i = 0; i < byteStream.Count(); i++)
            {
                str += (" " +byteStream[i].ToString("X2"));
            }
            return str;
        }
        #region 内部功能
        private void UdpRecvCallback(IAsyncResult iar)
        {
            WqrfidUdpState udpState = iar.AsyncState as WqrfidUdpState;
            if(iar.IsCompleted)
            {
                Byte[] recBytes = udpState.udpClient.EndReceive(iar, ref udpState.ipEndPoint);
                if (recBytes != null && recBytes.Count() > 0)
                {
                    lock(bufLock)
                    {
                        this.recBuffer.AddRange(recBytes);
                    }
                    //Console.Write("Recv:");
                    //for(int i=0;i<recBytes.Count();i++)
                    //{
                    //    Console.Write("{0}", recBytes[i].ToString("X2"));
                    //}
                    //Console.WriteLine("");

                }

            }
            udpClient.BeginReceive(UdpRecvCallback, udpState);

        }
        
        private WqrfidCmdPackage WaitRecvPackage(int timeOut)
        {
            DateTime st = System.DateTime.Now;
            if (timeOut > 5000)
            {
                timeOut = 5000;
            }
            string reStr = "";
            while(true)
            {
                Thread.Sleep(5);
                WqrfidCmdPackage package = ParseRecvData(ref reStr);
                DateTime cur = System.DateTime.Now;
                TimeSpan ts = cur - st;
                if (ts.TotalMilliseconds > timeOut)
                {
                    reStr = "PLC返回超时," + timeOut + "毫秒"+reStr;
                    Console.WriteLine(reStr);
                    return null;
                }
                if(package != null)
                {
                    return package;
                }
            }
            return null;
        }
        private bool SendData(byte[] sendData)
        {
            lock(bufLock)
            {
                this.recBuffer.Clear();//发送请求之前清缓存
            }
            int re = this.udpClient.Send(sendData, sendData.Count(), this.udpRemotePoint);
            if (re > 0)
            {
                return true;
            }
            return false;
        }
        private WqrfidCmdPackage ParseRecvData(ref string reStr)
        {
             lock(bufLock)
             {
                 if(this.recBuffer.Count()<4)
                 {
                     reStr="接收数据长度不足4字节";
                     return null;
                 }
                 byte bootCode = this.recBuffer[0];
                 byte len = this.recBuffer[1];
                 byte cmd = this.recBuffer[2];
                
                 int sumLen = len+2;
                 byte check = this.recBuffer[sumLen - 1];
                 if(this.recBuffer.Count()<sumLen)
                 {
                     reStr = string.Format("接收数据长度不足,理论长度：{0}",len+2);
                     return null;
                 }
                 byte[] packageSum = new byte[sumLen-1];
                 for(int i=0;i<sumLen-1;i++)
                 {
                     packageSum[i] = this.recBuffer[i];
                 }
                 byte checkSum = CheckSum(packageSum);
                 if(checkSum != check)
                 {
                     reStr = string.Format("校验和错误，理论值：{0},接收到的值:{1}", checkSum, check);
                     return null;
                 }
                 WqrfidCmdPackage package = new WqrfidCmdPackage();
                 package.BootCode = bootCode;
                 package.Length = len;
                 package.Cmd = cmd;
                
                 for(int i=0;i<len-2;i++)
                 {
                     package.data.Add(recBuffer[i + 3]);
                 }
                 package.checkSum = checkSum;
                 return package;
             }
        }
        private byte CheckSum(byte[] byteData)
        {
            if(byteData == null|| byteData.Count()<1)
            {
                return 0;
            }
            int checkSum = 0;
            for(int i=0;i<byteData.Count();i++)
            {
                checkSum += byteData[i];
            }
            checkSum = ~checkSum;
            checkSum += 1;
            checkSum = checkSum & 0xff;
            return (byte)checkSum;
        }
        #endregion
    }
}
