namespace advancementchart.Model
{
    /// <summary>
    /// A named thing in the advancement model (a rank or a requirement) with a short name and a longer description.
    /// </summary>
    public abstract class BaseEntity
    {
        protected BaseEntity()
        { }

        protected BaseEntity(string name, string description)
            : this()
        {
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Short name, e.g. "Star" for a rank or "3a" for a requirement.
        /// </summary>
        public string Name { get; protected set; }
        /// <summary>
        /// Full text of the rank or requirement.
        /// </summary>
        public string Description { get; protected set; }
    }
}
