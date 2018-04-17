using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.Services;
using System.Web.Services.Description;
using System.CodeDom;
using System.Net;
using System.IO;
using DevAccess;
namespace ME_CCDCommTest
{
    public partial class Form1 : Form
    {
        private MingmeiDeviceAcc devAcc = new MingmeiDeviceAcc();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Console.SetOut(new CommonPL.TextBoxWriter(this.richTextBoxLog));
            this.comboBoxCmd.Items.AddRange(new string[] {"START","END","GET"});
            this.comboBoxCmd.SelectedIndex = 0;
            this.textBoxMesAddr.Text = "http://172.20.1.7:9012/MesFrameWork.asmx";//内网地址
            //this.textBoxMesAddr.Text = "http://58.222.123.130:9012/MesFrameWork.asmx";//外网地址
        }
        private void richTextBoxLog_TextChanged(object sender, EventArgs e)
        {
            this.richTextBoxLog.SelectionStart = this.richTextBoxLog.Text.Length; //Set the current caret position at the end
            this.richTextBoxLog.ScrollToCaret();
        }

        private void btnParseRecvXML_Click(object sender, EventArgs e)
        {
            IDictionary<string, string> dic = null;
            string reStr = "";
            dic=devAcc.ParseRecvData(this.richTextBoxRecv.Text,ref reStr);
            if(dic == null)
            {
                Console.WriteLine("解析失败," + reStr);
            }
            else
            {
                Console.WriteLine("解析结果如下：");
                foreach(string keyStr in dic.Keys)
                {
                    string str = string.Format("产品ID:{0}，数据：{1}", keyStr, dic[keyStr]);
                    Console.WriteLine(str);
                }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.richTextBoxLog.Text = "";
        }

        private void btnConn_Click(object sender, EventArgs e)
        {
            this.devAcc.ConnStr = string.Format("{0}:{1}", this.textBoxIP.Text, this.textBoxPort.Text);
            this.devAcc.LocalPort = int.Parse(this.textBoxPcPort.Text);
            string reStr="";
            if(this.devAcc.Connect(ref reStr))
            {
                Console.WriteLine("连接服务器成功");
            }
            else
            {
                Console.WriteLine("连接服务器失败");
            }
        }

        private void btnDisconn_Click(object sender, EventArgs e)
        {
            
        }

        private void btnSnd_Click(object sender, EventArgs e)
        {
            string cmdStr = comboBoxCmd.Text;
            List<string> productIDS = new List<string>();
            if(cmdStr == "START")
            {
                 if(string.IsNullOrWhiteSpace(this.richTextBoxProductIDS.Text))
                 {
                     Console.WriteLine("缺少产品ID信息");
                     return;
                 }
                 string[] ids = this.richTextBoxProductIDS.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                 productIDS.AddRange(ids);
            }
            string devID = this.textBoxDevID.Text;
            this.richTextBoxSnd.Text = this.devAcc.GenerateSndXML(devID, cmdStr, productIDS);
            string reStr="";
            if(cmdStr == "START")
            {
                if(!this.devAcc.StartDev(productIDS,devID,ref reStr))
                {
                    Console.WriteLine(reStr);
                }
                else
                {
                    this.richTextBoxRecv.Text = this.devAcc.RecvStr;
                    Console.WriteLine("命令发送成功！");
                }
            }
            else if(cmdStr=="END")
            {
                if (!this.devAcc.EndDev(devID, ref reStr))
                {
                    Console.WriteLine(reStr);
                }
                else
                {
                    this.richTextBoxRecv.Text = this.devAcc.RecvStr;
                    Console.WriteLine("命令发送成功！");
                }
            }
            else
            {
                IDictionary<string, string> dic = this.devAcc.GetData(devID, ref reStr);
                if(dic == null || dic.Keys.Count()<1)
                {
                    Console.WriteLine("获取数据失败," + reStr);
                }
                else
                {
                    this.richTextBoxRecv.Text = this.devAcc.RecvStr;
                    Console.WriteLine("获取数据成功，解析结果如下：");
                    foreach (string keyStr in dic.Keys)
                    {
                        string str = string.Format("产品ID:{0}，数据：{1}", keyStr, dic[keyStr]);
                        Console.WriteLine(str);
                    }
                }
            }
        }

        private void buttonReadWelder_Click(object sender, EventArgs e)
        {
            try
            {
                string welderIP = "192.168.0.45";

                int channelIndex = 1;
              //  string filePath = string.Format(@"\\{0}\MESReport\DeviceInfoLane{1}.txt", welderIP, channelIndex);
               string filePath = this.textBoxFilepath.Text;// string.Format("{0}", this.textBoxFilepath.Text);//@"\\192.168.0.45\MESReport\DeviceInfoLane1.txt"; 
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("文件{0}不存在!");
                    return;
                }
                FileStream fs = System.IO.File.Open(filePath, FileMode.Open);
                StreamReader reader = new StreamReader(fs);
                string str = reader.ReadToEnd();
                this.richTextBox1.Text = str;
                fs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
            
        }

        private void buttonWriteWelder_Click(object sender, EventArgs e)
        {
            try
            {
                string filePath = this.textBoxFilepath.Text;// string.Format("{0}", this.textBoxFilepath.Text);
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("文件{0}不存在!");
                    return;
                }
               // FileStream fs = System.IO.File.Open(filePath,FileMode.OpenOrCreate,FileAccess.ReadWrite);

                StreamWriter writter = new StreamWriter(filePath, false);
            
                writter.Write(this.richTextBox1.Text);
                writter.Flush();
                writter.Close();
               // fs.Flush();
               /// fs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void btnMesCall1_Click(object sender, EventArgs e)
        {
            OnMESCall1();
        }
        private void OnMESCall1()
        {
            try
            {
                //WEB服务地址
                string url = this.textBoxMesAddr.Text;
                //客户端代理服务命名空间，可以设置成需要的值。
                string ns = string.Format("JCJProxMES");
                //获取WSDL
                WebClient wc = new WebClient();
                Stream stream = wc.OpenRead(url + "?WSDL");
                ServiceDescription sd = ServiceDescription.Read(stream);//服务的描述信息都可以通过ServiceDescription获取
                string classname = sd.Services[0].Name;

                ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
                sdi.AddServiceDescription(sd, "", "");
                CodeNamespace cn = new CodeNamespace(ns);

                //生成客户端代理类代码
                CodeCompileUnit ccu = new CodeCompileUnit();
                ccu.Namespaces.Add(cn);
                sdi.Import(cn, ccu);
                Microsoft.CSharp.CSharpCodeProvider csc = new Microsoft.CSharp.CSharpCodeProvider();

                //设定编译参数
                System.CodeDom.Compiler.CompilerParameters cplist = new System.CodeDom.Compiler.CompilerParameters();
                cplist.GenerateExecutable = false;
                cplist.GenerateInMemory = true;
                cplist.ReferencedAssemblies.Add("System.dll");
                cplist.ReferencedAssemblies.Add("System.XML.dll");
                cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
                cplist.ReferencedAssemblies.Add("System.Data.dll");

                //编译代理类
                System.CodeDom.Compiler.CompilerResults cr = csc.CompileAssemblyFromDom(cplist, ccu);
                if (cr.Errors.HasErrors == true)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                    {
                        sb.Append(ce.ToString());
                        sb.Append(System.Environment.NewLine);
                    }
                    throw new Exception(sb.ToString());
                }
                //生成代理实例
                System.Reflection.Assembly assembly = cr.CompiledAssembly;
                Type t = assembly.GetType(ns + "." + classname, true, true);
                object obj = Activator.CreateInstance(t);

                //实例化方法
                System.Reflection.MethodInfo helloWorld = t.GetMethod("DxTestDataUpload");

                //设置参数

                object device = textBox1.Text.Trim();
                object workstation_sn = textBox2.Text.Trim();
                object emp_no = textBox3.Text.Trim();
                object mo_number = textBox4.Text.Trim();
                object container_no = textBox5.Text.Trim();
                object product_sn = textBox6.Text.Trim();
                object union_list = textBox7.Text.Trim();


                object mod_level = textBox8.Text.Trim();
                object test_re = textBox9.Text.Trim();
                object test_val = textBox10.Text.Trim();
               
                object test_time = textBox11.Text.Trim();
                object mark = textBox12.Text.Trim();
                object m_flag = int.Parse(textBox13.Text.Trim());
                object[] addParams = new object[] { m_flag,device, workstation_sn, emp_no, mo_number, container_no,product_sn, union_list,mod_level,test_re,test_val, test_time,mark};
                
                //参数赋值并调用方法
                object helloWorldReturn = helloWorld.Invoke(obj, addParams);

                Console.WriteLine(helloWorldReturn.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                
            }
        }
    }
}
