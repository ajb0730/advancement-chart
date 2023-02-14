using System;
using System.Drawing;
using System.Linq;

namespace advancementchart.Model.Ranks
{
    public class SecondClass : Rank
    {
        public SecondClass(DateTime? earned = null) :
            base(
                name: "Second Class",
                description: "",
                earned: earned,
                version: "2016"
            )
        {
            FillColor = ColorTranslator.FromHtml("#006B3F");
        }

        public override void SwitchVersion(string version)
        {
            this.Version = version;
            if (Requirements.Any())
            {
                Requirements.Clear();
            }
            switch (version)
            {
                case "2016":
                    Requirements
                        .Add(new RankRequirement(name: "1a",
                            tag: "Participate in five outdoor activities since joining",
                            description: "Since joining, participate in five separate troop/patrol activities, three of which include overnight camping. These five activities do not include troop or patrol meetings. On at least two of the three campouts, spend the night in a tent that you pitch or other structure that you help erect (such as a lean-to, snow cave, or tepee.)",
                            rank: this,
                            handbookPages: "",
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "1b",
                            tag: "Explain Leave No Trace",
                            description: "Explain the principles of Leave No Trace, and tell how you practiced them while on a campout or outing. This outing must be different from the one used for Tenderfoot requirement 1c.",
                            rank: this,
                            handbookPages: "224-234",
                            curriculumGroup: CurriculumGroup.OutdoorEthics,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "1c",
                            tag: "Select a location for patrol site",
                            description: "On one of these campouts, select a location for your patrol site and recommend it to your patrol leader, senior patrol leader, or troop guide. Explain what factors you should consider when choosing a patrol site and where to pitch a tent.",
                            rank: this,
                            handbookPages: "265-266",
                            curriculumGroup: CurriculumGroup.Camping2,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "2a",
                            tag: "Explain when it is appropriate to use a fire",
                            description: "Explain when it is appropriate to use a fire for cooking or other purposes and when it would not be appropriate to do so.",
                            rank: this,
                            handbookPages: "223,226,231,266,303-304,387-393",
                            curriculumGroup: CurriculumGroup
                                .FiresAndFireSafety,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "2b",
                            tag: "Prepare tinder, kindling and fuel for a fire",
                            description: "Use the tools listed in Tenderfoot requirement 3d to prepare tinder, kindling, and fuel wood for a cooking fire.",
                            rank: this,
                            handbookPages: "388-392",
                            curriculumGroup: CurriculumGroup.TotinChip,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "2c",
                            tag: "Demonstrate building a fire",
                            description: "At an approved outdoor location and time, use the tinder, kindling, and fuel wood from Second Class requirement 2b to demonstrate how to build a fire. Unless prohibited by local fire restrictions, light the fire. After allowing the flames to burn safely for at least two minutes, safely extinguish the flames with minimal impact to the fire site.",
                            rank: this,
                            handbookPages: "388-392",
                            curriculumGroup: CurriculumGroup
                                .FiresAndFireSafety,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "2d",
                            tag: "Demonstrate using a lightweight stove",
                            description: "Explain when it is appropriate to use a lightweight stove and when it is appropriate to use a propane stove. Set up a lightweight stove or propane stove. Light the stove, unless prohibited by local fire restrictions. Describe the safety procedures for using these types of stoves.",
                            rank: this,
                            handbookPages: "392-393",
                            curriculumGroup: CurriculumGroup
                                .FiresAndFireSafety,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "2e",
                            tag: "Plan and cook a hot breakfast or lunch",
                            description: "On one campout, plan and cook one hot breakfast or lunch, selecting foods from MyPlate or the current USDA nutrition model. Explain the importance of good nutrition. Demonstrate how to transport, store, and prepare the foods you selected.",
                            rank: this,
                            handbookPages: "290-308",
                            curriculumGroup: CurriculumGroup.Cooking,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "2f",
                            tag: "Demonstrate the sheet bend knot",
                            description: "Demonstrate how to tie the sheet bend knot. Describe a situation in which you would use this knot.",
                            rank: this,
                            handbookPages: "370",
                            curriculumGroup: CurriculumGroup
                                .KnotsAndLashings2,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "2g",
                            tag: "Demonstrate the bowline knot",
                            description: "Demonstrate how to tie the bowline knot. Describe a situation in which you would use this knot.",
                            rank: this,
                            handbookPages: "369-370",
                            curriculumGroup: CurriculumGroup
                                .KnotsAndLashings2,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "3a",
                            tag: "Demonstrate using a compass, orient a map, and explain map symbols",
                            description: "Demonstrate how a compass works and how to orient a map. Use a map to point out and tell the meaning of five map symbols.",
                            rank: this,
                            handbookPages: "332-339,343",
                            curriculumGroup: CurriculumGroup.MapAndCompass1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "3b",
                            tag: "Using a compass and map, take a 5 mile hike",
                            description: "Using a compass and a map together, take a five-mile hike (or 10 miles by bike) approved by your adult leader and your parent or guardian.",
                            rank: this,
                            handbookPages: "343-345",
                            curriculumGroup: CurriculumGroup.MapAndCompass1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "3c",
                            tag: "Describe hazards and injuries of hiking and how to prevent",
                            description: "Describe some hazards or injuries that you might encounter on your hike and what you can do to help prevent them.",
                            rank: this,
                            handbookPages: "125-133,142,252",
                            curriculumGroup: CurriculumGroup.MapAndCompass1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "3d",
                            tag: "Demonstrate finding direction in day and night without compass",
                            description: "Demonstrate how to find directions during the day and at night without using a compass or an electronic device.",
                            rank: this,
                            handbookPages: "354-357",
                            curriculumGroup: CurriculumGroup.MapAndCompass3,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "4",
                            tag: "Identify/show evidence of 10 animals",
                            description: "Identify or show evidence of at least ten kinds of wild animals (such as birds, mammals, reptiles, fish, mollusks) found in your local area or camping location. You may show evidence by tracks, signs, or photographs you have taken.",
                            rank: this,
                            handbookPages: "200-210",
                            curriculumGroup: CurriculumGroup.Nature1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "5a",
                            tag: "Tell precautions for a safe swim",
                            description: "Tell what precautions must be taken for a safe swim.",
                            rank: this,
                            handbookPages: "158-161",
                            curriculumGroup: CurriculumGroup.WaterSafety,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "5b",
                            tag: "Demonstrate swimming ability",
                            description: "Demonstrate your ability to pass the BSA beginner test.  Jump feetfirst into water over your head in depth, level off and swim 25 feet on the surface, stop, turn sharply, resume swimming, then return to your starting place.",
                            rank: this,
                            handbookPages: "161-170",
                            curriculumGroup: CurriculumGroup.WaterSafety,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "5c",
                            tag: "Demonstrate water rescue methods",
                            description: "Demonstrate water rescue methods by reaching with your arm or leg, by reaching with a suitable object, and by throwing lines and objects.",
                            rank: this,
                            handbookPages: "177-179",
                            curriculumGroup: CurriculumGroup.WaterSafety,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "5d",
                            tag: "Explain why swimming rescues are avoided",
                            description: "Explain why swimming rescues should not be attempted when a reaching or throwing rescue is possible. Explain why and how a rescue swimmer should avoid contact with the victim.",
                            rank: this,
                            handbookPages: "180-181",
                            curriculumGroup: CurriculumGroup.WaterSafety,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "6a",
                            tag: "Demonstrate advanced First Aid",
                            description: "Demonstrate first aid for the following: Object in the eye; Bite of a warm blooded animal; Puncture wounds from a splinter, nail, and fishhook; Serious burns (partial thickness, or second degree); Heat exhaustion; Shock; Heatstroke, dehydration, hypothermia, and hyperventilation.",
                            rank: this,
                            handbookPages: "114,123-141",
                            curriculumGroup: CurriculumGroup.FirstAidBasics2,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "6b",
                            tag: "Show what to do for 'Hurry Cases'",
                            description: "Show what to do for \"hurry\" cases of stopped breathing, stroke, severe bleeding, and ingested poisoning.",
                            rank: this,
                            handbookPages: "115-124",
                            curriculumGroup: CurriculumGroup
                                .FirstAidHurryCases,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "6c",
                            tag: "Tell how to prevent problems in 6a & 6b",
                            description: "Tell what you can do while on a campout or hike to prevent or reduce the occurrence of the injuries listed in Second Class requirements 6a and 6b.",
                            rank: this,
                            handbookPages: "150-151",
                            curriculumGroup: CurriculumGroup.FirstAidBasics2,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "6d",
                            tag: "Explain what to do in emergencies",
                            description: "Explain what to do in case of accidents that require emergency response in the home and the backcountry. Explain what constitutes an emergency and what information you will need to provide to a responder.",
                            rank: this,
                            handbookPages: "111-124,150-154",
                            curriculumGroup: CurriculumGroup.Emergencies,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "6e",
                            tag: "Tell what to do when finding vehicle accident",
                            description: "Tell how you should respond if you come upon the scene of a vehicular accident.",
                            rank: this,
                            handbookPages: "152",
                            curriculumGroup: CurriculumGroup.Emergencies,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "7a",
                            tag: "Be physically active for four weeks after earning T6c",
                            description: "After competing Tenderfoot requirement 6c, be physically active at least 30 minutes a day for five days a week for four weeks. Keep track of your activities.",
                            rank: this,
                            handbookPages: "94-98",
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "7b",
                            tag: "Set goal and make plan to remain physically active",
                            description: "Share your challenges and successes in completing Second Class requirement 7a. Set a goal for continuing to include physical activity as part of your daily life and develop a plan for doing so.",
                            rank: this,
                            handbookPages: "94-98",
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "7c",
                            tag: "Participate in program on dangers of drugs",
                            description: "Participate in a school, community, or troop program on the dangers of using drugs, alcohol, and tobacco, and other practices that could be harmful to your health. Discuss your participation in the program with your family, and explain the dangers of substance addictions. Report to your Scoutmaster or other adult leader in your troop about which parts of the Scout Oath and Law relate to what you learned.",
                            rank: this,
                            handbookPages: "94-98",
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "8a",
                            tag: "Participate in a flag ceremony",
                            description: "Participate in a flag ceremony for your school, religious institution, chartered organization, community, or Scouting activity.",
                            rank: this,
                            handbookPages: "56-61",
                            curriculumGroup: CurriculumGroup.Citizenship,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "8b",
                            tag: "Explain flag respect",
                            description: "Explain what respect is due the flag of the United States.",
                            rank: this,
                            handbookPages: "58-61",
                            curriculumGroup: CurriculumGroup.Citizenship,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "8c",
                            tag: "Make and follow plan to earn money",
                            description: "With your parents or guardian, decide on an amount of money that you would like to earn, based on the cost of a specific item you would like to purchase. Develop a written plan to earn the amount agreed upon and follow that plan; it is acceptable to make changes to your plan along the way. Discuss any changes made to your original plan and whether you met your goal.",
                            rank: this,
                            handbookPages: "",
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "8d",
                            tag: "Compare prices of an item",
                            description: "At a minimum of three locations, compare the cost of the item for which you are saving to determine the best place to purchase it. After completing Second Class requirement 8c, decide if you will use the amount that you earned as originally intended, save all or part of it, or use it for another purpose.",
                            rank: this,
                            handbookPages: "",
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "8e",
                            tag: "Participate in 2 hour service project",
                            description: "Participate in two hours of service through one or more service projects approved by your Scoutmaster. Tell how your service to others relates to the Scout Oath.",
                            rank: this,
                            handbookPages: "",
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "9a",
                            tag: "Explain three R's of personal safety",
                            description: "Explain the three R's of personal safety and protection.",
                            rank: this,
                            handbookPages: "28-29,395-409",
                            curriculumGroup: CurriculumGroup.Citizenship,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "9b",
                            tag: "Describe bullying",
                            description: "Describe bullying; tell what the appropriate response is to someone who is bullying you or another person.",
                            rank: this,
                            handbookPages: "404-406",
                            curriculumGroup: CurriculumGroup.Citizenship,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "10",
                            tag: "Show Scout spirit",
                            description: "Demonstrate scout spirit by living the Scout Oath and Scout Law. Tell how you have done your duty to God and how you have lived four different points of the Scout Law (not to include those used for Tenderfoot requirement 9) in your everyday life.",
                            rank: this,
                            handbookPages: "",
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "11",
                            tag: "Scoutmaster conference",
                            description: "While working toward the Second Class rank, and after completing Tenderfoot requirement 10, participate in a Scoutmaster conference.",
                            rank: this,
                            handbookPages: "",
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "12",
                            tag: "Board of Review",
                            description: "Successfully complete your board of review for the Second Class rank.",
                            rank: this,
                            handbookPages: "",
                            version: version));
                    break;
                default:
                    throw new ArgumentException($"Version {version} not recognized");
            }
        }
    }
}
