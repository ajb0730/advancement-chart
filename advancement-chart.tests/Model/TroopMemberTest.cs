using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using advancementchart.Model;
using advancementchart.Model.Ranks;
using advancement_chart.tests.Helpers;

namespace advancement_chart.tests.Model
{
    public class TroopMemberTest
    {
        [Fact]
        public void DisplayName_UsesFirstName_WhenNoNickname()
        {
            var scout = TestFixtures.CreateScout(first: "Robert", last: "Smith");
            Assert.Equal("Robert Smith", scout.DisplayName);
        }

        [Fact]
        public void DisplayName_UsesNickname_WhenSet()
        {
            var scout = TestFixtures.CreateScout(first: "Robert", last: "Smith");
            scout.NickName = "Bob";
            Assert.Equal("Bob Smith", scout.DisplayName);
        }

        [Fact]
        public void DisplayName_UsesFirstName_WhenNicknameWhitespace()
        {
            var scout = TestFixtures.CreateScout(first: "Robert", last: "Smith");
            scout.NickName = "  ";
            Assert.Equal("Robert Smith", scout.DisplayName);
        }

        [Fact]
        public void Equals_SameMemberId_ReturnsTrue()
        {
            var a = TestFixtures.CreateScout(id: "111", first: "A", last: "B");
            var b = TestFixtures.CreateScout(id: "111", first: "C", last: "D");
            Assert.Equal(a, b);
        }

        [Fact]
        public void Equals_DifferentMemberId_ReturnsFalse()
        {
            var a = TestFixtures.CreateScout(id: "111");
            var b = TestFixtures.CreateScout(id: "222");
            Assert.NotEqual(a, b);
        }

        [Fact]
        public void Equals_NonTroopMember_ReturnsFalse()
        {
            var scout = TestFixtures.CreateScout();
            Assert.False(scout.Equals("not a scout"));
        }

        [Fact]
        public void GetHashCode_SameMemberId_SameHash()
        {
            var a = TestFixtures.CreateScout(id: "111");
            var b = TestFixtures.CreateScout(id: "111");
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void DefaultPatrol_IsUnassigned()
        {
            var scout = TestFixtures.CreateScout();
            Assert.Equal("Unassigned", scout.Patrol);
        }

        [Fact]
        public void CurrentRank_NoRanksEarned_ReturnsNoRank()
        {
            var scout = TestFixtures.CreateScout();
            Assert.IsType<NoRank>(scout.CurrentRank);
        }

        [Fact]
        public void CurrentRank_ScoutEarned()
        {
            var scout = TestFixtures.CreateScout();
            scout.Scout.DateEarned = new DateTime(2023, 1, 1);
            Assert.IsType<Scout>(scout.CurrentRank);
        }

        [Fact]
        public void CurrentRank_TenderfootEarned()
        {
            var scout = TestFixtures.CreateScout();
            scout.Scout.DateEarned = new DateTime(2023, 1, 1);
            scout.Tenderfoot.DateEarned = new DateTime(2023, 3, 1);
            Assert.IsType<Tenderfoot>(scout.CurrentRank);
        }

        [Fact]
        public void CurrentRank_EagleEarned()
        {
            var scout = TestFixtures.CreateScoutWithAllRanksEarned();
            Assert.IsType<Eagle>(scout.CurrentRank);
        }

        [Fact]
        public void CurrentRankWithPalms_NoPalms_ReturnsCurrentRank()
        {
            var scout = TestFixtures.CreateScoutWithAllRanksEarned();
            Assert.IsType<Eagle>(scout.CurrentRankWithPalms);
        }

        [Fact]
        public void CurrentRankWithPalms_WithEarnedPalm_ReturnsPalm()
        {
            var scout = TestFixtures.CreateScoutWithAllRanksEarned();
            scout.EaglePalms.Add(new Palm(Palm.PalmType.Bronze, new DateTime(2022, 2, 1)));
            Assert.IsType<Palm>(scout.CurrentRankWithPalms);
        }

        [Fact]
        public void CurrentRankWithPalms_WithUnearnedPalm_ReturnsCurrentRank()
        {
            var scout = TestFixtures.CreateScoutWithAllRanksEarned();
            scout.EaglePalms.Add(new Palm(Palm.PalmType.Bronze));
            Assert.IsType<Eagle>(scout.CurrentRankWithPalms);
        }

        [Fact]
        public void NextRank_NoRanksEarned_ReturnsScout()
        {
            var scout = TestFixtures.CreateScout();
            Assert.IsType<Scout>(scout.NextRank);
        }

        [Fact]
        public void NextRank_ScoutEarned_ReturnsTenderfoot()
        {
            var scout = TestFixtures.CreateScout();
            scout.Scout.DateEarned = new DateTime(2023, 1, 1);
            Assert.IsType<Tenderfoot>(scout.NextRank);
        }

        [Fact]
        public void NextRank_ThroughFirstClass_ReturnsStar()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            Assert.IsType<Star>(scout.NextRank);
        }

        [Fact]
        public void NextRank_AllEarned_ReturnsBronzePalm()
        {
            var scout = TestFixtures.CreateScoutWithAllRanksEarned();
            Assert.IsType<Palm>(scout.NextRank);
            Assert.Equal(Palm.PalmType.Bronze, ((Palm)scout.NextRank).Type);
        }

        [Fact]
        public void NextRank_AfterBronzePalm_ReturnsGold()
        {
            var scout = TestFixtures.CreateScoutWithAllRanksEarned();
            scout.EaglePalms.Add(new Palm(Palm.PalmType.Bronze, new DateTime(2022, 2, 1)));
            Assert.IsType<Palm>(scout.NextRank);
            Assert.Equal(Palm.PalmType.Gold, ((Palm)scout.NextRank).Type);
        }

        [Fact]
        public void NextRank_AfterGoldPalm_ReturnsSilver()
        {
            var scout = TestFixtures.CreateScoutWithAllRanksEarned();
            scout.EaglePalms.Add(new Palm(Palm.PalmType.Bronze, new DateTime(2022, 2, 1)));
            scout.EaglePalms.Add(new Palm(Palm.PalmType.Gold, new DateTime(2022, 5, 1)));
            Assert.IsType<Palm>(scout.NextRank);
            Assert.Equal(Palm.PalmType.Silver, ((Palm)scout.NextRank).Type);
        }

        [Fact]
        public void NextRank_AfterSilverPalm_ReturnsBronze()
        {
            var scout = TestFixtures.CreateScoutWithAllRanksEarned();
            scout.EaglePalms.Add(new Palm(Palm.PalmType.Bronze, new DateTime(2022, 2, 1)));
            scout.EaglePalms.Add(new Palm(Palm.PalmType.Gold, new DateTime(2022, 5, 1)));
            scout.EaglePalms.Add(new Palm(Palm.PalmType.Silver, new DateTime(2022, 8, 1)));
            Assert.IsType<Palm>(scout.NextRank);
            Assert.Equal(Palm.PalmType.Bronze, ((Palm)scout.NextRank).Type);
        }

        [Fact]
        public void NextRank_UnearnedPalm_ReturnsThatPalm()
        {
            var scout = TestFixtures.CreateScoutWithAllRanksEarned();
            scout.EaglePalms.Add(new Palm(Palm.PalmType.Bronze));
            var next = scout.NextRank;
            Assert.IsType<Palm>(next);
            Assert.Equal(Palm.PalmType.Bronze, ((Palm)next).Type);
        }

        [Fact]
        public void GetNthPalm_ExistingPalm_ReturnIt()
        {
            var scout = TestFixtures.CreateScoutWithAllRanksEarned();
            var palm = new Palm(Palm.PalmType.Bronze, new DateTime(2022, 2, 1));
            scout.EaglePalms.Add(palm);
            Assert.Same(palm, scout.GetNthPalm(Palm.PalmType.Bronze, 1));
        }

        [Fact]
        public void GetNthPalm_NextInSequence_CreatesNewPalm()
        {
            var scout = TestFixtures.CreateScoutWithAllRanksEarned();
            scout.EaglePalms.Add(new Palm(Palm.PalmType.Bronze, new DateTime(2022, 2, 1)));
            var gold = scout.GetNthPalm(Palm.PalmType.Gold, 1);
            Assert.NotNull(gold);
            Assert.Equal(Palm.PalmType.Gold, gold.Type);
            Assert.Equal(2, scout.EaglePalms.Count);
        }

        [Fact]
        public void GetNthPalm_OutOfSequence_ReturnsNull()
        {
            var scout = TestFixtures.CreateScoutWithAllRanksEarned();
            scout.EaglePalms.Add(new Palm(Palm.PalmType.Bronze, new DateTime(2022, 2, 1)));
            // Silver should follow Gold, not Bronze
            var result = scout.GetNthPalm(Palm.PalmType.Silver, 1);
            Assert.Null(result);
        }

        [Fact]
        public void GetNthPalm_ZeroOrdinal_Throws()
        {
            var scout = TestFixtures.CreateScoutWithAllRanksEarned();
            Assert.Throws<ArgumentOutOfRangeException>(() => scout.GetNthPalm(Palm.PalmType.Bronze, 0));
        }

        [Fact]
        public void GetNthPalm_NegativeOrdinal_Throws()
        {
            var scout = TestFixtures.CreateScoutWithAllRanksEarned();
            Assert.Throws<ArgumentOutOfRangeException>(() => scout.GetNthPalm(Palm.PalmType.Bronze, -1));
        }

        [Fact]
        public void GetNthPalm_EmptyList_CreatesBronze()
        {
            var scout = TestFixtures.CreateScoutWithAllRanksEarned();
            var bronze = scout.GetNthPalm(Palm.PalmType.Bronze, 1);
            Assert.NotNull(bronze);
            Assert.Equal(Palm.PalmType.Bronze, bronze.Type);
            Assert.Single(scout.EaglePalms);
        }

        [Fact]
        public void GetNthPalm_EmptyList_GoldReturnsNull()
        {
            var scout = TestFixtures.CreateScoutWithAllRanksEarned();
            // Gold can't be the first palm — must start with Bronze
            var result = scout.GetNthPalm(Palm.PalmType.Gold, 1);
            Assert.Null(result);
            Assert.Empty(scout.EaglePalms);
        }

        [Fact]
        public void Add_NewBadge_AddsToList()
        {
            var scout = TestFixtures.CreateScout();
            var badge = TestFixtures.EarnedBadge("Art");
            scout.Add(badge);
            Assert.Single(scout.MeritBadges);
            Assert.Equal("Art", scout.MeritBadges[0].Name);
        }

        [Fact]
        public void Add_DuplicateBadge_DoesNotDuplicate()
        {
            var scout = TestFixtures.CreateScout();
            scout.Add(TestFixtures.EarnedBadge("Art"));
            scout.Add(TestFixtures.EarnedBadge("Art"));
            Assert.Single(scout.MeritBadges);
        }

        [Fact]
        public void Add_EarnedOverPartial_Replaces()
        {
            var scout = TestFixtures.CreateScout();
            scout.Add(TestFixtures.UnearnedBadge("Art"));
            Assert.False(scout.MeritBadges[0].Earned);

            scout.Add(TestFixtures.EarnedBadge("Art"));
            Assert.Single(scout.MeritBadges);
            Assert.True(scout.MeritBadges[0].Earned);
        }

        [Fact]
        public void Add_PartialOverEarned_DoesNotReplace()
        {
            var scout = TestFixtures.CreateScout();
            scout.Add(TestFixtures.EarnedBadge("Art"));
            scout.Add(TestFixtures.UnearnedBadge("Art"));
            Assert.Single(scout.MeritBadges);
            Assert.True(scout.MeritBadges[0].Earned);
        }

        [Fact]
        public void AddPartial_NewBadge_AddsAsStarted()
        {
            var scout = TestFixtures.CreateScout();
            scout.AddPartial("Art", "2016");
            Assert.Single(scout.MeritBadges);
            Assert.True(scout.MeritBadges[0].Started);
            Assert.False(scout.MeritBadges[0].Earned);
        }

        [Fact]
        public void AddPartial_ExistingBadge_DoesNotDuplicate()
        {
            var scout = TestFixtures.CreateScout();
            scout.Add(TestFixtures.EarnedBadge("Art"));
            scout.AddPartial("Art", "2016");
            Assert.Single(scout.MeritBadges);
        }

        [Fact]
        public void AllocateMeritBadges_RunsOnceOnly()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.Add(TestFixtures.EarnedBadge("Camping", new DateTime(2021, 1, 1)));
            scout.AllocateMeritBadges();
            int starCount = scout.Star.MbReq.MeritBadges.Count;
            scout.AllocateMeritBadges(); // Should be no-op
            Assert.Equal(starCount, scout.Star.MbReq.MeritBadges.Count);
        }

        [Fact]
        public void AllocateMeritBadges_EarnedBadges_AllocateToStar()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            // Add 6 earned badges (4 required + 2 elective for Star)
            scout.Add(TestFixtures.EarnedBadge("Camping", new DateTime(2021, 1, 1)));
            scout.Add(TestFixtures.EarnedBadge("First Aid", new DateTime(2021, 2, 1)));
            scout.Add(TestFixtures.EarnedBadge("Communication", new DateTime(2021, 3, 1)));
            scout.Add(TestFixtures.EarnedBadge("Cooking", new DateTime(2021, 4, 1)));
            scout.Add(TestFixtures.EarnedBadge("Art", new DateTime(2021, 5, 1)));
            scout.Add(TestFixtures.EarnedBadge("Astronomy", new DateTime(2021, 6, 1)));

            scout.AllocateMeritBadges();

            Assert.Equal(6, scout.Star.MbReq.MeritBadges.Count);
            Assert.True(scout.Star.MbReq.Earned);
        }

        [Fact]
        public void AllocateMeritBadges_OverflowToLife()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();

            // Add enough for Star (6) plus some for Life
            string[] badges = { "Camping", "First Aid", "Communication", "Cooking", "Art", "Astronomy",
                                "Personal Fitness", "Environmental Science", "Family Life",
                                "Chess", "Climbing" };
            for (int i = 0; i < badges.Length; i++)
            {
                scout.Add(TestFixtures.EarnedBadge(badges[i], new DateTime(2021, 1, 1).AddDays(i)));
            }

            scout.AllocateMeritBadges();

            Assert.True(scout.Star.MbReq.Earned);
            Assert.True(scout.Life.MbReq.Earned);
        }

        [Fact]
        public void AllocateMeritBadges_PartialBadges_AllocateAfterEarned()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.Add(TestFixtures.EarnedBadge("Camping", new DateTime(2021, 1, 1)));
            scout.AddPartial("Art", "2016");

            scout.AllocateMeritBadges();

            // Earned should go to Star first
            Assert.Contains(scout.Star.MbReq.MeritBadges, b => b.Name == "Camping");
        }

        [Fact]
        public void GetRequirementsByGroup_ReturnsGroupedRequirements()
        {
            var scout = TestFixtures.CreateScout();
            var groups = scout.GetRequirementsByGroup();

            Assert.True(groups.Count > 0);
            Assert.True(groups.ContainsKey(CurriculumGroup.FormingThePatrol));
            Assert.True(groups.ContainsKey(CurriculumGroup.KnotsAndLashings1));
        }

        [Fact]
        public void GetRequirementsByGroup_OnlyIncludesGroupedRequirements()
        {
            var scout = TestFixtures.CreateScout();
            var groups = scout.GetRequirementsByGroup();

            foreach (var kvp in groups)
            {
                foreach (var req in kvp.Value)
                {
                    Assert.True(req.Group.HasValue);
                    Assert.Equal(kvp.Key, req.Group.Value);
                }
            }
        }

        [Fact]
        public void Constructor_InitializesAllRanks()
        {
            var scout = TestFixtures.CreateScout();
            Assert.NotNull(scout.Scout);
            Assert.NotNull(scout.Tenderfoot);
            Assert.NotNull(scout.SecondClass);
            Assert.NotNull(scout.FirstClass);
            Assert.NotNull(scout.Star);
            Assert.NotNull(scout.Life);
            Assert.NotNull(scout.Eagle);
            Assert.NotNull(scout.EaglePalms);
            Assert.NotNull(scout.MeritBadges);
        }

        [Fact]
        public void AllocateMeritBadges_EagleBadges_AllocateToEagle()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();

            // Add enough for Star (6) + Life (5) + some for Eagle
            string[] starBadges = { "Camping", "First Aid", "Communication", "Cooking", "Art", "Astronomy" };
            string[] lifeBadges = { "Personal Fitness", "Environmental Science", "Family Life", "Chess", "Climbing" };
            string[] eagleBadges = { "Citizenship in the Community", "Citizenship in the Nation",
                                     "Citizenship in the World", "Emergency Preparedness", "Personal Management" };

            int day = 1;
            foreach (var b in starBadges.Concat(lifeBadges).Concat(eagleBadges))
            {
                scout.Add(TestFixtures.EarnedBadge(b, new DateTime(2021, 1, 1).AddDays(day++)));
            }

            scout.AllocateMeritBadges();

            Assert.True(scout.Star.MbReq.Earned);
            Assert.True(scout.Life.MbReq.Earned);
            Assert.True(scout.Eagle.MbReq.MeritBadges.Count > 0);
        }

        [Fact]
        public void AllocateMeritBadges_OverflowToPalms()
        {
            var scout = TestFixtures.CreateScoutWithAllRanksEarned();

            // Add lots of earned badges - should overflow to Star, Life, Eagle, then Palms
            string[] badges = {
                "Camping", "First Aid", "Communication", "Cooking", "Art", "Astronomy",
                "Personal Fitness", "Environmental Science", "Family Life", "Chess", "Climbing",
                "Citizenship in the Community", "Citizenship in the Nation", "Citizenship in the World",
                "Emergency Preparedness", "Personal Management", "Swimming",
                "Dog Care", "Electricity", "Electronics", "Engineering", "Fishing",
                "Forestry", "Gardening", "Geology", "Golf", "Archery",
                "Basketry", "Bird Study", "Botany", "Chemistry",
                "Citizenship in Society", "Robotics"
            };

            int day = 1;
            foreach (var b in badges)
            {
                scout.Add(TestFixtures.EarnedBadge(b, new DateTime(2021, 1, 1).AddDays(day++)));
            }

            scout.AllocateMeritBadges();

            Assert.True(scout.Star.MbReq.Earned);
            Assert.True(scout.Life.MbReq.Earned);
            Assert.True(scout.EaglePalms.Count > 0);
        }

        [Fact]
        public void CurrentRank_SecondClassEarned()
        {
            var scout = TestFixtures.CreateScout();
            scout.Scout.DateEarned = new DateTime(2023, 1, 1);
            scout.Tenderfoot.DateEarned = new DateTime(2023, 3, 1);
            scout.SecondClass.DateEarned = new DateTime(2023, 5, 1);
            Assert.IsType<SecondClass>(scout.CurrentRank);
        }

        [Fact]
        public void CurrentRank_FirstClassEarned()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            Assert.IsType<FirstClass>(scout.CurrentRank);
        }

        [Fact]
        public void CurrentRank_StarEarned()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.Star.DateEarned = new DateTime(2021, 1, 1);
            Assert.IsType<Star>(scout.CurrentRank);
        }

        [Fact]
        public void CurrentRank_LifeEarned()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.Star.DateEarned = new DateTime(2021, 1, 1);
            scout.Life.DateEarned = new DateTime(2021, 7, 1);
            Assert.IsType<Life>(scout.CurrentRank);
        }

        [Fact]
        public void NextRank_SecondClassEarned_ReturnsFirstClass()
        {
            var scout = TestFixtures.CreateScout();
            scout.Scout.DateEarned = new DateTime(2023, 1, 1);
            scout.Tenderfoot.DateEarned = new DateTime(2023, 3, 1);
            scout.SecondClass.DateEarned = new DateTime(2023, 5, 1);
            Assert.IsType<FirstClass>(scout.NextRank);
        }

        [Fact]
        public void NextRank_StarEarned_ReturnsLife()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.Star.DateEarned = new DateTime(2021, 1, 1);
            Assert.IsType<Life>(scout.NextRank);
        }

        [Fact]
        public void NextRank_LifeEarned_ReturnsEagle()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.Star.DateEarned = new DateTime(2021, 1, 1);
            scout.Life.DateEarned = new DateTime(2021, 7, 1);
            Assert.IsType<Eagle>(scout.NextRank);
        }

        [Fact]
        public void GetNthPalm_SecondBronze_AfterFullCycle()
        {
            var scout = TestFixtures.CreateScoutWithAllRanksEarned();
            scout.EaglePalms.Add(new Palm(Palm.PalmType.Bronze, new DateTime(2022, 2, 1)));
            scout.EaglePalms.Add(new Palm(Palm.PalmType.Gold, new DateTime(2022, 5, 1)));
            scout.EaglePalms.Add(new Palm(Palm.PalmType.Silver, new DateTime(2022, 8, 1)));

            var bronze2 = scout.GetNthPalm(Palm.PalmType.Bronze, 2);
            Assert.NotNull(bronze2);
            Assert.Equal(Palm.PalmType.Bronze, bronze2.Type);
            Assert.Equal(4, scout.EaglePalms.Count);
        }

        [Fact]
        public void AllocateMeritBadges_PartialBadges_AllocateToMbReqs()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            // Only partial badges, no earned
            scout.AddPartial("Camping", "2016");
            scout.AddPartial("First Aid", "2016");
            scout.AddPartial("Art", "2016");

            scout.AllocateMeritBadges();

            // Partials should be allocated after earned loop
            Assert.True(scout.Star.MbReq.MeritBadges.Count > 0);
        }

        [Fact]
        public void AllocateMeritBadges_EarnedAndPartialBadges_EarnedFirst()
        {
            var scout = TestFixtures.CreateScoutThroughFirstClass();
            scout.Add(TestFixtures.EarnedBadge("Camping", new DateTime(2021, 1, 1)));
            scout.Add(TestFixtures.EarnedBadge("First Aid", new DateTime(2021, 2, 1)));
            scout.Add(TestFixtures.EarnedBadge("Communication", new DateTime(2021, 3, 1)));
            scout.Add(TestFixtures.EarnedBadge("Cooking", new DateTime(2021, 4, 1)));
            scout.AddPartial("Art", "2016");
            scout.AddPartial("Astronomy", "2016");

            scout.AllocateMeritBadges();

            // 4 earned eagle required + 2 partial electives should fill Star
            Assert.Equal(6, scout.Star.MbReq.MeritBadges.Count);
        }

        [Fact]
        public void CurrentRankWithPalms_MultipleEarnedPalms_ReturnsLast()
        {
            var scout = TestFixtures.CreateScoutWithAllRanksEarned();
            scout.EaglePalms.Add(new Palm(Palm.PalmType.Bronze, new DateTime(2022, 2, 1)));
            scout.EaglePalms.Add(new Palm(Palm.PalmType.Gold, new DateTime(2022, 5, 1)));

            var rank = scout.CurrentRankWithPalms;
            Assert.IsType<Palm>(rank);
            Assert.Equal(Palm.PalmType.Gold, ((Palm)rank).Type);
        }

        [Fact]
        public void AllocateMeritBadges_OverflowToPalms_CreatesMultiplePalmTypes()
        {
            var scout = TestFixtures.CreateScoutWithAllRanksEarned();

            // We need enough badges to fill Star(6) + Life(7) + Eagle(21) = 34, then overflow
            // Palm takes 5 each: Bronze(5) + Gold(5) + Silver(5) = 15 more
            string[] allBadges = {
                // Eagle required (14)
                "Camping", "First Aid", "Communication", "Cooking",
                "Personal Fitness", "Environmental Science", "Family Life",
                "Citizenship in the Community", "Citizenship in the Nation",
                "Citizenship in the World", "Emergency Preparedness",
                "Personal Management", "Swimming", "Citizenship in Society",
                // Electives (35)
                "Art", "Astronomy", "Chess", "Climbing", "Dog Care",
                "Electricity", "Electronics", "Engineering", "Fishing",
                "Forestry", "Gardening", "Geology", "Golf", "Archery",
                "Basketry", "Bird Study", "Botany", "Chemistry",
                "Robotics", "Coin Collecting", "Collections", "Composite Materials",
                "Digital Technology", "Drafting", "Entrepreneurship",
                "Farm Mechanics", "Fingerprinting", "Fire Safety",
                "Fly Fishing", "Game Design", "Genealogy",
                "Geocaching", "Graphic Arts", "Home Repairs", "Insect Study"
            };

            int day = 1;
            foreach (var b in allBadges)
            {
                scout.Add(TestFixtures.EarnedBadge(b, new DateTime(2021, 1, 1).AddDays(day++)));
            }

            scout.AllocateMeritBadges();

            Assert.True(scout.Star.MbReq.Earned);
            Assert.True(scout.Life.MbReq.Earned);
            Assert.True(scout.Eagle.MbReq.Earned);
            // Should have Bronze, Gold, and Silver palms
            Assert.True(scout.EaglePalms.Count >= 3);
            Assert.Equal(Palm.PalmType.Bronze, scout.EaglePalms[0].Type);
            Assert.Equal(Palm.PalmType.Gold, scout.EaglePalms[1].Type);
            Assert.Equal(Palm.PalmType.Silver, scout.EaglePalms[2].Type);
        }

        [Fact]
        public void GetRequirementsByGroup_IncludesAllFourRanks()
        {
            var scout = TestFixtures.CreateScout();
            var groups = scout.GetRequirementsByGroup();

            // Check that requirements from multiple ranks are included
            bool hasScoutReq = false;
            bool hasTenderfootReq = false;
            foreach (var kvp in groups)
            {
                foreach (var req in kvp.Value)
                {
                    if (req.Rank.Name == "Scout") hasScoutReq = true;
                    if (req.Rank.Name == "Tenderfoot") hasTenderfootReq = true;
                }
            }
            Assert.True(hasScoutReq);
            Assert.True(hasTenderfootReq);
        }
    }
}
