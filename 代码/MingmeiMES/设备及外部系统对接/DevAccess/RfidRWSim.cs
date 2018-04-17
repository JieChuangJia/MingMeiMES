using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevInterface;
namespace DevAccess
{
    public class rfidRWSim:IrfidRW
    {
        private byte readerID = 1;
        private bool isOpened = true;
        public Int64 simReadRfid = -1;
        public string Uid = "UID123";
        public bool IsOpened
        {
            get
            {
                return isOpened;
            }
        }
      
        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        public bool Connect() 
        {
            return true;
        }

        /// <summary>
        /// 断开
        /// </summary>
        /// <returns></returns>
        public bool Disconnect()
        {
            return true;
        }
        public bool WriteSBlock(byte[] bytesData)
        {
            return true;
        }
        public byte[] ReadSBlock(byte blockIndex, ref byte[] recvByteArray)
        {
            byte[] reBytes = new byte[4];
            return reBytes;//"0001234567";

        }
        public byte[] ReadRfidMultiBlock(byte blockStart, byte blockNum)
        {
            byte[] reBytes = new byte[4 * blockNum];
            return reBytes;
        }
        public bool WriteMBlock(byte blockIndex, byte[] bytesData)
        {
            return true;
        }
        public bool WriteData(int palletID)
        {
            this.simReadRfid = (Int64)palletID;
            return true;
        }
        public bool WriteDataInt64(Int64 palletID)
        {
            this.simReadRfid = palletID;
            return true;
        }


        public int ReadData()
        {
            return (int)simReadRfid;
        }
        public string ReadStrData()
        {
            return Uid;
        }
        public Int64 ReadDataInt64()
        {
            return (Int64)simReadRfid;
        }
        public byte[] ReadBytesData()
        {
            return BitConverter.GetBytes(simReadRfid);
        }
        public bool WriteBytesData(byte[] bytesData)
        {
            simReadRfid = BitConverter.ToInt64(bytesData, 0);
            return true;
        }
        public string ReadUID()
        {
            return Uid;
        }
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
    }
}
