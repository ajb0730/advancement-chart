using System;
using System.Collections.Generic;
using System.Linq;

namespace advancementchart.Model
{
    public class EagleMeritBadgeRequirement : MeritBadgeRequirement
    {
        protected EagleMeritBadgeRequirement()
            : base()
        { }

        public EagleMeritBadgeRequirement(string name, string description, Rank rank, string version = "2016", string handbookPages = "", DateTime? earned = null)
            : base(name: name, description: description, rank: rank, required: 13, total: 21, version: version, handbookPages: handbookPages)
        { }

        public override bool Add(MeritBadge badge, bool forceElective = false)
        {
            if (badge.EagleRequired)
            {
                // (a) First Aid
                // (b) Citizenship in the Community
                // (c) Citizenship in the Nation
                // (d) Citizenship in the World
                // (e) Communication
                // (f) Cooking
                // (g) Personal Fitness
                // (h) Emergency Preparedness or Lifesaving
                if((badge.Name == "Emergency Preparedness" && MeritBadges.Any(mb => mb.Name == "Lifesaving"))
                    || (badge.Name == "Lifesaving" && MeritBadges.Any(mb => mb.Name == "Emergency Preparedness")))
                {
                    return false;
                }
                // (i) Environmental Science or Sustainability
                if((badge.Name == "Environmental Science" && MeritBadges.Any(mb => mb.Name == "Sustainability"))
                    || (badge.Name == "Sustainability" && MeritBadges.Any(mb => mb.Name == "Environmental Science")))
                {
                    return false;
                }
                // (j) Personal Management
                // (k) Swimming or Hiking or Cycling
                if((badge.Name == "Swimming" && MeritBadges.Any(mb => mb.Name == "Hiking" || mb.Name == "Cycling"))
                    || (badge.Name == "Hiking" && MeritBadges.Any(mb => mb.Name == "Swimming" || mb.Name == "Cycling"))
                    || (badge.Name == "Cycling" && MeritBadges.Any(mb => mb.Name == "Swimming" || mb.Name == "Hiking")))
                {
                    return false;
                }
                // (l) Camping
                // (m) Family Life
            }
            return base.Add(badge, false);
        }
    }
}
