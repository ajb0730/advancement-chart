using System;
using System.Linq;
using Xunit;
using advancementchart.Model;
using advancementchart.Model.Ranks;

namespace advancement_chart.tests.Model.Ranks
{
    public class EagleTest
    {
        [Fact]
        public void Name_IsEagle()
        {
            var rank = new Eagle();
            Assert.Equal("Eagle", rank.Name);
        }

        [Fact]
        public void Requirements_Has7Items()
        {
            var rank = new Eagle();
            Assert.Equal(7, rank.Requirements.Count);
        }

        [Fact]
        public void MbReq_IsEagleMeritBadgeRequirement()
        {
            var rank = new Eagle();
            Assert.NotNull(rank.MbReq);
            Assert.IsType<EagleMeritBadgeRequirement>(rank.MbReq);
        }

        [Fact]
        public void MbReq_Has21Total()
        {
            var rank = new Eagle();
            Assert.Equal(21, rank.MbReq.Total);
        }

        [Fact]
        public void Requirements_HasTimeRequirement()
        {
            var rank = new Eagle();
            var timeReq = rank.Requirements.First(r => r.Name == "1");
            Assert.Equal(6, timeReq.TimeRequirementMonths);
        }

        [Fact]
        public void Earned_WhenDateSet()
        {
            var rank = new Eagle(new DateTime(2023, 1, 1));
            Assert.True(rank.Earned);
        }

        [Fact]
        public void Requirements_HasEagleProject()
        {
            var rank = new Eagle();
            Assert.Contains(rank.Requirements, r => r.Name == "5" && r.Description.Contains("Eagle Project"));
        }
    }
}
