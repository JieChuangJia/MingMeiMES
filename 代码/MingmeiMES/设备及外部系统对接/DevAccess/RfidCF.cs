using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;
using DevInterface;
namespace DevAccess
{
    public class RfidCF : IrfidRW
    {
        // Fields
        private string bufRfidUID = "";
        private ManualResetEvent connectDone = new ManualResetEvent(false);
        private bool isConnect;
        private byte readerID;
        public string readerIP = "";
        public int readerPort = 0x1770;
        private List<byte> recvBuffer = new List<byte>();
        private object recvBufLock = new object();
        private TcpClient tcpClient;
        private NetworkStream workStream;

        // Methods
        public RfidCF(byte id, string ip, int port)
        {
            this.readerID = id;
            this.readerIP = ip;
            this.readerPort = port;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="len"></param>
        /// <param name="space">1:输出16进制字符加空格</param>
        /// <returns></returns>
        public static string bytes2hexString(byte[] data, int len, int space)
        {
            string str = "";
            string str2 = "";
            if (space == 1)
            {
                str2 = " ";
            }
            for (int i = 0; i < len; i++)
            {
                str = str + data[i].ToString("X2") + str2;
            }
            return str;
        }

        public void ClearBufUID()
        {
            this.recvBuffer.Clear();
            this.bufRfidUID = string.Empty;
        }

        public bool Connect()
        {
            try
            {
                if (string.IsNullOrEmpty(this.readerIP))
                {
                    return false;
                }
                if (this.tcpClient != null)
                {
                    this.tcpClient.Close();
                }
                this.tcpClient = new TcpClient();
                this.tcpClient.ReceiveTimeout = 0xbb8;
      
                this.connectDone.Reset();
                this.tcpClient.BeginConnect(this.readerIP, this.readerPort, new AsyncCallback(this.ConnectCallback), this.tcpClient);
                this.connectDone.WaitOne();
                if ((this.tcpClient != null) && this.isConnect)
                {
                    this.workStream = this.tcpClient.GetStream();
                    StateObject state = new StateObject
                    {
                        client = this.tcpClient
                    };
                    if (this.workStream.CanRead)
                    {
                        this.workStream.BeginRead(state.buffer, 0, 0x400, new AsyncCallback(this.TCPReadCallBack), state);
                    }
                    Console.WriteLine(string.Format("RFID {0} 连接成功", this.readerIP));
                    this.isConnect = true;
                }
                else
                {
                    this.isConnect = false;
                }
                return this.isConnect;
            }
            catch (Exception exception)
            {
                Console.WriteLine(string.Format("RFID {0} 连接失败,{1}", this.readerIP, exception.Message));
                this.isConnect = false;
                return false;
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                TcpClient asyncState = (TcpClient)ar.AsyncState;
                if (asyncState.Connected)
                {
                    this.isConnect = true;
                    asyncState.EndConnect(ar);
                }
                else
                {
                    this.isConnect = false;
                    asyncState.EndConnect(ar);
                }
            }
            catch (SocketException)
            {
                this.isConnect = false;
            }
            finally
            {
                this.connectDone.Set();
            }
        }

        public bool Disconnect()
        {
            if (this.tcpClient != null)
            {
                this.tcpClient.Close();
            }
            this.isConnect = false;
            return true;
        }

        public byte[] ReadBytesData()
        {
            throw new NotImplementedException();
        }

        public int ReadData()
        {
            throw new NotImplementedException();
        }

        public long ReadDataInt64()
        {
            throw new NotImplementedException();
        }

        public string ReadStrData()
        {
            throw new NotImplementedException();
        }

        public string ReadUID()
        {
            try
            {
                
                if (!this.isConnect && !this.Connect())
                {
                    return string.Empty;
                }
                if(string.IsNullOrWhiteSpace(this.bufRfidUID))
                {
                    if (!IsOnline())
                    {
                        isConnect = false;
                        return string.Empty;
                        //this.Connect();
                    }
                }
                
                return this.bufRfidUID;
            }
            catch (Exception ex)
            {
                isConnect = false;
                Console.WriteLine("{0},{1}", this.readerIP, ex.ToString());
                return "";
            }
           
        }

        private bool RecvDataProcess(byte[] buf, int recvLen, ref string reStr)
        {
            bool flag2;
            try
            {
                lock (this.recvBufLock)
                {
                    if ((buf == null) || (buf.Count<byte>() < recvLen))
                    {
                        reStr = "接收数据处理错误，数据长度参数和实际不一致";
                        return false;
                    }
                    this.recvBuffer.AddRange(buf);
                    //Console.WriteLine("RFID{0}接收缓存区数据(16进制）:{1}",this.readerIP,bytes2hexString(this.recvBuffer.ToArray(), this.recvBuffer.Count<byte>(), 1));
                    int num = this.recvBuffer[0];
                    switch (num)
                    {
                        case 8:
                        case 11:
                            if (this.recvBuffer.Count<byte>() >= (num + 1))
                            {
                                byte[] source = new byte[num + 1];
                                for (int i = 0; i < source.Count<byte>(); i++)
                                {
                                    source[i] = this.recvBuffer[i];
                                }
                                byte[] destinationArray = new byte[num - 4];
                                Array.Copy(source, 3, destinationArray, 0, destinationArray.Count<byte>());
                                this.bufRfidUID = bytes2hexString(destinationArray, destinationArray.Count<byte>(), 0);
                                this.recvBuffer.RemoveRange(0, num + 1);
                            }
                            return true;
                    }
                    reStr = string.Format("RFID{0}数据错误,不符合通信协议,接收到的数据(16进制):{1}",this.readerIP,bytes2hexString(this.recvBuffer.ToArray(), this.recvBuffer.Count<byte>(), 1));
                    this.recvBuffer.Clear();
                    flag2 = false;
                }
            }
            catch (Exception exception)
            {
                reStr = exception.Message;
                flag2 = false;
            }
            return flag2;
        }

        private void TCPReadCallBack(IAsyncResult ar)
        {
            try
            {
                StateObject asyncState = (StateObject)ar.AsyncState;
                if ((asyncState.client == null) || !asyncState.client.Connected)
                {
                    this.isConnect = false;
                }
                else
                {
                    NetworkStream stream = asyncState.client.GetStream();
                    int length = stream.EndRead(ar);
                    asyncState.totalBytesRead += length;
                    if (length > 0)
                    {
                        byte[] destinationArray = new byte[length];
                        Array.Copy(asyncState.buffer, 0, destinationArray, 0, length);
                        string reStr = "";
                        if (!this.RecvDataProcess(destinationArray, destinationArray.Count<byte>(), ref reStr))
                        {
                            Console.WriteLine(reStr);
                        }
                        stream.BeginRead(asyncState.buffer, 0, 0x400, new AsyncCallback(this.TCPReadCallBack), asyncState);
                    }
                    else
                    {
                        stream.Close();
                        asyncState.client.Close();
                        stream = null;
                        asyncState = null;
                        this.isConnect = false;
                    }
                }
            }
            catch (Exception exception)
            {
                //Console.WriteLine("RFID {0},{1}", this.readerIP, exception.Message);
                this.isConnect = false;
                //this.Connect();
            }
        }

        public  bool IsOnline()
        {
            return PingIp(this.readerIP);
           
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
        public bool WriteBytesData(byte[] bytesData)
        {
            throw new NotImplementedException();
        }

        public bool WriteData(int palletID)
        {
            throw new NotImplementedException();
        }

        public bool WriteDataInt64(long palletID)
        {
            throw new NotImplementedException();
        }

        // Properties
        public bool IsOpened { get; set; }

        public byte ReaderID
        {
            get
            {
                return this.readerID;
            }
            set
            {
                this.readerID = value;
            }
        }
    }


}
