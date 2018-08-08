using System;
using System.Data;
using System.Text;
using System.Data.OleDb;
 
namespace ALineScrewDB
{
    /// <summary>
    /// 数据访问类:db
    /// </summary>
    public partial class dbDAL
    {
        public dbDAL()
        { }
        #region  Method


        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string 二维码)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from db");
            strSql.Append(" where 二维码='" + 二维码 + "' ");
            return DbHelperOleDb.Exists(strSql.ToString());
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(ALineScrewDB.dbModel model)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder strSql1 = new StringBuilder();
            StringBuilder strSql2 = new StringBuilder();
            if (model.测试时间 != null)
            {
                strSql1.Append("测试时间,");
                strSql2.Append("'" + model.测试时间 + "',");
            }
            if (model.二维码 != null)
            {
                strSql1.Append("二维码,");
                strSql2.Append("'" + model.二维码 + "',");
            }
            if (model.螺丝1马头结果 != null)
            {
                strSql1.Append("螺丝1马头结果,");
                strSql2.Append("'" + model.螺丝1马头结果 + "',");
            }
            if (model.螺丝1马头扭矩 != null)
            {
                strSql1.Append("螺丝1马头扭矩,");
                strSql2.Append("'" + model.螺丝1马头扭矩 + "',");
            }
            if (model.螺丝1马头角度 != null)
            {
                strSql1.Append("螺丝1马头角度,");
                strSql2.Append("'" + model.螺丝1马头角度 + "',");
            }
            if (model.螺丝1图片路径 != null)
            {
                strSql1.Append("螺丝1图片路径,");
                strSql2.Append("'" + model.螺丝1图片路径 + "',");
            }
            if (model.螺丝2马头结果 != null)
            {
                strSql1.Append("螺丝2马头结果,");
                strSql2.Append("'" + model.螺丝2马头结果 + "',");
            }
            if (model.螺丝2马头扭矩 != null)
            {
                strSql1.Append("螺丝2马头扭矩,");
                strSql2.Append("'" + model.螺丝2马头扭矩 + "',");
            }
            if (model.螺丝2马头角度 != null)
            {
                strSql1.Append("螺丝2马头角度,");
                strSql2.Append("'" + model.螺丝2马头角度 + "',");
            }
            if (model.螺丝2图片路径 != null)
            {
                strSql1.Append("螺丝2图片路径,");
                strSql2.Append("'" + model.螺丝2图片路径 + "',");
            }
            if (model.螺丝3马头结果 != null)
            {
                strSql1.Append("螺丝3马头结果,");
                strSql2.Append("'" + model.螺丝3马头结果 + "',");
            }
            if (model.螺丝3马头扭矩 != null)
            {
                strSql1.Append("螺丝3马头扭矩,");
                strSql2.Append("'" + model.螺丝3马头扭矩 + "',");
            }
            if (model.螺丝3马头角度 != null)
            {
                strSql1.Append("螺丝3马头角度,");
                strSql2.Append("'" + model.螺丝3马头角度 + "',");
            }
            if (model.螺丝3图片路径 != null)
            {
                strSql1.Append("螺丝3图片路径,");
                strSql2.Append("'" + model.螺丝3图片路径 + "',");
            }
            if (model.螺丝4马头结果 != null)
            {
                strSql1.Append("螺丝4马头结果,");
                strSql2.Append("'" + model.螺丝4马头结果 + "',");
            }
            if (model.螺丝4马头扭矩 != null)
            {
                strSql1.Append("螺丝4马头扭矩,");
                strSql2.Append("'" + model.螺丝4马头扭矩 + "',");
            }
            if (model.螺丝4马头角度 != null)
            {
                strSql1.Append("螺丝4马头角度,");
                strSql2.Append("'" + model.螺丝4马头角度 + "',");
            }
            if (model.螺丝4图片路径 != null)
            {
                strSql1.Append("螺丝4图片路径,");
                strSql2.Append("'" + model.螺丝4图片路径 + "',");
            }
            strSql.Append("insert into db(");
            strSql.Append(strSql1.ToString().Remove(strSql1.Length - 1));
            strSql.Append(")");
            strSql.Append(" values (");
            strSql.Append(strSql2.ToString().Remove(strSql2.Length - 1));
            strSql.Append(")");
            int rows = DbHelperOleDb.ExecuteSql(strSql.ToString());
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
        public bool Update(ALineScrewDB.dbModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update db set ");
            if (model.测试时间 != null)
            {
                strSql.Append("测试时间='" + model.测试时间 + "',");
            }
            else
            {
                strSql.Append("测试时间= null ,");
            }
            if (model.螺丝1马头结果 != null)
            {
                strSql.Append("螺丝1马头结果='" + model.螺丝1马头结果 + "',");
            }
            else
            {
                strSql.Append("螺丝1马头结果= null ,");
            }
            if (model.螺丝1马头扭矩 != null)
            {
                strSql.Append("螺丝1马头扭矩='" + model.螺丝1马头扭矩 + "',");
            }
            else
            {
                strSql.Append("螺丝1马头扭矩= null ,");
            }
            if (model.螺丝1马头角度 != null)
            {
                strSql.Append("螺丝1马头角度='" + model.螺丝1马头角度 + "',");
            }
            else
            {
                strSql.Append("螺丝1马头角度= null ,");
            }
            if (model.螺丝1图片路径 != null)
            {
                strSql.Append("螺丝1图片路径='" + model.螺丝1图片路径 + "',");
            }
            else
            {
                strSql.Append("螺丝1图片路径= null ,");
            }
            if (model.螺丝2马头结果 != null)
            {
                strSql.Append("螺丝2马头结果='" + model.螺丝2马头结果 + "',");
            }
            else
            {
                strSql.Append("螺丝2马头结果= null ,");
            }
            if (model.螺丝2马头扭矩 != null)
            {
                strSql.Append("螺丝2马头扭矩='" + model.螺丝2马头扭矩 + "',");
            }
            else
            {
                strSql.Append("螺丝2马头扭矩= null ,");
            }
            if (model.螺丝2马头角度 != null)
            {
                strSql.Append("螺丝2马头角度='" + model.螺丝2马头角度 + "',");
            }
            else
            {
                strSql.Append("螺丝2马头角度= null ,");
            }
            if (model.螺丝2图片路径 != null)
            {
                strSql.Append("螺丝2图片路径='" + model.螺丝2图片路径 + "',");
            }
            else
            {
                strSql.Append("螺丝2图片路径= null ,");
            }
            if (model.螺丝3马头结果 != null)
            {
                strSql.Append("螺丝3马头结果='" + model.螺丝3马头结果 + "',");
            }
            else
            {
                strSql.Append("螺丝3马头结果= null ,");
            }
            if (model.螺丝3马头扭矩 != null)
            {
                strSql.Append("螺丝3马头扭矩='" + model.螺丝3马头扭矩 + "',");
            }
            else
            {
                strSql.Append("螺丝3马头扭矩= null ,");
            }
            if (model.螺丝3马头角度 != null)
            {
                strSql.Append("螺丝3马头角度='" + model.螺丝3马头角度 + "',");
            }
            else
            {
                strSql.Append("螺丝3马头角度= null ,");
            }
            if (model.螺丝3图片路径 != null)
            {
                strSql.Append("螺丝3图片路径='" + model.螺丝3图片路径 + "',");
            }
            else
            {
                strSql.Append("螺丝3图片路径= null ,");
            }
            if (model.螺丝4马头结果 != null)
            {
                strSql.Append("螺丝4马头结果='" + model.螺丝4马头结果 + "',");
            }
            else
            {
                strSql.Append("螺丝4马头结果= null ,");
            }
            if (model.螺丝4马头扭矩 != null)
            {
                strSql.Append("螺丝4马头扭矩='" + model.螺丝4马头扭矩 + "',");
            }
            else
            {
                strSql.Append("螺丝4马头扭矩= null ,");
            }
            if (model.螺丝4马头角度 != null)
            {
                strSql.Append("螺丝4马头角度='" + model.螺丝4马头角度 + "',");
            }
            else
            {
                strSql.Append("螺丝4马头角度= null ,");
            }
            if (model.螺丝4图片路径 != null)
            {
                strSql.Append("螺丝4图片路径='" + model.螺丝4图片路径 + "',");
            }
            else
            {
                strSql.Append("螺丝4图片路径= null ,");
            }
            int n = strSql.ToString().LastIndexOf(",");
            strSql.Remove(n, 1);
            strSql.Append(" where 二维码='" + model.二维码 + "' ");
            int rowsAffected = DbHelperOleDb.ExecuteSql(strSql.ToString());
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
        public bool Delete(string 二维码)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from db ");
            strSql.Append(" where 二维码='" + 二维码 + "' ");
            int rowsAffected = DbHelperOleDb.ExecuteSql(strSql.ToString());
            if (rowsAffected > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }		/// <summary>
        /// 批量删除数据
        /// </summary>
        public bool DeleteList(string 二维码list)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from db ");
            strSql.Append(" where 二维码 in (" + 二维码list + ")  ");
            int rows = DbHelperOleDb.ExecuteSql(strSql.ToString());
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
        public ALineScrewDB.dbModel GetModel(string 二维码)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  ");
            strSql.Append(" 测试时间,二维码,螺丝1马头结果,螺丝1马头扭矩,螺丝1马头角度,螺丝1图片路径,螺丝2马头结果,螺丝2马头扭矩,螺丝2马头角度,螺丝2图片路径,螺丝3马头结果,螺丝3马头扭矩,螺丝3马头角度,螺丝3图片路径,螺丝4马头结果,螺丝4马头扭矩,螺丝4马头角度,螺丝4图片路径 ");
            strSql.Append(" from db ");
            strSql.Append(" where 二维码='" + 二维码 + "' ");
            ALineScrewDB.dbModel model = new ALineScrewDB.dbModel();
            DataSet ds = DbHelperOleDb.Query(strSql.ToString());
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
        public ALineScrewDB.dbModel DataRowToModel(DataRow row)
        {
            ALineScrewDB.dbModel model = new ALineScrewDB.dbModel();
            if (row != null)
            {
                if (row["测试时间"] != null)
                {
                    model.测试时间 = row["测试时间"].ToString();
                }
                if (row["二维码"] != null)
                {
                    model.二维码 = row["二维码"].ToString();
                }
                if (row["螺丝1马头结果"] != null)
                {
                    model.螺丝1马头结果 = row["螺丝1马头结果"].ToString();
                }
                if (row["螺丝1马头扭矩"] != null)
                {
                    model.螺丝1马头扭矩 = row["螺丝1马头扭矩"].ToString();
                }
                if (row["螺丝1马头角度"] != null)
                {
                    model.螺丝1马头角度 = row["螺丝1马头角度"].ToString();
                }
                if (row["螺丝1图片路径"] != null)
                {
                    model.螺丝1图片路径 = row["螺丝1图片路径"].ToString();
                }
                if (row["螺丝2马头结果"] != null)
                {
                    model.螺丝2马头结果 = row["螺丝2马头结果"].ToString();
                }
                if (row["螺丝2马头扭矩"] != null)
                {
                    model.螺丝2马头扭矩 = row["螺丝2马头扭矩"].ToString();
                }
                if (row["螺丝2马头角度"] != null)
                {
                    model.螺丝2马头角度 = row["螺丝2马头角度"].ToString();
                }
                if (row["螺丝2图片路径"] != null)
                {
                    model.螺丝2图片路径 = row["螺丝2图片路径"].ToString();
                }
                if (row["螺丝3马头结果"] != null)
                {
                    model.螺丝3马头结果 = row["螺丝3马头结果"].ToString();
                }
                if (row["螺丝3马头扭矩"] != null)
                {
                    model.螺丝3马头扭矩 = row["螺丝3马头扭矩"].ToString();
                }
                if (row["螺丝3马头角度"] != null)
                {
                    model.螺丝3马头角度 = row["螺丝3马头角度"].ToString();
                }
                if (row["螺丝3图片路径"] != null)
                {
                    model.螺丝3图片路径 = row["螺丝3图片路径"].ToString();
                }
                if (row["螺丝4马头结果"] != null)
                {
                    model.螺丝4马头结果 = row["螺丝4马头结果"].ToString();
                }
                if (row["螺丝4马头扭矩"] != null)
                {
                    model.螺丝4马头扭矩 = row["螺丝4马头扭矩"].ToString();
                }
                if (row["螺丝4马头角度"] != null)
                {
                    model.螺丝4马头角度 = row["螺丝4马头角度"].ToString();
                }
                if (row["螺丝4图片路径"] != null)
                {
                    model.螺丝4图片路径 = row["螺丝4图片路径"].ToString();
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
            strSql.Append("select 测试时间,二维码,螺丝1马头结果,螺丝1马头扭矩,螺丝1马头角度,螺丝1图片路径,螺丝2马头结果,螺丝2马头扭矩,螺丝2马头角度,螺丝2图片路径,螺丝3马头结果,螺丝3马头扭矩,螺丝3马头角度,螺丝3图片路径,螺丝4马头结果,螺丝4马头扭矩,螺丝4马头角度,螺丝4图片路径 ");
            strSql.Append(" FROM db ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return DbHelperOleDb.Query(strSql.ToString());
        }

        /// <summary>
        /// 获取记录总数
        /// </summary>
        public int GetRecordCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) FROM db ");
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
                strSql.Append("order by T.二维码 desc");
            }
            strSql.Append(")AS Row, T.*  from db T ");
            if (!string.IsNullOrEmpty(strWhere.Trim()))
            {
                strSql.Append(" WHERE " + strWhere);
            }
            strSql.Append(" ) TT");
            strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);
            return DbHelperOleDb.Query(strSql.ToString());
        }

        /*
        */

        #endregion  Method
        #region  ExtensionMethod

        #endregion  ExtensionMethod
    }
}

