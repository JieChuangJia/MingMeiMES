using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PLProcessModel;
using LogInterface;
using DevInterface;
using DevAccess;
using FTDataAccess.Model;
namespace LineNodes
{
    /// <summary>
    /// 提前下线节点控制
    /// </summary>
    public class NodeProductSwitch:CtlNodeBaseModel
    {
        private string barcode = "";
        private IMesAccess mesDA = null;
        private IPrinterInfoDev prienterRW = null;
        public IPrinterInfoDev PrienterRW { get { return prienterRW; } set { prienterRW = value; } }

        public NodeProductSwitch()
        {
            if(NodeFactory.SimMode)
            {
                mesDA = new MesDASim();
            }
            else
            {
                mesDA = new MesDA();
            }
            
        }
        public override bool BuildCfg(System.Xml.Linq.XElement xe, ref string reStr)
        {
            if (!base.BuildCfg(xe, ref reStr))
            {
                return false;
            }
            this.dicCommuDataDB1[1].DataDescription = "1：MES允许下线，2：MES禁止下线，4：读卡/条码失败，未投产，8：需要检测，16：不需要检测";
            for (int i = 0; i < 30; i++)
            {
                this.dicCommuDataDB1[2 + i].DataDescription = string.Format("条码[{0}]", i + 1);
            }
            this.dicCommuDataDB2[1].DataDescription = "0:无板,1：有产品,2：空板";
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
                case 0:
                    {
                        db1ValsToSnd[0] = db1StatCheckNoneed; //空板进入，放行
                        this.currentStat.Status = EnumNodeStatus.工位有板;
                        this.currentStat.ProductBarcode = "";
                        this.currentStat.StatDescribe = "空板";
                        break;
                    }
                case 1:
                    {
                        DevCmdReset();
                        rfidUID = string.Empty;
                        this.currentStat.Status = EnumNodeStatus.设备空闲;
                        this.currentStat.ProductBarcode = "";
                        this.currentStat.StatDescribe = "设备空闲";
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
                        if (!string.IsNullOrWhiteSpace(rfidUID))
                        {
                            this.currentStat.StatDescribe = "RFID识别完成";
                            //根据绑定，查询条码，赋条码
                            OnlineProductsModel productBind = productBindBll.GetModelByrfid(rfidUID);
                            if (productBind == null)
                            {
                                db1ValsToSnd[0] = db1StatRfidFailed;
                                this.currentTaskPhase = 4;
                                this.currentStat.StatDescribe = "未投产";
                                break;
                            }

                            //查询本地数据库，之前工位是否有不合格项，若有，下线
                            if (!PreDetectCheck(productBind.productBarcode))
                            {
                                db1ValsToSnd[0] = db1StatNG;//
                               
                            }
                            barcode = productBind.productBarcode;
                            productBind.currentNode = this.nodeName;
                            productBindBll.Update(productBind);
                            BarcodeFillDB1(productBind.productBarcode, 1);
                            //状态赋条码, 
                            this.currentStat.ProductBarcode = productBind.productBarcode;
                            currentTaskPhase++;
                        }
                        else
                        {
                            db1ValsToSnd[0] = db1StatRfidFailed;
                            this.currentStat.Status = EnumNodeStatus.无法识别;
                            this.currentStat.StatDescribe = "读RFID卡失败";
                            break;
                        }
                        if (LineMonitorPresenter.DebugMode)
                        {
                            logRecorder.AddDebugLog(this.nodeName, "读卡完成:" + rfidUID);
                        }
                        break;
                    }
                case 3:
                    {
                        //查询MES是否允许下线
                        if(0 !=mesDA.MesAssemDown(new string[]{LineMonitorPresenter.mesLineID,barcode},ref reStr))
                        {
                            db1ValsToSnd[0]=db1StatNG; //禁止下线
                           
                            //检测不合格，下线
                            OutputRecord(this.currentStat.ProductBarcode);
                        }
                        else
                        {
                              db1ValsToSnd[0]=db1StatCheckOK;//允许下线
                        }
                          //发送条码到贴标机
                        if(!prienterRW.SndBarcode(barcode,ref reStr))
                        {
                            ThrowErrorStat("给贴标机发送条码失败",EnumNodeStatus.设备故障);
                            break;
                        }
                        this.currentStat.StatDescribe = "下线";
                        if (LineMonitorPresenter.DebugMode)
                        {
                            logRecorder.AddDebugLog(this.nodeName, "下线");
                        }
                        currentTaskPhase++;
                        break;
                    }
                case 4:
                    {
                        //流程结束
                        break;
                    }
                default:
                    break;
            }
            return true;
        }
    }
}
