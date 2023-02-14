using System;
using System.Collections.Generic;
using advancementchart.Model;
using System.Linq;

namespace advancementchart.Reports
{
    public class AdvancementCheck : IReport
    {
        public AdvancementCheck(List<TroopMember> scouts)
        {
            Scouts = scouts;
        }

        public List<TroopMember> Scouts { get; set; }

        public void Run(string outputFileName)
        {
            Console.WriteLine("Running Advancement Check");
            foreach (var scout in this.Scouts)
            {
                // Eagle implies Life implies Star implies 1C implies 2C implies Tenderfoot implies Scout
                if (scout.Eagle.Earned && !scout.Life.Earned)
                {
                    Console
                        .Error
                        .WriteLine("ERROR [{0}] has earned Eagle but not Life",
                        scout.DisplayName);
                }
                if (scout.Life.Earned && !scout.Star.Earned)
                {
                    Console
                        .Error
                        .WriteLine("ERROR [{0}] has earned Life but not Star",
                        scout.DisplayName);
                }
                if (scout.Star.Earned && !scout.FirstClass.Earned)
                {
                    Console
                        .Error
                        .WriteLine("ERROR [{0}] has earned Star but not First Class",
                        scout.DisplayName);
                }
                if (scout.FirstClass.Earned && !scout.SecondClass.Earned)
                {
                    Console
                        .Error
                        .WriteLine("ERROR [{0}] has earned First Class but not Second Class",
                        scout.DisplayName);
                }
                if (scout.SecondClass.Earned && !scout.Tenderfoot.Earned)
                {
                    Console
                        .Error
                        .WriteLine("ERROR [{0}] has earned Second Class but not Tenderfoot",
                        scout.DisplayName);
                }
                if (scout.Tenderfoot.Earned && !scout.Scout.Earned)
                {
                    Console
                        .Error
                        .WriteLine("ERROR [{0}] has earned Tenderfoot but not Scout",
                        scout.DisplayName);
                }

                // 1C.1a implies 2C.1a
                if (
                    scout
                        .FirstClass
                        .Requirements
                        .Find(x => x.Name == "1a")
                        .Earned &&
                    !scout
                        .SecondClass
                        .Requirements
                        .Find(x => x.Name == "1a")
                        .Earned
                )
                {
                    Console
                        .Error
                        .WriteLine("ERROR [{0}] has earned First Class #1a but not Second Class #1a",
                        scout.DisplayName);
                }

                // if !Star.earned && Today > (1C.DateEarned + 4 Months)
                if (
                    !scout.Star.Earned &&
                    scout.FirstClass.Earned &&
                    DateTime.Now >=
                    scout.FirstClass.DateEarned.Value.AddMonths(4) &&
                    !scout.Star.Requirements.Find(x => x.Name == "1").Earned
                )
                {
                    Console
                        .Error
                        .WriteLine("WARN  [{0}] {1} is 4 months since earning First Class, check Star #1",
                        scout.DisplayName,
                        scout.FirstClass.DateEarned.Value.AddMonths(4));
                }

                if (
                    !scout.Life.Earned &&
                    scout.Star.Earned &&
                    DateTime.Now >= scout.Star.DateEarned.Value.AddMonths(6) &&
                    !scout.Life.Requirements.Find(x => x.Name == "1").Earned
                )
                {
                    Console
                        .Error
                        .WriteLine("WARN  [{0}] {1} is 6 months since earning Star, check Life #1",
                        scout.DisplayName,
                        scout.Star.DateEarned.Value.AddMonths(6));
                }

                if (
                    !scout.Eagle.Earned &&
                    scout.Life.Earned &&
                    DateTime.Now >= scout.Life.DateEarned.Value.AddMonths(6) &&
                    !scout.Eagle.Requirements.Find(x => x.Name == "1").Earned
                )
                {
                    Console
                        .Error
                        .WriteLine("WARN  [{0}] {1} is 6 months since earning Life, check Eagle #1",
                        scout.DisplayName,
                        scout.Life.DateEarned.Value.AddMonths(6));
                }

                if (
                    scout.Tenderfoot.Requirements.Any((x) => x.DateEarned.HasValue) &&
                    scout.Scout.Requirements.Any((x) => x.DateEarned.HasValue) &&
                    scout.Tenderfoot.Version.CompareTo(scout.Scout.Version) < 0
                )
                {
                    Console.Error.WriteLine("WARN  [{0}] Tenderfoot version, {1}, should be >= Scout version, {2}", scout.DisplayName, scout.Tenderfoot.Version, scout.Scout.Version);
                }
                if(
                    scout.SecondClass.Requirements.Any((x) => x.DateEarned.HasValue) &&
                    scout.Tenderfoot.Requirements.Any((x) => x.DateEarned.HasValue) &&
                    scout.SecondClass.Version.CompareTo(scout.Tenderfoot.Version) < 0
                ) {
                    Console.Error.WriteLine("WARN  [{0}] Second Class version, {1}, should be >= Tenderfoot version, {2}", scout.DisplayName, scout.SecondClass.Version, scout.Tenderfoot.Version);
                }
                if(
                    scout.FirstClass.Requirements.Any((x) => x.DateEarned.HasValue) &&
                    scout.SecondClass.Requirements.Any((x) => x.DateEarned.HasValue) &&
                    scout.FirstClass.Version.CompareTo(scout.SecondClass.Version) < 0
                ) {
                    Console.Error.WriteLine("WARN  [{0}] First Class version, {1}, should be >= Second Class version, {2}", scout.DisplayName, scout.FirstClass.Version, scout.SecondClass.Version);
                }
                if(
                    scout.Star.Requirements.Any((x) => x.DateEarned.HasValue) &&
                    scout.FirstClass.Requirements.Any((x) => x.DateEarned.HasValue) &&
                    scout.Star.Version.CompareTo(scout.FirstClass.Version) < 0
                ) {
                    Console.Error.WriteLine("WARN  [{0}] Star version, {1}, should be >= First Class version, {2}", scout.DisplayName, scout.Star.Version, scout.FirstClass.Version);
                }
                if(
                    scout.Life.Requirements.Any((x) => x.DateEarned.HasValue) &&
                    scout.Star.Requirements.Any((x) => x.DateEarned.HasValue) &&
                    scout.Life.Version.CompareTo(scout.Star.Version) < 0
                ) {
                    Console.Error.WriteLine("WARN  [{0}] Life version, {1}, should be >= Star version, {2}", scout.DisplayName, scout.Life.Version, scout.Star.Version);
                }
            }
        }
    }
}
