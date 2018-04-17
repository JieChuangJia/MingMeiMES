using System;
namespace DBAccess.Model
{
    /// <summary>
    /// BatteryModule:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class BatteryModuleModel
    {
       public BatteryModuleModel()
		{}
		#region Model
		private string _batmoduleid;
		private string _batchname;
		private DateTime _asmtime;
		private string _curprocessstage;
		private string _batpackid;
		private string _topcapopworkerid;
		private string _downcapopworkerid;
		private string _palletid;
		private bool _palletbinded;
		private int? _topcapwelderid;
		private int? _bottomcapwelderid;
		private int? _checkresult;
		private string _tag1;
		private string _tag2;
		private string _tag3;
		private string _tag4;
		private string _tag5;
		/// <summary>
		/// 模组条码
		/// </summary>
		public string batModuleID
		{
			set{ _batmoduleid=value;}
			get{return _batmoduleid;}
		}
		/// <summary>
		/// 批次
		/// </summary>
		public string batchName
		{
			set{ _batchname=value;}
			get{return _batchname;}
		}
		/// <summary>
		/// 组装时间
		/// </summary>
		public DateTime asmTime
		{
			set{ _asmtime=value;}
			get{return _asmtime;}
		}
		/// <summary>
		/// 当前所在工艺
		/// </summary>
		public string curProcessStage
		{
			set{ _curprocessstage=value;}
			get{return _curprocessstage;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string batPackID
		{
			set{ _batpackid=value;}
			get{return _batpackid;}
		}
		/// <summary>
		/// 封上盖操作的员工编号
		/// </summary>
		public string topcapOPWorkerID
		{
			set{ _topcapopworkerid=value;}
			get{return _topcapopworkerid;}
		}
		/// <summary>
		/// 封下盖操作的员工编号
		/// </summary>
		public string downcapOPWorkerID
		{
			set{ _downcapopworkerid=value;}
			get{return _downcapopworkerid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string palletID
		{
			set{ _palletid=value;}
			get{return _palletid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public bool palletBinded
		{
			set{ _palletbinded=value;}
			get{return _palletbinded;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? topcapWelderID
		{
			set{ _topcapwelderid=value;}
			get{return _topcapwelderid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? bottomcapWelderID
		{
			set{ _bottomcapwelderid=value;}
			get{return _bottomcapwelderid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? checkResult
		{
			set{ _checkresult=value;}
			get{return _checkresult;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string tag1
		{
			set{ _tag1=value;}
			get{return _tag1;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string tag2
		{
			set{ _tag2=value;}
			get{return _tag2;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string tag3
		{
			set{ _tag3=value;}
			get{return _tag3;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string tag4
		{
			set{ _tag4=value;}
			get{return _tag4;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string tag5
		{
			set{ _tag5=value;}
			get{return _tag5;}
		}
		#endregion Model

    }
}

