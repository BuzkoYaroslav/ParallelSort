using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace ParallelSort
{
    enum SortType { Bubble, Merge, Quick};
    static class Analyser
    {
        private const int minCount = 1000;
        private const int maxCount = 10000;
        private const int step = 100;
        private const int minPower = 0;
        private const int maxPower = 4;

        private const int minValue = -1000;
        private const int maxValue = 1000;

        private static Random rnd = new Random();

        public static void AnalyzeSortAlgorithm(SortType type, string fileName)
        {
            List<Tuple<int, int, long>> results = new List<Tuple<int, int, long>>();
            Stopwatch wtch = new Stopwatch();

            Sorter<int>[] sorters = new Sorter<int>[maxPower + 1];
            Action<int[]>[] algorithms = new Action<int[]>[maxPower + 1];
            for (int i = 0; i <= maxPower; i++)
            {
                sorters[i] = new Sorter<int>(Convert.ToInt32(Math.Pow(2, i)));
                algorithms[i] = SortMethod(sorters[i], type, i != 0);
            }

            for (int j = minCount; j <= maxCount; j += step)
            {
                int[] array = GenerateRandomArray(j);

                for (int i = 0; i <= maxPower; i++)
                {
                    int[] copyArr = new int[j];
                    array.CopyTo(copyArr, 0);
                    wtch.Reset();

                    wtch.Start();
                    algorithms[i](copyArr);
                    wtch.Stop();

                    results.Add(new Tuple<int, int, long>(j, i, wtch.ElapsedMilliseconds));
                }
            }

            WriteResultsInFile(results, fileName);
        }

        private static void WriteResultsInFile(List<Tuple<int, int, long>> result, string fileName)
        {
            StreamWriter writer = new StreamWriter(fileName, false);

            writer.WriteLine("{0} {1}", (maxCount - minCount) / step + 1, maxPower);

            foreach (Tuple<int, int, long> res in result)
                writer.WriteLine("{0} {1} {2}", res.Item1, res.Item2, res.Item3);

            writer.Close();
        }
        private static int[] GenerateRandomArray(int count)
        {
            int[] array = new int[count];
            for (int i = 0; i < count; i++)
                array[i] = rnd.Next(minValue, maxValue);

            return array;
        }
        private static Action<int[]> SortMethod(Sorter<int> sorter, SortType type, bool parallel)
        {
            switch (type)
            {
                case SortType.Bubble:
                    if (parallel)
                        return sorter.ParallelBubbleSort;
                    else
                        return sorter.BubbleSort;
                case SortType.Merge:
                    if (parallel)
                        return sorter.ParallelMergeSort;
                    else
                        return sorter.MergeSort;
                case SortType.Quick:
                    if (parallel)
                        return sorter.ParallelQuickSort;
                    else
                        return sorter.QuickSort;
                default:
                    return sorter.BubbleSort;
            }
        }
    }
}
