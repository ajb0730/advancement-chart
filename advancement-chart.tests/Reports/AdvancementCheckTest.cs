using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using advancementchart.Model;
using advancementchart.Reports;
using advancement_chart.tests.Helpers;

namespace advancement_chart.tests.Reports
{
    [Collection("ReportTests")]
    public class AdvancementCheckTest
    {
        private string CaptureStdErr(Action action)
        {
            var original = Console.Error;
            using (var writer = new StringWriter())
            {
                Console.SetError(writer);
                try
                {
                    action();
                }
                finally
                {
                    Console.SetError(original);
                }
                return writer.ToString();
            }
        }

        [Fact]
        public void Validate_ReturnsStructuredIssues()
        {
            var scout = TestFixtures.CreateScout();
            scout.Eagle.DateEarned = new DateTime(2023, 1, 1);
            var issues = new AdvancementCheck(new List<TroopMember> { scout }).Validate();

            var issue = Assert.Single(issues);
            Assert.Equal(IssueSeverity.Error, issue.Severity);
            Assert.Equal(scout.DisplayName, issue.ScoutName);
            Assert.Equal("has earned Eagle but not Life", issue.Message);
            Assert.Equal($"ERROR [{scout.DisplayName}] has earned Eagle but not Life", issue.ToString());
        }

        [Fact]
        public void Validate_ValidHierarchy_NoIssues()
        {
            var scout = TestFixtures.CreateScoutWithAllRanksEarned();
            Assert.Empty(new AdvancementCheck(new List<TroopMember> { scout }).Validate());
        }

        [Fact]
        public void Validate_RankEarnedBeforePreviousRank_ReportsError()
        {
            var scout = TestFixtures.CreateScoutWithAllRanksEarned();
            scout.Life.DateEarned = new DateTime(2020, 1, 1); // before Star (2020-11-01)

            var issue = Assert.Single(new AdvancementCheck(new List<TroopMember> { scout }).Validate());
            Assert.Equal(IssueSeverity.Warning, issue.Severity);
            Assert.Contains("earned Life", issue.Message);
            Assert.Contains("before Star", issue.Message);
        }

        [Fact]
        public void Validate_RanksEarnedOnSameDay_NoDateOrderIssue()
        {
            var scout = TestFixtures.CreateScout();
            var day = new DateTime(2020, 1, 1);
            scout.Scout.DateEarned = day;
            scout.Tenderfoot.DateEarned = day.AddHours(9);
            scout.SecondClass.DateEarned = day.AddHours(1);
            scout.FirstClass.DateEarned = day;
            var issues = new AdvancementCheck(new List<TroopMember> { scout }).Validate();
            Assert.DoesNotContain(issues, i => i.Message.Contains(" before "));
            Assert.DoesNotContain(issues, i => i.Severity == IssueSeverity.Error);
        }

        [Fact]
        public void Validate_PastTheRank_NoTimeRequirementWarning()
        {
            // Life is earned although Star is not (a data gap); the "check Star #1" nudge is moot.
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.Life.DateEarned = new DateTime(2022, 1, 1);
            var issues = new AdvancementCheck(new List<TroopMember> { scout }).Validate();
            Assert.Contains(issues, i => i.Message == "has earned Life but not Star");
            Assert.DoesNotContain(issues, i => i.Message.Contains("check Star #1"));
        }

        [Fact]
        public void Validate_OverdueTimeRequirement_IsWarning()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            var issue = Assert.Single(new AdvancementCheck(new List<TroopMember> { scout }).Validate());
            Assert.Equal(IssueSeverity.Warning, issue.Severity);
            Assert.StartsWith("WARN  [", issue.ToString());
            Assert.Contains("check Star #1", issue.Message);
        }

        [Fact]
        public void Validate_SkipsInactiveScouts()
        {
            var scout = TestFixtures.CreateScout();
            scout.Eagle.DateEarned = new DateTime(2023, 1, 1);
            scout.Patrol = "Inactive";
            Assert.Empty(new AdvancementCheck(new List<TroopMember> { scout }).Validate());
        }

        [Fact]
        public void Run_NoScouts_NoErrors()
        {
            var report = new AdvancementCheck(new List<TroopMember>());
            var output = CaptureStdErr(() => report.Run(""));
            Assert.DoesNotContain("ERROR", output);
        }

        [Fact]
        public void Run_ValidHierarchy_NoErrors()
        {
            var scout = TestFixtures.CreateScoutWithAllRanksEarned();
            var report = new AdvancementCheck(new List<TroopMember> { scout });
            var output = CaptureStdErr(() => report.Run(""));
            Assert.DoesNotContain("ERROR", output);
        }

        [Fact]
        public void Run_StarRequirementOneUnearned_WarnsAfterFourMonths()
        {
            var scout = TestFixtures.CreateScout();
            scout.FirstClass.DateEarned = DateTime.Now.AddYears(-2);
            var report = new AdvancementCheck(new List<TroopMember> { scout });
            var output = CaptureStdErr(() => report.Run(""));
            Assert.Contains("check Star #1", output);
        }

        [Fact]
        public void Run_MissingRequirement_DoesNotThrow()
        {
            var scout = TestFixtures.CreateScout();
            scout.FirstClass.DateEarned = DateTime.Now.AddYears(-2);
            scout.Star.Requirements.RemoveAll(r => r.Name == "1");
            var report = new AdvancementCheck(new List<TroopMember> { scout });
            var output = CaptureStdErr(() => report.Run(""));
            Assert.DoesNotContain("check Star #1", output);
        }

        [Fact]
        public void Run_EagleWithoutLife_ReportsError()
        {
            var scout = TestFixtures.CreateScout();
            scout.Eagle.DateEarned = new DateTime(2023, 1, 1);
            var report = new AdvancementCheck(new List<TroopMember> { scout });
            var output = CaptureStdErr(() => report.Run(""));
            Assert.Contains("has earned Eagle but not Life", output);
        }

        [Fact]
        public void Run_LifeWithoutStar_ReportsError()
        {
            var scout = TestFixtures.CreateScout();
            scout.Life.DateEarned = new DateTime(2023, 1, 1);
            var report = new AdvancementCheck(new List<TroopMember> { scout });
            var output = CaptureStdErr(() => report.Run(""));
            Assert.Contains("has earned Life but not Star", output);
        }

        [Fact]
        public void Run_StarWithoutFirstClass_ReportsError()
        {
            var scout = TestFixtures.CreateScout();
            scout.Star.DateEarned = new DateTime(2023, 1, 1);
            var report = new AdvancementCheck(new List<TroopMember> { scout });
            var output = CaptureStdErr(() => report.Run(""));
            Assert.Contains("has earned Star but not First Class", output);
        }

        [Fact]
        public void Run_FirstClassWithoutSecondClass_ReportsError()
        {
            var scout = TestFixtures.CreateScout();
            scout.FirstClass.DateEarned = new DateTime(2023, 1, 1);
            var report = new AdvancementCheck(new List<TroopMember> { scout });
            var output = CaptureStdErr(() => report.Run(""));
            Assert.Contains("has earned First Class but not Second Class", output);
        }

        [Fact]
        public void Run_SecondClassWithoutTenderfoot_ReportsError()
        {
            var scout = TestFixtures.CreateScout();
            scout.SecondClass.DateEarned = new DateTime(2023, 1, 1);
            var report = new AdvancementCheck(new List<TroopMember> { scout });
            var output = CaptureStdErr(() => report.Run(""));
            Assert.Contains("has earned Second Class but not Tenderfoot", output);
        }

        [Fact]
        public void Run_TenderfootWithoutScout_ReportsError()
        {
            var scout = TestFixtures.CreateScout();
            scout.Tenderfoot.DateEarned = new DateTime(2023, 1, 1);
            var report = new AdvancementCheck(new List<TroopMember> { scout });
            var output = CaptureStdErr(() => report.Run(""));
            Assert.Contains("has earned Tenderfoot but not Scout", output);
        }

        [Fact]
        public void Run_FirstClass1aWithoutSecondClass1a_ReportsError()
        {
            var scout = TestFixtures.CreateScout();
            scout.Scout.DateEarned = new DateTime(2020, 1, 1);
            scout.Tenderfoot.DateEarned = new DateTime(2020, 3, 1);
            scout.SecondClass.DateEarned = new DateTime(2020, 5, 1);
            scout.FirstClass.DateEarned = new DateTime(2020, 7, 1);
            // Mark FC 1a earned but not 2C 1a
            scout.FirstClass.Requirements.Find(x => x.Name == "1a").DateEarned = new DateTime(2020, 6, 1);
            // Note: SecondClass 1a is not set as earned directly, but SecondClass IS earned
            // So 2C.1a.Earned => rank.Earned || DateEarned.HasValue, and since SecondClass.Earned is true, it will be true
            // This test won't actually fire the error in the current code because SecondClass is earned
            // Let's test with SecondClass NOT earned
            var scout2 = TestFixtures.CreateScout();
            scout2.FirstClass.Requirements.Find(x => x.Name == "1a").DateEarned = new DateTime(2020, 6, 1);

            var report = new AdvancementCheck(new List<TroopMember> { scout2 });
            var output = CaptureStdErr(() => report.Run(""));
            Assert.Contains("has earned First Class #1a but not Second Class #1a", output);
        }

        [Fact]
        public void Run_TimeWarning_StarRequirement()
        {
            var scout = TestFixtures.CreateScout();
            scout.Scout.DateEarned = new DateTime(2020, 1, 1);
            scout.Tenderfoot.DateEarned = new DateTime(2020, 3, 1);
            scout.SecondClass.DateEarned = new DateTime(2020, 5, 1);
            // Set FirstClass earned long enough ago to trigger the warning
            scout.FirstClass.DateEarned = new DateTime(2020, 7, 1);

            var report = new AdvancementCheck(new List<TroopMember> { scout });
            var output = CaptureStdErr(() => report.Run(""));
            Assert.Contains("is 4 months since earning First Class, check Star #1", output);
        }

        [Fact]
        public void Run_TimeWarning_LifeRequirement()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.Star.DateEarned = new DateTime(2020, 1, 1);

            var report = new AdvancementCheck(new List<TroopMember> { scout });
            var output = CaptureStdErr(() => report.Run(""));
            Assert.Contains("is 6 months since earning Star, check Life #1", output);
        }

        [Fact]
        public void Run_TimeWarning_EagleRequirement()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.Star.DateEarned = new DateTime(2020, 1, 1);
            scout.Life.DateEarned = new DateTime(2020, 7, 1);

            var report = new AdvancementCheck(new List<TroopMember> { scout });
            var output = CaptureStdErr(() => report.Run(""));
            Assert.Contains("is 6 months since earning Life, check Eagle #1", output);
        }

        [Fact]
        public void Run_NoTimeWarning_WhenRequirementAlreadyEarned()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.Star.DateEarned = new DateTime(2020, 1, 1);
            scout.Star.Requirements.Find(x => x.Name == "1").DateEarned = new DateTime(2020, 5, 1);
            // Life req 1 is already signed off
            scout.Life.Requirements.Find(x => x.Name == "1").DateEarned = new DateTime(2020, 7, 1);

            var report = new AdvancementCheck(new List<TroopMember> { scout });
            var output = CaptureStdErr(() => report.Run(""));
            Assert.DoesNotContain("check Life #1", output);
        }

        [Fact]
        public void Scouts_Property_CanBeSetAndGet()
        {
            var scouts = new List<TroopMember> { TestFixtures.CreateScout() };
            var report = new AdvancementCheck(scouts);
            Assert.Same(scouts, report.Scouts);
        }
    }
}
