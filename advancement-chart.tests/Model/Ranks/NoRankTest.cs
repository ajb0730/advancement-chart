using Xunit;
using advancementchart.Model.Ranks;

namespace advancement_chart.tests.Model.Ranks
{
    public class NoRankTest
    {
        [Fact]
        public void Name_IsNoRank()
        {
            var rank = new NoRank();
            Assert.Equal("No Rank", rank.Name);
        }

        [Fact]
        public void NotEarned()
        {
            var rank = new NoRank();
            Assert.False(rank.Earned);
        }

        [Fact]
        public void Requirements_Empty()
        {
            var rank = new NoRank();
            Assert.Empty(rank.Requirements);
        }
    }
}
