using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using DevInterface;
using DBAccess.BLL;
using DBAccess.DAL;
using DBAccess.Model;
namespace PLProcessModel
{
    /// <summary>
    /// 设备基类模型
    /// </summary>
    public class CtlDevBaseModel
    {
        protected string devID = "";
        protected string devName = "";
        protected string nodeID = "";
        protected int plcID = 0;
        protected string dbStartAddr = ""; //db开始地址

        protected int dbBlockNum = 12;
        protected short[] dbVals = null;
        protected string mesID = "";
        public IDictionary<string, DevWarnItemModel> devWarnList = null;
        public IDictionary<string, DevCfgItemModel> devCfgList = null;
        protected DevWarnRecordBll devWarnrecordBll = null;
        public IPlcRW PlcRW { get; set; }//设备的plc读写接口
        public string DevID { get { return devID; } }
        public string DevName { get { return devName; }}
        public string NodeID { get { return nodeID;} }
        public int PlcID { get { return plcID; } set { plcID = value; } }
        public string MesID { get { return mesID; } }
        public LogInterface.ILogRecorder LogRecorder { get; set; }
        public CtlDevBaseModel()
        {
            devWarnList = new Dictionary<string,DevWarnItemModel>();
            this.devCfgList = new Dictionary<string, DevCfgItemModel>();

            devWarnrecordBll = new DevWarnRecordBll();
        }
        #region 虚接口
        public virtual bool ParseCfg(XElement xe,ref string reStr)
        {
            //throw new NotImplementedException();
            try
            {
                if (xe.Attribute("mesID") != null)
                {
                    mesID = xe.Attribute("mesID").Value.ToString();
                }
                this.devID=xe.Attribute("id").Value.ToString();
                this.devName = xe.Attribute("devName").Value.ToString();
                this.plcID = int.Parse(xe.Attribute("plcID").Value.ToString());
                this.dbStartAddr = xe.Attribute("addrStart").Value.ToString().Trim(new char[]{'\0','\r','\n','\t',' '}).ToUpper();
                this.dbBlockNum = int.Parse(xe.Attribute("blockNum").Value.ToString());
                dbVals = new short[this.dbBlockNum*16];
                string warnCfgStr = xe.Element("RelayCfg").Value.ToString().Trim(new char[]{'\0','\r','\n','\t',' '});
                string[] strWarnArray = warnCfgStr.Split(new string[] { ";"}, StringSplitOptions.RemoveEmptyEntries);
                foreach(string strItem in strWarnArray)
                {
                    if(string.IsNullOrWhiteSpace(strItem))
                    {
                        continue;
                    }
                    string[] strItemArray = strItem.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    if(strItemArray == null || strItemArray.Count()<1)
                    {
                        continue;
                    }
                    DevWarnItemModel warnItem = new DevWarnItemModel();
                    warnItem.PlcAddr = strItemArray[0].Trim(new char[] { '\0', '\r', '\n', '\t', ' ' });
                    if (strItemArray.Length>1)
                    {
                        warnItem.WarnInfo = strItemArray[1];
                    }
                    if(strItemArray.Length>2)
                    {
                        warnItem.MesWarnID = strItemArray[2];
                    }
                    warnItem.WarnStat = 0; //初始化状态
                    warnItem.RecordTime = System.DateTime.Now;
                    this.devWarnList[warnItem.PlcAddr] = warnItem;

                }

                string setCfgStr = xe.Element("ParamCfg").Value.ToString().Trim(new char[] { '\0', '\r', '\n', '\t', ' ' });
                string[] strSetArray = setCfgStr.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string strItem in strSetArray)
                {
                    if (string.IsNullOrWhiteSpace(strItem))
                    {
                        continue;
                    }
                    string[] strItemArray = strItem.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    if (strItemArray == null || strItemArray.Count() < 1)
                    {
                        continue;
                    }
                    DevCfgItemModel setItem = new DevCfgItemModel();
                    string[] addrAndTypeArray = strItemArray[0].Trim(new char[] { '\0', '\r', '\n', '\t', ' ' }).Split(',');
                    setItem.PlcAddr = addrAndTypeArray[0];
                    setItem.AddrDataType = (EnumAddrDataType)Enum.Parse(typeof(EnumAddrDataType), addrAndTypeArray[1].ToUpper());
                    setItem.Desc = strItemArray[1]; //初始化状态

                    this.devCfgList[setItem.PlcAddr] = setItem;

                }
                return true;
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return false;
            }
        }
        public virtual bool DevWarnMonitor(ref string reStr)
        {
            if(PlcRW == null)
            {
                reStr = "PLC通信对象不存在";
                return false;
            }
            if(!SysCfgModel.SimMode)
            {
                if (!PlcRW.ReadMultiDB(this.dbStartAddr, this.dbBlockNum*16, ref dbVals))
                {
                    reStr = devName + "PLC未通信上";
                    Console.WriteLine(this.devName + "读取报警地址失败！");//测试
                    return false;
                }
            }
            //if (dbVals[0] > 0)//测试用
            //{
            //    dbVals[0] = 0;
            //    dbVals[1] = 0;
            //}
            //else
            //{
            //    dbVals[0] = 255;
            //    dbVals[1] = 255;
            //}
          
             //Console.WriteLine(this.devName + dbVals[0].ToString() + "," + dbVals[1].ToString());
            
            foreach(string key in devWarnList.Keys)
            {
                DevWarnItemModel warnItem = devWarnList[key];
                string[] plcAddrArray = warnItem.PlcAddr.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                int blockIndex = int.Parse(plcAddrArray[0].Substring(1)) - int.Parse(this.dbStartAddr.Substring(1));
                int bitIndex = int.Parse(plcAddrArray[1]);
                int val = dbVals[blockIndex * 16 + bitIndex];//(dbVals[blockIndex]>>bitIndex)&0x01;

                if(val != warnItem.WarnStat)
                {
                    DevWarnRecordModel warnRecord = new DevWarnRecordModel();
                    warnRecord.devID = this.devID;
                    warnRecord.plcAddr = warnItem.PlcAddr;
                    warnRecord.recordID = System.Guid.NewGuid().ToString();
                    warnRecord.recordTime = System.DateTime.Now;
                    warnRecord.warnStat = val;
                   
                    string areaLine="L001";
                    string reJsonStr="";
                    if(val == 0)
                    {
                        warnRecord.warnInfo += "已清除";
                        if(!string.IsNullOrWhiteSpace(this.mesID) && !string.IsNullOrWhiteSpace(warnItem.MesWarnID))
                        {
                            RootObject rObj = WShelper.DevErrorUpload(mesID, areaLine, warnItem.MesWarnID, 1, ref reJsonStr);
                            if(LogRecorder != null)
                            {
                                LogRecorder.AddDebugLog(devName, string.Format("报警{0}复位，上传MES，返回结果:{1}", warnItem.WarnInfo, rObj.RES));
                            }
                            
                        }
                       
                    }
                    else
                    {
                        warnRecord.warnInfo = warnItem.WarnInfo;
                        if(!string.IsNullOrWhiteSpace(this.mesID) && !string.IsNullOrWhiteSpace(warnItem.MesWarnID))
                        {
                            RootObject rObj = WShelper.DevErrorUpload(mesID, areaLine, warnItem.MesWarnID, 0, ref reJsonStr);
                            if (LogRecorder != null)
                            {
                                LogRecorder.AddDebugLog(devName, string.Format("报警{0}发生，上传MES，返回结果:{1}", warnItem.WarnInfo, rObj.RES));
                            }
                            
                        }
                       
                    }
                    devWarnrecordBll.Add(warnRecord);
                    Console.WriteLine(this.devName+":" +  warnRecord.warnInfo+"-》添加报警记录成功！");
                    warnItem.WarnStat = val;

                }
            }
            return true;
        }
        #endregion
    }
}
