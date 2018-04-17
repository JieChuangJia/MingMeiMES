using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PLProcessModel;
using LogInterface;
using FTDataAccess.Model;
using DevInterface;
using DevAccess;
//using FTDataAccess.Model;
using FTDataAccess.BLL;
namespace LineNodes
{
    public class NodePrePrinter : CtlNodeBaseModel
    {
        private IPrinterInfoDev prienterRW = null;
        private List<string> printList = new List<string>();//
        private object lockPrintbuf = new object();
        private DateTime rfidFailSt;
        private bool rfidTimeCounterBegin = false; //RFID超时计时开始
        public IPrinterInfoDev PrienterRW { get { return prienterRW; } set { prienterRW = value; } }
        public NodePrePrinter()
        {

        }
        public override bool BuildCfg(System.Xml.Linq.XElement xe, ref string reStr)
        {
            if (!base.BuildCfg(xe, ref reStr))
            {
                return false;
            }
            this.dicCommuDataDB1[1].DataDescription = "1：检查OK，放行，2：NG，4：读卡/条码失败，未投产，8：需要检测，16：不需要检测，32：前面工序有NG,64：MES禁止下线";
           

            this.dicCommuDataDB2[1].DataDescription = "0:无板,1：有产品,2：空板";
           // this.dicCommuDataDB2[2].DataDescription = "1：检测OK,2：检测NG";
          //  this.dicCommuDataDB2[3].DataDescription = "不合格项编码";
            return true;
        }
        public override bool ExeBusiness(ref string reStr)
        {
            if (!NodeStatParse(ref reStr))
            {
                return false;
            }
            switch (currentTaskPhase)
            {
               
                case 1:
                    {
                        DevCmdReset();
                        rfidUID = string.Empty;
                        this.currentStat.Status = EnumNodeStatus.设备空闲;
                        this.currentStat.ProductBarcode = "";
                        this.currentStat.StatDescribe = "设备空闲";
                      
                        checkFinished = false;
                        currentTaskDescribe = "等待有板信号";
                        break;
                    }
                case 2:
                    {

                      
                        if (this.currentStat.Status == EnumNodeStatus.设备故障)
                        {
                            break;
                        }

                        this.currentStat.Status = EnumNodeStatus.设备使用中;
                        this.currentStat.ProductBarcode = "";
                        this.currentStat.StatDescribe = "设备使用中";
                        //开始读卡
                        if (!SimMode)
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
                                // this.currentTaskPhase = 5;
                                this.currentStat.StatDescribe = "未投产";
                                logRecorder.AddDebugLog(nodeName, "未投产，rfid:" + rfidUID);
                                checkEnable = false;
                                break;
                            }
                            currentTaskPhase++;
                        }
                        else
                        {

                            if (!rfidTimeCounterBegin)
                            {
                                //logRecorder.AddDebugLog(nodeName, "读RFID卡失败");
                                rfidFailSt = System.DateTime.Now;
                            }
                            rfidTimeCounterBegin = true;
                            TimeSpan ts = System.DateTime.Now - rfidFailSt;

                            if (ts.TotalSeconds > SysCfgModel.RfidDelayTimeout)
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
                case 3:
                    {
                        if (PLProcessModel.SysCfgModel.PrienterEnable)
                        {
                            SendPrinterinfo(this.currentStat.ProductBarcode);//异步发送
                        }
                        currentTaskPhase++;

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
        private void SendPrinterinfo(string productBarcode)
        {
            /*
            DelegateSndPrinter dlgt = new DelegateSndPrinter(AsySndPrinterinfo);
            string reStr = "";
            IAsyncResult ar = dlgt.BeginInvoke(productBarcode, ref reStr, null, dlgt);
             */
            lock (lockPrintbuf)
            {
                this.printList.Add(productBarcode);
            }


        }
        /// <summary>
        /// 贴标队列处理,周期执行
        /// </summary>
        public void PrinterListProcess()
        {
            lock (lockPrintbuf)
            {
                if (!PLProcessModel.SysCfgModel.PrienterEnable)
                {
                    this.printList.Clear();
                    return;
                }
                if (this.printList.Count() == 0)
                {
                    return;
                }
                string productBarcode = this.printList[0];
                int mesRe = 0;
                string reStr = "";
                if (!SysCfgModel.MesOfflineMode && PLProcessModel.SysCfgModel.MesCheckEnable)
                {
                    mesRe = mesDA.MesAssemDown(new string[] { productBarcode, LineMonitorPresenter.mesLineID }, ref reStr);
                }
                if (0 == mesRe)
                {
                    if (!PLProcessModel.SysCfgModel.PrienterEnable)
                    {
                        this.printList.Clear();
                        return;
                    }
                    bool re = prienterRW.SndBarcode(productBarcode, ref reStr);
                    if (!re)
                    {
                        string failInfo = string.Format("给贴标机发送条码{0} 失败,错误信息：{1}", productBarcode, reStr);
                        logRecorder.AddDebugLog(nodeName, failInfo);
                    }
                    else
                    {
                        this.printList.Remove(productBarcode);
                        logRecorder.AddDebugLog(nodeName, "成功发送贴标条码：" + productBarcode + "," + reStr);
                    }
                }
                else
                {
                    MesStatRecord mesStat = NodePack.GetMequeryStat(productBarcode);
                    if (mesStat != null)
                    {
                        if (3 == mesStat.StatVal)
                        {
                            logRecorder.AddDebugLog(this.nodeName, productBarcode + ":MES禁止下线:" + reStr);
                            this.printList.Remove(productBarcode);
                            return;
                        }
                    }
                }
            }
        }
    }
}
