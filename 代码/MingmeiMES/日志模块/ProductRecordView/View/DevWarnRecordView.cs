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
using DBAccess.BLL;
using DBAccess.Model;
using PLProcessModel;
namespace ProductRecordView
{
    public partial class DevWarnRecordView : BaseChildView
    {
        DevWarnRecordBll devWarnBll = new DevWarnRecordBll();
        DevWarnrcdViewBll devWarnviewBll = new DevWarnrcdViewBll();
        private delegate void SetWarnningStrDelegate(string warnstr);
      
        ThreadBaseModel devWarnMonitorThread = null;
    
        private List<CtlDevBaseModel> devList = new List<CtlDevBaseModel>();
        private string currDevName = "";
        public DevWarnRecordView(string captionText)
            : base(captionText)
        {
            InitializeComponent();
            this.Text = captionText;
        }
        private void DevWarnRecordView_Load(object sender, EventArgs e)
        {
            this.dateTimePicker1.Value = DateTime.Now - (new TimeSpan(30, 0, 0, 0));
            this.dateTimePicker2.Value = DateTime.Now + (new TimeSpan(1, 0, 0, 0));
            IniWarnningRoll();
            IniDevWarnThread();
        }
        public void SetDevList(List<CtlDevBaseModel> devList)
        {
            if(devList == null)
            {
                return;
            }
            this.devList = devList;
            this.cbxDevName.Items.Clear();
            this.cbxDevName.Items.Add("所有");
            for (int i = 0; i < devList.Count;i++ )
            {
                this.cbxDevName.Items.Add(devList[i].DevName);
            }
               
            if(this.cbxDevName.Items.Count>0)
            {
                this.cbxDevName.SelectedIndex = 0;
                this.currDevName = "所有";
                
            }
        }

        private void IniWarnningRoll()
        {
            this.warningRoll1.SetFontSize(30);
            this.warningRoll1.StartRoll();
        }
        /// <summary>
        /// 初始化报警监控线程
        /// </summary>
        private void IniDevWarnThread()
        {
            string reStr = "";
            devWarnMonitorThread = new ThreadBaseModel(12, "设备报警数据采集线程");
            devWarnMonitorThread.LoopInterval = 1000;
            devWarnMonitorThread.SetThreadRoutine(DevMonitorLoop);
            devWarnMonitorThread.TaskInit(ref reStr);
            devWarnMonitorThread.TaskStart(ref reStr);
        }
        /// <summary>
        /// 监控线程函数
        /// </summary>
        private void DevMonitorLoop()
        {
            string warnningStr = GetWarnningStr();


            SetWarnningStr(warnningStr);
        }
        private string GetWarnningStr()
        {
            StringBuilder sb = new StringBuilder();

            if (this.currDevName == "所有")
            {
                string warnStr = GetAllDevWarnningStr();
                sb.Append(warnStr);
            }
            else
            { 
                CtlDevBaseModel currDev = GetDev(this.devList, this.currDevName);
                string warnStr = GetSelectDevWarnningStr(currDev);
                sb.Append(warnStr);
            }
            return sb.ToString();
        }
        private string GetAllDevWarnningStr()
        {
            StringBuilder warnStr = new StringBuilder();
            foreach (CtlDevBaseModel dev in this.devList)
            {
                foreach (DevWarnItemModel warn in dev.devWarnList.Values)
                {
                    if (warn == null)
                    {
                        continue;
                    }
                    if (warn.WarnStat == 0)
                    {
                        continue;
                    }
                    warnStr.Append("[" + warn.WarnInfo + "]");
                }
            }
          
            return warnStr.ToString();
        }
        private string GetSelectDevWarnningStr(CtlDevBaseModel dev)
        {
            StringBuilder warnStr = new StringBuilder();
            foreach (DevWarnItemModel warn in dev.devWarnList.Values)
            {
                if (warn == null)
                {
                    continue;
                }
                if (warn.WarnStat == 0)
                {
                    continue;
                }
                warnStr.Append("[" + warn.WarnInfo + "]");
            }
            return warnStr.ToString();
        }
        /// <summary>
        /// 设置报警函数
        /// </summary>
        /// <param name="warnning"></param>
        private void SetWarnningStr(string warnning)
        {
            if (this.warningRoll1.InvokeRequired)
            {
                SetWarnningStrDelegate swsd = new SetWarnningStrDelegate(SetWarnningStr);
                this.warningRoll1.Invoke(swsd, new object[1] { warnning });
            }
            else
            {
                this.warningRoll1.SetWarnningStr(warnning);
            }
        }
        private void OnDevWarnQuery()
        {
            DateTime dtSt = this.dateTimePicker1.Value;
            DateTime dtEnd = this.dateTimePicker2.Value + (new TimeSpan(24, 0, 0));
            string devName = this.cbxDevName.Text;
            string strWhere = string.Format(@"(recordTime between '{0}' and '{1}')", dtSt.ToString("yyyy-MM-dd"), dtEnd.ToString("yyyy-MM-dd"));
            //string strWhere = string.Format(@"(recordTime between '{0}' and '{1}') and (devName='{2}') ",dtSt.ToString("yyyy-MM-dd"),dtEnd.ToString("yyyy-MM-dd"),devName);
            if(this.cbxDevName.Text  != "所有")
            {
                strWhere += string.Format("and (devName='{0}') ", devName);
            }
            DataSet ds = devWarnviewBll.GetList(strWhere);
            this.dataGridView1.DataSource = ds.Tables[0];
            this.dataGridView1.Columns["recordTime"].HeaderText = "记录时间";
            this.dataGridView1.Columns["recordTime"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
            this.dataGridView1.Columns["recordTime"].Width = 150;
            this.dataGridView1.Columns["warnStat"].HeaderText = "报警状态(0:复位,1: 报警)";
            this.dataGridView1.Columns["devName"].HeaderText = "设备名称";
            this.dataGridView1.Columns["devName"].Width = 200;
            this.dataGridView1.Columns["warnInfo"].HeaderText = "状态描述";
            this.dataGridView1.Columns["warnInfo"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }
        private void btnDevWarnQuery_Click(object sender, EventArgs e)
        {
            OnDevWarnQuery();
        }

        private void cbxDevName_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.currDevName = this.cbxDevName.Text.Trim();
           
        }

        private CtlDevBaseModel GetDev(List<CtlDevBaseModel> devList,string devName)
        {
            if(devList == null)
            {
                return null;
            }
            for(int i=0;i<devList.Count;i++)
            {
                if(devList[i].DevName == devName)
                {
                    return devList[i];
                }
            }
            return null;
        }


    }
}
