using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevAccess;
namespace DeviceAssist
{
    public partial class RfidSgUrb3Form : Form
    {
        SgUrb3RW rfidRW = null;
        public RfidSgUrb3Form()
        {
            InitializeComponent();
           
        }

        private void buttonConn_Click(object sender, EventArgs e)
        {
           
            uint port = uint.Parse(this.textBoxPort.Text);
            this.rfidRW = new SgUrb3RW(1, this.textBoxIP.Text,port);
            //rfidRW.HostIP = "192.168.1.100";
           // rfidRW.HostPort = 32834;
         //   byte[] reBytes = null;
            if(this.rfidRW.Connect())
            {
               
                Console.WriteLine("连接成功");
            }
            else
            {
                Console.WriteLine("连接失败");
            }
        }

        private void buttonDisconn_Click(object sender, EventArgs e)
        {
            if(this.rfidRW != null)
            {
                if(!this.rfidRW.Disconnect())
                {
                    Console.WriteLine("断开连接失败");
                }
                else
                {
                    Console.WriteLine("断开连接OK");
                }
            }
        }

        private void buttonReadUID_Click(object sender, EventArgs e)
        {
            string uid = rfidRW.ReadUID();
            if(string.IsNullOrWhiteSpace(uid))
            {

                Console.WriteLine("读UID失败");
            }
            else
            {
                this.textBoxUID.Text = uid;
                this.listBox1.Items.Add(uid);
                Console.WriteLine("读UID成功");
            }
            
        }

        private void buttonReadID_Click(object sender, EventArgs e)
        {
            if(radioButton1.Checked)
            {
                int data = rfidRW.ReadData();
                if (data >= 0)
                {
                    this.textBoxReadData.Text = data.ToString();
                    Console.WriteLine("读数据成功");
                }
                else
                {
                    Console.WriteLine("读数据失败");
                }
            }
            else
            {
                byte[] bytesData = rfidRW.ReadBytesData();
                if(bytesData == null || bytesData.Count()<1)
                {
                    Console.WriteLine("读数据失败");
                }
                else
                {
                    string str = System.Text.Encoding.UTF8.GetString(bytesData);
                    this.textBoxReadData.Text = str;
                    Console.WriteLine("读数据成功");
                }
               
            }
            
        }

        private void buttonWriteID_Click(object sender, EventArgs e)
        {
            try
            {
                if (radioButton1.Checked)
                {
                    int val = int.Parse(this.textBoxWriteData.Text);
                    if (!rfidRW.WriteData(val))
                    {
                        Console.WriteLine("写入数据失败");
                    }
                    else
                    {
                        Console.WriteLine("写入成功");
                    }
                }
                else
                {
                    byte[] bytesData = System.Text.UTF8Encoding.Default.GetBytes(this.textBoxWriteData.Text);
                    byte[] bytesDataToWrite = new byte[bytesData.Count() + 1];
                    Array.Copy(bytesData, 0, bytesDataToWrite, 0, bytesData.Count());
                    bytesDataToWrite[bytesData.Count()] = 0;
                    if (rfidRW.WriteBytesData(bytesDataToWrite))
                    {
                        Console.WriteLine("写入成功");
                    }
                    else
                    {
                         Console.WriteLine("写入数据失败");
                    }
                    
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }

        private void btnClearEpcs_Click(object sender, EventArgs e)
        {
            this.listBox1.Items.Clear();
        }
    }
}
