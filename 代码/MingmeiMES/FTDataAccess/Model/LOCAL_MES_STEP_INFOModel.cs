using System;
namespace FTDataAccess.Model
{
    /// <summary>
    /// 工位检测结果数据，本地数据库存储
    /// </summary>
    [Serializable]
    public partial class LOCAL_MES_STEP_INFOModel:ICloneable
    {
        public LOCAL_MES_STEP_INFOModel()
        { }
        #region Model
        private string _serial_number;
        private string _step_number;
        private int _check_result;
        private int _step_mark;
        private int _status = 0;
        private DateTime _trx_time;
        private DateTime _last_modify_time;
        private string _defect_codes;
        private string _recid;
        private string _user_name;
        private string _reason;
        private bool _upload_flag = false;
        private string _autostationname;
        /// <summary>
        /// 
        /// </summary>
        public string SERIAL_NUMBER
        {
            set { _serial_number = value; }
            get { return _serial_number; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string STEP_NUMBER
        {
            set { _step_number = value; }
            get { return _step_number; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int CHECK_RESULT
        {
            set { _check_result = value; }
            get { return _check_result; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int STEP_MARK
        {
            set { _step_mark = value; }
            get { return _step_mark; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int STATUS
        {
            set { _status = value; }
            get { return _status; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime TRX_TIME
        {
            set { _trx_time = value; }
            get { return _trx_time; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime LAST_MODIFY_TIME
        {
            set { _last_modify_time = value; }
            get { return _last_modify_time; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string DEFECT_CODES
        {
            set { _defect_codes = value; }
            get { return _defect_codes; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string RECID
        {
            set { _recid = value; }
            get { return _recid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string USER_NAME
        {
            set { _user_name = value; }
            get { return _user_name; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string REASON
        {
            set { _reason = value; }
            get { return _reason; }
        }
        /// <summary>
        /// 上传到MES是否成功的标识
        /// </summary>
        public bool UPLOAD_FLAG
        {
            set { _upload_flag = value; }
            get { return _upload_flag; }
        }
        /// <summary>
        /// 对应的自动线工位名称
        /// </summary>
        public string AutoStationName
        {
            set { _autostationname = value; }
            get { return _autostationname; }
        }
        #endregion Model
        public object Clone()
        {
            LOCAL_MES_STEP_INFOModel cloneM = new LOCAL_MES_STEP_INFOModel();
            cloneM.AutoStationName = this.AutoStationName;
            cloneM.CHECK_RESULT = this.CHECK_RESULT;
            cloneM.DEFECT_CODES = this.DEFECT_CODES;
            cloneM.LAST_MODIFY_TIME = this.LAST_MODIFY_TIME;
            cloneM.REASON = this.REASON;
            cloneM.RECID = this.RECID;
            cloneM.SERIAL_NUMBER = this.SERIAL_NUMBER;
            cloneM.STATUS = this.STATUS;
            cloneM.STEP_MARK = this.STEP_MARK;
            cloneM.STEP_NUMBER = this.STEP_NUMBER;
            cloneM.TRX_TIME = this.TRX_TIME;
            cloneM.UPLOAD_FLAG = this.UPLOAD_FLAG;
            cloneM.USER_NAME = this.USER_NAME;
            return cloneM;
        }
    }
}

