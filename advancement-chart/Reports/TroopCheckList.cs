using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using advancementchart.Model;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace advancementchart.Reports
{
    public class TroopCheckList : IReport
    {
        private const int CheckboxColumns = 10;
        private const double CheckboxColumnWidth = 2.7;

        public TroopCheckList(List<TroopMember> scouts)
        {
            Scouts = scouts;
        }

        public List<TroopMember> Scouts { get; set; }

        public void Run(string outputFileName)
        {
            Console.WriteLine("Running Troop Checklist");

            if (File.Exists(outputFileName))
                File.Delete(outputFileName);

            var fi = new FileInfo(outputFileName);
            using (var package = new ExcelPackage(fi))
            {
                var wks = package.Workbook.Worksheets.Add("Troop Checklist");
                wks.Cells.Style.Font.Size += 4;

                wks.PrinterSettings.Orientation = eOrientation.Landscape;
                wks.PrinterSettings.FitToPage = true;
                wks.PrinterSettings.FitToWidth = 1;
                wks.PrinterSettings.FitToHeight = 0;

                // Header row with left/bottom/right borders, 4x height
                int row = 1;
                wks.Row(row).Height = wks.DefaultRowHeight * 8;
                for (int col = 2; col <= 1 + CheckboxColumns; col++)
                {
                    wks.Cells[row, col].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    wks.Cells[row, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    wks.Cells[row, col].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                }
                row++;

                int scoutRowCount = 0;
                bool firstPatrol = true;

                foreach (var patrolScouts in Scouts
                    .Where(s => !string.Equals(s.Patrol, "Inactive", StringComparison.OrdinalIgnoreCase))
                    .GroupBy(s => s.Patrol)
                    .OrderBy(g => g.Key))
                {
                    // Blank line before each patrol (except the first)
                    if (!firstPatrol)
                    {
                        row++;
                    }
                    firstPatrol = false;

                    // Patrol header row
                    wks.Cells[row, 1].Value = patrolScouts.Key;
                    wks.Cells[row, 1].Style.Font.Bold = true;
                    row++;

                    foreach (var scout in patrolScouts
                        .OrderBy(s => s.LastName)
                        .ThenBy(s => s.FirstName))
                    {
                        wks.Cells[row, 1].Value = scout.DisplayName;

                        // Alternating row shading on scout rows
                        if (scoutRowCount % 2 == 1)
                        {
                            wks.Cells[row, 1, row, 1 + CheckboxColumns].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            wks.Cells[row, 1, row, 1 + CheckboxColumns].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        }

                        // Checkbox borders on scout rows
                        for (int col = 2; col <= 1 + CheckboxColumns; col++)
                        {
                            wks.Cells[row, col].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            wks.Cells[row, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            wks.Cells[row, col].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            wks.Cells[row, col].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        }

                        scoutRowCount++;
                        row++;
                    }
                }

                // Set checkbox column widths
                for (int col = 2; col <= 1 + CheckboxColumns; col++)
                {
                    wks.Column(col).Width = CheckboxColumnWidth;
                }

                // Auto-fit the name column
                if (wks.Dimension != null)
                {
                    wks.Column(1).AutoFit(10);
                }

                // Freeze the header row and name column
                wks.View.FreezePanes(2, 2);

                package.Save();
            }
        }
    }
}
