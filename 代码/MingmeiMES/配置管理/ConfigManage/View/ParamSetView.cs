using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SuperTextEdit;
using PLProcessModel;
namespace ConfigManage
{
    public partial class ParamSetView : Form
    {
        private IGrapTB IGrapTB { get; set; }
        private ISuperTextEdit ISuperEdit { get; set; }
        private CtlDevBaseModel CurrDev { get; set; }
        public ParamSetView(IGrapTB grapTb, ISuperTextEdit iSuperTextEdit, CtlDevBaseModel dev)
        {
            InitializeComponent();
            this.IGrapTB = grapTb;
            this.ISuperEdit = iSuperTextEdit;
            this.CurrDev = dev;
        }

        private void ParamSetView_Load(object sender, EventArgs e)
        {
            IniParamCtrl();
        }
        private void IniParamCtrl()
        {
            this.lb_ParamName.Text = this.IGrapTB.Desc;
            Point loction = new Point();
            loction.X = this.lb_ParamName.Location.X + this.lb_ParamName.Size.Width + 10;
            loction.Y= this.lb_ParamName.Location.Y-2;
            this.nud_ParamValue.Location = loction;
           
             
            this.nud_ParamValue.Value =decimal.Parse( this.IGrapTB.Value);
            //if(this.IGrapTB.ValueDataType == DATATYPE.INT)
            //{
                this.nud_ParamValue.DecimalPlaces = 0;
            //}
            //else if(this.IGrapTB.ValueDataType == DATATYPE.DINT)
            //{
            //    this.nud_ParamValue.DecimalPlaces = 0;
            //}
            //else if (this.IGrapTB.ValueDataType == DATATYPE.UINT)
            //{
            //    this.nud_ParamValue.DecimalPlaces = 0;
            //}
            //else if(this.IGrapTB.ValueDataType == DATATYPE.REAL)
            //{
            //    this.nud_ParamValue.DecimalPlaces = 2;
            //}
            //else
            //{
            //    this.nud_ParamValue.DecimalPlaces = 0;
            //}

        }

        private void bt_ParamSet_Click(object sender, EventArgs e)
        {
            if (this.ISuperEdit == null)
            {
                MessageBox.Show("控件句柄为空！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if(this.CurrDev == null)
            {
                MessageBox.Show("当前设备对象为空！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string addr = this.IGrapTB.Name;

            DevInterface.IPlcRWEtd plcEtd = this.CurrDev.PlcRW as DevInterface.IPlcRWEtd;
            int[] intValue = new int[1];
            intValue[0] = (int)this.nud_ParamValue.Value;
            bool writeStatus = plcEtd.WriteMultiDB(addr,1, intValue);//接口可能不对换一下即可
            if(writeStatus == false)
            {
                MessageBox.Show("地址值写入失败！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            this.ISuperEdit.SetValue(this.IGrapTB.Name, this.nud_ParamValue.Value.ToString());
        }

        private void bt_ParamCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
