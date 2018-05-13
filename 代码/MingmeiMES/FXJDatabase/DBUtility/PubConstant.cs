using System;
using System.Configuration;

namespace FXJDatabase
{
    
    public class PubConstant
    {        
        
        /// <summary>
        /// 获取连接字符串
        /// </summary>
        public static string ConnectionString="Data Source = .;Initial Catalog=FangTAIYanji;User ID=sa;Password=123456;";
        //{
        //    //get;
            
        //    get
        //    {
        //        //string connectStr = "Data Source = .\\SQLEXPRESS;Initial Catalog=FangTAIZaojuA;User ID=sa;Password=123456;";
        //        ////string connectStr = "Data Source = .;Initial Catalog=FangTAIZaojuA;User ID=sa;Password=123456;";
        //        ////string dbFileName = AppDomain.CurrentDomain.BaseDirectory + @"ECAMSDataBase.mdf;";
        //        ////string connectStr = @"Data Source =.\SQLEXPRESS;attachDbFileName=" + dbFileName + "Integrated Security=true;User Instance=True";
        //        ////string _connectionString = ConfigurationSettings.AppSettings["connectString"];
             
        //        //return _connectionString;
        //    }
        //    set { }
        //}
        /// <summary>
        /// 作者:np
        /// 时间:2014年5月15日
        /// 内容:连接客户数据库字符串
        /// </summary>
        public static string ConnectionString2
        {
          
            get
            {
                string connectStr = "Data Source = .;Initial Catalog=GXDB;User ID=sa1;Password=123456;";
                return connectStr;
            }
            set { }
        }
    }
}
