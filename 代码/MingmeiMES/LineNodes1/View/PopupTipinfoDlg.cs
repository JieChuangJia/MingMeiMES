using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LineNodes
{
    public partial class PopupTipinfoDlg : Form
    {
        //private string dispInfo = "工位信息";
        public PopupTipinfoDlg()
        {
            InitializeComponent();
        }
        public void RefreshDisp(string info)
        {
           this.richTextBox1.Text = info;
        }
    }
}
