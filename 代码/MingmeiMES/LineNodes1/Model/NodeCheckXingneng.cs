using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PLProcessModel;
namespace LineNodes
{
    public class NodeCheckXingneng : CtlNodeBaseModel
    {

        public override bool BuildCfg(System.Xml.Linq.XElement xe, ref string reStr)
        {

            if (!base.BuildCfg(xe, ref reStr))
            {
                return false;
            }

            return true;
        }
        public override bool ExeBusiness(ref string reStr)
        {
            return true;
        }
    }
}
