using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using PLProcessModel;
using DevAccess;
using LogInterface;
using FTDataAccess.Model;
using FTDataAccess.BLL;
namespace LineNodes
{
    public class NodeCheckGongneng : CtlNodeBaseModel
    {
        private int checkRe = 0;
        public AipuAccess AipuObj { get; set; }
        private ProductSizeCfgModel cfgModel = null; 
        private DateTime detectStartTime;//启动检测后开始计时
        public int aipuMachineID = 1;
        private int time1 = 10000;//10秒,高档持续时间
        private int time2 = 7000; //7秒,抵挡持续时间
        public NodeCheckGongneng()
        {
            processName = "功能检测";
        }
        public override bool BuildCfg(System.Xml.Linq.XElement xe, ref string reStr)
        {

            if (!base.BuildCfg(xe, ref reStr))
            {
                return false;
            }
            this.dicCommuDataDB1[1].DataDescription = "1：检查OK，放行，2：读卡/条码失败，3：NG，4：不需要检测，5:产品未绑定，6:产品配置信息不存在";
            this.dicCommuDataDB1[2].DataDescription = "0：允许流程开始，1:流程锁定";
            this.dicCommuDataDB1[3].DataDescription = "机器人配方数据";
            for (int i = 0; i < 30; i++)
            {
                this.dicCommuDataDB1[4 + i].DataDescription = string.Format("条码[{0}]", i + 1);
            }
            this.dicCommuDataDB2[1].DataDescription = "0:无产品,1：有产品";
            this.dicCommuDataDB2[2].DataDescription = "0:无板,1：有板"; 
            this.dicCommuDataDB2[3].DataDescription = "1：检测OK,2：检测NG";
            this.dicCommuDataDB2[4].DataDescription = "不合格项编码";
            XElement selfDataXE = xe.Element("SelfDatainfo");
            XElement machineXE = selfDataXE.Element("Machine");
            this.time1 = int.Parse(machineXE.Attribute("time1").Value);
            if (time1 > 20000)
            {
                this.time1 = 20000;
            }
            this.time2 = int.Parse(machineXE.Attribute("time2").Value);
            if (time2 > 20000)
            {
                this.time2 = 20000;
            }
            this.aipuMachineID = int.Parse(machineXE.Attribute("id").Value);
            return true;
        }
         public override bool ExeBusiness(ref string reStr)
        {
            if (!base.ExeBusiness(ref reStr))
            {
                return false;
            }
            if (db2Vals[0] == 0)
            {
                if (currentTaskPhase > 2)
                {
                    if (!SysCfgModel.SimMode)
                    {
                        if (!AipuObj.CloseDev(ref reStr))
                        {
                            logRecorder.AddDebugLog(nodeName, "关闭艾普检测设备失败," + reStr);
                          
                        }
                    }
                }
            }
            if (!NodeStatParse(ref reStr))
            {
                return false;
            }
            //SendPrintcodeFromBuf();

            if (!checkEnable)
            {
                return true;
            }
            switch (currentTaskPhase)
            {
                case 1:
                    {
                        checkRe = 0;
                        if (!ProcessStartCheck(3, !SysCfgModel.DebugMode))
                        {
                            break;
                        }
                       
                        //发送机器人配方
                        ProductSizeCfgBll productCfg = new ProductSizeCfgBll();
                        string cataCode = this.currentStat.ProductBarcode.Substring(0, 13);
                        cfgModel = productCfg.GetModel(cataCode);
                   
                        if (cfgModel == null)
                        {
                            if (this.db1ValsToSnd[0] != 6)
                            {
                                ThrowErrorStat(string.Format("{0}产品配置信息不存在", this.currentStat.ProductBarcode), EnumNodeStatus.设备故障);
                                // logRecorder.AddDebugLog(nodeName, string.Format("{0}产品配置信息不存在", this.currentStat.ProductBarcode));
                            }
                            this.currentStat.StatDescribe = "配置不存在";
                            checkEnable = false;
                            this.db1ValsToSnd[0] = 6;
                            return true;
                        }
                        this.db1ValsToSnd[2] = (short)cfgModel.robotProg;
                        this.currentTaskPhase++;
                        break;
                    }
                case 2:
                    {
                        if (!SysCfgModel.SimMode)
                        {
                            float frequenze = 50.0f;// (float)cfgModel.frequency1;
                            float volt = 233.0f;// (float)cfgModel.volt1;
                            if(!AipuObj.SetRunningParam(frequenze,volt,ref reStr))
                            {
                                logRecorder.AddDebugLog(nodeName, "设置艾普强档检测参数失败," + reStr);
                                break;
                            }
                            logRecorder.AddDebugLog(nodeName, string.Format("设置艾普强档电压/频率参数,{0}V,{1}HZ", volt, frequenze));
                            //if(!AipuObj.OpenDev(ref reStr))
                            //{
                            //    logRecorder.AddDebugLog(nodeName, "设置艾普强档检测参数失败," + reStr);
                            //    break;
                            //}
                            detectStartTime = System.DateTime.Now;
                           
                        }
                        logRecorder.AddDebugLog(this.nodeName, this.currentStat.ProductBarcode + "开始检测，启动艾普检测仪");
                        this.currentTaskPhase++;
                        break;
                    }
                case 3:
                    {
                        float frequenze = 50.0f;//(float)cfgModel.frequency1;
                        float volt = 220.0f;//(float)cfgModel.volt2;
                        if (!SysCfgModel.SimMode)
                        {
                            TimeSpan ts = System.DateTime.Now - detectStartTime;
                            if (ts.TotalSeconds > 10)
                            {
                                //开始切换到弱档参数
                                logRecorder.AddDebugLog(nodeName, string.Format("设置艾普弱档电压/频率参数,{0}V,{1}HZ", volt, frequenze));
                                if (!AipuObj.SetRunningParam(frequenze, volt, ref reStr))
                                {
                                    logRecorder.AddDebugLog(nodeName, "设置艾普弱档检测参数失败," + reStr);
                                    break;
                                }
                               
                                this.currentTaskPhase++;
                            }
                        }
                        else
                        {
                            logRecorder.AddDebugLog(nodeName, string.Format("设置艾普弱档电压/频率参数,{0}V,{1}HZ", volt, frequenze));
                            this.currentTaskPhase++;

                        }
                       
                        break;
                    }
                case 4:
                    {
                        currentTaskDescribe = "等待检测结果";
                        if (db2Vals[2] == 0)
                        {
                            break;
                        }
                       
                        string detectCodes = "";
                        if (db2Vals[2] == 1)
                        {
                            //合格
                            checkRe = 0;
                        }
                        else
                        {
                            checkRe = 1;
                            detectCodes = GetDetectCodeStr();
                            if (string.IsNullOrWhiteSpace(detectCodes))
                            {
                                break;
                            }
                            OutputRecord(this.currentStat.ProductBarcode);
                        }
                        currentTaskDescribe = "开始保存结果到本地";
                        if (!MesDatalocalSave(this.currentStat.ProductBarcode, checkRe, detectCodes, "", 0))
                        {
                            logRecorder.AddLog(new LogModel(this.nodeName, "保存检测数据到本地数据库失败", EnumLoglevel.警告));
                            break;
                        }
                       
                        currentTaskPhase++;
                        break;
                    }
                case 5:
                    {
                        currentTaskDescribe = "开始上传结果到MES";
                        if(checkRe == 1)
                        {
                            if (!UploadMesdata(true, this.currentStat.ProductBarcode, mesProcessSeq, ref reStr))
                            {
                                this.currentStat.StatDescribe = "上传MES失败";
                                logRecorder.AddDebugLog(this.nodeName, this.currentStat.StatDescribe);
                                break;
                            }
                        }
                       
                        currentTaskPhase++;
                        break;
                    }
                case 6:
                    {
                        currentTaskDescribe = "放行";
                        this.currentStat.StatDescribe = "检测完成";
                        string checkreStr = "OK";
                        if (checkRe == 1)
                        {
                            checkreStr = "NG";
                            db1ValsToSnd[0] = db1StatNG;
                        }
                        else
                        {
                            db1ValsToSnd[0] = db1StatCheckOK;//允许下线
                        }
                        logRecorder.AddDebugLog(this.nodeName, this.currentStat.ProductBarcode + "检测完成," + checkreStr);
                        this.currentStat.StatDescribe = "下线";
                        checkFinished = true;
                        currentTaskPhase++;
                        break;
                    }
                case 7:
                    {
                        this.currentStat.StatDescribe = "流程完成";
                        currentTaskDescribe = "流程结束";
                        break;
                    }
            }
            return true;
        }
        
    }
}
