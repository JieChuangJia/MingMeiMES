using System;
namespace FTDataAccess.Model
{
    /// <summary>
    /// LOCAL_MES_STEP_INFO_DETAILModel:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class LOCAL_MES_STEP_INFO_DETAILModel:ICloneable
    {
        public LOCAL_MES_STEP_INFO_DETAILModel()
        { }
        #region Model
        private string _serial_number;
        private string _step_number;
        private int _status = 0;
        private string _data_name;
        private string _data_value;
        private DateTime _trx_time;
        private DateTime _last_modify_time;
        private string _recid;
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
        public int STATUS
        {
            set { _status = value; }
            get { return _status; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string DATA_NAME
        {
            set { _data_name = value; }
            get { return _data_name; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string DATA_VALUE
        {
            set { _data_value = value; }
            get { return _data_value; }
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
        public string RECID
        {
            set { _recid = value; }
            get { return _recid; }
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
            LOCAL_MES_STEP_INFO_DETAILModel cloneM = new LOCAL_MES_STEP_INFO_DETAILModel();
            cloneM.AutoStationName = this.AutoStationName;
            cloneM.DATA_NAME = this.DATA_NAME;
            cloneM.DATA_VALUE = this.DATA_VALUE;
            cloneM.LAST_MODIFY_TIME = this.LAST_MODIFY_TIME;
            cloneM.RECID = this.RECID;
            cloneM.SERIAL_NUMBER = this.SERIAL_NUMBER;
            cloneM.STATUS = this.STATUS;
            cloneM.STEP_NUMBER = this.STEP_NUMBER;
            cloneM.TRX_TIME = this.TRX_TIME;
            cloneM.UPLOAD_FLAG = this.UPLOAD_FLAG;
            return cloneM;
        }
    }
}

