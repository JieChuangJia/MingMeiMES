using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CommonPL;
using System.Configuration;
namespace CommonPL.Login
{
    public partial class LoginView : Form,ILoginView
    {
        #region 全局变量
        public bool isChangeUser = false;
        public int UserID { get; set; }
        public LoginPresenter Presenter { get; set; }
        #endregion

        #region 初始化
        public LoginView()
        {
            InitializeComponent();
            
        }

        private void LoginView_Load(object sender, EventArgs e)
        {
           
            this.Presenter = new LoginPresenter(this);
            OnBindRoleData();
        }

        #endregion

        #region 事件
        private void bt_login_Click(object sender, EventArgs e)
        {

            OnLogin();
        }

        private void bt_cancel_Click(object sender, EventArgs e)
        {
            if (this.isChangeUser == false)
            {
                this.Close();
            }
            else
            {
                this.Hide();
            }
        }
        private void LoginView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.isChangeUser == true)
            {
                e.Cancel = true;
                this.Hide();
            }

        }
        #endregion

    

        #region 触发ILoginView 事件
        private void OnLogin()
        {
            if (this.cb_UserRole.SelectedItem == null)
            {
                MessageBox.Show("请选择用户角色！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            int roleID = int.Parse(this.cb_UserRole.SelectedValue.ToString());
            //string roleName = this.cb_UserRole.Text;
            string userName = this.cb_UserRole.Text.Trim();
            string password = this.tb_userPassword.Text.Trim();
            this.UserName = userName;
            LoginUserModel userModel = new LoginUserModel();
            userModel.IsChangeUser = this.isChangeUser;
            userModel.Password = password;
            userModel.RoleID = roleID;
            userModel.UserName = UserName;
            this.Presenter.Login(userModel);
          
        }
        private void OnBindRoleData()
        {
            this.Presenter.BindRoleData();
        
        }
        #endregion

        #region 实现ILoginView 方法

        public void RefreshCbRoleList(DataTable dt)
        {
            this.cb_UserRole.DataSource = dt;
            this.cb_UserRole.DisplayMember = "UserName";
            this.cb_UserRole.ValueMember = "RoleID";

            
        }
        public void ShowMainForm(int roleID,string userName)
        {

            MainForm main = new MainForm(roleID,userName);
            main.ShowDialog();
        }
        public void ShowDialog(string content)
        {
            MessageBox.Show(content, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public void ShowLoginForm()
        {
            OnBindRoleData();
            this.isChangeUser = true;
            this.Show();
        }

        public void HideLoginForm()
        {
            this.Hide();
        }
        #endregion

        #region 实现ILoginView属性
        public string UserName { get; set; }
        public void ShowMessage(string content, string title)
        {
            MessageBox.Show(content, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public int AskMessageBox(string content, string title)
        {
            if (MessageBox.Show(content, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        #endregion

        private void cb_UserRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(this.cb_UserRole.Text == "操作员")
            {
                this.tb_userPassword.Enabled = false;
            }
            else
            {
                this.tb_userPassword.Enabled = true;
            }
        }


    }
}
