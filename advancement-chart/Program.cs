using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using advancementchart.Model;
using advancementchart.Reports;
using CsvHelper;

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

            var report = new TroopReport(scouts);
            report.Run(@"/Users/andrew/src/TroopAdvancementChart.xlsx");
        }

        static bool LoadFile(string fileName)
        {
            bool result = false;
            if (File.Exists(fileName))
            {
                TextReader txtReader = new StreamReader(fileName);
                var csvReader = new CsvReader(txtReader);
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
                    }

                    string type = csvReader.GetField(index: 4);
                    string subtype = csvReader.GetField(index: 5);
                    string version = csvReader.GetField(index: 6);
                    DateTime date = csvReader.GetField<DateTime>(index: 7);
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
                            scout.Scout.Requirements.First(req => req.Name == subtype).DateEarned = date;
                            break;
                        case "Tenderfoot Rank Requirement":
                            scout.Tenderfoot.Requirements.First(req => req.Name == subtype).DateEarned = date;
                            break;
                        case "Second Class Rank Requirement":
                            scout.SecondClass.Requirements.First(req => req.Name == subtype).DateEarned = date;
                            break;
                        case "First Class Rank Requirement":
                            scout.FirstClass.Requirements.First(req => req.Name == subtype).DateEarned = date;
                            break;
                        case "Star Scout Rank Requirement":
                            scout.Star.Requirements.First(req => req.Name == subtype).DateEarned = date;
                            break;
                        case "Life Scout Rank Requirement":
                            scout.Life.Requirements.First(req => req.Name == subtype).DateEarned = date;
                            break;
                        case "Eagle Scout Rank Requirement":
                            scout.Eagle.Requirements.First(req => req.Name == subtype).DateEarned = date;
                            break;
                    }
                }
                result = true;
            }
            return result;
        }
    }

}