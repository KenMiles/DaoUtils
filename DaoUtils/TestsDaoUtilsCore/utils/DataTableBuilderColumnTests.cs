using System;
using System.Linq;
using DaoUtilsCore.utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestsDaoUtilsCore.utils
{
    [TestClass]
    public class DataTableBuilderColumnTests
    {
        private const string DefaultColumnName = "A Column Name";
        private DataTableBuilderColumn<T> Uut<T>(string columnName = DefaultColumnName)
        {
            return new DataTableBuilderColumn<T>(columnName);
        }

        private void CheckColumnName(string columnName)
        {
            var uut = Uut<int>(columnName);
            Assert.AreEqual(columnName, uut.ColumnName);
            Assert.AreEqual(columnName, uut.CreateColumn().ColumnName);
        }

        [TestMethod]
        public void ColumnNameSetOnConstructor()
        {
            CheckColumnName("A Test Column Name");
            CheckColumnName("A Different Column Name");
            CheckColumnName(DefaultColumnName);
        }

        private void CheckTypeOf<T>()
        {
            var uut = Uut<T>();
            Assert.AreEqual(typeof(T), uut.CreateColumn().DataType);
        }

        [TestMethod]
        public void TypeOf()
        {
            CheckTypeOf<bool>(); // Boolean
            CheckTypeOf<byte>(); // Byte
            CheckTypeOf<char>(); // Char
            CheckTypeOf<DateTime>(); // DateTime
            CheckTypeOf<decimal>(); // Decimal
            CheckTypeOf<double>(); // Double
            CheckTypeOf<Guid>(); // Guid
            CheckTypeOf<short>(); // Int16
            CheckTypeOf<int>(); // Int32
            CheckTypeOf<long>(); // Int64
            CheckTypeOf<sbyte>(); // SByte
            CheckTypeOf<float>(); // Single
            CheckTypeOf<string>(); // String
            CheckTypeOf<TimeSpan>(); // TimeSpan
            CheckTypeOf<ushort>(); // UInt16
            CheckTypeOf<uint>(); // UInt32
            CheckTypeOf<ulong>(); // UInt64
            CheckTypeOf<byte[]>(); // Binary
        }

        [TestMethod]
        public void Value()
        {
            var values = Enumerable.Range(0, 15).Select(i => $"Value {i}").ToArray();
            var uut = Uut<string>();
            uut.Values = values;
            for (var idx = 0; idx < values.Length; idx++)
            {
                Assert.AreEqual(values[idx], uut.Value(idx));
            }
            CollectionAssert.AreEqual(values, uut.Values);
        }

        [TestMethod]
        public void SetValues()
        {
            var values = Enumerable.Range(0, 15).Select(i => $"Value {i}").ToArray();
            var uut = Uut<string>();
            uut.SetValues(values);
            for (var idx = 0; idx < values.Length; idx++)
            {
                Assert.AreEqual(values[idx], uut.Value(idx));
            }
            CollectionAssert.AreEqual(values, uut.Values);
            Assert.AreEqual(values.Length, uut.NoValues);
        }

        [TestMethod]
        public void SetValuesHandlesNull()
        {
            var uut = Uut<string>();
            Assert.AreEqual(0, uut.NoValues);
            Assert.IsNull(uut.Values);
            uut.SetValues(null);
            Assert.IsNotNull(uut.Values);
            Assert.AreEqual(0, uut.Values.Length);
            Assert.AreEqual(0, uut.NoValues);
        }
    }
}
