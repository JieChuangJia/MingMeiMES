using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using PLProcessModel;
using FTDataAccess.BLL;
using FTDataAccess.Model;
using MTDBAccess.BLL;
using MTDBAccess.Model;

namespace LineNodes
{
    public class NodeSwitchBind:CtlNodeBaseModel
    {
        private PLNodesBll plNodeBll = new PLNodesBll();
        private MTDBAccess.BLL.dbBLL blldb = new MTDBAccess.BLL.dbBLL();//码头螺丝数据

        int modPalletMax = 4; //每个模组最多码放模块数量
        public override bool BuildCfg(System.Xml.Linq.XElement xe, ref string reStr)
        {
            if (!base.BuildCfg(xe, ref reStr))
            {
                return false;
            }
            this.dicCommuDataDB1[1].DataDescription = "1：正常分档,2：读条码失败,3：产品信息不存在";
            this.dicCommuDataDB1[2].DataDescription = "分档信息";
            this.dicCommuDataDB1[3].DataDescription = "待绑定模块数量";
            this.dicCommuDataDB1[4].DataDescription = "1：复位/待机状态,2：RFID读取成功,3：RFID读取失败";
            this.dicCommuDataDB1[5].DataDescription = "1：复位,2：数据绑定完成，允许放行,3: 模块数量为空";
            this.dicCommuDataDB2[1].DataDescription = "1：扫码请求(PLC>MES),2：扫码完成(MES->PLC)";
            this.dicCommuDataDB2[2].DataDescription = "1：无板,2：有板，读卡请求";
            this.dicCommuDataDB2[3].DataDescription = "1：复位,2：抓取完成，允许绑定";
            this.dicCommuDataDB2[4].DataDescription = "正在绑定的分档位(1~4)";
            return true;
        }
        public override bool ExeBusiness(ref string reStr)
        {
            modPalletMax = int.Parse(SysCfgModel.SysParamDic["C线电池包模块数量"].ParamVal);
            ExeSwitch();
            if(!ExeBind(ref reStr))
            {
                return false;
            }
            return true;
        }
        private void ExeSwitch()
        {
            
            string reStr="";
            if(db2Vals[0] == 0)
            {
                this.db1ValsToSnd[0] = 0;
                this.db1ValsToSnd[1] = 0;
            }
            if (db2Vals[0] != 1)
            {
                return;
            }
            //扫码请求
            string barcode = "";
            if (SysCfgModel.SimMode)
            {
                barcode = this.SimBarcode;
            }
            else
            {
                barcode = barcodeRW.ReadBarcode();
            }
            if (string.IsNullOrWhiteSpace(barcode))
            {
                if (this.db1ValsToSnd[0] != 2)
                {
                    logRecorder.AddDebugLog(nodeName, "读条码失败");
                }
                this.db1ValsToSnd[0] = 2;
                return;
            }
            DBAccess.Model.BatteryModuleModel mod = modBll.GetModel(barcode);
            if(mod == null)
            {
                if(this.db1ValsToSnd[0] != 3)
                {
                    LogRecorder.AddDebugLog(nodeName, string.Format("条码{0}，产品信息不存在",barcode));
                }
                this.db1ValsToSnd[0] = 3;
                return;
            }
            string pattern = @"^[0-9]*$"; //数字正则  
            if (!System.Text.RegularExpressions.Regex.IsMatch(mod.tag1, pattern))
            {
                if (this.db1ValsToSnd[1]!=0)
                {
                    logRecorder.AddDebugLog(nodeName, string.Format("分档字符串:{0}错误，要求为数字,", mod.tag1));
                }
                this.db1ValsToSnd[1] = 0;
                return;
            }
         //  if (this.db1ValsToSnd[1]==0)
            
            this.db1ValsToSnd[0] = 1;
            short groupSeq = short.Parse(mod.tag1);
            this.db1ValsToSnd[1] = groupSeq;
            PLNodesBll plNodesBll = new PLNodesBll();
            plNodeModel = plNodesBll.GetModel(this.nodeID);
            switch(groupSeq)
            {
                case 1:
                    {
                        string[] modExistArray = plNodeModel.tag1.Split(new string[]{ "," },StringSplitOptions.RemoveEmptyEntries);
                        if (modExistArray != null && modExistArray.Count() >= modPalletMax)
                        {
                            this.db1ValsToSnd[0] = 4;
                            this.currentTaskDescribe = string.Format("档位{0}已满,最大允许数量，目前数量{1}",groupSeq,modPalletMax);
                            return;
                        }
                        if(!plNodeModel.tag1.ToUpper().Contains(barcode.ToUpper()))
                        {
                            plNodeModel.tag1 = plNodeModel.tag1 + barcode + ",";
                            LogRecorder.AddDebugLog(nodeName, string.Format("模块：{0}分档完成,档位：{1}", barcode, mod.tag1));
                            
                        }
                        else
                        {
                            LogRecorder.AddDebugLog(nodeName, string.Format("模块：{0}分档缓存数据已经存在，分档完成,档位：{1}", barcode, mod.tag1));
                        }
                        break;
                    }
                case 2:
                    {
                        string[] modExistArray = plNodeModel.tag2.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        if (modExistArray != null && modExistArray.Count() >= modPalletMax)
                        {
                            this.db1ValsToSnd[0] = 4;
                            this.currentTaskDescribe = string.Format("档位{0}已满,最大允许数量，目前数量{1}", groupSeq, modPalletMax);
                            return;
                        }
                        if (!plNodeModel.tag2.ToUpper().Contains(barcode.ToUpper()))
                        {
                            plNodeModel.tag2 = plNodeModel.tag2 + barcode + ",";
                            LogRecorder.AddDebugLog(nodeName, string.Format("模块：{0}分档完成,档位：{1}", barcode, mod.tag1));
                        }
                        else
                        {
                            LogRecorder.AddDebugLog(nodeName, string.Format("模块：{0}分档缓存数据已经存在，分档完成,档位：{1}", barcode, mod.tag1));
                        }
                        break;
                    }
                case 3:
                    {
                        string[] modExistArray = plNodeModel.tag3.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        if (modExistArray != null && modExistArray.Count() >= modPalletMax)
                        {
                            this.db1ValsToSnd[0] = 4;
                            this.currentTaskDescribe = string.Format("档位{0}已满,最大允许数量，目前数量{1}", groupSeq, modPalletMax);
                            return;
                        }
                        if (!plNodeModel.tag3.ToUpper().Contains(barcode.ToUpper()))
                        {
                            plNodeModel.tag3 = plNodeModel.tag3 + barcode + ",";
                            LogRecorder.AddDebugLog(nodeName, string.Format("模块：{0}分档完成,档位：{1}", barcode, mod.tag1));
                        }
                        else
                        {
                            LogRecorder.AddDebugLog(nodeName, string.Format("模块：{0}分档缓存数据已经存在，分档完成,档位：{1}", barcode, mod.tag1));
                        }
                        break;
                    }
                case 4:
                    {
                        string[] modExistArray = plNodeModel.tag4.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        if (modExistArray != null && modExistArray.Count() >= modPalletMax)
                        {
                            this.db1ValsToSnd[0] = 4;
                            this.currentTaskDescribe = string.Format("档位{0}已满,最大允许数量，目前数量{1}", groupSeq, modPalletMax);
                            return;
                        }
                        if (!plNodeModel.tag4.ToUpper().Contains(barcode.ToUpper()))
                        {
                            plNodeModel.tag4 = plNodeModel.tag4 + barcode + ",";
                            LogRecorder.AddDebugLog(nodeName, string.Format("模块：{0}分档完成,档位：{1}", barcode, mod.tag1));
                        }
                        else
                        {
                            LogRecorder.AddDebugLog(nodeName, string.Format("模块：{0}分档缓存数据已经存在，分档完成,档位：{1}", barcode, mod.tag1));
                        }
                        break;
                    }
                default:
                    break;
            }
            plNodeBll.Update(plNodeModel);

            if(!NodeDB2Commit(0,2,ref reStr))
            {
                logRecorder.AddDebugLog(nodeName, "发送PLC数据失败");
            }
            //else
            //{
            //    Console.WriteLine("扫码完成信号写入2");
            //}

        }
        private bool ExeBind(ref string reStr)
        {
            if (!devStatusRestore)
            {
                devStatusRestore = DevStatusRestore();

            }
            if (!devStatusRestore)
            {
                return false;
            }
            if (this.rfidRWList != null && this.rfidRWList.Count() > 0)
            {
                this.rfidRW = this.rfidRWList[0];  //临时

            }
            if (db2Vals[1] == 1)
            {
                if (this.currentTask != null)
                {
                    //查询未执行完任务，清掉
                    if (!ctlTaskBll.ClearTask(string.Format("DeviceID='{0}'", this.nodeID)))
                    {
                        logRecorder.AddDebugLog(nodeName, "清理任务失败");
                        return false;
                    }
                    this.currentTask = null;


                    this.currentStat.Status = EnumNodeStatus.设备空闲;
                    this.currentStat.ProductBarcode = "";
                    this.currentStat.StatDescribe = "设备空闲";
                    checkFinished = false;
                    currentTaskDescribe = "等待有板信号";

                }
                currentTaskPhase = 0;
                db1ValsToSnd[3] = 1;
                db1ValsToSnd[4] = 1;
                this.rfidUID = string.Empty;
                //if (!SysCfgModel.SimMode)
                //{
                //    (this.rfidRW as DevAccess.RfidCF).ClearBufUID();
                //}
                this.currentStat.Status = EnumNodeStatus.设备空闲;
                this.currentStat.StatDescribe = "工位空闲";
                currentTaskDescribe = "";
                return true;
            }
            if (db2Vals[1] == 2)
            {
                if (currentTaskPhase == 0)
                {
                    db1ValsToSnd[3] = 1;
                    db1ValsToSnd[4] = 1;
                    currentTaskPhase = 1;
                    this.currentStat.Status = EnumNodeStatus.设备使用中;

                    this.currentStat.StatDescribe = "工作中";

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
                }
            }
            
          
            this.db1ValsToSnd[2] = (short)modPalletMax;
            switch(currentTaskPhase)
            {
                case 1:
                    {
                        currentTaskDescribe = "开始读RFID";
                        if (!RfidReadC())
                        {
                            break;
                        }
                        currentTaskPhase++;
                        this.currentTask.TaskParam = rfidUID;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        logRecorder.AddDebugLog(nodeName, string.Format("读到RFID:{0}，开始绑定", this.rfidUID));
                        break;
                    }
                case 2:
                    {
                        currentTaskDescribe = "等待抓取完成";
                        
                        if(this.db2Vals[2] != 2)
                        {
                            break;
                        }
                        if(this.db2Vals[3]<1 || this.db2Vals[3]>4)
                        {
                            break;
                        }
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 3:
                    {
                        short groupSeq = this.db2Vals[3];
                        plNodeModel = plNodeBll.GetModel(nodeID);
                        List<string> mods = new List<string>();
                        switch (groupSeq)
                        {
                            case 1:
                                {
                                    string[] strArray = plNodeModel.tag1.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    if (strArray != null && strArray.Count() > 0)
                                    {
                                        mods.AddRange(strArray);
                                    }

                                    break;
                                }
                            case 2:
                                {
                                    string[] strArray = plNodeModel.tag2.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    if (strArray != null && strArray.Count() > 0)
                                    {
                                        mods.AddRange(strArray);
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    string[] strArray = plNodeModel.tag3.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    if (strArray != null && strArray.Count() > 0)
                                    {
                                        mods.AddRange(strArray);
                                    }
                                    break;
                                }
                            case 4:
                                {
                                    string[] strArray = plNodeModel.tag4.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    if (strArray != null && strArray.Count() > 0)
                                    {
                                        mods.AddRange(strArray);
                                    }
                                    break;
                                }
                            default:
                                break;
                        }
                        if (mods.Count() < 1)
                        {
                            this.db1ValsToSnd[4] = 3;
                            this.currentTaskDescribe = string.Format("分档位：{0} 可供绑定的模块为空，", groupSeq);
                            break;
                        }
                        //绑定
                        TryUnbind(this.rfidUID, ref reStr);
                        for (int i = 0; i < Math.Min(mods.Count(), modPalletMax);i++ )
                        {
                            string modID = mods[i];
                            DBAccess.Model.BatteryModuleModel mod = modBll.GetModel(modID);
                            if (mod == null)
                            {
                                mod = new DBAccess.Model.BatteryModuleModel();
                                mod.batModuleID = modID;
                                mod.palletID = this.rfidUID;
                                mod.palletBinded = true;
                                mod.batPackID = "";
                                mod.curProcessStage = nodeName;
                                mod.asmTime = System.DateTime.Now;
                                mod.tag1 = this.db2Vals[3].ToString();
                                modBll.Add(mod);
                            }
                            else
                            {
                                mod.palletBinded = true;
                                mod.palletID = this.rfidUID;
                                modBll.Update(mod);
                            }
                          
                            this.currentTaskDescribe = string.Format("绑定，RFID:{0},模块条码:{1}", rfidUID, modID);
                            MTDBAccess.Model.dbModel mtModel = blldb.GetModel(modID);
                            string screwData = "";
                            if (mtModel != null)
                            {
                                screwData = ",测试时间:" + mtModel.测试时间.ToString() + ",反螺丝数据:" + mtModel.反螺丝数据.ToString() + "正螺丝数据:" + mtModel.正螺丝数据.ToString();
                            }
                            else
                            {
                                screwData = ",无螺丝数据";
                            }
                            logRecorder.AddDebugLog(nodeName, string.Format("绑定，RFID:{0},模块条码:{1}", rfidUID, modID) + screwData);
                            AddProcessRecord(mod.batModuleID, "模块", "追溯记录", string.Format("绑定，RFID:{0},模块条码:{1}", rfidUID, modID) + screwData, "");
                            //AddProcessRecord(mod.batModuleID, this.nodeName, "");
                           
                        }
                        int removeSum = Math.Min(mods.Count(), modPalletMax);
                        mods.RemoveRange(0, removeSum);
                       
                        string strMods = "";
                        for(int i=0;i<mods.Count();i++)
                        {
                            strMods += (mods[i] + ",");
                        }
                        switch (groupSeq)
                        {
                            case 1:
                                {
                                    plNodeModel.tag1 = strMods;
                                    break;
                                }
                            case 2:
                                {
                                    plNodeModel.tag2 = strMods;
                                    break;
                                }
                            case 3:
                                {
                                    plNodeModel.tag3 = strMods;
                                    break;
                                }
                            case 4:
                                {
                                    plNodeModel.tag4 = strMods;
                                    break;
                                }
                            default:
                                break;
                        }
                        plNodeBll.Update(plNodeModel);
                        this.db1ValsToSnd[4] = 2;
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                       
                    }
                case 4:
                    {
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
        protected override bool RfidReadC()
        {
            currentTaskDescribe = "开始读RFID";
            if (string.IsNullOrWhiteSpace(this.rfidUID))
            {
                if (!SysCfgModel.SimMode)
                {
                    if (this.rfidRW != null)
                    {
                        this.rfidUID = rfidRW.ReadUID();
                    }
                }
                else
                {
                    this.rfidUID = SimRfidUID;
                }
            }

            if (string.IsNullOrWhiteSpace(this.rfidUID))
            {

                if (this.db1ValsToSnd[3] != 3)
                {
                    //logRecorder.AddDebugLog(nodeName, "读RFID失败");
                    db1ValsToSnd[3] = 3;
                }
                else
                {
                    db1ValsToSnd[3] = 1;
                }
                Thread.Sleep(1000);

                ////读RFID失败 
                //if (this.db1ValsToSnd[3] != 3)
                //{
                //    logRecorder.AddDebugLog(nodeName, "读RFID失败");
                //}
                //db1ValsToSnd[3] = 3;
                this.currentStat.Status = EnumNodeStatus.无法识别;
                this.currentStat.StatDescribe = "读RFID失败";
                this.currentTaskDescribe = "读RFID失败";
                return false;
            }
            this.currentStat.Status = EnumNodeStatus.设备使用中;
            db1ValsToSnd[3] = 2;//读到RFID
            this.currentStat.StatDescribe = "RFID识别完成";
            this.currentTaskDescribe = "RFID识别完成:"+this.rfidUID;
            logRecorder.AddDebugLog(nodeName, string.Format("读到RFID:{0}", this.rfidUID));
            return true;
        }
    }
}
