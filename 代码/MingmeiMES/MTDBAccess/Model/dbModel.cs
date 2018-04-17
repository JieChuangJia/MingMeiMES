using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTDBAccess.Model
{ 
        /// <summary>
	/// db:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class dbModel
	{
        public dbModel()
		{}
		#region Model
		private string _测试时间;
		private string _二维码;
		private string _正螺丝数据;
		private string _反螺丝数据;
		/// <summary>
		/// 
		/// </summary>
		public string 测试时间
		{
			set{ _测试时间=value;}
			get{return _测试时间;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string 二维码
		{
			set{ _二维码=value;}
			get{return _二维码;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string 正螺丝数据
		{
			set{ _正螺丝数据=value;}
			get{return _正螺丝数据;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string 反螺丝数据
		{
			set{ _反螺丝数据=value;}
			get{return _反螺丝数据;}
		}
		#endregion Model

    }
}
