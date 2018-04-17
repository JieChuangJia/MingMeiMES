using System;
namespace DBAccess.Model
{
    /// <summary>
    /// DevWarnrcdViewModel:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class DevWarnrcdViewModel
    {
        public DevWarnrcdViewModel()
        { }
        #region Model
        private DateTime _recordtime;
        private int _warnstat;
        private string _warninfo;
        private string _devname;
        /// <summary>
        /// 
        /// </summary>
        public DateTime recordTime
        {
            set { _recordtime = value; }
            get { return _recordtime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int warnStat
        {
            set { _warnstat = value; }
            get { return _warnstat; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string warnInfo
        {
            set { _warninfo = value; }
            get { return _warninfo; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string devName
        {
            set { _devname = value; }
            get { return _devname; }
        }
        #endregion Model

    }
}


