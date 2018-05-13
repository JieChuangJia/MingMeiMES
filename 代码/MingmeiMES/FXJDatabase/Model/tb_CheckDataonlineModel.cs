using System;
namespace FXJDatabase
{
    /// <summary>
    /// tb_CheckDataonline:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class tb_CheckDataonlineModel
    {
        public tb_CheckDataonlineModel()
        { }
        #region Model
        private long? _id;
        private string _barcode;
        private string _ctype;
        private string _filename;
        private string _fltcapacity;
        private decimal? _fltvolgap;
        private decimal? _fltselffrequency;
        private decimal? _fltvol;
        private decimal? _fltresistance;
        private string _cgrade;
        private decimal? _cstate;
        private string _cdate;
        private string _cstatecode;
        private string _tf_batchid;
        private string _tf_trayid;
        private string _tf_location;
        private string _tf_checkgrade;
        private int? _tf_tag;
        private string _dlocv;
        private string _dlocvtime;
        private string _tf_tempe;
        private string _tf_docv;
        private string _kvalue;
        private string _tf_pick;
        private string _tf_resultnum;
        private long _hlid;
        /// <summary>
        /// 
        /// </summary>
        public long? ID
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string BarCode
        {
            set { _barcode = value; }
            get { return _barcode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string cType
        {
            set { _ctype = value; }
            get { return _ctype; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FileName
        {
            set { _filename = value; }
            get { return _filename; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string fltCapacity
        {
            set { _fltcapacity = value; }
            get { return _fltcapacity; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? fltVolGap
        {
            set { _fltvolgap = value; }
            get { return _fltvolgap; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? fltSelfFrequency
        {
            set { _fltselffrequency = value; }
            get { return _fltselffrequency; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? fltVol
        {
            set { _fltvol = value; }
            get { return _fltvol; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? fltResistance
        {
            set { _fltresistance = value; }
            get { return _fltresistance; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string cGrade
        {
            set { _cgrade = value; }
            get { return _cgrade; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? cState
        {
            set { _cstate = value; }
            get { return _cstate; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string cDate
        {
            set { _cdate = value; }
            get { return _cdate; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string cStateCode
        {
            set { _cstatecode = value; }
            get { return _cstatecode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Tf_BatchId
        {
            set { _tf_batchid = value; }
            get { return _tf_batchid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Tf_TrayId
        {
            set { _tf_trayid = value; }
            get { return _tf_trayid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string tf_Location
        {
            set { _tf_location = value; }
            get { return _tf_location; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string tf_CheckGrade
        {
            set { _tf_checkgrade = value; }
            get { return _tf_checkgrade; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? tf_Tag
        {
            set { _tf_tag = value; }
            get { return _tf_tag; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string dlOCV
        {
            set { _dlocv = value; }
            get { return _dlocv; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string dlOCVTime
        {
            set { _dlocvtime = value; }
            get { return _dlocvtime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Tf_TempE
        {
            set { _tf_tempe = value; }
            get { return _tf_tempe; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string tf_DOcv
        {
            set { _tf_docv = value; }
            get { return _tf_docv; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Kvalue
        {
            set { _kvalue = value; }
            get { return _kvalue; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string tf_Pick
        {
            set { _tf_pick = value; }
            get { return _tf_pick; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string tf_ResultNum
        {
            set { _tf_resultnum = value; }
            get { return _tf_resultnum; }
        }
        /// <summary>
        /// 
        /// </summary>
        public long HLId
        {
            set { _hlid = value; }
            get { return _hlid; }
        }
        #endregion Model

    }
}

