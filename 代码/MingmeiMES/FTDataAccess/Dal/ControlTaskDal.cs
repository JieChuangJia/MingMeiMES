using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using FTDataAccess.DBUtility;//Please add references
using FTDataAccess.Model;
namespace FTDataAccess.DAL
{/// <summary>
    /// 数据访问类:ControlTaskModel
    /// </summary>
    public partial class ControlTaskDal
    {
        public ControlTaskDal()
        { }
        #region  BasicMethod
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string TaskID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from ControlTask");
            strSql.Append(" where TaskID=@TaskID ");
            SqlParameter[] parameters = {
					new SqlParameter("@TaskID", SqlDbType.NVarChar,255)			};
            parameters[0].Value = TaskID;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(ControlTaskModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into ControlTask(");
            strSql.Append("TaskID,TaskType,TaskParam,TaskStatus,TaskPhase,CreateTime,FinishTime,CreateMode,Remark,DeviceID,tag1,tag2,tag3,tag4,tag5)");
            strSql.Append(" values (");
            strSql.Append("@TaskID,@TaskType,@TaskParam,@TaskStatus,@TaskPhase,@CreateTime,@FinishTime,@CreateMode,@Remark,@DeviceID,@tag1,@tag2,@tag3,@tag4,@tag5)");
            SqlParameter[] parameters = {
					new SqlParameter("@TaskID", SqlDbType.NVarChar,255),
					new SqlParameter("@TaskType", SqlDbType.Int,4),
					new SqlParameter("@TaskParam", SqlDbType.NVarChar,255),
					new SqlParameter("@TaskStatus", SqlDbType.NVarChar,50),
					new SqlParameter("@TaskPhase", SqlDbType.Int,4),
					new SqlParameter("@CreateTime", SqlDbType.DateTime),
					new SqlParameter("@FinishTime", SqlDbType.DateTime),
					new SqlParameter("@CreateMode", SqlDbType.NVarChar,50),
					new SqlParameter("@Remark", SqlDbType.NVarChar,50),
					new SqlParameter("@DeviceID", SqlDbType.NVarChar,50),
					new SqlParameter("@tag1", SqlDbType.NVarChar,255),
					new SqlParameter("@tag2", SqlDbType.NVarChar,255),
					new SqlParameter("@tag3", SqlDbType.NVarChar,255),
					new SqlParameter("@tag4", SqlDbType.NVarChar,255),
					new SqlParameter("@tag5", SqlDbType.NVarChar,255)
				};
            parameters[0].Value = model.TaskID;
            parameters[1].Value = model.TaskType;
            parameters[2].Value = model.TaskParam;
            parameters[3].Value = model.TaskStatus;
            parameters[4].Value = model.TaskPhase;
            parameters[5].Value = model.CreateTime;
            parameters[6].Value = model.FinishTime;
            parameters[7].Value = model.CreateMode;
            parameters[8].Value = model.Remark;
            parameters[9].Value = model.DeviceID;
            parameters[10].Value = model.tag1;
            parameters[11].Value = model.tag2;
            parameters[12].Value = model.tag3;
            parameters[13].Value = model.tag4;
            parameters[14].Value = model.tag5;
          
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
        public bool Update(ControlTaskModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update ControlTask set ");
            strSql.Append("TaskType=@TaskType,");
            strSql.Append("TaskParam=@TaskParam,");
            strSql.Append("TaskStatus=@TaskStatus,");
            strSql.Append("TaskPhase=@TaskPhase,");
            strSql.Append("CreateTime=@CreateTime,");
            strSql.Append("FinishTime=@FinishTime,");
            strSql.Append("CreateMode=@CreateMode,");
            strSql.Append("Remark=@Remark,");
            strSql.Append("DeviceID=@DeviceID,");
            strSql.Append("tag1=@tag1,");
            strSql.Append("tag2=@tag2,");
            strSql.Append("tag3=@tag3,");
            strSql.Append("tag4=@tag4,");
            strSql.Append("tag5=@tag5");
           
            strSql.Append(" where TaskID=@TaskID ");
            SqlParameter[] parameters = {
					new SqlParameter("@TaskType", SqlDbType.Int,4),
					new SqlParameter("@TaskParam", SqlDbType.NVarChar,255),
					new SqlParameter("@TaskStatus", SqlDbType.NVarChar,50),
					new SqlParameter("@TaskPhase", SqlDbType.Int,4),
					new SqlParameter("@CreateTime", SqlDbType.DateTime),
					new SqlParameter("@FinishTime", SqlDbType.DateTime),
					new SqlParameter("@CreateMode", SqlDbType.NVarChar,50),
					new SqlParameter("@Remark", SqlDbType.NVarChar,50),
					new SqlParameter("@DeviceID", SqlDbType.NVarChar,50),
					new SqlParameter("@tag1", SqlDbType.NVarChar,255),
					new SqlParameter("@tag2", SqlDbType.NVarChar,255),
					new SqlParameter("@tag3", SqlDbType.NVarChar,255),
					new SqlParameter("@tag4", SqlDbType.NVarChar,255),
					new SqlParameter("@tag5", SqlDbType.NVarChar,255),
					new SqlParameter("@TaskID", SqlDbType.NVarChar,255)};
            parameters[0].Value = model.TaskType;
            parameters[1].Value = model.TaskParam;
            parameters[2].Value = model.TaskStatus;
            parameters[3].Value = model.TaskPhase;
            parameters[4].Value = model.CreateTime;
            parameters[5].Value = model.FinishTime;
            parameters[6].Value = model.CreateMode;
            parameters[7].Value = model.Remark;
            parameters[8].Value = model.DeviceID;
            parameters[9].Value = model.tag1;
            parameters[10].Value = model.tag2;
            parameters[11].Value = model.tag3;
            parameters[12].Value = model.tag4;
            parameters[13].Value = model.tag5;
          
            parameters[14].Value = model.TaskID;

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
        public bool Delete(string TaskID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from ControlTask ");
            strSql.Append(" where TaskID=@TaskID ");
            SqlParameter[] parameters = {
					new SqlParameter("@TaskID", SqlDbType.NVarChar,255)			};
            parameters[0].Value = TaskID;

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
        public bool DeleteList(string TaskIDlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from ControlTask ");
            strSql.Append(" where TaskID in (" + TaskIDlist + ")  ");
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
        public ControlTaskModel GetModel(string TaskID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 TaskID,TaskType,TaskParam,TaskStatus,TaskPhase,CreateTime,FinishTime,CreateMode,Remark,DeviceID,tag1,tag2,tag3,tag4,tag5 from ControlTask ");
            strSql.Append(" where TaskID=@TaskID ");
            SqlParameter[] parameters = {
					new SqlParameter("@TaskID", SqlDbType.NVarChar,255)			};
            parameters[0].Value = TaskID;

            ControlTaskModel model = new ControlTaskModel();
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
        public ControlTaskModel DataRowToModel(DataRow row)
        {
            ControlTaskModel model = new ControlTaskModel();
            if (row != null)
            {
                if (row["TaskID"] != null)
                {
                    model.TaskID = row["TaskID"].ToString();
                }
                if (row["TaskType"] != null && row["TaskType"].ToString() != "")
                {
                    model.TaskType = int.Parse(row["TaskType"].ToString());
                }
                if (row["TaskParam"] != null)
                {
                    model.TaskParam = row["TaskParam"].ToString();
                }
                if (row["TaskStatus"] != null)
                {
                    model.TaskStatus = row["TaskStatus"].ToString();
                }
                if (row["TaskPhase"] != null && row["TaskPhase"].ToString() != "")
                {
                    model.TaskPhase = int.Parse(row["TaskPhase"].ToString());
                }
                if (row["CreateTime"] != null && row["CreateTime"].ToString() != "")
                {
                    model.CreateTime = DateTime.Parse(row["CreateTime"].ToString());
                }
                if (row["FinishTime"] != null && row["FinishTime"].ToString() != "")
                {
                    model.FinishTime = DateTime.Parse(row["FinishTime"].ToString());
                }
                if (row["CreateMode"] != null)
                {
                    model.CreateMode = row["CreateMode"].ToString();
                }
                if (row["Remark"] != null)
                {
                    model.Remark = row["Remark"].ToString();
                }
                if (row["DeviceID"] != null)
                {
                    model.DeviceID = row["DeviceID"].ToString();
                }
                if (row["tag1"] != null)
                {
                    model.tag1 = row["tag1"].ToString();
                }
                if (row["tag2"] != null)
                {
                    model.tag2 = row["tag2"].ToString();
                }
                if (row["tag3"] != null)
                {
                    model.tag3 = row["tag3"].ToString();
                }
                if (row["tag4"] != null)
                {
                    model.tag4 = row["tag4"].ToString();
                }
                if (row["tag5"] != null)
                {
                    model.tag5 = row["tag5"].ToString();
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
            strSql.Append("select TaskID,TaskType,TaskParam,TaskStatus,TaskPhase,CreateTime,FinishTime,CreateMode,Remark,DeviceID,tag1,tag2,tag3,tag4,tag5 ");
            strSql.Append(" FROM ControlTask ");
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
            strSql.Append(" TaskID,TaskType,TaskParam,TaskStatus,TaskPhase,CreateTime,FinishTime,CreateMode,Remark,DeviceID,tag1,tag2,tag3,tag4,tag5 ");
            strSql.Append(" FROM ControlTask ");
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
            strSql.Append("select count(1) FROM ControlTask ");
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
                strSql.Append("order by T.TaskID desc");
            }
            strSql.Append(")AS Row, T.*  from ControlTask T ");
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
            parameters[0].Value = "ControlTask";
            parameters[1].Value = "TaskID";
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
