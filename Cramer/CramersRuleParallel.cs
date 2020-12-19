using System;
using System.Threading;
using System.Collections.Generic;


namespace Cramer
{
    class CramersRuleParallel
    {
        private static int ProcessorCount => Environment.ProcessorCount;
        private class SubMatrix
        {
            private int[,] martix;
            private SubMatrix prev;
            private int[] solutions;
            public SubMatrix(int[,] martix, int[] solutions)
            {
                this.martix = martix;
                this.solutions = solutions;
                this.prev = null;
                this.ColumnIndex = -1;
                Size = solutions.Length;
            }
            private SubMatrix(SubMatrix prev, int deletedColumnIndex = -1)
            {
                this.martix = null;
                this.prev = prev;
                this.ColumnIndex = deletedColumnIndex;
                Size = prev.Size - 1;
            }

            public int ColumnIndex { get; set; }
            public int Size { get; }

            public int this[int row, int column]
            {
                get
                {
                    if (martix != null)
                    {
                        return column == ColumnIndex ? solutions[row] : martix[row, column];
                    }
                    return prev[row + 1, column < ColumnIndex ? column : column + 1];
                }
            }
            public int Det()
            {
                if (Size == 1) return this[0, 0];
                if (Size == 2) return this[0, 0] * this[1, 1] - this[0, 1] * this[1, 0];
                SubMatrix m = new SubMatrix(this);
                int det = 0;
                int sign = 1;
                for (int c = 0; c < Size; c++)
                {
                    m.ColumnIndex = c;
                    int d = m.Det();
                    det += this[0, c] * d * sign;
                    sign = -sign;
                }
                return det;
            }
        }
        public static int[] SolveCramer(int[,] A, int[] B)
        {
            /*int n = A.GetLength(0);
            var matrix = new int[n, n + 1];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    matrix[i, j] = A[i, j];
                }
                matrix[i, n] = B[i];
            }*/
            return Solve(new SubMatrix(A, B));
        }
        private static int[] Solve(SubMatrix matrix)
        {
            var threads = new List<Thread>();

            int det = matrix.Det();
            if (det == 0) throw new ArgumentException("Ответ: Определитель равен 0");

            int[] result = new int[matrix.Size];
            for (int i = 0; i < matrix.Size; i++)
            {
                SubMatrix matrix_i;
                matrix_i = matrix;
                matrix_i.ColumnIndex = i;
                var thread = new Thread(new ThreadStart(() => result[i] = matrix_i.Det() / det));
            }
            foreach (var thread in threads)
            {
                thread.Join();
            }
            return result;
        }
    }
}
