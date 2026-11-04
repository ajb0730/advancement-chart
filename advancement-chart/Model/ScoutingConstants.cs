namespace advancementchart.Model
{
    /// <summary>
    /// Time-based Scouts BSA rules that were previously scattered as bare numbers.
    /// </summary>
    public static class ScoutingConstants
    {
        /// <summary>Star requirement 1: months active since earning First Class.</summary>
        public const int StarActiveMonths = 4;

        /// <summary>Life requirement 1: months active since earning Star.</summary>
        public const int LifeActiveMonths = 6;

        /// <summary>Eagle requirement 1: months active since earning Life.</summary>
        public const int EagleActiveMonths = 6;

        /// <summary>Palm requirement 1: months active since earning Eagle or the previous palm.</summary>
        public const int PalmActiveMonths = 3;

        /// <summary>Personal Fitness requires a 12-week fitness program.</summary>
        public const int PersonalFitnessWeeks = 12;

        /// <summary>Personal Management requires a 13-week budget and record-keeping period.</summary>
        public const int PersonalManagementWeeks = 13;

        /// <summary>Family Life requires 90 days of carrying out family projects.</summary>
        public const int FamilyLifeDays = 90;

        /// <summary>
        /// Days after Tenderfoot 6a (physical-fitness test) is signed off by which 6c (retest) is
        /// expected; the Advancement Report flags scouts who are past this.
        /// </summary>
        public const int Tenderfoot6cWarningDays = 45;
    }
}
