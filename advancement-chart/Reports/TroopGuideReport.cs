using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using advancementchart.Model;
using advancementchart.Model.Ranks;
using OfficeOpenXml;
using System.Drawing;

namespace advancementchart.Reports
{
    public class TroopGuideReport : TroopReport
    {
        public TroopGuideReport(List<TroopMember> scouts) : base(scouts)
        {
        }

        new public void Run(string outputFilename)
        {
            Console.WriteLine("Running Troop Guide Report");

            if (File.Exists(outputFilename))
                File.Delete(outputFilename);
            var fi = new FileInfo(outputFilename);
            using (var package = new ExcelPackage(fi))
            {
                CellAddress taCell = new CellAddress("A1");

                var ta = package.Workbook.Worksheets.Add("Troop Guide Report");
                ta.Cells[taCell].Value = "Troop Guide Report";

                List<Tuple<int, int>> taLines = new List<Tuple<int, int>>();

                taCell.Row++;
                taCell.Row++;
                int start = taCell.ColumnNumber;
                taCell = AddHeader(ta, taCell, new Scout());
                int end = taCell.ColumnNumber;
                taLines.Add(new Tuple<int, int>(start + 1, end));
                start = end;
                taCell = AddHeader(ta, taCell, new Tenderfoot());
                end = taCell.ColumnNumber;
                taLines.Add(new Tuple<int, int>(start + 1, end));
                start = end;
                taCell = AddHeader(ta, taCell, new SecondClass());
                end = taCell.ColumnNumber;
                taLines.Add(new Tuple<int, int>(start + 1, end));
                start = end;
                taCell = AddHeader(ta, taCell, new FirstClass());
                end = taCell.ColumnNumber;
                taLines.Add(new Tuple<int, int>(start + 1, end));

                bool taFrozen = false;

                int taHeaderRow = taCell.Row;

                int taLastRow = taHeaderRow;

                foreach (TroopMember scout in Scouts.Where(s => s.FirstClass.Earned == false).OrderBy(s => s.LastName).ThenBy(s => s.FirstName))
                {
                    taCell.Row++;
                    taCell.ColumnNumber = 1;
                    taLastRow = taCell.Row;

                    if (!taFrozen)
                    {
                        ta.View.FreezePanes(taCell.Row, taCell.ColumnNumber + 1);
                        taFrozen = true;
                    }

                    if (((taCell.Row - taHeaderRow) % 2) > 0)
                    {
                        ta.Row(taCell.Row).Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ta.Row(taCell.Row).Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    }

                    scout.AllocateMeritBadges();

                    ta.Cells[taCell].Value = $"   {scout.DisplayName}";

                    taCell = AddContent(ta, taCell, scout.Scout);
                    taCell = AddContent(ta, taCell, scout.Tenderfoot);
                    taCell = AddContent(ta, taCell, scout.SecondClass);
                    taCell = AddContent(ta, taCell, scout.FirstClass);
                }

                ta.DefaultColWidth = 0.1;
                ta.Calculate();
                ta.Cells[ta.Dimension.Address].AutoFitColumns(10);
                ta.Cells[ta.Dimension.Address].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick, Color.Black);
                foreach (var tuple in taLines)
                {
                    CellAddress a = new CellAddress(tuple.Item1, 1);
                    CellAddress b = new CellAddress(tuple.Item2, taLastRow);
                    ta.Cells[$"{a}:{b}"].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    ta.Cells[$"{a}:{b}"].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    ta.Cells[$"{a}:{b}"].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    ta.Cells[$"{a}:{b}"].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    ta.Cells[$"{a}:{b}"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick, Color.Black);
                }
                ta.Row(taHeaderRow).Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thick;

                package.Save();
            }
        }
    }
}
