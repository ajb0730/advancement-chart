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

                        string type = csvReader.GetField("Advancement Type");
                        string subtype = csvReader.GetField("Advancement");
                        string version = csvReader.GetField("Version");
                        DateTime date = csvReader.GetField<DateTime>("Date Completed");
                        result = date > result ? date : result;
                        try
                        {
                            switch (type)
                            {
                                case "Rank":
                                    switch (subtype)
                                    {
                                        case "Scout":
                                            scout.Scout.DateEarned = date;
                                            break;
                                        case "Tenderfoot":
                                            scout.Tenderfoot.DateEarned = date;
                                            break;
                                        case "Second Class":
                                            scout.SecondClass.DateEarned = date;
                                            break;
                                        case "First Class":
                                            scout.FirstClass.DateEarned = date;
                                            break;
                                        case "Star Scout":
                                            scout.Star.DateEarned = date;
                                            break;
                                        case "Life Scout":
                                            scout.Life.DateEarned = date;
                                            break;
                                        case "Eagle Scout":
                                            scout.Eagle.DateEarned = date;
                                            break;
                                    }
                                    break;
                                case "Award":
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
                                    break;
                                case "Merit Badge":
                                    MeritBadge badge = new MeritBadge(name: subtype, description: version, earned: date);
                                    scout.Add(badge);
                                    break;
                                case "Scout Rank Requirement":
                                    if (!string.IsNullOrWhiteSpace(subtype) && scout.Scout.Requirements.Any(req => req.Name == subtype))
                                        scout.Scout.Requirements.First(req => req.Name == subtype).DateEarned = date;
                                    break;
                                case "Tenderfoot Rank Requirement":
                                    if (!string.IsNullOrWhiteSpace(subtype) && scout.Tenderfoot.Requirements.Any(req => req.Name == subtype))
                                        scout.Tenderfoot.Requirements.First(req => req.Name == subtype).DateEarned = date;
                                    break;
                                case "Second Class Rank Requirement":
                                    if (!string.IsNullOrWhiteSpace(subtype) && scout.SecondClass.Requirements.Any(req => req.Name == subtype))
                                        scout.SecondClass.Requirements.First(req => req.Name == subtype).DateEarned = date;
                                    break;
                                case "First Class Rank Requirement":
                                    if (!string.IsNullOrWhiteSpace(subtype) && scout.FirstClass.Requirements.Any(req => req.Name == subtype))
                                        scout.FirstClass.Requirements.First(req => req.Name == subtype).DateEarned = date;
                                    break;
                                case "Star Scout Rank Requirement":
                                    if (!string.IsNullOrWhiteSpace(subtype) && scout.Star.Requirements.Any(req => req.Name == subtype))
                                        scout.Star.Requirements.First(req => req.Name == subtype).DateEarned = date;
                                    break;
                                case "Life Scout Rank Requirement":
                                    if (!string.IsNullOrWhiteSpace(subtype) && scout.Life.Requirements.Any(req => req.Name == subtype))
                                        scout.Life.Requirements.First(req => req.Name == subtype).DateEarned = date;
                                    break;
                                case "Eagle Scout Rank Requirement":
                                    if (!string.IsNullOrWhiteSpace(subtype) && scout.Eagle.Requirements.Any(req => req.Name == subtype))
                                        scout.Eagle.Requirements.First(req => req.Name == subtype).DateEarned = date;
                                    break;
                                case "Merit Badge Requirement":
                                    var badgeName = subtype.Substring(0, subtype.IndexOf("#") - 1).Trim();
                                    scout.AddPartial(badgeName, version);
                                    break;
                                case "Award Requirement":
                                    var palmName = subtype.Substring(0, subtype.LastIndexOf("#") - 1).Trim();
                                    var requirementNumber = subtype.Substring(subtype.LastIndexOf("#") + 1).Trim();
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
                                            palm.Requirements.First(x => x.Name == requirementNumber).DateEarned = date;
                                        }
                                    }
                                    break;
                            }
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
    }
}
