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
        public bool Exists(string 测试时间, string 二维码, string 电阻值, string 放电电流1, string 放电电流2, string 放电时间, string 静置时间, string 结果电压, string 结果电流)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from db");
            strSql.Append(" where 测试时间=@测试时间 and 二维码=@二维码 and 电阻值=@电阻值 and 放电电流1=@放电电流1 and 放电电流2=@放电电流2 and 放电时间=@放电时间 and 静置时间=@静置时间 and 结果电压=@结果电压 and 结果电流=@结果电流 ");
            OleDbParameter[] parameters = {
					new OleDbParameter("@测试时间", OleDbType.VarChar,255),
					new OleDbParameter("@二维码", OleDbType.VarChar,255),
					new OleDbParameter("@电阻值", OleDbType.VarChar,255),
					new OleDbParameter("@放电电流1", OleDbType.VarChar,255),
					new OleDbParameter("@放电电流2", OleDbType.VarChar,255),
					new OleDbParameter("@放电时间", OleDbType.VarChar,255),
					new OleDbParameter("@静置时间", OleDbType.VarChar,255),
					new OleDbParameter("@结果电压", OleDbType.VarChar,255),
					new OleDbParameter("@结果电流", OleDbType.VarChar,255)			};
            parameters[0].Value = 测试时间;
            parameters[1].Value = 二维码;
            parameters[2].Value = 电阻值;
            parameters[3].Value = 放电电流1;
            parameters[4].Value = 放电电流2;
            parameters[5].Value = 放电时间;
            parameters[6].Value = 静置时间;
            parameters[7].Value = 结果电压;
            parameters[8].Value = 结果电流;

            return DbHelperOleDb.Exists(strSql.ToString(), parameters);
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(DCIRDBAccess.dcirMode model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into db(");
            strSql.Append("测试时间,二维码,电阻值,放电电流1,放电电流2,放电时间,静置时间,结果电压,结果电流)");
            strSql.Append(" values (");
            strSql.Append("@测试时间,@二维码,@电阻值,@放电电流1,@放电电流2,@放电时间,@静置时间,@结果电压,@结果电流)");
            OleDbParameter[] parameters = {
					new OleDbParameter("@测试时间", OleDbType.VarChar,255),
					new OleDbParameter("@二维码", OleDbType.VarChar,255),
					new OleDbParameter("@电阻值", OleDbType.VarChar,255),
					new OleDbParameter("@放电电流1", OleDbType.VarChar,255),
					new OleDbParameter("@放电电流2", OleDbType.VarChar,255),
					new OleDbParameter("@放电时间", OleDbType.VarChar,255),
					new OleDbParameter("@静置时间", OleDbType.VarChar,255),
					new OleDbParameter("@结果电压", OleDbType.VarChar,255),
					new OleDbParameter("@结果电流", OleDbType.VarChar,255)};
            parameters[0].Value = model.测试时间;
            parameters[1].Value = model.二维码;
            parameters[2].Value = model.电阻值;
            parameters[3].Value = model.放电电流1;
            parameters[4].Value = model.放电电流2;
            parameters[5].Value = model.放电时间;
            parameters[6].Value = model.静置时间;
            parameters[7].Value = model.结果电压;
            parameters[8].Value = model.结果电流;

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
        public bool Update(DCIRDBAccess.dcirMode model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update db set ");
            strSql.Append("二维码=@二维码,");
            strSql.Append("电阻值=@电阻值,");
            strSql.Append("放电电流1=@放电电流1,");
            strSql.Append("放电电流2=@放电电流2,");
            strSql.Append("放电时间=@放电时间,");
            strSql.Append("静置时间=@静置时间,");
            strSql.Append("结果电压=@结果电压,");
            strSql.Append("结果电流=@结果电流");
            strSql.Append(" where 测试时间=@测试时间 and 二维码=@二维码 and 电阻值=@电阻值 and 放电电流1=@放电电流1 and 放电电流2=@放电电流2 and 放电时间=@放电时间 and 静置时间=@静置时间 and 结果电压=@结果电压 and 结果电流=@结果电流 ");
            OleDbParameter[] parameters = {
					new OleDbParameter("@二维码", OleDbType.VarChar,255),
					new OleDbParameter("@电阻值", OleDbType.VarChar,255),
					new OleDbParameter("@放电电流1", OleDbType.VarChar,255),
					new OleDbParameter("@放电电流2", OleDbType.VarChar,255),
					new OleDbParameter("@放电时间", OleDbType.VarChar,255),
					new OleDbParameter("@静置时间", OleDbType.VarChar,255),
					new OleDbParameter("@结果电压", OleDbType.VarChar,255),
					new OleDbParameter("@结果电流", OleDbType.VarChar,255),
					new OleDbParameter("@测试时间", OleDbType.VarChar,255)};
            parameters[0].Value = model.二维码;
            parameters[1].Value = model.电阻值;
            parameters[2].Value = model.放电电流1;
            parameters[3].Value = model.放电电流2;
            parameters[4].Value = model.放电时间;
            parameters[5].Value = model.静置时间;
            parameters[6].Value = model.结果电压;
            parameters[7].Value = model.结果电流;
            parameters[8].Value = model.测试时间;

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
        public bool Delete(string 测试时间, string 二维码, string 电阻值, string 放电电流1, string 放电电流2, string 放电时间, string 静置时间, string 结果电压, string 结果电流)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from db ");
            strSql.Append(" where 测试时间=@测试时间 and 二维码=@二维码 and 电阻值=@电阻值 and 放电电流1=@放电电流1 and 放电电流2=@放电电流2 and 放电时间=@放电时间 and 静置时间=@静置时间 and 结果电压=@结果电压 and 结果电流=@结果电流 ");
            OleDbParameter[] parameters = {
					new OleDbParameter("@测试时间", OleDbType.VarChar,255),
					new OleDbParameter("@二维码", OleDbType.VarChar,255),
					new OleDbParameter("@电阻值", OleDbType.VarChar,255),
					new OleDbParameter("@放电电流1", OleDbType.VarChar,255),
					new OleDbParameter("@放电电流2", OleDbType.VarChar,255),
					new OleDbParameter("@放电时间", OleDbType.VarChar,255),
					new OleDbParameter("@静置时间", OleDbType.VarChar,255),
					new OleDbParameter("@结果电压", OleDbType.VarChar,255),
					new OleDbParameter("@结果电流", OleDbType.VarChar,255)			};
            parameters[0].Value = 测试时间;
            parameters[1].Value = 二维码;
            parameters[2].Value = 电阻值;
            parameters[3].Value = 放电电流1;
            parameters[4].Value = 放电电流2;
            parameters[5].Value = 放电时间;
            parameters[6].Value = 静置时间;
            parameters[7].Value = 结果电压;
            parameters[8].Value = 结果电流;

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
        public DCIRDBAccess.dcirMode GetModel(string 测试时间, string 二维码, string 电阻值, string 放电电流1, string 放电电流2, string 放电时间, string 静置时间, string 结果电压, string 结果电流)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select 测试时间,二维码,电阻值,放电电流1,放电电流2,放电时间,静置时间,结果电压,结果电流 from db ");
            strSql.Append(" where 测试时间=@测试时间 and 二维码=@二维码 and 电阻值=@电阻值 and 放电电流1=@放电电流1 and 放电电流2=@放电电流2 and 放电时间=@放电时间 and 静置时间=@静置时间 and 结果电压=@结果电压 and 结果电流=@结果电流 ");
            OleDbParameter[] parameters = {
					new OleDbParameter("@测试时间", OleDbType.VarChar,255),
					new OleDbParameter("@二维码", OleDbType.VarChar,255),
					new OleDbParameter("@电阻值", OleDbType.VarChar,255),
					new OleDbParameter("@放电电流1", OleDbType.VarChar,255),
					new OleDbParameter("@放电电流2", OleDbType.VarChar,255),
					new OleDbParameter("@放电时间", OleDbType.VarChar,255),
					new OleDbParameter("@静置时间", OleDbType.VarChar,255),
					new OleDbParameter("@结果电压", OleDbType.VarChar,255),
					new OleDbParameter("@结果电流", OleDbType.VarChar,255)			};
            parameters[0].Value = 测试时间;
            parameters[1].Value = 二维码;
            parameters[2].Value = 电阻值;
            parameters[3].Value = 放电电流1;
            parameters[4].Value = 放电电流2;
            parameters[5].Value = 放电时间;
            parameters[6].Value = 静置时间;
            parameters[7].Value = 结果电压;
            parameters[8].Value = 结果电流;

            DCIRDBAccess.dcirMode model = new DCIRDBAccess.dcirMode();
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
        public DCIRDBAccess.dcirMode DataRowToModel(DataRow row)
        {
            DCIRDBAccess.dcirMode model = new DCIRDBAccess.dcirMode();
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
                if (row["电阻值"] != null)
                {
                    model.电阻值 = row["电阻值"].ToString();
                }
                if (row["放电电流1"] != null)
                {
                    model.放电电流1 = row["放电电流1"].ToString();
                }
                if (row["放电电流2"] != null)
                {
                    model.放电电流2 = row["放电电流2"].ToString();
                }
                if (row["放电时间"] != null)
                {
                    model.放电时间 = row["放电时间"].ToString();
                }
                if (row["静置时间"] != null)
                {
                    model.静置时间 = row["静置时间"].ToString();
                }
                if (row["结果电压"] != null)
                {
                    model.结果电压 = row["结果电压"].ToString();
                }
                if (row["结果电流"] != null)
                {
                    model.结果电流 = row["结果电流"].ToString();
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
            strSql.Append("select 测试时间,二维码,电阻值,放电电流1,放电电流2,放电时间,静置时间,结果电压,结果电流 ");
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
                strSql.Append("order by T.结果电流 desc");
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
            parameters[1].Value = "结果电流";
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
