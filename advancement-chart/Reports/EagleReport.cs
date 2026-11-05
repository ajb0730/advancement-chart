using System.Collections.Generic;
using advancementchart.Model;
using System.IO;
using OfficeOpenXml;
using System.Linq;
using System;


namespace advancementchart.Reports
{
    /// <summary>
    /// Trail to Eagle workbook: a sheet per active scout who is First Class or above but not yet Eagle, showing the merit badges still needed and the latest dates they can be started (from the scout's date of birth).
    /// </summary>
    public class EagleReport : IReport
    {
        /// <summary>
        /// Creates the report for the given scouts.
        /// </summary>
        public EagleReport(List<TroopMember> scouts)
        {
            Scouts = scouts;
        }

        /// <summary>
        /// The scouts the report is generated from.
        /// </summary>
        public List<TroopMember> Scouts { get; set; }

        private static bool HasDateOfBirth(TroopMember scout)
        {
            return scout.DateOfBirth != default(DateTime);
        }

        /// <summary>
        /// Writes a rank's Board of Review status. With no <paramref name="lastStartDate"/> (the
        /// scout's DOB is unknown) only the earned date, or "not yet earned", is shown.
        /// </summary>
        private CellAddress AddRank(ExcelWorksheet wks, CellAddress cell, Rank rank, DateTime? lastStartDate)
        {
            wks.Cells[cell].Value = $"{rank.Name} Board of Review:";
            wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            cell.ColumnNumber++;
            if (rank.Earned && (lastStartDate == null || rank.DateEarned <= lastStartDate))
            {
                wks.Cells[cell].Value = $"{rank.DateEarned.Value.ToShortDateString()}";
            }
            else if (lastStartDate == null)
            {
                wks.Cells[cell].Value = "not yet earned";
                wks.Cells[cell].Style.Font.Bold = true;
            }
            else
            {
                wks.Cells[cell].Value = $"on or before {lastStartDate.Value.ToShortDateString()}";
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
                if (selected.First().Earned)
                {
                    wks.Cells[cell].Value = $"Earned {selected.First().DateEarned.Value.ToShortDateString()}";
                }
                else
                {
                    wks.Cells[cell].Value = $"Started";
                }
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
                var earnedList = selected.Where(b => b.Earned).OrderBy(b => b.DateEarned).ThenBy(b => b.BsaId);
                if (earnedList.Any())
                {
                    var earned = earnedList.First();
                    wks.Cells[cell].Value = $"Earned {earned.Name} on {earned.DateEarned.Value.ToShortDateString()}";
                }
                else
                {
                    wks.Cells[cell].Value = $"Started {String.Join(", ", selected.OrderBy(b => b.BsaId).Select(b => b.Name))}";
                }
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

        /// <summary>
        /// Writes the workbook to <paramref name="outputFileName"/>, replacing any existing file. Writes nothing if no scout qualifies. Scouts without a known date of birth are listed last, without deadlines.
        /// </summary>
        public void Run(string outputFileName)
        {
            Console.WriteLine("Running Eagle Report");

            if (File.Exists(outputFileName))
                File.Delete(outputFileName);


            List<TroopMember> scouts = this.Scouts.Where(m => m.FirstClass.Earned && !m.Eagle.Earned && !string.Equals(m.Patrol, "Inactive", StringComparison.OrdinalIgnoreCase)).ToList();
            if (!scouts.Any())
            {
                return;
            }

            // Scouts with no known DOB sort last; they cannot be placed on the age timeline.
            scouts = scouts
                .OrderBy(m => HasDateOfBirth(m) ? m.DateOfBirth : DateTime.MaxValue)
                .ThenBy(m => m.LastName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(m => m.FirstName, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var fi = new FileInfo(outputFileName);
            using (var package = new ExcelPackage(fi))
            {
                foreach (var scout in scouts)
                {
                    ExcelWorksheet wks = WorksheetNames.AddUnique(package.Workbook, WorksheetNames.Abbreviated(scout.FirstName, scout.LastName));
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
                    bool hasDob = HasDateOfBirth(scout);
                    DateTime doneBy = default(DateTime);
                    if (hasDob)
                    {
                        wks.Cells[cell].Value = "All Eagle requirements, except Board of Review, must be completed by:";
                        wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        cell.ColumnNumber++;
                        doneBy = scout.DateOfBirth.AddYears(18).AddDays(-1);
                        Console.WriteLine($"{scout.DisplayName} = {scout.DateOfBirth.ToShortDateString()} {doneBy.ToShortDateString()}");
                        wks.Cells[cell].Value = $"{doneBy.ToShortDateString()}";
                        wks.Cells[cell].Style.Font.Bold = true;
                    }
                    else
                    {
                        Console.Error.WriteLine($"WARNING: No date of birth for {scout.DisplayName}; Eagle deadlines not calculated");
                        wks.Cells[cell].Value = "Date of birth unknown - no deadlines shown";
                        wks.Cells[cell].Style.Font.Bold = true;
                    }
                    cell.ColumnNumber = 1;
                    cell.Row++;
                    cell.Row++;

                    cell = this.AddRank(wks, cell, scout.FirstClass, hasDob ? doneBy.AddMonths(-(ScoutingConstants.StarActiveMonths + ScoutingConstants.LifeActiveMonths + ScoutingConstants.EagleActiveMonths)) : (DateTime?)null);
                    cell = this.AddRank(wks, cell, scout.Star, hasDob ? doneBy.AddMonths(-(ScoutingConstants.LifeActiveMonths + ScoutingConstants.EagleActiveMonths)) : (DateTime?)null);
                    cell = this.AddRank(wks, cell, scout.Life, hasDob ? doneBy.AddMonths(-ScoutingConstants.EagleActiveMonths) : (DateTime?)null);

                    cell.Row++;
                    cell.Row++;

                    EagleMeritBadgeRequirement req = scout.Eagle.Requirements.Where(r => r is EagleMeritBadgeRequirement).First() as EagleMeritBadgeRequirement;
                    var required = req.MeritBadges.Where(mb => mb.EagleRequired).OrderBy(mb => mb.DateEarned).ThenBy(mb => mb.BsaId).ToList();
                    // Electives are the non-Eagle-required badges plus any extra earned badges from an
                    // either-or group (e.g. Lifesaving after Emergency Preparedness).
                    var extras = req.SplitEarned().Elective.Where(mb => mb.EagleRequired);
                    var elective = req.MeritBadges.Where(mb => !mb.EagleRequired).Concat(extras).OrderBy(mb => mb.DateEarned).ThenBy(mb => mb.BsaId).ToList();

                    int remaining = req.Elective - elective.Count();

                    // Same columns, in the same order, as the troop advancement chart; this includes
                    // Citizenship in Society only while it is Eagle-required (it is not now).
                    foreach (var column in TroopReport.EagleColumnsFor(MeritBadge.GetEagleRequired()))
                    {
                        cell = column.Length == 1
                            ? this.AddMeritBadge(wks, cell, required, column[0])
                            : this.AddMeritBadge(wks, cell, required, column.ToList());
                    }
                    cell.Row++;
                    if (hasDob)
                    {
                        cell = this.WarnAboutLongRequirement(wks, cell, required, MeritBadge.PersonalFitness, doneBy.AddDays(-ScoutingConstants.PersonalFitnessDays));
                        cell = this.WarnAboutLongRequirement(wks, cell, required, MeritBadge.PersonalManagement, doneBy.AddDays(-ScoutingConstants.PersonalManagementDays));
                        cell = this.WarnAboutLongRequirement(wks, cell, required, MeritBadge.FamilyLife, doneBy.AddDays(-ScoutingConstants.FamilyLifeDays));
                    }
                    cell.Row++;
                    wks.Cells[cell].Value = "Elective merit badges earned:";
                    wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    cell.ColumnNumber++;
                    wks.Cells[cell].Value = $"{elective.Count()} of {req.Elective}";
                    cell.ColumnNumber = 1;
                    if (remaining > 0)
                    {
                        cell.Row++;
                        wks.Cells[cell].Value = hasDob
                            ? $"The {remaining} remaining elective badge{((remaining != 1) ? "s" : "")} must be earned by:"
                            : $"The {remaining} remaining elective badge{((remaining != 1) ? "s" : "")} still need to be earned.";
                        wks.Cells[cell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        if (hasDob)
                        {
                            cell.ColumnNumber++;
                            wks.Cells[cell].Value = $"{doneBy.ToShortDateString()}";
                            wks.Cells[cell].Style.Font.Bold = true;
                        }
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
