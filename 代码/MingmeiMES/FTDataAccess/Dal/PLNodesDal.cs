using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using FTDataAccess.DBUtility;//Please add references
namespace FTDataAccess.DAL
{
    /// <summary>
    /// 数据访问类:PLNodesModel
    /// </summary>
    public partial class PLNodesDal
    {
        public PLNodesDal()
        { }
        #region  BasicMethod
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string nodeID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from PLNodes");
            strSql.Append(" where nodeID=@nodeID ");
            SqlParameter[] parameters = {
					new SqlParameter("@nodeID", SqlDbType.NVarChar,50)			};
            parameters[0].Value = nodeID;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(FTDataAccess.Model.PLNodesModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into PLNodes(");
            strSql.Append("nodeID,nodeName,enableRun,checkRequired,workerID,lastLoginTime,tag1,tag2,tag3,tag4,tag5)");
            strSql.Append(" values (");
            strSql.Append("@nodeID,@nodeName,@enableRun,@checkRequired,@workerID,@lastLoginTime,@tag1,@tag2,@tag3,@tag4,@tag5)");
            SqlParameter[] parameters = {
					new SqlParameter("@nodeID", SqlDbType.NVarChar,50),
					new SqlParameter("@nodeName", SqlDbType.NVarChar,50),
					new SqlParameter("@enableRun", SqlDbType.Bit,1),
					new SqlParameter("@checkRequired", SqlDbType.Bit,1),
					new SqlParameter("@workerID", SqlDbType.NVarChar,255),
					new SqlParameter("@lastLoginTime", SqlDbType.DateTime),
					new SqlParameter("@tag1", SqlDbType.NVarChar,1024),
					new SqlParameter("@tag2", SqlDbType.NVarChar,1024),
					new SqlParameter("@tag3", SqlDbType.NVarChar,1024),
					new SqlParameter("@tag4", SqlDbType.NVarChar,1024),
					new SqlParameter("@tag5", SqlDbType.NVarChar,1024)};
            parameters[0].Value = model.nodeID;
            parameters[1].Value = model.nodeName;
            parameters[2].Value = model.enableRun;
            parameters[3].Value = model.checkRequired;
            parameters[4].Value = model.workerID;
            parameters[5].Value = model.lastLoginTime;
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
        public bool Update(FTDataAccess.Model.PLNodesModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update PLNodes set ");
            strSql.Append("nodeName=@nodeName,");
            strSql.Append("enableRun=@enableRun,");
            strSql.Append("checkRequired=@checkRequired,");
            strSql.Append("workerID=@workerID,");
            strSql.Append("lastLoginTime=@lastLoginTime,");
            strSql.Append("tag1=@tag1,");
            strSql.Append("tag2=@tag2,");
            strSql.Append("tag3=@tag3,");
            strSql.Append("tag4=@tag4,");
            strSql.Append("tag5=@tag5");
            strSql.Append(" where nodeID=@nodeID ");
            SqlParameter[] parameters = {
					new SqlParameter("@nodeName", SqlDbType.NVarChar,50),
					new SqlParameter("@enableRun", SqlDbType.Bit,1),
					new SqlParameter("@checkRequired", SqlDbType.Bit,1),
					new SqlParameter("@workerID", SqlDbType.NVarChar,255),
					new SqlParameter("@lastLoginTime", SqlDbType.DateTime),
					new SqlParameter("@tag1", SqlDbType.NVarChar,1024),
					new SqlParameter("@tag2", SqlDbType.NVarChar,1024),
					new SqlParameter("@tag3", SqlDbType.NVarChar,1024),
					new SqlParameter("@tag4", SqlDbType.NVarChar,1024),
					new SqlParameter("@tag5", SqlDbType.NVarChar,1024),
					new SqlParameter("@nodeID", SqlDbType.NVarChar,50)};
            parameters[0].Value = model.nodeName;
            parameters[1].Value = model.enableRun;
            parameters[2].Value = model.checkRequired;
            parameters[3].Value = model.workerID;
            parameters[4].Value = model.lastLoginTime;
            parameters[5].Value = model.tag1;
            parameters[6].Value = model.tag2;
            parameters[7].Value = model.tag3;
            parameters[8].Value = model.tag4;
            parameters[9].Value = model.tag5;
            parameters[10].Value = model.nodeID;

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
        public bool Delete(string nodeID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from PLNodes ");
            strSql.Append(" where nodeID=@nodeID ");
            SqlParameter[] parameters = {
					new SqlParameter("@nodeID", SqlDbType.NVarChar,50)			};
            parameters[0].Value = nodeID;

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
        public bool DeleteList(string nodeIDlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from PLNodes ");
            strSql.Append(" where nodeID in (" + nodeIDlist + ")  ");
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
        public FTDataAccess.Model.PLNodesModel GetModel(string nodeID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 nodeID,nodeName,enableRun,checkRequired,workerID,lastLoginTime,tag1,tag2,tag3,tag4,tag5 from PLNodes ");
            strSql.Append(" where nodeID=@nodeID ");
            SqlParameter[] parameters = {
					new SqlParameter("@nodeID", SqlDbType.NVarChar,50)			};
            parameters[0].Value = nodeID;

            FTDataAccess.Model.PLNodesModel model = new FTDataAccess.Model.PLNodesModel();
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
        public FTDataAccess.Model.PLNodesModel DataRowToModel(DataRow row)
        {
            FTDataAccess.Model.PLNodesModel model = new FTDataAccess.Model.PLNodesModel();
            if (row != null)
            {
                if (row["nodeID"] != null)
                {
                    model.nodeID = row["nodeID"].ToString();
                }
                if (row["nodeName"] != null)
                {
                    model.nodeName = row["nodeName"].ToString();
                }
                if (row["enableRun"] != null && row["enableRun"].ToString() != "")
                {
                    if ((row["enableRun"].ToString() == "1") || (row["enableRun"].ToString().ToLower() == "true"))
                    {
                        model.enableRun = true;
                    }
                    else
                    {
                        model.enableRun = false;
                    }
                }
                if (row["checkRequired"] != null && row["checkRequired"].ToString() != "")
                {
                    if ((row["checkRequired"].ToString() == "1") || (row["checkRequired"].ToString().ToLower() == "true"))
                    {
                        model.checkRequired = true;
                    }
                    else
                    {
                        model.checkRequired = false;
                    }
                }
                if (row["workerID"] != null)
                {
                    model.workerID = row["workerID"].ToString();
                }
                if (row["lastLoginTime"] != null && row["lastLoginTime"].ToString() != "")
                {
                    model.lastLoginTime = DateTime.Parse(row["lastLoginTime"].ToString());
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
            strSql.Append("select nodeID,nodeName,enableRun,checkRequired,workerID,lastLoginTime,tag1,tag2,tag3,tag4,tag5 ");
            strSql.Append(" FROM PLNodes ");
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
            strSql.Append(" nodeID,nodeName,enableRun,checkRequired,workerID,lastLoginTime,tag1,tag2,tag3,tag4,tag5 ");
            strSql.Append(" FROM PLNodes ");
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
            strSql.Append("select count(1) FROM PLNodes ");
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
                strSql.Append("order by T.nodeID desc");
            }
            strSql.Append(")AS Row, T.*  from PLNodes T ");
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
            parameters[0].Value = "PLNodes";
            parameters[1].Value = "nodeID";
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

