using System;
using System.Data;
using System.Collections.Generic;
 
namespace FXJDatabase
{
    /// <summary>
    /// tb_CheckDataonline
    /// </summary>
    public partial class tb_CheckDataonlineBLL
    {
        private readonly FXJDatabase.tb_CheckDataonlineDAL dal = new FXJDatabase.tb_CheckDataonlineDAL();
        public tb_CheckDataonlineBLL()
        { }
        #region  BasicMethod

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public long Add(FXJDatabase.tb_CheckDataonlineModel model)
        {
            return dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(FXJDatabase.tb_CheckDataonlineModel model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(long HLId)
        {

            return dal.Delete(HLId);
        }
        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool DeleteList(string HLIdlist)
        {
            return dal.DeleteList(HLIdlist);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public FXJDatabase.tb_CheckDataonlineModel GetModel(long HLId)
        {

            return dal.GetModel(HLId);
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
        public List<FXJDatabase.tb_CheckDataonlineModel> GetModelList(string strWhere)
        {
            DataSet ds = dal.GetList(strWhere);
            return DataTableToList(ds.Tables[0]);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<FXJDatabase.tb_CheckDataonlineModel> DataTableToList(DataTable dt)
        {
            List<FXJDatabase.tb_CheckDataonlineModel> modelList = new List<FXJDatabase.tb_CheckDataonlineModel>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                FXJDatabase.tb_CheckDataonlineModel model;
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
        public List<tb_CheckDataonlineModel> GetDataToUpload(int count)
        {
            DataSet ds = dal.GetDataToUpload(count);
            if(ds!=null&&ds.Tables.Count>0)
            {
                List<tb_CheckDataonlineModel> dataList = DataTableToList(ds.Tables[0]);

                return dataList;
            }
            else
            {
                return null;
            }

        }
        #endregion  ExtensionMethod
    }
}

