using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockItDFS
{
    public class Program
    {
        public static List<Region> Regions { get; set; } = new List<Region>();

        private static void Main(string[] args)
        {
            var spinResponse = new List<List<int>>()
            {
                new List<int>() {1, 1, 1, 1, 1},
                new List<int>() {1, 1, 1, 1, 1},
                new List<int>() {1, 1, 1, 1, 1}
            };

            var watch = System.Diagnostics.Stopwatch.StartNew();
            GetAllRegions(spinResponse);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine($"Execution time {elapsedMs} ms");
            
            var filteredRegions = FilterRegions();
            foreach (var region in filteredRegions)
            {
                region.GenerateBorders(spinResponse);
                Console.WriteLine("\n");
                Console.WriteLine("-------------------------------------------------------------------------------");
                Console.WriteLine($"** Region with Id: {region.RegionId} is {GetRegionValidString(region.IsValid)}");
                Console.WriteLine("-------------------------------------------------------------------------------");
                region.PrintIconBorders();
            }
        }

        private static List<Region> FilterRegions()
        {
            return Regions.Where(r => r.IsValid).ToList();
        }

        private static string GetRegionValidString(in bool regionIsValid)
        {
            return regionIsValid ? "valid" : "invalid";
        }

        private static void GetAllRegions(IReadOnlyList<List<int>> matrix)
        {
            int regionId = 0;
            for (var row = 0; row < matrix.Count; row++)
            {
                for (var column = 0; column < matrix[row].Count; column++)
                {
                    if (matrix[row][column] != 1)
                        continue;
                    regionId++;
                    CreateNewRegion(matrix, row, column, regionId);
                }
            }
        }

        private static void CreateNewRegion(IReadOnlyList<List<int>> matrix, int row, int column,
            int regionId)
        {
            var region = new Region(regionId);
            PopulateRegionData(matrix, row, column, region);

            if (region.Size > 0)
            {
                Regions.Add(region);
            }
        }

        private static void PopulateRegionData(IReadOnlyList<List<int>> matrix, in int row, in int column,
            Region region)
        {
            if (region.ContainsIcon(row, column))
            {
                return;
            }

            if (row < 0 || column < 0 || row >= matrix.Count || column >= matrix[row].Count)
            {
                return;
            }

            if (matrix[row][column] == 0)
            {
                return;
            }

            var icon = new IconPosition(row, column, region.RegionId);

            if (IconIsInAnotherRegion(row, column))
            {
                return;
            }

            region.AddIconPosition(icon);
            AddIconToColumnDictionary(column, region);
            AddIconToRowDictionary(row, region);
            PopulateRegionData(matrix, row + 1, column, region);
            PopulateRegionData(matrix, row - 1, column, region);
            PopulateRegionData(matrix, row, column + 1, region);
            PopulateRegionData(matrix, row, column - 1, region);
        }

        private static bool IconIsInAnotherRegion(int row, int column)
        {
            return Regions.Any(region => region.ContainsIcon(row, column));
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
        public int Size => _iconPositions.Count;
        public int RegionId { get; }
        public Dictionary<int, int> IconsCountByRow;
        public Dictionary<int, int> IconsCountByColumn;
        public bool IsValid => IsValidRegion();
        private List<IconPosition> _iconPositions;


        public Region(int regionId)
        {
            RegionId = regionId;
            _iconPositions = new List<IconPosition>();
            IconsCountByRow = new Dictionary<int, int>();
            IconsCountByColumn = new Dictionary<int, int>();
        }

        public bool ContainsIcon(int row, int column)
        {
            return _iconPositions.Any(icon => icon.Row == row && icon.Column == column);
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
        public int ParentRegionId { get; }

        public IconPosition(int row, int column, int parentRegionId)
        {
            ParentRegionId = parentRegionId;
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
            if (Row + 1 > matrix.Count - 1)
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
            if (Column + 1 > matrix[Row].Count - 1)
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
