using System;
using System.Linq;

namespace advancementchart.Model.Ranks
{
    public class Eagle : Rank
    {

        protected Eagle()
            : base()
        { }

        public Eagle(DateTime? earned = null)
            : base(name: "Eagle", description: "", earned: earned, version: "2016")
        {
            Requirements.Add(new RankRequirement(name: "1", description: "Be active 6 months since earning Life Scout", rank: this, handbookPages: "", timeRequirementMonths: 6));
            Requirements.Add(new RankRequirement(name: "2", description: "Demonstrate Scout spirit while Life Scout", rank: this, handbookPages: ""));
            Requirements.Add(new EagleMeritBadgeRequirement(name: "3", description: "Earn a total of 21 merit badges (10 more than required for Life rank)", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "4", description: "Hold leadership position at least 6 months while Life Scout", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "5", description: "Eagle Project", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "6", description: "Scoutmaster conference", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "7", description: "Eagle Board of Review", rank: this, handbookPages: ""));
        }
        override public void SwitchVersion(string version) {
            this.Version = version;
            if (Requirements.Any())
            {
                Requirements.Clear();
            }
            switch (version)
            {
                case "2016":
                    break;
                default:
                    throw new ArgumentException($"Version {version} not recognized");
            }
        }


        public EagleMeritBadgeRequirement MbReq => Requirements.First(req => req is EagleMeritBadgeRequirement) as EagleMeritBadgeRequirement;
    }
}
