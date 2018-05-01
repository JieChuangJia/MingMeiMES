using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PLProcessModel;
using FTDataAccess.BLL;
using DevInterface;
using System.Threading;
namespace LineNodes
{
    public class NodeUV:CtlNodeBaseModel
    {
        public override bool BuildCfg(System.Xml.Linq.XElement xe, ref string reStr)
        {
            if (!base.BuildCfg(xe, ref reStr))
            {
                return false;
            }
            this.dicCommuDataDB1[1].DataDescription = "A通道读卡结果，1：复位/待机状态,2：RFID读取成功,3：RFID读取失败,4：无绑定数据";
            this.dicCommuDataDB1[2].DataDescription = "B通道读卡结果,1：复位/待机状态,2：RFID读取成功,3：RFID读取失败,4：无绑定数据";
            this.dicCommuDataDB1[3].DataDescription = "A通道,1：复位/待机状态,2：数据读取中,3：数据读取完毕，放行";
            this.dicCommuDataDB1[4].DataDescription = "B通道,1：复位/待机状态,2：数据读取中,3：数据读取完毕，放行";
            this.dicCommuDataDB2[1].DataDescription = "0：无,1：正在运行A通道,2：正在运行B通道";
            this.dicCommuDataDB2[2].DataDescription = "1：A通道无板,2：A通道有板，读卡请求";
            this.dicCommuDataDB2[3].DataDescription = "1：B通道无板,2：B通道有板，读卡请求";
            return true;
        }
        public override bool ExeBusiness(ref string reStr)
        {
            if (!nodeEnabled)
            {
                return true;
            }
            if (!ExeBusinessAB(ref reStr))
            {
                return false;
            }
            switch (currentTaskPhase)
            {
                case 1:
                    {
                        if (!RfidReadAB())
                        {
                            break;
                        }
                        this.currentTask.TaskParam = rfidUID;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        if (!ProductTraceRecord())
                        {
                            break;
                        }
                        currentTaskPhase++;
                 
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 2:
                    {

                        List<DBAccess.Model.BatteryModuleModel> modList = modBll.GetModelList(string.Format("palletID='{0}' and palletBinded=1", this.rfidUID));
                       
                        if (!PreMech(modList, ref reStr))
                        {
                            Console.WriteLine(string.Format("{0},{1}", nodeName, reStr));
                            break;
                        }
                        //if (modList.Count() < 1)
                        //{
                        //    currentTaskPhase = 4;
                        //    break;
                        //}
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 3:
                    {
                        List<DBAccess.Model.BatteryModuleModel> modList = modBll.GetModelList(string.Format("palletID='{0}' and palletBinded=1", this.rfidUID));
                       
                        if (!AfterMech(modList, ref reStr))
                        {
                           // Console.WriteLine(string.Format("{0},{1}", nodeName, reStr));
                            break;
                        }
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 4:
                    {
                        currentTaskDescribe = "开始上传MES数据";
                        List<DBAccess.Model.BatteryModuleModel> modList = modBll.GetModelList(string.Format("palletID='{0}' and palletBinded=1", this.rfidUID));
                        if(modList==null || modList.Count()==0)
                        {
                            currentTaskPhase = 5;
                            break;
                        }
                        int uvTime = 0;
                        int uvTem = 0;
                        if(!plcRW2.ReadDB("D9000", ref uvTime))
                        {
                            currentTaskDescribe = "读UV设备PLC 时间，温度参数失败";
                            break;
                        }
                        if (!plcRW2.ReadDB("D9002", ref uvTem))
                        {
                            currentTaskDescribe = "读UV设备PLC 时间，温度参数失败";
                            break;
                        }
                        string mesItemVal = string.Format("UV胶固化时间:{0}:s|UV胶固化温度:{1}:℃", uvTime, uvTem);
                        string M_WORKSTATION_SN = "Y00100701";
                        int M_FLAG = 3;
                        string M_DEVICE_SN = "";
                        string M_UNION_SN = "";
                        string M_CONTAINER_SN = "";
                        string M_LEVEL = "";
                        string M_ITEMVALUE = mesItemVal;
                        string strJson = "";
                        foreach(DBAccess.Model.BatteryModuleModel mod in modList)
                        {
                            string M_SN = mod.batModuleID;
                            RootObject rObj = new RootObject();
                            rObj = WShelper.DevDataUpload(M_FLAG, M_DEVICE_SN, M_WORKSTATION_SN, M_SN, M_UNION_SN, M_CONTAINER_SN, M_LEVEL, M_ITEMVALUE, ref strJson);
                            logRecorder.AddDebugLog(nodeName, string.Format("模组{0}UV结果{1}上传MES，返回{2}", M_SN,mesItemVal, rObj.RES));
                            this.currentTaskDescribe = string.Format("模组{0}UV结果{1}上传MES，返回{2}", M_SN, mesItemVal, rObj.RES);
                        }
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        
                        break;
                    }
                case 5:
                    {
                        db1ValsToSnd[2 + this.channelIndex - 1] = 3;
                        currentTaskDescribe = "流程完成";
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                       
                        this.currentTask.TaskStatus = EnumTaskStatus.已完成.ToString();
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                default:
                    break;
            }
            return true;
        }

        protected override  void ExeRfidBusinessAB()
        {
            
            if (this.rfidRWList == null || this.rfidRWList.Count() < 1)
            {
                return;
            }
            PLNodesBll plNodeBll = new PLNodesBll();
            if (this.db2Vals[1] == 2)
            {
                //A通道
                if (string.IsNullOrWhiteSpace(this.rfidUIDA))
                {
                    IrfidRW rw = null;
                    if (SysCfgModel.SimMode)
                    {
                        this.rfidUIDA = this.SimRfidUID;
                    }
                    else if (this.rfidRWList.Count > 0)
                    {
                        rw = this.rfidRWList[0];
                        this.rfidUIDA = rw.ReadUID();

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
                       // logRecorder.AddDebugLog(nodeName, string.Format("A通道读到RFID:{0}", this.rfidUIDA));
                        if (IsEmptyPallet(this.rfidUIDA) == true)
                        {
                            this.db1ValsToSnd[0] = 4;
                        }
                        else
                        {
                            this.db1ValsToSnd[0] = 2;
                        }
                      
                      
                    }
                }
                else
                {
                  //  logRecorder.AddDebugLog(nodeName, string.Format("A通道读到RFID:{0}", this.rfidUIDA));
                    if (IsEmptyPallet(this.rfidUIDA) == true)
                    {
                        this.db1ValsToSnd[0] = 4;
                    }
                    else
                    {
                        this.db1ValsToSnd[0] = 2;
                    }
                }

            }
            else if (this.db2Vals[1] == 1)
            {
                this.db1ValsToSnd[0] = 1;
                this.plNodeModel.tag1 = "";
                plNodeBll.Update(this.plNodeModel);
            }

            if (this.db2Vals[2] == 2)
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
                    //    logRecorder.AddDebugLog(nodeName, string.Format("B通道读到RFID:{0}", this.rfidUIDB));
                      //  logRecorder.AddDebugLog(nodeName, string.Format("A通道读到RFID:{0}", this.rfidUIDA));
                        if (IsEmptyPallet(this.rfidUIDA) == true)
                        {
                            this.db1ValsToSnd[1] = 4;
                        }
                        else
                        {
                            this.db1ValsToSnd[1] = 2;
                        }
                      
                    }

                }
                else
                {
                  //  logRecorder.AddDebugLog(nodeName, string.Format("A通道读到RFID:{0}", this.rfidUIDA));
                    if (IsEmptyPallet(this.rfidUIDA) == true)
                    {
                        this.db1ValsToSnd[1] = 4;
                    }
                    else
                    {
                        this.db1ValsToSnd[1] = 2;
                    }
                }

            }
            else if (this.db2Vals[2] == 1)
            {

                this.db1ValsToSnd[1] = 1;
                this.plNodeModel.tag2 = "";
                plNodeBll.Update(this.plNodeModel);
            }

        }

        /// <summary>
        /// 是否为空板，空板就是没有绑定数据
        /// </summary>
        /// <param name="rfidUID">rfid数据</param>
        /// <returns></returns>
        public bool IsEmptyPallet(string rfidUID)
        {

            List<DBAccess.Model.BatteryModuleModel> modList = modBll.GetModelList(string.Format("palletID='{0}' and palletBinded=1", rfidUID));
            if(modList!=null&&modList.Count>0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}
