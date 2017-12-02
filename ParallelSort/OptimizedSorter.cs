using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ParallelSort
{
    class OptimizedSorter<T> : Sorter<T> where T : IComparable
    {
        private CountdownEvent cEvent;

        public OptimizedSorter(int maxNumberOfThreads = 1) : base(maxNumberOfThreads)
        {

        }

        protected override void InitializeComponents(int threadsCount)
        {
            maxNumberOfThreads = threadsCount;

            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(maxNumberOfThreads, maxNumberOfThreads);
        }

        public override void ParallelBubbleSort(T[] array)
        {
            int n = array.Length;
            ParallelOptions opt = new ParallelOptions() { MaxDegreeOfParallelism = maxNumberOfThreads };
            for (int i = 0; i < n; i++)
            {
                int startIndex = i % 2 == 0 ? 0 : 1;
                int comparisonNum = (n - startIndex) / 2,
                    usedThreads = maxNumberOfThreads;
                if (maxNumberOfThreads > comparisonNum)
                    usedThreads = comparisonNum;
                int numInAll = comparisonNum / usedThreads,
                    numInLast = comparisonNum - comparisonNum / usedThreads * (usedThreads - 1);

                Parallel.For(0, usedThreads, opt, j =>
                {
                    int numInThrd = j == usedThreads - 1 ?
                        numInLast :
                        numInAll;
                    int index = startIndex + j * numInAll;
                    ParallelComparison(array, index, index + 2 * numInThrd - 1);
                });            
            }
        }

        public override void ParallelQuickSort(T[] array)
        {
            cEvent = new CountdownEvent(1);

            ParallelQS(array, 0, array.Length - 1);

            cEvent.Signal();
            cEvent.Wait();
            
        }

        protected override void ParallelQS(T[] array, int left, int right)
        {
            int i = left,
               j = right;

            T element = array[(left + right) / 2];

            while (i <= j)
            {
                while (array[i].CompareTo(element) < 0) i++;
                while (array[j].CompareTo(element) > 0) j--;

                if (i <= j)
                {
                    T tmp = array[i];
                    array[i] = array[j];
                    array[j] = tmp;
                    i++;
                    j--;
                }
            }

            if (left < j)
            {
                cEvent.AddCount(1);
                ThreadPool.QueueUserWorkItem((state)=>
                {
                    ParallelQS(array, left, j);
                    cEvent.Signal();
                });
            }
            if (right > i)
            {
                cEvent.AddCount(1);
                ThreadPool.QueueUserWorkItem((state) =>
                {
                    ParallelQS(array, i, right);
                    cEvent.Signal();
                });
            }
        }

        public override void ParallelMergeSort(T[] array)
        {
            T[] copy = new T[array.Length];
            for (int i = 0; i < copy.Length; i++)
                copy[i] = array[i];

            ParallelSplitMerge(copy, 0, copy.Length, array, Convert.ToInt32(Math.Log(maxNumberOfThreads, 2)));
        }

        protected void ParallelSplitMerge(T[] arrayCopy, int begin, int end, T[] array, int level)
        {
            if (end - begin < 2)
                return;

            int middle = (begin + end) / 2;

            if (level > 0)
            {
                Parallel.Invoke(
                    () => ParallelSplitMerge(array, begin, middle, arrayCopy, level - 1),
                    () => ParallelSplitMerge(array, middle, end, arrayCopy, level - 1)
                );
            } else
            {
                ParallelSplitMerge(array, begin, middle, arrayCopy, level - 1);
                ParallelSplitMerge(array, middle, end, arrayCopy, level - 1);
            }

            ParallelMerge(arrayCopy, begin, middle, end, array);
        }
    }
}
