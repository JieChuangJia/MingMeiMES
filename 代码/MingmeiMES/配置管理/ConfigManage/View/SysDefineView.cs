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
using FTDataAccess.DBUtility;
using DevAccess;
namespace ConfigManage
{
    public partial class SysDefineView : BaseChildView
    {
        #region 公共接口
        MesDA mesDA = null;
        public SysDefineView(string captionText):base(captionText)
        {
            InitializeComponent();
            this.Text = captionText;
        }
        #endregion

        private void SysDefineView_Load(object sender, EventArgs e)
        {
            this.toolStripComboBox1.Items.AddRange(new string[] { "线体控制数据库", "MES数据库" });
            this.toolStripComboBox1.SelectedIndex = 0;
            mesDA = new MesDA();
            string reStr="";
            if(!mesDA.ConnDB(ref reStr))
            {
                MessageBox.Show(reStr,"MES数据库连接失败");
            }
        }

        private void btnSqlparser_Click(object sender, EventArgs e)
        {

        }

        private void btnSqlExec_Click(object sender, EventArgs e)
        {
            OnSqlExec();
        }
        private void OnSqlparse()
        {
           
        }
        private void OnSqlExec()
        {
            try
            {
                string strSql = this.richTextBox1.Text;
                if (string.IsNullOrEmpty(strSql))
                {

                    return;
                }

                if (this.toolStripComboBox1.Text == "线体控制数据库")
                {
                    DataSet ds = DbHelperSQL.Query(strSql);
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        this.dataGridView1.DataSource = ds.Tables[0];
                    }
                }
                else if (this.toolStripComboBox1.Text == "MES数据库")
                {
                    this.dataGridView1.DataSource = mesDA.ReadMesTable(strSql);
                }
                else
                {
                    MessageBox.Show("错误的选择");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
           
        }
       
    }
}
