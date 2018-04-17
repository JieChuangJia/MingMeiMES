using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using FTDataAccess.DBUtility;//Please add references
namespace FTDataAccess.DAL
{
    /// <summary>
    /// 数据访问类:ProductHeightDefModel
    /// </summary>
    public partial class ProductHeightDefDal
    {
        public ProductHeightDefDal()
        { }
        #region  BasicMethod

        /// <summary>
        /// 得到最大ID
        /// </summary>
        public int GetMaxId()
        {
            return (int)DbHelperSQL.GetMaxID("productHeight", "ProductHeightDef");
        }

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int productHeight)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from ProductHeightDef");
            strSql.Append(" where productHeight=@productHeight ");
            SqlParameter[] parameters = {
					new SqlParameter("@productHeight", SqlDbType.Int,4)			};
            parameters[0].Value = productHeight;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(FTDataAccess.Model.ProductHeightDefModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into ProductHeightDef(");
            strSql.Append("productHeight,heightSeq,mark)");
            strSql.Append(" values (");
            strSql.Append("@productHeight,@heightSeq,@mark)");
            SqlParameter[] parameters = {
					new SqlParameter("@productHeight", SqlDbType.Int,4),
					new SqlParameter("@heightSeq", SqlDbType.Int,4),
					new SqlParameter("@mark", SqlDbType.NVarChar,200)};
            parameters[0].Value = model.productHeight;
            parameters[1].Value = model.heightSeq;
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
        public bool Update(FTDataAccess.Model.ProductHeightDefModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update ProductHeightDef set ");
            strSql.Append("heightSeq=@heightSeq,");
            strSql.Append("mark=@mark");
            strSql.Append(" where productHeight=@productHeight ");
            SqlParameter[] parameters = {
					new SqlParameter("@heightSeq", SqlDbType.Int,4),
					new SqlParameter("@mark", SqlDbType.NVarChar,200),
					new SqlParameter("@productHeight", SqlDbType.Int,4)};
            parameters[0].Value = model.heightSeq;
            parameters[1].Value = model.mark;
            parameters[2].Value = model.productHeight;

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
        public bool Delete(int productHeight)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from ProductHeightDef ");
            strSql.Append(" where productHeight=@productHeight ");
            SqlParameter[] parameters = {
					new SqlParameter("@productHeight", SqlDbType.Int,4)			};
            parameters[0].Value = productHeight;

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
        public bool DeleteList(string productHeightlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from ProductHeightDef ");
            strSql.Append(" where productHeight in (" + productHeightlist + ")  ");
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
        public FTDataAccess.Model.ProductHeightDefModel GetModel(int productHeight)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 productHeight,heightSeq,mark from ProductHeightDef ");
            strSql.Append(" where productHeight=@productHeight ");
            SqlParameter[] parameters = {
					new SqlParameter("@productHeight", SqlDbType.Int,4)			};
            parameters[0].Value = productHeight;

            FTDataAccess.Model.ProductHeightDefModel model = new FTDataAccess.Model.ProductHeightDefModel();
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
        public FTDataAccess.Model.ProductHeightDefModel DataRowToModel(DataRow row)
        {
            FTDataAccess.Model.ProductHeightDefModel model = new FTDataAccess.Model.ProductHeightDefModel();
            if (row != null)
            {
                if (row["productHeight"] != null && row["productHeight"].ToString() != "")
                {
                    model.productHeight = int.Parse(row["productHeight"].ToString());
                }
                if (row["heightSeq"] != null && row["heightSeq"].ToString() != "")
                {
                    model.heightSeq = int.Parse(row["heightSeq"].ToString());
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
           // strSql.Append("select productHeight as 产品高度,heightSeq as 产品高度编号,mark as 备注");
            strSql.Append("select productHeight,heightSeq,mark");
            strSql.Append(" FROM ProductHeightDef ");
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
            strSql.Append(" productHeight as 产品高度,heightSeq as 产品高度编号,mark as 备注 ");
            strSql.Append(" FROM ProductHeightDef ");
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
            strSql.Append("select count(1) FROM ProductHeightDef ");
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
                strSql.Append("order by T.productHeight desc");
            }
            strSql.Append(")AS Row, T.*  from ProductHeightDef T ");
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
            parameters[0].Value = "ProductHeightDef";
            parameters[1].Value = "productHeight";
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

