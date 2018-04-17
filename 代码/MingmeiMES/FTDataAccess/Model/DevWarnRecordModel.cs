using System;
namespace DBAccess.Model
{
    /// <summary>
    /// DevWarnRecord:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class DevWarnRecordModel
    {
        public DevWarnRecordModel()
        { }
        #region Model
        private string _recordid;
        private DateTime _recordtime;
        private int _warnstat;
        private string _warninfo;
        private string _plcaddr;
        private string _devid;
        private string _tag1;
        private string _tag2;
        private string _tag3;
        private string _tag4;
        private string _tag5;
        /// <summary>
        /// 
        /// </summary>
        public string recordID
        {
            set { _recordid = value; }
            get { return _recordid; }
        }
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
        public string plcAddr
        {
            set { _plcaddr = value; }
            get { return _plcaddr; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string devID
        {
            set { _devid = value; }
            get { return _devid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string tag1
        {
            set { _tag1 = value; }
            get { return _tag1; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string tag2
        {
            set { _tag2 = value; }
            get { return _tag2; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string tag3
        {
            set { _tag3 = value; }
            get { return _tag3; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string tag4
        {
            set { _tag4 = value; }
            get { return _tag4; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string tag5
        {
            set { _tag5 = value; }
            get { return _tag5; }
        }
        #endregion Model

    }
}

