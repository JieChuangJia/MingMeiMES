using System;
using System.Data;
using System.Collections.Generic;
 
using FTDataAccess.Model;
namespace FTDataAccess.BLL
{
    /// <summary>
    /// OfflineData
    /// </summary>
    public partial class OfflineDataBLL
    {
        private readonly FTDataAccess.DAL.OfflineDataDAL dal = new FTDataAccess.DAL.OfflineDataDAL();
        public OfflineDataBLL()
        { }
        #region  BasicMethod
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string OfflineDataID)
        {
            return dal.Exists(OfflineDataID);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(FTDataAccess.Model.OfflineDataModel model)
        {
            return dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(FTDataAccess.Model.OfflineDataModel model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string OfflineDataID)
        {

            return dal.Delete(OfflineDataID);
        }
        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool DeleteList(string OfflineDataIDlist)
        {
            return dal.DeleteList(OfflineDataIDlist);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public FTDataAccess.Model.OfflineDataModel GetModel(string OfflineDataID)
        {

            return dal.GetModel(OfflineDataID);
        }

        

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            return dal.GetList(strWhere);
        }
        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataSet GetList(int Top, string strWhere, string filedOrder)
        {
            return dal.GetList(Top, strWhere, filedOrder);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<FTDataAccess.Model.OfflineDataModel> GetModelList(string strWhere)
        {
            DataSet ds = dal.GetList(strWhere);
            return DataTableToList(ds.Tables[0]);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<FTDataAccess.Model.OfflineDataModel> DataTableToList(DataTable dt)
        {
            List<FTDataAccess.Model.OfflineDataModel> modelList = new List<FTDataAccess.Model.OfflineDataModel>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                FTDataAccess.Model.OfflineDataModel model;
                for (int n = 0; n < rowsCount; n++)
                {
                    model = dal.DataRowToModel(dt.Rows[n]);
                    if (model != null)
                    {
                        modelList.Add(model);
                    }
                }
            }
            return modelList;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetAllList()
        {
            return GetList("");
        }

        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public int GetRecordCount(string strWhere)
        {
            return dal.GetRecordCount(strWhere);
        }
        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public DataSet GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
        {
            return dal.GetListByPage(strWhere, orderby, startIndex, endIndex);
        }
        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        //public DataSet GetList(int PageSize,int PageIndex,string strWhere)
        //{
        //return dal.GetList(PageSize,PageIndex,strWhere);
        //}

        #endregion  BasicMethod
        #region  ExtensionMethod
        public List<OfflineDataModel> GetDataByUploadStatus(string isUpload)
        {
            string sqlStr = "IsUpload ='" + isUpload + "'";
            return GetModelList(sqlStr);
        }
        public OfflineDataModel GetLastDataByWorkStationID(string workStaionID)
        {
            string sqlStr = "WorkStationID = '" +workStaionID +"' order by CreateTime Desc";
            List<OfflineDataModel> lastDataList = GetModelList(sqlStr);
            if(lastDataList!= null&&lastDataList.Count>0)
            {
                return lastDataList[0];
            }
            else
            {
                return null;
            }
        }
        public bool UpdateRefuseUpload(string isUpload,string status)
        {
            string sqlStr = "IsUpload ='" + isUpload + "'";
             List<OfflineDataModel> offlist= GetModelList(sqlStr);
            if(offlist == null)
            {
                return false;
            }
            foreach(OfflineDataModel off in offlist)
            {
                off.IsUpLoad = status;
                Update(off);
            }
            return true;
        }
        public void UpdateDataByStatus(string oldStatus,string newStatus)
        {
            string sqlStr = "IsUpload ='" +oldStatus + "'";
            List<OfflineDataModel> offList = GetModelList(sqlStr);
            if(offList!=null&&offList.Count>0)
            {
              foreach(OfflineDataModel off in offList)
              {
                  off.IsUpLoad = newStatus;
                  Update(off);
              }
            }
        }

        public DataTable GetOfflineDataByStatus(DateTime st,DateTime ed,string uploadStatus,string workStation)
        {
            string stStr = st.ToString("yyyy-MM-dd 0:00:00");
            string edStr = ed.ToString("yyyy-MM-dd 23:59:59");
               string sqlStr = "CreateTime>='" + stStr + "' and CreateTime<='" + edStr + "' and  IsUpLoad ='" + uploadStatus + "'";
            if(workStation != "所有")
            {
                sqlStr += " and WorkStationID='" +workStation+"'";
            }
           
            DataSet ds = GetList(sqlStr);
            if(ds!=null && ds.Tables.Count>0)
            {
                return ds.Tables[0];
            }
            else
            {
                return null;
            }
        }

        public List<string> GetDistinctWorkStation()
        {
            List<string> workStation = new List<string>();
            DataSet ds = dal.GetDistinctWorkStation();
            if(ds!= null&&ds.Tables.Count>0)
            {
                for(int i=0;i<ds.Tables[0].Rows.Count;i++)
                {
                       workStation.Add(ds.Tables[0].Rows[i][0].ToString());
                }
             
            }
            return workStation;
        }
        #endregion  ExtensionMethod
    }
}

