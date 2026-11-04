using System.Linq;
using Xunit;
using advancementchart.Model;
using advancementchart.Model.Ranks;

namespace advancement_chart.tests.Model
{
    public class ScoutingConstantsTest
    {
        [Fact]
        public void RankTimeRequirements_UseTheNamedConstants()
        {
            Assert.Equal(ScoutingConstants.StarActiveMonths, new Star().Requirements.First(r => r.Name == "1").TimeRequirementMonths);
            Assert.Equal(ScoutingConstants.LifeActiveMonths, new Life().Requirements.First(r => r.Name == "1").TimeRequirementMonths);
            Assert.Equal(ScoutingConstants.EagleActiveMonths, new Eagle().Requirements.First(r => r.Name == "1").TimeRequirementMonths);
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
