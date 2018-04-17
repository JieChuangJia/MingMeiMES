using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCIRDBAccess
{
    /// <summary>
    /// dbBll
    /// </summary>
    public partial class dbDCIRBll
    {
        private readonly dbDCIRDal dal = new dbDCIRDal();
        public dbDCIRBll()
        { }
        #region  BasicMethod
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string 测试时间, string 电流, string 电压, string 容量, string 能量, string 总时间, string 相对时间)
        {
            return dal.Exists(测试时间, 电流, 电压, 容量, 能量, 总时间, 相对时间);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(dcirMode model)
        {
            return dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(dcirMode model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string 测试时间, string 电流, string 电压, string 容量, string 能量, string 总时间, string 相对时间)
        {

            return dal.Delete(测试时间, 电流, 电压, 容量, 能量, 总时间, 相对时间);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public dcirMode GetModel(string 测试时间, string 电流, string 电压, string 容量, string 能量, string 总时间, string 相对时间)
        {

            return dal.GetModel(测试时间, 电流, 电压, 容量, 能量, 总时间, 相对时间);
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
        public List<dcirMode> GetModelList(string strWhere)
        {
            DataSet ds = dal.GetList(strWhere);
            return DataTableToList(ds.Tables[0]);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<dcirMode> DataTableToList(DataTable dt)
        {
            List<dcirMode> modelList = new List<dcirMode>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                dcirMode model;
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
        public List<dcirMode> GetModelListByTestTimeASC()
        {
            DataSet ds = dal.GetListByTime();
            return DataTableToList(ds.Tables[0]);
        }
        #endregion  ExtensionMethod
    }
}
