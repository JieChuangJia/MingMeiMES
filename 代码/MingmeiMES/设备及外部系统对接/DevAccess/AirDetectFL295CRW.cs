using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevInterface;
using System.IO.Ports;
using System.Threading;
using System.Text.RegularExpressions;
namespace DevAccess
{
    public class AirDetectFL295CRW:IAirlossDetectDev
    {
        private string devName = "";
        private int readerID = 1;
        private Thread recvThread;
        private bool recvExit = false;
        private bool detectBegin = false;
        private bool detectDataOK = false;
        private List<byte> detectData = null;
        private byte STX = 0x02;
        private byte ETX = 0x03;
        public int recvInterval = 10;
        private bool pauseFlag = false;
        public int ReaderID { get { return readerID; } }
        public SerialPort ComPortObj { get; set; }
        public string Comport { get; set; }
        public AirDetectFL295CRW(int id,string port)
        {
            this.Comport = port;
            readerID = id;
            devName = "气密检测仪" + readerID.ToString();
            detectData = new List<byte>();
            recvExit = false;
            recvThread = new Thread(new ThreadStart(ComRecvProc));
            recvThread.IsBackground = true;
            recvThread.Priority = ThreadPriority.Highest;
            recvThread.Name = string.Format("气密仪{0}接收线程", this.readerID);
        }
        public bool StartMonitor(ref string reStr)
        {
            try
            {
                string[] ports = System.IO.Ports.SerialPort.GetPortNames();
                if (!ports.Contains(this.Comport))
                {
                    reStr = string.Format("{0} 口不存在", this.Comport);
                    return false;
                }
                if (this.ComPortObj == null)
                {
                    this.ComPortObj = new SerialPort(this.Comport, 9600, Parity.None, 8, StopBits.One);
                }
                if (!this.ComPortObj.IsOpen)
                {
                    this.ComPortObj.Open();
                }
                this.pauseFlag = false;
                if (this.recvThread.ThreadState == (ThreadState.Background | ThreadState.Unstarted))
                {
                    recvThread.Start();
                }
                return true;
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return false;
            }  
          
        }
        public void StopMonitor()
        {
            this.pauseFlag = true;
        }
        /// <summary>
        /// 开始检测
        /// </summary>
        /// <param name="reStr"></param>
        /// <returns></returns>
        public bool StartDetect(ref string reStr)
        {
            if(!ComPortObj.IsOpen)
            {
                reStr = devName + " 串口未打开";
                return false;
            }
            byte[] sndBuf= new byte[1]{0x23};
            try
            {
                detectDataOK = false;
             
                detectData.Clear();
                detectBegin = true;
                ComPortObj.Write(sndBuf, 0, 1);
                return true;
            }
            catch (Exception ex)
            {
                reStr = devName + ex.ToString();
                return false;
            }
           
        }
        public AirlossDetectModel QueryResultData(ref string reStr)
        {
            //模拟测试
            //detectDataOK = true;
            ////D[1],序号
            ////D[2],组号
            ////D[3],DLY
            ////D[4],CHG
            ////D[5],BAL
            ////D[6],DET
            ////D[7],EXH
            ////D[8],检测值
            ////D[9],单位
            ////D[10],结果
            //List<byte> testBuf = new List<byte>();
            //string str = "      12";
            //testBuf.AddRange(System.Text.UTF8Encoding.Default.GetBytes(str));//序号
            //str = "      02";
            //testBuf.AddRange(System.Text.UTF8Encoding.Default.GetBytes(str));//序号
            //for (int i = 0; i < 5;i++ )
            //{
            //    str = "      02";
            //    testBuf.AddRange(System.Text.UTF8Encoding.Default.GetBytes(str));//序号
            //}
            //str = "-  42.02";
            //testBuf.AddRange(System.Text.UTF8Encoding.Default.GetBytes(str));//检测值
            //str = "      00";
            //testBuf.AddRange(System.Text.UTF8Encoding.Default.GetBytes(str));//单位
            //str = "      05";
            //testBuf.AddRange(System.Text.UTF8Encoding.Default.GetBytes(str)); //结果
            //detectData = testBuf;

            if (!detectDataOK)
            {
                reStr = "检测数据未接收完";
                return null;
            }
            
            if(detectData.Count()<80)
            {
                reStr = "检测数据长度不足80字节,实际字节数："+detectData.Count().ToString();
                return null;
            }
            try
            {
                AirlossDetectModel model = new AirlossDetectModel();
                List<byte> unitBytes = new List<byte>();
                List<byte> detectValBytes = new List<byte>();
                List<byte> detectResultBytes = new List<byte>();
                for (int i = 56; i < 64;i++ )
                {
                    detectValBytes.Add(detectData[i]);
                }
                for (int i = 64; i < 72;i++ )
                {
                    unitBytes.Add(detectData[i]);
                }
                for (int i = 72; i < 80;i++ )
                {
                    detectResultBytes.Add(detectData[i]);
                }
                string str=System.Text.Encoding.Default.GetString(detectValBytes.ToArray());
                str=Regex.Replace(str, @"\s", "");
                model.DetectVal = float.Parse(str);
                str = System.Text.Encoding.Default.GetString(unitBytes.ToArray());
                str = Regex.Replace(str, @"\s", "");
                int unitVal = int.Parse(str);
                str = System.Text.Encoding.Default.GetString(detectResultBytes.ToArray());
                str = Regex.Replace(str, @"\s", "");
                int detectResult = int.Parse(str);
                if(unitVal<0 || unitVal>2)
                {
                    reStr = "数据错误，单位值超出范围（0~2）";
                    return null;
                }
                if(detectResult<1 || detectResult >7)
                {
                    reStr = "数据错误，结果值超出范围（1~7）";
                    return null;
                }
                string[] unitEnum = new string[3] { "mL/min", "Pa", "mmH20" };
                string[] detectResultEnum = new string[8] { "", "PNG", "+-NGP", "+NG", "-NG", "OK", "+0NG", "-0NG" };
                model.UnitDesc = unitEnum[unitVal];
                model.DetectResult = detectResultEnum[detectResult];
                return model;
            }
            catch (Exception ex)
            {
                reStr = ex.ToString();
                return null;
                
            }
           
          //  throw new NotImplementedException();
        }
        //public bool SndQueryCmd(ref string reStr)
        //{
        //    if (!ComPortObj.IsOpen)
        //    {
        //        reStr = devName + " 串口未打开";
        //        return false;
        //    }
        //    byte[] sndBuf = new byte[1] { 0x32 };
        //    try
        //    {
        //        detectDataOK = false;
        //        detectBegin = false;
        //        detectData.Clear();
        //        ComPortObj.Write(sndBuf, 0, 1);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        reStr = devName + ex.ToString();
        //        return false;
        //    }
        //}
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
                if (!ComPortObj.IsOpen)
                {
                    continue;
                }
                int recvLen = 0;
                recvLen = ComPortObj.Read(buf, 0, 256);
                for(int i=0;i<recvLen;i++)
                {
                    if(buf[i] == STX)
                    {
                        detectBegin = true;
                        continue;
                    }
                    if(buf[i]== ETX)
                    {
                        detectDataOK = true;
                        break;
                    }
                    if (detectBegin)
                    {
                        detectData.Add(buf[i]);
                    }
                    if (detectData.Count() > 79)
                    {
                        detectDataOK = true;
                        break;
                    }
                   
                }
            }
        }
    }
}
