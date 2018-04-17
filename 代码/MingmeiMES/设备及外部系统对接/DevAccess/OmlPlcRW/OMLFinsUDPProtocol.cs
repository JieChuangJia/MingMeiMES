using DevInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevAccess
{
    public class OMLFinsUDPProtocol:IOMLFins
    {
        private const string FINSHEADCODE = "800002";    //FINS协议体头
        private const string SID = "00";                 //FINS协议体sid
        private const string WRITEORDERTYPE = "0102";    //写命令
        private const string READORDERTYPE = "0101";    //读取命令
        private const string DMADDRAREA = "82";        //DM区地址
        private const string CIOADDRAREA = "31";        //CIO区地址
        private const string RESPONSEHEAD = "C00002";  //返回命令头
        private const string RIGHTRESPONSE = "0000";    //命令正确回复
        private string PCIP { get; set; }
        private string PLCIP { get; set; }

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

        public OMLFinsUDPProtocol(string pcIP, string plcIP)
        {
            this.PCIP = pcIP;
            this.PLCIP = plcIP;
        }
        //无握手协议
        public bool HandCmd(ref byte[] handCmd, ref string restr)
        {
            return true;
        }
        public bool HandCmdResponse(byte[] hanCmdResponse, ref string restr)
        {
            return true;
        }

        public bool ReadDMAddrCmd(short startAddr,short readCount,ref byte[] cmdBytes,ref string restr)
        {
            try
            {
                string readDMStr = "";
                string addrCountStr = Convert.ToString((short)readCount, 16).PadLeft(4, '0');
                readDMStr += FINSHEADCODE + this.PLCNode + this.PCNode + SID + READORDERTYPE + DMADDRAREA
                    + DataConvert.ConvertDMAddr(startAddr) + addrCountStr;

                cmdBytes = DataConvert.StrToToHexByte(readDMStr);
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
        /// <param name="readDMBytes"></param>
        /// <param name="readCount"></param>
        /// <param name="dataType"></param>
        /// <param name="values"></param>
        /// <param name="restr"></param>
        /// <returns></returns>
        public bool ReadDMAddrCmdResponse(byte[] readDMBytes, short readCount, ref int[] values, ref string restr)
        {
            try
            {
                string rdDMStr = DataConvert.ByteToHexStr(readDMBytes);
                //object[] readStr = new object[1];
                //readStr[0] = rdDMStr;
                //rdDMStr = "C000020058000035000001010000F9724121";
                

                if (rdDMStr.Length != (28 + 8 * readCount))
                {
                    restr = "返回数据长度错误！";
                    return false;
                }
                string resposeHead = rdDMStr.Substring(0, 6);
                if (resposeHead != RESPONSEHEAD)
                {
                    restr = "返回数据头错误！";
                    return false;
                }
                string readType = rdDMStr.Substring(20, 4);
                if (readType != READORDERTYPE)
                {
                    restr = "返回数据命令错误！";
                    return false;
                }
                string response = rdDMStr.Substring(24, 4);
                if (response != RIGHTRESPONSE)
                {
                    restr = "返回数据返回错误！";
                    return false;
                }
                values = new int[readCount];
                for (int i = 0; i < readCount; i++)
                {
                    string hexValue = rdDMStr.Substring(28 + i * 8, 8);
                    values[i] = ConvertToInt(hexValue);
                }

                return true;
            }
            catch (Exception ex)
            {
                restr = ex.Message;
                return false;
            }
        }
       

        public bool ReadDMAddrCmdResponse(byte[] readDMBytes, short readCount, ref short[] values, ref string restr)
        {
            try
            {
                string rdDMStr = DataConvert.ByteToHexStr(readDMBytes);
                //rdDMStr = "C0000200C00000030000010100000123";
                //rdDMStr = "C00002005800000F00000101000000030004";

                if (rdDMStr.Length != (28+4*readCount))
                {
                    restr = "返回数据长度错误！";
                    return false;
                }
                string resposeHead = rdDMStr.Substring(0, 6);
                if (resposeHead != RESPONSEHEAD)
                {
                    restr = "返回数据头错误！";
                    return false;
                }
                string readType = rdDMStr.Substring(20, 4);
                if (readType != READORDERTYPE)
                {
                    restr = "返回数据命令错误！";
                    return false;
                }
                string response = rdDMStr.Substring(24, 4);
                if (response != RIGHTRESPONSE)
                {
                    restr = "返回数据返回错误！";
                    return false;
                }
                values = new short[readCount];
                for (int i = 0; i < readCount; i++)
                {
                    string hexValue = rdDMStr.Substring(28 + i * 4, 4);
                    values[i] = Convert.ToInt16(hexValue, 16);
                }

                return true;
            }
            catch (Exception ex)
            {
                restr = ex.Message;
                return false;
            }
        }

        public bool WriteDMAddrCmd(short startAddr, short[] shortValue, ref byte[] writeDmCmdBytes, ref string restr)
        {
            try
            {
                string writeDMStr = "";
                string addrCountStr = Convert.ToString((short)shortValue.Length, 16).PadLeft(4, '0');
                writeDMStr += FINSHEADCODE + this.PLCNode + this.PCNode + SID + WRITEORDERTYPE + DMADDRAREA
                    + DataConvert.ConvertDMAddr(startAddr) + addrCountStr;
                for (int i = 0; i < shortValue.Length; i++)
                {
                    string valueStr = Convert.ToString(shortValue[i], 16).PadLeft(4, '0');
                    writeDMStr += valueStr;
                }
                writeDmCmdBytes = DataConvert.StrToToHexByte(writeDMStr);
                return true;
            }
            catch (Exception ex)
            {
                restr = ex.Message;
                return false;
            }
        }

        public bool WriteDMAddrCmd(short startAddr, int[] intValue, ref byte[] writeDmCmdBytes, ref string restr)
        {
            try
            {
                string writeDMStr = "";
                string addrCountStr = Convert.ToString((short)intValue.Length*2, 16).PadLeft(4, '0');
                writeDMStr += FINSHEADCODE + this.PLCNode + this.PCNode + SID + WRITEORDERTYPE + DMADDRAREA
                    + DataConvert.ConvertDMAddr(startAddr) + addrCountStr;
                for (int i = 0; i < intValue.Length; i++)
                {
                    string valueStr = ConvertToHexStr(intValue[i]);
                    writeDMStr += valueStr;
                }
                writeDmCmdBytes = DataConvert.StrToToHexByte(writeDMStr);
                return true;
            }
            catch (Exception ex)
            {
                restr = ex.Message;
                return false;
            }
        }
      
        public bool WriteDMAddrCmdResponse(byte[] writeDMResponse, ref string restr)
        {
            try
            {
                string wrDMStr = DataConvert.ByteToHexStr(writeDMResponse);
                //wrDMStr  = "C0000200C0000003000001020000";
                if (wrDMStr.Length != 28)
                {
                    restr = "返回命令长度错误！";
                    return false;
                }

                //比对下地址是否一致，加上返回码0000
                string finsHeader = wrDMStr.Substring(0, 6).ToUpper();
                if (finsHeader != RESPONSEHEAD)
                {
                    restr = "FINS协议头错误！";
                    return false;
                }
                string orderType = wrDMStr.Substring(20, 4).ToUpper();
                if (orderType != WRITEORDERTYPE)
                {
                    restr = "命令类型错误！";
                    return false;
                }
                string response = wrDMStr.Substring(24, 4).ToUpper();
                if (response != RIGHTRESPONSE)
                {
                    restr = "命令返回错误！";
                    return false;
                }
              
                return true;
            }
            catch (Exception ex)
            {
                restr = ex.Message;
                return false;
            }
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
               
                string addrStr = DataConvert.ConvertCIOAddr(startAddr);
                string addrCountStr = Convert.ToString(readAddrCount, 16).PadLeft(4, '0');

                readDmStr = FINSHEADCODE  + this.PLCNode + this.PCNode + SID
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
            try
            {
                string rdCIOStr = DataConvert.ByteToHexStr(readCIOResponse);
                //rdCIOStr = "C0000200C000000300000101000001";//一个数据的反馈
                //rdCIOStr = "C00002005800000F0000010100000000";

                if (rdCIOStr.Length != (28+readAddrCount*2))
                {
                    restr = "返回命令长度错误！";
                    return false;
                }

                //比对下地址是否一致，加上返回码0000
                string finsHeader = rdCIOStr.Substring(0, 6).ToUpper();
                if (finsHeader != RESPONSEHEAD)
                {
                    restr = "FINS协议头错误！";
                    return false;
                }
                string orderType = rdCIOStr.Substring(20, 4).ToUpper();
                if (orderType != READORDERTYPE)
                {
                    restr = "命令类型错误！";
                    return false;
                }
                string response = rdCIOStr.Substring(24, 4).ToUpper();
                if (response != RIGHTRESPONSE)
                {
                    restr = "命令返回错误！";
                    return false;
                }
                readValues = new short[readAddrCount];
                for (int i = 0; i < readAddrCount; i++)
                {
                    string hexValue = rdCIOStr.Substring(28 + i * 2, 2);
                    readValues[i] = Convert.ToInt16(hexValue, 16);
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
                 
                string addrStr = DataConvert.ConvertCIOAddr(startAddr);
                string addrCountStr = Convert.ToString((short)bitValue.Count(), 16).PadLeft(4, '0');

                writeCioStr = FINSHEADCODE + this.PLCNode + this.PCNode + SID
                    + WRITEORDERTYPE + CIOADDRAREA + addrStr + addrCountStr;

                for (int i = 0; i < bitValue.Length; i++)
                {
                    if(bitValue[i]>1)
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
            catch(Exception ex)
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
        public bool WriteCIOAddrCmdResponse(byte[] wrCIOResponse, ref string restr)
        {
            try
            {
                string wrDMStr = DataConvert.ByteToHexStr(wrCIOResponse);
                //wrDMStr = "C0000200C0000003000001020000";
                            

                if (wrDMStr.Length != 28)
                {
                    restr = "返回命令长度错误！";
                    return false;
                }

                //比对下地址是否一致，加上返回码0000
                string finsHeader = wrDMStr.Substring(0, 6).ToUpper();
                if (finsHeader != RESPONSEHEAD)
                {
                    restr = "FINS协议头错误！";
                    return false;
                }
                string orderType = wrDMStr.Substring(20, 4).ToUpper();
                if (orderType != WRITEORDERTYPE)
                {
                    restr = "命令类型错误！";
                    return false;
                }
                string response = wrDMStr.Substring(24, 4).ToUpper();
                if (response != RIGHTRESPONSE)
                {
                    restr = "命令返回错误！";
                    return false;
                }
              
                return true;
            }
            catch(Exception ex)
            {
                restr= ex.Message;
                return false;
            }
        }

        private string ConvertToHexStr(int value)
        {
            string hexstr = "";
            byte[] intBytes = BitConverter.GetBytes(value);
            string h1 = intBytes[1].ToString("X2");
            string h2 = intBytes[0].ToString("X2");
            string l1 = intBytes[3].ToString("X2");
            string l2 = intBytes[2].ToString("X2");
            hexstr = h1 + h2 + l1 + l2;
            return hexstr;
        }
        private int ConvertToInt(string hexStr)
        {
            List<byte> valueBytes = new List<byte>();
            valueBytes.Add(Convert.ToByte(hexStr.Substring(2, 2), 16));
            valueBytes.Add(Convert.ToByte(hexStr.Substring(0, 2), 16));
            valueBytes.Add(Convert.ToByte(hexStr.Substring(6, 2), 16));
            valueBytes.Add(Convert.ToByte(hexStr.Substring(4, 2), 16));

            int test = BitConverter.ToInt32(valueBytes.ToArray(), 0);
            return test;
        }

    }
}
