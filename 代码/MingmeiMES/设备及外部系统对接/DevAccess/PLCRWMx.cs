using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Text;
using ACTETHERLib;
using DevInterface;
namespace DevAccess
{
   
    /// <summary>
    /// PLC读写功能类
    /// </summary>
    public class PLCRWMx:IPlcRW
    {
        private Int64 plcStatCounter = 0;
        private delegate bool DelegateConnPlc(ref string reStr);
        private delegate bool DelegateReadDB(string addr, ref int val);
        private delegate bool DelegateReadMultiDB(string addr, int blockNum, ref short[] vals);
        private delegate bool DelegateWriteDB(string addr, int val);
        private delegate bool DelegateWriteMultiDB(string addr, int blockNum, short[] vals);
        private int db1Len = 1000;
        private int db2Len = 1000;
        private Int16[] db1Vals = null;
        private Int16[] db2Vals = null;
        public Int16[] Db1Vals { get { return db1Vals; } set { db1Vals = value; } }
        public Int16[] Db2Vals { get { return db2Vals; } set { db2Vals = value; } }
        public Int64 PlcStatCounter { get { return plcStatCounter; } }
        public string PlcRole { get; set; }
        #region 数据区
        
       // private ActQNUDECPUTCP _actObj;
      //  private ActAJ71E71TCP _actObj;
       // private ActAJ71E71UDP _actObj;
      //  private ActQJ71E71UDP _actObj;
        private ActQJ71E71TCP _actObj;
       // private ActLCPUTCP _actObj;
        private bool isConnected = false;
        private string connStr = "";
        public string ConnStr
        { get { return connStr; } set { this.connStr = value; } }
        public System.Windows.Forms.Control hostControl { get; set; }
     //   public Int64 PlcStatCounter { get { return plcStatCounter; } }
        private object rwLock = new object();
        #endregion 

        /// <summary>
        /// 构造函数
        /// </summary>
        public PLCRWMx()
        {
            //L系
            //_actObj = new ActLCPUTCP();// new ActQNUDECPUTCP();
            //_actObj.ActCpuType = 0xA1; //Q03UDECPU：0x90,CPU_L02CPU：0xA1,Q02:0x141,Q01:0x32

            //Q
            //_actObj = new ActAJ71E71TCP();//new ActQNUDECPUTCP();
            _actObj = new ActQJ71E71TCP(); //new ActQJ71E71UDP(); //
          
            //_actObj.ActCpuType = 0x141; //Q02,实际是Q01
            _actObj.ActCpuType = 0x32; //Q01cpu
            _actObj.ActHostAddress = "192.168.1.100";
           //_actObj.ActTimeOut

            //_actObj.ActNetworkNumber = 0;
           // _actObj.ActStationNumber = 0;
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
        #region  接口实现
        public int PlcID { get; set; }
        /// <summary>
        /// 连接断开事件
        /// </summary>
        public event EventHandler<PlcReLinkArgs> eventLinkLost;
        //public event EventHandler<LogEventArgs> eventLogDisp;
        public int StationNumber { get; set; }
        public bool IsConnect 
        {
            get
            {
                return isConnected;
            }
           
        }
        public void Init()
        {

        }
        public void Exit()
        {

        }
        public bool dlgtConnPLC(ref string reStr)
        {
            string[] splitStr = new string[] { ",", ";", ":", "-", "|" };
            string[] strArray = this.connStr.Split(splitStr, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Count() < 2)
            {
                return false;
            }
            _actObj.ActHostAddress = strArray[0];
          
            //_actObj.ActDestinationPortNumber = int.Parse(strArray[1]);
            // _actObj.ActDestinationPortNumber = int.Parse(strArray[1]);
            int re = _actObj.Open();
            if (re == 0)
            {
                isConnected = true;
                reStr = "连接PLC成功！";

                return true;
            }
            else
            {
                reStr = "连接PLC失败!错误码：0x"+re.ToString("X");
                isConnected = false;
                return false;
            }
        }
        public bool ConnectPLC(ref string reStr)
        {
            //if (string.IsNullOrEmpty(this.connStr))
            //{
            //    reStr = "PLC通信地址为空!";
            //    return false;
            //}
            ////string[] splitStr = new string[] { ",", ";", ":", "-", "|" };
            ////string[] strArray = this.connStr.Split(splitStr, StringSplitOptions.RemoveEmptyEntries);
            ////if (strArray.Count() < 2)
            ////{
            ////    isConnected = false;
            ////}
            //DelegateConnPlc dlgt = new DelegateConnPlc(dlgtConnPLC);
            //return dlgt.Invoke(ref reStr);
            return dlgtConnPLC(ref reStr);
            
        }
        //关闭连接
        public bool CloseConnect()
        {
            if (_actObj.Close() == 0)
            {
                isConnected = false;
                return true;
            }
            else
            {
                
                return false;
            }
        }

        private bool dlgtReadDB(string addr, ref int val)
        {
            //object io;
            //short[] reVals = new short[15]; ;
            //_actObj.ReadDeviceBlock2(addr, 15, out reVals[0]);//批量读取
           
            int int16Val = 0;
           // int re = _actObj.GetDevice2(addr, out int16Val);
            int re =_actObj.GetDevice(addr, out int16Val);
            if (re == 0x01010020)
            {
                //Link error，Link communications could not be made.
                isConnected = false;
                if (eventLinkLost != null)
                {
                    eventLinkLost.Invoke(this, null);
                }
            }
            if (re != 0)
                return false; //读取失败

            //val = Convert.ToInt32(io);
            val = int16Val;

            return true;
        }
        public bool ReadDB(string addr, ref int val)
        {
            //DelegateReadDB dlgt = new DelegateReadDB(dlgtReadDB);
            //object[] paramArray = new object[] { addr, val };
            //bool re =(bool)this.hostControl.Invoke(dlgt,paramArray);
          
            //val = (int)paramArray[1];
            //return re;
            return dlgtReadDB(addr, ref val);
        }
        private bool dlgtWriteDB(string addr, int val)
        {
            int re = _actObj.SetDevice(addr, val);
         
            if (re == 0x01010020)
            {
                isConnected = false;
                if (eventLinkLost != null)
                {
                    eventLinkLost.Invoke(this, null);
                }
            }
            if (re != 0)
                return false; //写入失败
            return true;
        }
        public bool WriteDB(string addr, int val)
        {
          //  DelegateWriteDB dlgt = new DelegateWriteDB(dlgtWriteDB);
           // object[] paramArray = new object[] {addr,val};
            //bool re =(bool) this.hostControl.Invoke(dlgt, paramArray);
            //return re;
            return dlgtWriteDB(addr, val);
        }

        private bool dlgtReadMultiDB(string addr, int blockNum, ref short[] vals)
        {
            if (blockNum <= 0)
            {
                return false;
            }
            vals = new short[blockNum];
          
            int re = _actObj.ReadDeviceBlock2(addr, blockNum, out vals[0]);//批量读取
            if (re != 0)
                return false; //读取失败
            return true;
        }
        public bool ReadMultiDB(string addr, int blockNum, ref short[] vals)
        {
            //DelegateReadMultiDB dlgt = new DelegateReadMultiDB(dlgtReadMultiDB);
            //object[] paramArray = new object[] { addr, blockNum, vals };
            //bool re = (bool)this.hostControl.Invoke(dlgt, paramArray);

            //vals = (short[])paramArray[2];
            //return re;
            return dlgtReadMultiDB(addr, blockNum, ref vals);
           
        }
        private bool dlgtWriteMultiDB(string addr, int blockNum, short[] vals)
        {
            if (blockNum <= 0)
            {
                return false;
            }
            if (vals == null || vals.Count() < blockNum)
            {
                return false;
            }
            int re = _actObj.WriteDeviceBlock2(addr, blockNum, ref vals[0]);
           
            if (re != 0)
                return false; //读取失败
            return true;
        }
        public bool WriteMultiDB(string addr, int blockNum, short[] vals)
        {
            //DelegateWriteMultiDB dlgt = new DelegateWriteMultiDB(dlgtWriteMultiDB);
            //object[] paramArray = new object[] {addr,blockNum,vals};
            //bool re = (bool)this.hostControl.Invoke(dlgt, paramArray);
            return dlgtWriteMultiDB(addr, blockNum, vals);
           // return re;
        }
        public void PlcRWStatUpdate()
        {
            this.plcStatCounter++;
            if (this.plcStatCounter > long.MaxValue-10)
            {
                this.plcStatCounter = 1;
            }
        }
        public void GetDB2Data(int addrSt,int blockNum,ref short[] buf)
        {
            lock(rwLock)
            {
                for (int i = 0; i < blockNum; i++)
                {
                    buf[i] = db2Vals[addrSt + i];
                   
                }
            }
        }
        public void SetDB1Data(int addrSt,int blockNum,short[] buf)
        {
            lock(rwLock)
            {
                 for (int i = 0; i < blockNum; i++)
                 {
                     db1Vals[addrSt + i] = buf[i];

                 }
            }
        }
        public void DB2Switch(short[] tempBuf)
        {
            lock(rwLock)
            {
                Array.Copy(tempBuf, db2Vals, tempBuf.Count());
            }
        }
        public void DB1Switch(ref short[] tempBuf)
        {
            lock(rwLock)
            {
                Array.Copy(db1Vals, tempBuf, tempBuf.Count());
            }
        }
        #endregion
    }
}
