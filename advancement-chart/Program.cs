using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using advancementchart.Model;
using advancementchart.Reports;
using CsvHelper;
using System.Globalization;
using advancementchart.Model.Ranks;

[assembly: InternalsVisibleTo("advancement-chart.tests")]

namespace advancement_chart
{
    /// <summary>
    /// An input file (Scoutbook export or roster) that cannot be used, e.g. because it is empty
    /// or lacks required columns.
    /// </summary>
    internal class InputFileException : InvalidOperationException
    {
        public InputFileException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// Invalid command-line usage.
    /// </summary>
    internal class UsageException : Exception
    {
        public UsageException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// Parsed command-line options.
    /// </summary>
    internal class Options
    {
        public string PatrolFile { get; set; } = "./scouts.csv";
        public bool PatrolFileSpecified { get; set; }
        public string OutputDir { get; set; } = ".";
        public List<string> InputFiles { get; } = new List<string>();
        public bool ShowHelp { get; set; }
    }

    class Program
    {
        internal const string Usage =
            "USAGE: advancement-chart [--patrol FILE] [--output-dir DIR] [-h|--help] [--] advancement.csv [advancement2.csv ...]\n" +
            "  --patrol FILE      patrol lookup file with DOBs (default: ./scouts.csv)\n" +
            "  --output-dir DIR   directory to write reports to (default: current directory)";

        /// <summary>
        /// Parses command-line arguments. Throws <see cref="UsageException"/> for unknown options,
        /// options missing their value, or no input files.
        /// </summary>
        internal static Options ParseArgs(string[] args)
        {
            var options = new Options();
            bool optionsEnded = false;
            bool patrolSeen = false;
            bool outputDirSeen = false;
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (optionsEnded)
                {
                    options.InputFiles.Add(arg);
                    continue;
                }
                switch (arg)
                {
                    case "--":
                        // Everything after "--" is an input file, even if it starts with "-".
                        optionsEnded = true;
                        break;
                    case "-h":
                    case "--help":
                        options.ShowHelp = true;
                        break;
                    case "--patrol":
                    case "--output-dir":
                        if (i + 1 >= args.Length || args[i + 1].StartsWith("--"))
                        {
                            throw new UsageException($"Option {arg} requires a value");
                        }
                        string value = args[++i];
                        if (arg == "--patrol")
                        {
                            if (patrolSeen)
                            {
                                throw new UsageException("Option --patrol given more than once");
                            }
                            patrolSeen = true;
                            options.PatrolFile = value;
                            options.PatrolFileSpecified = true;
                        }
                        else
                        {
                            if (outputDirSeen)
                            {
                                throw new UsageException("Option --output-dir given more than once");
                            }
                            outputDirSeen = true;
                            options.OutputDir = value;
                        }
                        break;
                    default:
                        if (arg.StartsWith("-"))
                        {
                            throw new UsageException($"Unknown option {arg}");
                        }
                        options.InputFiles.Add(arg);
                        break;
                }
            }
            if (!options.ShowHelp && options.InputFiles.Count == 0)
            {
                throw new UsageException("No advancement input file given");
            }
            return options;
        }

        static int Main(string[] args)
        {
            return Execute(args);
        }

        internal static int Execute(string[] args)
        {
            try
            {
                Run(args);
                return 0;
            }
            catch (InputFileException e)
            {
                Console.Error.WriteLine($"ERROR: {e.Message}");
                return 1;
            }
            catch (UsageException e)
            {
                Console.Error.WriteLine($"ERROR: {e.Message}");
                Console.Error.WriteLine(Usage);
                return 2;
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            {
                // e.g. a report could not be written to the output directory.
                Console.Error.WriteLine($"ERROR: {e.Message}");
                return 1;
            }
        }

        private static void Run(string[] args)
        {
            var options = ParseArgs(args);
            if (options.ShowHelp)
            {
                Console.WriteLine(Usage);
                return;
            }

            // Validate everything named on the command line before reading any data. Naming the same
            // file twice would load (and double-count) its records twice, so keep only the first.
            var inputFiles = new List<string>();
            foreach (string input in options.InputFiles)
            {
                if (inputFiles.Any(f => Path.GetFullPath(f) == Path.GetFullPath(input)))
                {
                    Console.Error.WriteLine($"WARNING: Ignoring duplicate input file '{input}'");
                    continue;
                }
                inputFiles.Add(input);
                if (!File.Exists(input))
                {
                    throw new InputFileException($"Input file not found: {input}");
                }
            }
            if (options.PatrolFileSpecified && !File.Exists(options.PatrolFile))
            {
                throw new InputFileException($"Patrol lookup file not found: {options.PatrolFile}");
            }
            var scouts = new List<TroopMember>();
            DateTime maxDate = DateTime.MinValue;
            foreach (string arg in inputFiles)
            {
                var fileMaxDate = LoadFile(arg, scouts);
                maxDate = fileMaxDate > maxDate ? fileMaxDate : maxDate;
            }
            //
            // Download the "Scout" backup report from Scoutbook and rename
            // the file "scouts.csv"
            //
            var patrolFile = options.PatrolFile;
            if (!File.Exists(patrolFile))
            {
                Console.Error.WriteLine($"WARNING: Patrol lookup file '{patrolFile}' not found.");
                Console.Error.WriteLine("Scouts will not have patrol assignments, nicknames, or dates of birth.");
                Console.Error.WriteLine("The Eagle Report will not show deadlines for scouts without a date of birth.");
            }
            LoadPatrolLookup(patrolFile, scouts);
            try
            {
                Directory.CreateDirectory(options.OutputDir);
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException || e is ArgumentException || e is NotSupportedException)
            {
                throw new InputFileException($"Cannot use output directory '{options.OutputDir}': {e.Message}");
            }
            string Out(string name) => Path.Combine(options.OutputDir, name);
            {
                var report = new TroopReport(scouts);
                report.Run(Out("TroopAdvancementChart.xlsx"));
            }
            {
                var report = new IndividualReport(scouts);
                report.Run(Out("IndividualReport.xlsx"));
            }
            {
                var report = new EagleReport(scouts);
                report.Run(Out("EagleReport.xlsx"));
            }
            {
                var report = new TroopGuideReport(scouts);
                report.Run(Out("TroopGuideReport.xlsx"));
            }
            {
                var report = new TroopCheckList(scouts);
                report.Run(Out("TroopCheckList.xlsx"));
            }
            {
                var report = new AdvancementCheck(scouts);
                report.Run(@"");
            }
            {
                var report = new AdvancementReport(scouts, maxDate);
                report.Run(Out("AdvancementReport.docx"));
            }
        }

        internal static void LoadPatrolLookup(string fileName, List<TroopMember> scouts)
        {
            if (File.Exists(fileName))
            {
                using (TextReader txtReader = new StreamReader(fileName))
                using (var csvReader = new CsvReader(txtReader, CultureInfo.CurrentCulture))
                {
                    ReadHeader(csvReader, fileName);
                    RequireColumns(csvReader, fileName, "BSA Member ID", "Nickname", "Patrol Name", "DOB");

                    int memberIdIndex = csvReader.GetFieldIndex(name: "BSA Member ID");
                    int firstNameIndex = csvReader.GetFieldIndex(name: "First Name", isTryGet: true);
                    int lastNameIndex = csvReader.GetFieldIndex(name: "Last Name", isTryGet: true);
                    int nicknameIndex = csvReader.GetFieldIndex(name: "Nickname");
                    int patrolNameIndex = csvReader.GetFieldIndex(name: "Patrol Name");
                    int dobIndex = csvReader.GetFieldIndex(name: "DOB");

                    while (csvReader.Read())
                    {
                        string id = csvReader.GetField(memberIdIndex);
                        if (string.IsNullOrWhiteSpace(id))
                        {
                            // Skip blank/placeholder roster rows (e.g. an empty line).
                            continue;
                        }
                        TroopMember scout = scouts.FirstOrDefault(tm => tm.BsaMemberId == id);
                        if (scout == null)
                        {
                            // Seed scouts who are on the roster but have no advancement
                            // records, so every registered scout still appears in the reports.
                            string firstName = firstNameIndex >= 0 ? csvReader.GetField(firstNameIndex) : "";
                            string lastName = lastNameIndex >= 0 ? csvReader.GetField(lastNameIndex) : "";
                            scout = new TroopMember(memberId: id, firstName: firstName, middleName: "", lastName: lastName);
                            scouts.Add(scout);
                            Console.WriteLine($"Adding roster-only record for {firstName} {lastName}.");
                        }

                        string nickname = csvReader.GetField(nicknameIndex);
                        if (!string.IsNullOrWhiteSpace(nickname))
                        {
                            scout.NickName = nickname;
                        }
                        string patrol = csvReader.GetField(patrolNameIndex);
                        if (!string.IsNullOrWhiteSpace(patrol))
                        {
                            scout.Patrol = patrol;
                        }
                        string dob = csvReader.GetField(dobIndex);
                        if (!string.IsNullOrWhiteSpace(dob))
                        {
                            DateTime dobDate;
                            if (!DateTime.TryParse(dob, out dobDate))
                            {
                                Console.Error.WriteLine($"WARNING: Unparseable DOB '{dob}' for {scout.DisplayName}, ignoring it");
                            }
                            else
                            {
                                if (dobDate.Year < 1990 || dobDate > DateTime.Now)
                                {
                                    // An implausible DOB would produce a misleading Eagle deadline, so
                                    // leave the DOB unset rather than accept it.
                                    Console.Error.WriteLine($"WARNING: Suspicious DOB '{dob}' for {scout.DisplayName}, ignoring it");
                                }
                                else
                                {
                                    scout.DateOfBirth = dobDate;
                                }
                            }
                        }
                    }
                }
            }
        }

        internal static DateTime LoadFile(string fileName, List<TroopMember> scouts)
        {
            DateTime result = DateTime.MinValue;
            var warnings = new List<string>();

            if (!File.Exists(fileName))
            {
                throw new InputFileException($"Input file not found: {fileName}");
            }
            Console.WriteLine($"Reading data from {fileName}.");
            using (TextReader txtReader = new StreamReader(fileName))
            using (var csvReader = new CsvReader(txtReader, CultureInfo.CurrentCulture))
            {
                ReadHeader(csvReader, fileName);
                RequireColumns(csvReader, fileName, "BSA Member ID", "First Name", "Middle Name", "Last Name",
                    "Advancement Type", "Advancement", "Version", "Date Completed");

                while (csvReader.Read())
                {
                    string id = csvReader.GetField("BSA Member ID");
                    TroopMember scout = scouts.FirstOrDefault(tm => tm.BsaMemberId == id);
                    if (null == scout)
                    {
                        string firstName = csvReader.GetField("First Name");
                        string middleName = csvReader.GetField("Middle Name");
                        string lastName = csvReader.GetField("Last Name");
                        scout = new TroopMember(memberId: id, firstName: firstName, middleName: middleName, lastName: lastName);
                        scouts.Add(scout);
                        Console.WriteLine($"Adding record for {firstName} {lastName}.");
                    }

                    string type = csvReader.GetField("Advancement Type")?.Trim();
                    string subtype = csvReader.GetField("Advancement")?.Trim();
                    string version = csvReader.GetField("Version")?.Trim();
                    // Defensive: normalize a null Advancement value to empty so the '#' parsing
                    // below reports the row as malformed instead of throwing.
                    subtype = subtype ?? string.Empty;
                    // Rows without a parseable completion date carry no advancement signal we
                    // can use (and would otherwise throw); skip them rather than crash the load.
                    if (!csvReader.TryGetField<DateTime>("Date Completed", out DateTime date))
                    {
                        continue;
                    }
                    result = date > result ? date : result;
                    try
                    {
                        if (type == "Rank")
                        {
                            // New exports suffix the rank name with " Rank" (e.g. "Scout Rank");
                            // older exports used the bare name ("Scout"). Cub ranks (Bobcat,
                            // Tiger, ...) resolve to null and are ignored.
                            Rank rank = GetRankByName(scout, StripSuffix(subtype, " Rank"));
                            if (rank != null)
                            {
                                rank.DateEarned = date;
                            }
                        }
                        else if (type == "Merit Badge" || type == "Merit Badges")
                        {
                            // New exports suffix the badge name with " MB" and replace commas in
                            // the name with semicolons (e.g. "Signs; Signals; and Codes MB").
                            var badgeName = StripSuffix(subtype, " MB").Replace("; ", ", ");
                            if (string.IsNullOrWhiteSpace(badgeName))
                            {
                                warnings.Add($"Skipped merit badge row with no badge name: version={version} date={date.ToShortDateString()}");
                                continue;
                            }
                            MeritBadge badge = new MeritBadge(name: badgeName, description: version, earned: date);
                            scout.Add(badge);
                        }
                        else if (type == "Award" || type == "Awards")
                        {
                            switch (subtype)
                            {
                                case "Eagle Palm Pin #1 (Bronze)":
                                case "Eagle Palm Pin #4 (Bronze)":
                                    scout.EaglePalms.Add(new Palm(Palm.PalmType.Bronze, date));
                                    break;
                                case "Eagle Palm Pin #2 (Gold)":
                                case "Eagle Palm Pin #5 (Gold)":
                                    scout.EaglePalms.Add(new Palm(Palm.PalmType.Gold, date));
                                    break;
                                case "Eagle Palm Pin #3 (Silver)":
                                case "Eagle Palm Pin #6 (Silver)":
                                    scout.EaglePalms.Add(new Palm(Palm.PalmType.Silver, date));
                                    break;
                            }
                        }
                        else if (type != null && (type.EndsWith("Rank Requirement") || type.EndsWith("Rank Requirements")))
                        {
                            // New exports move the rank name into the type column and pluralize
                            // the suffix, e.g. "First Class Rank Requirements" with the
                            // requirement number ("1a") in the Advancement column. Legacy exports
                            // used the singular "First Class Rank Requirement". Cub-rank
                            // requirement rows resolve to a null rank and are ignored.
                            var rankName = StripSuffix(StripSuffix(type, "Requirements"), "Requirement");
                            rankName = StripSuffix(rankName, "Rank");
                            Rank rank = GetRankByName(scout, rankName);
                            if (rank != null && !string.IsNullOrWhiteSpace(subtype)
                                && rank.Requirements.Any(req => req.Name == subtype))
                            {
                                rank.Requirements.First(req => req.Name == subtype).DateEarned = date;
                            }
                        }
                        else if (type == "Merit Badge Requirement")
                        {
                            // Legacy format: badge name and requirement number share the
                            // Advancement column, e.g. "Camping #1a".
                            int hashIndex = subtype.IndexOf('#');
                            if (hashIndex <= 0)
                            {
                                warnings.Add($"Skipped malformed merit badge requirement (no badge name before '#'): {subtype}");
                                continue;
                            }
                            var badgeName = subtype.Substring(0, hashIndex).Trim();
                            scout.AddPartial(badgeName, version);
                        }
                        else if (type != null && type.EndsWith("Merit Badge Requirements"))
                        {
                            // New format: the badge name lives in the type column and keeps its
                            // real commas ("Signs, Signals, and Codes Merit Badge Requirements"),
                            // while the requirement number is in the Advancement column. We only
                            // need the badge name to mark the badge as started.
                            var badgeName = StripSuffix(type, "Merit Badge Requirements");
                            scout.AddPartial(badgeName, version);
                        }
                        else if (type == "Award Requirement")
                        {
                            // Legacy palm-requirement format: "Eagle Palm Pin #1 (Bronze) #1".
                            int hashIndex = subtype.LastIndexOf('#');
                            if (hashIndex <= 0)
                            {
                                warnings.Add($"Skipped malformed palm requirement (no palm name before '#'): {subtype}");
                                continue;
                            }
                            var palmName = subtype.Substring(0, hashIndex).Trim();
                            var requirementNumber = subtype.Substring(hashIndex + 1).Trim().TrimEnd('.');
                            // Console.WriteLine($"Found '{palmName}' and '{requirementNumber}' in '{subtype}'");
                            if (!string.IsNullOrWhiteSpace(palmName) && !string.IsNullOrWhiteSpace(requirementNumber))
                            {
                                Palm palm = null;
                                switch (palmName)
                                {
                                    case "Eagle Palm Pin #1 (Bronze)":
                                        palm = scout.GetNthPalm(Palm.PalmType.Bronze, 1);
                                        break;
                                    case "Eagle Palm Pin #2 (Gold)":
                                        palm = scout.GetNthPalm(Palm.PalmType.Gold, 1);
                                        break;
                                    case "Eagle Palm Pin #3 (Silver)":
                                        palm = scout.GetNthPalm(Palm.PalmType.Silver, 1);
                                        break;
                                    case "Eagle Palm Pin #4 (Bronze)":
                                        palm = scout.GetNthPalm(Palm.PalmType.Bronze, 2);
                                        break;
                                    case "Eagle Palm Pin #5 (Gold)":
                                        palm = scout.GetNthPalm(Palm.PalmType.Gold, 2);
                                        break;
                                    case "Eagle Palm Pin #6 (Silver)":
                                        palm = scout.GetNthPalm(Palm.PalmType.Silver, 2);
                                        break;
                                }
                                if (null != palm)
                                {
                                    var req = palm.Requirements.FirstOrDefault(x => x.Name == requirementNumber);
                                    if (req != null)
                                    {
                                        req.DateEarned = date;
                                    }
                                    else
                                    {
                                        warnings.Add($"Unknown requirement '{requirementNumber}' for '{palmName}'");
                                    }
                                }
                            }
                        }
                        // New-format "<Award> Award Requirements" rows carry the requirement as
                        // descriptive text rather than a number, and palms are only tracked once
                        // earned (via "Awards" rows); there is nothing to attach, so those rows
                        // are intentionally ignored.
                    }
                    catch (FormatException e)
                    {
                        warnings.Add($"Skipped row: type={type} subtype={subtype} version={version} date={date.ToShortDateString()} - {e.Message}");
                    }
                    catch (ArgumentException e)
                    {
                        warnings.Add($"Invalid data: type={type} subtype={subtype} version={version} date={date.ToShortDateString()} - {e.Message}");
                    }
                }
            }

            if (warnings.Count > 0)
            {
                Console.Error.WriteLine($"Warning: {warnings.Count} row(s) skipped while loading {fileName}:");
                foreach (var warning in warnings)
                {
                    Console.Error.WriteLine($"  {warning}");
                }
            }

            return result;
        }

        /// <summary>
        /// Reads the header row, throwing <see cref="InputFileException"/> if the file is empty.
        /// </summary>
        internal static void ReadHeader(CsvReader csvReader, string fileName)
        {
            if (!csvReader.Read() || !csvReader.ReadHeader())
            {
                throw new InputFileException($"No header row found in {fileName}");
            }
        }

        /// <summary>
        /// Throws if any of the named columns is missing from the header row just read, so a
        /// changed Scoutbook export fails loudly instead of silently yielding null fields.
        /// </summary>
        internal static void RequireColumns(CsvReader csvReader, string fileName, params string[] columns)
        {
            var missing = columns.Where(c => csvReader.GetFieldIndex(name: c, isTryGet: true) < 0).ToList();
            if (missing.Count > 0)
            {
                throw new InputFileException(
                    $"Required column(s) {string.Join(", ", missing.Select(m => $"'{m}'"))} not found in {fileName}");
            }
        }

        /// <summary>
        /// Removes a trailing token from a value (case-sensitive) and trims trailing
        /// whitespace, returning the value unchanged if the suffix is absent. Used to
        /// normalize the pluralized/suffixed labels in newer Scoutbook exports
        /// (e.g. "Scout Rank" -> "Scout", "Camping MB" -> "Camping").
        /// </summary>
        internal static string StripSuffix(string value, string suffix)
        {
            if (!string.IsNullOrEmpty(value) && value.EndsWith(suffix))
            {
                return value.Substring(0, value.Length - suffix.Length).TrimEnd();
            }
            return value?.Trim();
        }

        /// <summary>
        /// Maps a rank display name to the corresponding <see cref="Rank"/> on the scout.
        /// Returns null for names that are not Scouts BSA ranks (e.g. Cub Scout ranks),
        /// so their rows are ignored.
        /// </summary>
        internal static Rank GetRankByName(TroopMember scout, string rankName)
        {
            switch (rankName)
            {
                case "Scout":
                    return scout.Scout;
                case "Tenderfoot":
                    return scout.Tenderfoot;
                case "Second Class":
                    return scout.SecondClass;
                case "First Class":
                    return scout.FirstClass;
                case "Star Scout":
                    return scout.Star;
                case "Life Scout":
                    return scout.Life;
                case "Eagle Scout":
                    return scout.Eagle;
                default:
                    return null;
            }
        }
    }
}
