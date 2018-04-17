using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace LineNodes.View
{
    public partial class UserControlLine : UserControl
    {
        private List<UserControlCtlNode> nodeList = new List<UserControlCtlNode>();
        public string LineName { get { return this.label1.Text; } set { this.label1.Text = value; } }
        public UserControlLine()
        {
           
            InitializeComponent();
           
        }
        public void Add(UserControlCtlNode node)
        {
            this.flowLayoutPanel1.Controls.Add(node);
            nodeList.Add(node);
        }
        public void ReLayout()
        {

            int nodeCount = nodeList.Count();
            if (nodeCount > 0)
            {
                Size boxSize = new Size(0, 0);
                boxSize.Width = (int)(this.flowLayoutPanel1.Width / (float)nodeCount-10);
                boxSize.Height = (int)(this.flowLayoutPanel1.Height);
                for (int i = 0; i < nodeCount;i++ )
                {
                    UserControlCtlNode node = nodeList[i];
                   
                    node.Size = boxSize;
                }
            }
        }

        private void flowLayoutPanel1_SizeChanged(object sender, EventArgs e)
        {
            ReLayout();
        }
    }
}
