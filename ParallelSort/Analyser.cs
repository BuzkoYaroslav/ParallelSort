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
        private const int minCount = 1000000;
        private const int maxCount = 3000000;
        private const int step = 1000000;
        private const int minPower = 0;
        private const int maxPower = 3;
        private const int experimentCount = 5;

        private const int minValue = -1000;
        private const int maxValue = 1000;

        private static Random rnd = new Random();

        public static void AnalyzeSortAlgorithm(SortType type, string fileName)
        {
            Analyze(type, (num) => new Sorter<int>(num), fileName);
        }
        public static void AnalyzeOptimizedSortAlgorithm(SortType type, string fileName)
        {
            Analyze(type, (num) => new OptimizedSorter<int>(num), fileName);
        }

        private static void Analyze(SortType type, Func<int, Sorter<int>> factoryFunc, string fileName)
        {
            Sorter<int>[] sorters = new Sorter<int>[maxPower + 1];
            Action<int[]>[] algorithms = new Action<int[]>[maxPower + 2];
            for (int i = 0; i <= maxPower; i++)
            {
                sorters[i] = factoryFunc(Convert.ToInt32(Math.Pow(2, i)));
                algorithms[i] = SortMethod(sorters[i], type);
            }
            algorithms[maxPower + 1] = StaticSortMethod(sorters[0], type);

            AnalyzeSortAlgorithm(algorithms, fileName);
        }
        private static void AnalyzeSortAlgorithm(Action<int[]>[] algorithms, string fileName)
        {
            List<Tuple<int, int, long>> results = new List<Tuple<int, int, long>>();
            Stopwatch wtch = new Stopwatch();

            for (int j = minCount; j <= maxCount; j += step)
            {
                int[] array = GenerateRandomArray(j);

                for (int i = 0; i <= maxPower + 1; i++)
                {
                    long time = 0;

                    for (int p = 0; p < experimentCount; p++)
                    {
                        int[] copyArr = new int[j];
                        array.CopyTo(copyArr, 0);
                        wtch.Reset();
                        wtch.Start();
                        algorithms[i](copyArr);
                        wtch.Stop();

                        time += wtch.ElapsedMilliseconds;
                    }


                    results.Add(new Tuple<int, int, long>(j, i == maxPower + 1 ? -1 : i, time / experimentCount));
                }
            }

            FileWorker.WriteSortResultsInFile(results, fileName, (maxCount - minCount) / step + 1, maxPower);
        }

        private static int[] GenerateRandomArray(int count)
        {
            int[] array = new int[count];
            for (int i = 0; i < count; i++)
                array[i] = rnd.Next(minValue, maxValue);

            return array;
        }
        private static Action<int[]> SortMethod(Sorter<int> sorter, SortType type)
        {
            switch (type)
            {
                case SortType.Bubble:
                        return sorter.ParallelBubbleSort;
                case SortType.Merge:
                        return sorter.ParallelMergeSort;
                case SortType.Quick:
                        return sorter.ParallelQuickSort;
                default:
                    return sorter.BubbleSort;
            }
        }
        private static Action<int[]> StaticSortMethod(Sorter<int> sorter, SortType type)
        {
            switch (type)
            {
                case SortType.Bubble:
                    return sorter.BubbleSort;
                case SortType.Merge:
                    return sorter.MergeSort;
                case SortType.Quick:
                    return sorter.QuickSort;
                default:
                    return sorter.BubbleSort;
            }
        }
    }
}
