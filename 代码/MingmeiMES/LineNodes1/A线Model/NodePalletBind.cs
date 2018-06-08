using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Threading;
using DevInterface;
using DevAccess;
using PLProcessModel;
using FTDataAccess.BLL;
using FTDataAccess.Model;
namespace LineNodes
{
    public class NodePalletBind : CtlNodeBaseModel
    {
        //public IPlcRW plcRW2 = null;//跟锦帛方PLC接口
        //public int plcID2 = 0;
        private int bindModeCt = 0;//当前绑定的模块数量
        public override bool BuildCfg(System.Xml.Linq.XElement xe, ref string reStr)
        {
            if (!base.BuildCfg(xe, ref reStr))
            {
                return false;
            }
           
            if(this.nodeID=="OPA004")
            {
                this.dicCommuDataDB1[1].DataDescription = "A通道读卡结果，1:rfid复位，2：RFID成功，3：读RFID失败";
                this.dicCommuDataDB1[2].DataDescription = "B通道读卡结果，1:rfid复位，2：RFID成功，3：读RFID失败";
                this.dicCommuDataDB1[3].DataDescription = "工装板绑定模块计数";
                this.dicCommuDataDB1[4].DataDescription = "1：等待2：二维码和档位及位置信息绑定完成";
                this.dicCommuDataDB1[5].DataDescription = "A道最后绑定完成信号1：等待 2：完成";
                this.dicCommuDataDB1[6].DataDescription = "B道最后绑定完成信号1：等待 2：完成";
           
                this.dicCommuDataDB2[1].DataDescription = "正在运行通道，0：无,1：A通道,2：B通道";
                this.dicCommuDataDB2[2].DataDescription = "A通道，1：无板,2：有板，读卡请求";
                this.dicCommuDataDB2[3].DataDescription = "B通道，1：无板,2：有板，读卡请求";

                this.dicCommuDataDB2[4].DataDescription = "1：PLC请求MES读取(PLC写入),2：MES读取完成(MES写入）";
                this.dicCommuDataDB2[5].DataDescription = "二维码读取状态,1：正常2：失败";
                //this.dicCommuDataDB2[6].DataDescription = "档位";
                //this.dicCommuDataDB2[7].DataDescription = "支架二维码(分配30个字）";
                //mod 20180320
                this.dicCommuDataDB2[6].DataDescription = "档位读取状态,1：正常2：失败";
                this.dicCommuDataDB2[7].DataDescription = "电级性检测，1:OK,2:NG, 0：mes读数据完成（MES写）";
                this.dicCommuDataDB2[8].DataDescription = "A道工装板上有几个电池包1:一个2两个";
                this.dicCommuDataDB2[9].DataDescription = "B道工装板上有几个电池包1:一个2两个";
                this.dicCommuDataDB2[10].DataDescription = "A道工装板上取的第几个电池包电池包1:第一个 2:第二个";
                this.dicCommuDataDB2[11].DataDescription = "B道工装板上取的第几个电池包电池包1:第一个 2:第二个";
                this.dicCommuDataDB2[12].DataDescription = "A道绑定的第一个位置电池包放回工装板上  1：电池包放回工装板上（plc写入）2：MES确认后返回（mes写入）";
                this.dicCommuDataDB2[13].DataDescription = "A道绑定的第二个位置电池包放回工装板上  1：电池包放回工装板上（plc写入）2：MES确认后返回（mes写入）";
                this.dicCommuDataDB2[14].DataDescription = "B道绑定的第一个位置电池包放回工装板上  1：电池包放回工装板上（plc写入）2：MES确认后返回（mes写入）";
                this.dicCommuDataDB2[15].DataDescription = "B道绑定的第二个位置电池包放回工装板上  1：电池包放回工装板上（plc写入）2：MES确认后返回（mes写入）";
                this.dicCommuDataDB2[16].DataDescription = "档位1字节";
                this.dicCommuDataDB2[17].DataDescription = "档位2字节";
                this.dicCommuDataDB2[18].DataDescription = "档位3字节";
                this.dicCommuDataDB2[19].DataDescription = "档位4字节";
                this.dicCommuDataDB2[20].DataDescription = "档位5字节";
                this.dicCommuDataDB2[21].DataDescription = "档位6字节";
                this.dicCommuDataDB2[22].DataDescription = "档位7字节";
                this.dicCommuDataDB2[23].DataDescription = "档位8字节";
                this.dicCommuDataDB2[24].DataDescription = "档位9字节";
                this.dicCommuDataDB2[25].DataDescription = "档位10字节";
                this.dicCommuDataDB2[26].DataDescription = "支架二维码(分配30个字）";

            }
            else if(this.nodeID=="OPB001")
            {
                //XElement baseDataXE = xe.Element("BaseDatainfo");
                //XAttribute attr = baseDataXE.Attribute("plcID2");
                //if (attr != null)
                //{
                //    this.plcID2 = int.Parse(attr.Value);
                //}
                this.dicCommuDataDB1[1].DataDescription = "1:rfid复位，2：RFID成功，3：读RFID失败";
                this.dicCommuDataDB1[2].DataDescription = "工装板绑定模块计数";
                this.dicCommuDataDB1[3].DataDescription = "1：等待2：绑定完成，放行，3：不可识别的条码";
                this.dicCommuDataDB2[1].DataDescription = "1：无板,2：有板，读卡请求";
                this.dicCommuDataDB2[2].DataDescription = "1：PLC请求MES读取(PLC写入),2：MES读取完成(MES写入）";
                this.dicCommuDataDB2[3].DataDescription = "二维码读取状态,1：正常2：失败,3:条码重复绑定";
                //this.dicCommuDataDB2[4].DataDescription = "支架二维码(分配30个字）";
            }
            return true;
        }
        //public override bool ReadDB2(ref string reStr)
        //{
        //    if(this.nodeID == "OPB001")
        //    {
        //        if(!base.ReadDB2(ref reStr))
        //        {
        //            return false;
        //        }
        //        if(!SysCfgModel.SimMode && plcRW2 != null)
        //        {
        //            short[] vals = null;
        //            //同步通信
        //            if (!plcRW2.ReadMultiDB(db2StartAddr+1,1, ref vals))
        //            {
        //                // refreshStatusOK = false;
        //                ThrowErrorStat(this.nodeName + (plcRW2 as OmlPlcRW).PlcRole+",读PLC数据(DB2）失败,", EnumNodeStatus.设备故障);
        //                // logRecorder.AddDebugLog(this.nodeName, "读PLC数据(DB2）失败");
        //                return false;
        //            }
        //            this.dicCommuDataDB2[2].Val = vals[0];
                   
        //        }
        //        return true;
        //    }
        //    else
        //    {
        //        return base.ReadDB2(ref reStr);
        //    }
           
        //}
        //public override bool NodeDB2Commit(int addrIndex, short val, ref string reStr)
        //{
        //    if (this.nodeID == "OPB001")
        //    {
        //        int commID = addrIndex + 1;
        //        PLCDataDef commObj = dicCommuDataDB2[commID];
        //        if (!SysCfgModel.SimMode)
        //        {
        //            return plcRW2.WriteDB(commObj.DataAddr, val);
        //        }
        //        else
        //        {

        //            commObj.Val = val;
        //        }
        //        return true;
        //    }
        //    else
        //    {
        //        return base.NodeDB2Commit(addrIndex, val, ref reStr);
        //    }
            
        //}
        public override bool ExeBusiness(ref string reStr)
        {
            if (!nodeEnabled)
            {
                return true;
            }
            if(this.nodeID=="OPA004")
            {
                return ExeBindALine(ref reStr);
            }
            else if(this.nodeID=="OPB001")
            {
                return ExeBindBLine(ref reStr);
            }
           
            
            return true;
        }
        protected bool ExeBindALine(ref string reStr)
        {
            
            
            if(!ExeBusinessAB(ref reStr))
            {
                return false;
            }
           
            switch(currentTaskPhase)
            {
                case 1:
                    {
                       // Console.WriteLine("currentTaskPhase:" + 1);
                        currentTaskDescribe = "开始读RFID";
                        if(!RfidReadAB())
                        {
                            break;
                        }
                        //db1ValsToSnd[0] = 2;//读到RFID
                        db1ValsToSnd[2] = 0;//绑定计数
                        db1ValsToSnd[3] = 1;//复位
                        db1ValsToSnd[4] = 1;//复位
                        db1ValsToSnd[5] = 1;//复位
                     
                        this.currentTask.TaskParam = rfidUID;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);

                        bindModeCt = 0;
                        this.currentStat.Status = EnumNodeStatus.设备使用中;
                        currentTaskPhase++;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);

                        if (!TryUnbind(rfidUID, ref reStr))
                        {
                            logRecorder.AddDebugLog(nodeName, "解绑RFID:" + rfidUID + "失败," + reStr);
                            break;
                        }
                        logRecorder.AddDebugLog(nodeName, string.Format("读到RFID:{0}，开始绑定",this.rfidUID));
                        this.bindModeCt = modBll.GetRecordCount(string.Format("palletID='{0}' and palletBinded=1", this.rfidUID));
                        this.db1ValsToSnd[2] = (short)this.bindModeCt;
                        break;
                    }
                case 2:
                    {
                        //先数据绑定
                        if (bindModeCt > 0)  //判断是否都绑定完成
                        {
                        //    Console.WriteLine("bindModeCt:" + bindModeCt);
                            if ((db2Vals[0] == 1 && bindModeCt == db2Vals[7] ) || (db2Vals[0] == 2 && bindModeCt == db2Vals[8]))
                            {
                                currentTaskPhase++;
                                this.currentTask.TaskPhase = this.currentTaskPhase;
                                this.ctlTaskBll.Update(this.currentTask);
                                break;
                            }
                        }
                       if(!BindBatteryLoop(ref reStr))
                       {
                           break;
                       }
                       break;
                    }
                case 3:
                    {
                        //二维码和档位及位置信息绑定完成
                        this.db1ValsToSnd[3] = 2;
                        currentTaskPhase++;
                        
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.currentTask.TaskStatus = EnumTaskStatus.已完成.ToString();
                        this.ctlTaskBll.Update(this.currentTask);
                        break;
                    }
                case 4:
                    {
                        currentTaskDescribe = "模组-工装板数据绑定完成，下一步:极性检测";
                        break;
                    }
                //case 3:
                //    {
                //        this.currentTaskDescribe = "开始上传MES数据";
                //        // 1 上传档位，电压，容量，内阻

                //        //2 上传极性检测结果
                //        if(!MESDataUnload(ref reStr))
                //        {
                //            break;
                //        }
                //        this.db1ValsToSnd[3 + channelIndex] = 2;
                //        currentTaskPhase++;
                //        this.currentTask.TaskPhase = this.currentTaskPhase;
                //        this.ctlTaskBll.Update(this.currentTask);
                //        break;
                //    }
                default:
                    break;
            }
            if(!BindCheckLoopA(ref reStr))
            {
                Console.WriteLine(reStr);
            }
            if(!BindCheckLoopB(ref reStr))
            {
                Console.WriteLine(reStr);
            }
            return true;
        }
        private bool BindBatteryLoop(ref string reStr)
        {
            try
            {
                
                if (db2Vals[3] != 1)
                {
                    return false;
                }
                
                //PLC请求读支架二维码和分档信息
                string modBarcode = "";
                string modGrade = "";
               
                List<byte> modGradeBytes = new List<byte>();
                for (int j = 0; j < 9; j++)//原来是10，现在改为9，后一个数为数字分档
                {
                    int indexSt = 15 + j;
                    modGradeBytes.Add((byte)(this.db2Vals[indexSt] & 0xff));
                    modGradeBytes.Add((byte)((this.db2Vals[indexSt] >> 8) & 0xff));
                }
                short modGradeNum = this.db2Vals[15 + 9];//分档数字
                //字节流转换成字符串
                modGrade = System.Text.ASCIIEncoding.UTF8.GetString(modGradeBytes.ToArray()).Trim(new char[] { '\0', '\r', '\n', '\t', ' ' });
                //      Console.WriteLine("modGrade:" + modGrade);
                //判断二维码是否读取成功
                if (string.IsNullOrWhiteSpace(modGrade))
                {
                    //   Console.WriteLine("  db2Vals[5] = 2;");
                    db2Vals[5] = 2;
                    if (!NodeDB2Commit(5, this.db2Vals[5], ref reStr))
                    {
                        logRecorder.AddDebugLog(nodeName, "发送PLC数据失败，db2Vals[5]" + reStr);
                        return false;
                    }
                    return false;
                }
                else
                {
                    //    Console.WriteLine("  db2Vals[5] = 1;");
                    db2Vals[5] = 1;
                    if (!NodeDB2Commit(5, this.db2Vals[5], ref reStr))
                    {
                        logRecorder.AddDebugLog(nodeName, "发送PLC数据失败，db2Vals[5]" + reStr);
                        return false;
                    }
                }
                //add over
                if (SysCfgModel.SimMode)
                {
                    //生成模拟数据
                    //GenerateSimBatterys();
                    modBarcode = SimBarcode;
                }
                else
                {
                    List<byte> batteryBytes = new List<byte>();
                    for (int j = 0; j < 30; j++)
                    {
                        //  int indexSt = 6 + j;
                        //mod 20180320
                        int indexSt = 25 + j;
                        batteryBytes.Add((byte)(this.db2Vals[indexSt] & 0xff));
                        batteryBytes.Add((byte)((this.db2Vals[indexSt] >> 8) & 0xff));
                    }
                    //字节流转换成字符串
                    modBarcode = System.Text.ASCIIEncoding.UTF8.GetString(batteryBytes.ToArray());
                }

                modBarcode = modBarcode.Trim(new char[] { '\0', '\r', '\n', '\t', ' ' }).ToUpper();
                Console.WriteLine("modBarcode:" + modBarcode);
                //判断二维码是否读取成功
                if (string.IsNullOrWhiteSpace(modBarcode))
                {
                    // Console.WriteLine("db2Vals[4] = 2;");
                    db2Vals[4] = 2;
                    if (!NodeDB2Commit(4, this.db2Vals[4], ref reStr))
                    {
                        logRecorder.AddDebugLog(nodeName, "发送PLC数据失败，db2Vals[4]" + reStr);
                        return false;
                    }
                    return false;
                }
                else
                {
                    //  Console.WriteLine("db2Vals[4] = 1;");
                    db2Vals[4] = 1;
                    if (!NodeDB2Commit(4, this.db2Vals[4], ref reStr))
                    {
                        logRecorder.AddDebugLog(nodeName, "发送PLC数据失败，db2Vals[4]" + reStr);
                        return false;
                    }
                }

                DBAccess.Model.BatteryModuleModel mod = null;
                if (modBll.Exists(modBarcode))
                {
                    //Console.WriteLine("modBll.Exists");
                    modBll.Delete(modBarcode);
                    this.bindModeCt = modBll.GetRecordCount(string.Format("palletID='{0}' and palletBinded=1", this.rfidUID));
                }
                mod = new DBAccess.Model.BatteryModuleModel();
                mod.batModuleID = modBarcode;
                mod.palletID = this.rfidUID;
                mod.palletBinded = true;

                // mod.checkResult = 1;
                mod.batPackID = "";
                mod.curProcessStage = nodeName;
                mod.asmTime = System.DateTime.Now;
                mod.tag1 = modGrade.ToString();
                mod.tag5 = modGradeNum.ToString();//分档数字放到tag5中np_added
                this.logRecorder.AddDebugLog(this.nodeName, "位置8：" + this.db2Vals[8] + "位置9：" + this.db2Vals[9] + "位置10：" + this.db2Vals[10]);
                   
                if (this.db2Vals[0] == 1) //A通道
                {                 
                    mod.tag2 = this.db2Vals[9].ToString();
                }
                else if (this.db2Vals[0] == 2) //B通道
                {                
                    mod.tag2 = this.db2Vals[10].ToString();
                }
                //if(this.db2Vals[6] == 1)
                //{
                //    mod.checkResult = 1;
                //}
                //else if(this.db2Vals[6] == 2)
                //{
                //    mod.checkResult = 2;
                //}
                // mod.tag2 = (bindModeCt+1).ToString();
                modBll.Add(mod);
                //Console.WriteLine("modBll.Add(mod)");
                logRecorder.AddDebugLog(nodeName, string.Format("绑定，RFID:{0},模块条码:{1},档位:{2},极性检测：{3}", rfidUID, modBarcode,modGrade,mod.checkResult));
                AddProcessRecord(mod.batModuleID, "模块", "追溯记录", string.Format("绑定，RFID:{0},模块条码:{1}", rfidUID, modBarcode), "");
                //ProductTraceRecord();
                if (!NodeDB2Commit(6, 0, ref reStr)) //极性检测复位
                {
                    return false;
                }
                if (!NodeDB2Commit(3, 2, ref reStr)) //MES读取完成
                {
                    logRecorder.AddDebugLog(nodeName, "发送PLC数据失败，" + reStr);
                    return false;
                }
                //  Console.WriteLine("db2Vals[3] = 2");
                this.bindModeCt = modBll.GetRecordCount(string.Format("palletID='{0}' and curProcessStage = '{1}' and palletBinded=1", this.rfidUID, this.nodeName));
                this.db1ValsToSnd[2] = (short)this.bindModeCt;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
          
        }
      
        /// <summary>
        ///A通道对应绑定的模组极性检测判断
        /// </summary>
        /// <param name="reStr"></param>
        /// <returns></returns>
        private bool BindCheckLoopA(ref string reStr)
        {
            if(this.db1ValsToSnd[4] ==2) //已经检测完成
            {
                return true;
            }
            string tempRfidUID = this.rfidUIDA;
            int requiredBindCt = Math.Max(2,(int)db2Vals[7]); //A通道要求绑定数量
            bool checkOK = true;
            int posBase = 10;
            for(int i=1;i<requiredBindCt+1;i++)
            {
                DBAccess.Model.BatteryModuleModel model = modBll.GetModelByPalletIDAndTag2(tempRfidUID, i.ToString(), this.nodeName); ;
                if(model == null)
                {
                    continue;
                }
                if (this.db2Vals[posBase + i] == 1)
                {
                    model.checkResult = this.db1ValsToSnd[6];
                    if(!modBll.Update(model))
                    {
                        reStr = "模组极性检测状态提交到数据库失败";
                        return false;
                    }
                    logRecorder.AddDebugLog(nodeName,string.Format("工装板{0} ,位置{1} 模组{2} 极性检测结果{3}保存，给PLC应答2",tempRfidUID,i,model.batModuleID,model.checkResult));
                    if (!NodeDB2Commit(posBase + i, 2, ref reStr))
                    {
                        return false;
                    }
                }
            }
            List<DBAccess.Model.BatteryModuleModel> modelList = modBll.GetBindedMods(tempRfidUID);//modBll.GetModelByPalletID(tempRfidUID, this.nodeName);
            if (modelList == null || modelList.Count == 0)
            {
                return true;
            }
            for (int i = 0; i < modelList.Count; i++)
            {
                if (modelList[i].checkResult == null)
                {
                    checkOK = false;
                }
            }
            if(checkOK)
            {
                //提交数据
                if (MESDataUnload(ref reStr))
                {
                    this.db1ValsToSnd[4] = 2;
                }
                else
                {
                    return false;
                }

            }
            return true;
        }
        /// <summary>
        ///B通道对应绑定的模组极性检测判断
        /// </summary>
        /// <param name="reStr"></param>
        /// <returns></returns>
        private bool BindCheckLoopB(ref string reStr)
        {
            if (this.db1ValsToSnd[5] == 2) //已经检测完成
            {
                return true;
            }
            string tempRfidUID =   this.rfidUIDB;
            int requiredBindCt = Math.Max(2, (int)db2Vals[8]); //B通道要求绑定数量
            bool checkOK = true;
            int posBase = 12;
            for (int i = 1; i < requiredBindCt + 1; i++)
            {
                DBAccess.Model.BatteryModuleModel model = modBll.GetModelByPalletIDAndTag2(tempRfidUID, i.ToString(), this.nodeName); ;
                if (model == null)
                {
                    continue;
                }
                if (this.db2Vals[posBase + i] == 1)
                {
                    model.checkResult = this.db1ValsToSnd[6];
                    if (!modBll.Update(model))
                    {
                        reStr = "模组极性检测状态提交到数据库失败";
                        return false;
                    }
                    if (!NodeDB2Commit(posBase + i, 2, ref reStr))
                    {
                        return false;
                    }
                }
            }
            List<DBAccess.Model.BatteryModuleModel> modelList = modBll.GetBindedMods(tempRfidUID);//modBll.GetModelByPalletID(tempRfidUID, this.nodeName);
            if (modelList == null || modelList.Count == 0)
            {
                return true;
            }
            for (int i = 0; i < modelList.Count; i++)
            {
                if (modelList[i].checkResult == null)
                {
                    checkOK = false;
                }
            }
            if (checkOK)
            {
                //提交数据
                if (MESDataUnload(ref reStr))
                {
                    this.db1ValsToSnd[5] = 2;
                }
                else
                {
                    return false;
                }
               
            }
            return true;
            
        }
        protected bool BindCheckResult(ref string reStr)
        {
            string tempRfidUID = "";
            //绑定极性检测结果
            int index = 0;
            if(db2Vals[11] == 1)
            {
                index = 11;
            }
            else if (db2Vals[12] == 1)
            {
                index = 12;
            }
            else if (db2Vals[13] == 1)
            {
                index = 13;
            }
            else if (db2Vals[14] == 1)
            {
                index = 14;
            }
            if(index == 0)
            {
                return false;
            }
            if (SysCfgModel.SimMode)
            {
                tempRfidUID = SimRfidUID;
            }
            else
            {
                if (index == 11 || index == 12)
                {
                   // Console.WriteLine("this.rfidUID = this.rfidUIDA;");
                    tempRfidUID = this.rfidUIDA;
                }
                else if (index == 13 || index == 14)
                {
                    //Console.WriteLine("this.rfidUID = this.rfidUIDB;");
                    tempRfidUID = this.rfidUIDB;
                }
                
            }
                    
            DBAccess.Model.BatteryModuleModel model = null;
            if (index == 11 || index == 13)
            {
                model = modBll.GetModelByPalletIDAndTag2(tempRfidUID, "1", this.nodeName);
            }
            else
            {
                model = modBll.GetModelByPalletIDAndTag2(tempRfidUID, "2", this.nodeName);
            }
            if (model == null)
            {
                return false;
            }
            //Console.WriteLine("model != null" + model.batModuleID);
            model.checkResult = this.db2Vals[6];
            bool re = modBll.Update(model);
            if(re == true)
            {
                db2Vals[index] = 2;
              //  Console.WriteLine("db2Vals[index] = 2;");
                if (!NodeDB2Commit(index, this.db2Vals[index], ref reStr))
                {
                    logRecorder.AddDebugLog(nodeName, "发送PLC数据失败， db2Vals[11]" + reStr);
                    return false;
                }
                db2Vals[6] = 0;
                if (!NodeDB2Commit(6, this.db2Vals[6], ref reStr))
                {
                    logRecorder.AddDebugLog(nodeName, "发送PLC数据失败， db2Vals[6]" + reStr);
                    return false;
                }
                //判断绑定极性结果是否完成
                List<DBAccess.Model.BatteryModuleModel> modelList = modBll.GetBindedMods(tempRfidUID);//modBll.GetModelByPalletID(tempRfidUID, this.nodeName);
                if (modelList == null || modelList.Count == 0)
                {
                    return false;
                }
               // Console.WriteLine("modelList != null");
                bool isAllBindCheckResult = true;
                for (int i = 0; i < modelList.Count; i++)
                {
                    if (modelList[i].checkResult == null)
                    {
                        isAllBindCheckResult = false;
                    }
                }
                if (isAllBindCheckResult == true)
                {
                    if (this.rfidUID == this.rfidUIDA)
                    {
                        this.db1ValsToSnd[4] = 2;
                        this.db1ValsToSnd[0] = 0;

                    }
                    if (this.rfidUID == this.rfidUIDB)
                    {
                        this.db1ValsToSnd[5] = 2;
                        this.db1ValsToSnd[1] = 0;
                    }
                }
             //   Console.WriteLine("isAllBindCheckResult = false");
                return true;
            }
            return false;
        }

        protected bool MESDataUnload(ref string reStr)
        {
            if(isWithMes == false)
            {
                return true;
            }
            //判断绑定极性结果是否完成
            List<DBAccess.Model.BatteryModuleModel> modelList = modBll.GetModelList(string.Format("palletID='{0}' and palletBinded=1", this.rfidUID)); //modBll.GetModelByPalletID(this.rfidUID, this.nodeName);
            if (modelList == null || modelList.Count == 0)
            {
                return false;
            }
            //bool isAllBindCheckResult = true;
            //for (int i = 0; i < modelList.Count; i++)
            //{
            //    if (modelList[i].checkResult == null)
            //    {
            //        isAllBindCheckResult = false;
            //    }
            //}
            //if (isAllBindCheckResult == false)
            //{
            //    return false;
            //}
          
            for (int i = 0; i < modelList.Count; i++)
            {
                //1 极性检测上传
               
              //  int M_FLAG = 3;
                string M_DEVICE_SN = "";
                string M_SN = modelList[i].batModuleID;
                string M_UNION_SN = "";
                string M_CONTAINER_SN = "";
                string M_LEVEL = "";
                string M_ITEMVALUE = "";
                if (modelList[i].checkResult == 2)
                {
                    M_ITEMVALUE = "极性检测结果:" + "NG:";
                }
                else
                {
                    M_ITEMVALUE = "极性检测结果:" + "OK:";
                }
                RootObject rObj = new RootObject();
                string strJson = "";
                
                //1 档位上传
                string  M_WORKSTATION_SN = "Y00100301";
                //M_LEVEL = "档位:" + modelList[i].tag1 + ":";
                M_LEVEL = modelList[i].tag1; //档位直接填值
              
                rObj = DevDataUpload(1, M_DEVICE_SN, M_WORKSTATION_SN, M_SN, M_UNION_SN, M_CONTAINER_SN, M_LEVEL, "", ref strJson);
                
                logRecorder.AddDebugLog(nodeName, string.Format("模组{0} 档位{1}上传MES，返回{2},发送json{3}", M_SN, M_LEVEL, rObj.RES,strJson));
               
                this.currentTaskDescribe = string.Format("模组{0}档位{1}上传MES，返回{2}", M_SN, M_ITEMVALUE, rObj.RES);
                if(rObj.RES.ToUpper() == "OK")
                {
                  
                    Thread.Sleep(300);
                    //2 传数据
                    M_WORKSTATION_SN = "Y00100401";
                    rObj = DevDataUpload(3, M_DEVICE_SN, M_WORKSTATION_SN, M_SN, M_UNION_SN, M_CONTAINER_SN, "", M_ITEMVALUE, ref strJson);
                    logRecorder.AddDebugLog(nodeName, string.Format("模组{0} 极性检测{1}上传MES，返回{2},发送json{3}", M_SN, M_ITEMVALUE, rObj.RES, strJson));
                    this.currentTaskDescribe = string.Format("模组{0}极性检测{1}上传MES，返回{2}", M_SN, M_ITEMVALUE, rObj.RES);
                }
              
                //if (rObj.CONTROL_TYPE == "STOP" && rObj.RES == "OK")
                //{
                //    Console.WriteLine(this.nodeName + "CONTROL_TYPE = STOP");
                //}
                //currentTaskDescribe = this.nodeName + rObj.CONTROL_TYPE + "," + M_SN;
                      
            }
            return true;
        }
        protected bool ExeBindBLine(ref string reStr)
        {
            if (!devStatusRestore)
            {
                devStatusRestore = DevStatusRestore();

            }
            if (!devStatusRestore)
            {
                return false;
            }
            this.rfidRW = this.rfidRWList[0];
            if (db2Vals[0] == 1)
            {
                if (this.currentTask != null)
                {
                    //查询未执行完任务，清掉
                    if (!ctlTaskBll.ClearTask(string.Format("DeviceID='{0}'", this.nodeID)))
                    {
                        logRecorder.AddDebugLog(nodeName, "清理任务失败");
                        return false;
                    }
                    this.currentTask = null;
                    this.currentStat.Status = EnumNodeStatus.设备空闲;
                    this.currentStat.ProductBarcode = "";
                    this.currentStat.StatDescribe = "设备空闲";
                    checkFinished = false;
                    currentTaskDescribe = "等待有板信号";
                }

                currentTaskPhase = 0;
                db1ValsToSnd[0] = 1;
                db1ValsToSnd[1] = 0;
                db1ValsToSnd[2] = 0;
                this.rfidUID = string.Empty;
                //if(!SysCfgModel.SimMode)
                //{
                //    (this.rfidRW as RfidCF).ClearBufUID();
                //}
                this.currentStat.Status = EnumNodeStatus.设备空闲;
                this.currentStat.StatDescribe = "工位空闲";
                currentTaskDescribe = "";
               
            }
            if (db2Vals[0] == 2)
            {
                if (currentTaskPhase == 0)
                {
                    this.db1ValsToSnd[0] = 1;
                    this.db1ValsToSnd[1] = 0;
                    currentTaskPhase = 1;
                    this.currentStat.Status = EnumNodeStatus.设备使用中;

                    this.currentStat.StatDescribe = "工作中";
                    ControlTaskModel task = new ControlTaskModel();
                    task.TaskID = System.Guid.NewGuid().ToString("N");
                    task.TaskParam = string.Empty;
                    task.TaskPhase = 1;
                    task.CreateTime = System.DateTime.Now;
                    task.DeviceID = this.nodeID;
                    task.CreateMode = "自动";
                    task.TaskStatus = EnumTaskStatus.执行中.ToString();
                
                    ctlTaskBll.Add(task);
                    this.currentTask = task;

                }
            }
          
            switch (currentTaskPhase)
            {
                case 1:
                    {
                        currentTaskDescribe = "开始读RFID";
                        if(SysCfgModel.SimMode)
                        {
                            this.rfidUID = SimRfidUID;
                        }
                       else 
                        {
                            if(string.IsNullOrWhiteSpace(rfidUID))
                            {
                                this.rfidUID = this.rfidRW.ReadUID();
                            }
                           
                        }
                        if (string.IsNullOrWhiteSpace(this.rfidUID))
                        {
                            this.currentStat.Status = EnumNodeStatus.无法识别;
                            this.currentStat.StatDescribe = "读RFID失败";
                            this.currentTaskDescribe = "读RFID失败";

                            if (this.db1ValsToSnd[0] != 3)
                            {
                                //logRecorder.AddDebugLog(nodeName, "读RFID失败");
                                db1ValsToSnd[0] = 3;
                            }
                            else
                            {
                                db1ValsToSnd[0] = 1;
                            }
                            Thread.Sleep(1000);
                            this.currentStat.Status = EnumNodeStatus.无法识别;

                            break;
                        }
                       

                        this.currentTask.TaskParam = rfidUID;
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);

                        db1ValsToSnd[0] = 2;//读到RFID
                        db1ValsToSnd[1] = 0;//绑定计数
                        if (!TryUnbind(rfidUID, ref reStr))
                        {
                            logRecorder.AddDebugLog(nodeName, "解绑RFID:" + rfidUID + "失败," + reStr);
                            break;
                        }
                        bindModeCt = 0;
                        this.currentStat.Status = EnumNodeStatus.设备使用中;
                        currentTaskPhase++;
                        logRecorder.AddDebugLog(nodeName, string.Format("读到RFID:{0}，开始绑定", this.rfidUID));
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        this.bindModeCt = modBll.GetRecordCount(string.Format("palletID='{0}' and palletBinded=1", this.rfidUID));
                        this.db1ValsToSnd[1] = (short)this.bindModeCt;
                        break;
                    }
                case 2:
                    {
                       
                        if (bindModeCt > 3)
                        {
                            currentTaskPhase++;
                            this.currentTask.TaskPhase = this.currentTaskPhase;
                            this.ctlTaskBll.Update(this.currentTask);
                            break;
                        }
                        if(db2Vals[1] == 0)
                        {
                            db2Vals[2] = 0;
                            if (!NodeDB2Commit(2, this.db2Vals[2], ref reStr))
                            {
                                logRecorder.AddDebugLog(nodeName, "发送PLC数据失败，" + reStr);
                                break;
                            }
                            break;
                        }
                        if (db2Vals[1] == 1)
                        {
                            //PLC请求读支架二维码和分档信息
                            string modBarcode = "";
                            if (SysCfgModel.SimMode)
                            {
                                //生成模拟数据
                               // GenerateSimBatterys();
                                modBarcode = SimBarcode;
                            }
                            else
                            {
                                modBarcode = barcodeRW.ReadBarcode();
                            }
                          
                            modBarcode = modBarcode.Trim(new char[] { '\0', '\r', '\n', '\t', ' ' }).ToUpper();
                            if(string.IsNullOrWhiteSpace(modBarcode))
                            {
                                this.db2Vals[2] = 2;
                                this.currentTaskDescribe = "扫条码失败";
                                if (!NodeDB2Commit(2, this.db2Vals[2], ref reStr))
                                {
                                    logRecorder.AddDebugLog(nodeName, "发送PLC数据失败，" + reStr);
                                    break;
                                }
                                break;
                            }
                            else
                            {
                                this.currentTaskDescribe = "扫到条码："+modBarcode;
                               
                            }
                            DBAccess.Model.BatteryModuleModel mod = modBll.GetModel(modBarcode);
                           
                            if(mod == null)
                            {
                                this.db1ValsToSnd[2] = 3;
                                break;
                            }
                            this.db2Vals[1] = 2;
                            if (!NodeDB2Commit(1, this.db2Vals[1], ref reStr))
                            {
                                logRecorder.AddDebugLog(nodeName, "发送PLC数据失败，" + reStr);
                                break;
                            }
                            if(mod.palletBinded=true && mod.palletID== rfidUID)
                            {
                                logRecorder.AddDebugLog(nodeName, string.Format("模块条码:{0}重复绑定到RFID{1}",modBarcode,rfidUID));
                                this.db2Vals[2] = 3;
                                if (!NodeDB2Commit(2, this.db2Vals[2], ref reStr))
                                {
                                    logRecorder.AddDebugLog(nodeName, "发送PLC数据失败，" + reStr);
                                    break;
                                }
                                break;
                            }
                            else
                            {
                                this.db2Vals[2] = 1;
                                if (!NodeDB2Commit(2, this.db2Vals[2], ref reStr))
                                {
                                    logRecorder.AddDebugLog(nodeName, "发送PLC数据失败，" + reStr);
                                    break;
                                }
                            }
                            mod.palletID = this.rfidUID;
                            mod.palletBinded = true;
                            mod.curProcessStage = nodeName;
                            mod.tag2 = (bindModeCt+1).ToString();
                            modBll.Update(mod);
                            logRecorder.AddDebugLog(nodeName, string.Format("绑定，RFID:{0},模块条码:{1}", rfidUID, modBarcode));
                           // AddProcessRecord(mod.batModuleID, this.nodeName, "");
                            AddProcessRecord(mod.batModuleID, "模块", "追溯记录", string.Format("绑定，RFID:{0},模块条码:{1}", rfidUID, modBarcode), "");
                           
                            
                            this.bindModeCt =  modBll.GetRecordCount(string.Format("palletID='{0}' and palletBinded=1", this.rfidUID));
                            this.db1ValsToSnd[1] = (short)this.bindModeCt;
                        }

                        break;
                    }
                case 3:
                    {
                        List<DBAccess.Model.BatteryModuleModel> modList = modBll.GetModelList(string.Format("palletID='{0}' and palletBinded=1", this.rfidUID));

                        if (!PreMech(modList, ref reStr))
                        {
                            Console.WriteLine(string.Format("{0},{1}", nodeName, reStr));
                            break;
                        }
                        db1ValsToSnd[2] = 2;
                        currentTaskDescribe = "流程完成";
                        this.currentTask.TaskPhase = this.currentTaskPhase;
                        this.ctlTaskBll.Update(this.currentTask);
                        this.currentTask.TaskStatus = EnumTaskStatus.已完成.ToString();
                        break;
                    }
                default:
                    break;
            }
            return true;
        }
        int batterySum = 0;
        private void GenerateSimBatterys()
        {
            // batterys = new List<string>();
               
            if (this.nodeID == "OPA004")
            {
                batterySum = this.modBll.GetRecordCount("");
            }
            else
            {
                if (batterySum > 3)
                {
                    batterySum = 0;
                }
            }
            batterySum++;
            string batteryID = string.Format("012345678901234567890123{0}", batterySum.ToString().PadLeft(6, '0'));
            //  batterys.Add(batteryID);

            byte[] byteArray = System.Text.ASCIIEncoding.UTF8.GetBytes(batteryID);
            Int16[] vals = new Int16[15];
            Array.Clear(vals, 0, 15);
            int blockAlloc = byteArray.Count() / 2;
            for (int blockIndex = 0; blockIndex < blockAlloc; blockIndex++)
            {
                vals[blockIndex] = (Int16)(byteArray[blockIndex * 2] + (byteArray[blockIndex * 2 + 1] << 8));
            }
            if (blockAlloc * 2 < byteArray.Count())
            {
                vals[blockAlloc] = byteArray[byteArray.Count() - 1];
                blockAlloc++;
            }

            int db2St = 5;
            if (this.nodeID == "OPB001")
            {
                db2St = 3;
            }
            Array.Copy(vals, 0, this.db2Vals, db2St, blockAlloc);
               
               
                
            
        }
        protected override bool ChannelReset(int channel)
        {
            if (channel < 1)
            {
                return false;
            }
            if (this.currentTask != null)
            {
                //查询未执行完任务，清掉
                if (!ctlTaskBll.ClearTask(string.Format("DeviceID='{0}'", this.nodeID)))
                {
                    logRecorder.AddDebugLog(nodeName, "清理任务失败");
                    return false;
                }
                this.currentTask = null;
                this.currentStat.Status = EnumNodeStatus.设备空闲;
                this.currentStat.ProductBarcode = "";
                this.currentStat.StatDescribe = "设备空闲";
                checkFinished = false;
                currentTaskDescribe = "等待有板信号";
            }
            currentTaskPhase = 0;
            db1ValsToSnd[channel - 1] = 1;
            db1ValsToSnd[2] = 0;
            db1ValsToSnd[3] = 1;

            this.rfidUID = string.Empty;
            this.currentStat.Status = EnumNodeStatus.设备空闲;
            this.currentStat.StatDescribe = "工位空闲";
            currentTaskDescribe = "";
            //if (channel == 1)
            //{
            //    this.rfidUIDA = "";
            //}
            //else
            //{
            //    this.rfidUIDB = "";
            //}
            return true;
        }
    }
}
