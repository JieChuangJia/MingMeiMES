using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
 
namespace FXJDatabase
{
    /// <summary>
    /// 数据访问类:tb_CheckDataonline
    /// </summary>
    public partial class tb_CheckDataonlineDAL
    {
        public tb_CheckDataonlineDAL()
        { }
        #region  Method


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public long Add(FXJDatabase.tb_CheckDataonlineModel model)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder strSql1 = new StringBuilder();
            StringBuilder strSql2 = new StringBuilder();
            if (model.ID != null)
            {
                strSql1.Append("ID,");
                strSql2.Append("" + model.ID + ",");
            }
            if (model.BarCode != null)
            {
                strSql1.Append("BarCode,");
                strSql2.Append("'" + model.BarCode + "',");
            }
            if (model.cType != null)
            {
                strSql1.Append("cType,");
                strSql2.Append("'" + model.cType + "',");
            }
            if (model.FileName != null)
            {
                strSql1.Append("FileName,");
                strSql2.Append("'" + model.FileName + "',");
            }
            if (model.fltCapacity != null)
            {
                strSql1.Append("fltCapacity,");
                strSql2.Append("'" + model.fltCapacity + "',");
            }
            if (model.fltVolGap != null)
            {
                strSql1.Append("fltVolGap,");
                strSql2.Append("" + model.fltVolGap + ",");
            }
            if (model.fltSelfFrequency != null)
            {
                strSql1.Append("fltSelfFrequency,");
                strSql2.Append("" + model.fltSelfFrequency + ",");
            }
            if (model.fltVol != null)
            {
                strSql1.Append("fltVol,");
                strSql2.Append("" + model.fltVol + ",");
            }
            if (model.fltResistance != null)
            {
                strSql1.Append("fltResistance,");
                strSql2.Append("" + model.fltResistance + ",");
            }
            if (model.cGrade != null)
            {
                strSql1.Append("cGrade,");
                strSql2.Append("'" + model.cGrade + "',");
            }
            if (model.cState != null)
            {
                strSql1.Append("cState,");
                strSql2.Append("" + model.cState + ",");
            }
            if (model.cDate != null)
            {
                strSql1.Append("cDate,");
                strSql2.Append("'" + model.cDate + "',");
            }
            if (model.cStateCode != null)
            {
                strSql1.Append("cStateCode,");
                strSql2.Append("'" + model.cStateCode + "',");
            }
            if (model.Tf_BatchId != null)
            {
                strSql1.Append("Tf_BatchId,");
                strSql2.Append("'" + model.Tf_BatchId + "',");
            }
            if (model.Tf_TrayId != null)
            {
                strSql1.Append("Tf_TrayId,");
                strSql2.Append("'" + model.Tf_TrayId + "',");
            }
            if (model.tf_Location != null)
            {
                strSql1.Append("tf_Location,");
                strSql2.Append("'" + model.tf_Location + "',");
            }
            if (model.tf_CheckGrade != null)
            {
                strSql1.Append("tf_CheckGrade,");
                strSql2.Append("'" + model.tf_CheckGrade + "',");
            }
            if (model.tf_Tag != null)
            {
                strSql1.Append("tf_Tag,");
                strSql2.Append("" + model.tf_Tag + ",");
            }
            if (model.dlOCV != null)
            {
                strSql1.Append("dlOCV,");
                strSql2.Append("'" + model.dlOCV + "',");
            }
            if (model.dlOCVTime != null)
            {
                strSql1.Append("dlOCVTime,");
                strSql2.Append("'" + model.dlOCVTime + "',");
            }
            if (model.Tf_TempE != null)
            {
                strSql1.Append("Tf_TempE,");
                strSql2.Append("'" + model.Tf_TempE + "',");
            }
            if (model.tf_DOcv != null)
            {
                strSql1.Append("tf_DOcv,");
                strSql2.Append("'" + model.tf_DOcv + "',");
            }
            if (model.Kvalue != null)
            {
                strSql1.Append("Kvalue,");
                strSql2.Append("'" + model.Kvalue + "',");
            }
            if (model.tf_Pick != null)
            {
                strSql1.Append("tf_Pick,");
                strSql2.Append("'" + model.tf_Pick + "',");
            }
            if (model.tf_ResultNum != null)
            {
                strSql1.Append("tf_ResultNum,");
                strSql2.Append("'" + model.tf_ResultNum + "',");
            }
            strSql.Append("insert into tb_CheckDataonline(");
            strSql.Append(strSql1.ToString().Remove(strSql1.Length - 1));
            strSql.Append(")");
            strSql.Append(" values (");
            strSql.Append(strSql2.ToString().Remove(strSql2.Length - 1));
            strSql.Append(")");
            strSql.Append(";select @@IDENTITY");
            object obj = DbHelperSQL.GetSingle(strSql.ToString());
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt64(obj);
            }
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(FXJDatabase.tb_CheckDataonlineModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update tb_CheckDataonline set ");
            if (model.ID != null)
            {
                strSql.Append("ID=" + model.ID + ",");
            }
            else
            {
                strSql.Append("ID= null ,");
            }
            if (model.BarCode != null)
            {
                strSql.Append("BarCode='" + model.BarCode + "',");
            }
            else
            {
                strSql.Append("BarCode= null ,");
            }
            if (model.cType != null)
            {
                strSql.Append("cType='" + model.cType + "',");
            }
            else
            {
                strSql.Append("cType= null ,");
            }
            if (model.FileName != null)
            {
                strSql.Append("FileName='" + model.FileName + "',");
            }
            else
            {
                strSql.Append("FileName= null ,");
            }
            if (model.fltCapacity != null)
            {
                strSql.Append("fltCapacity='" + model.fltCapacity + "',");
            }
            else
            {
                strSql.Append("fltCapacity= null ,");
            }
            if (model.fltVolGap != null)
            {
                strSql.Append("fltVolGap=" + model.fltVolGap + ",");
            }
            else
            {
                strSql.Append("fltVolGap= null ,");
            }
            if (model.fltSelfFrequency != null)
            {
                strSql.Append("fltSelfFrequency=" + model.fltSelfFrequency + ",");
            }
            else
            {
                strSql.Append("fltSelfFrequency= null ,");
            }
            if (model.fltVol != null)
            {
                strSql.Append("fltVol=" + model.fltVol + ",");
            }
            else
            {
                strSql.Append("fltVol= null ,");
            }
            if (model.fltResistance != null)
            {
                strSql.Append("fltResistance=" + model.fltResistance + ",");
            }
            else
            {
                strSql.Append("fltResistance= null ,");
            }
            if (model.cGrade != null)
            {
                strSql.Append("cGrade='" + model.cGrade + "',");
            }
            else
            {
                strSql.Append("cGrade= null ,");
            }
            if (model.cState != null)
            {
                strSql.Append("cState=" + model.cState + ",");
            }
            else
            {
                strSql.Append("cState= null ,");
            }
            if (model.cDate != null)
            {
                strSql.Append("cDate='" + model.cDate + "',");
            }
            else
            {
                strSql.Append("cDate= null ,");
            }
            if (model.cStateCode != null)
            {
                strSql.Append("cStateCode='" + model.cStateCode + "',");
            }
            else
            {
                strSql.Append("cStateCode= null ,");
            }
            if (model.Tf_BatchId != null)
            {
                strSql.Append("Tf_BatchId='" + model.Tf_BatchId + "',");
            }
            else
            {
                strSql.Append("Tf_BatchId= null ,");
            }
            if (model.Tf_TrayId != null)
            {
                strSql.Append("Tf_TrayId='" + model.Tf_TrayId + "',");
            }
            else
            {
                strSql.Append("Tf_TrayId= null ,");
            }
            if (model.tf_Location != null)
            {
                strSql.Append("tf_Location='" + model.tf_Location + "',");
            }
            else
            {
                strSql.Append("tf_Location= null ,");
            }
            if (model.tf_CheckGrade != null)
            {
                strSql.Append("tf_CheckGrade='" + model.tf_CheckGrade + "',");
            }
            else
            {
                strSql.Append("tf_CheckGrade= null ,");
            }
            if (model.tf_Tag != null)
            {
                strSql.Append("tf_Tag=" + model.tf_Tag + ",");
            }
            else
            {
                strSql.Append("tf_Tag= null ,");
            }
            if (model.dlOCV != null)
            {
                strSql.Append("dlOCV='" + model.dlOCV + "',");
            }
            else
            {
                strSql.Append("dlOCV= null ,");
            }
            if (model.dlOCVTime != null)
            {
                strSql.Append("dlOCVTime='" + model.dlOCVTime + "',");
            }
            else
            {
                strSql.Append("dlOCVTime= null ,");
            }
            if (model.Tf_TempE != null)
            {
                strSql.Append("Tf_TempE='" + model.Tf_TempE + "',");
            }
            else
            {
                strSql.Append("Tf_TempE= null ,");
            }
            if (model.tf_DOcv != null)
            {
                strSql.Append("tf_DOcv='" + model.tf_DOcv + "',");
            }
            else
            {
                strSql.Append("tf_DOcv= null ,");
            }
            if (model.Kvalue != null)
            {
                strSql.Append("Kvalue='" + model.Kvalue + "',");
            }
            else
            {
                strSql.Append("Kvalue= null ,");
            }
            if (model.tf_Pick != null)
            {
                strSql.Append("tf_Pick='" + model.tf_Pick + "',");
            }
            else
            {
                strSql.Append("tf_Pick= null ,");
            }
            if (model.tf_ResultNum != null)
            {
                strSql.Append("tf_ResultNum='" + model.tf_ResultNum + "',");
            }
            else
            {
                strSql.Append("tf_ResultNum= null ,");
            }
            int n = strSql.ToString().LastIndexOf(",");
            strSql.Remove(n, 1);
            strSql.Append(" where HLId=" + model.HLId + "");
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
        public bool Delete(long HLId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from tb_CheckDataonline ");
            strSql.Append(" where HLId=" + HLId + "");
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
        public bool DeleteList(string HLIdlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from tb_CheckDataonline ");
            strSql.Append(" where HLId in (" + HLIdlist + ")  ");
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
        public FXJDatabase.tb_CheckDataonlineModel GetModel(long HLId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1  ");
            strSql.Append(" ID,BarCode,cType,FileName,fltCapacity,fltVolGap,fltSelfFrequency,fltVol,fltResistance,cGrade,cState,cDate,cStateCode,Tf_BatchId,Tf_TrayId,tf_Location,tf_CheckGrade,tf_Tag,dlOCV,dlOCVTime,Tf_TempE,tf_DOcv,Kvalue,tf_Pick,tf_ResultNum,HLId ");
            strSql.Append(" from tb_CheckDataonline ");
            strSql.Append(" where HLId=" + HLId + "");
            FXJDatabase.tb_CheckDataonlineModel model = new FXJDatabase.tb_CheckDataonlineModel();
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
        public FXJDatabase.tb_CheckDataonlineModel DataRowToModel(DataRow row)
        {
            FXJDatabase.tb_CheckDataonlineModel model = new FXJDatabase.tb_CheckDataonlineModel();
            if (row != null)
            {
                if (row["ID"] != null && row["ID"].ToString() != "")
                {
                    model.ID = long.Parse(row["ID"].ToString());
                }
                if (row["BarCode"] != null)
                {
                    model.BarCode = row["BarCode"].ToString();
                }
                if (row["cType"] != null)
                {
                    model.cType = row["cType"].ToString();
                }
                if (row["FileName"] != null)
                {
                    model.FileName = row["FileName"].ToString();
                }
                if (row["fltCapacity"] != null)
                {
                    model.fltCapacity = row["fltCapacity"].ToString();
                }
                if (row["fltVolGap"] != null && row["fltVolGap"].ToString() != "")
                {
                    model.fltVolGap = decimal.Parse(row["fltVolGap"].ToString());
                }
                if (row["fltSelfFrequency"] != null && row["fltSelfFrequency"].ToString() != "")
                {
                    model.fltSelfFrequency = decimal.Parse(row["fltSelfFrequency"].ToString());
                }
                if (row["fltVol"] != null && row["fltVol"].ToString() != "")
                {
                    model.fltVol = decimal.Parse(row["fltVol"].ToString());
                }
                if (row["fltResistance"] != null && row["fltResistance"].ToString() != "")
                {
                    model.fltResistance = decimal.Parse(row["fltResistance"].ToString());
                }
                if (row["cGrade"] != null)
                {
                    model.cGrade = row["cGrade"].ToString();
                }
                if (row["cState"] != null && row["cState"].ToString() != "")
                {
                    model.cState = decimal.Parse(row["cState"].ToString());
                }
                if (row["cDate"] != null)
                {
                    model.cDate = row["cDate"].ToString();
                }
                if (row["cStateCode"] != null)
                {
                    model.cStateCode = row["cStateCode"].ToString();
                }
                if (row["Tf_BatchId"] != null)
                {
                    model.Tf_BatchId = row["Tf_BatchId"].ToString();
                }
                if (row["Tf_TrayId"] != null)
                {
                    model.Tf_TrayId = row["Tf_TrayId"].ToString();
                }
                if (row["tf_Location"] != null)
                {
                    model.tf_Location = row["tf_Location"].ToString();
                }
                if (row["tf_CheckGrade"] != null)
                {
                    model.tf_CheckGrade = row["tf_CheckGrade"].ToString();
                }
                if (row["tf_Tag"] != null && row["tf_Tag"].ToString() != "")
                {
                    model.tf_Tag = int.Parse(row["tf_Tag"].ToString());
                }
                if (row["dlOCV"] != null)
                {
                    model.dlOCV = row["dlOCV"].ToString();
                }
                if (row["dlOCVTime"] != null)
                {
                    model.dlOCVTime = row["dlOCVTime"].ToString();
                }
                if (row["Tf_TempE"] != null)
                {
                    model.Tf_TempE = row["Tf_TempE"].ToString();
                }
                if (row["tf_DOcv"] != null)
                {
                    model.tf_DOcv = row["tf_DOcv"].ToString();
                }
                if (row["Kvalue"] != null)
                {
                    model.Kvalue = row["Kvalue"].ToString();
                }
                if (row["tf_Pick"] != null)
                {
                    model.tf_Pick = row["tf_Pick"].ToString();
                }
                if (row["tf_ResultNum"] != null)
                {
                    model.tf_ResultNum = row["tf_ResultNum"].ToString();
                }
                if (row["HLId"] != null && row["HLId"].ToString() != "")
                {
                    model.HLId = long.Parse(row["HLId"].ToString());
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
            strSql.Append("select ID,BarCode,cType,FileName,fltCapacity,fltVolGap,fltSelfFrequency,fltVol,fltResistance,cGrade,cState,cDate,cStateCode,Tf_BatchId,Tf_TrayId,tf_Location,tf_CheckGrade,tf_Tag,dlOCV,dlOCVTime,Tf_TempE,tf_DOcv,Kvalue,tf_Pick,tf_ResultNum,HLId ");
            strSql.Append(" FROM tb_CheckDataonline ");
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
            strSql.Append(" ID,BarCode,cType,FileName,fltCapacity,fltVolGap,fltSelfFrequency,fltVol,fltResistance,cGrade,cState,cDate,cStateCode,Tf_BatchId,Tf_TrayId,tf_Location,tf_CheckGrade,tf_Tag,dlOCV,dlOCVTime,Tf_TempE,tf_DOcv,Kvalue,tf_Pick,tf_ResultNum,HLId ");
            strSql.Append(" FROM tb_CheckDataonline ");
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
            strSql.Append("select count(1) FROM tb_CheckDataonline ");
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
                strSql.Append("order by T.HLId desc");
            }
            strSql.Append(")AS Row, T.*  from tb_CheckDataonline T ");
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
        public DataSet GetDataToUpload(int count)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (count > 0)
            {
                strSql.Append(" top " + count.ToString());
            }
            strSql.Append(" ID,BarCode,cType,FileName,fltCapacity,fltVolGap,fltSelfFrequency,fltVol,fltResistance,cGrade,cState,cDate,cStateCode,Tf_BatchId,Tf_TrayId,tf_Location,tf_CheckGrade,tf_Tag,dlOCV,dlOCVTime,Tf_TempE,tf_DOcv,Kvalue,tf_Pick,tf_ResultNum,HLId ");
            strSql.Append(" FROM tb_CheckDataonline ");

            strSql.Append(" where  tf_Tag=0");
            
            strSql.Append(" order by cDate asc" );
            return DbHelperSQL.Query(strSql.ToString());
        }
        #endregion  MethodEx
    }
}

