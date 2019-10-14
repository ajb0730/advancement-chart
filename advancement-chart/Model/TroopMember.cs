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
        public string Patrol { get; set; }

        public Scout Scout { get; protected set; }
        public Tenderfoot Tenderfoot { get; protected set; }
        public SecondClass SecondClass { get; protected set; }
        public FirstClass FirstClass { get; protected set; }
        public Star Star { get; protected set; }
        public Life Life { get; protected set; }
        public Eagle Eagle { get; protected set; }
        public List<Palm> EaglePalms { get; protected set; }

        protected List<MeritBadge> MeritBadges { get; set; }

        public void Add(MeritBadge badge)
        {
            MeritBadges.Add(badge);
        }

        public void AllocateMeritBadges()
        {
            foreach (MeritBadge badge in MeritBadges.OrderBy(mb => mb.DateEarned).ThenBy(mb => mb.Name))
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

        public Dictionary<CurriculumGroup,List<RankRequirement>> GetRequirementsByGroup()
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
