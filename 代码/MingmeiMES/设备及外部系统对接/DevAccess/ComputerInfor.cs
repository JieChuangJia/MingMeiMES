using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Net;
using System.Diagnostics;
using System.IO;

namespace RobotSystemLicenceManager
{
    /// <summary>
    /// 枚举写入注册表的各项名称枚举
    /// </summary>
    public enum RegeditNameType
    {
        LastRegeditDate,                //最后一次登陆时间
        RegeditStartDate,               //软件注册开始时间
        RegeditEndDate,                 //软件注册结束时间
        CpuIdStr,                       //cpu序列号
        HardIdStr,                      //硬盘序列号
        MacAddressStr,                  //mac地址程序暂不用
        SiaSunRobot,                    //唯一标示名称
        SSMD5KEY                        //加密解密密钥（必须为8个字符）
    }

    /// <summary>
    /// 获取电脑信息类
    /// </summary>
    public static class ComputerInfor
    {
      //  public static string HardID;
      //  public static string CpuID;
       // public static string MainBoardID;
       // public static string MacAddress;
        //public static bool ComputerHardWareValid; //硬件信息是否有效 四个当中可获取到2个以上为有效

        ///// <summary>
        ///// 构造函数
        ///// </summary>
        //static ComputerInfor()
        //{
        //    HardID = GetHardIDOther();
        //    CpuID = GetCpuID();
        //    MainBoardID = GetComMainBoard();
        //    MacAddress = GetMacAddress();
        //    int validNum = 0;
        //    if (HardID.Trim() != "")
        //    {
        //        validNum++;
        //    }
        //    if (CpuID.Trim() != "")
        //    {
        //        validNum++;
        //    }
        //    if (MainBoardID.Trim() != "")
        //    {
        //        validNum++;
        //    }
        //    if (MacAddress.Trim() != "")
        //    {
        //        validNum++;
        //    }
        //    if (validNum >= 2)
        //    {
        //        ComputerHardWareValid = true;
        //    }
        //    else
        //    {
        //        ComputerHardWareValid = false;
        //    }
        //}

        /// <summary>
        /// 获取硬盘号
        /// </summary>
        /// <returns></returns>
        private static string GetHardIDOther()
        {
            return GetDiskSerialNumber();
            //string HDid = "";
            //ManagementClass cimobject = new ManagementClass("Win32_DiskDrive");
            //ManagementObjectCollection moc = cimobject.GetInstances();
            //foreach (ManagementObject mo in moc)
            //{
            //    HDid = (string)mo.Properties["Model"].Value;
            //}
            //return HDid;
        }

        /// <summary>
        /// 获取硬盘号
        /// </summary>
        /// <returns></returns>
        public static string GetDiskSerialNumber()
        {
            //ManagementObjectSearcher mos = new ManagementObjectSearcher();
            //mos.Query = new SelectQuery("Win32_DiskDrive", "", new string[] { "PNPDeviceID", "Signature" });
            //ManagementObjectCollection myCollection = mos.Get();
            //ManagementObjectCollection.ManagementObjectEnumerator em = myCollection.GetEnumerator();
            //em.MoveNext();
            //ManagementBaseObject moo = em.Current;
            //string id = moo.Properties["signature"].Value.ToString().Trim();
            //return id;


            string hdInfo = "";//硬盘序列号  

            ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\"c:\"");

            hdInfo = disk.Properties["VolumeSerialNumber"].Value.ToString();

            disk = null;

            return hdInfo.Trim();

        }

        private static string GetCPUIdStr()
        {
            string cpuid = "";
            ManagementClass clsMgtClass = new ManagementClass("Win32_ComputerSystemProduct");
            ManagementObjectCollection colMgtObjCol = clsMgtClass.GetInstances();
            foreach (ManagementObject objMgtObj in colMgtObjCol)
            {
                //MessageBox.Show(objMgtObj.Properties["IdentifyingNumber"].Value.ToString()); 
                cpuid = objMgtObj.Properties["UUID"].Value.ToString();
            }
            return cpuid;
        }
        /// <summary>
        /// 获取硬盘序列号
        /// </summary>
        /// <returns></returns>
        public static string GetHardID()
        {
            return GetDiskSerialNumber();
            //string hardIDStr = "";
            //try
            //{
            //    var searcher = new ManagementObjectSearcher("select * from Win32_Physicalmedia");
            //    var moc = searcher.Get();

            //    foreach (var mo in moc)
            //    {
            //        hardIDStr = mo["SerialNumber"].ToString().Trim();
            //        break;
            //    }
            //    return hardIDStr;
            //}
            //catch
            //{
            //    return hardIDStr;
            //}
        }

        /// <summary>
        /// 获取cpu序列号
        /// </summary>
        /// <returns></returns>
        public static string GetCpuID()
        {

            var cpuIDStr = "";
            try
            {
                var mc = new ManagementClass("Win32_Processor");
                var moc = mc.GetInstances();
                foreach (var mo in moc)
                {
                    cpuIDStr = mo.Properties["ProcessorId"].Value.ToString();
                    break;
                }
                return cpuIDStr;
            }
            catch
            {
                return cpuIDStr;
            }
        }


        ///<summary>
        /// 根据截取ipconfig /all命令的输出流获取网卡Mac
        ///</summary>
        ///<returns></returns>
        private static List<string> GetMacByIPConfig()
        {
            List<string> macs = new List<string>();
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo("ipconfig", "/all");
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardInput = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.CreateNoWindow = true;
                Process p = Process.Start(startInfo);
                //截取输出流
                StreamReader reader = p.StandardOutput;
                string line = reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        line = line.Replace(" ","").Trim();
                        if (line.StartsWith("Ethernetadapter本地连接") || line.StartsWith("以太网适配器本地连接"))
                        {
                            while (true)
                            {
                                line = reader.ReadLine().Replace(" " ,"").Trim();
                                if (line.StartsWith("PhysicalAddress") || line.StartsWith("物理地址"))
                                {
                                    macs.Add(line);
                                    break;
                                }
                            }
                        }
                    }

                    line = reader.ReadLine();
                }

                //等待程序执行完退出进程
                p.WaitForExit();
                p.Close();
                reader.Close();
            }
            catch { }
            return macs;
        }

        /// <summary>
        /// 获取mac地址
        /// </summary>
        /// <returns></returns>
        public static string GetMacAddress()
        {
            var mac = "";
            try
            {
                if ( GetMacByIPConfig().Count > 0 )
                {
                    string[] macAddr = GetMacByIPConfig()[0].Split(':');
                    if (macAddr.Length == 2)
                    {
                        mac = macAddr[1].Trim();
                    }
                }
                if (mac == "")
                {
                    var mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                    var moc = mc.GetInstances();
                    foreach (var mo in moc)
                    {
                        if ((bool)mo["IPEnabled"] == true)
                        {
                            mac = mo["MacAddress"].ToString();
                            break;
                        }
                    }
                }
                return mac;
            }
            catch
            {
                return mac;
            }
        }

        /// <summary>
        /// 获取主板号
        /// </summary>
        /// <returns></returns>
        public static string GetComMainBoard()
        {
            string strbNumber = string.Empty;
            ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_baseboard");
            foreach (ManagementObject mo in mos.Get())
            {
                strbNumber = mo["SerialNumber"].ToString();
                break;
            }
            return strbNumber;
        }
    }
}
