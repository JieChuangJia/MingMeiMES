using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MTDBAccess.BLL
{

    /// <summary>
    /// db
    /// </summary>
    public partial class dbBLL
    {
        private readonly MTDBAccess.DAL.dbDAL dal = new MTDBAccess.DAL.dbDAL();
        public dbBLL()
        { }
        #region  BasicMethod
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string 测试时间, string 二维码, string 正螺丝数据, string 反螺丝数据)
        {
            return dal.Exists(测试时间, 二维码, 正螺丝数据, 反螺丝数据);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(MTDBAccess.Model.dbModel model)
        {
            return dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(MTDBAccess.Model.dbModel model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string 测试时间, string 二维码, string 正螺丝数据, string 反螺丝数据)
        {

            return dal.Delete(测试时间, 二维码, 正螺丝数据, 反螺丝数据);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public MTDBAccess.Model.dbModel GetModel(string 测试时间, string 二维码, string 正螺丝数据, string 反螺丝数据)
        {

            return dal.GetModel(测试时间, 二维码, 正螺丝数据, 反螺丝数据);
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
        public List<MTDBAccess.Model.dbModel> GetModelList(string strWhere)
        {
            DataSet ds = dal.GetList(strWhere);
            return DataTableToList(ds.Tables[0]);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<MTDBAccess.Model.dbModel> DataTableToList(DataTable dt)
        {
            List<MTDBAccess.Model.dbModel> modelList = new List<MTDBAccess.Model.dbModel>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                MTDBAccess.Model.dbModel model;
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
        public MTDBAccess.Model.dbModel GetModel(string code)
        {
            DataSet ds = dal.GetOneData("二维码='" + code + "'");
           
            List<MTDBAccess.Model.dbModel> dbList =DataTableToList(ds.Tables[0]);
            if(dbList!=null&&dbList.Count>0)
            {
                return dbList[0];
            }
            else
            {
                return null;
            }
        }
        #endregion  ExtensionMethod
    }



}
