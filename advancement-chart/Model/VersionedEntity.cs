namespace advancementchart.Model
{
    public class VersionedEntity : BaseEntity
    {
        protected VersionedEntity()
            : base()
        { }

        public VersionedEntity(string name, string description, string version = "2016")
            : base(name, description)
        {
            Version = version;
        }

        public string Version { get; protected set; }
    }
}
