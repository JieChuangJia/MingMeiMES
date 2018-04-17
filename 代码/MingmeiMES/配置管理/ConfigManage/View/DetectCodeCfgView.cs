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
using FTDataAccess.Model;
namespace ConfigManage
{
    public partial class DetectCodeCfgView : BaseChildView
    {
        DetectCodeDefBll detectCodeBll = null;
        public DetectCodeCfgView(string captionText)
            : base(captionText)
        {
            InitializeComponent();
            this.Text = captionText;
            detectCodeBll = new DetectCodeDefBll();
        }

        private void DetectCodeCfgView_Load(object sender, EventArgs e)
        {
            this.toolStripComboBox1.Items.AddRange(new string[] { "功能检测", "外观检查"});
            this.toolStripComboBox1.SelectedIndex = 0;
        }
        private void SaveDetectcodes()
        {
            try
            {
                DataTable dt = this.dataGridView1.DataSource as DataTable;
                foreach(DataRow dr in dt.Rows)
                {
                     string codeIndex=dr["detectIndex"].ToString();
                     string code = dr["detectCode"].ToString();
                     if (string.IsNullOrWhiteSpace(codeIndex) || string.IsNullOrWhiteSpace(code))
                     {
                         continue;
                     }
                     DetectCodeDefModel m = new DetectCodeDefModel();
                     m.nodeName = this.toolStripComboBox1.Text;
                     m.detectIndex = int.Parse(dr["detectIndex"].ToString());
                     m.detectCode = dr["detectCode"].ToString();
                     m.detectItemName = dr["detectItemName"].ToString();
                     if (detectCodeBll.Exists(m.nodeName, m.detectIndex))
                     {
                         detectCodeBll.Update(m);
                     }
                     else
                     {
                         detectCodeBll.Add(m);
                     }
                }
              
                MessageBox.Show("保存成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
           
        }
        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            SaveDetectcodes();
        }
        private void RefreshCfg()
        {
            string cfgCata = this.toolStripComboBox1.Text;
            DataSet ds = detectCodeBll.GetList(string.Format("nodeName='{0}' ",cfgCata));
            if(ds != null && ds.Tables.Count>0)
            {
                this.dataGridView1.DataSource = ds.Tables[0];
               // this.dataGridView1.Columns["nodeName"].HeaderText = "检测类别";
                this.dataGridView1.Columns["detectIndex"].HeaderText = "序号";
                this.dataGridView1.Columns["detectCode"].HeaderText = "不良代码";
                this.dataGridView1.Columns["detectItemName"].HeaderText = "名称";
                this.dataGridView1.Columns["detectItemName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                this.dataGridView1.Columns["nodeName"].Visible = false;
            }
        }
        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            RefreshCfg();
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshCfg();
        }
        private void DelCfg()
        {
            try
            {
                if (this.dataGridView1.SelectedRows != null && this.dataGridView1.SelectedRows.Count > 0)
                {
                    DataGridViewRow dr = this.dataGridView1.SelectedRows[0];
                    string cata = this.toolStripComboBox1.Text;
                    int codeIndex = int.Parse(dr.Cells["detectIndex"].Value.ToString());
                    if (detectCodeBll.Exists(cata, codeIndex))
                    {
                        if (detectCodeBll.Delete(cata, codeIndex))
                        {
                            MessageBox.Show("删除成功!");
                            RefreshCfg();
                        }
                        else
                        {
                            MessageBox.Show("删除失败!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }
        private void btnDelSizeCfg_Click(object sender, EventArgs e)
        {
            DelCfg();
        }
    }
}
