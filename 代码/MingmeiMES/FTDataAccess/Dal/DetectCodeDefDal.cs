using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using FTDataAccess.DBUtility;//Please add references
namespace FTDataAccess.DAL
{
    /// <summary>
    /// 数据访问类:DetectCodeDefModel
    /// </summary>
    public partial class DetectCodeDefDal
    {
        public DetectCodeDefDal()
        { }
        #region  Method

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string nodeName, int detectIndex)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from DetectCodeDef");
            strSql.Append(" where nodeName='" + nodeName + "' and detectIndex=" + detectIndex + " ");
            return DbHelperSQL.Exists(strSql.ToString());
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(FTDataAccess.Model.DetectCodeDefModel model)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder strSql1 = new StringBuilder();
            StringBuilder strSql2 = new StringBuilder();
            if (model.nodeName != null)
            {
                strSql1.Append("nodeName,");
                strSql2.Append("'" + model.nodeName + "',");
            }
            if (model.detectIndex != null)
            {
                strSql1.Append("detectIndex,");
                strSql2.Append("" + model.detectIndex + ",");
            }
            if (model.detectCode != null)
            {
                strSql1.Append("detectCode,");
                strSql2.Append("'" + model.detectCode + "',");
            }
            if (model.detectItemName != null)
            {
                strSql1.Append("detectItemName,");
                strSql2.Append("'" + model.detectItemName + "',");
            }
            strSql.Append("insert into DetectCodeDef(");
            strSql.Append(strSql1.ToString().Remove(strSql1.Length - 1));
            strSql.Append(")");
            strSql.Append(" values (");
            strSql.Append(strSql2.ToString().Remove(strSql2.Length - 1));
            strSql.Append(")");
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
        /// 更新一条数据
        /// </summary>
        public bool Update(FTDataAccess.Model.DetectCodeDefModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update DetectCodeDef set ");
            if (model.detectCode != null)
            {
                strSql.Append("detectCode='" + model.detectCode + "',");
            }
            if (model.detectItemName != null)
            {
                strSql.Append("detectItemName='" + model.detectItemName + "',");
            }
            int n = strSql.ToString().LastIndexOf(",");
            strSql.Remove(n, 1);
            strSql.Append(" where nodeName='" + model.nodeName + "' and detectIndex=" + model.detectIndex + " ");
            int rowsAffected = DbHelperSQL.ExecuteSql(strSql.ToString());
            if (rowsAffected > 0)
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
        public bool Delete(string nodeName, int detectIndex)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from DetectCodeDef ");
            strSql.Append(" where nodeName='" + nodeName + "' and detectIndex=" + detectIndex + " ");
            int rowsAffected = DbHelperSQL.ExecuteSql(strSql.ToString());
            if (rowsAffected > 0)
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
        public FTDataAccess.Model.DetectCodeDefModel GetModel(string nodeName, int detectIndex)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1  ");
            strSql.Append(" nodeName,detectIndex,detectCode,detectItemName ");
            strSql.Append(" from DetectCodeDef ");
            strSql.Append(" where nodeName='" + nodeName + "' and detectIndex=" + detectIndex + " ");
            FTDataAccess.Model.DetectCodeDefModel model = new FTDataAccess.Model.DetectCodeDefModel();
            DataSet ds = DbHelperSQL.Query(strSql.ToString());
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
        public FTDataAccess.Model.DetectCodeDefModel DataRowToModel(DataRow row)
        {
            FTDataAccess.Model.DetectCodeDefModel model = new FTDataAccess.Model.DetectCodeDefModel();
            if (row != null)
            {
                if (row["nodeName"] != null)
                {
                    model.nodeName = row["nodeName"].ToString();
                }
                if (row["detectIndex"] != null && row["detectIndex"].ToString() != "")
                {
                    model.detectIndex = int.Parse(row["detectIndex"].ToString());
                }
                if (row["detectCode"] != null)
                {
                    model.detectCode = row["detectCode"].ToString();
                }
                if (row["detectItemName"] != null)
                {
                    model.detectItemName = row["detectItemName"].ToString();
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
            strSql.Append("select nodeName,detectIndex,detectCode,detectItemName ");
            strSql.Append(" FROM DetectCodeDef ");
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
            strSql.Append(" nodeName,detectIndex,detectCode,detectItemName ");
            strSql.Append(" FROM DetectCodeDef ");
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
            strSql.Append("select count(1) FROM DetectCodeDef ");
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
                strSql.Append("order by T.detectIndex desc");
            }
            strSql.Append(")AS Row, T.*  from DetectCodeDef T ");
            if (!string.IsNullOrEmpty(strWhere.Trim()))
            {
                strSql.Append(" WHERE " + strWhere);
            }
            strSql.Append(" ) TT");
            strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);
            return DbHelperSQL.Query(strSql.ToString());
        }

        /*
        */

        #endregion  Method
        #region  MethodEx

        #endregion  MethodEx
    }
}

