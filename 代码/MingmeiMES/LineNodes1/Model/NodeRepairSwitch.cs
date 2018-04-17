using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PLProcessModel;
using FTDataAccess.Model;
using FTDataAccess.BLL;
namespace LineNodes
{
    public class NodeRepairSwitch : CtlNodeBaseModel
    {
        
        public override bool BuildCfg(System.Xml.Linq.XElement xe, ref string reStr)
        {

            if (!base.BuildCfg(xe, ref reStr))
            {
                return false;
            }
            this.dicCommuDataDB1[1].DataDescription = "1：检查OK，放行,2：读卡失败,3：进维修,4：产品未绑定";
            this.dicCommuDataDB1[2].DataDescription = "0：允许检测流程开始,1：流程锁定";
            
            for (int i = 0; i < 30; i++)
            {
                this.dicCommuDataDB1[3 + i].DataDescription = string.Format("条码[{0}]", i + 1);
            }
            this.dicCommuDataDB1[33].DataDescription = "前4个工位NG状态,MES联网状态";

            this.dicCommuDataDB2[1].DataDescription = "0:无产品,1：有产品";
            this.dicCommuDataDB2[2].DataDescription = "0:无板,1：有板"; 
           
            return true;
        }
        public override bool ExeBusiness(ref string reStr)
        {
            if (!base.ExeBusiness(ref reStr))
            {
                return false;
            }
            if (!NodeStatParse(ref reStr))
            {
                return false;
            }
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
                        if(string.IsNullOrWhiteSpace(rfidUID))
                        {
                            this.currentStat.StatDescribe = "读RFID卡失败";
                            if (db1ValsToSnd[0] != db1StatRfidFailed)
                            {
                                logRecorder.AddDebugLog(nodeName, "读RFID卡失败");
                            }
                            db1ValsToSnd[0] = db1StatRfidFailed;
                            break;
                        }
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.currentTask.TaskParam = rfidUID;

                        this.ctlTaskBll.Update(this.currentTask);
                        db1ValsToSnd[0] = 0;
                        this.currentStat.StatDescribe = "RFID识别完成";
                        //根据绑定，查询条码，赋条码
                        OnlineProductsModel productBind = productBindBll.GetModelByrfid(rfidUID);
                        if (productBind == null)
                        {
                            db1ValsToSnd[0] = 4;
                            this.currentStat.StatDescribe = "未投产";
                            logRecorder.AddDebugLog(nodeName, "未投产，rfid:" + rfidUID);
                            checkEnable = false;
                            break;
                        }
                        this.currentStat.ProductBarcode = productBind.productBarcode;
                        productBind.currentNode = this.nodeName;
                        productBindBll.Update(productBind);
                        BarcodeFillDB1(productBind.productBarcode, 2);

                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 2:
                    {
                        currentTaskDescribe = "开始判断是否需要进维修";
                        Int16 checkStat = 0;
                        for (int i = 0; i < SysCfgModel.checkStations.Count();i++ )
                        {
                            string checkNodeName = SysCfgModel.checkStations[i];
                            string strSql = string.Format("SERIAL_NUMBER='{0}' and AutoStationName='{1}'", this.currentStat.ProductBarcode, checkNodeName);
                            LOCAL_MES_STEP_INFOModel localCheckModel = localMesBasebll.GetLatestModel(strSql);
                            if(localCheckModel == null || (localCheckModel.CHECK_RESULT == 1))
                            {
                                checkStat |= (Int16)(1 << i);
                            }
                            
                        }
                        int mesRe = 0;
                        string logStr = "";
                        if(checkStat>0)
                        {
                            this.db1ValsToSnd[0] = 3;
                            logStr = string.Format("{0} 进入维修分流线，因为有检测NG项", this.currentStat.ProductBarcode);
                            logRecorder.AddDebugLog(nodeName, logStr);
                        }
                        else
                        {
                            if (!SysCfgModel.MesOfflineMode && PLProcessModel.SysCfgModel.MesCheckEnable)
                            {
                                logRecorder.AddDebugLog(nodeName, "MES下线查询开始:" + this.currentStat.ProductBarcode);
                                mesRe = mesDA.MesAssemDown(new string[] { this.currentStat.ProductBarcode, PLProcessModel.SysCfgModel.mesLineID }, ref reStr);
                                //string mesDownQueryMesID = "";// "DQ-F-0080"; //MES下线许可的MES工位id
                                //if(SysCfgModel.mesLineID=="L10")
                                //{
                                //    mesDownQueryMesID = "DQ-G-0104"; 
                                //}
                                //else
                                //{
                                //   // throw new NotImplementedException();
                                //    mesDownQueryMesID = "DQ-H-0104";
                                //}
                               
                                //mesRe = mesDA.MesDownEnabled(PLProcessModel.SysCfgModel.mesLineID, this.currentStat.ProductBarcode, mesDownQueryMesID, ref reStr);

                            }
                            
                            if(mesRe== 0)
                            {
                                logStr=string.Format("{0} MES下线允许，进入下线位",this.currentStat.ProductBarcode);
                                logRecorder.AddDebugLog(nodeName, logStr);
                                this.db1ValsToSnd[0] = 1;
                            }
                            else if(mesRe== 1)
                            {
                                if(this.db1ValsToSnd[0] != 3)
                                {
                                    logStr = string.Format("{0} 进入维修分流线，因为:mes禁止下线,{1}", this.currentStat.ProductBarcode, reStr);
                                    logRecorder.AddDebugLog(nodeName, logStr);
                                }
                               
                                this.db1ValsToSnd[0] = 3;
                            }
                            else if(reStr.Contains("已下线"))
                            {
                                if (this.db1ValsToSnd[0] != 3)
                                {
                                    logStr = string.Format("{0} ,{1},进入维修分流线", this.currentStat.ProductBarcode, reStr);
                                    logRecorder.AddDebugLog(nodeName, logStr);
                                }
                               
                                this.db1ValsToSnd[0] = 3;
                             
                            }
                            else
                            {
                                break;
                            }
                            

                        }
                        //判断MES网络是否断开
                        if(mesRe != 0)
                        {
                            checkStat |= (1<<4);
                        }
                        this.db1ValsToSnd[32] = checkStat;
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 3:
                    {
                        currentTaskDescribe = "维修分流完成";
                        this.currentStat.StatDescribe = "流程完成";
                        break;
                    }
                default:
                    break;
            }
            return true;
        }
    }
}
