using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DevAccess
{
    public class DataConvert
    {
        
        public static int AsciiBytesToVerfiyInt(byte[] bytes)
        {
            string assciStr = System.Text.Encoding.ASCII.GetString(bytes).ToUpper();
            int convertInt = 0;
            convertInt = Convert.ToInt32(assciStr, 16);
            //int len = int.Parse(assciStr);
            //bool convertStatus = int.TryParse(assciStr, out convertInt);

            return convertInt;
        }
         public static string AsciiBytesToHex(byte[] bytes)
        {
            string hexStr = System.Text.Encoding.ASCII.GetString(bytes);

            return hexStr;
        }

        public static  int AsciiBytesToInt(byte[] bytes)
        {
            string assciStr = System.Text.Encoding.ASCII.GetString(bytes);
            int convertInt = 0;
            //convertInt = Convert.ToInt32(assciStr, 16);
            
            bool convertStatus = int.TryParse(assciStr, out convertInt);

            return convertInt;
        }

        public static double AsciiBytesToDouble(byte[] bytes)
        {
            string assciStr = System.Text.Encoding.ASCII.GetString(bytes);
            double convertDouble = 0;
            bool convertStatus = double.TryParse(assciStr, out convertDouble);

            return convertDouble;
        }

        public static string AsciiStringToString(string ascStr)
        {
            string numStr = "";
            for(int i=0;i<ascStr.Length/2;i++)
            {
                string ascValueStr = ascStr.Substring(i * 2, 2);
                int ascIntValue = Convert.ToInt32(ascValueStr, 16);
                char ascChar = (char)ascIntValue;
                numStr += ascChar;
            }

            return numStr.ToString();
        }
        public static byte[] IntToOrderLength(int length)
        {
            List<byte> assciList = new List<byte>();
            string valueStr = length.ToString().PadLeft(4,'0');
            foreach (char c in valueStr)
            {
                assciList.Add((byte)c);
            }
            return assciList.ToArray();
        }
        public static byte[] DoubleToAssci(string valueStr)
        {
            List<byte> assciList = new List<byte>();
            //string valueStr = value.ToString();
            foreach (char c in valueStr)
            {
                assciList.Add((byte)c);
            }
            return assciList.ToArray();
        }

        public static string StringToHexString(string s, Encoding encode)
        {
            byte[] b = encode.GetBytes(s);//按照指定编码将string编程字节数组
            string result = string.Empty;
            for (int i = 0; i < b.Length; i++)//逐字节变为16进制字符，以%隔开
            {
                result += Convert.ToString(b[i], 16);
            }
            return result;
        }

        public static string HexStringToString(string hs, Encoding encode)
        {
            //以%分割字符串，并去掉空字符
            string[] chars = hs.Split(new char[] { '%' }, StringSplitOptions.RemoveEmptyEntries);
            byte[] b = new byte[chars.Length];
            //逐个字符变为16进制字节数据
            for (int i = 0; i < chars.Length; i++)
            {
                b[i] = Convert.ToByte(chars[i], 16);
            }
            //按照指定编码将字节数组变为字符串
            return encode.GetString(b);
        }

        /// <summary>
        /// 获取校验码
        /// ascii码加和
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static List<byte> GetCheckCode(byte[] bytes)
        {
            List<byte> checkCodeList = new List<byte>();
            string checkCode = "";
            int sumByte = 0;
            for (int i = 0; i < bytes.Count(); i++)
            {
                sumByte += bytes[i];
            }

            checkCode = Convert.ToString(sumByte, 16).PadLeft(4, '0').ToUpper();
            foreach (char c in checkCode)
            {
                checkCodeList.Add((byte)c);
            }
            return checkCodeList;
        }

        /// <summary>
        /// 字符串转换为byte数组
        /// 如：11-->3131-->byte[0]=48,byte[1]=48
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<byte> StringToAssciBytes(string str)
        {
            List<byte> assciBytes = new List<byte>();
            foreach (char c in str)
            {
                assciBytes.Add((byte)c);
            }
            return assciBytes;
        }

        /// <summary>
        /// 字符串转16进制字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] StrToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        /// <summary>
        /// 字节数组转16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ByteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr +=bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }
        public static string ReplaceChar(string str)
        {
            string resultStr ="";
            for (int i = 0; i < str.Length; i++)
            {
                string strChar = str[i].ToString();
                if (Regex.Match(strChar, "^[a-zA-Z]+$").Success == false)
                {
                    resultStr += strChar;
                }
                else
                {
                    break;//遇到字母就退出
                }
            }
            return resultStr;
        }
        public static double ConvertPercentToDouble(string percentStr)
        {
            string[] valueArr = percentStr.Split('%');
            double percentValue = double.Parse(valueArr[0]) / 100;
            return percentValue;
        }
        /// <summary>
        /// 转换为PLCCIO区地址
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public static string ConvertCIOAddr(float addr)
        {
            string addrStr = addr.ToString("0.00");
            string addrShort = addrStr.Split('.')[0];
            string addDecimal = addrStr.Split('.')[1];
            string addrShortHex = Convert.ToString(short.Parse(addrShort), 16).PadLeft(4, '0').ToUpper();
            string addDecimalHex = Convert.ToString(short.Parse(addDecimal), 16).PadLeft(2, '0').ToUpper();

            return addrShortHex + addDecimalHex;
        }
        /// <summary>
        /// 转换为PLCDM区地址
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public static string ConvertDMAddr(short addr)
        {
            string addHexStr = Convert.ToString(addr, 16).PadLeft(4, '0').ToUpper();

            string hex = addHexStr + "00";

            return hex;
        }

        public static string AcciiStringToHexStr(string acciiStr)
        {
            byte[] acciiBytes = System.Text.ASCIIEncoding.UTF8.GetBytes(acciiStr);
            string hexStr = ByteToHexStr(acciiBytes);
            return hexStr;

        }

    }
}
