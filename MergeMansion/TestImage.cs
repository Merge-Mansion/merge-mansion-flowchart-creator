using QuickGraph.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MergeMansion
{
    public partial class TestImage : Form
    {
        public TestImage()
        {
            InitializeComponent();
        }

        private void TestImage_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = imageList1.Images[0];
        }
    }
}
