using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using DevInterface;

namespace DevAccess
{
    public class OcvAccess:IOcvAccess
    {
        public string ocvDBConn = "";
        private int channelSum = 36; //最大通道数量
        
        public OcvAccess(string connStr,int channelSum)
        {
            this.ocvDBConn = connStr;
            this.channelSum = channelSum;
        }
        public DataTable GetOcvTestDt(string palletID,ref string reStr)
        {
            string SQLString = string.Format("select ContainerCode,SerialNumber,BatchNo,ChannelNo,Tag,Status,Pick,Type,Grade,LoadType from [JG_HZKK].[V_JXS] where ContainerCode='{0}'",palletID);
           // if(testSeqList != null && testSeqList.Count()>0)
            //{
            //    SQLString += " and ";
            //    for(int i=0;i<testSeqList.Count();i++)
            //    {
            //        if(i==0)
            //        {
            //            SQLString += string.Format(" (tag={0}", testSeqList[i]);
            //        }
            //        else
            //        {
            //            SQLString += string.Format(" or tag={0}", testSeqList[i]);
            //        }
            //        if (i == testSeqList.Count() - 1)
            //        {
            //            SQLString += ")";
            //        }
                  
            //    }
            //}
            SQLString += " order by ChannelNo";
            using (SqlConnection connection = new SqlConnection(ocvDBConn))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SqlDataAdapter command = new SqlDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                    if(ds.Tables[0].Rows.Count<1)
                    {
                        return null;
                    }

                    return ds.Tables[0] ;
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    // Console.WriteLine(connectionString);
                    reStr = ex.Message;
                    return null;

                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="palletID"></param>
        /// <param name="testSeqIDS"></param>
        /// <param name="vals"> 1:合格，2：NG，3：空</param>
        /// <param name="reStr"></param>
        /// <returns></returns>
        public bool GetCheckResult(string palletID, List<int> testSeqIDS, ref List<int> vals, ref string reStr)
        {
            vals = new List<int>();
            //for (int i = 0; i < 36; i++)
            //{
            //    vals.Add("1");
            //}
            DataTable dt = GetOcvTestDt(palletID,ref reStr);
            if(dt == null||dt.Rows.Count<1)
            {
                reStr = "没有查询到对应的OCV测试数据," + reStr;
                return false;
            }
            for (int i = 0; i < channelSum;i++ )
            {
                int checkRe = 3;//默认为空
                int channelID=i+1;
                foreach(DataRow dr in dt.Rows)
                {
                    if (int.Parse(dr["ChannelNo"].ToString()) == channelID)
                    {
                        if (int.Parse(dr["Status"].ToString()) == 1 && testSeqIDS.Contains(int.Parse(dr["tag"].ToString())))
                        {
                            checkRe = 2;
                            break;
                        }
                        else if (int.Parse(dr["Status"].ToString()) == 0)
                        {
                            checkRe = 1;
                        }
                        else
                        {
                            checkRe = 3;
                        }
                    }
                    
                }
                vals.Add(checkRe);
            }

            return true;
        }
        
        /// <summary>
        /// 获得托盘OCV最新步号
        /// </summary>
        /// <param name="palletID"></param>
        /// <param name="reStr"></param>
        /// <returns>-1：无记录，1:OCV1,2:OCV2,3:分容,4:OCV3,5:OCV4</returns>
        public int GetOcvStepSeq(string palletID,ref string reStr)
        {
            int ocvSeq = 0;
            DataTable dt = GetOcvTestDt(palletID, ref reStr);
            if (dt == null || dt.Rows.Count < 1)
            {
                reStr = "没有查询到对应的OCV测试数据," + reStr;
                return -1;
            }
            foreach (DataRow dr in dt.Rows)
            {
                int seq = 0;
                if(int.TryParse(dr["Type"].ToString(),out seq))
                {
                    if(seq>ocvSeq)
                    {
                        ocvSeq = seq;
                    }
                }
            }
           
            return ocvSeq;
        }
    }
}
