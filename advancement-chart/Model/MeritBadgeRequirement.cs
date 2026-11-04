using System;
using System.Collections.Generic;
using System.Linq;

namespace advancementchart.Model
{
    /// <summary>
    /// A requirement satisfied by earning merit badges: <see cref="Total"/> badges of which <see cref="Required"/> must be
    /// Eagle-required and the rest (<see cref="Elective"/>) may be anything. Badges are placed into it by
    /// <see cref="TroopMember.AllocateMeritBadges"/>.
    /// </summary>
    public class MeritBadgeRequirement : RankRequirement
    {
        protected MeritBadgeRequirement()
            : base()
        {
            MeritBadges = new List<MeritBadge>();
        }

        /// <summary>
        /// Creates a requirement for <paramref name="total"/> badges, <paramref name="required"/> of them Eagle-required.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="total"/> is not positive, or <paramref name="required"/> is not less than it.</exception>
        public MeritBadgeRequirement(string name, string description, Rank rank, int required, int total, string handbookPages = "", DateTime? earned = null)
            : base(name, description, rank, handbookPages, earned, null)
        {
            if (total <= 0)
            {
                throw new ArgumentException(message: "Total must be greater than Zero");
            }
            Total = total;
            if (required >= total)
            {
                throw new ArgumentException(message: "Required must be less than Total");
            }
            Required = required;
            MeritBadges = new List<MeritBadge>();
        }

        /// <summary>
        /// How many of the badges must be Eagle-required.
        /// </summary>
        public int Required { get; protected set; }
        /// <summary>
        /// How many of the badges may be any badge (<see cref="Total"/> minus <see cref="Required"/>).
        /// </summary>
        public int Elective => Total - Required;
        /// <summary>
        /// The total number of badges needed.
        /// </summary>
        public int Total { get; protected set; }

        /// <summary>
        /// The badges allocated to this requirement so far, earned or only started.
        /// </summary>
        public List<MeritBadge> MeritBadges { get; protected set; }

        /// <summary>
        /// Derived from the allocated merit badges. Assignments are ignored: this requirement is
        /// earned only when its badges are, so a stored date would contradict <see cref="Earned"/>.
        /// </summary>
        public override DateTime? DateEarned
        {
            get => Earned ? MeritBadges.Max(mb => mb.DateEarned) : null;
            set { }
        }

        public override bool Earned => MeritBadges.Where(x => x.Earned).Count() == Total;

        /// <summary>
        /// Adds the badge to any open slot, letting an Eagle-required badge fill an elective slot.
        /// </summary>
        /// <returns>True if the badge was added.</returns>
        public bool AddAny(MeritBadge badge)
        {
            return this.Add(badge, true);
        }

        /// <summary>
        /// Adds the badge if there is a suitable slot: an Eagle-required badge takes a required slot (or an elective
        /// one when <paramref name="forceElective"/> is set); any other badge takes an elective slot.
        /// </summary>
        /// <returns>True if the badge was added; false if the requirement has no suitable slot left.</returns>
        public virtual bool Add(MeritBadge badge, bool forceElective = false)
        {
            int count = MeritBadges.Count;
            if (count < Total)
            {
                int req = MeritBadges.Count(mb => mb.EagleRequired);
                if (req > Required)
                {
                    req = Required;
                }
                int ele = count - req;

                if ((badge.EagleRequired && (req < Required || forceElective))
                    || (!badge.EagleRequired && ele < Elective)
                    )
                {
                    MeritBadges.Add(badge);
                    return true;
                }
            }
            return false;
        }
    }
}
