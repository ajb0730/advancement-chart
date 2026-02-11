using System;
using System.Collections.Generic;
using System.Linq;
using advancementchart.Model.Ranks;

namespace advancementchart.Model
{
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

        public string BsaMemberId { get; protected set; }
        public string FirstName { get; protected set; }
        public string MiddleName { get; protected set; }
        public string LastName { get; protected set; }
        public string NickName { get; set; }
        public string DisplayName => $"{(string.IsNullOrWhiteSpace(NickName) ? FirstName : NickName)} {LastName}";
        public string Patrol { get; set; }
        public DateTime DateOfBirth { get; set; }

        public Scout Scout { get; protected set; }
        public Tenderfoot Tenderfoot { get; protected set; }
        public SecondClass SecondClass { get; protected set; }
        public FirstClass FirstClass { get; protected set; }
        public Star Star { get; protected set; }
        public Life Life { get; protected set; }
        public Eagle Eagle { get; protected set; }
        public List<Palm> EaglePalms { get; protected set; }

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

        /// Gets the Nth Palm of the given type (i.e. Second Bronze Palm)
        /// Will create a new, blank Palm and add it to the list if that Palm would be
        /// next available.
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


        public List<MeritBadge> MeritBadges { get; protected set; }

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

        public void AddPartial(string badgeName, string version)
        {
            if (!MeritBadges.Any(x => x.Name == badgeName))
            {
                MeritBadge partial = new MeritBadge(name: badgeName, description: version, earned: null);
                partial.Started = true;
                this.Add(partial);
            }
        }

        private bool runOnce = false;

        public void AllocateMeritBadges()
        {
            if (runOnce) { return; }
            runOnce = true;
            // run once for the earned badges
            foreach (MeritBadge badge in MeritBadges.Where(x => x.Earned).OrderBy(mb => mb.DateEarned).ThenBy(mb => mb.BsaId))
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

                bool eagleAdded = false;
                if (!Eagle.MbReq.Earned)
                {
                    eagleAdded = Eagle.MbReq.Add(badge);
                }

                if (!added && !eagleAdded)
                {
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
                        Palm.PalmType nextType = Palm.PalmType.None;
                        switch (lastPalm)
                        {
                            case Palm.PalmType.None:
                            case Palm.PalmType.Silver:
                                nextType = Palm.PalmType.Bronze;
                                break;
                            case Palm.PalmType.Bronze:
                                nextType = Palm.PalmType.Gold;
                                break;
                            case Palm.PalmType.Gold:
                                nextType = Palm.PalmType.Silver;
                                break;
                        }
                        var newPalm = new Palm(nextType);
                        added = newPalm.MbReq.AddAny(badge);
                        EaglePalms.Add(newPalm);
                    }
                }
            }
            // run again for the partials
            foreach (MeritBadge badge in MeritBadges.Where(x => !x.Earned && x.Started).OrderBy(mb => mb.BsaId))
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

                bool eagleAdded = false;
                if (!Eagle.MbReq.Earned)
                {
                    eagleAdded = Eagle.MbReq.Add(badge);
                }

                if (!added && !eagleAdded)
                {
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
                        Palm.PalmType nextType = Palm.PalmType.None;
                        switch (lastPalm)
                        {
                            case Palm.PalmType.None:
                            case Palm.PalmType.Silver:
                                nextType = Palm.PalmType.Bronze;
                                break;
                            case Palm.PalmType.Bronze:
                                nextType = Palm.PalmType.Gold;
                                break;
                            case Palm.PalmType.Gold:
                                nextType = Palm.PalmType.Silver;
                                break;
                        }
                        var newPalm = new Palm(nextType);
                        added = newPalm.MbReq.AddAny(badge);
                        EaglePalms.Add(newPalm);
                    }
                }
            }
        }

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
