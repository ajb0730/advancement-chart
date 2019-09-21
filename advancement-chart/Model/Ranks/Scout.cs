using System;
using System.Drawing;

namespace advancementchart.Model.Ranks
{
    public class Scout : Rank
    {
        public Scout(DateTime? earned = null)
            : base(name: "Scout", description: "", earned: earned, version: "2016")
        {
            Requirements.Add(new RankRequirement(name: "1a", description: "Repeat from memory and explain: Scout Oath, Law, Motto, Slogan", rank: this, handbookPages: "11-18", curriculumGroup: CurriculumGroup.FormingThePatrol));
            Requirements.Add(new RankRequirement(name: "1b", description: "Explain Scout spirit", rank: this, handbookPages: "15", curriculumGroup: CurriculumGroup.FormingThePatrol));
            Requirements.Add(new RankRequirement(name: "1c", description: "Scout sign, salute and handshake", rank: this, handbookPages: "18-19", curriculumGroup: CurriculumGroup.FormingThePatrol));
            Requirements.Add(new RankRequirement(name: "1d", description: "Describe First Class badge", rank: this, handbookPages: "19-20", curriculumGroup: CurriculumGroup.FormingThePatrol));
            Requirements.Add(new RankRequirement(name: "1e", description: "Repeat and explain Outdoor Code", rank: this, handbookPages: "223-224", curriculumGroup: CurriculumGroup.FormingThePatrol));
            Requirements.Add(new RankRequirement(name: "1f", description: "Repeat Pledge of Allegiance", rank: this, handbookPages: "60", curriculumGroup: CurriculumGroup.FormingThePatrol));
            //Requirements.Add(new RankRequirement(name: "2", description: "After attending at least one meeting", rank: this, handbookPages: "", curriculumGroup: CurriculumGroup.FormingThePatrol));
            Requirements.Add(new RankRequirement(name: "2a", description: "Describe how Scouts provide leadership", rank: this, handbookPages: "24,32-33,42-43,420-423", curriculumGroup: CurriculumGroup.FormingThePatrol));
            Requirements.Add(new RankRequirement(name: "2b", description: "Describe four steps of advancement", rank: this, handbookPages: "412-415", curriculumGroup: CurriculumGroup.FormingThePatrol));
            Requirements.Add(new RankRequirement(name: "2c", description: "Describe Scout ranks and process", rank: this, handbookPages: "27-28,439-452", curriculumGroup: CurriculumGroup.FormingThePatrol));
            Requirements.Add(new RankRequirement(name: "2d", description: "Describe merit badges and process", rank: this, handbookPages: "28,416-417,454-456", curriculumGroup: CurriculumGroup.FormingThePatrol));
            Requirements.Add(new RankRequirement(name: "3a", description: "Explain the patrol method", rank: this, handbookPages: "25", curriculumGroup: CurriculumGroup.FormingThePatrol));
            Requirements.Add(new RankRequirement(name: "3b", description: "Know Patrol name, emblem, flag and yell", rank: this, handbookPages: "26", curriculumGroup: CurriculumGroup.FormingThePatrol));
            Requirements.Add(new RankRequirement(name: "4a", description: "Tie square knot, two half-hitches, and taughtline hitch", rank: this, handbookPages: "364-367", curriculumGroup: CurriculumGroup.KnotsAndLashings1));
            Requirements.Add(new RankRequirement(name: "4b", description: "Whip and fuse rope", rank: this, handbookPages: "360-362", curriculumGroup: CurriculumGroup.KnotsAndLashings1));
            Requirements.Add(new RankRequirement(name: "5", description: "Demonstrate pocktknife safety", rank: this, handbookPages: "379-381", curriculumGroup: CurriculumGroup.TotinChip));
            //Requirements.Add(new RankRequirement(name: "6", description: "Personal protection", rank: this, handbookPages: "407-409"));
            Requirements.Add(new RankRequirement(name: "6a", description: "Complete abuse packet with parent", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "6b", description: "Earn cyber chip", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "7", description: "Scoutmaster conference", rank: this, handbookPages: ""));
            FillColor = ColorTranslator.FromHtml("#CC9900");
        }
    }
}
