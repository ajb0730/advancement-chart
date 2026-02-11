using System;
using System.Drawing;
using Xunit;
using advancementchart.Model;

namespace advancement_chart.tests.Model
{
    public class RankTest
    {
        [Fact]
        public void Rank_NotEarned_ByDefault()
        {
            var rank = new Rank("Test", "Desc");
            Assert.False(rank.Earned);
            Assert.Null(rank.DateEarned);
        }

        [Fact]
        public void Rank_Earned_WhenDateSet()
        {
            var date = new DateTime(2023, 1, 1);
            var rank = new Rank("Test", "Desc", earned: date);
            Assert.True(rank.Earned);
            Assert.Equal(date, rank.DateEarned);
        }

        [Fact]
        public void Rank_DefaultFillColor_IsWhite()
        {
            var rank = new Rank("Test", "Desc");
            Assert.Equal(Color.White, rank.FillColor);
        }

        [Fact]
        public void Rank_RequirementsInitialized()
        {
            var rank = new Rank("Test", "Desc");
            Assert.NotNull(rank.Requirements);
            Assert.Empty(rank.Requirements);
        }

        [Fact]
        public void Rank_SetsNameAndVersion()
        {
            var rank = new Rank("Scout", "desc", version: "2016");
            Assert.Equal("Scout", rank.Name);
            Assert.Equal("2016", rank.Version);
        }

        [Fact]
        public void Rank_DateEarned_CanBeSetAfterConstruction()
        {
            var rank = new Rank("Test", "Desc");
            Assert.False(rank.Earned);
            rank.DateEarned = new DateTime(2023, 6, 1);
            Assert.True(rank.Earned);
        }
    }
}
