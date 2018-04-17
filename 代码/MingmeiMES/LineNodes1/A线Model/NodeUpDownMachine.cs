
using PLProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LineNodes
{
    public class NodeUpDownMachine:CtlNodeBaseModel
    {
        string machionIP = "192.168.0.1";
        public override bool BuildCfg(System.Xml.Linq.XElement xe, ref string reStr)
        {
            if (!base.BuildCfg(xe, ref reStr))
            {
                return false;
            }
            this.dicCommuDataDB1[1].DataDescription = "A通道读卡结果，1：复位/待机状态,2：RFID读取成功,3：RFID读取失败";
            this.dicCommuDataDB1[2].DataDescription = "B通道读卡结果,1：复位/待机状态,2：RFID读取成功,3：RFID读取失败";
            this.dicCommuDataDB1[3].DataDescription = "A通道,1：复位/待机状态,2：数据读取中,3：数据读取完毕，放行";
            this.dicCommuDataDB1[4].DataDescription = "B通道,1：复位/待机状态,2：数据读取中,3：数据读取完毕，放行";

            this.dicCommuDataDB2[1].DataDescription = "0：无,1：正在运行A通道,2：正在运行B通道";
            this.dicCommuDataDB2[2].DataDescription = "1：A通道无板,2：A通道有板，读卡请求";
            this.dicCommuDataDB2[3].DataDescription = "1：B通道无板,2：B通道有板，读卡请求";
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
                        LogRecorder.AddDebugLog(nodeName, string.Format("RFID：{0}", this.rfidUID));
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 2:
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
            if (this.nodeName == "OPA010")
            {
                ExeBusinessPLC2(ref reStr);
            }
            return true;
        }

        private bool ExeBusinessPLC2(ref string reStr)
        {
            RequireModCode(ref reStr);
            ScanCodeIsSuccess(ref reStr);
            return true;
        }

        private bool RequireModCode(ref string reStr)
        {
            int val = 0;
            if (!plcRW2.ReadDB("D9000", ref val))
            {
                plcRW2.CloseConnect();
                plcRW2.ConnectPLC(ref reStr);
                return false;
            }
            if (val != 1)
            {
                LogRecorder.AddDebugLog(nodeName, "D9000 = 0");
                return false;
            }
            LogRecorder.AddDebugLog(nodeName, "D9000 = 1");
            string M_SN = "";
            if (!ModuleCodeRequire(ref M_SN, ref reStr))
            {
                LogRecorder.AddDebugLog(nodeName, "ModuleCodeRequire FALSE" + "," + reStr);
                if (plcRW2.WriteDB("D9100", 3))
                {
                    plcRW2.CloseConnect();
                    plcRW2.ConnectPLC(ref reStr);
                    return false;
                }
                return false;
            }
            LogRecorder.AddDebugLog(nodeName, "M_SN:" + M_SN);
            //写条码到文件
            string modCodeFile = "";
            modCodeFile = string.Format(@"\\{0}\打标文件\加工文件\打码内容.txt", machionIP);
            if (!System.IO.File.Exists(modCodeFile))
            {
                LogRecorder.AddDebugLog(nodeName,string.Format("打码内容文件：{0}不存在", modCodeFile));
                return false;
            }
            System.IO.StreamWriter writter = new System.IO.StreamWriter(modCodeFile, false);
            StringBuilder strBuild = new StringBuilder();
            strBuild.AppendLine(M_SN);
            writter.Write(strBuild.ToString());
            writter.Flush();
            writter.Close();
            if (plcRW2.WriteDB("D9100", 2))
            {
                plcRW2.CloseConnect();
                plcRW2.ConnectPLC(ref reStr);
                return false;
            }
            LogRecorder.AddDebugLog(nodeName, "模组条码写到打码内容文本中");
            return true;
        }


        private bool ScanCodeIsSuccess(ref string reStr)
        {
            int val = 0;
            if (!plcRW2.ReadDB("D9001", ref val))
            {
                plcRW2.CloseConnect();
                plcRW2.ConnectPLC(ref reStr);
                return false;
            }

            string checkResult = "";
            if (val == 1)
            {
                checkResult = "OK";
            }
            else if(val == 2)
            {
                checkResult = "NG";
            }
            else
            {
                return false;
            }
            LogRecorder.AddDebugLog(nodeName, "D9001 = " + val.ToString());
           
            //读模组条码文件
            string modCodeFile = "";
            modCodeFile = string.Format(@"\\{0}\打标文件\加工文件\打码内容.txt", machionIP);
            if (!System.IO.File.Exists(modCodeFile))
            {
                LogRecorder.AddDebugLog(nodeName, string.Format("打码内容文件：{0}不存在", modCodeFile));
                return false;
            }
            System.IO.StreamReader reader = new System.IO.StreamReader(modCodeFile, Encoding.Default);
            string modCode = reader.ReadLine();
             //上传Mes
            int M_FLAG = 3;
            string M_WORKSTATION_SN = "Y00100101";
            string M_DEVICE_SN = "";
            string M_SN = modCode;
            string M_UNION_SN = "";
            string M_CONTAINER_SN = "";
            string M_LEVEL = "";
            string M_ITEMVALUE = "扫码结果:" + checkResult;
            RootObject rObj = new RootObject();
            rObj = WShelper.DevDataUpload(M_FLAG, M_DEVICE_SN, M_WORKSTATION_SN, M_SN, M_UNION_SN, M_CONTAINER_SN, M_LEVEL, M_ITEMVALUE);
            LogRecorder.AddDebugLog(nodeName, rObj.RES);
            if (rObj.RES.Contains("NG"))
            {
                return false;
            }
            //清空文件
            System.IO.StreamWriter writter = new System.IO.StreamWriter(modCodeFile, false);
            StringBuilder strBuild = new StringBuilder();
            strBuild.AppendLine("");
            writter.Write(strBuild.ToString());
            writter.Flush();
            writter.Close();
            LogRecorder.AddDebugLog(nodeName, "打码内容文件清空");    
            if (plcRW2.WriteDB("D9100", 1))
            {
                plcRW2.CloseConnect();
                plcRW2.ConnectPLC(ref reStr);
                return false;
            }
            LogRecorder.AddDebugLog(nodeName, "D9100 = 1");    
            return true;
        }
        /// 模组条码请求
        /// </summary>
        /// <returns></returns>
        private bool ModuleCodeRequire(ref string M_SN,ref string reStr)
        {
            string M_WORKSTATION_SN = "Y00100101";
            RootObject rObj = new RootObject();
            rObj = WShelper.BarCodeRequest(M_WORKSTATION_SN);
            if (rObj.RES.Contains("OK"))
            {
                M_SN = rObj.M_COMENT[0].M_SN;
                reStr = this.nodeName + "模组条码请求成功!";
                return true;
            }
            else
            {
                M_SN = "";
                reStr = this.nodeName + "模组条码请求失败!" + rObj.RES;
                return false;
            }
        }
    }
}
