using System;
using System.Data;
using System.Linq;
using DaoUtilsCore.def.utils;
using DaoUtilsCore.utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestsDaoUtilsCore.testHelpers;

namespace TestsDaoUtilsCore.utils
{
    interface IDataTableBuilderTesting : IDataTableBuilderRoot<IDataTableBuilderTesting>
    {

    }

    internal class TableBuilderTesting : TableBuilderBase<IDataTableBuilderTesting>, IDataTableBuilderTesting
    {
        public TableBuilderTesting(string tableName) : base(tableName)
        {
        }
    }

    [TestClass]
    public class TableBuilderBaseTests
    {
        private TableBuilderTesting _uut;

        private const string TableName = "A Table";

        [TestInitialize]
        public void SetUp()
        {
            _uut = new TableBuilderTesting(TableName);
        }

        private class ExpectedColumnDetails
        {
            public Type ColumnType { get; set; }
            public string Name { get; set; }
            public object[] Values { get; set; }
        }

        private ExpectedColumnDetails ExpectedColumn(string columnName, Type type, params object[] values)
        {
            return new ExpectedColumnDetails { ColumnType = type, Name = columnName, Values = values ?? new object[0] };
        }


        private void CheckTableCreated(DataTable table, string tableName, params ExpectedColumnDetails[] cols)
        {
            Assert.IsNotNull(table);
            Assert.AreEqual(tableName, table.TableName);
            CheckArrays.CheckSameValues("Check Column Details",
                table.Columns.Cast<DataColumn>().Select(c => $"{c.ColumnName} - {c.DataType?.FullName}"),
                cols.Select(c => $"{c.Name} - {c.ColumnType?.FullName}"));
            for (var idx = 0; idx < cols.Length; idx++)
            {
                var idx1 = idx;
                var colValues = table.Rows.Cast<DataRow>().Select(r => r[idx1]).ToArray();
                CollectionAssert.AreEqual(cols[idx].Values, colValues, $"col{cols[idx].Name}");
            }
        }

        private void CheckTableCreated(string tableName, params ExpectedColumnDetails[] cols)
        {
            CheckTableCreated(_uut.BuildTable(), tableName, cols);
        }

        private void CheckTableCreated(params ExpectedColumnDetails[] cols)
        {
            CheckTableCreated(TableName, cols);
        }

        private void CheckSingleColumnTableCreated<T>(string columnName, params T[] values)
        {
            CheckTableCreated(ExpectedColumn(columnName, typeof(T), values.Cast<object>().ToArray()));
        }

        private void CheckSingleColumnTableCreated(string columnName, Type type)
        {
            CheckTableCreated(ExpectedColumn(columnName, type));
        }


        [TestMethod]
        public void BooleanColumn()
        {
            var values = new bool[] { true, false,false, false, false };
            const string columnName = "Boolean.Column";
            var column = _uut.BooleanColumn(columnName);
            CheckSingleColumnTableCreated(columnName, typeof(bool));
            column.Values = values;
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new bool[] { false, false, false, true };
            column.Values = values2;
            CheckSingleColumnTableCreated(columnName, values2);
        }

        [TestMethod]
        public void ByteColumn()
        {
            var values = new byte[] { 38, 215, 179, 41, 62 };
            const string columnName = "Byte.Column";
            var column = _uut.ByteColumn(columnName);
            CheckSingleColumnTableCreated(columnName, typeof(byte));
            column.Values = values;
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new byte[] { 90, 41, 236, 243 };
            column.Values = values2;
            CheckSingleColumnTableCreated(columnName, values2);
        }

        [TestMethod]
        public void CharColumn()
        {
            var values = new char[] { 'E', 'L', 'N', 'D', 'h' };
            const string columnName = "Char.Column";
            var column = _uut.CharColumn(columnName);
            CheckSingleColumnTableCreated(columnName, typeof(char));
            column.Values = values;
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new char[] { 'V', 'c', 'm', 'j' };
            column.Values = values2;
            CheckSingleColumnTableCreated(columnName, values2);
        }

        [TestMethod]
        public void DateTimeColumn()
        {
            var values = new DateTime[] { new DateTime(1991, 8, 6), new DateTime(2000, 7, 13), new DateTime(2005, 6, 2), new DateTime(2004, 11, 27), new DateTime(2013, 5, 27) };
            const string columnName = "DateTime.Column";
            var column = _uut.DateTimeColumn(columnName);
            CheckSingleColumnTableCreated(columnName, typeof(DateTime));
            column.Values = values;
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new DateTime[] { new DateTime(2005, 2, 26), new DateTime(2001, 10, 9), new DateTime(2010, 5, 15), new DateTime(1995, 2, 17) };
            column.Values = values2;
            CheckSingleColumnTableCreated(columnName, values2);
        }

        [TestMethod]
        public void DecimalColumn()
        {
            var values = new decimal[] { 988, 329, 224, 628, 511 };
            const string columnName = "Decimal.Column";
            var column = _uut.DecimalColumn(columnName);
            CheckSingleColumnTableCreated(columnName, typeof(decimal));
            column.Values = values;
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new decimal[] { 224, 592, 438, 633 };
            column.Values = values2;
            CheckSingleColumnTableCreated(columnName, values2);
        }

        [TestMethod]
        public void DoubleColumn()
        {
            var values = new double[] { 728.74, 213.23, 552.74, 282.57, 0.96 };
            const string columnName = "Double.Column";
            var column = _uut.DoubleColumn(columnName);
            CheckSingleColumnTableCreated(columnName, typeof(double));
            column.Values = values;
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new double[] { 393.97, 450.69, 650.57, 236.92 };
            column.Values = values2;
            CheckSingleColumnTableCreated(columnName, values2);
        }

        [TestMethod]
        public void GuidColumn()
        {
            var values = new Guid[] { new Guid("a69c80eb-5303-4588-96df-df874f284df4"), new Guid("1bbd9716-ab1a-4609-81b8-e6681f93ad19"), new Guid("e7468c5a-1179-4118-a4af-946746b3e8e0"), new Guid("8504feaa-3b87-4639-aa99-2c50313b6f6d"), new Guid("8a7c70f6-5683-403e-9e7c-76e63e05284a") };
            const string columnName = "Guid.Column";
            var column = _uut.GuidColumn(columnName);
            CheckSingleColumnTableCreated(columnName, typeof(Guid));
            column.Values = values;
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new Guid[] { new Guid("19d68fa8-5346-4c3e-8e48-aa212b411ab0"), new Guid("2f499217-7a20-4b7a-b93a-fb89348c256d"), new Guid("9befc02a-ef6e-4d92-b438-b05e6d90e71e"), new Guid("8037d112-99e7-447f-90fd-3901131ded3d") };
            column.Values = values2;
            CheckSingleColumnTableCreated(columnName, values2);
        }

        [TestMethod]
        public void Int16Column()
        {
            var values = new short[] { 438, 221, 256, 648, 190 };
            const string columnName = "Int16.Column";
            var column = _uut.Int16Column(columnName);
            CheckSingleColumnTableCreated(columnName, typeof(short));
            column.Values = values;
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new short[] { 469, 846, 620, 167 };
            column.Values = values2;
            CheckSingleColumnTableCreated(columnName, values2);
        }

        [TestMethod]
        public void Int32Column()
        {
            var values = new int[] { -845, -960, -414, -766, -355 };
            const string columnName = "Int32.Column";
            var column = _uut.Int32Column(columnName);
            CheckSingleColumnTableCreated(columnName, typeof(int));
            column.Values = values;
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new int[] { -10, -355, -909, -351 };
            column.Values = values2;
            CheckSingleColumnTableCreated(columnName, values2);
        }

        [TestMethod]
        public void Int64Column()
        {
            var values = new long[] { -745, -216, -322, -580, -541 };
            const string columnName = "Int64.Column";
            var column = _uut.Int64Column(columnName);
            CheckSingleColumnTableCreated(columnName, typeof(long));
            column.Values = values;
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new long[] { -834, -537, -252, -767 };
            column.Values = values2;
            CheckSingleColumnTableCreated(columnName, values2);
        }

        [TestMethod]
        public void SByteColumn()
        {
            var values = new sbyte[] { -81, -67, -80, -72, -6 };
            const string columnName = "SByte.Column";
            var column = _uut.SByteColumn(columnName);
            CheckSingleColumnTableCreated(columnName, typeof(sbyte));
            column.Values = values;
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new sbyte[] { -34, -38, -19, -27 };
            column.Values = values2;
            CheckSingleColumnTableCreated(columnName, values2);
        }

        [TestMethod]
        public void SingleColumn()
        {
            var values = new float[] { 377.7f, 552.74f, 388.93f, 579.43f, 614.58f };
            const string columnName = "Single.Column";
            var column = _uut.SingleColumn(columnName);
            CheckSingleColumnTableCreated(columnName, typeof(float));
            column.Values = values;
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new float[] { 198.3f, 963.9f, 784.29f, 828.1f };
            column.Values = values2;
            CheckSingleColumnTableCreated(columnName, values2);
        }

        [TestMethod]
        public void StringColumn()
        {
            var values = new string[] { "A String 152", "A String 218", "A String 173", "A String 107", "A String 28" };
            const string columnName = "String.Column";
            var column = _uut.StringColumn(columnName);
            CheckSingleColumnTableCreated(columnName, typeof(string));
            column.Values = values;
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new string[] { "A String 239", "A String 178", "A String 50", "A String 20" };
            column.Values = values2;
            CheckSingleColumnTableCreated(columnName, values2);
        }

        [TestMethod]
        public void TimeSpanColumn()
        {
            var values = new TimeSpan[] { new TimeSpan(53699), new TimeSpan(27891), new TimeSpan(90782), new TimeSpan(14860), new TimeSpan(47021) };
            const string columnName = "TimeSpan.Column";
            var column = _uut.TimeSpanColumn(columnName);
            CheckSingleColumnTableCreated(columnName, typeof(TimeSpan));
            column.Values = values;
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new TimeSpan[] { new TimeSpan(20999), new TimeSpan(79577), new TimeSpan(97835), new TimeSpan(83158) };
            column.Values = values2;
            CheckSingleColumnTableCreated(columnName, values2);
        }

        [TestMethod]
        public void UInt16Column()
        {
            var values = new ushort[] { 798, 0, 828, 390, 0 };
            const string columnName = "UInt16.Column";
            var column = _uut.UInt16Column(columnName);
            CheckSingleColumnTableCreated(columnName, typeof(ushort));
            column.Values = values;
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new ushort[] { 105, 220, 951, 53 };
            column.Values = values2;
            CheckSingleColumnTableCreated(columnName, values2);
        }

        [TestMethod]
        public void UInt32Column()
        {
            var values = new uint[] { 729, 683, 443, 851, 275 };
            const string columnName = "UInt32.Column";
            var column = _uut.UInt32Column(columnName);
            CheckSingleColumnTableCreated(columnName, typeof(uint));
            column.Values = values;
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new uint[] { 224, 254, 702, 295 };
            column.Values = values2;
            CheckSingleColumnTableCreated(columnName, values2);
        }

        [TestMethod]
        public void UInt64Column()
        {
            var values = new ulong[] { 536, 955, 968, 614, 42 };
            const string columnName = "UInt64.Column";
            var column = _uut.UInt64Column(columnName);
            CheckSingleColumnTableCreated(columnName, typeof(ulong));
            column.Values = values;
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new ulong[] { 189, 850, 47, 165 };
            column.Values = values2;
            CheckSingleColumnTableCreated(columnName, values2);
        }

        [TestMethod]
        public void BinaryColumn()
        {
            var values = new byte[][] { new byte[] { 180, 113, 187, 213, 43 }, new byte[] { 29, 243, 127, 211, 48 }, new byte[] { 53, 218, 146, 51, 40 }, new byte[] { 225, 247, 131, 162, 173 }, new byte[] { 254, 173, 225, 194, 231 } };
            const string columnName = "Binary.Column";
            var column = _uut.BinaryColumn(columnName);
            CheckSingleColumnTableCreated(columnName, typeof(byte[]));
            column.Values = values;
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new byte[][] { new byte[] { 65, 5, 219, 46, 244 }, new byte[] { 162, 137, 243, 239, 130 }, new byte[] { 128, 81, 227, 130, 83 }, new byte[] { 228, 197, 101, 16, 125 } };
            column.Values = values2;
            CheckSingleColumnTableCreated(columnName, values2);
        }

        [TestMethod]
        public void ColumnForArrayOfBoolean()
        {
            var values = new bool[] { true, false, false, false, false };
            const string columnName = "bool.Column";
            Assert.AreSame(_uut, _uut.Column(columnName, values));
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new bool[] { false, false, false,false, true };
            Assert.AreSame(_uut, _uut.Column($"{columnName}.2", values2));
            CheckSingleColumnTableCreated($"{columnName}.2", values2);
        }

        [TestMethod]
        public void ColumnForArrayOfByte()
        {
            var values = new byte[] { 123, 187, 189, 211, 237 };
            const string columnName = "byte.Column";
            Assert.AreSame(_uut, _uut.Column(columnName, values));
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new byte[] { 218, 78, 196, 50, 200 };
            Assert.AreSame(_uut, _uut.Column($"{columnName}.2", values2));
            CheckSingleColumnTableCreated($"{columnName}.2", values2);
        }

        [TestMethod]
        public void ColumnForArrayOfChar()
        {
            var values = new char[] { '#', 'p', '/', '3', '2' };
            const string columnName = "char.Column";
            Assert.AreSame(_uut, _uut.Column(columnName, values));
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new char[] { 'G', 'r', '|', 'a', 'P' };
            Assert.AreSame(_uut, _uut.Column($"{columnName}.2", values2));
            CheckSingleColumnTableCreated($"{columnName}.2", values2);
        }

        [TestMethod]
        public void ColumnForArrayOfDateTime()
        {
            var values = new DateTime[] { new DateTime(2006, 5, 17), new DateTime(2013, 8, 6), new DateTime(1999, 1, 2), new DateTime(1997, 1, 3), new DateTime(2011, 4, 5) };
            const string columnName = "DateTime.Column";
            Assert.AreSame(_uut, _uut.Column(columnName, values));
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new DateTime[] { new DateTime(2013, 6, 4), new DateTime(1994, 6, 12), new DateTime(1997, 2, 2), new DateTime(2007, 9, 4), new DateTime(1992, 4, 24) };
            Assert.AreSame(_uut, _uut.Column($"{columnName}.2", values2));
            CheckSingleColumnTableCreated($"{columnName}.2", values2);
        }

        [TestMethod]
        public void ColumnForArrayOfDecimal()
        {
            var values = new decimal[] { 406, 317, 264, 775, 981 };
            const string columnName = "decimal.Column";
            Assert.AreSame(_uut, _uut.Column(columnName, values));
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new decimal[] { 117, 126, 820, 152, 742 };
            Assert.AreSame(_uut, _uut.Column($"{columnName}.2", values2));
            CheckSingleColumnTableCreated($"{columnName}.2", values2);
        }

        [TestMethod]
        public void ColumnForArrayOfDouble()
        {
            var values = new double[] { 493.25, 731.88, 704.96, 984.86, 14.24 };
            const string columnName = "double.Column";
            Assert.AreSame(_uut, _uut.Column(columnName, values));
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new double[] { 875.71, 812.20, 77.36, 212.53, 215.93 };
            Assert.AreSame(_uut, _uut.Column($"{columnName}.2", values2));
            CheckSingleColumnTableCreated($"{columnName}.2", values2);
        }

        [TestMethod]
        public void ColumnForArrayOfGuid()
        {
            var values = new Guid[] { new Guid("72301c9d-e4af-4621-97a2-44ac9ab7c7d6"), new Guid("5ea77035-a8f8-4356-89c0-651879b4ba91"), new Guid("9214aa16-ddc0-4f7d-ab4e-1b223d66e35f"), new Guid("daadc323-574b-4728-87cf-10ab703fa96b"), new Guid("97133e2a-be1f-4fe8-af02-b266c9f8d39e") };
            const string columnName = "Guid.Column";
            Assert.AreSame(_uut, _uut.Column(columnName, values));
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new Guid[] { new Guid("385d000e-1db6-4650-b8e3-c529cf303ad6"), new Guid("3c37aab8-011e-40ab-bed0-f374b34e7372"), new Guid("a774c19a-535a-40fc-b9ac-36d106716032"), new Guid("13cc8a6b-4b85-494d-8867-4b761f937a46"), new Guid("bd15d118-d63f-4fb9-870a-7d3652e50b94") };
            Assert.AreSame(_uut, _uut.Column($"{columnName}.2", values2));
            CheckSingleColumnTableCreated($"{columnName}.2", values2);
        }

        [TestMethod]
        public void ColumnForArrayOfInt16()
        {
            var values = new short[] { 68, 278, 768, 177, 908 };
            const string columnName = "short.Column";
            Assert.AreSame(_uut, _uut.Column(columnName, values));
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new short[] { 765, 841, 80, 310, 67 };
            Assert.AreSame(_uut, _uut.Column($"{columnName}.2", values2));
            CheckSingleColumnTableCreated($"{columnName}.2", values2);
        }

        [TestMethod]
        public void ColumnForArrayOfInt32()
        {
            var values = new int[] { -814, -710, -993, -421, -993 };
            const string columnName = "int.Column";
            Assert.AreSame(_uut, _uut.Column(columnName, values));
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new int[] { -169, -551, -271, -368, -745 };
            Assert.AreSame(_uut, _uut.Column($"{columnName}.2", values2));
            CheckSingleColumnTableCreated($"{columnName}.2", values2);
        }

        [TestMethod]
        public void ColumnForArrayOfInt64()
        {
            var values = new long[] { -751, -253, -69, -39, -987 };
            const string columnName = "long.Column";
            Assert.AreSame(_uut, _uut.Column(columnName, values));
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new long[] { -687, -505, -60, -697, -615 };
            Assert.AreSame(_uut, _uut.Column($"{columnName}.2", values2));
            CheckSingleColumnTableCreated($"{columnName}.2", values2);
        }

        [TestMethod]
        public void ColumnForArrayOfSByte()
        {
            var values = new sbyte[] { -113, -74, -76, -26, -84 };
            const string columnName = "sbyte.Column";
            Assert.AreSame(_uut, _uut.Column(columnName, values));
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new sbyte[] { -27, -61, -69, -122, -118 };
            Assert.AreSame(_uut, _uut.Column($"{columnName}.2", values2));
            CheckSingleColumnTableCreated($"{columnName}.2", values2);
        }

        [TestMethod]
        public void ColumnForArrayOfSingle()
        {
            var values = new float[] { 120.90f, 556.94f, 430.16f, 725.39f, 209.90f };
            const string columnName = "float.Column";
            Assert.AreSame(_uut, _uut.Column(columnName, values));
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new float[] { 814.94f, 166.47f, 187.81f, 208.72f, 190.22f };
            Assert.AreSame(_uut, _uut.Column($"{columnName}.2", values2));
            CheckSingleColumnTableCreated($"{columnName}.2", values2);
        }

        [TestMethod]
        public void ColumnForArrayOfString()
        {
            var values = new string[] { "A String 66", "A String 199", "A String 97", "A String 177", "A String 41" };
            const string columnName = "string.Column";
            Assert.AreSame(_uut, _uut.Column(columnName, values));
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new string[] { "A String 57", "A String 26", "A String 199", "A String 190", "A String 198" };
            Assert.AreSame(_uut, _uut.Column($"{columnName}.2", values2));
            CheckSingleColumnTableCreated($"{columnName}.2", values2);
        }

        [TestMethod]
        public void ColumnForArrayOfTimeSpan()
        {
            var values = new TimeSpan[] { new TimeSpan(68613), new TimeSpan(99832), new TimeSpan(30594), new TimeSpan(43572), new TimeSpan(62883) };
            const string columnName = "TimeSpan.Column";
            Assert.AreSame(_uut, _uut.Column(columnName, values));
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new TimeSpan[] { new TimeSpan(84890), new TimeSpan(69982), new TimeSpan(13138), new TimeSpan(61290), new TimeSpan(82408) };
            Assert.AreSame(_uut, _uut.Column($"{columnName}.2", values2));
            CheckSingleColumnTableCreated($"{columnName}.2", values2);
        }

        [TestMethod]
        public void ColumnForArrayOfUInt16()
        {
            var values = new ushort[] { 961, 114, 850, 789, 801 };
            const string columnName = "ushort.Column";
            Assert.AreSame(_uut, _uut.Column(columnName, values));
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new ushort[] { 963, 424, 135, 25, 859 };
            Assert.AreSame(_uut, _uut.Column($"{columnName}.2", values2));
            CheckSingleColumnTableCreated($"{columnName}.2", values2);
        }

        [TestMethod]
        public void ColumnForArrayOfUInt32()
        {
            var values = new uint[] { 7, 757, 363, 751, 678 };
            const string columnName = "uint.Column";
            Assert.AreSame(_uut, _uut.Column(columnName, values));
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new uint[] { 338, 521, 862, 784, 206 };
            Assert.AreSame(_uut, _uut.Column($"{columnName}.2", values2));
            CheckSingleColumnTableCreated($"{columnName}.2", values2);
        }

        [TestMethod]
        public void ColumnForArrayOfUInt64()
        {
            var values = new ulong[] { 59, 945, 643, 431, 224 };
            const string columnName = "ulong.Column";
            Assert.AreSame(_uut, _uut.Column(columnName, values));
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new ulong[] { 815, 642, 736, 845, 340 };
            Assert.AreSame(_uut, _uut.Column($"{columnName}.2", values2));
            CheckSingleColumnTableCreated($"{columnName}.2", values2);
        }

        [TestMethod]
        public void ColumnForArrayOfBinary()
        {
            var values = new byte[][] { new byte[] { 30, 21, 30, 93, 66 }, new byte[] { 37, 237, 151, 227, 50 }, new byte[] { 203, 247, 192, 226, 196 }, new byte[] { 236, 161, 139, 191, 72 }, new byte[] { 83, 212, 86, 102, 194 } };
            const string columnName = "byte[].Column";
            Assert.AreSame(_uut, _uut.Column(columnName, values));
            CheckSingleColumnTableCreated(columnName, values);
            var values2 = new byte[][] { new byte[] { 4, 120, 106, 144, 251 }, new byte[] { 81, 175, 74, 174, 189 }, new byte[] { 235, 162, 253, 125, 135 }, new byte[] { 104, 236, 247, 149, 103 }, new byte[] { 22, 48, 192, 168, 76 } };
            Assert.AreSame(_uut, _uut.Column($"{columnName}.2", values2));
            CheckSingleColumnTableCreated($"{columnName}.2", values2);
        }

        [TestMethod]
        public void MultipleColumnsMultipleTables()
        {
            var columnBoolean = _uut.BooleanColumn("Boolean.Column");
            var columnByte = _uut.ByteColumn("Byte.Column");
            var columnChar = _uut.CharColumn("Char.Column");
            var columnDateTime = _uut.DateTimeColumn("DateTime.Column");
            var columnDecimal = _uut.DecimalColumn("Decimal.Column");
            var columnDouble = _uut.DoubleColumn("Double.Column");
            var columnGuid = _uut.GuidColumn("Guid.Column");
            var columnInt16 = _uut.Int16Column("Int16.Column");
            var columnInt32 = _uut.Int32Column("Int32.Column");
            var columnInt64 = _uut.Int64Column("Int64.Column");
            var columnSByte = _uut.SByteColumn("SByte.Column");
            var columnSingle = _uut.SingleColumn("Single.Column");
            var columnString = _uut.StringColumn("String.Column");
            var columnTimeSpan = _uut.TimeSpanColumn("TimeSpan.Column");
            var columnUInt16 = _uut.UInt16Column("UInt16.Column");
            var columnUInt32 = _uut.UInt32Column("UInt32.Column");
            var columnUInt64 = _uut.UInt64Column("UInt64.Column");
            var columnBinary = _uut.BinaryColumn("Binary.Column");

            var valuesBoolean = new bool[] { true, false,false, false,true };
            var valuesByte = new byte[] { 250, 17, 100, 223, 143 };
            var valuesChar = new char[] { ')', '@', 'b', 'w', 'g' };
            var valuesDateTime = new DateTime[] { new DateTime(1995, 11, 1), new DateTime(2001, 8, 19), new DateTime(1990, 1, 18), new DateTime(2007, 1, 5), new DateTime(1998, 3, 15) };
            var valuesDecimal = new decimal[] { 965, 325, 10, 701, 654 };
            var valuesDouble = new double[] { 631.34, 566.67, 625.22, 924.0, 873.61 };
            var valuesGuid = new Guid[] { new Guid("2ce31c09-95be-4c6a-acdf-0d36d56cb1b7"), new Guid("737fb019-3409-4bf6-b84e-1feeecb01fec"), new Guid("cbe755e1-ef30-4b53-90c5-4fa448c7c8db"), new Guid("b47cda84-1f60-4506-9fbd-1aa4107c37ce"), new Guid("aecd4638-2abc-4cc7-a9be-ddb9cbc74490") };
            var valuesInt16 = new short[] { 188, 212, 172, 317, 527 };
            var valuesInt32 = new int[] { -78, -754, -327, -190, -461 };
            var valuesInt64 = new long[] { -831, -654, -796, -871, -672 };
            var valuesSByte = new sbyte[] { -100, -92, -22, -41, -75 };
            var valuesSingle = new float[] { 689.26f, 966.26f, 92.89f, 403.34f, 831.41f };
            var valuesString = new string[] { "A String 197", "A String 4", "A String 42", "A String 5", "A String 127" };
            var valuesTimeSpan = new TimeSpan[] { new TimeSpan(82665), new TimeSpan(1818), new TimeSpan(2259), new TimeSpan(69173), new TimeSpan(47121) };
            var valuesUInt16 = new ushort[] { 211, 997, 820, 239, 822 };
            var valuesUInt32 = new uint[] { 977, 548, 694, 2, 824 };
            var valuesUInt64 = new ulong[] { 492, 747, 675, 276, 922 };
            var valuesBinary = new byte[][] { new byte[] { 235, 62, 231, 57, 162 }, new byte[] { 172, 103, 126, 198, 174 }, new byte[] { 207, 124, 197, 95, 216 }, new byte[] { 201, 184, 124, 220, 98 }, new byte[] { 176, 113, 185, 112, 29 } };

            columnBoolean.Values = valuesBoolean;
            columnByte.Values = valuesByte;
            columnChar.Values = valuesChar;
            columnDateTime.Values = valuesDateTime;
            columnDecimal.Values = valuesDecimal;
            columnDouble.Values = valuesDouble;
            columnGuid.Values = valuesGuid;
            columnInt16.Values = valuesInt16;
            columnInt32.Values = valuesInt32;
            columnInt64.Values = valuesInt64;
            columnSByte.Values = valuesSByte;
            columnSingle.Values = valuesSingle;
            columnString.Values = valuesString;
            columnTimeSpan.Values = valuesTimeSpan;
            columnUInt16.Values = valuesUInt16;
            columnUInt32.Values = valuesUInt32;
            columnUInt64.Values = valuesUInt64;
            columnBinary.Values = valuesBinary;

            var table1 = _uut.BuildTable();

            var valuesBoolean2 = new bool[] { true, false,false, false,false };
            var valuesByte2 = new byte[] { 28, 148, 70, 183, 4 };
            var valuesChar2 = new char[] { '^', '^', '=', '<', 'A' };
            var valuesDateTime2 = new DateTime[] { new DateTime(1993, 5, 17), new DateTime(2003, 5, 9), new DateTime(1996, 7, 16), new DateTime(2012, 8, 4), new DateTime(2005, 4, 1) };
            var valuesDecimal2 = new decimal[] { 302, 886, 648, 369, 455 };
            var valuesDouble2 = new double[] { 768.76, 21.72, 919.41, 842.25, 651.32 };
            var valuesGuid2 = new Guid[] { new Guid("48643a60-aaa6-4f75-92f7-23fd3618a93b"), new Guid("163f6d52-ab3b-4db9-b518-b68a734148a8"), new Guid("cab70b16-c778-4741-8885-0c154f6f4c3f"), new Guid("8066c3d8-bd98-4ac9-a4ce-f2d31f02b6fa"), new Guid("f5be7688-77e0-458c-a84e-43fdffb7ac90") };
            var valuesInt162 = new short[] { 203, 155, 868, 29, 974 };
            var valuesInt322 = new int[] { -682, -708, -628, -295, -300 };
            var valuesInt642 = new long[] { -373, -389, -522, -22, -332 };
            var valuesSByte2 = new sbyte[] { -123, -121, -123, -87, -89 };
            var valuesSingle2 = new float[] { 736.97f, 877.77f, 522.36f, 375.90f, 614.2f };
            var valuesString2 = new string[] { "A String 120", "A String 252", "A String 242", "A String 62", "A String 182" };
            var valuesTimeSpan2 = new TimeSpan[] { new TimeSpan(56210), new TimeSpan(27055), new TimeSpan(59425), new TimeSpan(33151), new TimeSpan(35276) };
            var valuesUInt162 = new ushort[] { 594, 257, 353, 69, 81 };
            var valuesUInt322 = new uint[] { 379, 243, 998, 389, 941 };
            var valuesUInt642 = new ulong[] { 450, 865, 566, 936, 586 };
            var valuesBinary2 = new byte[][] { new byte[] { 57, 70, 23, 129, 155 }, new byte[] { 78, 205, 3, 70, 210 }, new byte[] { 97, 111, 70, 78, 197 }, new byte[] { 182, 94, 166, 88, 30 }, new byte[] { 122, 158, 206, 177, 36 } };

            columnBoolean.Values = valuesBoolean2;
            columnByte.Values = valuesByte2;
            columnChar.Values = valuesChar2;
            columnDateTime.Values = valuesDateTime2;
            columnDecimal.Values = valuesDecimal2;
            columnDouble.Values = valuesDouble2;
            columnGuid.Values = valuesGuid2;
            columnInt16.Values = valuesInt162;
            columnInt32.Values = valuesInt322;
            columnInt64.Values = valuesInt642;
            columnSByte.Values = valuesSByte2;
            columnSingle.Values = valuesSingle2;
            columnString.Values = valuesString2;
            columnTimeSpan.Values = valuesTimeSpan2;
            columnUInt16.Values = valuesUInt162;
            columnUInt32.Values = valuesUInt322;
            columnUInt64.Values = valuesUInt642;
            columnBinary.Values = valuesBinary2;

            var table2 = _uut.BuildTable();


            CheckTableCreated(table1, TableName
                , ExpectedColumn("Boolean.Column", typeof(bool), valuesBoolean.Cast<object>().ToArray())
                , ExpectedColumn("Byte.Column", typeof(byte), valuesByte.Cast<object>().ToArray())
                , ExpectedColumn("Char.Column", typeof(char), valuesChar.Cast<object>().ToArray())
                , ExpectedColumn("DateTime.Column", typeof(DateTime), valuesDateTime.Cast<object>().ToArray())
                , ExpectedColumn("Decimal.Column", typeof(decimal), valuesDecimal.Cast<object>().ToArray())
                , ExpectedColumn("Double.Column", typeof(double), valuesDouble.Cast<object>().ToArray())
                , ExpectedColumn("Guid.Column", typeof(Guid), valuesGuid.Cast<object>().ToArray())
                , ExpectedColumn("Int16.Column", typeof(short), valuesInt16.Cast<object>().ToArray())
                , ExpectedColumn("Int32.Column", typeof(int), valuesInt32.Cast<object>().ToArray())
                , ExpectedColumn("Int64.Column", typeof(long), valuesInt64.Cast<object>().ToArray())
                , ExpectedColumn("SByte.Column", typeof(sbyte), valuesSByte.Cast<object>().ToArray())
                , ExpectedColumn("Single.Column", typeof(float), valuesSingle.Cast<object>().ToArray())
                , ExpectedColumn("String.Column", typeof(string), valuesString.Cast<object>().ToArray())
                , ExpectedColumn("TimeSpan.Column", typeof(TimeSpan), valuesTimeSpan.Cast<object>().ToArray())
                , ExpectedColumn("UInt16.Column", typeof(ushort), valuesUInt16.Cast<object>().ToArray())
                , ExpectedColumn("UInt32.Column", typeof(uint), valuesUInt32.Cast<object>().ToArray())
                , ExpectedColumn("UInt64.Column", typeof(ulong), valuesUInt64.Cast<object>().ToArray())
                , ExpectedColumn("Binary.Column", typeof(byte[]), valuesBinary.Cast<object>().ToArray()));

            CheckTableCreated(table2, TableName
                , ExpectedColumn("Boolean.Column", typeof(bool), valuesBoolean2.Cast<object>().ToArray())
                , ExpectedColumn("Byte.Column", typeof(byte), valuesByte2.Cast<object>().ToArray())
                , ExpectedColumn("Char.Column", typeof(char), valuesChar2.Cast<object>().ToArray())
                , ExpectedColumn("DateTime.Column", typeof(DateTime), valuesDateTime2.Cast<object>().ToArray())
                , ExpectedColumn("Decimal.Column", typeof(decimal), valuesDecimal2.Cast<object>().ToArray())
                , ExpectedColumn("Double.Column", typeof(double), valuesDouble2.Cast<object>().ToArray())
                , ExpectedColumn("Guid.Column", typeof(Guid), valuesGuid2.Cast<object>().ToArray())
                , ExpectedColumn("Int16.Column", typeof(short), valuesInt162.Cast<object>().ToArray())
                , ExpectedColumn("Int32.Column", typeof(int), valuesInt322.Cast<object>().ToArray())
                , ExpectedColumn("Int64.Column", typeof(long), valuesInt642.Cast<object>().ToArray())
                , ExpectedColumn("SByte.Column", typeof(sbyte), valuesSByte2.Cast<object>().ToArray())
                , ExpectedColumn("Single.Column", typeof(float), valuesSingle2.Cast<object>().ToArray())
                , ExpectedColumn("String.Column", typeof(string), valuesString2.Cast<object>().ToArray())
                , ExpectedColumn("TimeSpan.Column", typeof(TimeSpan), valuesTimeSpan2.Cast<object>().ToArray())
                , ExpectedColumn("UInt16.Column", typeof(ushort), valuesUInt162.Cast<object>().ToArray())
                , ExpectedColumn("UInt32.Column", typeof(uint), valuesUInt322.Cast<object>().ToArray())
                , ExpectedColumn("UInt64.Column", typeof(ulong), valuesUInt642.Cast<object>().ToArray())
                , ExpectedColumn("Binary.Column", typeof(byte[]), valuesBinary2.Cast<object>().ToArray()));
        }

        [TestMethod]
        public void MultipleColumnsForArraysOf()
        {
            var valuesBoolean = new bool[] { false, false,false, false, false };
            var valuesByte = new byte[] { 32, 126, 215, 49, 4 };
            var valuesChar = new char[] { '/', '\\', 'x', 't', 'v' };
            var valuesDateTime = new DateTime[] { new DateTime(2014, 9, 9), new DateTime(2011, 10, 25), new DateTime(2002, 9, 19), new DateTime(1998, 2, 20), new DateTime(1993, 1, 7) };
            var valuesDecimal = new decimal[] { 305, 428, 82, 844, 213 };
            var valuesDouble = new double[] { 841.39, 179.87, 471.57, 395.44, 62.7 };
            var valuesGuid = new Guid[] { new Guid("defe624b-c002-4ec4-a891-25c8fe91768d"), new Guid("7a3f0133-3934-4836-a725-9a95bffd1f46"), new Guid("f4e344b8-6db7-4ddb-8f60-9ecf445d4fb9"), new Guid("21c199fb-cff6-48d4-b871-05b806b671c8"), new Guid("8ff53c85-85c2-4361-ba6f-c8ec2244a7ce") };
            var valuesInt16 = new short[] { 76, 727, 703, 979, 913 };
            var valuesInt32 = new int[] { -275, -859, -980, -831, -335 };
            var valuesInt64 = new long[] { -983, -2, -432, -843, -133 };
            var valuesSByte = new sbyte[] { -48, -44, -101, -119, -89 };
            var valuesSingle = new float[] { 660.52f, 81.66f, 61.56f, 557.42f, 403.29f };
            var valuesString = new string[] { "A String 132", "A String 17", "A String 172", "A String 157", "A String 72" };
            var valuesTimeSpan = new TimeSpan[] { new TimeSpan(40054), new TimeSpan(4358), new TimeSpan(17043), new TimeSpan(13222), new TimeSpan(97088) };
            var valuesUInt16 = new ushort[] { 445, 447, 251, 509, 230 };
            var valuesUInt32 = new uint[] { 838, 961, 335, 745, 82 };
            var valuesUInt64 = new ulong[] { 223, 589, 497, 351, 411 };
            var valuesBinary = new byte[][] { new byte[] { 140, 164, 9, 233, 87 }, new byte[] { 183, 110, 147, 137, 207 }, new byte[] { 232, 83, 208, 143, 188 }, new byte[] { 90, 48, 174, 249, 67 }, new byte[] { 54, 69, 145, 110, 57 } }; var values = new byte[] { 123, 187, 189, 211, 237 };

            Assert.AreSame(_uut,
                _uut.Column("Boolean.Column", valuesBoolean)
                    .Column("Byte.Column", valuesByte)
                    .Column("Char.Column", valuesChar)
                    .Column("DateTime.Column", valuesDateTime)
                    .Column("Decimal.Column", valuesDecimal)
                    .Column("Double.Column", valuesDouble)
                    .Column("Guid.Column", valuesGuid)
                    .Column("Int16.Column", valuesInt16)
                    .Column("Int32.Column", valuesInt32)
                    .Column("Int64.Column", valuesInt64)
                    .Column("SByte.Column", valuesSByte)
                    .Column("Single.Column", valuesSingle)
                    .Column("String.Column", valuesString)
                    .Column("TimeSpan.Column", valuesTimeSpan)
                    .Column("UInt16.Column", valuesUInt16)
                    .Column("UInt32.Column", valuesUInt32)
                    .Column("UInt64.Column", valuesUInt64)
                    .Column("Binary.Column", valuesBinary)
                  );
            CheckTableCreated(
                ExpectedColumn("Boolean.Column", typeof(bool), valuesBoolean.Cast<object>().ToArray())
                , ExpectedColumn("Byte.Column", typeof(byte), valuesByte.Cast<object>().ToArray())
                , ExpectedColumn("Char.Column", typeof(char), valuesChar.Cast<object>().ToArray())
                , ExpectedColumn("DateTime.Column", typeof(DateTime), valuesDateTime.Cast<object>().ToArray())
                , ExpectedColumn("Decimal.Column", typeof(decimal), valuesDecimal.Cast<object>().ToArray())
                , ExpectedColumn("Double.Column", typeof(double), valuesDouble.Cast<object>().ToArray())
                , ExpectedColumn("Guid.Column", typeof(Guid), valuesGuid.Cast<object>().ToArray())
                , ExpectedColumn("Int16.Column", typeof(short), valuesInt16.Cast<object>().ToArray())
                , ExpectedColumn("Int32.Column", typeof(int), valuesInt32.Cast<object>().ToArray())
                , ExpectedColumn("Int64.Column", typeof(long), valuesInt64.Cast<object>().ToArray())
                , ExpectedColumn("SByte.Column", typeof(sbyte), valuesSByte.Cast<object>().ToArray())
                , ExpectedColumn("Single.Column", typeof(float), valuesSingle.Cast<object>().ToArray())
                , ExpectedColumn("String.Column", typeof(string), valuesString.Cast<object>().ToArray())
                , ExpectedColumn("TimeSpan.Column", typeof(TimeSpan), valuesTimeSpan.Cast<object>().ToArray())
                , ExpectedColumn("UInt16.Column", typeof(ushort), valuesUInt16.Cast<object>().ToArray())
                , ExpectedColumn("UInt32.Column", typeof(uint), valuesUInt32.Cast<object>().ToArray())
                , ExpectedColumn("UInt64.Column", typeof(ulong), valuesUInt64.Cast<object>().ToArray())
                , ExpectedColumn("Binary.Column", typeof(byte[]), valuesBinary.Cast<object>().ToArray()));
        }

    }
}
