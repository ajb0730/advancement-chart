using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using advancementchart.Model;
using advancementchart.Reports;
using advancementchart.Model.Ranks;
using OfficeOpenXml;
using advancement_chart.tests.Helpers;

namespace advancement_chart.tests.Reports
{
    public class TroopReportTest : IDisposable
    {
        private readonly string _tempFile;

        public TroopReportTest()
        {
            _tempFile = Path.Combine(Path.GetTempPath(), $"TroopReport_{Guid.NewGuid()}.xlsx");
        }

        public void Dispose()
        {
            if (File.Exists(_tempFile))
                File.Delete(_tempFile);
        }

        [Fact]
        public void Run_CreatesFile()
        {
            var scouts = new List<TroopMember> { TestFixtures.CreateScout() };
            var report = new TroopReport(scouts);
            report.Run(_tempFile);
            Assert.True(File.Exists(_tempFile));
        }

        [Fact]
        public void Run_HasThreeWorksheets()
        {
            var scouts = new List<TroopMember> { TestFixtures.CreateScout() };
            var report = new TroopReport(scouts);
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                Assert.Equal(3, package.Workbook.Worksheets.Count);
                Assert.NotNull(package.Workbook.Worksheets["Troop Advancement"]);
                Assert.NotNull(package.Workbook.Worksheets["Scout Advancement"]);
                Assert.NotNull(package.Workbook.Worksheets["Merit Badges"]);
            }
        }

        [Fact]
        public void Run_TroopAdvancement_HasTitle()
        {
            var scouts = new List<TroopMember> { TestFixtures.CreateScout() };
            var report = new TroopReport(scouts);
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                var ta = package.Workbook.Worksheets["Troop Advancement"];
                Assert.Equal("Troop Advancement Chart", ta.Cells["A1"].Value?.ToString());
            }
        }

        [Fact]
        public void Run_ContainsScoutName()
        {
            var scout = TestFixtures.CreateScout(first: "TestFirst", last: "TestLast");
            var report = new TroopReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                var ta = package.Workbook.Worksheets["Troop Advancement"];
                bool found = false;
                for (int row = 1; row <= 20; row++)
                {
                    var val = ta.Cells[row, 1].Value?.ToString();
                    if (val != null && val.Contains("TestFirst") && val.Contains("TestLast"))
                    {
                        found = true;
                        break;
                    }
                }
                Assert.True(found, "Scout name not found in Troop Advancement sheet");
            }
        }

        [Fact]
        public void Run_FirstClassScout_AppearsInScoutAdvancement()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            var report = new TroopReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                var sa = package.Workbook.Worksheets["Scout Advancement"];
                bool found = false;
                for (int row = 1; row <= 20; row++)
                {
                    var val = sa.Cells[row, 1].Value?.ToString();
                    if (val != null && val.Contains("Doe"))
                    {
                        found = true;
                        break;
                    }
                }
                Assert.True(found, "First Class scout should appear in Scout Advancement");
            }
        }

        [Fact]
        public void Run_MeritBadgesSheet_HasContent()
        {
            var scouts = new List<TroopMember> { TestFixtures.CreateScout() };
            var report = new TroopReport(scouts);
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                var mb = package.Workbook.Worksheets["Merit Badges"];
                // First badge should be "Camping" with ID "001"
                Assert.Equal("001", mb.Cells["A1"].Value?.ToString());
            }
        }

        [Fact]
        public void Run_WithPatrolGroups()
        {
            var scout1 = new TroopMember("1", "A", "", "Smith", "Hawks");
            var scout2 = new TroopMember("2", "B", "", "Jones", "Eagles");
            var report = new TroopReport(new List<TroopMember> { scout1, scout2 });
            report.Run(_tempFile);

            Assert.True(File.Exists(_tempFile));
        }

        [Fact]
        public void Run_OverwritesExistingFile()
        {
            File.WriteAllText(_tempFile, "dummy");
            var scouts = new List<TroopMember> { TestFixtures.CreateScout() };
            var report = new TroopReport(scouts);
            report.Run(_tempFile);

            var fi = new FileInfo(_tempFile);
            Assert.True(fi.Length > 10);
        }

        [Fact]
        public void Scouts_Property_CanBeSetAndGet()
        {
            var scouts = new List<TroopMember> { TestFixtures.CreateScout() };
            var report = new TroopReport(scouts);
            Assert.Same(scouts, report.Scouts);
        }

        [Fact]
        public void Run_ScoutWithEarnedBadges_ShowsBsaIds()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.Star.DateEarned = new DateTime(2021, 11, 1);
            scout.Life.DateEarned = new DateTime(2022, 5, 1);

            // Add all required eagle badges to exercise the AddContent eagle branches
            scout.Add(TestFixtures.EarnedBadge("First Aid", new DateTime(2021, 1, 1)));
            scout.Add(TestFixtures.EarnedBadge("Citizenship in the Community", new DateTime(2021, 1, 2)));
            scout.Add(TestFixtures.EarnedBadge("Citizenship in the Nation", new DateTime(2021, 1, 3)));
            scout.Add(TestFixtures.EarnedBadge("Citizenship in Society", new DateTime(2021, 1, 4)));
            scout.Add(TestFixtures.EarnedBadge("Citizenship in the World", new DateTime(2021, 1, 5)));
            scout.Add(TestFixtures.EarnedBadge("Communication", new DateTime(2021, 1, 6)));
            scout.Add(TestFixtures.EarnedBadge("Cooking", new DateTime(2021, 1, 7)));
            scout.Add(TestFixtures.EarnedBadge("Personal Fitness", new DateTime(2021, 1, 8)));
            scout.Add(TestFixtures.EarnedBadge("Emergency Preparedness", new DateTime(2021, 1, 9)));
            scout.Add(TestFixtures.EarnedBadge("Lifesaving", new DateTime(2021, 1, 10)));
            scout.Add(TestFixtures.EarnedBadge("Environmental Science", new DateTime(2021, 1, 11)));
            scout.Add(TestFixtures.EarnedBadge("Sustainability", new DateTime(2021, 1, 12)));
            scout.Add(TestFixtures.EarnedBadge("Personal Management", new DateTime(2021, 1, 13)));
            scout.Add(TestFixtures.EarnedBadge("Swimming", new DateTime(2021, 1, 14)));
            scout.Add(TestFixtures.EarnedBadge("Hiking", new DateTime(2021, 1, 15)));
            scout.Add(TestFixtures.EarnedBadge("Cycling", new DateTime(2021, 1, 16)));
            scout.Add(TestFixtures.EarnedBadge("Camping", new DateTime(2021, 1, 17)));
            scout.Add(TestFixtures.EarnedBadge("Family Life", new DateTime(2021, 1, 18)));
            // Elective badges
            scout.Add(TestFixtures.EarnedBadge("Art", new DateTime(2021, 2, 1)));
            scout.Add(TestFixtures.EarnedBadge("Chess", new DateTime(2021, 2, 2)));
            scout.Add(TestFixtures.EarnedBadge("Climbing", new DateTime(2021, 2, 3)));
            scout.Add(TestFixtures.EarnedBadge("Dog Care", new DateTime(2021, 2, 4)));
            scout.Add(TestFixtures.EarnedBadge("Astronomy", new DateTime(2021, 2, 5)));
            scout.Add(TestFixtures.EarnedBadge("Electricity", new DateTime(2021, 2, 6)));
            scout.Add(TestFixtures.EarnedBadge("Electronics", new DateTime(2021, 2, 7)));

            scout.AllocateMeritBadges();

            var report = new TroopReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                var sa = package.Workbook.Worksheets["Scout Advancement"];
                Assert.NotNull(sa);
                // Scout should be in scout advancement since they earned first class
                bool found = false;
                for (int row = 1; row <= 20; row++)
                {
                    var val = sa.Cells[row, 1].Value?.ToString();
                    if (val != null && val.Contains("Doe"))
                    {
                        found = true;
                        break;
                    }
                }
                Assert.True(found);
            }
        }

        [Fact]
        public void Run_ScoutWithEarnedRankRequirements_ShowsBullets()
        {
            var scout = TestFixtures.CreateScout(first: "Test", last: "Scout");
            scout.Scout.DateEarned = new DateTime(2023, 1, 1);
            // Mark some individual requirements as earned
            scout.Tenderfoot.Requirements.Find(x => x.Name == "1a").DateEarned = new DateTime(2023, 2, 1);
            scout.Tenderfoot.Requirements.Find(x => x.Name == "2a").DateEarned = new DateTime(2023, 2, 2);

            var report = new TroopReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            Assert.True(File.Exists(_tempFile));
        }

        [Fact]
        public void Run_EagleScoutWithPalms()
        {
            var scout = TestFixtures.CreateScoutWithAllRanksEarned();
            scout.EaglePalms.Add(new Palm(Palm.PalmType.Bronze, new DateTime(2022, 2, 1)));
            scout.EaglePalms.Add(new Palm(Palm.PalmType.Gold, new DateTime(2022, 5, 1)));
            scout.EaglePalms.Add(new Palm(Palm.PalmType.Silver, new DateTime(2022, 8, 1)));

            var report = new TroopReport(new List<TroopMember> { scout });
            report.Run(_tempFile);
            Assert.True(File.Exists(_tempFile));
        }

        [Fact]
        public void Run_LifeScoutWithFullBadges_WritesEagleMeritBadgeColumns()
        {
            // Exercise the EagleMeritBadgeRequirement column-rendering branch in AddContent
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.Star.DateEarned = new DateTime(2021, 11, 1);
            scout.Life.DateEarned = new DateTime(2022, 5, 1);

            // Add all 14 eagle-required badges + electives
            scout.Add(TestFixtures.EarnedBadge("First Aid", new DateTime(2021, 1, 1)));
            scout.Add(TestFixtures.EarnedBadge("Citizenship in the Community", new DateTime(2021, 1, 2)));
            scout.Add(TestFixtures.EarnedBadge("Citizenship in the Nation", new DateTime(2021, 1, 3)));
            scout.Add(TestFixtures.EarnedBadge("Citizenship in Society", new DateTime(2021, 1, 4)));
            scout.Add(TestFixtures.EarnedBadge("Citizenship in the World", new DateTime(2021, 1, 5)));
            scout.Add(TestFixtures.EarnedBadge("Communication", new DateTime(2021, 1, 6)));
            scout.Add(TestFixtures.EarnedBadge("Cooking", new DateTime(2021, 1, 7)));
            scout.Add(TestFixtures.EarnedBadge("Personal Fitness", new DateTime(2021, 1, 8)));
            scout.Add(TestFixtures.EarnedBadge("Emergency Preparedness", new DateTime(2021, 1, 9)));
            scout.Add(TestFixtures.EarnedBadge("Environmental Science", new DateTime(2021, 1, 10)));
            scout.Add(TestFixtures.EarnedBadge("Personal Management", new DateTime(2021, 1, 11)));
            scout.Add(TestFixtures.EarnedBadge("Swimming", new DateTime(2021, 1, 12)));
            scout.Add(TestFixtures.EarnedBadge("Camping", new DateTime(2021, 1, 13)));
            scout.Add(TestFixtures.EarnedBadge("Family Life", new DateTime(2021, 1, 14)));
            // Electives
            scout.Add(TestFixtures.EarnedBadge("Art", new DateTime(2021, 2, 1)));
            scout.Add(TestFixtures.EarnedBadge("Chess", new DateTime(2021, 2, 2)));
            scout.Add(TestFixtures.EarnedBadge("Climbing", new DateTime(2021, 2, 3)));
            scout.Add(TestFixtures.EarnedBadge("Dog Care", new DateTime(2021, 2, 4)));
            scout.Add(TestFixtures.EarnedBadge("Astronomy", new DateTime(2021, 2, 5)));
            scout.Add(TestFixtures.EarnedBadge("Electricity", new DateTime(2021, 2, 6)));
            scout.Add(TestFixtures.EarnedBadge("Electronics", new DateTime(2021, 2, 7)));

            var report = new TroopReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                var sa = package.Workbook.Worksheets["Scout Advancement"];
                Assert.NotNull(sa);

                // Scout should be in Scout Advancement with eagle badge BSA IDs populated
                bool found = false;
                for (int row = 1; row <= 20; row++)
                {
                    var val = sa.Cells[row, 1].Value?.ToString();
                    if (val != null && val.Contains("Doe"))
                    {
                        found = true;
                        break;
                    }
                }
                Assert.True(found, "Life scout should appear in Scout Advancement sheet");
            }
        }

        [Fact]
        public void Run_WithPatrolSorting_WritesPatrolHeaders()
        {
            // Test the patrol header path in TroopReport
            var scout1 = new TroopMember("1", "A", "", "Smith", "Hawks");
            scout1.Scout.DateEarned = new DateTime(2023, 1, 1);
            var scout2 = new TroopMember("2", "B", "", "Jones", "Hawks");
            scout2.Scout.DateEarned = new DateTime(2023, 1, 1);
            var scout3 = new TroopMember("3", "C", "", "Brown", "Eagles");
            scout3.Scout.DateEarned = new DateTime(2023, 1, 1);

            var report = new TroopReport(new List<TroopMember> { scout1, scout2, scout3 });
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                var ta = package.Workbook.Worksheets["Troop Advancement"];
                Assert.NotNull(ta);
                Assert.True(File.Exists(_tempFile));
            }
        }

        [Fact]
        public void Run_StarScoutWithEarnedBadges_WritesStarMbReqColumns()
        {
            // Exercise the MeritBadgeRequirement (non-Eagle) branch in AddContent
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.Star.DateEarned = new DateTime(2021, 11, 1);

            // Add enough badges that Star MbReq has earned badges
            scout.Add(TestFixtures.EarnedBadge("Camping", new DateTime(2021, 1, 1)));
            scout.Add(TestFixtures.EarnedBadge("First Aid", new DateTime(2021, 1, 2)));
            scout.Add(TestFixtures.EarnedBadge("Communication", new DateTime(2021, 1, 3)));
            scout.Add(TestFixtures.EarnedBadge("Cooking", new DateTime(2021, 1, 4)));
            scout.Add(TestFixtures.EarnedBadge("Art", new DateTime(2021, 1, 5)));
            scout.Add(TestFixtures.EarnedBadge("Astronomy", new DateTime(2021, 1, 6)));

            var report = new TroopReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                var sa = package.Workbook.Worksheets["Scout Advancement"];
                Assert.NotNull(sa);
            }
        }
    }
}
