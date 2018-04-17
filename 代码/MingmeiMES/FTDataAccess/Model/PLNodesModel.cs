using System;
namespace FTDataAccess.Model
{
    /// <summary>
    /// PLNodesModel:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class PLNodesModel
    {
        public PLNodesModel()
        { }
        #region Model
        private string _nodeid;
        private string _nodename;
        private bool _enablerun = true;
        private bool _checkrequired;
        private string _workerid;
        private DateTime? _lastlogintime;
        private string _tag1;
        private string _tag2;
        private string _tag3;
        private string _tag4;
        private string _tag5;
        /// <summary>
        /// 检测工位号
        /// </summary>
        public string nodeID
        {
            set { _nodeid = value; }
            get { return _nodeid; }
        }
        /// <summary>
        /// 工位名称
        /// </summary>
        public string nodeName
        {
            set { _nodename = value; }
            get { return _nodename; }
        }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool enableRun
        {
            set { _enablerun = value; }
            get { return _enablerun; }
        }
        /// <summary>
        /// 是否需要检测
        /// </summary>
        public bool checkRequired
        {
            set { _checkrequired = value; }
            get { return _checkrequired; }
        }
        /// <summary>
        /// 工人ID
        /// </summary>
        public string workerID
        {
            set { _workerid = value; }
            get { return _workerid; }
        }
        /// <summary>
        /// 工人号最近一次登录时间
        /// </summary>
        public DateTime? lastLoginTime
        {
            set { _lastlogintime = value; }
            get { return _lastlogintime; }
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

