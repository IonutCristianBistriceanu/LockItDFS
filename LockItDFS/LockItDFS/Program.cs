using System;
using System.Collections.Generic;

namespace LockItDFS
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var matrix = new List<List<int>>()
            {
                new List<int>() {0, 0, 0, 1, 0},
                new List<int>() {0, 0, 1, 1, 0},
                new List<int>() {0, 0, 0, 0, 0}
            };

            Console.WriteLine(GetBiggestRegion(matrix));
        }

        private static int GetBiggestRegion(IReadOnlyList<List<int>> matrix)
        {
            var maxRegion = 0;

            for (var row = 0; row < matrix.Count; row++)
            {
                for (var column = 0; column < matrix[row].Count; column++)
                {
                    if (matrix[row][column] != 1) continue;
                    var size = GetRegionSize(matrix, row, column);
                    maxRegion = Math.Max(size, maxRegion);
                }
            }

            return maxRegion;
        }

        private static int GetRegionSize(IReadOnlyList<List<int>> matrix, in int row, in int column)
        {
            if (row < 0 || column < 0 || row >= matrix.Count || column >= matrix[row].Count)
            {
                return 0;
            }

            if (matrix[row][column] == 0)
            {
                return 0;
            }

            matrix[row][column] = 0;
            var size = 1;

            for (var r = row - 1; r <= row + 1; r++)
            {
                for (var c = column - 1; c <= column + 1; c++)
                {
                    if (r != row || c != column)
                    {
                        size += GetRegionSize(matrix, r, c);
                    }
                }
            }

            return size;
        }
    }
}