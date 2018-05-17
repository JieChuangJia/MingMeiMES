using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LogInterface;
using PLProcessModel;
using ModuleCrossPnP;
//using LogInterface;

namespace LineNodes
{
   public delegate void DelgateCtlEnable(bool enabled);

    public partial class NodeMonitorView : BaseChildView,ILineMonitorView
    {
        private delegate void DelegateStop();
        private delegate void DlgtPopupWelcome();
        private WelcomeForm welcomeDlg = new WelcomeForm();
        private ViewThemColor themColor = new ViewThemColor();
        private Dictionary<EnumNodeStatus, Color> nodeColorMap;
    
        private LineMonitorPresenter lineMonitorPresenter = null;
     
        private List<CtlNodeStatus> nodeStatusList = null;
        private Dictionary<string,UserControlCtlNode> userCtlNodes = null;
     //   protected ILogRecorder logRecorder = null;
      
        #region 公共接口
        public List<CtlDevBaseModel> GetDevList()
        {
            return this.LinePresenter.GetDevModelList();
        }
        public LineMonitorPresenter LinePresenter { get { return lineMonitorPresenter; } }
        public NodeMonitorView(string captionText):base(captionText)
        {
            InitializeComponent();
            this.Text = captionText;
            //this.
        }
        public void AbortApp()
        {
            OnStop();
           

        }
        public  void Init()
        {
            //heightDefBll = new ProductHeightDefBll();
            //packsizeDefBll = new ProductPacsizeDefBll();
            //productCfgBll = new ProductSizeCfgBll();
            try
            {
                lineMonitorPresenter = new LineMonitorPresenter(this);
                lineMonitorPresenter.SetLogRecorder(logRecorder);
                userCtlNodes = new Dictionary<string, UserControlCtlNode>();
                nodeStatusList = new List<CtlNodeStatus>();

                // this.captionText = captionText;
                string reStr = "";
                if (!lineMonitorPresenter.ProcessInit(ref reStr))
                {
                    MessageBox.Show(reStr);
                    return;
                }
            

                this.comboBoxDevList.Items.AddRange(lineMonitorPresenter.GetNodeNames().ToArray());
                this.comboBoxDevList.SelectedIndex = 0;


                nodeColorMap = new Dictionary<EnumNodeStatus, Color>();
                nodeColorMap[EnumNodeStatus.设备故障] = Color.Red;
                nodeColorMap[EnumNodeStatus.设备空闲] = Color.FromArgb(34,177,76);
                nodeColorMap[EnumNodeStatus.设备使用中] = Color.Yellow;
              
                nodeColorMap[EnumNodeStatus.无法识别] = Color.PaleVioletRed;
                this.pictureBox1.BackColor = nodeColorMap[EnumNodeStatus.设备故障];
                this.pictureBox2.BackColor = nodeColorMap[EnumNodeStatus.设备空闲];
                this.pictureBox3.BackColor = nodeColorMap[EnumNodeStatus.设备使用中];
            
                this.pictureBox5.BackColor = nodeColorMap[EnumNodeStatus.无法识别];

                //配色
                this.flowLayoutPicSample.BackColor = themColor.picSampleColor;
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
          
        }
        public override List<string> GetLogsrcList()
        {
            List<string> logNodes = lineMonitorPresenter.GetNodeNames();
            return logNodes;
        }
        public List<string> GetNodeNames()
        {
            return lineMonitorPresenter.GetNodeNames();
        }

        public void SetEnabled(bool enabled)
        {
            if (this.tabControl1.InvokeRequired)
            {
                DelgateCtlEnable delgateCtlEnable = new DelgateCtlEnable(SetEnabled);
                this.Invoke(delgateCtlEnable, new object[] { enabled });
            }
            else
            {
                if (enabled)
                {
                   

                }
                else
                {
                    //lineMonitorPresenter.PauseRun();
                    bt_StopSystem_Click(this, null);
                  
                }
            }



        }
        public bool IsDebugMode()
        {
            return LineMonitorPresenter.DebugMode;
        }
        public bool OnStart()
        {
            Console.WriteLine("系统启动");
            if (!lineMonitorPresenter.DevConnOK)
            {
                PopupMes("设备通信有故障，禁止启动");
                return false;
            }
            if (this.lineMonitorPresenter.StartRun())
            {
                this.timerNodeStatus.Start();
               
                return true;
            }
            else
            {
                return false;
            }

        }
        public void OnStop()
        {
            this.timerNodeStatus.Stop();
            this.lineMonitorPresenter.PauseRun();

        }
        public bool OnExit()
        {
            if (0 == PoupAskmes("确定要退出系统？"))
            {
                return false;
            }
            if (lineMonitorPresenter.NeedSafeClosing())
            {
                ClosingWaitDlg dlg = new ClosingWaitDlg();
                if (DialogResult.Cancel == dlg.ShowDialog())
                {
                    return false;
                }
            }
            OnStop();
            this.lineMonitorPresenter.ExitSystem();
            System.Environment.Exit(0);
            return true;
        }
        #endregion
      
        #region IModuleAttach接口实现
        public override void SetLoginterface(ILogRecorder logRecorder)
        {
            this.logRecorder = logRecorder;
            
        }
        #endregion
        #region ILineMonitorView接口实现
        //public void BindNodeStat()
        //{
        //    //this.textBox1.DataBindings.Add("Text", nodeList[0].CurrentStat, "StatDescribe");
        //    //this.label3.DataBindings.Add("Text", nodeStatusList[0], "ProductBarcode", true, DataSourceUpdateMode.OnPropertyChanged);
        //}
        public void Stop()
        {
            OnStop();
        }
        public void PopupMes(string strMes)
        {
            MessageBox.Show(strMes);
        }
        public void InitLineMonitor(int lineSeq, CtlLineBaseModel line)
        {
          
            //this.flowLayoutPanel1.Controls.Clear();
          
            Size boxSize = new Size(0, 0);
            View.UserControlLine userControlLine = null;
            if (lineSeq == 1)
            {
                userControlLine = this.userControlLine1;

            }
            else if (lineSeq == 2)
            {
                userControlLine = this.userControlLine2;

            }
            else
            {
                userControlLine = this.userControlLine3;
            }
            userControlLine.LineName = line.LineName;
            int nodeCount = line.NodeList.Count();
            for (int i = 0; i < nodeCount; i++)
            {
                CtlNodeBaseModel node = line.NodeList[i];
                UserControlCtlNode monitorNode = new UserControlCtlNode();
                monitorNode.Title = node.NodeName;
                userControlLine.Add(monitorNode);
                userCtlNodes[node.NodeName] = monitorNode;
                monitorNode.TimerInfo = "40";
                monitorNode.RefreshDisp();
                nodeStatusList.Add(new CtlNodeStatus(node.NodeName) { ProductBarcode = node.CurrentStat.ProductBarcode });
            }
            userControlLine.ReLayout();
        }
        //public void InitNodeMonitorview(int lineSeq,List<CtlNodeBaseModel> nodeList)
        //{
        //    userCtlNodes = new Dictionary<string,UserControlCtlNode>();
        //    nodeStatusList = new List<CtlNodeStatus>();
        //    //this.flowLayoutPanel1.Controls.Clear();
        //    int nodeCount = nodeList.Count();
        //    Size boxSize = new Size(0,0);
        //    View.UserControlLine userControlLine = null;
        //    if(lineSeq == 1)
        //    {
        //        userControlLine = this.userControlLine1;
                
        //    }
        //    else if(lineSeq == 2)
        //    {
        //        userControlLine = this.userControlLine2;
               
        //    }
        //    else
        //    {
        //        userControlLine = this.userControlLine3;
        //    }
        //    for (int i = 0; i < nodeCount; i++)
        //    {
        //        CtlNodeBaseModel node = nodeList[i];
        //        UserControlCtlNode monitorNode = new UserControlCtlNode();
        //        monitorNode.Title = "工位#" + (i + 1).ToString();
        //        userControlLine.Add(monitorNode);
        //        userCtlNodes[node.NodeName] = monitorNode;
        //        monitorNode.TimerInfo = "40";
        //        monitorNode.RefreshDisp();
        //        nodeStatusList.Add(new CtlNodeStatus(node.NodeName) { ProductBarcode = node.CurrentStat.ProductBarcode });
        //    }
        //    userControlLine.ReLayout();

        //        // boxSize.Width = this.flowLayoutPanel1.Width/(nodeCount+1);
        //        //   boxSize.Height =(int)(this.flowLayoutPanel1.Height);
        //        //foreach(CtlNodeBaseModel node in nodeList)
        //        //for (int i = 0; i < nodeList.Count();i++ )
        //        //{
        //        //    CtlNodeBaseModel node = nodeList[i];
        //        //    UserControlCtlNode monitorNode = new UserControlCtlNode();
        //        //    monitorNode.Size = boxSize;
        //        //    //监控控件属性赋值
        //        //    monitorNode.Title = node.NodeName;
        //        //    monitorNode.TimerInfo = "40";

        //        //    monitorNode.RefreshDisp();
        //        //    this.flowLayoutPanel1.Controls.Add(monitorNode);
        //        //    userCtlNodes[node.NodeName] = monitorNode;

        //        //    nodeStatusList.Add(new CtlNodeStatus(node.NodeName) { ProductBarcode = node.CurrentStat.ProductBarcode });
        //        //}
        //        //配色方案

        //        //设备状态图例颜色
           
        //}
        public  System.Windows.Forms.Control GetHostControl()
        {
            return this;
        }
        public void RefreshNodeStatus()
        {
          
            List<CtlNodeStatus> ns = this.lineMonitorPresenter.GetNodeStatus();
            for (int i = 0; i < Math.Min(ns.Count(),this.nodeStatusList.Count());i++ )
            {
                CtlNodeStatus nodeStatus = this.nodeStatusList[i];
                ns[i].Copy(ref nodeStatus);
                UserControlCtlNode monitorNode = userCtlNodes[nodeStatus.NodeName];
                monitorNode.BkgColor = nodeColorMap[nodeStatus.Status];
                monitorNode.DispDetail = nodeStatus.StatDescribe;
                monitorNode.RefreshDisp();
                //monitorNode.DispPopupInfo = "主机条码："+nodeStatus.ProductBarcode;
         
            }
           

           
            
        }
        public void WelcomeAddStartinfo(string info)
        {
            this.welcomeDlg.AddDispContent(info);
        }
        public  void WelcomeDispCurinfo(string info)
        {
           
            this.welcomeDlg.DispCurrentInfo(info);
        }
        
        public void AsynWelcomePopup()
        {
           
            this.welcomeDlg.ShowDialog();
        }
        public void WelcomePopup()
        {
            welcomeDlg = new WelcomeForm();
            DlgtPopupWelcome dlgt = new DlgtPopupWelcome(AsynWelcomePopup);
            dlgt.BeginInvoke(null, null);

        }
        public void WelcomeClose()
        {
            this.welcomeDlg.CloseDisp();
            this.welcomeDlg = null;
        }
        private delegate void DlgtDispCommInfo(string strInfo);
        public void DispCommInfo(string strInfo)
        {
            if(this.labelCommInfo.InvokeRequired)
            {
                DlgtDispCommInfo dlgt = new DlgtDispCommInfo(DispCommInfo);
                this.Invoke(dlgt, new object[] { strInfo });
            }
            else
            {
                strInfo += "\n";
                if (SysCfgModel.ManualMode1)
                {
                    strInfo += "1号房人工模式";
                }
                if(SysCfgModel.ManualMode3)
                {
                    strInfo += ",3号房人工模式";
                }
                this.labelCommInfo.Text = strInfo;
               
            }
        }
        public void StartEnabled(bool enabled)
        {
            this.lineMonitorPresenter.StartRun();
            this.timerNodeStatus.Start();
        }
        #endregion
        #region UI事件
        private void timerNodeStatus_Tick(object sender, EventArgs e)
        {
            try
            {
                RefreshNodeStatus();

                if (this.checkBoxAutorefresh.Checked)
                {
                    RefreshPLCComm();
                }
            }
            catch (Exception ex )
            {
                Console.WriteLine(ex.ToString());
            }
           
        }

        private void NodeMonitorView_SizeChanged(object sender, EventArgs e)
        {
            if(this.userCtlNodes != null)
            {
               // Size monitorBoxSize = new Size(this.flowLayoutPanel1.Width / (this.userCtlNodes.Count() + 2), (int)(this.flowLayoutPanel1.Height * 0.8));
                //foreach (string nodeKey in this.userCtlNodes.Keys)
                //{
                //    UserControl monitorNode = this.userCtlNodes[nodeKey];
                //    monitorNode.Size = monitorBoxSize;
                //}
            }
          
        }
        private void NodeMonitorView_Load(object sender, EventArgs e)
        {
            if (SysCfgModel.SimMode)
            {
                this.comboBoxDB2.Items.Clear();
                for (int i = 0; i < 5; i++)
                {
                    this.comboBoxDB2.Items.Add((i + 1).ToString());
                }
                this.comboBoxDB2.SelectedIndex = 0;
                this.comboBoxRfid.Items.Clear();
                for (int i = 0; i < 13; i++)
                {
                    this.comboBoxRfid.Items.Add((i + 1).ToString());
                }
                this.comboBoxRfid.SelectedIndex = 0;

                this.comboBoxBarcodeGun.Items.AddRange(new string[] { "1", "2", "3" });
                this.comboBoxBarcodeGun.SelectedIndex = 0;
            }
            else
            {
                this.groupBoxCtlSim.Visible = false;
            }
            
          
           
        }
        private void bt_StartSystem_Click(object sender, EventArgs e)
        {
            OnStart();
          
        }

        private void bt_StopSystem_Click(object sender, EventArgs e)
        {
            OnStop();
        }
        private void bt_ExitSys_Click(object sender, EventArgs e)
        {
            OnExit();
        }
        

        private void btnDevCheck_Click(object sender, EventArgs e)
        {
            OnStop();
            lineMonitorPresenter.CommDevConnect();
           
        }
        private void NodeMonitorView_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void checkBoxAutorefresh_CheckedChanged(object sender, EventArgs e)
        {

        }
        #endregion
        #region 通信数据监控与流程仿真 
        private void buttonRefreshDevStatus_Click(object sender, EventArgs e)
        {
            RefreshPLCComm();
            
        }
        private void buttonClearDevCmd_Click(object sender, EventArgs e)
        {

        }

        private void buttonDB2SimSet_Click(object sender, EventArgs e)
        {
            if (!SysCfgModel.SimMode)
            {
                MessageBox.Show("当前不处于仿真模式");
                return;
            }
            try
            {
                string devID = this.comboBoxDevList.Text;
                int itemID = int.Parse(comboBoxDB2.Text);
                lineMonitorPresenter.SimSetDB2(devID, itemID, int.Parse(this.textBoxDB2ItemVal.Text));

                RefreshPLCComm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
           
        }

        private void buttonDB2Reset_Click(object sender, EventArgs e)
        {

        }

        private void comboBoxDevList_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshPLCComm();
        }
        private void RefreshPLCComm()
        {
            string nodeName = this.comboBoxDevList.Text;

            DataTable dt1 = null;
            DataTable dt2 = null;
            //DataTable dtTask = null;
            string taskDetail="";
            bool mesStop = true;
            if (!lineMonitorPresenter.GetDevRunningInfo(nodeName, ref dt1, ref dt2,ref taskDetail,ref mesStop))
            {
                Console.WriteLine("刷新工位息失败");
                return;
            }
            this.checkBoxMesRun.Checked = !mesStop;
            this.dataGridViewDevDB1.DataSource = dt1;
            for (int i = 0; i < this.dataGridViewDevDB1.Columns.Count; i++)
            {
                this.dataGridViewDevDB1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                this.dataGridViewDevDB1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            this.dataGridViewDevDB1.Columns[this.dataGridViewDevDB1.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewDevDB2.DataSource = dt2;
            for (int i = 0; i < this.dataGridViewDevDB2.Columns.Count; i++)
            {
                this.dataGridViewDevDB2.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                this.dataGridViewDevDB2.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            this.dataGridViewDevDB2.Columns[this.dataGridViewDevDB2.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
       
            this.richTextBoxTaskInfo.Text = taskDetail;
        }

        private void buttonRfidSimWrite_Click(object sender, EventArgs e)
        {
            try
            {
                string nodeName = this.comboBoxDevList.Text;
                string rfidVal = this.textBoxRfidVal.Text;
                lineMonitorPresenter.SimSetRFID(nodeName, rfidVal);
                //DevControlPresenter.rfidRWDic[rfidID].WriteDataInt64(rfidVal);
                int gunIndex = int.Parse(this.comboBoxBarcodeGun.Text);
                string barcode = this.textBoxBarcode.Text;
              //  lineMonitorPresenter.SimSetBarcode(gunIndex, barcode);
                lineMonitorPresenter.SimSetBarcode(nodeName, barcode);
                //if(this.comboBoxDevList.Text== "气密检查1" || this.comboBoxDevList.Text== "气密检查2"||this.comboBoxDevList.Text== "气密检查3")
                //{
                //    lineMonitorPresenter.SimSetCheckRe(nodeName, this.comboBoxAirlossRe.Text);
                //}
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        #endregion  
       
        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void buttonClearDevCmd_Click_1(object sender, EventArgs e)
        {

        }
        private void OnMesRun()
        {
            string nodeName = this.comboBoxDevList.Text;
            string reStr="";
            if(LinePresenter.MesRunCommit(nodeName, true, ref reStr))
            {
                Console.WriteLine("{0}MES停机已恢复",nodeName);
            }
            else
            {
                Console.WriteLine("{0}MES停机恢复失败,{1}", nodeName,reStr);
            }
            RefreshPLCComm();
        }
        private void buttonMesRun_Click(object sender, EventArgs e)
        {
            OnMesRun();
        }
    }
}
