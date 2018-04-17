using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DevInterface;
using LogInterface;
namespace PLProcessModel
{
    /// <summary>
    /// 任务运行建模，相当于操作系统的线程
    /// 封装了任务的操作接口，
    /// 可以动态的增加工位节点，在每个循环内对工位节点执行控制流程
    /// </summary>
    public class ThreadRunModel : ThreadBaseModel,ILogRequired
    {
        #region 内部数据
       
        private List<CtlNodeBaseModel> nodeList = null;
        private Int64 lastPlcStat = 0;
        #endregion
        #region 属性
        public DelegateThreadRoutine2 threadRoutine2 = null;
        public List<CtlNodeBaseModel> NodeList { get { return nodeList; } }

       // public int ThreadID { get { return threadID; } }
        //public int LoopInterval { get { return loopInterval; } set { this.loopInterval = value; } }
      //  public ILogRecorder LogRecorder { get { return logRecorder; } set { logRecorder = value; } }
        #endregion
        #region 公开接口
        public ThreadRunModel(int id,string taskName):base(id,taskName)
        {
            this.threadID = id;
            this.taskName = taskName;
            this.nodeList = new List<CtlNodeBaseModel>();
        }
        //public bool TaskInit(ref string reStr)
        //{
        //    this.threadHandler = new Thread(new ThreadStart(TaskloopProc));
        //    this.threadHandler.IsBackground = true;
        //    this.threadHandler.Name = this.taskName;
        //    this.pauseFlag = false;
        //    this.exitRunning = false;
        //    return true;
        //}
        //public bool TaskExit(ref string reStr)
        //{
        //    this.exitRunning = true;
        //    if(threadHandler.ThreadState == (ThreadState.Running | ThreadState.Background))
        //    {
        //        if (!threadHandler.Join(500))
        //        {
        //            threadHandler.Abort();
        //        }
        //    }
           
        //    return true;
        //}
        //public bool TaskStart(ref string reStr)
        //{
        //    this.pauseFlag = false;
        //    if(this.threadHandler.ThreadState == (ThreadState.Unstarted | ThreadState.Background))
        //    {
        //        this.threadHandler.Start();
        //    }
           
        //    return true;
        //}
        //public bool TaskPause(ref string reStr)
        //{
        //    this.pauseFlag = true;
        //    return true;
        //}
        //public bool TaskResume(ref string reStr)
        //{
        //    return true;
        //}
        //public CtlNodeBaseModel FindNode(string nodeName)
        //{
        //    return null;
        //}
        public bool InsertNode(int insertIndex,CtlNodeBaseModel node,ref string reStr)
        {
            if(this.nodeList.Count()<insertIndex)
            {
                reStr = "线程：" + this.taskName + " 插入控制节点失败，插入索引越界";
                return false;
            }
            this.nodeList.Insert(insertIndex,node);
            return true;
        }
        public void AddNode(CtlNodeBaseModel node)
        {
            this.nodeList.Add(node);
        }
        //public bool DelNode(string nodeName,ref string reStr)
        //{
        //    return false;
        //}

        #endregion
        #region 内部接口
        protected override void TaskloopProc()
        {
            while (!exitRunning)
            {
                Thread.Sleep(loopInterval);
                if (pauseFlag)
                {
                    continue;
                }
               
               // logRecorder.AddDebugLog("线程：" + threadID,"线程：" + threadID+"循环开始：");

                IPlcRW plcRW = null;
                if (!SysCfgModel.PlcCommSynMode)
                {
                    plcRW = nodeList[0].PlcRW;

                    if (!SysCfgModel.SimMode)
                    {
                        if (lastPlcStat == plcRW.PlcStatCounter)
                        {
                            continue;
                        }
                    }
                }
                for (int nodeIndex = 0; nodeIndex < nodeList.Count(); nodeIndex++)
                {
                  
                    DateTime commSt = System.DateTime.Now;
                    CtlNodeBaseModel node = nodeList[nodeIndex];
                    try
                    {
                        string reStr = "";
                        
                        //if (!node.ReadDB1())
                        //{
                        //    continue;
                        //}
                       // DateTime commSt = System.DateTime.Now;
                        if (!node.ReadDB2(ref reStr))
                        {
                            continue;
                        }
                        if (!node.ExeBusiness(ref reStr))
                        {
                            continue;
                        }

                       // commSt = System.DateTime.Now;
                        if (!node.NodeCmdCommit(true, ref reStr))
                        {
                            continue;
                        }

                        DateTime commEd = System.DateTime.Now;
                        TimeSpan ts = commEd - commSt;
                        //   string dispCommInfo = string.Format("PLC控制周期:{0}毫秒", (int)ts.TotalMilliseconds);
                        if (ts.TotalMilliseconds > 500)
                        {
                            // node.LogRecorder.AddDebugLog(node.NodeName, dispCommInfo);
                            node.CurrentStat.StatDescribe = string.Format("周期:{0}毫秒", (int)ts.TotalMilliseconds);
                        }
                        
                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(node.NodeName + ex.ToString());
                        node.ThrowErrorStat(ex.ToString(), EnumNodeStatus.设备故障);
                    }

                }
                //if (!SysCfgModel.PlcCommSynMode)
                //{
                //    lastPlcStat = plcRW.PlcStatCounter;
                //}

                //if (threadID == 1)
                //{
                //    Console.WriteLine("线程：" + threadID + "循环结束");
                //}
               
            }
        }
        #endregion
    }
}
