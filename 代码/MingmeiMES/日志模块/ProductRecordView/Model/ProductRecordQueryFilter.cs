using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProductRecordView
{
    /// <summary>
    /// 生产记录查询过滤参数
    /// </summary>
    public class ProductRecordQueryFilter
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LikeContent { get; set; }
        public bool LikeQueryEnable { get; set; }
        public ProductRecordQueryFilter()
        {
            StartDate = System.DateTime.Now - (new TimeSpan(30, 0, 0, 0));
            EndDate = System.DateTime.Now + (new TimeSpan(1, 0, 0, 0));
            LikeContent = "";
            LikeQueryEnable = false;
        }
    }
}
