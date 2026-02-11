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
        public void Run_IsPortrait()
        {
            var scout = TestFixtures.CreateScout();
            var report = new TroopCheckList(new List<TroopMember> { scout });
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                var wks = package.Workbook.Worksheets["Troop Checklist"];
                Assert.Equal(eOrientation.Portrait, wks.PrinterSettings.Orientation);
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

                // Row 1 is header, row 2 is patrol header, row 3 is scout
                // Column B (2) should have thin borders on scout row
                var border = wks.Cells[3, 2].Style.Border;
                Assert.Equal(OfficeOpenXml.Style.ExcelBorderStyle.Thin, border.Top.Style);
                Assert.Equal(OfficeOpenXml.Style.ExcelBorderStyle.Thin, border.Bottom.Style);
                Assert.Equal(OfficeOpenXml.Style.ExcelBorderStyle.Thin, border.Left.Style);
                Assert.Equal(OfficeOpenXml.Style.ExcelBorderStyle.Thin, border.Right.Style);

                // Column K (11) should also have borders (last checkbox column)
                var farBorder = wks.Cells[3, 11].Style.Border;
                Assert.Equal(OfficeOpenXml.Style.ExcelBorderStyle.Thin, farBorder.Top.Style);
            }
        }

        [Fact]
        public void Run_HasHeaderRow()
        {
            var scout = TestFixtures.CreateScout();
            var report = new TroopCheckList(new List<TroopMember> { scout });
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                var wks = package.Workbook.Worksheets["Troop Checklist"];

                // Header row should be 12x default height
                Assert.Equal(wks.DefaultRowHeight * 12, wks.Row(1).Height);

                // Header checkbox cells should have left, bottom, right borders but not top
                var border = wks.Cells[1, 2].Style.Border;
                Assert.Equal(OfficeOpenXml.Style.ExcelBorderStyle.Thin, border.Left.Style);
                Assert.Equal(OfficeOpenXml.Style.ExcelBorderStyle.Thin, border.Bottom.Style);
                Assert.Equal(OfficeOpenXml.Style.ExcelBorderStyle.Thin, border.Right.Style);
                Assert.Equal(OfficeOpenXml.Style.ExcelBorderStyle.None, border.Top.Style);
            }
        }

        [Fact]
        public void Run_FiltersInactivePatrol()
        {
            var active = TestFixtures.CreateScout(id: "1", first: "Alice", last: "Smith");
            active.Patrol = "Eagles";
            var inactive = TestFixtures.CreateScout(id: "2", first: "Bob", last: "Jones");
            inactive.Patrol = "Inactive";

            var report = new TroopCheckList(new List<TroopMember> { active, inactive });
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                var wks = package.Workbook.Worksheets["Troop Checklist"];
                bool foundActive = false;
                bool foundInactive = false;
                for (int row = 1; row <= 20; row++)
                {
                    var val = wks.Cells[row, 1].Value?.ToString();
                    if (val != null && val.Contains("Alice")) foundActive = true;
                    if (val != null && (val.Contains("Bob") || val.Contains("Inactive"))) foundInactive = true;
                }
                Assert.True(foundActive, "Active scout should appear");
                Assert.False(foundInactive, "Inactive scout and patrol should not appear");
            }
        }

        [Fact]
        public void Run_BlankLineBetweenPatrols()
        {
            var scout1 = TestFixtures.CreateScout(id: "1", first: "Alice", last: "Smith");
            scout1.Patrol = "Bears";
            var scout2 = TestFixtures.CreateScout(id: "2", first: "Bob", last: "Jones");
            scout2.Patrol = "Eagles";

            var report = new TroopCheckList(new List<TroopMember> { scout1, scout2 });
            report.Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                var wks = package.Workbook.Worksheets["Troop Checklist"];

                // Row 1: header, Row 2: "Bears", Row 3: Alice, Row 4: blank, Row 5: "Eagles", Row 6: Bob
                Assert.Equal("Bears", wks.Cells[2, 1].Value?.ToString());
                Assert.Contains("Alice", wks.Cells[3, 1].Value?.ToString());
                Assert.Null(wks.Cells[4, 1].Value);
                Assert.Equal("Eagles", wks.Cells[5, 1].Value?.ToString());
                Assert.Contains("Bob", wks.Cells[6, 1].Value?.ToString());
            }
        }

        [Fact]
        public void Run_EmptyScoutList()
        {
            var report = new TroopCheckList(new List<TroopMember>());
            report.Run(_tempFile);
            Assert.True(File.Exists(_tempFile));
        }

        [Fact]
        public void Run_CommentsColumnWidthVariesWithScoutCount()
        {
            // With few scouts, comments column should be wider than with many scouts
            var fewScouts = new List<TroopMember>();
            for (int i = 0; i < 3; i++)
            {
                var s = TestFixtures.CreateScout(id: i.ToString(), first: $"Scout{i}", last: $"Last{i}");
                s.Patrol = "Alpha";
                fewScouts.Add(s);
            }

            var manyScouts = new List<TroopMember>();
            for (int i = 0; i < 35; i++)
            {
                var s = TestFixtures.CreateScout(id: i.ToString(), first: $"Scout{i}", last: $"Last{i}");
                s.Patrol = "Alpha";
                manyScouts.Add(s);
            }

            var tempFile2 = Path.Combine(Path.GetTempPath(), $"TroopCheckList_{Guid.NewGuid()}.xlsx");
            try
            {
                new TroopCheckList(fewScouts).Run(_tempFile);
                new TroopCheckList(manyScouts).Run(tempFile2);

                double fewWidth, manyWidth;
                using (var pkg = new ExcelPackage(new FileInfo(_tempFile)))
                {
                    fewWidth = pkg.Workbook.Worksheets["Troop Checklist"].Column(12).Width;
                }
                using (var pkg = new ExcelPackage(new FileInfo(tempFile2)))
                {
                    manyWidth = pkg.Workbook.Worksheets["Troop Checklist"].Column(12).Width;
                }

                // Both should be at least the minimum
                Assert.True(fewWidth >= 20, $"Few-scout comments width ({fewWidth}) should be >= 20");
                Assert.True(manyWidth >= 20, $"Many-scout comments width ({manyWidth}) should be >= 20");

                // More scouts = taller content = wider target = wider comments column
                Assert.True(manyWidth > fewWidth,
                    $"Many-scout comments width ({manyWidth}) should be greater than few-scout width ({fewWidth})");
            }
            finally
            {
                if (File.Exists(tempFile2))
                    File.Delete(tempFile2);
            }
        }
    }
}
