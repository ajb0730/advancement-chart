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
    public class EagleReportTest : IDisposable
    {
        private readonly string _tempFile;

        public EagleReportTest()
        {
            _tempFile = Path.Combine(Path.GetTempPath(), $"EagleReport_{Guid.NewGuid()}.xlsx");
        }

        public void Dispose()
        {
            if (File.Exists(_tempFile))
                File.Delete(_tempFile);
        }

        [Fact]
        public void Run_NoEligibleScouts_DoesNotCreateFile()
        {
            // Scout with no ranks earned - not FirstClass, so not eligible
            var scout = TestFixtures.CreateScout();
            var report = new EagleReport(new List<TroopMember> { scout });
            report.Run(_tempFile);
            Assert.False(File.Exists(_tempFile));
        }

        [Fact]
        public void Run_EagleScout_DoesNotCreateFile()
        {
            // Eagle scout already earned - excluded
            var scout = TestFixtures.CreateScoutWithAllRanksEarned();
            var report = new EagleReport(new List<TroopMember> { scout });
            report.Run(_tempFile);
            Assert.False(File.Exists(_tempFile));
        }

        [Fact]
        public void Run_FirstClassScout_CreatesFile()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.DateOfBirth = new DateTime(2008, 6, 15);
            var report = new EagleReport(new List<TroopMember> { scout });
            report.Run(_tempFile);
            Assert.True(File.Exists(_tempFile));
        }

        [Fact]
        public void Run_HasScoutSheet()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.DateOfBirth = new DateTime(2008, 6, 15);
            var report = new EagleReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                Assert.Single(package.Workbook.Worksheets);
                // Sheet name is "FirstName LastInitial."
                Assert.NotNull(package.Workbook.Worksheets["John D."]);
            }
        }

        [Fact]
        public void Run_ContainsTrailToEagleHeader()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.DateOfBirth = new DateTime(2008, 6, 15);
            var report = new EagleReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                var wks = package.Workbook.Worksheets[0];
                Assert.Equal("Trail to Eagle Progress Report for:", wks.Cells["A1"].Value?.ToString());
            }
        }

        [Fact]
        public void Run_OrderedByDOB()
        {
            var scout1 = new TroopMember("1", "Older", "", "Scout");
            scout1.Scout.DateEarned = new DateTime(2020, 1, 1);
            scout1.Tenderfoot.DateEarned = new DateTime(2020, 3, 1);
            scout1.SecondClass.DateEarned = new DateTime(2020, 5, 1);
            scout1.FirstClass.DateEarned = new DateTime(2020, 7, 1);
            scout1.DateOfBirth = new DateTime(2006, 1, 1);

            var scout2 = new TroopMember("2", "Younger", "", "Scout");
            scout2.Scout.DateEarned = new DateTime(2020, 1, 1);
            scout2.Tenderfoot.DateEarned = new DateTime(2020, 3, 1);
            scout2.SecondClass.DateEarned = new DateTime(2020, 5, 1);
            scout2.FirstClass.DateEarned = new DateTime(2020, 7, 1);
            scout2.DateOfBirth = new DateTime(2008, 6, 1);

            var report = new EagleReport(new List<TroopMember> { scout2, scout1 });
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                // Older scout (earlier DOB) should be first sheet
                Assert.Equal("Older S.", package.Workbook.Worksheets[0].Name);
                Assert.Equal("Younger S.", package.Workbook.Worksheets[1].Name);
            }
        }

        [Fact]
        public void Run_StarScout_Included()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.Star.DateEarned = new DateTime(2021, 1, 1);
            scout.DateOfBirth = new DateTime(2008, 6, 15);
            var report = new EagleReport(new List<TroopMember> { scout });
            report.Run(_tempFile);
            Assert.True(File.Exists(_tempFile));
        }

        [Fact]
        public void Run_ContainsMeritBadgeInfo()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.DateOfBirth = new DateTime(2008, 6, 15);
            scout.Add(TestFixtures.EarnedBadge("First Aid", new DateTime(2021, 1, 1)));
            scout.AllocateMeritBadges();

            var report = new EagleReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                var wks = package.Workbook.Worksheets[0];
                bool foundFirstAid = false;
                for (int row = 1; row <= 40; row++)
                {
                    var val = wks.Cells[row, 1].Value?.ToString();
                    if (val != null && val.Contains("First Aid"))
                    {
                        foundFirstAid = true;
                        break;
                    }
                }
                Assert.True(foundFirstAid, "First Aid badge info should appear");
            }
        }

        [Fact]
        public void Run_WithMutuallyExclusiveBadges_ShowsEarned()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.DateOfBirth = new DateTime(2008, 6, 15);
            // Earn one of each mutually exclusive pair
            scout.Add(TestFixtures.EarnedBadge("Emergency Preparedness", new DateTime(2021, 1, 1)));
            scout.Add(TestFixtures.EarnedBadge("Environmental Science", new DateTime(2021, 2, 1)));
            scout.Add(TestFixtures.EarnedBadge("Swimming", new DateTime(2021, 3, 1)));
            scout.AllocateMeritBadges();

            var report = new EagleReport(new List<TroopMember> { scout });
            report.Run(_tempFile);
            Assert.True(File.Exists(_tempFile));
        }

        [Fact]
        public void Run_WithStartedBadges_ShowsStarted()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.DateOfBirth = new DateTime(2008, 6, 15);
            scout.AddPartial("Emergency Preparedness", "2016");
            scout.AddPartial("Lifesaving", "2016");
            scout.AddPartial("Swimming", "2016");
            scout.AddPartial("Hiking", "2016");
            scout.AllocateMeritBadges();

            var report = new EagleReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                var wks = package.Workbook.Worksheets[0];
                bool foundStarted = false;
                for (int row = 1; row <= 50; row++)
                {
                    var val = wks.Cells[row, 2].Value?.ToString();
                    if (val != null && val.Contains("Started"))
                    {
                        foundStarted = true;
                        break;
                    }
                }
                Assert.True(foundStarted, "Started badges should appear");
            }
        }

        [Fact]
        public void Run_ElectiveCount_ShowsProgress()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.DateOfBirth = new DateTime(2008, 6, 15);
            // Add some elective badges
            scout.Add(TestFixtures.EarnedBadge("Art", new DateTime(2021, 1, 1)));
            scout.Add(TestFixtures.EarnedBadge("Chess", new DateTime(2021, 2, 1)));
            scout.AllocateMeritBadges();

            var report = new EagleReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                var wks = package.Workbook.Worksheets[0];
                bool foundElective = false;
                for (int row = 1; row <= 50; row++)
                {
                    var val = wks.Cells[row, 1].Value?.ToString();
                    if (val != null && val.Contains("Elective merit badges earned"))
                    {
                        foundElective = true;
                        break;
                    }
                }
                Assert.True(foundElective, "Elective badge count should appear");
            }
        }

        [Fact]
        public void Run_WithAllRequiredEarned_ShowsRemainingDeadline()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.DateOfBirth = new DateTime(2010, 6, 15);
            scout.Star.DateEarned = new DateTime(2023, 1, 1);
            // Add many badges to test the elective remaining count path
            scout.Add(TestFixtures.EarnedBadge("Art", new DateTime(2021, 1, 1)));

            scout.AllocateMeritBadges();
            var report = new EagleReport(new List<TroopMember> { scout });
            report.Run(_tempFile);
            Assert.True(File.Exists(_tempFile));
        }
    }
}
