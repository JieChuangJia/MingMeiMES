using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using DevInterface;
namespace DevAccess
{
    public class SgrfidRW:IrfidRW
    {
        private const int UID_LEN = 8;
        private byte readerID = 0x00; //读写器ID
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
        public string readerNmae { get; set; }
        //Sygole.HFReader.UserCfg rfidCfg;
        private SygoleHFReaderIF.UserConfig rfidCfg;
        private bool isOpened = false;
        public bool IsOpened
        {
            get
            {
                return isOpened;
            }
        }
       // public HFReader ReaderIF = new HFReader();

        /// <summary>
        /// 读写器设备接口对象，一个接口对象可以带多个485节点
        /// </summary>
        public SygoleHFReaderIF.HFReaderIF ReaderIF { get; set; }

       
        public SgrfidRW(byte readerID)
        {
            this.readerID = readerID;
            this.rfidCfg = new SygoleHFReaderIF.UserConfig();
            this.rfidCfg.AddrMode = 0;
            this.rfidCfg.ReaderID = readerID;
            this.rfidCfg.RFChipPower = 1;
            this.rfidCfg.NeedBcc = true;
            this.rfidCfg.ComPortType = 1;
            this.rfidCfg.BlockBytes = 4;
            this.rfidCfg.AvailableType = 0;
        }
       
        //public bool SetComport(string comPort)
        //{
        //    this.ComPort = comPort;
        //    //System.IO.Ports.SerialPort serialPort = new SerialPort();
        //    ////serialPort.DataReceived += RecvEventHandler;
        //    //serialPort.PortName = comPort;
        //    //serialPort.DataBits = 8;
        //    //serialPort.StopBits = StopBits.One;
        //    //serialPort.Parity = Parity.None;
        //    //serialPort.ReceivedBytesThreshold = 1;
        //    //serialPort.BaudRate = 9600;
        //    return true;
           
        //}

        public void SetReadTimeout(int timeOut)
        {
            if(timeOut<500)
            {
                timeOut = 500;
            }
            if(timeOut>2000)
            {
                timeOut = 2000;
            }
            if(ReaderIF != null)
            {
                ReaderIF.recvTimeOut = timeOut;
            }
        }
        public SygoleHFReaderIF.EnumREStatus GetUserCfg(byte rfidID, ref SygoleHFReaderIF.UserConfig cfg)
        {
            SygoleHFReaderIF.EnumREStatus re = SygoleHFReaderIF.EnumREStatus.RW_FAILURE;
            if (ReaderIF != null)
            {
                byte[] recvBytes = null;
                re = ReaderIF.ReadUserCfg(rfidID, ref cfg,ref recvBytes);
            }
            ReaderIF.ClearRecvBuffer();
            this.rfidCfg = cfg;
            return re;
        }
        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            if(ReaderIF.commType==SygoleHFReaderIF.EnumCommType.URAT)
            {
                byte[] recvBytes = null;
                if (ReaderIF == null || !ReaderIF.IsConnect)
                {
                    return false;
                }
                SygoleHFReaderIF.UserConfig cfg = null;

                SygoleHFReaderIF.EnumREStatus re = ReaderIF.ReadUserCfg(this.readerID, ref cfg, ref recvBytes);
                int reTryMax = 2;
                int tryCount = 1;
                while (re != SygoleHFReaderIF.EnumREStatus.RW_SUCCESS)
                {
                    if (tryCount > reTryMax)
                    {
                        break;
                    }
                    System.Threading.Thread.Sleep(100);
                    re = ReaderIF.ReadUserCfg(this.readerID, ref cfg, ref recvBytes);
                    tryCount++;
                }
                ReaderIF.ClearRecvBuffer();
                if (re == SygoleHFReaderIF.EnumREStatus.RW_SUCCESS && cfg != null)
                {
                    this.rfidCfg = cfg;
                    isOpened = true;
                    return true;
                }
                else
                {
                    isOpened = false;
                    return false;
                }
               
           }
            else
            {
                string reStr = "";
                if (!ReaderIF.ConnNet(ref reStr))
                {
                    Console.WriteLine(reStr);
                    return false;
                }
                else
                {
                    return true;
                }
            }
  
           
        }

        /// <summary>
        /// 断开
        /// </summary>
        /// <returns></returns>
        public bool Disconnect()
        {
            if (ReaderIF != null && ReaderIF.IsConnect)
            {
                ReaderIF.CloseReader();
            }
            isOpened = false;
            return true;
            
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="bytesData"></param>
        /// <returns></returns>
        public bool WriteSBlock(byte[] bytesData)
        {
            if ((bytesData.Count() != 4) && (bytesData.Count() != 8))
            {
                return false;
            }
          
            byte BlockStart = 0;
            byte dataLen = 4;//  rfidCfg.BlockBytes;
            byte[] blockData = new byte[dataLen];
            Array.Copy(bytesData,blockData, Math.Min(dataLen, bytesData.Count()));
            if (SygoleHFReaderIF.EnumREStatus.RW_SUCCESS == ReaderIF.WriteSBlock(this.readerID, this.rfidCfg, BlockStart, blockData, dataLen))
            {
                return true;
            }
            else
            {

                return false;
            }
           
        }

        /// <summary>
        /// 读单块数据
        /// </summary>
        /// <returns></returns>
        public byte[] ReadSBlock(byte blockIndex)
        {
            if (this.rfidCfg == null)
            {
                return null;
            }
            byte blockBytes = 4;
            if (rfidCfg.BlockBytes == 8)
            {
                blockBytes = 8;
            }
           
           
            byte[] ReceiveDatas = new byte[blockBytes];
            
            byte DataLen = 0;
           // string reStr = "";
            byte[] recvByteArray = null;
            SygoleHFReaderIF.EnumREStatus re = ReaderIF.ReadSBlock(readerID, this.rfidCfg, blockIndex, ref ReceiveDatas, ref DataLen, ref recvByteArray);
            if (SygoleHFReaderIF.EnumREStatus.RW_SUCCESS == re)
            {
                //reStr = bytes2hexString(ReceiveDatas, DataLen, 1);
                return ReceiveDatas;
            }
            else
            {
               // Console.WriteLine("读卡失败，返回"+re.ToString());
                return null;
            }
            
        }
        public byte[] ReadRfidMultiBlock(byte blockStart, byte blockNum)
        {
           
            if (this.rfidCfg == null)
            {
                return null;
            }
            byte[] UID = new byte[0];
            byte blockBytes = 4;
           
            byte[] ReceiveDatas = new byte[blockNum * blockBytes];

            byte DataLen = 0;
            if (SygoleHFReaderIF.EnumREStatus.RW_SUCCESS == ReaderIF.ReadMBlock(readerID, this.rfidCfg, blockStart, blockNum, ref ReceiveDatas, ref DataLen))
            {
                return ReceiveDatas;
            }
            else
            {
                string str = bytes2hexString(ReceiveDatas, ReceiveDatas.Count(), 1);
                Console.WriteLine("{0},接收到数据：", ReaderIF.readerIP, str);
                return null;
            }
        }
        public bool WriteMBlock(byte blockIndex, byte[] bytesData)
        {
            if (this.rfidCfg == null)
            {
                return false;
            }
            if (SygoleHFReaderIF.EnumREStatus.RW_SUCCESS == ReaderIF.WriteMBlock(readerID, this.rfidCfg, blockIndex, bytesData, (byte)bytesData.Count()))
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
        public int ReadData()
        {
            byte[] palletBytes = ReadSBlock(0);
            bool readOk = false;
            if ((palletBytes == null) || (palletBytes.Count() < 4))
            {
                readOk = false;
            }
            else
            {
                readOk = true;
            }
            
            if (readOk)
            {
                int rfidID = BitConverter.ToInt32(palletBytes, 0);
                return rfidID;
            }
            else
            {
                return -1;
            }
        }
        public byte[] ReadBytesData()
        {
            byte[] palletBytes = ReadRfidMultiBlock(0, 4);// ReadSBlock(0);

            
            return palletBytes;
        }


        public string ReadStrData()
        {
            byte[] bytesData = ReadBytesData();
            if (bytesData == null)
            {
                return null;
            }
            return System.Text.Encoding.UTF8.GetString(bytesData);
        }
        /// <summary>
        /// 读64位ID
        /// </summary>
        /// <param name="recvByteArray"></param>
        /// <returns></returns>
        public Int64 ReadDataInt64()
        {
            byte[] palletBytes = ReadRfidMultiBlock(0, 2);
            bool readOk = false;
            if ((palletBytes == null) || (palletBytes.Count() < 8))
            {
                readOk = false;
            }
            else
            {
                readOk = true;
            }
            if (readOk)
            {
                Int64 rfidID = BitConverter.ToInt64(palletBytes, 0);
                return rfidID;
            }
            else
            {
                return -1;
            }
        }

        public bool WriteData(int palletID)
        {
            if (palletID <= 0)
            {
                return false;
            }
            byte[] byteArray = BitConverter.GetBytes(palletID);
            return WriteSBlock(byteArray);
            
        }
        public bool WriteDataInt64(Int64 palletID)
        {
            if (palletID <= 0)
            {
                return false;
            }
            byte[] byteArray = BitConverter.GetBytes(palletID);
            if (byteArray.Count() < 8)
            {
                return false;
            }
            return WriteMBlock(0, byteArray);
           
        }
        public bool WriteBytesData(byte[] bytesData)
        {
            return WriteMBlock(0, bytesData);
        }
         public byte[] GetRecvBuf()
        {
            if(this.ReaderIF != null)
            {
                return ReaderIF.GetRecvBuf();
            }
            return null;
        }
         public string GetRecvBufStr()
         {
             if(this.ReaderIF != null)
             {
                 return ReaderIF.GetRecvBufStr();

             }
             return string.Empty;
         }
        public string ReadUID()
        {
           // throw new NotImplementedException();
            byte[] UIDBytes = null;
            SygoleHFReaderIF.EnumREStatus re = ReaderIF.GetUID(this.readerID, this.rfidCfg, out UIDBytes);
            if(re == SygoleHFReaderIF.EnumREStatus.RW_SUCCESS)
            {
               // string uidStr = System.Text.Encoding.Default.GetString(UIDBytes);
                string uidStr = "";
                for (int i = 0; i < UIDBytes.Count();i++ )
                {
                    uidStr += UIDBytes[i].ToString("X2");
                }
                ReaderIF.ClearRecvBuffer();
                return uidStr;
            }
            else
            {
                ReaderIF.ClearRecvBuffer();
                return string.Empty;
            }
        }
        #region 静态功能接口
        //返回一个字节的16进制字符串
        public static string byte2hexString(byte data)
        {
            string outString = "";
            if (data < 16)
                outString += "0";
            outString += data.ToString("X");
            return outString;
        }

        //返回若干个字节的16进制字符串,
        //space为两个字节间的间隔符，0：无，1：空格
        public static string bytes2hexString(byte[] data, int len, int space)
        {
            string outString = "";
            string SpaceStr = "";
            switch (space)
            {
                case 1:
                    SpaceStr = " ";
                    break;
                default:
                    break;
            }

            for (int i = 0; i < len; i++)
            {
                outString += byte2hexString(data[i]) + SpaceStr;
            }
            return outString;
        }
        public static string DelSpace(string str)
        {
            string result = "";

            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] != ' ')
                {
                    result += str[i];
                }
            }

            return result;
        }
        public static byte CharToByte(char ch)
        {
            byte value = 0;

            if ((ch >= '0') && (ch <= '9'))
            {
                value = (byte)(ch - '0');
            }
            else if ((ch >= 'a') && (ch <= 'f'))
            {
                value = (byte)((ch - 'a') + 10);
            }
            else if ((ch >= 'A') && (ch <= 'F'))
            {
                value = (byte)((ch - 'A') + 10);
            }
            else
            {
                throw new Exception();
            }

            return value;
        }//a->10
        public static byte String2Byte(string str, int pos)
        {
            byte temp = 0;
            int len;
            string SubStr = str.Substring(pos);
            SubStr = DelSpace(SubStr);

            if (SubStr.Length == 0)
            {
                return 0;
            }
            else if (SubStr.Length < 2)
            {
                len = SubStr.Length;
            }
            else
            {
                len = 2;
            }

            for (int i = 0; i < len; i++)
            {
                temp = (byte)(temp * 16 + CharToByte(SubStr[i]));
            }

            return temp;
        }
        public static byte[] String2Bytes(string str, int pos)
        {
            string SubStr = str.Substring(pos);

            SubStr = DelSpace(SubStr);
            byte[] temp = new byte[(SubStr.Length + 1) / 2];

            for (int i = 0; i < (SubStr.Length + 1) / 2; i++)
            {
                temp[i] = String2Byte(SubStr, 2 * i);
            }

            return temp;
        }
        #endregion

       
    }
}
