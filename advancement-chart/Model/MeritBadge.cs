using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;

namespace advancementchart.Model
{
    /// <summary>
    /// Helpers for working with collections of <see cref="MeritBadge"/>.
    /// </summary>
    public static class MeritBadgeExtensions
    {
        /// <summary>
        /// The badges in the list that could substitute for <paramref name="badgeName"/> toward an Eagle requirement.
        /// </summary>
        public static IEnumerable<MeritBadge> GetEagleEquivalents(this IEnumerable<MeritBadge> list, string badgeName)
        {
            var equivalents = MeritBadge.GetEagleEquivalents(badgeName);
            return list.Where(x => equivalents.Contains(x.Name));
        }
    }

    /// <summary>
    /// A merit badge a scout has earned or started. <see cref="BaseEntity.Name"/> must be a badge known to BSA
    /// (see <see cref="BsaMeritBadgeIds"/>); <see cref="BaseEntity.Description"/> holds the requirements version from the export.
    /// </summary>
    public class MeritBadge : BaseEntity
    {
        // Names of the Eagle-required merit badges, shared so a typo is a compile error.
        public const string FirstAid = "First Aid";
        public const string CitizenshipCommunity = "Citizenship in the Community";
        public const string CitizenshipNation = "Citizenship in the Nation";
        public const string CitizenshipSociety = "Citizenship in Society";
        public const string CitizenshipWorld = "Citizenship in the World";
        public const string Communication = "Communication";
        public const string Cooking = "Cooking";
        public const string PersonalFitness = "Personal Fitness";
        public const string EmergencyPreparedness = "Emergency Preparedness";
        public const string Lifesaving = "Lifesaving";
        public const string EnvironmentalScience = "Environmental Science";
        public const string Sustainability = "Sustainability";
        public const string PersonalManagement = "Personal Management";
        public const string Swimming = "Swimming";
        public const string Hiking = "Hiking";
        public const string Cycling = "Cycling";
        public const string Camping = "Camping";
        public const string FamilyLife = "Family Life";

        /// <summary>
        /// Groups of Eagle-required badges of which only one fills a required slot; any others earned count as electives (see <see cref="EagleMeritBadgeRequirement.Add"/>).
        /// </summary>
        public static readonly string[][] MutuallyExclusiveGroups = new[]
        {
            new[] { EmergencyPreparedness, Lifesaving },
            new[] { EnvironmentalScience, Sustainability },
            new[] { Swimming, Hiking, Cycling }
        };

        /// <summary>
        /// The other badges in <paramref name="badgeName"/>'s mutually exclusive group, or an empty array if it has none.
        /// </summary>
        static public string[] GetEagleEquivalents(string badgeName)
        {
            foreach (var group in MutuallyExclusiveGroups)
            {
                if (group.Contains(badgeName))
                {
                    return group.Where(name => name != badgeName).ToArray();
                }
            }

            return new string[] { };
        }

        /// <summary>
        /// True if the badge belongs to a group of mutually exclusive Eagle-required badges.
        /// </summary>
        static public bool IsMultiple(string badgeName)
        {
            return MutuallyExclusiveGroups.Any(group => group.Contains(badgeName));
        }

        /// <summary>
        /// True if the badge belongs to a group of mutually exclusive Eagle-required badges.
        /// </summary>
        static public bool IsMultiple(MeritBadge badge)
        {
            return IsMultiple(badge.Name);
        }

        private sealed class EagleRequiredSnapshot
        {
            public DateTime Day;
            public IReadOnlySet<string> Badges;
        }

        private static volatile EagleRequiredSnapshot eagleRequiredCache;

        /// <summary>
        /// The set of Eagle-required badge names. The result depends on today's date (the Citizenship
        /// in Society window) so it is cached per calendar day. The returned set is shared, so it is
        /// exposed read-only.
        /// </summary>
        static public IReadOnlySet<string> GetEagleRequired()
        {
            var now = DateTime.Now;
            var today = now.Date;
            var cached = eagleRequiredCache;
            if (cached != null && cached.Day == today)
            {
                return cached.Badges;
            }
            var badges = BuildEagleRequired(now).ToFrozenSet();
            eagleRequiredCache = new EagleRequiredSnapshot { Day = today, Badges = badges };
            return badges;
        }

        private static HashSet<string> BuildEagleRequired(DateTime now)
        {
            HashSet<string> eagleRequired = new HashSet<string>(){
                Camping,
                CitizenshipCommunity,
                CitizenshipNation,
                CitizenshipWorld,
                Communication,
                Cooking,
                Lifesaving,
                EmergencyPreparedness,
                EnvironmentalScience,
                Sustainability,
                FamilyLife,
                FirstAid,
                PersonalFitness,
                PersonalManagement,
                Swimming,
                Hiking,
                Cycling
            };
            if (EagleMeritBadgeRequirement.IsCitizenshipInSocietyRequired(now))
            {
                eagleRequired.Add(CitizenshipSociety);
            }
            return eagleRequired;
        }

        /// <summary>
        /// Every merit badge name known to this program, mapped to its BSA id (used for compact display and sorting).
        /// </summary>
        static public readonly IReadOnlyDictionary<string, string> BsaMeritBadgeIds = new System.Collections.ObjectModel.ReadOnlyDictionary<string, string>(new Dictionary<string, string>()
        {
            {Camping, "001"},
            {CitizenshipCommunity, "002"},
            {CitizenshipNation, "003"},
            {CitizenshipWorld, "004"},
            {Communication, "005"},
            {EmergencyPreparedness, "006"},
            {EnvironmentalScience, "007"},
            {FirstAid, "008"},
            {Lifesaving, "009"},
            {PersonalFitness, "010"},
            {PersonalManagement, "011"},
            {"Safety", "012"},
            {"Sports", "013"},
            {Swimming, "014"},
            {"American Business", "015"},
            {"American Heritage", "016"},
            {"American Cultures", "017"},
            {"Animal Science", "018"},
            {"Archery", "019"},
            {"Architecture", "020"},
            {"Art", "021"},
            {"Astronomy", "022"},
            {"Athletics", "023"},
            {"Nuclear Science", "024"},
            {"Aviation", "025"},
            {"Backpacking", "026"},
            {"Basketry", "027"},
            {"Beekeeping", "028"},
            {"Bird Study", "029"},
            {"Bookbinding", "030"},
            {"Botany", "031"},
            {"Bugling", "032"},
            {"Canoeing", "033"},
            {"Chemistry", "034"},
            {"Coin Collecting", "035"},
            {"Consumer Buying", "037"},
            {Cooking, "038"},
            {Cycling, "039"},
            {"Dentistry", "040"},
            {"Dog Care", "041"},
            {"Drafting", "042"},
            {"Electricity", "043"},
            {"Electronics", "044"},
            {"Energy", "045"},
            {"Engineering", "046"},
            {"Farm and Ranch Management", "047"},
            {"Farm Mechanics", "048"},
            {"Fingerprinting", "049"},
            {"Fire Safety", "050"},
            {"Fish and Wildlife Management", "051"},
            {"Fishing", "052"},
            {"Food Systems", "053"},
            {"Forestry", "054"},
            {"Gardening", "055"},
            {"Genealogy", "056"},
            {"General Science", "057"},
            {"Geology", "058"},
            {"Golf", "059"},
            {"Disabilities Awareness", "060"},
            {Hiking, "061"},
            {"Home Repairs", "062"},
            {"Horsemanship", "063"},
            {"American Indian Culture", "064"},
            {"Insect Study", "065"},
            {"Journalism", "066"},
            {"Landscape Architecture", "067"},
            {"Law", "068"},
            {"Leatherwork", "069"},
            {"Machinery", "070"},
            {"Mammal Study", "071"},
            {"Masonry", "072"},
            {"Metals Engineering", "073"},
            {"Metalwork", "074"},
            {"Model Design and Building", "075"},
            {"Motorboating", "076"},
            {"Music", "077"},
            {"Nature", "078"},
            {"Oceanography", "079"},
            {"Orienteering", "080"},
            {"Painting", "081"},
            {"Pets", "082"},
            {"Photography", "083"},
            {"Pioneering", "084"},
            {"Plant Science", "085"},
            {"Plumbing", "086"},
            {"Pottery", "087"},
            {"Printing", "088"},
            {"Public Health", "089"},
            {"Public Speaking", "090"},
            {"Pulp and Paper", "091"},
            {"Rabbit Raising", "092"},
            {"Radio", "093"},
            {"Railroading", "094"},
            {"Reading", "095"},
            {"Reptile and Amphibian Study", "096"},
            {"Rifle and Shotgun Shooting", "097"},
            {"Rowing", "098"},
            {"Salesmanship", "099"},
            {"Scholarship", "100"},
            {"Sculpture", "101"},
            {"Skating", "103"},
            {"Skiing", "104"},
            {"Small-Boat Sailing", "105"},
            {"Soil and Water Conservation", "106"},
            {"Space Exploration", "107"},
            {"Stamp Collecting", "108"},
            {"Surveying", "109"},
            {"Textile", "110"},
            {"Theater", "111"},
            {"Traffic Safety", "112"},
            {"Truck Transportation", "113"},
            {"Veterinary Medicine", "114"},
            {"Water Sports", "115"},
            {"Weather", "116"},
            {"Wilderness Survival", "117"},
            {"Wood Carving", "118"},
            {"Woodwork", "119"},
            {"Agribusiness", "120"},
            {"American Labor", "121"},
            {"Graphic Arts", "122"},
            {"Rifle Shooting", "123"},
            {"Shotgun Shooting", "124"},
            {"Whitewater", "125"},
            {"Automotive Maintenance", "127"}, // renamed from Auto Mechanics; see LegacyNames
            {"Collections", "128"},
            {FamilyLife, "129"},
            {"Medicine", "130"},
            {"Crime Prevention", "131"},
            {"Archaeology", "132"},
            {"Climbing", "133"},
            {"Entrepreneurship", "134"},
            {"Snow Sports", "135"},
            {"Fly Fishing", "136"},
            {"Composite Materials", "137"},
            {"Scuba Diving", "138"},
            {"Carpentry", "139"},
            {"Pathfinding", "140"},
            {"Signaling", "141"},
            {"Tracking", "142"},
            {"Scouting Heritage", "143"},
            {"Inventing", "144"},
            {"Geocaching", "145"},
            {"Robotics", "146"},
            {"Chess", "147"},
            {"Welding", "148"},
            {"Kayaking", "149"},
            {"Search and Rescue", "150"},
            {"Game Design", "151"},
            {Sustainability, "152"},
            {"Programming", "153"},
            {"Digital Technology", "154"},
            {"Mining in Society", "155"},
            {"Moviemaking", "156"},
            {"Signs, Signals, and Codes", "157"},
            {"Animation", "158"},
            {"Exploration", "159"},
            {CitizenshipSociety, "160"},
            {"Health Care Professions", "161"},
            // 162 is unused (the old Automotive Maintenance id; the badge kept Auto Mechanics' id 127).
            {"Multisport", "163"},
            {"Artificial Intelligence", "164"},
            {"Cybersecurity", "165"},
            // 166+ are locally assigned, not BSA numbers: the ids only need to be unique.
            {"Competitive Gaming", "166"},
            {"Wildland Fire Management", "167"}
        });

        /// <summary>
        /// Former names of renamed badges, mapped to the current name. Older Scoutbook exports still use them.
        /// </summary>
        static public readonly IReadOnlyDictionary<string, string> LegacyNames = new System.Collections.ObjectModel.ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"Auto Mechanics", "Automotive Maintenance"},
            {"Healthcare Professions", "Health Care Professions"},
            {"Cinematography", "Moviemaking"},
            {"Computers", "Digital Technology"},
            {"Signalling", "Signaling"}
        });

        private static readonly IReadOnlyDictionary<string, string> CanonicalNames = BsaMeritBadgeIds.Keys.ToFrozenDictionary(k => k, StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Returns the canonical name for a badge name: surrounding whitespace is trimmed, case is ignored, and a legacy name
        /// maps to its replacement. A name that is not recognized is returned trimmed. A badge keeps its current id, so historical badges under a legacy name report the current badge's id.
        /// </summary>
        public static string CurrentName(string name)
        {
            if (name is null)
            {
                return null;
            }
            var trimmed = name.Trim();
            if (LegacyNames.TryGetValue(trimmed, out var current) || CanonicalNames.TryGetValue(trimmed, out current))
            {
                return current;
            }
            return trimmed;
        }

        protected MeritBadge()
            : base()
        { }

        /// <summary>
        /// Creates a merit badge.
        /// </summary>
        /// <param name="name">The badge name; must be a key of <see cref="BsaMeritBadgeIds"/> or of <see cref="LegacyNames"/> (stored under the current name).</param>
        /// <param name="description">Free text; the importer stores the requirements version here.</param>
        /// <param name="earned">When it was earned; null for a badge that is only started.</param>
        /// <exception cref="ArgumentException"><paramref name="name"/> is not a known merit badge.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is null.</exception>
        public MeritBadge(string name, string description, DateTime? earned = null)
            : base(CurrentName(name), description)
        {
            if (!BsaMeritBadgeIds.ContainsKey(Name))
            {
                throw new ArgumentException(message: $"Merit Badge name '{name}' not found.");
            }
            DateEarned = earned;
        }

        /// <summary>
        /// When the badge was earned, or null.
        /// </summary>
        public DateTime? DateEarned { get; protected set; }
        /// <summary>
        /// True if the badge is currently Eagle-required (see <see cref="GetEagleRequired"/>).
        /// </summary>
        public bool EagleRequired => GetEagleRequired().Contains(Name);
        /// <summary>
        /// The badge's BSA id.
        /// </summary>
        public string BsaId => BsaMeritBadgeIds[Name];
        /// <summary>
        /// True if the scout has started, but not finished, the badge.
        /// </summary>
        public bool Started { get; set; }

        /// <summary>
        /// True once the badge has a <see cref="DateEarned"/>.
        /// </summary>
        public bool Earned => DateEarned.HasValue;
    }
}
