using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CommonPL.Login
{
    public interface ILoginView
    {
        #region 方法
        void RefreshCbRoleList(DataTable dt);
        void ShowMainForm(int roleID,string userName);

        void ShowLoginForm();
        void HideLoginForm();
        void ShowMessage(string content, string title);
        /// <summary>
        /// 消息询问窗体
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        int AskMessageBox(string content, string title);
        #endregion
        #region 属性
        int UserID { get; set; }
        #endregion
    }
}
