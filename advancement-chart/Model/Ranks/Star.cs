using System;
using System.Drawing;
using System.Linq;

namespace advancementchart.Model.Ranks
{
    public class Star : Rank
    {
        protected Star()
            : base()
        { }

        public Star(DateTime? earned = null)
            : base(name: "Star", description: "", earned: earned, version: "2016")
        {
            Requirements.Add(new RankRequirement(name: "1", description: "Be active 4 months since earning First Class", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "2", description: "Demonstrate Scout spirit while First Class", rank: this, handbookPages: ""));
            Requirements.Add(new MeritBadgeRequirement(name: "3", description: "Earn 6 merit badges, including 4 Eagle-required", total: 6, required: 4, rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "4", description: "Participate in 6 hours of service while First Class", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "5", description: "Hold leadership position at least 4 months while First Class", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "6", description: "Complete abuse booklet with parent & cyber chip", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "7", description: "Scoutmaster conference", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "8", description: "Board of Review", rank: this, handbookPages: ""));
            FillColor = ColorTranslator.FromHtml("#003F87");
        }

        public MeritBadgeRequirement MbReq => Requirements.First(req => req is MeritBadgeRequirement) as MeritBadgeRequirement;
    }
}
