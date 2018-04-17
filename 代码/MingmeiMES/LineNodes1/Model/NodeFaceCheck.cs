using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using PLProcessModel;
using LogInterface;
using DevInterface;
using DevAccess;
using FTDataAccess.Model;
using FTDataAccess.BLL;
namespace LineNodes
{
    public class NodeFaceCheck:CtlNodeBaseModel
    {
        protected delegate bool DelegateSndPrinter(string productBarcode, ref string reStr);
        
       
        //DetectCodeDefBll detectCodeDefbll = new DetectCodeDefBll();
       
        private DateTime rfidFailSt ;
        private bool rfidTimeCounterBegin = false; //RFID超时计时开始
        private int checkRe = 0;
        public NodeFaceCheck()
        {
            processName = "外观检查";
           
        }
       
        public override bool BuildCfg(System.Xml.Linq.XElement xe, ref string reStr)
        {
            if (!base.BuildCfg(xe, ref reStr))
            {
                return false;
            }
            this.dicCommuDataDB1[1].DataDescription = "1：检查OK，放行，2：读卡/条码失败，3：NG，4：不需要检测，5:未投产,6：前面工序有NG,7：MES禁止下线";
            this.dicCommuDataDB1[2].DataDescription = "0：允许流程开始，1:流程锁定";
            for (int i = 0; i < 30; i++)
            {
                this.dicCommuDataDB1[3 + i].DataDescription = string.Format("条码[{0}]", i + 1);
            }

            this.dicCommuDataDB2[1].DataDescription = "0:无产品,1：有产品";
            this.dicCommuDataDB2[2].DataDescription = "0:无板,1：有板"; 
            this.dicCommuDataDB2[3].DataDescription = "1：检测OK,2：检测NG";
            this.dicCommuDataDB2[4].DataDescription = "不合格项编码";
            return true;
        }
        public override bool ExeBusiness(ref string reStr)
        {
            
            MessLossCheck();
            if(!base.ExeBusiness(ref reStr))
            {
                return false;
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
                      
                        db1ValsToSnd[1] = 1;//流程锁定
                        if (this.currentStat.Status == EnumNodeStatus.设备故障)
                        {
                            break;
                        }
                      
                        this.currentStat.Status = EnumNodeStatus.设备使用中;
                        this.currentStat.ProductBarcode = "";
                        this.currentStat.StatDescribe = "设备使用中";
                        //开始读卡
                        if(string.IsNullOrWhiteSpace(rfidUID))
                        {
                            if (SysCfgModel.SimMode)
                            {
                                rfidUID = SimRfidUID;
                            }
                            else
                            {
                                rfidUID = rfidRW.ReadUID();
                            }
                        }
                       
                        currentTaskDescribe = "开始读RFID";
                        if (!string.IsNullOrWhiteSpace(rfidUID))
                        {
                            this.currentTask.TaskPhase = this.currentTaskPhase;
                            this.currentTask.TaskParam = rfidUID;

                            this.ctlTaskBll.Update(this.currentTask);
                            db1ValsToSnd[0] = 0;
                            this.currentStat.StatDescribe = "RFID识别完成";
                            //根据绑定，查询条码，赋条码
                            OnlineProductsModel productBind = productBindBll.GetModelByrfid(rfidUID);
                            if (productBind == null)
                            {
                                db1ValsToSnd[0]= db1StatCheckUnbinded;
                               // this.currentTaskPhase = 5;
                                this.currentStat.StatDescribe = "未投产";
                                logRecorder.AddDebugLog(nodeName, "未投产，rfid:" + rfidUID);
                                checkEnable = false; 
                                break;
                            }
                            productBind.currentNode = this.nodeName;
                            productBindBll.Update(productBind);
                            BarcodeFillDB1(productBind.productBarcode, 2);
                            
                           //reDetectQuery=2,无记录，继续后面的流程
                           
                            //状态赋条码, 
                            this.currentStat.ProductBarcode = productBind.productBarcode;
                            logRecorder.AddDebugLog(this.nodeName, this.currentStat.ProductBarcode + "开始检测");
                            //查询本地数据库，之前工位是否有不合格项，若有，下线
                            
                            if (!SysCfgModel.DebugMode)
                            {
                                int reDetectQuery = ReDetectQuery(productBind.productBarcode);
                                ////if (0 == reDetectQuery)
                                ////{
                                ////    db1ValsToSnd[0] = db1StatCheckOK;
                                //////    checkEnable = false;
                                ////    logRecorder.AddDebugLog(nodeName, string.Format("{0}本地已经存在检验记录,检验结果：OK", productBind.productBarcode));
                                ////  //  break;
                                ////}
                                //else if (1 == reDetectQuery)
                                //{
                                //    db1ValsToSnd[0] = db1StatNG;
                                //  //  checkEnable = false;
                                //    logRecorder.AddDebugLog(nodeName, string.Format("{0}本地已经存在检验记录,检验结果：NG", productBind.productBarcode));
                                // //   break;
                                //}

                                if (!PreDetectCheck(productBind.productBarcode))
                                {
                                    db1ValsToSnd[0] = db1StatCheckPreNG;//
                                    logRecorder.AddDebugLog(this.nodeName, string.Format("{0} 在前面工位有检测NG项", productBind.productBarcode));
                                  //  checkEnable = false;
                                  //  break;
                                }
                                if (!LossCheck(productBind.productBarcode, ref reStr))
                                {
                                    db1ValsToSnd[0] = db1StatCheckPreNG;//
                                    logRecorder.AddDebugLog(this.nodeName, string.Format("{0} 检测漏项,{1}", productBind.productBarcode, reStr));
                                   // checkEnable = false;

                                   // break;
                                }
                            } 
        
                            currentTaskPhase++;
                            this.currentTask.TaskPhase = this.currentTaskPhase;
                            this.ctlTaskBll.Update(this.currentTask);
                        }
                        else
                        {
                          
                            if(!rfidTimeCounterBegin)
                            {
                                //logRecorder.AddDebugLog(nodeName, "读RFID卡失败");
                                rfidFailSt = System.DateTime.Now;
                            }
                            rfidTimeCounterBegin = true;
                            TimeSpan ts = System.DateTime.Now - rfidFailSt;
                            if(ts.TotalSeconds>SysCfgModel.RfidDelayTimeout)
                            {
                                if (db1ValsToSnd[0] != db1StatRfidFailed)
                                {
                                    logRecorder.AddDebugLog(nodeName, "读RFID卡失败");
                                }
                                db1ValsToSnd[0] = db1StatRfidFailed;
                                
                            }
                            this.currentStat.Status = EnumNodeStatus.无法识别;
                            this.currentStat.StatDescribe = "读RFID卡失败";
                            break;
                        }
                       
                        break;
                    }
                case 2:
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
                            if(string.IsNullOrWhiteSpace(detectCodes))
                            {
                                break;
                            }
                            OutputRecord(this.currentStat.ProductBarcode);
                        }
                        currentTaskDescribe = "开始保存结果到本地";
                        if (!MesDatalocalSave(this.currentStat.ProductBarcode,checkRe, detectCodes, "", 0))
                        {
                            logRecorder.AddLog(new LogModel(this.nodeName, "保存检测数据到本地数据库失败", EnumLoglevel.警告));
                            break;
                        }
                        currentTaskDescribe = "开始上传结果到MES";
                        if (!UploadMesdata(true,this.currentStat.ProductBarcode, mesProcessSeq, ref reStr))
                        {
                            this.currentStat.StatDescribe = "上传MES失败";
                            logRecorder.AddDebugLog(this.nodeName, this.currentStat.StatDescribe);
                            break;
                        }
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 3:
                    {
                        currentTaskDescribe = "放行";
                        this.currentStat.StatDescribe = "外观检测完成";
                        string checkreStr = "OK";
                        if (checkRe == 1)
                        {
                            checkreStr = "NG";
                            db1ValsToSnd[0] = db1StatNG;
                        }
                        else
                        {
                            db1ValsToSnd[0] = db1StatCheckOK;//允许下线
                            //if (PLProcessModel.SysCfgModel.PrienterEnable)
                            //{
                            //    SendPrinterinfo(this.currentStat.ProductBarcode);//异步发送
                            //}
                       
                        }
                        logRecorder.AddDebugLog(this.nodeName, this.currentStat.ProductBarcode + "检测完成," + checkreStr);
                        this.currentStat.StatDescribe = "下线";
                        checkFinished = true;
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 4:
                    {
                        this.currentStat.StatDescribe = "流程完成";
                        currentTaskDescribe = "流程结束";
                        //DevCmdReset();
                       
                        //checkFinished = true;
                        break;
                    }
               
                default:
                    break;
            }
            return true;
        }
        
        /// <summary>
        /// MES漏传检查，用于MES断网再恢复时
        /// </summary>
        private void MessLossCheck()
        {
            if(SysCfgModel.MesOfflineMode)
            {
                return;
            }
          //  string[] mesProcessSeq = new string[] { "MES投产位", "RQ-Z8005", "RQ-Z8007", "RQ-Z8008", "RQ-Z8011", "RQ-Z8012", "RQ-Z8013", "RQ-Z8030" };
           // string[] mesProcessSeq = new string[] {  "RQ-Z8005", "RQ-Z8007", "RQ-Z8008", "RQ-Z8011", "RQ-Z8012", "RQ-Z8013", "RQ-Z8030" };
            string strWhere = string.Format(" AutoStationName='{0}' and UPLOAD_FLAG=0", this.nodeName);

            List<LOCAL_MES_STEP_INFOModel> unUploads = localMesBasebll.GetModelList(strWhere);
            if(unUploads != null && unUploads.Count()>0)
            {
                foreach(LOCAL_MES_STEP_INFOModel infoModel in unUploads)
                {
                    string reStr = "";
                    if (!UploadMesdata(true, infoModel.SERIAL_NUMBER, mesProcessSeq, ref reStr))
                    {
                        logRecorder.AddDebugLog(this.nodeName, infoModel.SERIAL_NUMBER+",上传MES失败");
                        
                    }
                }
            }
        }
       
       
       
    
    }
}
