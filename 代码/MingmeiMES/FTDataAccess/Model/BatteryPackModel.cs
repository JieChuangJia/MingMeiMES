using System;
namespace DBAccess.Model
{
    /// <summary>
    /// BatteryPackModel:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class BatteryPackModel
    {
        public BatteryPackModel()
        { }
        #region Model
        private string _batpackid;
        private DateTime _packasmtime;
        private string _opworkerid;
        private string _bmsid;
        private string _tag1;
        private string _tag2;
        private string _tag3;
        private string _tag4;
        private string _tag5;
        /// <summary>
        /// 
        /// </summary>
        public string batPackID
        {
            set { _batpackid = value; }
            get { return _batpackid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime packAsmTime
        {
            set { _packasmtime = value; }
            get { return _packasmtime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string opWorkerID
        {
            set { _opworkerid = value; }
            get { return _opworkerid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string bmsID
        {
            set { _bmsid = value; }
            get { return _bmsid; }
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

