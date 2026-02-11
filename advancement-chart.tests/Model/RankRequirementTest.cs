using System;
using Xunit;
using advancementchart.Model;

namespace advancement_chart.tests.Model
{
    public class RankRequirementTest
    {
        private Rank CreateRank(DateTime? earned = null)
        {
            return new Rank("TestRank", "", earned: earned, version: "2016");
        }

        [Fact]
        public void Earned_WhenRankEarned_ReturnsTrue()
        {
            var rank = CreateRank(earned: new DateTime(2023, 1, 1));
            var req = new RankRequirement("1a", "desc", rank);
            Assert.True(req.Earned);
        }

        [Fact]
        public void Earned_WhenRequirementDateSet_ReturnsTrue()
        {
            var rank = CreateRank();
            var req = new RankRequirement("1a", "desc", rank, earned: new DateTime(2023, 1, 1));
            Assert.True(req.Earned);
        }

        [Fact]
        public void Earned_WhenBothNull_ReturnsFalse()
        {
            var rank = CreateRank();
            var req = new RankRequirement("1a", "desc", rank);
            Assert.False(req.Earned);
        }

        [Fact]
        public void Earned_WhenBothSet_ReturnsTrue()
        {
            var rank = CreateRank(earned: new DateTime(2023, 1, 1));
            var req = new RankRequirement("1a", "desc", rank, earned: new DateTime(2023, 2, 1));
            Assert.True(req.Earned);
        }

        [Fact]
        public void Constructor_VersionMismatch_Throws()
        {
            var rank = new Rank("TestRank", "", version: "2020");
            Assert.Throws<ArgumentException>(() =>
                new RankRequirement("1a", "desc", rank, version: "2016"));
        }

        [Fact]
        public void TimeRequirementMonths_SetsCorrectly()
        {
            var rank = CreateRank();
            var req = new RankRequirement("1", "desc", rank, timeRequirementMonths: 4);
            Assert.Equal(4, req.TimeRequirementMonths);
        }

        [Fact]
        public void TimeRequirementMonths_NullByDefault()
        {
            var rank = CreateRank();
            var req = new RankRequirement("1", "desc", rank);
            Assert.Null(req.TimeRequirementMonths);
        }

        [Fact]
        public void HandbookPages_SetsCorrectly()
        {
            var rank = CreateRank();
            var req = new RankRequirement("1", "desc", rank, handbookPages: "11-18");
            Assert.Equal("11-18", req.HandbookPages);
        }

        [Fact]
        public void Group_SetsCorrectly()
        {
            var rank = CreateRank();
            var req = new RankRequirement("1", "desc", rank, curriculumGroup: CurriculumGroup.Camping1);
            Assert.Equal(CurriculumGroup.Camping1, req.Group);
        }

        [Fact]
        public void Group_NullByDefault()
        {
            var rank = CreateRank();
            var req = new RankRequirement("1", "desc", rank);
            Assert.Null(req.Group);
        }

        [Fact]
        public void Rank_PropertySet()
        {
            var rank = CreateRank();
            var req = new RankRequirement("1", "desc", rank);
            Assert.Same(rank, req.Rank);
        }

        [Fact]
        public void DateEarned_CanBeSetAfterConstruction()
        {
            var rank = CreateRank();
            var req = new RankRequirement("1", "desc", rank);
            Assert.False(req.Earned);
            req.DateEarned = new DateTime(2023, 6, 1);
            Assert.True(req.Earned);
        }
    }
}
