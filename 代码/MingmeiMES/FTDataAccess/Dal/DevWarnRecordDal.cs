using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using FTDataAccess.DBUtility;//Please add references
namespace DBAccess.DAL
{
    /// <summary>
    /// 数据访问类:DevWarnRecordModel
    /// </summary>
    public partial class DevWarnRecordDal
    {
        public DevWarnRecordDal()
        { }
        #region  BasicMethod
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string recordID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from DevWarnRecord");
            strSql.Append(" where recordID=@recordID ");
            SqlParameter[] parameters = {
					new SqlParameter("@recordID", SqlDbType.NVarChar,255)			};
            parameters[0].Value = recordID;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(DBAccess.Model.DevWarnRecordModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into DevWarnRecord(");
            strSql.Append("recordID,recordTime,warnStat,warnInfo,plcAddr,devID,tag1,tag2,tag3,tag4,tag5)");
            strSql.Append(" values (");
            strSql.Append("@recordID,@recordTime,@warnStat,@warnInfo,@plcAddr,@devID,@tag1,@tag2,@tag3,@tag4,@tag5)");
            SqlParameter[] parameters = {
					new SqlParameter("@recordID", SqlDbType.NVarChar,255),
					new SqlParameter("@recordTime", SqlDbType.DateTime),
					new SqlParameter("@warnStat", SqlDbType.Int,4),
					new SqlParameter("@warnInfo", SqlDbType.NVarChar,50),
					new SqlParameter("@plcAddr", SqlDbType.NChar,10),
					new SqlParameter("@devID", SqlDbType.NVarChar,50),
					new SqlParameter("@tag1", SqlDbType.NChar,10),
					new SqlParameter("@tag2", SqlDbType.NChar,10),
					new SqlParameter("@tag3", SqlDbType.NChar,10),
					new SqlParameter("@tag4", SqlDbType.NChar,10),
					new SqlParameter("@tag5", SqlDbType.NChar,10)};
            parameters[0].Value = model.recordID;
            parameters[1].Value = model.recordTime;
            parameters[2].Value = model.warnStat;
            parameters[3].Value = model.warnInfo;
            parameters[4].Value = model.plcAddr;
            parameters[5].Value = model.devID;
            parameters[6].Value = model.tag1;
            parameters[7].Value = model.tag2;
            parameters[8].Value = model.tag3;
            parameters[9].Value = model.tag4;
            parameters[10].Value = model.tag5;

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
        public bool Update(DBAccess.Model.DevWarnRecordModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update DevWarnRecord set ");
            strSql.Append("recordTime=@recordTime,");
            strSql.Append("warnStat=@warnStat,");
            strSql.Append("warnInfo=@warnInfo,");
            strSql.Append("plcAddr=@plcAddr,");
            strSql.Append("devID=@devID,");
            strSql.Append("tag1=@tag1,");
            strSql.Append("tag2=@tag2,");
            strSql.Append("tag3=@tag3,");
            strSql.Append("tag4=@tag4,");
            strSql.Append("tag5=@tag5");
            strSql.Append(" where recordID=@recordID ");
            SqlParameter[] parameters = {
					new SqlParameter("@recordTime", SqlDbType.DateTime),
					new SqlParameter("@warnStat", SqlDbType.Int,4),
					new SqlParameter("@warnInfo", SqlDbType.NVarChar,50),
					new SqlParameter("@plcAddr", SqlDbType.NChar,10),
					new SqlParameter("@devID", SqlDbType.NVarChar,50),
					new SqlParameter("@tag1", SqlDbType.NChar,10),
					new SqlParameter("@tag2", SqlDbType.NChar,10),
					new SqlParameter("@tag3", SqlDbType.NChar,10),
					new SqlParameter("@tag4", SqlDbType.NChar,10),
					new SqlParameter("@tag5", SqlDbType.NChar,10),
					new SqlParameter("@recordID", SqlDbType.NVarChar,255)};
            parameters[0].Value = model.recordTime;
            parameters[1].Value = model.warnStat;
            parameters[2].Value = model.warnInfo;
            parameters[3].Value = model.plcAddr;
            parameters[4].Value = model.devID;
            parameters[5].Value = model.tag1;
            parameters[6].Value = model.tag2;
            parameters[7].Value = model.tag3;
            parameters[8].Value = model.tag4;
            parameters[9].Value = model.tag5;
            parameters[10].Value = model.recordID;

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
        public bool Delete(string recordID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from DevWarnRecord ");
            strSql.Append(" where recordID=@recordID ");
            SqlParameter[] parameters = {
					new SqlParameter("@recordID", SqlDbType.NVarChar,255)			};
            parameters[0].Value = recordID;

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
        public bool DeleteList(string recordIDlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from DevWarnRecord ");
            strSql.Append(" where recordID in (" + recordIDlist + ")  ");
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
        public DBAccess.Model.DevWarnRecordModel GetModel(string recordID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 recordID,recordTime,warnStat,warnInfo,plcAddr,devID,tag1,tag2,tag3,tag4,tag5 from DevWarnRecord ");
            strSql.Append(" where recordID=@recordID ");
            SqlParameter[] parameters = {
					new SqlParameter("@recordID", SqlDbType.NVarChar,255)			};
            parameters[0].Value = recordID;

            DBAccess.Model.DevWarnRecordModel model = new DBAccess.Model.DevWarnRecordModel();
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
        public DBAccess.Model.DevWarnRecordModel DataRowToModel(DataRow row)
        {
            DBAccess.Model.DevWarnRecordModel model = new DBAccess.Model.DevWarnRecordModel();
            if (row != null)
            {
                if (row["recordID"] != null)
                {
                    model.recordID = row["recordID"].ToString();
                }
                if (row["recordTime"] != null && row["recordTime"].ToString() != "")
                {
                    model.recordTime = DateTime.Parse(row["recordTime"].ToString());
                }
                if (row["warnStat"] != null && row["warnStat"].ToString() != "")
                {
                    model.warnStat = int.Parse(row["warnStat"].ToString());
                }
                if (row["warnInfo"] != null)
                {
                    model.warnInfo = row["warnInfo"].ToString();
                }
                if (row["plcAddr"] != null)
                {
                    model.plcAddr = row["plcAddr"].ToString();
                }
                if (row["devID"] != null)
                {
                    model.devID = row["devID"].ToString();
                }
                if (row["tag1"] != null)
                {
                    model.tag1 = row["tag1"].ToString();
                }
                if (row["tag2"] != null)
                {
                    model.tag2 = row["tag2"].ToString();
                }
                if (row["tag3"] != null)
                {
                    model.tag3 = row["tag3"].ToString();
                }
                if (row["tag4"] != null)
                {
                    model.tag4 = row["tag4"].ToString();
                }
                if (row["tag5"] != null)
                {
                    model.tag5 = row["tag5"].ToString();
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
            strSql.Append("select recordID,recordTime,warnStat,warnInfo,plcAddr,devID,tag1,tag2,tag3,tag4,tag5 ");
            strSql.Append(" FROM DevWarnRecord ");
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
            strSql.Append("select ");
            if (Top > 0)
            {
                strSql.Append(" top " + Top.ToString());
            }
            strSql.Append(" recordID,recordTime,warnStat,warnInfo,plcAddr,devID,tag1,tag2,tag3,tag4,tag5 ");
            strSql.Append(" FROM DevWarnRecord ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" order by " + filedOrder);
            return DbHelperSQL.Query(strSql.ToString());
        }

        /// <summary>
        /// 获取记录总数
        /// </summary>
        public int GetRecordCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) FROM DevWarnRecord ");
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
                strSql.Append("order by T.recordID desc");
            }
            strSql.Append(")AS Row, T.*  from DevWarnRecord T ");
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
            parameters[0].Value = "DevWarnRecord";
            parameters[1].Value = "recordID";
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


