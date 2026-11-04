using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using OfficeOpenXml;
using advancementchart.Model;
using advancementchart.Reports;
using advancement_chart.tests.Helpers;

namespace advancement_chart.tests.Reports
{
    [Collection("ReportTests")]
    public class WorksheetNamesTest : IDisposable
    {
        private readonly string _tempFile = Path.Combine(Path.GetTempPath(), $"WorksheetNames_{Guid.NewGuid()}.xlsx");

        public void Dispose()
        {
            if (File.Exists(_tempFile))
                File.Delete(_tempFile);
        }

        [Theory]
        [InlineData("John S.", "John S.")]
        [InlineData("a:b/c\\d?e*f[g]h", "a-b-c-d-e-f-g-h")]
        [InlineData("", "Sheet")]
        [InlineData("   ", "Sheet")]
        [InlineData("'quoted'", "quoted")]
        [InlineData("History", "History (1)")]
        [InlineData("history", "history (1)")]
        public void Sanitize_ProducesValidNames(string input, string expected)
        {
            Assert.Equal(expected, WorksheetNames.Sanitize(input));
        }

        [Fact]
        public void Sanitize_TruncatesToExcelLimit()
        {
            Assert.Equal(WorksheetNames.MaxLength, WorksheetNames.Sanitize(new string('x', 80)).Length);
        }

        [Fact]
        public void Sanitize_TruncationNeverLeavesTrailingApostropheOrSpace()
        {
            // The 31st character is an apostrophe, so cutting there must not leave one behind.
            string name = new string('x', 30) + "'" + "tail";
            string result = WorksheetNames.Sanitize(name);
            Assert.DoesNotContain("'", result);
            Assert.Equal(result.Trim(), result);
        }

        [Fact]
        public void Abbreviated_OmitsInitialWhenNoLastName()
        {
            Assert.Equal("Pat", WorksheetNames.Abbreviated("Pat", ""));
            Assert.Equal("Pat S.", WorksheetNames.Abbreviated("Pat", "Smith"));
        }

        [Fact]
        public void Initial_KeepsSurrogatePairsWhole()
        {
            string emoji = char.ConvertFromUtf32(0x1F600);
            Assert.Equal(emoji, WorksheetNames.Initial(emoji + "x"));
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("  ", "")]
        [InlineData("Smith", "S")]
        public void Initial_HandlesMissingLastNames(string last, string expected)
        {
            Assert.Equal(expected, WorksheetNames.Initial(last));
        }

        [Fact]
        public void AddUnique_DuplicatesGetNumericSuffix_CaseInsensitively()
        {
            using (var package = new ExcelPackage())
            {
                var first = WorksheetNames.AddUnique(package.Workbook, "John S.");
                var second = WorksheetNames.AddUnique(package.Workbook, "john s.");
                var third = WorksheetNames.AddUnique(package.Workbook, "John S.");
                Assert.Equal("John S.", first.Name);
                Assert.Equal("john s. (2)", second.Name);
                Assert.Equal("John S. (3)", third.Name);
            }
        }

        [Fact]
        public void AddUnique_LongDuplicateStaysWithinLimit()
        {
            using (var package = new ExcelPackage())
            {
                string longName = new string('x', 40);
                WorksheetNames.AddUnique(package.Workbook, longName);
                var second = WorksheetNames.AddUnique(package.Workbook, longName);
                Assert.True(second.Name.Length <= WorksheetNames.MaxLength);
                Assert.EndsWith(" (2)", second.Name);
            }
        }

        private static TroopMember FirstClassScout(string id, string first, string last)
        {
            var scout = new TroopMember(id, first, "", last);
            scout.Scout.DateEarned = new DateTime(2020, 1, 1);
            scout.Tenderfoot.DateEarned = new DateTime(2020, 3, 1);
            scout.SecondClass.DateEarned = new DateTime(2020, 5, 1);
            scout.FirstClass.DateEarned = new DateTime(2020, 7, 1);
            scout.DateOfBirth = new DateTime(2008, 6, 1);
            return scout;
        }

        [Fact]
        public void EagleReport_SameFirstNameAndLastInitial_DoesNotThrow()
        {
            var scouts = new List<TroopMember>
            {
                FirstClassScout("1", "John", "Smith"),
                FirstClassScout("2", "John", "Stone"),
                FirstClassScout("3", "Pat", ""),
            };
            new EagleReport(scouts).Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                Assert.Equal(new[] { "John S.", "John S. (2)", "Pat" },
                    package.Workbook.Worksheets.Select(w => w.Name).OrderBy(n => n).ToArray());
            }
        }

        [Fact]
        public void IndividualReport_PatrolNamesThatCollideAfterSanitizing_GetUniqueSheets()
        {
            var a = TestFixtures.CreateScout("1", "Ann", "Able");
            var b = TestFixtures.CreateScout("2", "Bob", "Baker");
            var c = TestFixtures.CreateScout("3", "Cal", "Cook");
            a.Patrol = "A:B";
            b.Patrol = "A-B";
            c.Patrol = "History";
            new IndividualReport(new List<TroopMember> { a, b, c }).Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                var names = package.Workbook.Worksheets.Select(w => w.Name).ToList();
                Assert.Contains("A-B", names);
                Assert.Contains("A-B (2)", names);
                Assert.Contains("History (1)", names);
                Assert.Equal(names.Count, names.Select(n => n.ToLowerInvariant()).Distinct().Count());
            }
        }

        [Fact]
        public void IndividualReport_SameFirstNameAndLastInitial_DoesNotThrow()
        {
            var a = TestFixtures.CreateScout("1", "John", "Smith");
            var b = TestFixtures.CreateScout("2", "John", "Stone");
            var c = TestFixtures.CreateScout("3", "Pat", "");
            a.Patrol = b.Patrol = c.Patrol = "Hawks";
            new IndividualReport(new List<TroopMember> { a, b, c }).Run(_tempFile);

            using (var package = new ExcelPackage(new FileInfo(_tempFile)))
            {
                // Troop Summary plus one sheet per scout.
                Assert.Equal(new[] { "John S", "John S (2)", "Pat", "Troop Summary" },
                    package.Workbook.Worksheets.Select(w => w.Name).OrderBy(n => n).ToArray());
            }
        }
    }
}
