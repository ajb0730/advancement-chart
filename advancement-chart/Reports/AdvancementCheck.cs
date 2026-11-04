using System;
using System.Collections.Generic;
using System.Linq;
using advancementchart.Model;

namespace advancementchart.Reports
{
    /// <summary>How serious an <see cref="AdvancementIssue"/> is.</summary>
    public enum IssueSeverity
    {
        /// <summary>Worth a look, e.g. a time-based requirement that appears overdue.</summary>
        Warning,
        /// <summary>The data is inconsistent, e.g. a rank earned without the rank before it.</summary>
        Error,
    }

    /// <summary>A single inconsistency found by <see cref="AdvancementCheck"/>.</summary>
    public class AdvancementIssue
    {
        public AdvancementIssue(IssueSeverity severity, string scoutName, string message)
        {
            Severity = severity;
            ScoutName = scoutName;
            Message = message;
        }

        public IssueSeverity Severity { get; }
        public string ScoutName { get; }
        public string Message { get; }

        public override string ToString()
        {
            string label = Severity == IssueSeverity.Error ? "ERROR" : "WARN ";
            return $"{label} [{ScoutName}] {Message}";
        }
    }

    /// <summary>
    /// Sanity-checks scouts' loaded advancement for logical inconsistencies. Scoutbook data is treated
    /// as authoritative, so problems are reported but never block the other reports.
    /// </summary>
    public class AdvancementCheck : IReport
    {
        public AdvancementCheck(List<TroopMember> scouts)
        {
            Scouts = scouts;
        }

        public List<TroopMember> Scouts { get; set; }

        /// <summary>Finds the inconsistencies for every active (non-Inactive) scout.</summary>
        public List<AdvancementIssue> Validate()
        {
            var issues = new List<AdvancementIssue>();
            foreach (var scout in this.Scouts.Where(s => !string.Equals(s.Patrol, "Inactive", StringComparison.OrdinalIgnoreCase)))
            {
                ValidateScout(scout, issues);
            }
            return issues;
        }

        /// <summary>Writes each issue found by <see cref="Validate"/> to standard error.</summary>
        /// <param name="outputFileName">Unused; this check only writes to the console.</param>
        public void Run(string outputFileName)
        {
            Console.WriteLine("Running Advancement Check");
            foreach (var issue in Validate())
            {
                Console.Error.WriteLine(issue.ToString());
            }
        }

        private static void ValidateScout(TroopMember scout, List<AdvancementIssue> issues)
        {
            string name = scout.DisplayName;

            // Eagle implies Life implies Star implies 1C implies 2C implies Tenderfoot implies Scout,
            // and each rank is earned on or after the one before it.
            Rank[] ladder = { scout.Scout, scout.Tenderfoot, scout.SecondClass, scout.FirstClass, scout.Star, scout.Life, scout.Eagle };
            for (int i = ladder.Length - 1; i > 0; i--)
            {
                Rank higher = ladder[i];
                Rank lower = ladder[i - 1];
                if (!higher.Earned)
                {
                    continue;
                }
                if (!lower.Earned)
                {
                    issues.Add(new AdvancementIssue(IssueSeverity.Error, name, $"has earned {higher.Name} but not {lower.Name}"));
                }
                else if (higher.DateEarned.Value < lower.DateEarned.Value)
                {
                    issues.Add(new AdvancementIssue(IssueSeverity.Error, name,
                        $"earned {higher.Name} ({higher.DateEarned.Value.ToShortDateString()}) before {lower.Name} ({lower.DateEarned.Value.ToShortDateString()})"));
                }
            }

            // 1C.1a implies 2C.1a
            if (RequirementEarned(scout.FirstClass, "1a", whenMissing: false) &&
                !RequirementEarned(scout.SecondClass, "1a", whenMissing: true))
            {
                issues.Add(new AdvancementIssue(IssueSeverity.Error, name, "has earned First Class #1a but not Second Class #1a"));
            }

            // The "active for N months" requirement of the next rank is due once the time has passed.
            CheckTimeRequirement(issues, name, scout.Star, scout.FirstClass, 4, "First Class");
            CheckTimeRequirement(issues, name, scout.Life, scout.Star, 6, "Star");
            CheckTimeRequirement(issues, name, scout.Eagle, scout.Life, 6, "Life");
        }

        private static void CheckTimeRequirement(List<AdvancementIssue> issues, string scoutName, Rank next, Rank previous, int months, string previousName)
        {
            if (!next.Earned && previous.Earned)
            {
                DateTime due = previous.DateEarned.Value.AddMonths(months);
                if (DateTime.Now >= due && !RequirementEarned(next, "1", whenMissing: true))
                {
                    issues.Add(new AdvancementIssue(IssueSeverity.Warning, scoutName,
                        $"{due} is {months} months since earning {previousName}, check {next.Name} #1"));
                }
            }
        }

        /// <summary>
        /// Whether the named requirement on the rank is earned, or <paramref name="whenMissing"/>
        /// if the rank has no requirement by that name.
        /// </summary>
        private static bool RequirementEarned(Rank rank, string name, bool whenMissing)
        {
            var req = rank.Requirements.Find(x => x.Name == name);
            return req == null ? whenMissing : req.Earned;
        }
    }
}