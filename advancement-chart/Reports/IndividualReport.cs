using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using advancementchart.Model;
using OfficeOpenXml;

namespace advancementchart.Reports
{
    public class IndividualReport : IReport
    {
        public IndividualReport(List<TroopMember> scouts)
        {
            Scouts = scouts;
        }

        public List<TroopMember> Scouts { get; set; }

        public void Run(string outputFileName)
        {
            Console.WriteLine("Running Individual Report");

            if (File.Exists(outputFileName))
                File.Delete(outputFileName);
            var fi = new FileInfo(outputFileName);
            using (var package = new ExcelPackage(fi))
            {
                ExcelWorksheet wks = package.Workbook.Worksheets.Add("Troop Summary");
                Dictionary<string, ExcelWorksheet> patrolSheets = new Dictionary<string, ExcelWorksheet>();

                // Figure out Patrols
                Dictionary<string, List<TroopMember>> patrols = Scouts.Where(s => !string.Equals(s.Patrol, "Inactive", StringComparison.OrdinalIgnoreCase)).GroupBy(s => s.Patrol).ToDictionary(g => g.Key, g => g.ToList());
                if (patrols.Keys.Count > 1)
                {
                    foreach (string patrolName in patrols.Keys.OrderBy(s => s))
                    {
                        patrolSheets.Add(patrolName, package.Workbook.Worksheets.Add(patrolName));
                    }
                }

                // Collect data (and also add individual Scout sheets)
                foreach (var scout in Scouts.Where(s => !string.Equals(s.Patrol, "Inactive", StringComparison.OrdinalIgnoreCase)))
                {
                    // Skip Scouts that have already earned First Class
                    if (!scout.FirstClass.Earned)
                    {
                        Console.WriteLine("    Running Scout Report for {0}", scout.DisplayName);
                        RunScoutReport(scout, package);
                    }
                }

                Console.WriteLine("    Writing Group Worksheet");
                WriteGroupWorksheet(numberIncomplete, numberNotStarted, wks);

                // Output Patrol Summaries
                if (patrols.Keys.Count > 1)
                {
                    foreach (string patrolName in patrols.Keys)
                    {
                        List<TroopMember> members = patrols[patrolName];
                        Console.Error.WriteLine($"{patrolName} has {members.Count} member(s).");
                        if (members.Count > 0)
                        {
                            bool flag = false;
                            Dictionary<CurriculumGroup, List<TroopMember>> patrolIncomplete = new Dictionary<CurriculumGroup, List<TroopMember>>();
                            foreach (CurriculumGroup group in numberIncomplete.Keys)
                            {
                                if (numberIncomplete[group].Intersect(members).Any())
                                {
                                    patrolIncomplete.Add(group, numberIncomplete[group].Intersect(members).ToList());
                                    flag = true;
                                }
                            }
                            Dictionary<CurriculumGroup, List<TroopMember>> patrolNotStarted = new Dictionary<CurriculumGroup, List<TroopMember>>();
                            foreach (CurriculumGroup group in numberNotStarted.Keys)
                            {
                                if (numberNotStarted[group].Intersect(members).Any())
                                {
                                    patrolNotStarted.Add(group, numberNotStarted[group].Intersect(members).ToList());
                                    flag = true;
                                }
                            }
                            if (flag)
                            {
                                WriteGroupWorksheet(patrolIncomplete, patrolNotStarted, patrolSheets[patrolName]);
                            }
                        }
                    }
                }

                package.Save();
            }
        }

        private void WriteGroupWorksheet(
            Dictionary<CurriculumGroup, List<TroopMember>> incomplete,
            Dictionary<CurriculumGroup, List<TroopMember>> notStarted,
            ExcelWorksheet excelWorksheet)
        {
            List<CurriculumGroup> groups = new List<CurriculumGroup>();
            groups.AddRange(incomplete.Keys);
            groups.AddRange(notStarted.Keys);

            // Output Troop Summary
            CellAddress cell = new CellAddress("A1");
            foreach (CurriculumGroup key in groups
                .Distinct()
                .OrderByDescending(k => (incomplete.ContainsKey(k) ? incomplete[k].Count : 0) + (notStarted.ContainsKey(k) ? notStarted[k].Count : 0))
                .ThenByDescending(k => (incomplete.ContainsKey(k) ? incomplete[k].Count : 0))
                .ThenBy(k => k.GetDisplayName())
                )
            {
                cell.Column = "A";
                excelWorksheet.Cells[cell].Value = key.GetDisplayName();
                cell.Row += 1;
                cell.ColumnNumber += 1;
                if (incomplete.ContainsKey(key))
                {
                    foreach (TroopMember scout in incomplete[key].OrderBy(s => s.LastName).ThenBy(s => s.FirstName))
                    {
                        excelWorksheet.Cells[cell].Value = $"{(string.IsNullOrWhiteSpace(scout.NickName) ? scout.FirstName : scout.NickName)} {scout.LastName.First()}*";
                        cell.Row += 1;
                    }
                }
                if (notStarted.ContainsKey(key))
                {
                    foreach (TroopMember scout in notStarted[key].OrderBy(s => s.LastName).ThenBy(s => s.FirstName))
                    {
                        excelWorksheet.Cells[cell].Value = $"{(string.IsNullOrWhiteSpace(scout.NickName) ? scout.FirstName : scout.NickName)} {scout.LastName.First()}";
                        cell.Row += 1;
                    }
                }
                cell.Row += 1;
            }
            excelWorksheet.Calculate();
            try
            {
                excelWorksheet.Cells[excelWorksheet.Dimension.Address].AutoFitColumns(10);
            }
            catch (NullReferenceException)
            {
                Console.Error.WriteLine($"{excelWorksheet.Name} failed to fit columns");
            }
        }

        private decimal PercentComplete(List<RankRequirement> requirements)
        {
            decimal total = requirements.Count;
            decimal complete = requirements.Count(r => r.Earned);
            return complete / total;
        }

        private readonly Dictionary<CurriculumGroup, List<TroopMember>> numberComplete = new Dictionary<CurriculumGroup, List<TroopMember>>();
        private readonly Dictionary<CurriculumGroup, List<TroopMember>> numberIncomplete = new Dictionary<CurriculumGroup, List<TroopMember>>();
        private readonly Dictionary<CurriculumGroup, List<TroopMember>> numberNotStarted = new Dictionary<CurriculumGroup, List<TroopMember>>();

        private void RunScoutReport(TroopMember scout, ExcelPackage package)
        {
            var wks = package.Workbook.Worksheets.Add($"{(string.IsNullOrWhiteSpace(scout.NickName) ? scout.FirstName : scout.NickName)} {scout.LastName.First()}");
            CellAddress cell = new CellAddress("A1");
            wks.Cells[cell].Value = scout.DisplayName;

            Dictionary<CurriculumGroup, List<RankRequirement>> groups = scout.GetRequirementsByGroup();
            foreach (CurriculumGroup key in groups.Keys
                //.Where(k => PercentComplete(groups[k]) < 1m)
                .OrderByDescending(k => PercentComplete(groups[k]))
                .ThenByDescending(k => groups[k].Count)
                .ThenBy(k => k.GetDisplayName())
                )
            {
                decimal percentComplete = PercentComplete(groups[key]);
                if (percentComplete == 1m)
                {
                    if (!numberComplete.ContainsKey(key))
                    {
                        numberComplete.Add(key, new List<TroopMember>());
                    }
                    numberComplete[key].Add(scout);

                    continue;
                }
                else if (percentComplete == 0m)
                {
                    if (!numberNotStarted.ContainsKey(key))
                    {
                        numberNotStarted.Add(key, new List<TroopMember>());
                    }
                    numberNotStarted[key].Add(scout);
                }
                else
                {
                    if (!numberIncomplete.ContainsKey(key))
                    {
                        numberIncomplete.Add(key, new List<TroopMember>());
                    }
                    numberIncomplete[key].Add(scout);
                }

                cell.Row += 1;
                cell.Column = "A";
                wks.Cells[cell].Value = key.GetDisplayName();
                cell.ColumnNumber += 1;
                wks.Cells[cell].Value = $"{percentComplete:p1}";

                string rank = string.Empty;
                foreach (var req in groups[key])
                {
                    cell.Row += 1;
                    cell.Column = "B";

                    wks.Cells[$"${cell.Row}:${cell.Row}"].Style.Font.Bold |= !req.Earned;

                    if (req.Rank.Name != rank)
                    {
                        wks.Cells[cell].Value = req.Rank.Name;
                    }
                    cell.ColumnNumber += 1;
                    wks.Cells[cell].Value = req.Name;
                    cell.ColumnNumber += 1;
                    if (req.Earned)
                    {
                        wks.Cells[cell].Value = "(earned)";
                    }
                    else
                    {
                        wks.Cells[cell].Value = req.HandbookPages ?? string.Empty;
                    }
                    cell.ColumnNumber += 1;
                    wks.Cells[cell].Value = req.Description;
                }
                cell.Row += 1;
            }

            wks.Calculate();
            wks.Cells[wks.Dimension.Address].AutoFitColumns(10);
        }
    }
}
