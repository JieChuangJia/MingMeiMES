using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PLProcessModel;
namespace LineNodes
{
    /// <summary>
    /// 锁螺丝，待修改（C线）
    /// </summary>
    public class NodeScrewLock: CtlNodeBaseModel
    {
        protected System.DateTime devOpenSt = DateTime.Now;
        //protected string ccdDevName = "A线锁螺丝机";
         public override bool BuildCfg(System.Xml.Linq.XElement xe, ref string reStr)
        {
            if (!base.BuildCfg(xe, ref reStr))
            {
                return false;
            }
            if(this.nodeID == "OPA005")
            {
                this.dicCommuDataDB1[1].DataDescription = "A通道读卡结果，1：复位/待机状态,2：RFID读取成功,3：RFID读取失败,4：无绑定数据";
                this.dicCommuDataDB1[2].DataDescription = "B通道读卡结果,1：复位/待机状态,2：RFID读取成功,3：RFID读取失败,4：无绑定数据";
                this.dicCommuDataDB1[3].DataDescription = "A通道,1：复位/待机状态,2：数据读取中,3：数据读取完毕，放行";
                this.dicCommuDataDB1[4].DataDescription = "B通道,1：复位/待机状态,2：数据读取中,3：数据读取完毕，放行";
                this.dicCommuDataDB2[1].DataDescription = "0：无,1：正在运行A通道,2：正在运行B通道";
                this.dicCommuDataDB2[2].DataDescription = "1：A通道无板,2：A通道有板，读卡请求";
                this.dicCommuDataDB2[3].DataDescription = "1：B通道无板,2：B通道有板，读卡请求";
            }
            else if(this.nodeID == "OPC006")
            {
                this.dicCommuDataDB1[1].DataDescription = "工位读卡结果，1：复位/待机状态,2：RFID读取成功,3：RFID读取失败,4：无绑定数据";
                this.dicCommuDataDB1[2].DataDescription = "工位状态,1：复位/待机状态,2：数据读取中,3：数据读取完毕，放行";
                this.dicCommuDataDB2[1].DataDescription = "1：通道无板,2：通道有板，读卡请求";
            }
           
            return true;
        }
        public override bool ExeBusiness(ref string reStr)
        {
            if (this.nodeID == "OPA005")
            {
                return ExeBindA(ref reStr);
            }
            else if (this.nodeID == "OPC006")
            {
                return ExeBindC(ref reStr);
            }
            return true;
        }


        protected bool ExeBindA(ref string reStr)
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
                        db1ValsToSnd[1] = 2;//
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
                        if (modList.Count() < 1)
                        {
                            currentTaskPhase = 6;
                            break;
                        }
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 3:
                    {
                        //发送启动加工命令
                        //int channelIndex = 1;
                        //if (this.db2Vals[0] == 1 || this.db2Vals[0] == 2)
                        //{
                        //    channelIndex = this.db2Vals[0];
                        //}
                        //else
                        //{
                        //    this.currentTaskDescribe = "通道号无效！";
                        //    break;
                        //}
                        List<DBAccess.Model.BatteryModuleModel> modList = modBll.GetModelList(string.Format("palletID='{0}' and palletBinded=1", this.rfidUID));
                        if (modList.Count() < 1)
                        {
                            this.db1ValsToSnd[channelIndex - 1] = 4;
                            currentTaskPhase = 5;
                            break;
                        }
                        List<string> products = new List<string>();
                        foreach (DBAccess.Model.BatteryModuleModel m in modList)
                        {
                            products.Add(m.batModuleID);
                        }
                        if (products.Count() > 0)
                        {
                            if (ccdDevAcc != null)
                            {
                                if (!ccdDevAcc.StartDev(products, ccdDevName, ref reStr))
                                {
                                    this.currentTaskDescribe = "发送设备加工启动命令失败";
                                    //Console.WriteLine(string.Format("{0}发送设备加工启动命令失败,{1}", nodeName, reStr));
                                    break;
                                }
                                else
                                {
                                    logRecorder.AddDebugLog(nodeName, "发送设备加工启动命令成功");
                                }
                            }
                        }
                        devOpenSt = System.DateTime.Now;
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 4:
                    {

                        System.DateTime cur = System.DateTime.Now;
                        TimeSpan ts = cur - devOpenSt;
                        if (ts.TotalSeconds < 30)
                        {
                            break;
                        }
                        IDictionary<string, string> ccdDataDic = ccdDevAcc.GetData(ccdDevName, ref reStr);
                        if (ccdDataDic == null || ccdDataDic.Keys.Count() < 1)
                        {
                            //Console.WriteLine(string.Format("{0}获取CCD{1}数据失败,{2}",nodeName,ccdDevName,reStr));
                            this.currentTaskDescribe = string.Format("获取CCD{0}数据失败,{1}", ccdDevName, reStr);
                            break;
                        }
                        foreach (string keyStr in ccdDataDic.Keys)
                        {
                            string str = string.Format("CCD数据，产品ID:{0}，数据：{1}", keyStr, ccdDataDic[keyStr]);
                            logRecorder.AddDebugLog(nodeName, str);
                            AddProcessRecord(keyStr, "模块", "检测数据", string.Format("读取到{0}检测数据", ccdDevName), ccdDataDic[keyStr]);
                        }
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 5:
                    {
                        //发送停止加工命令
                        if (!ccdDevAcc.EndDev(this.ccdDevName, ref reStr))
                        {
                            //logRecorder.AddDebugLog(nodeName, "发送设备停止命令失败");
                            //  Console.WriteLine(nodeName + "发送设备停止命令失败");
                            this.currentTaskDescribe = "发送设备停止命令失败";
                            break;
                        }
                        else
                        {
                            logRecorder.AddDebugLog(nodeName, "发送设备停止命令成功");
                        }
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 6:
                    {
                        currentTaskDescribe = "发送有料字";
                        List<DBAccess.Model.BatteryModuleModel> modList = modBll.GetModelList(string.Format("palletID='{0}' and palletBinded=1", this.rfidUID));

                        if (!AfterMech(modList, ref reStr))
                        {
                            Console.WriteLine(string.Format("{0},{1}", nodeName, reStr));
                            break;
                        }
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        logRecorder.AddDebugLog(nodeName, "发送有料字");
                        break;
                    }
                case 7:
                    {
                        db1ValsToSnd[2 + this.channelIndex - 1] = 3;
                        currentStat.StatDescribe = "流程完成";
                       
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                default:
                    break;
            }
            return true;
           
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
                        //   db1ValsToSnd[1] = 2;//
                        if (!ProductTraceRecord())
                        {
                            break;
                        }
                        currentTaskPhase++;
                        this.currentTask.TaskParam = rfidUID;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 2:
                    {
                        List<DBAccess.Model.BatteryModuleModel> modList = modBll.GetModelList(string.Format("palletID='{0}' and palletBinded=1", this.rfidUID));
                        if (modList.Count() < 1)
                        {
                            currentTaskPhase = 3;
                            break;
                        }

                        if (!PreMechB(modList, ref reStr))
                        {
                            Console.WriteLine(string.Format("{0},{1}", nodeName, reStr));
                            break;
                        }                        
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 3:
                    {
                        db1ValsToSnd[1] = 3;
                        currentStat.StatDescribe = "流程完成";
                        currentTaskDescribe = "流程完成";
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
