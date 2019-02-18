namespace advancementchart.Model
{
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

        public string Name { get; protected set; }
        public string Description { get; protected set; }
    }
}
