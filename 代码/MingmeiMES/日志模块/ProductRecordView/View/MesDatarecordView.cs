using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ModuleCrossPnP;
using FTDataAccess.BLL;
using LogInterface;
using DevAccess;
namespace ProductRecordView
{
    public partial class MesDatarecordView : BaseChildView
    {
        #region 数据
        private MesQueryFilter queryFilter = null;
        private LOCAL_MES_STEP_INFOBll localMesStepBll = null;
        private LOCAL_MES_STEP_INFO_DETAILBll localMesDetail = null;
        private MesDA mesDA = null;
        #endregion
        #region 公共接口
        public MesDatarecordView(string captionText)
            : base(captionText)
        {
            InitializeComponent();
            this.Text = captionText;
            queryFilter = new MesQueryFilter();
            localMesStepBll = new LOCAL_MES_STEP_INFOBll();
            localMesDetail = new LOCAL_MES_STEP_INFO_DETAILBll();
            mesDA = new MesDA();
        }
        private delegate void DelegateQueryRecord(string strWhere,string tableName);
        /// <summary>
        /// 异步查询
        /// </summary>
        public void AsyQueryRecord(string logQueryCondition,string tableName)
        {

            RefreshQueryStat("正在查询...");
            DateTime dt1 = System.DateTime.Now;
            DataSet ds = null;
            if (tableName == "基本信息表")
            {
                ds = localMesStepBll.GetList(-1,logQueryCondition,"TRX_TIME");
            }
            else if (tableName == "详细信息表")
            {
                ds = localMesDetail.GetList(-1,logQueryCondition,"TRX_TIME");
            }
            else
            {
                return;
            }
            
            DateTime dt2 = System.DateTime.Now;
            TimeSpan timeCost = dt2 - dt1;
            string strTimespan = string.Format("查询完成，用时：{0}:{1}:{2}.{3}", timeCost.Hours, timeCost.Minutes, timeCost.Seconds, timeCost.Milliseconds);
            if (ds != null && ds.Tables.Count > 0)
            {
                RefreshQueryContent(ds.Tables[0]);
            }

            RefreshQueryStat(strTimespan);
            //this.bindingNavigator1.BindingSource = new BindingSource()
        }

        private delegate void DelegateRefreshDT(DataTable dt);
        public void RefreshQueryContent(DataTable dt)
        {
            if (this.dataGridView1.InvokeRequired)
            {
                DelegateRefreshDT dlgt = new DelegateRefreshDT(RefreshQueryContent);
                this.Invoke(dlgt, new object[] { dt });
            }
            else
            {
                if (dt == null)
                {
                    return;
                }
              
                BindingSource bs = new BindingSource();
                bs.DataSource = dt;
                bindingNavigator1.BindingSource = bs;
                dataGridView1.DataSource = bs;

                this.dataGridView1.Columns["SERIAL_NUMBER"].HeaderText = "主机条码";
                this.dataGridView1.Columns["STEP_NUMBER"].HeaderText = "工位号";
                this.dataGridView1.Columns["STATUS"].HeaderText = "状态";
                this.dataGridView1.Columns["TRX_TIME"].HeaderText = "数据上传时间";
                this.dataGridView1.Columns["TRX_TIME"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
                this.dataGridView1.Columns["LAST_MODIFY_TIME"].HeaderText = "最后一次修改时间";
                this.dataGridView1.Columns["LAST_MODIFY_TIME"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
                this.dataGridView1.Columns["RECID"].HeaderText = "记录ID";
                this.dataGridView1.Columns["UPLOAD_FLAG"].HeaderText = "上传状态";
                this.dataGridView1.Columns["STATUS"].Width = 50;
                this.dataGridView1.Columns["AutoStationName"].Width = 100;
                this.dataGridView1.Columns["AutoStationName"].HeaderText = "产线工位名称";
                this.dataGridView1.Columns["SERIAL_NUMBER"].Width = 150;
                this.dataGridView1.Columns["TRX_TIME"].Width = 150;
                this.dataGridView1.Columns["LAST_MODIFY_TIME"].Width = 150;
                if (this.comboBox1.Text == "基本信息表")
                {
                    this.dataGridView1.Columns["CHECK_RESULT"].HeaderText = "检测结果";
                    this.dataGridView1.Columns["STEP_MARK"].HeaderText = "工位标识(检测/下线)";
                    this.dataGridView1.Columns["USER_NAME"].HeaderText = "操作者";
                    this.dataGridView1.Columns["DEFECT_CODES"].HeaderText = "不合格项代码";
                    this.dataGridView1.Columns["RECID"].HeaderText = "记录ID";
                    this.dataGridView1.Columns["USER_NAME"].HeaderText = "操作者";
                    this.dataGridView1.Columns["REASON"].HeaderText = "原因说明";
                    this.dataGridView1.Columns["UPLOAD_FLAG"].HeaderText = "上传状态";
                    this.dataGridView1.Columns["REASON"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    this.dataGridView1.Columns["CHECK_RESULT"].Width = 50;
                    this.dataGridView1.Columns["STEP_MARK"].Width = 50;
                    this.dataGridView1.Columns["CHECK_RESULT"].Width = 50;

                }
                else
                {
                    this.dataGridView1.Columns["DATA_NAME"].HeaderText = "数据名称";
                    this.dataGridView1.Columns["DATA_VALUE"].HeaderText = "检测数值";
                    this.dataGridView1.Columns["DATA_NAME"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
                
                //bs.Sort = "TRX_TIME";
                //this.dataGridView1.Columns[0].Width = 100;
                //this.dataGridView1.Columns[1].Width = 200;
                //this.dataGridView1.Columns[2].Width = 200;
                //this.dataGridView1.Columns[3].Width = 200;
                //this.dataGridView1.Columns[4].Width = 80;
                //this.dataGridView1.Columns[2].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
                //this.dataGridView1.Columns[3].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
                //this.dataGridView1.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }


        }
        #endregion
      
        #region UI事件
        private void MesDatarecordView_Load(object sender, EventArgs e)
        {
            this.comboBox1.Items.AddRange(new string[] { "基本信息表", "详细信息表" });
            this.comboBox1.SelectedIndex = 0;
            //过滤参数绑定
            this.dateTimePicker1.DataBindings.Add("Value", this.queryFilter, "StartDate");
            this.dateTimePicker2.DataBindings.Add("Value", this.queryFilter, "EndDate");

        }
        private void buttonRecord_Click(object sender, EventArgs e)
        {
            OnRecordQuery();
        }
    
        private void OnRecordQuery()
        {
            StringBuilder strWhere = new StringBuilder();
            strWhere.AppendFormat("TRX_TIME between '{0}' and '{1}' ",
            queryFilter.StartDate.ToString("yyyy-MM-dd 0:00:00"),
                queryFilter.EndDate.ToString("yyyy-MM-dd 0:00:00"));
            if(this.checkBarcode.Checked)
            {
                strWhere.AppendFormat(" and SERIAL_NUMBER='{0}'", this.textBoxBarcode.Text);
            }
            string logQueryCondition = strWhere.ToString();
            DelegateQueryRecord dlgt = new DelegateQueryRecord(AsyQueryRecord);
            IAsyncResult ar = dlgt.BeginInvoke(logQueryCondition,this.comboBox1.Text, null, dlgt);

            //AsyQueryRecord(logQueryCondition);

        }

        private void btnOrderTime_Click(object sender, EventArgs e)
        {
            if (bindingNavigator1.BindingSource != null)
            {
                bindingNavigator1.BindingSource.Sort = "TRX_TIME";
            }
            
        }

        private void btnOrderByStep_Click(object sender, EventArgs e)
        {
            if (bindingNavigator1.BindingSource != null)
            {
                bindingNavigator1.BindingSource.Sort = "STEP_NUMBER";
            }
            
        }

        private void btnOrderBarcode_Click(object sender, EventArgs e)
        {
            if (bindingNavigator1.BindingSource != null)
            {
                bindingNavigator1.BindingSource.Sort = "SERIAL_NUMBER";
            }
            
        }

        private void btnExportRecord_Click(object sender, EventArgs e)
        {
            BindingSource bs = this.dataGridView1.DataSource as BindingSource;
            if (bs == null)
            {
                MessageBox.Show("当前数据为空!");
                return;
            }
            DataTable dt = (DataTable)bs.DataSource;
            ExportDtToExcel(dt, null,"MES上传记录");
        }
        private void OnMesDown()
        {
            if(string.IsNullOrWhiteSpace(this.textBoxBarcode2.Text))
            {
                return;
            }
            this.richTextBoxMesRe.Text = "";
            string reStr = "";
            int mesDown = mesDA.MesAssemDown(new string[] { this.textBoxBarcode2.Text, "L44" }, ref reStr);
            if (mesDown == 1)
            {
                string logInfo = string.Format("{0} 禁止下线，{1}", this.textBoxBarcode2.Text,reStr);
              
                this.richTextBoxMesRe.Text = logInfo;
            }
            else if (mesDown == 0)
            {
                string logInfo = string.Format("{0}允许下线", this.textBoxBarcode2.Text);
                this.richTextBoxMesRe.Text = logInfo;
            }
            else
            {
                this.richTextBoxMesRe.Text = "查询失败";
            }
        }
        private void buttonMesDownQuery_Click(object sender, EventArgs e)
        {
            OnMesDown();
        }
        private void OnMesData()
        {
            if(string.IsNullOrWhiteSpace(this.textBoxBarcode2.Text))
            {
                return;
            }
            string strSql=string.Format("select * from FT_MES_STEP_INFO where SERIAL_NUMBER='{0}' order by TRX_TIME desc",this.textBoxBarcode2.Text);
            DataTable dt = mesDA.ReadMesTable(strSql);
            this.dataGridView2.DataSource = dt;

            this.dataGridView1.Columns["SERIAL_NUMBER"].HeaderText = "主机条码";
            this.dataGridView1.Columns["STEP_NUMBER"].HeaderText = "工位号";
            this.dataGridView1.Columns["STATUS"].HeaderText = "状态";
            this.dataGridView1.Columns["TRX_TIME"].HeaderText = "数据上传时间";
            this.dataGridView1.Columns["TRX_TIME"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
            this.dataGridView1.Columns["LAST_MODIFY_TIME"].HeaderText = "最后一次修改时间";
            this.dataGridView1.Columns["LAST_MODIFY_TIME"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
           
            
            
            this.dataGridView1.Columns["CHECK_RESULT"].HeaderText = "检测结果";
            this.dataGridView1.Columns["STEP_MARK"].HeaderText = "工位标识(检测/下线)";
            this.dataGridView1.Columns["DEFECT_CODES"].HeaderText = "不合格项代码";
            this.dataGridView1.Columns["REASON"].HeaderText = "原因说明";

            this.dataGridView1.Columns["USER_NAME"].Visible = false;
            this.dataGridView1.Columns["RECID"].Visible=false;

            this.dataGridView1.Columns["STATUS"].Width = 50;
            this.dataGridView1.Columns["SERIAL_NUMBER"].Width = 150;
            this.dataGridView1.Columns["TRX_TIME"].Width = 150;
            this.dataGridView1.Columns["LAST_MODIFY_TIME"].Width = 150;
            this.dataGridView1.Columns["REASON"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView1.Columns["CHECK_RESULT"].Width = 50;
            this.dataGridView1.Columns["STEP_MARK"].Width = 50;
            this.dataGridView1.Columns["CHECK_RESULT"].Width = 50;
        }
        private void buttonMesData_Click(object sender, EventArgs e)
        {
            OnMesData();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.textBoxBarcode2.Text = "";
            this.richTextBoxMesRe.Text = "";
            this.dataGridView2.DataSource = null;
        }


        #endregion
        #region 私有方法
        private delegate void DelegateRefreshStat(string stat);
        private void RefreshQueryStat(string stat)
        {
            if (this.bindingNavigator1.InvokeRequired)
            {
                DelegateRefreshStat dlgt = new DelegateRefreshStat(RefreshQueryStat);
                this.Invoke(dlgt, new object[] { stat });
            }
            else
            {
                this.labelQueryStat.Text = stat;
            }

        }
        #endregion

      


       
    }
}
