using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using FTDataAccess.DBUtility;//Please add references
namespace FTDataAccess.DAL
{
    /// <summary>
    /// 数据访问类:workerResModel
    /// </summary>
    public partial class workerResDal
    {
        public workerResDal()
        { }
        #region  BasicMethod
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string workerID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from workerRes");
            strSql.Append(" where workerID=@workerID ");
            SqlParameter[] parameters = {
					new SqlParameter("@workerID", SqlDbType.NVarChar,50)			};
            parameters[0].Value = workerID;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(FTDataAccess.Model.workerResModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into workerRes(");
            strSql.Append("workerID,name,sex,age,shiftNo,photo,tag1,tag2,tag3,tag4,tag5)");
            strSql.Append(" values (");
            strSql.Append("@workerID,@name,@sex,@age,@shiftNo,@photo,@tag1,@tag2,@tag3,@tag4,@tag5)");
            SqlParameter[] parameters = {
					new SqlParameter("@workerID", SqlDbType.NVarChar,50),
					new SqlParameter("@name", SqlDbType.NVarChar,50),
					new SqlParameter("@sex", SqlDbType.NChar,10),
					new SqlParameter("@age", SqlDbType.Int,4),
					new SqlParameter("@shiftNo", SqlDbType.NVarChar,50),
					new SqlParameter("@photo", SqlDbType.Image),
					new SqlParameter("@tag1", SqlDbType.NVarChar,50),
					new SqlParameter("@tag2", SqlDbType.NVarChar,50),
					new SqlParameter("@tag3", SqlDbType.NVarChar,50),
					new SqlParameter("@tag4", SqlDbType.NVarChar,50),
					new SqlParameter("@tag5", SqlDbType.NVarChar,50)};
            parameters[0].Value = model.workerID;
            parameters[1].Value = model.name;
            parameters[2].Value = model.sex;
            parameters[3].Value = model.age;
            parameters[4].Value = model.shiftNo;
            parameters[5].Value = model.photo;
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
        public bool Update(FTDataAccess.Model.workerResModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update workerRes set ");
            strSql.Append("name=@name,");
            strSql.Append("sex=@sex,");
            strSql.Append("age=@age,");
            strSql.Append("shiftNo=@shiftNo,");
            strSql.Append("photo=@photo,");
            strSql.Append("tag1=@tag1,");
            strSql.Append("tag2=@tag2,");
            strSql.Append("tag3=@tag3,");
            strSql.Append("tag4=@tag4,");
            strSql.Append("tag5=@tag5");
            strSql.Append(" where workerID=@workerID ");
            SqlParameter[] parameters = {
					new SqlParameter("@name", SqlDbType.NVarChar,50),
					new SqlParameter("@sex", SqlDbType.NChar,10),
					new SqlParameter("@age", SqlDbType.Int,4),
					new SqlParameter("@shiftNo", SqlDbType.NVarChar,50),
					new SqlParameter("@photo", SqlDbType.Image),
					new SqlParameter("@tag1", SqlDbType.NVarChar,50),
					new SqlParameter("@tag2", SqlDbType.NVarChar,50),
					new SqlParameter("@tag3", SqlDbType.NVarChar,50),
					new SqlParameter("@tag4", SqlDbType.NVarChar,50),
					new SqlParameter("@tag5", SqlDbType.NVarChar,50),
					new SqlParameter("@workerID", SqlDbType.NVarChar,50)};
            parameters[0].Value = model.name;
            parameters[1].Value = model.sex;
            parameters[2].Value = model.age;
            parameters[3].Value = model.shiftNo;
            parameters[4].Value = model.photo;
            parameters[5].Value = model.tag1;
            parameters[6].Value = model.tag2;
            parameters[7].Value = model.tag3;
            parameters[8].Value = model.tag4;
            parameters[9].Value = model.tag5;
            parameters[10].Value = model.workerID;

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
        public bool Delete(string workerID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from workerRes ");
            strSql.Append(" where workerID=@workerID ");
            SqlParameter[] parameters = {
					new SqlParameter("@workerID", SqlDbType.NVarChar,50)			};
            parameters[0].Value = workerID;

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
        public bool DeleteList(string workerIDlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from workerRes ");
            strSql.Append(" where workerID in (" + workerIDlist + ")  ");
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
        public FTDataAccess.Model.workerResModel GetModel(string workerID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 workerID,name,sex,age,shiftNo,photo,tag1,tag2,tag3,tag4,tag5 from workerRes ");
            strSql.Append(" where workerID=@workerID ");
            SqlParameter[] parameters = {
					new SqlParameter("@workerID", SqlDbType.NVarChar,50)			};
            parameters[0].Value = workerID;

            FTDataAccess.Model.workerResModel model = new FTDataAccess.Model.workerResModel();
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
        public FTDataAccess.Model.workerResModel DataRowToModel(DataRow row)
        {
            FTDataAccess.Model.workerResModel model = new FTDataAccess.Model.workerResModel();
            if (row != null)
            {
                if (row["workerID"] != null)
                {
                    model.workerID = row["workerID"].ToString();
                }
                if (row["name"] != null)
                {
                    model.name = row["name"].ToString();
                }
                if (row["sex"] != null)
                {
                    model.sex = row["sex"].ToString();
                }
                if (row["age"] != null && row["age"].ToString() != "")
                {
                    model.age = int.Parse(row["age"].ToString());
                }
                if (row["shiftNo"] != null)
                {
                    model.shiftNo = row["shiftNo"].ToString();
                }
                if (row["photo"] != null && row["photo"].ToString() != "")
                {
                    model.photo = (byte[])row["photo"];
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
            strSql.Append("select workerID,name,sex,age,shiftNo,photo,tag1,tag2,tag3,tag4,tag5 ");
            strSql.Append(" FROM workerRes ");
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
            strSql.Append(" workerID,name,sex,age,shiftNo,photo,tag1,tag2,tag3,tag4,tag5 ");
            strSql.Append(" FROM workerRes ");
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
            strSql.Append("select count(1) FROM workerRes ");
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
                strSql.Append("order by T.workerID desc");
            }
            strSql.Append(")AS Row, T.*  from workerRes T ");
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
            parameters[0].Value = "workerRes";
            parameters[1].Value = "workerID";
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

