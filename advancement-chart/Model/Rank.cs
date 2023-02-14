using System;
using System.Collections.Generic;
using System.Drawing;

namespace advancementchart.Model
{
    public abstract class Rank : VersionedEntity
    {
        protected Rank()
            : base()
        { }

        public Rank(string name, string description, DateTime? earned = null, string version = "2016")
            : base(name,description,version)
        {
            DateEarned = earned;
            Requirements = new List<RankRequirement>();
            FillColor = Color.White;
            this.SwitchVersion(version);
        }

        public DateTime? DateEarned { get; set; }
        public List<RankRequirement> Requirements { get; protected set; }

        public bool Earned => DateEarned.HasValue;

        public Color FillColor { get; protected set; }

        public abstract void SwitchVersion(string version);

    }
}
