using FTDataAccess.Model;
using PLProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LineNodes
{
    /// <summary>
    /// 人工工位
    /// </summary>
   public class NodeManualStation : CtlNodeBaseModel
    {
       Dictionary<string, string> bakeAddrDic = new Dictionary<string, string>();

       public NodeManualStation()
       {
           bakeAddrDic["OPA016-A-High"] = "D9020";
           bakeAddrDic["OPA016-A-Low"] = "D9022";
           bakeAddrDic["OPA016-A-Time"] = "D9024";

           bakeAddrDic["OPA016-B-High"] = "D9026";
           bakeAddrDic["OPA016-B-Low"] = "D9028";
           bakeAddrDic["OPA016-B-Time"] = "D9030";

           bakeAddrDic["OPA017-A-High"] = "D9040";
           bakeAddrDic["OPA017-A-Low"] = "D9042";
           bakeAddrDic["OPA017-A-Time"] = "D9044";

           bakeAddrDic["OPA017-B-High"] = "D9046";
           bakeAddrDic["OPA017-B-Low"] = "D9048";
           bakeAddrDic["OPA017-B-Time"] = "D9050";
       }

        public override bool BuildCfg(System.Xml.Linq.XElement xe, ref string reStr)
        {
            if (!base.BuildCfg(xe, ref reStr))
            {
                return false;
            }

            if (this.nodeID == "OPA013" || this.nodeID == "OPA014" || this.nodeID == "OPA015")
            {
                this.dicCommuDataDB1[1].DataDescription = "A通道读卡结果，1：复位/待机状态,2：RFID读取成功,3：RFID读取失败,4：无绑定数据";
                this.dicCommuDataDB1[2].DataDescription = "B通道读卡结果,1：复位/待机状态,2：RFID读取成功,3：RFID读取失败,4：无绑定数据";
                this.dicCommuDataDB1[3].DataDescription = "1：复位/待机状态2：数据读取中3：数据读取完毕，放行";
              
                this.dicCommuDataDB2[1].DataDescription = "0：无,1：正在运行A通道,2：正在运行B通道";
                this.dicCommuDataDB2[2].DataDescription = "1：A通道无板,2：A通道有板，读卡请求";
                this.dicCommuDataDB2[3].DataDescription = "1：B通道无板,2：B通道有板，读卡请求";
            }
            else if (this.nodeID == "OPC008" || this.nodeID == "OPC009" || this.nodeID == "OPC010")
            {
                this.dicCommuDataDB1[1].DataDescription = "工位读卡结果，1：复位/待机状态,2：RFID读取成功,3：RFID读取失败,4：无绑定数据";
                this.dicCommuDataDB1[2].DataDescription = "工位状态,1：复位/待机状态,2：数据读取中,3：数据读取完毕，放行";

                this.dicCommuDataDB2[1].DataDescription = "1：通道无板,2：通道有板，读卡请求";
            }
            return true;
        }

        public override bool ExeBusiness(ref string reStr)
        {
            if (!nodeEnabled)
            {
                return true;
            }
            if (this.nodeID == "OPA013" || this.nodeID == "OPA014" || this.nodeID == "OPA015")
            {
                return ExeBindA(ref reStr);
            }
            else if (this.nodeID == "OPC008" || this.nodeID == "OPC009" || this.nodeID == "OPC010")
            {
                return ExeBindC(ref reStr);
            }


            return true;
        }
        protected bool ExeBindA(ref string reStr)
        {
            if (!ExeBusinessAB(ref reStr))
            {
                return false;
            }

            switch (currentTaskPhase)
            {
                case 1:
                    if (!RfidReadAB())
                    {
                        break;
                    }
                    bool needReparid = false;
                    if (this.repairProcess.GetNeedRepair(this.rfidUID, this.NodeID, ref needReparid, ref reStr) == false)
                    {
                        this.logRecorder.AddDebugLog(this.nodeName, "获取返修状态失败:" + reStr);
                        break;
                    }
                    if (needReparid == false)
                    {
                        currentTaskPhase = 3;//直接放行
                        break;
                    }

                    if (!ProductTraceRecord())
                    {

                        break;
                    }
                    LogRecorder.AddDebugLog(nodeName, string.Format("RFID：{0}", this.rfidUID));
                    currentTaskPhase++;
                    this.currentTask.TaskPhase = this.currentTaskPhase;


                    this.ctlTaskBll.Update(this.currentTask);
                    break;
                case 2:
                    int uploadStatus = 0;
                    if (this.nodeID == "OPA013")
                    {
                        uploadStatus = UploadDataToMes(1, "Y00100501", this.rfidUID, ref reStr);
                    }
                    else if (this.nodeID == "OPA014")
                    {
                        //LogRecorder.AddDebugLog(nodeName, "烘烤1NG流程开始处理：");
                        if (BakeNHHandler(this.rfidUID, "OPA016", ref reStr) == false)
                        {
                            LogRecorder.AddDebugLog(nodeName, "烘烤1NG流程处理失败：" + reStr);
                            break;
                        }
                        //LogRecorder.AddDebugLog(nodeName, "烘烤1NG流程开始成功！");
                        uploadStatus = UploadDataToMes(1, "Y00101001", this.rfidUID, ref reStr);
                    }
                    else if (this.nodeID == "OPA015")
                    {
                        //LogRecorder.AddDebugLog(nodeName, "烘烤2NG流程开始处理：");
                        if (BakeNHHandler(this.rfidUID, "OPA017", ref reStr) == false)
                        {
                            LogRecorder.AddDebugLog(nodeName, "烘烤2NG流程处理失败：" + reStr);
                            break;
                        }
                        //LogRecorder.AddDebugLog(nodeName, "烘烤2NG流程成功！");
                        uploadStatus = UploadDataToMes(1, "Y00101301", this.rfidUID, ref reStr);
                    }


                    if (uploadStatus == 0)
                    {
                        //this.logRecorder.AddDebugLog(this.nodeName, "上传MES数据成功：" + reStr);
                    }
                    else if (uploadStatus == 1)
                    {
                        //this.logRecorder.AddDebugLog(this.nodeName, "上传MES数据成功，返回NG：" + reStr);
                    }
                    else if (uploadStatus == 3)//空板放行
                    {
                        //this.logRecorder.AddDebugLog(this.nodeName, "空板直接放行！" +this.rfidUID);
                    }
                    else
                    {
                        currentTaskDescribe = "上传MES数据失败：" + reStr;
                        Console.WriteLine(this.nodeName + "上传MES数据失败：" + reStr);
                        break;
                    }


                    currentTaskPhase++;
                    this.currentTask.TaskPhase = this.currentTaskPhase;


                    this.ctlTaskBll.Update(this.currentTask);
                    LogRecorder.AddDebugLog(nodeName, string.Format("上传MES二位码成功：{0}", this.rfidUID));
                    this.currentTaskDescribe = string.Format("上传MES二维码成功！");
                    break;
                case 3:
                    {
                        this.currentTask.TaskPhase = this.currentTaskPhase;

                        this.currentTask.TaskStatus = EnumTaskStatus.已完成.ToString();
                        this.ctlTaskBll.Update(this.currentTask);
                        db1ValsToSnd[2 + this.channelIndex - 1] = 3;
                        currentTaskDescribe = "流程完成";
                        break;
                    }
                default:
                    break;
            }

            return true;
        }

        private bool BakeNHHandler(string rfid, string bakeName, ref string restr)
        {
            int highTemperature = 0;
            int lowTemperature = 0;
            int bakeTime = 0;
            string channel = "A";
            if(this.channelIndex==1)
            {
                channel = "A";
            }
            else
            {
                channel = "B";
            }
            if (ReadBakeData(bakeName,channel, ref highTemperature, ref lowTemperature, ref bakeTime) == true)
            {
                restr = "读取烘烤数据失败！";
                return false;
            }
            string bakeData = "烘烤最高温度:" + highTemperature / 10 + ":℃|烘烤最低温度:" + lowTemperature / 10 + ":℃|烘烤时间:" + bakeTime / 10 + ":s（这段时间内的最高温度和最低温度）";
            int uploadStatus = 0;
            if (bakeName == "OPA016")
            {
                 uploadStatus = UploadMesProcessParam("Y00100901", bakeData);
            }
            else
            {
                 uploadStatus = UploadMesProcessParam("Y00101201", bakeData);
            }
            if(uploadStatus==0)
            {
                logRecorder.AddDebugLog(this.nodeName, "烘烤上传MES成功！");
            }
            else if (uploadStatus == 1)//NG处理
            {
                logRecorder.AddDebugLog(this.nodeName, "烘烤上传MES返回NG！");
                List<DBAccess.Model.BatteryModuleModel> modList = modBll.GetModelList(string.Format("palletID='{0}' and palletBinded=1", rfid));
                foreach (DBAccess.Model.BatteryModuleModel mod in modList)//解绑
                {
                    logRecorder.AddDebugLog(this.nodeName, mod+":解绑成功！");
                    mod.palletBinded = false;
                    mod.checkResult = 2;
                    modBll.Update(mod);
                }
            }
            else if(uploadStatus == 3)//空板放行
            {
              
                return true;
            }
            else
            {
                //logRecorder.AddDebugLog(this.nodeName, "烘烤上传MES失败！");
                return false;
            }


            return true;
        }
        private bool ReadBakeData(string bakeName, string rfidPos, ref int highTemperature, ref int lowTemperature, ref int bakeTime)
        {
            string highTemperatureAddr = bakeName + "-" + rfidPos + "-High";
            string lowTemperatureAddr = bakeName + "-" + rfidPos + "-Low";
            string bakeTimeAddr = bakeName + "-" + rfidPos + "-Time";

            logRecorder.AddDebugLog(this.nodeName, "高温地址：" + bakeAddrDic[highTemperatureAddr]);
            logRecorder.AddDebugLog(this.nodeName, "低温地址：" + bakeAddrDic[lowTemperatureAddr]);
            logRecorder.AddDebugLog(this.nodeName, "时间：" + bakeAddrDic[bakeTimeAddr]);

            if (this.plcRW.ReadDB(highTemperatureAddr, ref highTemperature) == false)
            {
                return false;
            }
            if (this.plcRW.ReadDB(lowTemperatureAddr, ref lowTemperature) == false)
            {
                return false;
            }
            if (this.plcRW.ReadDB(bakeTimeAddr, ref bakeTime) == false)
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// 上传烘烤工位数据，主要处理NG流程
        /// </summary>
        /// <param name="workStationNum"></param>
        /// <param name="paramItems"></param>
        /// <returns></returns>
        private int UploadMesProcessParam(string workStationNum, string paramItems)
        {
            string M_AREA = "Y001";
            string M_WORKSTATION_SN = workStationNum;
            string M_DEVICE_SN = "";
            // string M_SN = modCode;
            string M_UNION_SN = "";
            string M_CONTAINER_SN = "";
            string M_LEVEL = "";
            string M_ITEMVALUE = paramItems;
            RootObject rObj = new RootObject();
            //this.currentTaskDescribe = "开始上传MES点胶机过程参数";
            List<DBAccess.Model.BatteryModuleModel> modelList = modBll.GetModelList(string.Format("palletID='{0}' ", this.rfidUID)); //modBll.GetModelByPalletID(this.rfidUID, this.nodeName);
            if (modelList == null || modelList.Count == 0)//空板
            {
                this.logRecorder.AddDebugLog(this.nodeName, "空板直接放行！" + this.rfidUID);
             
                return 3;
            }
            string barcode = modelList[0].batModuleID;
            string strJson = "";
            rObj = ProcParamUpload(M_AREA, M_DEVICE_SN, M_WORKSTATION_SN, barcode, M_UNION_SN, M_CONTAINER_SN, M_LEVEL, M_ITEMVALUE, ref strJson);
            if (rObj.RES.ToUpper().Contains("OK"))
            {
                logRecorder.AddDebugLog(this.nodeName, "上传过程数据成功；上传数据：二维码[" + barcode + "],数据[" + paramItems + "]，MES返回：" + rObj.RES);
                
                return 0;
            }
            else if (rObj.RES.ToUpper().Contains("NG"))
            {
                //Console.WriteLine(this.nodeName + "上传过程数据成功，但返回NG：" + rObj.RES);
                logRecorder.AddDebugLog(this.nodeName, "上传过程数据成功，但返回NG；上传数据：二维码["+barcode +"],数据["+paramItems+"]，MES返回："  + rObj.RES);
                return 1;
            }
            else
            {
                logRecorder.AddDebugLog(this.nodeName, "上传过程数据失败，上传数据：二维码[" + barcode + "],数据[" + paramItems + "]，MES返回：" + rObj.RES);
               
                return 2;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="M_WORKSTATION_SN">工作中心号码</param>
        /// <param name="rfid">二维码</param>
        /// <returns></returns>
        private int UploadDataToMes(int flag,string workStaionSn,string rfid,ref string restr)
        {
            try
            {


                string M_AREA = "Y001";
                string M_WORKSTATION_SN = workStaionSn;
                string M_DEVICE_SN = "";

                string M_UNION_SN = "";
                string M_CONTAINER_SN = "";
                string M_LEVEL = "";
                string M_ITEMVALUE = "";
                RootObject rObj = new RootObject();
                List<DBAccess.Model.BatteryModuleModel> modelList = modBll.GetModelList(string.Format("palletID='{0}' and palletBinded=1", rfid)); //modBll.GetModelByPalletID(this.rfidUID, this.nodeName);
                if (modelList == null || modelList.Count == 0)
                {
                    restr = "工装板绑定数据为空："+ rfid;
                    this.logRecorder.AddDebugLog(this.nodeName, "空板直接放行！" + this.rfidUID);
                    return 3;
                }
                string barcode = modelList[0].batModuleID;
                string strJson = "";

                rObj = DevDataUpload(flag, M_DEVICE_SN, M_WORKSTATION_SN, barcode, M_UNION_SN, M_CONTAINER_SN, M_LEVEL, M_ITEMVALUE, ref strJson);
                restr = rObj.RES;
                if (rObj.RES.ToUpper().Contains("OK"))
                {
                    logRecorder.AddDebugLog(this.nodeName, "上传过程数据成功，上传数据：二维码[" + barcode + "],MES返回：" + rObj.RES);
               
                    return 0;
                }
                else if (rObj.RES.ToUpper().Contains("NG"))
                {
                    logRecorder.AddDebugLog(this.nodeName, "上传过程数据成功，但返回NG；上传数据：二维码[" + barcode + "]，MES返回：" + rObj.RES);
               
                    return 1;
                }
                else
                {
                    logRecorder.AddDebugLog(this.nodeName, "上传过程数据失败，上传数据：二维码[" + barcode + "]，MES返回：" + rObj.RES);
               
                    //  Console.WriteLine(this.nodeName + "上传MES二维码信息错误：" + rObj.RES);
                    return 2;
                }
            }
            catch(Exception ex)
            {
                restr = ex.ToString();
                Console.WriteLine(this.nodeName + ex.StackTrace.ToString());
                return 2;
            }
        }
        protected bool ExeBindC(ref string reStr)
        {
            if (!ExeBusinessC(ref reStr))
            {
                return false;
            }
         
            switch (currentTaskPhase)
            {
                case 1:
                    {
                        if (!RfidReadC())
                        {
                            break;
                        }
                        LogRecorder.AddDebugLog(nodeName, string.Format("RFID：{0}", this.rfidUID));
                        currentTaskPhase++;
                        break;
                    }
                case 2:
                    {
                        db1ValsToSnd[2] = 2;
                        currentTaskDescribe = "流程完成";
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        this.currentTask.TaskStatus = EnumTaskStatus.已完成.ToString();
                        break;
                    }
                default:
                    break;
            }
            return true;
        }
    }
}
