using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevInterface;
using System.IO.Ports;
using System.Threading;
namespace DevAccess
{
    /// <summary>
    /// 艾诺安规设备访问
    /// </summary>
    public class AinuoAccess
    {
        private string devName = "";
        private int readerID = 1;
        private int devAddr = 1;
        private Thread recvThread;
        private bool recvExit = false;
        private bool detectBegin = false;
        private bool detectDataOK = false;
        private List<byte> detectData = null;
        private SerialPort comportObj = null;
        public int recvInterval = 10;
        private bool pauseFlag = false;
        private string strCmdRes = ""; //命令应答
        private int recvPhase = 0;//接收起止标识，recvPhase = 1:收到'{'，有效字符串开始，2：收到'}',有效字符结束
        //private AinuoDetectModel detectModel = new AinuoDetectModel();
        public int ReaderID { get { return readerID; } }
        //public SerialPort ComPortObj { get; set; }
        public string ComportName { get; set; }
        public AinuoAccess(int id, int devAddr,string port)
        {
            this.devAddr = devAddr;
            this.ComportName = port;
            readerID = id;
            devName = "艾诺安规检测仪" + readerID.ToString();
            detectData = new List<byte>();
            recvExit = false;
            recvThread = new Thread(new ThreadStart(ComRecvProc));
            recvThread.IsBackground = true;
            recvThread.Priority = ThreadPriority.Highest;
            recvThread.Name = string.Format("艾诺安规仪{0}接收线程", this.readerID);
        }
        public bool CommPortOpen(ref string reStr)
        {
            try
            {
                string[] ports = System.IO.Ports.SerialPort.GetPortNames();
                if (!ports.Contains(this.ComportName))
                {
                    reStr = string.Format("{0} 口不存在", this.ComportName);
                    return false;
                }
                if(this.comportObj == null)
                {
                    this.comportObj = new SerialPort(this.ComportName, 9600, Parity.None, 8, StopBits.One);

                }
                if(this.comportObj != null && this.comportObj.IsOpen)
                {
                    this.comportObj.Close();
                }
                this.comportObj.Open();
                return true;
            }
            catch (Exception ex)
            {
                reStr = ex.Message;
                return false;
            }
        }
        public bool StartMonitor(ref string reStr)
        {
            this.pauseFlag = false;
            if (this.recvThread.ThreadState == (ThreadState.Background | ThreadState.Unstarted))
            {
                recvThread.Start();
            }
            return true;
        }
        public void StopMonitor()
        {
            this.pauseFlag = false;
            
           
        }
        public bool CommPortClose(ref string reStr)
        {
            try
            {
                if(this.comportObj == null)
                {
                    reStr = string.Format("串口:{0}未打开", this.ComportName);
                    return false;
                }
                this.comportObj.Close();
                return true;
            }
            catch (Exception ex)
            {
                reStr = ex.Message;
                return false;
            }
        }
        /// <summary>
        /// 打开设备，开始检测
        /// </summary>
        /// <param name="reStr"></param>
        /// <returns></returns>
        public bool OpenDev(ref string reStr)
        {
            try
            {
                if(this.comportObj == null || (!this.comportObj.IsOpen))
                {
                    reStr = "串口未打卡";
                    return false;
                }
                string strCmd = string.Format("{0}{1}{2}{3}{4}","{",devAddr.ToString().PadLeft(3,'0'),"1","00","}");
                string cmdRes1 = string.Format("{0}11XX", devAddr.ToString().PadLeft(3, '0'));
                string cmdRes2 = string.Format("{0}12XX", devAddr.ToString().PadLeft(3, '0'));
                recvPhase = 0;
              //  Console.WriteLine("发送命令：" + strCmd);
                strCmdRes = "";
                this.comportObj.Write(strCmd);
                int timeCounter = 0;
                int reTryInterval = 100;
                int timeOut = 2000;
                
                while (timeCounter < timeOut)
                {
                    if(recvPhase == 2)
                    {
                       // Console.WriteLine("接收：" + strCmdRes);
                        if (strCmdRes.Trim().ToUpper() == cmdRes1 || strCmdRes.Trim().ToUpper() == cmdRes2)
                        {
                            return true;
                        }
                    }
                  
                    Thread.Sleep(reTryInterval);

                    timeCounter += reTryInterval;
                }
                reStr = "应答超时:2秒";
                return false;
            }
            catch (Exception ex)
            {
                reStr = ex.Message;
                return false;
            }
        }
        /// <summary>
        /// 关闭设备，停止检测
        /// </summary>
        /// <param name="reStr"></param>
        /// <returns></returns>
        public bool CloseDev(ref string reStr)
        {
            try
            {
                if (this.comportObj == null || (!this.comportObj.IsOpen))
                {
                    reStr = "串口未打卡";
                    return false;
                }
                string strCmd = string.Format("{0}{1}{2}{3}{4}", "{", devAddr.ToString().PadLeft(3, '0'), "2", "00", "}"); //"{015200}";
                string cmdRes1 = string.Format("{0}21XX", devAddr.ToString().PadLeft(3, '0'));
                string cmdRes2 = string.Format("{0}22XX", devAddr.ToString().PadLeft(3, '0'));
                recvPhase = 0;
                strCmdRes = "";
                this.comportObj.Write(strCmd);
                int timeCounter = 0;
                int reTryInterval = 100;
                int timeOut = 2000;
                while (timeCounter < timeOut)
                {
                    if (recvPhase == 2)
                    {
                        if (strCmdRes.Trim().ToUpper() == cmdRes1 || strCmdRes.Trim().ToUpper() == cmdRes2)
                        {
                            return true;
                        }
                    }
                   
                    Thread.Sleep(reTryInterval);

                    timeCounter += reTryInterval;
                }
                reStr = "应答超时:2秒";
                return false;
            }
            catch (Exception ex)
            {
                reStr = ex.Message;
                return false;
            }
        }
        public AinuoDetectModel GetDetectResult(ref string reStr)
        {
            try
            {
                if (this.comportObj == null || (!this.comportObj.IsOpen))
                {
                    reStr = "串口未打卡";
                    return null;
                }
                string strCmd = string.Format("{0}{1}{2}{3}{4}", "{", devAddr.ToString().PadLeft(3, '0'), "0", "00", "}"); //"{015000}";
                recvPhase = 0;
                strCmdRes = "";
                this.comportObj.Write(strCmd);
                int timeCounter = 0;
                int reTryInterval = 100;
                int timeOut = 2000;
                string cmdResHead = string.Format("{0}0",  devAddr.ToString().PadLeft(3, '0'));
                while (timeCounter < timeOut)
                {
                    if (recvPhase == 2)
                    {
                        if(strCmdRes.Count()<191)
                        {
                            reStr = "返回数据长度不足191字符(不包括头尾符）";
                            return null;
                        }
                        if (strCmdRes.Trim().Substring(0, 4) == cmdResHead)
                        {
                            AinuoDetectModel detectModel = new AinuoDetectModel();
                            if(detectModel.Parse(strCmdRes.Substring(4,184),ref reStr))
                            {
                                return detectModel;
                            }
                            else
                            {
                                return null;
                            }
                                
                        }
                    }

                    Thread.Sleep(reTryInterval);

                    timeCounter += reTryInterval;
                }
                reStr = "应答超时:2秒,接收数据："+strCmdRes;
                return null;
               
            }
            catch (Exception ex)
            {
                reStr = ex.Message;
                return null;
            }
        }
        private void ComRecvProc()
        {
            //string strRecv = serialPort.ReadExisting();
            byte[] buf = new byte[256];
            while (!recvExit)
            {
                Thread.Sleep(recvInterval);
                if (pauseFlag)
                {
                    continue;
                }
                if (!comportObj.IsOpen)
                {
                    continue;
                }
                try
                {
                    int readNum = comportObj.Read(buf, 0, 256);
                    if (readNum <= 0)
                    {
                        continue;
                    }
                    for (int i = 0; i < readNum; i++)
                    {
                        // Console.WriteLine(string.Format("Recv:0x{0}",buf[i].ToString("X2")));
                        if (buf[i] == '{')
                        {
                            this.strCmdRes = "";
                            recvPhase = 1;
                            continue;
                        }
                        else if(buf[i]=='}')
                        {
                            recvPhase = 2;
                            break;
                        }
                        if(recvPhase == 1)
                        {
                            strCmdRes = strCmdRes + System.Text.Encoding.UTF8.GetString(buf,i,1);
                        }
                       
                    }
                }
                catch (Exception ex)
                {
                     comportObj.Close();
                   
                }
              
             
            }
        }
    }

    public enum AinuoEnumDetectItem
    {
        无= 0,
        接地= 1,
        绝缘= 2,
        耐压 =3,
        泄漏= 4,
        功率 = 5,
        启动 = 6,
        直耐 = 7,
        其它 = 8
    }
    public enum  AinuoEnumDetectResult
    {
        未判断 = 0,
        OK = 1,
        NG= 2,
        PT = 3,
        HP = 4
    }
    public class AinuoDetectModel
    {
        private const int dataItemLen = 23; //数据项长度
        private List<AinuoDetectItemModel> detectItems = new List<AinuoDetectItemModel>();
        public List<AinuoDetectItemModel> DetectItems { get { return detectItems; } }
        public AinuoDetectModel()
        {

        }
        public bool Parse(string strDetectData,ref string reStr)
        {
            detectItems.Clear();
            int groupNum = strDetectData.Count() / dataItemLen;
            if(groupNum<1)
            {
                reStr = "不足一组数据的长度";
                return false;
            }
            if(groupNum>7)
            {
                groupNum = 7;
            }
            for(int i=0;i<groupNum;i++)
            {
                string strItem = strDetectData.Substring(i*dataItemLen,dataItemLen);
                AinuoDetectItemModel itemModel = new AinuoDetectItemModel();
                if(itemModel.Parse(strItem))
                {
                    detectItems.Add(itemModel);
                }
            }
            return true;
        }
        //public string GetValStr()
        //{
        //    return "";
        //}
    }
    public class  AinuoParamModel
    {
        public string paramName = "";
        public float val = 0;
    }
    public class AinuoDetectItemModel
    {
        private const int dataItemLen = 23; //数据项长度
        public AinuoEnumDetectItem detectItemType = AinuoEnumDetectItem.无;
        public List<AinuoParamModel> detectParams = new List<AinuoParamModel>();
        public string detectCondition = "静";
        public AinuoEnumDetectResult detectResult = AinuoEnumDetectResult.未判断;
        private string strDetectData = "";
        public AinuoDetectItemModel()
        {

        }
        public bool Parse(string strDetectData)
        {
            this.strDetectData = strDetectData;
           if(strDetectData.Count()<dataItemLen)
           {
               return false;
           }
           try
           {
               detectParams.Clear();
               int itemCata = int.Parse(strDetectData.Substring(0,1));
               switch(itemCata)
               {
                   case 1:
                       {
                           detectItemType = AinuoEnumDetectItem.接地;
                           AinuoParamModel param1 = new AinuoParamModel();
                           param1.paramName = "电流值(A)";
                           param1.val = int.Parse(strDetectData.Substring(1, 4)) / 100.0f;
                           detectParams.Add(param1);
                           AinuoParamModel param2 = new AinuoParamModel();
                           param2.paramName = "电阻值(兆欧)";
                           param2.val = int.Parse(strDetectData.Substring(5, 4)) / 10.0f;
                           detectParams.Add(param2);
                           AinuoParamModel param5 = new AinuoParamModel();
                           param5.paramName = "时间";
                           param5.val = int.Parse(strDetectData.Substring(17, 4)) / 10.0f;
                           detectResult = (AinuoEnumDetectResult)(int.Parse(strDetectData.Substring(dataItemLen - 1, 1)));

                           break;
                       }
                   case 2:
                       {
                           detectItemType = AinuoEnumDetectItem.绝缘;
                           AinuoParamModel param1 = new AinuoParamModel();
                           param1.paramName = "电压值(V)";
                           param1.val = int.Parse(strDetectData.Substring(1, 4));
                           detectParams.Add(param1);
                           AinuoParamModel param2 = new AinuoParamModel();
                           param2.paramName = "电阻值(兆欧)";
                           param2.val = int.Parse(strDetectData.Substring(5, 4))/10.0f;
                           detectParams.Add(param2);
                           AinuoParamModel param5 = new AinuoParamModel();
                           param5.paramName = "时间";
                           param5.val = int.Parse(strDetectData.Substring(17, 4)) / 10.0f;
                           detectParams.Add(param5);
                           detectResult = (AinuoEnumDetectResult)(int.Parse(strDetectData.Substring(dataItemLen - 1, 1)));
                           break;
                       }
                   case 3:
                       {
                           detectItemType = AinuoEnumDetectItem.耐压;
                           AinuoParamModel param1 = new AinuoParamModel();
                           param1.paramName = "电压值V";
                           param1.val = int.Parse(strDetectData.Substring(1, 4));
                           detectParams.Add(param1);
                           AinuoParamModel param2 = new AinuoParamModel();
                           param2.paramName = "电流值(mA)";
                           param2.val = int.Parse(strDetectData.Substring(5, 4)) / 100.0f;
                           detectParams.Add(param2);
                           AinuoParamModel param5 = new AinuoParamModel();
                           param5.paramName = "时间";
                           param5.val = int.Parse(strDetectData.Substring(17, 4)) / 10.0f;
                           detectParams.Add(param5);
                           if(strDetectData.Substring(21,1)=="0")
                           {
                               this.detectCondition = "静";
                           }
                           else
                           {
                               this.detectCondition = "动";
                           }
                           detectResult = (AinuoEnumDetectResult)(int.Parse(strDetectData.Substring(dataItemLen - 1, 1)));
                           break;
                       }
                   case 4:
                       {
                           detectItemType = AinuoEnumDetectItem.泄漏;
                           AinuoParamModel param1 = new AinuoParamModel();
                           param1.paramName = "电压值(V)";
                           param1.val = int.Parse(strDetectData.Substring(1, 4))/10.0f;
                           detectParams.Add(param1);
                           AinuoParamModel param2 = new AinuoParamModel();
                           param2.paramName = "电流值(mA)";
                           param2.val = int.Parse(strDetectData.Substring(5, 4)) / 1000.0f;
                           detectParams.Add(param2);
                           AinuoParamModel param5 = new AinuoParamModel();
                           param5.paramName = "时间";
                           param5.val = int.Parse(strDetectData.Substring(17, 4)) / 10.0f;
                           detectParams.Add(param5);
                           if (strDetectData.Substring(21, 1) == "0")
                           {
                               this.detectCondition = "静";
                           }
                           else
                           {
                               this.detectCondition = "动";
                           }
                           detectResult = (AinuoEnumDetectResult)(int.Parse(strDetectData.Substring(dataItemLen - 1, 1)));
                           break;
                       }
                   case 5:
                       {
                           detectItemType = AinuoEnumDetectItem.功率;
                           AinuoParamModel param1 = new AinuoParamModel();
                           param1.paramName = "电压值(V)";
                           param1.val = int.Parse(strDetectData.Substring(1, 4)) / 10.0f;
                           detectParams.Add(param1);
                           AinuoParamModel param2 = new AinuoParamModel();
                           param2.paramName = "电流值(mA)";
                           param2.val = int.Parse(strDetectData.Substring(5, 4)) / 1000.0f;
                           detectParams.Add(param2);
                           AinuoParamModel param3 = new AinuoParamModel();
                           param3.paramName = "功率(W)";
                           param3.val = int.Parse(strDetectData.Substring(9, 4)) / 10.0f;
                           detectParams.Add(param3);
                           AinuoParamModel param4 = new AinuoParamModel();
                           param4.paramName = "功率因数(W)";
                           param4.val = int.Parse(strDetectData.Substring(13, 4)) / 1000.0f;
                           detectParams.Add(param4);
                           AinuoParamModel param5 = new AinuoParamModel();
                           param5.paramName = "时间";
                           param5.val = int.Parse(strDetectData.Substring(17, 4)) / 10.0f;
                           detectParams.Add(param5);
                           detectResult = (AinuoEnumDetectResult)(int.Parse(strDetectData.Substring(dataItemLen - 1, 1)));
                           break;
                       }
                   case 6:
                       {
                           detectItemType = AinuoEnumDetectItem.启动;
                           AinuoParamModel param1 = new AinuoParamModel();
                           param1.paramName = "电压值(V)";
                           param1.val = int.Parse(strDetectData.Substring(1, 4)) / 10.0f;
                           detectParams.Add(param1);
                           AinuoParamModel param2 = new AinuoParamModel();
                           param2.paramName = "电流值(A)";
                           param2.val = int.Parse(strDetectData.Substring(5, 4)) / 1000.0f;
                           detectParams.Add(param2);

                           AinuoParamModel param5 = new AinuoParamModel();
                           param5.paramName = "时间";
                           param5.val = int.Parse(strDetectData.Substring(17, 4)) / 10.0f;
                           detectParams.Add(param5);
                           detectResult = (AinuoEnumDetectResult)(int.Parse(strDetectData.Substring(dataItemLen - 1, 1)));
                           break;
                       }
                   case 7:
                       {
                           detectItemType = AinuoEnumDetectItem.直耐;
                           AinuoParamModel param1 = new AinuoParamModel();
                           param1.paramName = "电压值(V)";
                           param1.val = int.Parse(strDetectData.Substring(1, 4));
                           detectParams.Add(param1);
                           AinuoParamModel param2 = new AinuoParamModel();
                           param2.paramName = "电流值(A)";
                           param2.val = int.Parse(strDetectData.Substring(5, 4));
                           detectParams.Add(param2);
                           AinuoParamModel param5 = new AinuoParamModel();
                           param5.paramName = "时间";
                           param5.val = int.Parse(strDetectData.Substring(17, 4)) / 10.0f;
                           detectParams.Add(param5);
                           if (strDetectData.Substring(21, 1) == "0")
                           {
                               this.detectCondition = "静";
                           }
                           else
                           {
                               this.detectCondition = "动";
                           }
                           detectResult = (AinuoEnumDetectResult)(int.Parse(strDetectData.Substring(dataItemLen - 1, 1)));
                           break;
                       }
                   default:
                       break;
               }
               return true;
             
           }
           catch (Exception ex)
           {
               return false;
           }
        }
        public string GetDataStr()
        {
            string strVal = detectItemType.ToString();
            for(int i=0;i<detectParams.Count();i++)
            {
                if(i==(detectParams.Count()-1))
                {
                    strVal += string.Format(" {0} {1}", detectParams[i].paramName, detectParams[i].val);
                }
                else
                {
                    strVal += string.Format(" {0} {1},", detectParams[i].paramName, detectParams[i].val);
                }
            }
            strVal += " 状态:" +detectResult.ToString();
            return strVal;
        }
    }
}
