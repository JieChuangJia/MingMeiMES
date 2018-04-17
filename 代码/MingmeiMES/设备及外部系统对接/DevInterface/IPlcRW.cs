using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevInterface
{
    public enum EnumPLCType
    {
        MIT_FX,
        MIT_Q,
        MIT_L,
        OML_TCP,
        OML_UDP
    }
    public enum EnumDevCommStatus
    {
        通信正常,
        通信断开
    }
 

    /// <summary>
    /// 作者:np
    /// 时间:2014年6月3日
    /// 内容:类型
    /// </summary>
    public enum EnumRequireType
    {
        成批写入,
        成批读取
    }

    /// <summary>
    /// 系统中用到的PLC
    /// </summary>
    public enum EnumDevPLC
    {
        PLC_MASTER_Q,
        PLC_STACKER_FX
    }

    /// <summary>
    /// PLC自动重连事件参数
    /// </summary>
    public class PlcReLinkArgs:EventArgs
    {
        /// <summary>
        /// PLC连接信息，包括IP,端口号，
        /// </summary>
        public string StrConn { get; set; }

        /// <summary>
        /// PLC 分配的ID，自定义
        /// </summary>
        public int PlcID { get; set; }
    }
    /// <summary>
    /// plc 读写功能的接口
    /// </summary>
    public interface IPlcRW
    {
        Int64 PlcStatCounter { get; }
        string PlcRole { get; set; }
        /// <summary>
        /// PLC id，自定义
        /// </summary>
        int PlcID { get; set; }
        /// <summary>
        /// 是否处于连接状态
        /// </summary>
        bool IsConnect { get; }

        int StationNumber { get; set; }
        Int16[] Db1Vals { get; set; }
        Int16[] Db2Vals { get; set; }
        /// <summary>
        /// 连接PLC
        /// </summary>
        /// <param name="plcAddr">plc地址</param>
        /// <param name="reStr"></param>
        /// <returns></returns>
        bool ConnectPLC(ref string reStr); 

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <returns></returns>
        bool CloseConnect();

        /// <summary>
        /// 读PLC 数据
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        bool ReadDB(string addr,ref int val);

        bool ReadMultiDB(string addr, int blockNum, ref short[] vals);

        /// <summary>
        /// 写PLC数据
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        bool WriteDB(string addr,int val);
        bool WriteMultiDB(string addr, int blockNum, short[] vals);
        void PlcRWStatUpdate();
       // event EventHandler<PlcReLinkArgs> eventLinkLost;
     //   event EventHandler<LogEventArgs> eventLogDisp;
    }
}
