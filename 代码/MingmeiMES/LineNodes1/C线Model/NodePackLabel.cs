using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using FTDataAccess.BLL;
using FTDataAccess.Model;
using PLProcessModel;
namespace LineNodes
{
    /// <summary>
    /// 打码机
    /// </summary>
    public class NodePackLabel : CtlNodeBaseModel
    {
        private string packBarcode = "";
        private int barcodeFailCounter = 0;
        private int readPackLabelCodeFailTimes = 0;//打码完成后，计次，当读取6次还不成功就认为读取失败
        string qrCodeIP = "192.168.0.69";//向电脑发送二维码
        private DBAccess.BLL.BatteryPackBll packBll = new DBAccess.BLL.BatteryPackBll();
        public override bool BuildCfg(System.Xml.Linq.XElement xe, ref string reStr)
        {
            if (!base.BuildCfg(xe, ref reStr))
            {
                return false;
            }
            this.dicCommuDataDB1[1].DataDescription = "1:rfid复位，2：RFID成功，3：读RFID失败4:绑定数据为空";
            this.dicCommuDataDB1[2].DataDescription = "1：复位/待机状态,2：数据读取中,3：数据读取完毕，放行";
            this.dicCommuDataDB1[3].DataDescription = "1：读码状态复位,2：读码成功,3：读码失败,4：读码两次失败，报警";

            this.dicCommuDataDB2[1].DataDescription = "1：无板,2：有板，读卡请求";
            this.dicCommuDataDB2[2].DataDescription = "1：无扫码请求,2：扫码请求";
            this.dicCommuDataDB2[3].DataDescription = "1：申请二维码（PLC->MES）,2：申请成功（MES->PLC）3:申请失败（MES->PLC）";
            return true;
        }
        public override bool ExeBusiness(ref string reStr)
        {
            if (!ExeBusinessC(ref reStr))
            {
                return false;
            }
           if(this.db2Vals[0] == 1)
           {
               this.db1ValsToSnd[2] = 0;
           }

            switch (currentTaskPhase)
            {
                case 1:
                    {
                        if (!RfidReadC())
                        {
                            break;
                        }
                        db1ValsToSnd[1] = 2;//
                        if (!ProductTraceRecord())
                        {
                            break;
                        }
                        if(GetPalletCheckNg() ==true)//NG处理
                        {
                            db1ValsToSnd[0] = 5;//
                            currentTaskPhase = 4;
                            this.TxtLogRecorder.WriteLog("绑定数据有NG产品，线体服务器处理流程结束！直接放行！");
                            this.logRecorder.AddDebugLog(this.nodeName, "绑定数据有NG产品，线体服务器处理流程结束！");
                            break;
                        }
                        barcodeFailCounter = 0;
                        this.readPackLabelCodeFailTimes = 0;
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
                  
                        if (this.db2Vals[2] != 1)
                        {
                            break;
                        }
                      
                       List<DBAccess.Model.BatteryModuleModel> modList=  modBll.GetBindedMods(this.rfidUID);
                        if(modList == null|| modList.Count==0)
                        {
                            this.currentTaskDescribe = "获取二维码失败，请确认绑定工位是否成功绑定！";
                            if (NodeDB2Commit(2, 3, ref reStr) == false)
                            {
                                break;
                            }
                            break;
                        }
                       
                        
                      
                        packBarcode = modList[0].batPackID;
                        logRecorder.AddDebugLog(nodeName, string.Format("获取PACK条码:{0}", packBarcode));
                       
                        string qrCodeFile = string.Format(@"\\{0}\加工文件\打码内容.txt", qrCodeIP);
                        //清空文件
                        System.IO.StreamWriter writter = new System.IO.StreamWriter(qrCodeFile, false);
                        StringBuilder strBuild = new StringBuilder();
                        strBuild.Append(packBarcode);
                         
                        writter.Write(strBuild.ToString());
                        writter.Flush();
                        writter.Close();
                    
                       if(NodeDB2Commit(2,2,ref reStr)==false)
                        {
                            break;
                        }
                       //this.logRecorder.AddDebugLog(this.nodeName, "申请成功（MES->PLC）成功！数值已经更改为2");

                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        this.currentTaskDescribe = "打码数据写入成功！";
                        this.TxtLogRecorder.WriteLog("下发打码设备条码成功，条码：" + packBarcode);
                        break;
                    }
                case 3:
                    {
                        #region 原来流程
                        //PACK-模块绑定
                        //List<DBAccess.Model.BatteryModuleModel> modList = modBll.GetModelList(string.Format("palletID='{0}' and palletBinded=1", this.rfidUID));
                        //if (modList != null && modList.Count() > 0)
                        //{
                        //    DBAccess.Model.BatteryPackModel pack = new DBAccess.Model.BatteryPackModel();
                        //    pack.batPackID = packBarcode;
                        //    pack.packAsmTime = System.DateTime.Now;
                        //    pack.bmsID = "";
                        //    pack.opWorkerID = "";
                        //    if(packBll.Exists(packBarcode))
                        //    {
                        //        packBll.Delete(packBarcode);
                        //    }
                        //    packBll.Add(pack);
                        //    foreach (DBAccess.Model.BatteryModuleModel mod in modList)
                        //    {
                        //        mod.batPackID = packBarcode;
                        //        modBll.Update(mod);
                        //        logRecorder.AddDebugLog(nodeName, string.Format("模块{0}绑定到PACK{1}", mod.batModuleID, packBarcode));
                        //    }
                        //}
                        //else
                        //{
                        //    logRecorder.AddDebugLog(nodeName, "工装板：" + this.rfidUID + "无绑定模块数据");
                        //}
                        //currentTaskPhase++;
                        //this.currentTask.TaskPhase = this.currentTaskPhase;
                        //this.ctlTaskBll.Update(this.currentTask);
                        //break;
                        #endregion
                        this.currentTaskDescribe = "开始扫码！";
                        Console.WriteLine("900");
                        if(this.db2Vals[1]!= 2)
                        {
                            break;
                        }
                        bool packLabelStatus = true;
                        string packBarcodeNew = "";
                        if (SysCfgModel.SimMode)
                        {
                            packBarcodeNew = SimBarcode;
                        }
                        else
                        {
                            Console.WriteLine("901");
                            packBarcodeNew = barcodeRW.ReadBarcode();
                            Console.WriteLine("902");
                        }

                        if (string.IsNullOrWhiteSpace(packBarcodeNew) && this.readPackLabelCodeFailTimes<6)//读取6次不成功就任务失败
                        {
                            this.readPackLabelCodeFailTimes++;
                            //packLabelStatus = false;
                            Console.WriteLine(this.nodeName +":扫码数据为空！扫码失败！"+ "已经第"+ this.readPackLabelCodeFailTimes+"次扫码失败！");
                            break;
                        }
                        Console.WriteLine("903");
                        #region 如果所发下去的码和打完之后的码不匹配就要重新发送

                        if (this.packBarcode != packBarcodeNew && this.barcodeFailCounter < 2&&this.readPackLabelCodeFailTimes<6)//如果不相等两次启动打码
                        {
                            //packLabelStatus = false;
                            this.barcodeFailCounter++;
                            this.db1ValsToSnd[2] = 3;

                            Console.WriteLine("904");
                            string qrCodeFile = string.Format(@"\\{0}\加工文件\打码内容.txt", qrCodeIP);
                            //清空文件
                            System.IO.StreamWriter writter = new System.IO.StreamWriter(qrCodeFile, false);
                            StringBuilder strBuild = new StringBuilder();
                            strBuild.Append(packBarcode);
                            Console.WriteLine("905");
                            writter.Write(strBuild.ToString());
                            writter.Flush();
                            writter.Close();
                            logRecorder.AddDebugLog(nodeName, "第" + this.barcodeFailCounter + "次写入打码机数据成功：" + packBarcode);
                            if (NodeDB2Commit(2, 2, ref reStr) == false)
                            {
                                break;
                            }
                            Console.WriteLine("906");
                            break;
                        }
                       
                        if(packBarcode ==packBarcodeNew )
                        {
                            packLabelStatus = true;
                        }
                        else
                        {
                            packLabelStatus = false;
                        }

                        string itemValue = "";
                        if(packLabelStatus == true)
                        {
                            itemValue = "扫码结果:OK:";
                        }
                        else
                        {
                            itemValue = "扫码结果:NG:";
                        }
                        Console.WriteLine("908");
                        #endregion
                        Console.WriteLine("920");
                        this.db1ValsToSnd[2] = 2;
                        Console.WriteLine("921");
                        int handleStatus = 0;
                        Console.WriteLine("922");
                        if (this.plcRW2.ReadDB("D9200", ref handleStatus) == false)//NG处理
                        {
                            Console.WriteLine("923");
                            break;
                        }
                        Console.WriteLine("909");
                        if (handleStatus == 1)//NG
                        {
                            UpdatePalletCheckResult(2);
                            db1ValsToSnd[0] = 5;
                            //db1ValsToSnd[1] = 3;
                            currentTaskPhase = 4;//流程完成
                            break;
                        }
                        Console.WriteLine("910");
                        RootObject obj = DevDataUpload(1, "", "M00100301", this.packBarcode, "", this.rfidUID, "", itemValue, ref reStr);
                        Console.WriteLine("901");
                        this.TxtLogRecorder.WriteLog("上传MES数据，工作中心号：M00100301，工装板号：" + this.rfidUID + ",数据：" + itemValue);
                        if (obj.RES.ToUpper().Contains("OK") == true)
                        {
                            this.logRecorder.AddDebugLog(this.nodeName, "绑定上传MES成功！" + obj.RES);
                        }
                        else if(obj.RES.ToUpper().Contains("NG") == true)
                        {
                            this.logRecorder.AddDebugLog(this.nodeName, "绑定上传MES成功，返回NG！" +obj.RES);
                            db1ValsToSnd[0] = 5;
                            Console.WriteLine("912");
                            currentTaskPhase = 4;//流程完成

                            UpdatePalletCheckResult(2);
                            this.logRecorder.AddDebugLog(this.nodeName, "MES分析数据NG，线体服务器处理流程结束！");
                            break;

                        }
                        else
                        {
                            Console.WriteLine(this.nodeName + "绑定上传MES失败：" + obj.RES);
                            break;
                            
                        }
                        this.currentTaskPhase++;
                        break;
                    }
                case 4:
                    {
                      
                        db1ValsToSnd[1] = 3;
                        currentTaskDescribe = "流程完成";
                        //UploadDataToMes(1, "M00100301", this.rfidUID, "OK");
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.currentTask.TaskStatus = EnumTaskStatus.已完成.ToString();
                        this.ctlTaskBll.Update(this.currentTask);
                        this.TxtLogRecorder.WriteLog("工位流程处理完成！");

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
        private bool UploadDataToMes(int flag, string workStaionSn, string rfid,string result)
        {
            string M_AREA = "Y001";
            string M_WORKSTATION_SN = workStaionSn;
            string M_DEVICE_SN = "";

            string M_UNION_SN = "";
            string M_CONTAINER_SN = "";
            string M_LEVEL = "";
            string M_ITEMVALUE = "扫码结果:"+result;
            RootObject rObj = new RootObject();
            List<DBAccess.Model.BatteryModuleModel> modelList = modBll.GetModelList(string.Format("palletID='{0}' and palletBinded=1", rfid)); //modBll.GetModelByPalletID(this.rfidUID, this.nodeName);
            if (modelList == null || modelList.Count == 0)
            {
                return false;
            }
            string barcode = modelList[0].batModuleID;
            string strJson = "";

            rObj = DevDataUpload(flag, M_DEVICE_SN, M_WORKSTATION_SN, barcode, M_UNION_SN, M_CONTAINER_SN, M_LEVEL, M_ITEMVALUE, ref strJson);
            if (rObj.RES.Contains("OK"))
            {
                return true;
            }
            else
            {
                Console.WriteLine(this.nodeName + "上传MES二维码信息错误：" + rObj.RES);
                return false;
            }
        }
    }
}
