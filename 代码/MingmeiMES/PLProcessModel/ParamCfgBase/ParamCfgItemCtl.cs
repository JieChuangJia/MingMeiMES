using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PLProcessModel.ParamCfgBase
{
    public partial class ParamCfgItemCtl : UserControl
    {
        public string itemName = "配置项名称";
        public string itemVal = "";
        private ParamModel paramObj = new ParamModel("参数", "", "VAR_INT");
      
        public ParamCfgItemCtl()
        {
            InitializeComponent();
            this.label1.Text = "参数";
            this.textBox1.Text = "";
          
        }
        public void SetParmObj(ParamModel paramModel)
        {
            paramObj = paramModel;
            this.label1.Text = paramObj.ParamName; 
            this.textBox1.Text = paramObj.ParamVal;
            this.textBox1.Left = this.label1.Right + 5;
        }
        public ParamModel GetParamObj()
        {
            return paramObj;
        }
        private void ParamCfgItemCtl_Load(object sender, EventArgs e)
        {
           
        }
        public void ParamApply()
        {
            paramObj.ParamVal = this.textBox1.Text;
        }
        public void ParamCancel()
        {
            this.textBox1.Text = paramObj.ParamVal;
        }
    }
}
