using System;
using System.Diagnostics;

namespace Cramer
{
    class Program
    {
        public static void Test(Func<int[]> func, String funcTitle)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = func();
            stopwatch.Stop();
            Console.WriteLine($"Время выполнения [{funcTitle}]: {stopwatch.ElapsedMilliseconds} мс");
        }
        static void Main(string[] args)
        {
            var n = 9;
            int maxValue = 1000;
            int minValue = 1;
            var A = new int[n, n];
            var B = new int[n];

            var rand = new Random();
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    A[i, j] = rand.Next() * (maxValue - minValue) + minValue;
                    B[j] = rand.Next() * (maxValue - minValue) + minValue;
                }
            }
            Test(() => CramersRule.SolveCramer(A, B), "Метод Крамера");
            Test(() => CramersRuleParallel.SolveCramer(A, B), "Параллельный метод Крамера");
        }
    }
}
