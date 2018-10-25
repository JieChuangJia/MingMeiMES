using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FTDataAccess.BLL;
using FTDataAccess.Model;
using PLProcessModel;
namespace LineNodes
{
    public class NodePackOutput : CtlNodeBaseModel
    {
        public override bool BuildCfg(System.Xml.Linq.XElement xe, ref string reStr)
        {
            if (!base.BuildCfg(xe, ref reStr))
            {
                return false;
            }
            this.dicCommuDataDB1[1].DataDescription = "1:rfid复位，2：RFID成功，3：读RFID失败";
            this.dicCommuDataDB1[2].DataDescription = "1：复位/待机状态,2：数据读取中,3：数据读取完毕，放行";
            this.dicCommuDataDB2[1].DataDescription = "1：无板,2：有板，读卡请求";
     
            return true;
        }
        public override bool ExeBusiness(ref string reStr)
        {
            if (!ExeBusinessC(ref reStr))
            {
                return false;
            }
         
            switch (currentTaskPhase)
            {
                case 1:
                    {
                        currentTaskDescribe = "读取RFID！";
                        if (!RfidReadC())
                        {
                            break;
                        }
                        this.TxtLogRecorder.WriteLog("工装板数据读取成功:" + this.rfidUID);
                        if (GetPalletCheckNg() == true)//NG处理
                        {
                            db1ValsToSnd[0] = 5;//
                            currentTaskPhase = 3;
                            this.TxtLogRecorder.WriteLog("绑定数据有NG产品，线体服务器处理流程结束！直接放行！");
                            this.logRecorder.AddDebugLog(this.nodeName, "绑定数据有NG产品，线体服务器处理流程结束！");
                            break;
                        }

                        int bindModeCt = modBll.GetRecordCount(string.Format("palletID='{0}' and palletBinded=1", this.rfidUID));
                        if (plcRW2 != null)
                        {
                            if (!plcRW2.WriteDB("D8500", bindModeCt))
                            {
                                this.currentTaskDescribe = "给机台发送模块数量失败";
                                break;
                            }
                        }
                       
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.currentTask.TaskParam = rfidUID;
                        this.ctlTaskBll.Update(this.currentTask);
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;

                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 2:
                    {
                        currentTaskDescribe = "数据上报MES并解绑！";
                        int uploadMesStatus = UploadMesData(this.rfidUID, ref reStr);
                        if (uploadMesStatus == 0)
                        {
                            this.logRecorder.AddDebugLog(this.nodeName, "上传MES数据成功！" + reStr);
                        }
                        else if (uploadMesStatus == 1)
                        {
                            this.logRecorder.AddDebugLog(this.nodeName, "上传MES数据成功！,返回NG：" + reStr);
                        }
                        else
                        {
                            Console.WriteLine(this.nodeName + "上传MES数据失败！" + reStr);
                            break;

                        }
                      
                        db1ValsToSnd[1] = 2;//
                        if (!ProductTraceRecord())
                        {
                            break;
                        }
                        if(!TryUnbind(this.rfidUID,ref reStr))
                        {
                            break;
                        }
                        db1ValsToSnd[1] = 3;
                     
                     
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 3:
                    {
                      
                        currentTaskDescribe = "流程完成";
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.currentTask.TaskStatus = EnumTaskStatus.已完成.ToString();
                        this.ctlTaskBll.Update(this.currentTask);
                        this.TxtLogRecorder.WriteLog("工位流程处理完毕！");
                    
                        break;
                    }
                default:
                    break;
            }
            return true;
        }

        public int UploadMesData(string rfid,ref string reStr)
        {
            string M_AREA = "Y001";
            string M_WORKSTATION_SN = "M00100801";
            string M_DEVICE_SN = "";

            string M_UNION_SN = "";
            string M_CONTAINER_SN = "";
            string M_LEVEL = "";
            string M_ITEMVALUE = "";
            RootObject rObj = new RootObject();
            List<DBAccess.Model.BatteryModuleModel> modelList = modBll.GetModelList(string.Format("palletID='{0}' and palletBinded=1", rfid)); //modBll.GetModelByPalletID(this.rfidUID, this.nodeName);
            if (modelList == null || modelList.Count == 0)
            {
                return 2;
            }
            string barcode = modelList[0].batPackID;
            string strJson = "";

            rObj = DevDataUpload(1, M_DEVICE_SN, M_WORKSTATION_SN, barcode, M_UNION_SN, M_CONTAINER_SN, M_LEVEL, M_ITEMVALUE, ref strJson);
            reStr = rObj.RES+"：上报模组调组条码：" +barcode;

            this.TxtLogRecorder.WriteLog("上传MES数据：" + this.rfidUID + ",工作中心号：M00100801，模组条码："+ barcode);
            if (rObj.RES.ToUpper().Contains("OK"))
            {
                
                return 0;
            }
            else if (rObj.RES.ToUpper().Contains("NG"))
            {
                
                return 1;
            }
            else
            {
                Console.WriteLine(this.nodeName + "上传MES二维码信息错误：" + rObj.RES);

                return 2;
            }
        }
    }
}
