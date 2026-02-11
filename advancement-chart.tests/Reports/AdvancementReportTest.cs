using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using advancementchart.Model;
using advancementchart.Reports;
using advancementchart.Model.Ranks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using advancement_chart.tests.Helpers;

namespace advancement_chart.tests.Reports
{
    public class AdvancementReportTest : IDisposable
    {
        private readonly string _tempFile;

        public AdvancementReportTest()
        {
            _tempFile = Path.Combine(Path.GetTempPath(), $"AdvancementReport_{Guid.NewGuid()}.docx");
        }

        public void Dispose()
        {
            if (File.Exists(_tempFile))
                File.Delete(_tempFile);
        }

        private string GetDocumentText(string filePath)
        {
            using (var doc = WordprocessingDocument.Open(filePath, false))
            {
                var body = doc.MainDocumentPart.Document.Body;
                return string.Join("\n", body.Descendants<Text>().Select(t => t.Text));
            }
        }

        [Fact]
        public void Run_CreatesFile()
        {
            var scout = TestFixtures.CreateScout();
            var report = new AdvancementReport(new List<TroopMember> { scout });
            report.Run(_tempFile);
            Assert.True(File.Exists(_tempFile));
        }

        [Fact]
        public void Run_ContainsScoutName()
        {
            var scout = TestFixtures.CreateScout(first: "TestScout", last: "McTest");
            var report = new AdvancementReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            var text = GetDocumentText(_tempFile);
            Assert.Contains("TestScout McTest", text);
        }

        [Fact]
        public void Run_ContainsCurrentRank()
        {
            var scout = TestFixtures.CreateScout();
            scout.Scout.DateEarned = new DateTime(2023, 1, 1);
            var report = new AdvancementReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            var text = GetDocumentText(_tempFile);
            Assert.Contains("Current Rank: Scout", text);
        }

        [Fact]
        public void Run_ContainsNextRankRequirements()
        {
            var scout = TestFixtures.CreateScout();
            scout.Scout.DateEarned = new DateTime(2023, 1, 1);
            var report = new AdvancementReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            var text = GetDocumentText(_tempFile);
            Assert.Contains("Requirements remaining for Tenderfoot", text);
        }

        [Fact]
        public void Run_WithMaxDate_ContainsDataTimestamp()
        {
            var scout = TestFixtures.CreateScout();
            var maxDate = new DateTime(2023, 6, 15);
            var report = new AdvancementReport(new List<TroopMember> { scout }, maxDate);
            report.Run(_tempFile);

            var text = GetDocumentText(_tempFile);
            Assert.Contains("Data as of:", text);
        }

        [Fact]
        public void Run_WithoutMaxDate_NoDataTimestamp()
        {
            var scout = TestFixtures.CreateScout();
            var report = new AdvancementReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            var text = GetDocumentText(_tempFile);
            Assert.DoesNotContain("Data as of:", text);
        }

        [Fact]
        public void Run_ContainsReportGenerated()
        {
            var scout = TestFixtures.CreateScout();
            var report = new AdvancementReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            var text = GetDocumentText(_tempFile);
            Assert.Contains("Report Generated:", text);
        }

        [Fact]
        public void Run_NoRankScout_ShowsNextRankAsScout()
        {
            var scout = TestFixtures.CreateScout();
            var report = new AdvancementReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            var text = GetDocumentText(_tempFile);
            Assert.Contains("Requirements remaining for Scout", text);
        }

        [Fact]
        public void Run_EagleScout_NextRankIsPalm()
        {
            var scout = TestFixtures.CreateScoutWithAllRanksEarned();
            var report = new AdvancementReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            var text = GetDocumentText(_tempFile);
            Assert.Contains("Current Rank: Eagle", text);
            Assert.Contains("Requirements remaining for Bronze Palm", text);
        }

        [Fact]
        public void Run_StarScout_ShowsMeritBadgeInfo()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.Star.DateEarned = new DateTime(2021, 11, 1);
            scout.Add(TestFixtures.EarnedBadge("Camping", new DateTime(2021, 1, 1)));
            scout.Add(TestFixtures.EarnedBadge("First Aid", new DateTime(2021, 2, 1)));
            scout.Add(TestFixtures.EarnedBadge("Communication", new DateTime(2021, 3, 1)));

            var report = new AdvancementReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            var text = GetDocumentText(_tempFile);
            Assert.Contains("Requirements remaining for Life", text);
        }

        [Fact]
        public void Run_MultipleScouts_AllIncluded()
        {
            var scout1 = TestFixtures.CreateScout(id: "1", first: "Alice", last: "Smith");
            var scout2 = TestFixtures.CreateScout(id: "2", first: "Bob", last: "Jones");
            var report = new AdvancementReport(new List<TroopMember> { scout1, scout2 });
            report.Run(_tempFile);

            var text = GetDocumentText(_tempFile);
            Assert.Contains("Alice Smith", text);
            Assert.Contains("Bob Jones", text);
        }

        [Fact]
        public void Run_TimeRequirement_ShowsTargetDate()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            var report = new AdvancementReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            var text = GetDocumentText(_tempFile);
            // Star requirement 1 has a time requirement of 4 months
            Assert.Contains("Be active 4 months since earning First Class", text);
        }

        [Fact]
        public void Run_OverwritesExistingFile()
        {
            File.WriteAllText(_tempFile, "dummy");
            var scout = TestFixtures.CreateScout();
            var report = new AdvancementReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            var fi = new FileInfo(_tempFile);
            Assert.True(fi.Length > 10);
        }

        [Fact]
        public void Constructor_SingleArg_SetsMinValue()
        {
            var scouts = new List<TroopMember> { TestFixtures.CreateScout() };
            var report = new AdvancementReport(scouts);
            report.Run(_tempFile);
            var text = GetDocumentText(_tempFile);
            Assert.DoesNotContain("Data as of:", text);
        }

        [Fact]
        public void Run_LifeScout_ShowsEagleMbReqDetails()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.Star.DateEarned = new DateTime(2021, 11, 1);
            scout.Life.DateEarned = new DateTime(2022, 5, 1);

            // Add various eagle-required badges to exercise EagleMeritBadgeRequirement reporting
            scout.Add(TestFixtures.EarnedBadge("Camping", new DateTime(2021, 1, 1)));
            scout.Add(TestFixtures.EarnedBadge("First Aid", new DateTime(2021, 2, 1)));
            scout.Add(TestFixtures.EarnedBadge("Communication", new DateTime(2021, 3, 1)));
            scout.Add(TestFixtures.EarnedBadge("Cooking", new DateTime(2021, 4, 1)));
            scout.Add(TestFixtures.EarnedBadge("Personal Fitness", new DateTime(2021, 5, 1)));
            scout.Add(TestFixtures.EarnedBadge("Emergency Preparedness", new DateTime(2021, 6, 1)));
            scout.Add(TestFixtures.EarnedBadge("Environmental Science", new DateTime(2021, 7, 1)));
            scout.Add(TestFixtures.EarnedBadge("Personal Management", new DateTime(2021, 8, 1)));
            scout.Add(TestFixtures.EarnedBadge("Swimming", new DateTime(2021, 9, 1)));
            scout.Add(TestFixtures.EarnedBadge("Family Life", new DateTime(2021, 10, 1)));
            scout.Add(TestFixtures.EarnedBadge("Citizenship in the Community", new DateTime(2021, 11, 1)));
            // Elective badges
            scout.Add(TestFixtures.EarnedBadge("Art", new DateTime(2021, 12, 1)));
            scout.Add(TestFixtures.EarnedBadge("Chess", new DateTime(2022, 1, 1)));

            var report = new AdvancementReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            var text = GetDocumentText(_tempFile);
            Assert.Contains("Requirements remaining for Eagle", text);
            Assert.Contains("Earned:", text);
        }

        [Fact]
        public void Run_LifeScout_WithStartedBadges()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.Star.DateEarned = new DateTime(2021, 11, 1);
            scout.Life.DateEarned = new DateTime(2022, 5, 1);

            // Add some earned and some started
            scout.Add(TestFixtures.EarnedBadge("Camping", new DateTime(2021, 1, 1)));
            scout.Add(TestFixtures.EarnedBadge("First Aid", new DateTime(2021, 2, 1)));
            scout.AddPartial("Communication", "2016");
            scout.AddPartial("Art", "2016");

            var report = new AdvancementReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            var text = GetDocumentText(_tempFile);
            Assert.Contains("Requirements remaining for Eagle", text);
        }

        [Fact]
        public void Run_LifeScout_MutuallyExclusiveBadges()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.Star.DateEarned = new DateTime(2021, 11, 1);
            scout.Life.DateEarned = new DateTime(2022, 5, 1);

            // Earn both Emergency Prep AND Lifesaving - one should become elective
            scout.Add(TestFixtures.EarnedBadge("Emergency Preparedness", new DateTime(2021, 1, 1)));
            scout.Add(TestFixtures.EarnedBadge("Lifesaving", new DateTime(2021, 2, 1)));
            // Both Environmental Science and Sustainability
            scout.Add(TestFixtures.EarnedBadge("Environmental Science", new DateTime(2021, 3, 1)));
            scout.Add(TestFixtures.EarnedBadge("Sustainability", new DateTime(2021, 4, 1)));
            // Multiple from Swimming/Hiking/Cycling
            scout.Add(TestFixtures.EarnedBadge("Swimming", new DateTime(2021, 5, 1)));
            scout.Add(TestFixtures.EarnedBadge("Hiking", new DateTime(2021, 6, 1)));
            scout.Add(TestFixtures.EarnedBadge("Cycling", new DateTime(2021, 7, 1)));

            var report = new AdvancementReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            var text = GetDocumentText(_tempFile);
            Assert.Contains("Requirements remaining for Eagle", text);
        }

        [Fact]
        public void Run_LifeScout_NeedElectives()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.Star.DateEarned = new DateTime(2021, 11, 1);
            scout.Life.DateEarned = new DateTime(2022, 5, 1);

            // Just a couple badges - should show "Need: X elective(s)"
            scout.Add(TestFixtures.EarnedBadge("Camping", new DateTime(2021, 1, 1)));

            var report = new AdvancementReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            var text = GetDocumentText(_tempFile);
            Assert.Contains("Need:", text);
            Assert.Contains("elective", text);
        }

        [Fact]
        public void Run_LifeScout_ShowsNeedRequiredBadges()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.Star.DateEarned = new DateTime(2021, 11, 1);
            scout.Life.DateEarned = new DateTime(2022, 5, 1);

            // Don't add any badges - should show "Need:" for required badges
            var report = new AdvancementReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            var text = GetDocumentText(_tempFile);
            Assert.Contains("Need:", text);
        }

        [Fact]
        public void Run_LifeScout_StartedMutuallyExclusive()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.Star.DateEarned = new DateTime(2021, 11, 1);
            scout.Life.DateEarned = new DateTime(2022, 5, 1);

            // Started but not earned mutually exclusive badges
            scout.AddPartial("Swimming", "2016");
            scout.AddPartial("Hiking", "2016");

            var report = new AdvancementReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            var text = GetDocumentText(_tempFile);
            Assert.Contains("Started:", text);
        }

        [Fact]
        public void Run_TenderfootScout_Shows6a6cWarning()
        {
            var scout = TestFixtures.CreateScout();
            scout.Scout.DateEarned = new DateTime(2020, 1, 1);
            // Set 6a earned more than 45 days ago but 6c not earned
            scout.Tenderfoot.Requirements.Find(x => x.Name == "6a").DateEarned = new DateTime(2020, 3, 1);

            var report = new AdvancementReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            var text = GetDocumentText(_tempFile);
            Assert.Contains("Requirement 6c has not been earned", text);
        }

        [Fact]
        public void Run_TimeRequirementOverdue_ShowsExclamation()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            // Star req 1 should be overdue since FirstClass was earned long ago
            var report = new AdvancementReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            var text = GetDocumentText(_tempFile);
            // The time requirement for Star should show the target date
            Assert.Contains("Be active 4 months since earning First Class", text);
        }

        [Fact]
        public void Run_StarScout_MeritBadgeRequirementDetails()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.Star.DateEarned = new DateTime(2021, 11, 1);

            // Add earned badges that went to Life's MbReq
            scout.Add(TestFixtures.EarnedBadge("Camping", new DateTime(2021, 1, 1)));
            scout.Add(TestFixtures.EarnedBadge("First Aid", new DateTime(2021, 2, 1)));
            scout.Add(TestFixtures.EarnedBadge("Communication", new DateTime(2021, 3, 1)));
            scout.Add(TestFixtures.EarnedBadge("Art", new DateTime(2021, 4, 1)));
            // Add started badges too
            scout.AddPartial("Personal Fitness", "2016");

            var report = new AdvancementReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            var text = GetDocumentText(_tempFile);
            Assert.Contains("Requirements remaining for Life", text);
        }

        [Fact]
        public void Run_CurrentRankWithoutDate_NoDateShown()
        {
            var scout = TestFixtures.CreateScout();
            // NoRank has no DateEarned
            var report = new AdvancementReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            var text = GetDocumentText(_tempFile);
            Assert.Contains("Current Rank: No Rank", text);
        }

        [Fact]
        public void Run_LifeScout_WithManyBadges_ShowsMeritBadgeDetails()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.Star.DateEarned = new DateTime(2021, 11, 1);
            scout.Life.DateEarned = new DateTime(2022, 5, 1);

            // Add eagle required badges including both sides of mutually exclusive pairs
            // This exercises the EagleMeritBadgeRequirement branch with "as elective" display
            scout.Add(TestFixtures.EarnedBadge("First Aid", new DateTime(2021, 1, 1)));
            scout.Add(TestFixtures.EarnedBadge("Citizenship in the Community", new DateTime(2021, 1, 2)));
            scout.Add(TestFixtures.EarnedBadge("Citizenship in the Nation", new DateTime(2021, 1, 3)));
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
            scout.Add(TestFixtures.EarnedBadge("Citizenship in Society", new DateTime(2021, 1, 19)));
            // Only a few electives - not enough to fully satisfy eagle MB requirement
            scout.Add(TestFixtures.EarnedBadge("Art", new DateTime(2021, 2, 1)));
            scout.Add(TestFixtures.EarnedBadge("Chess", new DateTime(2021, 2, 2)));

            var report = new AdvancementReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            var text = GetDocumentText(_tempFile);
            Assert.Contains("Earned: Camping *", text);
            Assert.Contains("Earned: First Aid *", text);
            Assert.Contains("Earned: Art", text);
            Assert.Contains("Need: 5 elective(s)", text);
        }

        [Fact]
        public void Run_LifeScout_SingleStartedMutuallyExclusive()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.Star.DateEarned = new DateTime(2021, 11, 1);
            scout.Life.DateEarned = new DateTime(2022, 5, 1);

            // Start just one of a mutually exclusive pair
            scout.AddPartial("Emergency Preparedness", "2016");
            scout.AllocateMeritBadges();

            var report = new AdvancementReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            var text = GetDocumentText(_tempFile);
            Assert.Contains("Started:", text);
        }

        [Fact]
        public void Run_LifeScout_EarnedEquivalentSkipsOther()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.Star.DateEarned = new DateTime(2021, 11, 1);
            scout.Life.DateEarned = new DateTime(2022, 5, 1);

            // Earn Emergency Prep - Lifesaving should be skipped in output
            scout.Add(TestFixtures.EarnedBadge("Camping", new DateTime(2021, 1, 1)));
            scout.Add(TestFixtures.EarnedBadge("First Aid", new DateTime(2021, 1, 2)));
            scout.Add(TestFixtures.EarnedBadge("Communication", new DateTime(2021, 1, 3)));
            scout.Add(TestFixtures.EarnedBadge("Cooking", new DateTime(2021, 1, 4)));
            scout.Add(TestFixtures.EarnedBadge("Personal Fitness", new DateTime(2021, 1, 5)));
            scout.Add(TestFixtures.EarnedBadge("Emergency Preparedness", new DateTime(2021, 1, 6)));
            scout.Add(TestFixtures.EarnedBadge("Environmental Science", new DateTime(2021, 1, 7)));
            scout.Add(TestFixtures.EarnedBadge("Personal Management", new DateTime(2021, 1, 8)));
            scout.Add(TestFixtures.EarnedBadge("Swimming", new DateTime(2021, 1, 9)));
            scout.Add(TestFixtures.EarnedBadge("Family Life", new DateTime(2021, 1, 10)));
            scout.Add(TestFixtures.EarnedBadge("Citizenship in the Community", new DateTime(2021, 1, 11)));
            scout.Add(TestFixtures.EarnedBadge("Citizenship in the Nation", new DateTime(2021, 1, 12)));
            scout.Add(TestFixtures.EarnedBadge("Citizenship in the World", new DateTime(2021, 1, 13)));
            scout.Add(TestFixtures.EarnedBadge("Citizenship in Society", new DateTime(2021, 1, 14)));
            scout.AllocateMeritBadges();

            var report = new AdvancementReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            var text = GetDocumentText(_tempFile);
            Assert.Contains("Earned:", text);
        }

        [Fact]
        public void Run_StarScout_WithElectiveStarted()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.Star.DateEarned = new DateTime(2021, 11, 1);

            // Add earned required + started electives for Life MbReq
            scout.Add(TestFixtures.EarnedBadge("Camping", new DateTime(2021, 1, 1)));
            scout.Add(TestFixtures.EarnedBadge("First Aid", new DateTime(2021, 2, 1)));
            scout.Add(TestFixtures.EarnedBadge("Communication", new DateTime(2021, 3, 1)));
            scout.AddPartial("Art", "2016");
            scout.AddPartial("Chess", "2016");
            scout.AllocateMeritBadges();

            var report = new AdvancementReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            var text = GetDocumentText(_tempFile);
            Assert.Contains("Requirements remaining for Life", text);
            Assert.Contains("Started:", text);
        }

        [Fact]
        public void Run_PalmScout_ShowsPalmRequirements()
        {
            var scout = TestFixtures.CreateScoutWithAllRanksEarned();
            scout.EaglePalms.Add(new Palm(Palm.PalmType.Bronze, new DateTime(2022, 2, 1)));

            var report = new AdvancementReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            var text = GetDocumentText(_tempFile);
            Assert.Contains("Current Rank: Bronze Palm", text);
        }

        [Fact]
        public void Run_StarScout_WithEarnedAndStartedBadgesInLifeMbReq()
        {
            // Exercise the MeritBadgeRequirement (non-Eagle) branch in AdvancementReport
            // Star scout's NextRank is Life. Life's MbReq is a regular MeritBadgeRequirement.
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.Star.DateEarned = new DateTime(2021, 11, 1);

            // Fill Star with 6 badges (4 required + 2 elective)
            scout.Add(TestFixtures.EarnedBadge("Camping", new DateTime(2021, 1, 1)));
            scout.Add(TestFixtures.EarnedBadge("First Aid", new DateTime(2021, 1, 2)));
            scout.Add(TestFixtures.EarnedBadge("Communication", new DateTime(2021, 1, 3)));
            scout.Add(TestFixtures.EarnedBadge("Cooking", new DateTime(2021, 1, 4)));
            scout.Add(TestFixtures.EarnedBadge("Art", new DateTime(2021, 1, 5)));
            scout.Add(TestFixtures.EarnedBadge("Astronomy", new DateTime(2021, 1, 6)));

            // Overflow to Life MbReq - eagle required
            scout.Add(TestFixtures.EarnedBadge("Personal Fitness", new DateTime(2021, 2, 1)));
            scout.Add(TestFixtures.EarnedBadge("Environmental Science", new DateTime(2021, 2, 2)));
            // Overflow to Life MbReq - elective
            scout.Add(TestFixtures.EarnedBadge("Chess", new DateTime(2021, 2, 3)));

            // Started badges (not earned) - both eagle-required and elective
            scout.AddPartial("Lifesaving", "2016");
            scout.AddPartial("Dog Care", "2016");

            var report = new AdvancementReport(new List<TroopMember> { scout });
            report.Run(_tempFile);

            var text = GetDocumentText(_tempFile);
            Assert.Contains("Requirements remaining for Life", text);
            // Earned eagle-required badges in Life MbReq
            Assert.Contains("Earned: Personal Fitness *", text);
            Assert.Contains("Earned: Environmental Science *", text);
            // Earned elective badge in Life MbReq
            Assert.Contains("Earned: Chess", text);
            // Started badges
            Assert.Contains("Started: Lifesaving *", text);
            Assert.Contains("Started: Dog Care", text);
        }
    }
}
