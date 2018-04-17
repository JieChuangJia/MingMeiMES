using System;
namespace FTDataAccess.Model
{
    /// <summary>
    /// 在线产品
    /// </summary>
    [Serializable]
    public partial class OnlineProductsModel
    {
        public OnlineProductsModel()
        { }
        #region Model
        private string _productbarcode;
        private DateTime _inputtime;
        private string _currentnode;
        private string _rfidcode;
        /// <summary>
        /// 
        /// </summary>
        public string productBarcode
        {
            set { _productbarcode = value; }
            get { return _productbarcode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime inputTime
        {
            set { _inputtime = value; }
            get { return _inputtime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string currentNode
        {
            set { _currentnode = value; }
            get { return _currentnode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string rfidCode
        {
            set { _rfidcode = value; }
            get { return _rfidcode; }
        }
        #endregion Model

    }
}

