namespace advancementchart.Model.Ranks
{
    public class NoRank : Rank
    {
        public NoRank()
            : base(name: "No Rank", description: "", earned: null, version: "2016")
        { }

        override public void SwitchVersion(string version) {
            this.Version = version;
        }
    }
}
