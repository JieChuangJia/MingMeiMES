using System;
using System.Data;
using System.Collections.Generic;
using FTDataAccess.DAL;
using FTDataAccess.Model;
namespace FTDataAccess.BLL
{
    /// <summary>
    /// 用户登录信息
    /// </summary>
    public partial class User_ListBll
    {
        private readonly FTDataAccess.DAL.User_ListDal dal = new FTDataAccess.DAL.User_ListDal();
        public User_ListBll()
        { }
        #region  BasicMethod
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string UserName)
        {
            return dal.Exists(UserName);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(FTDataAccess.Model.User_ListModel model)
        {
            return dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(FTDataAccess.Model.User_ListModel model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string UserName)
        {

            return dal.Delete(UserName);
        }
        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool DeleteList(string UserNamelist)
        {
            return dal.DeleteList(UserNamelist);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public FTDataAccess.Model.User_ListModel GetModel(string UserName)
        {

            return dal.GetModel(UserName);
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
        public List<FTDataAccess.Model.User_ListModel> GetModelList(string strWhere)
        {
            DataSet ds = dal.GetList(strWhere);
            return DataTableToList(ds.Tables[0]);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<FTDataAccess.Model.User_ListModel> DataTableToList(DataTable dt)
        {
            List<FTDataAccess.Model.User_ListModel> modelList = new List<FTDataAccess.Model.User_ListModel>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                FTDataAccess.Model.User_ListModel model;
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
        public bool IsUserRegister(int roleID, string userName, string password)
        {
            // password = EncAndDec.Encode(password,"zwx","xwz");
            return dal.IsUserRegister(roleID, userName, password);
        }
        #endregion  ExtensionMethod
    }
}

