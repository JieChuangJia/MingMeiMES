using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.IO.Ports;
using PLProcessModel;
using DevAccess;
using DevInterface;
namespace LineNodes
{
    public  class NodeFactory
    {
        #region 数据
        public static string[] rfidSimData = new string[] { "uid201606101234", "uid201606101236", "uid201606101237" };
        public static string[] barcodeSimData = new string[] { "71125800010L411606101234", "71125800010L411606101235", "71125800010L411606101236" };
        
        public static bool SimMode = false;//
        private List<CtlNodeBaseModel> nodeList = null;
        private List<ThreadBaseModel> threadList = null;
        private List<IPlcRW> plcRWList = null;
        private List<IrfidRW> rfidList = null;
        private List<IBarcodeRW> barcodeRWList = null;
        private List<IAirlossDetectDev> airDetectList = null;
        private List<IPrinterInfoDev> printerList = null;
        public List<IPlcRW> PlcRWList { get { return plcRWList; } }
        public List<IrfidRW> RfidRWList { get { return rfidList; } }
        public List<IBarcodeRW> BarcodeRWList { get { return barcodeRWList; } }
        public List<IAirlossDetectDev> AirDetectRWList { get { return airDetectList; } }
        public List<IPrinterInfoDev> PrinterRWList { get { return printerList; } }
        
        #endregion
        #region 公有接口
        public bool ConfigInit(ref string reStr)
        {
            nodeList = new List<CtlNodeBaseModel>();
            threadList = new List<ThreadBaseModel>();
            rfidList = new List<IrfidRW>();
            barcodeRWList = new List<IBarcodeRW>();
            airDetectList = new List<IAirlossDetectDev>();
            printerList = new List<IPrinterInfoDev>();
            plcRWList = new List<IPlcRW>();
            IPlcRW plcRW = null;
            IPrinterInfoDev prienterRW = null;
            try
            {
                if(!PLProcessModel.SysCfgModel.LoadCfg(ref reStr))
                {
                    return false;
                }
                string xmlCfgFile = System.AppDomain.CurrentDomain.BaseDirectory + @"data/DevConfigFTzj.xml";
                if (!File.Exists(xmlCfgFile))
                {
                    reStr = "系统配置文件：" + xmlCfgFile + " 不存在!";
                    return false;
                }
                XElement root = XElement.Load(xmlCfgFile);
                XElement runModeXE = root.Element("sysSet").Element("RunMode");
                string simStr=runModeXE.Attribute("sim").Value.ToString().ToUpper();
                if(simStr== "TRUE")
                {
                    NodeFactory.SimMode = true;
                    SysCfgModel.SimMode = true;
                }
                else
                {
                    NodeFactory.SimMode = false;
                    SysCfgModel.SimMode = false;
                }
                //1 解析结点信息
                XElement CtlnodeRoot = root.Element("CtlNodes");
                if(!ParseCtlnodes(CtlnodeRoot,ref reStr))
                {
                    return false;
                }
                //2 解析通信设备信息
                if (NodeFactory.SimMode)
                {
                    plcRW = new PlcRWSim();
                    plcRWList.Add(plcRW);
                    prienterRW = new PrinterRWSim(1);
                    printerList.Add(prienterRW);
                    for (int i = 0; i < 13; i++)
                    {
                        int rfidID = 0;// i + 1;
                        IrfidRW rfidRW = new rfidRWSim();
                        rfidRW.ReaderID = (byte)rfidID;
                        rfidList.Add(rfidRW);

                    }
                    for (int i = 0; i < 3; i++)
                    {
                        int barcodeID = i + 1;
                        IBarcodeRW barscanner = new BarcodeRWSim(barcodeID);
                        barcodeRWList.Add(barscanner);
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        int airDetectID = i + 1;
                        IAirlossDetectDev airDetecter = new AirDetectRWSim(airDetectID);
                        airDetectList.Add(airDetecter);
                    }
                }
                else
                {
                    XElement commDevXERoot = root.Element("CommDevCfg");
                    if (!ParseCommDevCfg(commDevXERoot, ref reStr))
                   {
                       return false;
                   }
                    plcRW = plcRWList[0];
                }

                //3 给节点分配设备读写接口对象

                for (int i = 0; i < nodeList.Count(); i++)
                {
                    CtlNodeBaseModel node = nodeList[i];
                    node.PlcRW = plcRW;
                    node.SimMode = NodeFactory.SimMode;
                }
                for (int i = 0; i < nodeList.Count() - 1; i++)
                {
                    CtlNodeBaseModel node = nodeList[i];
                    node.RfidRW = rfidList[i];
                }
                prienterRW = PrinterRWList[0];
                GetNodeByID("1001").BarcodeRW = barcodeRWList[0];
                GetNodeByID("8001").BarcodeRW = barcodeRWList[1];
                GetNodeByID("9001").BarcodeRW = barcodeRWList[2];
                //(GetNodeByID("2001") as NodeAirlossCheck).AirDetectRW = airDetectList[0];
                //(GetNodeByID("2002") as NodeAirlossCheck).AirDetectRW = airDetectList[1];
                //(GetNodeByID("2003") as NodeAirlossCheck).AirDetectRW = airDetectList[2];
               // (GetNodeByID("6001") as NodeFaceCheck).PrienterRW = prienterRW;
              
                //4 线程-结点分配
                XElement ThreadnodeRoot = root.Element("ThreadAlloc");
                if (!ParseTheadNodes(ThreadnodeRoot, ref reStr))
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                reStr = "加载系统配置文件出现异常：" + ex.ToString();
                return false;
            }


            return true;
        }
        /// <summary>
        /// 创建控制节点
        /// </summary>
        /// <param name="reStr"></param>
        /// <returns></returns>
        public List<CtlNodeBaseModel> GetNodes(ref string reStr)
        {
            return nodeList;
        }
        private CtlNodeBaseModel GetNodeByID(string nodeID)
        {
            foreach (CtlNodeBaseModel node in nodeList)
            {
                if (node.NodeID == nodeID)
                {
                    return node;
                }
            }
            return null;
        }
        private CtlNodeBaseModel GetNode(string nodeName)
        {

            foreach (CtlNodeBaseModel node in nodeList)
            {
                if (node.NodeName == nodeName)
                {
                    return node;
                }
            }
            return null;
        }
        /// <summary>
        /// 线程分配
        /// </summary>
        /// <param name="reStr"></param>
        /// <returns></returns>
        public List<ThreadBaseModel> GetThreadAlloc(ref string reStr)
        {
            return threadList;
        }

        /// <summary>
        /// 条码设备-控制节点分配
        /// </summary>
        /// <param name="reStr"></param>
        /// <returns></returns>
        public List<IBarcodeRW> GetBarcodeDevAlloc(ref string reStr)
        {
            return null;
        }
        /// <summary>
        /// rfid-控制节点分配
        /// </summary>
        /// <param name="reStr"></param>
        /// <returns></returns>
        public List<IrfidRW> GetRfidDevAlloc(ref string reStr)
        {
            return null;
        }

        ///// <summary>
        ///// plc读写接口-控制节点分配
        ///// </summary>
        ///// <param name="reStr"></param>
        ///// <returns></returns>
        //public static List<IPlcRW> PlcDevAlloc(ref string reStr)
        //{
        //    return null;
        //}

        /// <summary>
        /// MES访问接口-控制节点分配
        /// </summary>
        /// <param name="reStr"></param>
        /// <returns></returns>
        public List<IMesAccess> GetMesAccessAlloc(ref string reStr)
        {
            return null;
        }
        #endregion
        #region 私有方法
        private bool ParseCtlnodes(XElement CtlnodeRoot, ref string reStr)
        {
            if (CtlnodeRoot == null)
            {
                reStr = "系统配置文件错误，不存在CtlNodes节点";
                return false;
            }
            try
            {
               IEnumerable<XElement> nodeXEList =
               from el in CtlnodeRoot.Elements()
               where el.Name == "Node"
               select el;
                foreach (XElement el in nodeXEList)
                {
                    string className = (string)el.Attribute("className");
                    CtlNodeBaseModel ctlNode = null;
                    switch (className)
                    {
                        case "LineNodes.ProductInput":
                            {
                                ctlNode = new NodeProductInput();
                                break;
                            }
                        case "LineNodes.NodeCheckGongneng":
                            {
                                ctlNode = new NodeCheckGongneng();
                                break;
                            }
                        case "LineNodes.NodeCheckXingneng":
                            {
                                ctlNode = new NodeCheckXingneng();
                                break;
                            }
                        case "LineNodes.NodeCheckAngui":
                            {
                                ctlNode = new NodeCheckAngui();
                                break;
                            }
                     
                        case "LineNodes.NodeFaceCheck":
                            {
                                ctlNode = new NodeFaceCheck();
                                break;
                            }
                        case "LineNodes.NodeRepairSwitch":
                            {
                                ctlNode = new NodeRepairSwitch();
                                break;
                            }
                        case "LineNodes.NodeShenhe":
                            {
                                ctlNode = new NodeShenhe();
                                break;
                            }
                        case "LineNodes.NodePack":
                            {
                                ctlNode = new NodePack();
                                break;
                            }
                        case "LineNodes.NodeRobotPallet":
                            {
                                ctlNode = new NodeRobotPallet();
                                break;
                            }
                        default:
                            break;
                    }
                    if (ctlNode != null)
                    {
                        if (!ctlNode.BuildCfg(el, ref reStr))
                        {
                            return false;
                        }
                        //Console.WriteLine(ctlNode.NodeName + ",ID:" + ctlNode.NodeID + "创建成功！");
                        this.nodeList.Add(ctlNode);
                    }

                }
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return false;
            }
           
            return true;
        }
        private bool ParseTheadNodes(XElement ThreadnodeRoot,ref string reStr)
        {
            if(ThreadnodeRoot == null)
            {
                reStr = "系统配置文件错误，不存在ThreadAlloc节点";
                return false;
            }
            try
            {
                IEnumerable<XElement> nodeXEList =
                from el in ThreadnodeRoot.Elements()
                where el.Name == "Thread"
                select el;
                foreach (XElement el in nodeXEList)
                {
                    string threadName = el.Attribute("name").Value;
                    int threadID = int.Parse(el.Attribute("id").Value);
                    int loopInterval = int.Parse(el.Attribute("loopInterval").Value);
                    ThreadRunModel threadObj = new ThreadRunModel(threadID,threadName);
                  //  ThreadBaseModel threadObj = new ThreadBaseModel(threadID, threadName);
                    threadObj.TaskInit(ref reStr);
                    threadObj.LoopInterval = loopInterval;
                    XElement nodeContainer = el.Element("NodeContainer");

                    IEnumerable<XElement> nodeListAlloc =
                    from nodeEL in nodeContainer.Elements()
                    where nodeEL.Name == "NodeID"
                    select nodeEL;
                    foreach(XElement nodeEL in nodeListAlloc)
                    {
                        string nodeID = nodeEL.Value;
                        CtlNodeBaseModel node = FindNode(nodeID);
                        //threadObj.SetThreadRoutine(node.NodeLoop);
                       // break;
                        if(node != null)
                        {
                            threadObj.AddNode(node);
                        }
                    }
                    this.threadList.Add(threadObj);
                }
                return true;
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return false;
            }
        }
        private bool ParseCommDevCfg(XElement commDevRoot, ref string reStr)
        {
            try
            {
                //1 PLC
                XElement plcXE = commDevRoot.Element("PLCCfg");
                string addr = plcXE.Element("PLCAddr").Value.ToString();
                plcRWList = new List<IPlcRW>();
                if (plcXE.Element("PLCVendor").Value.ToString() == "三菱Q")
                {
                    PLCRwMCPro plcObj = new PLCRwMCPro(EnumPlcCata.Qn, 600, 600);
                    plcObj.NetProto = EnumNetProto.UDP;
                    plcObj.PlcID = 1;
                    plcObj.ConnStr = addr;
                    plcObj.StationNumber = 1;
                    plcRWList.Add(plcObj);

                }
                else
                {
                    reStr = "不可识别的PLC型号";
                    return false;
                }

                //2 rfid
                XElement rfidRootXE = commDevRoot.Element("SgRfidCfg");
                IEnumerable<XElement> rfidXES = rfidRootXE.Elements("RFID");
                rfidList = new List<IrfidRW>();
                foreach(XElement rfidXE in rfidXES)
                {
                    byte id = byte.Parse(rfidXE.Attribute("id").Value.ToString());
                    string commPort = rfidXE.Attribute("CommAddr").Value.ToString();
                    SygoleHFReaderIF.HFReaderIF readerIF = new SygoleHFReaderIF.HFReaderIF();
                    SgrfidRW rfidRW = new SgrfidRW(id);
                    rfidRW.ReaderIF = readerIF;
                    rfidRW.ReaderIF.ComPort = commPort;
                    rfidList.Add(rfidRW);
                }

                //3 条码枪
                XElement barcoderRootXE = commDevRoot.Element("BarScannerCfg");
                IEnumerable<XElement> barcodes = barcoderRootXE.Elements("BarScanner");
                foreach(XElement barcodeXE in barcodes)
                {
                    byte id = byte.Parse(barcodeXE.Attribute("id").Value.ToString());
                    string commPort = barcodeXE.Attribute("CommAddr").Value.ToString();
                    BarcodeRWHonevor barcodeReader = new BarcodeRWHonevor(id);
                    SerialPort comPort = new SerialPort(commPort);
                    comPort.BaudRate = 115200;
                    comPort.DataBits = 8;
                    comPort.StopBits = StopBits.One;
                    comPort.Parity = Parity.None;
                    barcodeReader.ComPortObj = comPort;
                    barcodeRWList.Add(barcodeReader);
                }

                //4 气密仪
                XElement airdetectRootXE = commDevRoot.Element("AircheckMachineCfg");
                IEnumerable<XElement> airdetects = airdetectRootXE.Elements("AircheckMachine");
                airDetectList = new List<IAirlossDetectDev>();
                foreach (XElement airdetectXE in airdetects)
                {
                    byte id = byte.Parse(airdetectXE.Attribute("id").Value.ToString());
                    string commPort = airdetectXE.Attribute("CommAddr").Value.ToString();
                    AirDetectFL295CRW airdetectRW = new AirDetectFL295CRW(id,commPort);
                    airDetectList.Add(airdetectRW); 
                }
                //5 打标机
                XElement printerRootXE = commDevRoot.Element("LabelPrinterCfg");
                IEnumerable<XElement> printers = printerRootXE.Elements("LabelPrinter");
                this.printerList = new List<IPrinterInfoDev>();
                foreach(XElement printerXE in printers)
                {
                    byte id = byte.Parse(printerXE.Attribute("id").Value.ToString());
                    string ip = printerXE.Attribute("ip").Value.ToString();
                    short port = short.Parse( printerXE.Attribute("port").Value.ToString());
                    PrinterRW printerRW = new PrinterRW(id,ip,port);
                    string printerDBStr = string.Format("Data Source ={0};Initial Catalog=yibin;User ID=sa;Password=159753;", ip);
                    PrinterRWdb printerRWDb = new PrinterRWdb(printerDBStr);
                    this.printerList.Add(printerRWDb);
                    //this.printerList.Add(printerRW);
                }
                return true;
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return false;
            }
           
        }
        private CtlNodeBaseModel FindNode(string nodeID)
        {
            CtlNodeBaseModel nodeFnd = null;
            if(this.nodeList == null)
            {
                return null;
            }
            foreach(CtlNodeBaseModel node in this.nodeList)
            {
                if(node != null && node.NodeID == nodeID)
                {
                    nodeFnd = node;
                }
            }
            return nodeFnd;
        }
        
        #endregion
    }
}
