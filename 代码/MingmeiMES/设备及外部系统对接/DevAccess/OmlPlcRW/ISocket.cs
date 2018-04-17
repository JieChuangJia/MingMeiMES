using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DevAccess
{
    public interface ISocket
    {
        /// <summary>
        /// 获取是否已连接。
        /// </summary>
        bool IsConnected { get; }
        /// <summary>
        /// 连接至服务器。
        /// </summary>
        /// <param name="endpoint">服务器终结点。</param>
        bool Connect(string ip, int port, ref string reStr);

        /// <summary>
        /// 发送数据。
        /// </summary>
        /// <param name="data">要发送的数据。</param>
        bool Send(byte[] data,ref string reStr);
       
        /// <summary>
        /// 断开连接。
        /// </summary>
        bool Disconnect();
        
        /// <summary>
        /// 接收完成时引发事件。
        /// </summary>
        event EventHandler<SocketEventArgs> ReceiveCompleted;
        
        /// <summary>
        /// 断开连接引发事件
        /// </summary>
        event EventHandler<LostLinkEventArgs> LostLink;
    }
    
    /// <summary>
    /// 断开连接事件
    /// </summary>
     public  class LostLinkEventArgs:EventArgs
     {
         public string IP { get; set; }
         public int Port { get; set; }
     }
    /// <summary>
    /// Socket事件参数
    /// </summary>
    public class SocketEventArgs : EventArgs
    {

        public ISocketAsyncState SockState { get; set; }
        public byte[] RecBytes { get; set; }
     
       
    }
    public interface ISocketAsyncState
    {
       // List<byte> BufferList { get; }
        // int BufferLength { get;  }
        //void AddBuffer(byte[] bytes);
        object SocketLock { get; }
        //void Dequeue(int bytes);
    }
    public class SocketAsyncState : ISocketAsyncState
    {
        private  object socketLock = new object();
        public bool IsCompleted = false;
        public TcpClient client = null;
       
        public const int BufferSize = 1024;
        //public int BufferLength { get { return this.bufferList.Count; } }
        public byte[] buffer = new byte[BufferSize];
    
        //private List<byte> bufferList = new List<byte>();
        //public List<byte> BufferList
        //{
        //    get { return this.bufferList; }
        //}
        public object SocketLock { get { return this.socketLock; } }
        public StringBuilder messageBuffer = new StringBuilder();
        //public void AddBuffer(byte[] bytes)
        //{
        //    lock (socketLock)
        //    {             
        //        this.bufferList.AddRange(bytes);
        //    }

        //}
        //public void Dequeue(int bytes)
        //{
        //    lock (socketLock)
        //    {
        //        this.bufferList.RemoveRange(0, bytes);
        //        //for (int i = 0; i < bytes; i++)
        //        //{
        //        //    if (this.bufferList.Count > 0)
        //        //    {
        //        //        this.bufferList.RemoveAt(0);
        //        //    }
        //        //}
        //    }
        //}
    }
}
