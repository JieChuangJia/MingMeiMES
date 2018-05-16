using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.CodeDom;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Web.Services.Description;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PLProcessModel
{
    #region JSON 结构
        /// <summary>
        /// JSON字符串组成
        /// </summary>
        public class RootObject
        {
            /// <summary>
            /// 返回值，上传成功返回 OK，否则返回NG加错误提示信息
            /// </summary>
            public string RES { get; set; }

            /// <summary>
            /// 返回值，停机时返回STOP，正常运行时返回RUN
            /// </summary>
            public string CONTROL_TYPE { get; set; }

            /// <summary>
            /// 交互数据类
            /// </summary>
            public List<ContentDetail> M_COMENT { get; set; }
        }

        /// <summary>
        /// 交互数据类:JSON字符串 Content成员
        /// </summary>
        public class ContentDetail
        {
            /// <summary>
            /// 操作类型
            /// </summary>
            public int M_FLAG { get; set; }

            /// <summary>
            /// 工作中心SN
            /// </summary>
            public string M_DEVICE_SN { get; set; }

            /// <summary>
            /// 工作中心 
            /// </summary>
            public string M_WORKSTATION_SN { get; set; }

            /// <summary>
            /// 员工号
            /// </summary>
            public string M_EMP_NO { get; set; }

            /// <summary>
            /// 线体
            /// </summary>
            public string M_AREA { get; set; }

            /// <summary>
            /// 制令单
            /// </summary>
            public string M_MO { get; set; }

            /// <summary>
            /// 料号/机种
            /// </summary>
            public string M_MODEL { get; set; }

            /// <summary>
            /// 容器SN
            /// </summary>
            public string M_CONTAINER_SN { get; set; }

            /// <summary>
            /// 模块SN/模组SN/电池SN
            /// </summary>
            public string M_SN { get; set; }

            /// <summary>
            /// 子组件SN(子组件SN1|子组件SN2|子组件SN3……),（电芯SN/模块SN）
            /// </summary>
            public string M_UNION_SN { get; set; }

            /// <summary>
            /// 档位
            /// </summary>
            public string M_LEVEL { get; set; }

            /// <summary>
            /// 测试结果(OK/NG),OK为良品,NG为不良品
            /// </summary>
            public string M_EC_FLAG { get; set; }

            /// <summary>
            /// 产品测试数据/设备参数数据/过程参数
            /// 测试项1:值1:单位1|测试项2:值2:单位2|测试项3:值3:单位……
            /// </summary>
            public string M_ITEMVALUE { get; set; }

            /// <summary>
            /// 测试时间(例如：2016-12-26 10:42:21)
            /// </summary>
            public string M_TEST_TIME { get; set; }

            /// <summary>
            /// 描述
            /// </summary>
            public string M_DECRIPTION { get; set; }

            /// <summary>
            /// 测试流程名称
            /// </summary>
            public string M_ROUTE { get; set; }

            /// <summary>
            /// 工序
            /// </summary>
            public string M_GROUP { get; set; }

            /// <summary>
            /// 报警代码
            /// </summary>
            public string M_ERROR_CODE { get; set; }

            /// <summary>
            /// 报警等级
            /// </summary>
            public string M_ERROR_LEVEL { get; set; }

            /// <summary>
            /// 报警状态（0发生、1恢复）
            /// </summary>
            public string M_ERROR_STATUS { get; set; }

            /// <summary>
            /// 数据类型 0测试数据 1设备参数
            /// </summary>
            public int M_ITEM_TYPE
            { get; set; }

            /// <summary>
            /// 极性
            /// </summary>
            public string M_POLAR { get; set; }

            /// <summary>
            /// 日志ID
            /// </summary>
            public int M_LOG_ID { get; set; }

            /// <summary>
            /// 预留字段1
            /// </summary>
            public string M_MARK1 { get; set; }

            /// <summary>
            /// 预留字段2
            /// </summary>
            public string M_MARK2 { get; set; }

            /// <summary>
            /// 预留字段3
            /// </summary>
            public string M_MARK3 { get; set; }

            /// <summary>
            /// 预留字段4
            /// </summary>
            public string M_MARK4 { get; set; }
        }

        #endregion
    public class WShelper
    {
        public static string url = "http://172.20.1.7:9012/MesFrameWork.asmx";

        /// < summary>           
        /// 动态调用web服务         
        /// < /summary>          
        /// < param name="url">WSDL服务地址< /param> 
        /// < param name="methodname">方法名< /param>           
        /// < param name="args">参数< /param>           
        /// < returns>< /returns>          
        public static object InvokeWebService(string url, string methodname, object[] args)
        {
            return WShelper.InvokeWebService(url, null, methodname, args);
        }

        public static object InvokeWebService(string url, string classname, string methodname, object[] args)
        {
            string @namespace = "EnterpriseServerBase.WebService.DynamicWebCalling";
            if ((classname == null) || (classname == ""))
            {
                classname = WShelper.GetWsClassName(url);
            }

            try
            {
                //获取WSDL
                WebClient wc = new WebClient();
               
                Stream stream = wc.OpenRead(url + "?WSDL");
                ServiceDescription sd = ServiceDescription.Read(stream);
                ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
                sdi.AddServiceDescription(sd, "", "");
                CodeNamespace cn = new CodeNamespace(@namespace);

                //生成客户端代理类代码
                CodeCompileUnit ccu = new CodeCompileUnit();
                ccu.Namespaces.Add(cn);
                sdi.Import(cn, ccu);
                CodeDomProvider provider = new CSharpCodeProvider();//设定编译参数
                CompilerParameters cplist = new CompilerParameters();
                cplist.GenerateExecutable = false;
                cplist.GenerateInMemory = true;
                cplist.ReferencedAssemblies.Add("System.dll");
                cplist.ReferencedAssemblies.Add("System.XML.dll");
                cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
                cplist.ReferencedAssemblies.Add("System.Data.dll");

                //编译代理类
                CompilerResults cr = provider.CompileAssemblyFromDom(cplist, ccu);
                if (true == cr.Errors.HasErrors)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                    {
                        sb.Append(ce.ToString());
                        sb.Append(System.Environment.NewLine);
                    }
                    throw new Exception(sb.ToString());
                }

                //生成代理实例，并调用方法
                System.Reflection.Assembly assembly = cr.CompiledAssembly;
                Type t = assembly.GetType(@namespace + "." + classname, true, true);
                object obj = Activator.CreateInstance(t);
                System.Reflection.MethodInfo mi = t.GetMethod(methodname);

                return mi.Invoke(obj, args);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message, new Exception(ex.InnerException.StackTrace));
            }
        }

        private static string GetWsClassName(string wsUrl)
        {
            string[] parts = wsUrl.Split('/');
            string[] pps = parts[parts.Length - 1].Split('.');
            return pps[0];
        }

        #region 返回JSON字符串
        /// <summary>
        /// 返回JSON字符串
        /// </summary>
        /// <param name="RES">返回执行结果</param>
        /// <param name="CDetail">JSON数据类</param>
        /// <returns></returns>
        public static string ReturnJsonData(string RES, string CONTROL_TYPE, List<ContentDetail> CDetail)
        {
            JArray l_jarray = new JArray();
            JObject l_jobject = new JObject();
            JObject l_total = new JObject();

            for (int i = 0; i < CDetail.Count; i++)
            {
                l_jobject = new JObject();
                l_jobject.Add("M_FLAG", CDetail[i].M_FLAG);
                l_jobject.Add("M_DEVICE_SN", CDetail[i].M_DEVICE_SN);
                l_jobject.Add("M_WORKSTATION_SN", CDetail[i].M_WORKSTATION_SN);
                l_jobject.Add("M_EMP_NO", CDetail[i].M_EMP_NO);
                l_jobject.Add("M_AREA", CDetail[i].M_AREA);
                l_jobject.Add("M_MO", CDetail[i].M_MO);
                l_jobject.Add("M_MODEL", CDetail[i].M_MODEL);
                l_jobject.Add("M_CONTAINER_SN", CDetail[i].M_CONTAINER_SN);
                l_jobject.Add("M_SN", CDetail[i].M_SN);

                l_jobject.Add("M_UNION_SN", CDetail[i].M_UNION_SN);
                l_jobject.Add("M_LEVEL", CDetail[i].M_LEVEL);
                l_jobject.Add("M_EC_FLAG", CDetail[i].M_EC_FLAG);
                l_jobject.Add("M_ITEMVALUE", CDetail[i].M_ITEMVALUE);
                l_jobject.Add("M_TEST_TIME", CDetail[i].M_TEST_TIME);
                l_jobject.Add("M_DECRIPTION", CDetail[i].M_DECRIPTION);

                l_jobject.Add("M_ROUTE", CDetail[i].M_ROUTE);
                l_jobject.Add("M_GROUP", CDetail[i].M_GROUP);
                l_jobject.Add("M_ERROR_CODE", CDetail[i].M_ERROR_CODE);
                l_jobject.Add("M_ERROR_LEVEL", CDetail[i].M_ERROR_LEVEL);
                l_jobject.Add("M_ERROR_STATUS", CDetail[i].M_ERROR_STATUS);
                l_jobject.Add("M_ITEM_TYPE", CDetail[i].M_ITEM_TYPE);
                l_jobject.Add("M_POLAR", CDetail[i].M_POLAR);
                l_jobject.Add("M_LOG_ID", CDetail[i].M_LOG_ID);

                l_jobject.Add("M_MARK1", CDetail[i].M_MARK1);
                l_jobject.Add("M_MARK2", CDetail[i].M_MARK2);
                l_jobject.Add("M_MARK3", CDetail[i].M_MARK3);
                l_jobject.Add("M_MARK4", CDetail[i].M_MARK4);

                l_jarray.Add(l_jobject);
            }

            l_total.Add("RES", RES);
            l_total.Add("CONTROL_TYPE", CONTROL_TYPE);
            l_total.Add("M_COMENT", l_jarray);

            return l_total.ToString();
        }
        #endregion

        /// <summary>
        /// 设备请求条码
        /// </summary>
        public static RootObject BarCodeRequest(string M_WORKSTATION_SN)
        {
        
            List<ContentDetail> CList = new List<ContentDetail>();
            ContentDetail tail = new ContentDetail();
            tail.M_FLAG = 11;
           // tail.M_WORKSTATION_SN = "Y00100101";
            tail.M_WORKSTATION_SN =M_WORKSTATION_SN;
            CList.Add(tail);
            //上传参数
            string strJson = WShelper.ReturnJsonData("OK", "RUN", CList);
            object objJson = strJson;
            object[] addParams = new object[] { objJson };
            object result = WShelper.InvokeWebService(url, "DxDataUploadJson", addParams);
            string strRES = result.ToString();
            RootObject rObj = new RootObject();
            rObj = JsonConvert.DeserializeObject<RootObject>(strRES);
            return rObj;
        }

        /// <summary>
        /// 设备数据上传
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static RootObject DevDataUpload(int M_FLAG, string M_DEVICE_SN, string M_WORKSTATION_SN, string M_SN, string M_UNION_SN, string M_CONTAINER_SN, string M_LEVEL, string M_ITEMVALUE, ref string strJson)
        {
            RootObject rObj = null;
            if(SysCfgModel.SimMode)
            {
                rObj = new RootObject();
                rObj.RES="OK";

                return rObj;
            }
            List<ContentDetail> CList = new List<ContentDetail>();
            ContentDetail tail = new ContentDetail();
            tail.M_FLAG = M_FLAG;
            tail.M_DEVICE_SN = M_DEVICE_SN;
            tail.M_WORKSTATION_SN = M_WORKSTATION_SN;
            tail.M_SN = M_SN;
            tail.M_UNION_SN = M_UNION_SN;
            tail.M_CONTAINER_SN = M_CONTAINER_SN;
            tail.M_LEVEL = M_LEVEL;
            tail.M_ITEMVALUE = M_ITEMVALUE;
            CList.Add(tail);
            //上传参数
            strJson = WShelper.ReturnJsonData("OK", "RUN", CList);
            object objJson = strJson;
            object[] addParams = new object[] { objJson };
            object result = WShelper.InvokeWebService(url, "DxDataUploadJson", addParams);
            string strRES = result.ToString();
            rObj = new RootObject();
            rObj = JsonConvert.DeserializeObject<RootObject>(strRES);
            return rObj;
        }
       /// <summary>
       /// 过程参数上传
       /// </summary>
       /// <param name="M_FLAG"></param>
       /// <param name="M_DEVICE_SN"></param>
       /// <param name="M_WORKSTATION_SN"></param>
       /// <param name="M_SN"></param>
       /// <param name="M_UNION_SN"></param>
       /// <param name="M_CONTAINER_SN"></param>
       /// <param name="M_LEVEL"></param>
       /// <param name="M_ITEMVALUE"></param>
       /// <returns></returns>
        public static RootObject ProcParamUpload(string M_AREA,string M_DEVICE_SN, string M_WORKSTATION_SN, string M_UNION_SN, string M_CONTAINER_SN, string M_LEVEL, string M_ITEMVALUE,ref string strJson)
        {
            RootObject rObj = null;
            if (SysCfgModel.SimMode)
            {
                rObj = new RootObject();
                rObj.RES = "OK";

                return rObj;
            }
            List<ContentDetail> CList = new List<ContentDetail>();
            ContentDetail tail = new ContentDetail();
            tail.M_FLAG = 6;
            tail.M_AREA = M_AREA;
            tail.M_DEVICE_SN = M_DEVICE_SN;
            tail.M_WORKSTATION_SN = M_WORKSTATION_SN;
            tail.M_UNION_SN = M_UNION_SN;
            tail.M_CONTAINER_SN = M_CONTAINER_SN;
            tail.M_LEVEL = M_LEVEL;
            tail.M_ITEMVALUE = M_ITEMVALUE;
            CList.Add(tail);
            //上传参数
            strJson = WShelper.ReturnJsonData("OK", "RUN", CList);
            object objJson = strJson;
            object[] addParams = new object[] { objJson };
            object result = WShelper.InvokeWebService(url, "DxDataUploadJson", addParams);
            string strRES = result.ToString();
            rObj = new RootObject();
            rObj = JsonConvert.DeserializeObject<RootObject>(strRES);
            return rObj;
        }
        public static RootObject DevErrorUpload(string M_DEVICE_SN, string M_AREA, string errCode, int errStatus, ref string strJson)
        {
            RootObject rObj = null;
            if (SysCfgModel.SimMode)
            {
                rObj = new RootObject();
                rObj.RES = "OK";

                return rObj;
            }
            List<ContentDetail> CList = new List<ContentDetail>();
            ContentDetail tail = new ContentDetail();
            tail.M_FLAG = 7;
            tail.M_AREA = M_AREA;
            tail.M_DEVICE_SN = M_DEVICE_SN;
            tail.M_ERROR_CODE = errCode;
            tail.M_ERROR_STATUS = errStatus.ToString();
            CList.Add(tail);
            strJson = WShelper.ReturnJsonData("OK", "RUN", CList);
            object objJson = strJson;
            object[] addParams = new object[] { objJson };
            object result = WShelper.InvokeWebService(url, "DxDataUploadJson", addParams);
            string strRES = result.ToString();
            rObj = new RootObject();
            rObj = JsonConvert.DeserializeObject<RootObject>(strRES);
            return rObj;
        }
    }
}
