using System;
using Common.Logging;
using DaoUtils.core;
using DaoUtils.Standard;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TestsDaoUtilsCore.core
{
    [TestClass]
    public class ParameterReadValueTest
    {
        private readonly Mock<IDaoParameterInternal> _parameter = new Mock<IDaoParameterInternal>(MockBehavior.Strict);
        private readonly Mock<ILog> _log = new Mock<ILog>(MockBehavior.Loose);
        private ParameterReadValue<T> GenericUut<T>()
        {
            return new ParameterReadValue<T>(_parameter.Object, _log.Object);
        }

        private ParameterReadValue _uut;

        [TestInitialize]
        public void SetUp()
        {
            _uut = new ParameterReadValue(_parameter.Object);
        }

        [TestMethod]
        public void GenericReadsValue()
        {
            _parameter.Setup(p => p.GetValueAsObject()).Returns("1234567890");
            Assert.AreEqual("1234567890", GenericUut<string>().Value);
            Assert.AreEqual("1234567890", GenericUut<string>().Read());
            Assert.AreEqual("1234567890", GenericUut<string>().Read("10221"));
        }

        [TestMethod]
        public void GenericReadsAndConverts()
        {
            _parameter.Setup(p => p.GetValueAsObject()).Returns("1234");
            Assert.AreEqual(1234, GenericUut<int>().Value);
            Assert.AreEqual(1234, GenericUut<int>().Read());
            Assert.AreEqual(1234.0, GenericUut<double>().Read());
            Assert.AreEqual(1234.0, GenericUut<double>().Value);
        }

        [TestMethod]
        public void GenericUsesDefaultForType()
        {
            _parameter.Setup(p => p.GetValueAsObject()).Returns(DBNull.Value);
            Assert.AreEqual(default(int), GenericUut<int>().Value);
            Assert.AreEqual(null, GenericUut<int?>().Read());
            Assert.AreEqual(null, GenericUut<string>().Read());
            Assert.AreEqual(default(double), GenericUut<double>().Value);
        }

        [TestMethod]
        public void GenericUsesDefaultPassed()
        {
            _parameter.Setup(p => p.GetValueAsObject()).Returns(DBNull.Value);
            Assert.AreEqual(100, GenericUut<int?>().Read(100));
            Assert.AreEqual("A Value", GenericUut<string>().Read("A Value"));
        }

        [TestMethod]
        public void GenericCopesWithNull()
        {
            _parameter.Setup(p => p.GetValueAsObject()).Returns(null);
            Assert.AreEqual(default(int), GenericUut<int>().Value);
            Assert.AreEqual(null, GenericUut<int?>().Read());
            Assert.AreEqual(null, GenericUut<string>().Read());
            Assert.AreEqual(default(double), GenericUut<double>().Value);
        }

        [TestMethod]
        public void GenericLogsException()
        {
            var e = new Exception("A Test Errror");
            _parameter.Setup(p => p.GetValueAsObject()).Throws(e);
            _parameter.Setup(p => p.Name).Returns("A Parameter Name");
            try
            {
                GenericUut<int>().Read();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(e.Message, (ex.InnerException ?? ex).Message);
            }
            _log.Verify(l => l.Error($"Reading paramater '{"A Parameter Name"}' as {typeof(int).Name}", e));
        }

        [TestMethod]
        public void DbNull()
        {
            _parameter.Setup(p => p.GetValueAsObject()).Returns(DBNull.Value);
            Assert.IsTrue(_uut.DbNull);
            _parameter.Setup(p => p.GetValueAsObject()).Returns(100);
            Assert.IsFalse(_uut.DbNull);
        }

        [TestMethod]
        public void AsByteReader()
        {
            byte value = 72;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<byte> reader = _uut.AsByteReader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsByteReader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsByte()
        {
            byte value = 72;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsByte);
        }

        [TestMethod]
        public void AsBinaryReader()
        {
            byte[] value = new byte[] { 0, 1, 2, 3, 4, 5, 37 };
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<byte[]> reader = _uut.AsBinaryReader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsBinaryReader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsBinary()
        {
            byte[] value = new byte[] { 0, 1, 2, 3, 4, 5, 37 };
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsBinary);
        }

        [TestMethod]
        public void AsByteNullableReader()
        {
            byte? value = 57;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<byte?> reader = _uut.AsByteNullableReader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsByteNullableReader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsByteNullable()
        {
            byte? value = 57;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsByteNullable);
        }

        [TestMethod]
        public void AsDateTimeReader()
        {
            DateTime value = DateTime.Now;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<DateTime> reader = _uut.AsDateTimeReader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsDateTimeReader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsDateTime()
        {
            DateTime value = DateTime.Now;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsDateTime);
        }

        [TestMethod]
        public void AsDateReader()
        {
            DateTime value = DateTime.Now;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<DateTime> reader = _uut.AsDateReader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsDateReader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsDate()
        {
            DateTime value = DateTime.Now;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsDate);
        }

        [TestMethod]
        public void AsTimeReader()
        {
            DateTime value = DateTime.Now;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<DateTime> reader = _uut.AsTimeReader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsTimeReader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsTime()
        {
            DateTime value = DateTime.Now;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsTime);
        }

        [TestMethod]
        public void AsDateTimeNullableReader()
        {
            DateTime? value = DateTime.Now;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<DateTime?> reader = _uut.AsDateTimeNullableReader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsDateTimeNullableReader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsDateTimeNullable()
        {
            DateTime? value = DateTime.Now;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsDateTimeNullable);
        }

        [TestMethod]
        public void AsDateNullableReader()
        {
            DateTime? value = DateTime.Now;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<DateTime?> reader = _uut.AsDateNullableReader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsDateNullableReader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsDateNullable()
        {
            DateTime? value = DateTime.Now;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsDateNullable);
        }

        [TestMethod]
        public void AsTimeNullableReader()
        {
            DateTime? value = DateTime.Now;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<DateTime?> reader = _uut.AsTimeNullableReader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsTimeNullableReader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsTimeNullable()
        {
            DateTime? value = DateTime.Now;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsTimeNullable);
        }

        [TestMethod]
        public void AsDecimalReader()
        {
            decimal value = 97;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<decimal> reader = _uut.AsDecimalReader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsDecimalReader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsDecimal()
        {
            decimal value = 97;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsDecimal);
        }

        [TestMethod]
        public void AsDecimalNullableReader()
        {
            decimal? value = 889;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<decimal?> reader = _uut.AsDecimalNullableReader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsDecimalNullableReader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsDecimalNullable()
        {
            decimal? value = 889;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsDecimalNullable);
        }

        [TestMethod]
        public void AsDoubleReader()
        {
            double value = 309.62;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<double> reader = _uut.AsDoubleReader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsDoubleReader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsDouble()
        {
            double value = 309.62;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsDouble);
        }

        [TestMethod]
        public void AsDoubleNullableReader()
        {
            double? value = 209.47;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<double?> reader = _uut.AsDoubleNullableReader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsDoubleNullableReader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsDoubleNullable()
        {
            double? value = 209.47;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsDoubleNullable);
        }

        [TestMethod]
        public void AsInt16Reader()
        {
            short value = 211;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<short> reader = _uut.AsInt16Reader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsInt16Reader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsInt16()
        {
            short value = 211;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsInt16);
        }

        [TestMethod]
        public void AsInt16NullableReader()
        {
            short? value = 822;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<short?> reader = _uut.AsInt16NullableReader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsInt16NullableReader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsInt16Nullable()
        {
            short? value = 822;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsInt16Nullable);
        }

        [TestMethod]
        public void AsInt32Reader()
        {
            int value = -368;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<int> reader = _uut.AsInt32Reader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsInt32Reader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsInt32()
        {
            int value = -368;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsInt32);
        }

        [TestMethod]
        public void AsInt32NullableReader()
        {
            int? value = -694;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<int?> reader = _uut.AsInt32NullableReader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsInt32NullableReader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsInt32Nullable()
        {
            int? value = -694;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsInt32Nullable);
        }

        [TestMethod]
        public void AsIntReader()
        {
            int value = -967;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<int> reader = _uut.AsIntReader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsIntReader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsInt()
        {
            int value = -967;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsInt);
        }

        [TestMethod]
        public void AsIntNullableReader()
        {
            int? value = -994;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<int?> reader = _uut.AsIntNullableReader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsIntNullableReader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsIntNullable()
        {
            int? value = -994;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsIntNullable);
        }

        [TestMethod]
        public void AsInt64Reader()
        {
            long value = -944;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<long> reader = _uut.AsInt64Reader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsInt64Reader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsInt64()
        {
            long value = -944;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsInt64);
        }

        [TestMethod]
        public void AsInt64NullableReader()
        {
            long? value = -338;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<long?> reader = _uut.AsInt64NullableReader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsInt64NullableReader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsInt64Nullable()
        {
            long? value = -338;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsInt64Nullable);
        }

        [TestMethod]
        public void AsLongReader()
        {
            long value = -868;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<long> reader = _uut.AsLongReader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsLongReader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsLong()
        {
            long value = -868;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsLong);
        }

        [TestMethod]
        public void AsLongNullableReader()
        {
            long? value = -951;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<long?> reader = _uut.AsLongNullableReader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsLongNullableReader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsLongNullable()
        {
            long? value = -951;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsLongNullable);
        }

        [TestMethod]
        public void AsSByteReader()
        {
            sbyte value = -38;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<sbyte> reader = _uut.AsSByteReader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsSByteReader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsSByte()
        {
            sbyte value = -38;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsSByte);
        }

        [TestMethod]
        public void AsSByteNullableReader()
        {
            sbyte? value = -104;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<sbyte?> reader = _uut.AsSByteNullableReader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsSByteNullableReader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsSByteNullable()
        {
            sbyte? value = -104;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsSByteNullable);
        }

        [TestMethod]
        public void AsSingleReader()
        {
            float value = 248.93f;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<float> reader = _uut.AsSingleReader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsSingleReader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsSingle()
        {
            float value = 248.93f;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsSingle);
        }

        [TestMethod]
        public void AsSingleNullableReader()
        {
            float? value = 552.92f;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<float?> reader = _uut.AsSingleNullableReader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsSingleNullableReader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsSingleNullable()
        {
            float? value = 552.92f;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsSingleNullable);
        }

        [TestMethod]
        public void AsStringReader()
        {
            string value = "A String 52";
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<string> reader = _uut.AsStringReader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsStringReader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsString()
        {
            string value = "A String 52";
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsString);
        }

        [TestMethod]
        public void AsAnsiStringReader()
        {
            string value = "A String 121";
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<string> reader = _uut.AsAnsiStringReader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsAnsiStringReader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsAnsiString()
        {
            string value = "A String 121";
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsAnsiString);
        }

        [TestMethod]
        public void AsUInt16Reader()
        {
            ushort value = 254;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<ushort> reader = _uut.AsUInt16Reader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsUInt16Reader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsUInt16()
        {
            ushort value = 254;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsUInt16);
        }

        [TestMethod]
        public void AsUInt16NullableReader()
        {
            ushort? value = 156;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<ushort?> reader = _uut.AsUInt16NullableReader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsUInt16NullableReader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsUInt16Nullable()
        {
            ushort? value = 156;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsUInt16Nullable);
        }

        [TestMethod]
        public void AsUInt32Reader()
        {
            uint value = 265;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<uint> reader = _uut.AsUInt32Reader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsUInt32Reader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsUInt32()
        {
            uint value = 265;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsUInt32);
        }

        [TestMethod]
        public void AsUInt32NullableReader()
        {
            uint? value = 484;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<uint?> reader = _uut.AsUInt32NullableReader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsUInt32NullableReader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsUInt32Nullable()
        {
            uint? value = 484;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsUInt32Nullable);
        }

        [TestMethod]
        public void AsUInt64Reader()
        {
            ulong value = 733;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<ulong> reader = _uut.AsUInt64Reader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsUInt64Reader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsUInt64()
        {
            ulong value = 733;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsUInt64);
        }

        [TestMethod]
        public void AsUInt64NullableReader()
        {
            ulong? value = 102;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            IReadValue<ulong?> reader = _uut.AsUInt64NullableReader;
            // Check Cached
            Assert.AreEqual(reader, _uut.AsUInt64NullableReader);
            Assert.AreEqual(value, reader.Value);
        }

        [TestMethod]
        public void AsUInt64Nullable()
        {
            ulong? value = 102;
            _parameter.Setup(p => p.GetValueAsObject()).Returns(value);
            Assert.AreEqual(value, _uut.AsUInt64Nullable);
        }
    }
}
