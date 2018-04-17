using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public enum EnumRunStat
    {
        未启动,
        正常运行,
        错误发生,
        系统异常发生
    }
    /// <summary>
    /// 运行状态，适用于所有对象
    /// </summary>
    public class RuntimeStatus
    {
        protected string statDescibe = "";
        protected EnumRunStat enumStat = EnumRunStat.未启动;

    }
}
