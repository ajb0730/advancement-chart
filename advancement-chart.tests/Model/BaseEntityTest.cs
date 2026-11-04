using System;
using Xunit;
using advancementchart.Model;

namespace advancement_chart.tests.Model
{
    public class BaseEntityTest
    {
        [Fact]
        public void RankRequirement_SetsNameAndDescription()
        {
            var rank = new Rank("TestRank", "");
            var entity = new RankRequirement("TestName", "TestDesc", rank);
            Assert.Equal("TestName", entity.Name);
            Assert.Equal("TestDesc", entity.Description);
        }
    }
}
