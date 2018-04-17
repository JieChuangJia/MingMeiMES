using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using DevInterface;
namespace DevAccess
{
    public enum EnumTag
    {
        TagEPCC1G2,
        Tag180006B
    }
    public class WqwlRfidRW:IrfidRW
    {
        #region 数据
    //    private int readFailedCounter = 0;
        private EnumTag tagType = EnumTag.TagEPCC1G2;
        private string readerIP = "";
        private uint readerPort = 0;
        private byte readerID = 0x00; //读写器ID
        private int readerSocket = -1;

        public byte[] AccPaswd { get; set; }
        public string HostIP { get; set; }
        public uint HostPort { get; set; }
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
        public string ReaderIP { get { return readerIP; } set{readerIP=value;}}
        public uint ReaderPort { get { return readerPort; } set { readerPort = value; } }
        public string readerNmae { get; set; }
        public bool IsOpened { get; set; }
        #endregion
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
        public WqwlRfidRW(EnumTag tagType,byte readerID, string ip, uint port)
        {
            this.readerID = readerID;
            this.tagType = tagType;
            this.readerIP = ip;
            this.readerPort = port;
            AccPaswd = new byte[4] { 0, 0, 0, 0 };
            IsOpened = false;
        }
      
        public bool Connect()
        {
            int tryMax=20;
            int counter = 0;
            while(PortInUse((int)HostPort))
            {
                if(HostPort>10000)
                {
                    HostPort = 5000;
                }
                HostPort += 1;
                counter++;
                if (counter > tryMax)
                {
                    Console.WriteLine("{0}RFID连接失败，端口占用，自动分配方案失败", readerID);
                    return false;
                }
            }
            int res = Reader.Net_ConnectScanner(ref readerSocket,readerIP,readerPort,HostIP,HostPort);
            if(res==0)
            {
             //   readFailedCounter = 0;
                Reader.Net_SetAntenna(readerSocket, 1);
                Console.WriteLine("RFID:{0},连接成功：{1},{2},{3},{4}", this.readerID, readerIP, readerPort, HostIP, HostPort);
                IsOpened = true;
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool Disconnect()
        {
            int res = Reader.DisconnectScanner(readerSocket);
            if(res == Reader._OK)
            {
               // Console.WriteLine("{0}:断开", readerID);
                IsOpened = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        //读TID号，有效字6个
        public string ReadUID()
        {
            if(!IsOpened)
            {
                if(!Connect())
                {
                    Console.WriteLine("{0}号RFID读卡失败，重连失败", readerID);
                    return string.Empty;
                }
            }
            int nCounter = 0;
            int res = 0;
            string str = "";
          
           if(tagType == EnumTag.Tag180006B)
           {
               byte[,] TagNumber = new byte[100, 9];
               res = Reader.Net_ISO6B_ReadLabelID(readerSocket, TagNumber, ref nCounter);
              
               //for (j = 0; j < 8; j++)
              
               for (int j = 1; j <= 8; j++)
               {
                   str += TagNumber[0, j].ToString("X2");
               }
               return str;
           }
           else
           {
               //res = Reader.Net_SetAntenna(readerSocket, 1);
               //if (res != Reader._OK)
               //{
               //    Console.WriteLine("{0}号RFID设置天线失败", readerID);
               //    return str;
               //}
               int tryMax = 3;
               byte[] epcBytes = ReadEPC(); 
              

               if (epcBytes==null || epcBytes.Count()<1)
                {
                   
                    return str;
                }
               else 
               { Console.WriteLine("读到EPC:{0},来自RFID:{1}", BytesToString(epcBytes), readerIP); }
              // Console.WriteLine("EPC:" + BytesToString(epcBytes));
               byte EPC_Word = epcBytes[0];
               byte epcLen = (byte)(epcBytes[0]*2);
               byte[] epcData = new byte[epcLen];
               Array.Copy(epcBytes, 1, epcData, 0, epcLen);
               byte mem= 2; //TID
               byte ptr= 0;
               byte len = 6;
               byte[] DB = new byte[len*2];
              
               for (int i = 0; i < tryMax; i++)
               {
                   res = Reader.Net_EPC1G2_ReadWordBlock(readerSocket, EPC_Word, epcData, mem, ptr, len, DB, AccPaswd);
                   if (res == Reader._OK)
                   {
                       break;
                   }
                   else
                   {
                      // Console.WriteLine("读TID数据失败,返回错误码：" + res.ToString());
                       System.Threading.Thread.Sleep(100);
                   }

               }

              // res = Reader.Net_EPC1G2_ReadWordBlock(readerSocket, EPC_Word, epcData, mem, ptr, len, DB, AccPaswd);
               if(res != Reader._OK)
               {
                   Console.WriteLine(readerIP+":读TID数据失败,返回错误码："+res.ToString());
                  
                   return string.Empty;
               }
               str = "";
               for (int j = 0; j < len * 2; j++)
               {
                   str += DB[j].ToString("X2");
               }
               return str;
           }
           
        }
        public int ReadData()
        {
            int data = -1;
            byte[] bytesData = ReadBytesData();
            if(bytesData == null)
            {
                return data;
            }
            else
            {
                data = BitConverter.ToInt32(bytesData, 0);
            }
            return data;
            //byte mem = 1;//epc区
            //byte ptr = 2; //从第4字节开始 
            //byte len = 2;
            //byte[] DB = new byte[4];
            //byte[] EpcBytes = ReadLabelID();
            //byte[] IDTemp = null;
            //byte EPC_Word = 0;
            //if (EpcBytes != null && EpcBytes.Count() > 1)
            //{
            //    EPC_Word = EpcBytes[0];
            //    IDTemp = new byte[EpcBytes.Count() - 1];
            //    Array.Copy(EpcBytes, 1, IDTemp, 0, EpcBytes.Count() - 1);
            //}
            //else
            //{
            //    return data;
            //}

            //int res = Reader.Net_EPC1G2_ReadWordBlock(readerSocket, EPC_Word, IDTemp, mem, ptr, len, DB, AccPaswd);
            //if (res != Reader._OK)
            //{
            //    return data;
            //}
            //data = BitConverter.ToInt32(DB,0);
            
            //byte[] EpcWord = null;
            //byte[] IDBuffer = null;
            //byte mem= 1; //1:EPC
            //byte ptr = 0;
            //byte len = 2;
            //byte[] UserDataBuf=new byte[32];
            //int res = Reader.Net_EPC1G2_ReadEPCandData(readerSocket, EpcWord, IDBuffer, mem, ptr, len, UserDataBuf);
            //if(res == Reader._OK)
            //{
            //    data = BitConverter.ToInt32(UserDataBuf);
            //}
            
           
      
        }
        public bool WriteData(int val)
        {
            //byte mem = 1;//epc区
            //byte ptr = 0;
            //byte len = 2;
            //byte[] DB = new byte[8];
            //byte[] EpcBytes = ReadLabelID();
            //byte[] IDTemp = null;
            //byte EPC_Word = 0;
            //if (EpcBytes != null && EpcBytes.Count() > 8)
            //{
            //    EPC_Word = EpcBytes[0];
            //    IDTemp = new byte[EpcBytes.Count() - 1];
            //    Array.Copy(EpcBytes, 1, IDTemp, 0, EpcBytes.Count() - 1);
            //}
            //else
            //{
            //    return false;
            //}
            byte[] byteVals= BitConverter.GetBytes(val);
            return WriteBytesData(byteVals);
            //int res = Reader.Net_EPC1G2_WriteEPC(readerSocket, 2, byteVlas, AccPaswd);
            //if(res == Reader._OK)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }
        public Int64 ReadDataInt64()
        {
            Int64 data = -1;
            byte[] bytesData = ReadBytesData();
            //byte[] EpcWord = null;
            //byte[] IDBuffer = null;
            //byte mem = 1; //1:EPC
            //byte ptr = 2;
            //byte len = 4;
            //byte[] UserDataBuf = null;
            //int res = Reader.Net_EPC1G2_ReadEPCandData(readerSocket, EpcWord, IDBuffer, mem, ptr, len, UserDataBuf);
            //if (res == Reader._OK)
            //{
            //    data = Convert.ToInt64(UserDataBuf);
            //}
            if (bytesData == null)
            {
                return data;
            }
            else
            {
                data = BitConverter.ToInt64(bytesData, 0);
            }
            return data;
        }
        public string ReadStrData()
        {
            if (!IsOpened)
            {
                if (!Connect())
                {
                    Console.WriteLine("{0}号RFID读卡失败，重连失败", readerID);
                    return string.Empty;
                }
            }
            byte[] bytesData= ReadBytesData();
            if(bytesData == null)
            {
                return null;
            }
            
            string str= System.Text.Encoding.UTF8.GetString(bytesData);
         //   Console.WriteLine("读到USER区数据:{0},来自RFID:{1}", str, readerIP);
            return str;
        }

        //最多8个字，可写部分6个字
        public byte[] ReadBytesData()
        {
            byte[] EpcBytes = ReadEPC();
            if (EpcBytes == null || EpcBytes.Count() < 1)
            {
                // Console.WriteLine("读EPC，返回字节数据为空");
                return null;
            }
            else
            {
               // Console.WriteLine("读到EPC:{0},来自RFID:{1}", BytesToString(EpcBytes), readerIP);
                int epcLen = EpcBytes[0] * 2;
                byte[] reBytes = new byte[12];
              
                Array.Copy(EpcBytes,1,reBytes,0,Math.Min(12, epcLen));
                return reBytes;
            }

        }

        //public byte[] ReadBytesData()
        //{
        //    byte mem = 1;//epc区
        //    byte ptr = 2; //从第4字节开始 
        //    byte len = 6; //6个字
        //    byte[] DB = new byte[12];

            
          
        //    int tryMax = 3;
        //    byte[] EpcBytes = ReadEPC();
        //    if (EpcBytes == null || EpcBytes.Count() < 1)
        //    {
        //       // Console.WriteLine("读EPC，返回字节数据为空");
        //        return null;
        //    }
        //    else
        //    {
        //        Console.WriteLine("读到EPC:{0},来自RFID:{1}", BytesToString(EpcBytes), readerIP);
        //    }
        //    byte[] IDTemp = null;
        //    byte EPC_Word = 0;

        //   // if (EpcBytes != null && EpcBytes.Count() > 1)
            
        //    EPC_Word = EpcBytes[0];
        //    IDTemp = new byte[EpcBytes.Count() - 1];
        //    Array.Copy(EpcBytes, 1, IDTemp, 0, EpcBytes.Count() - 1);


        //    int res = 0;
        //    for (int i = 0; i < tryMax;i++ )
        //    {
        //        res = Reader.Net_EPC1G2_ReadWordBlock(readerSocket, EPC_Word, IDTemp, mem, ptr, len, DB, AccPaswd);
        //        if(res == Reader._OK)
        //        {
        //            break;
        //        }
        //        System.Threading.Thread.Sleep(150);
        //    }
        //    if (res != Reader._OK)
        //    {
        //        return null;
        //    }
        //    return DB;
        //}
        public bool WriteDataInt64(Int64 val)
        {
            byte[] byteVlas = BitConverter.GetBytes(val);
            byte len = 4; 
            int res = Reader.Net_EPC1G2_WriteEPC(readerSocket, len, byteVlas, AccPaswd);
            if (res == Reader._OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool WriteBytesData(byte[] bytesData)
        {
            byte[] bytesDataToWriten = null;
            if(bytesData == null || bytesData.Count()<0)
            {
                return false;
            }
            bytesDataToWriten = bytesData;
            byte wordLen = (byte)(bytesData.Count() / 2);
            if(wordLen*2<bytesData.Count())
            {
                wordLen++;
                bytesDataToWriten = new byte[wordLen * 2];
                bytesDataToWriten[bytesData.Count()] = 0;
                Array.Copy(bytesData, 0, bytesDataToWriten, 0, bytesData.Count());
            }
            int res = Reader.Net_EPC1G2_WriteEPC(readerSocket, wordLen, bytesDataToWriten, AccPaswd);
            if (res == Reader._OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private byte[] ReadLabelID()
        {
            int mem = 1; //只有EPC有效
            int ptr = 0; //起始位
            int len = 0;
            int nCounter = 0;
            byte[] TagNumber = new byte[13];
            byte[] mask = new byte[13];
            //EPC，总共12字节，第0字节是字的长度（一个字占2字节)
          
            int res = Reader.Net_EPC1G2_ReadLabelID(readerSocket, mem, ptr, len, mask, TagNumber, ref nCounter);
            if(res == Reader._OK)
            {
                return TagNumber;
            }
            else
            {
                //Console.WriteLine("读卡器:{0}识别标签错误，返回结果：{1}", this.readerID, res);
                //readFailedCounter++;
                //if (readFailedCounter > 2)
                //{
                //    Disconnect();
                //    Connect();
                //    readFailedCounter = 0;
                //}
               
                return null;
            }
        }
        public string BytesToString(byte[] byteStream)
        {
            string str = "";
            for(int i=0;i<byteStream.Count();i++)
            {
                str += byteStream[i].ToString("X2");
            }
            return str;
        }
        public byte[] ReadEPC()
        {
            int tryMax = 3;
            byte[] epcBytes = null;
            for (int i = 0; i < tryMax; i++)
            {
                epcBytes = ReadLabelID();
                if (epcBytes != null && epcBytes.Count() > 0)
                {
                    break;
                }
                System.Threading.Thread.Sleep(150);
            }
            if(epcBytes == null || epcBytes.Count()<1)
            {
                Console.WriteLine("RFID{0}:读EPC，返回字节数据为空", readerIP);
            }
            
            return epcBytes;
           
        }
    }
}
