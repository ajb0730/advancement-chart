using System;
using System.Linq;

namespace advancementchart.Model.Ranks
{
    public class Palm : Rank
    {
        protected Palm()
            : base()
        { }

        public enum PalmType { None, Bronze, Gold, Silver }

        public Palm(PalmType type, DateTime? earned = null)
            : base(name: $"{Enum.GetName(typeof(PalmType),type)} Palm", description: "", earned: earned, version: "2016")
        {
            if(type == PalmType.None)
            {
                throw new ArgumentException(message: "PalmType.None is not permitted");
            }
            Type = type;

            Requirements.Add(new RankRequirement(name: "1", description: "Be active 3 months since earning Eagle Scout or previous Eagle Palm", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "2", description: "Demonstrate Scout spirit since earning Eagle Scout or previous Eagle Palm", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "3", description: "Demonstrate leadership or responsibility since earning Eagle Scout or previous Eagle Palm", rank: this, handbookPages: ""));
            Requirements.Add(new MeritBadgeRequirement(name: "4", description: "Earn 5 more merit badges", total: 5, required: 0, rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "5", description: "Scoutmaster conference", rank: this, handbookPages: ""));
        }

        public PalmType Type { get; protected set; }

        public MeritBadgeRequirement MbReq => Requirements.First(req => req is MeritBadgeRequirement) as MeritBadgeRequirement;
    }
}
