using System;
using System.Collections.Generic;
using System.Linq;

namespace advancementchart.Model
{
    public class MeritBadgeRequirement : RankRequirement
    {
        protected MeritBadgeRequirement()
            : base()
        {
            MeritBadges = new List<MeritBadge>();
        }

        public MeritBadgeRequirement(string name, string description, Rank rank, int required, int total, string version = "2016", string handbookPages = "", DateTime? earned = null)
            : base(name, description, rank, version, handbookPages, earned, null)
        {
            if(total <= 0)
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

        public int Required { get; protected set; }
        public int Elective => Total - Required;
        public int Total { get; protected set; }

        public List<MeritBadge> MeritBadges { get; protected set; }

        public new DateTime? DateEarned => MeritBadges.Count == Total ? MeritBadges.Max(mb => mb.DateEarned) : null;

        public override bool Earned => /*Rank.Earned ||*/ MeritBadges.Count == Total;

        public bool AddAny(MeritBadge badge)
        {
            return this.Add(badge, true);
        }

        public virtual bool Add(MeritBadge badge, bool forceElective = false)
        {
            int count = MeritBadges.Count;
            if(count < Total)
            {
                int req = MeritBadges.Count(mb => mb.EagleRequired);
                if(req > Required)
                {
                    req = Required;
                }
                int ele = count - req;

                if (   (badge.EagleRequired  &&  (req < Required || forceElective))
                    || (!badge.EagleRequired &&   ele < Elective                  )
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
