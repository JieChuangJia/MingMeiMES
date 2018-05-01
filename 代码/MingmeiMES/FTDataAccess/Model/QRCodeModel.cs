using System;
namespace DBAccess.Model
{
    /// <summary>
    /// QRCodeModel:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class QRCodeModel
    {
        public QRCodeModel()
        { }
        #region Model
        private string _qrcode;
        private string _qrtype;
        private string _pintstatus;
        private string _reserve1;
        private string _reserve2;
        private string _reserve3;
        private string _reserve4;
        private string _reserve5;
        /// <summary>
        /// 二维码
        /// </summary>
        public string QRCode
        {
            set { _qrcode = value; }
            get { return _qrcode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string QRType
        {
            set { _qrtype = value; }
            get { return _qrtype; }
        }
        /// <summary>
        /// 打印状态
        /// </summary>
        public string PintStatus
        {
            set { _pintstatus = value; }
            get { return _pintstatus; }
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

