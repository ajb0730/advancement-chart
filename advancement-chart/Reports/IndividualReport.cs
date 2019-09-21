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
            if (File.Exists(outputFileName))
                File.Delete(outputFileName);
            var fi = new FileInfo(outputFileName);
            using (var package = new ExcelPackage(fi))
            {
                var wks = package.Workbook.Worksheets.Add("Troop Summary");
                CellAddress cell = new CellAddress("A1");

                foreach (var scout in Scouts)
                {
                    if (scout.FirstClass.Earned)
                    {

                    }
                    else
                    {
                        RunScoutReport(scout, package);
                    }
                }

                List<CurriculumGroup> groups = new List<CurriculumGroup>();
                groups.AddRange(numberIncomplete.Keys);
                groups.AddRange(numberNotStarted.Keys);

                foreach(CurriculumGroup key in groups
                    .Distinct()
                    .OrderByDescending(k => (numberIncomplete.ContainsKey(k) ? numberIncomplete[k].Count : 0) + (numberNotStarted.ContainsKey(k) ? numberNotStarted[k].Count : 0))
                    .ThenByDescending(k => (numberIncomplete.ContainsKey(k) ? numberIncomplete[k].Count : 0))
                    .ThenBy(k => k.GetDisplayName())
                    )
                {
                    cell.Column = "A";
                    wks.Cells[cell].Value = key.GetDisplayName();
                    cell.Row += 1;
                    cell.ColumnNumber += 1;
                    if (numberIncomplete.ContainsKey(key))
                    {
                        foreach (TroopMember scout in numberIncomplete[key].OrderBy(s => s.LastName).ThenBy(s => s.FirstName))
                        {
                            wks.Cells[cell].Value = $"{scout.FirstName} {scout.LastName.First()}*";
                            cell.Row += 1;
                        }
                    }
                    if(numberNotStarted.ContainsKey(key))
                    {
                        foreach (TroopMember scout in numberNotStarted[key].OrderBy(s => s.LastName).ThenBy(s => s.FirstName))
                        {
                            wks.Cells[cell].Value = $"{scout.FirstName} {scout.LastName.First()}";
                            cell.Row += 1;
                        }
                    }
                    cell.Row += 1;
                }

                wks.Calculate();
                wks.Cells[wks.Dimension.Address].AutoFitColumns();

                package.Save();
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
            var wks = package.Workbook.Worksheets.Add($"{scout.FirstName} {scout.LastName.First()}");
            CellAddress cell = new CellAddress("A1");
            wks.Cells[cell].Value = $"{scout.FirstName} {scout.LastName}";

            Dictionary<CurriculumGroup, List<RankRequirement>> groups = scout.GetRequirementsByGroup();
            foreach(CurriculumGroup key in groups.Keys
                //.Where(k => PercentComplete(groups[k]) < 1m)
                .OrderByDescending(k => PercentComplete(groups[k]))
                .ThenByDescending(k => groups[k].Count)
                .ThenBy(k => k.GetDisplayName())
                )
            {
                decimal percentComplete = PercentComplete(groups[key]);
                if(percentComplete == 1m)
                {
                    if(!numberComplete.ContainsKey(key))
                    {
                        numberComplete.Add(key, new List<TroopMember>());
                    }
                    numberComplete[key].Add(scout);

                    continue;
                } 
                else if(percentComplete == 0m)
                {
                    if(!numberNotStarted.ContainsKey(key))
                    {
                        numberNotStarted.Add(key, new List<TroopMember>());
                    }
                    numberNotStarted[key].Add(scout);
                }
                else
                {
                    if(!numberIncomplete.ContainsKey(key))
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
                foreach(var req in groups[key])
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
            wks.Cells[wks.Dimension.Address].AutoFitColumns();
        }
    }
}
