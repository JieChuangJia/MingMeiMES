using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DevInterface;
using Oracle.DataAccess.Types;
using Oracle.ManagedDataAccess.Client;

namespace DevAccess
{
    	/// <summary>
	/// MES_STEP_INFO:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public class FT_MES_STEP_INFOModel
	{
		public FT_MES_STEP_INFOModel()
		{}
		#region Model
		private string _recid;
		private string _serial_number;
		private string _step_number;
        private decimal _check_result;
		private decimal _step_mark;
		private decimal _status;
		private DateTime _trx_time;
		private DateTime _last_modify_time;
		private string _defect_codes;
		private string _user_name;
		private string _reason;
		/// <summary>
		/// 
		/// </summary>
		public string RECID
		{
			set{ _recid=value;}
			get{return _recid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string SERIAL_NUMBER
		{
			set{ _serial_number=value;}
			get{return _serial_number;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string STEP_NUMBER
		{
			set{ _step_number=value;}
			get{return _step_number;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal CHECK_RESULT
		{
			set{ _check_result=value;}
			get{return _check_result;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal STEP_MARK
		{
			set{ _step_mark=value;}
			get{return _step_mark;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal STATUS
		{
			set{ _status=value;}
			get{return _status;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime TRX_TIME
		{
			set{ _trx_time=value;}
			get{return _trx_time;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime LAST_MODIFY_TIME
		{
			set{ _last_modify_time=value;}
			get{return _last_modify_time;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string DEFECT_CODES
		{
			set{ _defect_codes=value;}
			get{return _defect_codes;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string USER_NAME
		{
			set{ _user_name=value;}
			get{return _user_name;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string REASON
		{
			set{ _reason=value;}
			get{return _reason;}
		}
		#endregion Model

	}
    /// <summary>
    /// FT_MES_STEP_INFO_DETAILModel:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public class FT_MES_STEP_INFO_DETAILModel
    {
        public FT_MES_STEP_INFO_DETAILModel()
        { }
        #region Model
        private string _recid;
        private string _serial_number;
        private string _step_number;
        private decimal _status;
        private DateTime _trx_time;
        private DateTime _last_modify_time;
        private string _data_name;
        private string _data_value;
        /// <summary>
        /// 
        /// </summary>
        public string RECID
        {
            set { _recid = value; }
            get { return _recid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string SERIAL_NUMBER
        {
            set { _serial_number = value; }
            get { return _serial_number; }
        }
        public string STEP_NUMBER 
        {
            get { return _step_number; }
            set { _step_number = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal STATUS
        {
            set { _status = value; }
            get { return _status; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime TRX_TIME
        {
            set { _trx_time = value; }
            get { return _trx_time; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime LAST_MODIFY_TIME
        {
            set { _last_modify_time = value; }
            get { return _last_modify_time; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string DATA_NAME
        {
            set { _data_name = value; }
            get { return _data_name; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string DATA_VALUE
        {
            set { _data_value = value; }
            get { return _data_value; }
        }
        #endregion Model

    }
    /// <summary>
    /// 数据访问类:MES_STEP_INFOModel
    /// </summary>
    //public partial class MES_STEP_INFODal
    //{
    //    public MES_STEP_INFODal()
    //    { }
    //    #region  BasicMethod

    //    /// <summary>
    //    /// 增加一条数据
    //    /// </summary>
    //    public bool Add(MES_STEP_INFOModel model)
    //    {
    //        StringBuilder strSql = new StringBuilder();
    //        strSql.Append("insert into LOCAL_MES_STEP_INFO(");
    //        strSql.Append("RECID,SERIAL_NUMBER,STEP_NUMBER,CHECK_RESULT,STEP_MARK,STATUS,TRX_TIME,LAST_MODIFY_TIME,DEFECT_CODES,USER_NAME,REASON)");
    //        strSql.Append(" values (");
    //        strSql.Append(":RECID,:SERIAL_NUMBER,:STEP_NUMBER,:CHECK_RESULT,:STEP_MARK,:STATUS,:TRX_TIME,:LAST_MODIFY_TIME,:DEFECT_CODES,:USER_NAME,:REASON)");
    //        OracleParameter[] parameters = {
    //                new OracleParameter(":RECID", Oracle.DataAccess.Types.OracleS),
    //                new OracleParameter(":SERIAL_NUMBER", OracleType.NVarChar),
    //                new OracleParameter(":STEP_NUMBER", OracleType.NVarChar),
    //                new OracleParameter(":CHECK_RESULT", OracleType.Number,4),
    //                new OracleParameter(":STEP_MARK", OracleType.Number,4),
    //                new OracleParameter(":STATUS", OracleType.Number,4),
    //                new OracleParameter(":TRX_TIME", OracleType.DateTime),
    //                new OracleParameter(":LAST_MODIFY_TIME", OracleType.DateTime),
    //                new OracleParameter(":DEFECT_CODES", OracleType.NVarChar),
    //                new OracleParameter(":USER_NAME", OracleType.NVarChar),
    //                new OracleParameter(":REASON", OracleType.NVarChar)};
    //        parameters[0].Value = model.RECID;
    //        parameters[1].Value = model.SERIAL_NUMBER;
    //        parameters[2].Value = model.STEP_NUMBER;
    //        parameters[3].Value = model.CHECK_RESULT;
    //        parameters[4].Value = model.STEP_MARK;
    //        parameters[5].Value = model.STATUS;
    //        parameters[6].Value = model.TRX_TIME;
    //        parameters[7].Value = model.LAST_MODIFY_TIME;
    //        parameters[8].Value = model.DEFECT_CODES;
    //        parameters[9].Value = model.USER_NAME;
    //        parameters[10].Value = model.REASON;

    //        int rows = DbHelperOra.ExecuteSql(strSql.ToString(), parameters);
    //        if (rows > 0)
    //        {
    //            return true;
    //        }
    //        else
    //        {
    //            return false;
    //        }
    //    }
    //    /// <summary>
    //    /// 更新一条数据
    //    /// </summary>
    //    public bool Update(MES_STEP_INFOModel model)
    //    {
    //        StringBuilder strSql = new StringBuilder();
    //        strSql.Append("update LOCAL_MES_STEP_INFO set ");
    //        strSql.Append("SERIAL_NUMBER=:SERIAL_NUMBER,");
    //        strSql.Append("STEP_NUMBER=:STEP_NUMBER,");
    //        strSql.Append("CHECK_RESULT=:CHECK_RESULT,");
    //        strSql.Append("STEP_MARK=:STEP_MARK,");
    //        strSql.Append("STATUS=:STATUS,");
    //        strSql.Append("TRX_TIME=:TRX_TIME,");
    //        strSql.Append("LAST_MODIFY_TIME=:LAST_MODIFY_TIME,");
    //        strSql.Append("DEFECT_CODES=:DEFECT_CODES,");
    //        strSql.Append("USER_NAME=:USER_NAME,");
    //        strSql.Append("REASON=:REASON");
    //        strSql.Append(" where RECID=:RECID ");
    //        OracleParameter[] parameters = {
    //                new OracleParameter(":SERIAL_NUMBER", OracleType.NVarChar),
    //                new OracleParameter(":STEP_NUMBER", OracleType.NVarChar),
    //                new OracleParameter(":CHECK_RESULT", OracleType.Number,4),
    //                new OracleParameter(":STEP_MARK", OracleType.Number,4),
    //                new OracleParameter(":STATUS", OracleType.Number,4),
    //                new OracleParameter(":TRX_TIME", OracleType.DateTime),
    //                new OracleParameter(":LAST_MODIFY_TIME", OracleType.DateTime),
    //                new OracleParameter(":DEFECT_CODES", OracleType.NVarChar),
    //                new OracleParameter(":USER_NAME", OracleType.NVarChar),
    //                new OracleParameter(":REASON", OracleType.NVarChar),
    //                new OracleParameter(":RECID", OracleType.NVarChar)};
    //        parameters[0].Value = model.SERIAL_NUMBER;
    //        parameters[1].Value = model.STEP_NUMBER;
    //        parameters[2].Value = model.CHECK_RESULT;
    //        parameters[3].Value = model.STEP_MARK;
    //        parameters[4].Value = model.STATUS;
    //        parameters[5].Value = model.TRX_TIME;
    //        parameters[6].Value = model.LAST_MODIFY_TIME;
    //        parameters[7].Value = model.DEFECT_CODES;
    //        parameters[8].Value = model.USER_NAME;
    //        parameters[9].Value = model.REASON;
    //        parameters[10].Value = model.RECID;

    //        int rows = DbHelperOra.ExecuteSql(strSql.ToString(), parameters);
    //        if (rows > 0)
    //        {
    //            return true;
    //        }
    //        else
    //        {
    //            return false;
    //        }
    //    }

    //    /// <summary>
    //    /// 删除一条数据
    //    /// </summary>
    //    public bool Delete(string RECID)
    //    {

    //        StringBuilder strSql = new StringBuilder();
    //        strSql.Append("delete from LOCAL_MES_STEP_INFO ");
    //        strSql.Append(" where RECID=:RECID ");
    //        OracleParameter[] parameters = {
    //                new OracleParameter(":RECID", OracleType.NVarChar)			};
    //        parameters[0].Value = RECID;

    //        int rows = DbHelperOra.ExecuteSql(strSql.ToString(), parameters);
    //        if (rows > 0)
    //        {
    //            return true;
    //        }
    //        else
    //        {
    //            return false;
    //        }
    //    }
    //    /// <summary>
    //    /// 批量删除数据
    //    /// </summary>
    //    public bool DeleteList(string RECIDlist)
    //    {
    //        StringBuilder strSql = new StringBuilder();
    //        strSql.Append("delete from LOCAL_MES_STEP_INFO ");
    //        strSql.Append(" where RECID in (" + RECIDlist + ")  ");
    //        int rows = DbHelperOra.ExecuteSql(strSql.ToString());
    //        if (rows > 0)
    //        {
    //            return true;
    //        }
    //        else
    //        {
    //            return false;
    //        }
    //    }


    //    /// <summary>
    //    /// 得到一个对象实体
    //    /// </summary>
    //    public MES_STEP_INFOModel GetModel(string RECID)
    //    {

    //        StringBuilder strSql = new StringBuilder();
    //        strSql.Append("select RECID,SERIAL_NUMBER,STEP_NUMBER,CHECK_RESULT,STEP_MARK,STATUS,TRX_TIME,LAST_MODIFY_TIME,DEFECT_CODES,USER_NAME,REASON from LOCAL_MES_STEP_INFO ");
    //        strSql.Append(" where RECID=:RECID ");
    //        OracleParameter[] parameters = {
    //                new OracleParameter(":RECID", OracleType.NVarChar)			};
    //        parameters[0].Value = RECID;

    //        Maticsoft.Model.MES_STEP_INFOModel model = new Maticsoft.Model.MES_STEP_INFOModel();
    //        DataSet ds = DbHelperOra.Query(strSql.ToString(), parameters);
    //        if (ds.Tables[0].Rows.Count > 0)
    //        {
    //            return DataRowToModel(ds.Tables[0].Rows[0]);
    //        }
    //        else
    //        {
    //            return null;
    //        }
    //    }


    //    /// <summary>
    //    /// 得到一个对象实体
    //    /// </summary>
    //    public MES_STEP_INFOModel DataRowToModel(DataRow row)
    //    {
    //        MES_STEP_INFOModel model = new MES_STEP_INFOModel();
    //        if (row != null)
    //        {
    //            if (row["RECID"] != null)
    //            {
    //                model.RECID = row["RECID"].ToString();
    //            }
    //            if (row["SERIAL_NUMBER"] != null)
    //            {
    //                model.SERIAL_NUMBER = row["SERIAL_NUMBER"].ToString();
    //            }
    //            if (row["STEP_NUMBER"] != null)
    //            {
    //                model.STEP_NUMBER = row["STEP_NUMBER"].ToString();
    //            }
    //            if (row["CHECK_RESULT"] != null && row["CHECK_RESULT"].ToString() != "")
    //            {
    //                model.CHECK_RESULT = decimal.Parse(row["CHECK_RESULT"].ToString());
    //            }
    //            if (row["STEP_MARK"] != null && row["STEP_MARK"].ToString() != "")
    //            {
    //                model.STEP_MARK = decimal.Parse(row["STEP_MARK"].ToString());
    //            }
    //            if (row["STATUS"] != null && row["STATUS"].ToString() != "")
    //            {
    //                model.STATUS = decimal.Parse(row["STATUS"].ToString());
    //            }
    //            if (row["TRX_TIME"] != null && row["TRX_TIME"].ToString() != "")
    //            {
    //                model.TRX_TIME = DateTime.Parse(row["TRX_TIME"].ToString());
    //            }
    //            if (row["LAST_MODIFY_TIME"] != null && row["LAST_MODIFY_TIME"].ToString() != "")
    //            {
    //                model.LAST_MODIFY_TIME = DateTime.Parse(row["LAST_MODIFY_TIME"].ToString());
    //            }
    //            if (row["DEFECT_CODES"] != null)
    //            {
    //                model.DEFECT_CODES = row["DEFECT_CODES"].ToString();
    //            }
    //            if (row["USER_NAME"] != null)
    //            {
    //                model.USER_NAME = row["USER_NAME"].ToString();
    //            }
    //            if (row["REASON"] != null)
    //            {
    //                model.REASON = row["REASON"].ToString();
    //            }
    //        }
    //        return model;
    //    }

    //    /// <summary>
    //    /// 获得数据列表
    //    /// </summary>
    //    public DataSet GetList(string strWhere)
    //    {
    //        StringBuilder strSql = new StringBuilder();
    //        strSql.Append("select RECID,SERIAL_NUMBER,STEP_NUMBER,CHECK_RESULT,STEP_MARK,STATUS,TRX_TIME,LAST_MODIFY_TIME,DEFECT_CODES,USER_NAME,REASON ");
    //        strSql.Append(" FROM LOCAL_MES_STEP_INFO ");
    //        if (strWhere.Trim() != "")
    //        {
    //            strSql.Append(" where " + strWhere);
    //        }
    //        return DbHelperOra.Query(strSql.ToString());
    //    }

    //    /// <summary>
    //    /// 获取记录总数
    //    /// </summary>
    //    public int GetRecordCount(string strWhere)
    //    {
    //        StringBuilder strSql = new StringBuilder();
    //        strSql.Append("select count(1) FROM LOCAL_MES_STEP_INFO ");
    //        if (strWhere.Trim() != "")
    //        {
    //            strSql.Append(" where " + strWhere);
    //        }
    //        object obj = DbHelperSQL.GetSingle(strSql.ToString());
    //        if (obj == null)
    //        {
    //            return 0;
    //        }
    //        else
    //        {
    //            return Convert.ToInt32(obj);
    //        }
    //    }
    //    #endregion  BasicMethod
    //    #region  ExtensionMethod

    //    #endregion  ExtensionMethod
    //}
    public class MesDA : IMesAccess
    {
        #region 数据
        protected string mesdbConnstr = "";
        protected OracleConnection dbConn = new OracleConnection();
        MesWS.EventService ws = new MesWS.EventService();
        #endregion
       
        public MesDA()
        {
            int localSim = 0;
            if(localSim==0)
            {
                mesdbConnstr = @"Data Source=(DESCRIPTION =
    (ADDRESS = (PROTOCOL = TCP)(HOST = 192.168.100.94)(PORT = 1521))
        (CONNECT_DATA =
          (SERVER = DEDICATED)
          (SERVICE_NAME = PRQMESDB)
        )
    );User Id=prqminda1;Password=prqminda1;Connection Timeout=5;";
            }
            else
            {
                // 本地模拟MES db       
                mesdbConnstr = @"Data Source=(DESCRIPTION =
    (ADDRESS = (PROTOCOL = TCP)(HOST =127.0.0.1)(PORT = 1521))
    (CONNECT_DATA =
      (SERVER = DEDICATED)
      (SERVICE_NAME = XE)
    )
  )
;User Id=MYDB;Password=atscu;Connection Timeout=5;";
            }

            
        }
        #region 公共接口
         public string MesdbConnstr
        {
            get { return mesdbConnstr; }
            set { mesdbConnstr = value; }
        }

        public bool ConnDB(ref string reStr)
        {
            try 
	        {
                this.dbConn.ConnectionString = this.mesdbConnstr;
                if (this.dbConn.State != ConnectionState.Open)
                {
                    this.dbConn.Open();
                }
                
                reStr = "MES数据库连接OK";
                return true;
	        }
	        catch (Exception ex)
	        {
                reStr = ex.ToString();
                return false;
	        }
        }
        public bool DisconnDB(ref string reStr)
        {
            try
            {
                this.dbConn.Close();
                reStr = "数据库连接关闭";
                return true;
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return false;
            }
        }
        public  DataTable ReadMesDataTable(string tableName,ref string reStr)
        {
            try
            {
                 using (OracleConnection conn = new OracleConnection(this.mesdbConnstr))
                 {
                     if (conn.State != ConnectionState.Open)
                         conn.Open();
                     // string sql = "select * from all_tables where owner = 'PRQMINDA1'";
                     string sql = string.Format("select * from {0} where rownum <=10", tableName);
                     OracleCommand com = new OracleCommand(sql, conn);
                     OracleDataReader dr = com.ExecuteReader();
                     DataTable dt = new DataTable();
                     int fieldcout = dr.FieldCount;
                     if (dr.FieldCount > 0)
                     {
                         for (int i = 0; i < dr.FieldCount; i++)
                         {
                             DataColumn dc = new DataColumn(dr.GetName(i), dr.GetFieldType(i));
                             dt.Columns.Add(dc);
                         }
                         object[] rowobject = new object[dr.FieldCount];
                         while (dr.Read())
                         {
                             dr.GetValues(rowobject);
                             dt.LoadDataRow(rowobject, true);
                         }
                     }
                     dr.Close();
                     reStr = "查询数据成功";
                     return dt;
                 }
                
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return null;
            }
           
        }
        public DataTable ReadMesTable(string strSql)
        {
            using (OracleConnection conn = new OracleConnection(this.mesdbConnstr))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                OracleCommand com = new OracleCommand(strSql, conn);
                OracleDataReader dr = com.ExecuteReader();
                DataTable dt = new DataTable();
                int fieldcout = dr.FieldCount;
                if (dr.FieldCount > 0)
                {
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        DataColumn dc = new DataColumn(dr.GetName(i), dr.GetFieldType(i));
                        dt.Columns.Add(dc);
                    }
                    object[] rowobject = new object[dr.FieldCount];
                    while (dr.Read())
                    {
                        dr.GetValues(rowobject);
                        dt.LoadDataRow(rowobject, true);
                    }
                    dr.Close();
                    return dt;
                }
                else
                {
                    dr.Close();
                    return null;
                }
            }
            
            
        }
        public bool MesBaseExist(string rcid)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from FT_MES_STEP_INFO");
            strSql.AppendFormat(" where RECID='{0}'",rcid);
            //OracleParameter[] parameters = {
            //        new OracleParameter(":RECID", OracleDbType.NVarchar2,255)			};
            //parameters[0].Value = rcid;
           
            return Exists(strSql.ToString());
        }
        public bool MesDetailExist(string rcid)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from FT_MES_STEP_INFO_DETAIL");
            strSql.AppendFormat(" where RECID='{0}'", rcid);
            //OracleParameter[] parameters = {
            //        new OracleParameter(":RECID", OracleDbType.NVarchar2,255)			};
            //parameters[0].Value = rcid;

            return Exists(strSql.ToString());
        }
        public bool AddMesBaseinfo(FT_MES_STEP_INFOModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into FT_MES_STEP_INFO(");
            strSql.Append("RECID,SERIAL_NUMBER,STEP_NUMBER,CHECK_RESULT,STEP_MARK,STATUS,TRX_TIME,LAST_MODIFY_TIME,DEFECT_CODES,USER_NAME,REASON)");
            strSql.Append(" values (");
            strSql.Append(":RECID,:SERIAL_NUMBER,:STEP_NUMBER,:CHECK_RESULT,:STEP_MARK,:STATUS,:TRX_TIME,:LAST_MODIFY_TIME,:DEFECT_CODES,:USER_NAME,:REASON)");
            OracleParameter[] parameters = {
					new OracleParameter(":RECID", OracleDbType.NVarchar2,255),
					new OracleParameter(":SERIAL_NUMBER", OracleDbType.NVarchar2,255),
					new OracleParameter(":STEP_NUMBER", OracleDbType.NVarchar2,255),
					new OracleParameter(":CHECK_RESULT", OracleDbType.Int32,4),
					new OracleParameter(":STEP_MARK", OracleDbType.Int32,4),
					new OracleParameter(":STATUS", OracleDbType.Int32,4),
					new OracleParameter(":TRX_TIME", OracleDbType.Date),
					new OracleParameter(":LAST_MODIFY_TIME",  OracleDbType.Date),
					new OracleParameter(":DEFECT_CODES",  OracleDbType.NVarchar2,255),
					new OracleParameter(":USER_NAME",  OracleDbType.NVarchar2,255),
					new OracleParameter(":REASON",  OracleDbType.NVarchar2,2000)};
            parameters[0].Value = model.RECID;
            parameters[1].Value = model.SERIAL_NUMBER;
            parameters[2].Value = model.STEP_NUMBER;
            parameters[3].Value = model.CHECK_RESULT;
            parameters[4].Value = model.STEP_MARK;
            parameters[5].Value = model.STATUS;
            parameters[6].Value = model.TRX_TIME;
            parameters[7].Value = model.LAST_MODIFY_TIME;
            parameters[8].Value = model.DEFECT_CODES;
            parameters[9].Value = model.USER_NAME;
            parameters[10].Value = model.REASON;
            using (OracleConnection conn = new OracleConnection(this.mesdbConnstr))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                       
                        if (conn.State != ConnectionState.Open)
                            conn.Open();
                        OracleTransaction m_OraTrans = conn.BeginTransaction();//创建事务对象
                        PrepareCommand(cmd, conn, strSql.ToString(), parameters);
                        int rows = cmd.ExecuteNonQuery();
                        m_OraTrans.Commit();
                        cmd.Parameters.Clear();
                        if (rows > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    catch (OracleException ex)
                    {
                        throw ex;
                     
                    }
                   
                }
            }
        }
        public bool AddMesDetailinfo(FT_MES_STEP_INFO_DETAILModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into FT_MES_STEP_INFO_DETAIL(");
            strSql.Append("RECID,SERIAL_NUMBER,STEP_NUMBER,STATUS,TRX_TIME,LAST_MODIFY_TIME,DATA_NAME,DATA_VALUE)");
            strSql.Append(" values (");
            strSql.Append(":RECID,:SERIAL_NUMBER,:STEP_NUMBER,:STATUS,:TRX_TIME,:LAST_MODIFY_TIME,:DATA_NAME,:DATA_VALUE)");
            OracleParameter[] parameters = {
					new OracleParameter(":RECID", OracleDbType.NVarchar2,255),
					new OracleParameter(":SERIAL_NUMBER", OracleDbType.NVarchar2,255),
                    new OracleParameter(":STEP_NUMBER", OracleDbType.NVarchar2,255),
					new OracleParameter(":STATUS", OracleDbType.Int32,4),
					new OracleParameter(":TRX_TIME", OracleDbType.Date),
					new OracleParameter(":LAST_MODIFY_TIME", OracleDbType.Date),
					new OracleParameter(":DATA_NAME", OracleDbType.NVarchar2,255),
					new OracleParameter(":DATA_VALUE", OracleDbType.NVarchar2,500)};
            parameters[0].Value = model.RECID;
            parameters[1].Value = model.SERIAL_NUMBER;
            parameters[2].Value = model.STEP_NUMBER;
            parameters[3].Value = model.STATUS;
            parameters[4].Value = model.TRX_TIME;
            parameters[5].Value = model.LAST_MODIFY_TIME;
            parameters[6].Value = model.DATA_NAME;
            parameters[7].Value = model.DATA_VALUE;
            using (OracleConnection conn = new OracleConnection(this.mesdbConnstr))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        if (conn.State != ConnectionState.Open)
                            conn.Open();
                        PrepareCommand(cmd, conn, strSql.ToString(), parameters);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        if (rows > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    catch (OracleException ex)
                    {
                        
                        throw ex;
                    }
                   
                }
            }

        }
        /// <summary>
        /// 投产
        /// </summary>
        /// <param name="paramArray"></param>
        /// <param name="reStr"></param>
        /// <returns>0:禁止投产，1：允许投产，2:服务调用返回数据错误，3：MES服务连接不上</returns>
        public int MesAssemAuto(string[] paramArray,ref string reStr)
        {
            try
            {
                return WsMethodCall("assembleAuto", paramArray,ref reStr);
                
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return 3;
               
            }
        }
        /// <summary>
        /// MES下线许可查询web service服务接口调用
        /// </summary>
        /// <param name="paramArray"></param>
        /// <param name="reStr"></param>
        /// <returns>0:允许下线，1：禁止下线，3：异常发生</returns>
        public int MesAssemDown(string[] paramArray,ref string reStr)
        {
            try
            {
                return WsMethodCall("assembleDown", paramArray, ref reStr);

            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return 3;

            }
        }
        /// <summary>
        /// MES是否允许下线查询接口，包括查询MES数据库判断
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="downQueryMesID"></param>
        /// <param name="reStr"></param>
        /// <returns>0:允许下线，1:禁止下线，2:无法确定，3：异常</returns>
        public int MesDownEnabled(string mesLinID,string barcode,string downQueryMesID,ref string reStr)
        {
            try
            {
                int re = 0;
                //1 先查询MES数据库，是否有错误发生
                string sql = string.Format("select * from FT_MES_STEP_INFO where SERIAL_NUMBER='{0}' and STEP_NUMBER='{1}' order by TRX_TIME desc", barcode, downQueryMesID);
                DataTable dt = ReadMesTable(sql);
                if(dt.Rows.Count>0)
                {
                    int stat = int.Parse(dt.Rows[0]["STATUS"].ToString());
                    if(stat == 2)
                    {
                        //mes处置结果为2，有错误发生，禁止下线
                        reStr = dt.Rows[0]["REASON"].ToString();
                        re = 1;
                        return re;
                    }
                }
                //2 查询MES服务接口
                string[] wsParam = new string[] { barcode, mesLinID };
                re=WsMethodCall("assembleDown", wsParam, ref reStr);
                if(re == 1)
                {
                    re = 2;//最后一个关键工位信息为空的情况下，如果下线许可失败，可能是因为网络延迟，数据未及时上传到MES数据库

                }
                return re;

            }
            catch (Exception ex)
            {
                reStr = ex.Message;
                return 3;

            }
        }

        /// <summary>
        /// 维修审核是否完成
        /// </summary>
        /// <param name="paramArray"></param>
        /// <param name="reStr"></param>
        /// <returns></returns>
        public int MesReAssemEnabled(string[] paramArray,ref string reStr)
        {
            
            try
            {
                return WsMethodCall("assembleRepair", paramArray, ref reStr);

            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return 3;

            }
        }
        
        /// <summary>
        /// 查询MES是否已经下线
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="reStr"></param>
        /// <returns>1: 下线，2：未下线，3：数据库无法访问</returns>
        public int MesDowned(string barcode,string mesDownStatName,ref string reStr)
        {
            try
            {
                string sql = string.Format("select * from FT_MES_STEP_INFO where SERIAL_NUMBER='{0}' and STEP_NUMBER='{1}'", barcode, mesDownStatName);
                DataTable dt = ReadMesTable(sql);
                if(dt.Rows.Count>0)
                {
                    return 1;
                }
                else
                {
                    return 2;
                }
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return 3;
            }
        }
        #endregion
        #region 私有接口
        private int WsMethodCall(string methodName, string[] paramArray,ref string reStr)
        {
            try
            {
                string[] reArray = ws.signalEvent2(methodName, paramArray);
                if (reArray == null ||reArray.Count() < 1)
                {
                    reStr = "MES返回数据错误";
                    return 2;
                }
                else if (reArray[0].Trim().Substring(0, 1) == "1")
                {
                    reStr = reArray[0].Trim().Substring(1);
                    return 1;
                }
                else if (reArray[0].Trim().Substring(0, 1) == "0")
                {
                    reStr = reArray[0].Trim().Substring(1);
                    return 0;
                }
                else
                {
                    reStr = "MES返回数据错误," + reArray[0];
                    return 2;
                }
               
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return 3;
            }
           
        }
        private void PrepareCommand(OracleCommand cmd, OracleConnection conn, string cmdText, OracleParameter[] cmdParms)
        {
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = CommandType.Text;
            if (cmdParms != null)
            {
                foreach (OracleParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
               (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }
        private  object GetSingle(string SQLString)
        {
            using (OracleConnection connection = new OracleConnection(this.mesdbConnstr))
            {
                using (OracleCommand cmd = new OracleCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }
        private bool Exists(string strSql)
        {
            using (OracleConnection conn = new OracleConnection(this.mesdbConnstr))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();
                    object obj = GetSingle(strSql.ToString());
                    int cmdresult;
                    if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                    {
                        cmdresult = 0;
                    }
                    else
                    {
                        cmdresult = int.Parse(obj.ToString());
                    }
                    if (cmdresult == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }
        #endregion
        
    }
}
