using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using FTDataAccess.DBUtility;//Please add references
namespace FTDataAccess.DAL
{
    /// <summary>
    /// 数据访问类:LOCAL_MES_STEP_INFO_DETAILModel
    /// </summary>
    public partial class LOCAL_MES_STEP_INFO_DETAILDal
    {
        public LOCAL_MES_STEP_INFO_DETAILDal()
        { }
        #region  BasicMethod
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string RECID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from LOCAL_MES_STEP_INFO_DETAIL");
            strSql.Append(" where RECID=@RECID ");
            SqlParameter[] parameters = {
					new SqlParameter("@RECID", SqlDbType.NVarChar,255)			};
            parameters[0].Value = RECID;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(FTDataAccess.Model.LOCAL_MES_STEP_INFO_DETAILModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into LOCAL_MES_STEP_INFO_DETAIL(");
            strSql.Append("SERIAL_NUMBER,STEP_NUMBER,STATUS,DATA_NAME,DATA_VALUE,TRX_TIME,LAST_MODIFY_TIME,RECID,UPLOAD_FLAG,AutoStationName)");
            strSql.Append(" values (");
            strSql.Append("@SERIAL_NUMBER,@STEP_NUMBER,@STATUS,@DATA_NAME,@DATA_VALUE,@TRX_TIME,@LAST_MODIFY_TIME,@RECID,@UPLOAD_FLAG,@AutoStationName)");
            SqlParameter[] parameters = {
					new SqlParameter("@SERIAL_NUMBER", SqlDbType.NVarChar,255),
					new SqlParameter("@STEP_NUMBER", SqlDbType.NVarChar,255),
					new SqlParameter("@STATUS", SqlDbType.Int,4),
					new SqlParameter("@DATA_NAME", SqlDbType.NVarChar,255),
					new SqlParameter("@DATA_VALUE", SqlDbType.NVarChar,500),
					new SqlParameter("@TRX_TIME", SqlDbType.DateTime),
					new SqlParameter("@LAST_MODIFY_TIME", SqlDbType.DateTime),
					new SqlParameter("@RECID", SqlDbType.NVarChar,255),
					new SqlParameter("@UPLOAD_FLAG", SqlDbType.Bit,1),
					new SqlParameter("@AutoStationName", SqlDbType.NVarChar,255)};
            parameters[0].Value = model.SERIAL_NUMBER;
            parameters[1].Value = model.STEP_NUMBER;
            parameters[2].Value = model.STATUS;
            parameters[3].Value = model.DATA_NAME;
            parameters[4].Value = model.DATA_VALUE;
            parameters[5].Value = model.TRX_TIME;
            parameters[6].Value = model.LAST_MODIFY_TIME;
            parameters[7].Value = model.RECID;
            parameters[8].Value = model.UPLOAD_FLAG;
            parameters[9].Value = model.AutoStationName;

            int rows = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(FTDataAccess.Model.LOCAL_MES_STEP_INFO_DETAILModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update LOCAL_MES_STEP_INFO_DETAIL set ");
            strSql.Append("SERIAL_NUMBER=@SERIAL_NUMBER,");
            strSql.Append("STEP_NUMBER=@STEP_NUMBER,");
            strSql.Append("STATUS=@STATUS,");
            strSql.Append("DATA_NAME=@DATA_NAME,");
            strSql.Append("DATA_VALUE=@DATA_VALUE,");
            strSql.Append("TRX_TIME=@TRX_TIME,");
            strSql.Append("LAST_MODIFY_TIME=@LAST_MODIFY_TIME,");
            strSql.Append("UPLOAD_FLAG=@UPLOAD_FLAG,");
            strSql.Append("AutoStationName=@AutoStationName");
            strSql.Append(" where RECID=@RECID ");
            SqlParameter[] parameters = {
					new SqlParameter("@SERIAL_NUMBER", SqlDbType.NVarChar,255),
					new SqlParameter("@STEP_NUMBER", SqlDbType.NVarChar,255),
					new SqlParameter("@STATUS", SqlDbType.Int,4),
					new SqlParameter("@DATA_NAME", SqlDbType.NVarChar,255),
					new SqlParameter("@DATA_VALUE", SqlDbType.NVarChar,500),
					new SqlParameter("@TRX_TIME", SqlDbType.DateTime),
					new SqlParameter("@LAST_MODIFY_TIME", SqlDbType.DateTime),
					new SqlParameter("@UPLOAD_FLAG", SqlDbType.Bit,1),
					new SqlParameter("@AutoStationName", SqlDbType.NVarChar,255),
					new SqlParameter("@RECID", SqlDbType.NVarChar,255)};
            parameters[0].Value = model.SERIAL_NUMBER;
            parameters[1].Value = model.STEP_NUMBER;
            parameters[2].Value = model.STATUS;
            parameters[3].Value = model.DATA_NAME;
            parameters[4].Value = model.DATA_VALUE;
            parameters[5].Value = model.TRX_TIME;
            parameters[6].Value = model.LAST_MODIFY_TIME;
            parameters[7].Value = model.UPLOAD_FLAG;
            parameters[8].Value = model.AutoStationName;
            parameters[9].Value = model.RECID;

            int rows = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string RECID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from LOCAL_MES_STEP_INFO_DETAIL ");
            strSql.Append(" where RECID=@RECID ");
            SqlParameter[] parameters = {
					new SqlParameter("@RECID", SqlDbType.NVarChar,255)			};
            parameters[0].Value = RECID;

            int rows = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 批量删除数据
        /// </summary>
        public bool DeleteList(string RECIDlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from LOCAL_MES_STEP_INFO_DETAIL ");
            strSql.Append(" where RECID in (" + RECIDlist + ")  ");
            int rows = DbHelperSQL.ExecuteSql(strSql.ToString());
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public FTDataAccess.Model.LOCAL_MES_STEP_INFO_DETAILModel GetModel(string RECID)
        {


            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 SERIAL_NUMBER,STEP_NUMBER,STATUS,DATA_NAME,DATA_VALUE,TRX_TIME,LAST_MODIFY_TIME,RECID,UPLOAD_FLAG,AutoStationName from LOCAL_MES_STEP_INFO_DETAIL ");
            strSql.Append(" where RECID=@RECID ");
            SqlParameter[] parameters = {
					new SqlParameter("@RECID", SqlDbType.NVarChar,255)			};
            parameters[0].Value = RECID;

            FTDataAccess.Model.LOCAL_MES_STEP_INFO_DETAILModel model = new FTDataAccess.Model.LOCAL_MES_STEP_INFO_DETAILModel();
            DataSet ds = DbHelperSQL.Query(strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return DataRowToModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public FTDataAccess.Model.LOCAL_MES_STEP_INFO_DETAILModel DataRowToModel(DataRow row)
        {
            FTDataAccess.Model.LOCAL_MES_STEP_INFO_DETAILModel model = new FTDataAccess.Model.LOCAL_MES_STEP_INFO_DETAILModel();
            if (row != null)
            {
                if (row["SERIAL_NUMBER"] != null)
                {
                    model.SERIAL_NUMBER = row["SERIAL_NUMBER"].ToString();
                }
                if (row["STEP_NUMBER"] != null)
                {
                    model.STEP_NUMBER = row["STEP_NUMBER"].ToString();
                }
                if (row["STATUS"] != null && row["STATUS"].ToString() != "")
                {
                    model.STATUS = int.Parse(row["STATUS"].ToString());
                }
                if (row["DATA_NAME"] != null)
                {
                    model.DATA_NAME = row["DATA_NAME"].ToString();
                }
                if (row["DATA_VALUE"] != null)
                {
                    model.DATA_VALUE = row["DATA_VALUE"].ToString();
                }
                if (row["TRX_TIME"] != null && row["TRX_TIME"].ToString() != "")
                {
                    model.TRX_TIME = DateTime.Parse(row["TRX_TIME"].ToString());
                }
                if (row["LAST_MODIFY_TIME"] != null && row["LAST_MODIFY_TIME"].ToString() != "")
                {
                    model.LAST_MODIFY_TIME = DateTime.Parse(row["LAST_MODIFY_TIME"].ToString());
                }
                if (row["RECID"] != null)
                {
                    model.RECID = row["RECID"].ToString();
                }
                if (row["UPLOAD_FLAG"] != null && row["UPLOAD_FLAG"].ToString() != "")
                {
                    if ((row["UPLOAD_FLAG"].ToString() == "1") || (row["UPLOAD_FLAG"].ToString().ToLower() == "true"))
                    {
                        model.UPLOAD_FLAG = true;
                    }
                    else
                    {
                        model.UPLOAD_FLAG = false;
                    }
                }
                if (row["AutoStationName"] != null)
                {
                    model.AutoStationName = row["AutoStationName"].ToString();
                }
            }
            return model;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select SERIAL_NUMBER,STEP_NUMBER,STATUS,DATA_NAME,DATA_VALUE,TRX_TIME,LAST_MODIFY_TIME,RECID,UPLOAD_FLAG,AutoStationName");
            strSql.Append(" FROM LOCAL_MES_STEP_INFO_DETAIL ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return DbHelperSQL.Query(strSql.ToString());
        }

        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataSet GetList(int Top, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select SERIAL_NUMBER,STEP_NUMBER,STATUS,DATA_NAME,DATA_VALUE,TRX_TIME,LAST_MODIFY_TIME,RECID,UPLOAD_FLAG,AutoStationName ");
            strSql.Append(" FROM LOCAL_MES_STEP_INFO_DETAIL ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return DbHelperSQL.Query(strSql.ToString());
        }

        /// <summary>
        /// 获取记录总数
        /// </summary>
        public int GetRecordCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) FROM LOCAL_MES_STEP_INFO_DETAIL ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            object obj = DbHelperSQL.GetSingle(strSql.ToString());
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }
        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public DataSet GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM ( ");
            strSql.Append(" SELECT ROW_NUMBER() OVER (");
            if (!string.IsNullOrEmpty(orderby.Trim()))
            {
                strSql.Append("order by T." + orderby);
            }
            else
            {
                strSql.Append("order by T.RECID desc");
            }
            strSql.Append(")AS Row, T.*  from LOCAL_MES_STEP_INFO_DETAIL T ");
            if (!string.IsNullOrEmpty(strWhere.Trim()))
            {
                strSql.Append(" WHERE " + strWhere);
            }
            strSql.Append(" ) TT");
            strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);
            return DbHelperSQL.Query(strSql.ToString());
        }

        /*
        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public DataSet GetList(int PageSize,int PageIndex,string strWhere)
        {
            SqlParameter[] parameters = {
                    new SqlParameter("@tblName", SqlDbType.VarChar, 255),
                    new SqlParameter("@fldName", SqlDbType.VarChar, 255),
                    new SqlParameter("@PageSize", SqlDbType.Int),
                    new SqlParameter("@PageIndex", SqlDbType.Int),
                    new SqlParameter("@IsReCount", SqlDbType.Bit),
                    new SqlParameter("@OrderType", SqlDbType.Bit),
                    new SqlParameter("@strWhere", SqlDbType.VarChar,1000),
                    };
            parameters[0].Value = "LOCAL_MES_STEP_INFO_DETAIL";
            parameters[1].Value = "RECID";
            parameters[2].Value = PageSize;
            parameters[3].Value = PageIndex;
            parameters[4].Value = 0;
            parameters[5].Value = 0;
            parameters[6].Value = strWhere;	
            return DbHelperSQL.RunProcedure("UP_GetRecordByPage",parameters,"ds");
        }*/

        #endregion  BasicMethod
        #region  ExtensionMethod

        #endregion  ExtensionMethod
    }
}

