using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Data.SqlClient;
using Excel = Microsoft.Office.Interop.Excel;
using ModuleCrossPnP;
using FTDataAccess.BLL;
using LogInterface;
namespace ProductRecordView
{
    public partial class OnlineProductView : BaseChildView
    {
        #region 数据
        private ProductRecordQueryFilter queryFilter = null;
        private OnlineProductsBll recordBll = null;
        #endregion
        public OnlineProductView(string captionText)
            : base(captionText)
        {
            InitializeComponent();
            queryFilter = new ProductRecordQueryFilter();
            this.Text = captionText;
          //  this.captionText = captionText;
            recordBll = new OnlineProductsBll();
            
        }
        private delegate void DelegateQueryRecord(string strWhere);
        /// <summary>
        /// 异步查询
        /// </summary>
        public void AsyQueryRecord(string logQueryCondition)
        {
            
            RefreshQueryStat("正在查询...");
            DateTime dt1 = System.DateTime.Now;
            DataSet ds = recordBll.GetList(logQueryCondition);
            DateTime dt2 = System.DateTime.Now;
            TimeSpan timeCost = dt2 - dt1;
            string strTimespan = string.Format("查询完成，用时：{0}:{1}:{2}.{3}", timeCost.Hours, timeCost.Minutes, timeCost.Seconds, timeCost.Milliseconds);
            if(ds != null && ds.Tables.Count>0)
            {
                RefreshQueryContent(ds.Tables[0]);
            }
           
            RefreshQueryStat(strTimespan);
            //this.bindingNavigator1.BindingSource = new BindingSource()
        }

        private delegate void DelegateRefreshDT(DataTable dt);
        public void RefreshQueryContent(DataTable dt)
        {
            if(this.dataGridView1.InvokeRequired)
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
                bs.Sort = "inputTime";


                this.dataGridView1.Columns[0].Width = 300;
                this.dataGridView1.Columns[1].Width = 200;
                this.dataGridView1.Columns[2].Width = 200;
                this.dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                this.dataGridView1.Columns[3].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
                //this.dataGridView1.Columns[4].Width = 100;
                //this.dataGridView1.Columns[2].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
                //this.dataGridView1.Columns[3].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
                //this.dataGridView1.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                // strSql.Append("select produceRecordID as 记录序号,productBarcode as 主机条码,inputTime as 投产时间,outputTime as 下线时间,lineOuted as 是否已经下线,outputNode 下线工位");
                this.dataGridView1.Columns["productBarcode"].HeaderText = "主机条码";
                this.dataGridView1.Columns["rfidCode"].HeaderText = "RFID";
                this.dataGridView1.Columns["currentNode"].HeaderText = "当前工位";
                this.dataGridView1.Columns["inputTime"].HeaderText = "投产时间";
                
                //this.dataGridView1.Columns["lineOuted"].HeaderText = "是否已经下线";
                //this.dataGridView1.Columns["outputNode"].HeaderText = "下线工位";
            }
           
            
        }
        #region UI事件
        private void ProduceReccordView_Load(object sender, EventArgs e)
        {
            //过滤参数绑定
            this.dateTimePicker1.DataBindings.Add("Value",this.queryFilter, "StartDate");
            this.dateTimePicker2.DataBindings.Add("Value", this.queryFilter, "EndDate");
        }
        private void OnRecordQuery()
        {
            StringBuilder strWhere = new StringBuilder();
            strWhere.AppendFormat("inputTime between '{0}' and '{1}' ",
            queryFilter.StartDate.ToString("yyyy-MM-dd 0:00:00"),
                queryFilter.EndDate.ToString("yyyy-MM-dd 0:00:00"));
            string logQueryCondition = strWhere.ToString();
            DelegateQueryRecord dlgt = new DelegateQueryRecord(AsyQueryRecord);
            IAsyncResult ar = dlgt.BeginInvoke(logQueryCondition, null, dlgt);

            //AsyQueryRecord(logQueryCondition);

        }
       
        private void buttonRecord_Click(object sender, EventArgs e)
        {
            OnRecordQuery();
        }

        private void btnOrderTime_Click(object sender, EventArgs e)
        {
            if (bindingNavigator1.BindingSource != null)
            {
                bindingNavigator1.BindingSource.Sort = "inputTime";
            }
           

        }

        private void btnOrderStep_Click(object sender, EventArgs e)
        {
            if (bindingNavigator1.BindingSource != null)
            {
                bindingNavigator1.BindingSource.Sort = "outputNode";
            }
           
        }

        private void btnProduct_Click(object sender, EventArgs e)
        {
            if (bindingNavigator1.BindingSource != null)
            {
                bindingNavigator1.BindingSource.Sort = "productBarcode";
            }
            
        }

        //private void btnExportRecord_Click(object sender, EventArgs e)
        //{
        //    BindingSource bs = this.dataGridView1.DataSource as BindingSource;
        //    if(bs == null)
        //    {
        //        MessageBox.Show("当前数据为空!");
        //        return;
        //    }
        //    DataTable dt = (DataTable)bs.DataSource;
        //    if(dt != null)
        //    {
        //        OnExportLog(dt);
        //    }
            
        //}

        #endregion
        #region 私有方法
        private delegate void DelegateRefreshStat(string stat);
        private void RefreshQueryStat(string stat)
        {
            if(this.bindingNavigator1.InvokeRequired)
            {
                DelegateRefreshStat dlgt = new DelegateRefreshStat(RefreshQueryStat);
                this.Invoke(dlgt, new object[] {stat });
            }
            else
            {
                this.labelQueryStat.Text = stat;
            }
           
        }

        //private void OnExportLog(DataTable dt)
        //{
        //    SaveFileDialog dlg = new SaveFileDialog();
        //    dlg.Filter = "excel files (*.xlsx)|*.xlsx";
        //    if (dlg.ShowDialog() == DialogResult.OK)
        //    {
        //        string fileName = dlg.FileName;
        //        string sheetName = "Log";
                
        //        string[] columnNames=new string[]{"记录序号","主机条码","投产时间","下线时间","是否已经下线","下线工位"};
        //        CreateExcelFile(fileName, sheetName,columnNames);
        //        //string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" +
        //        //"Data Source=" + fileName + ";" +
        //        //"Extended Properties='Excel 8.0; HDR=Yes; IMEX=2'";
        //        string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" +
        //        "Data Source=" + fileName + ";" +
        //        "Extended Properties='Excel 12.0; HDR=Yes; IMEX=0'";
        //        using (OleDbConnection ole_conn = new OleDbConnection(strConn))
        //        {
        //            ole_conn.Open();
        //            using (OleDbCommand ole_cmd = ole_conn.CreateCommand())
        //            {
        //                foreach (DataRow dr in dt.Rows)
        //                {
                           
        //                    //string logContent = dr["内容"].ToString();
        //                    //logContent = logContent.Substring(0, Math.Min(255, logContent.Count()));
        //                    string downlineStr = "否";
        //                    if(dr["lineOuted"].ToString()=="1")
        //                    {
        //                        downlineStr = "是";
        //                    }
        //                    ole_cmd.CommandText = string.Format("insert into [{0}$](记录序号,主机条码,投产时间,下线时间,是否已经下线,下线工位)values('{1}','{2}','{3}','{4}','{5}','{6}')", sheetName,
        //                        dr["produceRecordID"].ToString(), dr["productBarcode"], dr["inputTime"].ToString(), dr["outputTime"].ToString(), downlineStr, dr["outputNode"].ToString());
        //                    ole_cmd.ExecuteNonQuery();
        //                }

        //                MessageBox.Show("数据导出成功......");
        //            }
        //        }
        //    }
        //}
        /// <summary>  
        /// If the supplied excel File does not exist then Create it  
        /// </summary>  
        /// <param name="FileName"></param>  
        //private void CreateExcelFile(string FileName, string sheetName,string[] columnNames)
        //{
        //    //create  
        //    object Nothing = System.Reflection.Missing.Value;
        //    var app = new Excel.Application();
        //    app.Visible = false;
        //    Excel.Workbook workBook = app.Workbooks.Add(Nothing);
        //    Excel.Worksheet worksheet = (Excel.Worksheet)workBook.Sheets[1];
        //    worksheet.Name = sheetName;
        //    ////headline  
        //    for (int i = 0; i < columnNames.Count();i++ )
        //    {
        //        worksheet.Cells[1, i + 1] = columnNames[i];
        //    }
        //    //worksheet.Cells[1, 1] = "日志ID";
        //    //worksheet.Cells[1, 2] = "内容";
        //    //worksheet.Cells[1, 3] = "类别";
        //    //worksheet.Cells[1, 4] = "日志来源";
        //    //worksheet.Cells[1, 5] = "时间";

        //    worksheet.SaveAs(FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing);
        //    workBook.Close(false, Type.Missing, Type.Missing);
        //    app.Quit();
        //}  
        #endregion


    }
}
