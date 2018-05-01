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
        private string _二维码;
        private string _电阻值;
        private string _放电电流1;
        private string _放电电流2;
        private string _放电时间;
        private string _静置时间;
        private string _结果电压;
        private string _结果电流;
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
        public string 二维码
        {
            set { _二维码 = value; }
            get { return _二维码; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string 电阻值
        {
            set { _电阻值 = value; }
            get { return _电阻值; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string 放电电流1
        {
            set { _放电电流1 = value; }
            get { return _放电电流1; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string 放电电流2
        {
            set { _放电电流2 = value; }
            get { return _放电电流2; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string 放电时间
        {
            set { _放电时间 = value; }
            get { return _放电时间; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string 静置时间
        {
            set { _静置时间 = value; }
            get { return _静置时间; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string 结果电压
        {
            set { _结果电压 = value; }
            get { return _结果电压; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string 结果电流
        {
            set { _结果电流 = value; }
            get { return _结果电流; }
        }
        #endregion Model

    }
}
