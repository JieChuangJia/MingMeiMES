using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using ModuleCrossPnP;
using LogManage;
using LogInterface;
using ProductRecordView;
using ConfigManage;
using LineNodes;
using LicenceManager;
using CommonPL.Login;
using PLProcessModel;
using MTDBAccess.Model;
using MTDBAccess.BLL;
namespace CommonPL
{
    public partial class MainForm : Form, ILogDisp, IParentModule,ILicenseNotify
    {
         private delegate void DlgtAbortApp();
        #region 数据
        private string version = "1.0.26 测试版 2018-5-2";

        private int roleID = 3;
        private string userName = "";
        const int CLOSE_SIZE = 10;
        int iconWidth = 16;
        int iconHeight = 16;
        // private CtlcorePresenter corePresenter;
        private List<string> childList = null;
        //子模块
        private LicenseMonitor licenseMonitor = null;
        private ConfiManageView configView = null;
        private NodeMonitorView nodeMonitorView = null;
        private LogView logView = null;
        private RecordView recordView = null;

        private List<BaseChildView> childViews = null;
        #endregion
      
        public MainForm()
        {
            InitializeComponent();
            childList = new List<string>();
            childViews = new List<BaseChildView>();
        }
        public MainForm(int roleID,string userName)
        {
            InitializeComponent();
            childList = new List<string>();
            childViews = new List<BaseChildView>();
            this.roleID = roleID;
            this.userName = userName;
        }
      
        private void MainForm_Load(object sender, EventArgs e)
        {
            string dbSrc = ConfigurationManager.AppSettings["DBSource"];
            string mtdbSrc = ConfigurationManager.AppSettings["MTDBConn"];
            string dcirdbSrc = ConfigurationManager.AppSettings["DCIRDBConn"];
            string fxjSrc = ConfigurationManager.AppSettings["FXJDBConn"];
          
            //CtlDBAccess.DBUtility.PubConstant.ConnectionString = string.Format(@"{0}Initial Catalog=ACEcams;User ID=sa;Password=123456;", dbSrc);
            string dbConn1 = string.Format(@"{0}Initial Catalog=MingmeiMES;User ID=sa;Password=123456;", dbSrc);
            FTDataAccess.DBUtility.DbHelperSQL.SetConnstr(dbConn1);

            MTDBAccess.PubConstant.ConnectionString = @"Provider = Microsoft.ACE.OLEDB.12.0;" + mtdbSrc + ";Persist Security Info = False;";
            DCIRDBAccess.PubConstant.ConnectionString = @"Provider = Microsoft.ACE.OLEDB.12.0;" + dcirdbSrc + ";Persist Security Info = False;";

            string fxjDbConn = string.Format(@"{0}Initial Catalog=HL_JBF;User ID=sa;Password=sa;", fxjSrc);

            FXJDatabase.DbHelperSQL.SetConnstr(fxjDbConn);
            // string dbSrcFenxuan = ConfigurationManager.AppSettings["FenxuanDBSource"];
           // CtlDBAccess.DBUtility.PubConstant.ConnectionString2 = string.Format(@"{0}Initial Catalog=HL_LWN;User ID=sa;Password=123456;", dbSrcFenxuan);
          
            this.labelVersion.Text = this.version;
            this.MainTabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.MainTabControl.Padding = new System.Drawing.Point(CLOSE_SIZE, CLOSE_SIZE);
            this.MainTabControl.DrawItem += new DrawItemEventHandler(this.tabControlMain_DrawItem);
            this.MainTabControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tabControlMain_MouseDown);
            Console.SetOut(new TextBoxWriter(this.richTextBoxLog));
         
           // corePresenter = new CtlcorePresenter();
            
            this.labelUser.Text = "当前用户：" + this.userName;
           
            ModuleAttach();//加载子模块

            string licenseFile = AppDomain.CurrentDomain.BaseDirectory + @"\NBSSLicense.lic";
            this.licenseMonitor = new LicenseMonitor(this, 2000, licenseFile, "zzkeyFT1");
            if (!this.licenseMonitor.StartMonitor())
            {
                throw new Exception("license 监控失败");
            }
            string reStr = "";
            if (!this.licenseMonitor.IslicenseValid(ref reStr))
            {
                MessageBox.Show(reStr);
                return;
            }
            this.bt_StartSystem.Enabled = true;
            this.bt_StopSystem.Enabled = false;
           // this.label12.Text = "系统待启动！";
        }

        /// <summary>
        /// 模块加载
        /// </summary>
        private void ModuleAttach()
        {
            logView = new LogView("日志");
            childViews.Add(logView);
            logView.SetParent(this);


            nodeMonitorView = new NodeMonitorView("流程监控");
            childViews.Add(nodeMonitorView);
            nodeMonitorView.SetParent(this);
            nodeMonitorView.RegisterMenus(this.menuStrip1, "流程监控");
            nodeMonitorView.SetLoginterface(logView.GetLogrecorder());
            nodeMonitorView.Init();
            
            
            //nodeMonitorView.SetAsrsMonitors(asrsCtlView.AsrsMonitors);
          //  nodeMonitorView.SetAsrsBatchSetCtl(asrsCtlView.AsrsBatchSettingCtl);

            logView.RegisterMenus(this.menuStrip1, "日志查询");
            logView.SetLogDispInterface(this);

            recordView = new RecordView();
            childViews.Add(recordView);
            recordView.SetParent(this);
            recordView.RegisterMenus(this.menuStrip1, "记录查询与管理");
            recordView.SetLoginterface(logView.GetLogrecorder());

            configView = new ConfiManageView();
            childViews.Add(configView);
            configView.SetParent(this);
            configView.RegisterMenus(this.menuStrip1, "配置管理");
            configView.SetLoginterface(logView.GetLogrecorder());
          
            List<CtlDevBaseModel> devModelList = nodeMonitorView.GetDevList();
            configView.SetDevList(devModelList);
           
            List<string> logSrcList = new List<string>();
            //List<string> logSrcs = asrsCtlView.GetLogsrcList();
            //if(logSrcs != null)
            //{
            //    logSrcList.AddRange(logSrcs);
            //}
          
		  

            //List<CtlDevBaseModel> devModelList = nodeMonitorView.GetDevList();
            //plcSettingView = new PLCSettingView();
            //childViews.Add(plcSettingView);
            //plcSettingView.SetParent(this);
            //plcSettingView.RegisterMenus(this.menuStrip1, "配置管理");
            //plcSettingView.SetLoginterface(logView.GetLogrecorder());
            //plcSettingView.SetDevList(devModelList);

            List<string> logSrcs = nodeMonitorView.GetLogsrcList();
            if (logSrcs != null)
            {
                logSrcList.AddRange(logSrcs);
            }
            logView.SetLogsrcList(logSrcList);
            //IList<string> devList = nodeMonitorView.LinePresenter.GetDevList();
           
            recordView.SetDevList(devModelList);
            recordView.SetOpStations(nodeMonitorView.GetNodeNames().ToArray());
            AttachModuleView(nodeMonitorView);
           
            foreach (BaseChildView childView in childViews)
            {
                childView.ChangeRoleID(this.roleID);
            }
        }
        #region 接口实现
        public string CurUsername { get { return this.userName; } }
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
                if (this.richTextBoxLog.Text.Count() > 10000)
                {
                    this.richTextBoxLog.Text = "";
                }
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
        #region ILicenseNotify接口实现
        public void ShowWarninfo(string info)
        {
            LogModel log = new LogModel("其它", info, EnumLoglevel.警告);
            logView.GetLogrecorder().AddLog(log);
        }
        public void LicenseInvalid(string warnInfo)
        {

            AbortApp();
            //nodeMonitorView.AbortApp();
            //this.bt_StartSystem.Enabled = false;
            //LogModel log = new LogModel("其它", warnInfo, EnumLoglevel.警告);
            //logView.GetLogrecorder().AddLog(log);
        }
         public void AbortApp()
        {
            if (this.bt_StartSystem.InvokeRequired)
            {
                DlgtAbortApp dlgt = new DlgtAbortApp(AbortApp);
                this.Invoke(dlgt, null);
            }
            else
            {
                nodeMonitorView.AbortApp();
                this.bt_StartSystem.Enabled = false;
                this.bt_StopSystem.Enabled = false;
            }
        }
        public void LicenseReValid(string noteInfo)
        {

            this.bt_StartSystem.Enabled = true;
            LogModel log = new LogModel("其它", noteInfo, EnumLoglevel.提示);
            logView.GetLogrecorder().AddLog(log);
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
               
                 Image icon = this.imageList1.Images[0];
                 Brush biaocolor = Brushes.Transparent; //选项卡的背景色
                Graphics g = e.Graphics;
                Rectangle r = MainTabControl.GetTabRect(e.Index);
                if (e.Index == this.MainTabControl.SelectedIndex)    //当前选中的Tab页，设置不同的样式以示选中
                {
                    Brush selected_color = Brushes.Wheat; //选中的项的背景色
                    g.FillRectangle(selected_color, r); //改变选项卡标签的背景色
                    string title = MainTabControl.TabPages[e.Index].Text + "  ";

                    g.DrawString(title, this.Font, new SolidBrush(Color.Black), new PointF(r.X , r.Y + 6));//PointF选项卡标题的位置

                    r.Offset(r.Width - iconWidth - 3, 2);
                    g.DrawImage(icon, new Point(r.X + 2, r.Y + 2));//选项卡上的图标的位置 fntTab = new System.Drawing.Font(e.Font, FontStyle.Bold);
                }
                else//非选中的
                {
                    g.FillRectangle(biaocolor, r); //改变选项卡标签的背景色
                    string title = this.MainTabControl.TabPages[e.Index].Text + "  ";

                    g.DrawString(title, this.Font, new SolidBrush(Color.Black), new PointF(r.X , r.Y + 6));//PointF选项卡标题的位置
                    r.Offset(r.Width - iconWidth - 3, 2);
                    g.DrawImage(icon, new Point(r.X + 2, r.Y + 2));//选项卡上的图标的位置
                }
                //Rectangle myTabRect = this.MainTabControl.GetTabRect(e.Index);

                ////先添加TabPage属性   
                //e.Graphics.DrawString(this.MainTabControl.TabPages[e.Index].Text
                //, this.Font, SystemBrushes.ControlText, myTabRect.X + 2, myTabRect.Y + 2);

                //myTabRect.Offset(myTabRect.Width - (CLOSE_SIZE + 3), 2);
                //myTabRect.Width = CLOSE_SIZE;
                //myTabRect.Height = CLOSE_SIZE;
                ////再画一个矩形框
                //using (Pen p = new Pen(Color.Red))
                //{

                //    //  e.Graphics.DrawRectangle(p, myTabRect);
                //}

          
                ////画关闭符号
                //using (Pen objpen = new Pen(Color.DarkGray, 1.0f))
                //{
                //    //"\"线
                //    Point p1 = new Point(myTabRect.X + 3, myTabRect.Y + 3);
                //    Point p2 = new Point(myTabRect.X + myTabRect.Width - 3, myTabRect.Y + myTabRect.Height - 3);
                //    e.Graphics.DrawLine(objpen, p1, p2);

                //    //"/"线
                //    Point p3 = new Point(myTabRect.X + 3, myTabRect.Y + myTabRect.Height - 3);
                //    Point p4 = new Point(myTabRect.X + myTabRect.Width - 3, myTabRect.Y + 3);
                //    e.Graphics.DrawLine(objpen, p3, p4);
                //}

                //e.Graphics.Dispose();
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
                Point p = e.Location;
                Rectangle r = MainTabControl.GetTabRect(this.MainTabControl.SelectedIndex);
                r.Offset(r.Width - iconWidth, 4);
                r.Width = iconWidth;
                r.Height = iconHeight;
                if (this.MainTabControl.SelectedTab.Text == nodeMonitorView.Text)
                {
                    return;
                }
                string tabText = this.MainTabControl.SelectedTab.Text;
         
                if (r.Contains(p))
                {
                    this.childList.Remove(tabText);
                    this.MainTabControl.TabPages.RemoveAt(this.MainTabControl.SelectedIndex);
                }
                    
                //int x = e.X, y = e.Y;

                ////计算关闭区域   
                //Rectangle myTabRect = this.MainTabControl.GetTabRect(this.MainTabControl.SelectedIndex);

                //myTabRect.Offset(myTabRect.Width - (CLOSE_SIZE + 3), 2);
                //myTabRect.Width = CLOSE_SIZE;
                //myTabRect.Height = CLOSE_SIZE;

                ////如果鼠标在区域内就关闭选项卡   
                //bool isClose = x > myTabRect.X && x < myTabRect.Right
                // && y > myTabRect.Y && y < myTabRect.Bottom;

                //if (isClose == true)
                //{
                //    if (this.MainTabControl.SelectedTab.Text == nodeMonitorView.Text)
                //    {
                //        return;
                //    }
                //    string tabText = this.MainTabControl.SelectedTab.Text;
                //    this.childList.Remove(tabText);
                //    this.MainTabControl.TabPages.Remove(this.MainTabControl.SelectedTab);
                  
                //}
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

        private void richTextBoxLog_TextChanged(object sender, EventArgs e)
        {
            this.richTextBoxLog.SelectionStart = this.richTextBoxLog.Text.Length; //Set the current caret position at the end
            this.richTextBoxLog.ScrollToCaret();
        }

        private void OnChangeRoleID()
        {
            try
            {
                LoginView2 logView2 = new LoginView2();
                if (DialogResult.OK == logView2.ShowDialog())
                {
                    string tempUserName = "";
                    int tempRoleID = logView2.GetLoginRole(ref tempUserName);
                    if (tempRoleID < 1)
                    {
                        return;
                    }
                    this.roleID = tempRoleID;
                    this.userName = tempUserName;
                    this.labelUser.Text = "当前用户：" + this.userName;
                    foreach(BaseChildView childView in childViews)
                    {
                        childView.ChangeRoleID(this.roleID);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void 切换用户ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnChangeRoleID();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.nodeMonitorView.OnExit())
            {
                e.Cancel = true;
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void bt_StartSystem_Click(object sender, EventArgs e)
        {
            nodeMonitorView.OnStart();
            this.bt_StopSystem.Enabled = true;
            this.bt_StartSystem.Enabled = false;
        }

        private void bt_StopSystem_Click(object sender, EventArgs e)
        {
            nodeMonitorView.OnStop();
            this.bt_StopSystem.Enabled = false;
            this.bt_StartSystem.Enabled = true;
        }

        private void bt_ExitSys_Click(object sender, EventArgs e)
        {
            nodeMonitorView.OnExit();
        }

    }

}
