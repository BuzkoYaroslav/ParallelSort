using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using library;

namespace ParallelSort
{
    public partial class Graphic : Form
    {
        private GraphicPainter painter;
        private Random rnd = new Random();
        private const int width = 3;

        public Graphic()
        {
            InitializeComponent();
            painter = new GraphicPainter(pictureBox1);
        }

        public Graphic(string fileName): this()
        {
            ConfigureForm(fileName);
        }

        private void ConfigureForm(string fileName)
        {
            int nCount, maxPower;
            PointF[][] results;

            results = FileWorker.ReadSortResultsFromFile(fileName, out maxPower, out nCount);

            float[] timeArray = new float[maxPower + 2];
            for (int i = 0; i < maxPower + 2; i++)
                timeArray[i] = results[i][nCount - 1].Y;

            Point scale = SetScale(
                results[0][nCount - 1].X,
                MaxValue(timeArray));
            
            for (int i = 0; i < nCount; i++)
                for (int j = 0; j <= maxPower + 1; j++)
                    results[j][i] = new PointF(results[j][i].X / (float)Math.Pow(10, scale.X),
                        results[j][i].Y / (float)Math.Pow(10, scale.Y));

            listView1.Items.Add(new ListViewItem(string.Format("Scale: X - 1:{0} ; Y - 1:{1}", 
                Math.Pow(10, scale.X), Math.Pow(10, scale.Y))));

            for (int i = 0; i <= maxPower + 1; i++)
            {
                Color color = GetRandColor();
                painter.DrawPath(painter.LinePathFromDots(results[i]), color, 
                    width, color, 0);
                painter.DrawPath(painter.DotsPathFromDots(results[i]), color, 
                    width, Color.White, 2.55);

                ListViewItem item = new ListViewItem(new string[1] {
                    i == maxPower + 1 ? "Static sort" : string.Format("number of threads - {0}", Math.Pow(2, i)) },
                    0, color, color, new Font(Font.SystemFontName, 10));

                listView1.Items.Add(item);
            }
        }

        private T MaxValue<T>(params T[] vals) where T : IComparable
        {
            T max = vals[0];
            for (int i = 0; i < vals.Length; i++)
                if (vals[i].CompareTo(max) > 0)
                    max = vals[i];

            return max;
        }
        private Point SetScale(double maxCount, double maxTime)
        {
            int step2 = 0;
            while (maxTime > 10)
            {
                step2++;
                maxTime /= 10;
            }
            int step1 = 0;
            while (maxCount > 10)
            {
                step1++;
                maxCount /= 10;
            }

            if (maxCount == 1)
            {
                maxCount = 10;
                step1--;
            }
            if (maxTime == 1)
            {
                maxTime = 10;
                step2--;
            }

            painter.XBounds = new Point(-1, (int)maxCount + 1);
            painter.YBounds = new Point(-1, (int)maxTime + 1);

            return new Point(step1, step2);
        }
        private Color GetRandColor()
        {
            return Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
        }
    }
}
