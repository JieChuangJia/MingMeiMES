using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;  
using System.Data.SqlClient;
using LogInterface;
using FTDataAccess.Model;
using FTDataAccess.BLL;
namespace ConfigManage
{
    public class ProductDataPresenter
    {
        private ProductHeightDefBll heightDefBll = null;
        private ProductPacsizeDefBll packsizeDefBll = null;
        private ProductSizeCfgBll productCfgBll = null;
        private GasConfigBll gasCfgBll = null;
        private IProductDataSheetView view = null;
        protected ILogRecorder logRecorder = null;
        #region 公共接口
        public ProductDataPresenter(IProductDataSheetView view)
        {
            this.view = view;
            heightDefBll = new ProductHeightDefBll();
            packsizeDefBll = new ProductPacsizeDefBll();
            productCfgBll = new ProductSizeCfgBll();
            gasCfgBll = new GasConfigBll();
        }
        public void SetLogRecorder(ILogRecorder logRecorder)
        {
            this.logRecorder = logRecorder;
        }
        public void RefreshList(string dataCata)
        {
            try
            {
                switch (dataCata)
                {
                    case "产品高度字典":
                        {
                            DataTable dt = heightDefBll.GetAllList().Tables[0];
                            view.DispHeightDeflist(dt);
                            break;
                        }
                    case "产品包装尺寸字典":
                        {
                            DataTable dt = packsizeDefBll.GetAllList().Tables[0];
                            view.DispPacksizeDeflist(dt);
                            break;
                        }
                    case "产品型号配置":
                        {
                            DataTable dt = productCfgBll.GetAllList().Tables[0];
                            view.DispProductCfgList(dt);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                view.ShowPopupMes(ex.ToString());
            }
           
        }
        public void BeginAddProductCfg()
        {
            List < ProductHeightDefModel> heights = heightDefBll.GetModelList("");
            if(heights != null && heights.Count>0)
            {
                List<string> strHeights = new List<string>();
                foreach(ProductHeightDefModel m in heights)
                {
                    strHeights.Add(m.productHeight.ToString());
                }
                view.RefreshHeightList(strHeights);
            }
            List<ProductPacsizeDefModel> packs = packsizeDefBll.GetModelList("");
            if(packs != null && packs.Count>0)
            {
                List<string> strPacks = new List<string>();
                foreach(ProductPacsizeDefModel m in packs)
                {
                    strPacks.Add(m.packageSize);
                }
                view.RefreshPacksizeList(strPacks);
            }
            List<GasConfigModel> gasList = gasCfgBll.GetModelList("");
            if(gasList != null && gasList.Count>0)
            {
                List<string> strGas = new List<string>();
                foreach(GasConfigModel m in gasList)
                {
                    strGas.Add(m.gasName);

                }
                view.RefreshGasList(strGas);
            }
            
        }
        public void AddProductCfg(ProductSizeCfgModel m)
        {
            if(productCfgBll.Exists(m.productCataCode))
            {
                view.ShowPopupMes("已经存在" + m.productCataCode);
                return;
            }
            if(!productCfgBll.Add(m))
            {
                LogModel log = new LogModel("产品型号配置","添加型号配置失败",EnumLoglevel.错误);

                logRecorder.AddLog(log);
            }
            else
            {
                DataTable dt = productCfgBll.GetAllList().Tables[0];
                view.DispProductCfgList(dt);
            }
        }
        public void ModifyProductCfg(ProductSizeCfgModel m)
        {
            if(!productCfgBll.Exists(m.productCataCode))
            {
                view.ShowPopupMes(string.Format("{0} 的尺寸配置不存在，请添加", m.productCataCode));
            }
            else
            {
                productCfgBll.Update(m);
                RefreshList("产品型号配置");
            }
        }
        public void DelProductCfg(List<string> productCataCodes)
        {
            if(view.AskMessge("确定要删除吗?")<1)
            {
                return;
            }
            foreach(string cataCode in productCataCodes)
            {
                if (productCfgBll.Exists(cataCode))
                {
                    productCfgBll.Delete(cataCode);
                   
                }
            }
            RefreshList("产品型号配置");
        }
        public void ImportProductCfgData(string excelFile)
        {
            string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                "Data Source=" + excelFile + ";" +
                "Extended Properties='Excel 12.0; HDR=Yes; IMEX=0'";
            OleDbDataAdapter da = new OleDbDataAdapter("SELECT *  FROM [烟机线型号数据$]", strConn);  
           DataSet ds = new DataSet();  
           try 
           {  
               da.Fill(ds);
               DataTable dt = ds.Tables[0];  
               //this.dataGridView1.DataSource = dt;  
               //DataTable targetDt = new DataTable();
               //targetDt.Columns.AddRange(new DataColumn[]{new DataColumn("编号"),new DataColumn("物料号"),new DataColumn("型号名称"),new DataColumn("产品高度"),new DataColumn("包装尺寸"),new DataColumn("备注")});
               int count = 0;
               int existCount = 0;
                string info="";
               foreach(DataRow dr in dt.Rows)
               {
                   //string barcode = dr.ItemArray[1].ToString();
                   if (string.IsNullOrWhiteSpace(dr["物料码"].ToString().Trim()))
                   {
                       continue;
                   }
                   ProductSizeCfgModel m = new ProductSizeCfgModel();
                   m.productCataCode = dr["物料码"].ToString().Trim();
                   m.productName = dr["型号名称"].ToString().Trim();
                   m.cataSeq = int.Parse(dr["物料编号"].ToString());
                   m.packageSize = dr["包装尺寸"].ToString().Trim();// + "." + dr.ItemArray[4].ToString()
                 //  m.productHeight = (int)(float.Parse(dr["产品高度"].ToString()));
                   m.productHeight = 80;
                   m.robotProg = int.Parse(dr["机器人配方编号"].ToString());
                   //m.volt1 = int.Parse(dr["强档电压"].ToString());
                   //m.volt2 = int.Parse(dr["弱档电压"].ToString());
                   //m.frequency1 = int.Parse(dr["频率"].ToString());
                   m.power1 = dr["合格功率"].ToString();
                  
                   if(productCfgBll.Exists(m.productCataCode))
                   {
                       productCfgBll.Update(m);
                       existCount++;
                   }
                   else
                   {
                       if (productCfgBll.Add(m))
                       {
                           count++;
                          
                       }
                       else
                       {
                           info = string.Format("总共导入记录条数：{0},遇到错误发生，物料码{1}数据有错误", count, m.productCataCode);
                           view.ShowPopupMes(info);
                           RefreshList("产品型号配置");
                           return;
                       }
                   }
                   
               }
               info = string.Format("新增:{0}条数据,更新:{1}条数据", count, existCount);
               view.ShowPopupMes(info);
               RefreshList("产品型号配置");
              

           }  
           catch (Exception err)  
           {  
               view.ShowPopupMes("操作失败！" + err.ToString());  
           }  

        }
        #endregion
      
    }
}
