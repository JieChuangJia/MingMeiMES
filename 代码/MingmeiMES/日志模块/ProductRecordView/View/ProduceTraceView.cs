using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ModuleCrossPnP;
using MesDBAccess.BLL;
using MesDBAccess.Model;
using LogInterface;
namespace ProductRecordView
{
    public partial class ProduceTraceView : BaseChildView
    {
        #region 数据
        private TraceQueryFilter queryFilter = null;
        private ProduceRecordBll modRecordBll = new ProduceRecordBll();
      //  private BatteryBll batBll = new BatteryBll();
     //   private BatteryModuleBll batModBll = new BatteryModuleBll();
      //  private ProduceRecordBll recordBll = null;
        #endregion
        public ProduceTraceView(string captionText)
            : base(captionText)
        {
            InitializeComponent();
            queryFilter = new TraceQueryFilter();
            this.Text = captionText;
            //  this.captionText = captionText;


        }
        #region UI事件
        private void ProduceReccordView_Load(object sender, EventArgs e)
        {
            //过滤参数绑定
            this.comboBox1.Items.AddRange(new string[] {"模块","PACK"});
            this.comboBox1.SelectedIndex = 0;
            queryFilter.Cata = "模块";
            this.comboBox1.DataBindings.Add("Text", queryFilter, "Cata");
            this.textBoxBarcode.DataBindings.Add("Text", queryFilter, "BarCode");
            
        }

        private void buttonTrace_Click(object sender, EventArgs e)
        {
            OnRecordQuery();
        }
        private void OnRecordQuery()
        {


            string strWhere = string.Format("productID='{0}' and productCata='{1}' order by recordTime asc ", queryFilter.BarCode,queryFilter.Cata);
            DataSet ds = modRecordBll.GetList(strWhere);
            this.dataGridView1.DataSource = ds.Tables[0];
            
            this.dataGridView1.Columns["recordID"].Visible = false;
            this.dataGridView1.Columns["stationID"].Visible = false;
            this.dataGridView1.Columns["recordCata"].Visible = false;
           
            this.dataGridView1.Columns["tag3"].Visible = false;
            this.dataGridView1.Columns["tag4"].Visible = false;
            this.dataGridView1.Columns["tag5"].Visible = false;

            this.dataGridView1.Columns["checkResult"].HeaderText = "检测结果";
            this.dataGridView1.Columns["productID"].HeaderText = "条码";
            this.dataGridView1.Columns["productID"].Width = 200;
            this.dataGridView1.Columns["productCata"].HeaderText = "物料类别";
            this.dataGridView1.Columns["tag1"].HeaderText = "追溯记录";
            this.dataGridView1.Columns["tag2"].HeaderText = "检测数据";
            this.dataGridView1.Columns["tag2"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            
            this.dataGridView1.Columns["recordTime"].HeaderText = "记录时间";
            this.dataGridView1.Columns["recordTime"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
            this.dataGridView1.Columns["recordTime"].Width = 200;
        }
        #endregion

        #region 私有方法
       
      
        #endregion


    }
}
