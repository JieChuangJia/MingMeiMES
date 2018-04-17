using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevInterface;
using ReaderB;

namespace DevAccess
{
    public class RfidRW_CFRH390 : IrfidRW
    {
        public string readerIP = "";
        public int readerPort = 3001;
        public byte comAddr = 0;
        public int readHandle = 0;
        public byte ReaderID { get; set; }
        private bool isConnect = false;
        private byte[] SNR = new byte[4];
        public RfidRW_CFRH390(byte id,string ip,int port)
        {
            this.ReaderID = id;
            this.readerIP = ip;
            this.readerPort = port;
        }

        public bool IsOpened { get { return this.isConnect; } }
        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            this.comAddr = Convert.ToByte("FF", 16); // $FF;
            int re = StaticClassReaderB.OpenNetPort(this.readerPort, this.readerIP, ref this.comAddr, ref this.readHandle);
            if (re == 0)
            {
                this.isConnect = true;

                if (ChangeProtocol() == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                this.isConnect = false;
                return false;
            }
          

        }

   
        /// <summary>
        /// 断开
        /// </summary>
        /// <returns></returns>
        public bool Disconnect()
        {
            int re = StaticClassReaderB.CloseNetPort(this.readHandle);
            if (re == 0)
            {
                this.isConnect = false;
                return true;
            }
            else
            {
                return false;
            }
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
            int fCmdRet = 0x30;
            byte reserved = 0;
            byte errorCode = 0;
            if (Request() == false)
            {
                return string.Empty;
            }

            fCmdRet = StaticClassReaderB.ISO14443AAnticoll(ref this.comAddr, reserved, SNR, ref errorCode, this.readHandle);
            if(fCmdRet == 0)
            {
                string uid = ByteArrayToHexString(SNR).Replace(" ", "");
                return uid;
            }
            else
            {
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

        private bool Request()
        {
              int fCmdRet = 0x30;
            byte[] Data = new byte[2];
            byte errorCode = 0;

            fCmdRet = StaticClassReaderB.ISO14443ARequest(ref this.comAddr, 1, Data, ref errorCode, this.readHandle);

            if (fCmdRet == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
