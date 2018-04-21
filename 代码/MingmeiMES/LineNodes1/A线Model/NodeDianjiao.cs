using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PLProcessModel;
namespace LineNodes
{
    public class NodeDianjiao:CtlNodeBaseModel
    {
        private string trsnFilePath = "";
        private string inputFilePath = "";
        public override bool BuildCfg(System.Xml.Linq.XElement xe, ref string reStr)
        {
            if (!base.BuildCfg(xe, ref reStr))
            {
                return false;
            }
            this.dicCommuDataDB1[1].DataDescription = "A通道读卡结果，1：复位/待机状态,2：RFID读取成功,3：RFID读取失败,4：无绑定数据";
            this.dicCommuDataDB1[2].DataDescription = "B通道读卡结果,1：复位/待机状态,2：RFID读取成功,3：RFID读取失败,4：无绑定数据";
            this.dicCommuDataDB1[3].DataDescription = "1：复位/待机状态,2：数据读取中,3：数据读取完毕，放行";
         
            this.dicCommuDataDB2[1].DataDescription = "0：无,1：正在运行A通道,2：正在运行B通道";
            this.dicCommuDataDB2[2].DataDescription = "1：A通道无板,2：A通道有板，读卡请求";
            this.dicCommuDataDB2[3].DataDescription = "1：B通道无板,2：B通道有板，读卡请求";
            this.dicCommuDataDB2[4].DataDescription = "A通道工作完成状态,1：复位，2：点胶完成";
            this.dicCommuDataDB2[5].DataDescription = "B通道工作完成状态,1：复位，2：点胶完成";
            if (this.nodeID == "OPA011")
            {
                trsnFilePath = @"\\192.168.0.6\Info\Info.txt";
            }
            else if (this.nodeID == "OPA012")
            {
                trsnFilePath = @"\\192.168.0.5\Info\Info.txt";
            }
            else if(this.nodeID=="OPA007")
            {
                trsnFilePath = @"\\192.168.0.25\Info\Info.txt";
                inputFilePath = @"\\192.168.0.25\Info\input.txt";
            }
            else if(this.nodeID=="OPA008")
            {
                trsnFilePath = @"\\192.168.0.28\Info\Info.txt";
                inputFilePath = @"\\192.168.0.28\Info\input.txt";
            }
            if(SysCfgModel.SimMode)
            {
                trsnFilePath = @"E:\workspace\项目\明美项目\test\Info.txt";
                inputFilePath = @"E:\workspace\项目\明美项目\test\input.txt";
            }
            //trsnFilePath = @"E:\workspace\项目\明美项目\test\Info.txt";
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
            if (this.nodeID == "OPA011" || this.nodeID == "OPA012")
            {
                return ExeBusinessOP12(ref reStr);
            }
            else
            {
                return ExeBusinessOP34(ref reStr);
            }

            
        }
        public bool ExeBusinessOP12(ref string reStr)
        {
            switch(currentTaskPhase)
            {
                case 1:
                    {
                        this.db1ValsToSnd[channelIndex - 1] = 2;
                        this.currentTaskDescribe = "等待设备加工完成";
                        if(this.db2Vals[channelIndex+2] != 2)
                        {
                            break;
                        }
                        currentTaskPhase++;
                        this.db1ValsToSnd[2] = 2;
                        break;
                    }
                case 2:
                    {
                        this.currentTaskDescribe = "开始解析点胶机数据";
                        if (!System.IO.File.Exists(trsnFilePath))
                        {
                            this.currentTaskDescribe = string.Format("点胶机交互文件：{0}不存在", trsnFilePath);
                            
                            break;
                        }
                        System.IO.StreamReader reader = new System.IO.StreamReader(trsnFilePath,Encoding.Default);
                        string mesItemStr = "";
                        while(!reader.EndOfStream)
                        {
                            string str = reader.ReadLine();
                            string[] strArray = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                            if(strArray.Count()<2)
                            {
                                continue;
                            }
                            if(strArray[0] == "胶量")
                            {
                                if(string.IsNullOrWhiteSpace(mesItemStr))
                                {
                                    mesItemStr += string.Format("胶量:{0}:mg", strArray[1]);
                                }
                                else
                                {
                                    mesItemStr += string.Format("|胶量:{0}:mg", strArray[1]);
                                }
                            }
                        }
                        if (string.IsNullOrWhiteSpace(mesItemStr))
                        {
                            currentTaskDescribe = "点胶数据解析错误，结果为空";
                            break;
                        }
                        //1,2点胶工位，只上传过程参数
                       
                        string M_AREA = "Y001";
                        string M_WORKSTATION_SN = "Y00100201";
                        string M_DEVICE_SN = "";
                        // string M_SN = modCode;
                        string M_UNION_SN = "";
                        string M_CONTAINER_SN = "";
                        string M_LEVEL = "";
                        string M_ITEMVALUE = mesItemStr;
                        RootObject rObj = new RootObject();
                        this.currentTaskDescribe = "开始上传MES点胶机过程参数";
                      
                        string strJson = "";
                        rObj = WShelper.ProcParamUpload(M_AREA, M_DEVICE_SN, M_WORKSTATION_SN, M_UNION_SN, M_CONTAINER_SN, M_LEVEL, M_ITEMVALUE, ref strJson);
                        logRecorder.AddDebugLog(nodeName, string.Format("点胶机结果{0}上传MES，返回{1}", mesItemStr, rObj.RES));
                        this.currentTaskDescribe = string.Format("点胶机结果{0}上传MES，返回{1}", mesItemStr, rObj.RES);
                        this.db1ValsToSnd[2] = 3;
                        currentTaskPhase++;
                        break;
                    }
                case 3:
                    {
                        this.currentTaskDescribe = "流程完成";
                        break;
                    }
             
                default:
                    break;
            }
            return true;
        }
        public bool ExeBusinessOP34(ref string reStr)
        {
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
                        List<DBAccess.Model.BatteryModuleModel> modList = modBll.GetModelList(string.Format("palletID='{0}' and palletBinded=1", this.rfidUID));
                        if (modList.Count() < 1)
                        {
                            currentTaskPhase = 4;
                            break;
                        }
                      // @"\\192.168.0.45\MESReport\DeviceInfoLane1.txt";
                        if (!System.IO.File.Exists(inputFilePath))
                        {
                            currentTaskDescribe = string.Format("{0}不存在",inputFilePath);
                            return false;
                        }
                        System.IO.StreamWriter writter = new System.IO.StreamWriter(inputFilePath, false,Encoding.Default);
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
                        logRecorder.AddDebugLog(nodeName, "写入input文件:" + strBuild.ToString());
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 3:
                    {
                        currentTaskDescribe = "等待加工完成";
                        if (this.db2Vals[2 + channelIndex] != 2)
                        {
                            break;
                        }
                        currentTaskDescribe = "开始上传MES";
                        if (!System.IO.File.Exists(trsnFilePath))
                        {
                            this.currentTaskDescribe = string.Format("点胶机交互文件：{0}不存在", trsnFilePath);

                            break;
                        }
                        System.IO.StreamReader reader = new System.IO.StreamReader(trsnFilePath, Encoding.Default);
                        string mesItemStr = "";
                        while (!reader.EndOfStream)
                        {
                            string str = reader.ReadLine();
                            string[] strArray = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                            if (strArray.Count() < 2)
                            {
                                continue;
                            }
                            if (strArray[0] == "胶量")
                            {
                                if (string.IsNullOrWhiteSpace(mesItemStr))
                                {
                                    mesItemStr += string.Format("胶量:{0}:mg", strArray[1]);
                                }
                                else
                                {
                                    mesItemStr += string.Format("|胶量:{0}:mg", strArray[1]);
                                }
                            }
                        }
                        if (string.IsNullOrWhiteSpace(mesItemStr))
                        {
                            currentTaskDescribe = "点胶数据解析错误，结果为空";
                            break;
                        }
                        //1,2点胶工位，只上传过程参数

                        string M_AREA = "Y001";
                        string M_WORKSTATION_SN = "Y00100801";
                        if(this.nodeID=="OPA008")
                        {
                            M_WORKSTATION_SN = "Y00101101";
                        }
                        string M_DEVICE_SN = "";
                        // string M_SN = modCode;
                        string M_UNION_SN = "";
                        string M_CONTAINER_SN = "";
                        string M_LEVEL = "";
                        string M_ITEMVALUE = mesItemStr;
                        RootObject rObj = new RootObject();
                        this.currentTaskDescribe = "开始上传MES点胶机过程参数";

                        string strJson = "";
                        rObj = WShelper.ProcParamUpload(M_AREA, M_DEVICE_SN, M_WORKSTATION_SN, M_UNION_SN, M_CONTAINER_SN, M_LEVEL, M_ITEMVALUE, ref strJson);
                        logRecorder.AddDebugLog(nodeName, string.Format("点胶机结果{0}上传MES，返回{1}", mesItemStr, rObj.RES));
                        this.currentTaskDescribe = string.Format("点胶机结果{0}上传MES，返回{1}", mesItemStr, rObj.RES);
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 4:
                    {
                        
                        db1ValsToSnd[2] = 3;
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
