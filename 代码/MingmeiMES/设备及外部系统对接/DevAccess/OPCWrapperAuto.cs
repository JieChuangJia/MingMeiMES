using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OPCAutomation;
namespace DevAccess
{
    public class OPCWrapperAuto
    {
        #region private 
        private OPCServer myServer = new OPCServer();
        //private OPCGroups myGroups = null;
      //  private OPCGroup myGroup = null;
        private OPCBrowser myBrowser = null;
        #endregion
        #region public
        public bool Connect(string serverName, string serverIP,out string reStr)
        {
           
      
            reStr = "";
            bool re = false;
            try
            {
                myServer.Connect(serverName, serverIP);
                reStr = "连接OPC服务器成功";
                myBrowser = myServer.CreateBrowser();
                if (myBrowser == null)
                {
                    reStr += ";创建server浏览器对象失败";
                    return false;
                }
                return true;
            }
            catch (System.Exception ex)
            {
                reStr = "连接OPC服务器失败，错误：" + ex.Message;
                return false;
            }
            
          
        }
        public bool CloseConn(ref string reStr)
        {
            try
            {
                myServer.Disconnect();
                return true;
            }
            catch (Exception ex)
            {
                reStr = ex.Message;
                return false;
            }
        }
        

        public bool Read(string groupName, string itemID, out object result)
        {
            bool re = false;
            result = null;
           
            try
            {
               // Console.WriteLine("{0},{1}", groupName, itemID);
                OPCGroup myGroup = myServer.OPCGroups.GetOPCGroup(groupName);
                if (myGroup == null)
                {
                    return false;
                }
              //  Console.WriteLine("开始读OPC Item:{0}", itemID);
                OPCItem myItem = myGroup.OPCItems.Item(itemID);
                if (myItem == null)
                {
                    return false;
                }
                object quality = 0;
                object  timeStamp = 0;
                short source = (short)OPCDataSource.OPCDevice;
                OPCItem item = myGroup.OPCItems.Item(itemID);
                myItem = myGroup.OPCItems.GetOPCItem(item.ServerHandle);
                myItem.Read(source, out result, out quality, out timeStamp);


                return true;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("发生异常：{0},{1},{2}", groupName, itemID,ex.ToString());
                return false;
            }
          
        }
        public bool Write(string groupName, string itemID, object val)
        {
            bool re = false;
            
           
            try
            {
                OPCGroup myGroup = myServer.OPCGroups.GetOPCGroup(groupName);
                if (myGroup == null)
                {
                    return false;
                }
                OPCItem myItem = myGroup.OPCItems.Item(itemID);
                if (myItem == null)
                {
                    return false;
                }
                myItem.Write(val);
                re = true;
            }
            catch (System.Exception ex)
            {

               
                re = false;
            }
            return re;
        }
        public bool GetGroups(out string[] groupNames)
        {
            groupNames = null;
            OPCGroups myGroups = myServer.OPCGroups;
            
            if(myGroups == null)
            {
                return false;
            }

            groupNames =  new string[myGroups.Count];
            for (int i = 0; i < groupNames.Count(); i++)
            {
                groupNames[i] = myGroups.Item(i+1).Name;
            }

            return true;
        }
        public bool GetItems(string groupName, out string[] itemNames,out string reStr)
        {
            reStr = "";
            
            itemNames = null;
            try
            {
                OPCGroups myGroups = myServer.OPCGroups;
                if (myGroups == null)
                {
                    reStr = "得到项列表失败,组名：" +groupName+ "不存在";
                    return false;
                }
                OPCGroup myGroup = myGroups.GetOPCGroup(groupName);
                if (myGroup == null)
                {
                    reStr = "得到项列表失败";
                    return false;
                }
                OPCItems items = myGroup.OPCItems;
                itemNames = new string[items.Count];
                for (int i = 0; i < itemNames.Count(); i++)
                {
                    itemNames[i] = items.Item(i + 1).ItemID;
                }
                reStr = "得到项列成功!";
                return true;
            }
            catch (System.Exception ex)
            {
                reStr = "得到项列表失败，返回异常：" + ex.Message;
                return false;
            }
           
        }
        public bool AddGroup(string groupName, out string reStr)
        {
            reStr = "";
            bool re = false;
            try
            {
                //OPCGroup myGroup = GetGroup(groupName);
                //if (myGroup != null)
                //{
                //    reStr = "添加组：" + groupName + "失败,已经存在同样的组";
                //    return false;
                //}
                OPCGroup myGroup = myServer.OPCGroups.Add(groupName);
                if (myGroup == null)
                {
                    reStr = "添加组：" + groupName + "失败";
                    return false;
                }
                else
                {
                    reStr = "添加组：" + groupName + "成功!";
                    myGroup = myServer.OPCGroups.GetOPCGroup(groupName);
                    if(myGroup == null)
                    {
                        return false;
                    }
                    Console.WriteLine("添加组：" + groupName);
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                reStr = "添加组：" + groupName + "失败,异常信息："+ex.Message;
                re = false;
            }
           
            
            return re;
        }
        public bool RemoveGroup(string groupName,out string reStr)
        {
            reStr = "成功移除";
           
            try
            {
                myServer.OPCGroups.Remove(groupName);
                reStr = "移除组：" + groupName + " 成功!";
                return true;
            }
            catch (System.Exception ex)
            {
                reStr = "移除组：" + groupName +"失败，返回异常："+ ex.Message;
                return false;
            }
            
        }
        public bool AddItem(string groupName, string itemName, out string reStr)
        {
            reStr = "";
            try
            {
                int itemHandle = 0;
                OPCGroup myGroup = GetGroup(groupName);
                if (myGroup == null)
                {
                    reStr = "添加项:" + itemName + "失败,组名不存在";
                    return false;
                }
                OPCItem item = myGroup.OPCItems.AddItem(itemName, itemHandle);
                if (item == null)
                {
                    reStr = "添加项:" + itemName + "失败";
                    return false;
                }
                reStr = groupName+" 添加项:" + itemName + "成功";
              //  Console.WriteLine(reStr);
                return true;
            }
            catch (System.Exception ex)
            {
                reStr = groupName +"添加项:" + itemName + "失败,返回异常：" + ex.Message;
                return false;
            }
  
            
        }
        /// <summary>
        /// 移除项，注意：此接口目前未能实现，需要继续调试
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="itemName"></param>
        /// <param name="reStr"></param>
        /// <returns></returns>
        public bool RemoveItem(string groupName, string itemName, out string reStr)
        {
            reStr = "";
            try
            {
                OPCGroup myGroup = GetGroup(groupName);
                if (myGroup == null)
                {
                    reStr = "组：" + groupName + "不存在 ";
                    return false;
                }
                OPCItems items = myGroup.OPCItems;
                OPCItem item = items.Item(itemName);
                Array itemHandles = Array.CreateInstance(typeof(int),1);
                itemHandles.SetValue(item.ServerHandle, 0);
                Array errors = null;// Array.CreateInstance(typeof(long), 10);
               
                items.Remove((int)1, ref itemHandles,out errors);
               
                reStr = "移除项:" + itemName + "成功!";
                return true;
            }
            catch (System.Exception ex)
            {
                reStr = "移除项:" + itemName + "失败,返回异常：" + ex.Message;
                return false;
            }
        }
        #endregion

        #region private methods
        private OPCGroup GetGroup(string groupName)
        {
            OPCGroup group = null;
            try
            {
                //if (myServer.OPCGroups.Count != 0)//20140409判断OPCGroup中的group的数量不为0,再查找组中是否存在指定名称的组。
                //{
                    //return group;
                    
                    group = myServer.OPCGroups.Item(groupName);//20140409,马天牧，本机测试添加组不成功
                //}
                
            }
            catch (System.Exception ex)
            {
                group = null;
            }
            return group;
        }
        #endregion
    }
}
