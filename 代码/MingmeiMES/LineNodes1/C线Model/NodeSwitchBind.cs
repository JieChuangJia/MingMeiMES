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
        private DBAccess.BLL.QRCodeBLL bllQrCode = new DBAccess.BLL.QRCodeBLL();
        private List<string> qrList = new List<string>();//第二个扫码枪扫码列表
        private string mesReqGroupCode = "";//向mes申请的模组条码
        private string swithBarcode = "";
        private int readCodeTimes = 0;//扫码器读3次不成工认为失败，需要通知plc
        int modPalletMax = 4; //每个模组最多码放模块数量
        private int bindCountGlobal = 0;

        private int SwitchTaskIndex = 0;//分档索引
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
            this.dicCommuDataDB2[5].DataDescription = "1：2号扫码请求(PLC>MES),2：2号扫码完成(MES->PLC)3：扫码失败！";
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

        private bool UploadMesLogic(int swichDw,string barcode,ref string reStr)
        {
            PLNodesBll plNodesBll = new PLNodesBll();
            plNodeModel = plNodesBll.GetModel(this.nodeID);
            string swichDwAll = "";
            switch(swichDw)
            {
                case 1:
                    {
                        swichDwAll = plNodeModel.tag1.ToUpper();
                        break;
                    }
                case 2:
                    {
                        swichDwAll = plNodeModel.tag2.ToUpper();
                        break;
                    }
                case 3:
                    {
                        swichDwAll = plNodeModel.tag3.ToUpper();
                        break;
                    }
                case 4:
                    {
                        swichDwAll = plNodeModel.tag4.ToUpper();
                        break;
                    }
                default:
                    break;

            }

            //在分档之前要判断打螺丝的码头数据上传MES是否成功
            if (!swichDwAll.ToUpper().Contains(barcode.ToUpper()))//没有分档的
            {
                Console.WriteLine(this.nodeName + "debug9");
                int uploadStatus = UploadMesScrewData(this.rfidUID, barcode, ref reStr);
                if (uploadStatus == 0)//上报码头数据
                {
                    this.logRecorder.AddDebugLog(this.nodeName, "上传打螺丝数据成功！" + reStr);
                    
                }
                else if (uploadStatus == 1)
                {
                    this.logRecorder.AddDebugLog(this.nodeName, "上传打螺丝数据成功！，返回NG" + reStr);
                }
                else
                {
                    Console.WriteLine(this.nodeName + "，上传打螺丝数据失败：" + reStr);
                    return false;
                }

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
                this.swithBarcode = barcode;
            }
            else
            {
                
                barcode = barcodeRW.ReadBarcode();
                this.swithBarcode = barcode;
                
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
            else
            {
                Console.WriteLine(this.nodeName+ "读取条码成功：" + barcode);
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
            //分档信息不为数字为字符串，目前测试AAA为1档位
            //string pattern = @"^[0-9]*$"; //数字正则  
            //if (!System.Text.RegularExpressions.Regex.IsMatch(mod.tag1, pattern))
            //{
            //    if (this.db1ValsToSnd[1]!=0)
            //    {
            //        logRecorder.AddDebugLog(nodeName, string.Format("分档字符串:{0}错误，要求为数字,", mod.tag1));
            //    }
            //    this.db1ValsToSnd[1] = 0;
            //    return;
            //}

            //PLNodesBll plNodesBll = new PLNodesBll();
            //plNodeModel = plNodesBll.GetModel(this.nodeID);
         
           

            short groupSeq =0;
            if(short.TryParse(mod.tag5, out groupSeq)==false)
            {
                Console.WriteLine(this.nodeName +"分档信息有误：" +mod.tag5 +",无法转换为数字！");
                return;
            }
            //short groupSeq = 1;//测试默认为1档位
         

            if (UploadMesLogic(groupSeq,barcode,ref reStr) == false)
            {
                return;
            }
          
            this.db1ValsToSnd[0] = 1;
          
            this.db1ValsToSnd[1] = groupSeq;
            Console.WriteLine(this.nodeName, 11);
          
            switch (groupSeq)
            {
                case 1:
                    {
                        string[] modExistArray = plNodeModel.tag1.Split(new string[]{ "," },StringSplitOptions.RemoveEmptyEntries);
                        if (modExistArray != null && modExistArray.Count() > modPalletMax)
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
                        if (modExistArray != null && modExistArray.Count() > modPalletMax)
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
                        if (modExistArray != null && modExistArray.Count() > modPalletMax)
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
                        if (modExistArray != null && modExistArray.Count() > modPalletMax)
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
            //int bindCount = modPalletMax;
           
            //this.db1ValsToSnd[2] = (short)modPalletMax;
            //this.db1ValsToSnd[2] = (short)bindCount;
            switch(currentTaskPhase)
            {
                case 1:
                    {
                        currentTaskDescribe = "开始读RFID";
                        if (!RfidReadC())
                        {
                            break;
                        }
                       
                        if(SysCfgModel.SimMode== false)
                        {
                            if (ModuleCodeRequire(ref this.mesReqGroupCode, ref reStr) == false)
                            {
                                Console.WriteLine(this.nodeName + "请求模组二维码失败!" );
                                break;
                            }
                            else
                            {
                                this.logRecorder.AddDebugLog(this.nodeName, "请求模组二维码成功!" + this.mesReqGroupCode);
                            }
                           
                            //bindCount = 2;//现场条码不正确
                            //mesReqGroupCode = "123456789202345678988043";
                        }
                        else
                        {
                            //bindCountGlobal = 4;
                            mesReqGroupCode = "04GPE3VB150E118530000002";              
                        }
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 2:
                    {
                        //int bindCount = 0;
                        if(this.swithBarcode=="")
                        {
                            currentTaskDescribe = "模块二位码没有扫码！";
                            break;
                        }
                        currentTaskDescribe = "计算工装板绑定模块数量！";
                        if (SysCfgModel.SimMode == false)
                        {
                            if (CalcuBindCount(mesReqGroupCode, this.swithBarcode, ref this.bindCountGlobal, ref reStr) == false)
                            {
                                currentTaskDescribe = "带绑定模块数量计算错误！" + reStr;
                                return false;
                            }
                           
                        }
                        else
                        {
                           
                            bindCountGlobal = 2;
                        }
                        this.swithBarcode = "";
                        this.db1ValsToSnd[2] = (short)bindCountGlobal;
                        //this.db1ValsToSnd[2] = (short)modPalletMax;//测试版本
                        this.plcRW2.WriteDB("D8500", (short)bindCountGlobal);
                        currentTaskPhase++;
                        this.currentTask.TaskParam = rfidUID;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        logRecorder.AddDebugLog(nodeName, string.Format("读到RFID:{0}，开始绑定", this.rfidUID));

                       
                        break;
                    }
                case 3:
                    {
                        #region 原来绑定流程
                        //short groupSeq = this.db2Vals[3];
                        //plNodeModel = plNodeBll.GetModel(nodeID);
                        //List<string> mods = new List<string>();
                        //switch (groupSeq)
                        //{
                        //    case 1:
                        //        {
                        //            string[] strArray = plNodeModel.tag1.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        //            if (strArray != null && strArray.Count() > 0)
                        //            {
                        //                mods.AddRange(strArray);
                        //            }

                        //            break;
                        //        }
                        //    case 2:
                        //        {
                        //            string[] strArray = plNodeModel.tag2.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        //            if (strArray != null && strArray.Count() > 0)
                        //            {
                        //                mods.AddRange(strArray);
                        //            }
                        //            break;
                        //        }
                        //    case 3:
                        //        {
                        //            string[] strArray = plNodeModel.tag3.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        //            if (strArray != null && strArray.Count() > 0)
                        //            {
                        //                mods.AddRange(strArray);
                        //            }
                        //            break;
                        //        }
                        //    case 4:
                        //        {
                        //            string[] strArray = plNodeModel.tag4.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        //            if (strArray != null && strArray.Count() > 0)
                        //            {
                        //                mods.AddRange(strArray);
                        //            }
                        //            break;
                        //        }
                        //    default:
                        //        break;
                        //}
                        #endregion
                        currentTaskDescribe = "扫码中，计划扫码个数:" + bindCountGlobal + ",当前扫码计数：第" + this.qrList.Count() + "个！";
                        int rqRequire = 0;
                        if (this.plcRW2.ReadDB("D3004", ref rqRequire) == false)//有第二个扫码请求
                        {
                            break;
                        }
                        NodeDB2Commit(4, (short)rqRequire,ref reStr);
                        if (rqRequire != 1)//有第二个扫码请求
                        {
                            break;
                        }
                        //扫码请求
                        string barcode = "";
                        if (SysCfgModel.SimMode)
                        {
                            barcode = this.SimBarcode;
                        }
                        else
                        {
                            barcode = barcodeRW2.ReadBarcode();
                        }
                        if(string.IsNullOrWhiteSpace(barcode) == true)
                        {
                            this.readCodeTimes++;
                            if(this.readCodeTimes>4)
                            {
                                if (this.plcRW2.WriteDB("D3004", 3) == false)
                                {
                                    break;
                                }
                                NodeDB2Commit(4,3, ref reStr);
                                this.readCodeTimes = 0;
                            }
                            break;
                        }

                        if (this.plcRW2.WriteDB("D3004", 2) == false)
                        {
                            break;
                        }
                        NodeDB2Commit(4,2, ref reStr);
                        this.qrList.Add(barcode);

                        if (this.qrList.Count != bindCountGlobal)
                        {
                            break;
                        }
                        if (this.qrList.Count() < 1)
                        {
                            this.db1ValsToSnd[4] = 3;
                            //this.currentTaskDescribe = string.Format("分档位：{0} 可供绑定的模块为空，", groupSeq);
                            break;
                        }
                        this.currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                       
                    }
                case 4:
                    {
                        currentTaskDescribe = "绑定中！";
                        //绑定
                        //TryUnbind(this.rfidUID, ref reStr);

                        AddPack(this.mesReqGroupCode, ref reStr);

                        for (int i = 0; i < Math.Min(this.qrList.Count(), modPalletMax); i++)
                        {
                            string modID = this.qrList[i];
                            DBAccess.Model.BatteryModuleModel mod = modBll.GetModel(modID);
                            if (mod == null)
                            {
                                mod = new DBAccess.Model.BatteryModuleModel();
                                mod.batModuleID = modID;
                                mod.palletID = this.rfidUID;
                                mod.palletBinded = true;
                                mod.batPackID = this.mesReqGroupCode;
                                mod.curProcessStage = nodeName;
                                mod.asmTime = System.DateTime.Now;
                                mod.tag1 = this.db2Vals[3].ToString();
                                modBll.Add(mod);
                            }
                            else
                            {
                                mod.palletBinded = true;
                                mod.palletID = this.rfidUID;
                                mod.curProcessStage = nodeName;
                                mod.batPackID = this.mesReqGroupCode;
                                modBll.Update(mod);
                            }

                            this.currentTaskDescribe = string.Format("绑定，RFID:{0},模块条码:{1}", rfidUID, modID);
                           
                            logRecorder.AddDebugLog(nodeName, string.Format("绑定，RFID:{0},模块条码:{1}", rfidUID, modID));
                            AddProcessRecord(mod.batModuleID, "模块", "追溯记录", string.Format("绑定，RFID:{0},模块条码:{1}", rfidUID, modID), "");
                            //AddProcessRecord(mod.batModuleID, this.nodeName, "");

                        }
                        int removeSum = Math.Min(this.qrList.Count(), modPalletMax);
                        this.qrList.RemoveRange(0, removeSum);

                        //string strMods = "";
                        //for (int i = 0; i < this.qrList.Count(); i++)
                        //{
                        //    strMods += (this.qrList[i] + ",");
                        //}
                        //switch (groupSeq)
                        //{
                        //    case 1:
                        //        {
                        //            plNodeModel.tag1 = strMods;
                        //            break;
                        //        }
                        //    case 2:
                        //        {
                        //            plNodeModel.tag2 = strMods;
                        //            break;
                        //        }
                        //    case 3:
                        //        {
                        //            plNodeModel.tag3 = strMods;
                        //            break;
                        //        }
                        //    case 4:
                        //        {
                        //            plNodeModel.tag4 = strMods;
                        //            break;
                        //        }
                        //    default:
                        //        break;
                        //}
                        //plNodeBll.Update(plNodeModel);


                        if (this.db2Vals[2] != 2)
                        {
                            break;
                        }
                       
                        if (this.db2Vals[3] < 1 || this.db2Vals[3] > 4)
                        {
                            break;
                        }
                     
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        currentTaskDescribe = "绑定完成！";
                        break;
                    }
                case 5:
                    {
                        #region 上传MES绑定数据
                        currentTaskDescribe = "开始上传MES绑定数据!";
                        string restr = "";
                        int uploadStatus = UploadDataToMes(2, "M00100101", this.rfidUID, ref restr);
                        if (uploadStatus == 0)//OK
                        {
                            this.logRecorder.AddDebugLog(this.nodeName, "上传MES数据成功：" + restr);
                        }
                        else if (uploadStatus == 1)//NG
                        {
                            this.logRecorder.AddDebugLog(this.nodeName, "上传MES数据成功，返回NG：" + restr);
                        }
                        else
                        {
                            this.logRecorder.AddDebugLog(this.nodeName, "上传MES数据失败：" + restr);//需要重复上传
                            break;
                        }
                        #endregion
                        currentTaskPhase++;
                        this.db1ValsToSnd[4] = 2;


                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);

                        this.plNodeModel.tag1 = "";
                        this.plNodeModel.tag2 = "";
                        this.plNodeModel.tag3 = "";
                        this.plNodeModel.tag4 = "";
                        this.plNodeBll.Update(this.plNodeModel);
                        break;
                    }
                case 6:
                    {
                        currentTaskDescribe = "流程完成";
                        this.db1ValsToSnd[2] = 0;
                        this.mesReqGroupCode = "";
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

        private bool CalcuBindCount(string moduleGroupCode, string moduleCode, ref int bindCount, ref string restr)
        {
          
            int m_clNum = 0;
            int y_blNum = 0;
            int n_clNum = 0;
            int x_blNum = 0;
            if (AnalysisQRCode(moduleGroupCode, ref n_clNum, ref y_blNum, ref restr) == false)
            {
              
                this.logRecorder.AddDebugLog(this.nodeName, "模组二位码解析失败" );
                return false;
            }
           
            //List<DBAccess.Model.BatteryModuleModel> moduleList = modBll.GetBindedMods(palletID);
            //if (moduleList == null || moduleList.Count==0)
            //{
            //    currentTaskDescribe = "此RFID没有绑定数据！";
            //    return false;
            //}
            if (AnalysisQRCode(moduleCode, ref m_clNum, ref x_blNum, ref restr) == false)
            {
                currentTaskDescribe = "模块二位码解析失败！";
                return false;
            }
            if(n_clNum%m_clNum!=0)
            {
                restr = "工装板绑定个数计算错误：串联数不能整除，n/m不为整数！";
                return false;
            }
            if(y_blNum%x_blNum!=0)
            {
                restr = "工装板绑定个数计算错误：并联数不能整除，y/x不为整数！";
                return false;
            }
            bindCount = (n_clNum / m_clNum) * (y_blNum / x_blNum);
            return true;
        }

        /// 模组条码请求
        /// </summary>
        /// <returns></returns>
        private bool ModuleCodeRequire(ref string M_SN, ref string reStr)
        {
            if (SysCfgModel.MesOfflineMode == false)
            {
                string M_WORKSTATION_SN = "M00100101";
                RootObject rObj = new RootObject();
                rObj = WShelper.BarCodeRequest(M_WORKSTATION_SN);
                if (rObj.RES.Contains("OK"))
                {
                    M_SN = rObj.M_COMENT[0].M_SN;
                    reStr = this.nodeName + "模组条码请求成功";
                    return true;
                }
                else
                {
                    M_SN = "";
                    reStr = this.nodeName + "模组条码请求失败!" + rObj.RES;
                    return false;
                }
            }
            else
            {
                //DBAccess.Model.QRCodeModelModel qrCode = bllQrCode.RequireQrCode();
                //if(qrCode == null)
                //{
                //    reStr = this.nodeName + "当前系统中已经没有可申请的模组二维码!";
                //    return false;
                //}
                //M_SN = qrCode.QRCode;

                M_SN = "123456789202345678988043";//4个
            }
            return true;
      
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="M_WORKSTATION_SN">工作中心号码</param>
        /// <param name="rfid">二维码</param>
        /// <returns>0,表示ok，只传1次，1代码NG也只传一次，2表示其他错误一直上传</returns>
        private int UploadDataToMes(int flag, string workStaionSn, string rfid,ref string restr)
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
                restr = "RFID："+rfid +"无数据绑定";
                return 2;
            }
          
            foreach (DBAccess.Model.BatteryModuleModel battery in modelList)
            {
                M_UNION_SN += battery.batModuleID + "|";
            }
            string barcode = modelList[0].batPackID;
            string strJson = "";

            rObj = DevDataUpload(flag, M_DEVICE_SN, M_WORKSTATION_SN, barcode, M_UNION_SN, rfid, M_LEVEL, M_ITEMVALUE, ref strJson);
            restr = rObj.RES + ":上传数据：" + M_UNION_SN;
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

        public int UploadMesScrewData(string rfid, string moduCode, ref string reStr)
        {
            try
            {
                string M_AREA = "Y001";
                string M_WORKSTATION_SN = "Y00102201";
                string M_DEVICE_SN = "";

                string M_UNION_SN = "";
                string M_CONTAINER_SN = "";
                string M_LEVEL = "";
                string M_ITEMVALUE = "";
                RootObject rObj = new RootObject();

                DBAccess.Model.BatteryModuleModel battery = modBll.GetModel(moduCode);
                if (battery == null)
                {
                    reStr = "没有：" + moduCode + "模块";
                    return 2;
                }

                MTDBAccess.Model.dbModel screwModel = blldb.GetModel(moduCode);//获取码头数据
                if (screwModel == null)
                {

                    reStr = this.nodeName + "获取码头拧螺丝数据失败！";
                    return 2;
                }
                //M_LEVEL = battery.tag1;
                string barcode = battery.batModuleID;
                M_ITEMVALUE = "正螺丝1扭矩:"+screwModel.正螺丝1马头扭矩+":Nm|"+ "反螺丝1扭矩:"+screwModel.反螺丝1马头扭矩
                    + ":Nm|正螺丝2扭矩:" + screwModel.正螺丝2马头扭矩 + ":Nm|" + "反螺丝2扭矩:" + screwModel.反螺丝2马头扭矩
                    + ":Nm|正螺丝1角度:" + screwModel.正螺丝1马头角度 + ":°|反螺丝1角度:" + screwModel.反螺丝1马头角度
                    + ":°|正螺丝2角度:" + screwModel.正螺丝2马头角度 + ":°|反螺丝2角度:" + screwModel.反螺丝2马头角度 + ":°";//需要拼接螺丝数据
                string strJson = "";

                rObj = DevDataUpload(3, M_DEVICE_SN, M_WORKSTATION_SN, barcode, M_UNION_SN, M_CONTAINER_SN, M_LEVEL, M_ITEMVALUE, ref strJson);
                reStr = rObj.RES;

                if (rObj.RES.ToUpper().Contains("OK"))
                {
                    reStr += M_ITEMVALUE;
                    //logRecorder.AddDebugLog(nodeName, string.Format("上传螺丝数据失败：{0}", M_ITEMVALUE) + "-" + rObj.RES);
                    return 0;
                }
                else if(rObj.RES.ToUpper().Contains("NG"))
                {
                    return 1;
                }

                else
                {
                    logRecorder.AddDebugLog(nodeName, string.Format("上传螺丝数据失败：{0}", M_ITEMVALUE + "-" + rObj.RES));

                    return 2;
                }
            }
            catch(Exception ex)
            {
                reStr += ex.Message;
                return 2;
            }
          
        }
    }
}
