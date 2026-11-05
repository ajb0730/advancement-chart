using System;
using System.Linq;
using Xunit;
using advancementchart.Model;
using advancement_chart.tests.Helpers;

namespace advancement_chart.tests.Model
{
    public class MeritBadgeTest
    {
        [Fact]
        public void Constructor_ValidBadge_SetsProperties()
        {
            var date = new DateTime(2023, 6, 15);
            var badge = new MeritBadge("Camping", "2016", date);
            Assert.Equal("Camping", badge.Name);
            Assert.Equal("2016", badge.Description);
            Assert.Equal(date, badge.DateEarned);
            Assert.True(badge.Earned);
        }

        [Fact]
        public void Constructor_InvalidBadgeName_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                new MeritBadge("FakeBadge", "2016"));
        }

        [Fact]
        public void Constructor_NoDate_NotEarned()
        {
            var badge = new MeritBadge("Camping", "2016");
            Assert.False(badge.Earned);
            Assert.Null(badge.DateEarned);
        }

        [Fact]
        public void BsaId_ReturnsCorrectId()
        {
            var badge = new MeritBadge("Camping", "2016");
            Assert.Equal("001", badge.BsaId);
        }

        [Fact]
        public void BsaId_Swimming()
        {
            var badge = new MeritBadge("Swimming", "2016");
            Assert.Equal("014", badge.BsaId);
        }

        [Fact]
        public void EagleRequired_ForRequiredBadge()
        {
            var badge = new MeritBadge("Camping", "2016");
            Assert.True(badge.EagleRequired);
        }

        [Fact]
        public void EagleRequired_ForElectiveBadge()
        {
            var badge = new MeritBadge("Art", "2016");
            Assert.False(badge.EagleRequired);
        }

        [Fact]
        public void Started_DefaultFalse()
        {
            var badge = new MeritBadge("Camping", "2016");
            Assert.False(badge.Started);
        }

        [Fact]
        public void Started_CanBeSet()
        {
            var badge = new MeritBadge("Camping", "2016");
            badge.Started = true;
            Assert.True(badge.Started);
        }

        [Fact]
        public void GetEagleRequired_ContainsExpectedBadges()
        {
            var required = MeritBadge.GetEagleRequired();
            Assert.Contains("Camping", required);
            Assert.Contains("First Aid", required);
            Assert.Contains("Communication", required);
            Assert.Contains("Personal Fitness", required);
            Assert.Contains("Personal Management", required);
            Assert.Contains("Family Life", required);
            Assert.Contains("Cooking", required);
        }

        [Fact]
        public void GetEagleRequired_ContainsCitSociety_DuringWindow()
        {
            // Citizenship in Society is Eagle-required between 2022-07-01 and 2026-12-31
            var required = MeritBadge.GetEagleRequired();
            Assert.Contains("Citizenship in Society", required);
        }

        [Fact]
        public void GetEagleRequired_RepeatedCalls_ReturnCachedSet()
        {
            Assert.Same(MeritBadge.GetEagleRequired(), MeritBadge.GetEagleRequired());
        }

        [Fact]
        public void GetEagleRequired_DoesNotContainElectives()
        {
            var required = MeritBadge.GetEagleRequired();
            Assert.DoesNotContain("Art", required);
            Assert.DoesNotContain("Chess", required);
        }

        [Theory]
        [InlineData("Emergency Preparedness", new[] { "Lifesaving" })]
        [InlineData("Lifesaving", new[] { "Emergency Preparedness" })]
        [InlineData("Environmental Science", new[] { "Sustainability" })]
        [InlineData("Sustainability", new[] { "Environmental Science" })]
        [InlineData("Swimming", new[] { "Hiking", "Cycling" })]
        [InlineData("Hiking", new[] { "Swimming", "Cycling" })]
        [InlineData("Cycling", new[] { "Swimming", "Hiking" })]
        public void GetEagleEquivalents_Static_ReturnsCorrectEquivalents(string badge, string[] expected)
        {
            var result = MeritBadge.GetEagleEquivalents(badge);
            Assert.Equal(expected.Length, result.Length);
            foreach (var exp in expected)
            {
                Assert.Contains(exp, result);
            }
        }

        [Fact]
        public void GetEagleEquivalents_Static_NonEquivalent_ReturnsEmpty()
        {
            var result = MeritBadge.GetEagleEquivalents("Art");
            Assert.Empty(result);
        }

        [Fact]
        public void GetEagleEquivalents_Static_Camping_ReturnsEmpty()
        {
            var result = MeritBadge.GetEagleEquivalents("Camping");
            Assert.Empty(result);
        }

        [Theory]
        [InlineData("Lifesaving", true)]
        [InlineData("Emergency Preparedness", true)]
        [InlineData("Environmental Science", true)]
        [InlineData("Sustainability", true)]
        [InlineData("Swimming", true)]
        [InlineData("Hiking", true)]
        [InlineData("Cycling", true)]
        [InlineData("Camping", false)]
        [InlineData("Art", false)]
        public void IsMultiple_String_ReturnsCorrect(string badge, bool expected)
        {
            Assert.Equal(expected, MeritBadge.IsMultiple(badge));
        }

        [Fact]
        public void IsMultiple_MeritBadge_Works()
        {
            var badge = new MeritBadge("Swimming", "2016");
            Assert.True(MeritBadge.IsMultiple(badge));
        }

        [Fact]
        public void GetEagleEquivalents_Extension_ReturnsCorrect()
        {
            var badges = new[]
            {
                new MeritBadge("Swimming", "2016"),
                new MeritBadge("Hiking", "2016"),
                new MeritBadge("Cycling", "2016"),
                new MeritBadge("Art", "2016")
            };

            var result = badges.GetEagleEquivalents("Swimming").ToList();
            Assert.Equal(2, result.Count);
            Assert.Contains(result, b => b.Name == "Hiking");
            Assert.Contains(result, b => b.Name == "Cycling");
        }

        [Fact]
        public void GetEagleEquivalents_Extension_NonEquivalent_ReturnsEmpty()
        {
            var badges = new[]
            {
                new MeritBadge("Art", "2016"),
                new MeritBadge("Chess", "2016")
            };
            var result = badges.GetEagleEquivalents("Camping").ToList();
            Assert.Empty(result);
        }

        [Fact]
        public void BsaMeritBadgeIds_IdsAreUnique()
        {
            Assert.Equal(MeritBadge.BsaMeritBadgeIds.Count, MeritBadge.BsaMeritBadgeIds.Values.Distinct().Count());
        }

        [Fact]
        public void LegacyNames_MapToCurrentBadges()
        {
            Assert.All(MeritBadge.LegacyNames, p =>
            {
                Assert.False(MeritBadge.BsaMeritBadgeIds.ContainsKey(p.Key), p.Key);
                Assert.True(MeritBadge.BsaMeritBadgeIds.ContainsKey(p.Value), p.Value);
            });
        }

        [Theory]
        [InlineData("Auto Mechanics", "Automotive Maintenance")]
        [InlineData("Healthcare Professions", "Health Care Professions")]
        [InlineData("  auto mechanics ", "Automotive Maintenance")]
        [InlineData("camping ", "Camping")]
        [InlineData("Health  Care   Professions", "Health Care Professions")]
        public void Constructor_LegacyName_IsStoredUnderCurrentName(string legacy, string current)
        {
            var badge = new MeritBadge(legacy, "2016");
            Assert.Equal(current, badge.Name);
            Assert.Equal(MeritBadge.BsaMeritBadgeIds[current], badge.BsaId);
        }

        [Fact]
        public void CurrentName_NullIsNull_UnknownIsNormalized_NbspAndTabsCollapse()
        {
            Assert.Null(MeritBadge.CurrentName(null));
            Assert.Equal("Not  A Badge".Replace("  ", " "), MeritBadge.CurrentName(" Not  A Badge "));
            Assert.Equal("Health Care Professions", MeritBadge.CurrentName("Health\u00A0Care\tProfessions"));
        }

        [Fact]
        public void AddPartial_LegacyAndCurrentName_DoNotDuplicate()
        {
            var scout = TestFixtures.CreateScout();
            scout.AddPartial("Auto Mechanics", "2016");
            scout.AddPartial("Automotive Maintenance", "2016");
            Assert.Single(scout.MeritBadges, b => b.Name == "Automotive Maintenance");
        }

        [Fact]
        public void GetEagleEquivalents_Extension_EmergencyPrep()
        {
            var badges = new[]
            {
                new MeritBadge("Emergency Preparedness", "2016"),
                new MeritBadge("Lifesaving", "2016"),
                new MeritBadge("Art", "2016")
            };
            var result = badges.GetEagleEquivalents("Emergency Preparedness").ToList();
            Assert.Single(result);
            Assert.Equal("Lifesaving", result[0].Name);
        }

        [Fact]
        public void GetEagleEquivalents_Extension_Lifesaving()
        {
            var badges = new[]
            {
                new MeritBadge("Emergency Preparedness", "2016"),
                new MeritBadge("Lifesaving", "2016")
            };
            var result = badges.GetEagleEquivalents("Lifesaving").ToList();
            Assert.Single(result);
            Assert.Equal("Emergency Preparedness", result[0].Name);
        }

        [Fact]
        public void GetEagleEquivalents_Extension_EnvSci()
        {
            var badges = new[]
            {
                new MeritBadge("Environmental Science", "2016"),
                new MeritBadge("Sustainability", "2016")
            };
            var result = badges.GetEagleEquivalents("Environmental Science").ToList();
            Assert.Single(result);
            Assert.Equal("Sustainability", result[0].Name);
        }

        [Fact]
        public void GetEagleEquivalents_Extension_Sustainability()
        {
            var badges = new[]
            {
                new MeritBadge("Environmental Science", "2016"),
                new MeritBadge("Sustainability", "2016")
            };
            var result = badges.GetEagleEquivalents("Sustainability").ToList();
            Assert.Single(result);
            Assert.Equal("Environmental Science", result[0].Name);
        }

        [Fact]
        public void GetEagleEquivalents_Extension_Hiking()
        {
            var badges = new[]
            {
                new MeritBadge("Swimming", "2016"),
                new MeritBadge("Hiking", "2016"),
                new MeritBadge("Cycling", "2016")
            };
            var result = badges.GetEagleEquivalents("Hiking").ToList();
            Assert.Equal(2, result.Count);
            Assert.Contains(result, b => b.Name == "Swimming");
            Assert.Contains(result, b => b.Name == "Cycling");
        }

        [Fact]
        public void GetEagleEquivalents_Extension_Cycling()
        {
            var badges = new[]
            {
                new MeritBadge("Swimming", "2016"),
                new MeritBadge("Hiking", "2016"),
                new MeritBadge("Cycling", "2016")
            };
            var result = badges.GetEagleEquivalents("Cycling").ToList();
            Assert.Equal(2, result.Count);
            Assert.Contains(result, b => b.Name == "Swimming");
            Assert.Contains(result, b => b.Name == "Hiking");
        }
    }
}
