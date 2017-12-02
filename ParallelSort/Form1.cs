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

        private void button3_Click(object sender, EventArgs e)
        {
            SortType type = SortType.Quick;
            if (radioButton1.Checked == true)
                type = SortType.Quick;
            if (radioButton2.Checked == true)
                type = SortType.Merge;
            if (radioButton3.Checked == true)
                type = SortType.Bubble;

            Analyser.AnalyzeOptimizedSortAlgorithm(type, textBox1.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd1 = new OpenFileDialog(),
                           ofd2 = new OpenFileDialog();

            ofd1.Title = "Choose non-optimized parallel sort results";
            ofd2.Title = "Choose optimized parallel sort results";

            if (ofd1.ShowDialog() == DialogResult.OK &&
                ofd2.ShowDialog() == DialogResult.OK)
            {
                int nCount1, maxPower1,
                    nCount2, maxPower2;
                PointF[][] res1, res2;

                res1 = FileWorker.ReadSortResultsFromFile(ofd1.FileName, out maxPower1, out nCount1);
                res2 = FileWorker.ReadSortResultsFromFile(ofd2.FileName, out maxPower2, out nCount2);

                if (nCount1 != nCount2 ||
                    maxPower2 != maxPower1)
                {
                    MessageBox.Show("Incorrect files!");
                    return;
                }

                List<Tuple<int, int, long>> list = new List<Tuple<int, int, long>>();
                for (int j = 0; j < nCount1; j++)
                    for (int i = 0; i < maxPower1 + 2; i++)
                        list.Add(new Tuple<int, int, long>((int)res1[i][j].X, i == maxPower1 + 1 ? -1 : i, (long)(res1[i][j].Y - res2[i][j].Y)));

                FileWorker.WriteSortResultsInFile(list, "difference.txt", nCount1, maxPower1);
                new Graphic("difference.txt").Show();
            }
        }
    }
}
