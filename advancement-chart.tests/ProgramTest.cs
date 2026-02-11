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
        public void LoadFile_NonExistentFile_ReturnsMinValue()
        {
            var result = Program.LoadFile("/nonexistent/path.csv", _scouts);
            Assert.Equal(DateTime.MinValue, result);
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
        public void LoadPatrolLookup_UnknownScout_Ignored()
        {
            string csv = "BSA Member ID,Nickname,Patrol Name,DOB\n" +
                          "999,Nobody,Hawks,2008-06-15";
            var path = CreateTempCsv(csv);

            Program.LoadPatrolLookup(path, _scouts);
            // Should not throw, just skip unknown scouts
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
    }
}
