using System;
using Xunit;
using advancementchart.Model;

namespace advancement_chart.tests.Model
{
    public class EagleMeritBadgeRequirementTest
    {
        private Rank CreateRank()
        {
            return new Rank("Eagle", "", version: "2016");
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
        public void Add_RejectsEmergencyPrep_WhenLifesavingPresent()
        {
            var rank = CreateRank();
            var req = new EagleMeritBadgeRequirement("3", "desc", rank);

            req.Add(new MeritBadge("Lifesaving", "2016", new DateTime(2023, 1, 1)));
            bool added = req.Add(new MeritBadge("Emergency Preparedness", "2016", new DateTime(2023, 2, 1)));
            Assert.False(added);
        }

        [Fact]
        public void Add_RejectsLifesaving_WhenEmergencyPrepPresent()
        {
            var rank = CreateRank();
            var req = new EagleMeritBadgeRequirement("3", "desc", rank);

            req.Add(new MeritBadge("Emergency Preparedness", "2016", new DateTime(2023, 1, 1)));
            bool added = req.Add(new MeritBadge("Lifesaving", "2016", new DateTime(2023, 2, 1)));
            Assert.False(added);
        }

        [Fact]
        public void Add_RejectsSustainability_WhenEnvSciPresent()
        {
            var rank = CreateRank();
            var req = new EagleMeritBadgeRequirement("3", "desc", rank);

            req.Add(new MeritBadge("Environmental Science", "2016", new DateTime(2023, 1, 1)));
            bool added = req.Add(new MeritBadge("Sustainability", "2016", new DateTime(2023, 2, 1)));
            Assert.False(added);
        }

        [Fact]
        public void Add_RejectsEnvSci_WhenSustainabilityPresent()
        {
            var rank = CreateRank();
            var req = new EagleMeritBadgeRequirement("3", "desc", rank);

            req.Add(new MeritBadge("Sustainability", "2016", new DateTime(2023, 1, 1)));
            bool added = req.Add(new MeritBadge("Environmental Science", "2016", new DateTime(2023, 2, 1)));
            Assert.False(added);
        }

        [Fact]
        public void Add_RejectsHiking_WhenSwimmingPresent()
        {
            var rank = CreateRank();
            var req = new EagleMeritBadgeRequirement("3", "desc", rank);

            req.Add(new MeritBadge("Swimming", "2016", new DateTime(2023, 1, 1)));
            bool added = req.Add(new MeritBadge("Hiking", "2016", new DateTime(2023, 2, 1)));
            Assert.False(added);
        }

        [Fact]
        public void Add_RejectsCycling_WhenSwimmingPresent()
        {
            var rank = CreateRank();
            var req = new EagleMeritBadgeRequirement("3", "desc", rank);

            req.Add(new MeritBadge("Swimming", "2016", new DateTime(2023, 1, 1)));
            bool added = req.Add(new MeritBadge("Cycling", "2016", new DateTime(2023, 2, 1)));
            Assert.False(added);
        }

        [Fact]
        public void Add_RejectsSwimming_WhenHikingPresent()
        {
            var rank = CreateRank();
            var req = new EagleMeritBadgeRequirement("3", "desc", rank);

            req.Add(new MeritBadge("Hiking", "2016", new DateTime(2023, 1, 1)));
            bool added = req.Add(new MeritBadge("Swimming", "2016", new DateTime(2023, 2, 1)));
            Assert.False(added);
        }

        [Fact]
        public void Add_RejectsSwimming_WhenCyclingPresent()
        {
            var rank = CreateRank();
            var req = new EagleMeritBadgeRequirement("3", "desc", rank);

            req.Add(new MeritBadge("Cycling", "2016", new DateTime(2023, 1, 1)));
            bool added = req.Add(new MeritBadge("Swimming", "2016", new DateTime(2023, 2, 1)));
            Assert.False(added);
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
    }
}
