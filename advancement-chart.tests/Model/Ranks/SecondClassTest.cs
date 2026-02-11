using System;
using System.Drawing;
using System.Linq;
using Xunit;
using advancementchart.Model;
using advancementchart.Model.Ranks;

namespace advancement_chart.tests.Model.Ranks
{
    public class SecondClassTest
    {
        [Fact]
        public void Name_IsSecondClass()
        {
            var rank = new SecondClass();
            Assert.Equal("Second Class", rank.Name);
        }

        [Fact]
        public void FillColor_Is006B3F()
        {
            var rank = new SecondClass();
            Assert.Equal(ColorTranslator.FromHtml("#006B3F"), rank.FillColor);
        }

        [Fact]
        public void Requirements_Has37Items()
        {
            var rank = new SecondClass();
            Assert.Equal(37, rank.Requirements.Count);
        }

        [Fact]
        public void Requirements_FirstIs1a()
        {
            var rank = new SecondClass();
            Assert.Equal("1a", rank.Requirements.First().Name);
        }

        [Fact]
        public void Requirements_LastIs12()
        {
            var rank = new SecondClass();
            Assert.Equal("12", rank.Requirements.Last().Name);
        }

        [Fact]
        public void Earned_WhenDateSet()
        {
            var rank = new SecondClass(new DateTime(2023, 1, 1));
            Assert.True(rank.Earned);
        }

        [Fact]
        public void Requirements_HaveCurriculumGroups()
        {
            var rank = new SecondClass();
            Assert.Contains(rank.Requirements, r => r.Group == CurriculumGroup.OutdoorEthics);
            Assert.Contains(rank.Requirements, r => r.Group == CurriculumGroup.FiresAndFireSafety);
            Assert.Contains(rank.Requirements, r => r.Group == CurriculumGroup.KnotsAndLashings2);
            Assert.Contains(rank.Requirements, r => r.Group == CurriculumGroup.MapAndCompass1);
            Assert.Contains(rank.Requirements, r => r.Group == CurriculumGroup.Nature1);
            Assert.Contains(rank.Requirements, r => r.Group == CurriculumGroup.WaterSafety);
        }
    }
}
