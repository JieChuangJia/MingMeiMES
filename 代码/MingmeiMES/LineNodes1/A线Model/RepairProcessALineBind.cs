using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PLProcessModel;
using DBAccess.Model;

namespace LineNodes
{
   public class RepairProcessALineBind:RepairProcessBase
    {
       //private DBAccess.BLL.RepairProcessBLL bllRepairProcess = new DBAccess.BLL.RepairProcessBLL();
       //private DBAccess.BLL.RepairRecordBLL bllRepairRecord = new DBAccess.BLL.RepairRecordBLL();
       //private DBAccess.BLL.BatteryModuleBll modBll = new DBAccess.BLL.BatteryModuleBll();

       private string ProductCateAddr = "D3055";//1为正常产品，2为NG返修产品
       private string ProcessMatchAddr = "D3056";//流程是否匹配，1位不匹配，2是等待设备设置
       private string AtALineBindAddr="D3057";//是否在A线绑定工位，1是，2否
       private string ALineWorkStationNum = "Y00100301";
       private string ALineBindProcessNum = "A1";
       //private int stepIndex = 0;
    

   
       public RepairProcessALineBind(CtlNodeBaseModel nodeBase)
           : base(nodeBase)
       {
           this.nodeBase.repairProcess = this;
           stepIndex = 1;
       }
     

       /// <summary>
       /// 是否需要返修，只有AB线绑定工位是通过PLC地址判断其他工位只读取数据库判断
       /// </summary>
       /// <param name="moduleID">模块ID</param>
       /// <returns></returns>
       public  bool GetNeedRepair( ref bool needRepair, ref string restr)
       {
           int productCate = 1;
           this.NeedRepair = needRepair;
           if (this.nodeBase == null)
           {
               Console.WriteLine("nodebase = null");
           }
           if(this.nodeBase.PlcRW == null)
           {
               Console.WriteLine("plcrw = null");
           }
           if(this.nodeBase.PlcRW.ReadDB(this.ProductCateAddr,ref productCate)==false)
           {
               restr = "读取PLC地址：" + this.ProductCateAddr+"失败！";
               return false;
           }

           if (productCate == 2)
           {
              needRepair = true;
           }
           else
           {
               needRepair = false;
               this.DeleteRepairRecord();//正常模式要把记录删除掉
           }
           this.NeedRepair = needRepair;
           restr = "读取成功！";
           return true;
       }
       public bool GetIsAtBindStation(ref bool isAtBindStation,ref string restr)
       {
           if(this.repairParam == null)
           {
               restr = "返修流程参数为空！";
               return false;
           }
           if (this.repairParam.StartDevStation == ALineWorkStationNum && this.repairParam.ProcessNum == ALineBindProcessNum)
           {
               isAtBindStation = true;
           }
           else
           {
               isAtBindStation = false;
           }
           return true;
       }
    
       public bool AddRepairProcess(ref string restr,string isSpecial)
       {
           List<DBAccess.Model.BatteryModuleModel> modelList = modBll.GetModelList(string.Format("palletID='{0}'  and palletBinded=1", this.nodeBase.rfidUID)); //modBll.GetModelByPalletID(this.rfidUID, this.nodeName);
           if (modelList == null || modelList.Count == 0)
           {
               restr = "获取返修加工参数失败,工装板绑定数据为空：" + this.nodeBase.rfidUID;
               return false;
           }
           foreach (DBAccess.Model.BatteryModuleModel battery in modelList)
           {
               DBAccess.Model.RepairRecordModel existModule = bllRepairRecord.GetModel(battery.batModuleID);

               DBAccess.Model.RepairRecordModel repairModel = new DBAccess.Model.RepairRecordModel();
               repairModel.BatteryModuleID = battery.batModuleID;
               repairModel.PalletID = this.nodeBase.rfidUID;
               repairModel.RepairRec_Reserve1 = isSpecial;
               repairModel.RepairProcessNum = this.repairParam.ProcessNum;
               repairModel.RepairStartStationNum = this.repairParam.StartDevStation;

               if (existModule == null)
               {
                   bllRepairRecord.Add(repairModel);
               }
               else
               {
                   bllRepairRecord.Update(repairModel);
               }
           }
       
           return true;
                 
       }
       public bool AtBindStationBusiness(bool isAtBindStation,ref string restr)
       {
           if(isAtBindStation==true)
           {
              
               if (this.CommitDB2(57, 1, ref restr) == false)
               {

                   this.nodeBase.LogRecorder.AddDebugLog(this.nodeBase.NodeName, "写入PLC地址：" + this.AtALineBindAddr + "写入1失败！");

                   return false;
               }
               
               this.nodeBase.LogRecorder.AddDebugLog(this.nodeBase.NodeName, "写入PLC地址：" + this.AtALineBindAddr + "为1成功！");
           }
           else
           {
               if (this.CommitDB2(57, 2, ref restr) == false)
               {

                   this.nodeBase.LogRecorder.AddDebugLog(this.nodeBase.NodeName, "写入PLC地址：" + this.AtALineBindAddr + "写入2失败！");

                   return false;
               }
               this.nodeBase.LogRecorder.AddDebugLog(this.nodeBase.NodeName, "写入PLC地址：" + this.AtALineBindAddr + "为2成功！");
           }
           return true;
       }
       public override void RepairBusiness(ref bool businessComplete)
       {

           switch (stepIndex)
           {
               case 1:
                   {
                       string restr = "";
                       Console.WriteLine("Repair1");
                       if (GetRepairProcessParamByRfid(this.nodeBase.rfidUID, ref repairParam, ref restr) == false)
                       {
                           this.nodeBase.LogRecorder.AddDebugLog(this.nodeBase.NodeName, restr);
                           break;
                       }
                       string isSpecialProcess= false.ToString();
                       IsSpecialProcess(repairParam, ref isSpecialProcess, ref restr);
                   
                       this.nodeBase.LogRecorder.AddDebugLog(this.nodeBase.NodeName, restr);
                     
                       if (AddRepairProcess(ref restr, isSpecialProcess) == false)
                       {
                           this.nodeBase.LogRecorder.AddDebugLog(this.nodeBase.NodeName, restr);
                           break;
                       }
                       stepIndex++;
                       break;
                   }
               case 2:
                   {
                       Console.WriteLine("Repair3");
                       if (IsMatchProcess(this.repairParam) == true)
                       {
                           Console.WriteLine("Repair4");
                           bllRepairRecord.UpdateIsMatch(this.nodeBase.rfidUID, true);
                           this.nodeBase.LogRecorder.AddDebugLog(this.nodeBase.NodeName,"起始工位："+this.repairParam.StartDevStation +"，流程代号：" +this.repairParam.ProcessNum+"，匹配成功！");
                           stepIndex = 4;//绑定
                       }
                       else
                       {
                           Console.WriteLine("Repair5");
                           string restr = "";
                           bllRepairRecord.UpdateIsMatch(this.nodeBase.rfidUID, false);
                           if (this.CommitDB2(56, 1, ref restr) == false)
                           {
                               restr = "写入PLC地址：" + this.ProcessMatchAddr + "失败！";
                               this.nodeBase.LogRecorder.AddDebugLog(this.nodeBase.NodeName, restr);
                               break;
                           }
 
                           restr = "写入PLC地址：" + this.ProcessMatchAddr + "写入1成功！";
                           this.nodeBase.LogRecorder.AddDebugLog(this.nodeBase.NodeName, restr);
                           this.nodeBase.LogRecorder.AddDebugLog(this.nodeBase.NodeName, "起始工位：" + this.repairParam.StartDevStation + "，流程代号：" + this.repairParam.ProcessNum + "，匹配失败！");
                           
                           Console.WriteLine("Repair6");
                           stepIndex++;
                       }

                       break;
                   }
               case 3:
                   {
                       Console.WriteLine("Repair7");
                       int devStatus = 0;
                       if (this.nodeBase.PlcRW.ReadDB(this.ProcessMatchAddr, ref devStatus) == false)
                       {
                          string restr = "读取PLC地址：" + this.ProductCateAddr + "失败！";
                          this.nodeBase.LogRecorder.AddDebugLog(this.nodeBase.NodeName, restr);
                       }
                       if(devStatus!=2)
                       {
                           break;
                       }
                       Console.WriteLine("Repair8");
                       stepIndex++;
                       break;
                   }
            
               case 4:
                   {
                       Console.WriteLine("Repair9");
                       bool isAtALineStation = false;
                       string restr = "";
                       if (this.GetIsAtBindStation(ref isAtALineStation, ref restr) == false)
                       {
                           this.nodeBase.LogRecorder.AddDebugLog(this.nodeBase.NodeName, "获取是否在A线绑定工位失败：" + restr);
                           break;
                       }
                       if (this.AtBindStationBusiness(isAtALineStation, ref restr) == false)
                       {
                           this.nodeBase.LogRecorder.AddDebugLog(this.nodeBase.NodeName, "执行在A线绑定工位逻辑失败：" + restr);
                           break;
                       }
                       this.nodeBase.LogRecorder.AddDebugLog(this.nodeBase.NodeName, "MES下发开始工作中心号："
                           + this.repairParam.StartDevStation + ",流程代号：" + this.repairParam.ProcessNum);
                       businessComplete = true;
                       break;
                   }
               default:
                   break;
           }
       }

 
    }
}
