using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PLProcessModel;
using LogInterface;
using FTDataAccess.Model;
using FTDataAccess.BLL;
using DevAccess;
using DevInterface;
namespace LineNodes
{
    /// <summary>
    /// 产品上线控制节点
    /// </summary>
    public class NodeProductInput:CtlNodeBaseModel
    {

        //private IMesAccess mesDA = null;
        //protected string processName = "投产";
        //private bool devStatusRestore = false;//是否已经恢复下电前状态
        //private int secondCosume = 0; //读卡/条码消耗的时间
        
        private DateTime rfidFailSt;
        private bool rfidTimeCounterBegin = false; //RFID超时计时开始
       // private OnlineProductsBll onlineProductBll = null;
      //  private ControlTaskModel currentTask = null;
       // private ControlTaskBll ctlTaskBll = new ControlTaskBll();
        public NodeProductInput()
        {
             //this.currentStat.ProductBarcode="BARCODE1234567";
             
           // this.mesNodeID = "MES投产位";
            mesDA = new MesDA();
            //if(SysCfgModel.SimMode)
            //{
            //    mesDA = new MesDASim();
            //}
            //else
            //{
            //    mesDA = new MesDA();
            //}
           
        }

        /// <summary>
        /// 系统启动后，先回复设备运行状态
        /// </summary>
        /// <param name="errStr"></param>
        /// <returns></returns>
        public override bool DevStatusRestore()
        {
            bool readDB1OK = false;
            for (int i = 0; i < 5; i++)
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
                return false;
            }
            devStatusRestore = true;
            string strWhere = string.Format("(TaskStatus='执行中' or TaskStatus='超时') and DeviceID='{0}' order by CreateTime ", this.nodeID);
            this.currentTask = ctlTaskBll.GetFirstRequiredTask(strWhere);
            if (this.currentTask != null)
            {
                this.currentTaskPhase = this.currentTask.TaskPhase;
                if(!string.IsNullOrWhiteSpace(this.currentTask.TaskParam))
                {
                    string[] paramArray = this.currentTask.TaskParam.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if(paramArray != null && paramArray.Count()>0)
                    {
                        this.rfidUID = paramArray[0];
                    }
                }
                this.currentStat.StatDescribe = "设备使用中";
            }

            return true;

        }

        public override bool BuildCfg(System.Xml.Linq.XElement xe, ref string reStr)
        {
            if(!base.BuildCfg(xe, ref reStr))
            {
                return false;
            }
            this.dicCommuDataDB1[1].DataDescription = "1：检查OK，放行，2：读卡失败(5秒内未读到)，3：读条码失败(10秒内未读到)，4：NG";
            this.dicCommuDataDB1[2].DataDescription = "0：允许流程开始，1:流程锁定";
            for (int i = 0; i < 30;i++ )
            {
                this.dicCommuDataDB1[3+i].DataDescription = string.Format("条码[{0}]",i+1);
            }
           

            this.dicCommuDataDB2[1].DataDescription = "0:无产品,1：有产品";
            this.dicCommuDataDB2[2].DataDescription = "0:无板,1：有板"; 
            return true;
        }
       // int tempCounter = 0;
        public override bool ExeBusiness(ref string reStr)
        {
            try
            {
               if(!base.ExeBusiness(ref reStr))
               {
                   return false;
               }
                if (!NodeStatParse(ref reStr))
                {
                    return false;
                }
                if (!checkEnable)
                {
                    return true;
                }
                if(this.currentTask == null)
                {
                    return true;
                }
                switch (this.currentTaskPhase)
                {
                    case 1:
                        {
                            db1ValsToSnd[1] = 1;//流程锁定
                            if (this.currentStat.Status == EnumNodeStatus.设备故障)
                            {
                                break;
                            }
                           
                            this.currentStat.Status = EnumNodeStatus.设备使用中;
                            this.currentStat.ProductBarcode = "";
                            this.currentStat.StatDescribe = "设备使用中";

                            //读条码，rfid，绑定
                       
                            currentTaskDescribe = "开始读RFID";
                            DateTime dtSt = System.DateTime.Now;
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
                                DateTime dtEnd = DateTime.Now;
                                string recvStr = (rfidRW as SgrfidRW).GetRecvBufStr();
                                string logStr = string.Format("读RFID失败，发送读卡命令:{0},接收判断时间:{1},接收数据:{2}", dtSt.ToString("HH:mm:ss"), dtEnd.ToString("HH:mm:ss"), recvStr);
                                logRecorder.AddDebugLog(nodeName, logStr);

                                if (!rfidTimeCounterBegin)
                                {
                                    //logRecorder.AddDebugLog(nodeName, "读RFID卡失败");
                                    rfidFailSt = System.DateTime.Now;
                                }
                                rfidTimeCounterBegin = true;
                                TimeSpan ts = System.DateTime.Now - rfidFailSt;
                                if (ts.TotalSeconds > 5)
                                {
                                    if (db1ValsToSnd[0] != db1StatRfidFailed)
                                    {
                                        logRecorder.AddDebugLog(nodeName, "读RFID卡失败");
                                    }
                                    db1ValsToSnd[0] = db1StatRfidFailed;

                                }
                                this.currentStat.StatDescribe = "读RFID卡失败！";
                                break;
                            }
                            else
                            {
                                rfidTimeCounterBegin = false;
                            }
                            this.currentTaskPhase++;
                            this.currentTask.TaskPhase = this.currentTaskPhase;
                            this.currentTask.TaskParam = rfidUID;
                            this.barcodeRW.ClearBarcodesBuf(); //清空条码枪缓存
                            this.ctlTaskBll.Update(this.currentTask);
                            break;
                        }
                    case 2:
                        {
                            currentTaskDescribe = "开始读条码";
                            string barcode = barcodeRW.ReadBarcode().Trim();
                            if (string.IsNullOrWhiteSpace(barcode) || barcode.Length != 26)
                            {

                                if (!rfidTimeCounterBegin)
                                {
                                    //logRecorder.AddDebugLog(nodeName, "读RFID卡失败");
                                    rfidFailSt = System.DateTime.Now;
                                }
                                rfidTimeCounterBegin = true;
                                TimeSpan ts = System.DateTime.Now - rfidFailSt;
                                if (ts.TotalSeconds > 5)
                                {
                                    if (db1ValsToSnd[0] != 3)
                                    {
                                        logRecorder.AddDebugLog(nodeName, "读条码失败:" + barcode);
                                    }
                                    db1ValsToSnd[0] = 3;
                                    if(!string.IsNullOrWhiteSpace(barcode))
                                    {
                                        this.currentStat.StatDescribe = "无效的条码，位数不足26位！";
                                        this.currentStat.ProductBarcode = this.currentStat.StatDescribe;
                                        currentTaskDescribe = this.currentStat.StatDescribe;
                                    }
                                    else
                                    {
                                        this.currentStat.StatDescribe = "读条码失败！";
                                        this.currentStat.ProductBarcode = "读条码失败！";
                                        currentTaskDescribe = "读条码失败！";
                                    } 
                                }
                                break;
                            }
                           
                            //db1赋条码
                            BarcodeFillDB1(barcode, 2);
                            currentStat.ProductBarcode = barcode;
                            ProductSizeCfgBll productCfg = new ProductSizeCfgBll();
                            string cataCode = barcode.Substring(0, 13);
                            ProductSizeCfgModel cfgModel = productCfg.GetModel(cataCode);
                            if (cfgModel == null)
                            {
                                if (this.db1ValsToSnd[0] != 4)
                                {
                                   // ThrowErrorStat(string.Format("{0}产品配置信息不存在", this.currentStat.ProductBarcode), EnumNodeStatus.设备故障);
                                    logRecorder.AddDebugLog(nodeName, string.Format("{0}产品配置信息不存在", this.currentStat.ProductBarcode));
                                }
                                this.currentStat.StatDescribe = "配置不存在";
                              //  checkEnable = false;
                                this.db1ValsToSnd[0] = 4;
                                break;
                                //return true;
                            }

                            // 若已经存在，则解绑
                            if (!TryUnbind(rfidUID, barcode))
                            {
                                string strLog = string.Format("解绑错误,RFID:{0},主机条码：{1}", rfidUID, barcode);
                                logRecorder.AddDebugLog(this.nodeName, strLog);
                                currentTaskDescribe = strLog;
                                break;
                            }
                            ClearLoacalMesData(barcode);

                            //数据库绑定
                            if (onlineProductBll.Exists(barcode))
                            {
                                logRecorder.AddDebugLog(nodeName, "已经存在：" + barcode + ",删除");
                                onlineProductBll.Delete(barcode);
                            }

                            OnlineProductsModel productBind = new OnlineProductsModel();
                            productBind.rfidCode = rfidUID;
                            productBind.productBarcode = barcode;
                            productBind.currentNode = this.nodeName;
                            productBind.inputTime = System.DateTime.Now;
                            onlineProductBll.Add(productBind);
                            string logStr = string.Format("产品绑定完成，RFID UID:{0},整机条码：{1}", rfidUID, barcode);
                            logRecorder.AddDebugLog(nodeName, logStr);

                            //先存本地，再MES投产
                            if (SysCfgModel.MesAutodownEnabled)
                            {
                                if (!MesDatalocalSave(barcode, 0, "", "", 0))
                                {
                                    return false;
                                }

                            }
                            AddInputRecord(barcode);
                            currentTaskDescribe = "产品绑定完成,等待MES投产";
                            

                            this.currentTaskPhase++;
                            this.currentTask.TaskParam = this.rfidUID + "," + barcode;
                            this.currentTask.TaskPhase = this.currentTaskPhase;
                            this.ctlTaskBll.Update(this.currentTask);
                            break;
                        }
                    case 3:
                        {
                            //MES投产
                            if (SysCfgModel.MesAutodownEnabled)
                            {
                                if (!UploadMesdata(true, this.currentStat.ProductBarcode, new string[] { this.mesNodeID }, ref reStr))
                                {
                                    this.currentStat.StatDescribe = "MES投产失败";
                                    logRecorder.AddDebugLog(this.nodeName, this.currentStat.StatDescribe);
                                    break;
                                }
                                currentTaskDescribe = "MES投产完成";
                            }
                           
                            this.currentTaskPhase++;
                            this.currentTask.TaskPhase = this.currentTaskPhase;
                            this.ctlTaskBll.Update(this.currentTask);
                            db1ValsToSnd[0] = db1StatCheckOK;
                            break;
                        }
                    case 4:
                        {
                            //流程完成
                            this.currentStat.StatDescribe = "流程完成";
                            currentTaskDescribe = "流程完成";
                            this.currentTask.TaskStatus = EnumTaskStatus.已完成.ToString();
                            this.currentTask.TaskPhase = this.currentTaskPhase;
                            this.ctlTaskBll.Update(this.currentTask);
                            this.currentTask = null;
                            break;
                           
                        }
                    default:
                        break;
                }
           //     this.currentStat.StatDescribe = "流程步号：" + currentTaskPhase.ToString();
                return true;
            }
            catch (Exception ex)
            {
                ThrowErrorStat(ex.ToString(), EnumNodeStatus.设备故障);
                return false;
            }
 
        }
        /// <summary>
        /// 分析工位状态
        /// </summary>
        /// <param name="reStr"></param>
        /// <returns></returns>
        protected override bool NodeStatParse(ref string reStr)
        {
            if (db2Vals[0] == 0)
            {
                if (productChecked)
                {
                    logRecorder.AddDebugLog(nodeName, "产品离开工位");
                }
                productChecked = false;
                //查询未执行完任务，清掉
                if (!ctlTaskBll.ClearTask(string.Format("DeviceID='{0}'", this.nodeID)))
                {
                    logRecorder.AddDebugLog(nodeName, "清理任务失败");
                    return false;
                }
                this.currentTask = null;
                //if (currentTaskPhase != 0)
                //{
                //    //查询未执行完任务，清掉
                //    if (!ctlTaskBll.ClearTask(string.Format("DeviceID='{0}'", this.nodeID)))
                //    {
                //        logRecorder.AddDebugLog(nodeName, "清理任务失败");
                //        return false;
                //    }
                //    this.currentTask = null;
                //}
                //if(this.currentTask == null)
               // {
                    DevCmdReset();
                  
                    this.currentStat.Status = EnumNodeStatus.设备空闲;
                    this.currentStat.ProductBarcode = "";
                    this.currentStat.StatDescribe = "设备空闲";
                    checkFinished = false;
                    currentTaskDescribe = "等待有板信号";
                    currentTaskPhase = 0; //复位
                    checkEnable = true;
                //}
               
            }
            else if (db2Vals[0] == 1)
            {
                if (!productChecked)
                {
                    logRecorder.AddDebugLog(nodeName, "检测到有产品");
                }
                productChecked = true;

                if (currentTaskPhase == 0)
                {
                    //生成新任务
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
                   
                }
            }
            else
            {
                ThrowErrorStat("PLC错误的状态数据", EnumNodeStatus.设备故障);
                return true;
            }
            return true;
        }
        private void AddInputRecord(string productBarcode)
        {
            string strWhere = string.Format("productBarcode='{0}' and lineOuted = 0 order by inputTime desc ",productBarcode);
            List<ProduceRecordModel> recordList = produceRecordBll.GetModelList(strWhere);
            if (recordList == null || recordList.Count() < 1)
            {
                ProduceRecordModel record = new ProduceRecordModel();
                record.inputTime = System.DateTime.Now;
                record.lineOuted = false;
                record.productBarcode = productBarcode;
                produceRecordBll.Add(record);
            }
            else
            {
                ProduceRecordModel record = recordList[0];
                record.inputTime = System.DateTime.Now;
                record.outputNode = "";
                produceRecordBll.Update(record);
            }
        }
     
    }
}
