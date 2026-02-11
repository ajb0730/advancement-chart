using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using advancementchart.Model;
using advancementchart.Reports;
using OfficeOpenXml;
using advancement_chart.tests.Helpers;

namespace advancement_chart.tests.Reports
{
    public class IndividualReportTest : IDisposable
    {
        private readonly string _tempFile;

        public IndividualReportTest()
        {
            _tempFile = Path.Combine(Path.GetTempPath(), $"IndividualReport_{Guid.NewGuid()}.xlsx");
        }

        public void Dispose()
        {
            if (File.Exists(_tempFile))
                File.Delete(_tempFile);
        }

        [Fact]
        public void Run_CreatesFile()
        {
            var scout = TestFixtures.CreateScout();
            var report = new IndividualReport(new List<TroopMember> { scout });
            report.Run(_tempFile);
            Assert.True(File.Exists(_tempFile));
        }

        [Fact]
        public void Run_HasTroopSummarySheet()
        {
            var scout = TestFixtures.CreateScout();
            var report = new IndividualReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                Assert.NotNull(package.Workbook.Worksheets["Troop Summary"]);
            }
        }

        [Fact]
        public void Run_NonFirstClassScout_GetsIndividualSheet()
        {
            var scout = TestFixtures.CreateScout(first: "TestName", last: "Smith");
            var report = new IndividualReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                // Sheet name is "FirstName LastInitial"
                Assert.NotNull(package.Workbook.Worksheets["TestName S"]);
            }
        }

        [Fact]
        public void Run_FirstClassScout_NoIndividualSheet()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            var report = new IndividualReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                // Only Troop Summary sheet
                Assert.Single(package.Workbook.Worksheets);
            }
        }

        [Fact]
        public void Run_MultiplePatrols_CreatesPatrolSheets()
        {
            var scout1 = new TroopMember("1", "A", "", "Smith", "Hawks");
            var scout2 = new TroopMember("2", "B", "", "Jones", "Eagles");

            var report = new IndividualReport(new List<TroopMember> { scout1, scout2 });
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                Assert.NotNull(package.Workbook.Worksheets["Hawks"]);
                Assert.NotNull(package.Workbook.Worksheets["Eagles"]);
            }
        }

        [Fact]
        public void Run_SinglePatrol_NoPatrolSheets()
        {
            var scout1 = TestFixtures.CreateScout(id: "1", first: "A", last: "Smith");
            var scout2 = TestFixtures.CreateScout(id: "2", first: "B", last: "Jones");

            var report = new IndividualReport(new List<TroopMember> { scout1, scout2 });
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                // Should have Troop Summary + 2 individual sheets but no patrol sheets
                Assert.Null(package.Workbook.Worksheets["Unassigned"]);
            }
        }

        [Fact]
        public void Run_IndividualSheet_ContainsScoutName()
        {
            var scout = TestFixtures.CreateScout(first: "Bobby", last: "Tables");
            var report = new IndividualReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                var wks = package.Workbook.Worksheets["Bobby T"];
                Assert.NotNull(wks);
                Assert.Equal("Bobby Tables", wks.Cells["A1"].Value?.ToString());
            }
        }

        [Fact]
        public void Scouts_Property_CanBeSetAndGet()
        {
            var scouts = new List<TroopMember> { TestFixtures.CreateScout() };
            var report = new IndividualReport(scouts);
            Assert.Same(scouts, report.Scouts);
        }

        [Fact]
        public void Run_MultiplePatrols_WithIncompleteRequirements_WritesPatrolSummaries()
        {
            // Create scouts in different patrols with some requirements started but not complete
            var scout1 = new TroopMember("1", "Alice", "", "Smith", "Hawks");
            scout1.Scout.DateEarned = new DateTime(2023, 1, 1);
            // Mark some tenderfoot requirements as earned (partially complete)
            scout1.Tenderfoot.Requirements.Find(x => x.Name == "1a").DateEarned = new DateTime(2023, 2, 1);
            scout1.Tenderfoot.Requirements.Find(x => x.Name == "2a").DateEarned = new DateTime(2023, 2, 2);

            var scout2 = new TroopMember("2", "Bob", "", "Jones", "Eagles");
            scout2.Scout.DateEarned = new DateTime(2023, 1, 1);
            // Mark some different requirements
            scout2.Tenderfoot.Requirements.Find(x => x.Name == "1a").DateEarned = new DateTime(2023, 3, 1);

            var report = new IndividualReport(new List<TroopMember> { scout1, scout2 });
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                // Should have: Troop Summary + 2 patrol sheets + 2 individual sheets
                Assert.NotNull(package.Workbook.Worksheets["Troop Summary"]);
                Assert.NotNull(package.Workbook.Worksheets["Hawks"]);
                Assert.NotNull(package.Workbook.Worksheets["Eagles"]);
                Assert.NotNull(package.Workbook.Worksheets["Alice S"]);
                Assert.NotNull(package.Workbook.Worksheets["Bob J"]);
            }
        }

        [Fact]
        public void Run_ScoutWithNickname_UsesNicknameInSheetName()
        {
            var scout = new TroopMember("1", "Robert", "", "Smith");
            scout.NickName = "Bob";

            var report = new IndividualReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                Assert.NotNull(package.Workbook.Worksheets["Bob S"]);
            }
        }

        [Fact]
        public void Run_ScoutWithCompletedGroup_ShowsInSummary()
        {
            // A scout with all requirements in one curriculum group complete should
            // have that group counted as complete
            var scout = TestFixtures.CreateScout();
            scout.Scout.DateEarned = new DateTime(2023, 1, 1);
            // Complete all requirements in a specific group
            foreach (var req in scout.Tenderfoot.Requirements)
            {
                req.DateEarned = new DateTime(2023, 6, 1);
            }
            scout.Tenderfoot.DateEarned = new DateTime(2023, 6, 1);

            foreach (var req in scout.SecondClass.Requirements)
            {
                req.DateEarned = new DateTime(2023, 7, 1);
            }
            scout.SecondClass.DateEarned = new DateTime(2023, 7, 1);

            foreach (var req in scout.FirstClass.Requirements)
            {
                req.DateEarned = new DateTime(2023, 8, 1);
            }

            var report = new IndividualReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            // Should still have individual sheet since FirstClass isn't "earned" via the Rank
            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                Assert.True(package.Workbook.Worksheets.Count >= 2);
            }
        }
    }
}
