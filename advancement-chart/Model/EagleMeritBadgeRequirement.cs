using System;
using System.Collections.Generic;
using System.Linq;

namespace advancementchart.Model
{
    public class EagleMeritBadgeRequirement : MeritBadgeRequirement
    {
        public static readonly DateTime CitSocietyCutover = new DateTime(2022, 7, 1);

        protected EagleMeritBadgeRequirement()
            : base()
        { }

        public EagleMeritBadgeRequirement(string name, string description, Rank rank, string version = "2016", string handbookPages = "", DateTime? earned = null)
            : base(name: name, description: description, rank: rank, required: 14, total: 21, version: version, handbookPages: handbookPages)
        {
            if (DateTime.Now < CitSocietyCutover)
            {
                this.Required = 13;
            }
            else
            {
                this.Required = 14;
            }
        }

        public override bool Add(MeritBadge badge, bool forceElective = false)
        {
            if (badge.EagleRequired)
            {
                var equivalents = MeritBadge.GetEagleEquivalents(badge.Name);
                if (equivalents.Length > 0 && MeritBadges.Any(mb => equivalents.Contains(mb.Name)))
                {
                    return false;
                }
            }
            return base.Add(badge, false);
        }
    }
}
