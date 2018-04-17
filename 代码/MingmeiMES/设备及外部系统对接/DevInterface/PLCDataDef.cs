
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevInterface
{
   
    /// <summary>
    /// 设备通信的参数类型枚举
    /// </summary>
    public enum EnumCommuDataType
    {
        DEVCOM_byte, //单字节
        DEVCOM_real, //浮点型,4字节
        DEVCOM_short,//短整型，2字节
        DEVCOM_int, //整形,4字节
        DEVCOM_string //字符串型
    }

    /// <summary>
    /// 通信实现方式枚举
    /// </summary>
    public enum EnumCommMethod
    {
        PLC_OPC_COMMU = 1, //plc OPC通信
        PLC_MIT_COMMU, //plc 三菱控件通信
    }
    /// <summary>
    /// 通信功能数据类型定义
    /// </summary>
    public class PLCDataDef
    {
        /// <summary>
        /// 功能ID
        /// </summary>
        public int CommuID { get; set; }
       
        /// <summary>
        /// 功能描述
        /// </summary>
        public string DataDescription { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public EnumCommuDataType DataTypeDef { get; set; }

        /// <summary>
        /// 数据字节数
        /// </summary>
        public int DataByteLen { get; set; }

        /// <summary>
        /// 通信地址描述
        /// </summary>
        public string DataAddr { get; set; }

        /// <summary>
        /// 通信数据值
        /// </summary>
        public object Val { get; set; }

        /// <summary>
        /// 通信实现方式
        /// </summary>
        public EnumCommMethod CommuMethod { get; set; }
    }
}
