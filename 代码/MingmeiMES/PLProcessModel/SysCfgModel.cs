using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace PLProcessModel
{
    /// <summary>
    /// 任务执行状态（控制任务、管理任务）
    /// </summary>
    public enum EnumTaskStatus
    {
        待执行,
        执行中,
        已完成,
        超时, //任务在规定时间内未完成
        错误, //任务发生错误，不可能再继续执行了，必须人工清理掉
        任务撤销
    }
    public class SysCfgModel
    {
        public static string cfgFilePath = "";
        public static Dictionary<string, ParamCfgBase.ParamModel> SysParamDic = new Dictionary<string, ParamCfgBase.ParamModel>();
        public static string mesLineID = "L10";
        public static bool PlcCommSynMode = true;//同步通信模式
        public static bool SimMode = false;//仿真模式
        public static bool DebugMode = false; //调试模式，所有工位必检
        public static bool ManualMode1 = false;//1号房人工检测模式
        public static bool ManualMode3 = false;//1号房人工检测模式
        public static bool MesAutodownEnabled = false; //投产位是否调用MES投产
        public static bool IsRequireMesQRCode = false; //是否向MES请求二维码
        public static string DB1Start = "D2000";
        public static string DB2Start = "D3000";
        public static int DB1Len = 600;
        public static int DB2Len = 100;
        public static string[] checkStations = null;
        public static DateTime loginTime1 = DateTime.Parse("08:00:00");
        public static DateTime loginTime2 = DateTime.Parse("20:00:00");
       // public static bool MesInputCheck { get; set; }
        public static bool PreStationCheck { get; set; }

        public static bool MesCheckEnable { get; set; }
        public static bool ZeroFireEnable { get; set; }
        public static bool MesOfflineMode { get; set; }
        public static bool PrienterEnable { get; set; }

        public static int MesTimeout { get; set; }
        public static int RfidDelayTimeout { get; set; }
        public static bool SaveCfg(ref string reStr)
        {
            return SaveCfg(cfgFilePath, ref reStr);
        }
        public static bool SaveCfg(string xmlCfgFile,ref string reStr)
        {
            try
            {
               
                 //string xmlCfgFile = System.AppDomain.CurrentDomain.BaseDirectory + @"data/DevConfigFTzj.xml";
                if (!File.Exists(xmlCfgFile))
                {
                    reStr = "系统配置文件：" + xmlCfgFile + " 不存在!";
                    return false;
                }
                XElement root = XElement.Load(xmlCfgFile);
                XElement printerXE = root.Element("sysSet").Element("Printer");
                if(PrienterEnable)
                {
                    printerXE.Attribute("Enable").Value = "True";
                }
                else
                {
                    printerXE.Attribute("Enable").Value = "False";
                }

                XElement sysSetXE = root.Element("sysSet");
                XElement sysParamXE = sysSetXE.Element("SysParam");
                List<ParamCfgBase.ParamModel> paramList = new List<ParamCfgBase.ParamModel>();
                foreach(string strKey in SysParamDic.Keys)
                {
                    paramList.Add(SysParamDic[strKey]);
                }
                string strJsonParam = JsonConvert.SerializeObject(paramList);
                sysParamXE.Value = strJsonParam;

                XElement runModeXE = root.Element("sysSet").Element("RunMode");
                if(runModeXE.Attribute("debugMode") != null)
                {
                    if(DebugMode)
                    {
                        runModeXE.Attribute("debugMode").Value = "True";
                    }
                    else
                    {
                        runModeXE.Attribute("debugMode").Value = "False";
                    }
                }  
                if(runModeXE.Attribute("manualMode1") != null)
                {
                    if(ManualMode1)
                    {
                        runModeXE.Attribute("manualMode1").Value = "True";
                    }
                    else
                    {
                        runModeXE.Attribute("manualMode1").Value = "False";
                    }
                }
                if (runModeXE.Attribute("manualMode3") != null)
                {
                    if (ManualMode1)
                    {
                        runModeXE.Attribute("manualMode3").Value = "True";
                    }
                    else
                    {
                        runModeXE.Attribute("manualMode3").Value = "False";
                    }
                }
                XElement mesXE = root.Element("sysSet").Element("Mes");
                if( MesCheckEnable)
                {
                    mesXE.Attribute("Enable").Value="True";
                }
                else
                {
                    mesXE.Attribute("Enable").Value="False";
                }
                if(MesOfflineMode)
                {
                    mesXE.Attribute("OfflineMode").Value = "True";
                }
                else
                {
                    mesXE.Attribute("OfflineMode").Value = "False";
                }
                if(MesAutodownEnabled)
                {
                    mesXE.Attribute("MesAutodown").Value = "True";
                }
                else
                {
                    mesXE.Attribute("MesAutodown").Value = "False";
                }
                XElement mesTimeOutXE = root.Element("sysSet").Element("MesDownTimeout");
                mesTimeOutXE.Value = MesTimeout.ToString();
                XElement rfidTimeOutXE = root.Element("sysSet").Element("RfidTimeout");
                rfidTimeOutXE.Value = RfidDelayTimeout.ToString();

               
                root.Save(xmlCfgFile);
                return true;
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return false;
            }
        }
        public static bool LoadCfg(string xmlCfgFile,ref string reStr)
        {
            try
            {
              
              //  string xmlCfgFile = System.AppDomain.CurrentDomain.BaseDirectory + @"data/DevConfigFTYJ.xml";
                if (!File.Exists(xmlCfgFile))
                {
                    reStr = "系统配置文件：" + xmlCfgFile + " 不存在!";
                    return false;
                }
                cfgFilePath = xmlCfgFile;
                XElement root = XElement.Load(xmlCfgFile);

                XElement sysSetXE = root.Element("sysSet");
                XElement sysParamXE = sysSetXE.Element("SysParam");
                XElement qrCodewReqXe = sysSetXE.Element("QRCodeRequire");
                string strParam = sysParamXE.Value.ToString();
                if(!string.IsNullOrWhiteSpace(strParam))
                {
                    JArray jsonParams = (JArray)JsonConvert.DeserializeObject(strParam);
                    if(jsonParams != null && jsonParams.Count()>0)
                    {
                        foreach(JObject paramObj in jsonParams)
                        {
                            ParamCfgBase.ParamModel param = new ParamCfgBase.ParamModel(paramObj["ParamName"].ToString(),paramObj["ParamVal"].ToString(),paramObj["ParamType"].ToString());
                            SysParamDic[paramObj["ParamName"].ToString()] = param;
                        }
                        
                    }
                }
                XElement runModeXE = root.Element("sysSet").Element("RunMode");
                string simStr = runModeXE.Attribute("sim").Value.ToString().ToUpper();
                string qrCodeReq = qrCodewReqXe.Attribute("requireMes").Value.ToString().ToUpper();
                if(qrCodeReq == "TRUE")
                {
                    IsRequireMesQRCode = true;
                }
                else
                {
                    IsRequireMesQRCode = false;
                }
                if (simStr == "TRUE")
                {
                    SimMode = true;
                }
                else
                {
                    SimMode = false;
                }
                if(runModeXE.Attribute("debugMode") != null)
                {
                    string debugModeStr = runModeXE.Attribute("debugMode").Value.ToString().ToUpper();
                    if(debugModeStr == "TRUE")
                    {
                        DebugMode = true;
                    }
                    else
                    {
                        DebugMode = false;
                    }
                }
                if(runModeXE.Attribute("manualMode1") != null)
                {
                    string manualModeStr = runModeXE.Attribute("manualMode1").Value.ToString().ToUpper();
                    if(manualModeStr == "TRUE")
                    {
                        ManualMode1 = true;
                    }
                    else
                    {
                        ManualMode1 = false;
                    }
                }
                if (runModeXE.Attribute("manualMode3") != null)
                {
                    string manualModeStr = runModeXE.Attribute("manualMode3").Value.ToString().ToUpper();
                    if (manualModeStr == "TRUE")
                    {
                        ManualMode3 = true;
                    }
                    else
                    {
                        ManualMode3 = false;
                    }
                }
                XElement printerXE = root.Element("sysSet").Element("Printer");
                string str = printerXE.Attribute("Enable").Value.ToString().ToUpper();
                if (str == "TRUE")
                {
                    PrienterEnable = true;
                }
                else
                {
                    PrienterEnable = false;
                }
                XElement mesXE = root.Element("sysSet").Element("Mes");
                if (mesXE.Attribute("LineID") != null)
                {
                    mesLineID = mesXE.Attribute("LineID").Value.ToString();
                }
                else
                {
                    reStr = "MES产线ID未定义";
                    return false;
                }
                str = mesXE.Attribute("Enable").Value.ToString().ToUpper();
                if (str == "TRUE")
                {
                    MesCheckEnable = true;
                }
                else
                {
                    MesCheckEnable = false;
                }
                str = mesXE.Attribute("OfflineMode").Value.ToString().ToUpper();
                if(str== "TRUE")
                {
                    MesOfflineMode = true;
                }
                else
                {
                    MesOfflineMode = false;
                }
               str= mesXE.Attribute("MesAutodown").Value.ToString().ToUpper();
                if(str== "TRUE")
                {
                    MesAutodownEnabled = true;
                }
                else
                {
                    MesAutodownEnabled = false;
                }
                XElement mesTimeOutXE = root.Element("sysSet").Element("MesDownTimeout");
                MesTimeout = int.Parse(mesTimeOutXE.Value.ToString());
               
                XElement rfidTimeOutXE = root.Element("sysSet").Element("RfidTimeout");
                RfidDelayTimeout=int.Parse(rfidTimeOutXE.Value.ToString());
                
                return true;
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return false;
            }
           
        }
    }
}
