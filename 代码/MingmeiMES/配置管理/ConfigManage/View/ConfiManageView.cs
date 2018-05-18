using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ModuleCrossPnP;
using LogInterface;
using PLProcessModel;
namespace ConfigManage
{
    public partial class ConfiManageView : BaseChildView
    {
        //private string captionText = "";
        //private ILogRecorder logRecorder = null;
        //private IParentModule parentPNP = null;
        private ProductDatasheetView productDSview = null;
        private UserManageView userManageView = null;
        private SysSettingView sysSettignView = null;
       // private SysDefineView sysDefineView = null;
        private DetectCodeCfgView detectCodeView = null;
        private MesOfflineView mesOfflineView = null;
        private WorkerResView workerResView = null;
        private List<CtlDevBaseModel> DevList = new List<CtlDevBaseModel>();
        private PLCSettingView plcSettingView = null;
        #region 公共接口
        //public string CaptionText { get { return captionText; } set { captionText = value; this.Text = captionText; } }
        public ConfiManageView():base(string.Empty)
        {
            InitializeComponent();
           // productDSview = new ProductDatasheetView("产品型号管理");
            userManageView = new UserManageView("用户管理");
            sysSettignView = new SysSettingView("系统设置");
           // workerResView = new WorkerResView("操作员登录");
        //    sysDefineView = new SysDefineView("系统维护");
          //  detectCodeView = new DetectCodeCfgView("不良代码配置");
            mesOfflineView = new MesOfflineView("MES离线设置");
			plcSettingView = new PLCSettingView("设备参数");
        }
		public void SetDevList(List<CtlDevBaseModel> devList)
        {
            this.DevList = devList;
            this.plcSettingView.SetDevList(this.DevList);
            this.sysSettignView.SetDevList(this.DevList);
           
        }
        
        #endregion
       
        #region IModuleAttach接口实现
        public override void RegisterMenus(MenuStrip parentMenu, string rootMenuText)
        {
           
            ToolStripMenuItem rootMenuItem = new ToolStripMenuItem(rootMenuText);//parentMenu.Items.Add("仓储管理");
            //rootMenuItem.Click += LoadMainform_MenuHandler;
            parentMenu.Items.Add(rootMenuItem);
          
           
            ToolStripItem userItem = rootMenuItem.DropDownItems.Add("修改密码");
            ToolStripItem sysSetItem = rootMenuItem.DropDownItems.Add("系统设置");
        
           
            userItem.Click += LoadView_MenuHandler;
            sysSetItem.Click += LoadView_MenuHandler;
            ToolStripItem devCfg = rootMenuItem.DropDownItems.Add("设备参数");
            devCfg.Click += LoadView_MenuHandler;

            ToolStripItem mesOfflineSet = rootMenuItem.DropDownItems.Add("MES离线设置");
            mesOfflineSet.Click += LoadView_MenuHandler;
        }
        public override void SetParent(/*Control parentContainer, Form parentForm, */IParentModule parentPnP)
        {
            this.parentPNP = parentPnP;
            //if (parentPNP.RoleID == 1)
            //{
            //    sysDefineView = new SysDefineView("系统维护");
            //    this.sysDefineView.SetParent(parentPnP);
            //}
           // this.productDSview.SetParent(parentPnP);
            this.sysSettignView.SetParent(parentPnP);
            this.userManageView.SetParent(parentPnP);
         //   this.detectCodeView.SetParent(parentPNP);
         //   this.mesOfflineView.SetParent(parentPnP);
        }
        public override void SetLoginterface(ILogRecorder logRecorder)
        {
            this.logRecorder = logRecorder;
         //   lineMonitorPresenter.SetLogRecorder(logRecorder);
        //    this.productDSview.SetLoginterface(logRecorder);
            this.sysSettignView.SetLoginterface(logRecorder);
        //    this.workerResView.SetLoginterface(logRecorder);
            this.userManageView.SetLoginterface(logRecorder);
        //    this.mesOfflineView.SetLoginterface(logRecorder);
        }
        #endregion
        private void LoadView_MenuHandler(object sender, EventArgs e)
        {
            ToolStripItem menuItem = sender as ToolStripItem;
            if(menuItem == null)
            {
                return;
            }
            switch(menuItem.Text)
            {
                case "产品型号管理":
                    {
                        if(parentPNP.RoleID >=3)
                        {
                            Console.WriteLine("请切换到管理员用户");
                            break;
                        }
                        this.parentPNP.AttachModuleView(this.productDSview);
                        break;
                    }
                case "修改密码":
                    {
                        this.parentPNP.AttachModuleView(this.userManageView);
                        break;
                    }
                case "系统设置":
                    {
                        if (parentPNP.RoleID >= 3)
                        {
                            Console.WriteLine("请切换到管理员用户");
                            break;
                        }
                        this.parentPNP.AttachModuleView(this.sysSettignView);
                        break;
                    }
                case "操作员登录":
                    {
                        if (parentPNP.RoleID >= 3)
                        {
                            Console.WriteLine("请切换到管理员用户");
                            break;
                        }
                        this.parentPNP.AttachModuleView(this.workerResView);
                        break;
                    }
                case "MES离线设置":
                    {
                        if (parentPNP.RoleID >= 3)
                        {
                            Console.WriteLine("请切换到管理员用户");
                            break;
                        }
                        this.parentPNP.AttachModuleView(this.mesOfflineView);
                        break;
                    }
                //case "系统维护":
                //    {
                //        if (parentPNP.RoleID != 1)
                //        {
                //            Console.WriteLine("请切换到系统维护用户");
                //            break;
                //        }
                //        this.parentPNP.AttachModuleView(this.sysDefineView);
                //        break;
                //    }
                //case "不良代码配置":
                //        {
                //            if (parentPNP.RoleID != 1)
                //            {
                //                Console.WriteLine("请切换到系统维护用户");
                //                break;
                //            }
                //            this.parentPNP.AttachModuleView(this.detectCodeView);
                //            break;
                //        }
                //case "MES离线模式":
                //        {
                //            if (parentPNP.RoleID >= 3)
                //            {
                //                Console.WriteLine("请切换到系统维护用户");
                //                break;
                //            }
                //            this.parentPNP.AttachModuleView(this.mesOfflineView);
                //            break;
                //        }
                case "设备参数":
                        {
                            if (parentPNP.RoleID >= 3)
                            {
                                Console.WriteLine("请切换到管理员用户");
                                break;
                            }
                            this.parentPNP.AttachModuleView(this.plcSettingView);
                            break;
                        }
                default:
                    break;
            }
            
            
        }

      
    }
}
