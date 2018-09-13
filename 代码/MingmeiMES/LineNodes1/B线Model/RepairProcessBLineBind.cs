using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PLProcessModel;

namespace LineNodes
{
   public class RepairProcessBLineBind:RepairProcessBase
    {
       private DBAccess.BLL.RepairProcessBLL bllRepairProcess = new DBAccess.BLL.RepairProcessBLL();
       private DBAccess.BLL.RepairRecordBLL bllRepairRecord = new DBAccess.BLL.RepairRecordBLL();
       private DBAccess.BLL.BatteryModuleBll modBll = new DBAccess.BLL.BatteryModuleBll();

       private string ProductCateAddr = "D3003";//1为正常产品，2为NG返修产品
       private string ProcessMatchAddr = "D3004";//流程是否匹配，1位不匹配，2是等待设备设置
       //private string AtALineBindAddr="D3057";//是否在A线绑定工位，1是，2否
       //private string BLineWorkStationNum = "Y00100301";
       //private string BLineBindProcessNum = "A1";
     
       private RepairProcessParam repairParam = null;
       public RepairProcessBLineBind(CtlNodeBaseModel nodeBase)
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
     
       public bool AddRepairProcess(ref string restr)
       {
           List<DBAccess.Model.BatteryModuleModel> modelList = modBll.GetModelList(string.Format("palletID='{0}'  and palletBinded=1", this.nodeBase.rfidUID)); //modBll.GetModelByPalletID(this.rfidUID, this.nodeName);
           if (modelList == null || modelList.Count == 0)
           {
               restr = "获取返修加工参数失败,工装板绑定数据为空：" + this.nodeBase.rfidUID;
               return false;
           }
           foreach (DBAccess.Model.BatteryModuleModel battery in modelList)
           {
               DBAccess.Model.RepairRecordModel repairModel = new DBAccess.Model.RepairRecordModel();
               repairModel.BatteryModuleID = battery.batModuleID;
               repairModel.PalletID = this.nodeBase.rfidUID;
               
               repairModel.RepairProcessNum = this.repairParam.ProcessNum;
               repairModel.RepairStartStationNum = this.repairParam.StartDevStation;

               bllRepairRecord.Add(repairModel);
           }
       
           return true;
                 
       }
    
       public override void RepairBusiness(ref bool businessComplete)
       {
           string restr = "";
           switch (stepIndex)
           {
               case 1:
                   {
                     
                       Console.WriteLine("Repair1");
                       if (GetRepairProcessParam(this.nodeBase.rfidUID, ref repairParam, ref restr) == false)
                       {
                           this.nodeBase.LogRecorder.AddDebugLog(this.nodeBase.NodeName, restr);
                           break;
                       }
                       Console.WriteLine("Repair2");
                       if(AddRepairProcess(ref restr) ==false)
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
                           stepIndex = 4;//绑定
                       }
                       else
                       {
                           Console.WriteLine("Repair5");
                           bllRepairRecord.UpdateIsMatch(this.nodeBase.rfidUID, false);
                         
                           if (this.CommitDB2(4, 1,ref restr) == false)
                           {
                               this.nodeBase.LogRecorder.AddDebugLog(this.nodeBase.NodeName, restr);
                               break;
                           }

                           if (this.nodeBase.PlcRW.WriteDB(this.ProcessMatchAddr, 1) == false)
                           {
                               
                           }
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
               case 4://绑定
                   {
                       Console.WriteLine("Repair9");
                       businessComplete = true;
                       break;
                   }
               default:
                   break;
           }
       }

      
    }
}
