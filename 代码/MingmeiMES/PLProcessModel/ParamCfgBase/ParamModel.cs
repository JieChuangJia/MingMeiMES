using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLProcessModel.ParamCfgBase
{
    public enum EnumParamType
    {
        VAR_INT=1,
        VAR_FLOAT=2,
        VAR_STRING=3
    }
    public class ParamModel
    {
        public string ParamName { get; set; }
        public string ParamVal { get; set; }
        public string ParamType { get; set; }
        public ParamModel(string paramName,string paramVal,string paramType)
        {
            ParamName = paramName;
            ParamVal = paramVal;
            ParamType = paramType;
        }
    }
}
