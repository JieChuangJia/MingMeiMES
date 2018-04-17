using DevInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevAccess
{
    public class OMLFinsTCPProtocol:IOMLFins
    {
        private const string FINSHEAD = "46494E53";      //FINS头
        private const string FINSWRCMD = "00000002";       //读写命令码
        private const string FINSHANDCMD = "00000000";       //握手信号命令码
        private const string FINSERRORCODE = "00000000"; //错误码
        private const string FINSHEADCODE = "800002";    //FINS协议体头
        private const string SID = "00";                 //FINS协议体sid
        private const string WRITEORDERTYPE = "0102";    //写命令
        private const string READORDERTYPE = "0101";    //读取命令
        private const string DMADDRAREA = "82";        //DM区地址
        private const string CIOADDRAREA = "31";        //CIO区地址
        private const string RIGHTRESPONSE = "0000";    //命令正确回复
        private string PCIP { get; set; }
        private string PLCIP { get; set; }
        private int PCNodeNum 
        {
            get { 
                int pcNode = int.Parse(this.PCIP.Split('.')[3]);
                return pcNode;
            }
        }
        private int PLCNodeNum
        {
            get
            {
                int plcNode = int.Parse(this.PLCIP.Split('.')[3]);
                return plcNode;
            }
        }
        private string PLCNode
        {
            get 
            {
                try
                {
                    string plcNode = this.PLCIP.Split('.')[3];
                    string plcNodeStr = DataConvert.ConvertDMAddr(short.Parse(plcNode));
                    return plcNodeStr;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        private string PCNode
        {
            get
            {
                try
                {
                    string pcNode = this.PCIP.Split('.')[3];
                    string pcNodeStr = DataConvert.ConvertDMAddr(short.Parse(pcNode));
                    return pcNodeStr;
                }
                catch
                {
                    return string.Empty;
                }

            }
        }

        public OMLFinsTCPProtocol(string pcIP,string plcIP)
        {
            this.PCIP = pcIP;
            this.PLCIP = plcIP;
        }
        /// <summary>
        /// 获取握手命令
        /// </summary>
        /// <returns></returns>
        public bool HandCmd(ref byte[] handCmd,ref string restr)
        {
            try
            {
                if (this.PLCIP.Split('.').Length != 4)
                {
                    return false;
                }
                if (this.PCIP.Split('.').Length != 4)
                {
                    return false;
                }
                string length = "0000000C";//固定

                string handCmdStr = FINSHEAD + length + FINSHANDCMD + FINSERRORCODE + this.PCNodeNum.ToString("X2").PadLeft(8, '0');
                handCmd = DataConvert.StrToToHexByte(handCmdStr);
             
                return true;
            }
            catch(Exception ex)
            {
                restr = ex.Message;
                return false;
            }
           
        }

        public bool HandCmdResponse(byte[] hanCmdResponse,ref string restr)
        {
            try
            {
                string wrDMStr = DataConvert.ByteToHexStr(hanCmdResponse);
                
                if (wrDMStr.Length != 48)
                {
                    restr = "返回命令长度错误！";
                    return false;
                }

                //比对下地址是否一致，加上返回码0000
                string finsHeader = wrDMStr.Substring(0, 8).ToUpper();
                if (finsHeader != FINSHEAD)
                {
                    restr = "FINS协议头错误！";
                    return false;
                }
                string plcIP = wrDMStr.Substring(40, 8).ToUpper();
              
                int recPlcNode = Convert.ToInt32(plcIP, 16);
                if (this.PLCNodeNum != recPlcNode)
                {
                    restr = "IP地址错误！";
                    return false;
                }
             
                return true;
            }
            catch(Exception ex)
            {
                restr = ex.Message;
                return false;
            }
        }
        /// <summary>
        /// 向DM地址写入值命令
        /// </summary>
        /// <param name="startAddr"></param>
        /// <param name="shortValue"></param>
        /// <param name="writeDmCmdBytes"></param>
        /// <param name="restr"></param>
        /// <returns></returns>
        public bool WriteDMAddrCmd(short startAddr, short[] shortValue, ref byte[] writeDmCmdBytes, ref string restr)
        {
          
            try
            {
                string writeDmStr = "";
                string finsLength =Convert.ToString( (26 + shortValue.Length*2),16).PadLeft(8,'0').ToUpper();

                string addrStr = DataConvert.ConvertDMAddr(startAddr);
                string addrCountStr = Convert.ToString((short)shortValue.Count(), 16).PadLeft(4, '0');

                writeDmStr = FINSHEAD + finsLength + FINSWRCMD + FINSERRORCODE + FINSHEADCODE + this.PLCNode + this.PCNode + SID
                    + WRITEORDERTYPE + DMADDRAREA + addrStr + addrCountStr;

                for (int i = 0; i < shortValue.Length; i++)
                {
                    string valueStr = Convert.ToString(shortValue[i], 16).PadLeft(4, '0');
                    writeDmStr += valueStr;
                }
                writeDmCmdBytes = DataConvert.StrToToHexByte(writeDmStr);

                return true;
            }
            catch(Exception ex)
            {
                restr = ex.Message;
                return false;
            }
        }

        public bool WriteDMAddrCmdResponse(byte[] writeDMResponse,ref string restr)
        {
            string wrDMStr = DataConvert.ByteToHexStr(writeDMResponse);
            //wrDMStr = "46494E530000001C0000000200000000C000020003000001000001020000";
            if (wrDMStr.Length != 60)
            {
                restr = "返回命令长度错误！";
                return false;
            }
         
            //比对下地址是否一致，加上返回码0000
            string finsHeader = wrDMStr.Substring(0, 8).ToUpper();
            if(finsHeader != FINSHEAD)
            {
                restr = "FINS协议头错误！";
                return false;
            }
            string orderType = wrDMStr.Substring(52, 4).ToUpper();
            if(orderType != WRITEORDERTYPE)
            {
                restr = "命令类型错误！";
                return false;
            }
            string response = wrDMStr.Substring(56, 4).ToUpper();
            if (response != RIGHTRESPONSE)
            {
                restr = "命令返回错误！";
                return false;
            }
            return true;
        }
        public bool ReadDMAddrCmdResponse(byte[] readDMBytes, short readCount, ref int[] values, ref string restr)
        {
            //暂未实现
            return false;
        }
        public bool WriteDMAddrCmd(short startAddr, int[] intValue, ref byte[] writeDmCmdBytes, ref string restr)
        {
            //暂未实现
            return true;
        }
        /// <summary>
        /// 读取DM地址命令
        /// </summary>
        /// <param name="startAddr"></param>
        /// <param name="readAddrCount"></param>
        /// <param name="cmdBytes"></param>
        /// <param name="restr"></param>
        /// <returns></returns>
        public bool ReadDMAddrCmd(short startAddr,short readAddrCount,ref byte [] cmdBytes,ref string restr)
        {

            try
            {
                string readDmStr = "";
                string finsLenth= "0000001A";//读取命令长度固定
                string addrStr = DataConvert.ConvertDMAddr(startAddr);
                string addrCountStr = Convert.ToString(readAddrCount, 16).PadLeft(4, '0');

                readDmStr = FINSHEAD + finsLenth + FINSWRCMD + FINSERRORCODE + FINSHEADCODE + this.PLCNode + this.PCNode + SID
                    + READORDERTYPE + DMADDRAREA + addrStr + addrCountStr;

                cmdBytes = DataConvert.StrToToHexByte(readDmStr);

                return true;
            }
            catch (Exception ex)
            {
                restr = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 读取DM地址反馈
        /// </summary>
        /// <param name="readDMResponse"></param>
        /// <param name="readAddrCount"></param>
        /// <param name="readValues"></param>
        /// <param name="restr"></param>
        /// <returns></returns>
        public bool ReadDMAddrCmdResponse(byte[] readDMBytes, short readCount, ref short[] values, ref string restr)
        {
            string rdDMStr = DataConvert.ByteToHexStr(readDMBytes);
            //rdDMStr = "46494E530000001A0000000200000000C0000200C000002100000101000011223344";
            if (rdDMStr.Length != 60 + readCount * 4)
            {
                restr = "返回命令长度错误！";
                return false;
            }
         
            //比对下地址是否一致，加上返回码0000
            string finsHeader = rdDMStr.Substring(0, 8).ToUpper();
            if(finsHeader != FINSHEAD)
            {
                restr = "FINS协议头错误！";
                return false;
            }
            string orderType = rdDMStr.Substring(52, 4).ToUpper();
            if(orderType != READORDERTYPE)
            {
                restr = "命令类型错误！";
                return false;
            }
            string response = rdDMStr.Substring(56, 4).ToUpper();
            if (response != RIGHTRESPONSE)
            {
                restr = "命令返回错误！";
                return false;
            }
            values = new short[readCount];
            for (int i = 0; i < readCount; i++)
            {
                string hexValue = rdDMStr.Substring(60+i*4,4);
                values[i] = Convert.ToInt16(hexValue, 16);
            }
            return true;
        }
        /// <summary>
        /// 读取CIO区地址命令
        /// </summary>
        /// <param name="startAddr"></param>
        /// <param name="readAddrCount"></param>
        /// <param name="cmdBytes"></param>
        /// <param name="restr"></param>
        /// <returns></returns>
        public bool ReadCIOAddrCmd(float startAddr, short readAddrCount, ref byte[] cmdBytes, ref string restr)
        {
            try
            {
                string readDmStr = "";
                string finsLenth = "0000001A";//读取命令长度固定26
                string addrStr = DataConvert.ConvertCIOAddr(startAddr);
                string addrCountStr = Convert.ToString(readAddrCount, 16).PadLeft(4, '0');

                readDmStr = FINSHEAD + finsLenth + FINSWRCMD + FINSERRORCODE + FINSHEADCODE + this.PLCNode + this.PCNode + SID
                    + READORDERTYPE + CIOADDRAREA + addrStr + addrCountStr;

                cmdBytes = DataConvert.StrToToHexByte(readDmStr);

                return true;
            }
            catch (Exception ex)
            {
                restr = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 读取CIO区反馈
        /// </summary>
        /// <param name="readCIOResponse"></param>
        /// <param name="readAddrCount"></param>
        /// <param name="readValues"></param>
        /// <param name="restr"></param>
        /// <returns></returns>
        public bool ReadCIOAddrCmdResponse(byte[] readCIOResponse, short readAddrCount, ref short[] readValues, ref string restr)
        {
            string rdCIOStr = DataConvert.ByteToHexStr(readCIOResponse);
            //rdCIOStr = "46494E530000001A0000000200000000C0000200C00000210000010100000102";
            if (rdCIOStr.Length != 60 + readAddrCount * 2)
            {
                restr = "返回命令长度错误！";
                return false;
            }

            //比对下地址是否一致，加上返回码0000
            string finsHeader = rdCIOStr.Substring(0, 8).ToUpper();
            if (finsHeader != FINSHEAD)
            {
                restr = "FINS协议头错误！";
                return false;
            }
            string orderType = rdCIOStr.Substring(52, 4).ToUpper();
            if (orderType != READORDERTYPE)
            {
                restr = "命令类型错误！";
                return false;
            }
            string response = rdCIOStr.Substring(56, 4).ToUpper();
            if (response != RIGHTRESPONSE)
            {
                restr = "命令返回错误！";
                return false;
            }
            readValues = new short[readAddrCount];
            for (int i = 0; i < readAddrCount; i++)
            {
                string hexValue = rdCIOStr.Substring(60 + i * 2, 2);
                readValues[i] = Convert.ToInt16(hexValue, 16);
            }
            return true;
        }
        /// <summary>
        /// 向CIO区写入地址命令
        /// </summary>
        /// <param name="startAddr"></param>
        /// <param name="bitValue"></param>
        /// <param name="cmdBytes"></param>
        /// <param name="restr"></param>
        /// <returns></returns>
        public bool WriteCIOAddrCmd(float startAddr, short[] bitValue, ref byte[] cmdBytes, ref string restr)
        {
            try
            {
                string writeCioStr = "";
                string finsLength = Convert.ToString((26 + bitValue.Length ), 16).PadLeft(8, '0').ToUpper();

                string addrStr = DataConvert.ConvertCIOAddr(startAddr);
                string addrCountStr = Convert.ToString((short)bitValue.Count(), 16).PadLeft(4, '0');

                writeCioStr = FINSHEAD + finsLength + FINSWRCMD + FINSERRORCODE + FINSHEADCODE + this.PLCNode + this.PCNode + SID
                    + WRITEORDERTYPE + CIOADDRAREA + addrStr + addrCountStr;

                for (int i = 0; i < bitValue.Length; i++)
                {
                    if (bitValue[i] > 1)
                    {
                        restr = "地址位值错误！";
                        return false;
                    }
                    string valueStr = Convert.ToString(bitValue[i], 16).PadLeft(2, '0');
                    writeCioStr += valueStr;
                }
                cmdBytes = DataConvert.StrToToHexByte(writeCioStr);

                return true;
            }
            catch (Exception ex)
            {
                restr = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 跟写入DM区反馈一致
        /// </summary>
        /// <param name="wrCIOResponse"></param>
        /// <param name="restr"></param>
        /// <returns></returns>
        public bool WriteCIOAddrCmdResponse(byte[] wrCIOResponse,ref string restr)
        {
            string wrDMStr = DataConvert.ByteToHexStr(wrCIOResponse);
            //wrDMStr = "46494E530000001C0000000200000000C000020003000001000001020000";
            if (wrDMStr.Length != 60)
            {
                restr = "返回命令长度错误！";
                return false;
            }

            //比对下地址是否一致，加上返回码0000
            string finsHeader = wrDMStr.Substring(0, 8).ToUpper();
            if (finsHeader != FINSHEAD)
            {
                restr = "FINS协议头错误！";
                return false;
            }
            string orderType = wrDMStr.Substring(52, 4).ToUpper();
            if (orderType != WRITEORDERTYPE)
            {
                restr = "命令类型错误！";
                return false;
            }
            string response = wrDMStr.Substring(56, 4).ToUpper();
            if (response != RIGHTRESPONSE)
            {
                restr = "命令返回错误！";
                return false;
            }
            return true;
        }

       

    }

   
}
