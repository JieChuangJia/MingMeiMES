using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using DevAccess;
using FTDataAccess.Model;
using FTDataAccess.BLL;
using PLProcessModel;
namespace LineNodes
{
    public class NodeCheckAngui : CtlNodeBaseModel
    {
        #region 数据区
        public DevAccess.AinuoAccess AinuoObj {get;set;}
        private DateTime detectStartTime;//启动检测后开始计时
        private int detectTimeout = 15000; //检测结果返回最大允许延迟20000毫秒
       // private int checkRetryCounter = 0;
        private string detectCode = "";
        protected int checkRe = 1;
        private ProductSizeCfgModel cfgModel = null;
        public int ainuoMachineID = 1;
        private IDictionary<string, string> detectCodeMap = new Dictionary<string, string>();
	    #endregion
        public NodeCheckAngui()
        {
            detectCodeMap["1804"] = "接地不良";
            detectCodeMap["1812"] = "启动不良";
            detectCodeMap["1805"] = "耐压不良";
            detectCodeMap["1806"] = "泄漏不良";
            detectCodeMap["1829"] = "检测设备无数据";
            detectCodeMap["1821"] = "功率异常";
            detectCodeMap["1811"] = "弱档不启动";
            detectCodeMap["1810"] = "强档不启动";
           // detectCodeMap["1812"] = "电源键不启动";
     
        }
        public override bool BuildCfg(System.Xml.Linq.XElement xe, ref string reStr)
        {

            if (!base.BuildCfg(xe, ref reStr))
            {
                return false;
            }
            this.dicCommuDataDB1[1].DataDescription = "1：检查OK，放行，2：读卡/条码失败，3：NG，4：不需要检测，5:产品未绑定,6:产品配置信息不存在";
            this.dicCommuDataDB1[2].DataDescription = "0：允许流程开始，1:流程锁定";
            this.dicCommuDataDB1[3].DataDescription = "机器人配方数据";
            for (int i = 0; i < 30; i++)
            {
                this.dicCommuDataDB1[4 + i].DataDescription = string.Format("条码[{0}]", i + 1);
            }
            this.dicCommuDataDB1[34].DataDescription = "1:人工检测模式，０：自动模式";
            this.dicCommuDataDB2[1].DataDescription = "0:无产品,1：有产品";
            this.dicCommuDataDB2[2].DataDescription = "0:无板,1：有板";
            this.dicCommuDataDB2[3].DataDescription = "1:人工检测完成，0：未完成";
            XElement selfDataXE = xe.Element("SelfDatainfo");
            XElement machineXE = selfDataXE.Element("Machine");
            this.detectTimeout = int.Parse(machineXE.Attribute("detectTimeOut").Value);
            if (detectTimeout < 1000)
            {
                this.detectTimeout = 1000;
            }
            if (detectTimeout > 60000)
            {
                this.detectTimeout = 60000;
            }
            this.ainuoMachineID = int.Parse(machineXE.Attribute("id").Value);
            return true;
        }
       
        public  bool ManualExeBusiness(ref string reStr)
        {
             switch (currentTaskPhase)
             {
                 case 1:
                     {
                         detectCode = "";
                         cfgModel = null;

                         if (!ProcessStartCheck(3, !SysCfgModel.DebugMode))
                         {
                             break;
                         }
                         this.currentTask.TaskPhase = this.currentTaskPhase;
                         this.currentTask.TaskParam = rfidUID;
                         this.ctlTaskBll.Update(this.currentTask);
                         ProductSizeCfgBll productCfg = new ProductSizeCfgBll();
                         string cataCode = this.currentStat.ProductBarcode.Substring(0, 13);
                         cfgModel = productCfg.GetModel(cataCode);
                         if (cfgModel == null)
                         {
                             if (this.db1ValsToSnd[0] != 6)
                             {
                                 ThrowErrorStat(string.Format("{0}产品配置信息不存在", this.currentStat.ProductBarcode), EnumNodeStatus.设备故障);
                                 // logRecorder.AddDebugLog(nodeName, string.Format("{0}产品配置信息不存在", this.currentStat.ProductBarcode));
                             }
                             this.currentStat.StatDescribe = "配置不存在";
                             checkEnable = false;
                             this.db1ValsToSnd[0] = 6;
                             return true;
                         }
                         logRecorder.AddDebugLog(this.nodeName, this.currentStat.ProductBarcode + "开始检测");
                         this.db1ValsToSnd[2] = (short)cfgModel.robotProg;
                         this.currentTaskPhase++;
                         this.currentTask.TaskPhase = this.currentTaskPhase;
                         this.ctlTaskBll.Update(this.currentTask);
                         break;
                     }
                 case 2:
                     {
                         //等待人工检测完毕，
                         this.currentStat.StatDescribe = "等待人工检测完成";
                         this.currentTaskDescribe = "等待人工检测完成";
                         if(this.db2Vals[2] != 1)
                         {
                             break;
                         }
                         currentTaskDescribe = "开始查询安规结果";
                         DevAccess.AinuoDetectModel detectModel = null;
                         string strDetectVal = "";
                         if (SysCfgModel.SimMode)
                         {
                             detectModel = GenerateSimDetect();
                             //currentTaskDescribe = "开始保存本地检测结果";
                             //checkRe = 0;
                             //if (!MesDatalocalSave(this.currentStat.ProductBarcode, checkRe, detectCode, "", 0))
                             //{
                             //    Console.WriteLine(string.Format("{0}保存检测数据到本地数据库失败", nodeName));
                             //    break;
                             //}
                         }
                         else
                         {
                             detectModel = AinuoObj.GetDetectResult(ref reStr);
                             //if (detectModel == null)
                             //{
                             //    TimeSpan timeElapse = System.DateTime.Now - detectStartTime;
                             //    if (timeElapse.TotalMilliseconds < detectTimeout)
                             //    {
                             //        break;
                             //    }
                             //    else
                             //    {
                             //        detectStartTime = System.DateTime.Now;
                             //        logRecorder.AddDebugLog(this.nodeName, string.Format("检测超时,{0}毫秒,重新检测", detectTimeout));
                             //        currentStat.StatDescribe = string.Format("检测超时");
                             //    }
                             //}


                         }
                         if (detectModel == null)
                         {
                             logRecorder.AddDebugLog(nodeName, this.currentStat.ProductBarcode + "检测超时，无数据," + reStr);
                             ThreadBaseModel.Sleep(1000);
                             break;
                         }
                         float designPower = float.Parse(cfgModel.power1);
                         if (!GetDetectResult(this.currentStat.ProductBarcode, detectModel, designPower, ref detectCode, ref strDetectVal, ref checkRe))
                         {
                             ThreadBaseModel.Sleep(1000);
                             break;
                         }
                         //if (!SysCfgModel.SimMode)
                         //{
                         //    if (!AinuoObj.CloseDev(ref reStr))
                         //    {
                         //        logRecorder.AddDebugLog(nodeName, this.currentStat.ProductBarcode + "关闭艾诺检测设备失败," + reStr);
                         //        break;
                         //    }
                         //}

                         currentTaskDescribe = "开始保存本地检测结果";
                         if (!string.IsNullOrWhiteSpace(detectCode))
                         {
                             if (detectCode.Substring(detectCode.Length - 1, 1) == ",")
                             {
                                 detectCode = detectCode.Remove(detectCode.Length - 1, 1);
                             }
                             logRecorder.AddDebugLog(nodeName, this.currentStat.ProductBarcode + "故障码:" + detectCode);
                         }
                         if (checkRe > 0)
                         {
                             OutputRecord(this.currentStat.ProductBarcode);
                         }
                         if (!MesDatalocalSave(this.currentStat.ProductBarcode, checkRe, detectCode, strDetectVal, 0))
                         {
                             Console.WriteLine(string.Format("{0}保存检测数据到本地数据库失败", nodeName));
                             break;
                         }
                         this.currentTaskPhase++;
                         this.currentTask.TaskPhase = this.currentTaskPhase;
                         this.ctlTaskBll.Update(this.currentTask);
                         break;
                     }
                 case 3:
                     {
                         string checkReStr = "OK";
                         if(this.nodeID== "2001")
                         {
                             if(checkRe == 1)
                             {
                                 //安规1检测完成NG后上传数据
                                 if (!UploadMesdata(true, this.currentStat.ProductBarcode, mesProcessSeq, ref reStr))
                                 {
                                     this.currentStat.StatDescribe = "上传MES失败";
                                     logRecorder.AddDebugLog(this.nodeName, this.currentStat.StatDescribe);
                                     break;
                                 }
                             }
                         }
                         else
                         {
                             //安规1检测完成若合格后不上传数据
                             if (!UploadMesdata(true, this.currentStat.ProductBarcode, mesProcessSeq, ref reStr))
                             {
                                 this.currentStat.StatDescribe = "上传MES失败";
                                 logRecorder.AddDebugLog(this.nodeName, this.currentStat.StatDescribe);
                                 break;
                             }
                         }

                         currentTaskDescribe = "放行";
                         if (checkRe == 1)
                         {
                             db1ValsToSnd[0] = db1StatNG;
                             checkReStr = "NG";
                         }
                         else
                         {
                             db1ValsToSnd[0] = db1StatCheckOK;
                         }
                         logRecorder.AddDebugLog(this.nodeName, this.currentStat.ProductBarcode + "检测完成," + checkReStr);
                         this.currentStat.StatDescribe = "安规检测完成," + checkReStr;
                         checkFinished = true;
                         currentTaskPhase++;
                         this.currentTask.TaskPhase = this.currentTaskPhase;
                         this.ctlTaskBll.Update(this.currentTask);
                         break;
                     }
                 case 4:
                     {
                         //流程结束
                         this.currentStat.StatDescribe = "流程完成";
                         currentTaskDescribe = "流程结束";
                         // this.currentTaskPhase++;
                         break;
                     }
             }
             return true;
        }
        protected override bool NodeStatParse(ref string reStr)
        {
            if (db2Vals[0] == 0)
            {
                if (productChecked)
                {
                    logRecorder.AddDebugLog(nodeName, "产品离开工位");
                }
                productChecked = false;
                //if (currentTaskPhase != 0)
                if(this.currentTask != null)
                {
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

            else if (db2Vals[0] > 0)
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
                }

            }
            else
            {
                //ThrowErrorStat("PLC错误的状态数据", EnumNodeStatus.设备故障);
                return true;
            }
            return true;
        }

        public override bool ExeBusiness(ref string reStr)
        {
            if (!base.ExeBusiness(ref reStr))
            {
                return false;
            }
            if (db2Vals[0] == 0)
            {
                if (currentTaskPhase > 2)
                {
                    if (!SysCfgModel.SimMode)
                    {
                        if (!AinuoObj.CloseDev(ref reStr))
                        {
                            logRecorder.AddDebugLog(nodeName, this.currentStat.ProductBarcode + "关闭艾诺检测设备失败," + reStr);
                           
                        }
                    }
                }
            }
            if (!NodeStatParse(ref reStr))
            {
                return false;
            }
            
            if (!checkEnable)
            {
                return true;
            }
            bool manual = false;
            if(this.nodeID=="2001")
            {
                manual = SysCfgModel.ManualMode1;

            }
            else
            {
                manual = SysCfgModel.ManualMode3;
            }
            if (manual)
            {
                this.db1ValsToSnd[33] = 1;
                return ManualExeBusiness(ref reStr);
            }
            else
            {
                this.db1ValsToSnd[33] = 0;
            }
            switch (currentTaskPhase)
            {
                case 1:
                    {
                       // checkRetryCounter = 0;
                        cfgModel = null;
                        detectCode = "";
                 
                        if(!ProcessStartCheck(3,!SysCfgModel.DebugMode))
                        {
                            break;
                        }
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.currentTask.TaskParam = rfidUID;
                        this.ctlTaskBll.Update(this.currentTask);
                        //发送机器人配方
                        ProductSizeCfgBll productCfg = new ProductSizeCfgBll();
                        string cataCode = this.currentStat.ProductBarcode.Substring(0, 13);
                        cfgModel = productCfg.GetModel(cataCode);
                        if(cfgModel==null)
                        {
                            if(this.db1ValsToSnd[0] != 6)
                            {
                                ThrowErrorStat(string.Format("{0}产品配置信息不存在", this.currentStat.ProductBarcode), EnumNodeStatus.设备故障);
                               // logRecorder.AddDebugLog(nodeName, string.Format("{0}产品配置信息不存在", this.currentStat.ProductBarcode));
                            }
                            this.currentStat.StatDescribe = "配置不存在";
                            checkEnable = false;
                            this.db1ValsToSnd[0] = 6;
                            return true;
                        }
                        this.db1ValsToSnd[2] = (short)cfgModel.robotProg;
                        this.currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 2:
                    {
                        if(this.nodeID=="2001")
                        {
                            if (this.db2Vals[0] != 2)
                            {
                                break;
                            }
                        }
                       
                        if (!SysCfgModel.SimMode)
                        {
                            if (!AinuoObj.OpenDev(ref reStr))
                            {
                                logRecorder.AddDebugLog(nodeName, "启动艾诺安规测试失败," + reStr);
                                return false;
                            }
                        }
                        detectStartTime = DateTime.Now;
                        logRecorder.AddDebugLog(this.nodeName, this.currentStat.ProductBarcode + "开始检测，启动安规测试仪");
                        this.currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 3:
                    {
                        TimeSpan timeElapse = System.DateTime.Now - detectStartTime;
                        if(timeElapse.TotalSeconds<17)
                        {
                            break;
                        }
                        currentTaskDescribe = "开始查询安规结果";
                        DevAccess.AinuoDetectModel detectModel = null;
                        string strDetectVal = "";
                        if(SysCfgModel.SimMode)
                        {
                            detectModel = GenerateSimDetect();
                            //currentTaskDescribe = "开始保存本地检测结果";
                            //checkRe = 0;
                            //if (!MesDatalocalSave(this.currentStat.ProductBarcode, checkRe, detectCode, "", 0))
                            //{
                            //    Console.WriteLine(string.Format("{0}保存检测数据到本地数据库失败", nodeName));
                            //    break;
                            //}
                        }
                        else
                        {
                            detectModel = AinuoObj.GetDetectResult(ref reStr);
                            //if (detectModel == null)
                            //{
                            //    TimeSpan timeElapse = System.DateTime.Now - detectStartTime;
                            //    if (timeElapse.TotalMilliseconds < detectTimeout)
                            //    {
                            //        break;
                            //    }
                            //    else
                            //    {
                            //        detectStartTime = System.DateTime.Now;
                            //        logRecorder.AddDebugLog(this.nodeName, string.Format("检测超时,{0}毫秒,重新检测", detectTimeout));
                            //        currentStat.StatDescribe = string.Format("检测超时");
                            //    }
                            //}
                           
                               
                        }
                        if(detectModel == null)
                        {
                            logRecorder.AddDebugLog(nodeName, this.currentStat.ProductBarcode + "检测超时，无数据,"+reStr);
                            ThreadBaseModel.Sleep(1000);
                            break;
                        }
                        float designPower = float.Parse(cfgModel.power1);
                        if (!GetDetectResult(this.currentStat.ProductBarcode,detectModel, designPower, ref detectCode, ref strDetectVal, ref checkRe))
                        {
                            ThreadBaseModel.Sleep(1000);
                            break;
                        }
                        //if (!SysCfgModel.SimMode)
                        //{
                        //    if (!AinuoObj.CloseDev(ref reStr))
                        //    {
                        //        logRecorder.AddDebugLog(nodeName, this.currentStat.ProductBarcode + "关闭艾诺检测设备失败," + reStr);
                        //        break;
                        //    }
                        //}
                        
                        currentTaskDescribe = "开始保存本地检测结果";
                        if(!string.IsNullOrWhiteSpace(detectCode))
                        {
                            if(detectCode.Substring(detectCode.Length-1,1)==",")
                            {
                                detectCode=detectCode.Remove(detectCode.Length - 1, 1);
                            }
                            logRecorder.AddDebugLog(nodeName, this.currentStat.ProductBarcode + "故障码:" + detectCode);
                        }
                        if(checkRe>0)
                        {
                            OutputRecord(this.currentStat.ProductBarcode);
                        }
                        if (!MesDatalocalSave(this.currentStat.ProductBarcode, checkRe, detectCode, strDetectVal, 0))
                        {
                            Console.WriteLine(string.Format("{0}保存检测数据到本地数据库失败", nodeName));
                            break;
                        }
                        this.currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 4:
                    {
                        string checkReStr = "OK";
                        if(this.nodeID != "2001")
                        {
                            //安规1检测完成后不上传数据
                            if (!UploadMesdata(true, this.currentStat.ProductBarcode, mesProcessSeq, ref reStr))
                            {
                                this.currentStat.StatDescribe = "上传MES失败";
                                logRecorder.AddDebugLog(this.nodeName, this.currentStat.StatDescribe);
                                break;
                            }
                        }
                        
                        currentTaskDescribe = "放行";
                        if (checkRe == 1)
                        {
                            db1ValsToSnd[0] = db1StatNG;
                            checkReStr = "NG";
                        }
                        else
                        {
                            db1ValsToSnd[0] = db1StatCheckOK;
                        }
                        logRecorder.AddDebugLog(this.nodeName, this.currentStat.ProductBarcode + "检测完成," + checkReStr);
                        this.currentStat.StatDescribe = "安规检测完成," + checkReStr;
                        checkFinished = true;
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 5:
                    {
                        //流程结束
                        this.currentStat.StatDescribe = "流程完成";
                        currentTaskDescribe = "流程结束";
                       
                        // this.currentTaskPhase++;
                        break;
                    }
            }
            return true;
        }
        private bool GetDetectResult(string barcode,AinuoDetectModel detectModel,float designPower,ref string detectCode,ref string strDetectVal,ref int checkResult)
        {
           
             detectCode = "";
             string ngDetect = ""; 
             bool re = true;
             checkResult = 0;
             if (detectModel == null)
             {
                 checkResult = 1;
                 detectCode = "1829";
                 strDetectVal = "";
                 ngDetect = detectCodeMap["1829"];
                 logRecorder.AddDebugLog(nodeName, barcode + "不良信项:" + ngDetect);
                 return re;
             }
             string logStr = barcode + "测试结果：";
             if (nodeID == "2001")
             {
                 List<DevAccess.AinuoEnumDetectItem> detectItems = new List<DevAccess.AinuoEnumDetectItem>();
                 detectItems.AddRange(new DevAccess.AinuoEnumDetectItem[] { DevAccess.AinuoEnumDetectItem.接地, DevAccess.AinuoEnumDetectItem.启动, DevAccess.AinuoEnumDetectItem.泄漏, DevAccess.AinuoEnumDetectItem.耐压 });
                 foreach (DevAccess.AinuoDetectItemModel detectItem in detectModel.DetectItems)
                 {
                     if (!detectItems.Contains(detectItem.detectItemType))
                     {
                         continue;
                     }
                     strDetectVal += detectItem.GetDataStr();
                 }
                
                 foreach (DevAccess.AinuoDetectItemModel detectItem in detectModel.DetectItems)
                 {
                     if (!detectItems.Contains(detectItem.detectItemType))
                     {
                         continue;
                     }
                     logStr += (detectItem.detectItemType.ToString() + "测试数据：" + detectItem.GetDataStr() + ",");
                     if (detectItem.detectResult != DevAccess.AinuoEnumDetectResult.OK)
                     {
                         checkResult = 1;

                         //获取不良代码
                         if (detectItem.detectItemType == DevAccess.AinuoEnumDetectItem.启动)
                         {
                             string code="1812";
                             detectCode += code +",";
                             ngDetect = ngDetect + detectCodeMap[code] + ",";
                         }
                         else if (detectItem.detectItemType == DevAccess.AinuoEnumDetectItem.接地)
                         {
                             string code = "1804";
                             detectCode += code + ",";
                             ngDetect = ngDetect + detectCodeMap[code] + ",";
                         }
                         else if (detectItem.detectItemType == DevAccess.AinuoEnumDetectItem.耐压)
                         {
                             string code = "1805";
                             detectCode += code + ",";
                             ngDetect = ngDetect + detectCodeMap[code] + ",";
                             
                         }
                         else if (detectItem.detectItemType == DevAccess.AinuoEnumDetectItem.泄漏)
                         {
                             string code = "1806";
                             detectCode += code + ",";
                             ngDetect = ngDetect + detectCodeMap[code] + ",";
                            
                         }
                     }
                 }

                 //是否检测完成
                 foreach (DevAccess.AinuoEnumDetectItem itemEnum in detectItems)
                 {
                     bool schOK = false;
                    foreach(DevAccess.AinuoDetectItemModel detectItem in detectModel.DetectItems)
                    {
                        if(itemEnum == detectItem.detectItemType)
                        {
                            schOK = true;
                            break;
                        }
                    }
                    if(!schOK)
                    {
                        re = false;
                        break;
                    }
                 }

             }
             else if (nodeID == "2003")
             {
                 // List<DevAccess.AinuoEnumDetectItem> detectItems = new List<DevAccess.AinuoEnumDetectItem>();
                 //detectItems.AddRange(new DevAccess.AinuoEnumDetectItem[] { DevAccess.AinuoEnumDetectItem.功率});
                 //性能检测，判断是否合格，不再检测设备上判断，比较功率数值判断
                 List<float> powerList = new List<float>();
                 foreach (DevAccess.AinuoDetectItemModel detectItem in detectModel.DetectItems)
                 {
                     if (detectItem.detectItemType != DevAccess.AinuoEnumDetectItem.功率)
                     {
                         //re = false;
                         continue;
                     }
                    
                     strDetectVal += detectItem.GetDataStr();
                     powerList.Add(detectItem.detectParams[2].val);
                 }
                 logStr += strDetectVal;
                 //获取不良代码
               
                 if (powerList.Count() < 2)
                 {
                     string code = "1829";
                     ngDetect = ngDetect + detectCodeMap[code] + ",";
                     detectCode = code;
                     logRecorder.AddDebugLog(nodeName, string.Format("功率项数不足2条"));
                 }
                 else
                 {
                     float powerMax = powerList[0];
                     float powerMin = powerList[1];
                     logStr = string.Format("功率检测结果,强档：{0}W,弱档{1}W", powerMax, powerMin);
                     if (detectModel.DetectItems[0].detectResult != AinuoEnumDetectResult.OK)
                     {
                         detectCode = "1821";
                         ngDetect = "强档功率NG";
                         logStr += "强档功率NG";
                         checkResult = 1;
                     }
                     if (detectModel.DetectItems[1].detectResult != AinuoEnumDetectResult.OK)
                     {
                         detectCode = "1821";
                         ngDetect = "弱档功率NG";
                         logStr += "弱档功率NG";
                         checkResult = 1;
                     }
                     if(checkRe == 0)
                     {
                         if ((int)powerMin == 0 && (int)powerMax != 0)
                         {
                             checkResult = 1;
                             string code = "1811";
                             ngDetect = ngDetect + detectCodeMap[code] + ",";
                             detectCode = code;

                             //logStr += detectCodeMap[detectCode];
                         }
                         else if ((int)powerMax == 0 && (int)powerMin != 0)
                         {
                             checkResult = 1;
                             string code = "1810";
                             ngDetect = ngDetect + detectCodeMap[code] + ",";
                             detectCode = code;
                             //logStr += detectCodeMap[detectCode];
                         }
                         else if ((int)powerMax == 0 && (int)powerMin == 0)
                         {
                             checkResult = 1;
                             string code = "1812";
                             ngDetect = ngDetect + detectCodeMap[code] + ",";
                             detectCode = code;
                             //logStr += detectCodeMap[detectCode];
                         }
                         else if (powerMin > powerMax)
                         {
                             checkResult = 1;
                             string code = "1821";
                             ngDetect = ngDetect + detectCodeMap[code] + ",";
                             detectCode = code;
                             //logStr += detectCodeMap[detectCode];
                         }
                         else if(powerMin<10 || powerMin>60 || powerMax<60 || powerMax>90)
                    //     else if (powerMin > designPower * 1.2 || powerMax > designPower * 1.2)
                         {
                             checkResult = 1;
                             string code = "1821";
                             ngDetect = ngDetect + detectCodeMap[code] + ",";
                             detectCode = code;
                             //logStr += detectCodeMap[detectCode];
                         }
                         else
                         {
                             checkResult = 0;
                             detectCode = "";
                         }
                     }
                    

                 }
             }
           
             logRecorder.AddDebugLog(nodeName, logStr);
             if (!string.IsNullOrWhiteSpace(ngDetect))
             {
                 logRecorder.AddDebugLog(nodeName, barcode+"不良项:"+ngDetect);
             }
             if(checkRe != 0)
             {
                 re = true;
             }
             
             return re;
        }
        private DevAccess.AinuoDetectModel GenerateSimDetect()
        {
            AinuoDetectModel detectModel = new AinuoDetectModel();
            string strVal = "";
            string reStr="";
            if(nodeID=="2001")
            {
                strVal = @"1 3000 0000 0000 0000 0000 0 2
                          6 2200 1000 0000 0000 0000 0 1
                          3 0500 0000 0000 0000 0000 0 1
                          4 0037 0000 0000 0000 0000 0 1";
                strVal = strVal.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");
                Console.WriteLine(strVal);
              
            }
            else if(nodeID=="2003")
            {
                strVal = @"5 2200 2000 3500 1000 0000 0 1
                           5 2200 1000 2500 1000 0000 0 1";
                strVal = strVal.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");
                Console.WriteLine(strVal);
            }
            if(!detectModel.Parse(strVal,ref reStr))
            {
                Console.WriteLine("检测数据解析失败");
                return null;
            }
            return detectModel;
        }
    }
}
