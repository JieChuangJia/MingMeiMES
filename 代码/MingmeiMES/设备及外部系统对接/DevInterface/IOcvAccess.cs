using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevInterface
{
    /// <summary>
    /// OCV测试结果数据访问
    /// </summary>
    public interface IOcvAccess
    {
        /// <summary>
        /// 得到OCV测试结果
        /// </summary>
        /// <param name="palletID">托盘号</param>
        /// <param name="ocvID">检测工艺序号1~5，1:OCV1,2:OCV2,3:分容，4:OCV3,5:OCV4</param>
        /// <param name="vals">测试结果列表</param>
        /// <param name="reStr">失败返回原因</param>
        /// <returns></returns>
        bool GetCheckResult(string palletID, List<int> testSeqIDS, ref List<int> vals, ref string reStr);
    }
}
