using System;
using Xunit;
using advancementchart.Model;

namespace advancement_chart.tests
{
    public class CellAddressTest
    {
        [Theory]
        [InlineData("A", 1)]
        [InlineData("a", 1)]
        [InlineData("B", 2)]
        [InlineData("Z", 26)]
        [InlineData("AA", 27)]
        [InlineData("AB", 28)]
        [InlineData("BA", 53)]
        public void TestColumnNameToIndex(string column, int index)
        {
            Assert.Equal(index, CellAddress.ColumnNameToIndex(column));
        }

        [Theory]
        [InlineData(1,"A")]
        [InlineData(2,"B")]
        [InlineData(26, "Z")]
        [InlineData(27, "AA")]
        [InlineData(28, "AB")]
        [InlineData(53, "BA")]
        public void TestColumnIndexToName(int index, string column)
        {
            Assert.Equal(column, CellAddress.ColumnIndexToName(index));
        }

        [Theory]
        [InlineData("A1", 1, 1)]
        [InlineData("A2", 2, 1)]
        [InlineData("B1", 1, 2)]
        [InlineData("AA1", 1, 27)]
        [InlineData("AB1", 1, 28)]
        [InlineData("AB100", 100, 28)]
        public void TestCellNameToIndex(string address, int row, int col)
        {
            int r, c;
            CellAddress.CellNameToIndex(address, out r, out c);
            Assert.Equal(row, r);
            Assert.Equal(col, c);
        }

        [Theory]
        [InlineData(1, 1, "A1")]
        [InlineData(2, 1, "A2")]
        [InlineData(1, 2, "B1")]
        [InlineData(1, 27, "AA1")]
        [InlineData(1, 28, "AB1")]
        [InlineData(100, 28, "AB100")]
        public void TestCellIndexToName(int row, int col, string address)
        {
            Assert.Equal(address, CellAddress.CellIndexToName(row, col));
        }

        [Theory]
        [InlineData("A1", 1, 1)]
        [InlineData("a1", 1, 1)]
        [InlineData("A2", 2, 1)]
        [InlineData("B1", 1, 2)]
        [InlineData("AA1", 1, 27)]
        [InlineData("AB1", 1, 28)]
        [InlineData("Ab1", 1, 28)]
        [InlineData("ab1", 1, 28)]
        [InlineData("AB100", 100, 28)]
        public void TestStringConstructor(string address, int row, int col)
        {
            CellAddress ca = new CellAddress(address);
            Assert.Equal(row, ca.Row);
            Assert.Equal(col, ca.ColumnNumber);
        }

        [Theory]
        [InlineData(1, 1, "A1")]
        [InlineData(2, 1, "A2")]
        [InlineData(1, 2, "B1")]
        [InlineData(1, 27, "AA1")]
        [InlineData(1, 28, "AB1")]
        [InlineData(100, 28, "AB100")]
        public void TestToString(int row, int col, string address)
        {
            CellAddress ca = new CellAddress(col, row);
            Assert.Equal(address, ca.ToString());
        }

        [Fact]
        public void TestColumnNumberIncrement()
        {
            CellAddress ca = new CellAddress("A1");
            Assert.Equal(1, ca.Row);
            Assert.Equal(1, ca.ColumnNumber);
            Assert.Equal("A", ca.Column);
            ca.ColumnNumber += 1;
            Assert.Equal("B1", ca.ToString());
        }

        [Fact]
        public void TestDefaultConstructor()
        {
            CellAddress ca = new CellAddress();
            Assert.Equal(1, ca.Row);
            Assert.Equal(1, ca.ColumnNumber);
            Assert.Equal("A", ca.Column);
        }

        [Fact]
        public void TestEquals_SameCells()
        {
            CellAddress a = new CellAddress("B3");
            CellAddress b = new CellAddress("B3");
            Assert.True(a.Equals(b));
            Assert.Equal(a, b);
        }

        [Fact]
        public void TestEquals_DifferentCells()
        {
            CellAddress a = new CellAddress("A1");
            CellAddress b = new CellAddress("B2");
            Assert.False(a.Equals(b));
        }

        [Fact]
        public void TestEquals_Null()
        {
            CellAddress a = new CellAddress("A1");
            Assert.False(a.Equals(null));
        }

        [Fact]
        public void TestEquals_NonCellAddress()
        {
            CellAddress a = new CellAddress("A1");
            Assert.False(a.Equals("A1"));
        }

        [Fact]
        public void TestGetHashCode_SameCells()
        {
            CellAddress a = new CellAddress("B3");
            CellAddress b = new CellAddress("B3");
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void TestImplicitStringConversion()
        {
            CellAddress ca = new CellAddress("B3");
            string s = ca;
            Assert.Equal("B3", s);
        }

        [Fact]
        public void TestImplicitCellAddressConversion()
        {
            CellAddress ca = "B3";
            Assert.Equal(3, ca.Row);
            Assert.Equal(2, ca.ColumnNumber);
        }

        [Fact]
        public void TestColumnSetter()
        {
            CellAddress ca = new CellAddress("A1");
            ca.Column = "C";
            Assert.Equal(3, ca.ColumnNumber);
            Assert.Equal("C", ca.Column);
        }

        [Fact]
        public void TestColumnNumberOutOfRange_TooLow()
        {
            CellAddress ca = new CellAddress("A1");
            Assert.Throws<ArgumentOutOfRangeException>(() => ca.ColumnNumber = 0);
        }

        [Fact]
        public void TestColumnNumberOutOfRange_TooHigh()
        {
            CellAddress ca = new CellAddress("A1");
            Assert.Throws<ArgumentOutOfRangeException>(() => ca.ColumnNumber = CellAddress.MAX_COLUMN + 1);
        }

        [Fact]
        public void TestRowOutOfRange_TooLow()
        {
            CellAddress ca = new CellAddress("A1");
            Assert.Throws<ArgumentOutOfRangeException>(() => ca.Row = 0);
        }

        [Fact]
        public void TestRowOutOfRange_TooHigh()
        {
            CellAddress ca = new CellAddress("A1");
            Assert.Throws<ArgumentOutOfRangeException>(() => ca.Row = CellAddress.MAX_ROW + 1);
        }

        [Fact]
        public void TestColumnIndexToName_OutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => CellAddress.ColumnIndexToName(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => CellAddress.ColumnIndexToName(CellAddress.MAX_COLUMN + 1));
        }

        [Fact]
        public void TestStringColumnRowConstructor()
        {
            CellAddress ca = new CellAddress("C", 5);
            Assert.Equal(5, ca.Row);
            Assert.Equal(3, ca.ColumnNumber);
            Assert.Equal("C", ca.Column);
        }
    }
}
