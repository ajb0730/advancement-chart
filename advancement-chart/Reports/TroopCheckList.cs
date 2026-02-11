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
        private const double CheckboxColumnWidth = 4.05;
        private const int CommentsColumn = CheckboxColumns + 2;
        private const double CommentsColumnWidth = 20;

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

                wks.PrinterSettings.Orientation = eOrientation.Portrait;
                wks.PrinterSettings.FitToPage = true;
                wks.PrinterSettings.FitToWidth = 1;
                wks.PrinterSettings.FitToHeight = 0;

                // Header row with left/bottom/right borders, tall height
                int row = 1;
                wks.Row(row).Height = wks.DefaultRowHeight * 12;
                for (int col = 2; col <= 1 + CheckboxColumns; col++)
                {
                    wks.Cells[row, col].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    wks.Cells[row, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    wks.Cells[row, col].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                }
                // Comments header
                wks.Cells[row, CommentsColumn].Value = "Comments";
                wks.Cells[row, CommentsColumn].Style.Font.Bold = true;
                wks.Cells[row, CommentsColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                wks.Cells[row, CommentsColumn].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                wks.Cells[row, CommentsColumn].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                wks.Cells[row, CommentsColumn].Style.Border.Right.Style = ExcelBorderStyle.Thin;
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
                        wks.Row(row).Height = wks.DefaultRowHeight * 1.5;

                        // Alternating row shading on scout rows
                        if (scoutRowCount % 2 == 1)
                        {
                            wks.Cells[row, 1, row, CommentsColumn].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            wks.Cells[row, 1, row, CommentsColumn].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        }

                        // Checkbox borders on scout rows
                        for (int col = 2; col <= 1 + CheckboxColumns; col++)
                        {
                            wks.Cells[row, col].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            wks.Cells[row, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            wks.Cells[row, col].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            wks.Cells[row, col].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        }

                        // Comments column border
                        wks.Cells[row, CommentsColumn].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

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

                // Dynamically size comments column to fill the page
                if (wks.Dimension != null)
                {
                    int lastDataRow = row - 1;
                    double totalHeightPt = 0;
                    for (int r = 1; r <= lastDataRow; r++)
                    {
                        totalHeightPt += wks.Row(r).Height;
                    }

                    // Printable area (US Letter portrait, margins in inches -> points)
                    double printableWidthPt = (8.5 - (double)wks.PrinterSettings.LeftMargin
                                                   - (double)wks.PrinterSettings.RightMargin) * 72.0;
                    double printableHeightPt = (11.0 - (double)wks.PrinterSettings.TopMargin
                                                    - (double)wks.PrinterSettings.BottomMargin) * 72.0;

                    // Target width to fill page (match aspect ratio)
                    double targetWidthPt = Math.Max(printableWidthPt,
                        totalHeightPt * printableWidthPt / printableHeightPt);

                    // Convert to pixels (96 DPI: 1 pt = 4/3 px)
                    // Excel column width: pixel_width = width_units * MaxDigitWidth + Padding
                    // MaxDigitWidth ~= 7 (Calibri 11pt), Padding ~= 5 per column
                    const double maxDigitWidth = 7.0;
                    const double colPadding = 5.0;
                    double targetWidthPx = targetWidthPt * 96.0 / 72.0;

                    // Sum current column widths (excluding comments) in pixels
                    double otherWidthPx = 0;
                    for (int col = 1; col < CommentsColumn; col++)
                    {
                        otherWidthPx += wks.Column(col).Width * maxDigitWidth + colPadding;
                    }

                    double commentsWidthPx = targetWidthPx - otherWidthPx - colPadding;
                    double commentsWidth = Math.Max(CommentsColumnWidth, commentsWidthPx / maxDigitWidth);
                    wks.Column(CommentsColumn).Width = commentsWidth;
                }
                else
                {
                    wks.Column(CommentsColumn).Width = CommentsColumnWidth;
                }

                // Freeze the header row and name column
                wks.View.FreezePanes(2, 2);

                package.Save();
            }
        }
    }
}
