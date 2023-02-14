using System;
using System.Drawing;
using System.Linq;

namespace advancementchart.Model.Ranks
{
    public class Tenderfoot : Rank
    {
        public Tenderfoot(DateTime? earned = null) :
            base(
                name: "Tenderfoot",
                description: "",
                earned: earned,
                version: "2016"
            )
        {
            FillColor = ColorTranslator.FromHtml("#996633");
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
                            tag: "Prepare for campout",
                            description: "Present yourself to your leader prepared for an overnight camping trip. Show the personal and camping gear you will use. Show the right way to pack and carry it.",
                            rank: this,
                            handbookPages: "267-273",
                            curriculumGroup: CurriculumGroup.Camping1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "1b",
                            tag: "Spend one night on campout in tent",
                            description: "Spend at least one night on a patrol or troop campout. Sleep in a tent you have helped pitch.",
                            rank: this,
                            handbookPages: "",
                            curriculumGroup: CurriculumGroup.Camping2,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "1c",
                            tag: "Tell how you practiced Outdoor Code on outing",
                            description: "Tell how you practiced the Outdoor Code on a campout or outing.",
                            rank: this,
                            handbookPages: "223-224",
                            curriculumGroup: CurriculumGroup.OutdoorEthics,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "2a",
                            tag: "Assist with cooking",
                            description: "On the campout, assist in preparing one of the meals. Tell why it is important for each patrol member to share in meal preparation and cleanup.",
                            rank: this,
                            handbookPages: "301,304-305,307-309",
                            curriculumGroup: CurriculumGroup.Cooking,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "2b",
                            tag: "Demonstrate safe meal utensil cleaning",
                            description: "While on a campout, demonstrate the appropriate method of safely cleaning items used to prepare, serve, and eat a meal.",
                            rank: this,
                            handbookPages: "307-309",
                            curriculumGroup: CurriculumGroup.Cooking,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "2c",
                            tag: "Explain importance of eating as patrol",
                            description: "Explain the importance of eating together as a patrol.",
                            rank: this,
                            handbookPages: "304-305",
                            curriculumGroup: CurriculumGroup.Cooking,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "3a",
                            tag: "Demonstrate square knot",
                            description: "Demonstrate a practical use of the square knot.",
                            rank: this,
                            handbookPages: "365,145",
                            curriculumGroup: CurriculumGroup.KnotsAndLashings1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "3b",
                            tag: "Demonstrate two half-hitches",
                            description: "Demonstrate a practical use of two half-hitches.",
                            rank: this,
                            handbookPages: "366,369",
                            curriculumGroup: CurriculumGroup.KnotsAndLashings1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "3c",
                            tag: "Demonstrate taughtline hitch",
                            description: "Demonstrate a practical use of the taut line hitch.",
                            rank: this,
                            handbookPages: "367,369",
                            curriculumGroup: CurriculumGroup.KnotsAndLashings1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "3d",
                            tag: "Demonstrate proper care, sharpening, use of woods tools",
                            description: "Demonstrate proper care, sharpening, and use of the knife, saw, and ax. Describe when each should be used.",
                            rank: this,
                            handbookPages: "380-381,386",
                            curriculumGroup: CurriculumGroup.TotinChip,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "4a",
                            tag: "Demonstrate First Aid skills",
                            description: "Show first aid for the following: Simple cuts and scrapes, Blisters on the hand and foot, Minor (thermal/heat) burns or scalds (superficial, or first degree), Bites or stings of insects or ticks, Venomous snakebite, Nosebleed, Frostbite and sunburn, Choking.",
                            rank: this,
                            handbookPages: "120-141",
                            curriculumGroup: CurriculumGroup.FirstAidBasics1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "4b",
                            tag: "Describe and identify poisonous plants and treatment",
                            description: "Describe common poisonous or hazardous plants, identify any that grow in your local area or campsite location. Tell how to treat for exposure to them.",
                            rank: this,
                            handbookPages: "127,191-192",
                            curriculumGroup: CurriculumGroup.FirstAidBasics1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "4c",
                            tag: "Tell how to prevent occurrences of 4a & 4b",
                            description: "Tell what you can do on a campout or other outdoor activity to prevent or reduce the occurrence of injuries or exposure listed in Tenderfoot requirements 4a and 4b.",
                            rank: this,
                            handbookPages: "125",
                            curriculumGroup: CurriculumGroup.FirstAidBasics1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "4d",
                            tag: "Assemble personal first aid kit",
                            description: "Assemble a personal first-aid kit to carry with you on future campouts and hikes. Tell how each item in the kit would be used.",
                            rank: this,
                            handbookPages: "108",
                            curriculumGroup: CurriculumGroup.FirstAidBasics1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "5a",
                            tag: "Explain buddy system",
                            description: "Explain the importance of the buddy system as it relates to your personal safety on outings and in your neighborhood. Use the buddy system while on a troop or patrol outing.",
                            rank: this,
                            handbookPages: "29,160-161,172,404",
                            curriculumGroup: CurriculumGroup.FormingThePatrol,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "5b",
                            tag: "Describe what to do if you get lost",
                            description: "Explain what to do if you become lost on a hike or campout.",
                            rank: this,
                            handbookPages: "253-255",
                            curriculumGroup: CurriculumGroup.Camping1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "5c",
                            tag: "Explain rules of safe hiking",
                            description: "Explain the rules of safe hiking, both on the highway and cross-country, during the day and at night.",
                            rank: this,
                            handbookPages: "253-255",
                            curriculumGroup: CurriculumGroup.Camping1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "6a",
                            tag: "Record your best at fitness test",
                            description: "Record your best in the following tests: Pushups, Situps or curl-ups, Back-saver sit-and-reach, 1 mile walk/run.",
                            rank: this,
                            handbookPages: "77-81",
                            curriculumGroup: CurriculumGroup.Fitness1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "6b",
                            tag: "Describe your plan to improve after 30 days",
                            description: "Develop and describe a plan for improvement in each of the activities listed in Tenderfoot requirement 6a. Keep track of your activity for at least 30 days.",
                            rank: this,
                            handbookPages: "77-81",
                            curriculumGroup: CurriculumGroup.Fitness2,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "6c",
                            tag: "Show improvement at fitness test after 30 days",
                            description: "Show improvement (of any degree) in each activity listed in Tenderfoot requirement 6a after practicing for 30 days.",
                            rank: this,
                            handbookPages: "77-81",
                            curriculumGroup: CurriculumGroup.Fitness2,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "7a",
                            tag: "Demonstrate flag skills",
                            description: "Demonstrate how to display, raise, lower, and fold the U.S. flag.",
                            rank: this,
                            handbookPages: "56-60",
                            curriculumGroup: CurriculumGroup.Citizenship,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "7b",
                            tag: "Participate in 1 hour service project",
                            description: "Participate in a total of one hour of service in one or more service projects approved by your Scoutmaster. Explain how your service to others relates to the Scout slogan and Scout motto.",
                            rank: this,
                            handbookPages: "68-69",
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "8",
                            tag: "Describe and use EDGE training method",
                            description: "Describe the steps in Scouting's Teaching EDGE method. Use the Teaching EDGE method to teach another person how to tie the square knot.",
                            rank: this,
                            handbookPages: "38,365",
                            curriculumGroup: CurriculumGroup.KnotsAndLashings1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "9",
                            tag: "Show Scout spirit",
                            description: "Demonstrate Scout spirit by living the Scout Oath and Scout Law. Tell how you have done your duty to God and how you have lived four different points of the Scout Law in your everyday life.",
                            rank: this,
                            handbookPages: "11-16",
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "10",
                            tag: "Scoutmaster conference",
                            description: "While working toward Tenderfoot rank, and after completing Scout rank requirement 7, participate in a Scoutmaster conference.",
                            rank: this,
                            handbookPages: "",
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "11",
                            tag: "Board of Review",
                            description: "Successfully complete your board of review for the Tenderfoot rank.",
                            rank: this,
                            handbookPages: "",
                            version: version));
                    break;
                case "2022":
                    Requirements
                        .Add(new RankRequirement(name: "1a",
                            tag: "Prepare for campout",
                            description: "Present yourself to your leader prepared for an overnight camping trip. Show the personal and camping gear you will use. Show the right way to pack and carry it.",
                            rank: this,
                            handbookPages: "267-273",
                            curriculumGroup: CurriculumGroup.Camping1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "1b",
                            tag: "Spend one night on campout in tent",
                            description: "Spend at least one night on a patrol or troop campout. Sleep in a tent you have helped pitch.",
                            rank: this,
                            handbookPages: "",
                            curriculumGroup: CurriculumGroup.Camping2,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "1c",
                            tag: "Tell how you practiced Outdoor Code on outing",
                            description: "Explain how you demonstrated the Outdoor Code and Leave No Trace on a campout or outing.",
                            rank: this,
                            handbookPages: "223-224",
                            curriculumGroup: CurriculumGroup.OutdoorEthics,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "2a",
                            tag: "Assist with cooking",
                            description: "On the campout, assist in preparing one of the meals. Tell why it is important for each patrol member to share in meal preparation and cleanup.",
                            rank: this,
                            handbookPages: "301,304-305,307-309",
                            curriculumGroup: CurriculumGroup.Cooking,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "2b",
                            tag: "Demonstrate safe meal utensil cleaning",
                            description: "While on a campout, demonstrate the appropriate method of safely cleaning items used to prepare, serve, and eat a meal.",
                            rank: this,
                            handbookPages: "307-309",
                            curriculumGroup: CurriculumGroup.Cooking,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "2c",
                            tag: "Explain importance of eating as patrol",
                            description: "Explain the importance of eating together as a patrol.",
                            rank: this,
                            handbookPages: "304-305",
                            curriculumGroup: CurriculumGroup.Cooking,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "3a",
                            tag: "Demonstrate square knot",
                            description: "Demonstrate a practical use of the square knot.",
                            rank: this,
                            handbookPages: "365,145",
                            curriculumGroup: CurriculumGroup.KnotsAndLashings1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "3b",
                            tag: "Demonstrate two half-hitches",
                            description: "Demonstrate a practical use of two half-hitches.",
                            rank: this,
                            handbookPages: "366,369",
                            curriculumGroup: CurriculumGroup.KnotsAndLashings1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "3c",
                            tag: "Demonstrate taughtline hitch",
                            description: "Demonstrate a practical use of the taut line hitch.",
                            rank: this,
                            handbookPages: "367,369",
                            curriculumGroup: CurriculumGroup.KnotsAndLashings1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "3d",
                            tag: "Demonstrate proper care, sharpening, use of woods tools",
                            description: "Demonstrate proper care, sharpening, and use of the knife, saw, and ax. Describe when each should be used.",
                            rank: this,
                            handbookPages: "380-381,386",
                            curriculumGroup: CurriculumGroup.TotinChip,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "4a",
                            tag: "Demonstrate First Aid skills",
                            description: "Show first aid for the following: Simple cuts and scrapes, Blisters on the hand and foot, Minor (thermal/heat) burns or scalds (superficial, or first degree), Bites or stings of insects or ticks, Venomous snakebite, Nosebleed, Frostbite and sunburn, Choking.",
                            rank: this,
                            handbookPages: "120-141",
                            curriculumGroup: CurriculumGroup.FirstAidBasics1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "4b",
                            tag: "Describe and identify poisonous plants and treatment",
                            description: "Describe common poisonous or hazardous plants, identify any that grow in your local area or campsite location. Tell how to treat for exposure to them.",
                            rank: this,
                            handbookPages: "127,191-192",
                            curriculumGroup: CurriculumGroup.FirstAidBasics1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "4c",
                            tag: "Tell how to prevent occurrences of 4a & 4b",
                            description: "Tell what you can do on a campout or other outdoor activity to prevent or reduce the occurrence of injuries or exposure listed in Tenderfoot requirements 4a and 4b.",
                            rank: this,
                            handbookPages: "125",
                            curriculumGroup: CurriculumGroup.FirstAidBasics1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "4d",
                            tag: "Assemble personal first aid kit",
                            description: "Assemble a personal first-aid kit to carry with you on future campouts and hikes. Tell how each item in the kit would be used.",
                            rank: this,
                            handbookPages: "108",
                            curriculumGroup: CurriculumGroup.FirstAidBasics1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "5a",
                            tag: "Explain buddy system",
                            description: "Explain the importance of the buddy system as it relates to your personal safety on outings and where you live. Use the buddy system while on a troop or patrol outing.",
                            rank: this,
                            handbookPages: "29,160-161,172,404",
                            curriculumGroup: CurriculumGroup.FormingThePatrol,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "5b",
                            tag: "Describe what to do if you get lost",
                            description: "Explain what to do if you become lost on a hike or campout.",
                            rank: this,
                            handbookPages: "253-255",
                            curriculumGroup: CurriculumGroup.Camping1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "5c",
                            tag: "Explain rules of safe hiking",
                            description: "Explain the rules of safe and responsible hiking, both on the highway and cross-country, during the day and at night.",
                            rank: this,
                            handbookPages: "253-255",
                            curriculumGroup: CurriculumGroup.Camping1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "6a",
                            tag: "Record your best at fitness test",
                            description: "Record your best in the following tests: Pushups, Situps or curl-ups, Back-saver sit-and-reach, 1 mile walk/run.",
                            rank: this,
                            handbookPages: "77-81",
                            curriculumGroup: CurriculumGroup.Fitness1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "6b",
                            tag: "Describe your plan to improve after 30 days",
                            description: "Develop and describe a plan for improvement in each of the activities listed in Tenderfoot requirement 6a. Keep track of your activity for at least 30 days.",
                            rank: this,
                            handbookPages: "77-81",
                            curriculumGroup: CurriculumGroup.Fitness2,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "6c",
                            tag: "Show improvement at fitness test after 30 days",
                            description: "Show improvement (of any degree) in each activity listed in Tenderfoot requirement 6a after practicing for 30 days.",
                            rank: this,
                            handbookPages: "77-81",
                            curriculumGroup: CurriculumGroup.Fitness2,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "7a",
                            tag: "Demonstrate flag skills",
                            description: "Demonstrate how to display, raise, lower, and fold the U.S. flag.",
                            rank: this,
                            handbookPages: "56-60",
                            curriculumGroup: CurriculumGroup.Citizenship,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "7b",
                            tag: "Participate in 1 hour service project",
                            description: "Participate in a total of one hour of service in one or more service projects approved by your Scoutmaster. Explain how your service to others relates to the Scout slogan and Scout motto.",
                            rank: this,
                            handbookPages: "68-69",
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "8",
                            tag: "Describe and use EDGE training method",
                            description: "Describe the steps in Scouting's Teaching EDGE method. Use the Teaching EDGE method to teach another person how to tie the square knot.",
                            rank: this,
                            handbookPages: "38,365",
                            curriculumGroup: CurriculumGroup.KnotsAndLashings1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "9",
                            tag: "Show Scout spirit",
                            description: "Demonstrate Scout spirit by living the Scout Oath and Scout Law. Tell how you have done your duty to God and how you have lived four different points of the Scout Law in your everyday life.",
                            rank: this,
                            handbookPages: "11-16",
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "10",
                            tag: "Scoutmaster conference",
                            description: "While working toward Tenderfoot rank, and after completing Scout rank requirement 7, participate in a Scoutmaster conference.",
                            rank: this,
                            handbookPages: "",
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "11",
                            tag: "Board of Review",
                            description: "Successfully complete your board of review for the Tenderfoot rank.",
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
