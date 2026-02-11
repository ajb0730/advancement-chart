using System;
using Xunit;
using advancementchart.Model;

namespace advancement_chart.tests.Model
{
    public class BaseEntityTest
    {
        [Fact]
        public void VersionedEntity_SetsNameAndDescription()
        {
            var entity = new VersionedEntity("TestName", "TestDesc");
            Assert.Equal("TestName", entity.Name);
            Assert.Equal("TestDesc", entity.Description);
        }

        [Fact]
        public void VersionedEntity_DefaultVersion()
        {
            var entity = new VersionedEntity("Name", "Desc");
            Assert.Equal("2016", entity.Version);
        }

        [Fact]
        public void VersionedEntity_CustomVersion()
        {
            var entity = new VersionedEntity("Name", "Desc", "2020");
            Assert.Equal("2020", entity.Version);
        }
    }
}
