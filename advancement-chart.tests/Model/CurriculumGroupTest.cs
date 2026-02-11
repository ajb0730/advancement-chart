using Xunit;
using advancementchart.Model;

namespace advancement_chart.tests.Model
{
    public class CurriculumGroupTest
    {
        [Theory]
        [InlineData(CurriculumGroup.Camping1, "Camping I")]
        [InlineData(CurriculumGroup.Camping2, "Camping II")]
        [InlineData(CurriculumGroup.FirstAidBasics1, "First Aid : Basics I")]
        [InlineData(CurriculumGroup.KnotsAndLashings1, "Knots and Lashings I")]
        [InlineData(CurriculumGroup.OutdoorEthics, "Outdoor Ethics")]
        [InlineData(CurriculumGroup.WaterSafety, "Water Safety")]
        [InlineData(CurriculumGroup.TotinChip, "Totin' Chip")]
        [InlineData(CurriculumGroup.MapAndCompass1, "Map and Compass I")]
        [InlineData(CurriculumGroup.FiresAndFireSafety, "Fires and Fire Safety")]
        public void GetDisplayName_ReturnsDisplayAttribute(CurriculumGroup group, string expected)
        {
            Assert.Equal(expected, group.GetDisplayName());
        }

        [Fact]
        public void GetDisplayName_AllGroupsReturnNonEmpty()
        {
            foreach (CurriculumGroup group in System.Enum.GetValues(typeof(CurriculumGroup)))
            {
                string name = group.GetDisplayName();
                Assert.False(string.IsNullOrEmpty(name), $"{group} returned empty display name");
            }
        }
    }
}
