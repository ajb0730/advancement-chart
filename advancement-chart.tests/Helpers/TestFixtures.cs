using System;
using System.Linq;
using advancementchart.Model;
using advancementchart.Model.Ranks;

namespace advancement_chart.tests.Helpers
{
    public static class TestFixtures
    {
        public static MeritBadge EarnedBadge(string name = "Camping", DateTime? date = null)
        {
            return new MeritBadge(name: name, description: "2016", earned: date ?? new DateTime(2023, 6, 15));
        }

        public static MeritBadge UnearnedBadge(string name = "Camping")
        {
            return new MeritBadge(name: name, description: "2016", earned: null);
        }

        public static MeritBadge StartedBadge(string name = "Camping")
        {
            var badge = new MeritBadge(name: name, description: "2016", earned: null);
            badge.Started = true;
            return badge;
        }

        public static TroopMember CreateScout(string id = "123", string first = "John", string last = "Doe")
        {
            return new TroopMember(memberId: id, firstName: first, middleName: "", lastName: last);
        }

        public static TroopMember CreateScoutWithAllRanksEarned()
        {
            var scout = CreateScout();
            scout.Scout.DateEarned = new DateTime(2020, 1, 1);
            scout.Tenderfoot.DateEarned = new DateTime(2020, 3, 1);
            scout.SecondClass.DateEarned = new DateTime(2020, 5, 1);
            scout.FirstClass.DateEarned = new DateTime(2020, 7, 1);
            scout.Star.DateEarned = new DateTime(2020, 11, 1);
            scout.Life.DateEarned = new DateTime(2021, 5, 1);
            scout.Eagle.DateEarned = new DateTime(2021, 11, 1);
            return scout;
        }

        public static TroopMember CreateScoutThroughFirstClass()
        {
            var scout = CreateScout();
            scout.Scout.DateEarned = new DateTime(2020, 1, 1);
            scout.Tenderfoot.DateEarned = new DateTime(2020, 3, 1);
            scout.SecondClass.DateEarned = new DateTime(2020, 5, 1);
            scout.FirstClass.DateEarned = new DateTime(2020, 7, 1);
            return scout;
        }

        public static string[] EagleRequiredBadgeNames = new[]
        {
            "Camping", "Citizenship in the Community", "Citizenship in the Nation",
            "Citizenship in the World", "Communication", "Cooking",
            "First Aid", "Personal Fitness", "Emergency Preparedness",
            "Environmental Science", "Personal Management", "Swimming",
            "Family Life"
        };

        public static string[] ElectiveBadgeNames = new[]
        {
            "Art", "Astronomy", "Athletics", "Archery", "Basketry",
            "Bird Study", "Botany", "Chemistry", "Chess", "Climbing",
            "Dog Care", "Electricity", "Electronics", "Engineering",
            "Fishing", "Forestry", "Gardening", "Geology", "Golf"
        };
    }
}
