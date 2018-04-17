using System;
namespace FTDataAccess.Model
{
    /// <summary>
    /// 控制任务表
    /// </summary>
    [Serializable]
    public partial class ControlTaskModel
    {
        public ControlTaskModel()
        { }
        #region Model
        private string _taskid;
        private int _tasktype;
        private string _taskparam;
        private string _taskstatus;
        private int _taskphase;
        private DateTime _createtime;
        private DateTime? _finishtime;
        private string _createmode;
        private string _remark;
        private string _deviceid;
        private string _tag1;
        private string _tag2;
        private string _tag3;
        private string _tag4;
        private string _tag5;

        /// <summary>
        /// 
        /// </summary>
        public string TaskID
        {
            set { _taskid = value; }
            get { return _taskid; }
        }
        /// <summary>
        /// 任务类型
        /// </summary>
        public int TaskType
        {
            set { _tasktype = value; }
            get { return _tasktype; }
        }
        /// <summary>
        /// 任务参数
        /// </summary>
        public string TaskParam
        {
            set { _taskparam = value; }
            get { return _taskparam; }
        }
        /// <summary>
        /// 任务状态
        /// </summary>
        public string TaskStatus
        {
            set { _taskstatus = value; }
            get { return _taskstatus; }
        }
        /// <summary>
        /// 任务阶段
        /// </summary>
        public int TaskPhase
        {
            set { _taskphase = value; }
            get { return _taskphase; }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            set { _createtime = value; }
            get { return _createtime; }
        }
        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime? FinishTime
        {
            set { _finishtime = value; }
            get { return _finishtime; }
        }
        /// <summary>
        /// 创建模式
        /// </summary>
        public string CreateMode
        {
            set { _createmode = value; }
            get { return _createmode; }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            set { _remark = value; }
            get { return _remark; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string DeviceID
        {
            set { _deviceid = value; }
            get { return _deviceid; }
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

