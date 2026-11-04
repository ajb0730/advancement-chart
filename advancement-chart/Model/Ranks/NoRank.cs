namespace advancementchart.Model.Ranks
{
    /// <summary>
    /// Placeholder "rank" for a scout who has not earned any rank yet.
    /// </summary>
    public class NoRank : Rank
    {
        public NoRank()
            : base(name: "No Rank", description: "", earned: null)
        { }
    }
}
