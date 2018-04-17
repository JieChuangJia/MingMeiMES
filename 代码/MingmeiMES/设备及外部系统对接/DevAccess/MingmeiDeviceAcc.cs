using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
namespace DevAccess
{

    /// <summary>
    /// 明美项目，设备数据采集封装
    /// </summary>
    public class MingmeiDeviceAcc
    {
        //private string devIP = "";
       // private int devPort = 6666;
        private string connStr = "";
        private int connTimeOut = 5;  //连接超时，单位：秒
        private int recvTimeOut = 5000;//应答超时，单位：毫秒 
        private UdpClient udpClient = null;
        private IPEndPoint udpRemotePoint = null;
        private NetworkStream workStream = null;
        private ManualResetEvent connectDone = new ManualResetEvent(false);
        private object lockObj = new object();// 多线程锁
        private bool isConnected = false;
        private string recvStr = "";
        private List<byte> recBuffer = new List<byte>();  //接收缓存
        private int recvPhase = 0;//接收起止标识，recvPhase = 1:收到'{'，有效字符串开始，2：收到'}',有效字符结束
        public int DevID = 0;
        public string Role { get; set; }
       // public string DevIP { get { return devIP; } set { devIP = value; } }
        // public int DevPort { get { return devPort; } set { devPort = value; } }
        #region 公有
        public string ConnStr
        { get { return connStr; } set { this.connStr = value; } }
        public int LocalPort { get; set; }
        public string RecvStr { get { return recvStr; } }
        public MingmeiDeviceAcc()
        {

        }
        public bool Connect(ref string reStr)
        {
            try
            {
                lock (lockObj)
                {
                    if (string.IsNullOrEmpty(this.connStr))
                    {
                        reStr = "PLC通信地址为空!";
                        return false;
                    }
                    //this.connStr = plcAddr;
                    string[] splitStr = new string[] { ",", ";", ":", "-", "|" };
                    string[] strArray = this.connStr.Split(splitStr, StringSplitOptions.RemoveEmptyEntries);
                    if (strArray.Count() < 2)
                    {
                        isConnected = false;
                    }
                    string remoteIP = strArray[0];
                    int remotePort = int.Parse(strArray[1]);
                    if (udpClient == null)
                    {
                        udpClient = new UdpClient(LocalPort);
                    }
                    this.udpRemotePoint = new IPEndPoint(IPAddress.Parse(remoteIP), remotePort);

                    UdpState udpReceiveState = new UdpState();
                    udpReceiveState.ipEndPoint = this.udpRemotePoint;
                    udpReceiveState.udpClient = udpClient;

                    udpClient.BeginReceive(UdpRecvCallback, udpReceiveState);
                    return true;
                }
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return false;

            }
        }
        public bool StartDev(List<string> productList,string devID,ref string reStr)
        {
            try
            {
                lock(lockObj)
                {
                    string cmdStr="START";
                    string sndStr = GenerateSndXML(devID, cmdStr, productList);
                    if (string.IsNullOrWhiteSpace(sndStr))
                    {
                        reStr = "生成xml字符串为空";
                        return false;
                    }
                    sndStr = "{" + sndStr + "}";

                    Send(sndStr);
                    if (!WaitRecvOK(recvTimeOut,devID,cmdStr,ref reStr))
                    {
                        reStr = "设备应答超时," + reStr;
                        return false;
                    }
                    reStr = sndStr;
                    return true;
                }
               
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return false;
               
            }
        }
        public bool EndDev(string devID,ref string reStr)
        {
            try
            {
                lock(lockObj)
                {
                    string cmdStr = "END";
                    string sndStr = GenerateSndXML(devID, cmdStr, null);
                    if (string.IsNullOrWhiteSpace(sndStr))
                    {
                        reStr = "生成xml字符串为空";
                        return false;
                    }
                    sndStr = "{" + sndStr + "}";

                    Send(sndStr);
                    if (!WaitRecvOK(recvTimeOut, devID, cmdStr, ref reStr))
                    {
                        reStr = "设备应答超时," + reStr;
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return false;
            }
        }
        public IDictionary<string,string> GetData(string devID,ref string reStr)
        {
            try
            {
                lock(lockObj)
                {
                    string cmdStr = "GET";
                    string sndStr = GenerateSndXML(devID, cmdStr, null);
                    if (string.IsNullOrWhiteSpace(sndStr))
                    {
                        reStr = "生成xml字符串为空";
                        return null;
                    }
                    sndStr = "{" + sndStr + "}";

                    Send(sndStr);
                    if (!WaitRecvOK(recvTimeOut, devID, cmdStr, ref reStr))
                    {
                        reStr = "设备应答超时," + reStr;
                        return null;
                    }
                    IDictionary<string, string> processDataDic = ParseRecvData(this.recvStr, ref reStr);
                    return processDataDic;
                }
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return null;
            }
        }

        public IDictionary<string,string> ParseRecvData(string xmlStr,ref string reStr)
        {
            try
            {
                IDictionary<string, string> processDataDic = new Dictionary<string, string>();
                XElement root = XElement.Parse(xmlStr.Trim());
                if (root.Attribute("status").Value.ToString().Trim().ToUpper() != "OK")
                {
                    reStr = "设备未返回数据，因为数据状态:" + root.Attribute("status").Value.ToString().Trim().ToUpper();
                    return null;
                }
                IEnumerable<XElement> xeProducts = root.Elements("Data");
                if (xeProducts != null && xeProducts.Count() > 0)
                {
                    foreach (XElement xe in xeProducts)
                    {
                        if (xe.Attribute("productID") != null)
                        {
                            string productID = xe.Attribute("productID").Value.ToString().Trim();
                            processDataDic[productID] = xe.Value.ToString();
                        }
                    }
                }
                return processDataDic;
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return null;
              
            }
          
        }
        public string GenerateSndXML(string devID, string cmdStr, List<string> productList)
        {
            string xmlStr = "<?xml version=\"1.0\" encoding=\"GB2312\" ?><MesDT transFlag=\"COMMAND\" cmd=\"Ready\"><Desc deviceID=\"电池极性检测\"></Desc></MesDT>";
            XElement root = XElement.Parse(xmlStr);
            if (root == null)
            {
                xmlStr = string.Empty;
                return xmlStr;
            }
            root.Attribute("cmd").Value = cmdStr;
            XElement xeDesc = root.Element("Desc");
            xeDesc.Attribute("deviceID").Value = devID;
            if (cmdStr.ToUpper() == "START" && productList != null && productList.Count() > 0)
            {

                for (int i = productList.Count() - 1; i >= 0; i--)
                {
                    //int seq = 0;
                   
                    int seq = i + 1;
                    XElement xe = new XElement("ProductID", new XAttribute("seq", seq.ToString()));
                    xe.Value = productList[i].Trim();
                    xeDesc.AddAfterSelf(xe);
                }
            }
            xmlStr = root.ToString();
            xmlStr = "<?xml version=\"1.0\" encoding=\"GB2312\" ?>\r\n"+xmlStr;
            return xmlStr;
        }
        #endregion
        #region 私有
        private void Send(string strSnd)
        {
            Encoding gb2312 = Encoding.GetEncoding("GB2312");
            byte[] bytesToSnd = gb2312.GetBytes(strSnd);
            Send(bytesToSnd);
        }
        private void Send(byte[] bytesToSnd)
        {
            if(!this.isConnected)
            {
               
                string reStr = "";
                Connect(ref reStr);
            }
            this.recvPhase = 0;
            this.recBuffer.Clear();
            this.udpClient.Send(bytesToSnd, bytesToSnd.Count(), this.udpRemotePoint);
        }
        private bool WaitRecvOK(int timeOut,string devID,string cmd, ref string reStr)
        {
            DateTime st = System.DateTime.Now;
            if (timeOut > 5000)
            {
                timeOut = 5000;
            }
            try
            {
                while (true)
                {
                    Thread.Sleep(5);
                    if (this.recvPhase == 2 && !string.IsNullOrWhiteSpace(recvStr))
                    {
                        XElement root = XElement.Parse(recvStr);
                        if (root == null)
                        {
                            continue;
                        }
                        if (cmd.ToUpper() == "GET")
                        {
                            //等待数据帧
                            if (root.Attribute("transFlag") == null || root.Attribute("transFlag").Value.ToUpper().Trim() != "DATA")
                            {
                                continue;
                            }
                            if (root.Element("Src") == null || root.Element("Src").Attribute("deviceID") == null || root.Element("Src").Attribute("deviceID").Value.Trim() != devID.Trim())
                            {
                                continue;
                            }
                            return true;
                        }
                        else
                        {
                            //等待应答帧
                            if (root.Attribute("transFlag") == null || root.Attribute("transFlag").Value.ToUpper().Trim() != "RES")
                            {
                                continue;
                            }
                            if (root.Attribute("cmd") == null || root.Attribute("cmd").Value.ToUpper().Trim() != cmd.ToUpper().Trim())
                            {
                                continue;
                            }
                            if (root.Element("Src") == null || root.Element("Src").Attribute("deviceID") == null || root.Element("Src").Attribute("deviceID").Value.Trim() != devID.Trim())
                            {
                                continue;
                            }
                            return true;
                        }
                    }
                    DateTime cur = System.DateTime.Now;
                    TimeSpan ts = cur - st;
                    if (ts.TotalMilliseconds > timeOut)
                    {

                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return false;
            }
           
        }
       
         private void UdpRecvCallback(IAsyncResult iar)
        {
            try
            {
                UdpState udpState = iar.AsyncState as UdpState;
                if (iar.IsCompleted)
                {
                    Byte[] recBytes = udpState.udpClient.EndReceive(iar, ref udpState.ipEndPoint);
                    if (recBytes != null && recBytes.Count() > 0)
                    {
                        for (int i = 0; i < recBytes.Count(); i++)
                        {
                            if (recBytes[i] == '{')
                            {
                                this.recvStr = "";
                                this.recvPhase = 1;
                                continue;
                            }
                            else if (recBytes[i] == '}')
                            {
                                this.recvPhase = 2;
                                break;
                            }
                            if (this.recvPhase == 1)
                            {
                                this.recBuffer.Add(recBytes[i]);

                            }
                        }
                        // this.recBuffer.AddRange(recBytes);
                        if (this.recvPhase == 2)
                        {
                            Encoding gb2312 = Encoding.GetEncoding("GB2312");
                            this.recvStr = gb2312.GetString(this.recBuffer.ToArray()).Trim();
                            //Console.WriteLine("收到{0}数据：{1}", connStr, recvStr);
                        }
         
                       
                    }

                }
                udpClient.BeginReceive(UdpRecvCallback, udpState);
            }
            catch (Exception ex)
            {
                isConnected = false;
                Console.WriteLine("{0}数据通信异常，{1}",this.DevID,ex.ToString());
               
            }
           

        }
        #endregion
     
    }
}
