using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using advancementchart.Model;
using advancementchart.Model.Ranks;
using advancement_chart;

namespace advancement_chart.tests
{
    [Collection("ReportTests")]
    public class ProgramTest : IDisposable
    {
        private readonly List<string> _tempFiles = new List<string>();
        private readonly List<TroopMember> _scouts = new List<TroopMember>();

        public void Dispose()
        {
            foreach (var f in _tempFiles)
            {
                if (File.Exists(f))
                    File.Delete(f);
            }
        }

        private string CreateTempCsv(string content)
        {
            var path = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.csv");
            File.WriteAllText(path, content);
            _tempFiles.Add(path);
            return path;
        }

        [Fact]
        public void LoadFile_NonExistentFile_Throws()
        {
            var ex = Assert.Throws<InputFileException>(() => Program.LoadFile("/nonexistent/path.csv", _scouts));
            Assert.Contains("/nonexistent/path.csv", ex.Message);
        }

        [Fact]
        public void ParseArgs_Defaults()
        {
            var options = Program.ParseArgs(new[] { "a.csv", "b.csv" });
            Assert.Equal(new[] { "a.csv", "b.csv" }, options.InputFiles);
            Assert.Equal("./scouts.csv", options.PatrolFile);
            Assert.False(options.PatrolFileSpecified);
            Assert.Equal(".", options.OutputDir);
        }

        [Fact]
        public void ParseArgs_OptionsInAnyPosition()
        {
            var options = Program.ParseArgs(new[] { "a.csv", "--output-dir", "out", "--patrol", "p.csv", "b.csv" });
            Assert.Equal(new[] { "a.csv", "b.csv" }, options.InputFiles);
            Assert.Equal("p.csv", options.PatrolFile);
            Assert.True(options.PatrolFileSpecified);
            Assert.Equal("out", options.OutputDir);
        }

        [Theory]
        [InlineData("--bogus")]
        [InlineData("-patrol")]
        [InlineData("-")]
        [InlineData("--patrol")]
        [InlineData("--output-dir")]
        public void ParseArgs_BadUsage_Throws(string bad)
        {
            Assert.Throws<UsageException>(() => Program.ParseArgs(new[] { "a.csv", bad }));
        }

        [Fact]
        public void ParseArgs_DoubleDashEndsOptions()
        {
            var options = Program.ParseArgs(new[] { "--output-dir", "-out", "--", "-weird.csv", "--patrol" });
            Assert.Equal("-out", options.OutputDir);
            Assert.Equal(new[] { "-weird.csv", "--patrol" }, options.InputFiles);
            Assert.False(options.PatrolFileSpecified);
        }

        [Theory]
        [InlineData("--patrol")]
        [InlineData("--output-dir")]
        public void ParseArgs_RepeatedOption_Throws(string option)
        {
            var ex = Assert.Throws<UsageException>(() =>
                Program.ParseArgs(new[] { option, "x", option, "y", "a.csv" }));
            Assert.Contains("more than once", ex.Message);
        }

        [Fact]
        public void ParseArgs_OptionValueLooksLikeOption_Throws()
        {
            var ex = Assert.Throws<UsageException>(() =>
                Program.ParseArgs(new[] { "--patrol", "--output-dir", "out", "a.csv" }));
            Assert.Contains("--patrol requires a value", ex.Message);
        }

        [Fact]
        public void ParseArgs_NoInputFiles_Throws()
        {
            Assert.Throws<UsageException>(() => Program.ParseArgs(new string[0]));
        }

        [Fact]
        public void ParseArgs_Help_NeedsNoInputFiles()
        {
            Assert.True(Program.ParseArgs(new[] { "--help" }).ShowHelp);
        }

        private int ExecuteCapturingConsole(string[] args, out string stdout, out string stderr)
        {
            var oldOut = Console.Out;
            var oldErr = Console.Error;
            var outWriter = new StringWriter();
            var errWriter = new StringWriter();
            Console.SetOut(outWriter);
            Console.SetError(errWriter);
            try
            {
                return Program.Execute(args);
            }
            finally
            {
                Console.SetOut(oldOut);
                Console.SetError(oldErr);
                stdout = outWriter.ToString();
                stderr = errWriter.ToString();
            }
        }

        [Fact]
        public void Execute_UsageError_ReturnsTwoAndPrintsUsage()
        {
            int code = ExecuteCapturingConsole(new[] { "--bogus" }, out _, out string err);
            Assert.Equal(2, code);
            Assert.Contains("Unknown option --bogus", err);
            Assert.Contains("USAGE:", err);
        }

        [Fact]
        public void Execute_Help_ReturnsZero()
        {
            int code = ExecuteCapturingConsole(new[] { "--help" }, out string stdout, out _);
            Assert.Equal(0, code);
            Assert.Contains("USAGE:", stdout);
        }

        [Fact]
        public void Execute_MissingInputFile_ReturnsOne()
        {
            int code = ExecuteCapturingConsole(new[] { "/nonexistent/typo.csv" }, out _, out string err);
            Assert.Equal(1, code);
            Assert.Contains("Input file not found", err);
        }

        [Fact]
        public void Execute_MissingExplicitPatrolFile_ReturnsOne()
        {
            var advancement = CreateTempCsv("BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                                            "123,John,,Doe,Rank,Scout,2016,2023-02-01");
            int code = ExecuteCapturingConsole(new[] { "--patrol", "/nonexistent/scouts.csv", advancement }, out _, out string err);
            Assert.Equal(1, code);
            Assert.Contains("Patrol lookup file not found", err);
        }

        [Fact]
        public void Execute_UnusableOutputDir_ReturnsOne()
        {
            var advancement = CreateTempCsv("BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                                            "123,John,,Doe,Rank,Scout,2016,2023-02-01");
            var asFile = CreateTempCsv("not a directory");
            int code = ExecuteCapturingConsole(new[] { "--output-dir", asFile, advancement }, out _, out string err);
            Assert.Equal(1, code);
            Assert.Contains("Cannot use output directory", err);
        }

        [Fact]
        public void Execute_MissingPatrolFile_FailsBeforeLoadingInputs()
        {
            var advancement = CreateTempCsv("BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                                            "123,John,,Doe,Rank,Scout,2016,2023-02-01");
            int code = ExecuteCapturingConsole(new[] { "--patrol", "/nonexistent/scouts.csv", advancement }, out string stdout, out _);
            Assert.Equal(1, code);
            Assert.DoesNotContain("Reading data from", stdout);
        }

        [Fact]
        public void Execute_DuplicateInputFile_LoadedOnce()
        {
            var advancement = CreateTempCsv("BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                                            "123,John,,Doe,Rank,Scout,2016,2023-02-01");
            var outDir = Path.Combine(Path.GetTempPath(), $"out_{Guid.NewGuid()}");
            try
            {
                int code = ExecuteCapturingConsole(new[] { "--output-dir", outDir, advancement, advancement }, out string stdout, out string err);
                Assert.Equal(0, code);
                Assert.Contains("Ignoring duplicate input file", err);
                Assert.Single(System.Text.RegularExpressions.Regex.Matches(stdout, "Reading data from"));
            }
            finally
            {
                if (Directory.Exists(outDir))
                    Directory.Delete(outDir, true);
            }
        }

        [Fact]
        public void Execute_WritesReportsToOutputDir()
        {
            var advancement = CreateTempCsv("BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                                            "123,John,,Doe,Rank,Scout,2016,2023-02-01\n" +
                                            "123,John,,Doe,Rank,Tenderfoot,2016,2023-03-01\n" +
                                            "123,John,,Doe,Rank,Second Class,2016,2023-04-01\n" +
                                            "123,John,,Doe,Rank,First Class,2016,2023-05-01");
            var patrol = CreateTempCsv("BSA Member ID,Nickname,Patrol Name,DOB\n123,Johnny,Hawks,2010-06-15");
            var outDir = Path.Combine(Path.GetTempPath(), $"out_{Guid.NewGuid()}");
            try
            {
                int code = ExecuteCapturingConsole(new[] { "--patrol", patrol, "--output-dir", outDir, advancement }, out _, out _);
                Assert.Equal(0, code);
                Assert.True(File.Exists(Path.Combine(outDir, "TroopAdvancementChart.xlsx")));
                Assert.True(File.Exists(Path.Combine(outDir, "IndividualReport.xlsx")));
                Assert.True(File.Exists(Path.Combine(outDir, "EagleReport.xlsx")));
                Assert.True(File.Exists(Path.Combine(outDir, "TroopGuideReport.xlsx")));
                Assert.True(File.Exists(Path.Combine(outDir, "TroopCheckList.xlsx")));
                Assert.True(File.Exists(Path.Combine(outDir, "AdvancementReport.docx")));
            }
            finally
            {
                if (Directory.Exists(outDir))
                    Directory.Delete(outDir, true);
            }
        }

        [Fact]
        public void LoadFile_RankScout_SetsDateEarned()
        {
            string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                          "123,John,,Doe,Rank,Scout,2016,2023-01-15";
            var path = CreateTempCsv(csv);

            var maxDate = Program.LoadFile(path, _scouts);

            Assert.Single(_scouts);
            Assert.Equal("John", _scouts[0].FirstName);
            Assert.Equal("Doe", _scouts[0].LastName);
            Assert.True(_scouts[0].Scout.Earned);
            Assert.Equal(new DateTime(2023, 1, 15), _scouts[0].Scout.DateEarned);
        }

        [Fact]
        public void LoadFile_ReturnsMaxDate()
        {
            string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                          "123,John,,Doe,Rank,Scout,2016,2023-01-15\n" +
                          "123,John,,Doe,Rank,Tenderfoot,2016,2023-06-20";
            var path = CreateTempCsv(csv);

            var maxDate = Program.LoadFile(path, _scouts);

            Assert.Equal(new DateTime(2023, 6, 20), maxDate);
        }

        [Fact]
        public void LoadFile_AllRanks()
        {
            string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                          "123,John,,Doe,Rank,Scout,2016,2020-01-01\n" +
                          "123,John,,Doe,Rank,Tenderfoot,2016,2020-03-01\n" +
                          "123,John,,Doe,Rank,Second Class,2016,2020-05-01\n" +
                          "123,John,,Doe,Rank,First Class,2016,2020-07-01\n" +
                          "123,John,,Doe,Rank,Star Scout,2016,2020-11-01\n" +
                          "123,John,,Doe,Rank,Life Scout,2016,2021-05-01\n" +
                          "123,John,,Doe,Rank,Eagle Scout,2016,2021-11-01";
            var path = CreateTempCsv(csv);

            Program.LoadFile(path, _scouts);

            var scout = _scouts[0];
            Assert.True(scout.Scout.Earned);
            Assert.True(scout.Tenderfoot.Earned);
            Assert.True(scout.SecondClass.Earned);
            Assert.True(scout.FirstClass.Earned);
            Assert.True(scout.Star.Earned);
            Assert.True(scout.Life.Earned);
            Assert.True(scout.Eagle.Earned);
        }

        [Fact]
        public void LoadFile_MeritBadge_AddsToScout()
        {
            string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                          "123,John,,Doe,Merit Badge,Camping,2016,2023-03-15";
            var path = CreateTempCsv(csv);

            Program.LoadFile(path, _scouts);

            Assert.Single(_scouts[0].MeritBadges);
            Assert.Equal("Camping", _scouts[0].MeritBadges[0].Name);
            Assert.True(_scouts[0].MeritBadges[0].Earned);
        }

        [Fact]
        public void LoadFile_MeritBadgeRequirement_AddsPartial()
        {
            string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                          "123,John,,Doe,Merit Badge Requirement,Camping #1a,2016,2023-01-15";
            var path = CreateTempCsv(csv);

            Program.LoadFile(path, _scouts);

            Assert.Single(_scouts[0].MeritBadges);
            Assert.Equal("Camping", _scouts[0].MeritBadges[0].Name);
            Assert.True(_scouts[0].MeritBadges[0].Started);
            Assert.False(_scouts[0].MeritBadges[0].Earned);
        }

        [Fact]
        public void LoadFile_RankRequirement_SetsDateOnRequirement()
        {
            string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                          "123,John,,Doe,Scout Rank Requirement,1a,2016,2023-01-10";
            var path = CreateTempCsv(csv);

            Program.LoadFile(path, _scouts);

            var req = _scouts[0].Scout.Requirements.First(r => r.Name == "1a");
            Assert.Equal(new DateTime(2023, 1, 10), req.DateEarned);
        }

        [Fact]
        public void LoadFile_MultipleRankRequirements()
        {
            string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                          "123,John,,Doe,Tenderfoot Rank Requirement,1a,2016,2023-01-10\n" +
                          "123,John,,Doe,Second Class Rank Requirement,1a,2016,2023-02-10\n" +
                          "123,John,,Doe,First Class Rank Requirement,1a,2016,2023-03-10\n" +
                          "123,John,,Doe,Star Scout Rank Requirement,1,2016,2023-04-10\n" +
                          "123,John,,Doe,Life Scout Rank Requirement,1,2016,2023-05-10\n" +
                          "123,John,,Doe,Eagle Scout Rank Requirement,1,2016,2023-06-10";
            var path = CreateTempCsv(csv);

            Program.LoadFile(path, _scouts);

            var scout = _scouts[0];
            Assert.Equal(new DateTime(2023, 1, 10), scout.Tenderfoot.Requirements.First(r => r.Name == "1a").DateEarned);
            Assert.Equal(new DateTime(2023, 2, 10), scout.SecondClass.Requirements.First(r => r.Name == "1a").DateEarned);
            Assert.Equal(new DateTime(2023, 3, 10), scout.FirstClass.Requirements.First(r => r.Name == "1a").DateEarned);
            Assert.Equal(new DateTime(2023, 4, 10), scout.Star.Requirements.First(r => r.Name == "1").DateEarned);
            Assert.Equal(new DateTime(2023, 5, 10), scout.Life.Requirements.First(r => r.Name == "1").DateEarned);
            Assert.Equal(new DateTime(2023, 6, 10), scout.Eagle.Requirements.First(r => r.Name == "1").DateEarned);
        }

        [Fact]
        public void LoadFile_Award_BronzePalm()
        {
            string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                          "123,John,,Doe,Award,Eagle Palm Pin #1 (Bronze),2016,2023-01-15";
            var path = CreateTempCsv(csv);

            Program.LoadFile(path, _scouts);

            Assert.Single(_scouts[0].EaglePalms);
            Assert.Equal(Palm.PalmType.Bronze, _scouts[0].EaglePalms[0].Type);
            Assert.True(_scouts[0].EaglePalms[0].Earned);
        }

        [Fact]
        public void LoadFile_Award_GoldPalm()
        {
            string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                          "123,John,,Doe,Award,Eagle Palm Pin #2 (Gold),2016,2023-01-15";
            var path = CreateTempCsv(csv);

            Program.LoadFile(path, _scouts);

            Assert.Single(_scouts[0].EaglePalms);
            Assert.Equal(Palm.PalmType.Gold, _scouts[0].EaglePalms[0].Type);
        }

        [Fact]
        public void LoadFile_Award_SilverPalm()
        {
            string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                          "123,John,,Doe,Award,Eagle Palm Pin #3 (Silver),2016,2023-01-15";
            var path = CreateTempCsv(csv);

            Program.LoadFile(path, _scouts);

            Assert.Single(_scouts[0].EaglePalms);
            Assert.Equal(Palm.PalmType.Silver, _scouts[0].EaglePalms[0].Type);
        }

        [Fact]
        public void LoadFile_SecondSetPalms()
        {
            string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                          "123,John,,Doe,Award,Eagle Palm Pin #4 (Bronze),2016,2023-01-15\n" +
                          "123,John,,Doe,Award,Eagle Palm Pin #5 (Gold),2016,2023-04-15\n" +
                          "123,John,,Doe,Award,Eagle Palm Pin #6 (Silver),2016,2023-07-15";
            var path = CreateTempCsv(csv);

            Program.LoadFile(path, _scouts);

            Assert.Equal(3, _scouts[0].EaglePalms.Count);
        }

        [Fact]
        public void LoadFile_DuplicateScout_DoesNotDuplicate()
        {
            string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                          "123,John,,Doe,Rank,Scout,2016,2023-01-15\n" +
                          "123,John,,Doe,Rank,Tenderfoot,2016,2023-06-15";
            var path = CreateTempCsv(csv);

            Program.LoadFile(path, _scouts);

            Assert.Single(_scouts);
        }

        [Fact]
        public void LoadFile_MultipleScouts()
        {
            string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                          "123,John,,Doe,Rank,Scout,2016,2023-01-15\n" +
                          "456,Jane,,Smith,Rank,Scout,2016,2023-02-15";
            var path = CreateTempCsv(csv);

            Program.LoadFile(path, _scouts);

            Assert.Equal(2, _scouts.Count);
        }

        [Fact]
        public void LoadPatrolLookup_SetsNicknameAndPatrol()
        {
            _scouts.Add(new TroopMember("123", "John", "", "Doe"));

            string csv = "BSA Member ID,Nickname,Patrol Name,DOB\n" +
                          "123,Johnny,Hawks,2008-06-15";
            var path = CreateTempCsv(csv);

            Program.LoadPatrolLookup(path, _scouts);

            Assert.Equal("Johnny", _scouts[0].NickName);
            Assert.Equal("Hawks", _scouts[0].Patrol);
            Assert.Equal(new DateTime(2008, 6, 15), _scouts[0].DateOfBirth);
        }

        [Fact]
        public void LoadPatrolLookup_NonExistentFile_DoesNotThrow()
        {
            Program.LoadPatrolLookup("/nonexistent/scouts.csv", _scouts);
            // Should not throw
        }

        [Fact]
        public void LoadPatrolLookup_EmptyNickname_DoesNotOverwrite()
        {
            var scout = new TroopMember("123", "John", "", "Doe");
            scout.NickName = "OriginalNick";
            _scouts.Add(scout);

            string csv = "BSA Member ID,Nickname,Patrol Name,DOB\n" +
                          "123,,Hawks,2008-06-15";
            var path = CreateTempCsv(csv);

            Program.LoadPatrolLookup(path, _scouts);

            Assert.Equal("OriginalNick", _scouts[0].NickName);
        }

        [Fact]
        public void LoadPatrolLookup_EmptyPatrol_DoesNotOverwrite()
        {
            var scout = new TroopMember("123", "John", "", "Doe", "OriginalPatrol");
            _scouts.Add(scout);

            string csv = "BSA Member ID,Nickname,Patrol Name,DOB\n" +
                          "123,Johnny,,2008-06-15";
            var path = CreateTempCsv(csv);

            Program.LoadPatrolLookup(path, _scouts);

            Assert.Equal("OriginalPatrol", _scouts[0].Patrol);
        }

        [Fact]
        public void LoadPatrolLookup_RosterScoutNotInAdvancement_IsSeeded()
        {
            // A scout on the roster but with no advancement records must still be
            // added (seeded), otherwise they would never appear in any report.
            string csv = "BSA Member ID,First Name,Last Name,Nickname,Patrol Name,DOB\n" +
                          "999,Diana,Wilson,,Hawks,2010-06-15";
            var path = CreateTempCsv(csv);

            Program.LoadPatrolLookup(path, _scouts);

            Assert.Single(_scouts);
            Assert.Equal("999", _scouts[0].BsaMemberId);
            Assert.Equal("Diana", _scouts[0].FirstName);
            Assert.Equal("Wilson", _scouts[0].LastName);
            Assert.Equal("Hawks", _scouts[0].Patrol);
            Assert.Equal(new DateTime(2010, 6, 15), _scouts[0].DateOfBirth);
        }

        [Fact]
        public void LoadPatrolLookup_SeededScout_WithoutPatrol_DefaultsToUnassigned()
        {
            // A seeded scout with no patrol on the roster keeps the constructor's
            // "Unassigned" default rather than an empty patrol.
            string csv = "BSA Member ID,First Name,Last Name,Nickname,Patrol Name,DOB\n" +
                          "999,Diana,Wilson,,,";
            var path = CreateTempCsv(csv);

            Program.LoadPatrolLookup(path, _scouts);

            Assert.Single(_scouts);
            Assert.Equal("Unassigned", _scouts[0].Patrol);
        }

        [Fact]
        public void LoadPatrolLookup_BlankMemberId_IsSkipped()
        {
            // Placeholder/blank roster rows (no member ID) must not create scouts.
            string csv = "BSA Member ID,First Name,Last Name,Nickname,Patrol Name,DOB\n" +
                          ",,,,,";
            var path = CreateTempCsv(csv);

            Program.LoadPatrolLookup(path, _scouts);

            Assert.Empty(_scouts);
        }

        [Fact]
        public void LoadPatrolLookup_ExistingScout_NotDuplicatedBySeeding()
        {
            // A scout already loaded from advancement must be enriched, not duplicated,
            // when the same member ID appears on the roster.
            _scouts.Add(new TroopMember("123", "John", "", "Doe"));

            string csv = "BSA Member ID,First Name,Last Name,Nickname,Patrol Name,DOB\n" +
                          "123,John,Doe,Johnny,Hawks,2008-06-15";
            var path = CreateTempCsv(csv);

            Program.LoadPatrolLookup(path, _scouts);

            Assert.Single(_scouts);
            Assert.Equal("Johnny", _scouts[0].NickName);
            Assert.Equal("Hawks", _scouts[0].Patrol);
        }

        [Fact]
        public void LoadPatrolLookup_InvalidDob_DoesNotSet()
        {
            _scouts.Add(new TroopMember("123", "John", "", "Doe"));

            string csv = "BSA Member ID,Nickname,Patrol Name,DOB\n" +
                          "123,Johnny,Hawks,not-a-date";
            var path = CreateTempCsv(csv);

            Program.LoadPatrolLookup(path, _scouts);

            Assert.Equal(default(DateTime), _scouts[0].DateOfBirth);
        }

        [Fact]
        public void LoadFile_AwardRequirement_SetsPalmRequirement()
        {
            // First need to add a palm
            string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                          "123,John,,Doe,Award,Eagle Palm Pin #1 (Bronze),2016,2023-01-15\n" +
                          "123,John,,Doe,Award Requirement,Eagle Palm Pin #1 (Bronze) #1,2016,2023-01-10";
            var path = CreateTempCsv(csv);

            Program.LoadFile(path, _scouts);

            var palm = _scouts[0].EaglePalms.First(p => p.Type == Palm.PalmType.Bronze);
            var req = palm.Requirements.First(r => r.Name == "1");
            Assert.Equal(new DateTime(2023, 1, 10), req.DateEarned);
        }

        [Fact]
        public void LoadFile_MalformedMeritBadgeRequirement_LogsWarningAndContinues()
        {
            // "Camping" has no "#" separator, which will cause ArgumentOutOfRangeException from Substring
            string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                          "123,John,,Doe,Merit Badge Requirement,Camping,2016,2023-01-15\n" +
                          "123,John,,Doe,Rank,Scout,2016,2023-02-01";
            var path = CreateTempCsv(csv);

            var oldErr = Console.Error;
            var errWriter = new StringWriter();
            Console.SetError(errWriter);
            try
            {
                Program.LoadFile(path, _scouts);
            }
            finally
            {
                Console.SetError(oldErr);
            }

            // The malformed row should be skipped but Scout rank should still be loaded
            Assert.Single(_scouts);
            Assert.True(_scouts[0].Scout.Earned);
            Assert.Contains("skipped", errWriter.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void LoadFile_ValidData_NoWarnings()
        {
            string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                          "123,John,,Doe,Rank,Scout,2016,2023-01-15";
            var path = CreateTempCsv(csv);

            var oldErr = Console.Error;
            var errWriter = new StringWriter();
            Console.SetError(errWriter);
            try
            {
                Program.LoadFile(path, _scouts);
            }
            finally
            {
                Console.SetError(oldErr);
            }

            Assert.Empty(errWriter.ToString());
        }

        [Fact]
        public void LoadPatrolLookup_FutureDob_WarnsAndIgnores()
        {
            _scouts.Add(new TroopMember("123", "John", "", "Doe"));

            string csv = "BSA Member ID,Nickname,Patrol Name,DOB\n" +
                          "123,Johnny,Hawks,2030-01-01";
            var path = CreateTempCsv(csv);

            var oldErr = Console.Error;
            var errWriter = new StringWriter();
            Console.SetError(errWriter);
            try
            {
                Program.LoadPatrolLookup(path, _scouts);
            }
            finally
            {
                Console.SetError(oldErr);
            }

            Assert.Equal(default(DateTime), _scouts[0].DateOfBirth);
            Assert.Contains("Suspicious DOB", errWriter.ToString());
        }

        [Fact]
        public void LoadPatrolLookup_AncientDob_WarnsAndIgnores()
        {
            _scouts.Add(new TroopMember("123", "John", "", "Doe"));

            string csv = "BSA Member ID,Nickname,Patrol Name,DOB\n" +
                          "123,Johnny,Hawks,1980-01-01";
            var path = CreateTempCsv(csv);

            var oldErr = Console.Error;
            var errWriter = new StringWriter();
            Console.SetError(errWriter);
            try
            {
                Program.LoadPatrolLookup(path, _scouts);
            }
            finally
            {
                Console.SetError(oldErr);
            }

            Assert.Equal(default(DateTime), _scouts[0].DateOfBirth);
            Assert.Contains("Suspicious DOB", errWriter.ToString());
        }

        [Fact]
        public void LoadFile_MalformedPalmRequirement_LogsWarningAndContinues()
        {
            string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                          "123,John,,Doe,Award Requirement,Eagle Palm Pin,2016,2023-01-15\n" +
                          "123,John,,Doe,Rank,Scout,2016,2023-02-01";
            var path = CreateTempCsv(csv);

            var oldErr = Console.Error;
            var errWriter = new StringWriter();
            Console.SetError(errWriter);
            try
            {
                Program.LoadFile(path, _scouts);
            }
            finally
            {
                Console.SetError(oldErr);
            }

            Assert.True(_scouts[0].Scout.Earned);
            Assert.Contains("malformed palm requirement", errWriter.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void LoadFile_LeadingHashMeritBadgeRequirement_LogsWarningAndContinues()
        {
            string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                          "123,John,,Doe,Merit Badge Requirement,#1a,2016,2023-01-15";
            var path = CreateTempCsv(csv);

            var oldErr = Console.Error;
            var errWriter = new StringWriter();
            Console.SetError(errWriter);
            try
            {
                Program.LoadFile(path, _scouts);
            }
            finally
            {
                Console.SetError(oldErr);
            }

            Assert.Contains("malformed merit badge requirement", errWriter.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void LoadFile_MissingRequiredColumn_Throws()
        {
            string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version\n" +
                          "123,John,,Doe,Rank,Scout,2016";
            var path = CreateTempCsv(csv);

            var ex = Assert.Throws<InputFileException>(() => Program.LoadFile(path, _scouts));
            Assert.Contains("Date Completed", ex.Message);
            Assert.Contains(path, ex.Message);
        }

        [Fact]
        public void LoadFile_MultipleMissingColumns_ListsAll()
        {
            var path = CreateTempCsv("BSA Member ID,First Name\n123,John");

            var ex = Assert.Throws<InputFileException>(() => Program.LoadFile(path, _scouts));
            Assert.Contains("'Last Name'", ex.Message);
            Assert.Contains("'Date Completed'", ex.Message);
            Assert.DoesNotContain("'First Name'", ex.Message);
        }

        [Fact]
        public void LoadFile_EmptyFile_ThrowsNoHeader()
        {
            var path = CreateTempCsv("");

            var ex = Assert.Throws<InputFileException>(() => Program.LoadFile(path, _scouts));
            Assert.Contains("No header row", ex.Message);
        }

        [Fact]
        public void Execute_BadInputFile_ReturnsOneAndReportsError()
        {
            var path = CreateTempCsv("BSA Member ID,First Name\n123,John");

            var oldErr = Console.Error;
            var errWriter = new StringWriter();
            Console.SetError(errWriter);
            int exitCode;
            try
            {
                exitCode = Program.Execute(new[] { path });
            }
            finally
            {
                Console.SetError(oldErr);
            }

            Assert.Equal(1, exitCode);
            Assert.Contains("ERROR: Required column(s)", errWriter.ToString());
        }

        [Fact]
        public void LoadPatrolLookup_MissingRequiredColumn_Throws()
        {
            string csv = "BSA Member ID,Nickname,Patrol Name\n" +
                          "123,Johnny,Hawks";
            var path = CreateTempCsv(csv);

            var ex = Assert.Throws<InputFileException>(() => Program.LoadPatrolLookup(path, _scouts));
            Assert.Contains("DOB", ex.Message);
        }

        [Fact]
        public void LoadPatrolLookup_UnparseableDob_WarnsAndIgnores()
        {
            _scouts.Add(new TroopMember("123", "John", "", "Doe"));

            string csv = "BSA Member ID,Nickname,Patrol Name,DOB\n" +
                          "123,Johnny,Hawks,not-a-date";
            var path = CreateTempCsv(csv);

            var oldErr = Console.Error;
            var errWriter = new StringWriter();
            Console.SetError(errWriter);
            try
            {
                Program.LoadPatrolLookup(path, _scouts);
            }
            finally
            {
                Console.SetError(oldErr);
            }

            Assert.Equal(default(DateTime), _scouts[0].DateOfBirth);
            Assert.Contains("Unparseable DOB", errWriter.ToString());
        }

        [Fact]
        public void LoadFile_BlankAdvancementOnRequirementRow_LogsWarningAndContinues()
        {
            string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                          "123,John,,Doe,Merit Badge Requirement,,2016,2023-01-15\n" +
                          "123,John,,Doe,Award Requirement,,2016,2023-01-15\n" +
                          "123,John,,Doe,Rank,Scout,2016,2023-02-01";
            var path = CreateTempCsv(csv);

            var oldErr = Console.Error;
            var errWriter = new StringWriter();
            Console.SetError(errWriter);
            try
            {
                Program.LoadFile(path, _scouts);
            }
            finally
            {
                Console.SetError(oldErr);
            }

            Assert.True(_scouts[0].Scout.Earned);
            Assert.Contains("malformed", errWriter.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void LoadFile_MeritBadgeRowWithNoName_LogsWarningAndAddsNothing()
        {
            string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                          "123,John,,Doe,Merit Badge,,2016,2023-01-15";
            var path = CreateTempCsv(csv);

            var oldErr = Console.Error;
            var errWriter = new StringWriter();
            Console.SetError(errWriter);
            try
            {
                Program.LoadFile(path, _scouts);
            }
            finally
            {
                Console.SetError(oldErr);
            }

            Assert.Empty(_scouts[0].MeritBadges);
            Assert.Contains("no badge name", errWriter.ToString());
        }

        [Fact]
        public void LoadPatrolLookup_ValidDob_NoWarning()
        {
            _scouts.Add(new TroopMember("123", "John", "", "Doe"));

            string csv = "BSA Member ID,Nickname,Patrol Name,DOB\n" +
                          "123,Johnny,Hawks,2010-06-15";
            var path = CreateTempCsv(csv);

            var oldErr = Console.Error;
            var errWriter = new StringWriter();
            Console.SetError(errWriter);
            try
            {
                Program.LoadPatrolLookup(path, _scouts);
            }
            finally
            {
                Console.SetError(oldErr);
            }

            Assert.Equal(new DateTime(2010, 6, 15), _scouts[0].DateOfBirth);
            Assert.DoesNotContain("Suspicious", errWriter.ToString());
        }

        // ---------------------------------------------------------------------
        // New Scoutbook export format (2026): rank names carry a " Rank" suffix,
        // merit badges a " MB" suffix, types are pluralized ("Merit Badges",
        // "Awards", "... Rank Requirements"), the merit-badge / rank-requirement
        // name moves into the type column, commas inside the Advancement column
        // are rendered as semicolons, and dates are MM/DD/YYYY.
        // ---------------------------------------------------------------------

        [Fact]
        public void LoadFile_NewFormat_RankWithSuffix_SetsDateEarned()
        {
            // New exports label the rank "Scout Rank" rather than "Scout".
            string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                          "123,John,,Doe,Rank,Scout Rank,2022,2023-01-15";
            var path = CreateTempCsv(csv);

            Program.LoadFile(path, _scouts);

            Assert.True(_scouts[0].Scout.Earned);
            Assert.Equal(new DateTime(2023, 1, 15), _scouts[0].Scout.DateEarned);
        }

        [Fact]
        public void LoadFile_NewFormat_AllRanksWithSuffix()
        {
            // Every Scouts BSA rank should resolve when suffixed with " Rank".
            string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                          "123,John,,Doe,Rank,Scout Rank,2022,2020-01-01\n" +
                          "123,John,,Doe,Rank,Tenderfoot Rank,2022,2020-03-01\n" +
                          "123,John,,Doe,Rank,Second Class Rank,2022,2020-05-01\n" +
                          "123,John,,Doe,Rank,First Class Rank,2022,2020-07-01\n" +
                          "123,John,,Doe,Rank,Star Scout Rank,2022,2020-11-01\n" +
                          "123,John,,Doe,Rank,Life Scout Rank,2022,2021-05-01\n" +
                          "123,John,,Doe,Rank,Eagle Scout Rank,2022,2021-11-01";
            var path = CreateTempCsv(csv);

            Program.LoadFile(path, _scouts);

            var scout = _scouts[0];
            Assert.True(scout.Scout.Earned);
            Assert.True(scout.Tenderfoot.Earned);
            Assert.True(scout.SecondClass.Earned);
            Assert.True(scout.FirstClass.Earned);
            Assert.True(scout.Star.Earned);
            Assert.True(scout.Life.Earned);
            Assert.True(scout.Eagle.Earned);
        }

        [Fact]
        public void LoadFile_NewFormat_CubRank_IsIgnored()
        {
            // Cub Scout ranks (e.g. Webelos) appear in the export but are not tracked;
            // the row must be ignored without setting any Scouts BSA rank or throwing.
            string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                          "123,John,,Doe,Rank,Webelos Rank,2022,2019-06-01\n" +
                          "123,John,,Doe,Rank,Scout Rank,2022,2020-01-01";
            var path = CreateTempCsv(csv);

            Program.LoadFile(path, _scouts);

            Assert.Single(_scouts);
            Assert.True(_scouts[0].Scout.Earned);
            Assert.False(_scouts[0].Tenderfoot.Earned);
        }

        [Fact]
        public void LoadFile_NewFormat_MeritBadgesWithSuffix_AddsBadge()
        {
            // "Merit Badges" (plural) type with a " MB"-suffixed name.
            string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                          "123,John,,Doe,Merit Badges,Camping MB,2014,2023-03-15";
            var path = CreateTempCsv(csv);

            Program.LoadFile(path, _scouts);

            Assert.Single(_scouts[0].MeritBadges);
            Assert.Equal("Camping", _scouts[0].MeritBadges[0].Name);
            Assert.True(_scouts[0].MeritBadges[0].Earned);
        }

        [Fact]
        public void LoadFile_NewFormat_MeritBadgeNameWithSemicolons_RestoresCommas()
        {
            // The export renders commas in the Advancement column as semicolons, so
            // "Signs, Signals, and Codes" arrives as "Signs; Signals; and Codes MB"
            // and must be restored to match the model's badge name.
            string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                          "123,John,,Doe,Merit Badges,Signs; Signals; and Codes MB,2014,2023-03-15";
            var path = CreateTempCsv(csv);

            Program.LoadFile(path, _scouts);

            Assert.Single(_scouts[0].MeritBadges);
            Assert.Equal("Signs, Signals, and Codes", _scouts[0].MeritBadges[0].Name);
        }

        [Fact]
        public void LoadFile_NewFormat_RankRequirements_SetsDateOnRequirement()
        {
            // New format: rank name is in the (pluralized) type column, requirement
            // number in the Advancement column.
            string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                          "123,John,,Doe,First Class Rank Requirements,1a,2022,2023-03-10";
            var path = CreateTempCsv(csv);

            Program.LoadFile(path, _scouts);

            Assert.Equal(new DateTime(2023, 3, 10), _scouts[0].FirstClass.Requirements.First(r => r.Name == "1a").DateEarned);
        }

        [Fact]
        public void LoadFile_NewFormat_MeritBadgeRequirements_AddsPartial()
        {
            // New format: the badge name is in the type column ("Camping Merit Badge
            // Requirements"); the Advancement column holds only the requirement number.
            string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                          "123,John,,Doe,Camping Merit Badge Requirements,1a,2022,2023-01-15";
            var path = CreateTempCsv(csv);

            Program.LoadFile(path, _scouts);

            Assert.Single(_scouts[0].MeritBadges);
            Assert.Equal("Camping", _scouts[0].MeritBadges[0].Name);
            Assert.True(_scouts[0].MeritBadges[0].Started);
            Assert.False(_scouts[0].MeritBadges[0].Earned);
        }

        [Fact]
        public void LoadFile_NewFormat_AwardsWithSuffix_BronzePalm()
        {
            // "Awards" (plural) type still carries the palm name unchanged.
            string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                          "123,John,,Doe,Awards,Eagle Palm Pin #1 (Bronze),2016,2023-01-15";
            var path = CreateTempCsv(csv);

            Program.LoadFile(path, _scouts);

            Assert.Single(_scouts[0].EaglePalms);
            Assert.Equal(Palm.PalmType.Bronze, _scouts[0].EaglePalms[0].Type);
            Assert.True(_scouts[0].EaglePalms[0].Earned);
        }

        [Fact]
        public void LoadFile_NewFormat_AwardRequirementsText_IsIgnoredGracefully()
        {
            // New award-requirement rows carry descriptive text (not a number) and no
            // earned palm exists to attach to; the row must be skipped without warnings.
            string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                          "123,John,,Doe,Totin' Chip Award Requirements,Subscribe to the Outdoor Code.,2016,2023-01-15\n" +
                          "123,John,,Doe,Rank,Scout Rank,2022,2023-02-01";
            var path = CreateTempCsv(csv);

            var oldErr = Console.Error;
            var errWriter = new StringWriter();
            Console.SetError(errWriter);
            try
            {
                Program.LoadFile(path, _scouts);
            }
            finally
            {
                Console.SetError(oldErr);
            }

            Assert.Single(_scouts);
            Assert.True(_scouts[0].Scout.Earned);
            Assert.Empty(errWriter.ToString());
        }

        [Fact]
        public void LoadFile_NewFormat_UsDateFormat_Parses()
        {
            // New exports use MM/DD/YYYY dates; verify they parse under the en-US
            // culture the tool runs in.
            var originalCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            try
            {
                string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                              "123,John,,Doe,Rank,Scout Rank,2022,03/31/2023";
                var path = CreateTempCsv(csv);

                Program.LoadFile(path, _scouts);

                Assert.Equal(new DateTime(2023, 3, 31), _scouts[0].Scout.DateEarned);
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = originalCulture;
            }
        }

        [Fact]
        public void LoadFile_EmptyDateCompleted_SkipsRowWithoutCrashing()
        {
            // Rows without a completion date must be skipped rather than throwing.
            string csv = "BSA Member ID,First Name,Middle Name,Last Name,Advancement Type,Advancement,Version,Date Completed\n" +
                          "123,John,,Doe,First Class Rank Requirements,1a,2022,\n" +
                          "123,John,,Doe,Rank,Scout Rank,2022,2023-02-01";
            var path = CreateTempCsv(csv);

            Program.LoadFile(path, _scouts);

            Assert.Single(_scouts);
            Assert.True(_scouts[0].Scout.Earned);
            // The dateless requirement row should not have been applied.
            Assert.Null(_scouts[0].FirstClass.Requirements.First(r => r.Name == "1a").DateEarned);
        }

        [Fact]
        public void StripSuffix_RemovesSuffixAndTrims()
        {
            Assert.Equal("Scout", Program.StripSuffix("Scout Rank", " Rank"));
            Assert.Equal("Camping", Program.StripSuffix("Camping MB", " MB"));
            // Absent suffix returns the trimmed original.
            Assert.Equal("Scout", Program.StripSuffix("Scout", " Rank"));
        }

        [Fact]
        public void GetRankByName_MapsNamesAndIgnoresUnknown()
        {
            var scout = new TroopMember("123", "John", "", "Doe");
            Assert.Same(scout.FirstClass, Program.GetRankByName(scout, "First Class"));
            Assert.Same(scout.Eagle, Program.GetRankByName(scout, "Eagle Scout"));
            // Cub ranks and unknown names return null.
            Assert.Null(Program.GetRankByName(scout, "Webelos"));
        }
    }
}
