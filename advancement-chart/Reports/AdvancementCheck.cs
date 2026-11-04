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
        /// <summary>
        /// Creates an issue.
        /// </summary>
        public AdvancementIssue(IssueSeverity severity, string scoutName, string message)
        {
            Severity = severity;
            ScoutName = scoutName;
            Message = message;
        }

        /// <summary>
        /// How serious the issue is.
        /// </summary>
        public IssueSeverity Severity { get; }
        /// <summary>
        /// Display name of the scout the issue is about.
        /// </summary>
        public string ScoutName { get; }
        /// <summary>
        /// What is wrong.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// The issue as printed by <see cref="AdvancementCheck.Run"/>, e.g. <c>ERROR [Pat Doe] has earned Eagle but not Life</c>.
        /// </summary>
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
        /// <summary>
        /// Creates the check for the given scouts.
        /// </summary>
        public AdvancementCheck(List<TroopMember> scouts)
        {
            Scouts = scouts;
        }

        /// <summary>
        /// The scouts to check.
        /// </summary>
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
                if (!higher.Earned)
                {
                    continue;
                }
                if (!ladder[i - 1].Earned)
                {
                    issues.Add(new AdvancementIssue(IssueSeverity.Error, name, $"has earned {higher.Name} but not {ladder[i - 1].Name}"));
                }

                // Compare dates against the nearest earned rank below, so a missing rung in between
                // does not hide a date inversion.
                Rank lower = ladder.Take(i).LastOrDefault(r => r.Earned);
                if (lower != null && higher.DateEarned.Value.Date < lower.DateEarned.Value.Date)
                {
                    // Scoutbook is authoritative and dates are sometimes backfilled out of order,
                    // so this is a heads-up rather than an error.
                    issues.Add(new AdvancementIssue(IssueSeverity.Warning, name,
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
            CheckTimeRequirement(issues, name, ladder, nextIndex: 4);
            CheckTimeRequirement(issues, name, ladder, nextIndex: 5);
            CheckTimeRequirement(issues, name, ladder, nextIndex: 6);
        }

        /// <summary>
        /// Warns when the "active for N months" requirement (#1) of the rank at <paramref name="nextIndex"/>
        /// is overdue: the previous rank is earned, the time has passed, and the scout has not yet
        /// reached that rank or any rank above it.
        /// </summary>
        private static void CheckTimeRequirement(List<AdvancementIssue> issues, string scoutName, Rank[] ladder, int nextIndex)
        {
            Rank next = ladder[nextIndex];
            Rank previous = ladder[nextIndex - 1];
            // The month count comes from the rank's own requirement 1 so the two cannot drift apart.
            int? months = next.Requirements.Find(r => r.Name == "1")?.TimeRequirementMonths;
            if (months == null)
            {
                return;
            }
            if (ladder.Skip(nextIndex).Any(r => r.Earned) || !previous.Earned)
            {
                return;
            }
            DateTime due = previous.DateEarned.Value.AddMonths(months.Value);
            if (DateTime.Now >= due && !RequirementEarned(next, "1", whenMissing: true))
            {
                issues.Add(new AdvancementIssue(IssueSeverity.Warning, scoutName,
                    $"{due.ToShortDateString()} is {months} months since earning {previous.Name}, check {next.Name} #1"));
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