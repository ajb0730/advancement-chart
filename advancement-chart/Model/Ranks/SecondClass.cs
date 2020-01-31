using System;
using System.Drawing;

namespace advancementchart.Model.Ranks
{
    public class SecondClass : Rank
    {
        public SecondClass(DateTime? earned = null)
            : base(name: "Second Class", description: "", earned: earned, version: "2016")
        {
            Requirements.Add(new RankRequirement(name: "1a", description: "Participate in five outdoor activities since joining", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "1b", description: "Explain Leave No Trace", rank: this, handbookPages: "224-234", curriculumGroup: CurriculumGroup.OutdoorEthics));
            Requirements.Add(new RankRequirement(name: "1c", description: "Select a location for patrol site", rank: this, handbookPages: "265-266", curriculumGroup: CurriculumGroup.Camping2));
            Requirements.Add(new RankRequirement(name: "2a", description: "Explain when it is appropriate to use a fire", rank: this, handbookPages: "223,226,231,266,303-304,387-393", curriculumGroup: CurriculumGroup.FiresAndFireSafety));
            Requirements.Add(new RankRequirement(name: "2b", description: "Prepare tinder, kindling and fuel for a fire", rank: this, handbookPages: "388-392", curriculumGroup: CurriculumGroup.TotinChip));
            Requirements.Add(new RankRequirement(name: "2c", description: "Demonstrate building a fire", rank: this, handbookPages: "388-392", curriculumGroup: CurriculumGroup.FiresAndFireSafety));
            Requirements.Add(new RankRequirement(name: "2d", description: "Demonstrate using a lightweight stove", rank: this, handbookPages: "392-393", curriculumGroup: CurriculumGroup.FiresAndFireSafety));
            Requirements.Add(new RankRequirement(name: "2e", description: "Plan and cook a hot breakfast or lunch", rank: this, handbookPages: "290-308", curriculumGroup: CurriculumGroup.Cooking));
            Requirements.Add(new RankRequirement(name: "2f", description: "Demonstrate the sheet bend knot", rank: this, handbookPages: "370", curriculumGroup: CurriculumGroup.KnotsAndLashings2));
            Requirements.Add(new RankRequirement(name: "2g", description: "Demonstrate the bowline knot", rank: this, handbookPages: "369-370", curriculumGroup: CurriculumGroup.KnotsAndLashings2));
            Requirements.Add(new RankRequirement(name: "3a", description: "Demonstrate using a compass, orient a map, and explain map symbols", rank: this, handbookPages: "332-339,343", curriculumGroup: CurriculumGroup.MapAndCompass1));
            Requirements.Add(new RankRequirement(name: "3b", description: "Using a compass and map, take a 5 mile hike", rank: this, handbookPages: "343-345", curriculumGroup: CurriculumGroup.MapAndCompass1));
            Requirements.Add(new RankRequirement(name: "3c", description: "Describe hazards and injuries of hiking and how to prevent", rank: this, handbookPages: "125-133,142,252", curriculumGroup: CurriculumGroup.MapAndCompass1));
            Requirements.Add(new RankRequirement(name: "3d", description: "Demonstrate finding direction in day and night without compass", rank: this, handbookPages: "354-357", curriculumGroup: CurriculumGroup.MapAndCompass3));
            Requirements.Add(new RankRequirement(name: "4", description: "Identify/show evidence of 10 animals", rank: this, handbookPages: "200-210", curriculumGroup: CurriculumGroup.Nature1));
            Requirements.Add(new RankRequirement(name: "5a", description: "Tell precautions for a safe swim", rank: this, handbookPages: "158-161", curriculumGroup: CurriculumGroup.WaterSafety));
            Requirements.Add(new RankRequirement(name: "5b", description: "Demonstrate swimming ability", rank: this, handbookPages: "161-170", curriculumGroup: CurriculumGroup.WaterSafety));
            Requirements.Add(new RankRequirement(name: "5c", description: "Demonstrate water rescue methods", rank: this, handbookPages: "177-179", curriculumGroup: CurriculumGroup.WaterSafety));
            Requirements.Add(new RankRequirement(name: "5d", description: "Explain why swimming rescues are avoided", rank: this, handbookPages: "180-181", curriculumGroup: CurriculumGroup.WaterSafety));
            Requirements.Add(new RankRequirement(name: "6a", description: "Demonstrate advanced First Aid", rank: this, handbookPages: "114,123-141", curriculumGroup: CurriculumGroup.FirstAidBasics2));
            Requirements.Add(new RankRequirement(name: "6b", description: "Show what to do for 'Hurry Cases'", rank: this, handbookPages: "115-124", curriculumGroup: CurriculumGroup.FirstAidHurryCases));
            Requirements.Add(new RankRequirement(name: "6c", description: "Tell how to prevent problems in 6a & 6b", rank: this, handbookPages: "150-151", curriculumGroup: CurriculumGroup.FirstAidBasics2));
            Requirements.Add(new RankRequirement(name: "6d", description: "Explain what to do in emergencies", rank: this, handbookPages: "111-124,150-154", curriculumGroup: CurriculumGroup.Emergencies));
            Requirements.Add(new RankRequirement(name: "6e", description: "Tell what to do when finding vehicle accident", rank: this, handbookPages: "152", curriculumGroup: CurriculumGroup.Emergencies));
            Requirements.Add(new RankRequirement(name: "7a", description: "Be physically active for four weeks after earning T6c", rank: this, handbookPages: "94-98"));
            Requirements.Add(new RankRequirement(name: "7b", description: "Set goal and make plan to remain physically active", rank: this, handbookPages: "94-98"));
            Requirements.Add(new RankRequirement(name: "7c", description: "Participate in program on dangers of drugs", rank: this, handbookPages: "94-98"));
            Requirements.Add(new RankRequirement(name: "8a", description: "Participate in a flag ceremony", rank: this, handbookPages: "56-61", curriculumGroup: CurriculumGroup.Citizenship));
            Requirements.Add(new RankRequirement(name: "8b", description: "Explain flag respect", rank: this, handbookPages: "58-61", curriculumGroup: CurriculumGroup.Citizenship));
            Requirements.Add(new RankRequirement(name: "8c", description: "Make and follow plan to earn money", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "8d", description: "Compare prices of an item", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "8e", description: "Participate in 2 hour service project", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "9a", description: "Explain three R's of personal safety", rank: this, handbookPages: "28-29,395-409", curriculumGroup: CurriculumGroup.Citizenship));
            Requirements.Add(new RankRequirement(name: "9b", description: "Describe bullying", rank: this, handbookPages: "404-406", curriculumGroup: CurriculumGroup.Citizenship));
            Requirements.Add(new RankRequirement(name: "10", description: "Show Scout spirit", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "11", description: "Scoutmaster conference", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "12", description: "Board of Review", rank: this, handbookPages: ""));
            FillColor = ColorTranslator.FromHtml("#006B3F");
        }
    }
}
