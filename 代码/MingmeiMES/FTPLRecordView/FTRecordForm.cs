using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Windows.Forms;
using ModuleCrossPnP;
using LogManage;
using LogInterface;
using ProductRecordView;
using ConfigManage;


namespace FTPLRecordView
{
    /// <summary>
    /// 查询终端主程序
    /// </summary>
    public partial class FTRecordForm : Form, ILogDisp, IParentModule
    {
        MonitorSVC.INodeMonitorSvc nodeMonitorSvc = null;
        const int CLOSE_SIZE = 10;
       // private CtlcorePresenter corePresenter;
        private int roleID = 2;
        LogView logView = null;
        ProductRecordView.RecordView recordView = null;
        ConfiManageView configView = null;
        private List<string> childList = null;
        public string CurUsername { get { return ""; } }
        public FTRecordForm()
        {
            InitializeComponent();
            childList = new List<string>();
           
        }
      
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.MainTabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.MainTabControl.Padding = new System.Drawing.Point(CLOSE_SIZE, CLOSE_SIZE);
            this.MainTabControl.DrawItem += new DrawItemEventHandler(this.tabControlMain_DrawItem);
            this.MainTabControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tabControlMain_MouseDown);
            Console.SetOut(new TextBoxWriter(this.richTextBoxLog));
           // corePresenter = new CtlcorePresenter();
           
            ModuleAttach();

            string svcAddr = @"http://localhost:8733/ZZ/LineNodes/NodeMonitorSvc/";
            nodeMonitorSvc = ChannelFactory<MonitorSVC.INodeMonitorSvc>.CreateChannel(new BasicHttpBinding(), new EndpointAddress(svcAddr)); ;//new AsrsSvc.AsrsPowerSvcClient();
            //MonitorSVC.CtlNodeStatus[] nodeStatusArray = nodeMonitorSvc.GetNodeStatus();
            //foreach (MonitorSVC.CtlNodeStatus s in nodeStatusArray)
            //{
            //    Console.WriteLine(s.NodeName + "," + s.Status.ToString());
            //}
            //nodeMonitorSvc.DoWork();
        }

        /// <summary>
        /// 模块加载
        /// </summary>
        private void ModuleAttach()
        {
           
            logView = new LogView("日志");
            

            logView.SetParent(this);
            logView.RegisterMenus(this.menuStrip1, "日志查询");
            logView.SetLogDispInterface(this);
            string[] nodeNames = new string[]{"产品上线","气密检查1","气密检查2","气密检查3","气密检查4"};
            List<string> nodeList = new List<string>(nodeNames);
            logView.SetNodeNames(nodeList);

            recordView = new RecordView();
            recordView.SetParent(this);
            recordView.RegisterMenus(this.menuStrip1, "记录查询与管理");
            recordView.SetLoginterface(logView.GetLogrecorder());

           
            if(this.roleID <3 && this.roleID>0)
            {
                configView = new ConfiManageView();
                configView.SetParent(this);
                configView.RegisterMenus(this.menuStrip1, "配置管理");
                configView.SetLoginterface(logView.GetLogrecorder());
                
            }  
        }
        #region 接口实现
        public int RoleID { get { return this.roleID; } }
        private delegate void DelegateDispLog(LogModel log);//委托，显示日志
        public void DispLog(LogModel log)
        {
            if(this.richTextBoxLog.InvokeRequired)
            {
                DelegateDispLog delegateLog = new DelegateDispLog(DispLog);
                this.Invoke(delegateLog, new object[] {log });
            }
            else
            {
                this.richTextBoxLog.Text += (string.Format("[{0:yyyy-MM-dd HH:mm:ss.fff}]{1},{2}", log.LogTime, log.LogSource,log.LogContent) + "\r\n");
            }
            
        }
        public void AttachModuleView(System.Windows.Forms.Form childView)
        {
            TabPage tabPage = null;
           if(this.childList.Contains(childView.Text))
           {
               tabPage = this.MainTabControl.TabPages[childView.Text];
               this.MainTabControl.SelectedTab = tabPage;
               return;
           }
          
           this.MainTabControl.TabPages.Add(childView.Text, childView.Text);
           tabPage = this.MainTabControl.TabPages[childView.Text];
           tabPage.Controls.Clear();
           this.MainTabControl.SelectedTab = tabPage;
           childView.MdiParent = this;
           
           tabPage.Controls.Add(childView);
           this.childList.Add(childView.Text);
           childView.Dock = DockStyle.Fill;
           childView.Size = this.panelCenterview.Size;
           childView.Show();
           
        }
        #endregion
  
        #region UI事件

        private void panelCenterview_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                if(this.panelCenterview.Controls.Count<1)
                {
                    return;
                }
                Control child = this.panelCenterview.Controls[0];
                if (child != null)
                {
                    child.Size = this.panelCenterview.Size;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
               // throw;
            }
           
           
        }

        private void tabControlMain_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                Rectangle myTabRect = this.MainTabControl.GetTabRect(e.Index);

                //先添加TabPage属性   
                e.Graphics.DrawString(this.MainTabControl.TabPages[e.Index].Text
                , this.Font, SystemBrushes.ControlText, myTabRect.X + 2, myTabRect.Y + 2);

                myTabRect.Offset(myTabRect.Width - (CLOSE_SIZE + 3), 2);
                myTabRect.Width = CLOSE_SIZE;
                myTabRect.Height = CLOSE_SIZE;
                //再画一个矩形框
                using (Pen p = new Pen(Color.Red))
                {

                    //  e.Graphics.DrawRectangle(p, myTabRect);
                }

                //填充矩形框
                //Color recColor = e.State == DrawItemState.Selected ? Color.Red : Color.Transparent;
                //using (Brush b = new SolidBrush(recColor))
                //{
                //    e.Graphics.FillRectangle(b, myTabRect);
                //}

                //画关闭符号
                using (Pen objpen = new Pen(Color.DarkGray, 1.0f))
                {
                    //"\"线
                    Point p1 = new Point(myTabRect.X + 3, myTabRect.Y + 3);
                    Point p2 = new Point(myTabRect.X + myTabRect.Width - 3, myTabRect.Y + myTabRect.Height - 3);
                    e.Graphics.DrawLine(objpen, p1, p2);

                    //"/"线
                    Point p3 = new Point(myTabRect.X + 3, myTabRect.Y + myTabRect.Height - 3);
                    Point p4 = new Point(myTabRect.X + myTabRect.Width - 3, myTabRect.Y + 3);
                    e.Graphics.DrawLine(objpen, p3, p4);
                }

                e.Graphics.Dispose();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }
        }
        private void tabControlMain_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int x = e.X, y = e.Y;

                //计算关闭区域   
                Rectangle myTabRect = this.MainTabControl.GetTabRect(this.MainTabControl.SelectedIndex);

                myTabRect.Offset(myTabRect.Width - (CLOSE_SIZE + 3), 2);
                myTabRect.Width = CLOSE_SIZE;
                myTabRect.Height = CLOSE_SIZE;

                //如果鼠标在区域内就关闭选项卡   
                bool isClose = x > myTabRect.X && x < myTabRect.Right
                 && y > myTabRect.Y && y < myTabRect.Bottom;

                if (isClose == true)
                {
                    
                    string tabText = this.MainTabControl.SelectedTab.Text;
                    this.childList.Remove(tabText);
                    this.MainTabControl.TabPages.Remove(this.MainTabControl.SelectedTab);
                  
                }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.richTextBoxLog.Text = string.Empty;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
           
        }
        #endregion

    }

}
