using System;
namespace FTDataAccess.Model
{
    /// <summary>
    /// 气源配置
    /// </summary>
    [Serializable]
    public partial class GasConfigModel
    {
        public GasConfigModel()
        { }
        #region Model
        private string _gascode;
        private string _gasname;
        private int _gasseq;
        /// <summary>
        /// 气源编码（两位字符）
        /// </summary>
        public string gasCode
        {
            set { _gascode = value; }
            get { return _gascode; }
        }
        /// <summary>
        /// 气源名称
        /// </summary>
        public string gasName
        {
            set { _gasname = value; }
            get { return _gasname; }
        }
        /// <summary>
        /// 气源序号，PLC控制相关
        /// </summary>
        public int gasSeq
        {
            set { _gasseq = value; }
            get { return _gasseq; }
        }
        #endregion Model

    }
}

