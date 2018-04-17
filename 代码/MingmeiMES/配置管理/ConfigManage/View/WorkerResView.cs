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
using ConfigManage.Model;
using FTDataAccess;
using FTDataAccess.BLL;
using FTDataAccess.Model;
namespace ConfigManage
{
    public partial class WorkerResView : BaseChildView
    {
        private FTDataAccess.BLL.workerResBll workerBll = new workerResBll();
        public WorkerResView(string captionText)
            : base(captionText)
        {
            InitializeComponent();
            this.Text = captionText;
            //string[] nodeNames = new string[] { "安规检测", "功能检测", "性能检测", "外观检查", "下线" };
            string[] nodeNames = new string[] { "功能检测", "外观检查"};
            this.cbxCheckNodes.Items.AddRange(nodeNames);
            this.cbxCheckNodes.SelectedIndex = 0;
        }

        private void WorkerResView_Load(object sender, EventArgs e)
        {
            this.comboBoxShiftNo.Items.AddRange(new string[] {"班组1","班组2" });
            this.toolStripComboBoxShiftNo.Items.AddRange(new string[] { "班组1", "班组2","所有" });
            this.comboBoxSex.Items.AddRange(new string[] { "男", "女" });
            this.comboBoxSex.SelectedIndex = 0;
            this.comboBoxShiftNo.SelectedIndex = 0;
            this.toolStripComboBoxShiftNo.SelectedIndex = 2;
            RefreshWorkerCfgList();
            OnRefreshWorkerCfgs();
            OnRefreshWorkers();
        }
        private void RefreshWorkerCfgList()
        {
            List<workerResModel> workers = workerBll.GetModelList("");
            this.cbxWorkerNames.Items.Clear();
            this.cbxWorkerIDS.Items.Clear();
            if (workers.Count > 0)
            {
                List<string> workerIDS = new List<string>();
                List<string> workerNames = new List<string>();
                foreach (workerResModel w in workers)
                {
                    workerIDS.Add(w.workerID);
                    if (!string.IsNullOrWhiteSpace(w.name))
                    {
                        workerNames.Add(w.name);
                    }

                }
                if (workerNames.Count() > 0)
                {
                    RefreshWorkerNames(workerNames);
                }
                if (workerIDS.Count() > 0)
                {
                    RefreshWorkerIDS(workerIDS);
                }


            }
        }
        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }
        private void btnLoginWorker_Click(object sender, EventArgs e)
        {
            OnLoginWorkerInfo();
        }
        private void RefreshWorkerIDS(List<string> ids)
        {
            this.cbxWorkerIDS.Items.Clear();
            this.cbxWorkerIDS.Items.AddRange(ids.ToArray());
           
            this.cbxWorkerIDS.SelectedIndex = 0;
        }
        private void RefreshWorkerNames(List<string> names)
        {
            this.cbxWorkerNames.Items.Clear();
            this.cbxWorkerNames.Items.AddRange(names.ToArray());
            this.cbxWorkerNames.SelectedIndex = 0;
        }
        private void OnLoginWorkerInfo()
        {
            try
            {
                string nodeName = this.cbxCheckNodes.Text;
                string workerID = this.cbxWorkerIDS.Text;
                if (string.IsNullOrWhiteSpace(nodeName))
                {
                    MessageBox.Show("工位名称为空，请重新选择");
                    return;
                }
                if (string.IsNullOrWhiteSpace(workerID))
                {
                    MessageBox.Show("登录的员工号为空，请重新选择");
                    return;
                }
                FTDataAccess.BLL.PLNodesBll plNodesBll = new FTDataAccess.BLL.PLNodesBll();
                string strWhere = string.Format(" nodeName='{0}'", nodeName);

                List<FTDataAccess.Model.PLNodesModel> nodeList = plNodesBll.GetModelList(strWhere);
                if (nodeList == null || nodeList.Count() < 1)
                {
                    MessageBox.Show("没有工位信息:" + nodeName);
                    return;
                }
                FTDataAccess.Model.PLNodesModel node = nodeList[0];
                node.workerID = workerID;
                node.lastLoginTime = System.DateTime.Now;
                if (plNodesBll.Update(node))
                {
                    logRecorder.AddDebugLog(this.Text, string.Format("登录员工信息成功，工位：{0}，员工号：{1}", nodeName, workerID));
                }
                else
                {
                    logRecorder.AddDebugLog(this.Text, string.Format("登录员工信息失败，工位：{0}，员工号：{1}", nodeName, workerID));
                    MessageBox.Show(string.Format("登录员工信息失败，工位：{0}，员工号：{1}", nodeName, workerID));
                }
            }
            catch (Exception ex)
            {
                PoupAskmes(ex.Message);
            }

        }
        private void OnRefreshWorkerCfgs()
        {
            FTDataAccess.BLL.PLNodesBll plNodesBll = new FTDataAccess.BLL.PLNodesBll();
            DataSet ds = plNodesBll.GetList(string.Format("tag1='1'"));
            this.dataGridView1.DataSource = ds.Tables[0];
            this.dataGridView1.Columns["lastLoginTime"].HeaderText = "上次登录时间";
            this.dataGridView1.Columns["nodeName"].HeaderText = "工位";
            this.dataGridView1.Columns["workerID"].HeaderText = "员工号";


            this.dataGridView1.Columns["lastLoginTime"].Width = 200;
            this.dataGridView1.Columns["nodeID"].Visible = false;
            this.dataGridView1.Columns["checkRequired"].Visible = false;
            this.dataGridView1.Columns["enableRun"].Visible = false;
            this.dataGridView1.Columns["tag1"].Visible = false;
            this.dataGridView1.Columns["tag2"].Visible = false;
            this.dataGridView1.Columns["tag3"].Visible = false;
            this.dataGridView1.Columns["tag4"].Visible = false;
            this.dataGridView1.Columns["tag5"].Visible = false;
        }
        private void btnWorkerCfgs_Click(object sender, EventArgs e)
        {
            OnRefreshWorkerCfgs();
        }
        #region 员工信息管理
        private void OnAddWorker()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(this.textBoxID.Text))
                {
                    MessageBox.Show("员工号为空,请重新输入");
                    return;
                }
                if (string.IsNullOrWhiteSpace(this.textBoxName.Text))
                {
                    MessageBox.Show("员工姓名为空,请重新输入");
                    return;
                }
                workerResModel worker = new workerResModel();
                worker.workerID = this.textBoxID.Text;
                worker.name = this.textBoxName.Text;
                worker.sex = this.comboBoxSex.Text;
                if (!string.IsNullOrWhiteSpace(this.textBoxAge.Text))
                {
                    worker.age = int.Parse(this.textBoxAge.Text);
                }

                worker.shiftNo = this.comboBoxShiftNo.Text;
                if(workerBll.Add(worker))
                {
                    OnRefreshWorkers();
                    RefreshWorkerCfgList();
                }
                else
                {
                    MessageBox.Show("增加员工信息失败");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
          
            
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            OnAddWorker();
        }
        private void OnDelWorker()
        {
            try
            {
                if (this.dataGridView2.SelectedRows.Count < 1)
                {
                    return;
                }
                foreach (DataGridViewRow dr in this.dataGridView2.SelectedRows)
                {
                    string id = dr.Cells["workerID"].Value.ToString();
                    workerBll.Delete(id);
                    RefreshWorkerCfgList();
                }
                OnRefreshWorkers();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            OnDelWorker();
        }
       
        private void OnModifyWorker()
        {

        }
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            OnModifyWorker();
        }
        private void OnRefreshWorkers()
        {
            string str="";
            string strShiftNo=toolStripComboBoxShiftNo.Text;
            if(!string.IsNullOrWhiteSpace(strShiftNo) && strShiftNo != "所有")
            {
                str = string.Format(" shiftNo='{0}'", strShiftNo);
            }
            DataSet ds = workerBll.GetList(str);
            DataTable dt = ds.Tables[0];
            this.dataGridView2.DataSource = dt;
            this.dataGridView2.Columns["photo"].Visible = false;
            this.dataGridView2.Columns["tag1"].Visible = false;
            this.dataGridView2.Columns["tag2"].Visible = false;
            this.dataGridView2.Columns["tag3"].Visible = false;
            this.dataGridView2.Columns["tag4"].Visible = false;
            this.dataGridView2.Columns["tag5"].Visible = false;
            this.dataGridView2.Columns["workerID"].HeaderText = "员工号";
            this.dataGridView2.Columns["name"].HeaderText = "姓名";
            this.dataGridView2.Columns["sex"].HeaderText = "性别";
            this.dataGridView2.Columns["age"].HeaderText = "年龄";
            this.dataGridView2.Columns["shiftNo"].HeaderText = "班组";
        }
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            OnRefreshWorkers();
        }

        private void OnClearInput()
        {
            this.textBoxID.Text = "";
            this.textBoxName.Text = "";
            this.textBoxAge.Text = "";
            
        }
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            OnClearInput();
        }

        #endregion

        private void cbxWorkerIDS_SelectedIndexChanged(object sender, EventArgs e)
        {
            string workerID = this.cbxWorkerIDS.Text;
            workerResModel m = workerBll.GetModel(workerID);
            if(m != null)
            {
                this.label1.Text = "员工号：" + m.workerID;
                this.label2.Text = "姓名：" + m.name;
                this.label7.Text = "性别:" + m.sex;
                this.label6.Text = "班组：" + m.shiftNo;

            }
        }
       

       
    }
}
