using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using System.Threading;
using FTDataAccess.BLL;
using FTDataAccess.Model;
using MesDBAccess.BLL;
using DevInterface;
using DevAccess;
using LogInterface;

namespace PLProcessModel
{
    
    /// <summary>
    /// 控制节点模型基类，定义了所有控制节点对象共有的接口
    /// </summary>
    public abstract class CtlNodeBaseModel : ILogRequired
    {
        protected delegate bool DelegateUploadMes(string productBarcode, string[] mesProcessSeq, ref string reStr);
        #region 私有数据
        protected int channelIndex = 0;
        protected DBAccess.BLL.BatteryModuleBll modBll = new DBAccess.BLL.BatteryModuleBll();
        //protected DCIRDataAccess.dbBll dcirBll = new DCIRDataAccess.dbBll();
       
        protected bool palletChecked = false;//检测到有板
        protected bool productChecked = false; //检测到有产品
        protected bool devStatusRestore = false;//是否已经恢复下电前状态
        protected string processName = "外观检查";
        protected ControlTaskModel currentTask = null;
        protected ControlTaskBll ctlTaskBll = new ControlTaskBll();
        protected DBAccess.BLL.ModPsRecordBll modPsRecordBll = new DBAccess.BLL.ModPsRecordBll();

        protected const int db1StatCheckOK = 1; //OK,放行
        protected const int db1StatRfidFailed = 2; //读条码/RFID失败
        protected const int db1StatNG = 3; //NG
       // protected const int db1StatCheckneed = 8;
        protected const int db1StatCheckNoneed = 4; //不需要检测
        protected const int db1StatCheckUnbinded = 5;// 未绑定
        protected const int db1StatCheckPreNG = 6;//前道工序有NG或漏检
      //  protected const int db1ResOK = 64;
        protected bool checkEnable = true;
        protected bool nodeEnabled = true; //是否启用
        protected bool checkFinished = false;//检测完成
       // protected int taskPhase = 0; //流程步号（状态机）
        protected CtlNodeStatus currentStat;
        protected ILogRecorder logRecorder = null;
        protected string nodeID = "";
        protected string nodeName = "";
        protected string mesNodeID = "";
        protected string mesNodeName = "";
        protected string[] mesProcessSeq = null;
        protected string[] preStat = null; //漏项检查前序工位
        protected CtlSequenceModel currentCtlSequence = null;
        protected IPlcRW plcRW = null;//设备的plc读写接口
        protected IPlcRW plcRW2 = null;//设备的plc读写接口
        protected IrfidRW rfidRW = null;//rfid读写接口
        protected List<IrfidRW> rfidRWList = null;
        protected IBarcodeRW barcodeRW= null; //条码枪读写接口

        protected byte rfidID = 0;
        protected List<byte> rfidIDS = null;
        protected int plcID = 0;
        protected int plcID2 = 0;
        protected int barcodeID = 0;
   
        protected string db1StartAddr = ""; //db1 开始地址
        protected string db2StartAddr = ""; //db2 开始地址
        protected int db1BlockNum = 0;
        protected IDictionary<int, PLCDataDef> dicCommuDataDB1 = null;//通信功能项字典，DB1
        protected IDictionary<int, PLCDataDef> dicCommuDataDB2 = null;//通信功能项字典，DB2
        protected int currentTaskPhase = 0;//流程步号（状态机）,从1开始
        protected string  rfidUID = ""; //读到的rfid UID
        protected string rfidUIDA = "";
        protected string rfidUIDB = "";
        protected string currentTaskDescribe = "";// 当前任务描述
        protected Int16[] db1ValsToSnd = null; //db1待发送数据
        protected Int16[] db1ValsReal = null; //PLC 实际DB1数据
        protected Int16[] db2Vals = null;
        protected string workerID = "";//员工号
      
        protected MingmeiDeviceAcc ccdDevAcc = null;
        public PLNodesModel plNodeModel { get; set; }
        /// <summary>
        /// DB1数据区的锁
        /// </summary>
        private object lockDB1 = new object();

        /// <summary>
        /// DB2数据区的锁
        /// </summary>
        private object lockDB2 = new object();

        protected OnlineProductsBll productBindBll = null;
        protected LOCAL_MES_STEP_INFOBll mesInfoBllLocal = null;
        protected LOCAL_MES_STEP_INFO_DETAILBll mesDetailBllLocal = null;
        protected ProduceRecordBll produceRecordBll = null;
        protected LOCAL_MES_STEP_INFOBll localMesBasebll = null;
        protected LOCAL_MES_STEP_INFO_DETAILBll localMesDetailbll = null;
        protected MesDA mesDA = null;
        protected OnlineProductsBll onlineProductBll = null;
        protected DetectCodeDefBll detectCodeDefbll = new DetectCodeDefBll();
        #endregion
        #region 属性
        public int LineID { get; set; }
        public bool NodeEnabled { get { return nodeEnabled; } set { nodeEnabled = value; } }
        public string NodeID { get { return nodeID; } }
        public string NodeName { get { return nodeName; } }
        public IPlcRW PlcRW
        {
            get { return this.plcRW; }
            set { this.plcRW = value; }
        }
        public IPlcRW PlcRW2
        {
            get { return this.plcRW2; }
            set { this.plcRW2 = value; }
        }
        public IrfidRW RfidRW
        {
            get { return rfidRW; }
            set { rfidRW = value; }
        }
        public List<IrfidRW> RfidRWList
        {
            get { return rfidRWList; }
            set { rfidRWList = value; }
        }
        public IBarcodeRW BarcodeRW
        {
            get { return barcodeRW; }
            set { barcodeRW = value; }
        }
        public MingmeiDeviceAcc CCDDevAcc { get { return ccdDevAcc; } set { ccdDevAcc = value; } }
        public int ccdDevID = 0;
        public string ccdDevName = "";

        public string Db1StartAddr { get { return db1StartAddr; } set { db1StartAddr = value; } }
        public string Db2StartAddr { get { return db2StartAddr; } set { db2StartAddr = value; } }
        public int Db1BlockNum { get { return db1BlockNum; } set { db1BlockNum = value; } }
        public Int16[] Db1ValsToSnd { get { return db1ValsToSnd; } set { db1ValsToSnd = value; } }
        public IDictionary<int, PLCDataDef> DicCommuDataDB1
        {
            get { return dicCommuDataDB1; }
            set { dicCommuDataDB1 = value; }
        }
        public IDictionary<int, PLCDataDef> DicCommuDataDB2
        {
            get { return dicCommuDataDB2; }
            set { dicCommuDataDB2 = value; }
        }
        public ILogRecorder LogRecorder { get { return logRecorder; } set { logRecorder = value; } }

        public CtlNodeStatus CurrentStat { get { return currentStat; } set { currentStat = value; } }
        public string SimRfidUID { get; set; }
        public string SimBarcode { get; set; }
    //    public bool SimMode { get; set; }
        public int PlcID { get { return plcID; } set { plcID = value; } }
        public int PlcID2 { get { return plcID2; } set { plcID2 = value; } }
        public byte RfidID { get { return rfidID; } set { rfidID = value; } }
        public List<byte> RfidIDS { get { return rfidIDS; } set { rfidIDS = value; } }
        public string RfidUID { get { return rfidUID; } set { rfidUID = value; } }
        public int BarcodeID { get { return barcodeID; } set { barcodeID = value; } }
        public string WorkerID { get { return workerID; } set { workerID = value; } }
        #endregion
        #region 公共数据
        public bool isWithMes = true;
        #endregion
        #region 公开接口
        public CtlNodeBaseModel()
        {
            productBindBll = new OnlineProductsBll();
            mesInfoBllLocal = new LOCAL_MES_STEP_INFOBll();
            mesDetailBllLocal = new LOCAL_MES_STEP_INFO_DETAILBll();
            produceRecordBll = new ProduceRecordBll();
            mesDA = new MesDA();
            localMesBasebll = new LOCAL_MES_STEP_INFOBll();
            localMesDetailbll = new LOCAL_MES_STEP_INFO_DETAILBll();
            onlineProductBll = new OnlineProductsBll();
           // mesDataOpt = new MesDataOperate();
        }
        public virtual bool ReadDB1()
        {
            int blockNum = this.dicCommuDataDB1.Count();
            if (!SysCfgModel.SimMode)
            {
                short[] vals = null;
                //同步通信
                if (!plcRW.ReadMultiDB(db1StartAddr, blockNum, ref vals))
                {
                    // refreshStatusOK = false;
                    ThrowErrorStat(this.nodeName + "读PLC数据(DB1）失败", EnumNodeStatus.设备故障);
                    return false;
                }
                for (int i = 0; i < blockNum; i++)
                {
                    int commID = i + 1;
                    this.dicCommuDataDB1[commID].Val = vals[i];
                    this.db1ValsReal[i] = vals[i];
                }
            }
            else
            {
                for (int i = 0; i < blockNum; i++)
                {
                    this.db1ValsReal[i] = this.db1ValsToSnd[i];
                }
            }
            return true;


        }
        //public void WriteDB1()
        //{
        //    int blockNum = this.dicCommuDataDB1.Count();
        //    lock(lockDB1)
        //    {
        //        if (!SimMode)
        //        {
                    
        //            int addrSt = int.Parse(this.db1StartAddr.Substring(1)) - 2000;
        //            //Array.Copy(this.db1ValsToSnd,0,plcRwMx.db1Vals, addrSt, blockNum);

        //            for (int i = 0; i < blockNum; i++)
        //            {
        //                int commID = i + 1;
        //                this.dicCommuDataDB1[commID].Val = this.db1ValsToSnd[i];
        //                plcRW.Db1Vals[addrSt + i] = this.db1ValsToSnd[i];
        //                // this.db1ValsReal[i] = vals[i];
        //            }
        //        }
        //        else
        //        {
        //            for (int i = 0; i < blockNum; i++)
        //            {
        //                this.db1ValsToSnd[i] = this.db1ValsReal[i];
        //            }
        //        }
        //    }
            
        //}
        /// <summary>
        /// 查询节点的状态数据（DB2）
        /// </summary>
        /// <param name="reStr"></param>
        /// <returns></returns>
        public virtual bool ReadDB2(ref string reStr)
        {
            int blockNum = this.dicCommuDataDB2.Count();
          //  lock(lockDB2)
            {

                if (!SysCfgModel.SimMode)
                {
                    if (SysCfgModel.PlcCommSynMode)
                    {
                        //this.db2StartAddr = "D3000";//test
                        short[] vals = null;
                        //同步通信
                        if (!plcRW.ReadMultiDB(db2StartAddr, blockNum, ref vals))
                        {
                            // refreshStatusOK = false;
                            ThrowErrorStat(this.nodeName + "读PLC数据(DB2）失败", EnumNodeStatus.设备故障);
                            plcRW.CloseConnect();
                            if(!plcRW.ConnectPLC(ref reStr))
                            {
                                Console.WriteLine("重连PLC{0}失败", (plcRW as OmlPlcRW).PlcRole);
                            }
                           // logRecorder.AddDebugLog(this.nodeName, "读PLC数据(DB2）失败");
                            return false;
                        }
                        else
                        {
                            if(this.currentStat.Status == EnumNodeStatus.设备故障)
                            {
                                this.currentStat.Status = EnumNodeStatus.设备空闲;
                            }
                        }
                        for (int i = 0; i < blockNum; i++)
                        {
                            int commID = i + 1;
                            this.dicCommuDataDB2[commID].Val = vals[i];
                            this.db2Vals[i] = vals[i];
                        }
                    }
                    else
                    {
                        int addrSt = int.Parse(this.db2StartAddr.Substring(1)) - int.Parse(SysCfgModel.DB2Start.Substring(1));

                        for (int i = 0; i < blockNum; i++)
                        {
                            this.db2Vals[i] = plcRW.Db2Vals[addrSt + i];
                            int commID = i + 1;
                            this.dicCommuDataDB2[commID].Val = this.db2Vals[i];
                            //this.db2Vals[i] = vals[i];
                        }
                    }
                   
                }
                else
                {
                    plcRW.ReadMultiDB(this.db2StartAddr, blockNum, ref this.db2Vals);
                    for (int i = 0; i < blockNum; i++)
                    {
                        int commID = i + 1;
                        this.db2Vals[i] = short.Parse(this.dicCommuDataDB2[commID].Val.ToString());
                    }
                }

                return true;
            }
           
        }
        public DataTable GetDB1DataDetail()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("索引");
            dt.Columns.Add("地址");
            dt.Columns.Add("内容");
            dt.Columns.Add("描述");
            int index = 1;

            for (int i = 0; i < dicCommuDataDB1.Count(); i++)
            {
                PLCDataDef commObj = dicCommuDataDB1[i + 1];
                dt.Rows.Add(index++, commObj.DataAddr, commObj.Val, commObj.DataDescription);
            }


            return dt;
        }
        public DataTable GetDB2DataDetail()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("索引");
            dt.Columns.Add("地址");
            dt.Columns.Add("内容");
            dt.Columns.Add("描述");
            int index = 1;
            for (int i = 0; i < dicCommuDataDB2.Count(); i++)
            {
                PLCDataDef commObj = dicCommuDataDB2[i + 1];

                dt.Rows.Add(index++, commObj.DataAddr, commObj.Val, commObj.DataDescription);
            }
            return dt;

        }
        /// <summary>
        /// 获取当前任务的详细信息
        /// </summary>
        /// <returns></returns>
        public string GetRunningTaskDetail()
        {
            string channelStr = "";
            if(this.LineID<3)
            {
                if(channelIndex == 0)
                {
                    channelStr = "";
                }
                else if(channelIndex == 1)
                {
                    channelStr = "A通道,";
                }
                else 
                {
                    channelStr = "B通道,";
                }
            }
            string taskInfo = channelStr+"当前任务运行到第" + currentTaskPhase.ToString() + " 步;";

            taskInfo += currentTaskDescribe;
            return taskInfo;


        }
        public void ClearErrorStat(string content)
        {
            this.currentStat.StatDescribe = content;
            this.currentStat.Status = EnumNodeStatus.设备空闲;
            LogModel log = new LogModel(this.nodeName, content, EnumLoglevel.提示);
            this.logRecorder.AddLog(log);
        }
        ///// <summary>
        ///// 根据PLC返回的数据，更新状态
        ///// </summary>
        //public void RefreshNodeStatus()
        //{
        //    //this.currentStat.ProductBarcode;
        //}
        public void ThrowErrorStat(string statDescribe, EnumNodeStatus statEnum)
        {
            if (statEnum != EnumNodeStatus.设备故障)
            {
                return;
            }
            if (this.currentStat.Status == EnumNodeStatus.设备空闲 || this.currentStat.Status == EnumNodeStatus.设备使用中)
            {
                //增加日志提示
                LogModel log = new LogModel(this.nodeName, statDescribe, EnumLoglevel.错误);
                this.logRecorder.AddLog(log);
            }
            this.currentStat.Status = statEnum;
            this.currentStat.StatDescribe = statDescribe;
        }
       
        #endregion
        #region 虚接口
        protected virtual bool PreMech(IList<DBAccess.Model.BatteryModuleModel> modList,ref string reStr)
        {
            try
            {
                if(SysCfgModel.SimMode)
                {
                    return true;
                }
                int palletCap = 2;
                if(this.LineID == 3)
                {
                    return true;
                }
                if(this.LineID == 2)
                {
                    palletCap = 4;
                }
                short[] vals = new short[palletCap];
                Array.Clear(vals, 0, palletCap);
                foreach (DBAccess.Model.BatteryModuleModel mod in modList)
                {
                    if (string.IsNullOrWhiteSpace(mod.tag2))
                    {
                        continue;
                    }
                    int seq = 0;
                    if (int.TryParse(mod.tag2, out seq))
                    {
                        if (seq > 0 && seq < palletCap+1)
                        {
                            vals[seq - 1] = 1;
                        }
                    }
                }
                string addr = "D8710";
                if (channelIndex == 2)
                {
                    addr = "D8715";
                }
                if (!plcRW2.WriteMultiDB(addr, palletCap, vals))
                {
                    reStr = "给设备PLC发送'有无料状态字'失败,重新连接PLC";
                    plcRW2.CloseConnect();

                    plcRW2.ConnectPLC(ref reStr);
                   
                    return false;
                }
                System.Threading.Thread.Sleep(500);
                addr = "D8714";
                if (channelIndex == 2)
                {
                    addr = "D8719";
                }
                if (!plcRW2.WriteDB(addr, 1))
                {
                    reStr = "给设备PLC发送'料字完成标志'失败,重新连接PLC";
                    plcRW2.CloseConnect();

                    plcRW2.ConnectPLC(ref reStr);
                 
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return false;
            }
           
        }
        protected virtual bool AfterMech(IList<DBAccess.Model.BatteryModuleModel> modList,ref string reStr)
        {
            try
            {
                //if (SysCfgModel.SimMode)
                //{
                //    return true;
                //}
                if (this.LineID == 3)
                {
                    return true;
                }
                int palletCap = 2;
                if (this.LineID == 2)
                {
                    palletCap = 4;
                }
                string addr = "D8720";
                if (channelIndex == 2)
                {
                    addr = "D8725";
                }
                //short[] vals = new short[5] { 0, 1, 0, 0, 1 };
                
                short[] vals = new short[5];
                if (!plcRW2.ReadMultiDB(addr, 5, ref vals))
                {
                    plcRW2.CloseConnect();

                    plcRW2.ConnectPLC(ref reStr);
                    reStr = "读取设备PLC加工完状态失败";
                    return false;
                }
                if (vals[4] != 1)
                {
                    reStr = string.Format("{0}通道，设备PLC返回通道完成标志不为1，加工未完成",channelIndex);
                    return false;
                }
                foreach (DBAccess.Model.BatteryModuleModel mod in modList)
                {
                    if (string.IsNullOrWhiteSpace(mod.tag2))
                    {
                        continue;
                    }
                    int seq = 0;
                    if (int.TryParse(mod.tag2, out seq))
                    {
                        if (seq > 0 && seq < palletCap+1)
                        {
                            if (vals[seq - 1] == 0)
                            {
                                mod.palletBinded = false;
                                mod.checkResult = 2;//NG
                                if(this.nodeID == "OPB007")
                                {
                                    mod.tag3 = this.nodeName; //标识在哪个工位产生的NG,(共有三个工位，1#铝丝焊、2#铝丝焊、DCIR检测)
                                }
                                modBll.Update(mod);

                                string str = string.Format("模组{0} 加工后出现位置{1}NG被剔除,原工装板ID:{2}", mod.batModuleID, seq, mod.palletID);
                                logRecorder.AddDebugLog(nodeName, str);
                                AddProcessRecord(mod.batModuleID, "模块", "追溯记录", str, "");
                            }
                        }
                    }
                   
                }
                System.Threading.Thread.Sleep(300);
                addr = "D8724";
                if (channelIndex == 2)
                {
                    addr = "D8729";
                }
                if (!plcRW2.WriteDB(addr, 0))
                {
                    plcRW2.CloseConnect();

                    plcRW2.ConnectPLC(ref reStr);
                    reStr = "给设备PLC发送'料字完成标志'失败";
                    return false;
                }
                Thread.Sleep(500);
                int val = 1;
                if (!plcRW2.ReadDB(addr, ref val))
                {
                    plcRW2.CloseConnect();

                    plcRW2.ConnectPLC(ref reStr);
                    return false;
                }
                //logRecorder.AddDebugLog(nodeName, string.Format("返回标志{0}复位成0",addr));
                if (val != 0)
                {
                    logRecorder.AddDebugLog(nodeName, string.Format("有料字返回状态完成标志字{0}复位失败，", addr));
                    return false;
                }
                return true;
                
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                logRecorder.AddDebugLog(nodeName, ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// B线根据检测结果发送
        /// </summary>
        /// <param name="modList"></param>
        /// <param name="reStr"></param>
        /// <returns></returns>
        protected virtual bool PreMechB(IList<DBAccess.Model.BatteryModuleModel> modList, ref string reStr)
        {
            try
            {
                if (SysCfgModel.SimMode)
                {
                    return true;
                }
                int palletCap = 2;
                if (this.LineID == 3 && this.nodeID != "OPC006")
                {
                    return true;
                }
                if (this.LineID == 2 || this.nodeID == "OPC006")
                {
                    palletCap = 4;
                }
                short[] vals = new short[palletCap];
                Array.Clear(vals, 0, palletCap);
                foreach (DBAccess.Model.BatteryModuleModel mod in modList)
                {
                    if (string.IsNullOrWhiteSpace(mod.tag2))
                    {
                        continue;
                    }
                    int seq = 0;
                    if (int.TryParse(mod.tag2, out seq))
                    {
                        if (seq > 0 && seq < palletCap + 1)
                        {
                            if(mod.checkResult == 2)
                            {
                                vals[seq - 1] = 0;
                            }
                            else
                            {
                                vals[seq - 1] = 1;
                            }
                           
                        }
                    }
                }
                string addr = "D8710";
                if (channelIndex == 2)
                {
                    addr = "D8715";
                }
                if (!plcRW2.WriteMultiDB(addr, palletCap, vals))
                {
                    reStr = "给设备PLC发送'有无料状态字'失败,重新连接PLC";
                    plcRW2.CloseConnect();

                    plcRW2.ConnectPLC(ref reStr);

                    return false;
                }
                System.Threading.Thread.Sleep(500);
                addr = "D8714";
                if (channelIndex == 2)
                {
                    addr = "D8719";
                }
                if (!plcRW2.WriteDB(addr, 1))
                {
                    reStr = "给设备PLC发送'料字完成标志'失败,重新连接PLC";
                    plcRW2.CloseConnect();

                    plcRW2.ConnectPLC(ref reStr);

                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return false;
            }
        }

        /// <summary>
        /// 末端机器人发送合格标识
        /// </summary>
        /// <param name="modList"></param>
        /// <param name="reStr"></param>
        /// <returns></returns>
        protected virtual bool SendCheckResult(IList<DBAccess.Model.BatteryModuleModel> modList, ref string reStr)
        {
            try
            {
                if(SysCfgModel.SimMode)
                {
                    return true;
                }
                int palletCap = 2;
                if(this.LineID == 3)
                {
                    return true;
                }
                if(this.LineID == 2)
                {
                    palletCap = 4;
                }
               
                string addr = "D8734";
                //if (channelIndex == 2)
                //{
                //    addr = "D8719";
                //}
                if (!plcRW2.WriteDB(addr, 1))
                {
                    reStr = "给设备PLC发送'合格交互完成标志'失败,重新连接PLC";
                    plcRW2.CloseConnect();

                    plcRW2.ConnectPLC(ref reStr);

                    return false;
                }
                System.Threading.Thread.Sleep(500);
                short[] vals = new short[palletCap];
                Array.Clear(vals, 0, palletCap);
                foreach (DBAccess.Model.BatteryModuleModel mod in modList)
                {
                    if (string.IsNullOrWhiteSpace(mod.checkResult.ToString()))
                    {
                        continue;
                    }
                    int seq = 0;
                    if (int.TryParse(mod.tag2, out seq))
                    {
                        if (seq > 0 && seq < palletCap + 1)
                        {
                            if (mod.checkResult == 2)
                            {
                                vals[seq - 1] = 1;
                            }
                            else
                            {
                                vals[seq - 1] = 0;
                            }
                        }
                    }
                }
                addr = "D8730";
                //if (channelIndex == 2)
                //{
                //    addr = "D8715";
                //}
                if (!plcRW2.WriteMultiDB(addr, palletCap, vals))
                {
                    reStr = "给设备PLC发送'产品检测结果状态字'失败,重新连接PLC";
                    plcRW2.CloseConnect();

                    plcRW2.ConnectPLC(ref reStr);

                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return false;
            }
           
        }
        /// <summary>
        /// 系统启动后，先回复设备运行状态
        /// </summary>
        /// <param name="errStr"></param>
        /// <returns></returns>
        public virtual bool DevStatusRestore()
        {
            bool readDB1OK = false;
            for (int i = 0; i <2; i++)
            {
                if (!ReadDB1())
                {
                    Console.WriteLine(string.Format("恢复设备状态失败，读DB1区数据失败,{0}", this.nodeName));
                    logRecorder.AddDebugLog(nodeName, string.Format("恢复设备状态失败，读DB1区数据失败,{0}", this.nodeName));
                    //return false;
                }
                else
                {
                    readDB1OK = true;
                    break;
                }

            }
            if (!readDB1OK)
            {
                devStatusRestore = false;
                this.currentTaskDescribe = "恢复下电前状态失败，该设备将禁用，请尝试重启软件!";
               // return false;
            }
            devStatusRestore = true;
            
            string strWhere = string.Format("(TaskStatus='执行中' or TaskStatus='超时') and DeviceID='{0}' order by CreateTime ", this.nodeID);
            this.currentTask = ctlTaskBll.GetFirstRequiredTask(strWhere);
            if (this.currentTask != null)
            {
                //this.currentTaskPhase = this.currentTask.TaskPhase;
                this.currentTask.TaskPhase = 1;
                //if (!string.IsNullOrWhiteSpace(this.currentTask.TaskParam))
                //{
                //    string[] paramArray = this.currentTask.TaskParam.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                //    if (paramArray != null && paramArray.Count() > 0)
                //    {
                //        this.rfidUID = paramArray[0];
                //    }
                //}
                this.currentStat.StatDescribe = "设备使用中";
                //if (!string.IsNullOrWhiteSpace(this.currentTask.tag1))
                //{
                //    int.TryParse(this.currentTask.tag1, out channelIndex);
                //}
            }
            //if(this.nodeID.Substring(2,1) == "A" || this.nodeID.Substring(2,1)=="B")
            //{
            //    //this.rfidUIDA = this.plNodeModel.tag1;
            //    //this.rfidUIDB = this.plNodeModel.tag2;
            //}
            return true;

        }
        /// <summary>
        /// 业务逻辑，控制节点的流程执行
        /// </summary>
        /// <param name="reStr"></param>
        /// <returns></returns>
        public virtual bool ExeBusiness(ref string reStr)
        {
            if (!devStatusRestore)
            {
                devStatusRestore = DevStatusRestore();

            }
            if (!devStatusRestore)
            {
                return false;
            }

            if (db2Vals[1] == 1)
            {
                if (this.currentTask != null)
                {
                    //查询未执行完任务，清掉
                    if (!ctlTaskBll.ClearTask(string.Format("DeviceID='{0}'", this.nodeID)))
                    {
                        logRecorder.AddDebugLog(nodeName, "清理任务失败");
                        return false;
                    }
                    this.currentTask = null;

                  
                    this.currentStat.Status = EnumNodeStatus.设备空闲;
                    this.currentStat.ProductBarcode = "";
                    this.currentStat.StatDescribe = "设备空闲";
                    checkFinished = false;
                    currentTaskDescribe = "等待有板信号";

                }
                currentTaskPhase = 0;
                db1ValsToSnd[0] = 1;
                db1ValsToSnd[1] = 0;
                db1ValsToSnd[2] = 0;
                this.rfidUID = string.Empty;
                this.currentStat.Status = EnumNodeStatus.设备空闲;
                this.currentStat.StatDescribe = "工位空闲";
                currentTaskDescribe = "";
                return true;
            }
            if (db2Vals[1] == 2)
            {
                if (currentTaskPhase == 0)
                {
                    this.db1ValsToSnd[0] = 1;
                    this.db1ValsToSnd[1] = 0;
                    currentTaskPhase = 1;
                    this.currentStat.Status = EnumNodeStatus.设备使用中;

                    this.currentStat.StatDescribe = "工作中";

                    ControlTaskModel task = new ControlTaskModel();
                    task.TaskID = System.Guid.NewGuid().ToString("N");
                    task.TaskParam = string.Empty;
                    task.TaskPhase = 1;
                    task.CreateTime = System.DateTime.Now;
                    task.DeviceID = this.nodeID;
                    task.CreateMode = "自动";
                    task.TaskStatus = EnumTaskStatus.执行中.ToString();
                    ctlTaskBll.Add(task);
                    this.currentTask = task;
                }
            }
            if(this.rfidRWList != null && this.rfidRWList.Count()>0)
            {
                this.rfidRW = this.rfidRWList[0];  //临时
                if (this.db2Vals[0] == 1 && this.rfidRWList.Count > 0)
                {
                    this.rfidRW = this.rfidRWList[0];
                }
                else if (this.db2Vals[0] == 2 && this.rfidRWList.Count > 1)
                {
                    this.rfidRW = this.rfidRWList[1];
                }
            }
          
            return true;
        }
       /// <summary>
       /// A,b线双通道逻辑1
       /// </summary>
       /// <param name="reStr"></param>
       /// <returns></returns>
        protected bool ExeBusinessAB(ref string reStr)
        {
            if (!devStatusRestore)
            {
                devStatusRestore = DevStatusRestore();
                //Console.WriteLine(nodeName + "devStatusRestore");
            }
            if (!devStatusRestore)
            {
                return false;
            }
           if(db2Vals[0] == 0)
           {
               if(channelIndex>0)
               {
                  // Console.WriteLine(nodeName + "db2Vals[0] == 0");
                   ChannelReset(channelIndex);
               }
               channelIndex = 0;
           }
           else if(db2Vals[0]>0 )
           {
               if(db2Vals[0] != channelIndex)
               {
                   //Console.WriteLine(nodeName + "db2Vals[0]>0");
                   ChannelReset(channelIndex);
                   channelIndex = db2Vals[0];
               }
           }
           
           if(!SysCfgModel.SimMode)
           {
               if (db2Vals[1] != 2)
               {
                   this.rfidUIDA = "";
                   //if (this.nodeName != "C线打带")//此处rfid与其他不一样不需要清缓存
                   //{
                   //    (this.rfidRWList[0] as RfidCF).ClearBufUID();
                   //}
               }
               if(db2Vals[2] != 2)
               {
                   this.rfidUIDB = "";
                   //if (this.nodeName != "C线打带")//此处rfid与其他不一样不需要清缓存
                   //{
                   //    (this.rfidRWList[1] as RfidCF).ClearBufUID();
                   //}
               }
           }
           //Console.WriteLine(nodeName + "ExeRfidBusinessAB前");
           ExeRfidBusinessAB();
          // Console.WriteLine(nodeName + "ExeRfidBusinessAB后");
           if(channelIndex==0)
           {
               return true;
           }
          
            if (db2Vals[channelIndex] == 1)
            {
                 
                ChannelReset(channelIndex);
                //Console.WriteLine(nodeName + "db2Vals[channelIndex] == 1");
            }
            
            if (db2Vals[channelIndex] == 2)
            {
               
                ChannelBegin(channelIndex);
                //Console.WriteLine(nodeName + "db2Vals[channelIndex] == 2");
            }
            //if (this.rfidRWList != null && this.rfidRWList.Count() > 0)
            //{
            //    this.rfidRW = this.rfidRWList[0];  //临时
            //    if (channelIndex == 1 && this.rfidRWList.Count > 0)
            //    {
            //        this.rfidRW = this.rfidRWList[0];
            //    }
            //    else if (channelIndex == 2 && this.rfidRWList.Count > 1)
            //    {
            //        this.rfidRW = this.rfidRWList[1];
            //    }
            //}

            return true;
        }
        protected virtual bool ChannelReset(int channel)
        {
            if(channel<1)
            {
                return false;
            }
           
            currentTaskPhase = 0;
            db1ValsToSnd[channel - 1] = 1;
            db1ValsToSnd[2+channel-1] = 1;

            this.rfidUID = string.Empty;
            //if(!SysCfgModel.SimMode)
            //{
            //    if(this.rfidRWList.Count()>=channel)
            //    {
            //        if (this.nodeName != "C线打带")//此处rfid与其他不一样不需要清缓存
            //        {
            //            (this.rfidRWList[channel - 1] as RfidCF).ClearBufUID();
            //        }
            //    }
            //}
            this.currentStat.Status = EnumNodeStatus.设备空闲;
            this.currentStat.StatDescribe = "工位空闲";
            currentTaskDescribe = "";
            if (this.currentTask != null)
            {
                //查询未执行完任务，清掉
                this.currentTask = null;
                this.currentStat.Status = EnumNodeStatus.设备空闲;
                this.currentStat.ProductBarcode = "";
                this.currentStat.StatDescribe = "设备空闲";
                checkFinished = false;
                currentTaskDescribe = "等待有板信号";
                if (this.nodeID == "OPA011" || this.nodeID == "OPA012")
                {
                    return true;
                }
                if (!ctlTaskBll.ClearTask(string.Format("DeviceID='{0}'", this.nodeID)))
                {
                    logRecorder.AddDebugLog(nodeName, "清理任务失败");
                    return false;
                }
                
            }
            //if (channel == 1)
            //{
            //    this.rfidUIDA = "";
            //}
            //else
            //{
            //    this.rfidUIDB = "";
            //}
            return true;
        }
        protected bool ChannelBegin(int channel)
        {
            
            if(channel<1)
            {
                return false;
            }
            if (currentTaskPhase == 0)
            {
               // this.db1ValsToSnd[channel - 1] = 1;
                this.db1ValsToSnd[2] = 0;
                currentTaskPhase = 1;
                this.currentStat.Status = EnumNodeStatus.设备使用中;

                this.currentStat.StatDescribe = "工作中";
                if(this.nodeID=="OPA011" || this.nodeID=="OPA012")
                {
                    return true;
                }
                ControlTaskModel task = new ControlTaskModel();
                task.TaskID = System.Guid.NewGuid().ToString("N");
                task.TaskParam = string.Empty;
                task.TaskPhase = 1;
                task.CreateTime = System.DateTime.Now;
                task.DeviceID = this.nodeID;
                task.CreateMode = "自动";
                task.TaskStatus = EnumTaskStatus.执行中.ToString();
                task.tag1 = channel.ToString();
                ctlTaskBll.Add(task);
                this.currentTask = task;
            }
            return true;
        }
        protected virtual void ExeRfidBusinessAB()
        {
            if (this.nodeID == "OPA011" || this.nodeID == "OPA012") //点胶1,2工位不需要读rfid
            {
                return;
            }
            if(this.rfidRWList == null || this.rfidRWList.Count()<1)
            {
                return;
            }
            PLNodesBll plNodeBll = new PLNodesBll();
            if(this.db2Vals[1] == 2)
            {
                //A通道
                if(string.IsNullOrWhiteSpace(this.rfidUIDA))
                {
                    IrfidRW rw = null;
                    if (SysCfgModel.SimMode)
                    {
                        this.rfidUIDA = this.SimRfidUID;
                    }
                    else if (this.rfidRWList.Count > 0)
                    {
                        rw = this.rfidRWList[0];
                        this.rfidUIDA =rw.ReadUID(); 
                       
                    }
                    if (string.IsNullOrWhiteSpace(this.rfidUIDA))
                    {
                        //读RFID失败 

                        if (this.db1ValsToSnd[0] != 3)
                        {
                            //logRecorder.AddDebugLog(nodeName, "读RFID失败");
                            db1ValsToSnd[0] = 3;
                        }
                        else
                        {
                            db1ValsToSnd[0] = 1;
                        }
                        Thread.Sleep(1000);
                        this.currentStat.Status = EnumNodeStatus.无法识别;
                        this.currentStat.StatDescribe = "读A通道RFID失败:" + db1ValsToSnd[0].ToString();
                        this.currentTaskDescribe = "读A通道RFID失败:" + db1ValsToSnd[0].ToString();

                    }
                    else
                    {
                       
                        this.plNodeModel.tag1 = this.rfidUIDA;
                        plNodeBll.Update(this.plNodeModel);
                        logRecorder.AddDebugLog(nodeName, string.Format("A通道读到RFID:{0}", this.rfidUIDA));
                        this.db1ValsToSnd[0] = 2;
                        //if (!SysCfgModel.SimMode)
                        //{
                        //    if (this.nodeName != "C线打带")//此处rfid与其他不一样不需要清缓存
                        //    {
                        //        (rw as DevAccess.RfidCF).ClearBufUID();
                                
                        //    }
                        //}
                    }
                }
                else
                {
                    this.db1ValsToSnd[0] = 2;
                }
                
            }
            else if(this.db2Vals[1]==1)
            {
                this.db1ValsToSnd[0] = 1;
                this.plNodeModel.tag1 = "";
                plNodeBll.Update(this.plNodeModel);
            }

            if(this.db2Vals[2] == 2)
            {
                //B通道
                IrfidRW rw = null;
                if (string.IsNullOrWhiteSpace(this.rfidUIDB))
                {
                     if (SysCfgModel.SimMode)
                     {
                         this.rfidUIDB = this.SimRfidUID;
                     }
                     else if (this.rfidRWList.Count > 1)
                    {
                        rw = this.rfidRWList[1];
                        this.rfidUIDB = rw.ReadUID(); 
                    
                    }
                     if (string.IsNullOrWhiteSpace(this.rfidUIDB))
                     {
                         if (this.db1ValsToSnd[1] != 3)
                         {
                             //logRecorder.AddDebugLog(nodeName, "读RFID失败");
                             db1ValsToSnd[1] = 3;
                         }
                         else
                         {
                             db1ValsToSnd[1] = 1;
                         }
                         Thread.Sleep(1000);
                         this.currentStat.Status = EnumNodeStatus.无法识别;
                         this.currentStat.StatDescribe = "读B通道RFID失败:" + db1ValsToSnd[1].ToString();
                         this.currentTaskDescribe = "读B通道RFID失败:" + db1ValsToSnd[1].ToString();
                     }
                     else
                     {
                         this.plNodeModel.tag2 = this.rfidUIDB;
                         plNodeBll.Update(this.plNodeModel);
                         logRecorder.AddDebugLog(nodeName, string.Format("B通道读到RFID:{0}", this.rfidUIDB));
                         this.db1ValsToSnd[1] = 2;
                         //if (!SysCfgModel.SimMode)
                         //{
                         //    if (this.nodeName != "C线打带")//此处rfid与其他不一样不需要清缓存
                         //    {
                         //        (rw as DevAccess.RfidCF).ClearBufUID();
                         //    }
                         //}
                     }
                   
                }
                else
                {
                    this.db1ValsToSnd[1] = 2;
                }
               
            }
            else if(this.db2Vals[2] == 1)
            {
                
                this.db1ValsToSnd[1] = 1;
                this.plNodeModel.tag2 = "";
                plNodeBll.Update(this.plNodeModel);
            }
          
        }
        /// <summary>
        /// C线单通道逻辑1
        /// </summary>
        /// <param name="reStr"></param>
        /// <returns></returns>
        protected bool ExeBusinessC(ref string reStr)
        {
            if (!nodeEnabled)
            {
                return true;
            }
            if (!devStatusRestore)
            {
                devStatusRestore = DevStatusRestore();

            }
            if (!devStatusRestore)
            {

                return false;
            }
            if (this.rfidRWList.Count() > 0)
            {
                this.rfidRW = this.rfidRWList[0];  //临时
            }
            if (db2Vals[0] == 1)
            {
                if (this.currentTask != null)
                {
                    //查询未执行完任务，清掉
                    if (!ctlTaskBll.ClearTask(string.Format("DeviceID='{0}'", this.nodeID)))
                    {
                        logRecorder.AddDebugLog(nodeName, "清理任务失败");
                        return false;
                    }
                    this.currentTask = null;


                    this.currentStat.Status = EnumNodeStatus.设备空闲;
                    this.currentStat.ProductBarcode = "";
                    this.currentStat.StatDescribe = "设备空闲";
                    checkFinished = false;
                    currentTaskDescribe = "等待有板信号";

                }
                currentTaskPhase = 0;
                db1ValsToSnd[0] = 1;
                db1ValsToSnd[1] = 0;

                this.rfidUID = string.Empty;
                //if(!SysCfgModel.SimMode)
                //{
                //    if (this.nodeName != "C线打带")//此处rfid与其他不一样不需要清缓存
                //    {
                //        (rfidRW as RfidCF).ClearBufUID();
                //    }
                   
                //}
                this.currentStat.Status = EnumNodeStatus.设备空闲;
                this.currentStat.StatDescribe = "工位空闲";
                currentTaskDescribe = "";

            }
            if (db2Vals[0] == 2)
            {
                if (currentTaskPhase == 0)
                {
                    this.db1ValsToSnd[0] = 1;
                    this.db1ValsToSnd[1] = 1;
                    currentTaskPhase = 1;
                    this.currentStat.Status = EnumNodeStatus.设备使用中;

                    this.currentStat.StatDescribe = "工作中";

                    ControlTaskModel task = new ControlTaskModel();
                    task.TaskID = System.Guid.NewGuid().ToString("N");
                    task.TaskParam = string.Empty;
                    task.TaskPhase = 1;
                    task.CreateTime = System.DateTime.Now;
                    task.DeviceID = this.nodeID;
                    task.CreateMode = "自动";
                    task.TaskStatus = EnumTaskStatus.执行中.ToString();
                    ctlTaskBll.Add(task);
                    this.currentTask = task;
                }
            }
         
            return true;
        }
        /// <summary>
        /// 控制流程的命令数据提交
        /// </summary>
        /// <param name="reStr"></param>
        /// <returns></returns>
        public virtual bool NodeCmdCommit(bool diffSnd, ref string reStr)
        {
            try
            {
                int blockNum = this.dicCommuDataDB1.Count();
                if (!SysCfgModel.SimMode)
                {

                    if (SysCfgModel.PlcCommSynMode)
                    {
                        //this.db1StartAddr = "D3000"; //test
                        if (!plcRW.WriteMultiDB(this.db1StartAddr, blockNum, this.db1ValsToSnd))
                        {
                            ThrowErrorStat("发送设备命令失败！", EnumNodeStatus.设备故障);
                            return false;
                        }
                        for (int i = 0; i < blockNum; i++)
                        {
                            int commID = i + 1;
                            this.dicCommuDataDB1[commID].Val = this.db1ValsToSnd[i];

                        }
                    }
                    else
                    {
                        int addrSt = int.Parse(this.db1StartAddr.Substring(1)) - int.Parse(SysCfgModel.DB1Start.Substring(1));
                        for (int i = 0; i < blockNum; i++)
                        {
                            int commID = i + 1;
                            this.dicCommuDataDB1[commID].Val = this.db1ValsToSnd[i];
                            plcRW.Db1Vals[addrSt + i] = this.db1ValsToSnd[i];
                            // this.db1ValsReal[i] = vals[i];
                        }
                    }

                }
                else
                {
                    plcRW.WriteMultiDB(this.db1StartAddr, blockNum, db1ValsToSnd);
                    for (int i = 0; i < dicCommuDataDB1.Count(); i++)
                    {
                        int commID = i + 1;
                        PLCDataDef commObj = dicCommuDataDB1[commID];
                        commObj.Val = db1ValsToSnd[i];

                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return false;
               
            }
           

        }
        public virtual bool NodeDB2Commit(int addrIndex,short val,ref string reStr)
        {
            try
            {
                int commID = addrIndex + 1;
                PLCDataDef commObj = dicCommuDataDB2[commID];
                if (!SysCfgModel.SimMode)
                {
                    bool re = plcRW.WriteDB(commObj.DataAddr, val);
                    plcRW.CloseConnect();
                    
                    plcRW.ConnectPLC(ref reStr);
                    reStr = "发送数据失败，尝试重新连接,"+reStr;
                    return re;
                }
                else
                {

                    commObj.Val = val;
                }
                return true;
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return false;
            }
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xe"></param>
        /// <returns></returns>
        public virtual bool BuildCfg(XElement xe,ref string reStr)
        {
            this.nodeID = xe.Attribute("id").Value;
            this.nodeName = xe.Attribute("nodeName").Value;
            XElement baseDataXE = xe.Element("BaseDatainfo");
            if(baseDataXE == null)
            {
                reStr = this.nodeID + "，没有BaseDatainfo节点配置信息";
                return false;
            }
          
            //mes nodeid，nodename
            XElement db1XE = baseDataXE.Element("DB1Addr");
            string db1StartStr = db1XE.Attribute("addrStart").Value;
            this.db1StartAddr = db1StartStr;
            int db1Start = int.Parse(db1StartStr.Substring(1));
            this.db1BlockNum = int.Parse(db1XE.Attribute("blockNum").Value);
            int db1ID = 1;
            this.dicCommuDataDB1 = new Dictionary<int, PLCDataDef>();
            db1ValsReal = new Int16[db1BlockNum];
            db1ValsToSnd = new Int16[db1BlockNum];
            for (int i = 0; i < db1BlockNum; i++)
            {

                PLCDataDef commData = new PLCDataDef();
                commData.CommuID = db1ID++;
                commData.CommuMethod = EnumCommMethod.PLC_MIT_COMMU;
                commData.DataByteLen = 2;
                commData.DataDescription = "";
                commData.DataTypeDef = EnumCommuDataType.DEVCOM_short;
                commData.Val = 0;
                commData.DataAddr = "D" + (db1Start+i).ToString();
                dicCommuDataDB1[commData.CommuID] = commData;
                //db1Vals[i] = 0;
            }
            XElement db2XE = baseDataXE.Element("DB2Addr");
            string db2StartStr = db2XE.Attribute("addrStart").Value;
            this.db2StartAddr = db2StartStr;
            int db2Start = int.Parse(db2StartStr.Substring(1));
            int db2BlockNum = int.Parse(db2XE.Attribute("blockNum").Value);
            int db2ID = 1;
            this.dicCommuDataDB2 = new Dictionary<int, PLCDataDef>();
            db2Vals = new Int16[db2BlockNum];
            for (int i = 0; i < db2BlockNum; i++)
            {
                PLCDataDef commData = new PLCDataDef();
                commData.CommuID = db2ID++;
                commData.CommuMethod = EnumCommMethod.PLC_MIT_COMMU;
                commData.DataByteLen = 2;
                commData.DataDescription = "";
                commData.DataTypeDef = EnumCommuDataType.DEVCOM_short;
                commData.Val = 0;
                db2Vals[i] = 0;
                commData.DataAddr = "D" + (db2Start + i).ToString();
                dicCommuDataDB2[commData.CommuID] = commData;
            }
            XAttribute attr = baseDataXE.Attribute("barScanner");
            if (attr != null)
            {
                this.barcodeID = int.Parse(attr.Value);
            }
            attr = baseDataXE.Attribute("rfid");
            if (attr != null)
            {
                this.rfidIDS = new List<byte>();
                string[] rfidIDS = attr.Value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach(string str in rfidIDS)
                {
                    byte rid = byte.Parse(str);
                    this.rfidIDS.Add(rid);
                }
            }

            attr = baseDataXE.Attribute("plcID");
            if (attr != null)
            {
                this.plcID = int.Parse(attr.Value);
            }
            attr = baseDataXE.Attribute("plcID2");
            if(attr != null)
            {
                this.plcID2 = int.Parse(attr.Value);
            }
            attr = baseDataXE.Attribute("ccd");
            if(attr != null)
            {
              this.ccdDevID = int.Parse(attr.Value.ToString());

            }
            attr = baseDataXE.Attribute("ccdDevName");
            if(attr != null)
            {
                this.ccdDevName = attr.Value.ToString();
            }

            this.currentStat = new CtlNodeStatus(nodeName);
            this.currentStat.Status = EnumNodeStatus.设备空闲;
            this.currentStat.StatDescribe = "空闲状态";
            return true;
        }
        public virtual void DevCmdReset()
        {
            Array.Clear(db1ValsToSnd, 0, db1ValsToSnd.Count());
            //string reStr = "";
            //NodeCmdCommit(false,ref reStr);
        }

        /// <summary>
        /// 下线记录（包括不合格检测，提前下线），若有重复，更新最新的
        /// </summary>
        /// <param name="productBarcode"></param>
        //protected void OutputRecord(string productBarcode)
        //{
        //    //按时间先后查询最后一条记录，更改记录状态，若查询结果为空，则返回
        //    string strWhere = string.Format("productBarcode='{0}' order by inputTime desc ",productBarcode);
        //    List<ProduceRecordModel> recordList = produceRecordBll.GetModelList(strWhere);
        //    if(recordList == null || recordList.Count()<1)
        //    {
        //        return;
        //    }
        //    recordList[0].outputTime = System.DateTime.Now;
        //    recordList[0].lineOuted = true;
        //    recordList[0].outputNode = this.nodeName;
        //    produceRecordBll.Update(recordList[0]);
        //}
        //public void NodeLoop()
        //{
        //    try
        //    {

        //        string reStr = "";
        //        DateTime commSt = System.DateTime.Now;
        //        if (!ReadDB2(ref reStr))
        //        {
        //            return;
        //        }
        //        //DateTime commEd = System.DateTime.Now;
        //        //TimeSpan ts = commEd - commSt;
        //        //string dispCommInfo = string.Format("PLC 读通信周期:{0}毫秒", (int)ts.TotalMilliseconds);
        //        //if (ts.TotalMilliseconds > 100)
        //        //{
        //        //    node.LogRecorder.AddDebugLog(node.NodeName, dispCommInfo);
        //        //}

        //        //DateTime commEd = System.DateTime.Now;
        //        //TimeSpan ts = commEd - commSt;
        //        //string dispCommInfo = string.Format("PLC读状态周期:{0}毫秒", (int)ts.TotalMilliseconds);
        //        //if (ts.TotalMilliseconds > 500)
        //        //{
        //        //    node.LogRecorder.AddDebugLog(node.NodeName, dispCommInfo);
        //        //}

        //        if (!ExeBusiness(ref reStr))
        //        {
        //            return;
        //        }

        //        // commSt = System.DateTime.Now;
        //        if (!NodeCmdCommit(true, ref reStr))
        //        {
        //            return;
        //        }
        //        //commEd = System.DateTime.Now;
        //        //ts = commEd - commSt;
        //        //dispCommInfo = string.Format("PLC 发送通信周期:{0}毫秒", (int)ts.TotalMilliseconds);
        //        //if (ts.TotalMilliseconds > 500)
        //        //{
        //        //    node.LogRecorder.AddDebugLog(node.NodeName, dispCommInfo);
        //        //}
        //        DateTime commEd = System.DateTime.Now;
        //        TimeSpan ts = commEd - commSt;
        //        //   string dispCommInfo = string.Format("PLC控制周期:{0}毫秒", (int)ts.TotalMilliseconds);
        //        //  if (ts.TotalMilliseconds > 600)
        //        {
        //            // node.LogRecorder.AddDebugLog(node.NodeName, dispCommInfo);
        //            CurrentStat.StatDescribe = string.Format("周期:{0}毫秒", (int)ts.TotalMilliseconds);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(NodeName + ex.ToString());
        //        ThrowErrorStat(ex.ToString(), EnumNodeStatus.设备故障);
        //    }
        //}
        #endregion
        #region 内部功能接口
        

        /// <summary>
        /// db1 条码赋值
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="db1StIndex">db1地址块索引起始，从0开始编号</param>
        protected void BarcodeFillDB1(string barcode,int db1StIndex)
        {
            if(barcode.Count()>30)
            {
                barcode = barcode.Substring(0,30);
            }
            byte[] barcodeBytes = System.Text.UTF8Encoding.Default.GetBytes(barcode);
            for (int i = 0; i < barcodeBytes.Count(); i++)
            {
                db1ValsToSnd[db1StIndex + i] = barcodeBytes[i];
            }
            //string reStr="";
            //if(!NodeCmdCommit(false,ref reStr))
            //{
            //    ThrowErrorStat("PLC数据提交错误", EnumNodeStatus.设备故障);
            //}
        }

        /// <summary>
        /// 分析工位状态
        /// </summary>
        /// <param name="reStr"></param>
        /// <returns></returns>
        protected virtual bool NodeStatParse(ref string reStr)
        {
            if (db2Vals[0] == 1)
            {
                if (!productChecked)
                {
                    logRecorder.AddDebugLog(nodeName, "检测到有产品");
                }
                productChecked = true;
                if (currentTaskPhase == 0)
                {
                    ControlTaskModel task = new ControlTaskModel();
                    task.TaskID = System.Guid.NewGuid().ToString("N");
                    task.TaskParam = string.Empty;
                    task.TaskPhase = 1;
                    task.CreateTime = System.DateTime.Now;
                    task.DeviceID = this.nodeID;
                    task.CreateMode = "自动";
                    task.TaskStatus = EnumTaskStatus.执行中.ToString();
                    ctlTaskBll.Add(task);
                    this.currentTask = task;

                    currentTaskPhase = 1; //开始流程
                    checkEnable = true;
                    //logRecorder.AddDebugLog(nodeName, "检测到产品");


                }
            }
            else if (db2Vals[0] == 0)
            {
                if (productChecked)
                {
                    logRecorder.AddDebugLog(nodeName, "产品离开工位");
                }
                productChecked = false;
                //if(currentTaskPhase != 0)
                if (this.currentTask != null)
                {
                    //查询未执行完任务，清掉
                    if (!ctlTaskBll.ClearTask(string.Format("DeviceID='{0}'", this.nodeID)))
                    {
                        logRecorder.AddDebugLog(nodeName, "清理任务失败");
                        return false;
                    }
                    this.currentTask = null;

                    DevCmdReset();
                   
                    this.currentStat.Status = EnumNodeStatus.设备空闲;
                    this.currentStat.ProductBarcode = "";
                    this.currentStat.StatDescribe = "设备空闲";
                    checkFinished = false;
                    currentTaskDescribe = "等待有板信号";

                }
                currentTaskPhase = 0; //复位
                checkEnable = true;
              
            }
            else
            {
                //ThrowErrorStat("PLC错误的状态数据", EnumNodeStatus.设备故障);
                return true;
            }
            return true;
        }

        protected virtual bool MesDatalocalSave(string productBarcode,int checkResult,string detectCodes,string dataValues,int step_mark)
        {
            string strWhere = string.Format("SERIAL_NUMBER='{0}' and AutoStationName='{1}' and UPLOAD_FLAG=0", productBarcode, this.nodeName);
            List<LOCAL_MES_STEP_INFOModel> unuploads = mesInfoBllLocal.GetModelList(strWhere);
            if(unuploads != null && unuploads.Count>0)
            {
                foreach(LOCAL_MES_STEP_INFOModel m in unuploads)
                {
                    if(!mesInfoBllLocal.Delete(m.RECID))
                    {
                        return false;
                    }
                }
            }
            //1 存储基本信息
            LOCAL_MES_STEP_INFOModel infoModel = new LOCAL_MES_STEP_INFOModel();
            infoModel.CHECK_RESULT = checkResult;
            infoModel.DEFECT_CODES = detectCodes;
            infoModel.LAST_MODIFY_TIME = System.DateTime.Now;
            infoModel.RECID = System.Guid.NewGuid().ToString();
            infoModel.SERIAL_NUMBER = productBarcode;
            infoModel.STEP_NUMBER = this.mesNodeID; //mes工位号
            infoModel.TRX_TIME = System.DateTime.Now;
            infoModel.UPLOAD_FLAG = false;
            infoModel.USER_NAME="";
            infoModel.STATUS = 0;
            //if (!string.IsNullOrWhiteSpace(workerID))
            //{
            //    infoModel.USER_NAME = workerID;
            //}
           
            infoModel.STEP_MARK = step_mark;
            infoModel.AutoStationName = this.nodeName;
            if(!mesInfoBllLocal.Add(infoModel))
            {
                return false;
            }
            //2存储细节数据
            List<LOCAL_MES_STEP_INFO_DETAILModel> unuploadsDetial = mesDetailBllLocal.GetModelList(strWhere);
            if(unuploadsDetial != null && unuploadsDetial.Count>0)
            {
                foreach(LOCAL_MES_STEP_INFO_DETAILModel m in unuploadsDetial)
                {
                    mesDetailBllLocal.Delete(m.RECID);
                }
            }
            LOCAL_MES_STEP_INFO_DETAILModel detailModel = new LOCAL_MES_STEP_INFO_DETAILModel();
            detailModel.DATA_NAME = this.mesNodeName; //
            detailModel.DATA_VALUE = dataValues;
            detailModel.LAST_MODIFY_TIME = System.DateTime.Now;
            detailModel.RECID = System.Guid.NewGuid().ToString();
            detailModel.SERIAL_NUMBER = currentStat.ProductBarcode;
            detailModel.STATUS = 0;
            detailModel.STEP_NUMBER = this.mesNodeID; //
            detailModel.TRX_TIME = System.DateTime.Now;
            detailModel.UPLOAD_FLAG = false;
            detailModel.AutoStationName = this.nodeName;
            
            if(!mesDetailBllLocal.Add(detailModel))
            {
                return false;
            }
           
            return true;
        }
        
        protected virtual bool ProcessStartCheck(int barcodeStIndex,bool preNgCheck)
        {
            db1ValsToSnd[1] = 1;//流程锁定
            if (this.currentStat.Status == EnumNodeStatus.设备故障)
            {
                return false;
            }

            this.currentStat.Status = EnumNodeStatus.设备使用中;
            this.currentStat.ProductBarcode = "";
            this.currentStat.StatDescribe = "设备使用中";

            //读条码，rfid，绑定

            currentTaskDescribe = "开始读RFID";
            if (string.IsNullOrWhiteSpace(rfidUID))
            {
                if (SysCfgModel.SimMode)
                {
                    rfidUID = this.SimRfidUID;
                }
                else
                {
                    rfidUID = rfidRW.ReadUID();
                   
                }
            }
          
            if (string.IsNullOrEmpty(rfidUID))
            {
                if (db1ValsToSnd[0] != db1StatRfidFailed)
                {
                    logRecorder.AddDebugLog(nodeName, "读RFID卡失败！");
                }
                db1ValsToSnd[0] = db1StatRfidFailed;

                this.currentStat.StatDescribe = "读RFID卡失败！";
                return false;
            }
            this.currentTask.TaskPhase = this.currentTaskPhase;
            this.currentTask.TaskParam = rfidUID;

            this.ctlTaskBll.Update(this.currentTask);
            OnlineProductsModel productBind = productBindBll.GetModelByrfid(rfidUID);
            if (productBind == null)
            {
                if (db1ValsToSnd[0] != db1StatCheckUnbinded)
                {
                    logRecorder.AddDebugLog(nodeName, "未投产，rfid:" + rfidUID);
                }
                db1ValsToSnd[0] = db1StatCheckUnbinded;
                // this.currentTaskPhase = 5;
                this.currentStat.StatDescribe = "未投产";
                this.currentTaskDescribe = "未投产";
                checkEnable = false;
                return false;
            }
            productBind.currentNode = this.nodeName;
            productBindBll.Update(productBind);
            BarcodeFillDB1(productBind.productBarcode, barcodeStIndex);
           
            if(preNgCheck)
            {
                int reDetectQuery = ReDetectQuery(productBind.productBarcode);
                if (0 == reDetectQuery)
                {
                    db1ValsToSnd[0] = db1StatCheckOK;
                    checkEnable = false;
                    string logStr = string.Format("{0}本地已经存在检验记录,检验结果：OK", productBind.productBarcode);
                    logRecorder.AddDebugLog(nodeName, logStr);
                    this.currentTaskDescribe = logStr;
                    return false;
                }
                else if (1 == reDetectQuery)
                {
                    db1ValsToSnd[0] = db1StatNG;
                    checkEnable = false;
                    string logStr = string.Format("{0}本地已经存在检验记录,检验结果：NG", productBind.productBarcode);
                    logRecorder.AddDebugLog(nodeName, logStr);
                    this.currentTaskDescribe = logStr;
                    return false;
                }

                if (!PreDetectCheck(productBind.productBarcode))
                {
                    db1ValsToSnd[0] = db1StatNG;//
                    string logStr = string.Format("{0} 在前面工位有检测NG项", productBind.productBarcode);
                    logRecorder.AddDebugLog(this.nodeName, logStr);
                    checkEnable = false;
                    this.currentTaskDescribe = logStr;
                    return false;
                }
                string reStr = "";
                if (!LossCheck(productBind.productBarcode, ref reStr))
                {
                    db1ValsToSnd[0] = db1StatNG;//
                    string logStr = string.Format("{0} 检测漏项,{1}", productBind.productBarcode, reStr);
                    logRecorder.AddDebugLog(this.nodeName, logStr);
                    checkEnable = false;
                    this.currentTaskDescribe = logStr;
                    return false;
                }
            }
            
           

            this.currentStat.ProductBarcode = productBind.productBarcode;
            return true;
        }
        public bool UploadMesdata(bool syn,string productBarcode, string[] mesProcessSeq, ref string reStr)
        {
            if(SysCfgModel.MesOfflineMode)
            {
                reStr = "离线模式";
                return true;
            }
            if(syn)
            {
                return AsyUploadMesdata(productBarcode, mesProcessSeq, ref reStr);
            }
            else
            {
                DelegateUploadMes dlgt = new DelegateUploadMes(AsyUploadMesdata);
                IAsyncResult ar = dlgt.BeginInvoke(productBarcode, mesProcessSeq, ref reStr, null, dlgt);
                return true;
            }
           
        }
        protected bool AsyUploadMesdata(string productBarcode, string[] mesProcessSeq, ref string reStr)
        {
            try
            {
                #region 1上传基本数据
                for (int i = 0; i < mesProcessSeq.Count(); i++)
                {
                    string strWhere = string.Format("SERIAL_NUMBER='{0}' and UPLOAD_FLAG = 0 and STEP_NUMBER='{1}' order by TRX_TIME asc", productBarcode, mesProcessSeq[i]);
                    List<LOCAL_MES_STEP_INFOModel> models = localMesBasebll.GetModelList(strWhere);
                    if (models == null || models.Count() < 1)
                    {
                        continue;
                    }
                    string process = mesProcessSeq[i];
                    if (process == "MES投产位")
                    {
                        int mesRE = mesDA.MesAssemAuto(new string[] { productBarcode, "L09" }, ref reStr);
                        if (mesRE != 1)
                        {
                            continue;
                        }
                        else
                        {
                            models[0].UPLOAD_FLAG = true;
                            localMesBasebll.Update(models[0]);
                        }
                        continue;
                    }
                    LOCAL_MES_STEP_INFOModel m = models[models.Count() - 1].Clone() as LOCAL_MES_STEP_INFOModel;
                    if (models.Count() > 1)
                    {
                        for (int mIndex = 0; mIndex < models.Count() - 1; mIndex++)
                        {
                            LOCAL_MES_STEP_INFOModel tempModel = models[mIndex];
                            if (tempModel.CHECK_RESULT == 1)
                            {
                                m.CHECK_RESULT = 1;
                            }
                            m.DEFECT_CODES = tempModel.DEFECT_CODES + ";" + m.DEFECT_CODES;
                        }
                    }
                    if (!UploadMesbasicData(m))
                    {
                        return false;
                    }
                    for (int mIndex = 0; mIndex < models.Count(); mIndex++)
                    {
                        LOCAL_MES_STEP_INFOModel tempModel = models[mIndex];
                        tempModel.UPLOAD_FLAG = true;
                        localMesBasebll.Update(tempModel);
                    }
                    Thread.Sleep(2000);
                    if(m.CHECK_RESULT==1)
                    {
                        break;
                    }

                }
                #endregion
               
                #region 2 上传详细数据
                for (int i = 0; i < mesProcessSeq.Count(); i++)
                {
                    string strWhere = string.Format("SERIAL_NUMBER='{0}' and UPLOAD_FLAG = 0 and STEP_NUMBER='{1}' order by TRX_TIME asc", productBarcode, mesProcessSeq[i]);
                    List<LOCAL_MES_STEP_INFO_DETAILModel> models = localMesDetailbll.GetModelList(strWhere);
                    if (models == null || models.Count() < 1)
                    {
                        continue;
                    }
                    LOCAL_MES_STEP_INFO_DETAILModel m = models[models.Count() - 1].Clone() as LOCAL_MES_STEP_INFO_DETAILModel;
                    if (models.Count() > 1)
                    {
                        for (int mIndex = 0; mIndex < models.Count() - 1; mIndex++)
                        {
                            LOCAL_MES_STEP_INFO_DETAILModel tempModel = models[mIndex];
                            m.DATA_VALUE = tempModel.DATA_VALUE + ";" + m.DATA_VALUE;

                        }
                    }
                    if (!UploadMesdetailData(m))
                    {
                        reStr = string.Format("上传MES详细数据失败,{0},{1}", productBarcode, mesProcessSeq[i]);
                        return false;
                    }
                    for (int mIndex = 0; mIndex < models.Count(); mIndex++)
                    {
                        LOCAL_MES_STEP_INFO_DETAILModel tempModel = models[mIndex];
                        tempModel.UPLOAD_FLAG = true;
                        localMesDetailbll.Update(tempModel);
                    }
                   

                }
                #endregion
              
                return true;
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                this.ThrowErrorStat(ex.ToString(), EnumNodeStatus.设备故障);
                return false;

            }
        }
        protected bool UploadMesbasicData(LOCAL_MES_STEP_INFOModel m)
        {

            //临时
            //return true;
            //if (mesDA.MesBaseExist(m.RECID)) //？
            //{
            //    m.RECID = System.Guid.NewGuid().ToString(); 
            //}
           
            FT_MES_STEP_INFOModel ftM = new FT_MES_STEP_INFOModel();

            ftM.CHECK_RESULT = m.CHECK_RESULT;
            ftM.DEFECT_CODES = m.DEFECT_CODES;
            ftM.LAST_MODIFY_TIME =m.LAST_MODIFY_TIME;
            ftM.REASON = m.REASON;
            ftM.RECID = m.RECID;
            ftM.SERIAL_NUMBER = m.SERIAL_NUMBER;
            ftM.STATUS = m.STATUS;
            ftM.STEP_MARK = m.STEP_MARK;
            ftM.STEP_NUMBER = m.STEP_NUMBER;
            ftM.TRX_TIME = m.TRX_TIME;
            ftM.USER_NAME = m.USER_NAME;
            try
            {
                int reTryMax = 10;
                int reTryCounter = 0;
                while (!mesDA.AddMesBaseinfo(ftM))
                {
                    Thread.Sleep(1000);
                    reTryCounter++;
                    if (reTryCounter > reTryMax)
                    {
                        logRecorder.AddDebugLog(this.nodeName, string.Format("上传基本数据到MES失败,条码：{0},工位：{1}", ftM.SERIAL_NUMBER, ftM.STEP_NUMBER));
                        return false;
                    }
                }
                
                logRecorder.AddDebugLog(this.nodeName, string.Format("上传基本数据到MES成功,条码：{0},工位：{1}", ftM.SERIAL_NUMBER, ftM.STEP_NUMBER));
                if (!mesDA.MesBaseExist(ftM.RECID))
                {
                    logRecorder.AddDebugLog(this.nodeName, string.Format("MES数据未存在,条码：{0},工位：{1}", ftM.SERIAL_NUMBER, ftM.STEP_NUMBER));
                }
                return true;
            }
            catch (Exception ex)
            {
                logRecorder.AddDebugLog(this.nodeName, string.Format("上传基本数据到MES,数据库访问异常，条码：{0},工位：{1}，{2}", ftM.SERIAL_NUMBER, ftM.STEP_NUMBER, ex.Message));
                return false;
            }
        }
        protected bool UploadMesdetailData(LOCAL_MES_STEP_INFO_DETAILModel m)
        {
           //临时
            //if (mesDA.MesDetailExist(m.RECID))
            //{
            //    m.RECID = System.Guid.NewGuid().ToString();
            //}
            FT_MES_STEP_INFO_DETAILModel ftM = new FT_MES_STEP_INFO_DETAILModel();
            ftM.DATA_NAME = m.DATA_NAME;
            ftM.DATA_VALUE = m.DATA_VALUE;
            ftM.LAST_MODIFY_TIME = m.LAST_MODIFY_TIME;
            ftM.RECID = m.RECID;
            ftM.SERIAL_NUMBER = m.SERIAL_NUMBER;
            ftM.STATUS = m.STATUS;
            ftM.STEP_NUMBER = m.STEP_NUMBER;
            ftM.TRX_TIME = m.TRX_TIME;
            try
            {
                int reTryMax = 10;
                int reTryCounter = 0;
                while (!mesDA.AddMesDetailinfo(ftM))
                {
                    Thread.Sleep(1000);
                    reTryCounter++;
                    if (reTryCounter > reTryMax)
                    {
                        logRecorder.AddDebugLog(this.nodeName, string.Format("上传详细数据到MES失败,条码：{0},工位：{1}", ftM.SERIAL_NUMBER, ftM.STEP_NUMBER));
                        return false;
                    }
                }
                logRecorder.AddDebugLog(this.nodeName, string.Format("上传详细数据到MES成功,条码：{0},工位：{1}", ftM.SERIAL_NUMBER, ftM.STEP_NUMBER));
                return true;
            }
            catch (Exception ex)
            {
                logRecorder.AddDebugLog(this.nodeName, string.Format("上传详细数据失败，MES数据库访问异常，条码：{0},工位：{1}，{2}", ftM.SERIAL_NUMBER, ftM.STEP_NUMBER, ex.Message));
                return false;
            } 
        }
        
        /// <summary>
        /// 漏项检查
        /// </summary>
        /// <param name="reStr"></param>
        /// <returns></returns>
        protected  bool LossCheck(string barCode,ref string reStr)
        {
            if(preStat != null && preStat.Count()>0)
            {
                foreach (string stName in preStat)
                {
                    string strCond1 = string.Format("SERIAL_NUMBER='{0}' and AutoStationName='{1}'", barCode, stName);
                    //   strWheres.Add(strCond1);
                    if (!localMesBasebll.ExistByCondition(strCond1))
                    {
                        reStr = string.Format("检测数据漏项：{0}不存在", stName);
                        return false;
                    }
                }
            }
           
            return true;
        }
        protected bool PreDetectCheck(string barCode)
        {
           ////临时
           // return true;
            string strCond = string.Format("SERIAL_NUMBER='{0}' and CHECK_RESULT=1", barCode);
            
            if (localMesBasebll.ExistByCondition(strCond))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        protected int ReDetectQuery(string barcode)
        {
            int re = 0;
            string strWhere = string.Format("SERIAL_NUMBER='{0}' and AutoStationName='{1}'", barcode, this.nodeName);
            LOCAL_MES_STEP_INFOModel preCheckModel = localMesBasebll.GetLatestModel(strWhere);
            if (preCheckModel != null)
            {
                if (0 == preCheckModel.CHECK_RESULT)
                {
                    re = 0;
                }
                else
                {
                    re = 1;
                }
                
            }
            else
            {
                re = 2;//不存在记录
            }
            return re;
        }
        protected bool TryUnbind(string rfidCode, string barCode)
        {
            List<OnlineProductsModel> products = onlineProductBll.GetModelList(string.Format("rfidCode='{0}'", rfidCode));
            //   logRecorder.AddDebugLog(nodeName, "尝试解绑:" + rfidCode + "," + barCode);

            if (products != null && products.Count > 0)
            {
                foreach (OnlineProductsModel m in products)
                {
                    //  logRecorder.AddDebugLog(nodeName, "解绑rfid" + m.rfidCode);
                    if (!onlineProductBll.Delete(m.productBarcode))
                    {
                        return false;
                    }
                    else
                    {
                        logRecorder.AddDebugLog(nodeName, "解绑:" + rfidCode);
                    }
                }

            }
            if (onlineProductBll.Exists(barCode))
            {
                if (!onlineProductBll.Delete(barCode))
                {
                    return false;
                }
                else
                {
                    logRecorder.AddDebugLog(nodeName, "解绑:" + barCode);
                }
            }

            //logRecorder.AddDebugLog(nodeName, "解绑:" + rfidCode + "," + barCode);
            return true;
        }
        protected bool TryUnbind(string rfidCode,ref string reStr)
        {
            try
            {
               
                List<DBAccess.Model.BatteryModuleModel> modBinds = modBll.GetModelList(string.Format("palletID='{0}' and palletBinded=1 ",rfidCode));
                if(modBinds != null && modBinds.Count()>0)
                {
                    foreach(DBAccess.Model.BatteryModuleModel mod in modBinds)
                    {
                        mod.palletBinded = false;
                        mod.palletID = "";
                        modBll.Update(mod);
                        logRecorder.AddDebugLog(nodeName, string.Format("解绑,工装板RFID:{0},模块:{1}", rfidCode, mod.batModuleID));
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return false;
            }
        }
        protected bool ClearLoacalMesData(string barcode)
        {
            string strWhere = string.Format("SERIAL_NUMBER='{0}'", barcode);
            bool re1 = localMesBasebll.DelByCondition(strWhere);
            bool re2 = localMesDetailbll.DelByCondition(strWhere);
            return (re1 && re2);
        }
        protected virtual string GetDetectCodeStr()
        {
            string detectCodes = "";
            string detectDesc = string.Empty;
            DataSet ds = detectCodeDefbll.GetList(10000, string.Format("nodeName='{0}' ", processName), "detectIndex");
            
            List<DetectCodeDefModel> detectModels = detectCodeDefbll.DataTableToList(ds.Tables[0]);
            if(detectModels != null && detectModels.Count()>0)
            {
                foreach(DetectCodeDefModel detectItem in detectModels)
                {
                    if(detectItem == null)
                    {
                        continue;
                    }
                    int blockIndex = (detectItem.detectIndex/16) + 3;
                    int bitIndex = (detectItem.detectIndex % 16);
                    if((db2Vals[blockIndex]&(1<<bitIndex))>0)
                    {
                        detectCodes += (detectItem.detectCode + ",");
                        detectDesc += (detectItem.detectItemName + ",");
                    }
                }
            }
            
            //for (int i = 0; i < 16; i++)
            //{
            //    int codeIndex = i + 1;
            //    DetectCodeDefModel m = detectCodeDefbll.GetModel(this.processName, codeIndex);

            //    if (m != null)
            //    {
            //        if ((db2Vals[2] & (1 << i)) > 0)
            //        {
            //            detectCodes += (m.detectCode + ",");
            //            detectDesc += (m.detectItemName + ",");
            //        }
            //    }

            //}
            if (!string.IsNullOrEmpty(detectCodes))
            {
                if (detectCodes[detectCodes.Count() - 1] == ',')
                {
                    detectCodes = detectCodes.Remove(detectCodes.Count() - 1, 1);
                }
                //不合格
                logRecorder.AddDebugLog(this.nodeName, this.currentStat.ProductBarcode + ",故障码：" + detectCodes);
                logRecorder.AddDebugLog(this.nodeName, this.currentStat.ProductBarcode + ",不良项：" + detectDesc);
            }
            return detectCodes;
        }
        //protected bool AddProcessRecord(string batModID, string processName, string extendedInfo)
        //{
        //    DBAccess.Model.ModPsRecordModel modRecord = new DBAccess.Model.ModPsRecordModel();
        //    modRecord.RecordID = System.Guid.NewGuid().ToString();
        //    DBAccess.BLL.BatteryModuleBll batModuleBll = new DBAccess.BLL.BatteryModuleBll();
        //    DBAccess.Model.BatteryModuleModel mod = batModuleBll.GetModel(batModID);
        //    string batch = "";
        //    if (mod != null)
        //    {
        //        batch = mod.batchName;
        //    }

        //    modRecord.processRecord = processName;// +"批次:" + batch;
        //    if (!string.IsNullOrWhiteSpace(extendedInfo))
        //    {
        //        modRecord.processRecord += ("," + extendedInfo);
        //    }
        //    modRecord.batModuleID = batModID;
        //    modRecord.recordTime = System.DateTime.Now;
        //    return modPsRecordBll.Add(modRecord);

        //}
        protected bool AddProcessRecord(string productID, string productCata,string  recordCata,string recordInfo, string ccdData)
        {
            DBAccess.Model.BatteryModuleModel mod = modBll.GetModel(productID);
            if(mod != null)
            {
                mod.curProcessStage = this.nodeName;
                modBll.Update(mod);
            }
            MesDBAccess.Model.ProduceRecordModel record = new MesDBAccess.Model.ProduceRecordModel();
            record.recordID=System.Guid.NewGuid().ToString();
            record.recordTime = System.DateTime.Now;
            record.stationID = this.nodeName;
            record.recordCata = recordCata;
            record.productID = productID;
            record.productCata = productCata;
            record.tag1 = recordInfo;
            record.tag2 = ccdData;
            produceRecordBll.Add(record);
            return true;
        }
        protected virtual bool RfidReadC()
        {
            if (string.IsNullOrWhiteSpace(this.rfidUID))
            {
                //读RFID失败 

                if (this.db1ValsToSnd[0] != 3)
                {
                    //logRecorder.AddDebugLog(nodeName, "读RFID失败");
                    db1ValsToSnd[0] = 3;
                }
                else
                {
                    db1ValsToSnd[0] = 1;
                }
                if(SysCfgModel.SimMode)
                {
                    this.rfidUID = SimRfidUID;
                }
                else
                {
                    this.rfidUID = rfidRW.ReadUID();
                }
                Thread.Sleep(1000);
                this.currentStat.Status = EnumNodeStatus.无法识别;
                this.currentStat.StatDescribe = "读RFID失败";
                this.currentTaskDescribe = "读RFID失败";
                return false;
            }
            else
            {
                //if (!SysCfgModel.SimMode)
                //{
                //    if (this.nodeName != "C线打带")//这个rfid不一样不需要清缓存
                //    {

                //        (rfidRW as DevAccess.RfidCF).ClearBufUID();
                //    }
                //}
            }
            this.currentStat.StatDescribe = "RFID识别完成";
            List<DBAccess.Model.BatteryModuleModel> modList = modBll.GetModelList(string.Format("palletID='{0}' and palletBinded=1", this.rfidUID));
            if (modList == null || modList.Count() < 1)
            {
                db1ValsToSnd[0] = 4;//
                this.currentTaskDescribe = string.Format("工装板{0}绑定数据为空",rfidUID) ;
                this.currentStat.StatDescribe = "绑定数据为空";
                return false;
            }

            this.currentStat.Status = EnumNodeStatus.设备使用中;
            db1ValsToSnd[0] = 2;//读到RFID
          
            this.currentTaskDescribe = "RFID识别完成:" + this.rfidUID;
            return true;
        }
        protected virtual bool RfidReadAB()
        {
           
            currentTaskDescribe = "开始读RFID";

            int channelIndex = 1;
            if (this.db2Vals[0] == 1 || this.db2Vals[0] == 2)
            {
                channelIndex = this.db2Vals[0];
            }
            else
            {
                return false;
            }
            if (channelIndex == 1)
            {
                this.rfidUID = this.rfidUIDA;
            }
            else
            {
                this.rfidUID = this.rfidUIDB;
            }
            if (string.IsNullOrWhiteSpace(this.rfidUID))
            {
                this.currentStat.Status = EnumNodeStatus.无法识别;
                this.currentStat.StatDescribe = "读RFID失败";
                this.currentTaskDescribe = "读RFID失败";
                return false;
            }
            this.currentStat.Status = EnumNodeStatus.设备使用中;
            //db1ValsToSnd[0] = 2;//读到RFID
            this.currentStat.StatDescribe = "RFID识别完成";
            this.currentTaskDescribe = "RFID识别完成:" + this.rfidUID;
            //logRecorder.AddDebugLog(nodeName, string.Format("读到RFID:{0}", this.rfidUID));
            return true;
        }
        protected bool ProductTraceRecord()
        {
            List<DBAccess.Model.BatteryModuleModel> modList = modBll.GetModelList(string.Format("palletID='{0}' and palletBinded=1", this.rfidUID));
            if (modList != null && modList.Count() > 0)
            {
                foreach (DBAccess.Model.BatteryModuleModel mod in modList)
                {
                    mod.curProcessStage = this.nodeName;

                    modBll.Update(mod);
                    logRecorder.AddDebugLog(nodeName, string.Format("模块{0}开始加工", mod.batModuleID));
                   // AddProcessRecord(mod.batModuleID, nodeName, "");
                    AddProcessRecord(mod.batModuleID, "模块", "追溯记录 ", string.Format("{0}开始加工", nodeName), "");
                }
            }
            else
            {
                logRecorder.AddDebugLog(nodeName, "工装板：" + this.rfidUID + "无绑定模块数据");
            }
            return true;
        }
        #endregion
    }

}
