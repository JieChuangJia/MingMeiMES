using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using FTDataAccess.DBUtility;//Please add references
namespace FTDataAccess.DAL
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ProductPacsizeDefDal
    {
        public ProductPacsizeDefDal()
        { }
        #region  BasicMethod
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string packageSize)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from ProductPacsizeDef");
            strSql.Append(" where packageSize=@packageSize ");
            SqlParameter[] parameters = {
					new SqlParameter("@packageSize", SqlDbType.NVarChar,50)			};
            parameters[0].Value = packageSize;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(FTDataAccess.Model.ProductPacsizeDefModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into ProductPacsizeDef(");
            strSql.Append("packageSize,packageSeq,mark)");
            strSql.Append(" values (");
            strSql.Append("@packageSize,@packageSeq,@mark)");
            SqlParameter[] parameters = {
					new SqlParameter("@packageSize", SqlDbType.NVarChar,50),
					new SqlParameter("@packageSeq", SqlDbType.Int,4),
					new SqlParameter("@mark", SqlDbType.NVarChar,200)};
            parameters[0].Value = model.packageSize;
            parameters[1].Value = model.packageSeq;
            parameters[2].Value = model.mark;

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
        public bool Update(FTDataAccess.Model.ProductPacsizeDefModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update ProductPacsizeDef set ");
            strSql.Append("packageSeq=@packageSeq,");
            strSql.Append("mark=@mark");
            strSql.Append(" where packageSize=@packageSize ");
            SqlParameter[] parameters = {
					new SqlParameter("@packageSeq", SqlDbType.Int,4),
					new SqlParameter("@mark", SqlDbType.NVarChar,200),
					new SqlParameter("@packageSize", SqlDbType.NVarChar,50)};
            parameters[0].Value = model.packageSeq;
            parameters[1].Value = model.mark;
            parameters[2].Value = model.packageSize;

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
        public bool Delete(string packageSize)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from ProductPacsizeDef ");
            strSql.Append(" where packageSize=@packageSize ");
            SqlParameter[] parameters = {
					new SqlParameter("@packageSize", SqlDbType.NVarChar,50)			};
            parameters[0].Value = packageSize;

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
        public bool DeleteList(string packageSizelist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from ProductPacsizeDef ");
            strSql.Append(" where packageSize in (" + packageSizelist + ")  ");
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
        public FTDataAccess.Model.ProductPacsizeDefModel GetModel(string packageSize)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 packageSize,packageSeq,mark from ProductPacsizeDef ");
            strSql.Append(" where packageSize=@packageSize ");
            SqlParameter[] parameters = {
					new SqlParameter("@packageSize", SqlDbType.NVarChar,50)			};
            parameters[0].Value = packageSize;

            FTDataAccess.Model.ProductPacsizeDefModel model = new FTDataAccess.Model.ProductPacsizeDefModel();
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
        public FTDataAccess.Model.ProductPacsizeDefModel DataRowToModel(DataRow row)
        {
            FTDataAccess.Model.ProductPacsizeDefModel model = new FTDataAccess.Model.ProductPacsizeDefModel();
            if (row != null)
            {
                if (row["packageSize"] != null)
                {
                    model.packageSize = row["packageSize"].ToString();
                }
                if (row["packageSeq"] != null && row["packageSeq"].ToString() != "")
                {
                    model.packageSeq = int.Parse(row["packageSeq"].ToString());
                }
                if (row["mark"] != null)
                {
                    model.mark = row["mark"].ToString();
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
            //strSql.Append("select packageSize as 包装尺寸,packageSeq as 尺寸编号,mark as 备注");
            strSql.Append("select packageSize,packageSeq,mark ");
            strSql.Append(" FROM ProductPacsizeDef ");
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
            strSql.Append(" packageSize,packageSeq,mark ");
            strSql.Append(" FROM ProductPacsizeDef ");
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
            strSql.Append("select count(1) FROM ProductPacsizeDef ");
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
                strSql.Append("order by T.packageSize desc");
            }
            strSql.Append(")AS Row, T.*  from ProductPacsizeDef T ");
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
            parameters[0].Value = "ProductPacsizeDef";
            parameters[1].Value = "packageSize";
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

