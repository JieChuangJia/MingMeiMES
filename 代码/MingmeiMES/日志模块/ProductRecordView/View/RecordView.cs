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
namespace ProductRecordView
{
    public partial class RecordView : BaseChildView
    {
       // private ProduceReccordView produceRecord = null;
       // private MesDatarecordView mesDataRecord = null;
        //private OnlineProductView onlineProductView = null;
        private ProductDataView productView = null;
        private ProduceTraceView produceRecord = null;
        private DevWarnRecordView devWarnRecord = null;
        
        public RecordView():base(string.Empty)
        {
            InitializeComponent();
            //this.onlineProductView = new OnlineProductView("在线产品");
           // this.produceRecord = new ProduceReccordView("生产记录");
           // this.mesDataRecord = new MesDatarecordView("MES上传记录");
            this.produceRecord = new ProduceTraceView("生产追溯");
            this.devWarnRecord = new DevWarnRecordView("设备报警记录");
            this.productView = new ProductDataView("产品数据");
            childViews.Add(this.produceRecord);
            childViews.Add(this.devWarnRecord);
            childViews.Add(this.productView);
        }
        public void SetDevList(List<CtlDevBaseModel> devList)
        {
            this.devWarnRecord.SetDevList(devList);
        }
        public void SetOpStations(string[] stationNames)
        {
            this.productView.SetOpStations(stationNames);
        }
        #region IModuleAttach接口实现
        public override void RegisterMenus(MenuStrip parentMenu, string rootMenuText)
        {

            ToolStripMenuItem rootMenuItem = new ToolStripMenuItem(rootMenuText);//parentMenu.Items.Add("仓储管理");
            //rootMenuItem.Click += LoadMainform_MenuHandler;
            parentMenu.Items.Add(rootMenuItem);
            //ToolStripItem onlineProductsItem = rootMenuItem.DropDownItems.Add("在线产品");
            AddMenu(rootMenuItem,"生产记录");
            AddMenu(rootMenuItem,"设备报警记录");
            AddMenu(rootMenuItem,"产品数据");
           
        //    mesDataRecordItem.Click += LoadView_MenuHandler;
           // onlineProductsItem.Click += LoadView_MenuHandler;
        }

        public override void SetParent(/*Control parentContainer, Form parentForm, */IParentModule parentPnP)
        {
            this.parentPNP = parentPnP;
            foreach(BaseChildView childView in childViews)
            {
                childView.SetParent(parentPnP);
            }
        }
        public override void SetLoginterface(ILogRecorder logRecorder)
        {
            this.logRecorder = logRecorder;
            //   lineMonitorPresenter.SetLogRecorder(logRecorder);
            this.produceRecord.SetLoginterface(logRecorder);
            this.devWarnRecord.SetLoginterface(logRecorder);
            this.productView.SetLoginterface(logRecorder);
          //  this.mesDataRecord.SetLoginterface(logRecorder);
        }
       
        #endregion
        private void LoadView_MenuHandler(object sender, EventArgs e)
        {
            ToolStripItem menuItem = sender as ToolStripItem;
            if (menuItem == null)
            {
                return;
            }
            switch (menuItem.Text)
            {
                
                case "生产记录":
                    {
                        this.parentPNP.AttachModuleView(this.produceRecord);
                        break;
                    }
                case "设备报警记录":
                    {
                        this.parentPNP.AttachModuleView(this.devWarnRecord);
                        break;
                    }
                case "产品数据":
                    {
                        this.parentPNP.AttachModuleView(this.productView);
                        break;
                    }
                default:
                    break;
            }


        }
        private void AddMenu(ToolStripMenuItem rootMenuItem, string menuStr)
        {
            ToolStripItem menutem = rootMenuItem.DropDownItems.Add(menuStr);
            menutem.Click += LoadView_MenuHandler;
        }
    }
}
