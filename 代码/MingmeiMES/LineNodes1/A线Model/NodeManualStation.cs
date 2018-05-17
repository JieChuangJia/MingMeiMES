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
    class NodeManualStation : CtlNodeBaseModel
    {
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

                    LogRecorder.AddDebugLog(nodeName, string.Format("RFID：{0}", this.rfidUID));
                    currentTaskPhase++;
                    break;
                case 2:
                    if(this.nodeID =="OPA013")
                    {
                       if(false == UploadDataToMes(1,"Y00100501", this.rfidUID))
                       {
                          // Console.WriteLine("{0}上传MES二位码失败！", this.nodeName);
                           this.currentTaskDescribe = string.Format("上传MES二维码失败！");
                           break;
                       }
                    }
                    else if (this.nodeID == "OPA014")
                    {
                        if (false == UploadDataToMes(1,"Y00101001", this.rfidUID))
                        {
                            //Console.WriteLine("{0}上传MES二位码失败！", this.nodeName);
                            this.currentTaskDescribe = string.Format("上传MES二维码失败！");
                            break;
                        }
                        
                    }
                    else if (this.nodeID == "OPA015")
                    {
                        if (false == UploadDataToMes(1,"Y00101301", this.rfidUID))
                        {
                          //  Console.WriteLine("{0}上传MES二位码失败！", this.nodeName);
                            this.currentTaskDescribe = string.Format("上传MES二维码失败！");
                            break;
                        }
                    }
                    db1ValsToSnd[2 + this.channelIndex - 1] = 3;
                    currentTaskPhase++;
                    
                    LogRecorder.AddDebugLog(nodeName, string.Format("上传MES二位码成功：{0}", this.rfidUID));
                    this.currentTaskDescribe = string.Format("上传MES二维码成功！");
                    break;
                case 3:
                    {
                        currentTaskDescribe = "流程完成";
                        break;
                    }
                default:
                    break;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="M_WORKSTATION_SN">工作中心号码</param>
        /// <param name="rfid">二维码</param>
        /// <returns></returns>
        private bool UploadDataToMes(int flag,string workStaionSn,string rfid)
        {
            string M_AREA = "Y001";
            string M_WORKSTATION_SN = workStaionSn;
            string M_DEVICE_SN = "";
       
            string M_UNION_SN = "";
            string M_CONTAINER_SN = "";
            string M_LEVEL = "";
            string M_ITEMVALUE ="" ;
            RootObject rObj = new RootObject();
            List<DBAccess.Model.BatteryModuleModel> modelList = modBll.GetModelList(string.Format("palletID='{0}' and palletBinded=1", rfid)); //modBll.GetModelByPalletID(this.rfidUID, this.nodeName);
            if (modelList == null || modelList.Count == 0)
            {
                return false;
            }
            string barcode = modelList[0].batModuleID;
            string strJson = "";

            rObj = DevDataUpload(flag, M_DEVICE_SN, M_WORKSTATION_SN, barcode, M_UNION_SN, M_CONTAINER_SN, M_LEVEL, M_ITEMVALUE, ref strJson);
            if (rObj.RES.Contains("OK"))
            {
                return true;
            }
            else
            {
              //  Console.WriteLine(this.nodeName + "上传MES二维码信息错误：" + rObj.RES);
                return false;
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
