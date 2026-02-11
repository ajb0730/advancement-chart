using System;
using System.Drawing;
using System.Linq;
using Xunit;
using advancementchart.Model;
using advancementchart.Model.Ranks;

namespace advancement_chart.tests.Model.Ranks
{
    public class FirstClassTest
    {
        [Fact]
        public void Name_IsFirstClass()
        {
            var rank = new FirstClass();
            Assert.Equal("First Class", rank.Name);
        }

        [Fact]
        public void FillColor_IsCE1126()
        {
            var rank = new FirstClass();
            Assert.Equal(ColorTranslator.FromHtml("#CE1126"), rank.FillColor);
        }

        [Fact]
        public void Requirements_Has38Items()
        {
            var rank = new FirstClass();
            Assert.Equal(38, rank.Requirements.Count);
        }

        [Fact]
        public void Requirements_FirstIs1a()
        {
            var rank = new FirstClass();
            Assert.Equal("1a", rank.Requirements.First().Name);
        }

        [Fact]
        public void Requirements_LastIs13()
        {
            var rank = new FirstClass();
            Assert.Equal("13", rank.Requirements.Last().Name);
        }

        [Fact]
        public void Earned_WhenDateSet()
        {
            var rank = new FirstClass(new DateTime(2023, 1, 1));
            Assert.True(rank.Earned);
        }

        [Fact]
        public void Requirements_HaveCurriculumGroups()
        {
            var rank = new FirstClass();
            Assert.Contains(rank.Requirements, r => r.Group == CurriculumGroup.OutdoorEthics);
            Assert.Contains(rank.Requirements, r => r.Group == CurriculumGroup.Cooking);
            Assert.Contains(rank.Requirements, r => r.Group == CurriculumGroup.KnotsAndLashings2);
            Assert.Contains(rank.Requirements, r => r.Group == CurriculumGroup.MapAndCompass2);
            Assert.Contains(rank.Requirements, r => r.Group == CurriculumGroup.Nature2);
            Assert.Contains(rank.Requirements, r => r.Group == CurriculumGroup.WaterSafety);
            Assert.Contains(rank.Requirements, r => r.Group == CurriculumGroup.FirstAidBandages);
            Assert.Contains(rank.Requirements, r => r.Group == CurriculumGroup.FirstAidCPR);
            Assert.Contains(rank.Requirements, r => r.Group == CurriculumGroup.FirstAidRescues);
        }
    }
}
