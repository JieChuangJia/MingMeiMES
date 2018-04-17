using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevInterface;
namespace DevAccess
{
    /// <summary>
    /// 基于OPC 访问PLC的功能类
    /// </summary>
    public class OPCPlcRW:IPlcRW
    {
        #region 私有数据
        private bool isConnected = false;
        private Int64 plcStatCounter = 0;
        private int db1Len = 1000;
        private int db2Len = 1000;
        private Int16[] db1Vals = null;
        private Int16[] db2Vals = null;
        private OPCWrapperAuto opcWrapper = null;
        public OPCWrapperAuto OpcWrapper
        {
            get
            {
                return opcWrapper;
            }
            set
            {
                opcWrapper = value;
            }
        }
        public string serverName="";
        public string serverIP="";
        #endregion
        #region 公共接口
        public string PlcRole { get; set; }
        public bool IsConnect
        {
            get
            {
                return isConnected;
            }

        }
        public int PlcID { get; set; }
        public int StationNumber { get; set; }

        public Int16[] Db1Vals { get { return db1Vals; } set { db1Vals = value; } }
        public Int16[] Db2Vals { get { return db2Vals; } set { db2Vals = value; } }
        public Int64 PlcStatCounter { get { return plcStatCounter; } }
        public bool ConnectPLC(ref string reStr)
        {
            try
            {
                if(OpcWrapper.Connect(serverName, serverIP, out reStr))
                {
                    isConnected = true;
                    return true;
                }
                else
                {
                    isConnected = false;
                    return false;
                }
            }
            catch (Exception ex)
            {
                reStr = ex.Message;
                isConnected = false;
                return false;
            }
        }
        public bool CloseConnect()
        {
            try
            {
                string reStr = "";
                if(!OpcWrapper.CloseConn(ref reStr))
                {
                    Console.WriteLine(reStr);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                
                return false;
            }

        }
        /// <summary>
        /// 配置地址
        /// </summary>
        /// <param name="addr">DB名加上commobj.DataAddr</param>
        /// <returns></returns>
        public bool ConfigAddr(string addr,out string reStr)
        {
            reStr = "";
            string groupName = "";
            string itemID = "";
            if (opcWrapper == null)
            {
                reStr = "opc 客户端对象为空";
                return false;
            }
            // 解析groupName,itemID
            string[] splitStr = new string[] { "-"};
            string[] splitedAddrs = addr.Split(splitStr, StringSplitOptions.RemoveEmptyEntries);
            groupName = splitedAddrs[0];
            itemID = splitedAddrs[1];
            return opcWrapper.AddItem(groupName, itemID, out reStr);
        }
        /// <summary>
        /// 实现IPlcRW接口，读plc数据
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="val"></param>
        /// <param name="reStr"></param>
        /// <returns></returns>
        public bool ReadDB(string addr, ref int val)
        {
            string groupName = "";
            string itemID = "";
            if (opcWrapper == null)
            {
               
                return false;
            }
            // 解析groupName,itemID
            string[] splitStr = new string[] { "-"};
            string[] splitedAddrs = addr.Split(splitStr, StringSplitOptions.RemoveEmptyEntries);
            groupName = splitedAddrs[0];
            itemID = splitedAddrs[1];
            object Val = 0;
            if(!opcWrapper.Read(groupName, itemID, out Val))
            {
                return false;
            }
            val = (int)Val;
            return true;
        }

        public bool ReadMultiDB(string addr, int blockNum, ref short[] vals)
        {
            try
            {
                string groupName = "";
                string itemID = "";
                if (opcWrapper == null)
                {

                    return false;
                }
                // 解析groupName,itemID
                string[] splitStr = new string[] { "-" };
                string[] splitedAddrs = addr.Split(splitStr, StringSplitOptions.RemoveEmptyEntries);
                groupName = splitedAddrs[0];
                itemID = splitedAddrs[1]+","+blockNum.ToString();
                object Val=null;
                if(!opcWrapper.Read(groupName, itemID, out Val))
                {
                    return false;
                }
                int[] int32Vlas = (int[])Val;
                vals = new short[int32Vlas.Count()];
                for (int i = 0; i < vals.Count(); i++)
                {
                    vals[i] = (short)int32Vlas[i];
                }
                return true;

            }
            catch (Exception ex)
            {
              
                return false;
            }
        }
        /// <summary>
        /// 实现IPlcRW接口，写plc数据
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="val"></param>
        /// <param name="reStr"></param>
        /// <returns></returns>
        public bool WriteDB(string addr, int val)
        {
           
            string groupName = "";
            string itemID = "";
            if (opcWrapper == null)
            {
               
                return false;
            }
            string[] splitStr = new string[] {"-"};
            string[] splitedAddrs = addr.Split(splitStr, StringSplitOptions.RemoveEmptyEntries);
            groupName = splitedAddrs[0];
            itemID = splitedAddrs[1];
            return opcWrapper.Write(groupName, itemID, val);

        }

        public bool WriteMultiDB(string addr, int blockNum, short[] vals)
        {
            try
            {
                string groupName = "";
                string itemID = "";
                if (opcWrapper == null)
                {

                    return false;
                }
                string[] splitStr = new string[] { "-" };
                string[] splitedAddrs = addr.Split(splitStr, StringSplitOptions.RemoveEmptyEntries);
                groupName = splitedAddrs[0];
                itemID = splitedAddrs[1];
                int[] int32Vlas = new int[blockNum]; // (int[])Val;
              
                for (int i = 0; i < vals.Count(); i++)
                {
                    int32Vlas[i] = vals[i];
                }
                return opcWrapper.Write(groupName, itemID, int32Vlas);
            }
            catch (Exception ex)
            {
               
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
    }
}
