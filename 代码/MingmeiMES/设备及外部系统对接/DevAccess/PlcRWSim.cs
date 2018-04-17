using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevInterface;

namespace DevAccess
{
    /// <summary>
    /// plc读写模拟类
    /// </summary>
    public class PlcRWSim:IPlcRW
    {
        private object lockObj = new object();
        long plcStatCounter = 0;
        public Int16[] db1Vals = new Int16[1000];
        public Int16[] db2Vals = new Int16[1000];
        /// <summary>
        /// 连接断开事件
        /// </summary>
       // public event EventHandler<PlcReLinkArgs> eventLinkLost;
        public string PlcRole { get; set; }
        public Int64 PlcStatCounter { get { return 0; } }
        public int PlcID { get; set; }
        public int StationNumber { get; set; }
        public bool IsConnect
        {
            get
            {
                return true;
            }

        }
        public Int16[] Db1Vals { get { return db1Vals; } set { db1Vals = value; } }
        public Int16[] Db2Vals { get { return db2Vals; } set { db2Vals = value; } }
        public bool ConnectPLC( ref string reStr)
        {
            reStr = "连接成功！";
            return true;
        }
        public bool CloseConnect()
        {
            return true;
        }
        public bool ReadDB(string addr, ref int val)
        {
            //PlcDBSimModel model = dbSimBll.GetModel(addr);
            //if (model == null)
            //{
            //    return false;
            //}
            //val = model.Val;

            val = 0;
            return true;
        }
        public bool ReadMultiDB(string addr, int blockNum, ref short[] vals)
        {
            //lock (lockObj)
            {
                System.Threading.Thread.Sleep(100);
              //  Console.WriteLine("1");
              //  System.Threading.Thread.Sleep(100);
              //  Console.WriteLine("2");
             //   System.Threading.Thread.Sleep(100);
             //   Console.WriteLine("3");
            //    System.Threading.Thread.Sleep(100);
              //  Console.WriteLine("4");
                vals = new short[blockNum];
                return true;
            }
           
        }
        public bool WriteDB(string addr, int val)
        {
            //PlcDBSimModel model = dbSimBll.GetModel(addr);
            //if (model == null)
            //{
            //    return false;
            //}
            //model.Val = val;
            //return dbSimBll.Update(model);
            
            return true;

        }
        public bool WriteMultiDB(string addr, int blockNum, short[] vals)
        {
            //lock (lockObj)
            {
                System.Threading.Thread.Sleep(100);
             
                return true;
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
    }
}
