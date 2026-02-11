using System;
using System.Drawing;
using System.Linq;
using Xunit;
using advancementchart.Model;
using advancementchart.Model.Ranks;

namespace advancement_chart.tests.Model.Ranks
{
    public class TenderfootTest
    {
        [Fact]
        public void Name_IsTenderfoot()
        {
            var rank = new Tenderfoot();
            Assert.Equal("Tenderfoot", rank.Name);
        }

        [Fact]
        public void FillColor_Is996633()
        {
            var rank = new Tenderfoot();
            Assert.Equal(ColorTranslator.FromHtml("#996633"), rank.FillColor);
        }

        [Fact]
        public void Requirements_Has26Items()
        {
            var rank = new Tenderfoot();
            Assert.Equal(26, rank.Requirements.Count);
        }

        [Fact]
        public void Requirements_FirstIs1a()
        {
            var rank = new Tenderfoot();
            Assert.Equal("1a", rank.Requirements.First().Name);
        }

        [Fact]
        public void Requirements_LastIs11()
        {
            var rank = new Tenderfoot();
            Assert.Equal("11", rank.Requirements.Last().Name);
        }

        [Fact]
        public void Earned_WhenDateSet()
        {
            var rank = new Tenderfoot(new DateTime(2023, 1, 1));
            Assert.True(rank.Earned);
        }

        [Fact]
        public void Requirements_HaveCurriculumGroups()
        {
            var rank = new Tenderfoot();
            Assert.Contains(rank.Requirements, r => r.Group == CurriculumGroup.Camping1);
            Assert.Contains(rank.Requirements, r => r.Group == CurriculumGroup.Camping2);
            Assert.Contains(rank.Requirements, r => r.Group == CurriculumGroup.Cooking);
            Assert.Contains(rank.Requirements, r => r.Group == CurriculumGroup.FirstAidBasics1);
            Assert.Contains(rank.Requirements, r => r.Group == CurriculumGroup.Fitness1);
            Assert.Contains(rank.Requirements, r => r.Group == CurriculumGroup.Fitness2);
        }
    }
}
