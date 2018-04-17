using System;
namespace FTDataAccess.Model
{
    /// <summary>
    /// DetectCodeDefModel:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class DetectCodeDefModel
    {
        public DetectCodeDefModel()
        { }
        #region Model
        private string _nodename;
        private int _detectindex;
        private string _detectcode;
        private string _detectitemname;
        /// <summary>
        /// 
        /// </summary>
        public string nodeName
        {
            set { _nodename = value; }
            get { return _nodename; }
        }
        /// <summary>
        /// 检测项在PLC数据块的位序号，从1开始编号
        /// </summary>
        public int detectIndex
        {
            set { _detectindex = value; }
            get { return _detectindex; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string detectCode
        {
            set { _detectcode = value; }
            get { return _detectcode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string detectItemName
        {
            set { _detectitemname = value; }
            get { return _detectitemname; }
        }
        #endregion Model

    }
}

