using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCIRDBAccess
{
    /// <summary>
    /// 数据访问类:dbDal
    /// </summary>
    public partial class dbDCIRDal
    {
        public dbDCIRDal()
        { }
        #region  BasicMethod

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string 测试时间, string 电流, string 电压, string 容量, string 能量, string 总时间, string 相对时间)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from db");
            strSql.Append(" where 测试时间=@测试时间 and 电流=@电流 and 电压=@电压 and 容量=@容量 and 能量=@能量 and 总时间=@总时间 and 相对时间=@相对时间 ");
            OleDbParameter[] parameters = {
					new OleDbParameter("@测试时间", OleDbType.VarChar,255),
					new OleDbParameter("@电流", OleDbType.VarChar,255),
					new OleDbParameter("@电压", OleDbType.VarChar,255),
					new OleDbParameter("@容量", OleDbType.VarChar,255),
					new OleDbParameter("@能量", OleDbType.VarChar,255),
					new OleDbParameter("@总时间", OleDbType.VarChar,255),
					new OleDbParameter("@相对时间", OleDbType.VarChar,255)			};
            parameters[0].Value = 测试时间;
            parameters[1].Value = 电流;
            parameters[2].Value = 电压;
            parameters[3].Value = 容量;
            parameters[4].Value = 能量;
            parameters[5].Value = 总时间;
            parameters[6].Value = 相对时间;

            return DbHelperOleDb.Exists(strSql.ToString(), parameters);
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(dcirMode model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into db(");
            strSql.Append("测试时间,电流,电压,容量,能量,总时间,相对时间)");
            strSql.Append(" values (");
            strSql.Append("@测试时间,@电流,@电压,@容量,@能量,@总时间,@相对时间)");
            OleDbParameter[] parameters = {
					new OleDbParameter("@测试时间", OleDbType.VarChar,255),
					new OleDbParameter("@电流", OleDbType.VarChar,255),
					new OleDbParameter("@电压", OleDbType.VarChar,255),
					new OleDbParameter("@容量", OleDbType.VarChar,255),
					new OleDbParameter("@能量", OleDbType.VarChar,255),
					new OleDbParameter("@总时间", OleDbType.VarChar,255),
					new OleDbParameter("@相对时间", OleDbType.VarChar,255)};
            parameters[0].Value = model.测试时间;
            parameters[1].Value = model.电流;
            parameters[2].Value = model.电压;
            parameters[3].Value = model.容量;
            parameters[4].Value = model.能量;
            parameters[5].Value = model.总时间;
            parameters[6].Value = model.相对时间;

            int rows = DbHelperOleDb.ExecuteSql(strSql.ToString(), parameters);
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
        public bool Update(dcirMode model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update db set ");
            strSql.Append("电流=@电流,");
            strSql.Append("电压=@电压,");
            strSql.Append("容量=@容量,");
            strSql.Append("能量=@能量,");
            strSql.Append("总时间=@总时间,");
            strSql.Append("相对时间=@相对时间");
            strSql.Append(" where 测试时间=@测试时间 and 电流=@电流 and 电压=@电压 and 容量=@容量 and 能量=@能量 and 总时间=@总时间 and 相对时间=@相对时间 ");
            OleDbParameter[] parameters = {
					new OleDbParameter("@电流", OleDbType.VarChar,255),
					new OleDbParameter("@电压", OleDbType.VarChar,255),
					new OleDbParameter("@容量", OleDbType.VarChar,255),
					new OleDbParameter("@能量", OleDbType.VarChar,255),
					new OleDbParameter("@总时间", OleDbType.VarChar,255),
					new OleDbParameter("@相对时间", OleDbType.VarChar,255),
					new OleDbParameter("@测试时间", OleDbType.VarChar,255)};
            parameters[0].Value = model.电流;
            parameters[1].Value = model.电压;
            parameters[2].Value = model.容量;
            parameters[3].Value = model.能量;
            parameters[4].Value = model.总时间;
            parameters[5].Value = model.相对时间;
            parameters[6].Value = model.测试时间;

            int rows = DbHelperOleDb.ExecuteSql(strSql.ToString(), parameters);
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
        public bool Delete(string 测试时间, string 电流, string 电压, string 容量, string 能量, string 总时间, string 相对时间)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from db ");
            strSql.Append(" where 测试时间=@测试时间 and 电流=@电流 and 电压=@电压 and 容量=@容量 and 能量=@能量 and 总时间=@总时间 and 相对时间=@相对时间 ");
            OleDbParameter[] parameters = {
					new OleDbParameter("@测试时间", OleDbType.VarChar,255),
					new OleDbParameter("@电流", OleDbType.VarChar,255),
					new OleDbParameter("@电压", OleDbType.VarChar,255),
					new OleDbParameter("@容量", OleDbType.VarChar,255),
					new OleDbParameter("@能量", OleDbType.VarChar,255),
					new OleDbParameter("@总时间", OleDbType.VarChar,255),
					new OleDbParameter("@相对时间", OleDbType.VarChar,255)			};
            parameters[0].Value = 测试时间;
            parameters[1].Value = 电流;
            parameters[2].Value = 电压;
            parameters[3].Value = 容量;
            parameters[4].Value = 能量;
            parameters[5].Value = 总时间;
            parameters[6].Value = 相对时间;

            int rows = DbHelperOleDb.ExecuteSql(strSql.ToString(), parameters);
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
        public dcirMode GetModel(string 测试时间, string 电流, string 电压, string 容量, string 能量, string 总时间, string 相对时间)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select 测试时间,电流,电压,容量,能量,总时间,相对时间 from db ");
            strSql.Append(" where 测试时间=@测试时间 and 电流=@电流 and 电压=@电压 and 容量=@容量 and 能量=@能量 and 总时间=@总时间 and 相对时间=@相对时间 ");
            OleDbParameter[] parameters = {
					new OleDbParameter("@测试时间", OleDbType.VarChar,255),
					new OleDbParameter("@电流", OleDbType.VarChar,255),
					new OleDbParameter("@电压", OleDbType.VarChar,255),
					new OleDbParameter("@容量", OleDbType.VarChar,255),
					new OleDbParameter("@能量", OleDbType.VarChar,255),
					new OleDbParameter("@总时间", OleDbType.VarChar,255),
					new OleDbParameter("@相对时间", OleDbType.VarChar,255)			};
            parameters[0].Value = 测试时间;
            parameters[1].Value = 电流;
            parameters[2].Value = 电压;
            parameters[3].Value = 容量;
            parameters[4].Value = 能量;
            parameters[5].Value = 总时间;
            parameters[6].Value = 相对时间;

            dcirMode model = new dcirMode();
            DataSet ds = DbHelperOleDb.Query(strSql.ToString(), parameters);
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
        public dcirMode DataRowToModel(DataRow row)
        {
            dcirMode model = new dcirMode();
            if (row != null)
            {
                if (row["测试时间"] != null)
                {
                    model.测试时间 = row["测试时间"].ToString();
                }
                if (row["电流"] != null)
                {
                    model.电流 = row["电流"].ToString();
                }
                if (row["电压"] != null)
                {
                    model.电压 = row["电压"].ToString();
                }
                if (row["容量"] != null)
                {
                    model.容量 = row["容量"].ToString();
                }
                if (row["能量"] != null)
                {
                    model.能量 = row["能量"].ToString();
                }
                if (row["总时间"] != null)
                {
                    model.总时间 = row["总时间"].ToString();
                }
                if (row["相对时间"] != null)
                {
                    model.相对时间 = row["相对时间"].ToString();
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
            strSql.Append("select 测试时间,电流,电压,容量,能量,总时间,相对时间 ");
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
                strSql.Append("order by T.相对时间 desc");
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
        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public DataSet GetList(int PageSize,int PageIndex,string strWhere)
        {
            OleDbParameter[] parameters = {
                    new OleDbParameter("@tblName", OleDbType.VarChar, 255),
                    new OleDbParameter("@fldName", OleDbType.VarChar, 255),
                    new OleDbParameter("@PageSize", OleDbType.Integer),
                    new OleDbParameter("@PageIndex", OleDbType.Integer),
                    new OleDbParameter("@IsReCount", OleDbType.Boolean),
                    new OleDbParameter("@OrderType", OleDbType.Boolean),
                    new OleDbParameter("@strWhere", OleDbType.VarChar,1000),
                    };
            parameters[0].Value = "db";
            parameters[1].Value = "相对时间";
            parameters[2].Value = PageSize;
            parameters[3].Value = PageIndex;
            parameters[4].Value = 0;
            parameters[5].Value = 0;
            parameters[6].Value = strWhere;	
            return DbHelperOleDb.RunProcedure("UP_GetRecordByPage",parameters,"ds");
        }*/

        #endregion  BasicMethod
        #region  ExtensionMethod
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetListByTime()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select 测试时间,电流,电压,容量,能量,总时间,相对时间 ");
            strSql.Append(" FROM db order by 测试时间");
            return DbHelperOleDb.Query(strSql.ToString());
        }
        #endregion  ExtensionMethod
    }
}
