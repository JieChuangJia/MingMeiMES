using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using DevInterface;
using DevAccess;
using PLProcessModel;
namespace LineNodes
{

    /// <summary>
    /// 产线基类模型
    /// </summary>
    public class CtlLineBaseModel
    {
        private string lineName = "产线名称";
        private List<CtlNodeBaseModel> nodeList = new List<CtlNodeBaseModel>();
        private List<CtlDevBaseModel> devList = new List<CtlDevBaseModel>();
        private List<CtlNodeStatus> nodeStatusList = new List<CtlNodeStatus>();
        #region 公开接口
        public string LineName { get { return lineName; } set { lineName = value; } }
        public List<CtlNodeBaseModel> NodeList { get { return nodeList; } set { nodeList = value; } }
        public List<CtlDevBaseModel> DevList { get { return devList; } set { devList = value; } }
        public List<CtlNodeStatus> NodeStatusList { get { return nodeStatusList; } set { nodeStatusList = value; } }
        public CtlLineBaseModel()
        {

        }
        public CtlLineBaseModel(string lineName)
        {
            this.lineName = lineName;
        }
        public  CtlNodeBaseModel FindNode(string nodeID)
        {
            CtlNodeBaseModel nodeFnd = null;
            if (this.nodeList == null)
            {
                return null;
            }
            foreach (CtlNodeBaseModel node in this.nodeList)
            {
                if (node != null && node.NodeID == nodeID)
                {
                    nodeFnd = node;
                }
            }
            return nodeFnd;
        }
        public bool AllocCommObj(IList<IrfidRW> rfidList,IList<IBarcodeRW> barcodeList,IList<IPlcRW> plcRWList,List<MingmeiDeviceAcc> ccdRWList,ref string reStr)
        {
            try
            {

                foreach (CtlDevBaseModel dev in devList)
                {
                    dev.PlcRW = GetPlcByID(plcRWList, dev.PlcID);
                }
                foreach(CtlNodeBaseModel node in nodeList)
                {
                    node.PlcRW = GetPlcByID(plcRWList,node.PlcID); // this.plcRWs[2];         }
                    if(node.PlcID2>0)
                    {
                        node.PlcRW2 = GetPlcByID(plcRWList, node.PlcID2);
                    }
                    //if(node.NodeID== "OPB001")
                    //{

                    //    (node as NodePalletBind).plcRW2 = GetPlcByID(plcRWList, (node as NodePalletBind).plcID2);
                    //}
                    if(node.BarcodeID2>0)
                    {
                        node.BarcodeRW2 = GetBarcoderRWByID(barcodeList, node.BarcodeID2);
                    }
                    if(node.BarcodeID>0)
                    {
                        node.BarcodeRW = GetBarcoderRWByID(barcodeList,node.BarcodeID);
                    }
                    List<IrfidRW> rfids = new List<IrfidRW>();
                    foreach(byte rid in node.RfidIDS)
                    {
                        IrfidRW rfid = GetRfidByID(rfidList, rid);
                        if(rfid != null)
                        {
                            rfids.Add(rfid);
                        }
                    }
                    node.RfidRWList = rfids;
                    node.CCDDevAcc = GetCCDByID(ccdRWList,node.ccdDevID);
                }
                return true;
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return false;
               
            }
        }
        #endregion
        #region 虚接口
        public virtual bool ParseCfg(XElement xe,ref string reStr)
        {
            try
            {
               this.lineName= xe.Attribute("name").Value.ToString();
               XElement devCfgRoot = xe.Element("DevCfg");
               if(devCfgRoot != null)
               {
                   IEnumerable<XElement> devXEList = devCfgRoot.Elements("Dev");
                   foreach(XElement el in devXEList)
                   {
                       CtlDevBaseModel ctlDev = new CtlDevBaseModel();
                       if(!ctlDev.ParseCfg(el,ref reStr))
                       {
                           return false;
                       }
                       devList.Add(ctlDev);
                   }
               }
               XElement nodeCfgRoot = xe.Element("NodeCfg");
                if(nodeCfgRoot != null)
                {
                    IEnumerable<XElement> nodeXEList = nodeCfgRoot.Elements("Node");
                    foreach (XElement el in nodeXEList)
                    {
                        string className = (string)el.Attribute("className");
                        CtlNodeBaseModel ctlNode = null;
                        switch (className)
                        {

                            case "LineNodes.NodePalletBind":
                                {
                                    ctlNode = new NodePalletBind();
                                    break;
                                }
                            case "LineNodes.NodeScrewLock":
                                {
                                    ctlNode = new NodeScrewLock();
                                    break;
                                }
                            case "LineNodes.NodeUV":
                                {
                                    ctlNode = new NodeUV();
                                    break;
                                }
                            case "LineNodes.NodeDianjiao":
                                {
                                    ctlNode = new NodeDianjiao();
                                    break;
                                }
                            case "LineNodes.NodeSwitchBind":
                                {
                                    ctlNode = new NodeSwitchBind();
                                    break;
                                }
                            case "LineNodes.NodeCCDCheck":
                                {
                                    ctlNode = new NodeCCDCheck();
                                    break;
                                }
                            case "LineNodes.NodeLaserClean":
                                {
                                    ctlNode = new NodeLaserClean();
                                    break;
                                }
                            case "LineNodes.NodeWeld":
                                {
                                    ctlNode = new NodeWeld();
                                    break;
                                }
                            case "LineNodes.NodePackFasten":
                                {
                                    ctlNode = new NodePackFasten();
                                    break;
                                }
                            case "LineNodes.NodePackLabel":
                                {
                                    ctlNode = new NodePackLabel();
                                    break;
                                }
                            case "LineNodes.NodePackWeld":
                                {
                                    ctlNode = new NodePackWeld();
                                    break;
                                }
                            case "LineNodes.NodePackOutput":
                                {
                                    ctlNode = new NodePackOutput();
                                    break;
                                }
                            case "LineNodes.NodeUpDownMachine":
                                {
                                    ctlNode = new NodeUpDownMachine();
                                    break;
                                }
                            case "LineNodes.NodeTailRobot":
                                {
                                    ctlNode = new NodeTailRobot();
                                    break;
                                }
                            case "LineNodes.NodeManualStation":
                                {
                                    ctlNode = new  NodeManualStation();
                                    break;
                                }
                            case "LineNodes.BakeStation":
                                {
                                    ctlNode = new BakeStation();
                                    break;
                                }
                            case "LineNodes.NodeCManualStation":
                                {
                                    ctlNode = new  NodeCManualStation();
                                    break;
                                }
                            
                            default:
                                break;
                        }
                        if(ctlNode != null)
                        {
                            if(this.lineName == "A线")
                            {
                                ctlNode.LineID = 1;
                                
                            }
                            else if(this.lineName == "B线")
                            {
                                ctlNode.LineID = 2;
                            }
                            else
                            {
                                ctlNode.LineID = 3;
                            }
                            if (!ctlNode.BuildCfg(el, ref reStr))
                            {
                                return false;
                            }
                            this.nodeList.Add(ctlNode);
                           
                        } 
                    }
                }

               return true;
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return false;
                
            }
        }
        #endregion
        #region 私有接口
        private IrfidRW GetRfidByID(IList<IrfidRW> rfidList,byte readerID)
        {
            foreach (IrfidRW rfid in rfidList)
            {
                if (rfid != null && rfid.ReaderID == readerID)
                {
                    return rfid;
                }
            }
            return null;
        }
        public IPlcRW GetPlcByID(IList<IPlcRW> plcRWList,int plcID)
        {
            foreach (IPlcRW plcRW in plcRWList)
            {
                if (plcID == plcRW.PlcID)
                {
                    return plcRW;
                }
            }
            return null;
        }
     
        public IBarcodeRW GetBarcoderRWByID(IList<IBarcodeRW> barcodeList,int barcodReaderID)
        {
            foreach (IBarcodeRW barcodeReader in barcodeList)
            {
                if (barcodeReader != null && barcodeReader.ReaderID == barcodReaderID)
                {
                    return barcodeReader;
                }
            }
            return null;
        }
       public MingmeiDeviceAcc GetCCDByID(List<MingmeiDeviceAcc> ccdRWList,int ccdID)
        {
           foreach(MingmeiDeviceAcc ccd in ccdRWList)
           {
               if(ccd != null && ccd.DevID == ccdID)
               {
                   return ccd;
               }
           }
           return null;
        }
        #endregion

    }
}
