using System;
using System.ComponentModel;
namespace FTDataAccess.Model
{
    /// <summary>
    /// ProductSizeCfgModel:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class ProductSizeCfgModel : System.ComponentModel.INotifyPropertyChanged
    {
        public ProductSizeCfgModel()
        { }
        public event PropertyChangedEventHandler PropertyChanged;
        #region Model
        private int _cataseq;
        private string _productcatacode;
        private int _productheight;
        private string _mark;
        private string _packagesize;
        private string _productname;
        private string _gasname;
        private int? _robotprog;
        private int? _volt1;
        private int? _volt2;
        private int? _frequency1;
        private int? _frequency2;
        private string _power1;
        private string _power2;
        private string _tag1;
        private string _tag2;
        private string _tag3;
        private string _tag4;
        private string _tag5;
        public int cataSeq
        {
            set { _cataseq = value; OnPropertyChanged("cataSeq"); }
            get { return _cataseq; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string productCataCode
        {
            set { _productcatacode = value; OnPropertyChanged("productCataCode"); }
            get { return _productcatacode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int productHeight
        {
            set { _productheight = value; OnPropertyChanged("productHeight"); }
            get { return _productheight; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string mark
        {
            set { _mark = value; OnPropertyChanged("mark"); }
            get { return _mark; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string packageSize
        {
            set { _packagesize = value; OnPropertyChanged("packageSize"); }
            get { return _packagesize; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string productName
        {
            set { _productname = value; OnPropertyChanged("productName"); }
            get { return _productname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string gasName
        {
            set { _gasname = value; }
            get { return _gasname; }
        }
        /// <summary>
        /// 机器人配方
        /// </summary>
        public int? robotProg
        {
            set { _robotprog = value; OnPropertyChanged("robotProg"); }
            get { return _robotprog; }
        }
        /// <summary>
        /// 强档位电压
        /// </summary>
        public int? volt1
        {
            set { _volt1 = value; OnPropertyChanged("volt1"); }
            get { return _volt1; }
        }
        /// <summary>
        /// 弱档位电压
        /// </summary>
        public int? volt2
        {
            set { _volt2 = value; OnPropertyChanged("volt2"); }
            get { return _volt2; }
        }
        /// <summary>
        /// 强档位频率
        /// </summary>
        public int? frequency1
        {
            set { _frequency1 = value; OnPropertyChanged("frequency1"); }
            get { return _frequency1; }
        }
        /// <summary>
        /// 弱档位频率
        /// </summary>
        public int? frequency2
        {
            set { _frequency2 = value; OnPropertyChanged("frequency2"); }
            get { return _frequency2; }
        }
        /// <summary>
        /// 安规2，功率标准1
        /// </summary>
        public string power1
        {
            set { _power1 = value; OnPropertyChanged("power1"); }
            get { return _power1; }
        }
        /// <summary>
        /// 安规2功率标准2
        /// </summary>
        public string power2
        {
            set { _power2 = value; OnPropertyChanged("power2"); }
            get { return _power2; }
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
        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}

