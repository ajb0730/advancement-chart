using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using advancementchart.Model;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace advancementchart.Reports
{

    public class AdvancementReport : IReport
    {
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

        private static void WriteLine(Body body, string line, string font = "courier", string fontSize = "22" /* half-points */, bool indent = false)
        {
            Paragraph para = body.AppendChild(new Paragraph());
            if (indent)
            {
                para.ParagraphProperties = new ParagraphProperties();
                para.ParagraphProperties.AppendChild(
                    new Indentation()
                    {
                        Left = new StringValue("720")
                    }
                );
            }
            RunProperties runProperties = new RunProperties();
            runProperties.AppendChild(
                new RunFonts()
                {
                    Ascii = font
                });
            runProperties.AppendChild(
                new FontSize()
                {
                    Val = new StringValue(fontSize)
                }
            );
            Run run = para.AppendChild(new Run());
            run.PrependChild<RunProperties>(runProperties);
            run.AppendChild(new Text(line));
        }

        private static void WritePageBreak(Body body)
        {
            Paragraph para = body.AppendChild(new Paragraph());
            Run run = para.AppendChild(new Run());
            Break pageBreak = run.AppendChild(new Break() { Type = BreakValues.Page });
        }

        // Create a new style with the specified styleid and stylename and add it to the specified
        // style definitions part.
        private static void AddNewStyle(StyleDefinitionsPart styleDefinitionsPart, string styleid, string stylename, string targetFont = "courier", string targetFontSize = "22", bool indent = false)
        {
            // Get access to the root element of the styles part.
            Styles styles = styleDefinitionsPart.Styles;

            // Create a new paragraph style and specify some of the properties.
            Style style = new Style()
            {
                Type = StyleValues.Paragraph,
                StyleId = styleid,
                CustomStyle = true
            };
            StyleName styleName = new StyleName() { Val = stylename };
            BasedOn basedOn = new BasedOn() { Val = "Normal" };
            NextParagraphStyle nextParagraphStyle = new NextParagraphStyle() { Val = "Normal" };
            style.Append(styleName);
            style.Append(basedOn);
            style.Append(nextParagraphStyle);
            if (indent)
            {
                ParagraphProperties paragraphProperties = new ParagraphProperties() { Indentation = new Indentation() { Left = "22" } };
                style.Append(paragraphProperties);
            }

            // Create the StyleRunProperties object and specify some of the run properties.
            StyleRunProperties styleRunProperties = new StyleRunProperties();
            Bold bold = new Bold();
            Color color = new Color() { ThemeColor = ThemeColorValues.Accent2 };
            RunFonts font = new RunFonts() { Ascii = targetFont };
            Italic italic = new Italic();
            // Specify a 12 point size.
            FontSize fontSize = new FontSize() { Val = targetFontSize };
            styleRunProperties.Append(bold);
            styleRunProperties.Append(color);
            styleRunProperties.Append(font);
            styleRunProperties.Append(fontSize);
            styleRunProperties.Append(italic);

            // Add the run properties to the style.
            style.Append(styleRunProperties);

            // Add the style to the styles part.
            styles.Append(style);
        }

        public void Run(string outputFileName)
        {
            Console.WriteLine("Running Advancement Report");
            Console.WriteLine($"There are {this.Scouts.Count} scouts on the list");

            if (File.Exists(outputFileName))
                File.Delete(outputFileName);

            using (WordprocessingDocument wordDocument =
                    WordprocessingDocument.Create(outputFileName, WordprocessingDocumentType.Document))
            {
                // Add a main document part. 
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

                // Create the document structure and add some text.
                mainPart.Document = new Document();
                StyleDefinitionsPart part = mainPart.StyleDefinitionsPart;
                if (null == part)
                {
                    part = mainPart.AddNewPart<StyleDefinitionsPart>();
                    Styles root = new Styles();
                    root.Save(part);
                }

                Body body = mainPart.Document.AppendChild(new Body());

                Paragraph para = body.AppendChild(new Paragraph());

                // TODO: This should be in the header or footer
                WriteLine(body, $"Report Generated: {DateTime.Now.ToShortDateString()}");
                if (ReportDataTimestamp != DateTime.MinValue)
                {
                    WriteLine(body, $"Data as of: {ReportDataTimestamp.ToShortDateString()}");
                }
                WriteLine(body, string.Empty);

                foreach (var scout in this.Scouts)
                {
                    scout.AllocateMeritBadges();

                    WritePageBreak(body);
                    WriteLine(body, scout.DisplayName, "Arial", "36");
                    WriteLine(body, string.Empty);

                    var currentRank = scout.CurrentRankWithPalms;
                    if (currentRank.DateEarned.HasValue)
                    {
                        WriteLine(body, $"Current Rank: {currentRank.Name} (earned {currentRank.DateEarned.Value.ToShortDateString()})");
                    }
                    else
                    {
                        WriteLine(body, $"Current Rank: {currentRank.Name}");
                    }

                    WriteLine(body, string.Empty);
                    var nextRank = scout.NextRank;
                    WriteLine(body, $"Requirements remaining for {nextRank.Name}:");
                    foreach (var requirement in nextRank.Requirements)
                    {
                        if (!requirement.Earned)
                        {
                            WriteLine(body, $"{requirement.Name}. {requirement.Description}");
                            if (requirement.TimeRequirementMonths.HasValue)
                            {
                                var targetDate = currentRank.DateEarned.Value.AddMonths(requirement.TimeRequirementMonths.Value);
                                WriteLine(body: body, line: $"=> {targetDate.ToShortDateString()}{(DateTime.Now > targetDate ? " !!!" : "")}", indent: true);
                            }
                            else if (requirement is EagleMeritBadgeRequirement)
                            {
                                EagleMeritBadgeRequirement mbReq = requirement as EagleMeritBadgeRequirement;
                                WriteLine(body: body, line: $"=> {mbReq.MeritBadges.Where(x => x.Earned).Count()} of {mbReq.Total} earned.", indent: true);
                                List<MeritBadge> required = new List<MeritBadge>();
                                List<MeritBadge> elective = new List<MeritBadge>();
                                foreach (var badge in mbReq.MeritBadges.Where(x => x.Earned).OrderBy(x => x.DateEarned).ThenBy(x => x.BsaId))
                                {
                                    if (badge.EagleRequired)
                                    {
                                        required.Add(badge);
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
                                        continue;
                                    }
                                    skip.Add(badgeName);
                                    bool haveStarted = mbReq.MeritBadges.Any(x => x.Name == badgeName && !x.Earned);
                                    IEnumerable<MeritBadge> others = mbReq.MeritBadges.GetEagleEquivalents(badgeName);
                                    bool isMultiple = MeritBadge.IsMultiple(badgeName);
                                    bool haveEarnedOther = others.Any(x => x.Earned);
                                    bool haveStartedOther = others.Any(x => x.Started);

                                    // Eagle Required earned as an Eagle Required
                                    if (required.Any(x => x.Name == badgeName))
                                    {
                                        WriteLine(body: body, line: $"Earned: {badgeName} *", indent: true);
                                    }
                                    // Single Eagle Required
                                    else if (!isMultiple)
                                    {
                                        // Single Eagle Required Started
                                        if (haveStarted)
                                        {
                                            WriteLine(body: body, line: $"Started: {badgeName} *", indent: true);
                                        }
                                        // Single Eagle Required still needed
                                        else
                                        {
                                            WriteLine(body: body, line: $"Need: {badgeName} *", indent: true);
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
                                                stuffToList = stuffToList.Prepend(badgeName);
                                            }
                                            WriteLine(body: body, line: $"Started: {String.Join(" * and ", stuffToList)} *", indent: true);
                                        }
                                        else
                                        {
                                            var stuffToList = MeritBadge.GetEagleEquivalents(badgeName).Prepend(badgeName);
                                            WriteLine(body: body, line: $"Need: {String.Join(" * or ", stuffToList)} *", indent: true);
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
                                    WriteLine(body: body, line: $"Earned: {badge.Name}{(badge.EagleRequired ? " !" : "")}", indent: true);
                                }
                                if (elective.Count < mbReq.Elective)
                                {
                                    WriteLine(body: body, line: $"Need: {(mbReq.Elective - elective.Count)} elective(s)", indent: true);
                                }
                            }
                            else if (requirement is MeritBadgeRequirement)
                            {
                                MeritBadgeRequirement mbReq = requirement as MeritBadgeRequirement;
                                WriteLine(body: body, line: $"=> {mbReq.MeritBadges.Where(x => x.Earned).Count()} of {mbReq.Total} earned.", indent: true);
                                foreach (var mb in mbReq.MeritBadges.Where(x => x.Earned && x.EagleRequired))
                                {
                                    WriteLine(body: body, line: $"Earned: {mb.Name} *", indent: true);
                                }
                                foreach (var mb in mbReq.MeritBadges.Where(x => x.Earned && !x.EagleRequired))
                                {
                                    WriteLine(body: body, line: $"Earned: {mb.Name}", indent: true);
                                }
                                if (mbReq.MeritBadges.Where(x => x.Earned && x.EagleRequired).Count() < mbReq.Required)
                                {
                                    foreach (var mb in scout.MeritBadges.Where(x => !x.Earned && x.EagleRequired).OrderBy(x => x.BsaId))
                                    {
                                        WriteLine(body: body, line: $"Started: {mb.Name} *", indent: true);
                                    }
                                }
                                if (mbReq.MeritBadges.Where(x => x.Earned && !x.EagleRequired).Count() < mbReq.Elective)
                                {
                                    foreach (var mb in scout.MeritBadges.Where(x => !x.Earned && !x.EagleRequired).OrderBy(x => x.BsaId))
                                    {
                                        WriteLine(body: body, line: $"Started: {mb.Name}", indent: true);
                                    }
                                }

                            }
                        } // if ! requirement.Earned
                        // Check if Tenderfoot 6a is signed off but Tenderfoot 6c is not
                        else if (nextRank.Name == "Tenderfoot" && requirement.Name == "6a")
                        {
                            var matchedReq = nextRank.Requirements.First(x => x.Name == "6c");
                            if (!matchedReq.Earned && DateTime.Now.Subtract(requirement.DateEarned.Value).Days > 45)
                            {
                                WriteLine(body, $"{requirement.Name}. {requirement.Description}");
                                WriteLine(body: body, line: $"=> !!! Requirement 6c has not been earned ({DateTime.Now.Subtract(requirement.DateEarned.Value).Days} days)", indent: true);
                            }
                        }
                    } // foreach requirement
                } // foreach scout
                mainPart.Document.Save();
            } // using wordDocument = 
        } // Run
    } // Class AdvancementReport
} // namespace
