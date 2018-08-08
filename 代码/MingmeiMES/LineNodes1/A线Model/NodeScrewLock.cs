using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PLProcessModel;
using DevInterface;
using System.Threading;
using FTDataAccess.BLL;
using LogInterface;
namespace LineNodes
{
    /// <summary>
    /// 锁螺丝，待修改（C线）
    /// </summary>
    public class NodeScrewLock: CtlNodeBaseModel
    {
        protected System.DateTime devOpenSt = DateTime.Now;
        protected DBAccess.BLL.BatteryModuleBll modBll = new DBAccess.BLL.BatteryModuleBll();
        private MTDBAccess.BLL.dbBLL blldb = new MTDBAccess.BLL.dbBLL();//码头螺丝数据
        private ALineScrewDB.BLL.dbBLL bllScrewDb = new ALineScrewDB.BLL.dbBLL();//A线锁螺丝数据库

        private ScrewNGHandler screwNgHandler = null;
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
                        //db1ValsToSnd[1] = 2;//

                      
                        if (!ProductTraceRecord())
                        {
                            break;
                        }
                        currentTaskPhase++;
                        this.screwNgHandler = new ScrewNGHandler(this.plcRW, this.plcRW2, this.channelIndex,this.logRecorder);
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
                        Thread.Sleep(2000);//延时10秒等待下边设备
                        foreach (DBAccess.Model.BatteryModuleModel m in modList)
                        {
                            products.Add(m.batModuleID);
                        }
                        if (!SysCfgModel.SimMode)
                        { 
                        
                            if (products.Count() > 0)
                            {
                                if (ccdDevAcc != null)
                                {
                                    if (!ccdDevAcc.StartDev(products, ccdDevName, ref reStr))
                                    {
                                        this.currentTaskDescribe = "发送设备加工启动命令失败:" + reStr;
                                        //Console.WriteLine(string.Format("{0}发送设备加工启动命令失败,{1}", nodeName, reStr));
                                        break;
                                    }
                                    else
                                    {
                                        logRecorder.AddDebugLog(nodeName, "发送设备加工启动命令成功");
                                    }
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
                        #region 原来上传MES逻辑
                        //System.DateTime cur = System.DateTime.Now;
                        //TimeSpan ts = cur - devOpenSt;
                        //if (ts.TotalSeconds < 30)
                        //{
                        //    break;
                        //}
                        //IDictionary<string, string> ccdDataDic = ccdDevAcc.GetData(ccdDevName, ref reStr);
                        //if (ccdDataDic == null || ccdDataDic.Keys.Count() < 1)
                        //{
                        //    //Console.WriteLine(string.Format("{0}获取CCD{1}数据失败,{2}",nodeName,ccdDevName,reStr));
                        //    this.currentTaskDescribe = string.Format("获取CCD{0}数据失败,{1}", ccdDevName, reStr);
                        //    break;
                        //}
                        //foreach (string keyStr in ccdDataDic.Keys)
                        //{
                        //    string str = string.Format("CCD数据，产品ID:{0}，数据：{1}", keyStr, ccdDataDic[keyStr]);
                        //    logRecorder.AddDebugLog(nodeName, str);
                        //    AddProcessRecord(keyStr, "模块", "检测数据", string.Format("读取到{0}检测数据", ccdDevName), ccdDataDic[keyStr]);
                        //    string upLoadMesScrewData = "";
                        //    if (GetScrewData(ccdDataDic[keyStr], ref upLoadMesScrewData) == false)
                        //    {

                        //        logRecorder.AddDebugLog(nodeName, "CCD数据格式错误！无法转换为MES需要格式！");
                        //        continue;
                        //    }
                        //    if (UploadMesScrewData(keyStr, upLoadMesScrewData) == false)
                        //    {
                        //        logRecorder.AddDebugLog(nodeName, "上传MES锁螺丝数据失败！");
                        //        continue;
                        //    }
                        //    logRecorder.AddDebugLog(nodeName, "上传MES锁螺丝数据成功！");
                        //}
                        #endregion
                        //==更改为读取数据库形式，根据模块条码获取数据，判断所有条码的数据上传成功后执行下一步======//
                        int readDataApply = 0;
                        Console.WriteLine("sd1");
                        bool isScrewCpt = false;//所有螺丝加工完成标识
                        List<DBAccess.Model.BatteryModuleModel> modList = modBll.GetModelList(string.Format("palletID='{0}' and palletBinded=1", this.rfidUID));
                        List<DBAccess.Model.BatteryModuleModel> uploadModList = modList.FindAll(mod => mod.tag3 == "1");//查询已经上传的条码，上传的为1，没有的为0或为空 
                        if (uploadModList!=null&&uploadModList.Count == modList.Count)
                        {
                            isScrewCpt = true;
                        }
                       
                        Console.WriteLine("sd2");
                        if (isScrewCpt == true)
                        {
                            currentTaskPhase++;
                            this.currentTask.TaskPhase = this.currentTaskPhase;
                            this.ctlTaskBll.Update(this.currentTask);
                            break;
                        }
                        Console.WriteLine("sd3");
                        if (this.plcRW2.ReadDB("D9000", ref readDataApply) == false)
                        {
                        
                            break;
                        }
                        Console.WriteLine("sd4");
                        if (readDataApply != 1)
                        {
                            break;
                        }

                        Console.WriteLine("sd5");
                        List<DBAccess.Model.BatteryModuleModel> unUploadModList = modList.FindAll(mod => mod.tag3 != "1");//
                        foreach (DBAccess.Model.BatteryModuleModel module in unUploadModList)
                        {
                            ALineScrewDB.dbModel screwModel = bllScrewDb.GetModelByModuleID(module.batModuleID);

                            if (screwModel == null)
                            {
                                continue;
                            }
            
                            string mesScrewData = "";
                            if (GetScrewData(screwModel, ref mesScrewData, ref reStr) == false)
                            {
                                Console.WriteLine("模块:" + screwModel.二维码 + ",获取螺丝数据失败：" + reStr);
                                continue;
                            }
                         
                            int status = UploadMesScrewData(module.batModuleID, mesScrewData, ref reStr);
                            if (status == 0)
                            {                  
                                if (this.plcRW2.WriteDB("D9000", 2) == false)
                                {
                                    break;
                                }
                                logRecorder.AddDebugLog(nodeName, "上传MES锁螺丝数据成功！数据：" + module.batModuleID+":"+ mesScrewData + "返回：" + reStr);
                            }
                            else if (status == 1)
                            {
                                ScrewNGModule screwNgMod = new ScrewNGModule();
                                screwNgMod.ChannelIndex = this.channelIndex;
                                screwNgMod.ModPos = int.Parse(module.tag2);
                                Console.WriteLine("tag2:" + module.tag2);
                                this.screwNgHandler.AddNgModule(screwNgMod);
                                module.palletBinded =false;//NG解绑
                                module.checkResult = 2;//NG
                                logRecorder.AddDebugLog(nodeName, "上传MES锁螺丝数据成功，但返回NG！" + module.batModuleID + ":" + reStr);
                                if (this.plcRW2.WriteDB("D9000", 3) == false)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                //Console.WriteLine();
                                logRecorder.AddDebugLog(nodeName, "上传MES锁螺丝数据失败！" + reStr);
                                continue;
                            }
                            module.tag3 = "1";
                            modBll.Update(module);
                            //screwModel.UpLoad = "是";
                            
                            //bllScrewDb.Update(screwModel);
                        }
                        //========================================================================================//
                        if (isScrewCpt == false)
                        {                          
                            break;
                        }
                        Console.WriteLine("sd6");
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
                           // Console.WriteLine(string.Format("{0},{1}", nodeName, reStr));
                            currentTaskDescribe = reStr;
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
                        currentStat.StatDescribe = "工装板放行";
                        this.currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 8:
                    {
                        currentStat.StatDescribe = "NG流程处理";
                        Console.WriteLine("sd7");
                        if (this.screwNgHandler.Execute() == false)
                        {
                            break;
                        }
                        List<DBAccess.Model.BatteryModuleModel> modList = modBll.GetModelList(string.Format("palletID='{0}'", this.rfidUID));
                        foreach(DBAccess.Model.BatteryModuleModel mod in modList)//锁螺丝工位有拍出流程 不需要记录NG
                        {
                            mod.checkResult = 0;
                            modBll.Update(mod);
                        }
                        
                        currentStat.StatDescribe = "流程完成";
                        Console.WriteLine("sd8");
                      
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                default:
                    break;
            }
            return true;
           
        }

        //private bool MesNGHandler(List<DBAccess.Model.BatteryModuleModel> modList, ref string reStr)
        //{
        //    try
        //    {
        //        bool NoNg = true; 
                    
        //        foreach (DBAccess.Model.BatteryModuleModel module in modList)
        //        {
        //            ALineScrewDB.dbModel screwModel = bllScrewDb.GetModelByModuleID(module.batModuleID);
        //            if (screwModel == null)
        //            {
        //                continue;
        //            }

        //            if (screwModel.UpLoad == "是")
        //            {
        //                continue;
        //            }

        //            string mesScrewData = "";
        //            if (GetScrewData(screwModel, ref mesScrewData, ref reStr) == false)
        //            {
        //                Console.WriteLine("模块:" + screwModel.二维码 + ",获取螺丝数据失败：" + reStr);
        //                continue;
        //            }

        //            int status = UploadMesScrewData(module.batModuleID, mesScrewData, ref reStr);
        //            if (status == 0)
        //            {

        //                if (this.plcRW2.WriteDB("D9000", 2) == false)
        //                {
        //                    break;
        //                }
        //                logRecorder.AddDebugLog(nodeName, "上传MES锁螺丝数据成功！数据：" + mesScrewData + "返回：" + reStr);
        //            }
        //            else if (status == 1)
        //            {
        //                NoNg = false;
        //                if(this.channelIndex==1)//A通道
        //                { 
                            
        //                }
        //                else  //B通道
        //                {
                        
        //                }
        //                logRecorder.AddDebugLog(nodeName, "上传MES锁螺丝数据成功，但返回NG！" + reStr);
        //                if (this.plcRW2.WriteDB("D9000", 3) == false)
        //                {
        //                    break;
        //                }
        //            }
        //            else
        //            {
        //                //Console.WriteLine();
        //                logRecorder.AddDebugLog(nodeName, "上传MES锁螺丝数据失败！" + reStr);
        //                continue;
        //            }
        //            screwModel.UpLoad = "是";
        //            bllScrewDb.Update(screwModel);
        //            if(NoNg==true)//没有ng的处理
        //            { }
               
        //        }
        //        return true;
        //    }
        //    catch(Exception ex)
        //    {
        //        reStr = ex.Message;
        //        return false;
        //    }
        //}
        private bool GetScrewData( ALineScrewDB.dbModel screwModel,ref string screwdata,ref string restr)
        {
            try
            {


                if (screwModel == null)
                {
                    restr = "传入对象为空";
                    return false;
                }
                if (screwModel.螺丝1马头扭矩.ToLower().Trim() == "null")
                {
                    restr = "螺丝1码头扭矩为空！";
                    return false;
                }
                else if (screwModel.螺丝2马头扭矩.ToLower().Trim() == "null")
                {
                    restr = "螺丝2码头扭矩为空！";
                    return false;
                }
                else if (screwModel.螺丝3马头扭矩.ToLower().Trim() == "null")
                {
                    restr = "螺丝3码头扭矩为空！";
                    return false;
                }
                else if (screwModel.螺丝4马头扭矩.ToLower().Trim() == "null")
                {
                    restr = "螺丝4码头扭矩为空！";
                    return false;
                }
                else if (screwModel.螺丝1马头角度.ToLower().Trim() == "null")
                {
                    restr = "螺丝1马头角度为空！";
                    return false;
                }
                else if (screwModel.螺丝2马头角度.ToLower().Trim() == "null")
                {
                    restr = "螺丝2马头角度为空！";
                    return false;
                }
                else if (screwModel.螺丝3马头角度.ToLower().Trim() == "null")
                {
                    restr = "螺丝3马头角度为空！";
                    return false;
                }
                else if (screwModel.螺丝4马头角度.ToLower().Trim() == "null")
                {
                    restr = "螺丝4马头角度为空！";
                    return false;
                }

                string mesScrewData = "螺丝1扭矩:" + screwModel.螺丝1马头扭矩 + ":Nm|螺丝2扭矩:" + screwModel.螺丝2马头扭矩 + ":Nm|螺丝3扭矩:"
                               + screwModel.螺丝3马头扭矩 + ":Nm|螺丝4扭矩:" + screwModel.螺丝4马头扭矩 + ":Nm|螺丝1角度:" + screwModel.螺丝1马头角度
                               + ":°|螺丝2角度:" + screwModel.螺丝2马头角度 + ":°|螺丝3角度:" + screwModel.螺丝3马头角度 + ":°|螺丝4角度:"
                               + screwModel.螺丝4马头角度 + ":°";

                screwdata = mesScrewData;
                mesScrewData = "获取螺丝数据成功！";
                return true;
            }
            catch(Exception ex)
            {
                restr = ex.Message;
                
                return false;
            }
        }
        private int UploadMesScrewData(string modCode, string screwData,ref string restr)
        {

            int flag = 3;
            string M_AREA = "Y001";
            string M_WORKSTATION_SN = "Y00100601";
            string M_DEVICE_SN = "";
       
            string M_UNION_SN = "";
            string M_CONTAINER_SN = "";
            string M_LEVEL = "";
            string M_ITEMVALUE = screwData;
            RootObject rObj = new RootObject();
            
            string strJson = "";

            rObj = DevDataUpload(flag, M_DEVICE_SN, M_WORKSTATION_SN, modCode, M_UNION_SN, M_CONTAINER_SN, M_LEVEL, M_ITEMVALUE, ref strJson);
            restr = rObj.RES;
            //logRecorder.AddDebugLog(nodeName, string.Format("上传MES数据{0},返回{1},发送json数据{2}", M_ITEMVALUE, rObj.RES,strJson));
            if (rObj.RES.ToUpper().Contains("OK"))
            {
                return 0;
            }
            else if(rObj.RES.ToUpper().Contains("NG"))
            {
                return 1;
            }
            else
            {
               // Console.WriteLine(this.nodeName + "上传MES锁螺丝数据错误：" + rObj.RES);
                logRecorder.AddDebugLog(nodeName, string.Format("上传MES返回{0}", rObj.RES));
                return 2;
            }
        

        }

        private bool GetScrewData(string screwData, ref string mesScrewData)
        {
            try
            {
                string[] data = screwData.Split(',');
                string screwNJ1 = data[1].Split('=')[1];
                string screwNJ2 = data[5].Split('=')[1];
                string screwNJ3 = data[9].Split('=')[1];
                string screwNJ4 = data[13].Split('=')[1];

                string screwJD1 = data[2].Split('=')[1];
                string screwJD2 = data[6].Split('=')[1];
                string screwJD3 = data[10].Split('=')[1];
                string screwJD4 = data[14].Split('=')[1];

                mesScrewData = "螺丝1扭矩:" + screwNJ1 + ":Nm|螺丝2扭矩:" + screwNJ2 + ":Nm|螺丝3扭矩:" + screwNJ3 + ":Nm|螺丝4扭矩:" + screwNJ4
                   + ":Nm|螺丝1角度:" + screwJD1 + ":°|螺丝2角度:" + screwJD2 + ":°|螺丝3角度:" + screwJD3 + ":°|螺丝4角度:" + screwJD4 + ":°";
                return true;
            }
            catch
            {
                Console.WriteLine("CCD数据异常,{0}", screwData);
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

        protected override void ExeRfidBusinessAB()
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
                      //  logRecorder.AddDebugLog(nodeName, string.Format("A通道读到RFID:{0}", this.rfidUIDA));
                        if (this.nodeID == "OPA005")
                        {
                            if (IsEmptyPallet(this.rfidUIDA) == true)
                            {
                                this.db1ValsToSnd[0] = 4;
                            }
                            else
                            {
                                this.db1ValsToSnd[0] = 2;
                            }

                        }
                        else
                        {
                            this.db1ValsToSnd[0] = 2;
                        }
                    }

                }
                else
                {
                   // logRecorder.AddDebugLog(nodeName, string.Format("A通道读到RFID:{0}", this.rfidUIDA));
                    if (this.nodeID == "OPA005")
                    {
                        if (IsEmptyPallet(this.rfidUIDA) == true)
                        {
                            this.db1ValsToSnd[0] = 4;
                        }
                        else
                        {
                            this.db1ValsToSnd[0] = 2;
                        }

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
                       // logRecorder.AddDebugLog(nodeName, string.Format("B通道读到RFID:{0}", this.rfidUIDB));
                      //  logRecorder.AddDebugLog(nodeName, string.Format("A通道读到RFID:{0}", this.rfidUIDA));
                        if (this.nodeID == "OPA005")
                        {
                            if (IsEmptyPallet(this.rfidUIDB) == true)
                            {
                                this.db1ValsToSnd[1] = 4;
                            }
                            else
                            {
                                this.db1ValsToSnd[1] = 2;
                            }

                        }
                        else
                        {
                            this.db1ValsToSnd[1] = 2;
                        }

                    }

                }
                else
                {
                 //   logRecorder.AddDebugLog(nodeName, string.Format("A通道读到RFID:{0}", this.rfidUIDA));
                    if (this.nodeID == "OPA005")
                    {
                        if (IsEmptyPallet(this.rfidUIDB) == true)
                        {
                            this.db1ValsToSnd[1] = 4;
                        }
                        else
                        {
                            this.db1ValsToSnd[1] = 2;
                        }

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
            if (modList != null && modList.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //public bool UploadMesData(string rfid, string valueItems,ref string reStr)
        //{
        //    string M_AREA = "Y001";
        //    string M_WORKSTATION_SN = "Y00102201";
        //    string M_DEVICE_SN = "";

        //    string M_UNION_SN = "";
        //    string M_CONTAINER_SN = "";
        //    string M_LEVEL = "";
        //    string M_ITEMVALUE = valueItems;
        //    RootObject rObj = new RootObject();
        //    List<DBAccess.Model.BatteryModuleModel> modelList = modBll.GetModelList(string.Format("palletID='{0}' and palletBinded=1", rfid)); //modBll.GetModelByPalletID(this.rfidUID, this.nodeName);
        //    if (modelList == null || modelList.Count == 0)
        //    {
        //        return false;
        //    }
        //    foreach(DBAccess.Model.BatteryModuleModel battery in modelList)
        //    {
        //        MTDBAccess.Model.dbModel screwModel =  blldb.GetModel(battery.batModuleID);
        //        if(screwModel == null)
        //        {
        //            continue;
        //        }
        //        M_LEVEL = battery.tag1;
        //        string barcode = modelList[0].batModuleID;
        //        M_ITEMVALUE = "";//需要拼接螺丝数据
        //        string strJson = "";

        //        rObj = WShelper.DevDataUpload(3, M_DEVICE_SN, M_WORKSTATION_SN, barcode, M_UNION_SN, M_CONTAINER_SN, M_LEVEL, M_ITEMVALUE, ref strJson);
        //        reStr = rObj.RES;

        //        if (rObj.RES.Contains("OK"))
        //        {
        //            logRecorder.AddDebugLog(nodeName, string.Format("上传螺丝数据失败：{0}", M_ITEMVALUE) +"-"+ rObj.RES);
        //            return true;
        //        }
        //        else
        //        {
        //            logRecorder.AddDebugLog(nodeName, string.Format("上传螺丝数据失败：{0}", M_ITEMVALUE + "-" + rObj.RES));

        //            return false;
        //        }

        //    }

        //    return true;
        //}
    }
    public class ScrewNGHandler
    {
        private List<ScrewNGModule> NGModules= new List<ScrewNGModule>();
        private IPlcRW PlcRw = null;
        private IPlcRW PlcRw2 = null;
        private int currChannelIndex = 0;//当前通道
        private int Step = 0;
        private Dictionary<string, string> addrPosCfg = new Dictionary<string, string>();
        private Dictionary<int, string> addrChannelCfg = new Dictionary<int, string>();
        private string NGAddr = "D8734";
        private ILogRecorder logRecorder = null;
        public ScrewNGHandler(IPlcRW plcRw,IPlcRW plcRw2,int channelIndex,ILogRecorder logRec)
        {
            this.PlcRw = plcRw;
            this.PlcRw2 = plcRw2;
            this.logRecorder = logRec;
            this.currChannelIndex = channelIndex;
            addrPosCfg["1-1"] = "D8730";//A通道1位置
            addrPosCfg["1-2"] = "D8731";//A通道2位置
            addrPosCfg["2-1"] = "D8732";//B通道1位置
            addrPosCfg["2-2"] = "D8733";//B通道2位置

            addrChannelCfg[1] = "D8000";//A通道
            addrChannelCfg[2] = "D8001";//B通道
        }

        public void AddNgModule(ScrewNGModule ngMod)
        {
            List<ScrewNGModule> ngMods = this.NGModules.FindAll(n => n.ChannelIndex == ngMod.ChannelIndex && n.ModPos == ngMod.ModPos);
            logRecorder.AddDebugLog("A线锁螺丝", "添加数据通道：" + ngMod.ChannelIndex + "，位置：" +ngMod.ModPos);
            if (ngMods == null||ngMods.Count==0)
            {
                logRecorder.AddDebugLog("A线锁螺丝", "添加成功：" +ngMod.ChannelIndex+"-"+ ngMod.ModPos);
                this.NGModules.Add(ngMod);
            }
        }
        //public void Reset()
        //{
        //    this.Step = 0;
        //    this.currChannelIndex = 0;

        //    this.NGModules.Clear();
        //}
        public bool Execute()
        {
            Console.WriteLine("sw1");
            if(this.NGModules.Count >0)//有NG
            {
                switch (this.Step)
                {
                    case 0:
                        {
                            Console.WriteLine("sw2");
                            if (this.PlcRw2.WriteDB(NGAddr, 1) == false)
                            {
                                return false;
                            }
                            foreach (ScrewNGModule ngMod in this.NGModules)
                            {                              
                                Console.WriteLine("sw3");
                                string key = ngMod.ChannelIndex.ToString() + "-" + ngMod.ModPos;
                                string addr = addrPosCfg[key];
                                Console.WriteLine("swAddr" + addr);

                                if (this.PlcRw2.WriteDB(addr, 1) == false)
                                {
                                    return false;
                                }
                                logRecorder.AddDebugLog("A线锁螺丝", "写入地址：" + addr + "成功：值1");
                                Console.WriteLine("sw4");
                                string chennelAddr = addrChannelCfg[ngMod.ChannelIndex];
                                if (this.PlcRw.WriteDB(chennelAddr, 1) == false)
                                {
                                    return false;
                                }

                                logRecorder.AddDebugLog("A线锁螺丝", "写入通道地址：" + chennelAddr + "成功：值1");
                                Console.WriteLine("sw5");
                            }
                            this.Step++;
                            Console.WriteLine("sw6");
                            return false;
                        }
                    case 1:
                        {
                            Console.WriteLine("sw7");
                            string chennelAddr = addrChannelCfg[currChannelIndex];
                          
                            int manualStatus = 0;
                            if (this.PlcRw2.ReadDB(NGAddr, ref manualStatus) == false)
                            {
                                return false;
                            }
                            Console.WriteLine("sw8");
                            if (manualStatus != 2)
                            {
                                return false;
                            }
                            if(this.PlcRw.WriteDB(chennelAddr,2) ==false)
                            {
                                return false;
                            }
                            Console.WriteLine("sw9");
                            
                            return true;
                        }
                    default:
                        {
                            return false;
                        }
                }
            }
            else//没有NG
            {
                Console.WriteLine("sw10");
                if (this.PlcRw2.WriteDB(NGAddr, 2) == false)
                {
                    return false;
                }
                Console.WriteLine("sw11");
                string chennelAddr = addrChannelCfg[currChannelIndex];
                if (this.PlcRw.WriteDB(chennelAddr, 2) == false)
                {
                    return false;
                }
                Console.WriteLine("sw12");
                 return true;
            }
           
        }
    }
    public class ScrewNGModule
    {
        public int ChannelIndex { get; set; }
        public int ModPos { get; set; }
        public ScrewNGModule()
        { }

    }
}
