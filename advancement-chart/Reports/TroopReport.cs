using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using advancementchart.Model;
using advancementchart.Model.Ranks;
using OfficeOpenXml;

namespace advancementchart.Reports
{
    public class TroopReport : IReport
    {
        public TroopReport(List<TroopMember> scouts)
        {
            Scouts = scouts;
        }

        public List<TroopMember> Scouts { get; set; }

        private CellAddress AddHeader(ExcelWorksheet wks, CellAddress cell, Rank rank)
        {
            CellAddress rangeStart = new CellAddress(cell);
            rangeStart.ColumnNumber++;
            rangeStart.Row--;
            rangeStart.Row--;
            foreach (var req in rank.Requirements)
            {
                cell.ColumnNumber++;
                if(req is EagleMeritBadgeRequirement)
                {
                    var mbReq = req as EagleMeritBadgeRequirement;

                    CellAddress desc = new CellAddress(cell);
                    desc.Row--;

                    {
                        wks.Cells[desc].Value = "First Aid";
                        wks.Cells[desc].Style.TextRotation = 90;
                        wks.Cells[desc].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        wks.Cells[desc].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
                        wks.Cells[desc].Style.Font.Bold = true;
                        desc.ColumnNumber++;
                    }
                    {
                        wks.Cells[desc].Value = "Citizenship in the Community";
                        wks.Cells[desc].Style.TextRotation = 90;
                        wks.Cells[desc].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        wks.Cells[desc].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
                        wks.Cells[desc].Style.Font.Bold = true;
                        desc.ColumnNumber++;
                    }
                    {
                        wks.Cells[desc].Value = "Citizenship in the Nation";
                        wks.Cells[desc].Style.TextRotation = 90;
                        wks.Cells[desc].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        wks.Cells[desc].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
                        wks.Cells[desc].Style.Font.Bold = true;
                        desc.ColumnNumber++;
                    }
                    {
                        wks.Cells[desc].Value = "Citizenship in the World";
                        wks.Cells[desc].Style.TextRotation = 90;
                        wks.Cells[desc].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        wks.Cells[desc].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
                        wks.Cells[desc].Style.Font.Bold = true;
                        desc.ColumnNumber++;
                    }
                    {
                        wks.Cells[desc].Value = "Communication";
                        wks.Cells[desc].Style.TextRotation = 90;
                        wks.Cells[desc].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        wks.Cells[desc].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
                        wks.Cells[desc].Style.Font.Bold = true;
                        desc.ColumnNumber++;
                    }
                    {
                        wks.Cells[desc].Value = "Cooking";
                        wks.Cells[desc].Style.TextRotation = 90;
                        wks.Cells[desc].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        wks.Cells[desc].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
                        wks.Cells[desc].Style.Font.Bold = true;
                        desc.ColumnNumber++;
                    }
                    {
                        wks.Cells[desc].Value = "Personal Fitness";
                        wks.Cells[desc].Style.TextRotation = 90;
                        wks.Cells[desc].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        wks.Cells[desc].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
                        wks.Cells[desc].Style.Font.Bold = true;
                        desc.ColumnNumber++;
                    }
                    {
                        wks.Cells[desc].Value = "Emergency Preparedness / Lifesaving";
                        wks.Cells[desc].Style.TextRotation = 90;
                        wks.Cells[desc].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        wks.Cells[desc].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
                        wks.Cells[desc].Style.Font.Bold = true;
                        desc.ColumnNumber++;
                    }
                    {
                        wks.Cells[desc].Value = "Environmental Science / Sustainability";
                        wks.Cells[desc].Style.TextRotation = 90;
                        wks.Cells[desc].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        wks.Cells[desc].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
                        wks.Cells[desc].Style.Font.Bold = true;
                        desc.ColumnNumber++;
                    }
                    {
                        wks.Cells[desc].Value = "Personal Management";
                        wks.Cells[desc].Style.TextRotation = 90;
                        wks.Cells[desc].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        wks.Cells[desc].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
                        wks.Cells[desc].Style.Font.Bold = true;
                        desc.ColumnNumber++;
                    }
                    {
                        wks.Cells[desc].Value = "Swimming / Hiking / Cycling";
                        wks.Cells[desc].Style.TextRotation = 90;
                        wks.Cells[desc].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        wks.Cells[desc].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
                        wks.Cells[desc].Style.Font.Bold = true;
                        desc.ColumnNumber++;
                    }
                    {
                        wks.Cells[desc].Value = "Camping";
                        wks.Cells[desc].Style.TextRotation = 90;
                        wks.Cells[desc].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        wks.Cells[desc].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
                        wks.Cells[desc].Style.Font.Bold = true;
                        desc.ColumnNumber++;
                    }
                    {
                        wks.Cells[desc].Value = "Family Life";
                        wks.Cells[desc].Style.TextRotation = 90;
                        wks.Cells[desc].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        wks.Cells[desc].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
                        wks.Cells[desc].Style.Font.Bold = true;
                        desc.ColumnNumber++;
                    }

                    for (int i = 0; i < mbReq.Elective; i++)
                    {
                        wks.Cells[desc].Value = "Elective Merit Badge";
                        wks.Cells[desc].Style.TextRotation = 90;
                        wks.Cells[desc].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        wks.Cells[desc].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
                        wks.Cells[desc].Style.Font.Bold = true;
                        desc.ColumnNumber++;
                    }

                    CellAddress mbStart = new CellAddress(cell);
                    CellAddress mbEnd = new CellAddress(mbStart);
                    mbEnd.ColumnNumber += mbReq.Total - 1;
                    using (ExcelRange r = wks.Cells[$"{mbStart}:{mbEnd}"])
                    {
                        r.Merge = true;
                        r.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        r.Style.Font.Bold = true;
                        r.Value = req.Name;
                    }
                    cell.ColumnNumber = mbEnd.ColumnNumber;
                }
                else if (req is MeritBadgeRequirement)
                {
                    var mbReq = req as MeritBadgeRequirement;

                    CellAddress desc = new CellAddress(cell);
                    desc.Row--;
                    for(int i = 0; i < mbReq.Required; i++)
                    {
                        wks.Cells[desc].Value = "Eagle Required Merit Badge";
                        wks.Cells[desc].Style.TextRotation = 90;
                        wks.Cells[desc].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        wks.Cells[desc].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
                        wks.Cells[desc].Style.Font.Bold = true;
                        desc.ColumnNumber++;
                    }
                    for (int i = 0; i < mbReq.Elective; i++)
                    {
                        wks.Cells[desc].Value = "Elective Merit Badge";
                        wks.Cells[desc].Style.TextRotation = 90;
                        wks.Cells[desc].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        wks.Cells[desc].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
                        wks.Cells[desc].Style.Font.Bold = true;
                        desc.ColumnNumber++;
                    }

                    CellAddress mbStart = new CellAddress(cell);
                    CellAddress mbEnd = new CellAddress(mbStart);
                    mbEnd.ColumnNumber += mbReq.Total - 1;
                    using (ExcelRange r = wks.Cells[$"{mbStart}:{mbEnd}"])
                    {
                        r.Merge = true;
                        r.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        r.Style.Font.Bold = true;
                        r.Value = req.Name;
                    }
                    cell.ColumnNumber = mbEnd.ColumnNumber;
                }
                else
                {
                    CellAddress desc = new CellAddress(cell);
                    desc.Row--;
                    wks.Cells[desc].Value = req.Description;
                    wks.Cells[desc].Style.TextRotation = 90;
                    wks.Cells[desc].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    wks.Cells[desc].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
                    wks.Cells[desc].Style.Font.Bold = true;

                    wks.Cells[cell].Value = req.Name;
                    wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    wks.Cells[cell].Style.Font.Bold = true;
                }
            }
            cell.ColumnNumber++;
            //wks.Cells[cell].Value = $"{rank.Name} Earned";
            wks.Cells[cell].Value = "Earned";
            wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            wks.Cells[cell].Style.Font.Bold = true;
            CellAddress rangeEnd = new CellAddress(cell);
            rangeEnd.Row--;
            rangeEnd.Row--;
            using (ExcelRange r = wks.Cells[$"{rangeStart}:{rangeEnd}"])
            {
                r.Merge = true;
                r.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                r.Style.Font.Bold = true;
                r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                r.Style.Fill.BackgroundColor.SetColor(rank.FillColor);
                r.Value = rank.Name;
            }

            return cell;
        }

        private CellAddress AddContent(ExcelWorksheet wks, CellAddress cell, Rank rank)
        {
            foreach (var req in rank.Requirements)
            {
                if (req is EagleMeritBadgeRequirement)
                {
                    var mbReq = req as EagleMeritBadgeRequirement;
                    var required = mbReq.MeritBadges.Where(mb => mb.EagleRequired).OrderBy(mb => mb.DateEarned).ThenBy(mb => mb.BsaId).ToList();
                    var elective = mbReq.MeritBadges.Where(mb => !mb.EagleRequired).OrderBy(mb => mb.DateEarned).ThenBy(mb => mb.BsaId).ToList();

                    // Order is: First Aid, Cit Comm, Cit Nation, Cit World, 
                    //           Comm, Cooking, Pers Fitness, EPrep or Lifesaving
                    //           Envi Sci or Sustainability, Pers Mgmt
                    //           Swimming or Hiking or Cycling, Camping, Fam Life

                    cell.ColumnNumber++;
                    var selected = required.Where(mb => mb.Name == "First Aid");
                    if(selected.Any())
                    {
                        wks.Cells[cell].Value = selected.First().BsaId;
                        wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    }

                    cell.ColumnNumber++;
                    selected = required.Where(mb => mb.Name == "Citizenship in the Community");
                    if(selected.Any())
                    {
                        wks.Cells[cell].Value = selected.First().BsaId;
                        wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    }

                    cell.ColumnNumber++;
                    selected = required.Where(mb => mb.Name == "Citizenship in the Nation");
                    if (selected.Any())
                    {
                        wks.Cells[cell].Value = selected.First().BsaId;
                        wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    }

                    cell.ColumnNumber++;
                    selected = required.Where(mb => mb.Name == "Citizenship in the World");
                    if (selected.Any())
                    {
                        wks.Cells[cell].Value = selected.First().BsaId;
                        wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    }

                    cell.ColumnNumber++;
                    selected = required.Where(mb => mb.Name == "Communication");
                    if (selected.Any())
                    {
                        wks.Cells[cell].Value = selected.First().BsaId;
                        wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    }

                    cell.ColumnNumber++;
                    selected = required.Where(mb => mb.Name == "Cooking");
                    if (selected.Any())
                    {
                        wks.Cells[cell].Value = selected.First().BsaId;
                        wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    }

                    cell.ColumnNumber++;
                    selected = required.Where(mb => mb.Name == "Personal Fitness");
                    if (selected.Any())
                    {
                        wks.Cells[cell].Value = selected.First().BsaId;
                        wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    }

                    cell.ColumnNumber++;
                    selected = required.Where(mb => mb.Name == "Emergency Preparedness" || mb.Name == "Lifesaving");
                    if (selected.Any())
                    {
                        wks.Cells[cell].Value = selected.First().BsaId;
                        wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        if(selected.Count() > 1)
                        {
                            elective.Add(selected.Last());
                            elective = elective.OrderBy(mb => mb.DateEarned).ThenBy(mb => mb.BsaId).ToList();
                        }
                    }

                    cell.ColumnNumber++;
                    selected = required.Where(mb => mb.Name == "Environmental Science" || mb.Name == "Sustainability");
                    if (selected.Any())
                    {
                        wks.Cells[cell].Value = selected.First().BsaId;
                        wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        if (selected.Count() > 1)
                        {
                            elective.Add(selected.Last());
                            elective = elective.OrderBy(mb => mb.DateEarned).ThenBy(mb => mb.BsaId).ToList();
                        }
                    }

                    cell.ColumnNumber++;
                    selected = required.Where(mb => mb.Name == "Personal Management");
                    if (selected.Any())
                    {
                        wks.Cells[cell].Value = selected.First().BsaId;
                        wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    }

                    cell.ColumnNumber++;
                    selected = required.Where(mb => mb.Name == "Swimming" || mb.Name == "Hiking" || mb.Name == "Cycling");
                    if (selected.Any())
                    {
                        wks.Cells[cell].Value = selected.First().BsaId;
                        wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        if (selected.Count() > 1)
                        {
                            elective.Add(selected.Last());
                            if(selected.Count() > 2)
                            {
                                elective.Add(selected.SkipLast(1).Last());
                            }
                            elective = elective.OrderBy(mb => mb.DateEarned).ThenBy(mb => mb.BsaId).ToList();
                        }
                    }

                    cell.ColumnNumber++;
                    selected = required.Where(mb => mb.Name == "Camping");
                    if (selected.Any())
                    {
                        wks.Cells[cell].Value = selected.First().BsaId;
                        wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    }

                    cell.ColumnNumber++;
                    selected = required.Where(mb => mb.Name == "Family Life");
                    if (selected.Any())
                    {
                        wks.Cells[cell].Value = selected.First().BsaId;
                        wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    }

                    for (int i = 0; i < mbReq.Elective; i++)
                    {
                        cell.ColumnNumber++;
                        if (i < elective.Count())
                        {
                            wks.Cells[cell].Value = elective[i].BsaId;
                            wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        }
                    }
                }
                else if (req is MeritBadgeRequirement)
                {
                    var mbReq = req as MeritBadgeRequirement;
                    var required = mbReq.MeritBadges.Where(mb => mb.EagleRequired).OrderBy(mb => mb.DateEarned).ThenBy(mb => mb.BsaId).ToList();
                    var elective = mbReq.MeritBadges.Where(mb => !mb.EagleRequired).OrderBy(mb => mb.DateEarned).ThenBy(mb => mb.BsaId).ToList();
                    for (int i = 0; i < mbReq.Required; i++)
                    {
                        cell.ColumnNumber++;
                        if (i < required.Count())
                        {
                            wks.Cells[cell].Value = required[i].BsaId;
                            wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        }
                    }
                    for (int i = 0; i < mbReq.Elective; i++)
                    {
                        cell.ColumnNumber++;
                        if (i < elective.Count())
                        {
                            wks.Cells[cell].Value = elective[i].BsaId;
                            wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        }
                    }
                }
                else
                {
                    cell.ColumnNumber++;
                    //wks.Cells[cell].Value = req.Earned ? "X" : "";
                    wks.Cells[cell].Value = req.Earned ? "●" : "";
                    wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }
            }
            cell.ColumnNumber++;
            if(rank.Earned)
            {
                wks.Cells[cell].Value = rank.DateEarned.Value.ToString("M/yy");
                wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            }
            return cell;
        }

        public void Run(string outputFilename)
        {
            if (File.Exists(outputFilename))
                File.Delete(outputFilename);
            var fi = new FileInfo(outputFilename);
            using (var package = new ExcelPackage(fi))
            {
                CellAddress taCell = new CellAddress("A1");
                var ta = package.Workbook.Worksheets.Add("Troop Advancement");
                ta.Cells[taCell].Value = "Troop Advancement Chart";

                List<Tuple<int, int>> taLines = new List<Tuple<int, int>>();
                List<Tuple<int, int>> saLines = new List<Tuple<int, int>>();

                taCell.Row++;
                taCell.Row++;
                int start = taCell.ColumnNumber;
                taCell = AddHeader(ta, taCell, new Scout());
                int end = taCell.ColumnNumber;
                taLines.Add(new Tuple<int, int>(start+1, end));
                start = end;
                taCell = AddHeader(ta, taCell, new Tenderfoot());
                end = taCell.ColumnNumber;
                taLines.Add(new Tuple<int, int>(start+1, end));
                start = end;
                taCell = AddHeader(ta, taCell, new SecondClass());
                end = taCell.ColumnNumber;
                taLines.Add(new Tuple<int, int>(start+1, end));
                start = end;
                taCell = AddHeader(ta, taCell, new FirstClass());
                end = taCell.ColumnNumber;
                taLines.Add(new Tuple<int, int>(start+1, end));

                CellAddress saCell = new CellAddress("A1");
                var sa = package.Workbook.Worksheets.Add("Scout Advancement");
                sa.Cells[saCell].Value = "Scout Advancement Chart";

                saCell.Row++;
                saCell.Row++;
                start = saCell.ColumnNumber;
                saCell = AddHeader(sa, saCell, new Star());
                end = saCell.ColumnNumber;
                saLines.Add(new Tuple<int, int>(start+1, end));
                start = end;
                saCell = AddHeader(sa, saCell, new Life());
                end = saCell.ColumnNumber;
                saLines.Add(new Tuple<int, int>(start+1, end));
                start = end;
                saCell = AddHeader(sa, saCell, new Eagle());
                end = saCell.ColumnNumber;
                saLines.Add(new Tuple<int, int>(start+1, end));
                start = end;
                saCell = AddHeader(sa, saCell, new Palm(Palm.PalmType.Bronze));
                end = saCell.ColumnNumber;
                saLines.Add(new Tuple<int, int>(start+1, end));
                start = end;
                saCell = AddHeader(sa, saCell, new Palm(Palm.PalmType.Gold));
                end = saCell.ColumnNumber;
                saLines.Add(new Tuple<int, int>(start+1, end));
                start = end;
                saCell = AddHeader(sa, saCell, new Palm(Palm.PalmType.Silver));
                end = saCell.ColumnNumber;
                saLines.Add(new Tuple<int, int>(start+1, end));

                var mb = package.Workbook.Worksheets.Add("Merit Badges");

                bool taFrozen = false;
                bool saFrozen = false;

                int taHeaderRow = taCell.Row;
                int saHeaderRow = saCell.Row;

                int taLastRow = taHeaderRow;
                int saLastRow = saHeaderRow;

                foreach (var scout in Scouts.OrderBy(s => s.Scout.DateEarned.HasValue ? s.Scout.DateEarned : DateTime.Now).ThenBy(s => s.LastName).ThenBy(s => s.FirstName))
                {
                    scout.AllocateMeritBadges();

                    taCell.Row++;
                    taCell.ColumnNumber = 1;
                    ta.Cells[taCell].Value = $"{scout.FirstName} {scout.LastName}";

                    taLastRow = taCell.Row;

                    if(((taCell.Row - taHeaderRow) %2) > 0)
                    {
                        ta.Row(taCell.Row).Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ta.Row(taCell.Row).Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    }

                    if (!taFrozen)
                    {
                        ta.View.FreezePanes(taCell.Row, taCell.ColumnNumber + 1);
                        taFrozen = true;
                    }

                    taCell = AddContent(ta, taCell, scout.Scout);
                    taCell = AddContent(ta, taCell, scout.Tenderfoot);
                    taCell = AddContent(ta, taCell, scout.SecondClass);
                    taCell = AddContent(ta, taCell, scout.FirstClass);

                    if (scout.FirstClass.Earned)
                    {
                        saCell.Row++;
                        saCell.ColumnNumber = 1;
                        sa.Cells[saCell].Value = $"{scout.FirstName} {scout.LastName}";

                        saLastRow = saCell.Row;

                        if(((saCell.Row - saHeaderRow)%2)> 0)
                        {
                            sa.Row(saCell.Row).Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            sa.Row(saCell.Row).Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        }

                        if (!saFrozen)
                        {
                            sa.View.FreezePanes(saCell.Row, saCell.ColumnNumber + 1);
                            saFrozen = true;
                        }

                        saCell = AddContent(sa, saCell, scout.Star);
                        saCell = AddContent(sa, saCell, scout.Life);
                        saCell = AddContent(sa, saCell, scout.Eagle);
                        for(int i = 0; i < 3; i++)
                        {
                            if(scout.EaglePalms.Count > i)
                            {
                                saCell = AddContent(sa, saCell, scout.EaglePalms[i]);
                            }
                        }
                    }
                }

                CellAddress cell = new CellAddress("A1");
                foreach(var pair in MeritBadge.BsaMeritBadgeIds)
                {
                    if(cell.Row > 10)
                    {
                        mb.Cells[$"{cell.Column}1:{CellAddress.ColumnIndexToName(cell.ColumnNumber + 1)}10"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);
                        cell.ColumnNumber += 2;
                        cell.Row = 1;
                    }
                    mb.Cells[cell].Value = pair.Value;
                    cell.ColumnNumber++;
                    mb.Cells[cell].Value = $"{pair.Key}{(MeritBadge.eagleRequired.Contains(pair.Key) ? "*" :"")}";
                    cell.ColumnNumber--;
                    cell.Row++;
                }
                mb.Cells[$"{cell.Column}1:{CellAddress.ColumnIndexToName(cell.ColumnNumber + 1)}10"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);

                ta.DefaultColWidth = 0.1;
                ta.Calculate();
                ta.Cells[ta.Dimension.Address].AutoFitColumns();
                ta.Cells[ta.Dimension.Address].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick, Color.Black);
                foreach(var tuple in taLines)
                {
                    CellAddress a = new CellAddress(tuple.Item1, 1);
                    CellAddress b = new CellAddress(tuple.Item2, taLastRow);
                    ta.Cells[$"{a}:{b}"].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    ta.Cells[$"{a}:{b}"].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    ta.Cells[$"{a}:{b}"].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    ta.Cells[$"{a}:{b}"].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    ta.Cells[$"{a}:{b}"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick, Color.Black);
                }
                sa.DefaultColWidth = 0.1;
                sa.Calculate();
                sa.Cells[sa.Dimension.Address].AutoFitColumns();
                sa.Cells[sa.Dimension.Address].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick, Color.Black);
                foreach (var tuple in saLines)
                {
                    CellAddress a = new CellAddress(tuple.Item1, 1);
                    CellAddress b = new CellAddress(tuple.Item2, saLastRow);
                    sa.Cells[$"{a}:{b}"].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sa.Cells[$"{a}:{b}"].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sa.Cells[$"{a}:{b}"].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sa.Cells[$"{a}:{b}"].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sa.Cells[$"{a}:{b}"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick, Color.Black);
                }
                mb.DefaultColWidth = 0.1;
                mb.Calculate();
                mb.Cells[mb.Dimension.Address].AutoFitColumns();

                package.Save();
            }
        }
    }
}
