using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using PLProcessModel;
using DevInterface;
using LogInterface;
using FTDataAccess.Model;
//using FTDataAccess.BLL;
namespace LineNodes
{
    /// <summary>
    /// 气密检查控制节点
    /// </summary>
    public class NodeAirlossCheck:CtlNodeBaseModel
    {
        private int detectTimeout = 15000; //检测结果返回最大允许延迟20000毫秒
        private DateTime detectStartTime;//启动检测后开始计时
        private IAirlossDetectDev airDetectRW = null;
        private int checkRetryCounter = 0;
        private string detectCode = "";
        protected int checkRe = 0;
        protected string checkCode = "";
        public IAirlossDetectDev AirDetectRW { get { return airDetectRW; } set { airDetectRW = value; } }

        public override bool BuildCfg(System.Xml.Linq.XElement xe, ref string reStr)
        {
            if (!base.BuildCfg(xe, ref reStr))
            {
                return false;
            }
            XElement selfDataXE = xe.Element("SelfDatainfo");
            XElement machineXE = selfDataXE.Element("AircheckMachine");
            this.detectTimeout = int.Parse(machineXE.Attribute("detectTimeOut").Value);
            if(detectTimeout<1000)
            {
                this.detectTimeout = 1000;
            }
            if(detectTimeout>60000)
            {
                this.detectTimeout = 60000;
            }
            XElement detectCodeXE = selfDataXE.Element("DeftectCode");
            this.detectCode = detectCodeXE.Value.ToString();

            this.dicCommuDataDB1[1].DataDescription = "1：检查OK，放行，2：读卡/条码失败，未投产,3：NG,4：不需要检测,5:产品未绑定";
            this.dicCommuDataDB1[2].DataDescription = "0：允许流程开始，1:流程锁定";
            for (int i = 0; i < 30; i++)
            {
                this.dicCommuDataDB1[3 + i].DataDescription = string.Format("条码[{0}]", i + 1);
            }
           
            this.dicCommuDataDB2[1].DataDescription = "0:无板,1：有产品";
            this.dicCommuDataDB2[2].DataDescription = "0：插头未就绪,1：插头就绪 ";
            return true;
        }
        public override bool ExeBusiness(ref string reStr)
        {
            
           // int nodeStatus = 0; //状态机,0:初始化状态，1：有板进入，2：插头就绪，开始检测，3：检测完成,报警或放行，4：空板进入，放行
            if(!NodeStatParse(ref reStr))
            {
                return false;
            }
            if(!checkEnable)
            {
                return true;
            }
            switch (currentTaskPhase)
            {
                
                case 1:
                    {
                        checkRetryCounter = 0;
                        db1ValsToSnd[1] = 1;//流程锁定
                        if (this.currentStat.Status == EnumNodeStatus.设备故障)
                        {
                            break;
                        }
                        
                        this.currentStat.Status = EnumNodeStatus.设备使用中;
                        this.currentStat.ProductBarcode = "";
                        this.currentStat.StatDescribe = "设备使用中";
                        //开始读卡
                        if(!SysCfgModel.SimMode)
                        {
                            rfidUID = rfidRW.ReadUID();
                           
                        }
                        else
                        {
                            rfidUID = SimRfidUID;
                            
                        }
                        currentTaskDescribe = "开始读RFID";
                        if (!string.IsNullOrWhiteSpace(rfidUID))
                        {
                            db1ValsToSnd[0] = 0;
                            this.currentStat.StatDescribe = "RFID识别完成";
                            //根据绑定，查询条码，赋条码
                        
                             OnlineProductsModel productBind = productBindBll.GetModelByrfid(rfidUID);
                             if (productBind == null)
                             {
                                 db1ValsToSnd[0] = db1StatNG;
                                 //this.currentTaskPhase = 5;
                                 this.currentStat.StatDescribe = "未投产";
                                 logRecorder.AddDebugLog(nodeName, "未投产，rfid:" + rfidUID);
                                 checkEnable = false; 
                                 break;
                             }
                             //状态赋条码, 
                             this.currentStat.ProductBarcode = productBind.productBarcode;

                             productBind.currentNode = this.nodeName;
                             productBindBll.Update(productBind);
                             BarcodeFillDB1(productBind.productBarcode, 2);
                             int reDetectQuery = ReDetectQuery(productBind.productBarcode);
                             if (0 == reDetectQuery)
                             {
                                 db1ValsToSnd[0] = db1StatCheckOK;
                                 checkEnable = false;
                                 logRecorder.AddDebugLog(nodeName,string.Format("{0}本地已经存在检验记录,检验结果：OK",productBind.productBarcode));
                                 break;
                             }
                             else if (1 == reDetectQuery)
                             {
                                 db1ValsToSnd[0] = db1StatNG;
                                 checkEnable = false;
                                 logRecorder.AddDebugLog(nodeName, string.Format("{0}本地已经存在检验记录,检验结果：NG", productBind.productBarcode));
                                 break;
                             }

                             //查询本地数据库，之前工位是否有不合格项，若有，下线
                            // if (LineMonitorPresenter.checkPreStation)
                             {
                                 if (!PreDetectCheck(productBind.productBarcode))
                                 {
                                     db1ValsToSnd[0] = db1StatNG;//
                                     logRecorder.AddDebugLog(this.nodeName, string.Format("{0} 在前面工位有检测NG项", productBind.productBarcode));
                                     checkEnable = false;

                                     break;
                                 }
                             }

                             if (!LossCheck(productBind.productBarcode,ref reStr))
                             {
                                 db1ValsToSnd[0] = db1StatNG;//
                                 logRecorder.AddDebugLog(this.nodeName, string.Format("{0} 检测漏项,{1}", productBind.productBarcode,reStr));
                                 checkEnable = false;
                                
                                 break;
                             }
                             
                        }
                        else
                        {
                            if(db1ValsToSnd[0] != db1StatRfidFailed)
                            {
                                logRecorder.AddDebugLog(nodeName, "读RFID卡失败");
                            }
                            db1ValsToSnd[0] = db1StatRfidFailed;
                            this.currentStat.Status = EnumNodeStatus.无法识别;
                            this.currentStat.StatDescribe = "读RFID卡失败";
                           
                            break;
                        }
                       
                        //插头就绪
                        currentTaskDescribe = "等待插头就绪";
                        if(db2Vals[1]==1)
                        {
                           
                            //开始检测
                            currentTaskDescribe = "开始发送启动气密仪命令";
                            if(!airDetectRW.StartDetect(ref reStr))
                            {
                                //logRecorder.AddLog()
                                ThrowErrorStat("启动气密检测仪失败"+reStr,EnumNodeStatus.设备故障);
                                break;
                            }
                            this.currentStat.StatDescribe = "开始气密检测";
                            logRecorder.AddDebugLog(this.nodeName, this.currentStat.ProductBarcode + "开始检测");
                            currentTaskPhase++;
                            detectStartTime = DateTime.Now;
                         }
                        break;
                    }
                case 3:
                    {
                        //查询检测结果，检测完成或超时
                        currentTaskDescribe = "开始查询气密结果";
                        AirlossDetectModel detectRe = null;
                        detectRe = airDetectRW.QueryResultData(ref reStr);
                       
                        if(detectRe != null)
                        {
                           // int checkRe = 0;  //0合格，1：不合格
                            if(detectRe.DetectResult=="OK")
                            {
                                //db1ValsToSnd[0] = db1StatCheckOK; //所有数据都上传之后再给放行信号
                                checkRe = 0;
                            }
                            else
                            {
                                checkRe = 1;
                               // db1ValsToSnd[0] = db1StatNG;
                            
                                //检测不合格，下线
                                OutputRecord(this.currentStat.ProductBarcode);
                            }
                            //先存到本地数据库
                            checkCode= "";
                            if(checkRe>0)
                            {
                                checkCode=this.detectCode;
                            }
                            else
                            {
                                checkCode = "";
                            }
                            currentTaskDescribe = "开始保存本地气密结果";
                            if (!MesDatalocalSave(this.currentStat.ProductBarcode,checkRe,checkCode, detectRe.DetectVal.ToString(), 0))
                            {
                                logRecorder.AddLog(new LogModel(this.nodeName, "保存检测数据到本地数据库失败", EnumLoglevel.警告));
                                break;
                            }
                           
                            currentTaskPhase++;
                           
                        }
                        else
                        {
                           
                            TimeSpan timeElapse = System.DateTime.Now - detectStartTime;
                            if (timeElapse.TotalMilliseconds > detectTimeout)
                            {
                               
                                detectStartTime = System.DateTime.Now;
                                logRecorder.AddDebugLog(this.nodeName, string.Format("检测超时,{0}毫秒,重新检测", detectTimeout));
                                currentStat.StatDescribe = string.Format("检测超时,重新检测");
                                if(checkRetryCounter<2)
                                {
                                    airDetectRW.StartDetect(ref reStr);
                                }
                                
                                checkRetryCounter++;
                               
                            }
                            if(checkRetryCounter>2)
                            {
                                currentTaskDescribe = "气密超时，开始保存本地气密结果";
                               
                                checkRe = 1;//超时
                                if (!MesDatalocalSave(this.currentStat.ProductBarcode, checkRe, checkCode, "0", 0))
                                {
                                    logRecorder.AddLog(new LogModel(this.nodeName, "保存检测数据到本地数据库失败", EnumLoglevel.警告));
                                    break;
                                }
                                db1ValsToSnd[0] = db1StatNG;
                                logRecorder.AddDebugLog(this.nodeName, string.Format("检测超时,{0}毫秒",detectTimeout));
                                currentStat.StatDescribe = string.Format("检测超时,{0}毫秒", detectTimeout);
                                currentTaskPhase++;
                                break;
                            }
                        }
                       
                        break;
                    }
                case 4:
                    {
                        string[] mesProcessSeq = new string[] { "RQ-Z8005", "RQ-Z8007" };
                       // string[] mesProcessSeq = new string[] {"MES投产位", "RQ-Z8005", "RQ-Z8007" };
                        //string[] mesProcessSeq = new string[] { this.mesNodeID};
                        currentTaskDescribe = "开始上传检测结果（气密2工位NG时才上传）";
                        if (this.nodeID=="2002")
                        {
                            //气密2,3要求合并，若当前为气密2，则检测NG时立即上传
                            if(1==checkRe)
                            {
                                if (!UploadMesdata(true, this.currentStat.ProductBarcode, mesProcessSeq, ref reStr))
                                {
                                    this.currentStat.StatDescribe = "上传MES失败";
                                    logRecorder.AddDebugLog(this.nodeName, this.currentStat.StatDescribe);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (!UploadMesdata(true, this.currentStat.ProductBarcode, mesProcessSeq, ref reStr))
                            {
                                this.currentStat.StatDescribe = "上传MES失败";
                                logRecorder.AddDebugLog(this.nodeName, this.currentStat.StatDescribe);
                                break;
                            }
                        }
                            
                        string checkReStr = "OK";
                        currentTaskDescribe = "放行";
                        if (checkRe == 1)
                        {
                            db1ValsToSnd[0] = db1StatNG;
                            checkReStr = "NG";
                        }
                        else
                        {
                            db1ValsToSnd[0] = db1StatCheckOK;
                        }
                        logRecorder.AddDebugLog(this.nodeName, this.currentStat.ProductBarcode + "检测完成," + checkReStr);
                        this.currentStat.StatDescribe = "气密检测完成," + checkReStr;
                        checkFinished = true;
                        currentTaskPhase++;
                        
                        break;
                    }
                case 5:
                    {
                        //流程结束
                        this.currentStat.StatDescribe = "流程完成";
                        currentTaskDescribe = "流程结束";
                       // this.currentTaskPhase++;
                        break;
                    }
              
            }
            return true;
        }
    }
}
