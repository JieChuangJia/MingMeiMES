using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCIRDBAccess
{
    /// <summary>
    /// dbModel:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class dcirMode
    {
        public dcirMode()
        { }
        #region Model
        private string _测试时间;
        private string _电流;
        private string _电压;
        private string _容量;
        private string _能量;
        private string _总时间;
        private string _相对时间;
        /// <summary>
        /// 
        /// </summary>
        public string 测试时间
        {
            set { _测试时间 = value; }
            get { return _测试时间; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string 电流
        {
            set { _电流 = value; }
            get { return _电流; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string 电压
        {
            set { _电压 = value; }
            get { return _电压; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string 容量
        {
            set { _容量 = value; }
            get { return _容量; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string 能量
        {
            set { _能量 = value; }
            get { return _能量; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string 总时间
        {
            set { _总时间 = value; }
            get { return _总时间; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string 相对时间
        {
            set { _相对时间 = value; }
            get { return _相对时间; }
        }
        #endregion Model

    }
}
