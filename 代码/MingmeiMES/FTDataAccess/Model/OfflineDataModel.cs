using System;
namespace FTDataAccess.Model
{
    /// <summary>
    /// OfflineData:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class OfflineDataModel
    {
        public OfflineDataModel()
        { }
        #region Model
        private string _offlinedataid;
        private string _workstationid;
        private string _datatype;
        private string _uploadjsondata;
        private string _isupload;
        private DateTime _createtime;
        private string _reserve1;
        private string _reserve2;
        private string _reserve3;
        private string _reserve4;
        private string _reserve5;
        /// <summary>
        /// 离线数据编码
        /// </summary>
        public string OfflineDataID
        {
            set { _offlinedataid = value; }
            get { return _offlinedataid; }
        }
        /// <summary>
        /// 工作中心号
        /// </summary>
        public string WorkStationID
        {
            set { _workstationid = value; }
            get { return _workstationid; }
        }
        /// <summary>
        /// 数据类型（过程参数、上传数据）
        /// </summary>
        public string DataType
        {
            set { _datatype = value; }
            get { return _datatype; }
        }
        /// <summary>
        /// 上传MES数据字符串
        /// </summary>
        public string UploadJsonData
        {
            set { _uploadjsondata = value; }
            get { return _uploadjsondata; }
        }
        /// <summary>
        /// 待上传、已上传、用户拒绝上传
        /// </summary>
        public string IsUpLoad
        {
            set { _isupload = value; }
            get { return _isupload; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime
        {
            set { _createtime = value; }
            get { return _createtime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Reserve1
        {
            set { _reserve1 = value; }
            get { return _reserve1; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Reserve2
        {
            set { _reserve2 = value; }
            get { return _reserve2; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Reserve3
        {
            set { _reserve3 = value; }
            get { return _reserve3; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Reserve4
        {
            set { _reserve4 = value; }
            get { return _reserve4; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Reserve5
        {
            set { _reserve5 = value; }
            get { return _reserve5; }
        }
        #endregion Model

    }
}

