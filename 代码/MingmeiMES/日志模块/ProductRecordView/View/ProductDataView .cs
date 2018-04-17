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
using MesDBAccess.BLL;
using MesDBAccess.Model;
using LogInterface;
using DevAccess;
using DevInterface;
namespace ProductRecordView
{
    public partial class ProductDataView : BaseChildView
    {
        #region 数据
        //private ProductQueryFilter queryFilter = null;
        //private ProductOnlineBll productBll = null;


        //private LogManage.WaitDlg waitDlg = null;
       // public IHKAccess HkAccess { get; set; }
        //private BatteryBll batBll = new BatteryBll();
         private DBAccess.BLL.BatteryModuleBll batModBll = new DBAccess.BLL.BatteryModuleBll();
        //private BatteryPackBll batPackBll = new BatteryPackBll();
     //   private ProduceRecordBll recordBll = null;
        #endregion
        public ProductDataView(string captionText)
            : base(captionText)
        {
            InitializeComponent();
           // queryFilter = new ProductQueryFilter();
            this.Text = captionText;
          //  this.captionText = captionText;
          //  recordBll = new ProduceRecordBll();
          //  productBll = new ProductOnlineBll();
            //HkAccess = new HKAccess(2, "192.168.126.30", 13535);
        }
        private delegate void DelegateQueryRecord(string strWhere);
        /// <summary>
        /// 异步查询
        /// </summary>
        public void AsyQueryRecord(string logQueryCondition)
        {
            
            //RefreshQueryStat("正在查询...");
            //DateTime dt1 = System.DateTime.Now;
            //DataSet ds = recordBll.GetList(logQueryCondition);
            //DateTime dt2 = System.DateTime.Now;
            //TimeSpan timeCost = dt2 - dt1;
            //string strTimespan = string.Format("查询完成，用时：{0}:{1}:{2}.{3}", timeCost.Hours, timeCost.Minutes, timeCost.Seconds, timeCost.Milliseconds);
            //if(ds != null && ds.Tables.Count>0)
            //{
            //    RefreshQueryContent(ds.Tables[0]);
            //}
           
            //RefreshQueryStat(strTimespan);
            //this.bindingNavigator1.BindingSource = new BindingSource()
        }

        public void SetOpStations(string[] stationNames)
        {
            this.comboBoxStation.Items.AddRange(stationNames);
            if(this.comboBoxStation.Items.Count>0)
            {
                this.comboBoxStation.SelectedIndex = 0;
            }
        }
        #region UI事件

        private void ProductDataView_Load(object sender, EventArgs e)
        {
            this.dateTimePicker1.Value = DateTime.Now - (new TimeSpan(30, 0, 0, 0));
            this.dateTimePicker2.Value = DateTime.Now + (new TimeSpan(1, 0, 0, 0));
            this.comboBoxBindStat.Items.AddRange(new string[] { "在线", "已下线" });
            this.comboBoxBindStat.SelectedIndex = 0;

        }
        private void buttonQuery_Click(object sender, EventArgs e)
        {
            OnQueryBattery();
        }
        private void OnQueryBattery()
        {
            try
            {
                string strWhere = string.Format(" (asmTime between '{0}' and '{1}')", this.dateTimePicker1.Value.ToString("yyyy-MM-dd 0:00:00"), this.dateTimePicker2.Value.ToString("yyyy-MM-dd 0:00:00"));
                if(this.checkBoxPack.Checked)
                {
                    strWhere += string.Format(" and batPackID LIKE '%{0}%'", this.textBoxPack.Text);
                }
                if(this.checkboxMod.Checked)
                {
                    strWhere += string.Format(" and batModuleID LIKE '%{0}%'", this.textboxMod.Text);
                }
                if(this.checkboxPallet.Checked)
                {
                    strWhere += string.Format(" and palletID='{0}'", this.textboxPallet.Text);
                }
                if(this.checkBoxStation.Checked)
                {
                    strWhere += string.Format(" and curProcessStage='{0}'", this.comboBoxStation.Text);
                }
                if(this.checkBoxBindStat.Checked)
                {
                    if(this.comboBoxBindStat.Text == "在线")
                    {
                        strWhere += "and palletBinded=1";
                    }
                    else
                    {
                        strWhere += "and palletBinded=0";
                    }
                }
                DataSet ds = this.batModBll.GetList(strWhere);
                DataTable dt = ds.Tables[0];
                this.dataGridView2.DataSource = dt;
                this.dataGridView2.Columns["batchName"].Visible = false;
                this.dataGridView2.Columns["topcapOPWorkerID"].Visible = false;
                this.dataGridView2.Columns["downcapOPWorkerID"].Visible = false;
                this.dataGridView2.Columns["topcapWelderID"].Visible = false;
                this.dataGridView2.Columns["bottomcapWelderID"].Visible = false;
                this.dataGridView2.Columns["tag3"].Visible = false;
                this.dataGridView2.Columns["tag4"].Visible = false;
                this.dataGridView2.Columns["tag5"].Visible = false;
                this.dataGridView2.Columns["asmTime"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";

                this.dataGridView2.Columns["batModuleID"].HeaderText = "模块条码";
                this.dataGridView2.Columns["asmTime"].HeaderText = "上线时间";
                this.dataGridView2.Columns["curProcessStage"].HeaderText = "当前工位";
                this.dataGridView2.Columns["batPackID"].HeaderText = "PACK条码";
                this.dataGridView2.Columns["palletID"].HeaderText = "托盘条码";
                this.dataGridView2.Columns["palletBinded"].HeaderText = "托盘绑定";
                this.dataGridView2.Columns["checkResult"].HeaderText = "检测结果（1:OK,2:NG)";
                this.dataGridView2.Columns["tag1"].HeaderText = "档位";
                this.dataGridView2.Columns["tag2"].HeaderText = "托盘内位置编号";
                this.dataGridView2.Columns["batModuleID"].Width = 200;
                this.dataGridView2.Columns["asmTime"].Width = 150;
                this.dataGridView2.Columns["curProcessStage"].Width = 150;
                this.label3.Text = string.Format("合计：{0}", dt.Rows.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void buttonPalletQuery_Click(object sender, EventArgs e)
        {
            OnQueryPallet();
        }
        private void OnQueryPallet()
        {
            try
            {
                string strWhere = string.Format("palletID != '' and (asmTime between '{0}' and '{1}') and palletBinded=1", this.dateTimePicker1.Value.ToString("yyyy-MM-dd 0:00:00"), this.dateTimePicker2.Value.ToString("yyyy-MM-dd 0:00:00"));
                DataSet ds = this.batModBll.GetList(strWhere);
                DataTable dt = ds.Tables[0];
                this.dataGridView1.DataSource = dt;
                foreach (DataGridViewColumn col in this.dataGridView1.Columns)
                {
                    col.Visible = false;
                }
                this.dataGridView1.Columns["palletID"].Visible = true;
                this.dataGridView1.Columns["palletID"].HeaderText = "托盘号";
                this.label1.Text = "合计:" + dt.Rows.Count.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
           
           
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
        #endregion

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
           
        }
     
        private void dataGridViewBatterys_BL_CellClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
           if(e.RowIndex<0 || e.ColumnIndex<0)
           {
               return;
           }
            if (this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex] != null)
            {
                string str = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                this.textboxPallet.Text = str;
            }
           
        }
        private void btnExportExcel_Click(object sender, EventArgs e)
        {
           // OnExportData();
        }
        //private void OnExportData()
        //{
        //    List<DataTable> dts = new List<DataTable>();
        //    List<string> sheetNames = new List<string>();
        //    if (this.dataGridView2.DataSource != null)
        //    {
        //        DataTable dt = this.dataGridView2.DataSource as DataTable;
        //        dt.TableName = "电芯数据";
        //        dts.Add(dt);
        //        sheetNames.Add(dt.TableName);
        //    }
        //    SaveFileDialog dlg = new SaveFileDialog();
        //    dlg.Filter = "excel files (*.xlsx)|*.xlsx";
        //    if (dlg.ShowDialog() == DialogResult.OK)
        //    {
        //        try
        //        {
        //            string fileName = dlg.FileName;
                   
        //            DelegateExportLog dlgtExportLog = new DelegateExportLog(AsyExportLog);
        //            dlgtExportLog.BeginInvoke(dts.ToArray(), fileName, sheetNames.ToArray(), CallbackExportlogOK, dlgtExportLog);
        //            waitDlg = new LogManage.WaitDlg();
        //            waitDlg.ShowDialog();
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(ex.Message);
        //        }
        //    }
        //}
        private delegate void DelegateExportLog(DataTable[] dts, string fileName, string[] sheetNames);
        private void CallbackExportlogOK(IAsyncResult ar)
        {
            //结束
           // waitDlg.Finished = true;
            //  MessageBox.Show("数据导出成功......");
        }
        //private void AsyExportLog(DataTable[] dts, string fileName, string[] sheetNames)
        //{
        //    List<string[]> sheetCols = new List<string[]>();
        //    for (int i = 0; i < dts.Count(); i++)
        //    {
        //        DataTable dt = dts[i];
        //        string[] colNames = new string[dt.Columns.Count];
        //        for (int j = 0; j < dt.Columns.Count; j++)
        //        {
        //            colNames[j] = dt.Columns[j].ColumnName;
        //        }
        //        sheetCols.Add(colNames);
        //    }
        //    CreateExcelFile(fileName, sheetNames, sheetCols);
        //    //string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" +
        //    //"Data Source=" + fileName + ";" +
        //    //"Extended Properties='Excel 8.0; HDR=Yes; IMEX=2'";
        //    string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" +
        //    "Data Source=" + fileName + ";" +
        //    "Extended Properties='Excel 12.0; HDR=Yes; IMEX=0'";
        //    using (OleDbConnection ole_conn = new OleDbConnection(strConn))
        //    {
        //        ole_conn.Open();
        //        using (OleDbCommand ole_cmd = ole_conn.CreateCommand())
        //        {
        //            for (int dtIndex = 0; dtIndex < dts.Count(); dtIndex++)
        //            {
        //                DataTable dt = dts[dtIndex];

        //                string sheetName = sheetNames[dtIndex];
        //                // foreach (DataRow dr in dt.Rows)
        //                int rowCount = dt.Rows.Count;
        //                Console.WriteLine("准备写入{0} {1}条记录", sheetName, rowCount);
        //                for (int i = 0; i < rowCount; i++)
        //                {
        //                    int exportProgress = (int)(100 * ((float)i + 1.0) / (float)rowCount);
        //                    waitDlg.ProgressPercent = exportProgress;
        //                    DataRow dr = dt.Rows[i];
        //                    // string logContent = dr["内容"].ToString();
        //                    //logContent = logContent.Substring(0, Math.Min(255, logContent.Count()));
        //                    // ole_cmd.CommandText = string.Format(@"insert into [{0}$](日志ID,内容,类别,日志来源,时间)values('{1}','{2}','{3}','{4}','{5}')", sheetName, dr["日志ID"].ToString(), logContent, dr["类别"].ToString(), dr["日志来源"].ToString(), dr["时间"].ToString());
        //                    StringBuilder strBuild = new StringBuilder();
        //                    strBuild.AppendFormat("insert into[{0}$](", sheetName);
        //                    for (int colIndex = 0; colIndex < dt.Columns.Count; colIndex++)
        //                    {
        //                        if (colIndex == dt.Columns.Count - 1)
        //                        {
        //                            strBuild.Append(dt.Columns[colIndex].ColumnName + ")values(");
        //                        }
        //                        else
        //                        {
        //                            strBuild.Append(dt.Columns[colIndex].ColumnName + ",");
        //                        }

        //                    }
        //                    for (int colIndex = 0; colIndex < dt.Columns.Count; colIndex++)
        //                    {
        //                        if (colIndex == dt.Columns.Count - 1)
        //                        {
        //                            strBuild.Append("'" + dr[colIndex].ToString() + "')");
        //                        }
        //                        else
        //                        {
        //                            strBuild.Append("'" + dr[colIndex].ToString() + "',");
        //                        }
        //                    }
        //                    ole_cmd.CommandText = strBuild.ToString();
        //                    ole_cmd.ExecuteNonQuery();
        //                }
        //            }

        //        }
        //    }
        //}
        private void CreateExcelFile(string FileName, string[] sheetName, List<string[]> sheetCols)
        {
            //create  
            try
            {
                Console.WriteLine("开始创建文件：" + FileName);
                object Nothing = System.Reflection.Missing.Value;
                var app = new Excel.Application();
                app.Visible = false;
                Excel.Workbook workBook = app.Workbooks.Add(Nothing);
                for (int i = 0; i < sheetName.Count(); i++)
                {
                    Excel.Worksheet worksheet = (Excel.Worksheet)workBook.Sheets[i + 1];
                    worksheet.Name = sheetName[i];
                    for (int j = 0; j < sheetCols[i].Count(); j++)
                    {
                        worksheet.Cells[1, j + 1] = sheetCols[i][j];
                    }
                    //  worksheet.SaveAs(FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing);

                }
                workBook.SaveAs(FileName);
                workBook.Close(false, Type.Missing, Type.Missing);
                app.Quit();
                Console.WriteLine("创建完成！");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }


        }

        private void buttonDel_Click(object sender, EventArgs e)
        {
            if(parentPNP.RoleID>2)
            {
                MessageBox.Show("没有权限，请切换到管理员!");
                return;
            }
            int re = PoupAskmes("确定要删除，一旦删除不可恢复？");
            if(re != 1)
            {
                return;
            }
            foreach(DataGridViewRow dr in dataGridView2.SelectedRows)
            {
                if(dr.Cells["batModuleID"] == null)
                {
                    continue;
                }
                string batModID = dr.Cells["batModuleID"].Value.ToString();
                if(!string.IsNullOrWhiteSpace(batModID))
                {
                    batModBll.Delete(batModID);
                }
            }
            OnQueryBattery();
        }
        public override void ChangeRoleID(int roleID)
        {
            if(roleID<3)
            {
                this.buttonDel.Visible = true;
            }
            else
            {
                this.buttonDel.Visible = false;
            }
        }
       
      
    }
}
