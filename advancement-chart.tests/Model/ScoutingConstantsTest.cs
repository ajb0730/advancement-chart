using System.Linq;
using Xunit;
using advancementchart.Model;
using advancementchart.Model.Ranks;

namespace advancement_chart.tests.Model
{
    public class ScoutingConstantsTest
    {
        [Fact]
        public void Constants_HaveTheBsaValues()
        {
            Assert.Equal(4, ScoutingConstants.StarActiveMonths);
            Assert.Equal(6, ScoutingConstants.LifeActiveMonths);
            Assert.Equal(6, ScoutingConstants.EagleActiveMonths);
            Assert.Equal(3, ScoutingConstants.PalmActiveMonths);
            Assert.Equal(84, ScoutingConstants.PersonalFitnessDays);
            Assert.Equal(91, ScoutingConstants.PersonalManagementDays);
            Assert.Equal(90, ScoutingConstants.FamilyLifeDays);
            Assert.Equal(45, ScoutingConstants.Tenderfoot6cWarningDays);
        }

        [Fact]
        public void RankTimeRequirements_ArePresentAndUseTheNamedConstants()
        {
            // AdvancementCheck reads these months from requirement 1, so each must exist.
            Assert.Equal(ScoutingConstants.StarActiveMonths, new Star().Requirements.First(r => r.Name == "1").TimeRequirementMonths);
            Assert.Equal(ScoutingConstants.LifeActiveMonths, new Life().Requirements.First(r => r.Name == "1").TimeRequirementMonths);
            Assert.Equal(ScoutingConstants.EagleActiveMonths, new Eagle().Requirements.First(r => r.Name == "1").TimeRequirementMonths);
            Assert.Equal(ScoutingConstants.PalmActiveMonths,
                new Palm(Palm.PalmType.Bronze).Requirements.First(r => r.Name == "1").TimeRequirementMonths);
        }

        [Fact]
        public void RequirementDescriptions_MentionTheConstantMonths()
        {
            Assert.Contains($"{ScoutingConstants.StarActiveMonths} months", new Star().Requirements.First(r => r.Name == "1").Description);
            Assert.Contains($"{ScoutingConstants.LifeActiveMonths} months", new Life().Requirements.First(r => r.Name == "1").Description);
            Assert.Contains($"{ScoutingConstants.EagleActiveMonths} months", new Eagle().Requirements.First(r => r.Name == "1").Description);
        }

        [Fact]
        public void BadgeNameConstants_AreKnownToBsa()
        {
            // Every Eagle-required badge name constant must have a BSA id, or BsaId lookups would throw.
            string[] names =
            {
                MeritBadge.FirstAid, MeritBadge.CitizenshipCommunity, MeritBadge.CitizenshipNation,
                MeritBadge.CitizenshipSociety, MeritBadge.CitizenshipWorld, MeritBadge.Communication,
                MeritBadge.Cooking, MeritBadge.PersonalFitness, MeritBadge.EmergencyPreparedness,
                MeritBadge.Lifesaving, MeritBadge.EnvironmentalScience, MeritBadge.Sustainability,
                MeritBadge.PersonalManagement, MeritBadge.Swimming, MeritBadge.Hiking, MeritBadge.Cycling,
                MeritBadge.Camping, MeritBadge.FamilyLife,
            };
            Assert.All(names, n => Assert.True(MeritBadge.BsaMeritBadgeIds.ContainsKey(n), n));
        }
    }
}
