using System;
using System.Diagnostics;

namespace advancementchart.Model
{
    public class RankRequirement : VersionedEntity
    {
        protected RankRequirement()
            : base()
        { }

        public RankRequirement(string name, string description, Rank rank, string version = "2016", string handbookPages = "", DateTime? earned = null, CurriculumGroup? curriculumGroup = null)
            : base(name, description, version)
        {
            if(Version != rank.Version)
            {
                throw new ArgumentException(message: $"Version mismatch: RankRequirement.Version '{Version}' != Rank.Version '{rank.Version}'");
            }

            Rank = rank;
            if (!string.IsNullOrWhiteSpace(handbookPages))
            {
                HandbookPages = handbookPages;
            }
            if (DateEarned != null)
            {
                DateEarned = earned;
            }
            Group = curriculumGroup;
        }

        public Rank Rank { get; protected set; }
        public virtual DateTime? DateEarned { get; set; }
        public string HandbookPages { get; protected set; }
        public CurriculumGroup? Group { get; protected set; }

        public virtual bool Earned => Rank.Earned || DateEarned.HasValue;
    }
}
