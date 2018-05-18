using System;
using System.Data;
using System.Collections.Generic;
 
using DBAccess.Model;
namespace DBAccess.BLL
{
    /// <summary>
    /// QRCodeModel
    /// </summary>
    public partial class QRCodeBLL
    {
        private readonly DBAccess.DAL.QRCodeDAL dal = new DBAccess.DAL.QRCodeDAL();
        public QRCodeBLL()
        { }
        #region  BasicMethod
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string QRCode)
        {
            return dal.Exists(QRCode);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(DBAccess.Model.QRCodeModel model)
        {
            return dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(DBAccess.Model.QRCodeModel model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string QRCode)
        {

            return dal.Delete(QRCode);
        }
        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool DeleteList(string QRCodelist)
        {
            return dal.DeleteList(QRCodelist);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public DBAccess.Model.QRCodeModel GetModel(string QRCode)
        {

            return dal.GetModel(QRCode);
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
        public List<DBAccess.Model.QRCodeModel> GetModelList(string strWhere)
        {
            DataSet ds = dal.GetList(strWhere);
            return DataTableToList(ds.Tables[0]);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<DBAccess.Model.QRCodeModel> DataTableToList(DataTable dt)
        {
            List<DBAccess.Model.QRCodeModel> modelList = new List<DBAccess.Model.QRCodeModel>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                DBAccess.Model.QRCodeModel model;
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
        public QRCodeModel RequireQrCode(string qrCodeType)
        {
            string sqlStr = "PintStatus ='待打印' and QRType ='" + qrCodeType+"'";
            List<QRCodeModel> qrCodeList = GetModelList(sqlStr);
            if(qrCodeList!= null && qrCodeList.Count>0)
            {
                return qrCodeList[0];
            }
            else
            {
                return null;
            }

        }
        public DataTable GetQrCodeData(string qrType,string qrStatus)
        {

            string sqlStr = "QRType='" + qrType + "' and PintStatus='" + qrStatus + "'";
            DataSet ds = GetList(sqlStr);
            if (ds != null && ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            else
            {
                return null;
            }
        }
        #endregion  ExtensionMethod
    }
}

