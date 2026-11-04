using System;
using System.Linq;
using Xunit;
using advancementchart.Model;

namespace advancement_chart.tests.Model
{
    public class EagleMeritBadgeRequirementTest
    {
        private Rank CreateRank()
        {
            return new Rank("Eagle", "");
        }

        [Fact]
        public void Constructor_SetsTotal21()
        {
            var rank = CreateRank();
            var req = new EagleMeritBadgeRequirement("3", "desc", rank);
            Assert.Equal(21, req.Total);
        }

        [Fact]
        public void Constructor_Required14_DuringCitSocietyWindow()
        {
            // Required is 14 while Citizenship in Society is active (2022-07-01 to 2026-12-31)
            var rank = CreateRank();
            var req = new EagleMeritBadgeRequirement("3", "desc", rank);
            Assert.Equal(14, req.Required);
        }

        [Fact]
        public void CitSocietyCutover_IsJuly2022()
        {
            Assert.Equal(new DateTime(2022, 7, 1), EagleMeritBadgeRequirement.CitSocietyCutover);
        }

        [Fact]
        public void Add_CountsAsElective_EmergencyPrep_WhenLifesavingPresent()
        {
            var rank = CreateRank();
            var req = new EagleMeritBadgeRequirement("3", "desc", rank);

            req.Add(new MeritBadge("Lifesaving", "2016", new DateTime(2023, 1, 1)));
            bool added = req.Add(new MeritBadge("Emergency Preparedness", "2016", new DateTime(2023, 2, 1)));
            Assert.True(added);
            Assert.Single(req.SplitEarned().Elective);
            Assert.Single(req.SplitEarned().Required);
        }

        [Fact]
        public void Add_CountsAsElective_Lifesaving_WhenEmergencyPrepPresent()
        {
            var rank = CreateRank();
            var req = new EagleMeritBadgeRequirement("3", "desc", rank);

            req.Add(new MeritBadge("Emergency Preparedness", "2016", new DateTime(2023, 1, 1)));
            bool added = req.Add(new MeritBadge("Lifesaving", "2016", new DateTime(2023, 2, 1)));
            Assert.True(added);
            Assert.Single(req.SplitEarned().Elective);
            Assert.Single(req.SplitEarned().Required);
        }

        [Fact]
        public void Add_CountsAsElective_Sustainability_WhenEnvSciPresent()
        {
            var rank = CreateRank();
            var req = new EagleMeritBadgeRequirement("3", "desc", rank);

            req.Add(new MeritBadge("Environmental Science", "2016", new DateTime(2023, 1, 1)));
            bool added = req.Add(new MeritBadge("Sustainability", "2016", new DateTime(2023, 2, 1)));
            Assert.True(added);
            Assert.Single(req.SplitEarned().Elective);
            Assert.Single(req.SplitEarned().Required);
        }

        [Fact]
        public void Add_CountsAsElective_EnvSci_WhenSustainabilityPresent()
        {
            var rank = CreateRank();
            var req = new EagleMeritBadgeRequirement("3", "desc", rank);

            req.Add(new MeritBadge("Sustainability", "2016", new DateTime(2023, 1, 1)));
            bool added = req.Add(new MeritBadge("Environmental Science", "2016", new DateTime(2023, 2, 1)));
            Assert.True(added);
            Assert.Single(req.SplitEarned().Elective);
            Assert.Single(req.SplitEarned().Required);
        }

        [Fact]
        public void Add_CountsAsElective_Hiking_WhenSwimmingPresent()
        {
            var rank = CreateRank();
            var req = new EagleMeritBadgeRequirement("3", "desc", rank);

            req.Add(new MeritBadge("Swimming", "2016", new DateTime(2023, 1, 1)));
            bool added = req.Add(new MeritBadge("Hiking", "2016", new DateTime(2023, 2, 1)));
            Assert.True(added);
            Assert.Single(req.SplitEarned().Elective);
            Assert.Single(req.SplitEarned().Required);
        }

        [Fact]
        public void Add_CountsAsElective_Cycling_WhenSwimmingPresent()
        {
            var rank = CreateRank();
            var req = new EagleMeritBadgeRequirement("3", "desc", rank);

            req.Add(new MeritBadge("Swimming", "2016", new DateTime(2023, 1, 1)));
            bool added = req.Add(new MeritBadge("Cycling", "2016", new DateTime(2023, 2, 1)));
            Assert.True(added);
            Assert.Single(req.SplitEarned().Elective);
            Assert.Single(req.SplitEarned().Required);
        }

        [Fact]
        public void Add_CountsAsElective_Swimming_WhenHikingPresent()
        {
            var rank = CreateRank();
            var req = new EagleMeritBadgeRequirement("3", "desc", rank);

            req.Add(new MeritBadge("Hiking", "2016", new DateTime(2023, 1, 1)));
            bool added = req.Add(new MeritBadge("Swimming", "2016", new DateTime(2023, 2, 1)));
            Assert.True(added);
            Assert.Single(req.SplitEarned().Elective);
            Assert.Single(req.SplitEarned().Required);
        }

        [Fact]
        public void Add_CountsAsElective_Swimming_WhenCyclingPresent()
        {
            var rank = CreateRank();
            var req = new EagleMeritBadgeRequirement("3", "desc", rank);

            req.Add(new MeritBadge("Cycling", "2016", new DateTime(2023, 1, 1)));
            bool added = req.Add(new MeritBadge("Swimming", "2016", new DateTime(2023, 2, 1)));
            Assert.True(added);
            Assert.Single(req.SplitEarned().Elective);
            Assert.Single(req.SplitEarned().Required);
        }

        [Fact]
        public void Add_AcceptsNonConflicting()
        {
            var rank = CreateRank();
            var req = new EagleMeritBadgeRequirement("3", "desc", rank);

            bool added = req.Add(new MeritBadge("Camping", "2016", new DateTime(2023, 1, 1)));
            Assert.True(added);
            added = req.Add(new MeritBadge("First Aid", "2016", new DateTime(2023, 2, 1)));
            Assert.True(added);
        }

        [Fact]
        public void Add_AcceptsElective()
        {
            var rank = CreateRank();
            var req = new EagleMeritBadgeRequirement("3", "desc", rank);

            bool added = req.Add(new MeritBadge("Art", "2016", new DateTime(2023, 1, 1)));
            Assert.True(added);
        }

        [Fact]
        public void Elective_Is7_DuringCitSocietyWindow()
        {
            var rank = CreateRank();
            var req = new EagleMeritBadgeRequirement("3", "desc", rank);
            // Total=21, Required=14 during Cit in Society window (2022-07-01 to 2026-12-31), so Elective=7
            Assert.Equal(7, req.Elective);
        }

        [Theory]
        [InlineData(2022, 6, 30, 23, 59, false)]
        [InlineData(2022, 7, 1, 0, 0, true)]
        [InlineData(2024, 3, 15, 12, 0, true)]
        [InlineData(2026, 12, 30, 23, 59, true)]
        [InlineData(2026, 12, 31, 0, 0, true)]
        [InlineData(2026, 12, 31, 12, 30, true)]   // the whole last day counts
        [InlineData(2026, 12, 31, 23, 59, true)]
        [InlineData(2027, 1, 1, 0, 0, false)]
        public void IsCitizenshipInSocietyRequired_CoversWholeDaysOfTheWindow(int y, int mo, int d, int h, int mi, bool expected)
        {
            Assert.Equal(expected, EagleMeritBadgeRequirement.IsCitizenshipInSocietyRequired(new DateTime(y, mo, d, h, mi, 0)));
        }

        [Fact]
        public void Add_ThreeOfExclusiveGroup_FirstIsRequiredOthersElective()
        {
            var req = new EagleMeritBadgeRequirement("3", "desc", CreateRank());
            Assert.True(req.Add(new MeritBadge("Swimming", "2016", new DateTime(2023, 1, 1))));
            Assert.True(req.Add(new MeritBadge("Hiking", "2016", new DateTime(2023, 2, 1))));
            Assert.True(req.Add(new MeritBadge("Cycling", "2016", new DateTime(2023, 3, 1))));

            var (required, elective) = req.SplitEarned();
            Assert.Equal(new[] { "Swimming" }, required.Select(b => b.Name).ToArray());
            Assert.Equal(new[] { "Hiking", "Cycling" }, elective.Select(b => b.Name).ToArray());
        }

        [Fact]
        public void Add_ExtraDoesNotUseUpARequiredSlot()
        {
            // Fill every required slot with distinct badges after an extra pair member; the last
            // required badge must still fit.
            var req = new EagleMeritBadgeRequirement("3", "desc", CreateRank());
            var day = new DateTime(2023, 1, 1);
            req.Add(new MeritBadge("Emergency Preparedness", "2016", day));
            req.Add(new MeritBadge("Lifesaving", "2016", day.AddDays(1)));   // elective
            string[] others = { "Camping", "Cooking", "Communication", "First Aid", "Family Life", "Personal Fitness",
                                "Personal Management", "Citizenship in the Community", "Citizenship in the Nation",
                                "Citizenship in the World", "Environmental Science", "Swimming" };
            int d = 2;
            foreach (var name in others)
            {
                Assert.True(req.Add(new MeritBadge(name, "2016", day.AddDays(d++))), name);
            }
            // That is 13 distinct required groups; Citizenship in Society (when required) is the 14th.
            if (MeritBadge.GetEagleRequired().Contains("Citizenship in Society"))
            {
                Assert.True(req.Add(new MeritBadge("Citizenship in Society", "2016", day.AddDays(d))));
            }
            Assert.Equal(req.Required, req.SplitEarned().Required.Count);
        }

        [Fact]
        public void Add_ExtraIsRejectedWhenElectiveSlotsAreFull()
        {
            var req = new EagleMeritBadgeRequirement("3", "desc", CreateRank());
            req.Add(new MeritBadge("Emergency Preparedness", "2016", new DateTime(2023, 1, 1)));
            var electives = MeritBadge.BsaMeritBadgeIds.Keys
                .Where(n => !MeritBadge.GetEagleRequired().Contains(n) && n != "Citizenship in Society")
                .Take(req.Elective)
                .ToList();
            Assert.Equal(req.Elective, electives.Count);
            foreach (var name in electives)
            {
                Assert.True(req.Add(new MeritBadge(name, "2016", new DateTime(2023, 2, 1))), name);
            }

            // Every elective slot is taken, so the extra Lifesaving has nowhere to go.
            Assert.False(req.Add(new MeritBadge("Lifesaving", "2016", new DateTime(2023, 3, 1))));
        }

        [Fact]
        public void Add_StartedExtraOfEitherOrGroup_IsNotAdded()
        {
            var req = new EagleMeritBadgeRequirement("3", "desc", CreateRank());
            req.Add(new MeritBadge("Swimming", "2016", new DateTime(2023, 1, 1)));
            var started = new MeritBadge("Hiking", "2016") { Started = true };

            Assert.False(req.Add(started));
            Assert.Single(req.MeritBadges);
        }
    }
}
