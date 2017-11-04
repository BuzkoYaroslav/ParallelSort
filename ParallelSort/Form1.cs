using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParallelSort
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SortType type = SortType.Quick;
            if (radioButton1.Checked == true)
                type = SortType.Quick;
            if (radioButton2.Checked == true)
                type = SortType.Merge;
            if (radioButton3.Checked == true)
                type = SortType.Bubble;

            Analyser.AnalyzeSortAlgorithm(type, textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                new Graphic(ofd.FileName).Show();
            }
        }
    }
}
