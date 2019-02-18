using System.Collections.Generic;
using advancementchart.Model;

namespace advancementchart.Reports
{
    public interface IReport
    {
        List<TroopMember> Scouts { get; set; }
        void Run(string outputFileName);
    }
}
