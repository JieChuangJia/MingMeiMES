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
        #region  BasicMethod

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string 测试时间, string 二维码, string 螺丝1马头结果, string 螺丝1马头扭矩, string 螺丝1马头角度, string 螺丝1图片路径, string 螺丝2马头结果, string 螺丝2马头扭矩, string 螺丝2马头角度, string 螺丝2图片路径, string 螺丝3马头结果, string 螺丝3马头扭矩, string 螺丝3马头角度, string 螺丝3图片路径, string 螺丝4马头结果, string 螺丝4马头扭矩, string 螺丝4马头角度, string 螺丝4图片路径, bool UpLoad)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from db");
            strSql.Append(" where 测试时间=@测试时间 and 二维码=@二维码 and 螺丝1马头结果=@螺丝1马头结果 and 螺丝1马头扭矩=@螺丝1马头扭矩 and 螺丝1马头角度=@螺丝1马头角度 and 螺丝1图片路径=@螺丝1图片路径 and 螺丝2马头结果=@螺丝2马头结果 and 螺丝2马头扭矩=@螺丝2马头扭矩 and 螺丝2马头角度=@螺丝2马头角度 and 螺丝2图片路径=@螺丝2图片路径 and 螺丝3马头结果=@螺丝3马头结果 and 螺丝3马头扭矩=@螺丝3马头扭矩 and 螺丝3马头角度=@螺丝3马头角度 and 螺丝3图片路径=@螺丝3图片路径 and 螺丝4马头结果=@螺丝4马头结果 and 螺丝4马头扭矩=@螺丝4马头扭矩 and 螺丝4马头角度=@螺丝4马头角度 and 螺丝4图片路径=@螺丝4图片路径 and UpLoad=@UpLoad ");
            OleDbParameter[] parameters = {
					new OleDbParameter("@测试时间", OleDbType.VarChar,255),
					new OleDbParameter("@二维码", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝1马头结果", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝1马头扭矩", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝1马头角度", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝1图片路径", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝2马头结果", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝2马头扭矩", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝2马头角度", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝2图片路径", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝3马头结果", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝3马头扭矩", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝3马头角度", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝3图片路径", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝4马头结果", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝4马头扭矩", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝4马头角度", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝4图片路径", OleDbType.VarChar,255),
					new OleDbParameter("@UpLoad", OleDbType.Boolean,1)			};
            parameters[0].Value = 测试时间;
            parameters[1].Value = 二维码;
            parameters[2].Value = 螺丝1马头结果;
            parameters[3].Value = 螺丝1马头扭矩;
            parameters[4].Value = 螺丝1马头角度;
            parameters[5].Value = 螺丝1图片路径;
            parameters[6].Value = 螺丝2马头结果;
            parameters[7].Value = 螺丝2马头扭矩;
            parameters[8].Value = 螺丝2马头角度;
            parameters[9].Value = 螺丝2图片路径;
            parameters[10].Value = 螺丝3马头结果;
            parameters[11].Value = 螺丝3马头扭矩;
            parameters[12].Value = 螺丝3马头角度;
            parameters[13].Value = 螺丝3图片路径;
            parameters[14].Value = 螺丝4马头结果;
            parameters[15].Value = 螺丝4马头扭矩;
            parameters[16].Value = 螺丝4马头角度;
            parameters[17].Value = 螺丝4图片路径;
            parameters[18].Value = UpLoad;

            return DbHelperOleDb.Exists(strSql.ToString(), parameters);
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(ALineScrewDB.dbModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into db(");
            strSql.Append("测试时间,二维码,螺丝1马头结果,螺丝1马头扭矩,螺丝1马头角度,螺丝1图片路径,螺丝2马头结果,螺丝2马头扭矩,螺丝2马头角度,螺丝2图片路径,螺丝3马头结果,螺丝3马头扭矩,螺丝3马头角度,螺丝3图片路径,螺丝4马头结果,螺丝4马头扭矩,螺丝4马头角度,螺丝4图片路径,UpLoad)");
            strSql.Append(" values (");
            strSql.Append("@测试时间,@二维码,@螺丝1马头结果,@螺丝1马头扭矩,@螺丝1马头角度,@螺丝1图片路径,@螺丝2马头结果,@螺丝2马头扭矩,@螺丝2马头角度,@螺丝2图片路径,@螺丝3马头结果,@螺丝3马头扭矩,@螺丝3马头角度,@螺丝3图片路径,@螺丝4马头结果,@螺丝4马头扭矩,@螺丝4马头角度,@螺丝4图片路径,@UpLoad)");
            OleDbParameter[] parameters = {
					new OleDbParameter("@测试时间", OleDbType.VarChar,255),
					new OleDbParameter("@二维码", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝1马头结果", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝1马头扭矩", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝1马头角度", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝1图片路径", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝2马头结果", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝2马头扭矩", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝2马头角度", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝2图片路径", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝3马头结果", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝3马头扭矩", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝3马头角度", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝3图片路径", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝4马头结果", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝4马头扭矩", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝4马头角度", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝4图片路径", OleDbType.VarChar,255),
					new OleDbParameter("@UpLoad", OleDbType.Boolean,1)};
            parameters[0].Value = model.测试时间;
            parameters[1].Value = model.二维码;
            parameters[2].Value = model.螺丝1马头结果;
            parameters[3].Value = model.螺丝1马头扭矩;
            parameters[4].Value = model.螺丝1马头角度;
            parameters[5].Value = model.螺丝1图片路径;
            parameters[6].Value = model.螺丝2马头结果;
            parameters[7].Value = model.螺丝2马头扭矩;
            parameters[8].Value = model.螺丝2马头角度;
            parameters[9].Value = model.螺丝2图片路径;
            parameters[10].Value = model.螺丝3马头结果;
            parameters[11].Value = model.螺丝3马头扭矩;
            parameters[12].Value = model.螺丝3马头角度;
            parameters[13].Value = model.螺丝3图片路径;
            parameters[14].Value = model.螺丝4马头结果;
            parameters[15].Value = model.螺丝4马头扭矩;
            parameters[16].Value = model.螺丝4马头角度;
            parameters[17].Value = model.螺丝4图片路径;
            parameters[18].Value = model.UpLoad;

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
        public bool Update(ALineScrewDB.dbModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update db set ");
            strSql.Append("测试时间=@测试时间,");
            strSql.Append("螺丝1马头结果=@螺丝1马头结果,");
            strSql.Append("螺丝1马头扭矩=@螺丝1马头扭矩,");
            strSql.Append("螺丝1马头角度=@螺丝1马头角度,");
            strSql.Append("螺丝1图片路径=@螺丝1图片路径,");
            strSql.Append("螺丝2马头结果=@螺丝2马头结果,");
            strSql.Append("螺丝2马头扭矩=@螺丝2马头扭矩,");
            strSql.Append("螺丝2马头角度=@螺丝2马头角度,");
            strSql.Append("螺丝2图片路径=@螺丝2图片路径,");
            strSql.Append("螺丝3马头结果=@螺丝3马头结果,");
            strSql.Append("螺丝3马头扭矩=@螺丝3马头扭矩,");
            strSql.Append("螺丝3马头角度=@螺丝3马头角度,");
            strSql.Append("螺丝3图片路径=@螺丝3图片路径,");
            strSql.Append("螺丝4马头结果=@螺丝4马头结果,");
            strSql.Append("螺丝4马头扭矩=@螺丝4马头扭矩,");
            strSql.Append("螺丝4马头角度=@螺丝4马头角度,");
            strSql.Append("螺丝4图片路径=@螺丝4图片路径,");
            strSql.Append("UpLoad=@UpLoad");
            strSql.Append(" where 测试时间=@测试时间 and 二维码=@二维码 and 螺丝1马头结果=@螺丝1马头结果 and 螺丝1马头扭矩=@螺丝1马头扭矩 and 螺丝1马头角度=@螺丝1马头角度 and 螺丝1图片路径=@螺丝1图片路径 and 螺丝2马头结果=@螺丝2马头结果 and 螺丝2马头扭矩=@螺丝2马头扭矩 and 螺丝2马头角度=@螺丝2马头角度 and 螺丝2图片路径=@螺丝2图片路径 and 螺丝3马头结果=@螺丝3马头结果 and 螺丝3马头扭矩=@螺丝3马头扭矩 and 螺丝3马头角度=@螺丝3马头角度 and 螺丝3图片路径=@螺丝3图片路径 and 螺丝4马头结果=@螺丝4马头结果 and 螺丝4马头扭矩=@螺丝4马头扭矩 and 螺丝4马头角度=@螺丝4马头角度 and 螺丝4图片路径=@螺丝4图片路径 and UpLoad=@UpLoad ");
            OleDbParameter[] parameters = {
					new OleDbParameter("@测试时间", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝1马头结果", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝1马头扭矩", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝1马头角度", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝1图片路径", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝2马头结果", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝2马头扭矩", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝2马头角度", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝2图片路径", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝3马头结果", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝3马头扭矩", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝3马头角度", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝3图片路径", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝4马头结果", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝4马头扭矩", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝4马头角度", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝4图片路径", OleDbType.VarChar,255),
					new OleDbParameter("@UpLoad", OleDbType.Boolean,1),
					new OleDbParameter("@二维码", OleDbType.VarChar,255)};
            parameters[0].Value = model.测试时间;
            parameters[1].Value = model.螺丝1马头结果;
            parameters[2].Value = model.螺丝1马头扭矩;
            parameters[3].Value = model.螺丝1马头角度;
            parameters[4].Value = model.螺丝1图片路径;
            parameters[5].Value = model.螺丝2马头结果;
            parameters[6].Value = model.螺丝2马头扭矩;
            parameters[7].Value = model.螺丝2马头角度;
            parameters[8].Value = model.螺丝2图片路径;
            parameters[9].Value = model.螺丝3马头结果;
            parameters[10].Value = model.螺丝3马头扭矩;
            parameters[11].Value = model.螺丝3马头角度;
            parameters[12].Value = model.螺丝3图片路径;
            parameters[13].Value = model.螺丝4马头结果;
            parameters[14].Value = model.螺丝4马头扭矩;
            parameters[15].Value = model.螺丝4马头角度;
            parameters[16].Value = model.螺丝4图片路径;
            parameters[17].Value = model.UpLoad;
            parameters[18].Value = model.二维码;

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
        public bool Delete(string 测试时间, string 二维码, string 螺丝1马头结果, string 螺丝1马头扭矩, string 螺丝1马头角度, string 螺丝1图片路径, string 螺丝2马头结果, string 螺丝2马头扭矩, string 螺丝2马头角度, string 螺丝2图片路径, string 螺丝3马头结果, string 螺丝3马头扭矩, string 螺丝3马头角度, string 螺丝3图片路径, string 螺丝4马头结果, string 螺丝4马头扭矩, string 螺丝4马头角度, string 螺丝4图片路径, bool UpLoad)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from db ");
            strSql.Append(" where 测试时间=@测试时间 and 二维码=@二维码 and 螺丝1马头结果=@螺丝1马头结果 and 螺丝1马头扭矩=@螺丝1马头扭矩 and 螺丝1马头角度=@螺丝1马头角度 and 螺丝1图片路径=@螺丝1图片路径 and 螺丝2马头结果=@螺丝2马头结果 and 螺丝2马头扭矩=@螺丝2马头扭矩 and 螺丝2马头角度=@螺丝2马头角度 and 螺丝2图片路径=@螺丝2图片路径 and 螺丝3马头结果=@螺丝3马头结果 and 螺丝3马头扭矩=@螺丝3马头扭矩 and 螺丝3马头角度=@螺丝3马头角度 and 螺丝3图片路径=@螺丝3图片路径 and 螺丝4马头结果=@螺丝4马头结果 and 螺丝4马头扭矩=@螺丝4马头扭矩 and 螺丝4马头角度=@螺丝4马头角度 and 螺丝4图片路径=@螺丝4图片路径 and UpLoad=@UpLoad ");
            OleDbParameter[] parameters = {
					new OleDbParameter("@测试时间", OleDbType.VarChar,255),
					new OleDbParameter("@二维码", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝1马头结果", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝1马头扭矩", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝1马头角度", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝1图片路径", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝2马头结果", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝2马头扭矩", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝2马头角度", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝2图片路径", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝3马头结果", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝3马头扭矩", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝3马头角度", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝3图片路径", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝4马头结果", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝4马头扭矩", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝4马头角度", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝4图片路径", OleDbType.VarChar,255),
					new OleDbParameter("@UpLoad", OleDbType.Boolean,1)			};
            parameters[0].Value = 测试时间;
            parameters[1].Value = 二维码;
            parameters[2].Value = 螺丝1马头结果;
            parameters[3].Value = 螺丝1马头扭矩;
            parameters[4].Value = 螺丝1马头角度;
            parameters[5].Value = 螺丝1图片路径;
            parameters[6].Value = 螺丝2马头结果;
            parameters[7].Value = 螺丝2马头扭矩;
            parameters[8].Value = 螺丝2马头角度;
            parameters[9].Value = 螺丝2图片路径;
            parameters[10].Value = 螺丝3马头结果;
            parameters[11].Value = 螺丝3马头扭矩;
            parameters[12].Value = 螺丝3马头角度;
            parameters[13].Value = 螺丝3图片路径;
            parameters[14].Value = 螺丝4马头结果;
            parameters[15].Value = 螺丝4马头扭矩;
            parameters[16].Value = 螺丝4马头角度;
            parameters[17].Value = 螺丝4图片路径;
            parameters[18].Value = UpLoad;

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
        public ALineScrewDB.dbModel GetModel(string 测试时间, string 二维码, string 螺丝1马头结果, string 螺丝1马头扭矩, string 螺丝1马头角度, string 螺丝1图片路径, string 螺丝2马头结果, string 螺丝2马头扭矩, string 螺丝2马头角度, string 螺丝2图片路径, string 螺丝3马头结果, string 螺丝3马头扭矩, string 螺丝3马头角度, string 螺丝3图片路径, string 螺丝4马头结果, string 螺丝4马头扭矩, string 螺丝4马头角度, string 螺丝4图片路径, bool UpLoad)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select 测试时间,二维码,螺丝1马头结果,螺丝1马头扭矩,螺丝1马头角度,螺丝1图片路径,螺丝2马头结果,螺丝2马头扭矩,螺丝2马头角度,螺丝2图片路径,螺丝3马头结果,螺丝3马头扭矩,螺丝3马头角度,螺丝3图片路径,螺丝4马头结果,螺丝4马头扭矩,螺丝4马头角度,螺丝4图片路径,UpLoad from db ");
            strSql.Append(" where 测试时间=@测试时间 and 二维码=@二维码 and 螺丝1马头结果=@螺丝1马头结果 and 螺丝1马头扭矩=@螺丝1马头扭矩 and 螺丝1马头角度=@螺丝1马头角度 and 螺丝1图片路径=@螺丝1图片路径 and 螺丝2马头结果=@螺丝2马头结果 and 螺丝2马头扭矩=@螺丝2马头扭矩 and 螺丝2马头角度=@螺丝2马头角度 and 螺丝2图片路径=@螺丝2图片路径 and 螺丝3马头结果=@螺丝3马头结果 and 螺丝3马头扭矩=@螺丝3马头扭矩 and 螺丝3马头角度=@螺丝3马头角度 and 螺丝3图片路径=@螺丝3图片路径 and 螺丝4马头结果=@螺丝4马头结果 and 螺丝4马头扭矩=@螺丝4马头扭矩 and 螺丝4马头角度=@螺丝4马头角度 and 螺丝4图片路径=@螺丝4图片路径 and UpLoad=@UpLoad ");
            OleDbParameter[] parameters = {
					new OleDbParameter("@测试时间", OleDbType.VarChar,255),
					new OleDbParameter("@二维码", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝1马头结果", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝1马头扭矩", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝1马头角度", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝1图片路径", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝2马头结果", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝2马头扭矩", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝2马头角度", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝2图片路径", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝3马头结果", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝3马头扭矩", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝3马头角度", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝3图片路径", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝4马头结果", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝4马头扭矩", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝4马头角度", OleDbType.VarChar,255),
					new OleDbParameter("@螺丝4图片路径", OleDbType.VarChar,255),
					new OleDbParameter("@UpLoad", OleDbType.Boolean,1)			};
            parameters[0].Value = 测试时间;
            parameters[1].Value = 二维码;
            parameters[2].Value = 螺丝1马头结果;
            parameters[3].Value = 螺丝1马头扭矩;
            parameters[4].Value = 螺丝1马头角度;
            parameters[5].Value = 螺丝1图片路径;
            parameters[6].Value = 螺丝2马头结果;
            parameters[7].Value = 螺丝2马头扭矩;
            parameters[8].Value = 螺丝2马头角度;
            parameters[9].Value = 螺丝2图片路径;
            parameters[10].Value = 螺丝3马头结果;
            parameters[11].Value = 螺丝3马头扭矩;
            parameters[12].Value = 螺丝3马头角度;
            parameters[13].Value = 螺丝3图片路径;
            parameters[14].Value = 螺丝4马头结果;
            parameters[15].Value = 螺丝4马头扭矩;
            parameters[16].Value = 螺丝4马头角度;
            parameters[17].Value = 螺丝4图片路径;
            parameters[18].Value = UpLoad;

            ALineScrewDB.dbModel model = new ALineScrewDB.dbModel();
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
                if (row["UpLoad"] != null && row["UpLoad"].ToString() != "")
                {
                    if ((row["UpLoad"].ToString() == "1") || (row["UpLoad"].ToString().ToLower() == "true"))
                    {
                        model.UpLoad = true;
                    }
                    else
                    {
                        model.UpLoad = false;
                    }
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
            strSql.Append("select 测试时间,二维码,螺丝1马头结果,螺丝1马头扭矩,螺丝1马头角度,螺丝1图片路径,螺丝2马头结果,螺丝2马头扭矩,螺丝2马头角度,螺丝2图片路径,螺丝3马头结果,螺丝3马头扭矩,螺丝3马头角度,螺丝3图片路径,螺丝4马头结果,螺丝4马头扭矩,螺丝4马头角度,螺丝4图片路径,UpLoad ");
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
                strSql.Append("order by T.UpLoad desc");
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
            parameters[1].Value = "UpLoad";
            parameters[2].Value = PageSize;
            parameters[3].Value = PageIndex;
            parameters[4].Value = 0;
            parameters[5].Value = 0;
            parameters[6].Value = strWhere;	
            return DbHelperOleDb.RunProcedure("UP_GetRecordByPage",parameters,"ds");
        }*/

        #endregion  BasicMethod
        #region  ExtensionMethod

        #endregion  ExtensionMethod
    }
}

