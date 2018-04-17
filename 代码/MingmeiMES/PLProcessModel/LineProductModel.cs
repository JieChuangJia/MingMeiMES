using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLProcessModel
{
    /// <summary>
    /// 流水线产品建模
    /// </summary>
    public class LineProductModel
    {

        private string productID = "";
        private List<CtlNodeBaseModel> movedPath = null; //产品流过的节点
        public LineProductModel(string productID)
        {
            this.productID = productID;
        }
    }
}
