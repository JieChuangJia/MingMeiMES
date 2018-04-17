using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using PLProcessModel;
using LogInterface;
using DevInterface;
using DevAccess;
using FTDataAccess.Model;
using FTDataAccess.BLL;
namespace LineNodes
{
    public class MesStatRecord
    {
        public string ProductBarcode{get;set;}
        public DateTime StatModifyTime{get;set;}
        public int StatVal{get;set;}
        public MesStatRecord()
        {
            StatVal = 0;
            ProductBarcode="";
            StatModifyTime=System.DateTime.Now;
        }
    }
    /// <summary>
    /// 装箱校验控制节点
    /// </summary>
    public class NodePack:CtlNodeBaseModel
    {
        public static Dictionary<string, MesStatRecord> mesQueryStat = new Dictionary<string, MesStatRecord>(); //MES预下线状态,1:查询中，2：可以下线，3：禁止下线，按NG处理
        private bool graspBegin = false; //是否已经开始抓取
       
        //private short barcodeCompareFailed = 5; //条码校验未通过
    
        private short mesDownDisable = 6; //mes禁止下线
        private short heightNotCfg = 7; //高度未配置
        private short mesDownedFlag = 8; //mes已经下线
        private DateTime detectStartTime;//启动检测后开始计时
        private ProductSizeCfgBll productCfgBll = new ProductSizeCfgBll();
        private ProductHeightDefBll productHeightBll = new ProductHeightDefBll();
        public static object mesQueryLock = new object();
        private IPrinterInfoDev prienterRW = null;
        //private ThreadBaseModel printinfoSndThread = null;
        //    private Queue<string> printBuf = new Queue<string>(); //贴标机发送队列
        private List<string> printList = new List<string>();//
        private object lockPrintbuf = new object();
        public List<string> PrintList { get { return printList; } }
        public IPrinterInfoDev PrienterRW { get { return prienterRW; } set { prienterRW = value; } }
        public override bool BuildCfg(System.Xml.Linq.XElement xe, ref string reStr)
        {
            
            if (!base.BuildCfg(xe, ref reStr))
            {
                return false;
            }
            this.dicCommuDataDB1[1].DataDescription = "1：检查OK，放行，2：读卡,3：有NG或漏检，5:未投产,6：MES禁止下线,7：高度未配置，8:MES已经下线";
            this.dicCommuDataDB1[2].DataDescription = "0：允许流程开始，1:流程锁定";
            this.dicCommuDataDB1[3].DataDescription = "高度编号（从1开始）";
            for (int i = 0; i < 30; i++)
            {
                this.dicCommuDataDB1[4 + i].DataDescription = string.Format("条码[{0}]", i + 1);
            }
            this.dicCommuDataDB2[1].DataDescription = "0:无产品,1：有产品";
            this.dicCommuDataDB2[2].DataDescription = "0:无板,1：有板";
            return true;
        }
        protected override bool NodeStatParse(ref string reStr)
        {
            //if(!base.NodeStatParse(ref reStr))
            //{
            //    return false;
            //}
            if(db2Vals[0] == 1)
            {
                if (!productChecked)
                {
                    logRecorder.AddDebugLog(nodeName, "检测到有产品");
                }
                productChecked = true;

                if(!graspBegin)
                {
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
                        DevCmdReset();
                      
                        this.currentStat.Status = EnumNodeStatus.设备空闲;
                        this.currentStat.ProductBarcode = "";
                        this.currentStat.StatDescribe = "设备空闲";
                        checkFinished = false;
                        currentTaskDescribe = "等待有板信号";
                    }
                    
                }
            }
            else if(db2Vals[0] == 0)
            {
                if (productChecked)
                {
                    logRecorder.AddDebugLog(nodeName, "产品离开工位");
                }
                productChecked = false;

                if (!graspBegin) //非抓取状态
                {
                    if (!ctlTaskBll.ClearTask(string.Format("DeviceID='{0}'", this.nodeID)))
                    {
                        logRecorder.AddDebugLog(nodeName, "清理任务失败");
                        return false;
                    }
                    this.currentTask = null;

                    this.currentTaskPhase = 0;
                    checkEnable = true;
                    DevCmdReset();
                    rfidUID = string.Empty;
                    this.currentStat.Status = EnumNodeStatus.设备空闲;
                    this.currentStat.ProductBarcode = "";
                    this.currentStat.StatDescribe = "设备空闲";
                    checkFinished = false;
                    currentTaskDescribe = "等待有板信号";

                }
               
            }
            return true;
        }
        public override bool ExeBusiness(ref string reStr)
        {
            MessLossCheck();
            base.ExeBusiness(ref reStr);
            if (!NodeStatParse(ref reStr))
            {
                return false;
            }
            //清理MES查询记录字典
            //List<string> removeList = new List<string>();
            //foreach (string key in NodePack.mesQueryStat.Keys)
            //{
            //    MesStatRecord mesStat = NodePack.mesQueryStat[key];
            //    TimeSpan tmSpan = System.DateTime.Now - mesStat.StatModifyTime;
            //    if (tmSpan.TotalSeconds > 60)
            //    {

            //        removeList.Add(key);
            //    }
            //}
            //foreach (string key in removeList)
            //{
            //    NodePack.mesQueryStat.Remove(key);
            //}
            if (!checkEnable)
            {
                return true;
            }
           
            switch (currentTaskPhase)
            {
                case 1:
                    {
                        if (!ProcessStartCheck(3,!SysCfgModel.DebugMode))
                        {
                            break;
                        }
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.currentTask.TaskParam = rfidUID;

                        this.ctlTaskBll.Update(this.currentTask);
                        //检查是否已经下线,MES离线模式下不判断
                        if (!SysCfgModel.MesOfflineMode)
                        {
                            int mesDown = mesDA.MesDowned(this.currentStat.ProductBarcode, mesNodeID, ref reStr);
                            if (mesDown == 1)
                            {
                                if( db1ValsToSnd[0] != mesDownedFlag)
                                {
                                    string logInfo = string.Format("条码重复，{0} 已经下线，请检查", this.currentStat.ProductBarcode);
                                    logRecorder.AddDebugLog(nodeName, logInfo);
                                }
                                db1ValsToSnd[0] = mesDownedFlag;
                                checkEnable = false;
                                break;
                            }
                            else if (mesDown == 3)
                            {
                                string logInfo = string.Format("MES数据库访问失败,无法查询是否已经下线，{0},{1}", this.currentStat.ProductBarcode, reStr);
                                logRecorder.AddDebugLog(nodeName, logInfo);
                            }
                        }

                        this.detectStartTime = System.DateTime.Now;
                        logRecorder.AddDebugLog(nodeName, "MES下线查询开始:" + this.currentStat.ProductBarcode);
                        currentTaskDescribe = "MES下线查询开始:" + this.currentStat.ProductBarcode;
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 2:
                    {
                        currentTaskDescribe = "开始查询MES下线是否允许";
                        int mesRe = 0;

                        if (!SysCfgModel.MesOfflineMode && PLProcessModel.SysCfgModel. MesCheckEnable)
                        {
                            mesRe = mesDA.MesAssemDown(new string[] { this.currentStat.ProductBarcode, PLProcessModel.SysCfgModel.mesLineID }, ref reStr);
                            //string mesDownQueryMesID = "";// mesNodeID;
                            //if (SysCfgModel.mesLineID == "L10")
                            //{
                            //    mesDownQueryMesID = "DQ-G-0104";
                            //}
                            //else
                            //{
                            //    // throw new NotImplementedException();
                            //    mesDownQueryMesID = "DQ-H-0104";
                            //}
                            //mesRe = mesDA.MesDownEnabled(PLProcessModel.SysCfgModel.mesLineID, this.currentStat.ProductBarcode, mesDownQueryMesID, ref reStr);

                        }
                       
                        
                        if(0==mesRe)
                        {
                            
                            logRecorder.AddDebugLog(this.nodeName, string.Format("{0} 下线允许", this.currentStat.ProductBarcode));
                            currentTaskDescribe = "MES下线允许";
                            SetMesQueryStat(this.currentStat.ProductBarcode, 2);
                            //启用自动贴标功能
                            if (PLProcessModel.SysCfgModel.PrienterEnable)
                            {
                                //发送条码
                                if (!SendPrinterinfo(this.currentStat.ProductBarcode, true))
                                {
                                    break;
                                }
                            }
                            currentTaskPhase++;
                            this.currentTask.TaskPhase = this.currentTaskPhase;
                            this.ctlTaskBll.Update(this.currentTask);
                        }
                        else
                        {
                            if(reStr.Contains("已下线"))
                            {
                                if (db1ValsToSnd[0] != mesDownedFlag)
                                {
                                    string logInfo = string.Format("{0} 已经下线，请检查", this.currentStat.ProductBarcode);
                                    logRecorder.AddDebugLog(nodeName, logInfo);
                                }
                                db1ValsToSnd[0] = mesDownedFlag;
                                
                            }
                            else if (2 == mesRe || 3 == mesRe) //查询无结果或者
                            {
                                int delayTimeOut = SysCfgModel.MesTimeout;//20;//最多允许延迟10秒
                                TimeSpan timeElapse = System.DateTime.Now - detectStartTime;
                                if (timeElapse.TotalMilliseconds > delayTimeOut * 1000)
                                {
                                    SetMesQueryStat(this.currentStat.ProductBarcode, 3);//MES禁止下线
                                    this.currentStat.StatDescribe = string.Format("{0} :MES预下线查询超时({1}秒)，{2}", this.currentStat.ProductBarcode, delayTimeOut, reStr);
                                    currentTaskDescribe = "MES下线允许超时";
                                    if (db1ValsToSnd[0] != mesDownDisable)
                                    {
                                        logRecorder.AddDebugLog(this.nodeName, currentStat.StatDescribe);
                                    }
              
                                    db1ValsToSnd[0] = mesDownDisable;
                                    checkEnable = false;
                                    break;
                                }
                            }
                            else if(1== mesRe)
                            {
                                SetMesQueryStat(this.currentStat.ProductBarcode, 3);//MES禁止下线
                                this.currentStat.StatDescribe = string.Format("{0} :MES禁止下线，因为：{1}", this.currentStat.ProductBarcode, reStr);
                                if (db1ValsToSnd[0] != mesDownDisable)
                                {
                                    logRecorder.AddDebugLog(this.nodeName, currentStat.StatDescribe);
                                }
                                currentTaskDescribe = this.currentStat.StatDescribe;
                                db1ValsToSnd[0] = mesDownDisable;
                                checkEnable = false;
                                break;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        //查询产品高度参数
                        string productTypeCode = "";
                        productTypeCode = this.currentStat.ProductBarcode.Substring(0, 13);
                        ProductSizeCfgModel cfg = productCfgBll.GetModel(productTypeCode);
                        if (cfg == null)
                        {
                            db1ValsToSnd[0] = heightNotCfg;
                            this.currentStat.StatDescribe = "产品未配置";
                            currentTaskDescribe = "产品未配置";
                            checkEnable = false;
                            break;
                        }
                        //产品高度信息
                        //ProductHeightDefModel heightDef = productHeightBll.GetModel(cfg.productHeight);
                        //db1ValsToSnd[2] = (short)heightDef.heightSeq;
                        db1ValsToSnd[2] = (short)cfg.robotProg;
                        graspBegin = true;
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 4:
                    {
                        currentTaskDescribe = "等待有板信号复位";
                        if(db2Vals[0] != 0 || db2Vals[1] != 0)
                        {
                            //等待抓起
                            break;
                        }
                        DevCmdReset(); //
                        //解绑
                        if(!TryUnbind(this.rfidUID,this.currentStat.ProductBarcode))
                        {
                            currentTaskDescribe = string.Format("解绑失败,RFID:{0},主机条码：{1}",this.rfidUID,this.currentStat.ProductBarcode);
                            logRecorder.AddDebugLog(nodeName, currentTaskDescribe);
                            break;
                        }

                        
                        //下线，高度配方发完,MES入库
                        if (!MesDatalocalSave(this.currentStat.ProductBarcode, 0, "", "", 1))
                        {
                            logRecorder.AddLog(new LogModel(this.nodeName, "保存检测数据到本地数据库失败", EnumLoglevel.警告));
                            break;
                        }
                        if (!UploadMesdata(true, this.currentStat.ProductBarcode, mesProcessSeq, ref reStr))
                        {
                            this.currentStat.StatDescribe = "上传MES失败";
                            logRecorder.AddDebugLog(this.nodeName, this.currentStat.StatDescribe);
                            currentTaskDescribe = "上传MES失败";
                            break;
                        }
                        db1ValsToSnd[0] = db1StatCheckOK; //核对正确，允许搬运
                        checkFinished = true;
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 5:
                    {
                        logRecorder.AddDebugLog(nodeName, "入箱完成:" + this.currentStat.ProductBarcode);
                        DevCmdReset(); 
                        this.currentStat.StatDescribe = "流程完成";
                        currentTaskDescribe = "入箱完成";
                        graspBegin = false;//准备新的流程
                        currentTaskPhase = 1;
                        break;
                    }
              
                default:
                    break;
            }
            return true;
        }
        public static void SetMesQueryStat(string productBarcode, int val)
        {
            lock (mesQueryStat)
            {
                if (NodePack.mesQueryStat.Keys.Contains(productBarcode))
                {
                    NodePack.mesQueryStat[productBarcode].StatVal = val;
                    NodePack.mesQueryStat[productBarcode].ProductBarcode = productBarcode;
                    NodePack.mesQueryStat[productBarcode].StatModifyTime = System.DateTime.Now;
                }
                else
                {
                    MesStatRecord stat = new MesStatRecord();
                    stat.StatVal = val;
                    stat.StatModifyTime = System.DateTime.Now;
                    stat.ProductBarcode = productBarcode;
                    NodePack.mesQueryStat[productBarcode] = stat;

                }

            }
        }
        public static MesStatRecord GetMequeryStat(string productBarcode)
        {
            lock (mesQueryStat)
            {
                if (NodePack.mesQueryStat.Keys.Contains(productBarcode))
                {
                    return NodePack.mesQueryStat[productBarcode];
                }
                else
                {
                    return null;
                }

            }
        }
        /// <summary>
        /// 贴标队列处理,周期执行
        /// </summary>
        public void PrinterListProcess()
        {
            string productBarcode = "";
            lock (lockPrintbuf)
            {
                if (!PLProcessModel.SysCfgModel.PrienterEnable)
                {
                    this.printList.Clear();
                    return;
                }
                if (this.printList.Count() == 0)
                {
                    return;
                }
                productBarcode = this.printList[0];
            }
            int mesRe = 0;
            string reStr = "";//!SysCfgModel.SimMode && 
            //DateTime mesSt = DateTime.Now;
            //if (!SysCfgModel.MesOfflineMode && PLProcessModel.SysCfgModel.MesCheckEnable)
            //{
            //    mesRe = mesDA.MesAssemDown(new string[] { productBarcode, LineMonitorPresenter.mesLineID }, ref reStr);
            //}
            //int queryInterval = 100;

            //while (mesRe != 0)
            //{
            //    MesStatRecord mesStat = NodePack.GetMequeryStat(productBarcode);
            //    if (mesStat != null)
            //    {
            //        if (3 == mesStat.StatVal)
            //        {
            //            logRecorder.AddDebugLog(this.nodeName, productBarcode + ":MES禁止下线:" + reStr);
            //            break;
            //        }
            //    }
            //    TimeSpan timeElapse = System.DateTime.Now - mesSt;
            //    if (timeElapse.TotalSeconds > (SysCfgModel.MesTimeout + 5))
            //    {
            //        break;
            //    }
            //    Thread.Sleep(queryInterval);
            //    mesRe = mesDA.MesAssemDown(new string[] { productBarcode, LineMonitorPresenter.mesLineID }, ref reStr);
            //}
            if (0 == mesRe)
            {

                bool re = prienterRW.SndBarcode(productBarcode, ref reStr);
                if (!re)
                {
                    string failInfo = string.Format("给贴标机发送条码{0} 失败,错误信息：{1}", productBarcode, reStr);
                    logRecorder.AddDebugLog(nodeName, failInfo);
                }
                else
                {
                    lock (lockPrintbuf)
                    {
                        this.printList.Remove(productBarcode);
                    }

                    logRecorder.AddDebugLog(nodeName, "成功发送贴标条码：" + productBarcode + "," + reStr);
                }
            }
            else
            {
                logRecorder.AddDebugLog(this.nodeName, productBarcode + ":MES下线查询超时，" + reStr);
                lock (lockPrintbuf)
                {
                    this.printList.Remove(productBarcode);
                }

            }

        }

        private void MessLossCheck()
        {
            if (SysCfgModel.MesOfflineMode)
            {
                return;
            }
         
            string strWhere = string.Format(" AutoStationName='{0}' and UPLOAD_FLAG=0", this.nodeName);

            List<LOCAL_MES_STEP_INFOModel> unUploads = localMesBasebll.GetModelList(strWhere);
            if (unUploads != null && unUploads.Count() > 0)
            {
                foreach (LOCAL_MES_STEP_INFOModel infoModel in unUploads)
                {
                    string reStr = "";
                    if (!UploadMesdata(true, infoModel.SERIAL_NUMBER, mesProcessSeq, ref reStr))
                    {
                        logRecorder.AddDebugLog(this.nodeName, infoModel.SERIAL_NUMBER + ",上传MES失败");

                    }
                }
            }
        }
        private bool SendPrinterinfo(string productBarcode,bool synSnd)
        {
            /*
            DelegateSndPrinter dlgt = new DelegateSndPrinter(AsySndPrinterinfo);
            string reStr = "";
            IAsyncResult ar = dlgt.BeginInvoke(productBarcode, ref reStr, null, dlgt);
             */
            if (synSnd)
            {
                string reStr = "";
                bool re = prienterRW.SndBarcode(productBarcode, ref reStr);
                if (!re)
                {
                    string failInfo = string.Format("给贴标机发送条码{0}失败,错误信息：{1}", productBarcode, reStr);
                    logRecorder.AddDebugLog(nodeName, failInfo);
                    return false;
                }
                else
                {
                    string logStr = string.Format("发送条码给贴标机：{0}", productBarcode);
                    logRecorder.AddDebugLog(nodeName, logStr);
                    return true;
                }
            }
            else
            {
                lock (lockPrintbuf)
                {
                    this.printList.Add(productBarcode);
                }
                logRecorder.AddDebugLog(nodeName, string.Format("{0}添加到待发送队列", productBarcode));
                return true;
            }
           

        }
        /// <summary>
        /// 异步发送条码给贴标机
        /// </summary>
        /// <param name="productBarcode"></param>
        /// <param name="reStr"></param>
        /// <returns></returns>
        //private bool AsySndPrinterinfo(string productBarcode, ref string reStr)
        //{
        //    if (!PLProcessModel.SysCfgModel.PrienterEnable)
        //    {
        //        reStr = "贴标机已经禁用";
        //        return true;
        //    }
        //    int mesRe = 0;
        //    if (!SysCfgModel.MesOfflineMode && PLProcessModel.SysCfgModel.MesCheckEnable)
        //    {
        //        mesRe = mesDA.MesAssemDown(new string[] { productBarcode, LineMonitorPresenter.mesLineID }, ref reStr);
        //    }

        //    int delayTimeOut = 3600;//

        //    int queryInterval = 100;
        //    DateTime mesSt = DateTime.Now;
        //    while (0 != mesRe)
        //    {
        //        //this.currentStat.StatDescribe = productBarcode + ":MES禁止下线:" + reStr;
        //        // logRecorder.AddDebugLog(this.nodeName, this.currentStat.StatDescribe);
        //        MesStatRecord mesStat = NodePack.GetMequeryStat(productBarcode);
        //        if (mesStat != null)
        //        {
        //            if (3 == mesStat.StatVal)
        //            {
        //                logRecorder.AddDebugLog(this.nodeName, productBarcode + ":MES禁止下线:" + reStr);
        //                return false;
        //            }
        //        }
        //        TimeSpan timeElapse = System.DateTime.Now - mesSt;

        //        if (timeElapse.TotalMilliseconds > delayTimeOut * 1000)
        //        {
        //            break;
        //        }
        //        Thread.Sleep(queryInterval);
        //        mesRe = mesDA.MesAssemDown(new string[] { productBarcode, LineMonitorPresenter.mesLineID }, ref reStr);

        //    }
        //    if (0 == mesRe)
        //    {
        //        //PushBarcodeToBuf(productBarcode);
        //        bool re = prienterRW.SndBarcode(productBarcode, ref reStr);
        //        int reTryMax = 20;
        //        int tryCounter = 0;
        //        while (!re)
        //        {
        //            tryCounter++;
        //            string failInfo = string.Format("给贴标机发送条码{0} 失败,错误信息：{1}", productBarcode, reStr);
        //            logRecorder.AddDebugLog(nodeName, failInfo);
        //            if (tryCounter > reTryMax)
        //            {
        //                break;
        //            }
        //            Thread.Sleep(1000);
        //            re = prienterRW.SndBarcode(productBarcode, ref reStr);

        //        }
        //        if (re)
        //        {
        //            logRecorder.AddDebugLog(nodeName, "成功发送贴标条码：" + productBarcode + "," + reStr);
        //            return true;
        //        }
        //        else
        //        {
        //            string failInfo = string.Format("给贴标机发送条码失败:{0},错误信息：{1}", productBarcode, reStr);
        //            logRecorder.AddDebugLog(nodeName, failInfo);
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        string logStr = productBarcode + ":MES禁止下线:" + reStr;
        //        logRecorder.AddDebugLog(this.nodeName, logStr);
        //        return false;
        //    }
        //}
    }
}
