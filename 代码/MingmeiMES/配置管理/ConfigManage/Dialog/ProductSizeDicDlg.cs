using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FTDataAccess.BLL;
using FTDataAccess.Model;
namespace ConfigManage
{
    public partial class ProductSizeDicDlg : Form
    {
        private string[] dataArray = new string[3] { "", "", "" };
        private bool opAdd = true; //增加（修改）操作
        string dicCata = "";
        public string[] DataArray { get { return dataArray; } set { dataArray = value; } }
        public ProductSizeDicDlg(string dicName,bool opAdd)
        {
            this.opAdd = opAdd;
            InitializeComponent();
            this.Text = dicName;
            this.dicCata = dicName;
        }
        public ProductSizeDicDlg()
        {
            InitializeComponent();
        }

        private void ProductSizeDicDlg_Load(object sender, EventArgs e)
        {

            if(!opAdd)
            {
                textBox1.Enabled = false;
                this.textBox1.Text = dataArray[0];
                this.textBox2.Text = dataArray[1];
                this.richTextBox1.Text = dataArray[2];
            }
            else
            {
                textBox1.Enabled = true;
            }
            if(this.dicCata == "产品高度字典")
            {
                this.label1.Text = "产品高度";
            }
            else
            {
                this.label1.Text = "产品包装尺寸";
            }

            ToolTip toolTip1 = new ToolTip();
            toolTip1.AutoPopDelay = 2000;
            toolTip1.IsBalloon = true;
            toolTip1.SetToolTip(this.textBox2,"给PLC发送的值");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(opAdd)
            {
                //增加
                if(this.dicCata == "产品高度字典")
                {
                    ProductHeightDefModel hgt = new ProductHeightDefModel();
                    hgt.productHeight = int.Parse(this.textBox1.Text);
                    hgt.heightSeq = int.Parse(this.textBox2.Text);
                    hgt.mark = this.richTextBox1.Text;
                    ProductHeightDefBll hgtBll = new ProductHeightDefBll();
                    if(hgtBll.Exists(hgt.productHeight))
                    {
                        MessageBox.Show("该高度定义已经存在");
                    }
                    else
                    {
                        hgtBll.Add(hgt);
                    }
                    
                }
                else
                {
                    ProductPacsizeDefModel packSize = new ProductPacsizeDefModel();
                    packSize.packageSize = this.textBox1.Text;
                    packSize.packageSeq = int.Parse(this.textBox2.Text);
                    packSize.mark = this.richTextBox1.Text;
                    ProductPacsizeDefBll packSizeBll = new ProductPacsizeDefBll();
                    if(packSizeBll.Exists(packSize.packageSize))
                    {
                        MessageBox.Show("该包装尺寸定义已经存在");
                    }
                    else
                    {
                        packSizeBll.Add(packSize);
                    }
                }
            }
            else
            {
                //修改
                if (this.dicCata == "产品高度字典")
                {
                    ProductHeightDefModel hgt = new ProductHeightDefModel();
                    hgt.productHeight = int.Parse(this.textBox1.Text);
                    hgt.heightSeq = int.Parse(this.textBox2.Text);
                    hgt.mark = this.richTextBox1.Text;
                    ProductHeightDefBll hgtBll = new ProductHeightDefBll();
                    if (hgtBll.Exists(hgt.productHeight))
                    {
                        hgtBll.Update(hgt);
                    }
                    else
                    {
                        hgtBll.Add(hgt);
                    }
                    
                }
                else
                {
                    ProductPacsizeDefModel packSize = new ProductPacsizeDefModel();
                    packSize.packageSize = this.textBox1.Text;
                    packSize.packageSeq = int.Parse(this.textBox2.Text);
                    packSize.mark = this.richTextBox1.Text;
                    ProductPacsizeDefBll packSizeBll = new ProductPacsizeDefBll();
                    if (packSizeBll.Exists(packSize.packageSize))
                    {
                        packSizeBll.Update(packSize);
                    }
                    else
                    {
                        packSizeBll.Add(packSize);
                    }
                }
            }
            this.DialogResult = DialogResult.OK;
        }
    }
}
