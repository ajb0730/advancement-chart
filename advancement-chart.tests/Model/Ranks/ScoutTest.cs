using System;
using System.Drawing;
using System.Linq;
using Xunit;
using advancementchart.Model;
using advancementchart.Model.Ranks;

namespace advancement_chart.tests.Model.Ranks
{
    public class ScoutTest
    {
        [Fact]
        public void Name_IsScout()
        {
            var rank = new Scout();
            Assert.Equal("Scout", rank.Name);
        }

        [Fact]
        public void FillColor_IsCC9900()
        {
            var rank = new Scout();
            Assert.Equal(ColorTranslator.FromHtml("#CC9900"), rank.FillColor);
        }

        [Fact]
        public void Requirements_Has18Items()
        {
            var rank = new Scout();
            Assert.Equal(18, rank.Requirements.Count);
        }

        [Fact]
        public void Requirements_FirstIs1a()
        {
            var rank = new Scout();
            Assert.Equal("1a", rank.Requirements.First().Name);
        }

        [Fact]
        public void Requirements_LastIs7()
        {
            var rank = new Scout();
            Assert.Equal("7", rank.Requirements.Last().Name);
        }

        [Fact]
        public void NotEarned_ByDefault()
        {
            var rank = new Scout();
            Assert.False(rank.Earned);
        }

        [Fact]
        public void Earned_WhenDateSet()
        {
            var rank = new Scout(new DateTime(2023, 1, 1));
            Assert.True(rank.Earned);
        }

        [Fact]
        public void Version_Is2016()
        {
            var rank = new Scout();
            Assert.Equal("2016", rank.Version);
        }

        [Fact]
        public void Requirements_HaveCurriculumGroups()
        {
            var rank = new Scout();
            Assert.Contains(rank.Requirements, r => r.Group == CurriculumGroup.FormingThePatrol);
            Assert.Contains(rank.Requirements, r => r.Group == CurriculumGroup.KnotsAndLashings1);
            Assert.Contains(rank.Requirements, r => r.Group == CurriculumGroup.TotinChip);
        }
    }
}
