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
        public bool Exists(string 测试时间, string 二维码, string 电阻值, string 放电电流1, string 放电电流2, string 放电时间, string 静置时间, string 结果电压, string 结果电流)
        {
            return dal.Exists(测试时间, 二维码, 电阻值, 放电电流1, 放电电流2, 放电时间, 静置时间, 结果电压, 结果电流);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(DCIRDBAccess.dcirMode model)
        {
            return dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(DCIRDBAccess.dcirMode model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string 测试时间, string 二维码, string 电阻值, string 放电电流1, string 放电电流2, string 放电时间, string 静置时间, string 结果电压, string 结果电流)
        {

            return dal.Delete(测试时间, 二维码, 电阻值, 放电电流1, 放电电流2, 放电时间, 静置时间, 结果电压, 结果电流);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public DCIRDBAccess.dcirMode GetModel(string 测试时间, string 二维码, string 电阻值, string 放电电流1, string 放电电流2, string 放电时间, string 静置时间, string 结果电压, string 结果电流)
        {

            return dal.GetModel(测试时间, 二维码, 电阻值, 放电电流1, 放电电流2, 放电时间, 静置时间, 结果电压, 结果电流);
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
        public List<DCIRDBAccess.dcirMode> GetModelList(string strWhere)
        {
            DataSet ds = dal.GetList(strWhere);
            return DataTableToList(ds.Tables[0]);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<DCIRDBAccess.dcirMode> DataTableToList(DataTable dt)
        {
            List<DCIRDBAccess.dcirMode> modelList = new List<DCIRDBAccess.dcirMode>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                DCIRDBAccess.dcirMode model;
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

        /// <summary>
        /// 通过二维码查询dcir数据
        /// </summary>
        /// <param name="moudCode"></param>
        /// <returns></returns>
        public dcirMode GetModelByMoudCode(string moudCode)
        {
            string sqlStr = "二维码 ='" + moudCode + "'";
            List<dcirMode> dicrList = GetModelList(sqlStr);
            if(dicrList!= null&& dicrList.Count>0)
            {
                return dicrList[0];
            }
            else
            {
                return null;
            }
        }
        #endregion  ExtensionMethod
    }
}
