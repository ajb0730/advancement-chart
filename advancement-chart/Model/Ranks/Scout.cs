using System;
using System.Drawing;
using System.Linq;

namespace advancementchart.Model.Ranks
{
    public class Scout : Rank
    {
        public Scout(DateTime? earned = null) :
            base(
                name: "Scout",
                description: "",
                earned: earned,
                version: "2016"
            )
        {
            FillColor = ColorTranslator.FromHtml("#CC9900");
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
                            tag: "Scout Oath, Law, motto, slogan",
                            description: "Repeat from memory the Scout Oath, Scout Law, Scout motto, and Scout slogan. In your own words, explain their meaning.",
                            rank: this,
                            handbookPages: "11-18",
                            curriculumGroup: CurriculumGroup.FormingThePatrol,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "1b",
                            tag: "Scout Spirit",
                            description: "Explain what Scout spirit is. Describe some ways you have shown Scout spirit by practicing the Scout Oath, Scout Law, Scout motto, and Scout slogan.",
                            rank: this,
                            handbookPages: "15",
                            curriculumGroup: CurriculumGroup.FormingThePatrol,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "1c",
                            tag: "Scout sign, salute, handshake",
                            description: "Demonstrate the Boy Scout sign, salute, and handshake. Explain when they should be used.",
                            rank: this,
                            handbookPages: "18-19",
                            curriculumGroup: CurriculumGroup.FormingThePatrol,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "1d",
                            tag: "Describe First Class badge",
                            description: "Describe the First Class Scout badge and tell what each part stands for. Explain the significance of the First Class Scout badge.",
                            rank: this,
                            handbookPages: "19-20",
                            curriculumGroup: CurriculumGroup.FormingThePatrol,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "1e",
                            tag: "Repeat Outdoor Code",
                            description: "Repeat from memory the Outdoor Code. In your own words, explain what the Outdoor Code means to you.",
                            rank: this,
                            handbookPages: "223-224",
                            curriculumGroup: CurriculumGroup.FormingThePatrol,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "1f",
                            tag: "Repeat Pledge of Allegiance",
                            description: "Repeat from memory the Pledge of Allegiance. In your own words, explain its meaning.",
                            rank: this,
                            handbookPages: "60",
                            curriculumGroup: CurriculumGroup.FormingThePatrol,
                            version: version));

                    //Requirements.Add(new RankRequirement(name: "2", description: "After attending at least one meeting", rank: this, handbookPages: "", curriculumGroup: CurriculumGroup.FormingThePatrol));
                    Requirements
                        .Add(new RankRequirement(name: "2a",
                            tag: "Tell how Scouts provide leadership",
                            description: "Describe how the Scouts in the troop provide its leadership.",
                            rank: this,
                            handbookPages: "24,32-33,42-43,420-423",
                            curriculumGroup: CurriculumGroup.FormingThePatrol,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "2b",
                            tag: "Describe four steps of advancement",
                            description: "Describe the four steps of Boy Scout advancement.",
                            rank: this,
                            handbookPages: "412-415",
                            curriculumGroup: CurriculumGroup.FormingThePatrol,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "2c",
                            tag: "Describe ranks and how they are earned",
                            description: "Describe what the Boy Scout ranks are and how they are earned.",
                            rank: this,
                            handbookPages: "27-28,439-452",
                            curriculumGroup: CurriculumGroup.FormingThePatrol,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "2d",
                            tag: "Describe merit badges and how they are earned",
                            description: "Describe what merit badges are and how they are earned.",
                            rank: this,
                            handbookPages: "28,416-417,454-456",
                            curriculumGroup: CurriculumGroup.FormingThePatrol,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "3a",
                            tag: "Explain the patrol method",
                            description: "Explain the patrol method. Describe the types of patrols that are used in your troop.",
                            rank: this,
                            handbookPages: "25",
                            curriculumGroup: CurriculumGroup.FormingThePatrol,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "3b",
                            tag: "Patrol name, emblem, flag and yell",
                            description: "Become familiar with your patrol name, emblem, flag, and yell. Explain how these items create patrol spirit.",
                            rank: this,
                            handbookPages: "26",
                            curriculumGroup: CurriculumGroup.FormingThePatrol,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "4a",
                            tag: "Tie square knot, two half-hitches, taut-line hitch",
                            description: "Show how to tie a square knot, two half-hitches, and a taut-line hitch. Explain how each knot is used.",
                            rank: this,
                            handbookPages: "364-367",
                            curriculumGroup: CurriculumGroup.KnotsAndLashings1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "4b",
                            tag: "Whip and fuse rope",
                            description: "Show the proper care of a rope by learning how to whip and fuse the ends of different kinds of rope.",
                            rank: this,
                            handbookPages: "360-362",
                            curriculumGroup: CurriculumGroup.KnotsAndLashings1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "5",
                            tag: "Demonstrate pocketknife safety",
                            description: "Demonstrate your knowledge of pocketknife safety.",
                            rank: this,
                            handbookPages: "379-381",
                            curriculumGroup: CurriculumGroup.TotinChip,
                            version: version));

                    //Requirements.Add(new RankRequirement(name: "6", description: "Personal protection", rank: this, handbookPages: "407-409"));
                    Requirements
                        .Add(new RankRequirement(name: "6a",
                            tag: "Complete parent pamphlet",
                            description: "With your parent or guardian, complete the exercises in the pamphlet \"How to Protect Your Children from Child Abuse: A Parents Guide\"",
                            rank: this,
                            handbookPages: "",
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "6b",
                            tag: "Earn Cyber Chip",
                            description: "and earn the Cyber Chip Award for your grade.",
                            rank: this,
                            handbookPages: "",
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "7",
                            tag: "Scoutmaster Conference",
                            description: "Since joining the troop and while working on the Scout rank, participate in a Scoutmaster conference.",
                            rank: this,
                            handbookPages: "",
                            version: version));
                    break;
                case "2022":
                    Requirements
                        .Add(new RankRequirement(name: "1a",
                            tag: "Scout Oath, Law, motto, slogan",
                            description: "Repeat from memory the Scout Oath, Scout Law, Scout motto, and Scout slogan. In your own words, explain their meaning.",
                            rank: this,
                            handbookPages: "11-18",
                            curriculumGroup: CurriculumGroup.FormingThePatrol,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "1b",
                            tag: "Scout Spirit",
                            description: "Explain what Scout spirit is. Describe some ways you have shown Scout spirit by practicing the Scout Oath, Scout Law, Scout motto, and Scout slogan.",
                            rank: this,
                            handbookPages: "15",
                            curriculumGroup: CurriculumGroup.FormingThePatrol,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "1c",
                            tag: "Scout sign, salute, handshake",
                            description: "Demonstrate the Boy Scout sign, salute, and handshake. Explain when they should be used.",
                            rank: this,
                            handbookPages: "18-19",
                            curriculumGroup: CurriculumGroup.FormingThePatrol,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "1d",
                            tag: "Describe First Class badge",
                            description: "Describe the First Class Scout badge and tell what each part stands for. Explain the significance of the First Class Scout badge.",
                            rank: this,
                            handbookPages: "19-20",
                            curriculumGroup: CurriculumGroup.FormingThePatrol,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "1e",
                            tag: "Repeat Outdoor Code, describe Leave No Trace",
                            description: "Repeat from memory the Outdoor Code. List the seven principles of Leave No Trace. Explain the difference between the two.",
                            rank: this,
                            handbookPages: "223-224",
                            curriculumGroup: CurriculumGroup.FormingThePatrol,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "1f",
                            tag: "Repeat Pledge of Allegiance",
                            description: "Repeat from memory the Pledge of Allegiance. In your own words, explain its meaning.",
                            rank: this,
                            handbookPages: "60",
                            curriculumGroup: CurriculumGroup.FormingThePatrol,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "2a",
                            tag: "Tell how Scouts provide leadership",
                            description: "Describe how the Scouts in the troop provide its leadership.",
                            rank: this,
                            handbookPages: "24,32-33,42-43,420-423",
                            curriculumGroup: CurriculumGroup.FormingThePatrol,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "2b",
                            tag: "Describe four steps of advancement",
                            description: "Describe the four steps of Boy Scout advancement.",
                            rank: this,
                            handbookPages: "412-415",
                            curriculumGroup: CurriculumGroup.FormingThePatrol,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "2c",
                            tag: "Describe ranks and how they are earned",
                            description: "Describe what the Boy Scout ranks are and how they are earned.",
                            rank: this,
                            handbookPages: "27-28,439-452",
                            curriculumGroup: CurriculumGroup.FormingThePatrol,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "2d",
                            tag: "Describe merit badges and how they are earned",
                            description: "Describe what merit badges are and how they are earned.",
                            rank: this,
                            handbookPages: "28,416-417,454-456",
                            curriculumGroup: CurriculumGroup.FormingThePatrol,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "3a",
                            tag: "Explain the patrol method",
                            description: "Explain the patrol method. Describe the types of patrols that are used in your troop.",
                            rank: this,
                            handbookPages: "25",
                            curriculumGroup: CurriculumGroup.FormingThePatrol,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "3b",
                            tag: "Patrol name, emblem, flag and yell",
                            description: "Become familiar with your patrol name, emblem, flag, and yell. Explain how these items create patrol spirit.",
                            rank: this,
                            handbookPages: "26",
                            curriculumGroup: CurriculumGroup.FormingThePatrol,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "4a",
                            tag: "Tie square knot, two half-hitches, taut-line hitch",
                            description: "Show how to tie a square knot, two half-hitches, and a taut-line hitch. Explain how each knot is used.",
                            rank: this,
                            handbookPages: "364-367",
                            curriculumGroup: CurriculumGroup.KnotsAndLashings1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "4b",
                            tag: "Whip and fuse rope",
                            description: "Show the proper care of a rope by learning how to whip and fuse the ends of different kinds of rope.",
                            rank: this,
                            handbookPages: "360-362",
                            curriculumGroup: CurriculumGroup.KnotsAndLashings1,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "5",
                            tag: "Demonstrate pocketknife safety",
                            description: "Tell what you need to know about pocketknife safety and responsibility.",
                            rank: this,
                            handbookPages: "379-381",
                            curriculumGroup: CurriculumGroup.TotinChip,
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "6a",
                            tag: "Complete parent pamphlet",
                            description: "With your parent or guardian, complete the exercises in the pamphlet \"How to Protect Your Children from Child Abuse: A Parents Guide\"",
                            rank: this,
                            handbookPages: "",
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "6b",
                            tag: "Earn Cyber Chip or view Personal Safety Awareness videos",
                            description: "and earn the Cyber Chip Award for your grade or view the Personal Safety Awareness videos ( with your parent or Guardian's permission).",
                            rank: this,
                            handbookPages: "",
                            version: version));
                    Requirements
                        .Add(new RankRequirement(name: "7",
                            tag: "Scoutmaster Conference",
                            description: "Since joining the troop and while working on the Scout rank, participate in a Scoutmaster conference.",
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
