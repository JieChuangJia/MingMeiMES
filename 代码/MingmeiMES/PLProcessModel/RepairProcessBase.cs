using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess.BLL;
using DBAccess.Model;
namespace PLProcessModel
{
    public class RepairProcessBase
    {
        protected RepairRecordBLL bllRepairRecord = new RepairRecordBLL();
        protected DBAccess.BLL.BatteryModuleBll modBll = new BatteryModuleBll();
        protected DBAccess.BLL.RepairProcessBLL bllRepairProcess = new DBAccess.BLL.RepairProcessBLL();
        protected DBAccess.BLL.ModuleProcessBLL bllModuleProcess = new ModuleProcessBLL();
        public RepairProcessParam repairParam =  new RepairProcessParam();
        public int stepIndex = 0;
        protected CtlNodeBaseModel nodeBase { get; set; }
        private string dianjiao3 = "Y00100801";
        private string dianjiao4 = "Y00101101";
        private string ccdCheck = "Y00101401";
        private string manualSation2 = "Y00101001";

        private string laserClean1 = "Y00101501";
        private string glueOutCheck1= "Y00101601";
        private string glueOutCheck2 = "Y00101901";
        private string weldAluminum1 = "Y00101701";
        private string wedlAluminum2 = "Y00102001";
        private string laserClean2 = "Y00101801";
        private string cLineJYScrew = "Y00102201";
        private string dcir = "Y00102101";
        public bool NeedRepair = false;

        public RepairProcessBase(CtlNodeBaseModel nodeBase)
        {
            this.nodeBase = nodeBase;
        }
        public void StartStep()
        {
            stepIndex = 1;
            DeleteRepairRecord();
        }
        public void DeleteRepairRecord()
        {
            List<DBAccess.Model.BatteryModuleModel> modelList = modBll.GetModelList(string.Format("palletID='{0}'  and palletBinded=1", this.nodeBase.rfidUID)); //modBll.GetModelByPalletID(this.rfidUID, this.nodeName);
            if (modelList == null || modelList.Count == 0)
            {
                return;
            }
            foreach (DBAccess.Model.BatteryModuleModel battery in modelList)
            {
                bllRepairRecord.Delete(battery.batModuleID);
            }
        }
        public void SetNeedRepair(bool needRepair)
        {
            this.NeedRepair = needRepair;
        }
        private bool NeedRepairByWorkstationOrder(RepairRecordModel repairRecModel, string currlineStationID,ref string restr)
        {
            ModuleProcessModel cuurModuleProcess = bllModuleProcess.GetProcessByID(currlineStationID);
            if (cuurModuleProcess == null)
            {
                restr += "当前线体服务器工位编号错误：" + currlineStationID + ",线体服务器查无此工位编号！";
                return false;
            }
            ModuleProcessModel moduleProcess = bllModuleProcess.GetProcessByWorkStationID(repairRecModel.RepairStartStationNum);
            if (moduleProcess == null)
            {
                 restr += "开始工作中心编号错误：" + repairRecModel.RepairStartStationNum + ",线体服务器查无此工作中心编号！";
                return false;
            }
            if (cuurModuleProcess.ModuleProcessOrder < moduleProcess.ModuleProcessOrder)
            {
                   restr += "当前工作中心号："+cuurModuleProcess.WorkStationID+"的工艺顺序在开始工作中心"+moduleProcess.WorkStationID+"的前面，此工位无序加工！";
                return false;
            }
            return true;
        }
        private bool CCDCheckNeedRepair(RepairRecordModel repairRecModel,string currlineStationID,ref string restr)
        {
            restr = "流程匹配，需要返修！";
            if (repairRecModel.RepairProcessNum == "A4")
            {
                if (repairRecModel.RepairStartStationNum != this.dianjiao3)
                {
                    restr = "返修MES判定在CCD检测，流程代码为A4，起始工位不为3号点胶机";
                    return NeedRepairByWorkstationOrder(repairRecModel, currlineStationID,ref restr);
                }
                return true;
            }
            else if (repairRecModel.RepairProcessNum == "A5")
            {
                if (repairRecModel.RepairStartStationNum != this.dianjiao3)
                {
                    restr = "返修MES判定在CCD检测，流程代码为A5，起始工位不为3号点胶机";
                    return NeedRepairByWorkstationOrder(repairRecModel, currlineStationID,ref restr);
                }
                //if (repairRecModel.RepairStartStationNum != this.dianjiao3)
                //{
                //    restr = "返修MES判定在CCD检测，流程代码为A5，起始工位不为3号点胶机";
                //    return NeedRepairByWorkstationOrder(repairRecModel, currlineStationID);
                //}
                ModuleProcessModel cuurModuleProcess = bllModuleProcess.GetProcessByID(currlineStationID);
                if (cuurModuleProcess == null)
                {
                    restr = "开始工位中心号错误：" + currlineStationID + ",线体服务查无此线体服务编号！";
                    return false;
                }
                if (cuurModuleProcess.WorkStationID == this.ccdCheck || cuurModuleProcess.WorkStationID == this.dianjiao3 || cuurModuleProcess.WorkStationID==this.manualSation2)
                {
                    restr = "当前返修流程代号A5，加工中心号" + cuurModuleProcess .WorkStationID+ ",需要加工！";
                    return true;
                }
                else
                {
                    restr = "当前返修流程代号A5，加工中心号" + cuurModuleProcess.WorkStationID + ",不需要加工！";
                    
                    return false;
                }
                //ModuleProcessModel ccdModuleProcess = bllModuleProcess.GetProcessByWorkStationID(this.ccdCheck);
                //if (ccdModuleProcess == null)
                //{
                //    restr = "CCDCheck中心号错误：" + this.ccdCheck + ",线体服务查无此工作中心号！";
                //    return false;
                //}
                //if (cuurModuleProcess.ModuleProcessOrder < ccdModuleProcess.ModuleProcessOrder)
                //{
                //    restr = "开始工位中心号错误：" + repairRecModel.RepairStartStationNum + ",线体服务查无此工作中心号！";
                //    return false;
                //}
                //return true;
            }
            else if (repairRecModel.RepairProcessNum == "A6")
            {
                if (repairRecModel.RepairStartStationNum != this.dianjiao4)
                {
                    restr = "返修MES判定在CCD检测，流程代码为A6，起始工位不为4号点胶机";
                    return NeedRepairByWorkstationOrder(repairRecModel, currlineStationID,ref restr);
                }
                return true;
            }
            else if (repairRecModel.RepairProcessNum == "A7")
            {
                if (repairRecModel.RepairStartStationNum != this.ccdCheck)
                {
                    restr = "返修MES判定在CCD检测，流程代码为A7，起始工位不为CCCheck";
                    return NeedRepairByWorkstationOrder(repairRecModel, currlineStationID,ref restr);
                }
                if(repairRecModel.RepairStartStationNum!=currlineStationID)
                {
                    restr = "流程代码为A7，当前工位不为CCDCheck，不需要加工！";
                    return false;
                }
                return true;
            }
            else
            {
                return true;
            }
        }
        public virtual bool CommitDB2(int index, short value, ref string restr)
        {
            this.nodeBase.db2Vals[index] = value;
            if (this.nodeBase.NodeDB2Commit(index, value, ref restr) == false)
            {
                restr = "写入PLC地址索引："+index+"失败！";
                this.nodeBase.LogRecorder.AddDebugLog(this.nodeBase.NodeName, restr);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 是否需要返修，只有AB线绑定工位是通过PLC地址判断其他工位只读取数据库判断
        /// </summary>
        /// <param name="moduleID">模块ID</param>
        /// <returns></returns>
        public virtual bool GetNeedRepairBLine(string rfid,string lineStationID, ref bool needRepair,ref string restr)
        {
            restr = "获取是否需要加工成功！";
            this.NeedRepair = needRepair;
            
            List<DBAccess.Model.BatteryModuleModel> modList = modBll.GetModelList(string.Format("palletID='{0}'   and palletBinded=1", rfid));
            if(modList==null||modList.Count==0)
            {
                restr = "RFID：" + rfid +"绑定数据为空！";
                needRepair = false;
                return true;
            }
            RepairRecordModel repairRecModel = bllRepairRecord.GetModel(modList[0].batModuleID);
            if (repairRecModel == null)//如果没有这个记录则不为返修流程
            {
                needRepair = true;
                this.nodeBase.LogRecorder.AddDebugLog(this.nodeBase.NodeName, "返修记录不在二维码：" + modList[0].batModuleID + "，不为返修流程，需要加工！");
                return true;
            }


            if(repairRecModel.IsMatchRepairProcess ==false)
            {
                needRepair = true;
                this.nodeBase.LogRecorder.AddDebugLog(this.nodeBase.NodeName, "流程不匹配，当前工位需要加工！");
                return true;
            }

            if (repairRecModel.RepairStartStationNum=="B1")
            {
                return ProcessB1(repairRecModel, lineStationID,ref needRepair, ref restr);
             
            }
            else if (repairRecModel.RepairStartStationNum == "B2")
            {
                return ProcessB2(repairRecModel, lineStationID, ref needRepair, ref restr);
            }
            else if (repairRecModel.RepairStartStationNum == "B3")
            {
                return ProcessB3(repairRecModel, lineStationID, ref needRepair, ref restr);
            }
            else if (repairRecModel.RepairStartStationNum == "B4")
            {
                return ProcessB4(repairRecModel, lineStationID, ref needRepair, ref restr);
            }
            else if (repairRecModel.RepairStartStationNum == "B5")
            {
                return ProcessB5(repairRecModel, lineStationID, ref needRepair, ref restr);
            }
            else if (repairRecModel.RepairStartStationNum == "B6")
            {
                return ProcessB6(repairRecModel, lineStationID, ref needRepair, ref restr);
            }
            else if (repairRecModel.RepairStartStationNum == "B7")
            {
                return ProcessB7(repairRecModel, lineStationID, ref needRepair, ref restr);
            }
            else if (repairRecModel.RepairStartStationNum == "B8")
            {
                return ProcessB8(repairRecModel, lineStationID, ref needRepair, ref restr);
            }
            else if (repairRecModel.RepairStartStationNum == "B9")
            {
                return ProcessB9(repairRecModel, lineStationID, ref needRepair, ref restr);
            }
            else if (repairRecModel.RepairStartStationNum == "B10")
            {
                return ProcessB10(repairRecModel, lineStationID, ref needRepair, ref restr);
            }
            else if (repairRecModel.RepairStartStationNum == "B11")
            {
                return ProcessB11(repairRecModel, lineStationID, ref needRepair, ref restr);
            }
            else if (repairRecModel.RepairStartStationNum == "B12")
            {
                return ProcessB12(repairRecModel, lineStationID, ref needRepair, ref restr);
            }
            else
            {
                restr = "流程不匹配，流程代号错误！";
                needRepair = true;
                this.NeedRepair = needRepair;
            
                return true;
            }
           
        }

      
        /// <summary>
        /// 是否需要返修，只有AB线绑定工位是通过PLC地址判断其他工位只读取数据库判断
        /// </summary>
        /// <param name="moduleID">模块ID</param>
        /// <returns></returns>
        public virtual bool GetNeedRepair(string rfid, string lineStationID, ref bool needRepair, ref string restr)
        {
            restr = "获取是否需要加工成功！";
            this.NeedRepair = needRepair;

            List<DBAccess.Model.BatteryModuleModel> modList = modBll.GetModelList(string.Format("palletID='{0}'   and palletBinded=1", rfid));
            if (modList == null || modList.Count == 0)
            {
                restr = "RFID：" + rfid + "绑定数据为空！";
                needRepair = false;
                return true;
            }
            RepairRecordModel repairRecModel = bllRepairRecord.GetModel(modList[0].batModuleID);
            if (repairRecModel == null)//如果没有这个记录则不为返修流程
            {
                needRepair = true;
                this.nodeBase.LogRecorder.AddDebugLog(this.nodeBase.NodeName, "返修记录不在二维码：" + modList[0].batModuleID + "，不为返修流程所有工位,需要加工！");
                return true;
            }


            if (repairRecModel.IsMatchRepairProcess == false)
            {

                needRepair = true;
                this.nodeBase.LogRecorder.AddDebugLog(this.nodeBase.NodeName, "流程不匹配，当前工位需要加工！");
                return true;
            }

            if (repairRecModel.RepairProcessNum == "A4" || repairRecModel.RepairProcessNum == "A5"
                || repairRecModel.RepairProcessNum == "A6" || repairRecModel.RepairProcessNum == "A7")//CCD检测单独判断
            {
                needRepair = CCDCheckNeedRepair(repairRecModel, lineStationID, ref restr);
                return true;
            }

            ModuleProcessModel moduleProcess = bllModuleProcess.GetProcessByWorkStationID(repairRecModel.RepairStartStationNum);
            if (moduleProcess == null)
            {
                restr = "开始工位中心号错误：" + repairRecModel.RepairStartStationNum + ",线体服务查无此工作中心号！";
                return false;
            }

            ModuleProcessModel cuurModuleProcess = bllModuleProcess.GetProcessByID(lineStationID);
            if (cuurModuleProcess == null)
            {
                restr = "开始工位中心号错误：" + lineStationID + ",线体服务查无此线体服务编号！";
                return false;
            }

            if (moduleProcess.ModuleProcessOrder <= cuurModuleProcess.ModuleProcessOrder)
            {
                needRepair = true;
                this.nodeBase.LogRecorder.AddDebugLog(this.nodeBase.NodeName, "当前工位在开始工位或者在开始工位后续流程中，此工位需要加工！");

                return true;
            }
            return true;
        }
   
        public virtual void RepairBusiness(ref bool businessComplete)
        {

        }
        protected virtual bool IsMatchProcess(RepairProcessParam repairParam)
        {
            List<DBAccess.Model.RepairProcessModel> repairProcessList = bllRepairProcess.GetProesses(repairParam.StartDevStation, repairParam.ProcessNum);
            if (repairProcessList != null && repairProcessList.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool GetRepairProcessParam(string rfid, ref RepairProcessParam repairProcessParam, ref string restr)
        {
            try
            {
             
                repairProcessParam = new RepairProcessParam();
                if (SysCfgModel.IsRequireRequireFromMes == false)
                {
                    repairProcessParam.StartDevStation = SysCfgModel.StartWorkStationID;
                    repairProcessParam.ProcessNum = SysCfgModel.ProcessID;
                    return true;
                }
                string M_WORKSTATION_SN = "";
                string M_DEVICE_SN = "";
                string M_UNION_SN = "";
                string M_CONTAINER_SN = "";
                string M_LEVEL = "";
                string M_ITEMVALUE = "";
                int flag = 16;
                RootObject rObj = new RootObject();

                List<DBAccess.Model.BatteryModuleModel> modelList = modBll.GetModelList(string.Format("palletID='{0}'  and palletBinded=1", rfid)); //modBll.GetModelByPalletID(this.rfidUID, this.nodeName);
                if (modelList == null || modelList.Count == 0)
                {
                    restr = "获取返修加工参数失败,工装板上的模块数量为空：" + rfid;
                    return false;
                }
                foreach (DBAccess.Model.BatteryModuleModel bat in modelList)
                {
                    string barcode = bat.batModuleID;
                    string strJson = "";

                    rObj = WShelper.DevDataUpload(flag, M_DEVICE_SN, M_WORKSTATION_SN, barcode, M_UNION_SN, M_CONTAINER_SN, M_LEVEL, M_ITEMVALUE, ref strJson, "");

                    restr = rObj.RES;

                    List<ContentDetail> content = rObj.M_COMENT; ;
                    if (content != null && content.Count > 0)
                    {
                        if (repairProcessParam.StartDevStation == "" || repairProcessParam.ProcessNum == "")
                        {
                            restr = "二维码：" + barcode + "请求起始工位及流程代号失败，返回起始工位号或者流程代码为空：" + restr;
                            return false;
                        }
                        repairProcessParam.StartDevStation = content[0].M_WORKSTATION_SN;
                        repairProcessParam.ProcessNum = content[0].M_ROUTE;

                        restr = "获取MES返修流程参数成功：开始工位编号:" + repairProcessParam.StartDevStation + ",流程参数：" + repairProcessParam.ProcessNum;
                    }
                    else
                    {
                        restr = "二维码：" + barcode + "请求起始工位及流程代号失败，MES返回ContentDetail数据为空：" + restr;
                        return false;
                    }


                }
                this.repairParam = repairProcessParam;
              
                return true;
            }
            catch (Exception ex)
            {
                restr = ex.ToString();
                return false;
            }

        }

        #region B线流程代码判断
        private bool ProcessB1(RepairRecordModel repairRecModel, string lineStationID, ref bool needRepair, ref string restr)
        {
           needRepair = NeedRepairByWorkstationOrder(repairRecModel,lineStationID,ref restr);
           return true;
        }

        private bool ProcessB2(RepairRecordModel repairRecModel, string lineStationID, ref bool needRepair, ref string restr)
        {
            ModuleProcessModel cuurModuleProcess = bllModuleProcess.GetProcessByID(lineStationID);
            if (cuurModuleProcess == null)
            {
                restr += "当前线体服务器工位编号错误：" + lineStationID + ",线体服务器查无此工位编号！";
                needRepair = false;
                return false;
            }

            ModuleProcessModel laserClean1Procss = bllModuleProcess.GetModel(this.laserClean1);
            if (laserClean1Procss == null)
            {
                restr += "工作中心号：" + laserClean1 + ",线体服务器查无此工作中心号！";
                
                return false;
            }
            if(cuurModuleProcess.ModuleProcessOrder<laserClean1Procss.ModuleProcessOrder)
            {
                needRepair = false;
                restr += "当前工作中心号：" + cuurModuleProcess.WorkStationID + "工艺顺序在1#激光清洗工艺前面，不需要加工！";
                
                return true;
            }

            if(cuurModuleProcess.WorkStationID== this.glueOutCheck1)
            {
                needRepair = false;
                restr += "当前工作中心号：" + cuurModuleProcess.WorkStationID + "为1号胶溢检测需要加工！";
                
                return true;
            }
            ModuleProcessModel aluminumProecss = bllModuleProcess.GetProcessByWorkStationID(this.weldAluminum1);
            if(aluminumProecss== null)
            {
                
                restr += "铝丝焊1#：" + weldAluminum1 + "，线体服务器不存在此工作中心号！";
                
                return false;
            }
            if (cuurModuleProcess.ModuleProcessOrder>aluminumProecss.ModuleProcessOrder)
            {
                restr += "当前工作中心：" + cuurModuleProcess.WorkStationID + "工艺顺序在铝丝焊1#工艺之后，需要加工！";
                
                needRepair =true;
                return true;
            }
        
            return true;
        }

        private bool ProcessB3(RepairRecordModel repairRecModel, string lineStationID, ref bool needRepair, ref string restr)
        {
            needRepair = NeedRepairByWorkstationOrder(repairRecModel, lineStationID, ref restr);
            return true;
        }

        private bool ProcessB4(RepairRecordModel repairRecModel, string lineStationID, ref bool needRepair, ref string restr)
        {
            needRepair = NeedRepairByWorkstationOrder(repairRecModel, lineStationID, ref restr);
            return true;
        }

        private bool ProcessB5(RepairRecordModel repairRecModel, string lineStationID, ref bool needRepair, ref string restr)
        {
            ModuleProcessModel cuurModuleProcess = bllModuleProcess.GetProcessByID(lineStationID);
            if (cuurModuleProcess == null)
            {
                restr += "当前线体服务器工位编号错误：" + lineStationID + ",线体服务器查无此工位编号！";
                return false;
            }

            ModuleProcessModel laserClean2Process = bllModuleProcess.GetModel(this.laserClean2);
            if(laserClean2Process == null)
            {
                restr += "激光清洗工作中心号错误：" + laserClean2 + ",线体服务器查无此工作中心号！";            
                return false;
            }

            if (cuurModuleProcess.ModuleProcessOrder<laserClean2Process.ModuleProcessOrder)
            {
                restr += "当前工作中心：" + cuurModuleProcess.WorkStationID + ",工艺顺序在激光清洗2#前面，不需要加工！";
                needRepair = false;
                return true;
            }
          
            if(cuurModuleProcess.WorkStationID == this.glueOutCheck2)
            {
                restr += "当前工作中心：" + cuurModuleProcess.WorkStationID + ",胶外溢2，不需要加工！";
                needRepair = false;
                return true;
            }
            ModuleProcessModel wled2Process = bllModuleProcess.GetModel(this.wedlAluminum2);
            if(wled2Process == null)
            {
                restr = "工作中心号错误：" + this.wedlAluminum2 +"，不存在此工作中中心号！";
                return false;
            }
             
            needRepair = NeedRepairByWorkstationOrder(repairRecModel, wled2Process.ModuleProcessID, ref restr);
            return true;
        }
        private bool ProcessB6(RepairRecordModel repairRecModel, string lineStationID, ref bool needRepair, ref string restr)
        {
            ModuleProcessModel wled2Process = bllModuleProcess.GetModel(this.wedlAluminum2);
            if (wled2Process == null)
            {
                restr = "工作中心号错误：" + this.wedlAluminum2 + "，不存在此工作中中心号！";
                return false;
            }
            ModuleProcessModel currModuleProcess = bllModuleProcess.GetProcessByID(lineStationID);
            if (wled2Process == null)
            {
                restr = "工位编号错误：" + lineStationID + "，不存在此工位编号！";
                return false;
            }
            if(currModuleProcess.ModuleProcessOrder<wled2Process.ModuleProcessOrder)
            {
                restr = "当前加工工作中心：" + currModuleProcess.WorkStationID + "，工艺顺序在铝丝焊2#工位前，不需要加工！";
                needRepair = false;
                return true;
            }

            needRepair = NeedRepairByWorkstationOrder(repairRecModel, wled2Process.ModuleProcessID, ref restr);
            return true;
           
        }
        private bool ProcessB7(RepairRecordModel repairRecModel, string lineStationID, ref bool needRepair, ref string restr)
        {
            ModuleProcessModel currProcess = bllModuleProcess.GetProcessByID(lineStationID);
            if (currProcess == null)
            {
                restr = "工作中心号错误：" + lineStationID + "，不存在此工作中中心号！";
                return false;
            }
            ModuleProcessModel laster1Process = bllModuleProcess.GetModel(this.laserClean1);
            if (laster1Process == null)
            {
                restr = "工作中心号错误：" + laster1Process.WorkStationID + "，不存在此工作中中心号！";
                return false;
            }
            if(currProcess.ModuleProcessOrder<laster1Process.ModuleProcessOrder)
            {
                restr += "当前工作中心：" + currProcess.WorkStationID + ",工艺顺序在激光清洗1#前面，不需要加工！";
              
                needRepair = false;
                return true;
            }
            if(currProcess.WorkStationID == this.glueOutCheck1||currProcess.WorkStationID==this.laserClean2
                ||currProcess.WorkStationID == this.glueOutCheck2||currProcess.WorkStationID==this.wedlAluminum2)
            {
                needRepair = false;
                return true;
            }

            needRepair = true;
           return true;
            //}
            //return true;
        }

        private bool BeforeWrokStationProcess(string currLineStationID, string workStationID, ref bool needRepair,ref string restr)
        {
            ModuleProcessModel currProcess = bllModuleProcess.GetProcessByID(currLineStationID);
            if (currProcess == null)
            {
                restr = "工作中心号错误：" + currLineStationID + "，不存在此工作中中心号！";
                return false;
            }
            ModuleProcessModel weldProcess = bllModuleProcess.GetModel(workStationID);
            if (weldProcess == null)
            {
                restr = "工作中心号错误：" + weldProcess.WorkStationID + "，不存在此工作中中心号！";
                return false;
            }
            if (currProcess.ModuleProcessOrder < weldProcess.ModuleProcessOrder)
            {
                restr += "当前工作中心：" + currProcess.WorkStationID + ",工艺顺序在"+weldProcess.ModuleProcessName+"前面，不需要加工！";

                needRepair = false;
                return true;
            }
            needRepair = false;
            return true;
        }
        private bool ProcessB8(RepairRecordModel repairRecModel, string lineStationID, ref bool needRepair, ref string restr)
        {
            ModuleProcessModel currProcess = bllModuleProcess.GetProcessByID(lineStationID);
            if (currProcess == null)
            {
                restr = "工作中心号错误：" + lineStationID + "，不存在此工作中中心号！";
                return false;
            }

            ModuleProcessModel weldProcess = bllModuleProcess.GetModel(this.weldAluminum1);
            if (weldProcess == null)
            {
                restr = "工作中心号错误：" + weldProcess.WorkStationID + "，不存在此工作中中心号！";
                return false;
            }
            if (currProcess.ModuleProcessOrder < weldProcess.ModuleProcessOrder)
            {
                restr += "当前工作中心：" + currProcess.WorkStationID + ",工艺顺序在铝丝焊1#前面，不需要加工！";

                needRepair = false;
                return true;
            }

            if (currProcess.WorkStationID == this.laserClean2|| currProcess.WorkStationID == this.glueOutCheck2 
                || currProcess.WorkStationID == this.wedlAluminum2)
            {
                needRepair = false;
                restr += "当前工作中心：" + currProcess.WorkStationID + ",不需要加工！";

                return true;
            }
            if (currProcess.WorkStationID == this.dcir)
            {
                restr += "当前工作中心：" + currProcess.WorkStationID + "DCIR工位,需要加工！";

                needRepair = true;
                return true;

            }

            needRepair = true;
            return true;

        }

        private bool ProcessB9(RepairRecordModel repairRecModel, string lineStationID, ref bool needRepair, ref string restr)
        {
            ModuleProcessModel currProcess = bllModuleProcess.GetProcessByID(lineStationID);
            if (currProcess == null)
            {
                restr = "工作中心号错误：" + lineStationID + "，不存在此工作中中心号！";
                return false;
            }
            ModuleProcessModel laserClean2Process = bllModuleProcess.GetModel(this.laserClean2);
            if (laserClean2Process == null)
            {
                restr = "工作中心号错误：" + laserClean2Process.WorkStationID + "，不存在此工作中中心号！";
                return false;
            }
            if (currProcess.ModuleProcessOrder < laserClean2Process.ModuleProcessOrder)
            {
                restr += "当前工作中心：" + currProcess.WorkStationID + ",工艺顺序在"+laserClean2Process.ModuleProcessName+"前面，不需要加工！";

                needRepair = false;
                return true;
            }
            if (currProcess.WorkStationID == this.glueOutCheck2 )
            {
                restr = "当前工作中心号：" + this.glueOutCheck2 + "，胶外溢2，此工作中心不需要加工！";
                needRepair = false;
                return true;
            }
             
             ModuleProcessModel weld2Process = bllModuleProcess.GetProcessByID(this.wedlAluminum2);
             if (weld2Process == null)
            {
                restr = "工作中心号错误：" + this.wedlAluminum2 + "，不存在此工作中中心号！";
                return false;
            }

             needRepair=  NeedRepairByWorkstationOrder(repairRecModel, weld2Process.ModuleProcessID, ref restr);
            
            return true;

        }
        private bool ProcessB10(RepairRecordModel repairRecModel, string lineStationID, ref bool needRepair, ref string restr)
        {
            ModuleProcessModel currProcess = bllModuleProcess.GetProcessByID(lineStationID);
            if (currProcess == null)
            {
                restr = "工作中心号错误：" + lineStationID + "，不存在此工作中中心号！";
                return false;
            }
            ModuleProcessModel weld2Process = bllModuleProcess.GetModel(this.wedlAluminum2);
            if (weld2Process == null)
            {
                restr = "工作中心号错误：" + weld2Process.WorkStationID + "，不存在此工作中中心号！";
                return false;
            }
            if (currProcess.ModuleProcessOrder < weld2Process.ModuleProcessOrder)
            {
                restr += "当前工作中心：" + currProcess.WorkStationID + ",工艺顺序在" + weld2Process.ModuleProcessName + "前面，不需要加工！";

                needRepair = false;
                return true;
            }
            if (currProcess.WorkStationID == this.glueOutCheck2)
            {
                needRepair = false;
                return true;
            }
           

            needRepair = NeedRepairByWorkstationOrder(repairRecModel, weld2Process.ModuleProcessID, ref restr);

            return true;

        }
        private bool ProcessB11(RepairRecordModel repairRecModel, string lineStationID, ref bool needRepair, ref string restr)
        {
            ModuleProcessModel currProcess = bllModuleProcess.GetProcessByID(lineStationID);
            if (currProcess == null)
            {
                restr = "工作中心号错误：" + lineStationID + "，不存在此工作中中心号！";
                return false;
            }

            if (currProcess.WorkStationID == this.dcir)
            {
                needRepair = true;
                return true;
            }
            needRepair = false;
            return true;
        }

        private bool ProcessB12(RepairRecordModel repairRecModel, string lineStationID, ref bool needRepair, ref string restr)
        {
            ModuleProcessModel currProcess = bllModuleProcess.GetProcessByID(lineStationID);
            if (currProcess == null)
            {
                restr = "工作中心号错误：" + lineStationID + "，不存在此工作中中心号！";
                return false;
            }
            ModuleProcessModel jueyuanScrew = bllModuleProcess.GetModel(this.cLineJYScrew);
            if (currProcess == null)
            {
                restr = "工作中心号错误：" + cLineJYScrew + "，不存在此工作中中心号！";
                return false;
            }
            if (currProcess.ModuleProcessOrder < jueyuanScrew.ModuleProcessOrder)
            {
                needRepair = false;
                return true;
            }
            needRepair = true;
            return true;
        }
     
        #endregion
    }

    public class RepairProcessParam
    {
        /// <summary>
        /// 起始加工工位
        /// </summary>
        public string StartDevStation{get;set;}
        /// <summary>
        /// 加工流程代号
        /// </summary>
        public string ProcessNum{get;set;}
    }
}
