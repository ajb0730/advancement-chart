using System;
using System.Diagnostics;

namespace advancementchart.Model
{
    /// <summary>
    /// One requirement of a <see cref="Rank"/>, e.g. "1a". It counts as <see cref="Earned"/> when it has its own date
    /// or when its whole rank has been earned (earning a rank signs off everything under it).
    /// </summary>
    public class RankRequirement : BaseEntity
    {
        protected RankRequirement()
            : base()
        {
            Group = null;
        }

        /// <summary>
        /// Creates a requirement of <paramref name="rank"/>.
        /// </summary>
        /// <param name="name">Short name, e.g. "1a".</param>
        /// <param name="description">Full requirement text.</param>
        /// <param name="rank">The rank this requirement belongs to.</param>
        /// <param name="handbookPages">Handbook page reference, if any.</param>
        /// <param name="earned">When the requirement was completed, if it has been.</param>
        /// <param name="curriculumGroup">The Troop Guide topic this requirement belongs to, if any.</param>
        /// <param name="timeRequirementMonths">For "be active N months" requirements, the number of months.</param>
        /// <exception cref="ArgumentNullException"><paramref name="rank"/> is null.</exception>
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

        /// <summary>
        /// The rank this requirement belongs to.
        /// </summary>
        public Rank Rank { get; protected set; }
        /// <summary>
        /// When the requirement was completed, or null. Merit badge requirements derive this from their badges.
        /// </summary>
        public virtual DateTime? DateEarned { get; set; }
        /// <summary>
        /// Handbook page reference, if known.
        /// </summary>
        public string HandbookPages { get; protected set; }
        /// <summary>
        /// The Troop Guide topic this requirement belongs to, if any.
        /// </summary>
        public CurriculumGroup? Group { get; protected set; }
        /// <summary>
        /// For "be active N months since ..." requirements, N; otherwise null.
        /// </summary>
        public int? TimeRequirementMonths { get; protected set; }

        /// <summary>
        /// True if the requirement has a date or its rank has been earned.
        /// </summary>
        public virtual bool Earned => Rank.Earned || DateEarned.HasValue;
    }
}
