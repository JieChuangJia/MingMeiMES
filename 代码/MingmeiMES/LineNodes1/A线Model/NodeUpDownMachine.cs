
using PLProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LineNodes
{
    public class NodeUpDownMachine:CtlNodeBaseModel
    {
       // private short[] barcodeDb1 = new short[5]; //打码交互db1
      //  private short[] barcodeDb2 = new short[5]; //打码交互db2
        private string M_SN = "";
       // private int barcodeTaskPhase = 0;
        string machionIP = "192.168.0.1";
        public override bool BuildCfg(System.Xml.Linq.XElement xe, ref string reStr)
        {
            if (!base.BuildCfg(xe, ref reStr))
            {
                return false;
            }
            //this.dicCommuDataDB1[1].DataDescription = "A通道读卡结果，1：复位/待机状态,2：RFID读取成功,3：RFID读取失败";
            //this.dicCommuDataDB1[2].DataDescription = "B通道读卡结果,1：复位/待机状态,2：RFID读取成功,3：RFID读取失败";
            //this.dicCommuDataDB1[3].DataDescription = "A通道,1：复位/待机状态,2：数据读取中,3：数据读取完毕，放行";
            //this.dicCommuDataDB1[4].DataDescription = "B通道,1：复位/待机状态,2：数据读取中,3：数据读取完毕，放行";

            //this.dicCommuDataDB2[1].DataDescription = "0：无,1：正在运行A通道,2：正在运行B通道";
            //this.dicCommuDataDB2[2].DataDescription = "1：A通道无板,2：A通道有板，读卡请求";
            //this.dicCommuDataDB2[3].DataDescription = "1：B通道无板,2：B通道有板，读卡请求";
            this.dicCommuDataDB1[1].DataDescription = "请求二维码结果,1：复位/待机状态,2：请求二维码成功,3：请求二维码失败";
            this.dicCommuDataDB1[2].DataDescription = "上传MES结果，1:复位，2：成功";
            this.dicCommuDataDB2[1].DataDescription = "0：无,1：请求二维码";
            this.dicCommuDataDB2[2].DataDescription = "扫码结果,0：初始状态,1：扫码成功,2：扫码失败";
            return true;
        }
        public override bool ExeBusiness(ref string reStr)
        {
            if (!nodeEnabled)
            {
                return true;
            }
            if (this.nodeID == "OPA010")
            {
                return ExeBusinessPLC3(ref reStr);
            }
            return true;
            /*
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
           
            return true;*/
        }

        private bool ExeBusinessPLC2(ref string reStr)
        {
            if(!RequireModCode(ref reStr))
            {
                return false;
            }
            ScanCodeIsSuccess(ref reStr);
            return true;
        }
        private bool ExeBusinessPLC3(ref string reStr)
        {
            //if(!plcRW2.ReadMultiDB("D9000", 2, ref barcodeDb2))
            //{
            //    Console.WriteLine("{0}打码交互，读DB2数据区失败",nodeName);
            //    return false;
            //}
            if(this.db2Vals[0] == 1)
            {
               // barcodeTaskPhase = 1;
                currentTaskPhase = 1;
                M_SN = "";
                this.db1ValsToSnd[0] = 1;
                this.db1ValsToSnd[1] = 1;
            }

            switch (currentTaskPhase)
            {
                case 1:
                    {
                        Console.WriteLine("{0}开始从MES申请模块码", nodeName);
                        this.currentTaskDescribe = string.Format("{0}开始从MES申请模块码", nodeName);
                        if (!ModuleCodeRequire(ref M_SN, ref reStr))
                        {
                            LogRecorder.AddDebugLog(nodeName, "ModuleCodeRequire FALSE" + "," + reStr);
                            this.db1ValsToSnd[0] = 3;
                            this.currentTaskDescribe = "从MES申请模块码失败" + reStr;
                            break;
                        }
                        if(!plcRW.WriteDB("D9000", 0))
                        {
                            Console.WriteLine("复位D9000 失败");
                            plcRW.CloseConnect();
                            plcRW.ConnectPLC(ref reStr);
                            break;
                        }
                        LogRecorder.AddDebugLog(nodeName, "从MES申请到模块码:" + M_SN);
                        this.currentTaskDescribe = "从MES申请到模块码:" + M_SN;
                        currentTaskPhase++;
                        break;
                    }
                case 2:
                    {
                        this.db1ValsToSnd[1] = 1;
                        //写条码到文件
                        string modCodeFile = "";
                        modCodeFile = string.Format(@"\\{0}\打标文件\加工文件\打码内容.txt", machionIP);
                        if (!System.IO.File.Exists(modCodeFile))
                        {
                            this.currentTaskDescribe = string.Format("打码内容文件：{0}不存在", modCodeFile);
                            LogRecorder.AddDebugLog(nodeName, string.Format("打码内容文件：{0}不存在", modCodeFile));
                            break;
                        }
                        System.IO.StreamWriter writter = new System.IO.StreamWriter(modCodeFile, false);
                        StringBuilder strBuild = new StringBuilder();
                        strBuild.AppendLine(M_SN);
                        writter.Write(strBuild.ToString());
                        writter.Flush();
                        writter.Close();
                        this.db1ValsToSnd[0] = 2;
                        LogRecorder.AddDebugLog(nodeName, string.Format("模块码{0}写到打码内容文本中", M_SN));
                        this.currentTaskDescribe = string.Format("模块码{0}写到打码内容文本中", M_SN);
                        currentTaskPhase++;
                        break;
                    }
                case 3:
                    {
                        //等待扫码结果
                        this.currentTaskDescribe = "等待扫码结果";
                        string checkResult = "";
                        if(this.db2Vals[1] == 1)
                        {
                            checkResult = "OK:";
                        }
                        else if (this.db2Vals[1] == 2)
                        {
                            checkResult = "NG:";
                        }
                        else
                        {
                            break;
                        }
                        this.db1ValsToSnd[0] = 1; //复位
                        string modCodeFile = "";
                        modCodeFile = string.Format(@"\\{0}\打标文件\加工文件\打码内容.txt", machionIP);
                        /*
                        if (!System.IO.File.Exists(modCodeFile))
                        {
                            
                            LogRecorder.AddDebugLog(nodeName, string.Format("打码内容文件：{0}不存在", modCodeFile));
                            break;
                        }
                        System.IO.StreamReader reader = new System.IO.StreamReader(modCodeFile, Encoding.Default);
                        string modCode = reader.ReadLine();*/
                        //上传Mes
                        int M_FLAG = 3;
                        string M_AREA = "Y001";
                        string M_WORKSTATION_SN = "Y00100101";
                        string M_DEVICE_SN = "";
                       // string M_SN = modCode;
                        string M_UNION_SN = "";
                        string M_CONTAINER_SN = "";
                        string M_LEVEL = "";
                        string M_ITEMVALUE = "扫码结果:" + checkResult;
                        RootObject rObj = new RootObject();
                        string barcode = M_SN;
                        string strJson = "";
                        if (this.db2Vals[1] == 1)
                        {
                            barcode = M_SN;
                            M_FLAG = 3;
                            rObj = DevDataUpload(M_FLAG, M_DEVICE_SN, M_WORKSTATION_SN, barcode, M_UNION_SN, M_CONTAINER_SN, M_LEVEL, M_ITEMVALUE, ref strJson);
                        }
                        else if(this.db2Vals[1] == 2)
                        {
                            barcode = "";
                            M_FLAG = 6;
                            rObj = ProcParamUpload(M_AREA, M_DEVICE_SN, M_WORKSTATION_SN, M_UNION_SN, M_CONTAINER_SN, M_LEVEL, M_ITEMVALUE, ref strJson);
                        }
                       // LogRecorder.AddDebugLog(nodeName, rObj.RES);
                        logRecorder.AddDebugLog(nodeName, string.Format("打码结果{0}上传MES，返回{1}", checkResult,rObj.RES));
                        this.currentTaskDescribe = string.Format("打码结果{0}上传MES，返回{1}", checkResult, rObj.RES);
                        //清空文件
                        System.IO.StreamWriter writter = new System.IO.StreamWriter(modCodeFile, false);
                        StringBuilder strBuild = new StringBuilder();
                        strBuild.AppendLine("");
                        writter.Write(strBuild.ToString());
                        writter.Flush();
                        writter.Close();
                        LogRecorder.AddDebugLog(nodeName, "打码内容文件清空");
                        this.db1ValsToSnd[1] = 2;
                        if (this.db2Vals[1]==2)
                        {
                            currentTaskPhase = 2;
                        }
                        else
                        {
                            currentTaskPhase = 0;
                        }
                        break;
                    }
                default:
                    break;
            }

            //if(!plcRW2.WriteMultiDB("D9100", 2, barcodeDb1))
            //{
            //    Console.WriteLine("{0}打码交互，写DB1数据区失败", nodeName);
            //    return false;
            //}
            return true;
        }

        private bool RequireModCode(ref string reStr)
        {
            int val = 0;
          
            if (!plcRW2.ReadDB("D9000", ref val))
            {
                 Console.WriteLine(nodeName+"读设备PLC失败,PLC地址："+(plcRW2 as DevAccess.OmlPlcRW).PLCIP);
                plcRW2.CloseConnect();
                plcRW2.ConnectPLC(ref reStr);
                return false;
            }
            if (val != 1)
            {
               // LogRecorder.AddDebugLog(nodeName, "D9000 = 0");
                return false;
            }
            if(!plcRW2.WriteDB("D9101",1))
            {
                Console.WriteLine(nodeName + "复位D9101失败" );
                return false;
            }
          //  LogRecorder.AddDebugLog(nodeName, "D9000 = 1");
            Console.WriteLine("{0}开始从MES申请模块码", nodeName);
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
            LogRecorder.AddDebugLog(nodeName, "从MES申请到模块码:" + M_SN);
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
            if(!plcRW2.WriteDB("D9000", 0))
            {
                reStr = "复位D9000 失败";
                return false;
            }
            if (!plcRW2.WriteDB("D9100", 2))
            {
                plcRW2.CloseConnect();
                plcRW2.ConnectPLC(ref reStr);
                return false;
            }
            LogRecorder.AddDebugLog(nodeName, "模块条码写到打码内容文本中");
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
                checkResult = "OK:";
            }
            else if(val == 2)
            {
                checkResult = "NG:";
            }
            else
            {
                return false;
            }
           // LogRecorder.AddDebugLog(nodeName, "D9001 = " + val.ToString());
           
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
            string strJson = "";
            rObj = DevDataUpload(M_FLAG, M_DEVICE_SN, M_WORKSTATION_SN, M_SN, M_UNION_SN, M_CONTAINER_SN, M_LEVEL, M_ITEMVALUE,ref strJson);
            LogRecorder.AddDebugLog(nodeName, rObj.RES);
            //if (rObj.RES.Contains("NG"))
            //{
            //    return false;
            //}
            logRecorder.AddDebugLog(nodeName, string.Format("打码结果{0}上传MES", checkResult));
            if (plcRW2.WriteDB("D9101", 2))
            {
                plcRW2.CloseConnect();
                plcRW2.ConnectPLC(ref reStr);
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
           // LogRecorder.AddDebugLog(nodeName, "D9100 = 1");    
            return true;
        }
        /// 模组条码请求
        /// </summary>
        /// <returns></returns>
        private bool ModuleCodeRequire(ref string M_SN,ref string reStr)
        {
            string M_WORKSTATION_SN = "Y00200101";
            RootObject rObj = new RootObject();
            if(SysCfgModel.MesOfflineMode == false)
            {
                rObj = WShelper.BarCodeRequest(M_WORKSTATION_SN);
                if (rObj.RES.Contains("OK"))
                {
                    M_SN = rObj.M_COMENT[0].M_SN;
                    reStr = this.nodeName + "模块条码请求成功";
                    return true;
                }
                else
                {
                    M_SN = "";
                    reStr = this.nodeName + "模块条码请求失败!" + rObj.RES;
                    return false;
                }
            }
            else
            {
                M_SN = "123456789201345678999013";
                return true;
            }
          
        }
    }
}
