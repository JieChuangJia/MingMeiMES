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
using GDICommon;
using System.Xml;
using SuperTextEdit;
namespace ConfigManage
{
    public partial class PLCSettingView : BaseChildView
    {
        #region 数据区
        private List<CtlDevBaseModel> DevList = new List<CtlDevBaseModel>();
        XMLOperater xmlOpertor = new XMLOperater();
       
        private ISuperTextEdit iSuperTextEdit = null;
       
        private CtlDevBaseModel CurrDev { get; set; }
        #endregion
        public PLCSettingView(string captiontxt)
            : base(captiontxt)
        {
            InitializeComponent();
            this.Text = captiontxt;
        }
        private void PLCSettingView_Load(object sender, EventArgs e)
        {
            this.superTextEdit1.PositionsClick += ClickBtHandle;
            
        }
      
        #region IModuleAttach接口实现
        //public override void RegisterMenus(MenuStrip parentMenu, string rootMenuText)
        //{

        //    ToolStripMenuItem rootMenuItem = new ToolStripMenuItem(rootMenuText);//parentMenu.Items.Add("仓储管理");
        //    //rootMenuItem.Click += LoadMainform_MenuHandler;
        //    parentMenu.Items.Add(rootMenuItem);
        //    // if(parentPNP.RoleID<3)
        //    {
        //        ToolStripItem productDSItem = rootMenuItem.DropDownItems.Add("PLC设置");
              
        //        // ToolStripItem mesofflineItem = rootMenuItem.DropDownItems.Add("MES离线模式");
        //        productDSItem.Click += PLCSettingHandler;
              
        //        //  mesofflineItem.Click += LoadView_MenuHandler;
        //    }
            

        //}
        //public override void SetParent(/*Control parentContainer, Form parentForm, */IParentModule parentPnP)
        //{
        //    this.parentPNP = parentPnP;
        //    //if (parentPNP.RoleID == 1)
        //    //{
        //    //    sysDefineView = new SysDefineView("系统维护");
        //    //    this.sysDefineView.SetParent(parentPnP);
        //    //}
        //    //this.SetParent(parentPnP);
         
        //}
        //public override void SetLoginterface(ILogRecorder logRecorder)
        //{
        //    this.logRecorder = logRecorder;
         
        //}
        #endregion
        #region 私有函数
        /// <summary>
        /// 显示指定设备的配置
        /// </summary>
        /// <param name="deviceID"></param>
        private void ShowPLCSet(string deviceID,bool readPLCValue)
        {
            CtlDevBaseModel devModel = null;
            if (GetDeviceModel(deviceID, ref devModel) == false)
            {
                return;
            }
            this.CurrDev = devModel;
            XmlDocument xmldoc = new XmlDocument();
            XmlNode root = xmldoc.CreateElement("Root");
            int readtimes = 0;//读取三个地址都不上来就不读取了
            IDictionary<string, DevCfgItemModel> devCfg = devModel.devCfgList;
            DevInterface.IPlcRWEtd plcEtd = devModel.PlcRW as DevInterface.IPlcRWEtd;
            foreach (DevCfgItemModel cfg in devCfg.Values)
            {
                XmlElement objElement = xmldoc.CreateElement("Item");
                int []addrValue =new int[1];
               

                if (devModel.PlcRW.IsConnect == true && readPLCValue == true && readtimes < 3)
                {

                    if (plcEtd.ReadMultiDB(cfg.PlcAddr,1, ref addrValue) == false)//每次都读取plc地址的值
                    {
                        readtimes++;
                    }
                    else
                    {
                        readtimes = 0;
                    }
                }
               

                objElement.InnerXml = addrValue[0].ToString();
                objElement.SetAttribute("name", cfg.PlcAddr);
                objElement.SetAttribute("type", cfg.AddrDataType.ToString());
                objElement.SetAttribute("style", "EDITOR");
                objElement.SetAttribute("desc", cfg.Desc);


                root.AppendChild(objElement);
            }
            this.superTextEdit1.DataSource = root.OuterXml;
        }
        /// <summary>
        /// 显示设备列表
        /// </summary>
        private void ShowDevList()
        {
            if (this.DevList == null)
            {
                return;
            }
            this.dgv_DevList.Rows.Clear();
            for (int i = 0; i < this.DevList.Count; i++)
            {
                CtlDevBaseModel dev = this.DevList[i];
                this.dgv_DevList.Rows.Add();
                this.dgv_DevList.Rows[i].Cells["col_DeviceID"].Value = dev.DevID;
                this.dgv_DevList.Rows[i].Cells["col_DeviceName"].Value = dev.DevName;
            }
        }
        /// <summary>
        /// 菜单响应函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PLCSettingHandler(object sender, EventArgs e)
        {
            this.parentPNP.AttachModuleView(this);
        }
      
     
        /// <summary>
        /// 显示当前选中设备
        /// </summary>
        private void ShowSelectDevCfg(bool readPLCValue)
        {
           
            if (this.dgv_DevList.Rows.Count <= 0)
            {
                return;
            }
            if (this.dgv_DevList.CurrentRow == null)
            {
                return;
            }
         
            int index = this.dgv_DevList.CurrentRow.Index;
            object devObj = this.dgv_DevList.Rows[index].Cells["col_DeviceID"].Value;
            if (devObj == null)
            {
                return;
            }
            string devID = devObj.ToString();
            ShowPLCSet(devID, readPLCValue);
        }
        /// <summary>
        /// 获取指定设备模型
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        private bool GetDeviceModel(string deviceID, ref CtlDevBaseModel device)
        {
            if (this.DevList == null)
            {
                return false;
            }
            CtlDevBaseModel devModel = null;
            foreach (CtlDevBaseModel dev in this.DevList)
            {
                if (dev.DevID == deviceID)
                {
                    devModel = dev;
                    break;
                }
            }

            device = devModel;
            if (device == null)
            {
                return false;
            }
            return true;
        }
        #endregion
        #region 共有函数
        public void SetDevList(List<CtlDevBaseModel> devList)
        {
            this.DevList = devList;
            
            ShowDevList();
        }

        #endregion
        private void superTextEdit1_MouseClick(object sender, MouseEventArgs e)
        {
            
        }
        private void ClickBtHandle(object sender, SuperTextEdit.SuperTextEdit.ClickPositionsEventArgs e)
        {
            if (this.CurrDev.PlcRW.IsConnect == false)
            {
                MessageBox.Show("PLC连接失败！无法设置当前选项！");
                return;
            }
            ParamSetView psw = new ParamSetView(e.GrapTbox, this.superTextEdit1, this.CurrDev);
            psw.ShowDialog();
        }
        private void dgv_DevList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex<0)
            {
                return;
            }
            string devID = this.dgv_DevList.Rows[e.RowIndex].Cells["col_DeviceID"].Value.ToString();
            ShowPLCSet(devID,false);
        }

        private void dgv_DevList_SelectionChanged(object sender, EventArgs e)
        {

            ShowSelectDevCfg(false);
        }

        private void 刷新数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.CurrDev==null)
            {
                MessageBox.Show("当前设备对象为空！");
                return;
            }
            if(this.CurrDev.PlcRW.IsConnect == false)
            {
                MessageBox.Show("当前设备未连接至PLC，数据刷新失败！");
                return;
            }
            ShowSelectDevCfg(true);
        }
 

    }
}
