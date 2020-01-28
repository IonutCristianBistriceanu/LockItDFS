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
            var spinResponse = new List<List<int>>()
            {
                new List<int>() {1, 0, 0, 1, 0},
                new List<int>() {0, 1, 1, 1, 0},
                new List<int>() {0, 0, 0, 0, 0}
            };

            var clonedSpinResponse = spinResponse.CreateClone();
            var regions = GetAllRegions(clonedSpinResponse);

            foreach (var region in regions)
            {
                if (!region.IsValid)
                {
                    continue;
                }

                region.GenerateBorders(spinResponse);
                Console.WriteLine($"** Region is {GetRegionValidString(region.IsValid)}");
                region.PrintIconBorders();
            }
        }

        private static string GetRegionValidString(in bool regionIsValid)
        {
            return regionIsValid ? "valid" : "invalid";
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
        private List<IconPosition> _iconPositions;
        public Dictionary<int, int> IconsCountByRow;
        public Dictionary<int, int> IconsCountByColumn;
        public bool IsValid => IsValidRegion();

        public Region()
        {
            _iconPositions = new List<IconPosition>();
            IconsCountByRow = new Dictionary<int, int>();
            IconsCountByColumn = new Dictionary<int, int>();
        }

        public void AddIconPosition(IconPosition iconPosition)
        {
            _iconPositions.Add(iconPosition);
        }

        public IReadOnlyCollection<IconPosition> GetIconPositions()
        {
            return _iconPositions;
        }

        private bool IsValidRegion()
        {
            return IconsCountByColumn.Any(icc => icc.Value == 3) || IconsCountByRow.Any(icr => icr.Value == 3);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var iconPosition in _iconPositions)
            {
                sb.AppendLine(iconPosition.GetPosition());
            }

            return sb.ToString();
        }

        public void GenerateBorders(List<List<int>> matrix)
        {
            foreach (var icon in _iconPositions)
            {
                icon.GenerateBorders(matrix);
            }
        }

        public void PrintIconBorders()
        {
            foreach (var icon in _iconPositions)
            {
                icon.PrintBorders();
            }
        }
    }

    public class IconPosition
    {
        public int Row { get; }
        public int Column { get; }
        public readonly Dictionary<string, bool> Borders;

        public IconPosition(int row, int column)
        {
            Row = row;
            Column = column;
            Borders = new Dictionary<string, bool>()
            {
                {"top", false},
                {"right", false},
                {"bottom", false},
                {"left", false},
            };
        }

        public string GetPosition()
        {
            return $"Icon is on row {Row} and column {Column}";
        }

        public void PrintBorders()
        {
            Console.WriteLine("\n");
            Console.WriteLine("============================================");
            Console.WriteLine($"Icon is on row {Row} and column {Column}");
            Console.WriteLine("============================================");
            foreach (var border in Borders)
            {
                if (border.Value)
                {
                    Console.WriteLine($"Icon has border on the {border.Key}");
                }
            }
        }

        public void GenerateBorders(List<List<int>> matrix)
        {
            CalculateTopBorder(matrix);
            CalculateRightBorder(matrix);
            CalculateBottomBorder(matrix);
            CalculateLeftBorder(matrix);
        }

        private void CalculateLeftBorder(List<List<int>> matrix)
        {
            if (Column - 1 < 0)
            {
                Borders["left"] = true;
            }
            else
            {
                Borders["left"] = matrix[Row][Column - 1] != 1;
            }
        }

        private void CalculateBottomBorder(List<List<int>> matrix)
        {
            if (Row + 1 > matrix[Row].Count)
            {
                Borders["bottom"] = true;
            }
            else
            {
                Borders["bottom"] = matrix[Row + 1][Column] != 1;
            }
        }

        private void CalculateRightBorder(List<List<int>> matrix)
        {
            if (Column + 1 > matrix[Row].Count)
            {
                Borders["right"] = true;
            }
            else
            {
                Borders["right"] = matrix[Row][Column + 1] != 1;
            }
        }

        private void CalculateTopBorder(List<List<int>> matrix)
        {
            if (Row - 1 < 0)
            {
                Borders["top"] = true;
            }
            else
            {
                Borders["top"] = matrix[Row - 1][Column] != 1;
            }
        }
    }
}