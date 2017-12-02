using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace ParallelSort
{
    static class FileWorker
    {
        public static void WriteSortResultsInFile(List<Tuple<int, int, long>> result, string fileName, int resultsCount, int maxPower)
        {
            StreamWriter writer = new StreamWriter(fileName, false);

            writer.WriteLine("{0} {1}", resultsCount, maxPower);

            foreach (Tuple<int, int, long> res in result)
                writer.WriteLine("{0} {1} {2}", res.Item1, res.Item2, res.Item3);

            writer.Close();
        }

        public static PointF[][] ReadSortResultsFromFile(string fileName, out int maxPower, out int nCount)
        {
            StreamReader reader = new StreamReader(fileName);

            string[] gap = reader.ReadLine().Split(' ');
            nCount = Convert.ToInt32(gap[0]);
            maxPower = Convert.ToInt32(gap[1]);

            PointF[][] results = new PointF[maxPower + 2][];
            for (int i = 0; i <= maxPower + 1; i++)
                results[i] = new PointF[nCount];

            for (int i = 0; i < nCount * (maxPower + 2); i++)
            {
                string[] values = reader.ReadLine().Split(' ');

                int count = Convert.ToInt32(values[0]),
                    power = Convert.ToInt32(values[1]);
                long time = Convert.ToInt64(values[2]);

                results[power != -1 ? power : maxPower + 1][i / (maxPower + 2)] = new PointF(count, time);
            }

            reader.Close();

            return results;
        }
    }
}
