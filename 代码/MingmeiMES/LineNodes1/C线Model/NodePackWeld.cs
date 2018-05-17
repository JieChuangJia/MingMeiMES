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
    /// PACK焊接
    /// </summary>
    public class NodePackWeld : CtlNodeBaseModel
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
                        if(plcRW2 != null)
                        {
                            if (!plcRW2.WriteDB("D8500", bindModeCt))
                            {
                                this.currentTaskDescribe = "给机台发送模块数量失败";
                                break;
                            }
                        }
                           
                        
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.currentTask.TaskParam = rfidUID;
                        this.ctlTaskBll.Update(this.currentTask);
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
                        int requireReadData = 0;
                        if (this.plcRW2.ReadDB("D9100", ref requireReadData) == false)
                        {
                            break;
                        }
                        if (requireReadData != 1)
                        {
                            break;

                        }
                        int weldLJL = 0;//焊接离胶量
                        int hjdwjg = 0;//焊接定位结果
                        if (this.plcRW2.ReadDB("D9000", ref hjdwjg) == false)
                        {
                            break;
                        }
                        if (this.plcRW2.ReadDB("D9002", ref weldLJL) == false)
                        {
                            break;
                        }

                        if (this.plcRW2.WriteDB("D9100", 2) == false)
                        {
                            break;
                        }
                        string checkResult = "OK";
                        if (hjdwjg == 1)
                        {
                            checkResult = "OK";
                        }
                        else
                        {
                            checkResult = "NG";
                        }
                        float wedlljlF = (float)weldLJL / 100;
                        string uploadMesData = "焊接定位结果:" + checkResult + ":|焊接离焦量:" + wedlljlF + ":mm";
                        List<DBAccess.Model.BatteryModuleModel> moduleList = modBll.GetBindedMods(this.rfidUID);
                        if (moduleList == null || moduleList.Count == 0)
                        {
                            Console.WriteLine(this.nodeName, "此工装板没有绑定数据！");
                            break;
                        }
                        currentTaskDescribe = "焊接数据读取成功：" + uploadMesData;
                        int uploadMesStatus = UploadToMesData(3, moduleList[0].batPackID, "M00100601", uploadMesData, ref reStr);
                        if (uploadMesStatus == 0)
                        {
                            this.logRecorder.AddDebugLog(this.nodeName, "上传MES数据成功：" + reStr);
                        }
                        else if (uploadMesStatus == 1)
                        {
                            this.logRecorder.AddDebugLog(this.nodeName, "上传MES数据成功，返回NG：" + uploadMesData);

                        }
                        else
                        {
                            Console.WriteLine(this.nodeName + ":"+reStr);
                            break;
                        }
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);

                        break;
                    }
                case 4:
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

        private int UploadToMesData(int flag, string groupCode, string workStationNum,string valueItem, ref string reStr)
        {
            RootObject rObj = DevDataUpload(flag, "", workStationNum, groupCode, "", "", "", valueItem, ref reStr);
            reStr = rObj.RES;
            if (rObj.RES.ToUpper().Contains("OK"))
            {
                reStr += "数据：" + valueItem;
                return 0;
            }
             else if(rObj.RES.ToUpper().Contains("NG") ==true)
            {
           
                return 1;
             }
            else
            {
                reStr ="MES数据上传失败："+ rObj.RES;
                Console.WriteLine(this.nodeName + "上传MES二维码信息错误：" + rObj.RES);
                return 2;
            }
        }
    }
}
