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
                            currentTaskPhase = 5;
                            break;
                        }
                        string welderSndFile = string.Format(@"\\{0}\MESReport\DeviceInfoLane{1}.txt", welderIP, channelIndex); // @"\\192.168.0.45\MESReport\DeviceInfoLane1.txt";
                        if (!System.IO.File.Exists(welderSndFile))
                        {
                            currentTaskDescribe = string.Format("铝丝焊文件：{0}不存在", welderSndFile);
                            return false;
                        }
                        System.IO.StreamWriter writter = new System.IO.StreamWriter(welderSndFile, false);
                        StringBuilder strBuild = new StringBuilder();
                        for (int i = 1; i < 5; i++)
                        {
                            string str = string.Format("{0},{1},", i, this.rfidUID);
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

                     
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);


                        //List<DBAccess.Model.BatteryModuleModel> modList = modBll.GetModelList(string.Format("palletID='{0}' and palletBinded=1", this.rfidUID));
                        //if (modList.Count() < 1)//空板直接放行
                        //{
                        //    currentTaskPhase = 4;
                        //    break;
                        //}
                        break;
                    }
                case 3:
                    {
                        if(this.db2Vals[2+channelIndex] != 2)
                        {
                            currentTaskDescribe = "等待设备工作完成";
                            break;
                        }
                        List<DBAccess.Model.BatteryModuleModel> modList = modBll.GetModelList(string.Format("palletID='{0}' and palletBinded=1", this.rfidUID));
                        if (modList.Count() < 1) //空板直接放行
                        {
                            currentTaskPhase = 5;
                            break;
                        }
                        bool isReceive = true;
                        for(int i = 0;i< modList.Count;i++)
                        {
                          
                            string testResultPath = string.Format(@"\\{0}\MESReport\PullTestResult\{1}_{2}.csv", welderIP, modList[i].palletID, modList[i].batModuleID);
                            if (!System.IO.File.Exists(testResultPath))
                            {
                                currentTaskDescribe = string.Format(this.nodeName + "铝丝焊结果文件：{0}不存在", testResultPath);
                                isReceive = false;
                                break;
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
                                modList[i].checkResult = 2;
                                modList[i].palletBinded = false;
                                modList[i].tag3 = this.nodeName; //标识在哪个工位产生的NG,(共有三个工位，1#铝丝焊、2#铝丝焊、DCIR检测)
                            }
                            else
                            {

                                modList[i].checkResult = 1;
                            }
                            modBll.Update(modList[i]);
                        }
                        if(isReceive == false)
                        {
                            break;
                        }
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        
                        break;
                    }
                //case 4:
                //    {
                //        List<DBAccess.Model.BatteryModuleModel> modList = modBll.GetModelList(string.Format("palletID='{0}' and palletBinded=1", this.rfidUID));
                //        if (modList.Count() < 1) //空板直接放行
                //        {
                //            currentTaskPhase = 5;
                //            break;
                //        }
                //        //if (!AfterMechFromSoft(modList, ref reStr))
                //        //{
                //        //    Console.WriteLine(string.Format("{0},{1}", nodeName, reStr));
                //        //    break;
                //        //}
                //        //if (!SendCheckResult(modList, ref reStr))
                //        //{
                //        //    Console.WriteLine(string.Format("{0},{1}", nodeName, reStr));
                //        //    break;
                //        //}
                //        currentTaskPhase++;
                //        this.currentTask.TaskPhase = this.currentTaskPhase;
                //        this.ctlTaskBll.Update(this.currentTask);
                //        break;
                //    }
                case 4:
                    {
                        if (isWithMes == false)
                        {
                            currentTaskPhase = 5;
                            break;
                        }
                        currentTaskDescribe = "上传MES数据";
                        List<DBAccess.Model.BatteryModuleModel> modList = modBll.GetModelList(string.Format("palletID='{0}'", this.rfidUID));
                        if (modList.Count() < 1) //空板直接放行
                        {
                            currentTaskPhase = 5;
                            break;
                        }
                        bool isReceive = true;
                        string M_WORKSTATION_SN = "";
                        if (this.nodeID == "OPB004")
                        {
                            M_WORKSTATION_SN = "Y00101701";
                        }
                        else
                        {
                            M_WORKSTATION_SN = "Y00102001";
                        }

                        for (int i = 0; i < modList.Count; i++)
                        {
                            if (modList[i].palletBinded == false && modList[i].tag3 != null && modList[i].tag3 != this.nodeName)
                            {
                                continue;
                            }
                            string testResultPath = string.Format(@"\\{0}\MESReport\BondParameters\{1}_{2}.csv", welderIP, modList[i].palletID, modList[i].batModuleID);
                            if (!System.IO.File.Exists(testResultPath))
                            {
                                currentTaskDescribe = string.Format(this.nodeName + "铝丝焊参数文件：{0}不存在", testResultPath);
                                isReceive = false;
                                break;
                            }
                            DataTable dt = CSVFileHelper.OpenCSV(testResultPath);
                            int M_FLAG = 3;
                            string M_DEVICE_SN = "";
                            string M_SN = modList[i].batModuleID;
                            string M_UNION_SN = "";
                            string M_CONTAINER_SN = "";
                            string M_LEVEL = "";
                            string M_ITEMVALUE = GetItemVal(dt);
                            //上传数据
                            RootObject rObj = new RootObject();
                            string strJson = "";
                            rObj = WShelper.DevDataUpload(M_FLAG, M_DEVICE_SN, M_WORKSTATION_SN, M_SN, M_UNION_SN, M_CONTAINER_SN, M_LEVEL, M_ITEMVALUE,ref strJson);
                            currentTaskDescribe = rObj.RES + "," + M_FLAG + "," + M_WORKSTATION_SN + "," + M_ITEMVALUE + this.nodeName + rObj.CONTROL_TYPE;
                            Console.WriteLine(rObj.RES);
                            Console.WriteLine(M_ITEMVALUE);
                            logRecorder.AddDebugLog(nodeName, "上传mes数据:" + M_ITEMVALUE);
                        }
                        if (isReceive == false)
                        {
                            break;
                        }
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;;
                    }
                case 5:
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
                            
                            if (WriteProductsToWelder(1, rfidUIDA))
                            {
                                logRecorder.AddDebugLog(nodeName, string.Format("A通道读到RFID:{0}", this.rfidUIDA));
                                Thread.Sleep(5000);
                                this.db1ValsToSnd[0] = 2;
                              
                            }
                            else
                            {
                                //rfidUIDA = "";
                                this.db1ValsToSnd[0] = 2;
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

        private string GetItemVal(DataTable dt)
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
            int nIndex = 0;
            int lvIndex = 0;
            #region
           // for (int i = 0; i < dt.Rows.Count; i++)
            for (int i = 0; i < SysCfgModel.batteryNumInMod;i++ )
            {
                if (i > dt.Rows.Count-1)
                {
                    for(int j = 0;j<8;j++)
                    {
                        pIndex++;
                        string tempV = strPV + pIndex.ToString() + ":" + "0" + ":V|";
                        string tempA = strPA + pIndex.ToString() + ":" + "0" + ":A|";
                        string tempT = strPT + pIndex.ToString() + ":" + "0" + ":S|";
                        strP = strP + tempV + tempA + tempT;
                    }
                    for (int j = 0; j < 8; j++)
                    {
                        lvIndex++;


                        string tempV = strLvV + lvIndex.ToString() + ":" +"0" + ":V|";
                        string tempA = strLvA + lvIndex.ToString() + ":" +"0" + ":A|";
                        string tempT = strLvT + lvIndex.ToString() + ":" +"0" + ":S|";
                        strLv = strLv + tempV + tempA + tempT;
                    }
                    for (int j = 0; j < 8; j++)
                    {
                        pIndex++;
                        string tempV = strPV + pIndex.ToString() + ":" + "0" + ":V|";
                        string tempA = strPA + pIndex.ToString() + ":" + "0" + ":A|";
                        string tempT = strPT + pIndex.ToString() + ":" + "0" + ":S|";
                        strP = strP + tempV + tempA + tempT;
                    }
                }
                else
                {
                    if (dt.Rows[i]["Traceability - reference system"].ToString().ToUpper() == "2") //正极
                    {
                        pIndex++;
                        string tempV = strPV + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 1+Voltage [V]"].ToString() + ":V|";
                        string tempA = strPA + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 1+Bondforce [cN]"].ToString() + ":A|";
                        string tempT = strPT + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 1+Force ramp [ms]"].ToString() + ":S|";
                        strP = strP + tempV + tempA + tempT;
                        pIndex++;
                        tempV = strPV + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 2+Voltage [V]"].ToString() + ":V|";
                        tempA = strPA + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 2+Bondforce [cN]"].ToString() + ":A|";
                        tempT = strPT + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 2+Force ramp [ms]"].ToString() + ":S|";
                        strP = strP + tempV + tempA + tempT;
                        pIndex++;
                        tempV = strPV + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 3+Voltage [V]"].ToString() + ":V|";
                        tempA = strPA + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 3+Bondforce [cN]"].ToString() + ":A|";
                        tempT = strPT + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 3+Force ramp [ms]"].ToString() + ":S|";
                        strP = strP + tempV + tempA + tempT;
                        pIndex++;
                        tempV = strPV + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 4+Voltage [V]"].ToString() + ":V|";
                        tempA = strPA + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 4+Bondforce [cN]"].ToString() + ":A|";
                        tempT = strPT + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 4+Force ramp [ms]"].ToString() + ":S|";
                        strP = strP + tempV + tempA + tempT;
                        pIndex++;
                        tempV = strPV + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 5+Voltage [V]"].ToString() + ":V|";
                        tempA = strPA + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 5+Bondforce [cN]"].ToString() + ":A|";
                        tempT = strPT + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 5+Force ramp [ms]"].ToString() + ":S|";
                        strP = strP + tempV + tempA + tempT;
                        pIndex++;
                        tempV = strPV + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 6+Voltage [V]"].ToString() + ":V|";
                        tempA = strPA + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 6+Bondforce [cN]"].ToString() + ":A|";
                        tempT = strPT + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 6+Force ramp [ms]"].ToString() + ":S|";
                        strP = strP + tempV + tempA + tempT;
                        pIndex++;
                        tempV = strPV + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 7+Voltage [V]"].ToString() + ":V|";
                        tempA = strPA + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 7+Bondforce [cN]"].ToString() + ":A|";
                        tempT = strPT + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 7+Force ramp [ms]"].ToString() + ":S|";
                        strP = strP + tempV + tempA + tempT;
                        pIndex++;
                        tempV = strPV + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 8+Voltage [V]"].ToString() + ":V|";
                        tempA = strPA + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 8+Bondforce [cN]"].ToString() + ":A|";
                        tempT = strPT + pIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 8+Force ramp [ms]"].ToString() + ":S|";
                        strP = strP + tempV + tempA + tempT;
                    }
                    else if (dt.Rows[i]["Traceability - reference system"].ToString().ToUpper() == "1") //铝片
                    {
                        lvIndex++;
                        string tempV = strLvV + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 1+Voltage [V]"].ToString() + ":V|";
                        string tempA = strLvA + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 1+Bondforce [cN]"].ToString() + ":A|";
                        string tempT = strLvT + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 1+Force ramp [ms]"].ToString() + ":S|";
                        strLv = strLv + tempV + tempA + tempT;
                        lvIndex++;
                        tempV = strLvV + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 2+Voltage [V]"].ToString() + ":V|";
                        tempA = strLvA + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 2+Bondforce [cN]"].ToString() + ":A|";
                        tempT = strLvT + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 2+Force ramp [ms]"].ToString() + ":S|";
                        strLv = strLv + tempV + tempA + tempT;
                        lvIndex++;
                        tempV = strLvV + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 3+Voltage [V]"].ToString() + ":V|";
                        tempA = strLvA + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 3+Bondforce [cN]"].ToString() + ":A|";
                        tempT = strLvT + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 3+Force ramp [ms]"].ToString() + ":S|";
                        strLv = strLv + tempV + tempA + tempT;
                        lvIndex++;
                        tempV = strLvV + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 4+Voltage [V]"].ToString() + ":V|";
                        tempA = strLvA + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 4+Bondforce [cN]"].ToString() + ":A|";
                        tempT = strLvT + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 4+Force ramp [ms]"].ToString() + ":S|";
                        strLv = strLv + tempV + tempA + tempT;
                        lvIndex++;
                        tempV = strLvV + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 5+Voltage [V]"].ToString() + ":V|";
                        tempA = strLvA + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 5+Bondforce [cN]"].ToString() + ":A|";
                        tempT = strLvT + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 5+Force ramp [ms]"].ToString() + ":S|";
                        strLv = strLv + tempV + tempA + tempT;
                        lvIndex++;
                        tempV = strLvV + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 6+Voltage [V]"].ToString() + ":V|";
                        tempA = strLvA + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 6+Bondforce [cN]"].ToString() + ":A|";
                        tempT = strLvT + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 6+Force ramp [ms]"].ToString() + ":S|";
                        strLv = strLv + tempV + tempA + tempT;
                        lvIndex++;
                        tempV = strLvV + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 7+Voltage [V]"].ToString() + ":V|";
                        tempA = strLvA + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 7+Bondforce [cN]"].ToString() + ":A|";
                        tempT = strLvT + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 7+Force ramp [ms]"].ToString() + ":S|";
                        strLv = strLv + tempV + tempA + tempT;
                        lvIndex++;
                        tempV = strLvV + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 8+Voltage [V]"].ToString() + ":V|";
                        tempA = strLvA + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 8+Bondforce [cN]"].ToString() + ":A|";
                        tempT = strLvT + lvIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 8+Force ramp [ms]"].ToString() + ":S|";
                        strLv = strLv + tempV + tempA + tempT;
                    }
                    else if (dt.Rows[i]["Traceability - reference system"].ToString().ToUpper() == "3") //负极
                    {
                        //nIndex++;
                        nIndex = pIndex + 1;
                        string tempV = strPV + nIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 1+Voltage [V]"].ToString() + ":V|";
                        string tempA = strPA + nIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 1+Bondforce [cN]"].ToString() + ":A|";
                        string tempT = strPT + nIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 1+Force ramp [ms]"].ToString() + ":S|";
                        strN = strN + tempV + tempA + tempT;
                        nIndex++;
                        tempV = strPV + nIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 2+Voltage [V]"].ToString() + ":V|";
                        tempA = strPA + nIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 2+Bondforce [cN]"].ToString() + ":A|";
                        tempT = strPT + nIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 2+Force ramp [ms]"].ToString() + ":S|";
                        strN = strN + tempV + tempA + tempT;
                        nIndex++;
                        tempV = strPV + nIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 3+Voltage [V]"].ToString() + ":V|";
                        tempA = strPA + nIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 3+Bondforce [cN]"].ToString() + ":A|";
                        tempT = strPT + nIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 3+Force ramp [ms]"].ToString() + ":S|";
                        strN = strN + tempV + tempA + tempT;
                        nIndex++;
                        tempV = strPV + nIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 4+Voltage [V]"].ToString() + ":V|";
                        tempA = strPA + nIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 4+Bondforce [cN]"].ToString() + ":A|";
                        tempT = strPT + nIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 4+Force ramp [ms]"].ToString() + ":S|";
                        strN = strN + tempV + tempA + tempT;
                        nIndex++;
                        tempV = strPV + nIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 5+Voltage [V]"].ToString() + ":V|";
                        tempA = strPA + nIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 5+Bondforce [cN]"].ToString() + ":A|";
                        tempT = strPT + nIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 5+Force ramp [ms]"].ToString() + ":S|";
                        strN = strN + tempV + tempA + tempT;
                        nIndex++;
                        tempV = strPV + nIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 6+Voltage [V]"].ToString() + ":V|";
                        tempA = strPA + nIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 6+Bondforce [cN]"].ToString() + ":A|";
                        tempT = strPT + nIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 6+Force ramp [ms]"].ToString() + ":S|";
                        strN = strN + tempV + tempA + tempT;
                        nIndex++;
                        tempV = strPV + nIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 7+Voltage [V]"].ToString() + ":V|";
                        tempA = strPA + nIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 7+Bondforce [cN]"].ToString() + ":A|";
                        tempT = strPT + nIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 7+Force ramp [ms]"].ToString() + ":S|";
                        strN = strN + tempV + tempA + tempT;
                        nIndex++;
                        tempV = strPV + nIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 8+Voltage [V]"].ToString() + ":V|";
                        tempA = strPA + nIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 8+Bondforce [cN]"].ToString() + ":A|";
                        tempT = strPT + nIndex.ToString() + ":" + dt.Rows[i]["Traceability process parameter+Process phase+Process phase 8+Force ramp [ms]"].ToString() + ":S|";
                        strN = strN + tempV + tempA + tempT;
                    }
                }
                
            }
            #endregion
            string str = strP + strLv + strN;
            string itemVal = str.Substring(0, str.Length - 1);
            return itemVal;
        }
    }
}
