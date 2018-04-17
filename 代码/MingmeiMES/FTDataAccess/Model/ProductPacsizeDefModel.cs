using System;
namespace FTDataAccess.Model
{
    /// <summary>
    /// 包装尺寸定义
    /// </summary>
    [Serializable]
    public partial class ProductPacsizeDefModel
    {
        public ProductPacsizeDefModel()
        { }
        #region Model
        private string _packagesize;
        private int _packageseq;
        private string _mark;
        /// <summary>
        /// 
        /// </summary>
        public string packageSize
        {
            set { _packagesize = value; }
            get { return _packagesize; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int packageSeq
        {
            set { _packageseq = value; }
            get { return _packageseq; }
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

