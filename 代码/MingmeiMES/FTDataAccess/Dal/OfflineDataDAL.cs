using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using FTDataAccess.DBUtility;
 
namespace FTDataAccess.DAL
{
    /// <summary>
    /// 数据访问类:OfflineData
    /// </summary>
    public partial class OfflineDataDAL
    {
        public OfflineDataDAL()
        { }
        #region  Method


        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string OfflineDataID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from OfflineData");
            strSql.Append(" where OfflineDataID='" + OfflineDataID + "' ");
            return DbHelperSQL.Exists(strSql.ToString());
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(FTDataAccess.Model.OfflineDataModel model)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder strSql1 = new StringBuilder();
            StringBuilder strSql2 = new StringBuilder();
            if (model.OfflineDataID != null)
            {
                strSql1.Append("OfflineDataID,");
                strSql2.Append("'" + model.OfflineDataID + "',");
            }
            if (model.WorkStationID != null)
            {
                strSql1.Append("WorkStationID,");
                strSql2.Append("'" + model.WorkStationID + "',");
            }
            if (model.DataType != null)
            {
                strSql1.Append("DataType,");
                strSql2.Append("'" + model.DataType + "',");
            }
            if (model.UploadJsonData != null)
            {
                strSql1.Append("UploadJsonData,");
                strSql2.Append("'" + model.UploadJsonData + "',");
            }
            if (model.IsUpLoad != null)
            {
                strSql1.Append("IsUpLoad,");
                strSql2.Append("'" + model.IsUpLoad + "',");
            }
            if (model.CreateTime != null)
            {
                strSql1.Append("CreateTime,");
                strSql2.Append("'" + model.CreateTime + "',");
            }
            if (model.Reserve1 != null)
            {
                strSql1.Append("Reserve1,");
                strSql2.Append("'" + model.Reserve1 + "',");
            }
            if (model.Reserve2 != null)
            {
                strSql1.Append("Reserve2,");
                strSql2.Append("'" + model.Reserve2 + "',");
            }
            if (model.Reserve3 != null)
            {
                strSql1.Append("Reserve3,");
                strSql2.Append("'" + model.Reserve3 + "',");
            }
            if (model.Reserve4 != null)
            {
                strSql1.Append("Reserve4,");
                strSql2.Append("'" + model.Reserve4 + "',");
            }
            if (model.Reserve5 != null)
            {
                strSql1.Append("Reserve5,");
                strSql2.Append("'" + model.Reserve5 + "',");
            }
            strSql.Append("insert into OfflineData(");
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
        public bool Update(FTDataAccess.Model.OfflineDataModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update OfflineData set ");
            if (model.WorkStationID != null)
            {
                strSql.Append("WorkStationID='" + model.WorkStationID + "',");
            }
            if (model.DataType != null)
            {
                strSql.Append("DataType='" + model.DataType + "',");
            }
            if (model.UploadJsonData != null)
            {
                strSql.Append("UploadJsonData='" + model.UploadJsonData + "',");
            }
            if (model.IsUpLoad != null)
            {
                strSql.Append("IsUpLoad='" + model.IsUpLoad + "',");
            }
            if (model.CreateTime != null)
            {
                strSql.Append("CreateTime='" + model.CreateTime + "',");
            }
            if (model.Reserve1 != null)
            {
                strSql.Append("Reserve1='" + model.Reserve1 + "',");
            }
            else
            {
                strSql.Append("Reserve1= null ,");
            }
            if (model.Reserve2 != null)
            {
                strSql.Append("Reserve2='" + model.Reserve2 + "',");
            }
            else
            {
                strSql.Append("Reserve2= null ,");
            }
            if (model.Reserve3 != null)
            {
                strSql.Append("Reserve3='" + model.Reserve3 + "',");
            }
            else
            {
                strSql.Append("Reserve3= null ,");
            }
            if (model.Reserve4 != null)
            {
                strSql.Append("Reserve4='" + model.Reserve4 + "',");
            }
            else
            {
                strSql.Append("Reserve4= null ,");
            }
            if (model.Reserve5 != null)
            {
                strSql.Append("Reserve5='" + model.Reserve5 + "',");
            }
            else
            {
                strSql.Append("Reserve5= null ,");
            }
            int n = strSql.ToString().LastIndexOf(",");
            strSql.Remove(n, 1);
            strSql.Append(" where OfflineDataID='" + model.OfflineDataID + "' ");
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
        public bool Delete(string OfflineDataID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from OfflineData ");
            strSql.Append(" where OfflineDataID='" + OfflineDataID + "' ");
            int rowsAffected = DbHelperSQL.ExecuteSql(strSql.ToString());
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
        public bool DeleteList(string OfflineDataIDlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from OfflineData ");
            strSql.Append(" where OfflineDataID in (" + OfflineDataIDlist + ")  ");
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
        public FTDataAccess.Model.OfflineDataModel GetModel(string OfflineDataID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1  ");
            strSql.Append(" OfflineDataID,WorkStationID,DataType,UploadJsonData,IsUpLoad,CreateTime,Reserve1,Reserve2,Reserve3,Reserve4,Reserve5 ");
            strSql.Append(" from OfflineData ");
            strSql.Append(" where OfflineDataID='" + OfflineDataID + "' ");
            FTDataAccess.Model.OfflineDataModel model = new FTDataAccess.Model.OfflineDataModel();
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
        public FTDataAccess.Model.OfflineDataModel DataRowToModel(DataRow row)
        {
            FTDataAccess.Model.OfflineDataModel model = new FTDataAccess.Model.OfflineDataModel();
            if (row != null)
            {
                if (row["OfflineDataID"] != null)
                {
                    model.OfflineDataID = row["OfflineDataID"].ToString();
                }
                if (row["WorkStationID"] != null)
                {
                    model.WorkStationID = row["WorkStationID"].ToString();
                }
                if (row["DataType"] != null)
                {
                    model.DataType = row["DataType"].ToString();
                }
                if (row["UploadJsonData"] != null)
                {
                    model.UploadJsonData = row["UploadJsonData"].ToString();
                }
                if (row["IsUpLoad"] != null)
                {
                    model.IsUpLoad = row["IsUpLoad"].ToString();
                }
                if (row["CreateTime"] != null && row["CreateTime"].ToString() != "")
                {
                    model.CreateTime = DateTime.Parse(row["CreateTime"].ToString());
                }
                if (row["Reserve1"] != null)
                {
                    model.Reserve1 = row["Reserve1"].ToString();
                }
                if (row["Reserve2"] != null)
                {
                    model.Reserve2 = row["Reserve2"].ToString();
                }
                if (row["Reserve3"] != null)
                {
                    model.Reserve3 = row["Reserve3"].ToString();
                }
                if (row["Reserve4"] != null)
                {
                    model.Reserve4 = row["Reserve4"].ToString();
                }
                if (row["Reserve5"] != null)
                {
                    model.Reserve5 = row["Reserve5"].ToString();
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
            strSql.Append("select OfflineDataID,WorkStationID,DataType,UploadJsonData,IsUpLoad,CreateTime,Reserve1,Reserve2,Reserve3,Reserve4,Reserve5 ");
            strSql.Append(" FROM OfflineData ");
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
            strSql.Append(" OfflineDataID,WorkStationID,DataType,UploadJsonData,IsUpLoad,CreateTime,Reserve1,Reserve2,Reserve3,Reserve4,Reserve5 ");
            strSql.Append(" FROM OfflineData ");
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
            strSql.Append("select count(1) FROM OfflineData ");
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
                strSql.Append("order by T.OfflineDataID desc");
            }
            strSql.Append(")AS Row, T.*  from OfflineData T ");
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

