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
    public class TroopCheckListTest : IDisposable
    {
        private readonly string _tempFile;

        public TroopCheckListTest()
        {
            _tempFile = Path.Combine(Path.GetTempPath(), $"TroopCheckList_{Guid.NewGuid()}.xlsx");
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
            var report = new TroopCheckList(new List<TroopMember> { scout });
            report.Run(_tempFile);
            Assert.True(File.Exists(_tempFile));
        }

        [Fact]
        public void Run_ContainsScoutName()
        {
            var scout = TestFixtures.CreateScout(first: "Alice", last: "Smith");
            var report = new TroopCheckList(new List<TroopMember> { scout });
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                var wks = package.Workbook.Worksheets["Troop Checklist"];
                bool found = false;
                for (int row = 1; row <= 20; row++)
                {
                    var val = wks.Cells[row, 1].Value?.ToString();
                    if (val != null && val.Contains("Smith"))
                    {
                        found = true;
                        break;
                    }
                }
                Assert.True(found, "Scout name should appear in the checklist");
            }
        }

        [Fact]
        public void Run_IsLandscape()
        {
            var scout = TestFixtures.CreateScout();
            var report = new TroopCheckList(new List<TroopMember> { scout });
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                var wks = package.Workbook.Worksheets["Troop Checklist"];
                Assert.Equal(eOrientation.Landscape, wks.PrinterSettings.Orientation);
            }
        }

        [Fact]
        public void Run_GroupsByPatrol()
        {
            var scout1 = TestFixtures.CreateScout(id: "1", first: "Zara", last: "Adams");
            scout1.Patrol = "Eagles";
            var scout2 = TestFixtures.CreateScout(id: "2", first: "Bob", last: "Clark");
            scout2.Patrol = "Bears";
            var scout3 = TestFixtures.CreateScout(id: "3", first: "Alice", last: "Evans");
            scout3.Patrol = "Eagles";

            var report = new TroopCheckList(new List<TroopMember> { scout1, scout2, scout3 });
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                var wks = package.Workbook.Worksheets["Troop Checklist"];

                // Collect values from column A
                var values = new List<string>();
                for (int row = 1; row <= 20; row++)
                {
                    var val = wks.Cells[row, 1].Value?.ToString();
                    if (val != null)
                        values.Add(val);
                }

                // Bears patrol should come before Eagles (alphabetical)
                int bearsIndex = values.IndexOf("Bears");
                int eaglesIndex = values.IndexOf("Eagles");
                Assert.True(bearsIndex >= 0, "Bears patrol header should exist");
                Assert.True(eaglesIndex >= 0, "Eagles patrol header should exist");
                Assert.True(bearsIndex < eaglesIndex, "Bears should appear before Eagles");

                // Bob Clark should be under Bears, Zara Adams and Alice Evans under Eagles
                int bobIndex = values.FindIndex(v => v.Contains("Bob"));
                int aliceIndex = values.FindIndex(v => v.Contains("Alice"));
                int zaraIndex = values.FindIndex(v => v.Contains("Zara"));
                Assert.True(bobIndex > bearsIndex && bobIndex < eaglesIndex, "Bob should be under Bears");
                Assert.True(aliceIndex > eaglesIndex, "Alice should be under Eagles");
                Assert.True(zaraIndex > eaglesIndex, "Zara should be under Eagles");

                // Within Eagles, Zara Adams should come before Alice Evans (alphabetical by last name)
                Assert.True(zaraIndex < aliceIndex, "Zara Adams should come before Alice Evans (alpha by last name)");
            }
        }

        [Fact]
        public void Run_HasCheckboxColumns()
        {
            var scout = TestFixtures.CreateScout();
            var report = new TroopCheckList(new List<TroopMember> { scout });
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                var wks = package.Workbook.Worksheets["Troop Checklist"];

                // Scout is on row 2 (row 1 is patrol header)
                // Column B (2) should have thin borders
                var border = wks.Cells[2, 2].Style.Border;
                Assert.Equal(OfficeOpenXml.Style.ExcelBorderStyle.Thin, border.Top.Style);
                Assert.Equal(OfficeOpenXml.Style.ExcelBorderStyle.Thin, border.Bottom.Style);
                Assert.Equal(OfficeOpenXml.Style.ExcelBorderStyle.Thin, border.Left.Style);
                Assert.Equal(OfficeOpenXml.Style.ExcelBorderStyle.Thin, border.Right.Style);

                // Column Z (26) should also have borders (verifying many checkbox columns exist)
                var farBorder = wks.Cells[2, 26].Style.Border;
                Assert.Equal(OfficeOpenXml.Style.ExcelBorderStyle.Thin, farBorder.Top.Style);
            }
        }

        [Fact]
        public void Run_EmptyScoutList()
        {
            var report = new TroopCheckList(new List<TroopMember>());
            report.Run(_tempFile);
            Assert.True(File.Exists(_tempFile));
        }
    }
}
