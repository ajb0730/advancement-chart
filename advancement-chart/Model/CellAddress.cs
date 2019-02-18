using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace advancementchart.Model
{
    public class CellAddress
    {
        public const int MIN_COLUMN = 1;
        public const int MAX_COLUMN = 16384;
        public const int MIN_ROW = 1;
        public const int MAX_ROW = 1048576;

        public static void CellNameToIndex(string address, out int row, out int column)
        {
            row = 0;
            Match match = Regex.Match(address, @"\d+$");
            if (match.Success)
                row = Convert.ToInt32(match.Value);
            column = ColumnNameToIndex(address);
        }

        public static string CellIndexToName(int row, int column)
        {
            return $"{ColumnIndexToName(column)}{row}";
        }

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

        public CellAddress()
            : this(c: "A", r: 1)
        { }

        public CellAddress(string c, int r)
        {
            Column = c;
            Row = r;
        }

        public CellAddress(int c, int r)
        {
            ColumnNumber = c;
            Row = r;
        }

        public CellAddress(string address)
        {
            int r = 0;
            int c = 0;
            CellNameToIndex(address, out r, out c);
            Row = r;
            ColumnNumber = c;
        }

        private int column = 0;

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

        public override string ToString()
        {
            return CellIndexToName(Row, ColumnNumber);
        }

        public override bool Equals(object obj)
        {
            if (null == obj) { return false; }
            var other = obj as CellAddress;
            if (null == other) { return false; }
            return this.ToString() == other.ToString();
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public static implicit operator string(CellAddress c)
        {
            return c.ToString();
        }

        public static implicit operator CellAddress(string s)
        {
            return new CellAddress(s);
        }
    }
}
