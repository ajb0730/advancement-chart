using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using advancementchart.Model;
using advancementchart.Reports;
using CsvHelper;
using System.Globalization;

namespace advancement_chart
{
    class Program
    {
        static readonly List<TroopMember> scouts = new List<TroopMember>();

        static void Main(string[] args)
        {
            foreach (string arg in args)
            {
                LoadFile(arg);
            }
            //
            // Download the "Scout" backup report from Scoutbook and rename
            // the file "scouts.csv"
            //
            LoadPatrolLookup("./scouts.csv");
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
        }

        private static void LoadPatrolLookup(string fileName)
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
                            if(!string.IsNullOrWhiteSpace(dob))
                            {
                                DateTime dobDate;
                                if(DateTime.TryParse(dob, out dobDate))
                                {
                                    scout.DateOfBirth = dobDate;
                                }
                            }
                        }
                    }
                }
            }
        }

        static bool LoadFile(string fileName)
        {
            bool result = false;
            if (File.Exists(fileName))
            {
                Console.WriteLine($"Reading data from {fileName}.");
                TextReader txtReader = new StreamReader(fileName);
                var csvReader = new CsvReader(txtReader, CultureInfo.CurrentCulture);
                csvReader.Read();
                csvReader.ReadHeader();

                while (csvReader.Read())
                {
                    string id = csvReader.GetField(index: 0);
                    TroopMember scout = scouts.FirstOrDefault(tm => tm.BsaMemberId == id);
                    if (null == scout)
                    {
                        string firstName = csvReader.GetField(index: 1);
                        string middleName = csvReader.GetField(index: 2);
                        string lastName = csvReader.GetField(index: 3);
                        scout = new TroopMember(memberId: id, firstName: firstName, middleName: middleName, lastName: lastName);
                        scouts.Add(scout);
                        Console.WriteLine($"Adding record for {firstName} {lastName}.");
                    }

                    string type = csvReader.GetField(index: 4);
                    string subtype = csvReader.GetField(index: 5);
                    string version = csvReader.GetField(index: 6);
                    DateTime date = csvReader.GetField<DateTime>(index: 7);
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
                        }
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine($"type: {type} subtype: {subtype} version: {version} date: {date}");
                        Console.Error.WriteLine($"{e}");
                    }
                }
                result = true;
            }
            return result;
        }
    }
}
