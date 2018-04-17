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
namespace ConfigManage
{
    public partial class MesOfflineView : BaseChildView
    {
        public MesOfflineView(string captionText)
            : base(captionText)
        {
            InitializeComponent();
            this.Text = captionText;
        }
    }
}
