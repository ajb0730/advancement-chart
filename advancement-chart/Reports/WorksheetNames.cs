using System;
using System.Globalization;
using System.Linq;
using OfficeOpenXml;

namespace advancementchart.Reports
{
    /// <summary>
    /// Helpers for adding worksheets whose names come from roster data. Excel rejects duplicate
    /// names, names longer than 31 characters, and names containing <c>: \ / ? * [ ]</c>.
    /// </summary>
    internal static class WorksheetNames
    {
        internal const int MaxLength = 31;
        private static readonly char[] InvalidChars = { ':', '\\', '/', '?', '*', '[', ']' };

        /// <summary>
        /// Adds a worksheet named after <paramref name="desiredName"/>, sanitized, truncated to
        /// Excel's limit, and made unique within the workbook by appending " (2)", " (3)", ...
        /// </summary>
        internal static ExcelWorksheet AddUnique(ExcelWorkbook workbook, string desiredName)
        {
            return workbook.Worksheets.Add(Unique(workbook, desiredName));
        }

        private static string Unique(ExcelWorkbook workbook, string desiredName)
        {
            string baseName = Sanitize(desiredName);
            string candidate = baseName;
            int n = 2;
            while (workbook.Worksheets.Any(w => string.Equals(w.Name, candidate, StringComparison.OrdinalIgnoreCase)))
            {
                string suffix = $" ({n++})";
                candidate = Truncate(baseName, MaxLength - suffix.Length) + suffix;
            }
            return candidate;
        }

        internal static string Sanitize(string name)
        {
            string cleaned = new string((name ?? string.Empty).Select(c => InvalidChars.Contains(c) ? '-' : c).ToArray()).Trim();
            cleaned = Truncate(cleaned, MaxLength);
            if (cleaned.Length == 0)
            {
                cleaned = "Sheet";
            }
            // Excel reserves "History" for its own use.
            if (string.Equals(cleaned, "History", StringComparison.OrdinalIgnoreCase))
            {
                cleaned += " (1)";
            }
            return cleaned;
        }

        /// <summary>Cuts to at most <paramref name="length"/> characters without leaving a leading/trailing apostrophe or space.</summary>
        private static string Truncate(string value, int length)
        {
            if (value.Length > length)
            {
                value = value.Substring(0, length);
                // Do not cut a surrogate pair in half.
                if (char.IsHighSurrogate(value[value.Length - 1]))
                {
                    value = value.Substring(0, value.Length - 1);
                }
            }
            return value.Trim().Trim('\'').Trim();
        }

        /// <summary>First letter of a last name for abbreviated sheet names, or empty if there is none.</summary>
        internal static string Initial(string lastName)
        {
            return string.IsNullOrWhiteSpace(lastName)
                ? string.Empty
                : StringInfo.GetNextTextElement(lastName.Trim());
        }

        /// <summary>"First L." for a scout, or just "First" when there is no last name.</summary>
        internal static string Abbreviated(string firstName, string lastName)
        {
            string initial = Initial(lastName);
            return initial.Length == 0 ? firstName : $"{firstName} {initial}.";
        }
    }
}
