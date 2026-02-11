using System;
using System.Drawing;
using System.Linq;
using Xunit;
using advancementchart.Model;
using advancementchart.Model.Ranks;

namespace advancement_chart.tests.Model.Ranks
{
    public class StarTest
    {
        [Fact]
        public void Name_IsStar()
        {
            var rank = new Star();
            Assert.Equal("Star", rank.Name);
        }

        [Fact]
        public void FillColor_Is003F87()
        {
            var rank = new Star();
            Assert.Equal(ColorTranslator.FromHtml("#003F87"), rank.FillColor);
        }

        [Fact]
        public void Requirements_Has8Items()
        {
            var rank = new Star();
            Assert.Equal(8, rank.Requirements.Count);
        }

        [Fact]
        public void MbReq_IsNotNull()
        {
            var rank = new Star();
            Assert.NotNull(rank.MbReq);
        }

        [Fact]
        public void MbReq_Has6Total_4Required()
        {
            var rank = new Star();
            Assert.Equal(6, rank.MbReq.Total);
            Assert.Equal(4, rank.MbReq.Required);
            Assert.Equal(2, rank.MbReq.Elective);
        }

        [Fact]
        public void Requirements_HasTimeRequirement()
        {
            var rank = new Star();
            var timeReq = rank.Requirements.First(r => r.Name == "1");
            Assert.Equal(4, timeReq.TimeRequirementMonths);
        }

        [Fact]
        public void Earned_WhenDateSet()
        {
            var rank = new Star(new DateTime(2023, 1, 1));
            Assert.True(rank.Earned);
        }
    }
}
