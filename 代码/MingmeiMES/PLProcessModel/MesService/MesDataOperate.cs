using Microsoft.CSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PLProcessModel.MesServiceReference;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web.Services.Description;
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

    public class MesDataOperate
    {
        #region 数据
        ////WEB服务地址
        //string url = "http://58.222.123.130:9012/MesFrameWork.asmx";
       

        ////客户端代理服务命名空间，可以设置成需要的值。
        //string ns = string.Format("MesServiceReference");
        MesFrameWorkSoapClient client = null;
        //Type t = null;

        //object obj = null;
        #endregion
        public MesDataOperate()
        {
           client = new MesFrameWorkSoapClient();

        }


        private string DxTestDataUploadJson(string jsonStr)
        {
            return client.DxDataUploadJson(jsonStr); ;
        }


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

        /// <summary>
        /// 获得Json串
        /// </summary>
        /// <param name="jsonText"></param>
        /// <returns></returns>
        public RootObject GetJSON(string jsonText)
        {
            return JsonConvert.DeserializeObject<RootObject>(jsonText);  
        }



    }
}
