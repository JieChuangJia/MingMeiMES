using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;
using System.Configuration;
using System.Threading;
using SygoleHFReaderIF;
using DevInterface;
using DevAccess;
//using Oracle.DataAccess.Client;
//using Oracle.DataAccess.Types;
//using Oracle.ManagedDataAccess.Client;
namespace DeviceAssist
{
    public partial class DeviceAssistForm : Form
    {
        private PlcTestForm plcForm2 = null;
        private RfidWqwlForm rfidWQForm = null;
        private RfidSgUrb3Form rfidSgUrb3Form = null;
        public delegate void DelegateAddLog(string log);
        private delegate void DelegateRefeshRfid();
        #region 数据区
        
        FTDataAccess.BLL.SysLogBll logBll = new FTDataAccess.BLL.SysLogBll();

        PLCRwMCPro plcRwObj2 = null;
        IPlcRW plcRwIF = null;
        PlcRW485BD plcFx485 = null;
        SgrfidRW rfidRW = null;

        private Thread rfidWorkingThread = null;
        private int rfidRWInterval = 100;
        private bool exitRunning = false;
        private bool pauseFlag = false;
        private Int64 rwCounts = 0;
        private Int64 rwFaileCounts = 0;

        BarcodeRWHonevor barcodeReader = null;
        PrinterRW printer = null;
        PrinterRWdb printerDB = null;
        //private int makeCardCount = 0;
        AirDetectFL295CRW airDetecter = null;
       // OracleConnection conn = new OracleConnection();
        MesDA mesDA = new MesDA();
        #endregion
        public DeviceAssistForm()
        {
            InitializeComponent();
            plcForm2 = new PlcTestForm();
            rfidWQForm = new RfidWqwlForm();
            rfidSgUrb3Form = new RfidSgUrb3Form();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Console.SetOut(new TextBoxWriter(this.richTextBoxLog));
            plcForm2.MdiParent = this;
            this.panelPlc2.Controls.Clear();
            this.panelPlc2.Controls.Add(plcForm2);
            plcForm2.Dock = DockStyle.Fill;
            plcForm2.Show();

            rfidWQForm.MdiParent = this;
            this.panelWqrfid.Controls.Clear();
            this.panelWqrfid.Controls.Add(rfidWQForm);
            rfidWQForm.Dock = DockStyle.Fill;
            rfidWQForm.Show();

            rfidSgUrb3Form.MdiParent = this;
            this.panelSgUrb3.Controls.Clear();
            this.panelSgUrb3.Controls.Add(rfidSgUrb3Form);
            rfidSgUrb3Form.Dock = DockStyle.Fill;
            rfidSgUrb3Form.Show();


            // 隐藏不用 的tabpage
            //this.tabControl1.TabPages.Remove(this.tabPage1);
           // this.tabControl1.TabPages.Remove(this.tabPage3);
            this.tabControl1.TabPages.Remove(this.tabPage5);
            // this.comboBoxPlcObjList.SelectedIndex = 0;
            //  this.tabPage1.Enabled = false;
            this.comboBoxDatabitSel.Items.AddRange(new string[] { "32位整数", "64位整数" });
            this.comboBoxDatabitSel.SelectedIndex = 0;

            #region PLC相关
            this.cbxPlcCata.Items.AddRange(new string[] { "FX5U", "Q系列", "Fx3uNET模块" });
            this.cbxPlcCata.SelectedIndex = 0;
            plcRwObj2 = new PLCRwMCPro(EnumPlcCata.FX5U,1000,1000);
            plcRwObj2.eventLinkLost += PlcLostConnectHandler;
            plcRwIF = plcRwObj2;

            plcFx485 = new PlcRW485BD();
            plcFx485.StationNumber = 1;
            #endregion

           
            HFReaderIF readerIF = new HFReaderIF();
            rfidRW = new SgrfidRW(1);
            rfidRW.ReaderIF = readerIF;
            this.comboBoxComports.Items.Clear();

            barcodeReader = new BarcodeRWHonevor(1);

            string[] ports = System.IO.Ports.SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                this.comboBoxComports.Items.Add(port);
                this.comboBoxFXComs.Items.Add(port);
                this.comboBoxBarcode.Items.Add(port);
                this.comboBoxAirdetect.Items.Add(port);
            }
            if (ports != null && ports.Count() > 0)
            {
                this.comboBoxComports.Text = ports[0];
                this.comboBoxFXComs.Text = ports[0];
            }
            rfidWorkingThread = new Thread(new ThreadStart(SysWorkingProc));
            rfidWorkingThread.IsBackground = true;

            this.printer = new PrinterRW(1,"",8000);
            string dbConn = string.Format("Data Source ={0}\\SQLEXPRESS;Initial Catalog=FangTAIZaojuA;User ID=sa;Password=123456;", this.textBoxPrinterIP.Text);

            this.printerDB = new PrinterRWdb(dbConn);

            //MES 测试初始化
            this.textBoxMESwsAddr.Text = "http://192.168.100.90:8188/soap/EventService?wsdl";
            this.comboBoxInterfaces.Items.AddRange(new string[] {  "assembleDown", "assembleRepair" });
            this.comboBoxInterfaces.SelectedIndex = 0;
            this.comboBoxDTs.Items.AddRange(new string[] { "FT_MES_STEP_INFO","FT_MES_STEP_INFO_DETAIL" });
            this.comboBoxDTs.SelectedIndex = 0;
            this.textBoxMesParams.Text = "1002002100086L451607280194,L45";
            this.richTextBoxMesDBConn.Text = @"Data Source=(DESCRIPTION =
    (ADDRESS = (PROTOCOL = TCP)(HOST = 192.168.100.94)(PORT = 1521))
    (CONNECT_DATA =
      (SERVER = DEDICATED)
      (SERVICE_NAME = PRQMESDB)
    )
  )
;User Id=prqminda1;Password=prqminda1;Connection Timeout=5;";
//            this.richTextBoxMesDBConn.Text =  @"Data Source=(DESCRIPTION =
//    (ADDRESS = (PROTOCOL = TCP)(HOST = 192.168.100.94)(PORT = 1521))
//    (CONNECT_DATA =
//      (SERVER = DEDICATED)
//      (SERVICE_NAME = PRQMESDB)
//    )
//  )
//;User Id=prqminda1;Password=prqminda1;"; //这个也可以放到Web.Config中。 

            //气密
            airDetecter = new AirDetectFL295CRW(1,"");
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            exitRunning = true;
            rfidRW.Disconnect();
        }
        #region 日志相关
        /// <summary>
        /// 增加一条日志
        /// </summary>
        /// <param name="log"></param>
        private void AddLog(string log)
        {
            if (this.richTextBoxLog.InvokeRequired)
            {
                DelegateAddLog delegateObj = new DelegateAddLog(delegateAddLog);
                this.Invoke(delegateObj, new object[] { log });
            }
            else
            {
                string timeStr = string.Format("[{0:yyyy-MM-dd HH:mm:ss.fff}]", System.DateTime.Now);
                this.richTextBoxLog.Text += (timeStr+" "+log + "\r\n");
                if(this.richTextBoxLog.Text.Length>10000)
                {
                    this.richTextBoxLog.Text = "";
                }
            }
        }
        private void delegateAddLog(string log)
        {
            this.richTextBoxLog.Text += (log + "\r\n");
        }
       
       
        private void buttonClearLog_Click(object sender, EventArgs e)
        {
            this.richTextBoxLog.Clear();
        }
        #endregion
       
        #region Q 系列PLC 通信测试
        private void buttonConnectPlc_Click(object sender, EventArgs e)
        {
            string PlcIP = this.textBoxPlcIP.Text;//ConfigurationManager.AppSettings["plcIP"];
            string PlcPort = this.textBoxPlcPort.Text;// ConfigurationManager.AppSettings["plcPort"];
            string plcAddr = PlcIP + ":" + PlcPort;
            string reStr = "";
          //  plcRwObj2.ConnStr = plcAddr;
           // (plcRwIF as PLCRWNet).ConnStr = plcAddr;
            plcRwObj2.ConnStr = plcAddr;
            switch(cbxPlcCata.Text)
            {
                case "FX5U":
                    {
                        plcRwObj2.PlcCata = EnumPlcCata.FX5U;
                        break;
                    }
                case "Q系列":
                    {
                        plcRwObj2.PlcCata = EnumPlcCata.Qn;
                        break;
                    }
                case "Fx3uNET模块":
                    {
                        plcRwObj2.PlcCata = EnumPlcCata.FX3UENET;
                        break;
                    }
                default:
                    break;
            }
            plcRwIF = plcRwObj2;
            if (plcRwIF.ConnectPLC( ref reStr))
            {
                this.buttonClosePlc.Enabled = true;
                this.buttonReadPlc.Enabled = true;
                this.buttonWritePlc.Enabled = true;
            }
            AddLog(reStr);
            
        
        }
        private void buttonClosePlc_Click(object sender, EventArgs e)
        {

            if (plcRwIF.CloseConnect())
            {
                this.buttonClosePlc.Enabled = false;
                this.buttonReadPlc.Enabled = false;
                this.buttonWritePlc.Enabled = false;
                this.buttonConnectPlc.Enabled = true;
            }


            AddLog("PLC 连接已关闭!");
        }
        /// <summary>
        /// PLC 断开连接的事件响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlcLostConnectHandler(object sender, EventArgs e)
        {
            AddLog("PLC 通信连接断开，正在重连...");
            string PlcIP = ConfigurationManager.AppSettings["plcIP"];
            string PlcPort = ConfigurationManager.AppSettings["plcPort"];
            string plcAddr = PlcIP + ":" + PlcPort;
            string reStr = "";
            if (plcRwIF.ConnectPLC(ref reStr))
            {
                AddLog("PLC重连成功!");
            }
            else
            {
                AddLog("PLC重连失败!");
            }

        }
        private void buttonReadPlc_Click(object sender, EventArgs e)
        {

            string addr = this.textBoxPlcAddr.Text;
            int val = 0;
            if (!plcRwObj2.ReadDB(addr, ref val))
            {
                AddLog("PLC 读取地址：" + addr + "失败,"+plcRwObj2.GetLastErrorinfo());

            }
            else
            {
                this.textBoxPlcVal.Text = val.ToString();
            }

        }

        private void buttonWritePlc_Click(object sender, EventArgs e)
        {
            string addr = this.textBoxPlcAddr.Text;
            int val = int.Parse(this.textBoxPlcVal.Text);
            if (!plcRwObj2.WriteDB(addr, val))
            {
                AddLog("PLC 写入地址：" + addr + "失败,"+plcRwObj2.GetLastErrorinfo());
            }
            else
            {
                AddLog("PLC 写入地址：" + addr + "成功");
            }
        }

        private void buttonMultiWritePlc_Click(object sender, EventArgs e)
        {
            string addrStart = this.textBoxPlcAddrStart.Text;
            int blockNum = int.Parse(this.textBoxPlcBlockNum.Text);
            string[] splitStr = new string[] { ",", ":", "-", ";" };
            string strVals = this.richTextBoxMultiDBVal.Text;
            string[] strArray = strVals.Split(splitStr, StringSplitOptions.RemoveEmptyEntries);
            if (strArray == null || strArray.Count() < 1)
            {
                MessageBox.Show("输入数据错误");
                return;
            }
            short[] vals = new short[strArray.Count()];
            for (int i = 0; i < vals.Count(); i++)
            {
                vals[i] = short.Parse(strArray[i]);
            }
            if (plcRwObj2.WriteMultiDB(addrStart, blockNum, vals))
            {
                AddLog("批量写入成功");
            }
            else
            {
                AddLog("批量写入失败," + plcRwObj2.GetLastErrorinfo());
            }
        }
        private void buttonPLCDBReset_Click(object sender, EventArgs e)
        {
            try
            {
                string addrStart = this.textBoxPlcAddrStart.Text;
                int blockNum = int.Parse(this.textBoxPlcBlockNum.Text);
                short[] vals = new short[blockNum];
                Array.Clear(vals, 0, vals.Count());
                if (plcRwIF.WriteMultiDB(addrStart, blockNum, vals))
                {
                    AddLog("复位成功");
                }
                else
                {
                    AddLog("复位失败");
                }
            }
            catch (System.Exception ex)
            {
                AddLog("复位异常");
            }

        }
        private void buttonMultiReadPlc_Click(object sender, EventArgs e)
        {
            string addrStart = this.textBoxPlcAddrStart.Text;
            int blockNum = int.Parse(this.textBoxPlcBlockNum.Text);
            short[] reVals = null;
            if (plcRwObj2.ReadMultiDB(addrStart, blockNum, ref reVals))
            {
                string strVal = "";
                for (int i = 0; i < blockNum; i++)
                {
                    strVal += reVals[i].ToString() + ",";
                }
                this.richTextBoxMultiDBVal.Text = strVal;
            }
            else
            {
                AddLog("批量读取PLC数据失败,"+plcRwObj2.GetLastErrorinfo());
            }

        }

        private void comboBoxPlcObjList_SelectedIndexChanged(object sender, EventArgs e)
        {
           
              plcRwIF = plcRwObj2;

            
        }
        #endregion   
        #region FX系列PLC测试
        private void buttonFXOpen_Click(object sender, EventArgs e)
        {
            string reStr = "";
            plcFx485.ComPortName = this.comboBoxFXComs.Text;
            if(!plcFx485.ConnectPLC(ref reStr))
            {
                AddLog("打开串口失败,返回错误码:" + reStr);
            }
            else
            {
                
                AddLog("串口已打开");
            }
        }
        private void buttonFXClose_Click(object sender, EventArgs e)
        {
           plcFx485.CloseConnect();
           AddLog("串口关闭");
        }
        private void buttonReadMZone_Click(object sender, EventArgs e)
        {
            try
            {
                ReadMZone();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
          
        }
        private void ReadMZone()
        {
            plcFx485.PlcStationNumber = byte.Parse(this.textBoxPLCStation.Text);
            string addr = this.textBoxMZoneAddr.Text;
            int val = 0;
            if (!plcFx485.ExeBitRead(addr, ref val))
            {
                AddLog("读地址" + addr + "失败");
                return;
            }
           
            this.textBoxPLCBitVal.Text = val.ToString();
        }
        private void buttonWriteMZone_Click(object sender, EventArgs e)
        {
            try
            {
                WriteMZone();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }
        private void WriteMZone()
        {
            plcFx485.PlcStationNumber = byte.Parse(this.textBoxPLCStation.Text);
            string addr = this.textBoxMZoneAddr.Text;
            short val = 0;
            if (!short.TryParse(this.textBoxPLCBitVal.Text, out val))
            {
                MessageBox.Show("数值输入有误");
                return;
            }
            if(!plcFx485.ExeBitWrite(addr, val))
            {
                AddLog("写地址" + addr + "失败，");
                return;
            }
            else
            {
                AddLog("写地址" + addr + "成功");
            }
        }
        private void buttonReadSBlock_Click(object sender, EventArgs e)
        {
            try
            {
                ReadFx485SBlock();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }
        private void ReadFx485SBlock()
        {
            this.textBoxPlcBlockVal.Text = "";
            plcFx485.PlcStationNumber = byte.Parse(this.textBoxPLCStation.Text);
            string addr = this.textBoxDZoneAddr.Text;
            int val = 0; 
            if (!plcFx485.ReadDB(addr, ref val))
            {
                AddLog("读地址" + addr + "失败");
                return;
            }
            this.textBoxPlcBlockVal.Text = val.ToString();
        }
        private void buttonReadMBlock_Click(object sender, EventArgs e)
        {
            try
            {
                ReadFx485MBlock();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
           
        }
        private void ReadFx485MBlock()
        {
            this.richTextBoxMBlock.Text = "";
            plcFx485.PlcStationNumber = byte.Parse(this.textBoxPLCStation.Text);
            string addr = this.textBoxDZoneAddr.Text;
            short[] vals = null;
            int blockNum = int.Parse(this.textBoxFxBlockNum.Text);
            if (!plcFx485.ReadMultiDB(addr, blockNum, ref vals))
            {
                AddLog("批量读寄存器失败");
                return;
            }
            for (int i = 0; i < blockNum; i++)
            {
                this.richTextBoxMBlock.Text += (vals[i].ToString() + ",");
            }
        }

        private void WriteFx485SBlock()
        {
            
            plcFx485.PlcStationNumber = byte.Parse(this.textBoxPLCStation.Text);
            string addr = this.textBoxDZoneAddr.Text;
            int val = int.Parse(this.textBoxPlcBlockVal.Text);
            if (plcFx485.WriteDB(addr, val))
            {
                AddLog("写地址" + addr + "成功");
            }
            else
            {
                AddLog("写地址" + addr + "失败");
            }
        }
        private void buttonWriteSBlock_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(this.textBoxPlcBlockVal.Text))
                {
                    MessageBox.Show("数据为空");
                    return;
                }
                WriteFx485SBlock();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }
        private void WriteFx485MBlock()
        {
            string addrStart = this.textBoxDZoneAddr.Text;
            int blockNum = int.Parse(textBoxFxBlockNum.Text);
            string[] splitStr = new string[] { ",", ":", "-", ";" };
            string strVals = this.richTextBoxMultiDBVal.Text;
            string[] strArray = strVals.Split(splitStr, StringSplitOptions.RemoveEmptyEntries);
            if (strArray == null || strArray.Count() < 1)
            {
                MessageBox.Show("输入数据错误");
                return;
            }
            short[] vals = new short[strArray.Count()];
            for (int i = 0; i < vals.Count(); i++)
            {
                vals[i] = short.Parse(strArray[i]);
            }
            if (plcFx485.WriteMultiDB(addrStart, blockNum, vals))
            {
                AddLog("批量写入成功");
            }
            else
            {
                AddLog("批量写入失败");
            }
        }

        private void buttonFxWriteMBlock_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(this.richTextBoxMultiDBVal.Text))
                {
                    MessageBox.Show("数据为空");
                    return;
                }
                WriteFx485MBlock();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
           
        }
        #endregion
        #region 读卡测试
        private void buttonOpenComport_Click(object sender, EventArgs e)
        {
            // HFReaderIF readerIF = new HFReaderIF();
            //     rfidRW.ReaderIF = readerIF;
            rfidRW.ReaderIF.ComPort = this.comboBoxComports.Text;
            rfidRW.ReaderID = byte.Parse(this.textBoxReaderID.Text);
            string reStr = "";
            if (!rfidRW.ReaderIF.OpenComport(ref reStr))
            {
                AddLog(reStr);
                return;
            }
            AddLog("通信端口已经打开");

            byte[] recvBytes = null;
            if (rfidRW.Connect())
            {
                AddLog("读卡器已经连接上");
            }
            else
            {
                AddLog("读卡器连接失败");
            }
        }
        private void buttonClosePort_Click(object sender, EventArgs e)
        {
            if (rfidRW.Disconnect())
            {
                AddLog("读卡器已经关闭");
            }

        }
        private void buttonRfidReset_Click(object sender, EventArgs e)
        {
            //if (Status_enum.SUCCESS == rfidRW.reader.SoftReset(rfidRW.ReaderID))
            //{
            //    AddLog("读卡器软复位成功");
            //}
            //else
            //{
            //    AddLog("读卡器软复位失败");
            //}
        }
       
        private void buttonReadCfg_Click(object sender, EventArgs e)
        {
            rfidRW.ReaderID = byte.Parse(this.textBoxReaderID.Text);
            byte[] recvBytes = null;
            if (!rfidRW.Connect())
            {
                MessageBox.Show("连接读卡器，读取配置信息失败");
            }
            else
            {
                MessageBox.Show("连接读卡器，读取配置信息成功");
            }
        }
        private void SysWorkingProc()
        {
            //PLCRW plcRwObj2 = new PLCRW();
            //string reStr = "";
            //plcRwObj2.ConnectPLC("192.168.1.121:7000", ref reStr);
            while (!exitRunning)
            {
                if (pauseFlag)
                {
                    continue;
                }
                try
                {
                    Thread.Sleep(rfidRWInterval);
                   // byte[] recvBytes = null;
                   // byte[] rfidData = this.rfidRW.ReadSBlock(0,ref recvBytes);
                    byte[] rfidData = this.rfidRW.ReadRfidMultiBlock(0,2);
                    rwCounts++;
                    if (rfidData == null || rfidData.Count() < 8)
                    {
                        rwFaileCounts++;
                    }
                    Thread.Sleep(rfidRWInterval);
                    byte[] rfidDataWrite = new byte[8] { 1, 2, 3, 4,5,6,7,8 };

                    rwCounts++;
                   // if (!this.rfidRW.WriteSBlock(rfidDataWrite))
                    if(!this.rfidRW.WriteMBlock(0,rfidDataWrite))
                    {
                        rwFaileCounts++;
                    }

                    RefreshRWCounts();
                    //int val = 0;
                    //plcRwObj2.ReadDB("D3000", ref val);

                }
                catch (System.Exception ex)
                {
                    AddLog(ex.Message + "," + ex.StackTrace);
                }
            }
        }
        private void buttonStartRW_Click(object sender, EventArgs e)
        {
            if (rfidWorkingThread.ThreadState == (ThreadState.Unstarted| ThreadState.Background))
            {
                rfidWorkingThread.Start();
            }
            else
            {
                // Monitor.Exit(sysWorkingThreadLock);
                // sysWorkingThread.Resume();
                pauseFlag = false;
            }
            AddLog("自动读写卡启动");
        }

        private void buttonStopRW_Click(object sender, EventArgs e)
        {
            pauseFlag = true;
            AddLog("自动读写卡停止");
        }

        private void buttonClearRWCounts_Click(object sender, EventArgs e)
        {
            rwCounts = 0;
            rwFaileCounts = 0;
            this.textBoxrfidRWCounts.Text = "0";
            this.textBoxrfidRWFaileds.Text = "0";
        }
        private void delegateRefreshRWCounts()
        {
            this.textBoxrfidRWCounts.Text = rwCounts.ToString();
            this.textBoxrfidRWFaileds.Text = rwFaileCounts.ToString();
            if (rwCounts > 0)
            {
                float failRate = (float)rwFaileCounts / (float)rwCounts;
                this.textBoxrfidRWFailRate.Text = failRate.ToString();
            }
        }
        private void RefreshRWCounts()
        {
            DelegateRefeshRfid delegateRW = new DelegateRefeshRfid(delegateRefreshRWCounts);
            this.BeginInvoke(delegateRW, null);
        }

        //发卡
        private void buttonWriteID_Click(object sender, EventArgs e)
        {
            try
            {
               // int idCheck = int.Parse(this.textBoxWriteIDRepeat.Text);
                Int64 idCheck = int.Parse(this.textBoxWriteIDRepeat.Text);
                if (idCheck <0)
                {
                    MessageBox.Show("id错误，请重新输入");
                    return;
                }
               
                if (this.textBoxWriteIDRepeat.Text != this.textBoxWriteID.Text)
                {
                    MessageBox.Show("两次输入不一致，请确认");
                    return;
                }
                rfidRW.ReaderID = byte.Parse(this.textBoxReaderID.Text);
               // uint rfidID = uint.Parse(this.textBoxWriteID.Text);
                Int64 rfidID = uint.Parse(this.textBoxWriteID.Text);
                if (rfidID < 1)
                {
                    MessageBox.Show("请输入大于0的整数");
                    return;
                }
                if (this.comboBoxDatabitSel.Text == "32位整数")
                {
                    if (!rfidRW.WriteData((int)rfidID))
                    {
                        this.labelIDRWResult.Text = "发卡失败!";
                        this.labelIDRWResult.BackColor = Color.Red;
                        MessageBox.Show("发卡失败");

                        return;
                    }
                    byte[] recvByteArray = null;
                    int readPalletID = rfidRW.ReadData();
                    if (readPalletID < 0 || (readPalletID != idCheck))
                    {
                        string faildInfo = "发卡失败!发卡后回读结果不一致";
                        this.labelIDRWResult.Text = faildInfo;
                        this.labelIDRWResult.BackColor = Color.Red;
                        MessageBox.Show(faildInfo);
                        return;
                    }

                }
                else if (this.comboBoxDatabitSel.Text == "64位整数")
                {
                    if (!rfidRW.WriteDataInt64((Int64)rfidID))
                    {
                        this.labelIDRWResult.Text = "发卡失败!";
                        this.labelIDRWResult.BackColor = Color.Red;
                        MessageBox.Show("发卡失败");

                        return;
                    }
                    byte[] recvByteArray = null;
                    Int64 readPalletID = rfidRW.ReadDataInt64();
                    if (readPalletID < 0 || (readPalletID != idCheck))
                    {
                        string faildInfo = "发卡失败!发卡后回读结果不一致";
                        this.labelIDRWResult.Text = faildInfo;
                        this.labelIDRWResult.BackColor = Color.Red;
                        MessageBox.Show(faildInfo);
                        return;
                    }
                }
                else
                {
                    return;
                }
                //byte[] byteArray = BitConverter.GetBytes(rfidID);
                //if (byteArray != null && byteArray.Count() > 0)
                //{
                //    this.textBoxWriteID.SelectAll();
                //    if (!rfidRW.WriteSBlock(byteArray))
                //    {
                //        this.labelIDRWResult.Text = "发卡失败!";
                //        this.labelIDRWResult.BackColor = Color.Red;
                //        MessageBox.Show("发卡失败");

                //        return;
                //    }
                //    byte[] recvByteArray = null;
                //    int readPalletID = rfidRW.ReadData(ref recvByteArray);
                //    if (readPalletID<0 || (readPalletID != idCheck))
                //    {
                //        string faildInfo = "发卡失败!发卡后回读结果不一致";
                //        this.labelIDRWResult.Text = faildInfo;
                //        this.labelIDRWResult.BackColor = Color.Red;
                //        MessageBox.Show(faildInfo);
                //        return;
                //    }
                   
 
                //    this.labelIDRWResult.Text = "发卡成功!";
                //    this.labelIDRWResult.BackColor = Color.Green;

                //}
                this.labelIDRWResult.Text = "发卡成功!";
                this.labelIDRWResult.BackColor = Color.Green;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("发卡失败，错误信息：" + ex.Message);
            }

        }

        //回读
        private void buttonReadID_Click(object sender, EventArgs e)
        {
           
            try
            {
                this.textBoxReadID.Text = string.Empty;
                rfidRW.ReaderID = byte.Parse(this.textBoxReaderID.Text);
               
                string palletID = rfidRW.ReadData().ToString();
                //byte[] bytesArray = rfidRW.ReadSBlock(0);
                if (string.IsNullOrWhiteSpace(palletID) || palletID == "-1")
                {
                    MessageBox.Show("回读失败");
                    this.labelIDRWResult.Text = "读卡失败";
                    this.labelIDRWResult.BackColor = Color.Red;
                    string strByteArray = "";


                  

                    AddLog("读卡失败，");
                    return;
                }
                else
                {
                    this.labelIDRWResult.Text = "读卡成功";
                    this.labelIDRWResult.BackColor = Color.Green;
                }

                this.textBoxReadID.Text = palletID;
            }
            catch (System.Exception ex)
            {
                AddLog(ex.ToString());
            }
           
        }

        private void buttonReadUID_Click(object sender, EventArgs e)
        {
            try
            {
                string uid = rfidRW.ReadUID();
                this.textBoxUID.Text = uid;
            }
            catch (Exception ex)
            {

                AddLog(ex.ToString());
            }
        }
        private void textBoxWriteID_Click(object sender, EventArgs e)
        {
            this.textBoxWriteID.SelectAll();
        }
        private void textBoxWriteIDRepeat_Click(object sender, EventArgs e)
        {
            this.textBoxWriteIDRepeat.SelectAll();
        }


        private void textBoxWriteID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)//如果输入的是回车键
            {
                this.buttonWriteID_Click(sender, e);//触发button事件
            }
        }

        private void textBoxrfidRWFaileds_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonGetCode_Click(object sender, EventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

       
        //private void buttonGetMakeRecord_Click(object sender, EventArgs e)
        //{
        //    string idCheck = this.textBoxWriteID.Text;

        //    if (idCheck == null || idCheck == string.Empty)
        //    {
        //        MessageBox.Show("id为空，请重新输入");
        //        return;
        //    }
        //    idCheck = "TP" + idCheck.PadLeft(7, '0');
        //    List<MakeCardRecordModel> makeCardList = makeCardBll.GetModelList("cardID = '" + idCheck + "' ");
        //    if (makeCardList == null || makeCardList.Count == 0)
        //    {
        //        MessageBox.Show("发卡记录为空");
        //        return;
        //    }
        //    MakeCardRecordModel record= makeCardList[0];
        //    if (record == null)
        //    {
        //        MessageBox.Show("发卡记录为空");
        //        return;
        //    }
        //    string recordInfo = string.Format("发卡记录：\n托盘号:{0}\n发卡时间：{1}\n", record.cardID, record.makedTime);
        //    this.richTextBoxMakeRecord.Text = recordInfo;
        //}

        #endregion
        #region 条码枪相关
        private void buttonBarcodeManual_Click(object sender, EventArgs e)
        {
            if(this.barcodeReader.TriggerMode == EnumTriggerMode.程序命令触发)
            {
                string barcode = this.barcodeReader.ReadBarcode();
                AddLog(barcode);
            }
            else
            {
                foreach (string barcode in this.barcodeReader.GetBarcodesBuf())
                {
                    AddLog(barcode);
                }
                this.barcodeReader.ClearBarcodesBuf();
            }
            
        }
        private void buttonBarcodePortOpen_Click(object sender, EventArgs e)
        {
           
            if(!string.IsNullOrWhiteSpace(this.comboBoxBarcode.Text))
            {
                if(barcodeReader.ComPortObj != null && barcodeReader.ComPortObj.IsOpen)
                {
                    barcodeReader.ComPortObj.Close();
                    
                }
                SerialPort comPort = new SerialPort(this.comboBoxBarcode.Text);
                comPort.BaudRate = 115200;
                comPort.DataBits = 8;
                comPort.StopBits = StopBits.One;
                comPort.Parity = Parity.None;
                comPort.Open();
                barcodeReader.ComPortObj = comPort;
                if (this.radioButtonBarcode1.Checked)
                {
                    barcodeReader.TriggerMode = EnumTriggerMode.手动按钮触发;
                }
                else
                {
                    barcodeReader.TriggerMode = EnumTriggerMode.程序命令触发;
                }
                
                string reStr = "";
                barcodeReader.StartMonitor(ref reStr);
                AddLog("端口:" + comPort.PortName + "已打开");
            }
            else
            {
                AddLog("串口不存在");
            }
            
        }

        private void buttonBarcodePortClose_Click(object sender, EventArgs e)
        {
            if (barcodeReader.ComPortObj != null && barcodeReader.ComPortObj.IsOpen)
            {
                barcodeReader.ComPortObj.Close();
                return;
            }
        }
        #endregion
        #region 数据库测试
        private int logCounter = 0;
        private void timerLogTest_Tick(object sender, EventArgs e)
        {
            for(int i=0;i<100;i++)
            {
                FTDataAccess.Model.SysLog logModel = new FTDataAccess.Model.SysLog();
                logModel.LogLevel = "调试";
                logModel.LogSourceObject = "测试对象";
                logModel.LogContent = "这是测试日志";
                logModel.LogTime = System.DateTime.Now;
                DateTime beforeAddTime = System.DateTime.Now;
                logBll.Add(logModel);
                DateTime afterAddTime = System.DateTime.Now;
                TimeSpan timeSpan = afterAddTime - beforeAddTime;
                AddLog("插入数据库，耗时：" + timeSpan.Milliseconds.ToString() + "毫秒");
                logCounter++;
                this.labelLogCounter.Text = "增加记录：" + logCounter.ToString();
            }
           
        }

        private void buttonStartAddlog_Click(object sender, EventArgs e)
        {
            this.timerLogTest.Start();
        }

        private void buttonStopLogadd_Click(object sender, EventArgs e)
        {
            this.timerLogTest.Stop();
        }
        #endregion
        #region 贴标机测试
        private void buttonConnPrinter_Click(object sender, EventArgs e)
        {
            string ip = this.textBoxPrinterIP.Text;
            short port = short.Parse(this.textBoxPrinterPort.Text);
            string reStr="";
            printer.PrinterAddr = ip;
            printer.PrinterPort = port;
            printer.Connect(ref reStr);
            AddLog(reStr);
            
        }
        private void buttonDisconPrinter_Click(object sender, EventArgs e)
        {
            string reStr="";
            printer.Disconnect(ref reStr);
            AddLog(reStr);
        }

        private void buttonSndHello_Click(object sender, EventArgs e)
        {
            string reStr = "";
            bool re =printer.SndHelloInfo(ref reStr);
            AddLog(reStr);
        }

        private void buttonSndBarcode_Click(object sender, EventArgs e)
        {
            string code = this.textBoxSndBarcode.Text;
            string reStr = "";
            if(this.checkBoxPrinterDB.Checked)
            {
                printerDB.ConnStr = string.Format("Data Source ={0}\\SQLEXPRESS;Initial Catalog=FangTAIZaojuA;User ID=sa;Password=123456;", this.textBoxPrinterDB.Text);
              //  printerDB.ConnStr = string.Format("Data Source ={0};Initial Catalog=yibin;User ID=sa;Password=159753;", this.textBoxPrinterDB.Text);
                if(!printerDB.SndBarcode(code, ref reStr))
                {
                    AddLog(reStr);
                }
                else
                {
                    AddLog("发送条码到数据库成功:" + code+","+reStr);
                }
            }
            else
            {
                
               
                bool re = printer.SndBarcode(code, ref reStr);
                AddLog(reStr);
            }
           
        }
        private void buttonPrintStat_Click(object sender, EventArgs e)
        {
            string reStr = "";
            string code = this.textBoxSndBarcode.Text;
            bool re = printer.PrintFinished(code, ref reStr);
            AddLog(reStr);
        }

        private void buttonPrienterStat_Click(object sender, EventArgs e)
        {
            string reStr = "";
            PrinterReStat stat = new PrinterReStat();
            bool re = printer.PrinterStat(ref stat, ref reStr);
            
            AddLog(reStr);
            
        }

        #endregion
        #region 气密检测

        private void buttonAirdetectOpen_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.comboBoxAirdetect.Text))
            {
                if (airDetecter.ComPortObj != null && airDetecter.ComPortObj.IsOpen)
                {
                    return;
                }
                airDetecter.Comport = this.comboBoxAirdetect.Text;
                SerialPort comPort = new SerialPort(this.comboBoxAirdetect.Text);
                comPort.BaudRate = 9600;
                comPort.DataBits = 8;
                comPort.StopBits = StopBits.One;
                comPort.Parity = Parity.None;
                comPort.Open();
                airDetecter.ComPortObj = comPort;

                string reStr = "";
                airDetecter.StartMonitor(ref reStr);
                AddLog(reStr);
                //airDetecter.;
                //AddLog("端口:" + comPort.PortName + "已打开");
            }
            else
            {
                AddLog("串口不存在");
            }
        }

        private void btnAirdetectStart_Click(object sender, EventArgs e)
        {
            string reStr = "";
            if(!airDetecter.StartDetect(ref reStr))
            {
                AddLog(reStr);
            }

        }
        private void buttonAirdetectBegin_Click(object sender, EventArgs e)
        {
            string reStr="";
            AirlossDetectModel m= airDetecter.QueryResultData(ref reStr);
            if(m != null)
            {
                this.labelAirdetect.Text = string.Format("检测结果:{0},{1},{2}", m.DetectVal, m.UnitDesc, m.DetectResult);
            }
        }

        #endregion
        #region MES相关
        private void buttonCallMes_Click(object sender, EventArgs e)
        {
            //MesWs.EventService ws = new MesWs.EventService();
            //string[] reArray = ws.signalEvent2("assembleDown", new string[] { "700010020L401204032345" });
            //if (reArray != null && reArray.Count() > 0)
            //{
            //    AddLog(string.Format("assembleDown return：{0}", reArray));
            //}
            string reStr="";
            int re = 0;
            string[] paramArray = this.textBoxMesParams.Text.Split(new string[] { ",", ":" }, StringSplitOptions.RemoveEmptyEntries);
            switch(this.comboBoxInterfaces.Text)
            {
                //case "assembleAuto":
                //    {
                //        re = mesDA.MesAssemAuto(paramArray, ref reStr);
                //        break;
                //    }
                case "assembleDown":
                {
                     re =mesDA.MesAssemDown(paramArray, ref reStr);
                     
                    
                     break;
                }
                case "assembleRepair":
                    {
                        re= mesDA.MesReAssemEnabled(paramArray, ref reStr);
                      
                        break;
                    }
                default:
                    break;
               
            }
            //if(re)
            //{
            //    reStr = "OK，" + reStr;
            //}
            //else
            //{
            //    reStr = "错误," + reStr;
            //}
            AddLog(re.ToString()+","+reStr);
            
        }
       
        private void buttonConnMesDB_Click(object sender, EventArgs e)
        {

            MesDBConn(this.richTextBoxMesDBConn.Text);
        }
        private void buttonReadMESdata_Click(object sender, EventArgs e)
        {
            DataTable dt = ReadMesData(this.comboBoxDTs.Text);
            if(dt != null)
            {
                this.dataGridViewMESDt.DataSource = dt;
                this.dataGridViewMESDt.Columns["TRX_TIME"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
            }
        }
        private void MesDBConn(string connString)
        {
            try
            {
                AddLog(connString);
                mesDA.MesdbConnstr = connString;
              //  conn.ConnectionString = connString;
               // conn.Open();
                string reStr="";
                mesDA.ConnDB(ref reStr);
                AddLog(reStr);
            }
            catch (Exception ex)
            {

                AddLog(ex.ToString());
            }
        }

        private void buttonDisconn_Click(object sender, EventArgs e)
        {
            try
            {
                string reStr = "";
                mesDA.DisconnDB(ref reStr);
                AddLog(reStr);
            }
            catch (Exception ex)
            {
                AddLog(ex.ToString());
            }
        }
        private DataTable ReadMesData(string tableName)
        {
           // string sql = "select * from all_tables where owner = 'PRQMINDA1'";
           // string sql= string.Format("select * from {0} where rownum <=10",tableName);
            //OracleCommand com = new OracleCommand(sql,conn);
            //OracleDataReader dr = com.ExecuteReader();
            //DataTable dt = new DataTable();
            //int fieldcout = dr.FieldCount;
            //if (dr.FieldCount > 0)
            //{
            //    for (int i = 0; i < dr.FieldCount; i++)
            //    {
            //        DataColumn dc = new DataColumn(dr.GetName(i), dr.GetFieldType(i));
            //        dt.Columns.Add(dc);
            //    }
            //    object[] rowobject = new object[dr.FieldCount];
            //    while (dr.Read())
            //    {
            //        dr.GetValues(rowobject);
            //        dt.LoadDataRow(rowobject, true);
            //    }
            //}
            //dr.Close();
            string reStr = ""; 
            DataTable dt = this.mesDA.ReadMesDataTable(tableName, ref reStr);
            AddLog(reStr);
            return dt;
        }


        private void buttonExeSql_Click(object sender, EventArgs e)
        {
            OnSqlExec();
        }
        private void OnSqlExec()
        {
            try
            {
                string strSql = this.richTextBoxSql.Text;
                if (string.IsNullOrEmpty(strSql))
                {

                    return;
                }

               this.dataGridViewMESDt.DataSource = mesDA.ReadMesTable(strSql);
               if (this.dataGridViewMESDt.Columns.Contains("TRX_TIME"))
               {
                   this.dataGridViewMESDt.Columns["TRX_TIME"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
                   this.dataGridViewMESDt.Columns["CREATION_TIME"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
               }
               
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void buttonClearSqlre_Click(object sender, EventArgs e)
        {
            this.dataGridViewMESDt.DataSource = null;
        }
        #endregion
        private void groupBox6_Enter(object sender, EventArgs e)
        {

        }

        public void DataGridViewEnableCopy(DataGridView p_Data)
        {
            Clipboard.SetData(DataFormats.Text, p_Data.GetClipboardContent());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FT_MES_STEP_INFOModel m = new FT_MES_STEP_INFOModel();
            m.RECID = System.Guid.NewGuid().ToString();
            m.SERIAL_NUMBER = "71125800010L411606101238";
            m.STEP_NUMBER = "RQ-ZA041";
            m.TRX_TIME = System.DateTime.Now;
            m.STATUS = 0;
            m.CHECK_RESULT = 1;
            this.mesDA.AddMesBaseinfo(m);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FT_MES_STEP_INFO_DETAILModel m = new FT_MES_STEP_INFO_DETAILModel();
            m.RECID = System.Guid.NewGuid().ToString();
            m.SERIAL_NUMBER = "71125800010L411606101238";
            m.STEP_NUMBER = "RQ-ZA041";
            m.TRX_TIME = System.DateTime.Now;
            m.STATUS = 0;
            m.STEP_NUMBER = "RQ-ZA041";
            m.DATA_NAME = "L45-气密检查2";
            m.DATA_VALUE = "12.43";
            this.mesDA.AddMesDetailinfo(m);
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void buttonGetHardwareinfo_Click(object sender, EventArgs e)
        {
            switch(this.comboBoxHardtype.Text)
            {
                case "主板":
                    {
                        this.textBoxHardware.Text=RobotSystemLicenceManager.ComputerInfor.GetComMainBoard();
                        break;
                    }
                case "网卡MAC":
                    {
                        this.textBoxHardware.Text=RobotSystemLicenceManager.ComputerInfor.GetMacAddress();
                        break;
                    }
                case "硬盘":
                    {
                        this.textBoxHardware.Text = RobotSystemLicenceManager.ComputerInfor.GetHardID();
                        break;
                    }
                case "CPU":
                    {
                        this.textBoxHardware.Text = RobotSystemLicenceManager.ComputerInfor.GetCpuID();
                        break;
                    }
                default:
                    break;
            }
        }

        private void buttonGetAllHardinfo_Click(object sender, EventArgs e)
        {
            this.richTextBoxHard.Text = "";
            //this.richTextBoxHard.Text = 
            StringBuilder strBuild = new StringBuilder();
            strBuild.AppendFormat("硬盘：{0}\n", RobotSystemLicenceManager.ComputerInfor.GetHardID());
            strBuild.AppendFormat("主板：{0}\n", RobotSystemLicenceManager.ComputerInfor.GetComMainBoard());
            strBuild.AppendFormat("CPU：{0}\n", RobotSystemLicenceManager.ComputerInfor.GetCpuID());
            strBuild.AppendFormat("MAC：{0}\n", RobotSystemLicenceManager.ComputerInfor.GetMacAddress());
            this.richTextBoxHard.Text = strBuild.ToString();
           
        }





    }
}
