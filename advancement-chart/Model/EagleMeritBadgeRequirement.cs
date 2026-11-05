using System;
using System.Collections.Generic;
using System.Linq;

namespace advancementchart.Model
{
    /// <summary>
    /// The Eagle rank's merit badge requirement: 21 badges, of which 13 or 14 are Eagle-required (14 while
    /// Citizenship in Society is required, between <see cref="CitSocietyCutover"/> and <see cref="CitSocietyEnd"/>).
    /// The count is fixed when the requirement is constructed; <see cref="MeritBadge.EagleRequired"/> is evaluated per day.
    /// </summary>
    public class EagleMeritBadgeRequirement : MeritBadgeRequirement
    {
        /// <summary>
        /// First day on which Citizenship in Society counts as Eagle-required.
        /// </summary>
        public static readonly DateTime CitSocietyCutover = new DateTime(2022, 7, 1);
        /// <summary>
        /// Last day (inclusive) of the window in which Citizenship in Society counts as Eagle-required.
        /// </summary>
        public static readonly DateTime CitSocietyEnd = new DateTime(2026, 2, 26);

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
        /// True if the badge at <paramref name="index"/> belongs to a mutually exclusive group (e.g. Lifesaving
        /// and Emergency Preparedness) and another badge of that group is earlier in <paramref name="badges"/>.
        /// </summary>
        private static bool HasEarlierEquivalent(IList<MeritBadge> badges, int index)
        {
            var equivalents = MeritBadge.GetEagleEquivalents(badges[index].Name);
            for (int i = 0; i < index; i++)
            {
                if (equivalents.Contains(badges[i].Name))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Only the first badge of each mutually exclusive group fills a required slot; the others count
        /// as electives.
        /// </summary>
        protected override int RequiredSlotsUsed()
        {
            int used = 0;
            for (int i = 0; i < MeritBadges.Count; i++)
            {
                if (MeritBadges[i].EagleRequired && !HasEarlierEquivalent(MeritBadges, i))
                {
                    used++;
                }
            }
            return Math.Min(Required, used);
        }

        /// <summary>
        /// Splits the earned badges (oldest first) into those filling Eagle-required slots and those
        /// counting as electives. When a scout has earned several badges of an either-or group, the
        /// first fills the required slot and the others are electives.
        /// </summary>
        public (List<MeritBadge> Required, List<MeritBadge> Elective) SplitEarned()
        {
            var earned = MeritBadges.Where(mb => mb.Earned).OrderBy(mb => mb.DateEarned).ThenBy(mb => mb.BsaId).ToList();
            var required = new List<MeritBadge>();
            var elective = new List<MeritBadge>();
            for (int i = 0; i < earned.Count; i++)
            {
                if (earned[i].EagleRequired && !HasEarlierEquivalent(earned, i))
                {
                    required.Add(earned[i]);
                }
                else
                {
                    elective.Add(earned[i]);
                }
            }
            return (required, elective);
        }

        /// <summary>
        /// Adds the badge if there is a slot for it. An Eagle-required badge takes a required slot; if another
        /// badge from its mutually exclusive group is already counted (e.g. Lifesaving when Emergency
        /// Preparedness is there) it counts as an elective instead, as BSA rules allow. See
        /// <see cref="MeritBadge.MutuallyExclusiveGroups"/>. The <paramref name="forceElective"/> argument is
        /// ignored: <see cref="MeritBadgeRequirement.AddAny"/> behaves like Add here.
        /// </summary>
        /// <returns>True if the badge was added.</returns>
        public override bool Add(MeritBadge badge, bool forceElective = false)
        {
            if (badge.EagleRequired)
            {
                var equivalents = MeritBadge.GetEagleEquivalents(badge.Name);
                if (equivalents.Length > 0 && MeritBadges.Any(mb => equivalents.Contains(mb.Name)))
                {
                    // Only an earned badge can count as an elective; a merely started one is not a
                    // placeholder worth reserving a slot for (the reports only count earned electives).
                    if (badge.Earned && MeritBadges.Count < Total && MeritBadges.Count - RequiredSlotsUsed() < Elective)
                    {
                        MeritBadges.Add(badge);
                        return true;
                    }
                    return false;
                }
            }
            return base.Add(badge, false);
        }
    }
}
