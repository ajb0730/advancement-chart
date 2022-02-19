using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using advancementchart.Model;
using advancementchart.Model.Ranks;

namespace advancementchart.Reports
{

    public class AdvancementReport : IReport
    {
        const string EP = "Emergency Preparedness";
        const string LS = "Lifesaving";
        const string ES = "Environmental Science";
        const string SB = "Sustainability";
        const string SW = "Swimming";
        const string HK = "Hiking";
        const string CY = "Cycling";

        protected readonly DateTime ReportDataTimestamp = DateTime.MinValue;

        public AdvancementReport(List<TroopMember> scouts)
            : this(scouts, DateTime.MinValue)
        { }

        public AdvancementReport(List<TroopMember> scouts, DateTime maxDate)
        {
            Scouts = scouts;
            ReportDataTimestamp = maxDate;
        }

        public List<TroopMember> Scouts { get; set; }

        public void Run(string outputFileName)
        {
            Console.WriteLine("Running Advancement Report");
            Console.WriteLine($"There are {this.Scouts.Count} scouts on the list");

            if (File.Exists(outputFileName))
                File.Delete(outputFileName);

            using (StreamWriter output = File.CreateText(outputFileName))
            {
                output.WriteLine($"Report Generated: {DateTime.Now.ToShortDateString()}");
                if (ReportDataTimestamp != DateTime.MinValue)
                {
                    output.WriteLine($"Data as of: {ReportDataTimestamp.ToShortDateString()}");
                }
                output.WriteLine();

                foreach (var scout in this.Scouts)
                {
                    scout.AllocateMeritBadges();

                    output.WriteLine();
                    output.WriteLine(scout.DisplayName);
                    output.WriteLine();
                    var currentRank = scout.CurrentRankWithPalms;
                    if (currentRank.DateEarned.HasValue)
                    {
                        output.WriteLine($"Current Rank: {currentRank.Name} (earned {currentRank.DateEarned.Value.ToShortDateString()})");
                    }
                    else
                    {
                        output.WriteLine($"Current Rank: {currentRank.Name}");
                    }

                    output.WriteLine();
                    var nextRank = scout.NextRank;
                    output.WriteLine($"Requirements remaining for {nextRank.Name}:");
                    foreach (var requirement in nextRank.Requirements)
                    {
                        if (!requirement.Earned)
                        {
                            output.WriteLine($"{requirement.Name}. {requirement.Description}");
                            if (requirement.TimeRequirementMonths.HasValue)
                            {
                                var targetDate = currentRank.DateEarned.Value.AddMonths(requirement.TimeRequirementMonths.Value);
                                output.WriteLine($"   => {targetDate.ToShortDateString()}{(DateTime.Now > targetDate ? " !!!" : "")}");
                            }
                            else if (requirement is EagleMeritBadgeRequirement)
                            {
                                EagleMeritBadgeRequirement mbReq = requirement as EagleMeritBadgeRequirement;
                                output.WriteLine($"    => {mbReq.MeritBadges.Where(x => x.Earned).Count()} of {mbReq.Total} earned.");
                                List<MeritBadge> required = new List<MeritBadge>();
                                List<MeritBadge> elective = new List<MeritBadge>();
                                foreach (var badge in mbReq.MeritBadges.Where(x => x.Earned).OrderBy(x => x.DateEarned).ThenBy(x => x.BsaId))
                                {
                                    if (badge.EagleRequired)
                                    {
                                        bool addMe = true;

                                        // (a) First Aid
                                        // (b) Citizenship in the Community
                                        // (c) Citizenship in the Nation
                                        // (d) Citizenship in the World
                                        // (e) Communication
                                        // (f) Cooking
                                        // (g) Personal Fitness
                                        // (h) Emergency Preparedness or Lifesaving
                                        if ((badge.Name == EP && required.Any(mb => mb.Name == LS))
                                            || (badge.Name == LS && required.Any(mb => mb.Name == EP)))
                                        {
                                            addMe = false;
                                        }
                                        // (i) Environmental Science or Sustainability
                                        if ((badge.Name == ES && required.Any(mb => mb.Name == SB))
                                            || (badge.Name == SB && required.Any(mb => mb.Name == ES)))
                                        {
                                            addMe = false;
                                        }
                                        // (j) Personal Management
                                        // (k) Swimming or Hiking or Cycling
                                        if ((badge.Name == SW && required.Any(mb => mb.Name == HK || mb.Name == CY))
                                            || (badge.Name == HK && required.Any(mb => mb.Name == SW || mb.Name == CY))
                                            || (badge.Name == CY && required.Any(mb => mb.Name == SW || mb.Name == HK)))
                                        {
                                            addMe = false;
                                        }
                                        // (l) Camping
                                        // (m) Family Life
                                        if (addMe)
                                        {
                                            required.Add(badge);
                                        }
                                        else
                                        {
                                            elective.Add(badge);
                                        }
                                    }
                                    else
                                    {
                                        elective.Add(badge);
                                    }
                                }

                                HashSet<string> skip = new HashSet<string>();
                                foreach (var badgeName in MeritBadge.GetEagleRequired())
                                {
                                    if (skip.Any(x => x == badgeName))
                                    {
                                        // Console.WriteLine($"Skip {badgeName}");
                                        continue;
                                    }
                                    skip.Add(badgeName);
                                    bool haveStarted = mbReq.MeritBadges.Any(x => x.Name == badgeName && !x.Earned);
                                    IEnumerable<MeritBadge> others = mbReq.MeritBadges.GetEagleEquivalents(badgeName);
                                    bool isMultiple = MeritBadge.IsMultiple(badgeName);
                                    // Console.WriteLine($"{badgeName} isMultiple {isMultiple}");
                                    bool haveEarnedOther = others.Any(x => x.Earned);
                                    bool haveStartedOther = others.Any(x => x.Started);

                                    // Eagle Required earned as an Eagle Required
                                    if (required.Any(x => x.Name == badgeName))
                                    {
                                        output.WriteLine($"      Earned: {badgeName} *");
                                    }
                                    // Eagle Required earned as an Elective
                                    else if (elective.Any(x => x.Name == badgeName))
                                    {
                                        output.WriteLine($"      Earned: {badgeName} (as elective)");
                                    }
                                    // Single Eagle Required
                                    else if (!isMultiple)
                                    {
                                        // Single Eagle Required Started
                                        if (haveStarted)
                                        {
                                            output.WriteLine($"      Started: {badgeName} *");
                                        }
                                        // Single Eagle Required still needed
                                        else
                                        {
                                            output.WriteLine($"      Need: {badgeName} *");
                                        }
                                    }
                                    // Multiple Eagle Required; have not earned equivalent
                                    else if (!haveEarnedOther)
                                    {
                                        // have started equivalent Eagle Required
                                        if (haveStarted || haveStartedOther)
                                        {
                                            var stuffToList = others.Where(x => x.Started).Select(x => x.Name);
                                            if (haveStarted)
                                            {
                                                stuffToList.Prepend(badgeName);
                                            }
                                            output.WriteLine($"      Started: {String.Join(" * and ", stuffToList)} *");
                                        }
                                        else
                                        {
                                            var stuffToList = MeritBadge.GetEagleEquivalents(badgeName).Prepend(badgeName);
                                            output.WriteLine($"      Need: {String.Join(" * or ", stuffToList)} *");
                                        }
                                        foreach (var otherName in MeritBadge.GetEagleEquivalents(badgeName))
                                        {
                                            if (!elective.Any(x => x.Name == otherName))
                                            {
                                                skip.Add(otherName);
                                            }
                                        }
                                    }
                                    // Multiple Eagle Required; have earned equivalent; skip
                                }
                                foreach (var badge in elective)
                                {
                                    output.WriteLine($"      Earned: {badge.Name}{(badge.EagleRequired ? " !" : "")}");
                                }
                                if (elective.Count < mbReq.Elective)
                                {
                                    output.WriteLine($"      Need: {(mbReq.Elective - elective.Count)} elective(s)");
                                }
                            }
                            else if (requirement is MeritBadgeRequirement)
                            {
                                MeritBadgeRequirement mbReq = requirement as MeritBadgeRequirement;
                                output.WriteLine($"    => {mbReq.MeritBadges.Where(x => x.Earned).Count()} of {mbReq.Total} earned.");
                                // Console.WriteLine($"[{scout.DisplayName}] {String.Join(", ", mbReq.MeritBadges.Select(x => x.Name))}");
                                foreach (var mb in mbReq.MeritBadges.Where(x => x.Earned && x.EagleRequired))
                                {
                                    output.WriteLine($"      Earned: {mb.Name} *");
                                }
                                foreach (var mb in mbReq.MeritBadges.Where(x => x.Earned && !x.EagleRequired))
                                {
                                    output.WriteLine($"      Earned: {mb.Name}");
                                }
                                foreach (var mb in mbReq.MeritBadges.Where(x => !x.Earned).OrderBy(x => !x.EagleRequired).ThenBy(x => x.BsaId))
                                {
                                    output.WriteLine($"      Started: {mb.Name}{(mb.EagleRequired ? " *" : "")}");
                                }
                            }
                        }
                        else if (nextRank is Palm && requirement is MeritBadgeRequirement)
                        {
                            output.WriteLine($"Badges Earned: {String.Join(", ", (requirement as MeritBadgeRequirement).MeritBadges.Select(x => x.Name))}");
                        }
                    }
                    output.WriteLine();
                }
            }
        }
    }
}
