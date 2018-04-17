using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using FTDataAccess.DBUtility;//Please add references
namespace DBAccess.DAL
{
    /// <summary>
    /// 数据访问类:PLDeviceModel
    /// </summary>
    public partial class PLDeviceDal
    {
        public PLDeviceDal()
        { }
        #region  BasicMethod
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string devID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from PLDevice");
            strSql.Append(" where devID=@devID ");
            SqlParameter[] parameters = {
					new SqlParameter("@devID", SqlDbType.NVarChar,50)			};
            parameters[0].Value = devID;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(DBAccess.Model.PLDeviceModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into PLDevice(");
            strSql.Append("devID,devName,nodeID,tag1,tag2,tag3,tag4,tag5)");
            strSql.Append(" values (");
            strSql.Append("@devID,@devName,@nodeID,@tag1,@tag2,@tag3,@tag4,@tag5)");
            SqlParameter[] parameters = {
					new SqlParameter("@devID", SqlDbType.NVarChar,50),
					new SqlParameter("@devName", SqlDbType.NVarChar,50),
					new SqlParameter("@nodeID", SqlDbType.NVarChar,50),
					new SqlParameter("@tag1", SqlDbType.NVarChar,50),
					new SqlParameter("@tag2", SqlDbType.NVarChar,50),
					new SqlParameter("@tag3", SqlDbType.NVarChar,50),
					new SqlParameter("@tag4", SqlDbType.NVarChar,50),
					new SqlParameter("@tag5", SqlDbType.NVarChar,50)};
            parameters[0].Value = model.devID;
            parameters[1].Value = model.devName;
            parameters[2].Value = model.nodeID;
            parameters[3].Value = model.tag1;
            parameters[4].Value = model.tag2;
            parameters[5].Value = model.tag3;
            parameters[6].Value = model.tag4;
            parameters[7].Value = model.tag5;

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
        public bool Update(DBAccess.Model.PLDeviceModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update PLDevice set ");
            strSql.Append("devName=@devName,");
            strSql.Append("nodeID=@nodeID,");
            strSql.Append("tag1=@tag1,");
            strSql.Append("tag2=@tag2,");
            strSql.Append("tag3=@tag3,");
            strSql.Append("tag4=@tag4,");
            strSql.Append("tag5=@tag5");
            strSql.Append(" where devID=@devID ");
            SqlParameter[] parameters = {
					new SqlParameter("@devName", SqlDbType.NVarChar,50),
					new SqlParameter("@nodeID", SqlDbType.NVarChar,50),
					new SqlParameter("@tag1", SqlDbType.NVarChar,50),
					new SqlParameter("@tag2", SqlDbType.NVarChar,50),
					new SqlParameter("@tag3", SqlDbType.NVarChar,50),
					new SqlParameter("@tag4", SqlDbType.NVarChar,50),
					new SqlParameter("@tag5", SqlDbType.NVarChar,50),
					new SqlParameter("@devID", SqlDbType.NVarChar,50)};
            parameters[0].Value = model.devName;
            parameters[1].Value = model.nodeID;
            parameters[2].Value = model.tag1;
            parameters[3].Value = model.tag2;
            parameters[4].Value = model.tag3;
            parameters[5].Value = model.tag4;
            parameters[6].Value = model.tag5;
            parameters[7].Value = model.devID;

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
        public bool Delete(string devID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from PLDevice ");
            strSql.Append(" where devID=@devID ");
            SqlParameter[] parameters = {
					new SqlParameter("@devID", SqlDbType.NVarChar,50)			};
            parameters[0].Value = devID;

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
        public bool DeleteList(string devIDlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from PLDevice ");
            strSql.Append(" where devID in (" + devIDlist + ")  ");
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
        public DBAccess.Model.PLDeviceModel GetModel(string devID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 devID,devName,nodeID,tag1,tag2,tag3,tag4,tag5 from PLDevice ");
            strSql.Append(" where devID=@devID ");
            SqlParameter[] parameters = {
					new SqlParameter("@devID", SqlDbType.NVarChar,50)			};
            parameters[0].Value = devID;

            DBAccess.Model.PLDeviceModel model = new DBAccess.Model.PLDeviceModel();
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
        public DBAccess.Model.PLDeviceModel DataRowToModel(DataRow row)
        {
            DBAccess.Model.PLDeviceModel model = new DBAccess.Model.PLDeviceModel();
            if (row != null)
            {
                if (row["devID"] != null)
                {
                    model.devID = row["devID"].ToString();
                }
                if (row["devName"] != null)
                {
                    model.devName = row["devName"].ToString();
                }
                if (row["nodeID"] != null)
                {
                    model.nodeID = row["nodeID"].ToString();
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
            strSql.Append("select devID,devName,nodeID,tag1,tag2,tag3,tag4,tag5 ");
            strSql.Append(" FROM PLDevice ");
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
            strSql.Append(" devID,devName,nodeID,tag1,tag2,tag3,tag4,tag5 ");
            strSql.Append(" FROM PLDevice ");
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
            strSql.Append("select count(1) FROM PLDevice ");
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
                strSql.Append("order by T.devID desc");
            }
            strSql.Append(")AS Row, T.*  from PLDevice T ");
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
            parameters[0].Value = "PLDevice";
            parameters[1].Value = "devID";
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


