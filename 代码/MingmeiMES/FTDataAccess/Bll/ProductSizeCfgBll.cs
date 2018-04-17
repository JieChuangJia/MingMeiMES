using System;
using System.Data;
using System.Collections.Generic;
using FTDataAccess.Model;
namespace FTDataAccess.BLL
{
    /// <summary>
    /// ProductSizeCfgModel
    /// </summary>
    public partial class ProductSizeCfgBll
    {
        private readonly FTDataAccess.DAL.ProductSizeCfgDal dal = new FTDataAccess.DAL.ProductSizeCfgDal();
        public ProductSizeCfgBll()
        { }
        #region  BasicMethod
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string productCataCode)
        {
            return dal.Exists(productCataCode);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(FTDataAccess.Model.ProductSizeCfgModel model)
        {
            return dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(FTDataAccess.Model.ProductSizeCfgModel model)
        {
            return dal.Update(model);
        }

       
        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string productCataCode)
        {

            return dal.Delete(productCataCode);
        }
        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool DeleteList(string cataSeqlist)
        {
            return dal.DeleteList(cataSeqlist);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        //public FTDataAccess.Model.ProductSizeCfgModel GetModel(int cataSeq)
        //{

        //    return dal.GetModel(cataSeq);
        //}
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public FTDataAccess.Model.ProductSizeCfgModel GetModel(string productCataCode)
        {

            return dal.GetModel(productCataCode);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            return dal.GetList(strWhere);
        }
        
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<FTDataAccess.Model.ProductSizeCfgModel> GetModelList(string strWhere)
        {
            DataSet ds = dal.GetList(strWhere);
            return DataTableToList(ds.Tables[0]);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<FTDataAccess.Model.ProductSizeCfgModel> DataTableToList(DataTable dt)
        {
            List<FTDataAccess.Model.ProductSizeCfgModel> modelList = new List<FTDataAccess.Model.ProductSizeCfgModel>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                FTDataAccess.Model.ProductSizeCfgModel model;
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

        #endregion  ExtensionMethod
    }
}

