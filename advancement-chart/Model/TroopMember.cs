using System;
using System.Collections.Generic;
using System.Linq;
using advancementchart.Model.Ranks;

namespace advancementchart.Model
{
    /// <summary>
    /// One scout: identity, patrol, the progress of every rank, any Eagle Palms, and merit badges.
    /// Two members are equal when they have the same BSA member id.
    /// </summary>
    public class TroopMember
    {
        protected TroopMember()
        {
            Scout = new Scout();
            Tenderfoot = new Tenderfoot();
            SecondClass = new SecondClass();
            FirstClass = new FirstClass();
            Star = new Star();
            Life = new Life();
            Eagle = new Eagle();
            EaglePalms = new List<Palm>();
            MeritBadges = new List<MeritBadge>();
        }

        /// <summary>
        /// Creates a scout with all ranks unearned.
        /// </summary>
        public TroopMember(string memberId, string firstName, string middleName, string lastName, string patrol = "Unassigned")
            : this()
        {
            BsaMemberId = memberId;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            Patrol = patrol;
        }

        public override bool Equals(object obj)
        {
            if (obj is TroopMember)
            {
                return ((TroopMember)obj).BsaMemberId == this.BsaMemberId;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return BsaMemberId.GetHashCode();
        }

        /// <summary>
        /// The scout's BSA member id; the key used to merge the Scoutbook exports.
        /// </summary>
        public string BsaMemberId { get; protected set; }
        public string FirstName { get; protected set; }
        public string MiddleName { get; protected set; }
        public string LastName { get; protected set; }
        /// <summary>
        /// Preferred name, if the scout goes by one.
        /// </summary>
        public string NickName { get; set; }
        /// <summary>The nickname if one is set, otherwise the first name.</summary>
        public string PreferredFirstName => string.IsNullOrWhiteSpace(NickName) ? FirstName : NickName;
        public string DisplayName => $"{PreferredFirstName} {LastName}";
        /// <summary>
        /// The scout's patrol; "Inactive" scouts are left out of the reports.
        /// </summary>
        public string Patrol { get; set; }
        /// <summary>
        /// Date of birth, or <c>default(DateTime)</c> when unknown. The Eagle report derives deadlines from it.
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        public Scout Scout { get; protected set; }
        public Tenderfoot Tenderfoot { get; protected set; }
        public SecondClass SecondClass { get; protected set; }
        public FirstClass FirstClass { get; protected set; }
        public Star Star { get; protected set; }
        public Life Life { get; protected set; }
        public Eagle Eagle { get; protected set; }
        /// <summary>
        /// The Eagle Palms the scout has earned or is working on, in order (Bronze, Gold, Silver, repeating).
        /// </summary>
        public List<Palm> EaglePalms { get; protected set; }

        /// <summary>
        /// The highest rank earned (not counting Palms), or a <see cref="NoRank"/> if none.
        /// </summary>
        public Rank CurrentRank
        {
            get
            {
                if (this.Eagle.Earned) { return this.Eagle; }
                if (this.Life.Earned) { return this.Life; }
                if (this.Star.Earned) { return this.Star; }
                if (this.FirstClass.Earned) { return this.FirstClass; }
                if (this.SecondClass.Earned) { return this.SecondClass; }
                if (this.Tenderfoot.Earned) { return this.Tenderfoot; }
                if (this.Scout.Earned) { return this.Scout; }
                return new NoRank();
            }
        }

        /// <summary>
        /// The most recently earned Eagle Palm if there is one, otherwise <see cref="CurrentRank"/>.
        /// </summary>
        public Rank CurrentRankWithPalms
        {
            get
            {
                if (this.EaglePalms.Any() && this.EaglePalms.Where(x => x.Earned).Any())
                {
                    return this.EaglePalms.Where(x => x.Earned).Last();
                }
                else
                {
                    return this.CurrentRank;
                }
            }
        }

        /// <summary>
        /// Gets the Nth Palm of the given type (e.g. the second Bronze Palm). If that Palm would be the
        /// next one in the Bronze, Gold, Silver cycle, a new blank Palm is created and added to
        /// <see cref="EaglePalms"/>.
        /// </summary>
        /// <returns>The Palm, or null if it cannot exist yet (an earlier Palm in the cycle is missing).</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="ordinal"/> is not positive.</exception>
        public Palm GetNthPalm(Palm.PalmType type, int ordinal = 1)
        {
            if (ordinal <= 0)
                throw new ArgumentOutOfRangeException(nameof(ordinal), "Ordinal must be positive.");

            var palmsOfType = this.EaglePalms.Where(x => x.Type == type);
            if (palmsOfType.Count() >= ordinal)
            {
                return palmsOfType.ElementAt(ordinal - 1);
            }
            else if (palmsOfType.Count() == ordinal - 1)
            {
                bool canCreate;
                if (!this.EaglePalms.Any())
                {
                    // First palm must be Bronze
                    canCreate = type == Palm.PalmType.Bronze;
                }
                else
                {
                    Palm lastPalm = this.EaglePalms.Last();
                    canCreate =
                        (type == Palm.PalmType.Bronze && lastPalm.Type == Palm.PalmType.Silver)
                        || (type == Palm.PalmType.Gold && lastPalm.Type == Palm.PalmType.Bronze)
                        || (type == Palm.PalmType.Silver && lastPalm.Type == Palm.PalmType.Gold);
                }
                if (canCreate)
                {
                    Palm palm = new Palm(type);
                    this.EaglePalms.Add(palm);
                    return palm;
                }
            }
            return null;
        }

        /// <summary>
        /// The next rank to work on: the first unearned rank, or the next Palm in the Bronze, Gold, Silver cycle once Eagle is earned.
        /// </summary>
        public Rank NextRank
        {
            get
            {
                if (!this.Scout.Earned) { return this.Scout; }
                if (!this.Tenderfoot.Earned) { return this.Tenderfoot; }
                if (!this.SecondClass.Earned) { return this.SecondClass; }
                if (!this.FirstClass.Earned) { return this.FirstClass; }
                if (!this.Star.Earned) { return this.Star; }
                if (!this.Life.Earned) { return this.Life; }
                if (!this.Eagle.Earned) { return this.Eagle; }
                Palm.PalmType pt = Palm.PalmType.None;
                foreach (var palm in this.EaglePalms)
                {
                    if (!palm.Earned) { return palm; }
                    else { pt = palm.Type; }
                }
                switch (pt)
                {
                    case Palm.PalmType.None:
                    case Palm.PalmType.Silver:
                        return new Palm(Palm.PalmType.Bronze);
                    case Palm.PalmType.Bronze:
                        return new Palm(Palm.PalmType.Gold);
                    case Palm.PalmType.Gold:
                        return new Palm(Palm.PalmType.Silver);
                }
                throw new ArgumentOutOfRangeException();
            }
        }


        /// <summary>
        /// Every merit badge the scout has earned or started. Call <see cref="AllocateMeritBadges"/> to distribute them across requirements.
        /// </summary>
        public List<MeritBadge> MeritBadges { get; protected set; }

        /// <summary>
        /// Records a badge. If the scout already has that badge, an earned copy replaces a merely started one; otherwise the first is kept.
        /// </summary>
        public void Add(MeritBadge badge)
        {
            if (!MeritBadges.Any(x => x.Name == badge.Name))
            {
                MeritBadges.Add(badge);
            }
            else
            {
                var existing = MeritBadges.First(x => x.Name == badge.Name);
                if (!existing.Earned && badge.Earned)
                {
                    MeritBadges.Remove(existing);
                    MeritBadges.Add(badge);
                }
            }
        }

        /// <summary>
        /// Records that the scout has started the named badge, unless the scout already has it.
        /// </summary>
        public void AddPartial(string badgeName, string version)
        {
            // Add keeps an existing badge, so a started copy never replaces one the scout already has.
            MeritBadge partial = new MeritBadge(name: badgeName, description: version, earned: null);
            partial.Started = true;
            this.Add(partial);
        }

        private bool runOnce = false;

        /// <summary>
        /// Distributes the scout's merit badges across the merit badge requirements of Star, Life, Eagle,
        /// and the Eagle Palms. Safe to call more than once; only the first call does any work.
        /// </summary>
        /// <remarks>
        /// Earned badges are placed first, oldest first, so the earliest badges fill the earliest
        /// requirements; badges that are only started follow. A badge can count toward Star/Life
        /// and toward Eagle at the same time (they run in parallel), but only a badge that fits
        /// neither spills over to a Palm, creating new Palms (Bronze, Gold, Silver, Bronze, ...)
        /// as needed.
        /// </remarks>
        public void AllocateMeritBadges()
        {
            if (runOnce) { return; }
            runOnce = true;

            foreach (MeritBadge badge in MeritBadges.Where(x => x.Earned).OrderBy(mb => mb.DateEarned).ThenBy(mb => mb.BsaId))
            {
                AllocateBadge(badge);
            }
            foreach (MeritBadge badge in MeritBadges.Where(x => !x.Earned && x.Started).OrderBy(mb => mb.BsaId))
            {
                AllocateBadge(badge);
            }
        }

        private void AllocateBadge(MeritBadge badge)
        {
            bool addedToStarOrLife = TryAllocateToStarOrLife(badge);

            bool addedToEagle = !Eagle.MbReq.Earned && Eagle.MbReq.Add(badge);

            if (!addedToStarOrLife && !addedToEagle)
            {
                AllocateToPalms(badge);
            }
        }

        /// <summary>
        /// Star first, then Life: Eagle-required badges take the "required" slots, and anything left
        /// fills the elective slots.
        /// </summary>
        private bool TryAllocateToStarOrLife(MeritBadge badge)
        {
            bool added = false;
            if (!Star.MbReq.Earned)
            {
                added = Star.MbReq.Add(badge);
            }
            if (!added && !Life.MbReq.Earned)
            {
                added = Life.MbReq.Add(badge);
            }
            if (!added && !Star.MbReq.Earned)
            {
                added = Star.MbReq.AddAny(badge);
            }
            if (!added && !Life.MbReq.Earned)
            {
                added = Life.MbReq.AddAny(badge);
            }
            return added;
        }

        /// <summary>
        /// Places the badge in the first incomplete existing Palm, or starts a new Palm for it.
        /// </summary>
        private void AllocateToPalms(MeritBadge badge)
        {
            bool added = false;
            Palm.PalmType lastPalm = Palm.PalmType.None;
            foreach (Palm palm in EaglePalms)
            {
                lastPalm = palm.Type;
                if (!added && !palm.MbReq.Earned)
                {
                    added = palm.MbReq.AddAny(badge);
                }
            }
            if (!added)
            {
                var newPalm = new Palm(NextPalmType(lastPalm));
                newPalm.MbReq.AddAny(badge);
                EaglePalms.Add(newPalm);
            }
        }

        /// <summary>Palms cycle Bronze, Gold, Silver, then start over at Bronze.</summary>
        private static Palm.PalmType NextPalmType(Palm.PalmType lastPalm)
        {
            switch (lastPalm)
            {
                case Palm.PalmType.Bronze:
                    return Palm.PalmType.Gold;
                case Palm.PalmType.Gold:
                    return Palm.PalmType.Silver;
                default:
                    // None or Silver
                    return Palm.PalmType.Bronze;
            }
        }

        /// <summary>
        /// The requirements of ranks up to First Class, grouped by Troop Guide topic (requirements with no topic are omitted).
        /// </summary>
        public Dictionary<CurriculumGroup, List<RankRequirement>> GetRequirementsByGroup()
        {
            List<RankRequirement> requirements = new List<RankRequirement>();
            requirements.AddRange(Scout.Requirements.Where(r => r is RankRequirement && r.Group.HasValue));
            requirements.AddRange(Tenderfoot.Requirements.Where(r => r is RankRequirement && r.Group.HasValue));
            requirements.AddRange(SecondClass.Requirements.Where(r => r is RankRequirement && r.Group.HasValue));
            requirements.AddRange(FirstClass.Requirements.Where(r => r is RankRequirement && r.Group.HasValue));

            Dictionary<CurriculumGroup, List<RankRequirement>> groups = new Dictionary<CurriculumGroup, List<RankRequirement>>();
            foreach (var requirement in requirements)
            {
                if (!groups.ContainsKey(requirement.Group.Value))
                {
                    groups.Add(requirement.Group.Value, new List<RankRequirement>());
                }
                groups[requirement.Group.Value].Add(requirement);
            }
            return groups;
        }
    }
}
