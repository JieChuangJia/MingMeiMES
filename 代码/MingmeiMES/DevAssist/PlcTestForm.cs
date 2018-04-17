using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PLProcessModel;
using DevAccess;
using System.Threading;
namespace DeviceAssist
{
    public partial class PlcTestForm : Form
    {
        ThreadBaseModel autoReadThread = null;
        ThreadBaseModel autoWriteThread = null;
        PLCRWMx plcRwIF = null;
        protected Thread threadHandler = null; //线程句柄
        protected bool exitRunning = false; //退出标志
        protected bool pauseFlag = false;//暂停标志
        protected int loopInterval = 100; //任务循环周期
        
        public PlcTestForm()
        {
            InitializeComponent();
        }

        private void PlcTestForm_Load(object sender, EventArgs e)
        {
            string reStr="";
            Console.SetOut(new TextBoxWriter(this.richTextBoxLog));
            autoReadThread = new ThreadBaseModel(1, "自动读线程");
            autoReadThread.TaskInit(ref reStr);
            autoReadThread.SetThreadRoutine(new DelegateThreadRoutine(AutoReadPLC));
            autoWriteThread = new ThreadBaseModel(2, "自动写线程");
            autoReadThread.LoopInterval = 100;
            autoWriteThread.LoopInterval = 100;
            autoWriteThread.TaskInit(ref reStr);
            autoWriteThread.SetThreadRoutine(new DelegateThreadRoutine(AutoWritePLC));
            plcRwIF = new PLCRWMx();

            plcRwIF.hostControl = this;
            this.threadHandler = new Thread(new ThreadStart(TaskloopProc));
            this.threadHandler.IsBackground = true;

            this.pauseFlag = false;
            this.exitRunning = false;
            
        }
        private int counter = 0;

        private void dlgtReadPLC()
        {
            string addr = "D1000";
            short[] vals = new short[10];
            int val = 0;
           //  if (!plcRwIF.ReadMultiDB(addr, 10, ref vals))
            if (!plcRwIF.ReadDB(addr, ref val))
            {
                Console.WriteLine("PLC 读取地址：" + addr + "失败");

            }
        }
        private void AutoReadPLC()
        {
            
            if (this.textBoxPlcVal.InvokeRequired)
            {
                //dlgtReadPLC();
                DelegateThreadRoutine dlgt = new DelegateThreadRoutine(AutoReadPLC);
                this.Invoke(dlgt, new object[] { });
            }
            else
            {
                try
                {
                    string addr = this.textBoxPlcAddr.Text;
                    int val = 0;
                    if (!plcRwIF.ReadDB(addr, ref val))
                    {
                        Console.WriteLine("PLC 读取地址：" + addr + "失败");

                    }
                    else
                    {
                        this.textBoxPlcVal.Text = val.ToString();
                        counter++;
                        this.label1.Text = string.Format("执行次数：{0}", counter);
                    }
                   
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());

                }
            }

           
           
        }
        private void AutoWritePLC()
        {
            //Console.WriteLine("开始写");
            //short[] vals = new short[100];
            //string addr = "D2000";
            //if (!plcRwIF.WriteMultiDB(addr, 100, vals))
            //{
            //    Console.WriteLine("写入地址：" + addr + "失败");
            //}
            //else
            //{
            //    counter++;
            //    this.label1.Text = string.Format("执行次数：{0}", counter);
            //}
            //Console.WriteLine("写完成");
            if(label1.InvokeRequired)
            {
                DelegateThreadRoutine dlgt = new DelegateThreadRoutine(AutoWritePLC);
                this.Invoke(dlgt, new object[] { });
                
            }
            else
            {
                try
                {
                    string addr = this.textBoxPlcAddr.Text;
                    int val = int.Parse((this.textBoxPlcVal.Text));
                    Console.WriteLine("开始写");
                    if (!plcRwIF.WriteDB(addr, val))
                    {
                        Console.WriteLine("写入地址：" + addr + "失败");
                    }
                    else
                    {
                        counter++;
                        this.label1.Text = string.Format("执行次数：{0}", counter);
                    }
                    Console.WriteLine("写完成");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
               
            }
            

        }

        private void buttonConnectPlc_Click(object sender, EventArgs e)
        {
            string PlcIP = this.textBoxPlcIP.Text;//ConfigurationManager.AppSettings["plcIP"];
            string PlcPort = this.textBoxPlcPort.Text;// ConfigurationManager.AppSettings["plcPort"];
            string plcAddr = PlcIP + ":" + PlcPort;
            string reStr="";
            plcRwIF.ConnStr = plcAddr;
           // if(!plcRwIF.ConnectPLC(ref reStr))
            if(!plcRwIF.dlgtConnPLC(ref reStr))
            {
                Console.WriteLine("连接失败:" + reStr);
            }
            else
            {
                Console.WriteLine("连接成功");
            }
        }

        private void buttonClosePlc_Click(object sender, EventArgs e)
        {
            plcRwIF.CloseConnect();
        }

        private void buttonReadPlc_Click(object sender, EventArgs e)
        {
            string reStr = "";
            autoReadThread.TaskStart(ref reStr);
            this.pauseFlag = false;
            if (this.threadHandler.ThreadState == (ThreadState.Unstarted | ThreadState.Background))
            {
                this.threadHandler.Start();
            }
        }

        private void buttonWritePlc_Click(object sender, EventArgs e)
        {
            string reStr = "";
            autoWriteThread.TaskStart(ref reStr);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //this.pauseFlag = true;
            string reStr = "";
            autoReadThread.TaskPause(ref reStr);
            autoWriteThread.TaskPause(ref reStr);
        }

        private void PlcTestForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            string reStr="";
            if(autoWriteThread != null)
            {
                this.autoWriteThread.TaskExit(ref reStr);
            }
            if(autoReadThread != null)
            {
                this.autoReadThread.TaskExit(ref reStr);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.richTextBoxLog.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AutoReadPLC();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AutoWritePLC();
        }
        protected virtual void TaskloopProc()
        {
            while (!exitRunning)
            {
                Thread.Sleep(loopInterval);
                if (pauseFlag)
                {
                    continue;
                }
                short[] vals = new short[10];
              //  int val = 0;
                string addr = "D1000";
                if (!plcRwIF.ReadMultiDB(addr,10,ref vals))
               // if (!plcRwIF.ReadDB(addr, ref val))
                {
                    Console.WriteLine("读错误");
                }
            }
        }
    }
}
