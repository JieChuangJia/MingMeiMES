using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using FTDataAccess.BLL;
using FTDataAccess.Model;
using PLProcessModel;
namespace LineNodes
{
    /// <summary>
    /// 打码机
    /// </summary>
    public class NodePackLabel : CtlNodeBaseModel
    {
        private string packBarcode = "";
        private int barcodeFailCounter = 0;
        private DBAccess.BLL.BatteryPackBll packBll = new DBAccess.BLL.BatteryPackBll();
        public override bool BuildCfg(System.Xml.Linq.XElement xe, ref string reStr)
        {
            if (!base.BuildCfg(xe, ref reStr))
            {
                return false;
            }
            this.dicCommuDataDB1[1].DataDescription = "1:rfid复位，2：RFID成功，3：读RFID失败";
            this.dicCommuDataDB1[2].DataDescription = "1：复位/待机状态,2：数据读取中,3：数据读取完毕，放行";
            this.dicCommuDataDB1[3].DataDescription = "1：读码状态复位,2：读码成功,3：读码失败,4：读码两次失败，报警";
            this.dicCommuDataDB2[1].DataDescription = "1：无板,2：有板，读卡请求";
            this.dicCommuDataDB2[2].DataDescription = "1：无扫码请求,2：扫码请求";
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
                        if (!RfidReadC())
                        {
                            break;
                        }
                        db1ValsToSnd[1] = 2;//
                        if (!ProductTraceRecord())
                        {
                            break;
                        }
                        barcodeFailCounter = 0;
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
                        //读PACK条码
                        packBarcode = "";
                        if(this.db2Vals[1] !=2)
                        {
                            break;
                        }
                        if(SysCfgModel.SimMode)
                        {
                            packBarcode = SimBarcode;
                        }
                        else
                        {
                            packBarcode = barcodeRW.ReadBarcode();
                        }
                        if(string.IsNullOrWhiteSpace(packBarcode))
                        {
                            barcodeFailCounter++;
                            if(this.db1ValsToSnd[2] != 3)
                            {
                                currentTaskDescribe = "读PACK条码失败,尝试重新读取....";
                                logRecorder.AddDebugLog(nodeName, "读到PACK条码失败,尝试重新读取....");
                            }
                            this.db1ValsToSnd[2] = 3;
                            if(barcodeFailCounter>2)
                            {
                                this.db1ValsToSnd[2] = 4;
                            }
                            
                            Thread.Sleep(2000);
                            break;
                        }
                        this.db1ValsToSnd[2] = 2;
                        logRecorder.AddDebugLog(nodeName, string.Format("读到PACK条码:{0}", packBarcode));
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 3:
                    {
                        //PACK-模块绑定
                        List<DBAccess.Model.BatteryModuleModel> modList = modBll.GetModelList(string.Format("palletID='{0}' and palletBinded=1", this.rfidUID));
                        if (modList != null && modList.Count() > 0)
                        {
                            DBAccess.Model.BatteryPackModel pack = new DBAccess.Model.BatteryPackModel();
                            pack.batPackID = packBarcode;
                            pack.packAsmTime = System.DateTime.Now;
                            pack.bmsID = "";
                            pack.opWorkerID = "";
                            if(packBll.Exists(packBarcode))
                            {
                                packBll.Delete(packBarcode);
                            }
                            packBll.Add(pack);
                            foreach (DBAccess.Model.BatteryModuleModel mod in modList)
                            {
                                mod.batPackID = packBarcode;
                                modBll.Update(mod);
                                logRecorder.AddDebugLog(nodeName, string.Format("模块{0}绑定到PACK{1}", mod.batModuleID, packBarcode));
                            }
                        }
                        else
                        {
                            logRecorder.AddDebugLog(nodeName, "工装板：" + this.rfidUID + "无绑定模块数据");
                        }
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 4:
                    {
                        db1ValsToSnd[1] = 3;
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
