using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PLProcessModel;
using LogInterface;
namespace LineNodes
{
    /// <summary>
    /// 二次试火控制节点
    /// </summary>
    public class NodeFireTryingB:NodeFireTryingA
    {
        public override bool BuildCfg(System.Xml.Linq.XElement xe, ref string reStr)
        {
            if (!base.BuildCfg(xe, ref reStr))
            {
                return false;
            }
            this.dicCommuDataDB1[1].DataDescription = "1：检查OK，放行，2：NG，4：读卡/条码失败，未投产，8：需要检测，16：不需要检测";
            this.dicCommuDataDB1[2].DataDescription = "气源型号,从1开始编号";
            for (int i = 0; i < 30; i++)
            {
                this.dicCommuDataDB1[3 + i].DataDescription = string.Format("条码[{0}]", i + 1);
            }
            this.dicCommuDataDB2[1].DataDescription = "0:无板,1：有产品,2：空板";
            this.dicCommuDataDB2[2].DataDescription = "1：检测OK,2：检测NG";
            this.dicCommuDataDB2[3].DataDescription = "不合格项编码";
            return true;
        }
      
    }
}
