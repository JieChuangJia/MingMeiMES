using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PLProcessModel;
using FTDataAccess.Model;
using FTDataAccess.BLL;
namespace LineNodes
{
    public class NodeShenhe : CtlNodeBaseModel
    {
        public override bool BuildCfg(System.Xml.Linq.XElement xe, ref string reStr)
        {

            if (!base.BuildCfg(xe, ref reStr))
            {
                return false;
            }
            this.dicCommuDataDB1[1].DataDescription = "1：检查OK，放行,2：读卡失败,3：产品未绑定,4：审核未完成";
            this.dicCommuDataDB1[2].DataDescription = "0：允许检测流程开始,1：流程锁定";

            for (int i = 0; i < 30; i++)
            {
                this.dicCommuDataDB1[3 + i].DataDescription = string.Format("条码[{0}]", i + 1);
            }
            this.dicCommuDataDB2[1].DataDescription = "0:复位，1：有产品,2:无产品";
            this.dicCommuDataDB2[2].DataDescription = "0:无板,1：有板"; 
            return true;
        }
        protected override bool NodeStatParse(ref string reStr)
        {
            if (db2Vals[1] == 0) //对于审核工位，判断有板信号是否复位，决定流程是否复位
            {
                if (currentTaskPhase != 0)
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
                rfidUID = string.Empty;
                checkEnable = true;
            }
            else if (db2Vals[1] == 1) //有板信号
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
                }

            }
            else
            {
                ThrowErrorStat("PLC错误的状态数据", EnumNodeStatus.设备故障);
                return true;
            }
            return true;
        }
        public override bool ExeBusiness(ref string reStr)
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
            switch (currentTaskPhase)
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
                        //开始读卡
                        if(string.IsNullOrWhiteSpace(rfidUID))
                        {
                            if (SysCfgModel.SimMode)
                            {
                                rfidUID = SimRfidUID;

                            }
                            else
                            {
                                rfidUID = rfidRW.ReadUID();

                            }
                        }
                       
                        currentTaskDescribe = "开始读RFID";
                        if (string.IsNullOrWhiteSpace(rfidUID))
                        {
                            this.currentStat.StatDescribe = "读RFID卡失败";
                            currentTaskDescribe = this.currentStat.StatDescribe;
                            if (db1ValsToSnd[0] != db1StatRfidFailed)
                            {
                                logRecorder.AddDebugLog(nodeName, "读RFID卡失败");
                            }
                            db1ValsToSnd[0] = db1StatRfidFailed;
                            break;
                        }
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.currentTask.TaskParam = rfidUID;

                        this.ctlTaskBll.Update(this.currentTask);

                        db1ValsToSnd[0] = 0;
                        this.currentStat.StatDescribe = "RFID识别完成";
                        currentTaskPhase++;

                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        
                        break;
                    }
                case 2:
                    {

                        if (db2Vals[0] == 1)//有产品
                        {
                            if (!productChecked)
                            {
                                logRecorder.AddDebugLog(nodeName, "检测到有产品");
                            }
                            productChecked = true;

                            //根据绑定，查询条码，赋条码
                            OnlineProductsModel productBind = productBindBll.GetModelByrfid(rfidUID);
                            if (productBind == null)
                            {
                                if(db1ValsToSnd[0] !=3)
                                {
                                    logRecorder.AddDebugLog(nodeName, "未投产，rfid:" + rfidUID);
                                }
                                db1ValsToSnd[0] = 3;
                                this.currentStat.StatDescribe = "未投产";
                               
                                checkEnable = false;
                                break;
                            }

                            this.currentStat.ProductBarcode = productBind.productBarcode;
                            logRecorder.AddDebugLog(nodeName, "开始处理维修审核流程," + productBind.productBarcode);
                            productBind.currentNode = this.nodeName;
                            productBindBll.Update(productBind);
                            BarcodeFillDB1(productBind.productBarcode, 2);
                            if (!this.plNodeModel.checkRequired)
                            {
                                if (!ClearLoacalMesData(productBind.productBarcode))
                                {
                                    break;
                                }
                                this.db1ValsToSnd[0] = 1;
                                currentTaskPhase = 4;
                                break;
                            }
                            currentTaskPhase++;
                            this.currentTask.TaskPhase = this.currentTaskPhase;
                            this.ctlTaskBll.Update(this.currentTask);
                        }
                        else if(db2Vals[0] == 2)//无产品
                        {
                            if (productChecked)
                            {
                                logRecorder.AddDebugLog(nodeName, "产品离开工位");
                            }
                            productChecked = false;

                            OnlineProductsModel productBind = productBindBll.GetModelByrfid(rfidUID);
                            if (productBind != null)
                            {
                                this.currentStat.ProductBarcode = productBind.productBarcode;
                                ClearLoacalMesData(this.currentStat.ProductBarcode);
                                //解绑
                                if (!TryUnbind(this.rfidUID, this.currentStat.ProductBarcode))
                                {
                                    logRecorder.AddDebugLog(nodeName, string.Format("无产品状态，解绑{0}失败", this.currentStat.ProductBarcode));
                                    break;
                                }
                                else
                                {
                                    logRecorder.AddDebugLog(nodeName, string.Format("无产品状态，解绑{0}完成", this.currentStat.ProductBarcode));
                                    currentTaskPhase = 4;
                                    this.db1ValsToSnd[0]=1; //放行
                                }
                            }
                            else
                            {
                                logRecorder.AddDebugLog(nodeName, string.Format("RFID:{0}未绑定状态，无需解绑放行", this.rfidUID));
                                currentTaskPhase = 4;
                                this.db1ValsToSnd[0] = 1; //放行
                                //this.currentTask.TaskPhase = this.currentTaskPhase;
                                //this.ctlTaskBll.Update(this.currentTask);
                                break;
                            }
                           
                        }
                        else
                        {
                            //等待有板信号=1或2
                            break;
                        }
                        
                        break;
                    }
                case 3:
                    {
                        currentTaskDescribe = "开始查询MES维修审核是否完成";
                        int mesRe = 0;
                        if (!SysCfgModel.MesOfflineMode && PLProcessModel.SysCfgModel.MesCheckEnable)
                        {
                            // mesRe = mesDA.MesAssemDown(new string[] { this.currentStat.ProductBarcode, LineMonitorPresenter.mesLineID }, ref reStr);

                            mesRe = mesDA.MesReAssemEnabled(new string[] { this.currentStat.ProductBarcode, PLProcessModel.SysCfgModel.mesLineID }, ref reStr);
                        }
                       
                        if(mesRe == 0)
                        {
                            if(this.db1ValsToSnd[0] != 1)
                            {
                                logRecorder.AddDebugLog(nodeName, string.Format("{0}MES审核完成", this.currentStat.ProductBarcode));
                            }
                            this.db1ValsToSnd[0] = 1;
                           // logRecorder.AddDebugLog(nodeName, string.Format("MES离线模式，{0}无需审核，完成", this.currentStat.ProductBarcode));
                        }
                        else
                        {
                            currentTaskDescribe = this.currentStat.ProductBarcode+",维修审核未完成：" + reStr ;
                            if(this.db1ValsToSnd[0] != 4)
                            {
                                logRecorder.AddDebugLog(nodeName, currentTaskDescribe);
                            }
                            this.db1ValsToSnd[0] = 4;
                            break;
                        }
                        ClearLoacalMesData(this.currentStat.ProductBarcode);
                       
                        logRecorder.AddDebugLog(nodeName, string.Format("{0}维修审核完成", this.currentStat.ProductBarcode));
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 4:
                    {
                        this.db1ValsToSnd[0] = 1;
                        currentTaskDescribe = "审核工位处理完成";
                        this.currentTask.TaskStatus = EnumTaskStatus.已完成.ToString();
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                default:
                    break;
            }
            return true;
        }
    }
}
