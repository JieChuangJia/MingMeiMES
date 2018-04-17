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
using LogInterface;
namespace ModuleCrossPnP
{
    public partial class BaseChildView : Form, IModuleAttach
    {
       // protected string captionText = "";
        protected ILogRecorder logRecorder = null;
        protected IParentModule parentPNP = null;
        protected List<BaseChildView> childViews = new List<BaseChildView>();
        #region  公有接口
       // public string CaptionText { get { return captionText; } set { captionText = value; this.Text = captionText; } }
        public BaseChildView()
        { }
        public BaseChildView(string captionText)
        {
            InitializeComponent();
            this.Text = captionText;
          //  this.captionText = captionText;
        }
        public void ExportDtToExcel(DataTable dt,List<string> exportCols,string sheetName) 
        {
             SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "excel files (*.xlsx)|*.xlsx";
            if (dlg.ShowDialog() == DialogResult.OK)
            {

                try
                {
                 
                    string fileName = dlg.FileName;
                    //string sheetName = "sheet1";
                    List<string> colNames = new List<string>();

                    foreach (DataColumn dc in dt.Columns)
                    {
                        if (exportCols != null )
                        {
                            if(exportCols.Contains(dc.ColumnName))
                            {
                                colNames.Add(dc.ColumnName);
                            }
                            
                        }
                        else
                        {
                            colNames.Add(dc.ColumnName);
                        }
                        
                    }
                    CreateExcelFile(fileName, sheetName, colNames.ToArray());
                    //string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                    //"Data Source=" + fileName + ";" +
                    //"Extended Properties='Excel 8.0; HDR=Yes; IMEX=2'";
                    string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                    "Data Source=" + fileName + ";" +
                    "Extended Properties='Excel 12.0; HDR=Yes; IMEX=0'";
                    using (OleDbConnection ole_conn = new OleDbConnection(strConn))
                    {
                        ole_conn.Open();
                        using (OleDbCommand ole_cmd = ole_conn.CreateCommand())
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                //string logContent = dr["内容"].ToString();
                                //logContent = logContent.Substring(0, Math.Min(255, logContent.Count()));
                                StringBuilder cmdText = new StringBuilder();
                                cmdText.AppendFormat("insert into [{0}$]({1}", sheetName, colNames[0]);
                                for (int i = 1; i < colNames.Count(); i++)
                                {
                                    cmdText.AppendFormat(",{0}", colNames[i]);
                                }
                                string strVal = dr[colNames[0]].ToString();
                                if (strVal.Length > 255)
                                {
                                    strVal = strVal.Substring(0, 255);
                                }
                                cmdText.AppendFormat(")values('{0}'", dr[colNames[0]].ToString());
                                for (int i = 1; i < colNames.Count(); i++)
                                {
                                    strVal = dr[colNames[i]].ToString();
                                    if (strVal.Length > 255)
                                    {
                                        strVal = strVal.Substring(0, 255);
                                    }
                                    cmdText.AppendFormat(",'{0}'", strVal);
                                }
                                cmdText.Append(")");
                                ole_cmd.CommandText = cmdText.ToString();
                                // ole_cmd.CommandText = string.Format(@"insert into [{0}$](日志ID,内容,类别,日志来源,时间)values('{1}','{2}','{3}','{4}','{5}')", sheetName, dr["日志ID"].ToString(), logContent, dr["类别"].ToString(), dr["日志来源"].ToString(), dr["时间"].ToString());
                                ole_cmd.ExecuteNonQuery();
                            }

                            MessageBox.Show("数据导出成功......");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
        public virtual void ChangeRoleID(int roleID)
        {
            
            foreach(BaseChildView childView in childViews)
            {
                childView.ChangeRoleID(roleID);
            }
        }
        public virtual List<string> GetLogsrcList()
        {
            return null;
        }
        #endregion
        #region IModuleAttach接口实现
        public virtual void RegisterMenus(MenuStrip parentMenu, string rootMenuText)
        {
            ToolStripMenuItem rootMenuItem = new ToolStripMenuItem(rootMenuText);//parentMenu.Items.Add("仓储管理");
            rootMenuItem.Click += LoadMainform_MenuHandler;
            parentMenu.Items.Add(rootMenuItem);
        }
        public virtual void SetParent(/*Control parentContainer, Form parentForm, */IParentModule parentPnP)
        {
            this.parentPNP = parentPnP;
        }
        public virtual void SetLoginterface(ILogRecorder logRecorder)
        {
            this.logRecorder = logRecorder;
            //   lineMonitorPresenter.SetLogRecorder(logRecorder);
        }
        #endregion
        private void LoadMainform_MenuHandler(object sender, EventArgs e)
        {
            this.parentPNP.AttachModuleView(this);
            
        }
        protected int PoupAskmes(string info)
        {
            if (DialogResult.OK == MessageBox.Show(info, "提示", MessageBoxButtons.OKCancel))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        protected void CreateExcelFile(string FileName,string sheetName,string[] colName)
        {
            //create  
            object Nothing = System.Reflection.Missing.Value;
            var app = new Excel.Application();
            app.Visible = false;
            Excel.Workbook workBook = app.Workbooks.Add(Nothing);
            Excel.Worksheet worksheet = (Excel.Worksheet)workBook.Sheets[1];
            worksheet.Name = sheetName;
            for(int i=0;i<colName.Count();i++)
            {
                 worksheet.Cells[1,i+1] = colName[i];
            }
            ////headline  
            //worksheet.Cells[1, 1] = "日志ID";
            //worksheet.Cells[1, 2] = "内容";
            //worksheet.Cells[1, 3] = "类别";
            //worksheet.Cells[1,4] = "日志来源";
            //worksheet.Cells[1, 5] = "时间";

            worksheet.SaveAs(FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing);
            workBook.Close(false, Type.Missing, Type.Missing);
            app.Quit();
        }  
    }
}
