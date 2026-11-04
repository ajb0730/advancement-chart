using System.Collections.Generic;
using advancementchart.Model;

namespace advancementchart.Reports
{
    /// <summary>
    /// A report generated from the troop's scouts.
    /// </summary>
    public interface IReport
    {
        /// <summary>
        /// The scouts the report is generated from.
        /// </summary>
        List<TroopMember> Scouts { get; set; }
        /// <summary>
        /// Generates the report.
        /// </summary>
        /// <param name="outputFileName">Where to write the report; reports that only print to the console ignore it.</param>
        void Run(string outputFileName);
    }
}
