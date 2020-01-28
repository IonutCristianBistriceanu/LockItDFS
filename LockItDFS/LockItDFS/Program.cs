using System;
using System.Collections.Generic;
using System.Linq;
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
                new List<int>() {0, 1, 1, 1, 0},
                new List<int>() {0, 0, 0, 0, 0}
            };

            var regions = GetAllRegions(matrix);
            foreach (var region in regions)
            {
                Console.WriteLine(region.ToString());
                Console.WriteLine(region.IsValid);
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
            PopulateRegionData(matrix, row, column, region);
            totalRegions.Add(region);
        }

        private static int PopulateRegionData(IReadOnlyList<List<int>> matrix, in int row, in int column, Region region)
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

            AddIconToColumnDictionary(column, region);
            AddIconToRowDictionary(row, region);

            for (var r = row - 1; r <= row + 1; r++)
            {
                for (var c = column - 1; c <= column + 1; c++)
                {
                    if (r == row && c == column || CheckIfDiagonal(r, c, row, column))
                        continue;

                    size += PopulateRegionData(matrix, r, c, region);
                }
            }

            region.Size = size;
            return size;
        }

        private static bool CheckIfDiagonal(int r, int c, in int row, in int column)
        {
            return ((r == row - 1 && c == column - 1) || (r == row - 1 && c == column + 1) ||
                    (r == row && c == column + 1) || (r == row + 1 && c == column + 1));
        }

        private static void AddIconToRowDictionary(int rowKey, Region region)
        {
            if (!region.IconsCountByRow.ContainsKey(rowKey))
            {
                region.IconsCountByRow.Add(rowKey, 1);
            }
            else
            {
                region.IconsCountByRow[rowKey] = ++region.IconsCountByRow[rowKey];
            }
        }

        private static void AddIconToColumnDictionary(int columnKey, Region region)
        {
            if (!region.IconsCountByColumn.ContainsKey(columnKey))
            {
                region.IconsCountByColumn.Add(columnKey, 1);
            }
            else
            {
                region.IconsCountByColumn[columnKey] = ++region.IconsCountByColumn[columnKey];
            }
        }
    }

    public class Region
    {
        public int Size { get; set; } = 0;
        private List<IconPosition> _positions;
        public Dictionary<int, int> IconsCountByRow;
        public Dictionary<int, int> IconsCountByColumn;
        public bool IsValid => IsValidRegion();

        public Region()
        {
            _positions = new List<IconPosition>();
            IconsCountByRow = new Dictionary<int, int>();
            IconsCountByColumn = new Dictionary<int, int>();
        }

        public void AddIconPosition(IconPosition iconPosition)
        {
            _positions.Add(iconPosition);
        }

        public IReadOnlyCollection<IconPosition> GetIconPositions()
        {
            return _positions;
        }

        private bool IsValidRegion()
        {
            return IconsCountByColumn.Any(icc => icc.Value == 3) || IconsCountByRow.Any(icr => icr.Value == 3);
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
        public int Row { get; }
        public int Column { get; }

        public IconPosition(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public string GetPosition()
        {
            return $"Icon is on row {Row} and column {Column}";
        }
    }
}
