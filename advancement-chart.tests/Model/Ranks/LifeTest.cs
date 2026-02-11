using System;
using System.Drawing;
using System.Linq;
using Xunit;
using advancementchart.Model;
using advancementchart.Model.Ranks;

namespace advancement_chart.tests.Model.Ranks
{
    public class LifeTest
    {
        [Fact]
        public void Name_IsLife()
        {
            var rank = new Life();
            Assert.Equal("Life", rank.Name);
        }

        [Fact]
        public void FillColor_IsFFCC00()
        {
            var rank = new Life();
            Assert.Equal(ColorTranslator.FromHtml("#FFCC00"), rank.FillColor);
        }

        [Fact]
        public void Requirements_Has8Items()
        {
            var rank = new Life();
            Assert.Equal(8, rank.Requirements.Count);
        }

        [Fact]
        public void MbReq_IsNotNull()
        {
            var rank = new Life();
            Assert.NotNull(rank.MbReq);
        }

        [Fact]
        public void MbReq_Has5Total_3Required()
        {
            var rank = new Life();
            Assert.Equal(5, rank.MbReq.Total);
            Assert.Equal(3, rank.MbReq.Required);
            Assert.Equal(2, rank.MbReq.Elective);
        }

        [Fact]
        public void Requirements_HasTimeRequirement()
        {
            var rank = new Life();
            var timeReq = rank.Requirements.First(r => r.Name == "1");
            Assert.Equal(6, timeReq.TimeRequirementMonths);
        }

        [Fact]
        public void Earned_WhenDateSet()
        {
            var rank = new Life(new DateTime(2023, 1, 1));
            Assert.True(rank.Earned);
        }
    }
}
