using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PLProcessModel;
namespace LineNodes
{
    public interface ILineMonitorView
    {
        System.Windows.Forms.Control GetHostControl();
        //void InitNodeMonitorview(int lineSeq,List<CtlNodeBaseModel> nodeList);
        void InitLineMonitor(int lineSeq, CtlLineBaseModel line);
      //  void BindNodeStat(); //测试
        void RefreshNodeStatus();
        void WelcomeAddStartinfo(string info); //增加欢迎信息
        void WelcomeDispCurinfo(string info);//显示当前信息
        void WelcomePopup(); //弹出启动界面
        void WelcomeClose(); //关闭欢迎界面
        void PopupMes(string strMes);
        void DispCommInfo(string strInfo);
        void StartEnabled(bool enabled); //设置是否允许启动
        void Stop();
    }
}
