using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using FTDataAccess.BLL;
using FTDataAccess.Model;
namespace CommonPL.Login
{
    public class LoginUserModel
    {
        public int RoleID { get; set; }
       // public string RoleName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsChangeUser { get; set; }
    }
    public class LoginPresenter 
    {
        #region 全局变量
        private ILoginView View = null;
        private readonly User_ListBll bllUser = new User_ListBll();
        private readonly User_RoleBLL bllRole = new User_RoleBLL();
        #endregion

        #region 初始化
        public LoginPresenter(ILoginView view)
        {
            this.View = view;   
        }

        //protected override void OnViewSet()
        //{
        //    string jxDB = ConfigurationManager.AppSettings["JXDataBase"];
        //    string jxDBUserPwd = ConfigurationManager.AppSettings["JXDataBaseUserPwd"];
        //    jxDBUserPwd = EncAndDec.Decode(jxDBUserPwd, "zwx", "xwz");
        //    if (string.IsNullOrEmpty(jxDBUserPwd))
        //    {
                
        //        return;
        //    }
        //    PubConstant.ConnectionString = jxDB + jxDBUserPwd;
        //    //PubConstant.ConnectionString = jxDB + "Persist Security info = True;Initial Catalog=ECAMSDatabase;User ID=sa;Password=123456;";
        //    this.View.eventBindRoleData += BindRoleDataEventHandler;
        //    this.View.eventLogin += LoginEventHandler;

        //}
        #endregion

        #region 实现ILoginView事件方法
        public void Login(LoginUserModel userModel)
        {
            bool isRegister = false;
            if(userModel.RoleID == 3)
            {
                isRegister = true;
            }
            else
            {
                isRegister = bllUser.IsUserRegister(userModel.RoleID, userModel.UserName, userModel.Password);
            }

            //string roleName = userModel.RoleName;
      
            string loginInfo = null;
            if (isRegister)
            {
                loginInfo = string.Format("用户：{0},登录系统，结果：成功！", userModel.UserName);
            }
            else
            {
                loginInfo = string.Format("用户：{0},角色:{1},登录系统，结果：失败！", userModel.UserName, userModel.UserName);
            }
            if (userModel.IsChangeUser == false)
            {
                if (isRegister == true)
                {
                    this.View.HideLoginForm();
                    this.View.ShowMainForm(userModel.RoleID,userModel.UserName);
                }
                else
                {
                    this.View.ShowMessage("登录失败！用户权限、用户名或密码错误！", "信息提示");
                }
            }
            else
            {
                if (isRegister == true)
                {
                    //MainPresenter mainPre = (MainPresenter)this.View.GetPresenter(typeof(MainPresenter));
                    //if (mainPre != null)
                    //{
                    //    mainPre.View.ShowView();
                    //    mainPre.SetLimit(userModel.RoleID);
                    //    this.View.HideLoginForm();
                    //}
                }
                else
                {
                    this.View.ShowMessage("登录失败！用户权限、用户名或密码错误！", "信息提示");
                }
             
            }
        }

        public void BindRoleData()
        {
            DataSet ds = bllUser.GetList("");
            if (ds != null && ds.Tables.Count > 0)
            {
                this.View.RefreshCbRoleList(ds.Tables[0]);
            }

        }
        #endregion
    }
}
