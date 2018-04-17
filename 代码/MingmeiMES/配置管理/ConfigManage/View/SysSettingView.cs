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
using ConfigManage.Model;
using FTDataAccess.Model;
using FTDataAccess.Bll;
using FTDataAccess.BLL;
using PLProcessModel;
namespace ConfigManage
{
    public partial class SysSettingView : BaseChildView
    {
        private List<CtlDevBaseModel> devList = new List<CtlDevBaseModel>();
        #region  公有接口

       // public string CaptionText { get { return captionText; } set { captionText = value; this.Text = captionText; } }
        public SysSettingView(string captionText):base(captionText)
        {
            InitializeComponent();
          
        }
        public void SetDevList(List<CtlDevBaseModel> devList)
        {
            this.devList = devList;
        }
        #endregion

        private void buttonCfgApply_Click(object sender, EventArgs e)
        {
           
        }

        private void buttonCancelSet_Click(object sender, EventArgs e)
        {
           
        }

        private void SysSettingView_Load(object sender, EventArgs e)
        {
            this.cbxSwitchLine.Items.AddRange(new string[] { "1", "2", "3", "4" });
            this.cbxSwitchLine.SelectedIndex = 0;
            foreach(string strKey in SysCfgModel.SysParamDic.Keys)
            {
                PLProcessModel.ParamCfgBase.ParamModel paramModel = SysCfgModel.SysParamDic[strKey];
                PLProcessModel.ParamCfgBase.ParamCfgItemCtl paramCtl = new PLProcessModel.ParamCfgBase.ParamCfgItemCtl();
                paramCtl.SetParmObj(paramModel);
                this.flowLayoutPanel2.Controls.Add(paramCtl);
            }
           
        }
        private string[] GetSwitchLineMods(int switchLineNo)
        {
            
            PLNodesBll plnodeBll = new PLNodesBll();
            PLNodesModel plNode =  plnodeBll.GetModel("OPC001");
            if(plNode == null)
            {
                MessageBox.Show("不存在的工位号：OPC001");
                return null;
            }
            string[] modExistArray = null;
        
            if (switchLineNo==1)
            {
                modExistArray = plNode.tag1.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }
            else if(switchLineNo==2)
            {
                modExistArray = plNode.tag2.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }
            else if(switchLineNo == 3)
            {
                modExistArray = plNode.tag3.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                modExistArray = plNode.tag4.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }
           
            return modExistArray;
        }
        private bool UpdateSwitchLineMods(int switchLineNo, string[] mods)
        {
            PLNodesBll plnodeBll = new PLNodesBll();
            PLNodesModel plNode = plnodeBll.GetModel("OPC001");
            if (plNode == null)
            {
                MessageBox.Show("不存在的工位号：OPC001");
                return false;
            }
            string strMods = "";
            for (int i = 0; i < mods.Count(); i++)
            {
                strMods += (mods[i] + ",");
            }
            if (switchLineNo == 1)
            {
                plNode.tag1 = strMods;
            }
            else if (switchLineNo == 2)
            {
                plNode.tag2 = strMods;
            }
            else if (switchLineNo == 3)
            {
                plNode.tag3 = strMods;
            }
            else
            {
                plNode.tag4 = strMods;
            }
            return plnodeBll.Update(plNode);
        }
        private void OnRefreshSwitchlineBufMods()
        {
            int switchLineNo = int.Parse(this.cbxSwitchLine.Text);
            string[] modExistArray = GetSwitchLineMods(switchLineNo);
            this.listBoxMod.Items.Clear();
            if(modExistArray != null && modExistArray.Count()>0)
            {
                this.listBoxMod.Items.AddRange(modExistArray);
            }
        }
        private void btnDispBuf_Click(object sender, EventArgs e)
        {
            OnRefreshSwitchlineBufMods();
        }

        private void OnDelMod()
        {
            if (this.listBoxMod.SelectedItem == null)
            {
                MessageBox.Show("未选中待删除项");
                return;
            }
            int switchLineNo = int.Parse(this.cbxSwitchLine.Text);
            string[] modExistArray = GetSwitchLineMods(switchLineNo);
            string modSel = this.listBoxMod.SelectedItem.ToString();
            List<string> modList = new List<string>(modExistArray);
            if(PoupAskmes("确定要删除吗？") != 1)
            {
                return;
            }
            modList.Remove(modSel);
            if(UpdateSwitchLineMods(switchLineNo, modList.ToArray()))
            {
                MessageBox.Show("移除成功");
            }
            else
            {
                MessageBox.Show("移除失败");
            }
        }
        private void btnDel_Click(object sender, EventArgs e)
        {
            OnDelMod();
            OnRefreshSwitchlineBufMods();
        }
        private void OnAddMod()
        {
            string strModtoAdd = this.textBoxMod.Text;
            if(string.IsNullOrWhiteSpace(strModtoAdd))
            {
                return;
            }
            DBAccess.BLL.BatteryModuleBll modBll = new DBAccess.BLL.BatteryModuleBll();
            DBAccess.Model.BatteryModuleModel mod = modBll.GetModel(strModtoAdd);
            if (mod == null)
            {
                MessageBox.Show(string.Format("模块{0}不存在", strModtoAdd));
                return;
            }

            int switchLineNo = int.Parse(this.cbxSwitchLine.Text);
            string[] modExistArray = GetSwitchLineMods(switchLineNo);
           
            List<string> modList = new List<string>(modExistArray);
            if(modList.Contains(strModtoAdd))
            {
                MessageBox.Show(string.Format("模块{0}已经在分档线{1}", strModtoAdd, switchLineNo));
                return;
            }
            int insertPos = 0;
            if(this.listBoxMod.SelectedItem != null)
            {
                insertPos = this.listBoxMod.SelectedIndex+1;
                modList.Insert(insertPos, strModtoAdd);
            }
            else
            {
                modList.Add(strModtoAdd);
            }
            if(UpdateSwitchLineMods(switchLineNo, modList.ToArray()))
            {
                MessageBox.Show("增加模块成功");
            }
            else
            {
                MessageBox.Show("增加模块失败");
            }
           

        }
        private void btnAddMod_Click(object sender, EventArgs e)
        {
            OnAddMod();
            OnRefreshSwitchlineBufMods();
        }
        private void OnModifyMod()
        {
            DBAccess.BLL.BatteryModuleBll modBll = new DBAccess.BLL.BatteryModuleBll();
            DBAccess.Model.BatteryModuleModel mod = modBll.GetModel(this.textBoxMod.Text);
            if(mod == null)
            {
                MessageBox.Show(string.Format("模块{0}不存在", this.textBoxMod.Text));
                return;
            }
            int switchLineNo = int.Parse(this.cbxSwitchLine.Text);
            string[] modExistArray = GetSwitchLineMods(switchLineNo);
            string modSel = this.listBoxMod.SelectedItem.ToString();
            List<string> modList = new List<string>(modExistArray);

        }
        private void btnModifyMod_Click(object sender, EventArgs e)
        {

        }

        private void cbxSwitchLine_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnRefreshSwitchlineBufMods();
        }

        private void btnParamSave_Click(object sender, EventArgs e)
        {
            foreach(Control ctl in this.flowLayoutPanel2.Controls)
            {
                PLProcessModel.ParamCfgBase.ParamCfgItemCtl paramCtl = ctl as PLProcessModel.ParamCfgBase.ParamCfgItemCtl;
                paramCtl.ParamApply();
            }
            string reStr = "";
            if(!SysCfgModel.SaveCfg(ref reStr))
            {
                MessageBox.Show("保存配置失败:" + reStr);
            }
            else
            {
                MessageBox.Show("参数保存成功");
               
            }
            string str = "";
            SetModPalletNum(ref str);
          
            MessageBox.Show(str);
           
        }
        private bool SetModPalletNum(ref string restr)
        {
            int modPallet = 0;
            foreach (Control ctl in this.flowLayoutPanel2.Controls)
            {
                PLProcessModel.ParamCfgBase.ParamCfgItemCtl paramCtl = ctl as PLProcessModel.ParamCfgBase.ParamCfgItemCtl;
                paramCtl.ParamApply();
                modPallet = int.Parse(paramCtl .GetParamObj().ParamVal);
                break;
            }
            if (this.devList == null)
            {
                return false;
            }

            CtlDevBaseModel screw1 = GetDev("C线1号绝缘板锁螺丝机");
            if (screw1 == null)
            {
                restr = "C线绝缘板锁螺丝机1,设备对象为空！";
                return false;
            }
            bool status1 = screw1.PlcRW.WriteDB("D8500", modPallet);
            if (status1 == false)
            {
                restr = "C线绝缘板锁螺丝机1,发送模块数量失败！";
                return false;
            }

            CtlDevBaseModel screw2 = GetDev("C线2号绝缘板锁螺丝机");
            if (screw2 == null)
            {
                restr = "C线绝缘板锁螺丝机2,设备对象为空！";
                return false;
            }

            bool status2 = screw2.PlcRW.WriteDB("D8500", modPallet);
            if (status2 == false)
            {
                restr = "C线绝缘板锁螺丝机2,发送模块数量失败！";
                return false;
            }
            restr = "C线绝缘板锁螺丝机1、C线绝缘板锁螺丝机2设备模块数量设置成功！";
            return true;
        }
        private CtlDevBaseModel GetDev(string devname)
        {
            if(this.devList == null)
            {
                return null;
            }
            for(int i=0;i<this.devList.Count;i++)
            {
                CtlDevBaseModel dev = this.devList[i];
                if(dev.DevName ==devname)
                {
                    return dev;
                }
            }
            return null;
        }
        private void btnParamCancel_Click(object sender, EventArgs e)
        {
            foreach (Control ctl in this.flowLayoutPanel2.Controls)
            {
                PLProcessModel.ParamCfgBase.ParamCfgItemCtl paramCtl = ctl as PLProcessModel.ParamCfgBase.ParamCfgItemCtl;
                paramCtl.ParamCancel();
            }
        }

      
       
     
       
       
    }
}
