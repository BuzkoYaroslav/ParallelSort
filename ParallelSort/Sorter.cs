using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelSort
{
    class Sorter<T> where T : IComparable
    {
        private int maxNumberOfThreads;
        private int usedThreads;
        private List<Thread> threads;

        public Sorter(int maxNumberOfThreads = 1)
        {
            this.maxNumberOfThreads = maxNumberOfThreads;
            usedThreads = 0;
            threads = new List<Thread>();
            threads.Add(Thread.CurrentThread);
        }

        public void QuickSort(T[] array)
        {
            QuickSort(array, 0, array.Length - 1);
        }
        public void QuickSort(T[] array, int left, int right)
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

            if (left < j) QuickSort(array, left, j);
            if (right > i) QuickSort(array, i, right);
        }

        public void BubbleSort(T[] array)
        {
            int n = array.Length;

            for (int i = 1; i < n; i++)
                for (int j = 0; j < n - i; j++)
                    if (array[j].CompareTo(array[j + 1]) > 0)
                        Swap(ref array[j], ref array[j + 1]);
        }
        private static void Swap<U>(ref U x, ref U y)
        {
            U temp = x;
            x = y;
            y = temp;
        }

        public void MergeSort(T[] array)
        {
            T[] copy = new T[array.Length];
            for (int i = 0; i < copy.Length; i++)
                copy[i] = array[i];

            SplitMerge(copy, 0, copy.Length, array);
        }
        private void SplitMerge(T[] arrayCopy, int begin, int end, T[] array)
        {
            if (end - begin < 2)
                return;

            int middle = (begin + end) / 2;

            SplitMerge(array, begin, middle, arrayCopy);
            SplitMerge(array, middle, end, arrayCopy);

            Merge(arrayCopy, begin, middle, end, array);
        }
        private void Merge(T[] source, int begin, int middle, int end, T[] array)
        {
            int i = begin,
                j = middle;

            for (int k = begin; k < end; k++)
                if (i < middle && (j >= end || source[j].CompareTo(source[i]) >= 0))
                {
                    array[k] = source[i];
                    i++;
                } else
                {
                    array[k] = source[j];
                    j++;
                }
        }

        public void ParallelQuickSort(T[] array)
        {
            ParallelQS(array, 0, array.Length - 1);

            for (int i = 0; i < threads.Count; i++)
                if (threads[i] != Thread.CurrentThread)
                    threads[i].Join();

            ReleaseResouces();
        }
        private void ParallelQS(T[] array, int left, int right)
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
                StartInParallelIfAllowed(() => {
                    if (!threads.Contains(Thread.CurrentThread))
                        threads.Add(Thread.CurrentThread);
                    ParallelQS(array, left, j);
                    
                });
            }
            if (right > i) ParallelQS(array, i, right);
        }

        public void ParallelMergeSort(T[] array)
        {
            T[] copy = new T[array.Length];
            for (int i = 0; i < copy.Length; i++)
                copy[i] = array[i];

            ParallelSplitMerge(copy, 0, copy.Length, array);

            ReleaseResouces();
        }
        private void ParallelSplitMerge(T[] arrayCopy, int begin, int end, T[] array)
        {
            if (end - begin < 2)
                return;

            int middle = (begin + end) / 2;

            Thread thrd = StartInParallelIfAllowed(() => { ParallelSplitMerge(array, begin, middle, arrayCopy); });
            ParallelSplitMerge(array, middle, end, arrayCopy);

            if (thrd != Thread.CurrentThread) thrd.Join();
            ParallelMerge(arrayCopy, begin, middle, end, array);
        }
        private void ParallelMerge(T[] source, int begin, int middle, int end, T[] array)
        {
            int i = begin,
                j = middle;

            for (int k = begin; k < end; k++)
                if (i < middle && (j >= end || source[j].CompareTo(source[i]) >= 0))
                {
                    array[k] = source[i];
                    i++;
                }
                else
                {
                    array[k] = source[j];
                    j++;
                }
        }

        public void ParallelBubbleSort(T[] array)
        {
            int n = array.Length;

            for (int i = 0; i < n; i++)
            {
                int startIndex = i % 2 == 0 ? 0 : 1;
                int comparisonNum = (n - startIndex) / 2,
                    usedThreads = maxNumberOfThreads;
                if (maxNumberOfThreads > comparisonNum)
                    usedThreads = comparisonNum;

                threads.Clear();
                for (int j = 0; j < usedThreads; j++)
                {
                    int numInThrd = j == usedThreads - 1 ? 
                        comparisonNum - comparisonNum / usedThreads * (usedThreads - 1) : 
                        comparisonNum / usedThreads;
                    int index = startIndex;
                    Thread newTrd = StartInParallelIfAllowed(() =>
                    {
                        ParallelComparison(array, index, index + 2 * numInThrd - 1);
                    });
                    if (newTrd != Thread.CurrentThread)
                        threads.Add(newTrd);
                    startIndex += 2 * numInThrd;
                }

                foreach (Thread t in threads)
                    t.Join();
            }

            ReleaseResouces();
        }
        private void ParallelComparison(T[] array, int left, int right)
        {
            for (int i = left; i < right; i += 2)
                if (array[i].CompareTo(array[i + 1]) > 0)
                    Swap(ref array[i], ref array[i + 1]);
        }

        private Thread StartInParallelIfAllowed(Action action)
        {
            if (IsNewThreadsAvailable())
            {
                Interlocked.Increment(ref usedThreads);
                Thread newThrd = new Thread(() =>
                {
                    action();
                    Interlocked.Decrement(ref usedThreads);
                });
                newThrd.Start();

                return newThrd;
            } else
            {
                action();
                return Thread.CurrentThread;
            }
        }
        private bool IsNewThreadsAvailable()
        {
            lock (this)
            {
                return maxNumberOfThreads - usedThreads > 0;
            }
        }

        private void ReleaseResouces()
        {
            usedThreads = 0;
            threads.Clear();
        }
    }
}
