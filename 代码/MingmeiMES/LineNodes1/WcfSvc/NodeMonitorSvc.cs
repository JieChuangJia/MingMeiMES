using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using PLProcessModel;
namespace LineNodes
{
    public delegate List<MonitorSvcNodeStatus> DlgtGetNodeStatus();
    public delegate int DlgtGetRunningDetectdevs();
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的类名“NodeMonitorSvc”。
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class NodeMonitorSvc : INodeMonitorSvc
    {
        public DlgtGetNodeStatus dlgtNodeStatus;
        public DlgtGetRunningDetectdevs dlgtRunningDev;
        public void DoWork()
        {
            //Console.WriteLine("hello ，");
        }
        public List<MonitorSvcNodeStatus> GetNodeStatus()
        {
            if (this.dlgtNodeStatus != null)
            {
                return this.dlgtNodeStatus();
            }
            return null;
        }
        public int GetRunningDetectdevs()
        {
            if(dlgtRunningDev != null)
            {
                return dlgtRunningDev();
            }
            return 0;
        }
    }   
}
