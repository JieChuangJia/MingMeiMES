using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using PLProcessModel;
using FTDataAccess.BLL;
using FTDataAccess.Model;
using DevInterface;
using DevAccess;
using System.Data;
namespace LineNodes
{
    public class NodeWeld : CtlNodeBaseModel
    {
        string welderIP = "192.168.0.44";
        int stepIndex = 1;
        DBAccess.Model.BatteryModuleModel currWorkMod = null;
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
            this.dicCommuDataDB2[4].DataDescription = "A通道设备完成状态，1:复位，2:完成";
            this.dicCommuDataDB2[5].DataDescription = "A通道设备完成状态，1:复位，2:完成";
            if(this.nodeID=="OPB004")
            {
                welderIP = "192.168.0.44";
            }
            else if (this.nodeID == "OPB008")
            {
                welderIP = "192.168.0.45";
            }
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
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.currentTask.TaskParam = rfidUID;
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
                        List<DBAccess.Model.BatteryModuleModel> modList = modBll.GetModelList(string.Format("palletID='{0}' and palletBinded=1", rfidUID));
                        if (modList.Count() < 1) //空板直接放行
                        {
                            currentTaskPhase = 3;
                            break;
                        }
                        bool isAllComplete = false;
                        ModuleWeldBuiness(modList, ref isAllComplete);
                        if (isAllComplete == true)
                        {
                            currentTaskPhase++;
                            this.currentTask.TaskPhase = this.currentTaskPhase;
                            this.ctlTaskBll.Update(this.currentTask);
                        }
                         
                    }
                    break;
                case 3:
                    {

                      //  Thread.Sleep(10000);
                        db1ValsToSnd[2 + this.channelIndex - 1] = 3;
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

        private void ModuleWeldBuiness(List<DBAccess.Model.BatteryModuleModel> modList, ref bool isAllComplete)
        {
           
            string weldStr = "";
            string reStr = "";
          

            if (db2Vals[0] == 0)
            {
                stepIndex = 1;
            }
            switch (stepIndex)
            {
                case 1:
                    {
                         currWorkMod = null;
                        Console.WriteLine("weld1");
                        //tag3:SENDED记录数据已发送至焊机，COMPLETE为加工完成
                        string welderSndFile = string.Format(@"\\{0}\MESReport\DeviceInfoLane{1}.txt", welderIP, channelIndex); // @"\\192.168.0.45\MESReport\DeviceInfoLane1.txt";
                        if (!System.IO.File.Exists(welderSndFile))
                        {
                            currentTaskDescribe = string.Format("铝丝焊文件：{0}不存在", welderSndFile);
                            return;
                        }
                        Console.WriteLine("weld2");
                        for (int i = 0; i < modList.Count ; i++)
                        {                          
                            if (modList[i].tag4.ToUpper() == ENUMWeldStatus.SENDED.ToString() || modList[i].tag4.ToUpper() == ENUMWeldStatus.COMPLETE.ToString())
                            {
                                continue;
                            }
                            currWorkMod = modList[i];

                            if (GetWeldStr(currWorkMod, ref weldStr, ref reStr) == false)
                            {
                                logRecorder.AddDebugLog(nodeName, string.Format("获取模块焊接加工数据失败:{0}", reStr));
                                continue;
                            }
                            break;
                        }
                        Console.WriteLine("weld3");
                        if(currWorkMod ==null)
                        {
                            return;
                        }
                        Console.WriteLine("weld4");
                        System.IO.StreamWriter writter = new System.IO.StreamWriter(welderSndFile, false);
                        StringBuilder strBuild = new StringBuilder();
                        writter.Write(weldStr);
                        writter.Flush();
                        writter.Close();
                        logRecorder.AddDebugLog(nodeName, "写入铝丝焊:" + weldStr);
                        currWorkMod.tag4 = ENUMWeldStatus.SENDED.ToString();
                        modBll.Update(currWorkMod);
                        Console.WriteLine("weld5");
                        stepIndex++;
                        break;
                    }
                case 2:
                    {
                        Console.WriteLine("weld6");
                        if (this.db2Vals[2 + channelIndex] != 2)//有模块加工完成
                        {
                            currentTaskDescribe = "等待设备工作完成";
                            return;
                        }
                        if(this.NodeDB2Commit(2 + channelIndex,1,ref reStr)==false)//复位读写
                        {
                            return;
                        }
                        Console.WriteLine("weld7");
                        if (CompleteDescRecord(currWorkMod, ref reStr) == false)
                        {
                            logRecorder.AddDebugLog(nodeName, string.Format("焊接完成反馈数据处理失败:{0}", reStr));
                            return;
                        }
                        Console.WriteLine("weld8");
                        if (UploadBatteryModToMes(currWorkMod, ref reStr) == false)
                        {
                            logRecorder.AddDebugLog(nodeName, string.Format("焊接上传数据失败:{0}", reStr));
                            return;
                        }
                        Console.WriteLine("weld9");
                        currWorkMod.tag4 = ENUMWeldStatus.COMPLETE.ToString();
                        modBll.Update(currWorkMod);
                        if (IsAllModComplete() == true)
                        {
                            Console.WriteLine("weld10");
                            currWorkMod.tag4 = "";
                            modBll.Update(currWorkMod);
                            isAllComplete = true;
                        }
                        else
                        {
                            isAllComplete = false;
                            Console.WriteLine("weld11");
                        }

                        stepIndex = 1;
                        break;
                    }
            }

        }
        private bool IsAllModComplete( )
        {
            List<DBAccess.Model.BatteryModuleModel> modList = modBll.GetModelList(string.Format("palletID='{0}'", this.rfidUID));
            bool isAllCmd = true;
            foreach(DBAccess.Model.BatteryModuleModel  mod in modList)
            {
                if(mod.tag4.ToUpper()!=ENUMWeldStatus.COMPLETE.ToString())
                {
                    isAllCmd = false;
                }
            }
            return isAllCmd;
        }

        private bool CompleteDescRecord( DBAccess.Model.BatteryModuleModel modBattery  ,ref string restr)
        {
            try
            {
                string testResultPath = string.Format(@"\\{0}\MESReport\PullTestResult\{1}_{2}.csv", welderIP, modBattery.palletID, modBattery.batModuleID);
                if (!System.IO.File.Exists(testResultPath))
                {
                    currentTaskDescribe = string.Format(this.nodeName + "铝丝焊结果文件：{0}不存在", testResultPath);

                    return false;
                }
                DataTable dt = CSVFileHelper.OpenCSV(testResultPath);
                string checkRes = "";
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    if (dt.Rows[j]["Traceability - integrated pull test - passed"].ToString().ToUpper() == "FALSE")
                    {
                        checkRes = "NG";
                        break;
                    }
                }
                if (checkRes == "NG")
                {
                    modBattery.checkResult = 2;
                    modBattery.palletBinded = false;
                    modBattery.tag3 = this.nodeName; //标识在哪个工位产生的NG,(共有三个工位，1#铝丝焊、2#铝丝焊、DCIR检测)
                }
                else
                {

                    modBattery.checkResult = 1;
                }
                modBll.Update(modBattery);


                return true;
            }
            catch (Exception ex)
            {
                restr = ex.Message;
                return false;
            }
        }
        private bool GetWeldStr( DBAccess.Model.BatteryModuleModel currMod, ref string weldStr,ref string restr)
        {
            
            try
            {
                //StringBuilder strBuild = new StringBuilder();
                string str = string.Format("{0},{1},", currMod.tag2.Trim(), this.rfidUID);
                str += currMod.batModuleID;
                //foreach (DBAccess.Model.BatteryModuleModel mod in modList)
                //{
                //    if (mod.tag2.Trim() == modPosIndex)
                //    {
                //        str += mod.batModuleID;
                //        break;
                //    }
                //}
                //strBuild.AppendLine(str);

                weldStr = str;
                return true;
            }
            catch(Exception ex)
            {
                restr = ex.Message;
                return false;
            }
          
        }

        private bool UploadBatteryModToMes( DBAccess.Model.BatteryModuleModel modBattery ,ref string reStr)
        {
            try
            {
                string M_WORKSTATION_SN = "";
                if (this.nodeID == "OPB004")
                {
                    M_WORKSTATION_SN = "Y00101701";
                }
                else
                {
                    M_WORKSTATION_SN = "Y00102001";
                }


                if (modBattery.palletBinded == false && modBattery.tag3 != null && modBattery.tag3 != this.nodeName)
                {
                    reStr = this.nodeName + ":模块没有绑定！";
                    return false;
                }
                string testResultPath = string.Format(@"\\{0}\MESReport\BondParameters\{1}_{2}.csv", welderIP, modBattery.palletID, modBattery.batModuleID);
                if (!System.IO.File.Exists(testResultPath))
                {
                    currentTaskDescribe = string.Format(this.nodeName + "铝丝焊参数文件：{0}不存在", testResultPath);
                    reStr = currentTaskDescribe;
                    return false;
                }
                DataTable dt = CSVFileHelper.OpenCSV(testResultPath);
                string pullTestPath = string.Format(@"\\{0}\MESReport\PullTestResult\{1}_{2}.csv", welderIP, modBattery.palletID, modBattery.batModuleID);
                if (!System.IO.File.Exists(pullTestPath))
                {
                    currentTaskDescribe = string.Format(this.nodeName + "铝丝焊拉力结果参数文件：{0}不存在", testResultPath);
                    reStr = currentTaskDescribe;
                    return false;
                }
                DataTable dtPull = CSVFileHelper.OpenCSV(pullTestPath);
                int M_FLAG = 3;
                string M_DEVICE_SN = "";
                string M_SN = modBattery.batModuleID;
                string M_UNION_SN = "";
                string M_CONTAINER_SN = "";
                string M_LEVEL = "";
                string M_ITEMVALUE = GetItemVal(dt, dtPull);
                //上传数据
                RootObject rObj = new RootObject();
                string strJson = "";
                rObj = DevDataUpload(M_FLAG, M_DEVICE_SN, M_WORKSTATION_SN, M_SN, M_UNION_SN, M_CONTAINER_SN, M_LEVEL, M_ITEMVALUE, ref strJson);
                currentTaskDescribe = rObj.RES + "," + M_FLAG + "," + M_WORKSTATION_SN + "," + M_ITEMVALUE + this.nodeName + rObj.CONTROL_TYPE;
                Console.WriteLine(rObj.RES);
                Console.WriteLine(M_ITEMVALUE);
                logRecorder.AddDebugLog(nodeName, "上传mes数据:" + M_ITEMVALUE);
                this.WriteTxtLog(modBattery.batModuleID, "上传mes数据:" + M_ITEMVALUE + "返回结果:" + rObj.RES);
                logRecorder.AddDebugLog(nodeName, string.Format("上传MES，返回结果:{0}", rObj.RES));


                return true;
            }
            catch(Exception ex)
            {
                reStr = ex.Message;
                logRecorder.AddDebugLog(nodeName, string.Format("上传MES错误：{0}",ex.Message));
                return false;
            }
        
          
        }
        protected bool WriteProductsToWelder(int channel,string rfidStr)
        {
            List<DBAccess.Model.BatteryModuleModel> modList = modBll.GetModelList(string.Format("palletID='{0}' and palletBinded=1",rfidStr));
            if (modList.Count() <1)
            {
                return false;
            }
            string welderSndFile = string.Format(@"\\{0}\MESReport\DeviceInfoLane{1}.txt", welderIP, channel); // @"\\192.168.0.45\MESReport\DeviceInfoLane1.txt";
            if (!System.IO.File.Exists(welderSndFile))
            {
                currentTaskDescribe = string.Format("铝丝焊文件：{0}不存在", welderSndFile);
                return false;
            }
            System.IO.StreamWriter writter = new System.IO.StreamWriter(welderSndFile, false);
            StringBuilder strBuild = new StringBuilder();
            for (int i = 1; i < 5; i++)
            {
                string str = string.Format("{0},{1},", i, rfidStr);
                foreach (DBAccess.Model.BatteryModuleModel mod in modList)
                {
                    if (mod.tag2.Trim() == i.ToString())
                    {
                        str += mod.batModuleID;
                        break;
                    }
                }
                strBuild.AppendLine(str);
            }
            writter.Write(strBuild.ToString());
            writter.Flush();
            writter.Close();
            logRecorder.AddDebugLog(nodeName, "写入铝丝焊:" + strBuild.ToString());
            return true;
               
            
        }
        
        protected override  void ExeRfidBusinessAB()
        {
            if(this.rfidRWList == null || this.rfidRWList.Count()<1)
            {
                return ;
            }
            PLNodesBll plNodeBll = new PLNodesBll();
            if(this.db2Vals[1] == 2)
            {
               
                //A通道
                if(string.IsNullOrWhiteSpace(this.rfidUIDA))
                {
                    IrfidRW rw = null;
                    if (SysCfgModel.SimMode)
                    {
                        this.rfidUIDA = this.SimRfidUID;
                    }
                    else if (this.rfidRWList.Count > 0)
                    {
                        rw = this.rfidRWList[0];
                        this.rfidUIDA =rw.ReadUID();
                     
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
                        this.currentStat.StatDescribe = "读A通道RFID失败";
                        this.currentTaskDescribe = "读A通道RFID失败";

                    }
                    else
                    {
                        
                        this.plNodeModel.tag1 = this.rfidUIDA;
                        plNodeBll.Update(this.plNodeModel);
                        
                        if (this.db1ValsToSnd[0] != 2)
                        {
                            
                            //if (WriteProductsToWelder(1, rfidUIDA))
                            //{
                            //    logRecorder.AddDebugLog(nodeName, string.Format("A通道读到RFID:{0}", this.rfidUIDA));
                            //    Thread.Sleep(5000);
                            //    this.db1ValsToSnd[0] = 2;
                              
                            //}
                            //else
                            //{
                               
                            //    this.db1ValsToSnd[0] = 2;
                            //}
                            this.db1ValsToSnd[0] = 2;
                        }
                        //if (!SysCfgModel.SimMode)
                        //{
                        //    (rw as DevAccess.RfidCF).ClearBufUID();
                        //}
                    }
                }
                else
                {
                    this.db1ValsToSnd[0] = 2;
                     
                }
                
            }
            else if(this.db2Vals[1]==1)
            {
                this.rfidUIDA = ""; 
                this.db1ValsToSnd[0] = 1;
                this.plNodeModel.tag1 = "";
                plNodeBll.Update(this.plNodeModel);
            }

            if(this.db2Vals[2] == 2)
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
                         this.currentStat.StatDescribe = "读B通道RFID失败";
                         this.currentTaskDescribe = "读B通道RFID失败";
                     }
                     else
                     {
                         this.plNodeModel.tag2 = this.rfidUIDB;
                         plNodeBll.Update(this.plNodeModel);
                        
                      
                         if (this.db1ValsToSnd[1] != 2)
                         {
                            
                             if (WriteProductsToWelder(2, rfidUIDB))
                             {
                                 
                                 logRecorder.AddDebugLog(nodeName, string.Format("B通道读到RFID:{0}", this.rfidUIDB));
                                 Thread.Sleep(5000);
                                 this.db1ValsToSnd[1] = 2;
                             }
                             else
                             {
                                 //rfidUIDB = "";
                                 this.db1ValsToSnd[1] = 2;
                             }
                          
                         }
                         //if (!SysCfgModel.SimMode)
                         //{
                         //    (rw as DevAccess.RfidCF).ClearBufUID();
                         //}
                     }
                   
                }
                else
                {
                    this.db1ValsToSnd[1] = 2;
                }
               
            }
            else if(this.db2Vals[2] == 1)
            {
                this.rfidUIDB = "";
                this.db1ValsToSnd[1] = 1;
                this.plNodeModel.tag2 = "";
                plNodeBll.Update(this.plNodeModel);
            }
          
        }

         /// </summary>
        /// <param name="dt">源数据DataTable</param>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        private DataTable GetNewDataTable(DataTable dt, string condition)
        {
            DataTable newdt = new DataTable();
            newdt = dt.Clone();
            DataRow[] dr = dt.Select(condition);
            for (int i = 0; i < dr.Length; i++)
            {
                newdt.ImportRow((DataRow)dr[i]);
            }
            return newdt;//返回的查询结果
        }
 

        private string GetItemVal(DataTable dt,DataTable dtPull)
        {
            string strP = "";
            string strN = "";
            string strLv = "";
            string strPV = "正负极电压";
           // string strNV = "负极电压";
            string strPA = "正负极电流";
          //  string strNA = "负级电流";
            string strPT = "正负极时间";
           // string strNT = "负级时间";
            string strLvA = "铝片电流";
            string strLvV = "铝片电压";
            string strLvT = "铝片时间";
            int pIndex = 0;
           // int nIndex = 0;
            int lvIndex = 0;
            #region 电压、电流检测结果
           // for (int i = 0; i < dt.Rows.Count; i++)

            DataTable dtTemp1 = GetNewDataTable(dt, "[Traceability - reference system]='1'");
            for (int i = 0; i < SysCfgModel.batteryNumInMod;i++ )
            {
                if (i > dtTemp1.Rows.Count - 1)
                {
                    //for(int j = 0;j<3;j++)
                    //{
                    //    pIndex++;
                    //    string tempV = strPV + pIndex.ToString() + ":" + "0" + ":V|";
                    //    string tempA = strPA + pIndex.ToString() + ":" + "0" + ":A|";
                    //    string tempT = strPT + pIndex.ToString() + ":" + "0" + ":S|";
                    //    strP = strP + tempV + tempA + tempT;
                    //}
                    for (int j = 0; j < 3; j++)
                    {
                        lvIndex++;
                        string tempV = strLvV + lvIndex.ToString() + ":" +"0" + ":V|";
                        string tempA = strLvA + lvIndex.ToString() + ":" +"0" + ":A|";
                        string tempT = strLvT + lvIndex.ToString() + ":" +"0" + ":S|";
                        strLv = strLv + tempV + tempA + tempT;
                    }
                  
                }
                else
                {
                    //if (dt.Rows[i]["Traceability - reference system"].ToString().ToUpper() == "2" || dt.Rows[i]["Traceability - reference system"].ToString().ToUpper() == "3") //正负极
                    //{
                    //    pIndex++;
                    //    string tempV = strPV + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 1+Voltage [V]"].ToString() + ":V|";
                    //    string tempA = strPA + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 1+Bondforce [cN]"].ToString() + ":A|";
                    //    string tempT = strPT + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 1+Force ramp [ms]"].ToString() + ":S|";
                    //    strP = strP + tempV + tempA + tempT;
                    //    pIndex++;
                    //    tempV = strPV + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 2+Voltage [V]"].ToString() + ":V|";
                    //    tempA = strPA + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 2+Bondforce [cN]"].ToString() + ":A|";
                    //    tempT = strPT + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 2+Force ramp [ms]"].ToString() + ":S|";
                    //    strP = strP + tempV + tempA + tempT;
                    //    pIndex++;
                    //    tempV = strPV + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 3+Voltage [V]"].ToString() + ":V|";
                    //    tempA = strPA + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 3+Bondforce [cN]"].ToString() + ":A|";
                    //    tempT = strPT + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 3+Force ramp [ms]"].ToString() + ":S|";
                    //    strP = strP + tempV + tempA + tempT;
                      
                    //}
                    //else if (dt.Rows[i]["Traceability - reference system"].ToString().ToUpper() == "1") //铝片

                    //if (dtTemp1.Rows[i]["Traceability - reference system"].ToString().ToUpper() == "1") //铝片
                    //{
                        lvIndex++;
                        string tempV = strLvV + lvIndex.ToString() + ":" + dtTemp1.Rows[i]["Traceability process parameter+Process phase+Process phase 1+Voltage [V]"].ToString() + ":V|";
                        string tempA = strLvA + lvIndex.ToString() + ":" + dtTemp1.Rows[i]["Traceability process parameter+Process phase+Process phase 1+Bondforce [cN]"].ToString() + ":A|";
                        string tempT = strLvT + lvIndex.ToString() + ":" + dtTemp1.Rows[i]["Traceability process parameter+Process phase+Process phase 1+Force ramp [ms]"].ToString() + ":S|";
                        strLv = strLv + tempV + tempA + tempT;
                        lvIndex++;
                        tempV = strLvV + lvIndex.ToString() + ":" + dtTemp1.Rows[i]["Traceability process parameter+Process phase+Process phase 2+Voltage [V]"].ToString() + ":V|";
                        tempA = strLvA + lvIndex.ToString() + ":" + dtTemp1.Rows[i]["Traceability process parameter+Process phase+Process phase 2+Bondforce [cN]"].ToString() + ":A|";
                        tempT = strLvT + lvIndex.ToString() + ":" + dtTemp1.Rows[i]["Traceability process parameter+Process phase+Process phase 2+Force ramp [ms]"].ToString() + ":S|";
                        strLv = strLv + tempV + tempA + tempT;
                        lvIndex++;
                        tempV = strLvV + lvIndex.ToString() + ":" + dtTemp1.Rows[i]["Traceability process parameter+Process phase+Process phase 3+Voltage [V]"].ToString() + ":V|";
                        tempA = strLvA + lvIndex.ToString() + ":" + dtTemp1.Rows[i]["Traceability process parameter+Process phase+Process phase 3+Bondforce [cN]"].ToString() + ":A|";
                        tempT = strLvT + lvIndex.ToString() + ":" + dtTemp1.Rows[i]["Traceability process parameter+Process phase+Process phase 3+Force ramp [ms]"].ToString() + ":S|";
                        strLv = strLv + tempV + tempA + tempT;
                       
                    //}
                   
                }
                
            }

            DataTable dtTemp23 = GetNewDataTable(dt, "[Traceability - reference system]<>'1'");
            for (int i = 0; i < SysCfgModel.batteryNumInMod; i++)
            {
                if (i > dtTemp23.Rows.Count - 1)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        pIndex++;
                        string tempV = strPV + pIndex.ToString() + ":" + "0" + ":V|";
                        string tempA = strPA + pIndex.ToString() + ":" + "0" + ":A|";
                        string tempT = strPT + pIndex.ToString() + ":" + "0" + ":S|";
                        strP = strP + tempV + tempA + tempT;
                    }
                    //for (int j = 0; j < 3; j++)
                    //{
                    //    lvIndex++;
                    //    string tempV = strLvV + lvIndex.ToString() + ":" + "0" + ":V|";
                    //    string tempA = strLvA + lvIndex.ToString() + ":" + "0" + ":A|";
                    //    string tempT = strLvT + lvIndex.ToString() + ":" + "0" + ":S|";
                    //    strLv = strLv + tempV + tempA + tempT;
                    //}

                }
                else
                {
                    //if (dt.Rows[i]["Traceability - reference system"].ToString().ToUpper() == "2" || dt.Rows[i]["Traceability - reference system"].ToString().ToUpper() == "3") //正负极
                    //{
                        pIndex++;
                        string tempV = strPV + pIndex.ToString() + ":" + dtTemp23.Rows[i]["Traceability process parameter+Process phase+Process phase 1+Voltage [V]"].ToString() + ":V|";
                        string tempA = strPA + pIndex.ToString() + ":" + dtTemp23.Rows[i]["Traceability process parameter+Process phase+Process phase 1+Bondforce [cN]"].ToString() + ":A|";
                        string tempT = strPT + pIndex.ToString() + ":" + dtTemp23.Rows[i]["Traceability process parameter+Process phase+Process phase 1+Force ramp [ms]"].ToString() + ":S|";
                        strP = strP + tempV + tempA + tempT;
                        pIndex++;
                        tempV = strPV + pIndex.ToString() + ":" + dtTemp23.Rows[i]["Traceability process parameter+Process phase+Process phase 2+Voltage [V]"].ToString() + ":V|";
                        tempA = strPA + pIndex.ToString() + ":" + dtTemp23.Rows[i]["Traceability process parameter+Process phase+Process phase 2+Bondforce [cN]"].ToString() + ":A|";
                        tempT = strPT + pIndex.ToString() + ":" + dtTemp23.Rows[i]["Traceability process parameter+Process phase+Process phase 2+Force ramp [ms]"].ToString() + ":S|";
                        strP = strP + tempV + tempA + tempT;
                        pIndex++;
                        tempV = strPV + pIndex.ToString() + ":" + dtTemp23.Rows[i]["Traceability process parameter+Process phase+Process phase 3+Voltage [V]"].ToString() + ":V|";
                        tempA = strPA + pIndex.ToString() + ":" + dtTemp23.Rows[i]["Traceability process parameter+Process phase+Process phase 3+Bondforce [cN]"].ToString() + ":A|";
                        tempT = strPT + pIndex.ToString() + ":" + dtTemp23.Rows[i]["Traceability process parameter+Process phase+Process phase 3+Force ramp [ms]"].ToString() + ":S|";
                        strP = strP + tempV + tempA + tempT;

                    //}
                    //else if (dt.Rows[i]["Traceability - reference system"].ToString().ToUpper() == "1") //铝片
                    //{
                    //    lvIndex++;
                    //    string tempV = strLvV + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 1+Voltage [V]"].ToString() + ":V|";
                    //    string tempA = strLvA + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 1+Bondforce [cN]"].ToString() + ":A|";
                    //    string tempT = strLvT + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 1+Force ramp [ms]"].ToString() + ":S|";
                    //    strLv = strLv + tempV + tempA + tempT;
                    //    lvIndex++;
                    //    tempV = strLvV + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 2+Voltage [V]"].ToString() + ":V|";
                    //    tempA = strLvA + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 2+Bondforce [cN]"].ToString() + ":A|";
                    //    tempT = strLvT + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 2+Force ramp [ms]"].ToString() + ":S|";
                    //    strLv = strLv + tempV + tempA + tempT;
                    //    lvIndex++;
                    //    tempV = strLvV + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 3+Voltage [V]"].ToString() + ":V|";
                    //    tempA = strLvA + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 3+Bondforce [cN]"].ToString() + ":A|";
                    //    tempT = strLvT + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 3+Force ramp [ms]"].ToString() + ":S|";
                    //    strLv = strLv + tempV + tempA + tempT;

                    //}

                }

            }
            #endregion
            #region 拉力检测结果
            string strPullSwitch2 = "正负极拉力检测开关";
            string strPullRe2 = "正负极拉力检测结果";
            string strPullSwitch1 = "铝片拉力检测开关";
            string strPullRe1 = "铝片拉力检测结果";
            string strPullSum = "";
            int pullIndex1 = 0;
            int pullIndex2 = 0;
            DataTable dtPullTemp1 = GetNewDataTable(dtPull, "[Traceability - reference system]='1'");
            for (int i = 0; i < SysCfgModel.batteryNumInMod;i++ )
            {
                if (i > dtPullTemp1.Rows.Count - 1)
                {
                    pullIndex1++;
                    strPullSum = strPullSum + string.Format("{0}{1}:{2}:|{3}{4}:{5}:|", strPullSwitch1, pullIndex1, 0, strPullRe1,pullIndex1, 0);
                    //pullIndex2++;
                    //strPullSum = strPullSum + string.Format("{0}{1}:{2}:|{3}{4}:{5}:|", strPullSwitch2, pullIndex2, 0, strPullRe2, pullIndex2, 0);
                }
                else
                {
                    //if (dtPullTemp1.Rows[i]["Traceability - reference system"].ToString().ToUpper() == "1")
                    //{
                        pullIndex1++;
                        int pullSwitch = 0;
                        int pullRe = 0;
                        if (dtPullTemp1.Rows[i]["Traceability - integrated pull test - values available"].ToString().ToUpper() == "TRUE")
                        {
                            pullSwitch = 1;
                        }
                        if (dtPullTemp1.Rows[i]["Traceability - integrated pull test - passed"].ToString().ToUpper() == "TRUE")
                        {
                            pullRe = 1;
                        }
                        strPullSum = strPullSum + string.Format("{0}{1}:{2}:|{3}{4}:{5}:|", strPullSwitch1, pullIndex1, pullSwitch, strPullRe1, pullIndex1, pullRe);
                    //}
                    //else
                    //{
                    //    pullIndex2++;
                    //    int pullSwitch = 0;
                    //    int pullRe = 0;
                    //    if (dtPull.Rows[i]["Traceability - integrated pull test - values available"].ToString().ToUpper() == "TRUE")
                    //    {
                    //        pullSwitch = 1;
                    //    }
                    //    if (dtPull.Rows[i]["Traceability - integrated pull test - passed"].ToString().ToUpper() == "TRUE")
                    //    {
                    //        pullRe = 1;
                    //    }
                    //    strPullSum = strPullSum + string.Format("{0}{1}:{2}:|{3}{4}:{5}:|", strPullSwitch2, pullIndex2, pullSwitch, strPullRe2, pullIndex2, pullRe);
                    //}
                  
                }
            }
            DataTable dtpullTemp23 = GetNewDataTable(dtPull, "[Traceability - reference system]<>'1'");
            for (int i = 0; i < SysCfgModel.batteryNumInMod; i++)
            {
                if (i > dtpullTemp23.Rows.Count - 1)
                {
                    //pullIndex1++;
                    //strPullSum = strPullSum + string.Format("{0}{1}:{2}:|{3}{4}:{5}:|", strPullSwitch1, pullIndex1, 0, strPullRe1, pullIndex1, 0);
                    pullIndex2++;
                    strPullSum = strPullSum + string.Format("{0}{1}:{2}:|{3}{4}:{5}:|", strPullSwitch2, pullIndex2, 0, strPullRe2, pullIndex2, 0);
                }
                else
                {
                    //if (dtPull.Rows[i]["Traceability - reference system"].ToString().ToUpper()!= "1")
                    //{
                    //    pullIndex1++;
                    //    int pullSwitch = 0;
                    //    int pullRe = 0;
                    //    if (dtPull.Rows[i]["Traceability - integrated pull test - values available"].ToString().ToUpper() == "TRUE")
                    //    {
                    //        pullSwitch = 1;
                    //    }
                    //    if (dtPull.Rows[i]["Traceability - integrated pull test - passed"].ToString().ToUpper() == "TRUE")
                    //    {
                    //        pullRe = 1;
                    //    }
                    //    strPullSum = strPullSum + string.Format("{0}{1}:{2}:|{3}{4}:{5}:|", strPullSwitch1, pullIndex1, pullSwitch, strPullRe1, pullIndex1, pullRe);
                    //}
                    //else
                    //{
                        pullIndex2++;
                        int pullSwitch = 0;
                        int pullRe = 0;
                        if (dtpullTemp23.Rows[i]["Traceability - integrated pull test - values available"].ToString().ToUpper() == "TRUE")
                        {
                            pullSwitch = 1;
                        }
                        if (dtpullTemp23.Rows[i]["Traceability - integrated pull test - passed"].ToString().ToUpper() == "TRUE")
                        {
                            pullRe = 1;
                        }
                        strPullSum = strPullSum + string.Format("{0}{1}:{2}:|{3}{4}:{5}:|", strPullSwitch2, pullIndex2, pullSwitch, strPullRe2, pullIndex2, pullRe);
                    }

                //}
            }
            #endregion
            string str = strP + strLv + strN+strPullSum;
            string itemVal = str.Substring(0, str.Length - 1);
            return itemVal;
        }
    }

    public enum ENUMWeldStatus
    {
        SENDED,//记录数据已发送至焊机上位机
        COMPLETE  //铝丝焊焊接加工完成
    }
}
