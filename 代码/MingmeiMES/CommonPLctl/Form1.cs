using DBAccess.BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CommonPL
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BatteryModuleBll modBll = new BatteryModuleBll();
            DBAccess.Model.BatteryModuleModel model = modBll.GetModelByPalletIDAndTag2("AA080000", "2", "A线模组-工装板绑定");
            if(model != null)
            {
                MessageBox.Show("model != null");
            }
        }
    }
}
