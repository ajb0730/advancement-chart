using System;
using System.Linq;
using Xunit;
using advancementchart.Model;

namespace advancement_chart.tests.Model
{
    public class MeritBadgeRequirementTest
    {
        private Rank CreateRank()
        {
            return new Rank("TestRank", "", version: "2016");
        }

        [Fact]
        public void Constructor_SetsProperties()
        {
            var rank = CreateRank();
            var req = new MeritBadgeRequirement("3", "desc", rank, required: 4, total: 6);
            Assert.Equal(4, req.Required);
            Assert.Equal(6, req.Total);
            Assert.Equal(2, req.Elective);
        }

        [Fact]
        public void Constructor_TotalZero_Throws()
        {
            var rank = CreateRank();
            Assert.Throws<ArgumentException>(() =>
                new MeritBadgeRequirement("3", "desc", rank, required: 0, total: 0));
        }

        [Fact]
        public void Constructor_RequiredGteTotal_Throws()
        {
            var rank = CreateRank();
            Assert.Throws<ArgumentException>(() =>
                new MeritBadgeRequirement("3", "desc", rank, required: 6, total: 6));
        }

        [Fact]
        public void Constructor_TotalNegative_Throws()
        {
            var rank = CreateRank();
            Assert.Throws<ArgumentException>(() =>
                new MeritBadgeRequirement("3", "desc", rank, required: 0, total: -1));
        }

        [Fact]
        public void MeritBadges_InitializedEmpty()
        {
            var rank = CreateRank();
            var req = new MeritBadgeRequirement("3", "desc", rank, required: 4, total: 6);
            Assert.Empty(req.MeritBadges);
        }

        [Fact]
        public void Earned_FalseWhenEmpty()
        {
            var rank = CreateRank();
            var req = new MeritBadgeRequirement("3", "desc", rank, required: 4, total: 6);
            Assert.False(req.Earned);
        }

        [Fact]
        public void Earned_TrueWhenAllBadgesEarned()
        {
            var rank = CreateRank();
            var req = new MeritBadgeRequirement("3", "desc", rank, required: 2, total: 3);

            req.Add(new MeritBadge("Camping", "2016", new DateTime(2023, 1, 1)));
            req.Add(new MeritBadge("First Aid", "2016", new DateTime(2023, 2, 1)));
            req.Add(new MeritBadge("Art", "2016", new DateTime(2023, 3, 1)));

            Assert.True(req.Earned);
        }

        [Fact]
        public void DateEarned_ReturnsMaxDate()
        {
            var rank = CreateRank();
            var req = new MeritBadgeRequirement("3", "desc", rank, required: 2, total: 3);

            req.Add(new MeritBadge("Camping", "2016", new DateTime(2023, 1, 1)));
            req.Add(new MeritBadge("First Aid", "2016", new DateTime(2023, 2, 1)));
            req.Add(new MeritBadge("Art", "2016", new DateTime(2023, 3, 1)));

            Assert.Equal(new DateTime(2023, 3, 1), req.DateEarned);
        }

        [Fact]
        public void DateEarned_NullWhenNotAllEarned()
        {
            var rank = CreateRank();
            var req = new MeritBadgeRequirement("3", "desc", rank, required: 2, total: 3);

            req.Add(new MeritBadge("Camping", "2016", new DateTime(2023, 1, 1)));
            Assert.Null(req.DateEarned);
        }

        [Fact]
        public void Add_EagleRequired_FillsRequiredSlot()
        {
            var rank = CreateRank();
            var req = new MeritBadgeRequirement("3", "desc", rank, required: 4, total: 6);

            bool added = req.Add(new MeritBadge("Camping", "2016", new DateTime(2023, 1, 1)));
            Assert.True(added);
            Assert.Single(req.MeritBadges);
        }

        [Fact]
        public void Add_Elective_FillsElectiveSlot()
        {
            var rank = CreateRank();
            var req = new MeritBadgeRequirement("3", "desc", rank, required: 4, total: 6);

            bool added = req.Add(new MeritBadge("Art", "2016", new DateTime(2023, 1, 1)));
            Assert.True(added);
        }

        [Fact]
        public void Add_RejectWhenFull()
        {
            var rank = CreateRank();
            var req = new MeritBadgeRequirement("3", "desc", rank, required: 2, total: 3);

            req.Add(new MeritBadge("Camping", "2016", new DateTime(2023, 1, 1)));
            req.Add(new MeritBadge("First Aid", "2016", new DateTime(2023, 2, 1)));
            req.Add(new MeritBadge("Art", "2016", new DateTime(2023, 3, 1)));

            bool added = req.Add(new MeritBadge("Chess", "2016", new DateTime(2023, 4, 1)));
            Assert.False(added);
            Assert.Equal(3, req.MeritBadges.Count);
        }

        [Fact]
        public void Add_RejectElectiveWhenElectiveSlotsFull()
        {
            var rank = CreateRank();
            // 4 required, 2 elective => total 6
            var req = new MeritBadgeRequirement("3", "desc", rank, required: 4, total: 6);

            // Fill the 2 elective slots
            req.Add(new MeritBadge("Art", "2016", new DateTime(2023, 1, 1)));
            req.Add(new MeritBadge("Chess", "2016", new DateTime(2023, 2, 1)));

            // Third elective should be rejected
            bool added = req.Add(new MeritBadge("Astronomy", "2016", new DateTime(2023, 3, 1)));
            Assert.False(added);
        }

        [Fact]
        public void AddAny_ForcesElectiveAdd()
        {
            var rank = CreateRank();
            var req = new MeritBadgeRequirement("3", "desc", rank, required: 4, total: 6);

            // Add eagle required as forceElective
            bool added = req.AddAny(new MeritBadge("Camping", "2016", new DateTime(2023, 1, 1)));
            Assert.True(added);
        }

        [Fact]
        public void Add_FillsRequiredBeforeElective()
        {
            var rank = CreateRank();
            var req = new MeritBadgeRequirement("3", "desc", rank, required: 2, total: 4);

            // Add 2 required
            req.Add(new MeritBadge("Camping", "2016", new DateTime(2023, 1, 1)));
            req.Add(new MeritBadge("First Aid", "2016", new DateTime(2023, 2, 1)));

            // Try adding a 3rd required badge - should not be able to as required is full
            // and it won't fit the elective slot since it's an eagle required badge
            // Actually, looking at the code: if badge is eagle required AND req < Required,
            // then it adds. req=2, Required=2, so req !< Required, and forceElective is false.
            // So it should return false because it's eagle required but required slots are full.
            // And it's not !badge.EagleRequired so elective path doesn't fire.
            bool added = req.Add(new MeritBadge("Communication", "2016", new DateTime(2023, 3, 1)));
            Assert.False(added);

            // But electives should work
            added = req.Add(new MeritBadge("Art", "2016", new DateTime(2023, 3, 1)));
            Assert.True(added);
        }
    }
}
