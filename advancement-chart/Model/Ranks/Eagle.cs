using System;
using System.Linq;

namespace advancementchart.Model.Ranks
{
    /// <summary>
    /// The Eagle Scout rank. Its requirements are created in the constructor.
    /// </summary>
    public class Eagle : Rank
    {

        protected Eagle()
            : base()
        { }

        public Eagle(DateTime? earned = null)
            : base(name: "Eagle", description: "", earned: earned)
        {
            Requirements.Add(new RankRequirement(name: "1", description: $"Be active {ScoutingConstants.EagleActiveMonths} months since earning Life Scout", rank: this, handbookPages: "", timeRequirementMonths: ScoutingConstants.EagleActiveMonths));
            Requirements.Add(new RankRequirement(name: "2", description: "Demonstrate Scout spirit while Life Scout", rank: this, handbookPages: ""));
            Requirements.Add(new EagleMeritBadgeRequirement(name: "3", description: "Earn 10 more merit badges, including 5 Eagle-required", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "4", description: "Hold leadership position at least 6 months while Life Scout", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "5", description: "Eagle Project", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "6", description: "Scoutmaster conference", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "7", description: "Eagle Board of Review", rank: this, handbookPages: ""));
        }

        /// <summary>
        /// The Eagle merit badge requirement.
        /// </summary>
        public EagleMeritBadgeRequirement MbReq => Requirements.First(req => req is EagleMeritBadgeRequirement) as EagleMeritBadgeRequirement;
    }
}
