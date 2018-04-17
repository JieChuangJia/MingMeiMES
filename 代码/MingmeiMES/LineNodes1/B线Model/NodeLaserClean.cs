using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PLProcessModel;
using System.Diagnostics;
namespace LineNodes
{
    public class NodeLaserClean : CtlNodeBaseModel
    {
        string cleanerIP = "";
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
            if(this.nodeID == "OPB002")
            {
                this.cleanerIP = "192.168.0.40";
            }
            else if(this.nodeID == "OPB005")
            {
                this.cleanerIP = "192.168.0.47";
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
                            currentTaskPhase = 4;
                            break;
                        }
                        string cleanerSndFile = "";
                        cleanerSndFile = string.Format(@"\\{0}\MES交互文件\加工产品\加工产品.txt", cleanerIP); // @"\\192.168.0.45\MESReport\DeviceInfoLane1.txt";
                        if (!System.IO.File.Exists(cleanerSndFile))
                        {
                            currentTaskDescribe = string.Format("清洗机文件：{0}不存在", cleanerSndFile);
                            return false;
                        }
                        System.IO.StreamWriter writter = new System.IO.StreamWriter(cleanerSndFile, false);
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
                        logRecorder.AddDebugLog(nodeName, "写入清洗机:" + strBuild.ToString());
                       
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 3:
                    {
                        if (this.db2Vals[2 + channelIndex] != 2)
                        {
                            currentTaskDescribe = "等待设备工作完成";
                            break;
                        }

                       // List<DBAccess.Model.BatteryModuleModel> modList = modBll.GetModelList(string.Format("palletID='{0}' and palletBinded=1", this.rfidUID));
                        List<DBAccess.Model.BatteryModuleModel> modList = modBll.GetModelList(string.Format("palletID='{0}'", this.rfidUID));
                        if (modList.Count() < 1)
                        {
                            currentTaskPhase = 4;
                            break;
                        }

                        //读取产品信息，加工数据（需要添加）
                        string cleanerReadFile = string.Format(@"\\{0}\MES交互文件\加工参数\加工参数.txt", cleanerIP);
                        if (!System.IO.File.Exists(cleanerReadFile))
                        {
                            currentTaskDescribe = string.Format("清洗机文件：{0}不存在", cleanerReadFile);
                            return false;
                        }
                        System.IO.StreamReader reader = new System.IO.StreamReader(cleanerReadFile, Encoding.Default);
                        string line;
                        string M_WORKSTATION_SN = "Y00101501";
                        if (this.nodeID == "OPB005")
                        {
                            M_WORKSTATION_SN = "Y00101801";
                        }
                        while ((line = reader.ReadLine()) != null)
                        {
                            Console.WriteLine(this.nodeName + "加工数据" + line);
                            if(isWithMes == true)
                            {
                                string[] strArray = line.Split(',');
                                if (strArray.Length == 5)
                                {
                                    int M_FLAG = 3;
                                    string M_DEVICE_SN = "";
                                    string M_SN = strArray[1];
                                    string M_UNION_SN = "";
                                    string M_CONTAINER_SN = "";
                                    string M_LEVEL = "";
                                    //string M_ITEMVALUE = "清洗功率:" + strArray[2] + ":W";
                                    string M_ITEMVALUE = "清洗功率:" + strArray[2] + ":W|速度:" + strArray[3]+":m/s|频率:" + strArray[4] + ":s/次";
                                    RootObject rObj = new RootObject();
                                    rObj = WShelper.DevDataUpload(M_FLAG, M_DEVICE_SN, M_WORKSTATION_SN, M_SN, M_UNION_SN, M_CONTAINER_SN, M_LEVEL, M_ITEMVALUE);
                                    if (rObj.CONTROL_TYPE == "STOP" && rObj.RES == "OK")
                                    {
                                        Console.WriteLine(this.nodeName + "CONTROL_TYPE = STOP");
                                    }
                                    currentTaskDescribe = this.nodeName + rObj.CONTROL_TYPE + "," + M_SN;
                                }
                            }
                           
                        }
                      
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 4:
                    {
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

    }
}
