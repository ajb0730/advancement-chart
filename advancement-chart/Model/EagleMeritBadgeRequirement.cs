using System;
using System.Collections.Generic;
using System.Linq;

namespace advancementchart.Model
{
    /// <summary>
    /// The Eagle rank's merit badge requirement: 21 badges, of which 13 or 14 are Eagle-required (14 while
    /// Citizenship in Society is required, between <see cref="CitSocietyCutover"/> and <see cref="CitSocietyEnd"/>).
    /// </summary>
    public class EagleMeritBadgeRequirement : MeritBadgeRequirement
    {
        /// <summary>
        /// First date on which Citizenship in Society counts as Eagle-required.
        /// </summary>
        public static readonly DateTime CitSocietyCutover = new DateTime(2022, 7, 1);
        /// <summary>
        /// Last date of the window in which Citizenship in Society counts as Eagle-required.
        /// </summary>
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

        /// <summary>
        /// Creates the Eagle merit badge requirement for <paramref name="rank"/>.
        /// </summary>
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

        /// <summary>
        /// Adds the badge, except that an Eagle-required badge is rejected when a mutually exclusive alternative
        /// is already counted (e.g. Lifesaving when Emergency Preparedness is already there). See
        /// <see cref="MeritBadge.MutuallyExclusiveGroups"/>. Such a badge can still count as an elective elsewhere.
        /// </summary>
        /// <returns>True if the badge was added.</returns>
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
