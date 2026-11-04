using System;
using System.Diagnostics;

namespace advancementchart.Model
{
    public class RankRequirement : BaseEntity
    {
        protected RankRequirement()
            : base()
        {
            Group = null;
        }

        public RankRequirement(string name, string description, Rank rank, string handbookPages = "", DateTime? earned = null, CurriculumGroup? curriculumGroup = null, int? timeRequirementMonths = null)
            : base(name, description)
        {
            Rank = rank ?? throw new ArgumentNullException(nameof(rank));
            if (!string.IsNullOrWhiteSpace(handbookPages))
            {
                HandbookPages = handbookPages;
            }
            DateEarned = earned;
            Group = curriculumGroup;
            TimeRequirementMonths = timeRequirementMonths;
        }

        public Rank Rank { get; protected set; }
        public virtual DateTime? DateEarned { get; set; }
        public string HandbookPages { get; protected set; }
        public CurriculumGroup? Group { get; protected set; }
        public int? TimeRequirementMonths { get; protected set; }

        public virtual bool Earned => Rank.Earned || DateEarned.HasValue;
    }
}
