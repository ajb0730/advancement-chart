using System;
using System.Linq;
using Xunit;
using advancementchart.Model;
using advancementchart.Model.Ranks;

namespace advancement_chart.tests.Model.Ranks
{
    public class PalmTest
    {
        [Fact]
        public void BronzePalm_Name()
        {
            var palm = new Palm(Palm.PalmType.Bronze);
            Assert.Equal("Bronze Palm", palm.Name);
        }

        [Fact]
        public void GoldPalm_Name()
        {
            var palm = new Palm(Palm.PalmType.Gold);
            Assert.Equal("Gold Palm", palm.Name);
        }

        [Fact]
        public void SilverPalm_Name()
        {
            var palm = new Palm(Palm.PalmType.Silver);
            Assert.Equal("Silver Palm", palm.Name);
        }

        [Fact]
        public void PalmType_None_Throws()
        {
            Assert.Throws<ArgumentException>(() => new Palm(Palm.PalmType.None));
        }

        [Fact]
        public void Requirements_Has5Items()
        {
            var palm = new Palm(Palm.PalmType.Bronze);
            Assert.Equal(5, palm.Requirements.Count);
        }

        [Fact]
        public void MbReq_IsNotNull()
        {
            var palm = new Palm(Palm.PalmType.Bronze);
            Assert.NotNull(palm.MbReq);
        }

        [Fact]
        public void MbReq_Has5Total_0Required()
        {
            var palm = new Palm(Palm.PalmType.Bronze);
            Assert.Equal(5, palm.MbReq.Total);
            Assert.Equal(0, palm.MbReq.Required);
            Assert.Equal(5, palm.MbReq.Elective);
        }

        [Fact]
        public void Type_SetCorrectly()
        {
            var palm = new Palm(Palm.PalmType.Gold);
            Assert.Equal(Palm.PalmType.Gold, palm.Type);
        }

        [Fact]
        public void Earned_WhenDateSet()
        {
            var palm = new Palm(Palm.PalmType.Bronze, new DateTime(2023, 1, 1));
            Assert.True(palm.Earned);
        }

        [Fact]
        public void NotEarned_ByDefault()
        {
            var palm = new Palm(Palm.PalmType.Bronze);
            Assert.False(palm.Earned);
        }

        [Fact]
        public void Requirements_HasTimeRequirement()
        {
            var palm = new Palm(Palm.PalmType.Bronze);
            var timeReq = palm.Requirements.First(r => r.Name == "1");
            Assert.Equal(3, timeReq.TimeRequirementMonths);
        }
    }
}
