namespace advancementchart.Model
{
    public abstract class BaseEntity
    {
        protected BaseEntity()
        { }

        protected BaseEntity(string name, string tag, string description)
            : this()
        {
            Name = name;
            Tag = tag;
            Description = description;
        }

        public string Name { get; protected set; }
        public string Tag { get; protected set; }
        public string Description { get; protected set; }
    }
}
