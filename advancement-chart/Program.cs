using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using advancementchart.Model;
using advancementchart.Reports;
using CsvHelper;
using System.Globalization;
using advancementchart.Model.Ranks;

[assembly: InternalsVisibleTo("advancement-chart.tests")]

namespace advancement_chart
{
    class Program
    {
        static void Main(string[] args)
        {
            var scouts = new List<TroopMember>();
            DateTime maxDate = DateTime.MinValue;
            foreach (string arg in args)
            {
                var fileMaxDate = LoadFile(arg, scouts);
                maxDate = fileMaxDate > maxDate ? fileMaxDate : maxDate;
            }
            //
            // Download the "Scout" backup report from Scoutbook and rename
            // the file "scouts.csv"
            //
            var patrolFile = "./scouts.csv";
            if (!File.Exists(patrolFile))
            {
                Console.Error.WriteLine($"WARNING: Patrol lookup file '{patrolFile}' not found.");
                Console.Error.WriteLine("Scouts will not have patrol assignments, nicknames, or dates of birth.");
                Console.Error.WriteLine("Eagle Report deadline calculations will be incorrect.");
            }
            LoadPatrolLookup(patrolFile, scouts);
            {
                var report = new TroopReport(scouts);
                report.Run(@"./TroopAdvancementChart.xlsx");
            }
            {
                var report = new IndividualReport(scouts);
                report.Run(@"./IndividualReport.xlsx");
            }
            {
                var report = new EagleReport(scouts);
                report.Run(@"./EagleReport.xlsx");
            }
            {
                var report = new TroopGuideReport(scouts);
                report.Run(@"./TroopGuideReport.xlsx");
            }
            {
                var report = new TroopCheckList(scouts);
                report.Run(@"./TroopCheckList.xlsx");
            }
            {
                var report = new AdvancementCheck(scouts);
                report.Run(@"");
            }
            {
                var report = new AdvancementReport(scouts, maxDate);
                report.Run(@"./AdvancementReport.docx");
            }
        }

        internal static void LoadPatrolLookup(string fileName, List<TroopMember> scouts)
        {
            if (File.Exists(fileName))
            {
                using (TextReader txtReader = new StreamReader(fileName))
                using (var csvReader = new CsvReader(txtReader, CultureInfo.CurrentCulture))
                {
                    csvReader.Read();
                    csvReader.ReadHeader();

                    int memberIdIndex = csvReader.GetFieldIndex(name: "BSA Member ID");
                    int nicknameIndex = csvReader.GetFieldIndex(name: "Nickname");
                    int patrolNameIndex = csvReader.GetFieldIndex(name: "Patrol Name");
                    int dobIndex = csvReader.GetFieldIndex(name: "DOB");

                    while (csvReader.Read())
                    {
                        string id = csvReader.GetField(memberIdIndex);
                        TroopMember scout = scouts.FirstOrDefault(tm => tm.BsaMemberId == id);
                        if (scout != null)
                        {
                            string nickname = csvReader.GetField(nicknameIndex);
                            if (!string.IsNullOrWhiteSpace(nickname))
                            {
                                scout.NickName = nickname;
                            }
                            string patrol = csvReader.GetField(patrolNameIndex);
                            if (!string.IsNullOrWhiteSpace(patrol))
                            {
                                scout.Patrol = patrol;
                            }
                            string dob = csvReader.GetField(dobIndex);
                            if (!string.IsNullOrWhiteSpace(dob))
                            {
                                DateTime dobDate;
                                if (DateTime.TryParse(dob, out dobDate))
                                {
                                    if (dobDate.Year < 1990 || dobDate > DateTime.Now)
                                    {
                                        Console.Error.WriteLine($"WARNING: Suspicious DOB '{dob}' for {scout.DisplayName}");
                                    }
                                    scout.DateOfBirth = dobDate;
                                }
                            }
                        }
                    }
                }
            }
        }

        internal static DateTime LoadFile(string fileName, List<TroopMember> scouts)
        {
            DateTime result = DateTime.MinValue;
            var warnings = new List<string>();

            if (File.Exists(fileName))
            {
                Console.WriteLine($"Reading data from {fileName}.");
                using (TextReader txtReader = new StreamReader(fileName))
                using (var csvReader = new CsvReader(txtReader, CultureInfo.CurrentCulture))
                {
                    csvReader.Read();
                    csvReader.ReadHeader();

                    while (csvReader.Read())
                    {
                        string id = csvReader.GetField("BSA Member ID");
                        TroopMember scout = scouts.FirstOrDefault(tm => tm.BsaMemberId == id);
                        if (null == scout)
                        {
                            string firstName = csvReader.GetField("First Name");
                            string middleName = csvReader.GetField("Middle Name");
                            string lastName = csvReader.GetField("Last Name");
                            scout = new TroopMember(memberId: id, firstName: firstName, middleName: middleName, lastName: lastName);
                            scouts.Add(scout);
                            Console.WriteLine($"Adding record for {firstName} {lastName}.");
                        }

                        string type = csvReader.GetField("Advancement Type")?.Trim();
                        string subtype = csvReader.GetField("Advancement")?.Trim();
                        string version = csvReader.GetField("Version")?.Trim();
                        // Rows without a parseable completion date carry no advancement signal we
                        // can use (and would otherwise throw); skip them rather than crash the load.
                        if (!csvReader.TryGetField<DateTime>("Date Completed", out DateTime date))
                        {
                            continue;
                        }
                        result = date > result ? date : result;
                        try
                        {
                            if (type == "Rank")
                            {
                                // New exports suffix the rank name with " Rank" (e.g. "Scout Rank");
                                // older exports used the bare name ("Scout"). Cub ranks (Bobcat,
                                // Tiger, ...) resolve to null and are ignored.
                                Rank rank = GetRankByName(scout, StripSuffix(subtype, " Rank"));
                                if (rank != null)
                                {
                                    rank.DateEarned = date;
                                }
                            }
                            else if (type == "Merit Badge" || type == "Merit Badges")
                            {
                                // New exports suffix the badge name with " MB" and replace commas in
                                // the name with semicolons (e.g. "Signs; Signals; and Codes MB").
                                var badgeName = StripSuffix(subtype, " MB").Replace("; ", ", ");
                                MeritBadge badge = new MeritBadge(name: badgeName, description: version, earned: date);
                                scout.Add(badge);
                            }
                            else if (type == "Award" || type == "Awards")
                            {
                                switch (subtype)
                                {
                                    case "Eagle Palm Pin #1 (Bronze)":
                                    case "Eagle Palm Pin #4 (Bronze)":
                                        scout.EaglePalms.Add(new Palm(Palm.PalmType.Bronze, date));
                                        break;
                                    case "Eagle Palm Pin #2 (Gold)":
                                    case "Eagle Palm Pin #5 (Gold)":
                                        scout.EaglePalms.Add(new Palm(Palm.PalmType.Gold, date));
                                        break;
                                    case "Eagle Palm Pin #3 (Silver)":
                                    case "Eagle Palm Pin #6 (Silver)":
                                        scout.EaglePalms.Add(new Palm(Palm.PalmType.Silver, date));
                                        break;
                                }
                            }
                            else if (type.EndsWith("Rank Requirement") || type.EndsWith("Rank Requirements"))
                            {
                                // New exports move the rank name into the type column and pluralize
                                // the suffix, e.g. "First Class Rank Requirements" with the
                                // requirement number ("1a") in the Advancement column. Legacy exports
                                // used the singular "First Class Rank Requirement". Cub-rank
                                // requirement rows resolve to a null rank and are ignored.
                                var rankName = StripSuffix(StripSuffix(type, "Requirements"), "Requirement");
                                rankName = StripSuffix(rankName, "Rank");
                                Rank rank = GetRankByName(scout, rankName);
                                if (rank != null && !string.IsNullOrWhiteSpace(subtype)
                                    && rank.Requirements.Any(req => req.Name == subtype))
                                {
                                    rank.Requirements.First(req => req.Name == subtype).DateEarned = date;
                                }
                            }
                            else if (type == "Merit Badge Requirement")
                            {
                                // Legacy format: badge name and requirement number share the
                                // Advancement column, e.g. "Camping #1a".
                                var badgeName = subtype.Substring(0, subtype.IndexOf("#") - 1).Trim();
                                scout.AddPartial(badgeName, version);
                            }
                            else if (type.EndsWith("Merit Badge Requirements"))
                            {
                                // New format: the badge name lives in the type column and keeps its
                                // real commas ("Signs, Signals, and Codes Merit Badge Requirements"),
                                // while the requirement number is in the Advancement column. We only
                                // need the badge name to mark the badge as started.
                                var badgeName = StripSuffix(type, "Merit Badge Requirements");
                                scout.AddPartial(badgeName, version);
                            }
                            else if (type == "Award Requirement")
                            {
                                // Legacy palm-requirement format: "Eagle Palm Pin #1 (Bronze) #1".
                                var palmName = subtype.Substring(0, subtype.LastIndexOf("#") - 1).Trim();
                                var requirementNumber = subtype.Substring(subtype.LastIndexOf("#") + 1).Trim().TrimEnd('.');
                                // Console.WriteLine($"Found '{palmName}' and '{requirementNumber}' in '{subtype}'");
                                if (!string.IsNullOrWhiteSpace(palmName) && !string.IsNullOrWhiteSpace(requirementNumber))
                                {
                                    Palm palm = null;
                                    switch (palmName)
                                    {
                                        case "Eagle Palm Pin #1 (Bronze)":
                                            palm = scout.GetNthPalm(Palm.PalmType.Bronze, 1);
                                            break;
                                        case "Eagle Palm Pin #2 (Gold)":
                                            palm = scout.GetNthPalm(Palm.PalmType.Gold, 1);
                                            break;
                                        case "Eagle Palm Pin #3 (Silver)":
                                            palm = scout.GetNthPalm(Palm.PalmType.Silver, 1);
                                            break;
                                        case "Eagle Palm Pin #4 (Bronze)":
                                            palm = scout.GetNthPalm(Palm.PalmType.Bronze, 2);
                                            break;
                                        case "Eagle Palm Pin #5 (Gold)":
                                            palm = scout.GetNthPalm(Palm.PalmType.Gold, 2);
                                            break;
                                        case "Eagle Palm Pin #6 (Silver)":
                                            palm = scout.GetNthPalm(Palm.PalmType.Silver, 2);
                                            break;
                                    }
                                    if (null != palm)
                                    {
                                        var req = palm.Requirements.FirstOrDefault(x => x.Name == requirementNumber);
                                        if (req != null)
                                        {
                                            req.DateEarned = date;
                                        }
                                        else
                                        {
                                            warnings.Add($"Unknown requirement '{requirementNumber}' for '{palmName}'");
                                        }
                                    }
                                }
                            }
                            // New-format "<Award> Award Requirements" rows carry the requirement as
                            // descriptive text rather than a number, and palms are only tracked once
                            // earned (via "Awards" rows); there is nothing to attach, so those rows
                            // are intentionally ignored.
                        }
                        catch (FormatException e)
                        {
                            warnings.Add($"Skipped row: type={type} subtype={subtype} version={version} date={date.ToShortDateString()} - {e.Message}");
                        }
                        catch (ArgumentException e)
                        {
                            warnings.Add($"Invalid data: type={type} subtype={subtype} version={version} date={date.ToShortDateString()} - {e.Message}");
                        }
                    }
                }
            }

            if (warnings.Count > 0)
            {
                Console.Error.WriteLine($"Warning: {warnings.Count} row(s) skipped while loading {fileName}:");
                foreach (var warning in warnings)
                {
                    Console.Error.WriteLine($"  {warning}");
                }
            }

            return result;
        }

        /// <summary>
        /// Removes a trailing token from a value (case-sensitive) and trims trailing
        /// whitespace, returning the value unchanged if the suffix is absent. Used to
        /// normalize the pluralized/suffixed labels in newer Scoutbook exports
        /// (e.g. "Scout Rank" -> "Scout", "Camping MB" -> "Camping").
        /// </summary>
        internal static string StripSuffix(string value, string suffix)
        {
            if (!string.IsNullOrEmpty(value) && value.EndsWith(suffix))
            {
                return value.Substring(0, value.Length - suffix.Length).TrimEnd();
            }
            return value?.Trim();
        }

        /// <summary>
        /// Maps a rank display name to the corresponding <see cref="Rank"/> on the scout.
        /// Returns null for names that are not Scouts BSA ranks (e.g. Cub Scout ranks),
        /// so their rows are ignored.
        /// </summary>
        internal static Rank GetRankByName(TroopMember scout, string rankName)
        {
            switch (rankName)
            {
                case "Scout":
                    return scout.Scout;
                case "Tenderfoot":
                    return scout.Tenderfoot;
                case "Second Class":
                    return scout.SecondClass;
                case "First Class":
                    return scout.FirstClass;
                case "Star Scout":
                    return scout.Star;
                case "Life Scout":
                    return scout.Life;
                case "Eagle Scout":
                    return scout.Eagle;
                default:
                    return null;
            }
        }
    }
}
