using System;
using System.Data;
using System.Collections.Generic;
using FTDataAccess.Model;
namespace FTDataAccess.BLL
{
    /// <summary>
    /// LOCAL_MES_STEP_INFO_DETAILModel
    /// </summary>
    public partial class LOCAL_MES_STEP_INFO_DETAILBll
    {
        private readonly FTDataAccess.DAL.LOCAL_MES_STEP_INFO_DETAILDal dal = new FTDataAccess.DAL.LOCAL_MES_STEP_INFO_DETAILDal();
        public LOCAL_MES_STEP_INFO_DETAILBll()
        { }
        #region  BasicMethod
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string RECID)
        {
            return dal.Exists(RECID);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(FTDataAccess.Model.LOCAL_MES_STEP_INFO_DETAILModel model)
        {
            return dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(FTDataAccess.Model.LOCAL_MES_STEP_INFO_DETAILModel model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string RECID)
        {

            return dal.Delete(RECID);
        }
        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool DeleteList(string RECIDlist)
        {
            return dal.DeleteList(RECIDlist);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public FTDataAccess.Model.LOCAL_MES_STEP_INFO_DETAILModel GetModel(string RECID)
        {

            return dal.GetModel(RECID);
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
        public List<FTDataAccess.Model.LOCAL_MES_STEP_INFO_DETAILModel> GetModelList(string strWhere)
        {
            DataSet ds = dal.GetList(strWhere);
            return DataTableToList(ds.Tables[0]);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<FTDataAccess.Model.LOCAL_MES_STEP_INFO_DETAILModel> DataTableToList(DataTable dt)
        {
            List<FTDataAccess.Model.LOCAL_MES_STEP_INFO_DETAILModel> modelList = new List<FTDataAccess.Model.LOCAL_MES_STEP_INFO_DETAILModel>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                FTDataAccess.Model.LOCAL_MES_STEP_INFO_DETAILModel model;
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
        public bool ExistByCondition(string strWhere)
        {
            List<LOCAL_MES_STEP_INFO_DETAILModel> preuploadModels = GetModelList(strWhere);
            if (preuploadModels != null && preuploadModels.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool DelByCondition(string strWhere)
        {
            List<LOCAL_MES_STEP_INFO_DETAILModel> ms = GetModelList(strWhere);
             if (ms != null && ms.Count > 0)
             {
                 foreach(LOCAL_MES_STEP_INFO_DETAILModel m in ms)
                 {
                     if(!dal.Delete(m.RECID))
                     {
                         return false;
                     }
                 }
                 return true;
             }
             else
             {
                 return true;
             }
             
        }
        public void ClearHistorydata()
        {
            System.TimeSpan ts = new TimeSpan(7, 0, 0, 0); //7天
            System.DateTime delDate = System.DateTime.Now - ts;
            string strWhere = string.Format("delete from LOCAL_MES_STEP_INFO_DETAIL where TRX_TIME<'{0}' and UPLOAD_FLAG=1 ", delDate.ToString("yyyy-MM-dd"));
            FTDataAccess.DBUtility.DbHelperSQL.ExecuteSql(strWhere);
        }
        #endregion  ExtensionMethod
    }
}

