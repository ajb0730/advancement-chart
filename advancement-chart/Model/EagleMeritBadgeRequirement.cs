using System;
using System.Collections.Generic;
using System.Linq;

namespace advancementchart.Model
{
    public class EagleMeritBadgeRequirement : MeritBadgeRequirement
    {
        public static readonly DateTime CitSocietyCutover = new DateTime(2022, 7, 1);
        public static readonly DateTime CitSocietyEnd = new DateTime(2026, 12, 31);

        /// <summary>
        /// Whether Citizenship in Society counts as Eagle-required on the given day: from the cutover
        /// date through the end date, inclusive of both whole days.
        /// </summary>
        internal static bool IsCitizenshipInSocietyRequired(DateTime now)
        {
            return now.Date >= CitSocietyCutover.Date && now.Date <= CitSocietyEnd.Date;
        }

        protected EagleMeritBadgeRequirement()
            : base()
        { }

        public EagleMeritBadgeRequirement(string name, string description, Rank rank, string handbookPages = "", DateTime? earned = null)
            : base(name: name, description: description, rank: rank, required: 14, total: 21, handbookPages: handbookPages)
        {
            if (IsCitizenshipInSocietyRequired(DateTime.Now))
            {
                this.Required = 14;
            }
            else
            {
                this.Required = 13;
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
