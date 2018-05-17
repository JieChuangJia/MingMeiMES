using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FTDataAccess.BLL;
using FTDataAccess.Model;
using PLProcessModel;
namespace LineNodes
{
    /// <summary>
    ///  打带机
    /// </summary>
    public class NodePackFasten:CtlNodeBaseModel
    {
        public override bool BuildCfg(System.Xml.Linq.XElement xe, ref string reStr)
        {
            if (!base.BuildCfg(xe, ref reStr))
            {
                return false;
            }
            this.dicCommuDataDB1[1].DataDescription = "1:rfid复位，2：RFID成功，3：读RFID失败";
            this.dicCommuDataDB1[2].DataDescription = "1：复位/待机状态,2：数据读取中,3：数据读取完毕，放行";
            this.dicCommuDataDB2[1].DataDescription = "1：无板,2：有板，读卡请求";
            return true;
        }
        public override bool ExeBusiness(ref string reStr)
        {
            if (!ExeBusinessC(ref reStr))
            {
                return false;
            }
           
            
            switch (currentTaskPhase)
            {
                case 1:
                    {
                        if (!RfidReadC())
                        {
                            break;
                        }
                        int bindModeCt = modBll.GetRecordCount(string.Format("palletID='{0}' and palletBinded=1", this.rfidUID));
                        if (plcRW2 != null)
                        {
                            if (!plcRW2.WriteDB("D8500", bindModeCt))
                            {
                                this.currentTaskDescribe = "给机台发送模块数量失败";
                                break;
                            }
                        }
                        int require =0;
                        if (plcRW2.ReadDB("D9100", ref require) == false)
                        {
                            this.currentTaskDescribe = "读取服务模组压力、长度请求失败！";
                            break;
                        }
                        int modulePres = 0;//模组压力
                        int mouduleLen = 0;//模组长度
                        if(require != 1)
                        {                     
                            break;
                        }
                        if (plcRW2.ReadDB("D9000", ref modulePres) == false)
                        {
                            this.currentTaskDescribe = "服务模组压力失败！";
                            break;
                        }
                        if (plcRW2.ReadDB("D9002", ref mouduleLen) == false)
                        {
                            this.currentTaskDescribe = "服务模组长度失败！";
                            break;
                        }

                        if (plcRW2.WriteDB("D9100", 2) == false)
                        {
                            this.currentTaskDescribe = "读取服务模组压力、长度完成写入失败！";
                            break;
                        }
                        string restr = "";
                        //上报MES压力和长度
                         int uploadStatus =  UploadDataToMes(3, "M00100201", this.rfidUID, mouduleLen, modulePres, ref restr);
                        if(uploadStatus ==0)
                        {
                            this.logRecorder.AddDebugLog(this.nodeName, "上传MES数据成功：" + restr);
                        }
                        else if(uploadStatus==1)
                        {
                            this.logRecorder.AddDebugLog(this.nodeName, "上传MES数据成功，返回NG：" + restr);
                        }
                        else
                        {
                            Console.WriteLine(this.nodeName+"上传MES数据失败：" + restr);
                            break;
                        }


                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.currentTask.TaskParam = rfidUID+"," + mouduleLen+","+modulePres;
                        this.logRecorder.AddDebugLog(this.nodeName, "读取数据：压力值：" + modulePres + ";长度：" + mouduleLen);
                        
                        currentTaskPhase++;
                      
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        
                        break;
                    }
                case 2:
                    {
                       
                        db1ValsToSnd[1] = 2;//
                        if (!ProductTraceRecord())
                        {
                            break;
                        }
                        db1ValsToSnd[1] = 3;
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 3:
                    {
                        currentTaskDescribe = "流程完成";
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.currentTask.TaskStatus = EnumTaskStatus.已完成.ToString();
                        this.ctlTaskBll.Update(this.currentTask);
                      
                        break;
                    }
                default:
                    break;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="M_WORKSTATION_SN">工作中心号码</param>
        /// <param name="rfid">二维码</param>
        /// <returns></returns>
        private int UploadDataToMes(int flag, string workStaionSn, string rfid, int moduleLen, int modulePre,ref string restr)
        {
            string M_AREA = "Y001";
            string M_WORKSTATION_SN = workStaionSn;
            string M_DEVICE_SN = "";

            string M_UNION_SN = "";
            string M_CONTAINER_SN = "";
            string M_LEVEL = "";
            string M_ITEMVALUE = "模组长度:" + moduleLen + ":mm|模组压力:" + modulePre + ":kg";
            RootObject rObj = new RootObject();
            List<DBAccess.Model.BatteryModuleModel> modelList = modBll.GetModelList(string.Format("palletID='{0}' and palletBinded=1", rfid)); //modBll.GetModelByPalletID(this.rfidUID, this.nodeName);
            if (modelList == null || modelList.Count == 0)
            {
                restr = "无绑定数据！";
                return 2;
            }
            string barcode = modelList[0].batPackID;
            string strJson = "";

            rObj = DevDataUpload(flag, M_DEVICE_SN, M_WORKSTATION_SN, barcode, M_UNION_SN, M_CONTAINER_SN, M_LEVEL, M_ITEMVALUE, ref strJson);
            restr = rObj.RES;
            if (rObj.RES.ToUpper().Contains("OK") == true)
            {
                return 0;
            }
            else if (rObj.RES.ToUpper().Contains("NG") == true)
            {
                return 1;
            }
            else
            {
                Console.WriteLine(this.nodeName + "上传MES二维码信息错误：" + rObj.RES);
                return 2;
            }
        }
    }
}
