﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace advancementchart.Model
{
    public static class MeritBadgeExtensions
    {
        public static IEnumerable<MeritBadge> GetEagleEquivalents(this IEnumerable<MeritBadge> list, string badgeName)
        {
            if (badgeName == "Emergency Preparedness")
            {
                return list.Where(x => x.Name == "Lifesaving");
            }
            else if (badgeName == "Lifesaving")
            {
                return list.Where(x => x.Name == "Emergency Preparedness");
            }
            else if (badgeName == "Environmental Science")
            {
                return list.Where(x => x.Name == "Sustainability");
            }
            else if (badgeName == "Sustainability")
            {
                return list.Where(x => x.Name == "Environmental Science");
            }
            else if (badgeName == "Swimming")
            {
                return list.Where(x => x.Name == "Hiking" || x.Name == "Cycling");
            }
            else if (badgeName == "Hiking")
            {
                return list.Where(x => x.Name == "Swimming" || x.Name == "Cycling");
            }
            else if (badgeName == "Cycling")
            {
                return list.Where(x => x.Name == "Swimming" || x.Name == "Hiking");
            }
            else
            {
                return list.Where(x => false);
            }
        }
    }

    public class MeritBadge : BaseEntity
    {

        static public string[] GetEagleEquivalents(string badgeName)
        {
            if (badgeName == "Environmental Science")
            {
                return new string[] { "Sustainability" };
            }
            else if (badgeName == "Sustainability")
            {
                return new string[] { "Environmental Science" };
            }
            else if (badgeName == "Lifesaving")
            {
                return new string[] { "Emergency Preparedness" };
            }
            else if (badgeName == "Emergency Preparedness")
            {
                return new string[] { "Lifesaving" };
            }
            else if (badgeName == "Swimming")
            {
                return new string[] { "Hiking", "Cycling" };
            }
            else if (badgeName == "Hiking")
            {
                return new string[] { "Swimming", "Cycling" };
            }
            else if (badgeName == "Cycling")
            {
                return new string[] { "Swimming", "Hiking" };
            }

            return new string[] { };
        }

        static public bool IsMultiple(string badgeName)
        {
            return badgeName == "Lifesaving" ||
                badgeName == "Emergency Preparedness" ||
                badgeName == "Environmental Science" ||
                badgeName == "Sustainability" ||
                badgeName == "Swimming" ||
                badgeName == "Hiking" ||
                badgeName == "Cycling";
        }

        static public bool IsMultiple(MeritBadge badge)
        {
            return IsMultiple(badge.Name);
        }

        static public HashSet<string> GetEagleRequired()
        {
            HashSet<string> eagleRequired = new HashSet<string>(){
                "Camping",
                "Citizenship in the Community",
                "Citizenship in the Nation",
                "Citizenship in the World",
                "Communication",
                "Cooking",
                "Lifesaving",
                "Emergency Preparedness",
                "Environmental Science",
                "Sustainability",
                "Family Life",
                "First Aid",
                "Personal Fitness",
                "Personal Management",
                "Swimming",
                "Hiking",
                "Cycling"
            };
            if (DateTime.Now >= EagleMeritBadgeRequirement.CitSocietyCutover)
            {
                eagleRequired.Add("Citizenship in Society");
            }
            return eagleRequired;
        }

        static public readonly Dictionary<string, string> BsaMeritBadgeIds = new Dictionary<string, string>()
        {
            {"Camping", "001"},
            {"Citizenship in the Community", "002"},
            {"Citizenship in the Nation", "003"},
            {"Citizenship in the World", "004"},
            {"Communication", "005"},
            {"Emergency Preparedness", "006"},
            {"Environmental Science", "007"},
            {"First Aid", "008"},
            {"Lifesaving", "009"},
            {"Personal Fitness", "010"},
            {"Personal Management", "011"},
            {"Safety", "012"},
            {"Sports", "013"},
            {"Swimming", "014"},
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
            {"Computers", "036"},
            {"Consumer Buying", "037"},
            {"Cooking", "038"},
            {"Cycling", "039"},
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
            {"Hiking", "061"},
            {"Home Repairs", "062"},
            {"Horsemanship", "063"},
            {"Indian Lore", "064"},
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
            {"Signalling", "102"},
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
            {"Cinematography", "126"},
            {"Auto Mechanics", "127"},
            {"Collections", "128"},
            {"Family Life", "129"},
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
            {"Sustainability", "152"},
            {"Programming", "153"},
            {"Digital Technology", "154"},
            {"Mining in Society", "155"},
            {"Moviemaking", "156"},
            {"Signs, Signals, and Codes", "157"},
            {"Animation", "158"},
            {"Exploration", "159"},
            {"Citizenship in Society", "160"},
            {"Healthcare Professions", "161"}
        };

        protected MeritBadge()
            : base()
        { }

        public MeritBadge(string name, string description, DateTime? earned = null)
            : base(name, description)
        {
            if (!BsaMeritBadgeIds.ContainsKey(name))
            {
                throw new ArgumentException(message: $"Merit Badge name '{name}' not found.");
            }
            DateEarned = earned;
        }

        public DateTime? DateEarned { get; protected set; }
        public bool EagleRequired => GetEagleRequired().Contains(Name);
        public string BsaId => BsaMeritBadgeIds[Name];
        public bool Started { get; set; }

        public bool Earned => DateEarned.HasValue;
    }
}
