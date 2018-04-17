using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using PLProcessModel;
namespace LineNodes
{
    public class MonitorSvcNodeStatus
    {
        private string nodeName = "";
        private string status;//节点状态
        private string productBarcode;//产品条码
        private string statDescribe;//状态描述
        public string Status { get { return status; } set { status = value; } }
        public string NodeName { get { return nodeName; } set { nodeName = value; } }
        public string ProductBarcode { get { return productBarcode; } set { productBarcode = value; } }
        public string StatDescribe { get { return statDescribe; } set { statDescribe = value; } }

        public string ProductName { get; set; }
    }
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“INodeMonitorSvc”。
    [ServiceContract]
    public interface INodeMonitorSvc
    {
        [OperationContract]
        void DoWork();

        [OperationContract]
        List<MonitorSvcNodeStatus> GetNodeStatus();

         [OperationContract]
        int GetRunningDetectdevs();
    }
}
