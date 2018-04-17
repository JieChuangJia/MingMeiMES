using DevInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevAccess
{
    public interface IOMLFins
    {
        bool HandCmd(ref byte[] handCmd, ref string restr);
        bool HandCmdResponse(byte[] hanCmdResponse, ref string restr);
        bool WriteDMAddrCmd(short startAddr, short[] shortValue, ref byte[] writeDmCmdBytes, ref string restr);

        bool WriteDMAddrCmd(short startAddr, int[] intValue, ref byte[] writeDmCmdBytes, ref string restr);

        bool WriteDMAddrCmdResponse(byte[] writeDMResponse, ref string restr);
        

        /// <summary>
        /// 读取DM地址命令
        /// </summary>
        /// <param name="startAddr"></param>
        /// <param name="readAddrCount"></param>
        /// <param name="cmdBytes"></param>
        /// <param name="restr"></param>
        /// <returns></returns>
        bool ReadDMAddrCmd(short startAddr, short readAddrCount, ref byte[] cmdBytes, ref string restr);
     
        /// <summary>
        /// 读取DM地址反馈
        /// </summary>
        /// <param name="readDMBytes"></param>
        /// <param name="readCount"></param>
        /// <param name="dataType"></param>
        /// <param name="values"></param>
        /// <param name="restr"></param>
        /// <returns></returns>
        bool ReadDMAddrCmdResponse(byte[] readDMBytes, short readCount, ref int[] values, ref string restr);
        /// <summary>
        /// 读取DM地址反馈
        /// </summary>
        /// <param name="readDMBytes"></param>
        /// <param name="readCount"></param>
        /// <param name="values"></param>
        /// <param name="restr"></param>
        /// <returns></returns>
        bool ReadDMAddrCmdResponse(byte[] readDMBytes, short readCount, ref short[] values, ref string restr);
       
        
        /// <summary>
        /// 读取CIO区地址命令
        /// </summary>
        /// <param name="startAddr"></param>
        /// <param name="readAddrCount"></param>
        /// <param name="cmdBytes"></param>
        /// <param name="restr"></param>
        /// <returns></returns>
        bool ReadCIOAddrCmd(float startAddr, short readAddrCount, ref byte[] cmdBytes, ref string restr);
        /// <summary>
        /// 读取CIO区反馈
        /// </summary>
        /// <param name="readCIOResponse"></param>
        /// <param name="readAddrCount"></param>
        /// <param name="readValues"></param>
        /// <param name="restr"></param>
        /// <returns></returns>
        bool ReadCIOAddrCmdResponse(byte[] readCIOResponse, short readAddrCount, ref short[] readValues, ref string restr);

        /// <summary>
        /// 向CIO区写入地址命令
        /// </summary>
        /// <param name="startAddr"></param>
        /// <param name="bitValue"></param>
        /// <param name="cmdBytes"></param>
        /// <param name="restr"></param>
        /// <returns></returns>
        bool WriteCIOAddrCmd(float startAddr, short[] bitValue, ref byte[] cmdBytes, ref string restr);
       

        /// <summary>
        /// 跟写入DM区反馈一致
        /// </summary>
        /// <param name="wrCIOResponse"></param>
        /// <param name="restr"></param>
        /// <returns></returns>
        bool WriteCIOAddrCmdResponse(byte[] wrCIOResponse, ref string restr);

 
    }
}
