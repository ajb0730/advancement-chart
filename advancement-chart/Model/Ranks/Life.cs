using System;
using System.Drawing;
using System.Linq;

namespace advancementchart.Model.Ranks
{
    public class Life : Rank
    {
        protected Life()
            : base()
        { }

        public Life(DateTime? earned = null)
            : base(name: "Life", description: "", earned: earned, version: "2016")
        {
            Requirements.Add(new RankRequirement(name: "1", description: "Be active 6 months since earning Star Scout", rank: this, handbookPages: "", timeRequirementMonths: 6));
            Requirements.Add(new RankRequirement(name: "2", description: "Demonstrate Scout spirit while Star Scout", rank: this, handbookPages: ""));
            Requirements.Add(new MeritBadgeRequirement(name: "3", description: "Earn 5 more merit badges, including 3 Eagle-required", total: 5, required: 3, rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "4", description: "Participate in 6 hours of service, at least 3 conservation related, while Star Scout", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "5", description: "Hold leadership position at least 6 months while Star Scout", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "6", description: "Use EDGE method to teach another Scout", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "7", description: "Scoutmaster conference", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "8", description: "Board of Review", rank: this, handbookPages: ""));
            FillColor = ColorTranslator.FromHtml("#FFCC00");
        }

        public MeritBadgeRequirement MbReq => Requirements.First(req => req is MeritBadgeRequirement) as MeritBadgeRequirement;
    }
}
