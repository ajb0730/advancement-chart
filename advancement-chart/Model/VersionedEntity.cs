namespace advancementchart.Model
{
    public  class VersionedEntity : BaseEntity
    {
        protected VersionedEntity()
            : base()
        { }

        public VersionedEntity(string name, string tag, string description, string version = "2016")
            : base(name, tag, description)
        {
            Version = version;
        }

        public string Version { get; protected set; }


    }
}
