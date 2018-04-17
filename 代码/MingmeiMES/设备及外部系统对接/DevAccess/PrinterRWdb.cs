using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using DevInterface;
namespace DevAccess
{
    public class PrinterRWdb:IPrinterInfoDev
    {
        private bool isConnected = false;
        private string connStr = "";
        public string ConnStr { get { return connStr; } set { connStr = value; } }
        public PrinterRWdb(string dbConn)
        {
            this.connStr = dbConn;
        }
        #region 接口实现
        private int readerID = 1;
        public int ReaderID { get { return readerID; } }
        public bool IsConnect { get { return isConnected; } }
        public bool Connect(ref string reStr)
        {
            return true;
            
        }
        public bool Disconnect(ref string reStr)
        {
            return true;
        }
        public bool SndBarcode(string code,ref string reStr)
        {
            try
            {
               
               // if(GetLatestBarcode() == code)
                if (Exist(code))
                {
                    reStr = code + ",该条码24小时内已经贴过，在贴标队列数据库已经存在";
                    return true;
                }
                StringBuilder strSql = new StringBuilder();
                strSql.Append("insert into tempbar(adddate,barcode) values(@adddate,@barcode) ");
                SqlParameter[] parameters = {
                           new SqlParameter("@adddate", SqlDbType.DateTime),
                           new SqlParameter("@barcode", SqlDbType.NVarChar,30)
                                        };
                parameters[0].Value = System.DateTime.Now;
                parameters[1].Value = code;
                int rows = ExecuteSql(strSql.ToString(), parameters);
                if (rows <= 0)
                {
                    reStr = "提交贴标机系统数据库无效(tempbar表）";
                    return false;
                    
                }
                strSql.Clear();
                strSql.Append("insert into result(adddate,sn,state) values(@adddate,@sn,'wait') ");
                parameters = new SqlParameter[]{
                           new SqlParameter("@adddate", SqlDbType.DateTime),
                           new SqlParameter("@sn", SqlDbType.NVarChar,30)
                                        };
                parameters[0].Value = System.DateTime.Now;
                parameters[1].Value = code;
                rows = ExecuteSql(strSql.ToString(), parameters);
                if (rows <= 0)
                {
                    reStr = "提交贴标机系统数据库无效(result表)";
                    return false;

                }
                reStr = "";
                return true;
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return false;
            }
           
        }
        #endregion

        public string GetLatestBarcode()
        {
            StringBuilder strSql = new StringBuilder();
            //strSql.Append("select top 1 barcode from tempbar order by adddate desc");
            strSql.Append("select top 1 sn from result order by adddate desc");
            DataSet ds = Query(strSql.ToString());
            if (ds.Tables[0].Rows.Count > 0)
            {
               // return ds.Tables[0].Rows[0]["barcode"].ToString();
                return ds.Tables[0].Rows[0]["sn"].ToString();
            }
            else
            {
                return string.Empty;
            }
        }


       /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
         private  int ExecuteSql(string SQLString, params SqlParameter[] cmdParms)
        {
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        throw e;
                    }
                }
            }
        }
         /// <summary>
         /// 执行一条计算查询结果语句，返回查询结果（object）。
         /// </summary>
         /// <param name="SQLString">计算查询结果语句</param>
         /// <returns>查询结果（object）</returns>
         private object GetSingle(string SQLString, params SqlParameter[] cmdParms)
         {
             using (SqlConnection connection = new SqlConnection(connStr))
             {
                 using (SqlCommand cmd = new SqlCommand())
                 {
                     try
                     {
                         PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                         object obj = cmd.ExecuteScalar();
                         cmd.Parameters.Clear();
                         if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                         {
                             return null;
                         }
                         else
                         {
                             return obj;
                         }
                     }
                     catch (System.Data.SqlClient.SqlException e)
                     {
                         throw e;
                     }
                 }
             }
         }
         
         private bool Exist(string barcode)
         {
             
             StringBuilder strSql = new StringBuilder();
            
             strSql.Append("select count(1) from result");
             strSql.Append(" where sn=@sn ");
             DateTime tm = System.DateTime.Now;
             TimeSpan ts = new TimeSpan(24, 0, 0);
             tm = tm - ts;
             strSql.AppendFormat(" and adddate>='{0}' ", tm.ToString("yyyy-MM-dd HH:mm:ss.fff"));
             SqlParameter[] parameters = {
					new SqlParameter("@sn", SqlDbType.NVarChar,30)			};
             parameters[0].Value = barcode;

             return Exists(strSql.ToString(), parameters);
             //if(GetLatestBarcode()==barcode)
             //{
             //    return true;
             //}
             //else
             //{
             //    return false;
             //}
         }
         private bool Exists(string strSql, params SqlParameter[] cmdParms)
         {
             object obj = GetSingle(strSql, cmdParms);
             int cmdresult;
             if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
             {
                 cmdresult = 0;
             }
             else
             {
                 cmdresult = int.Parse(obj.ToString());
             }
             if (cmdresult == 0)
             {
                 return false;
             }
             else
             {
                 return true;
             }
         }
         /// <summary>
         /// 执行一条计算查询结果语句，返回查询结果（object）。
         /// </summary>
         /// <param name="SQLString">计算查询结果语句</param>
         /// <returns>查询结果（object）</returns>
         private object GetSingle(string SQLString)
         {
             using (SqlConnection connection = new SqlConnection(connStr))
             {
                 using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                 {
                     try
                     {
                         connection.Open();
                         object obj = cmd.ExecuteScalar();
                         if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                         {
                             return null;
                         }
                         else
                         {
                             return obj;
                         }
                     }
                     catch (System.Data.SqlClient.SqlException e)
                     {
                         connection.Close();
                         throw e;
                     }
                 }
             }
         }
         /// <summary>
         /// 执行查询语句，返回DataSet
         /// </summary>
         /// <param name="SQLString">查询语句</param>
         /// <returns>DataSet</returns>
         public DataSet Query(string SQLString)
         {
             using (SqlConnection connection = new SqlConnection(connStr))
             {
                 DataSet ds = new DataSet();
                 try
                 {
                     connection.Open();

                     SqlDataAdapter command = new SqlDataAdapter(SQLString, connection);
                     command.Fill(ds, "ds");
                 }
                 catch (System.Data.SqlClient.SqlException ex)
                 {
                     throw new Exception(ex.Message);
                 }
                 return ds;
             }
         }
         private void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string cmdText, SqlParameter[] cmdParms)
         {
             if (conn.State != ConnectionState.Open)
                 conn.Open();
             cmd.Connection = conn;
             cmd.CommandText = cmdText;
             if (trans != null)
                 cmd.Transaction = trans;
             cmd.CommandType = CommandType.Text;//cmdType;
             if (cmdParms != null)
             {


                 foreach (SqlParameter parameter in cmdParms)
                 {
                     if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                         (parameter.Value == null))
                     {
                         parameter.Value = DBNull.Value;
                     }
                     cmd.Parameters.Add(parameter);
                 }
             }
         }
    }
}
