using System;
using System.Drawing;

namespace advancementchart.Model.Ranks
{
    public class Scout : Rank
    {
        public Scout(DateTime? earned = null)
            : base(name: "Scout", description: "", earned: earned, version: "2016")
        {
            Requirements.Add(new RankRequirement(name: "1a", description: "Repeat from memory and explain: Scout Oath, Law, Motto, Slogan", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "1b", description: "Explain Scout spirit", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "1c", description: "Scout sign, salute and handshake", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "1d", description: "Describe First Class badge", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "1e", description: "Repeat and explain Outdoor Code", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "1f", description: "Repeat Pledge of Allegiance", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "2a", description: "Describe how Scouts provide leadership", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "2b", description: "Describe four steps of advancement", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "2c", description: "Describe Scout ranks and process", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "2d", description: "Describe merit badges and process", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "3a", description: "Explain the patrol method", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "3b", description: "Know Patrol name, emblem, flag and yell", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "4a", description: "Tie square knot, two half-hitches, and taughtline hitch", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "4b", description: "Whip and fuse rope", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "5", description: "Demonstrate pocktknife safety", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "6", description: "Complete abuse packet with parent & earn cyber chip", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "7", description: "Scoutmaster conference", rank: this, handbookPages: ""));
            FillColor = ColorTranslator.FromHtml("#CC9900");
        }
    }
}
