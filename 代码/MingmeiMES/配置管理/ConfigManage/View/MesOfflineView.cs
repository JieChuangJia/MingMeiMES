using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ModuleCrossPnP;
using FTDataAccess;
using LogInterface;
using FTDataAccess.Model;
using FTDataAccess.BLL;
using DBAccess.BLL;
using DBAccess.Model;
using FTDataAccess.Model;
using PLProcessModel;
namespace ConfigManage
{
   
    public partial class MesOfflineView : BaseChildView
    {
        OfflineDataBLL bllOfflineData = new OfflineDataBLL();
        QRCodeBLL bllQrCode = new QRCodeBLL();
        public MesOfflineView(string captionText)
            : base(captionText)
        {
            InitializeComponent();
            this.Text = captionText;
        }
        private void MesOfflineView_Load(object sender, EventArgs e)
        {
            if(SysCfgModel.MesOfflineMode == true)
            {
                this.rb_OfflineMode.Checked = true;
            }
            else
            {
                this.rb_OnlineMode.Checked = true;
            }
            this.dtp_StartDate.Value.AddDays(-5);
            this.cb_OfflineDataStatus.SelectedIndex = 0;
            this.cb_QrStatus.SelectedIndex = 0;
            this.cb_QrType.SelectedIndex = 0;
        }

        private void rb_OnlineMode_MouseDown(object sender, MouseEventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("您确认要切换模式吗？", "信息提示", MessageBoxButtons.YesNo))
            {
                this.rb_OnlineMode.Checked = true;
            }
        }
        private void rb_OfflineMode_MouseDown(object sender, MouseEventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("您确认要切换模式吗？", "信息提示", MessageBoxButtons.YesNo))
            {
                this.rb_OfflineMode.Checked = true;
            }
        } 

        private void bt_AddQR_Click(object sender, EventArgs e)
        {
            if(this.tb_QrCode.Text.Trim().Length!=24)
            {
                MessageBox.Show("此二维码条码长度错误！长度为24！");
                return;
            }
            AddQrCodeData(this.tb_QrCode.Text.Trim(), this.cb_QrType.Text.Trim());
        }

        private void AddQrCodeData(string qrCode,string codeType)
        {
            QRCodeModel qrTemp = bllQrCode.GetModel(qrCode);
            if(qrTemp != null)
            {
                MessageBox.Show("此二维码数据表中已经存在！");
                return;
            }

            QRCodeModel qrCodeModel = new QRCodeModel();
            qrCodeModel.PintStatus = "待打印";
            qrCodeModel.QRCode = qrCode;
            qrCodeModel.QRType = codeType;
            bllQrCode.Add(qrCodeModel);
            RefreshQrCode();
        }

        private void bt_QueryOffline_Click(object sender, EventArgs e)
        {
            RefreshOfflineData();
        }
        private void RefreshOfflineData()
        {
            DataTable dt = bllOfflineData.GetOfflineDataByStatus(this.dtp_StartDate.Value, this.dtp_EndDate.Value, this.cb_OfflineDataStatus.Text.Trim());
            this.dgv_OfflineData.DataSource = dt;
        }
        private void bt_QueryQR_Click(object sender, EventArgs e)
        {
            RefreshQrCode();
        }
        private void RefreshQrCode()
        {
            DataTable dt = bllQrCode.GetQrCodeData(this.cb_QrType.Text.Trim(), this.cb_QrStatus.Text.Trim());
            this.dgv_CodeList.DataSource = dt;
        }
        private void bt_DeleteQR_Click(object sender, EventArgs e)
        {
            if(this.dgv_CodeList.CurrentRow==null)
            {
                return;
            }
           
            string qrCode = this.dgv_CodeList.CurrentRow.Cells["QRCode"].Value.ToString();
            bllQrCode.Delete(qrCode);

            RefreshQrCode();
        }

        private void bt_ChangeMode_Click(object sender, EventArgs e)
        {
            if (this.rb_OnlineMode.Checked == true)
            {
                string restr = "";
                SysCfgModel.MesOfflineMode = false;
                SysCfgModel.SaveCfg(ref  restr);
                if (DialogResult.Yes == MessageBox.Show("您是否要上传MES离线数据？", "信息提示", MessageBoxButtons.YesNo))
                {
                    LineNodes.LineMonitorPresenter.uploadOfflineDataToMesSwitch = true;
                }
                else//更新数据为用户拒绝上传
                {
                    bllOfflineData.UpdateDataByStatus(EnumUploadStatus.待上传.ToString(), EnumUploadStatus.用户拒绝上传.ToString());
                }
            }
            else
            { }
        }
     
    }
}
