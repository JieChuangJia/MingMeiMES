using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.IO.Ports;
using System.Threading;
using System.ServiceModel;
using System.ServiceModel.Description;
using PLProcessModel;
using DevInterface;
using LogInterface;
using DevAccess;
using MesDBAccess.BLL;
using MesDBAccess.Model;
using FTDataAccess.BLL;
using FTDataAccess.Model;
using System.Windows.Forms;
namespace LineNodes
{
    /// <summary>
    /// 生产线监控
    /// </summary>
    public class LineMonitorPresenter: ILogRequired
    {
        #region 数据
        private const int plcCount = 30;
        private const int rfidCount = 50;
        private const int barReaderCount =5;
        public static bool MesOnline = true;//MES网络是否处于连接状态
        public static bool checkPreStation = true;//是否检测前面工序
      
        public static string moduleName = "线体控制";
        private string objectName = "线体控制";
        public static bool DebugMode = true;
        protected ILogRecorder logRecorder = null;
        private ILineMonitorView view;
        private List<ThreadBaseModel> threadList = null;
        private List<CtlNodeBaseModel> nodeList = null;
        private List<CtlNodeStatus> nodeStatusList = null;
        private List<CtlLineBaseModel> lineList = null;
        private List<CtlDevBaseModel> devList = null;
        //private List<MonitorSvcNodeStatus> svcNodeStatusList = null;
        private List<IPlcRW> plcRWs = null; //plc读写对象列表
        private List<IrfidRW> rfidRWs = null;//rfid读写对象列表
        private List<IBarcodeRW> barcodeRWs = null;
        private List<MingmeiDeviceAcc> ccdRWs = null;
        private ThreadBaseModel mainThread = null;
        private ThreadBaseModel historyDataClearThread = null; //历史数据清理线程，最多保持15天记录
        private ThreadBaseModel devWarnMonitorThread = null;
      //  private ThreadBaseModel printerLoopThread = null; //外观检测工位，贴标队列处理线程
      
        private SysLogBll logBll = null;
        private ProduceRecordBll produceRecordBll = null;
        private ProductSizeCfgBll productCfgBll = null;

        private DateTime lastStTime = System.DateTime.Now;
        private bool devConnOK = true;//设备自检通过
        public bool DevConnOK { get { return devConnOK; } }
       // ServiceHost host = null;
        #endregion
        #region 属性
        public ILogRecorder LogRecorder { get { return logRecorder; } set { logRecorder = value; } }
       
        #endregion
        public LineMonitorPresenter(ILineMonitorView view)
        {
            this.view = view;
           // mesDA = new MesDA();
            logBll = new SysLogBll();
            produceRecordBll = new ProduceRecordBll();
        }
        public List<CtlDevBaseModel> GetDevModelList()
        {
            return this.devList;
        }
        public IList<string> GetDevList()
        {
            IList<string> devNameList = new List<string>();
            devNameList.Add("所有");
            foreach (CtlDevBaseModel dev in this.devList)
            {
                devNameList.Add(dev.DevName);
            }
            return devNameList;
        }
        #region 公开接口
        public bool ProcessInit(ref string reStr)
        {
            try
            {
                this.devList = new List<CtlDevBaseModel>();
                this.nodeList = new List<CtlNodeBaseModel>();
                this.nodeStatusList = new List<CtlNodeStatus>();
                threadList = new List<ThreadBaseModel>();
                this.rfidRWs = new List<IrfidRW>();
                this.barcodeRWs = new List<IBarcodeRW>();
                //this.printerRWs = new List<IPrinterInfoDev>();
                this.plcRWs = new List<IPlcRW>();
                this.ccdRWs = new List<MingmeiDeviceAcc>();
             
                string xmlCfgFile = System.AppDomain.CurrentDomain.BaseDirectory + @"data/PLConfig.xml";
                if (!File.Exists(xmlCfgFile))
                {
                    reStr = "系统配置文件：" + xmlCfgFile + " 不存在!";
                    return false;
                }
                if (!PLProcessModel.SysCfgModel.LoadCfg(xmlCfgFile,ref reStr))
                {
                    return false;
                }
                
                XElement root = XElement.Load(xmlCfgFile);
                //1 解析通信设备信息
 
                XElement commDevXERoot = root.Element("CommDevCfg");
                if (!ParseCommDevCfg(commDevXERoot, ref reStr))
                {
                   // return false;
                }
                //2 解析结点信息
                XElement lineRoot = root.Element("LineCfg");
                if(!ParsePLines(lineRoot,ref reStr))
                {
                    return false;
                }
                foreach(CtlLineBaseModel line in lineList)
                {
                    if(!line.AllocCommObj(rfidRWs,barcodeRWs,plcRWs,ccdRWs,ref reStr))
                    {
                        return false;
                    }
                }
                if(lineList != null && lineList.Count()>0)
                {
                     foreach(CtlLineBaseModel line in lineList)
                     {
                         nodeList.AddRange(line.NodeList);
                         devList.AddRange(line.DevList);
                     }

                }
                PLNodesBll plNodeBll = new PLNodesBll();
                foreach (CtlNodeBaseModel node in this.nodeList)
                {
                    this.nodeStatusList.Add(node.CurrentStat);
                    node.LogRecorder = logRecorder;
                    if(!plNodeBll.Exists(node.NodeID))
                    {
                        PLNodesModel plNode = new PLNodesModel();
                        plNode.nodeID = node.NodeID;
                        plNode.nodeName = node.NodeName;
                        plNode.enableRun = true;
                        plNode.checkRequired = true;
                        plNode.tag1 = "";
                        plNode.tag2 = "";
                        plNode.tag3 = "";
                        plNode.tag4 = "";
                        plNode.tag5 = "";
                        plNodeBll.Add(plNode);
                        node.plNodeModel = plNode;
                        
                    }
                    else
                    {
                        PLNodesModel plNode = plNodeBll.GetModel(node.NodeID);
                        node.plNodeModel = plNode;
                    }
  
                }
                
                //3 给节点分配设备读写接口对象

                //for (int i = 0; i < nodeList.Count(); i++)
                //{
                //    CtlNodeBaseModel node = nodeList[i];

                //    node.SimMode = SysCfgModel.SimMode;
                //}

                // prienterRW = this.printerRWs[0];
                //(GetNodeByID("8001") as NodePack).PrienterRW = prienterRW;
                //4 线程-结点分配
                XElement ThreadnodeRoot = root.Element("ThreadAlloc");
                if (!ParseTheadNodes(ThreadnodeRoot, ref reStr))
                {
                    return false;
                }

                foreach (ThreadBaseModel threadObj in this.threadList)
                {
                    threadObj.LogRecorder = logRecorder;
                }

                
                if (!SysCfgModel.SimMode)
                {
                    CommDevConnect();//通信设备连接
                }
            
                foreach (CtlNodeBaseModel node in nodeList)
                {
                    if (!node.DevStatusRestore())
                    {
                        reStr = "恢复投产位状态失败";
                        //logRecorder.AddLog(new LogModel(objectName, reStr, EnumLoglevel.错误));
                        logRecorder.AddDebugLog(node.NodeName, reStr);
                        //return false;
                    }
                }
                
                SetMesconnStat();
               
                mainThread = new ThreadBaseModel(1, "业务线程");
                mainThread.LoopInterval = 100;
                mainThread.SetThreadRoutine(new DelegateThreadRoutine(BusinessLoop));
                if (!mainThread.TaskInit(ref reStr))
                {
                    logRecorder.AddLog(new LogModel(objectName, reStr, EnumLoglevel.错误));

                    return false;
                }
               
                historyDataClearThread = new ThreadBaseModel(2, "日志清理线程");
                historyDataClearThread.LoopInterval = 5000;//5秒清理一次
                historyDataClearThread.SetThreadRoutine(ClearLogLoop);
                if (!historyDataClearThread.TaskInit(ref reStr))
                {
                    logRecorder.AddLog(new LogModel(objectName, reStr, EnumLoglevel.错误));

                }
             
                devWarnMonitorThread = new ThreadBaseModel(3, "设备报警数据采集线程");
                devWarnMonitorThread.LoopInterval = 1000;
                devWarnMonitorThread.SetThreadRoutine(DevMonitorLoop);
                devWarnMonitorThread.TaskInit(ref reStr);
             
                for (int i = 0; i < lineList.Count();i++ )
                {
                    view.InitLineMonitor(i+1, lineList[i]);
                }
               
                return true;
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return false;
            }
        }
        public void SetLogRecorder(ILogRecorder logRecorder)
        {
            this.logRecorder = logRecorder;
            ////throw new NotImplementedException("节点监控模块，日志接口分配函数未实现");
            //foreach(ThreadRunModel threadObj in threadList)
            //{
            //    threadObj.LogRecorder = logRecorder;
            //}
            //foreach(CtlNodeBaseModel ctlNode in nodeList)
            //{
            //    ctlNode.LogRecorder = logRecorder;
            //}
        }
        public bool StartRun()
        {
            //foreach(CtlDevBaseModel dev in devList)
            //{
            //    dev.PlcRW.WriteDB()
            //}
            string reStr = "";
           
            this.mainThread.TaskStart(ref reStr);
            this.historyDataClearThread.TaskStart(ref reStr);
            //this.devWarnMonitorThread.TaskStart(ref reStr);
            lastStTime = System.DateTime.Now;
            Thread.Sleep(200);
          
            foreach (ThreadBaseModel threadObj in this.threadList)
            {

                if (!threadObj.TaskStart(ref reStr))
                {
                    Console.WriteLine(reStr);
                }
             
            }
            return true;
        }
        public bool PauseRun()
        {
            string reStr = "";
            foreach (ThreadBaseModel threadObj in this.threadList)
            {

                if (!threadObj.TaskPause(ref reStr))
                {
                    Console.WriteLine(reStr);
                }
            }
            this.mainThread.TaskPause(ref reStr);
            this.historyDataClearThread.TaskPause(ref reStr);
            this.devWarnMonitorThread.TaskPause(ref reStr);
            return true;
        }
        public void ExitSystem()
        {
            string reStr = "";
            foreach (ThreadRunModel threadObj in this.threadList)
            {
                if (!threadObj.TaskExit(ref reStr))
                {
                    Console.WriteLine(reStr);
                }
            }
          // this.mesUploadThread.TaskExit(ref reStr);
            this.mainThread.TaskExit(ref reStr);
            this.historyDataClearThread.TaskExit(ref reStr);
           // this.devWarnMonitorThread.TaskExit(ref reStr);
          //  this.printerLoopThread.TaskExit(ref reStr);
        }
       
        public List<CtlNodeStatus> GetNodeStatus()
        {
            return nodeStatusList;
        }
       
        public List<string> GetNodeNames()
        {
            List<string> names = new List<string>();
            foreach(CtlLineBaseModel line in lineList)
            {
                if(line == null)
                {
                    continue;
                }
                foreach (CtlNodeBaseModel node in line.NodeList)
                {
                    names.Add(node.NodeName);
                }
            }
            
            return names;
        }
        public bool GetDevRunningInfo(string nodeName, ref DataTable db1Dt, ref DataTable db2Dt,ref string taskDetail)
        {
            CtlNodeBaseModel node = GetNode(nodeName);
            if (node== null)
            {
                return false;
            }
            //任务
            db1Dt = node.GetDB1DataDetail();
            db2Dt = node.GetDB2DataDetail();
            taskDetail=node.GetRunningTaskDetail();
            return true;
        }
        public bool SimSetDB2(string nodeName,int dbItemID,int val)
        {
            CtlNodeBaseModel node = GetNode(nodeName);
            if(node == null)
            {
                Console.WriteLine("工位：" + nodeName + " 不存在");
                return false;
            }
            node.DicCommuDataDB2[dbItemID].Val = val;
            return true;
        }
        public void SimSetRFID(string nodeName,string strUID)
        {
            //if(rfidID<1 || rfidID>rfidRWs.Count())
            //{
            //    Console.WriteLine("RFID ID错误");
            //    return;
            //}
            CtlNodeBaseModel node = GetNode(nodeName);
            if (node == null)
            {
                Console.WriteLine("工位：" + nodeName + " 不存在");
                return;
            }
            node.SimRfidUID = strUID;
        }
        public void SimSetBarcode(string nodeName,string barcode)
        {
            CtlNodeBaseModel node = GetNode(nodeName);
            if (node == null)
            {
                Console.WriteLine("工位：" + nodeName + " 不存在");
                return;
            }
            node.SimBarcode = barcode;
        }
        public void SimSetCheckRe(string nodeName,string re)
        {
            //if (nodeName != "气密检查1" && nodeName != "气密检查2" && nodeName != "气密检查3")
            //{
            //    return;
            //}
            //CtlNodeBaseModel node = GetNode(nodeName);
            //NodeAirlossCheck airlossNode = node as NodeAirlossCheck;
            //(airlossNode.AirDetectRW as AirDetectRWSim).DetectRe = re;
        }
        public void SimSetBarcode(int gunIndex,string barcode)
        {
            (barcodeRWs[gunIndex - 1] as BarcodeRWSim).Barcode = barcode;
        }
        
        public int GetPrintListLen()
        {
            return 0;
            //NodeFaceCheck faceNode = GetNodeByID("6001") as NodeFaceCheck;
            //return faceNode.PrintList.Count();
        }
        public bool NeedSafeClosing()
        {
            //if(this.mainThread.IsPause)
            //{
            //    return false;
            //}
            
            return true;
        }
        public string GetProductCfgInfo(string cataCode)
        {
            string reStr = "";
            ProductSizeCfgModel cfg = productCfgBll.GetModel(cataCode);
            if(cfg == null)
            {
                return reStr;
            }
            reStr = string.Format("{0}", cfg.productName);
            return reStr;
        }

        public IrfidRW GetRfidByID(byte readerID)
        {
            foreach (IrfidRW rfid in rfidRWs)
            {
                if (rfid != null && rfid.ReaderID == readerID)
                {
                    return rfid;
                }
            }
            return null;
        }
        public IPlcRW GetPlcByID(int plcID)
        {
            foreach (IPlcRW plcRW in plcRWs)
            {
                if (plcID == plcRW.PlcID)
                {
                    return plcRW;
                }
            }
            return null;
        }
        //public IrfidRW GetRfidByID(int rfidID)
        //{
        //    foreach (IrfidRW rfidRW in rfidRWs)
        //    {
        //        if (rfidID == rfidRW.ReaderID)
        //        {
        //            return rfidRW;
        //        }
        //    }
        //    return null;
        //}
        public IBarcodeRW GetBarcoderRWByID(int barcodReaderID)
        {
            foreach (IBarcodeRW barcodeReader in this.barcodeRWs)
            {
                if (barcodeReader != null && barcodeReader.ReaderID == barcodReaderID)
                {
                    return barcodeReader;
                }
            }
            return null;
        }
        public void SetMesconnStat()
        {
            try
            {
                foreach (CtlDevBaseModel dev in devList)
                {
                    if (dev.PlcRW != null)
                    {
                        
                        if (dev.PlcRW.WriteDB("W32.0", 1))
                        {
                          
                            if (logRecorder != null)
                            {
                                logRecorder.AddDebugLog(objectName, dev.DevName + "MES连机成功");
                            }
                        }
                        else
                        {
                           
                            if (logRecorder != null)
                            {
                                logRecorder.AddDebugLog(objectName, dev.DevName + "MES连机失败");
                            }
                        }
                    }

                }
            }
            catch(Exception ex)
            {
                logRecorder.AddDebugLog("SetMesconnStat", ex.StackTrace.ToString());
            }
        }
        #endregion
        #region 内部功能
        private CtlNodeBaseModel FindNode(string nodeID)
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
        private CtlNodeBaseModel GetNode(string nodeName)
        {
           foreach(CtlLineBaseModel line in lineList)
           {
               foreach (CtlNodeBaseModel node in line.NodeList)
               {
                   if (node.NodeName == nodeName)
                   {
                       return node;
                   }
               }
           }
           
            return null;
        }
       
        public delegate void DelegateDevConn();
        public void CommDevConnect()
        {
            try
            {
                view.WelcomePopup();
                //DelegateDevConn dlgt = new DelegateDevConn(AsyCommDevConnect);
                //IAsyncResult ar = dlgt.BeginInvoke(CallbackDevConnFinished,dlgt);
                AsyCommDevConnect();
                
                view.WelcomeClose();
                 
            }
            catch (Exception ex)
            {
                view.PopupMes(ex.ToString());
            }
           
        }
        //异步，通信设备连接
        private void AsyCommDevConnect()
        {
            string reStr = "";
            devConnOK = true;
            //通信连接
            for (int i = 0; i < rfidRWs.Count(); i++)
            {
                string logStr = "";
                {
                  //  byte[] rfidRevBuf = null;
                   // if (!rfidRWs[i].Connect(ref rfidRevBuf))
                    if (!rfidRWs[i].Connect())
                    {
                        logStr = string.Format("{0} 号读卡器连接失败", rfidRWs[i].ReaderID);
                        logRecorder.AddLog(new LogModel(objectName, logStr, EnumLoglevel.错误));
                        //devConnOK = false;
                    }
                    else
                    {
                        logStr = string.Format("{0} 号读卡器连接成功！", rfidRWs[i].ReaderID);
                        logRecorder.AddLog(new LogModel(objectName, logStr, EnumLoglevel.提示));
                    }
                    view.WelcomeAddStartinfo(logStr);
                }
            }
            for (int i = 0; i < barcodeRWs.Count(); i++)
            {
                string logStr = "";
                if (!barcodeRWs[i].StartMonitor(ref reStr))
                {
                    logStr = string.Format("{0} 号条码枪连接失败{1}", barcodeRWs[i].ReaderID,reStr);
                    logRecorder.AddLog(new LogModel(objectName, logStr, EnumLoglevel.错误));
                    //devConnOK = false;
                }
                else
                {
                    logStr = string.Format("{0} 号条码枪连接成功！", barcodeRWs[i].ReaderID);
                    logRecorder.AddLog(new LogModel(objectName, logStr, EnumLoglevel.提示));
                }
                view.WelcomeAddStartinfo(logStr);
            }
            foreach(MingmeiDeviceAcc ccdDev in ccdRWs)
            {
                string logStr = "";
                if(!ccdDev.Connect(ref reStr))
                {
                    logStr = string.Format("CCD设备:{0} 连接失败{1}",ccdDev.Role, reStr);
                    logRecorder.AddLog(new LogModel(objectName, logStr, EnumLoglevel.错误));
                }
                else
                {
                    logStr = string.Format("CCD设备:{0} 连接成功", ccdDev.Role);
                    logRecorder.AddLog(new LogModel(objectName, logStr, EnumLoglevel.提示));
                }
            }
          

            view.WelcomeDispCurinfo("正在连接PLC...");
            for (int i = 0; i < plcRWs.Count(); i++)
            {
              
                string logStr = "";
               
                if (!plcRWs[i].ConnectPLC(ref reStr))
                {
                   
                    if(!SysCfgModel.SimMode)
                    {
                        logStr = string.Format("{0} PLC连接失败,{1}", (plcRWs[i] as OmlPlcRW).PlcRole, reStr);
                        logRecorder.AddLog(new LogModel(objectName, logStr, EnumLoglevel.错误));
                    }
                   
                }
                else
                {
                    if (!SysCfgModel.SimMode)
                    {
                        logStr = string.Format("{0} PLC连接成功", (plcRWs[i] as OmlPlcRW).PlcRole);
                        logRecorder.AddLog(new LogModel(objectName, logStr, EnumLoglevel.提示));
                    }
                  
                }
            }
             
        
        }

        private void CallbackDevConnFinished(IAsyncResult ar)
        {
            view.WelcomeClose();
        }
        private void BusinessLoop()//List<CtlNodeBaseModel> myNodeList
        {
            try
            {
                IPlcRW plc1 = GetPlcByID(1);
                IPlcRW plc2 = GetPlcByID(2);
                IPlcRW plc3 = GetPlcByID(3);
                CtlNodeBaseModel node1 = FindNode("OPA004");
                plc1.WriteMultiDB(node1.Db1StartAddr, node1.Db1BlockNum,node1.Db1ValsToSnd);
                CtlNodeBaseModel node2 = FindNode("OPB001");
                plc2.WriteMultiDB(node2.Db1StartAddr, node2.Db1BlockNum, node2.Db1ValsToSnd);
                //CtlNodeBaseModel node3 = FindNode("OPC003");
                //plc3.WriteMultiDB(node3.Db1StartAddr, node3.Db1BlockNum, node3.Db1ValsToSnd);

            }
            catch (Exception ex)
            {
                
               Console.WriteLine(ex.ToString());
            }
           
        }
        
        private void ClearLogLoop()
        {
            try
            {
                logBll.ClearHistorydata();
                produceRecordBll.ClearHistorydata(60);
               // localMesBasebll.ClearHistorydata();
             //   localMesDetailbll.ClearHistorydata();
            }
            catch (Exception ex)
            {
                logRecorder.AddDebugLog(objectName, ex.ToString());
               
            }
           

        }
        private void DevMonitorLoop()
        {
            try
            {
                string reStr = "";
                foreach(CtlDevBaseModel dev in devList)
                {
                    dev.DevWarnMonitor(ref reStr);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private bool ParsePLines(XElement lineRoot,ref string reStr)
        {
            lineList = new List<CtlLineBaseModel>();
            IEnumerable<XElement> lineXEList = lineRoot.Elements("Line");
            foreach (XElement el in lineXEList)
            {
                CtlLineBaseModel line = new CtlLineBaseModel();
                if(!line.ParseCfg(el,ref reStr))
                {
                    return false;
                }
                lineList.Add(line);

            }
            return true;
        }
        //private bool ParseCtlnodes(XElement CtlnodeRoot, ref string reStr)
        //{
        //    if (CtlnodeRoot == null)
        //    {
        //        reStr = "系统配置文件错误，不存在CtlNodes节点";
        //        return false;
        //    }
        //    try
        //    {
        //        PLNodesBll plNodeBll = new PLNodesBll();
        //        IEnumerable<XElement> nodeXEList =
        //        from el in CtlnodeRoot.Elements()
        //        where el.Name == "Node"
        //        select el;
        //        foreach (XElement el in nodeXEList)
        //        {
        //            string className = (string)el.Attribute("className");
        //            CtlNodeBaseModel ctlNode = null;
        //            switch (className)
        //            {
        //                case "LineNodes.ProductInput":
        //                    {
        //                        ctlNode = new NodeProductInput();
        //                        break;
        //                    }
        //                case "LineNodes.NodeCheckGongneng":
        //                    {
        //                        ctlNode = new NodeCheckGongneng();
        //                        break;
        //                    }
        //                //case "LineNodes.NodeCheckXingneng":
        //                //    {
        //                //        ctlNode = new NodeCheckXingneng();
        //                //        break;
        //                //    }
        //                case "LineNodes.NodeCheckAngui":
        //                    {
        //                        ctlNode = new NodeCheckAngui();
        //                        break;
        //                    }

        //                case "LineNodes.NodeFaceCheck":
        //                    {
        //                        ctlNode = new NodeFaceCheck();
        //                        break;
        //                    }
        //                case "LineNodes.NodeRepairSwitch":
        //                    {
        //                        ctlNode = new NodeRepairSwitch();
        //                        break;
        //                    }
        //                case "LineNodes.NodeShenhe":
        //                    {
        //                        ctlNode = new NodeShenhe();
        //                        break;
        //                    }
        //                case "LineNodes.NodePack":
        //                    {
        //                        ctlNode = new NodePack();
        //                        break;
        //                    }
        //                case "LineNodes.NodeRobotPallet":
        //                    {
        //                        ctlNode = new NodeRobotPallet();
        //                        break;
        //                    }
        //                default:
        //                    break;
        //            }
        //            if (ctlNode != null)
        //            {
        //                if (!ctlNode.BuildCfg(el, ref reStr))
        //                {
        //                    return false;
        //                }
        //                //Console.WriteLine(ctlNode.NodeName + ",ID:" + ctlNode.NodeID + "创建成功！");
        //                ctlNode.PlcRW = GetPlcByID(ctlNode.PlcID); // this.plcRWs[2];
        //                if (ctlNode.RfidID > 0)
        //                {
        //                    ctlNode.RfidRW = GetRfidByID(ctlNode.RfidID);
        //                }
        //               // if (!SysCfgModel.SimMode)
        //               // {
        //                    ctlNode.RfidRW = GetRfidByID((byte)ctlNode.RfidID);
        //                    ctlNode.BarcodeRW = GetBarcoderRWByID(ctlNode.BarcodeID);
        //              //  }

        //                //Console.WriteLine(ctlNode.NodeName + ",ID:" + ctlNode.NodeID + "创建成功！");
        //                if (className == "LineNodes.NodeCheckAngui")
        //                {
        //                    NodeCheckAngui anguiNode = ctlNode as NodeCheckAngui;
        //                    anguiNode.AinuoObj = GetAinuoRWByID(anguiNode.ainuoMachineID);
                            
        //                }
        //                if (className == "LineNodes.NodeCheckGongneng")
        //                {
        //                    NodeCheckGongneng gongNengNode = ctlNode as NodeCheckGongneng;
        //                    gongNengNode.AipuObj = GetAipuRWByID(gongNengNode.aipuMachineID);
        //                }   
        //                ctlNode.plNodeModel = plNodeBll.GetModel(ctlNode.NodeID);
        //                this.nodeList.Add(ctlNode);
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        reStr = ex.ToString();
        //        return false;
        //    }

        //    return true;
        //}
        private bool ParseTheadNodes(XElement ThreadnodeRoot, ref string reStr)
        {
            if (ThreadnodeRoot == null)
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
                    ThreadRunModel threadObj = new ThreadRunModel(threadID, threadName);
                    //  ThreadBaseModel threadObj = new ThreadBaseModel(threadID, threadName);
                    threadObj.TaskInit(ref reStr);
                    threadObj.LoopInterval = loopInterval;
                    XElement nodeContainer = el.Element("NodeContainer");

                    IEnumerable<XElement> nodeListAlloc =
                    from nodeEL in nodeContainer.Elements()
                    where nodeEL.Name == "NodeID"
                    select nodeEL;
                    foreach (XElement nodeEL in nodeListAlloc)
                    {
                        string nodeID = nodeEL.Value;
                        CtlNodeBaseModel node = FindNode(nodeID);
                        //threadObj.SetThreadRoutine(node.NodeLoop);
                        // break;
                        if (node != null)
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
                XElement plcRoot = commDevRoot.Element("PLCCfg");

                plcRWs = new List<IPlcRW>();
                IEnumerable<XElement> plcXES = plcRoot.Elements("PLC");
                if (plcRoot.Attribute("Vendor").Value.ToString() == "欧姆龙")
                {
                    string pcIP = "192.168.0.88";
                     foreach (XElement plcXE in plcXES)
                     {
                         string connStr = plcXE.Value.ToString();
                         string[] strArray = connStr.Split(new string[]{":"},StringSplitOptions.RemoveEmptyEntries);
                         string plcIP = strArray[0];
                         int plcPort = int.Parse(strArray[1]);
                         int db1Len = 100, db2Len = 100;
                         db1Len = int.Parse(plcXE.Attribute("db1Len").Value.ToString());
                         db2Len = int.Parse(plcXE.Attribute("db2Len").Value.ToString());
                         int plcID = int.Parse(plcXE.Attribute("id").Value.ToString());
                         OmlPlcRW plcRW = null;
                         if(plcXE.Attribute("netPro").Value.ToString() == "TCP")
                         {
                             plcRW = new OmlPlcRW(EnumPLCType.OML_TCP,plcIP, plcPort,pcIP,null);
                         }
                         else
                         {
                             plcRW = new OmlPlcRW(EnumPLCType.OML_UDP, plcIP, plcPort, pcIP, null);
                         }
                         plcRW.PlcRole = plcXE.Attribute("role").Value.ToString();
                         // PLCRwMCPro2 plcRW = new PLCRwMCPro2(db1Len, db2Len);
                       
                         plcRW.PlcID = plcID;
                         plcRWs.Add(plcRW);
                     }
                    
                }
                else if (plcRoot.Attribute("Vendor").Value.ToString() == "三菱")
                {
                    foreach (XElement plcXE in plcXES)
                    {
                        string connStr = plcXE.Value.ToString();
                        int db1Len = 100, db2Len = 100;
                        db1Len = int.Parse(plcXE.Attribute("db1Len").Value.ToString());
                        db2Len = int.Parse(plcXE.Attribute("db2Len").Value.ToString());
                        int plcID = int.Parse(plcXE.Attribute("id").Value.ToString());
                        EnumPlcCata plcCata = EnumPlcCata.FX5U;
                        if (plcXE.Attribute("cata") != null)
                        {
                            string strPlcCata = plcXE.Attribute("cata").Value.ToString();
                            plcCata = (EnumPlcCata)Enum.Parse(typeof(EnumPlcCata), strPlcCata);

                        }

                        PLCRwMCPro plcRW = new PLCRwMCPro(plcCata, db1Len, db2Len);
                        // PLCRwMCPro2 plcRW = new PLCRwMCPro2(db1Len, db2Len);
                        plcRW.ConnStr = plcXE.Value.ToString();
                        plcRW.PlcID = plcID;
                        plcRWs.Add(plcRW);
                    }
                }
                else
                {
                    reStr = "不识别的PLC厂家";
                    return false;
                }
               
              

                //2 rfid
                XElement rfidRootXE = commDevRoot.Element("RfidCfg");
                IEnumerable<XElement> rfidXES = rfidRootXE.Elements("RFID");
                this.rfidRWs = new List<IrfidRW>();
                foreach (XElement rfidXE in rfidXES)
                {
                    byte id = byte.Parse(rfidXE.Attribute("id").Value.ToString());
                    string addr = rfidXE.Attribute("CommAddr").Value.ToString();
                    string[] strAddrArray = addr.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    string ip = strAddrArray[0];
                    int port = int.Parse(strAddrArray[1]);
                     string rfidType = rfidXE.Value.Trim();
                    IrfidRW rfidRW=null;

                    //if (rfidType == "C线自动打带机")
                    //{
                        rfidRW = new RfidRW_CFRH390_Tcp(id,ip, port,null);
                    //}
                    //else
                    //{
                    //    rfidRW = new RfidCF(id, ip, port);
                    //}
                  
                    this.rfidRWs.Add(rfidRW);
                }

                //3 条码枪
                XElement barcoderRootXE = commDevRoot.Element("BarScannerCfg");
                IEnumerable<XElement> barcodes = barcoderRootXE.Elements("BarScanner");
                foreach (XElement barcodeXE in barcodes)
                {
                    byte id = byte.Parse(barcodeXE.Attribute("id").Value.ToString());
                    string connStr = barcodeXE.Attribute("CommAddr").Value.ToString();
                    string[] strArray = connStr.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    string ip = strArray[0];
                    int port = int.Parse(strArray[1]);
                    BarcodeRWSR_750 barcodeReader = new BarcodeRWSR_750(id, ip, port, null);
                    //BarcodeRWHonevor barcodeReader = new BarcodeRWHonevor(id);
                    //if(id== 1)
                    //{
                    //    barcodeReader.TriggerMode = EnumTriggerMode.手动按钮触发;
                    //}
                    //SerialPort comPort = new SerialPort(commPort);
                    //comPort.BaudRate = 115200;
                    //comPort.DataBits = 8;
                    //comPort.StopBits = StopBits.One;
                    //comPort.Parity = Parity.None;
                    //barcodeReader.ComPortObj = comPort;
                    this.barcodeRWs.Add(barcodeReader);
                }

              
               //4 CCD检测设备
                XElement CCDRoot = commDevRoot.Element("CCDCfg");
                IEnumerable<XElement> ccdXES = CCDRoot.Elements("CCD");
                int pcPort = 8888;
                foreach (XElement ccd in ccdXES)
                {
                    int id = int.Parse(ccd.Attribute("id").Value.ToString());
                    string connStr = ccd.Attribute("CommAddr").Value.ToString();
                    MingmeiDeviceAcc ccdDev = new MingmeiDeviceAcc();
                    ccdDev.LocalPort = pcPort;
                    pcPort++;
                    ccdDev.Role = ccd.Attribute("role").Value.ToString();
                    ccdDev.ConnStr = connStr;
                    ccdDev.DevID = id;
                    ccdRWs.Add(ccdDev);
                }
                return true;
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return false;
            }

        }
     
        private bool RefreshPlNodes(ref string reStr)
        {
            try
            {
                PLNodesBll plNodesBll = new PLNodesBll();
                foreach(CtlNodeBaseModel ctlNode in nodeList)
                {
                    ctlNode.plNodeModel = plNodesBll.GetModel(ctlNode.NodeID);
                }
                return true;
            }
            catch (Exception ex)
            {
                reStr = ex.Message;
                return false;
            }
        }
        #endregion
    }
}
