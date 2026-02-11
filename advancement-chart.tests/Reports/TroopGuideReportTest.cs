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
    public class TroopGuideReportTest : IDisposable
    {
        private readonly string _tempFile;

        public TroopGuideReportTest()
        {
            _tempFile = Path.Combine(Path.GetTempPath(), $"TroopGuideReport_{Guid.NewGuid()}.xlsx");
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
            var report = new TroopGuideReport(new List<TroopMember> { scout });
            report.Run(_tempFile);
            Assert.True(File.Exists(_tempFile));
        }

        [Fact]
        public void Run_HasOneWorksheet()
        {
            var scout = TestFixtures.CreateScout();
            var report = new TroopGuideReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                Assert.Single(package.Workbook.Worksheets);
                Assert.NotNull(package.Workbook.Worksheets["Troop Guide Report"]);
            }
        }

        [Fact]
        public void Run_OnlyIncludesNonFirstClassScouts()
        {
            var scoutNoRank = TestFixtures.CreateScout(id: "1", first: "NoRank", last: "Scout");
            var scoutFirstClass = TestFixtures.CreateScoutThroughFirstClass();

            var report = new TroopGuideReport(new List<TroopMember> { scoutNoRank, scoutFirstClass });
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                var wks = package.Workbook.Worksheets["Troop Guide Report"];
                bool foundNoRank = false;
                bool foundFirstClass = false;
                for (int row = 1; row <= 20; row++)
                {
                    var val = wks.Cells[row, 1].Value?.ToString();
                    if (val != null && val.Contains("NoRank"))
                        foundNoRank = true;
                    if (val != null && val.Contains("John"))
                        foundFirstClass = true;
                }
                Assert.True(foundNoRank, "Non-FirstClass scout should be in report");
                Assert.False(foundFirstClass, "FirstClass scout should NOT be in report");
            }
        }

        [Fact]
        public void Run_Title_IsTroopGuideReport()
        {
            var scout = TestFixtures.CreateScout();
            var report = new TroopGuideReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                var wks = package.Workbook.Worksheets["Troop Guide Report"];
                Assert.Equal("Troop Guide Report", wks.Cells["A1"].Value?.ToString());
            }
        }
    }
}
