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
                new List<int>() {1, 0, 0, 1, 0},
                new List<int>() {1, 1, 1, 1, 0},
                new List<int>() {1, 0, 0, 0, 0}
            };

            Console.WriteLine(GetBiggestRegion(matrix));
        }

        private static int GetBiggestRegion(IReadOnlyList<List<int>> matrix)
        {
            var maxRegion = 0;
            var totalRegions = new List<Region>();

            for (var row = 0; row < matrix.Count; row++)
            {
                for (var column = 0; column < matrix[row].Count; column++)
                {
                    if (matrix[row][column] != 1) continue;
                    var region  = new Region();
                    var size = GetRegionSize(matrix, row, column, region);
                    region.Size = size;
                    maxRegion = Math.Max(size, maxRegion);
                    totalRegions.Add(region);
                }
            }
            
                        
            return maxRegion;
        }

        private static int GetRegionSize(IReadOnlyList<List<int>> matrix, in int row, in int column, Region region)
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
            region.AddIconPosition(new IconPosition(row, column));
            
            for (var r = row - 1; r <= row + 1; r++)
            {
                for (var c = column - 1; c <= column + 1; c++)
                {
                    if (r != row || c != column)
                    {
                        size += GetRegionSize(matrix, r, c, region);
                    }
                }
            }

            return size;
        }
    }

    public class Region
    {
        public int Size { get; set; } = 0;
        private List<IconPosition> _positions;

        public Region()
        {
            _positions = new List<IconPosition>();
        }

        public void AddIconPosition(IconPosition iconPosition)
        {
            _positions.Add(iconPosition);
        }
    }

    public class IconPosition
    {
        private readonly int _row;
        private readonly int _column;

        public IconPosition(int row, int column)
        {
            this._row = row;
            _column = column;
        }
    }
}