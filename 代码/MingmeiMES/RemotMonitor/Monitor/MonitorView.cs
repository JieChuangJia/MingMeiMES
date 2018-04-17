using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LogInterface;
using ModuleCrossPnP;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Configuration;
//using LogInterface;

namespace RemotMonitor
{
    
 //  public delegate void DelgateCtlEnable(bool enabled);
    public partial class MonitorView : BaseChildView
    {
        MonitorSVC.INodeMonitorSvc nodeMonitorSvc = null;
        private ViewThemColor themColor = new ViewThemColor();
        private Dictionary<string, Color> nodeColorMap;
        private Dictionary<string,UserControlCtlNode> userCtlNodes = null;
    
        #region 公共接口
      
        public MonitorView(string captionText):base(captionText)
        {
            
            InitializeComponent();
            this.Text = captionText;
            string ipAddr = ConfigurationManager.AppSettings["svcAddr"];
           
            string svcAddr = string.Format(@"http://{0}:8733/ZZ/LineNodes/NodeMonitorSvc/",ipAddr);
            nodeMonitorSvc = ChannelFactory<MonitorSVC.INodeMonitorSvc>.CreateChannel(new BasicHttpBinding(), new EndpointAddress(svcAddr)); ;//new AsrsSvc.AsrsPowerSvcClient();
           
        }

        public  void Init()
        {

            InitNodeMonitorview();
          
            //配色
            this.flowLayoutPicSample.BackColor = themColor.picSampleColor;
           
        }
        #endregion
        #region IModuleAttach接口实现
        public override void SetLoginterface(ILogRecorder logRecorder)
        {
            this.logRecorder = logRecorder;
            
        }
        #endregion
        #region ILineMonitorView接口实现
        
        public void PopupMes(string strMes)
        {
            MessageBox.Show(strMes);
        }
        public void InitNodeMonitorview()
        {
            string[] nodeList = new string[]{"产品上线","气密检查1","气密检查2","气密检查3","零秒点火","一次试火:1",
        "一次试火:2","一次试火:3","一次试火:4","一次试火:5","一次试火:6","外观检查","装箱核对","机器人码垛"};
            userCtlNodes = new Dictionary<string,UserControlCtlNode>();
            
            this.flowLayoutPanel1.Controls.Clear();
            int nodeCount= nodeList.Count();
            Size boxSize = new Size(0,0);
            boxSize.Width = this.flowLayoutPanel1.Width/(nodeCount+1);
            boxSize.Height =(int)(this.flowLayoutPanel1.Height);
            //foreach(CtlNodeBaseModel node in nodeList)
            for (int i = 0; i < nodeList.Count();i++ )
            {
              //  CtlNodeBaseModel node = nodeList[i];
                UserControlCtlNode monitorNode = new UserControlCtlNode();
                monitorNode.Size = boxSize;
                //监控控件属性赋值
                monitorNode.Title = nodeList[i];
                monitorNode.TimerInfo = "40";

                monitorNode.RefreshDisp();
                this.flowLayoutPanel1.Controls.Add(monitorNode);
                userCtlNodes[nodeList[i]] = monitorNode;

            }
           //配色方案

            //设备状态图例颜色
            nodeColorMap = new Dictionary<string, Color>();
            nodeColorMap["设备故障"] = Color.Red;
            nodeColorMap["设备空闲"] = Color.Green;
            nodeColorMap["设备使用中"] = Color.Yellow;
            nodeColorMap["工位有板"] = Color.LightSeaGreen;
            nodeColorMap["无法识别"] = Color.PaleVioletRed;
            this.pictureBox1.BackColor = nodeColorMap["设备故障"];
            this.pictureBox2.BackColor = nodeColorMap["设备空闲"];
            this.pictureBox3.BackColor = nodeColorMap["设备使用中"];
            this.pictureBox4.BackColor = nodeColorMap["工位有板"];
            this.pictureBox5.BackColor = nodeColorMap["无法识别"];
        }
        public  System.Windows.Forms.Control GetHostControl()
        {
            return this;
        }
        public void RefreshNodeStatus()
        {
            try
            {
                MonitorSVC.MonitorSvcNodeStatus[] nodeStatusArray = nodeMonitorSvc.GetNodeStatus();
                //foreach (MonitorSVC.CtlNodeStatus s in nodeStatusArray)
                //{
                //    Console.WriteLine(s.NodeName + "," + s.Status.ToString());
                //}
                this.label12.Text = "当前正在检测的设备数量:" + nodeMonitorSvc.GetRunningDetectdevs().ToString();
                for(int i=0;i<nodeStatusArray.Count();i++)
                {
                    MonitorSVC.MonitorSvcNodeStatus nodeStatus = nodeStatusArray[i];
                    UserControlCtlNode monitorNode = userCtlNodes[nodeStatus.NodeName];
                    monitorNode.BkgColor = nodeColorMap[nodeStatus.Status];
                    monitorNode.DispDetail = nodeStatus.StatDescribe;
                    monitorNode.RefreshDisp();
                }

                string barcode = nodeStatusArray[0].ProductBarcode;
                if (string.IsNullOrWhiteSpace(barcode))
                {
                    this.label3.Text = "";
                    this.label4.Text = "";
                    return;
                }
                this.label3.Text = barcode.ToUpper();
                int L = barcode.IndexOf("L");
                if (L < 0 || L > barcode.Count())
                {
                    L = barcode.Count();
                }
                string productCata = barcode.Substring(0, L);

                this.label4.Text = string.Format("{0},{1}", productCata, nodeStatusArray[0].ProductName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }
      
        #endregion
        #region UI事件

        private void timerNodeStatus_Tick(object sender, EventArgs e)
        {
            RefreshNodeStatus();

          
        }

        private void NodeMonitorView_SizeChanged(object sender, EventArgs e)
        {
            if(this.userCtlNodes != null)
            {
                Size monitorBoxSize = new Size(this.flowLayoutPanel1.Width / (this.userCtlNodes.Count() + 1), (int)(this.flowLayoutPanel1.Height * 0.8));
                foreach (string nodeKey in this.userCtlNodes.Keys)
                {
                    UserControl monitorNode = this.userCtlNodes[nodeKey];
                    monitorNode.Size = monitorBoxSize;
                }
            }
          
        }
        private void NodeMonitorView_Load(object sender, EventArgs e)
        {
            Init();
            timerNodeStatus.Start();
        }
      
        public bool OnExit()
        {
          
            return true;
        }

        private void NodeMonitorView_FormClosed(object sender, FormClosedEventArgs e)
        {
            timerNodeStatus.Stop();
        }
        #endregion
       


    }
}
