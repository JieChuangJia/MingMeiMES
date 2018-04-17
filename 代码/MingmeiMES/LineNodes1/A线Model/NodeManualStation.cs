using FTDataAccess.Model;
using PLProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LineNodes
{
    /// <summary>
    /// 人工工位
    /// </summary>
    class NodeManualStation : CtlNodeBaseModel
    {
        public override bool BuildCfg(System.Xml.Linq.XElement xe, ref string reStr)
        {
            if (!base.BuildCfg(xe, ref reStr))
            {
                return false;
            }

            if (this.nodeID == "OPA013" || this.nodeID == "OPA014" || this.nodeID == "OPA015")
            {
                this.dicCommuDataDB1[1].DataDescription = "A通道读卡结果，1：复位/待机状态,2：RFID读取成功,3：RFID读取失败,4：无绑定数据";
                this.dicCommuDataDB1[2].DataDescription = "B通道读卡结果,1：复位/待机状态,2：RFID读取成功,3：RFID读取失败,4：无绑定数据";
                this.dicCommuDataDB1[3].DataDescription = "A通道,1：复位/待机状态,2：数据读取中,3：数据读取完毕，放行";
                this.dicCommuDataDB1[4].DataDescription = "B通道,1：复位/待机状态,2：数据读取中,3：数据读取完毕，放行";
                this.dicCommuDataDB2[1].DataDescription = "0：无,1：正在运行A通道,2：正在运行B通道";
                this.dicCommuDataDB2[2].DataDescription = "1：A通道无板,2：A通道有板，读卡请求";
                this.dicCommuDataDB2[3].DataDescription = "1：B通道无板,2：B通道有板，读卡请求";


            }
            else if (this.nodeID == "OPC008" || this.nodeID == "OPC009" || this.nodeID == "OPC010")
            {
                this.dicCommuDataDB1[1].DataDescription = "工位读卡结果，1：复位/待机状态,2：RFID读取成功,3：RFID读取失败,4：无绑定数据";
                this.dicCommuDataDB1[2].DataDescription = "工位状态,1：复位/待机状态,2：数据读取中,3：数据读取完毕，放行";
                this.dicCommuDataDB2[1].DataDescription = "1：通道无板,2：通道有板，读卡请求";
            }
            return true;
        }

        public override bool ExeBusiness(ref string reStr)
        {
            if (!nodeEnabled)
            {
                return true;
            }
            if (this.nodeID == "OPA013" || this.nodeID == "OPA014" || this.nodeID == "OPA015")
            {
                return ExeBindA(ref reStr);
            }
            else if (this.nodeID == "OPC008" || this.nodeID == "OPC009" || this.nodeID == "OPC010")
            {
                return ExeBindC(ref reStr);
            }


            return true;
        }
        protected bool ExeBindA(ref string reStr)
        {
            if (!ExeBusinessAB(ref reStr))
            {
                return false;
            }

            switch (currentTaskPhase)
            {
                case 1:
                    if (!RfidReadAB())
                    {
                        break;
                    }

                    LogRecorder.AddDebugLog(nodeName, string.Format("RFID：{0}", this.rfidUID));
                    currentTaskPhase++;
                    break;

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
                        LogRecorder.AddDebugLog(nodeName, string.Format("RFID：{0}", this.rfidUID));
                        currentTaskPhase++;
                        break;
                    }
                case 2:
                    {
                        db1ValsToSnd[2] = 2;
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
