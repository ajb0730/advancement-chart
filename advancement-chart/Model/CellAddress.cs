using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace advancementchart.Model
{
    /// <summary>
    /// A 1-based spreadsheet cell reference ("B7"). Convertible to and from strings, so it can be used
    /// directly as an EPPlus cell address, and its <see cref="Row"/> and <see cref="Column"/> can be moved
    /// to walk across a sheet.
    /// </summary>
    public class CellAddress
    {
        public const int MIN_COLUMN = 1;
        public const int MAX_COLUMN = 16384;
        public const int MIN_ROW = 1;
        public const int MAX_ROW = 1048576;

        /// <summary>
        /// Splits an address such as "AB12" into its 1-based row (12) and column (28) numbers.
        /// </summary>
        public static void CellNameToIndex(string address, out int row, out int column)
        {
            row = 0;
            Match match = Regex.Match(address, @"\d+$");
            if (match.Success)
                row = Convert.ToInt32(match.Value);
            column = ColumnNameToIndex(address);
        }

        /// <summary>
        /// Builds an address such as "AB12" from 1-based row and column numbers.
        /// </summary>
        public static string CellIndexToName(int row, int column)
        {
            return $"{ColumnIndexToName(column)}{row}";
        }

        /// <summary>
        /// Converts column letters ("A", "AB"; any trailing row digits are ignored) to a 1-based column number.
        /// </summary>
        public static int ColumnNameToIndex(string address)
        {
            address = Regex.Replace(address.ToUpper(), @"\d+$", string.Empty);
            int column = -1;
            int mul = 1;
            foreach (char c in address.ToCharArray().Reverse())
            {
                column += mul * ((int)c - 64);
                mul *= 26;
            }
            column += 1;
            return column;
        }

        /// <summary>
        /// Converts a 1-based column number to its letters (1 is "A", 28 is "AB").
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The column is outside <see cref="MIN_COLUMN"/>..<see cref="MAX_COLUMN"/>.</exception>
        public static string ColumnIndexToName(int column)
        {
            if (column < MIN_COLUMN || column > MAX_COLUMN)
                throw new ArgumentOutOfRangeException(paramName: nameof(column), message: $"{MIN_COLUMN} <= {column} <= {MAX_COLUMN}");
            column -= 1;
            int q = column / 26;
            char c = (char)(column % 26 + (int)'A');
            if (q > 0)
                return ColumnIndexToName(q) + c.ToString();
            else
                return c.ToString();
        }

        /// <summary>
        /// Creates the address A1.
        /// </summary>
        public CellAddress()
            : this(c: "A", r: 1)
        { }

        /// <summary>
        /// Creates an address from column letters and a 1-based row.
        /// </summary>
        public CellAddress(string c, int r)
        {
            Column = c;
            Row = r;
        }

        /// <summary>
        /// Creates an address from 1-based column and row numbers.
        /// </summary>
        public CellAddress(int c, int r)
        {
            ColumnNumber = c;
            Row = r;
        }

        /// <summary>
        /// Parses an address such as "B7".
        /// </summary>
        public CellAddress(string address)
        {
            int r = 0;
            int c = 0;
            CellNameToIndex(address, out r, out c);
            Row = r;
            ColumnNumber = c;
        }

        private int column = 0;

        /// <summary>
        /// The column letters ("A", "AB"). Setting it moves the address to that column.
        /// </summary>
        public string Column
        {
            get
            {
                return ColumnIndexToName(ColumnNumber);
            }
            set
            {
                ColumnNumber = ColumnNameToIndex(value);
            }
        }

        /// <summary>
        /// The 1-based column number. Incrementing it moves one cell to the right.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Set to a value outside the valid column range.</exception>
        public int ColumnNumber
        {
            get
            {
                return column + 1;
            }
            set
            {
                if (value < MIN_COLUMN || value > MAX_COLUMN)
                {
                    throw new ArgumentOutOfRangeException();
                }
                column = value - 1;
            }
        }

        private int row = 0;
        /// <summary>
        /// The 1-based row number. Incrementing it moves one cell down.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Set to a value outside the valid row range.</exception>
        public int Row
        {
            get
            {
                return row + 1;
            }
            set
            {
                if (value < MIN_ROW || value > MAX_ROW)
                {
                    throw new ArgumentOutOfRangeException();
                }
                row = value - 1;
            }
        }

        /// <summary>
        /// The address as text, e.g. "B7".
        /// </summary>
        public override string ToString()
        {
            return CellIndexToName(Row, ColumnNumber);
        }

        /// <summary>
        /// Two addresses are equal when they refer to the same cell.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is null) { return false; }
            var other = obj as CellAddress;
            if (other is null) { return false; }
            return this.ToString() == other.ToString();
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// Converts the address to its text form so it can be passed wherever a cell address string is expected.
        /// </summary>
        public static implicit operator string(CellAddress c)
        {
            return c.ToString();
        }

        /// <summary>
        /// Parses an address string such as "B7".
        /// </summary>
        public static implicit operator CellAddress(string s)
        {
            return new CellAddress(s);
        }
    }
}
