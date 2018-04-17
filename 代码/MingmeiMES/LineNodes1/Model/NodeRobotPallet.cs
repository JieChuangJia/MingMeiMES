using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PLProcessModel;
using LogInterface;
using FTDataAccess.Model;
using FTDataAccess.BLL;
namespace LineNodes
{

    /// <summary>
    /// 机器人码垛控制节点
    /// </summary>
    public class NodeRobotPallet:CtlNodeBaseModel
    {
        private ProductSizeCfgBll productCfgBll = new ProductSizeCfgBll();
        private ProductPacsizeDefBll packsizeDefBll = new ProductPacsizeDefBll();
        private short packageNotCfg = 4;//包装配置不存在
        public override bool BuildCfg(System.Xml.Linq.XElement xe, ref string reStr)
        {
            if (!base.BuildCfg(xe, ref reStr))
            {
                return false;
            }
            this.dicCommuDataDB1[1].DataDescription = "1：检查OK，放行，2：读条码失败,3:MES非下线产品,4:无尺寸配置";
            this.dicCommuDataDB1[2].DataDescription = "垛型编号（从1开始）";
            this.dicCommuDataDB1[3].DataDescription = "物料编号";
            for (int i = 0; i < 30; i++)
            {
                this.dicCommuDataDB1[4 + i].DataDescription = string.Format("条码[{0}]", i + 1);
            }
            this.dicCommuDataDB2[1].DataDescription = "0:无纸箱,1：有纸箱";
            return true;
        }
        public override bool ExeBusiness(ref string reStr)
        {
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
                        if (this.currentStat.Status == EnumNodeStatus.设备故障)
                        {
                            break;
                        }
                        this.currentStat.Status = EnumNodeStatus.设备使用中;
                        this.currentStat.ProductBarcode = "";
                        this.currentStat.StatDescribe = "设备使用中";
                        //读条码，做校验
                        currentTaskDescribe = "开始读产品条码";
                        string boxBarcode = barcodeRW.ReadBarcode();
                        if (boxBarcode == string.Empty || boxBarcode.Length < 26)
                        {
                            db1ValsToSnd[0] =  db1StatRfidFailed;
                            this.currentStat.StatDescribe= "无效的条码，位数不足26位！";
                            currentTaskDescribe = "无效的条码，位数不足26位！";
                            break;
                        }
                        else
                        {
                            db1ValsToSnd[0] = 0;
                        }
                        logRecorder.AddDebugLog(nodeName, "扫到纸箱条码：" + boxBarcode);
                        //状态赋条码, 
                        BarcodeFillDB1(boxBarcode, 3);
                        this.currentStat.ProductBarcode = boxBarcode;
                       
                        string productTypeCode = boxBarcode.Substring(0, 13);
                        ProductSizeCfgModel cfg = productCfgBll.GetModel(productTypeCode);
                        if(cfg == null)
                        {
                            db1ValsToSnd[0] = packageNotCfg;
                            this.currentStat.StatDescribe = "产品尺寸配置不存在";
                            logRecorder.AddDebugLog(nodeName, "包装配置不存在，" + boxBarcode);
                            currentTaskDescribe = "产品尺寸配置不存在";
                            checkEnable = false;
                            break;
                        }
                        ProductPacsizeDefModel packSize = packsizeDefBll.GetModel(cfg.packageSize);
                        db1ValsToSnd[1] = (short)packSize.packageSeq;
                        db1ValsToSnd[2] = (short)(cfg.cataSeq%256); 
                        if(db1ValsToSnd[2] ==0)
                        {
                            db1ValsToSnd[2] = 1;
                        }
                        
                        currentTaskPhase++;
                  
                        OutputRecord(this.currentStat.ProductBarcode);
                        break;
                    }
                case 2:
                    {
                        db1ValsToSnd[0] = db1StatCheckOK; 
                        this.currentStat.StatDescribe = "机器人码垛完成";
                        if (LineMonitorPresenter.DebugMode)
                        {
                            logRecorder.AddDebugLog(this.nodeName, "码垛完成");
                        }
                        currentTaskDescribe = "机器人码垛完成";
                        currentTaskPhase++;
                        break;
                    }
                case 3:
                    {
                        this.currentStat.StatDescribe = "流程完成";
                        currentTaskDescribe = "流程完成";
                        
                        break;
                    }
                   
                default:
                    break;
            }
            return true;
        }
    }
}
