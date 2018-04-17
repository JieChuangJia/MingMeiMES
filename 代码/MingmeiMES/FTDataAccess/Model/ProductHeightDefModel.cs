using System;
namespace FTDataAccess.Model
{
    /// <summary>
    /// 产品高度序列定义
    /// </summary>
    [Serializable]
    public partial class ProductHeightDefModel
    {
        public ProductHeightDefModel()
        { }
        #region Model
        private int _productheight;
        private int _heightseq;
        private string _mark;
        /// <summary>
        /// 
        /// </summary>
        public int productHeight
        {
            set { _productheight = value; }
            get { return _productheight; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int heightSeq
        {
            set { _heightseq = value; }
            get { return _heightseq; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string mark
        {
            set { _mark = value; }
            get { return _mark; }
        }
        #endregion Model

    }
}


