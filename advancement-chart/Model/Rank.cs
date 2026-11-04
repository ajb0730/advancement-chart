using System;
using System.Collections.Generic;
using System.Drawing;

namespace advancementchart.Model
{
    /// <summary>
    /// A Scouts BSA rank (Scout through Eagle, or an Eagle Palm) and its requirements.
    /// A rank is <see cref="Earned"/> once it has a <see cref="DateEarned"/>.
    /// </summary>
    public class Rank : BaseEntity
    {
        protected Rank()
            : base()
        { }

        /// <summary>
        /// Creates a rank with no requirements; subclasses add theirs.
        /// </summary>
        public Rank(string name, string description, DateTime? earned = null)
            : base(name, description)
        {
            DateEarned = earned;
            Requirements = new List<RankRequirement>();
            FillColor = Color.White;
        }

        /// <summary>
        /// When the rank was earned (its board of review date), or null if it has not been.
        /// </summary>
        public DateTime? DateEarned { get; set; }
        /// <summary>
        /// The rank's requirements, in handbook order.
        /// </summary>
        public List<RankRequirement> Requirements { get; protected set; }

        /// <summary>
        /// True once the rank has a <see cref="DateEarned"/>.
        /// </summary>
        public bool Earned => DateEarned.HasValue;

        /// <summary>
        /// Background color used for this rank's columns in the advancement chart.
        /// </summary>
        public Color FillColor { get; protected set; }
    }
}
