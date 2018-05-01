using PLProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LineNodes
{
    public class BakeStation : CtlNodeBaseModel
    {
        private int highTemperature = 0;//最高温度
        private int lowTemperature = 0;//最低温度
        private int bakeTime = 0;     //烘烤时间
     
        public override bool BuildCfg(System.Xml.Linq.XElement xe, ref string reStr)
        {
            if (!base.BuildCfg(xe, ref reStr))
            {
                return false;
            }
            this.dicCommuDataDB1[1].DataDescription = "左通道读卡结果1：复位/待机状态2：数据读取成功3：数据读取失败";
            this.dicCommuDataDB1[2].DataDescription = "右通道读卡结果：1：复位/待机状态2：数据读取成功3：数据读取失败";
            this.dicCommuDataDB1[3].DataDescription = "1：复位/待机状态,2：数据读取中,3：数据读取完毕，放行";
           
            this.dicCommuDataDB2[1].DataDescription = "0：无1：正在运行左通道2：正在运行右通道";
            this.dicCommuDataDB2[2].DataDescription = "1：左通道无板 2：左通道读取数据请求";
            this.dicCommuDataDB2[3].DataDescription = "1：右通道无板 2：右通道读取数据请求";
           
           
            return true;
        }


        public override bool ExeBusiness(ref string reStr)
        {
            if (!nodeEnabled)
            {
                return true;
            }
          
            if (!base.ExeBusinessAB(ref reStr))
            {
                return false;
            }

          
            switch (this.currentTaskPhase)
            {
                case 1:
                    if (ReadBakeData() == false)
                    {
                        SetReadStatus(3);//读取失败

                        break;
                    }
                    // SetReadStatus(2);//读取失败

                    string bakeData = "烘烤最高温度:" + this.highTemperature/10 + ":℃|烘烤最低温度:" + this.lowTemperature/10 + ":℃|烘烤时间:" + this.bakeTime/10 + ":s（这段时间内的最高温度和最低温度）";
                    if (this.nodeID == "OPA016")
                    {
                        if (UploadMesProcessParam("Y00100901", bakeData) == false)
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (UploadMesProcessParam("Y00101201", bakeData) == false)
                        {
                            break;
                        }
                    }
                    this.currentTask.TaskPhase = this.currentTaskPhase;
                    this.currentTask.TaskParam = this.highTemperature + "-" + this.lowTemperature + "-" + this.bakeTime;
                    this.ctlTaskBll.Update(this.currentTask);
                    this.currentTaskDescribe = "读取数据成功；上报MES数据成功！";
                    SetReadStatus(2);//读取成功
                    this.currentTaskPhase++;
                    this.LogRecorder.AddDebugLog(this.nodeName, "读取数据成功；上报MES数据成功！数据" + bakeData);
                    break;
                case 2:
                    
                    db1ValsToSnd[2] = 3;
                    this.currentTask.TaskPhase = this.currentTaskPhase;
                    this.currentTask.TaskType =(int)EnumTaskStatus.已完成;
                    this.ctlTaskBll.Update(this.currentTask);
                    this.currentTaskDescribe = "流程完成！";
                   
                    break;

                default:
                    break;
            }
          

            return true;

        }
        private bool ReadBakeData()
        {
            if (SysCfgModel.SimMode == true)
            {
                
                return true;
            }
            if (this.nodeID == "OPA016")
            {

                if (ReadOPA16Data() == false)
                {
                    return false;
                }
               
            }
            else if (this.nodeID == "OPA017")
            {
               if (ReadOPA17Data() == false)
                {
                    return false;
                }

            }

            
            return true;
        }

        private void SetReadStatus(short status)
        {
            if (this.db2Vals[1] == 2)
            { 
                this.db1ValsToSnd[0] =status;
            }
            if(this.db2Vals[2] == 2)
            {
                this.db1ValsToSnd[1] =status;
            }
        }

     
             
        private bool ReadOPA16Data()
        {
            if (this.plcRW.ReadDB("D9000", ref this.highTemperature) == false)
            {
                return false;
            }
            if (this.plcRW.ReadDB("D9002", ref this.lowTemperature) == false)
            {
                return false;
            }
            if (this.plcRW.ReadDB("D9004", ref this.bakeTime) == false)
            {
                return false;
            }
            return true;
        }

        private bool ReadOPA17Data()
        {
            if (this.plcRW.ReadDB("D9006", ref this.highTemperature) == false)
            {
                return false;
            }
            if (this.plcRW.ReadDB("D9008", ref this.lowTemperature) == false)
            {
                return false;
            }
            if (this.plcRW.ReadDB("D9010", ref this.bakeTime) == false)
            {
                return false;
            }
            return true;
        }
        public bool UploadMesProcessParam(string workStationNum,string paramItems)
        {
            string M_AREA = "Y001";
            string M_WORKSTATION_SN = workStationNum;
            string M_DEVICE_SN = "";
            // string M_SN = modCode;
            string M_UNION_SN = "";
            string M_CONTAINER_SN = "";
            string M_LEVEL = "";
            string M_ITEMVALUE = paramItems;
            RootObject rObj = new RootObject();
            //this.currentTaskDescribe = "开始上传MES点胶机过程参数";

            string strJson = "";
            rObj = WShelper.ProcParamUpload(M_AREA, M_DEVICE_SN, M_WORKSTATION_SN, M_UNION_SN, M_CONTAINER_SN, M_LEVEL, M_ITEMVALUE, ref strJson);
            if (rObj.RES.Contains("OK"))
            {
                return true;
            }
            else
            {
                Console.WriteLine(this.nodeName + "上传过程数据失败：" + rObj.RES);
                return false;
            }
            
        }
    }
}
