using System;
using System.Linq;

namespace advancementchart.Model.Ranks
{
    /// <summary>
    /// An Eagle Palm: a Bronze, Gold, or Silver pin earned for 3 months of activity and 5 more merit badges
    /// after Eagle (or the previous Palm).
    /// </summary>
    public class Palm : Rank
    {
        protected Palm()
            : base()
        { }

        /// <summary>
        /// The Palm pin type. Palms cycle Bronze, Gold, Silver, Bronze, ...; <c>None</c> means "no Palm yet" and is not a valid Palm.
        /// </summary>
        public enum PalmType { None, Bronze, Gold, Silver }

        /// <summary>
        /// Creates a Palm of the given type.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="type"/> is <see cref="PalmType.None"/>.</exception>
        public Palm(PalmType type, DateTime? earned = null)
            : base(name: $"{Enum.GetName(typeof(PalmType), type)} Palm", description: "", earned: earned)
        {
            if (type == PalmType.None)
            {
                throw new ArgumentException(message: "PalmType.None is not permitted");
            }
            Type = type;

            Requirements.Add(new RankRequirement(name: "1", description: $"Be active {ScoutingConstants.PalmActiveMonths} months since earning Eagle Scout or previous Eagle Palm", rank: this, handbookPages: "", timeRequirementMonths: ScoutingConstants.PalmActiveMonths));
            Requirements.Add(new RankRequirement(name: "2", description: "Demonstrate Scout spirit since earning Eagle Scout or previous Eagle Palm", rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "3", description: "Demonstrate leadership or responsibility since earning Eagle Scout or previous Eagle Palm", rank: this, handbookPages: ""));
            Requirements.Add(new MeritBadgeRequirement(name: "4", description: "Earn 5 more merit badges", total: 5, required: 0, rank: this, handbookPages: ""));
            Requirements.Add(new RankRequirement(name: "5", description: "Scoutmaster conference", rank: this, handbookPages: ""));
        }

        /// <summary>
        /// Which Palm pin this is.
        /// </summary>
        public PalmType Type { get; protected set; }

        public MeritBadgeRequirement MbReq => Requirements.First(req => req is MeritBadgeRequirement) as MeritBadgeRequirement;
    }
}
