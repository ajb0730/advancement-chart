using System.Collections.Generic;
using advancementchart.Model;
using System.IO;
using OfficeOpenXml;
using System.Linq;
using System;


namespace advancementchart.Reports
{
    public class EagleReport : IReport
    {
        public EagleReport(List<TroopMember> scouts)
        {
            Scouts = scouts;
        }

        public List<TroopMember> Scouts { get; set; }

        private CellAddress AddRank(ExcelWorksheet wks, CellAddress cell, Rank rank, DateTime lastStartDate)
        {
            wks.Cells[cell].Value = $"{rank.Name} Board of Review:";
            wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            cell.ColumnNumber++;
            if(rank.Earned && rank.DateEarned <= lastStartDate)
            {
                wks.Cells[cell].Value = $"{rank.DateEarned.Value.ToShortDateString()}";
            }
            else
            {
                wks.Cells[cell].Value = $"on or before {lastStartDate.ToShortDateString()}";
                wks.Cells[cell].Style.Font.Bold = true;
            }
            cell.ColumnNumber = 1;
            cell.Row++;
            return cell;
        }

        private CellAddress AddMeritBadge(ExcelWorksheet wks, CellAddress cell, List<MeritBadge> badges, string meritBadge)
        {
            //cell.ColumnNumber++;
            var selected = badges.Where(b => b.Name == meritBadge);
            wks.Cells[cell].Value = $"{meritBadge}:";
            wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            cell.ColumnNumber++;
            if (selected.Any())
            {
                wks.Cells[cell].Value = $"Earned {selected.First().DateEarned.Value.ToShortDateString()}";
            }
            else
            {
                wks.Cells[cell].Value = "required";
                wks.Cells[cell].Style.Font.Bold = true;
            }
            cell.ColumnNumber = 1;
            cell.Row++;
            return cell;
        }

        private CellAddress AddMeritBadge(ExcelWorksheet wks, CellAddress cell, List<MeritBadge> badges, List<string> meritBadges)
        {
            //cell.ColumnNumber++;
            var selected = badges.Where(b => meritBadges.Contains(b.Name)).OrderBy(b => b.DateEarned);
            wks.Cells[cell].Value = $"{string.Join(" / ", meritBadges)}:";
            wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            cell.ColumnNumber++;
            if (selected.Any())
            {
                var earned = selected.OrderBy(b => b.DateEarned).ThenBy(b => b.BsaId).First();
                wks.Cells[cell].Value = $"Earned {earned.Name} on {earned.DateEarned.Value.ToShortDateString()}";
            }
            else
            {
                wks.Cells[cell].Value = "required";
                wks.Cells[cell].Style.Font.Bold = true;
            }
            cell.ColumnNumber = 1;
            cell.Row++;
            return cell;
        }

        private CellAddress WarnAboutLongRequirement(ExcelWorksheet wks, CellAddress cell, List<MeritBadge> badges, string meritBadge, DateTime lastStartDate)
        {
            var selected = badges.Where(b => b.Name == meritBadge);
            if (!selected.Any())
            {
                wks.Cells[cell].Value = $"The last possible day to begin the {meritBadge} merit badge is:";
                wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                cell.ColumnNumber++;
                wks.Cells[cell].Value = $"{lastStartDate.ToShortDateString()}";
                wks.Cells[cell].Style.Font.Bold = true;
                cell.ColumnNumber = 1;
                cell.Row++;
            }
            return cell;
        }

        public void Run(string outputFileName)
        {
            if (File.Exists(outputFileName))
                File.Delete(outputFileName);


            List<TroopMember> scouts = this.Scouts.Where(m => m.FirstClass.Earned).ToList();
            if (!scouts.Any())
            {
                return;
            }

            scouts = scouts.OrderBy(m => m.DateOfBirth).ToList();

            var fi = new FileInfo(outputFileName);
            using (var package = new ExcelPackage(fi))
            {
                foreach (var scout in scouts)
                {
                    ExcelWorksheet wks = package.Workbook.Worksheets.Add($"{scout.FirstName} {scout.LastName.First()}.");
                    CellAddress cell = new CellAddress("A1");
                    wks.Cells[cell].Value = "Trail to Eagle Progress Report for:";
                    wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    cell.ColumnNumber++;
                    wks.Cells[cell].Value = scout.DisplayName;
                    cell.ColumnNumber = 1;
                    cell.Row++;
                    wks.Cells[cell].Value = "As of:";
                    wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    cell.ColumnNumber++;
                    wks.Cells[cell].Value = DateTime.Now.ToLongDateString();
                    cell.ColumnNumber = 1;
                    cell.Row++;
                    cell.Row++;
                    wks.Cells[cell].Value = "All Eagle requirements, except Board of Review, must be completed by:";
                    wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    cell.ColumnNumber++;
                    DateTime doneBy = scout.DateOfBirth.AddYears(18).AddDays(-1);
                    wks.Cells[cell].Value = $"{doneBy.ToShortDateString()}";
                    wks.Cells[cell].Style.Font.Bold = true;
                    cell.ColumnNumber = 1;
                    cell.Row++;
                    cell.Row++;

                    cell = this.AddRank(wks, cell, scout.FirstClass, doneBy.AddMonths(-(4 + 6 + 6)));
                    cell = this.AddRank(wks, cell, scout.Star, doneBy.AddMonths(-(6 + 6)));
                    cell = this.AddRank(wks, cell, scout.Life, doneBy.AddMonths(-6));

                    cell.Row++;
                    cell.Row++;

                    EagleMeritBadgeRequirement req = scout.Eagle.Requirements.Where(r => r is EagleMeritBadgeRequirement).First() as EagleMeritBadgeRequirement;
                    var required = req.MeritBadges.Where(mb => mb.EagleRequired).OrderBy(mb => mb.DateEarned).ThenBy(mb => mb.BsaId).ToList();
                    var elective = req.MeritBadges.Where(mb => !mb.EagleRequired).OrderBy(mb => mb.DateEarned).ThenBy(mb => mb.BsaId).ToList();

                    int remaining = req.Elective - elective.Count();

                    cell = this.AddMeritBadge(wks, cell, required, "First Aid");
                    cell = this.AddMeritBadge(wks, cell, required, "Citizenship in the Community");
                    cell = this.AddMeritBadge(wks, cell, required, "Citizenship in the Nation");
                    cell = this.AddMeritBadge(wks, cell, required, "Citizenship in the World");
                    cell = this.AddMeritBadge(wks, cell, required, "Communication");
                    cell = this.AddMeritBadge(wks, cell, required, "Cooking");
                    cell = this.AddMeritBadge(wks, cell, required, "Personal Fitness");
                    cell = this.AddMeritBadge(wks, cell, required, new List<string>() { "Emergency Preparedness", "Lifesaving" });
                    cell = this.AddMeritBadge(wks, cell, required, new List<string>() { "Environmental Science", "Sustainability" });
                    cell = this.AddMeritBadge(wks, cell, required, "Personal Management");
                    cell = this.AddMeritBadge(wks, cell, required, new List<string>() { "Swimming", "Hiking", "Cycling" });
                    cell = this.AddMeritBadge(wks, cell, required, "Camping");
                    cell = this.AddMeritBadge(wks, cell, required, "Family Life");
                    cell.Row++;
                    cell = this.WarnAboutLongRequirement(wks, cell, required, "Personal Fitness", doneBy.AddDays(-12 * 7));
                    cell = this.WarnAboutLongRequirement(wks, cell, required, "Personal Management", doneBy.AddDays(-13 * 7));
                    cell = this.WarnAboutLongRequirement(wks, cell, required, "Family Life", doneBy.AddDays(-90));
                    cell.Row++;
                    wks.Cells[cell].Value = "Elective merit badges earned:";
                    wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    cell.ColumnNumber++;
                    wks.Cells[cell].Value = $"{elective.Count()} of {req.Elective}";
                    cell.ColumnNumber = 1;
                    if (remaining > 0)
                    {
                        cell.Row++;
                        wks.Cells[cell].Value = $"The {remaining} remaining elective badge{((remaining != 1) ? "s" : "")} must be earned by:";
                        wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        cell.ColumnNumber++;
                        wks.Cells[cell].Value = $"{doneBy.ToShortDateString()}";
                        wks.Cells[cell].Style.Font.Bold = true;
                        cell.ColumnNumber = 1;
                    }
                    wks.Calculate();
                    wks.Cells[wks.Dimension.Address].AutoFitColumns(10);
                }
                package.Save();
            }
        }
    }
}
