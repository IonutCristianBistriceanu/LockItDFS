using System;
using System.Collections.Generic;
using System.Text;

namespace LockItDFS
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var matrix = new List<List<int>>()
            {
                new List<int>() {1, 0, 0, 1, 0},
                new List<int>() {1, 0, 1, 1, 0},
                new List<int>() {1, 0, 0, 0, 0}
            };

            var regions = GetAllRegions(matrix);

            foreach (var region in regions)
            {
                Console.WriteLine(region.ToString());
            }
        }

        private static List<Region> GetAllRegions(IReadOnlyList<List<int>> matrix)
        {
            var totalRegions = new List<Region>();

            for (var row = 0; row < matrix.Count; row++)
            {
                for (var column = 0; column < matrix[row].Count; column++)
                {
                    if (matrix[row][column] != 1)
                        continue;

                    GetNewRegion(matrix, row, column, totalRegions);
                }
            }

            return totalRegions;
        }

        private static void GetNewRegion(IReadOnlyList<List<int>> matrix, int row, int column,
            List<Region> totalRegions)
        {
            var region = new Region();
            GetRegionSize(matrix, row, column, region);
            totalRegions.Add(region);
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

            region.Size = size;
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

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var iconPosition in _positions)
            {
                sb.AppendLine(iconPosition.GetPosition());
            }

            return sb.ToString();
        }
    }

    public class IconPosition
    {
        private readonly int _row;
        private readonly int _column;

        public IconPosition(int row, int column)
        {
            _row = row;
            _column = column;
        }

        public string GetPosition()
        {
            return $"Icon is on row {_row} and column {_column}";
        }
    }
}