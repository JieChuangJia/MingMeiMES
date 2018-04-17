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
using FTDataAccess.BLL;
using FTDataAccess.Model;
namespace LineNodes
{
    /// <summary>
    /// 生产线监控
    /// </summary>
    public class LineMonitorPresenter2: ILogRequired
    {
        #region 数据
        private const int rfidCount = 8;
        private const int barReaderCount = 2;
        public static bool MesOnline = true;//MES网络是否处于连接状态
        public static bool checkPreStation = true;//是否检测前面工序
      //  private bool mainThreadInitFlag = false;
      //  public const string mesLineID = "L10";
        public static string moduleName = "线体控制";
        private string objectName = "线体控制";
        public static bool DebugMode = true;
        protected ILogRecorder logRecorder = null;
        private ILineMonitorView view;
        private List<ThreadBaseModel> threadList = null;
        private List<CtlNodeBaseModel> nodeList = null;
        private List<CtlNodeStatus> nodeStatusList = null;
        private List<MonitorSvcNodeStatus> svcNodeStatusList = null;
        private List<IPlcRW> plcRWs = null; //plc读写对象列表
        private List<IrfidRW> rfidRWs = null;//rfid读写对象列表
        private List<IBarcodeRW> barcodeRWs = null;
        private List<IPrinterInfoDev> printerRWs = null;
        private List<AinuoAccess> ainuoRWs = null; //艾诺，检测1,3工位
        private List<AipuAccess> aipuRWs = null; //艾普，检测2工位

   //     private 
        private ThreadBaseModel mainThread = null;
        private ThreadBaseModel historyDataClearThread = null; //历史数据清理线程，最多保持15天记录
        private ThreadBaseModel printerLoopThread = null; //外观检测工位，贴标队列处理线程
        private MesDA mesDA = null;
        private LOCAL_MES_STEP_INFOBll localMesBasebll = null;
        private LOCAL_MES_STEP_INFO_DETAILBll localMesDetailbll = null;
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
        public LineMonitorPresenter2(ILineMonitorView view)
        {
            this.view = view;
            mesDA = new MesDA();
            logBll = new SysLogBll();
            produceRecordBll = new ProduceRecordBll();
        }
        #region 公开接口
        //public bool Init(ref string reStr)
        //{
        //    try
        //    {
               
            

               
        //      //  this.threadList = new List<ThreadRunModel>();

               

                
        //        //  Console.WriteLine("P3");
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        view.PopupMes(ex.ToString());
        //        return false;
              
        //    }
           
        //}
        public bool ProcessInit(ref string reStr)
        {
            try
            {
                localMesBasebll = new LOCAL_MES_STEP_INFOBll();
                localMesDetailbll = new LOCAL_MES_STEP_INFO_DETAILBll();
                productCfgBll = new ProductSizeCfgBll();

                nodeList = new List<CtlNodeBaseModel>();
                threadList = new List<ThreadBaseModel>();
                this.rfidRWs = new List<IrfidRW>();
                this.barcodeRWs = new List<IBarcodeRW>();
                this.printerRWs = new List<IPrinterInfoDev>();
                this.plcRWs = new List<IPlcRW>();
                IPlcRW plcRW = null;
                IPrinterInfoDev prienterRW = null;
                string xmlCfgFile = System.AppDomain.CurrentDomain.BaseDirectory + @"data/DevConfigFTYJ.xml";
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
                if (SysCfgModel.SimMode)
                {
                    plcRW = new PlcRWSim();
                    plcRW.PlcID = 1;
                    this.plcRWs.Add(plcRW);
                    prienterRW = new PrinterRWSim(1);
                    this.printerRWs.Add(prienterRW);
                    for (int i = 0; i < rfidCount; i++)
                    {
                        int rfidID =  i + 1;
                        IrfidRW rfidRW = new rfidRWSim();
                        rfidRW.ReaderID = (byte)rfidID;
                        this.rfidRWs.Add(rfidRW);

                    }
                    for (int i = 0; i < barReaderCount; i++)
                    {
                        int barcodeID = i + 1;
                        IBarcodeRW barscanner = new BarcodeRWSim(barcodeID);
                        this.barcodeRWs.Add(barscanner);
                    }
                    this.ainuoRWs = new List<AinuoAccess>();
                    this.aipuRWs = new List<AipuAccess>();
                    AinuoAccess ainuo = new AinuoAccess(1,1,"COM1");
                    this.ainuoRWs.Add(ainuo);
                    ainuo = new AinuoAccess(3,2,"COM3");
                    this.ainuoRWs.Add(ainuo);
                    AipuAccess aipu = new AipuAccess(2, "COM2");
                    this.aipuRWs.Add(aipu);
                }
                else
                {
                    XElement commDevXERoot = root.Element("CommDevCfg");
                    if (!ParseCommDevCfg(commDevXERoot, ref reStr))
                    {
                        return false;
                    }
                    plcRW = plcRWs[0];
                }

                //2 解析结点信息
                XElement CtlnodeRoot = root.Element("CtlNodes");
                if (!ParseCtlnodes(CtlnodeRoot, ref reStr))
                {
                    return false;
                }
                string[] checkNodeIDs = new string[] { "2001", "2002", "2003", "3001" };
                List<string> checkNodeList = new List<string>();
                for (int i = 0; i < checkNodeIDs.Count();i++ )
                {
                    checkNodeList.Add(GetNodeByID(checkNodeIDs[i]).NodeName);

                }
                SysCfgModel.checkStations = checkNodeList.ToArray();

                //3 给节点分配设备读写接口对象

                //for (int i = 0; i < nodeList.Count(); i++)
                //{
                //    CtlNodeBaseModel node = nodeList[i];

                //    node.SimMode = SysCfgModel.SimMode;
                //}

                 prienterRW = this.printerRWs[0];
                (GetNodeByID("8001") as NodePack).PrienterRW = prienterRW;
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
                //if (!(GetNodeByID("1001") as NodeProductInput).DevStatusRestore())
                //{
                //    reStr = "恢复投产位状态失败";
                //    logRecorder.AddLog(new LogModel(objectName, reStr, EnumLoglevel.错误));
                //    return false;
                //}
                foreach(CtlNodeBaseModel node in nodeList)
                {
                    if(!node.DevStatusRestore())
                    {
                        reStr = "恢复投产位状态失败";
                        //logRecorder.AddLog(new LogModel(objectName, reStr, EnumLoglevel.错误));
                        logRecorder.AddDebugLog(node.NodeName, reStr);
                        return false;
                    }
                }
                if (!SysCfgModel.SimMode)
                {
                    CommDevConnect();//通信设备连接
                }

                this.nodeStatusList = new List<CtlNodeStatus>();
                svcNodeStatusList = new List<MonitorSvcNodeStatus>();
                foreach (CtlNodeBaseModel node in this.nodeList)
                {
                    this.nodeStatusList.Add(node.CurrentStat);
                    node.LogRecorder = logRecorder;
                    MonitorSvcNodeStatus svcStat = new MonitorSvcNodeStatus();
                    svcStat.NodeName = node.NodeName;
                    svcStat.ProductBarcode = node.CurrentStat.ProductBarcode;
                    svcStat.StatDescribe = node.CurrentStat.StatDescribe;
                    svcStat.Status = node.CurrentStat.Status.ToString();
                    svcNodeStatusList.Add(svcStat);
                }

                mainThread = new ThreadBaseModel(1, "业务线程");
                mainThread.LoopInterval = 10;
                mainThread.SetThreadRoutine(new DelegateThreadRoutine(BusinessLoop));
                if (!mainThread.TaskInit(ref reStr))
                {
                    logRecorder.AddLog(new LogModel(objectName, reStr, EnumLoglevel.错误));

                    return false;
                }
                historyDataClearThread = new ThreadBaseModel(2, "日志清理线程");
                historyDataClearThread.LoopInterval = 60000;//1分钟清理一次
                historyDataClearThread.SetThreadRoutine(ClearLogLoop);
                if (!historyDataClearThread.TaskInit(ref reStr))
                {
                    logRecorder.AddLog(new LogModel(objectName, reStr, EnumLoglevel.错误));

                }
                printerLoopThread = new ThreadBaseModel(3, "贴标队列处理线程");
                printerLoopThread.LoopInterval = 500;//500
                printerLoopThread.SetThreadRoutine(PrinterQueueLoop);
                printerLoopThread.TaskInit(ref reStr);
                //   Console.WriteLine("P1");

                view.InitNodeMonitorview(this.nodeList);
                //   Console.WriteLine("P2");
                //宿主WCF服务
                Uri _baseAddress = new Uri("http://localhost:8733/ZZ/LineNodes/NodeMonitorSvc/");
                EndpointAddress _Address = new EndpointAddress(_baseAddress);
                BasicHttpBinding _Binding = new BasicHttpBinding();
                ContractDescription _Contract = ContractDescription.GetContract(typeof(LineNodes.INodeMonitorSvc));
                ServiceEndpoint endpoint = new ServiceEndpoint(_Contract, _Binding, _Address);
                NodeMonitorSvc monitorSvc = new NodeMonitorSvc();

                monitorSvc.dlgtNodeStatus += GetSvcNodeStatus;
                monitorSvc.dlgtRunningDev += GetRunningDetectdevs;

                ServiceHost host = new ServiceHost(monitorSvc, _baseAddress);
                //添加终结点ABC
                host.Description.Endpoints.Add(endpoint);
                //启用元数据交换
                ServiceMetadataBehavior meta = new ServiceMetadataBehavior();

                meta.HttpGetEnabled = true;
                host.Description.Behaviors.Add(meta);
                host.Open();

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
        
            string reStr = "";
            if (!StartingCheck(ref reStr))
            {
                if (!RefreshPlNodes(ref reStr))
                {
                    view.PopupMes("启动失败,刷新员工登录信息失败" + reStr);
                    return false;
                }
                if (!StartingCheck(ref reStr))
                {
                    view.PopupMes("启动失败," + reStr);
                    return false;
                }

            }
            this.mainThread.TaskStart(ref reStr);
            this.historyDataClearThread.TaskStart(ref reStr);
            this.printerLoopThread.TaskStart(ref reStr);

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
            this.printerLoopThread.TaskPause(ref reStr);
            //if (!this.mesUploadThread.TaskPause(ref reStr))
            //{
            //    logRecorder.AddLog(new LogModel(this.objectName, reStr, EnumLoglevel.警告));
            //}
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
            this.printerLoopThread.TaskExit(ref reStr);
        }
        public void ClearError()
        {
            foreach(CtlNodeBaseModel node in nodeList)
            {
                node.ClearErrorStat("手动清除错误");
            }
        }
        public List<CtlNodeStatus> GetNodeStatus()
        {
            return nodeStatusList;
        }
        public List<MonitorSvcNodeStatus> GetSvcNodeStatus()
        {
            for (int i = 0; i < svcNodeStatusList.Count();i++ )
            {
                svcNodeStatusList[i].ProductBarcode = nodeStatusList[i].ProductBarcode;
                svcNodeStatusList[i].Status = nodeStatusList[i].Status.ToString();
                svcNodeStatusList[i].StatDescribe = nodeStatusList[i].StatDescribe;
                
            }
            //投产产品名称
            
            string barcode=nodeStatusList[0].ProductBarcode;
            if(string.IsNullOrWhiteSpace(barcode) || barcode.Length<26)
            {
                return svcNodeStatusList;
            }
            int L = barcode.IndexOf("L");
            if(L<0 || L>barcode.Count())
            {
                L = barcode.Count();
            }
            string productCata = barcode.Substring(0, L);
            svcNodeStatusList[0].ProductName = GetProductCfgInfo(productCata);
            return svcNodeStatusList;
        }
        public List<string> GetNodeNames()
        {
            List<string> names = new List<string>();
            foreach(CtlNodeBaseModel node in nodeList)
            {
                names.Add(node.NodeName);
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
        public int GetRunningDetectdevs()
        {
            int counter = 0;
          
            for(int i=0;i<nodeList.Count;i++)
            {
                
                CtlNodeBaseModel node = nodeList[i];
               // if(detectDevSum.Contains(node.NodeName))
                {
                    if(node.CurrentStat.Status != EnumNodeStatus.设备空闲 && (node.CurrentStat.Status != EnumNodeStatus.工位有板))
                    {
                        counter++;
                    }
                }
                    
            }
            return counter;
        }
        public int GetPrintListLen()
        {
            return 0;
            //NodeFaceCheck faceNode = GetNodeByID("6001") as NodeFaceCheck;
            //return faceNode.PrintList.Count();
        }
        public bool NeedSafeClosing()
        {
            if(this.mainThread.IsPause)
            {
                return false;
            }
            if(GetRunningDetectdevs()<1)
            {
                return false;
            }
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
        public AinuoAccess GetAinuoRWByID(int id)
        {
            foreach (AinuoAccess ainuo in ainuoRWs)
            {
                if (ainuo != null && ainuo.ReaderID == id)
                {
                    return ainuo;
                }
            }
            return null;
        }
        public AipuAccess GetAipuRWByID(int id)
        {
            foreach(AipuAccess aipu in aipuRWs)
            {
                if(aipu != null && aipu.ReaderID == id)
                {
                    return aipu;
                }
            }
            return null;
        }
        #endregion
        #region 内部功能
        private CtlNodeBaseModel GetNode(string nodeName)
        {
           
            foreach(CtlNodeBaseModel node in nodeList)
            {
                if(node.NodeName == nodeName)
                {
                    return node;
                }
            }
            return null;
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
                if (!(rfidRWs[i] as SgrfidRW).ReaderIF.OpenComport(ref reStr))
                {
                    logStr = string.Format("{0} 号读卡器打卡端口失败,{1}", i+1, reStr);
                    logRecorder.AddLog(new LogModel(objectName, reStr, EnumLoglevel.错误));
                    view.WelcomeAddStartinfo(logStr);
                    devConnOK = false;
                }
                else
                {
                  //  byte[] rfidRevBuf = null;
                   // if (!rfidRWs[i].Connect(ref rfidRevBuf))
                    if (!rfidRWs[i].Connect())
                    {
                        logStr = string.Format("{0} 号读卡器连接失败", i + 1);
                        logRecorder.AddLog(new LogModel(objectName, logStr, EnumLoglevel.错误));
                        devConnOK = false;
                    }
                    else
                    {
                        logStr = string.Format("{0} 号读卡器连接成功！", i + 1);
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
                    logStr = string.Format("{0} 号条码枪端口打开失败,{1}", barcodeRWs[i].ReaderID,reStr);
                    logRecorder.AddLog(new LogModel(objectName, logStr, EnumLoglevel.错误));
                    devConnOK = false;
                }
                else
                {
                    logStr = string.Format("{0} 号条码枪端口打开成功！", barcodeRWs[i].ReaderID);
                    logRecorder.AddLog(new LogModel(objectName, logStr, EnumLoglevel.提示));
                }
                view.WelcomeAddStartinfo(logStr);
            }

            //艾诺
            for (int i = 0; i < this.ainuoRWs.Count(); i++)
            {
                string logStr = "";
                if(!this.ainuoRWs[i].CommPortOpen(ref reStr))
                {
                    logStr = string.Format("{0} 号艾诺检测端口打开失败,{1}", ainuoRWs[i].ReaderID, reStr);
                    logRecorder.AddLog(new LogModel(objectName, logStr, EnumLoglevel.错误));
                    devConnOK = false;
                }
                else
                {
                    logStr = string.Format("{0}  号艾诺检测端口打开成功", ainuoRWs[i].ReaderID);
                    logRecorder.AddLog(new LogModel(objectName, logStr, EnumLoglevel.提示));
                    this.ainuoRWs[i].StartMonitor(ref reStr);
                   
                }
                view.WelcomeAddStartinfo(logStr);
            }

            //艾普
            for (int i = 0; i < aipuRWs.Count();i++ )
            {
                string logStr = "";
                if(!aipuRWs[i].CommPortOpen(ref reStr))
                {
                    logStr = string.Format("{0} 号艾普检测端口打开失败,{1}", aipuRWs[i].ReaderID, reStr);
                    logRecorder.AddLog(new LogModel(objectName, logStr, EnumLoglevel.错误));
                    devConnOK = false;
                }
                else
                {
                    logStr = string.Format("{0}  号艾普检测端口打开成功", aipuRWs[i].ReaderID);
                    logRecorder.AddLog(new LogModel(objectName, logStr, EnumLoglevel.提示));
                    this.aipuRWs[i].StartMonitor(ref reStr);
                }
                view.WelcomeAddStartinfo(logStr);
            }
            //贴标机
            for (int i = 0; i < printerRWs.Count(); i++)
            {
                string logStr = "";
                if (!printerRWs[i].Connect(ref reStr))
                {
                    logStr = string.Format("{0} 号打标机连接失败,{1}", printerRWs[i].ReaderID, reStr);
                    logRecorder.AddLog(new LogModel(objectName, logStr, EnumLoglevel.错误));

                }
                else
                {
                    logStr = string.Format("{0} 号打标机连接成功", printerRWs[i].ReaderID);
                    logRecorder.AddLog(new LogModel(objectName, logStr, EnumLoglevel.提示));
                }

                view.WelcomeAddStartinfo(logStr);
            }   

            view.WelcomeDispCurinfo("正在连接主控PLC...");
            for (int i = 0; i < plcRWs.Count(); i++)
            {
                string logStr = "";
                if (!plcRWs[i].ConnectPLC(ref reStr))
                {
                    logStr = string.Format("{0} 号PLC连接失败,{1}", plcRWs[i].PlcID, reStr);
                    logRecorder.AddLog(new LogModel(objectName, logStr, EnumLoglevel.错误));
                }
                else
                {
                    logStr = string.Format("{0} 号PLC连接成功", plcRWs[i].PlcID);
                    logRecorder.AddLog(new LogModel(objectName, logStr, EnumLoglevel.提示));
                }
            }
          //  devConnOK = true;//临时
            //if(!devConnOK)
            //{
            //    view.DispCommInfo("通信设备自检故障，可尝试重新自检");
            //}
        }
        //private void MesUploadProc()
        //{
        //    try
        //    {
        //        //按照MES工艺路线要求，又考虑到实际工位布局顺序跟MES工艺路线并不一致，确定如下上传顺序：
        //        //气密2-零秒点火-气密1,3->一次试火->二次试火->外观检测->包装下线
        //        //MES工位顺序：RQ-ZA041->RQ-ZA030->RQ-ZA040->RQ-ZA050,RQ-ZA051,RQ-ZA052,RQ-ZA053->RQ-ZA060->RQ-ZA070->RQ-ZA080
        //        //1 先传RQ-ZA041
        //        string[] mesProcessSeq = new string[] { "RQ-ZA041", "RQ-ZA030", "RQ-ZA040", "RQ-ZA050", "RQ-ZA051", "RQ-ZA052", "RQ-ZA053", "RQ-ZA060", "RQ-ZA070"};
        //        string[] fireTryA = new string[]{ "RQ-ZA050", "RQ-ZA051", "RQ-ZA052", "RQ-ZA053"};
        //        List<string> strConditions = new List<string>();
        //        for(int i=0;i<mesProcessSeq.Count();i++)
        //        {
        //            strConditions.Add(string.Format("UPLOAD_FLAG = 0 and STEP_NUMBER='{0}' order by TRX_TIME asc",mesProcessSeq[i]));
        //        }
        //        for(int i=0;i<strConditions.Count;i++)
        //        {
        //            string strWhere = strConditions[i];
        //            List<LOCAL_MES_STEP_INFOModel> models = localMesBasebll.GetModelList(strWhere);
        //            if(models == null || models.Count()<1)
        //            {
        //                continue;
        //            }
        //            if(0==i)
        //            {
        //                 foreach (LOCAL_MES_STEP_INFOModel m in models)
        //                {
        //                    UploadMesbasicData(m);
        //                }
        //            }
        //            else
        //            {
        //                foreach (LOCAL_MES_STEP_INFOModel m in models)
        //                {
        //                    //检测上道工艺是否上传完成
        //                    string strCheckWhere = "";
        //                    if ("RQ-ZA040" == mesProcessSeq[i]) //气密1,3
        //                    {
        //                        //要求气密1,3都检测完一起上传
        //                        string strCond1 = string.Format("UPLOAD_FLAG=0 and SERIAL_NUMBER='{0}' and AutoStationName='气密检查1'", m.SERIAL_NUMBER);
        //                        string strCond2 = string.Format("UPLOAD_FLAG=0 and SERIAL_NUMBER='{0}' and AutoStationName='气密检查3'", m.SERIAL_NUMBER);
        //                        if((!localMesBasebll.ExistByCondition(strCond1)) || (!localMesBasebll.ExistByCondition(strCond2)))
        //                        {
        //                            continue;
        //                        }
        //                        strCheckWhere = string.Format("UPLOAD_FLAG = 1 and STEP_NUMBER='{0}' and SERIAL_NUMBER='{1}' order by TRX_TIME asc", mesProcessSeq[i - 1], m.SERIAL_NUMBER);
        //                    }
        //                    else if (fireTryA.Contains(mesProcessSeq[i])) //一次试火
        //                    {
        //                        strCheckWhere = string.Format("UPLOAD_FLAG = 1 and STEP_NUMBER='{0}' and SERIAL_NUMBER='{1}' order by TRX_TIME asc", "RQ-ZA040", m.SERIAL_NUMBER);
        //                    }
        //                    else if ("RQ-ZA060"==mesProcessSeq[i]) //二次试火
        //                    {
        //                        strCheckWhere = string.Format("UPLOAD_FLAG = 1 and (STEP_NUMBER='RQ-ZA050' or STEP_NUMBER='RQ-ZA051' or STEP_NUMBER='RQ-ZA052' or STEP_NUMBER='RQ-ZA053') and SERIAL_NUMBER='{0}' order by TRX_TIME asc", m.SERIAL_NUMBER);
        //                    }
        //                    else
        //                    {
        //                         strCheckWhere = string.Format("UPLOAD_FLAG = 1 and STEP_NUMBER='{0}' and SERIAL_NUMBER='{1}' order by TRX_TIME asc", mesProcessSeq[i-1],m.SERIAL_NUMBER);
                               
        //                    }
        //                  //  List<LOCAL_MES_STEP_INFOModel> preuploadModels = localMesBasebll.GetModelList(strCheckWhere);
        //                    if (localMesBasebll.ExistByCondition(strCheckWhere))
        //                    {
        //                        UploadMesbasicData(m);
        //                    }
        //                    else
        //                    {
        //                        continue;
        //                    }
        //                } 
        //            }
                   
        //        }
        //        //logRecorder.AddDebugLog(this.objectName,"MES 数据上传测试!");
        //        for (int i = 0; i < strConditions.Count; i++)
        //        {
        //            string strWhere = strConditions[i];
        //            List<LOCAL_MES_STEP_INFO_DETAILModel> models = localMesDetailbll.GetModelList(strWhere);
        //            if (models == null || models.Count() < 1)
        //            {
        //                continue;
        //            }
        //            if (0==i)
        //            {
        //                foreach (LOCAL_MES_STEP_INFO_DETAILModel m in models)
        //                {
        //                    UploadMesdetailData(m);
        //                }
        //            }
        //            else
        //            {
        //                foreach (LOCAL_MES_STEP_INFO_DETAILModel m in models)
        //                {
        //                    //检测上道工艺是否上传完成
        //                    string strCheckWhere = "";
        //                    if ("RQ-ZA040" == mesProcessSeq[i]) //气密1,3
        //                    {
        //                        //要求气密1,3都检测完一起上传
        //                        string strCond1 = string.Format("UPLOAD_FLAG=0 and SERIAL_NUMBER='{0}' and AutoStationName='气密检查1'", m.SERIAL_NUMBER);
        //                        string strCond2 = string.Format("UPLOAD_FLAG=0 and SERIAL_NUMBER='{0}' and AutoStationName='气密检查3'", m.SERIAL_NUMBER);
        //                        if(!localMesDetailbll.ExistByCondition(strCond1) || (!localMesDetailbll.ExistByCondition(strCond2)))
        //                        {
        //                            continue;
        //                        }
        //                    }
        //                    else if (fireTryA.Contains(mesProcessSeq[i]))
        //                    {
        //                        strCheckWhere = string.Format("UPLOAD_FLAG = 1 and STEP_NUMBER='{0}' and SERIAL_NUMBER='{1}' order by TRX_TIME asc", "RQ-ZA040", m.SERIAL_NUMBER);
        //                    }
        //                    else if (mesProcessSeq[i] == "RQ-ZA060")
        //                    {
        //                        strCheckWhere = string.Format("UPLOAD_FLAG = 1 and (STEP_NUMBER='RQ-ZA050' or STEP_NUMBER='RQ-ZA051' or STEP_NUMBER='RQ-ZA052' or STEP_NUMBER='RQ-ZA053') and SERIAL_NUMBER='{0}' order by TRX_TIME asc", m.SERIAL_NUMBER);
        //                    }
        //                    else
        //                    {
        //                        strCheckWhere = string.Format("UPLOAD_FLAG = 1 and STEP_NUMBER='{0}' and SERIAL_NUMBER='{1}' order by TRX_TIME asc", mesProcessSeq[i - 1], m.SERIAL_NUMBER);

        //                    }
        //                    if (localMesDetailbll.ExistByCondition(strCheckWhere))
        //                    {
        //                        UploadMesdetailData(m);
        //                    }
        //                    else
        //                    {
        //                        continue;
        //                    }
        //                }
        //            } 
        //        }
               
        //    }
        //    catch (Exception ex)
        //    {
        //        logRecorder.AddLog(new LogModel(objectName, ex.ToString(), EnumLoglevel.错误));
                
        //    }
            
        //}
      
        //private void UploadMesbasicData(LOCAL_MES_STEP_INFOModel m)
        //{
        //    if (mesDA.MesBaseExist(m.RECID))
        //    {
        //        m.UPLOAD_FLAG = true;
        //        localMesBasebll.Update(m);
        //        return;
        //    }
        //    FT_MES_STEP_INFOModel ftM = new FT_MES_STEP_INFOModel();

        //    ftM.CHECK_RESULT = m.CHECK_RESULT;
        //    ftM.DEFECT_CODES = m.DEFECT_CODES;
        //    ftM.LAST_MODIFY_TIME = System.DateTime.Now; //m.LAST_MODIFY_TIME;
        //    ftM.REASON = m.REASON;
        //    ftM.RECID = m.RECID;
        //    ftM.SERIAL_NUMBER = m.SERIAL_NUMBER;
        //    ftM.STATUS = m.STATUS;
        //    ftM.STEP_MARK = m.STEP_MARK;
        //    ftM.STEP_NUMBER = m.STEP_NUMBER;
        //    ftM.TRX_TIME = System.DateTime.Now;//m.TRX_TIME;
        //    ftM.USER_NAME = m.USER_NAME;
        //    mesDA.AddMesBaseinfo(ftM);
        //    logRecorder.AddDebugLog(objectName, string.Format("上传基本数据到MES成功,条码：{0},工位：{1}", ftM.SERIAL_NUMBER, ftM.STEP_NUMBER));
        //    m.UPLOAD_FLAG = true;
        //    localMesBasebll.Update(m);
        //}
        private void UploadMesdetailData(LOCAL_MES_STEP_INFO_DETAILModel m)
        {
            if (mesDA.MesDetailExist(m.RECID))
            {
                m.UPLOAD_FLAG = true;
                localMesDetailbll.Update(m);
                return;
            }
            FT_MES_STEP_INFO_DETAILModel ftM = new FT_MES_STEP_INFO_DETAILModel();
            ftM.DATA_NAME = m.DATA_NAME;
            ftM.DATA_VALUE = m.DATA_VALUE;
            ftM.LAST_MODIFY_TIME = System.DateTime.Now; //m.LAST_MODIFY_TIME;
            ftM.RECID = m.RECID;
            ftM.SERIAL_NUMBER = m.SERIAL_NUMBER;
            ftM.STATUS = m.STATUS;
            ftM.STEP_NUMBER = m.STEP_NUMBER;
            ftM.TRX_TIME = System.DateTime.Now;// m.TRX_TIME;
            mesDA.AddMesDetailinfo(ftM);
            logRecorder.AddDebugLog(objectName, string.Format("上传详细数据到MES成功,条码：{0},工位：{1}", ftM.SERIAL_NUMBER, ftM.STEP_NUMBER));
            m.UPLOAD_FLAG = true;
            localMesDetailbll.Update(m);
        }
        private void CallbackDevConnFinished(IAsyncResult ar)
        {
            view.WelcomeClose();
        }
        private void BusinessLoop()//List<CtlNodeBaseModel> myNodeList
        {
            try
            {
                #region 员工登录信息检查,若未按照规定的时间更新，则禁止启动
                string reStr = "";
                /*
                if(!StartingCheck(ref reStr))
                {
               
                    view.Stop();
                    view.PopupMes("启动失败," + reStr);
                    return;
                }*/

                #endregion

                DateTime commSt = System.DateTime.Now;
                TimeSpan ts = commSt - this.lastStTime;
                string dispCommInfo = string.Format("PLC通信周期:{0}毫秒", (int)ts.TotalMilliseconds);
                view.DispCommInfo(dispCommInfo);
                if (ts.TotalMilliseconds > 500)
                {
                    logRecorder.AddDebugLog(objectName, dispCommInfo);
                }
                lastStTime = commSt;
                if (!SysCfgModel.SimMode)
                {


                    PLCRwMCPro plcRW = plcRWs[0] as PLCRwMCPro;
                    string heartAddr = "D2700";
                    if (!plcRW.WriteDB(heartAddr, 1))
                    {
                        Console.WriteLine("PLC通信失败!");

                    }
                    if (SysCfgModel.PlcCommSynMode)
                    {
                        return;
                    }
                 
                   
                    #region 读DB2
                    short[] tempDb2Vals = new short[SysCfgModel.DB2Len];
                    if (!plcRW.ReadMultiDB(SysCfgModel.DB2Start, SysCfgModel.DB2Len, ref tempDb2Vals))
                    {

                        // logRecorder.AddLog(new LogModel(objectName, "PLC通信失败!", EnumLoglevel.错误));
                        Console.WriteLine("PLC通信失败!");

                        plcRW.CloseConnect();
                        if (!plcRW.ConnectPLC(ref reStr))
                        {
                            // logRecorder.AddLog(new LogModel(objectName, "PLC重新连接失败!", EnumLoglevel.错误));
                            Console.WriteLine("PLC重新连接失败!");
                            foreach (CtlNodeBaseModel node in nodeList)
                            {
                                node.CurrentStat.Status = EnumNodeStatus.设备故障;
                                node.CurrentStat.StatDescribe = "PLC通信断开";
                            }
                            return;
                        }
                        else
                        {
                            logRecorder.AddLog(new LogModel(objectName, "PLC重新连接成功!", EnumLoglevel.错误));
                            foreach (CtlNodeBaseModel node in nodeList)
                            {
                                if (node.CurrentStat.Status== EnumNodeStatus.设备故障)
                                {
                                    node.CurrentStat.Status = EnumNodeStatus.设备空闲;
                                    node.CurrentStat.StatDescribe = "设备空闲";
                                }
                               
                            }
                            return;
                        }

                    }
                    plcRW.DB2Switch(tempDb2Vals);
                    #endregion

                    #region 写DB1
                    short[] tempDB1ValsSnd = new short[SysCfgModel.DB1Len];

                    plcRW.DB1Switch(ref tempDB1ValsSnd);
                    if (!plcRW.WriteMultiDB(SysCfgModel.DB1Start, SysCfgModel.DB1Len, tempDB1ValsSnd))
                    {

                        //logRecorder.AddLog(new LogModel(objectName, "PLC通信失败!", EnumLoglevel.错误));
                        Console.WriteLine("PLC重新连接失败!");

                        plcRW.CloseConnect();
                        if (!plcRW.ConnectPLC(ref reStr))
                        {
                            //logRecorder.AddLog(new LogModel(objectName, "PLC重新连接失败!", EnumLoglevel.错误));
                            Console.WriteLine("PLC重新连接失败!");
                            return;
                        }
                        else
                        {
                            logRecorder.AddLog(new LogModel(objectName, "PLC重新连接成功!", EnumLoglevel.错误));
                            return;
                        }

                    }
                    plcRW.PlcRWStatUpdate();
                    #endregion

                    return;
                }   
            }
            catch (Exception ex)
            {
                
               Console.WriteLine(ex.ToString());
            }
           
        }
        private void PrinterQueueLoop()
        {
            try
            {
                NodePack packNode = (NodePack)GetNodeByID("8001");
                if (packNode != null)
                {
                    packNode.PrinterListProcess();
                }
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
                produceRecordBll.ClearHistorydata();
                localMesBasebll.ClearHistorydata();
                localMesDetailbll.ClearHistorydata();
            }
            catch (Exception ex)
            {
                logRecorder.AddDebugLog(objectName, ex.ToString());
               
            }
           

        }

    
        private bool ParseCtlnodes(XElement CtlnodeRoot, ref string reStr)
        {
            if (CtlnodeRoot == null)
            {
                reStr = "系统配置文件错误，不存在CtlNodes节点";
                return false;
            }
            try
            {
                PLNodesBll plNodeBll = new PLNodesBll();
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
                        //case "LineNodes.NodeCheckXingneng":
                        //    {
                        //        ctlNode = new NodeCheckXingneng();
                        //        break;
                        //    }
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
                        ctlNode.PlcRW = GetPlcByID(ctlNode.PlcID); // this.plcRWs[2];
                        if (ctlNode.RfidID > 0)
                        {
                            ctlNode.RfidRW = GetRfidByID(ctlNode.RfidID);
                        }
                       // if (!SysCfgModel.SimMode)
                       // {
                            ctlNode.RfidRW = GetRfidByID((byte)ctlNode.RfidID);
                            ctlNode.BarcodeRW = GetBarcoderRWByID(ctlNode.BarcodeID);
                      //  }

                        //Console.WriteLine(ctlNode.NodeName + ",ID:" + ctlNode.NodeID + "创建成功！");
                        if (className == "LineNodes.NodeCheckAngui")
                        {
                            NodeCheckAngui anguiNode = ctlNode as NodeCheckAngui;
                            anguiNode.AinuoObj = GetAinuoRWByID(anguiNode.ainuoMachineID);
                            
                        }
                        if (className == "LineNodes.NodeCheckGongneng")
                        {
                            NodeCheckGongneng gongNengNode = ctlNode as NodeCheckGongneng;
                            gongNengNode.AipuObj = GetAipuRWByID(gongNengNode.aipuMachineID);
                        }   
                        ctlNode.plNodeModel = plNodeBll.GetModel(ctlNode.NodeID);
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
                XElement plcXE = commDevRoot.Element("PLCCfg");
                string addr = plcXE.Element("PLCAddr").Value.ToString();
                this.plcRWs = new List<IPlcRW>();
                if (plcXE.Element("PLCVendor").Value.ToString() == "三菱Q")
                {
                    PLCRwMCPro plcObj = new PLCRwMCPro(EnumPlcCata.Qn,SysCfgModel.DB1Len,SysCfgModel.DB2Len);
                    plcObj.NetProto = EnumNetProto.TCP;// EnumNetProto.UDP;
                    plcObj.PlcID = 1;
                    plcObj.ConnStr = addr;
                    plcObj.StationNumber = 1;
                    this.plcRWs.Add(plcObj);

                }
                else
                {
                    reStr = "不可识别的PLC型号";
                    return false;
                }

                //2 rfid
                XElement rfidRootXE = commDevRoot.Element("SgRfidCfg");
                IEnumerable<XElement> rfidXES = rfidRootXE.Elements("RFID");
                this.rfidRWs = new List<IrfidRW>();
                foreach (XElement rfidXE in rfidXES)
                {
                    byte id = byte.Parse(rfidXE.Attribute("id").Value.ToString());
                    string commPort = rfidXE.Attribute("CommAddr").Value.ToString();
                    SygoleHFReaderIF.HFReaderIF readerIF = new SygoleHFReaderIF.HFReaderIF();
                    SgrfidRW rfidRW = new SgrfidRW(id);
                   
                    rfidRW.ReaderIF = readerIF;
                    readerIF.recvTimeOut = 800;
                   
                    rfidRW.ReaderIF.ComPort = commPort;
                    this.rfidRWs.Add(rfidRW);
                }

                //3 条码枪
                XElement barcoderRootXE = commDevRoot.Element("BarScannerCfg");
                IEnumerable<XElement> barcodes = barcoderRootXE.Elements("BarScanner");
                foreach (XElement barcodeXE in barcodes)
                {
                    byte id = byte.Parse(barcodeXE.Attribute("id").Value.ToString());
                    string commPort = barcodeXE.Attribute("CommAddr").Value.ToString();
                    BarcodeRWHonevor barcodeReader = new BarcodeRWHonevor(id);
                    if(id== 1)
                    {
                        barcodeReader.TriggerMode = EnumTriggerMode.手动按钮触发;
                    }
                    SerialPort comPort = new SerialPort(commPort);
                    comPort.BaudRate = 115200;
                    comPort.DataBits = 8;
                    comPort.StopBits = StopBits.One;
                    comPort.Parity = Parity.None;
                    barcodeReader.ComPortObj = comPort;
                    this.barcodeRWs.Add(barcodeReader);
                }

               //4 艾诺、艾普检测仪
                XElement checkMachineRootXE = commDevRoot.Element("CheckMachineCfg");
                IEnumerable<XElement> machinePortXES = checkMachineRootXE.Elements("CheckMachine");
                this.ainuoRWs = new List<AinuoAccess>();
                this.aipuRWs = new List<AipuAccess>();
                foreach (XElement xe in machinePortXES)
                {
                    byte id = byte.Parse(xe.Attribute("id").Value.ToString());
                    int devAddr = 1;
                    if(xe.Attribute("devAddr") != null)
                    {
                        devAddr = int.Parse(xe.Attribute("devAddr").Value.ToString());
                    }
                    string commPort = xe.Attribute("CommAddr").Value.ToString();
                    if(xe.Value=="安规" || xe.Value=="性能")
                    {
                        AinuoAccess ainuo = new AinuoAccess(id,devAddr,commPort);
                        this.ainuoRWs.Add(ainuo);
                    }
                    else
                    {
                        AipuAccess aipu = new AipuAccess(id, commPort);
                        this.aipuRWs.Add(aipu);
                    }
                }
                //5 打标机
                XElement printerRootXE = commDevRoot.Element("LabelPrinterCfg");
                IEnumerable<XElement> printers = printerRootXE.Elements("LabelPrinter");
                this.printerRWs= new List<IPrinterInfoDev>();
                foreach (XElement printerXE in printers)
                {
                    byte id = byte.Parse(printerXE.Attribute("id").Value.ToString());
                    string ip = printerXE.Attribute("ip").Value.ToString();
                    string db = printerXE.Attribute("dbName").Value.ToString();
                    string userName = printerXE.Attribute("user").Value.ToString();
                    string pswd = printerXE.Attribute("pswd").Value.ToString();
                    short port = short.Parse(printerXE.Attribute("port").Value.ToString());
                   // PrinterRW printerRW = new PrinterRW(id, ip, port);
                    string printerDBStr = string.Format("Data Source ={0};Initial Catalog={1};User ID={2};Password={3};", ip,db
                        ,userName,pswd);
                    PrinterRWdb printerRWDb = new PrinterRWdb(printerDBStr);
                    this.printerRWs.Add(printerRWDb);
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

        /// <summary>
        /// 启动检查
        /// </summary>
        /// <param name="reStr"></param>
        /// <returns></returns>
        private bool StartingCheck(ref string reStr)
        {
            try
            {
                return true;
               // PLNodesBll plNodesBll = new PLNodesBll();
                string[] loginNodeIDS = new string[] { "2002","3001"};
                string strTime1 = System.DateTime.Now.ToString("d") + " " + SysCfgModel.loginTime1.ToString("T");
                System.DateTime time1 = DateTime.Parse(strTime1);
                string strTime2 = System.DateTime.Now.ToString("d") + " " + SysCfgModel.loginTime1.ToString("T");
                System.DateTime time2 = DateTime.Parse(strTime2);

                for (int i = 0; i < loginNodeIDS.Count();i++ )
                {
                    string nodeID = loginNodeIDS[i];
                    CtlNodeBaseModel ctlNode= GetNodeByID(nodeID);
                    PLNodesModel plNode = ctlNode.plNodeModel;//plNodesBll.GetModel(nodeID);
                    if(plNode==null)
                    {
                        reStr = "启动检查失败,工位配置信息不存在";
                        return false;
                    }
                    ctlNode.WorkerID = plNode.workerID;
                    if(string.IsNullOrWhiteSpace(plNode.workerID))
                    {
                        reStr = string.Format("工位：{0} 操作员工信息未录入", plNode.nodeName);
                        return false;
                    }
                    System.DateTime dtNow = System.DateTime.Now;

                    if(dtNow<time1)
                    {
                        if(plNode.lastLoginTime>=time2.Subtract(new TimeSpan(24,0,0)))
                        {
                            continue;
                        }
                        else
                        {
                            reStr= string.Format("{0} 员工信息未更新，上次更新时间:{1}",plNode.nodeName,plNode.lastLoginTime);

                            return false;
                        }
                    }
                    else if(dtNow>=time1 && dtNow<time2)
                    {
                        if(plNode.lastLoginTime<time1)
                        {
                            reStr = string.Format("{0} 员工信息未更新，上次更新时间:{1}", plNode.nodeName, plNode.lastLoginTime);
                            return false;
                        }
                    }
                    else
                    {
                        if (plNode.lastLoginTime < time2)
                        {
                            reStr = string.Format("{0} 员工信息未更新，上次更新时间:{1}", plNode.nodeName, plNode.lastLoginTime);
                            return false;
                        }
                    }
                    
                }
                return true;
            }
            catch (Exception ex)
            {
                reStr = ex.Message;
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
