using Xunit;
using advancementchart.Model;

namespace advancement_chart.tests.Model
{
    public class BaseEntityTest
    {
        private class TestEntity : BaseEntity
        {
            public TestEntity(string name, string description) : base(name, description)
            {
            }
        }

        [Fact]
        public void BaseEntity_SetsNameAndDescription()
        {
            var entity = new TestEntity("TestName", "TestDesc");
            Assert.Equal("TestName", entity.Name);
            Assert.Equal("TestDesc", entity.Description);
        }
    }
}
