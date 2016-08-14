using System;
using System.Data;
using DaoUtils.core;
using DaoUtilsCore.core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TestsDaoUtilsCore.core
{
    [TestClass]
    public class ColumnReadValueTests
    {
        private readonly Mock<IDataReader> _dataReader = new Mock<IDataReader>(MockBehavior.Strict);
        private const int ColumnIndex = 9978;
        private ColumnReadValue _testing;

        [TestInitialize]
        public void Setup()
        {
            _testing = new ColumnReadValue(_dataReader.Object, ColumnIndex);
        }

        [TestMethod]
        public void DbNull()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsTrue(_testing.DbNull);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            Assert.IsFalse(_testing.DbNull);
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsByte()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual(0, _testing.AsByte);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetByte(ColumnIndex)).Returns(87).Verifiable();
            Assert.AreEqual((byte)87, _testing.AsByte);
            _dataReader.Verify();
        }

        [TestMethod]
        public void ByteReadValue()
        {
            var testing = _testing.AsByteReader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<byte>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual(0, testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetByte(ColumnIndex)).Returns(65).Verifiable();
            Assert.AreEqual((byte)65, testing.Read());
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsByteNullable()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(_testing.AsByteNullable);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetByte(ColumnIndex)).Returns(87).Verifiable();
            Assert.AreEqual((byte)87, _testing.AsByte);
            _dataReader.Verify();
        }

        [TestMethod]
        public void ByteNullableReader()
        {
            var testing = _testing.AsByteNullableReader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<byte?>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetByte(ColumnIndex)).Returns(65).Verifiable();
            Assert.AreEqual((byte)65, testing.Read());
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsBoolean()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual(false, _testing.AsBoolean);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetBoolean(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual(true, _testing.AsBoolean);
            _dataReader.Verify();
            _dataReader.Setup(h => h.GetBoolean(ColumnIndex)).Returns(false).Verifiable();
            Assert.AreEqual(false, _testing.AsBoolean);
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsBooleanReader()
        {
            var testing = _testing.AsBooleanReader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<bool>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual(false, testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetBoolean(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual(true, testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.GetBoolean(ColumnIndex)).Returns(false).Verifiable();
            Assert.AreEqual(false, testing.Read());
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsBooleanNullable()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(_testing.AsBooleanNullable);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetBoolean(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual(true, _testing.AsBoolean);
            _dataReader.Verify();
            _dataReader.Setup(h => h.GetBoolean(ColumnIndex)).Returns(false).Verifiable();
            Assert.AreEqual(false, _testing.AsBoolean);
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsBooleanNullableReader()
        {
            var testing = _testing.AsBooleanNullableReader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<bool?>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetBoolean(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual(true, testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.GetBoolean(ColumnIndex)).Returns(false).Verifiable();
            Assert.AreEqual(false, testing.Read());
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsDateTime()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual(default(DateTime), _testing.AsDateTime);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            var value = new DateTime(1928, 08, 23, 12, 34, 35);
            _dataReader.Setup(h => h.GetDateTime(ColumnIndex)).Returns(value).Verifiable();
            Assert.AreEqual(value, _testing.AsDateTime);
            _dataReader.Verify();
        }

        [TestMethod]
        public void DateTimeReadValue()
        {
            var testing = _testing.AsDateTimeReader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<DateTime>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual(default(DateTime), testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            var value = new DateTime(1956, 09, 13, 22, 21, 05);
            _dataReader.Setup(h => h.GetDateTime(ColumnIndex)).Returns(value).Verifiable();
            Assert.AreEqual(value, testing.Read());
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsDateTimeNullable()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(_testing.AsDateTimeNullable);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            var value = new DateTime(1976, 09, 13, 22, 21, 05);
            _dataReader.Setup(h => h.GetDateTime(ColumnIndex)).Returns(value).Verifiable();
            Assert.AreEqual(value, _testing.AsDateTimeNullable);
            _dataReader.Verify();
        }

        [TestMethod]
        public void DateTimeNullableReader()
        {
            var testing = _testing.AsDateTimeNullableReader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<DateTime?>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            var value = new DateTime(1928, 03, 12, 02, 34, 35);
            _dataReader.Setup(h => h.GetDateTime(ColumnIndex)).Returns(value).Verifiable();
            Assert.AreEqual(value, testing.Read());
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsDate()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual(default(DateTime), _testing.AsDate);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            var value = new DateTime(1928, 08, 23);
            _dataReader.Setup(h => h.GetDateTime(ColumnIndex)).Returns(value).Verifiable();
            Assert.AreEqual(value, _testing.AsDate);
            _dataReader.Verify();
        }

        [TestMethod]
        public void DateReadValue()
        {
            var testing = _testing.AsDateReader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<DateTime>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual(default(DateTime), testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            var value = new DateTime(1956, 09, 13);
            _dataReader.Setup(h => h.GetDateTime(ColumnIndex)).Returns(value).Verifiable();
            Assert.AreEqual(value, testing.Read());
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsDateNullable()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(_testing.AsDateNullable);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            var value = new DateTime(1976, 09, 13, 22, 21, 05);
            _dataReader.Setup(h => h.GetDateTime(ColumnIndex)).Returns(value).Verifiable();
            Assert.AreEqual(value, _testing.AsDateNullable);
            _dataReader.Verify();
        }

        [TestMethod]
        public void DateNullableReader()
        {
            var testing = _testing.AsDateNullableReader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<DateTime?>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            var value = new DateTime(1928, 03, 12, 02, 34, 35);
            _dataReader.Setup(h => h.GetDateTime(ColumnIndex)).Returns(value).Verifiable();
            Assert.AreEqual(value, testing.Read());
            _dataReader.Verify();
        }
        [TestMethod]
        public void AsTime()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual(default(DateTime), _testing.AsTime);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            var value = new DateTime(1928, 08, 23, 12, 34, 35);
            _dataReader.Setup(h => h.GetDateTime(ColumnIndex)).Returns(value).Verifiable();
            Assert.AreEqual(value, _testing.AsTime);
            _dataReader.Verify();
        }

        [TestMethod]
        public void TimeReadValue()
        {
            var testing = _testing.AsTimeReader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<DateTime>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual(default(DateTime), testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            var value = new DateTime(1956, 09, 13, 22, 21, 05);
            _dataReader.Setup(h => h.GetDateTime(ColumnIndex)).Returns(value).Verifiable();
            Assert.AreEqual(value, testing.Read());
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsTimeNullable()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(_testing.AsTimeNullable);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            var value = new DateTime(1976, 09, 13, 22, 21, 05);
            _dataReader.Setup(h => h.GetDateTime(ColumnIndex)).Returns(value).Verifiable();
            Assert.AreEqual(value, _testing.AsTime);
            _dataReader.Verify();
        }

        [TestMethod]
        public void TimeNullableReader()
        {
            var testing = _testing.AsTimeNullableReader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<DateTime?>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            var value = new DateTime(1928, 03, 12, 02, 34, 35);
            _dataReader.Setup(h => h.GetDateTime(ColumnIndex)).Returns(value).Verifiable();
            Assert.AreEqual(value, testing.Read());
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsDecimal()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual(0, _testing.AsDecimal);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetDecimal(ColumnIndex)).Returns(87).Verifiable();
            Assert.AreEqual(87, _testing.AsDecimal);
            _dataReader.Verify();
        }

        [TestMethod]
        public void DecimalReadValue()
        {
            var testing = _testing.AsDecimalReader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<decimal>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual(0, testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetDecimal(ColumnIndex)).Returns(65).Verifiable();
            Assert.AreEqual(65, testing.Read());
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsDecimalNullable()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(_testing.AsDecimalNullable);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetDecimal(ColumnIndex)).Returns(87).Verifiable();
            Assert.AreEqual(87, _testing.AsDecimal);
            _dataReader.Verify();
        }

        [TestMethod]
        public void DecimalNullableReader()
        {
            var testing = _testing.AsDecimalNullableReader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<decimal?>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetDecimal(ColumnIndex)).Returns(65).Verifiable();
            Assert.AreEqual(65, testing.Read());
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsDouble()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual(0, _testing.AsDouble);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetDouble(ColumnIndex)).Returns(87.1).Verifiable();
            Assert.AreEqual(87.1, _testing.AsDouble);
            _dataReader.Verify();
        }

        [TestMethod]
        public void DoubleReadValue()
        {
            var testing = _testing.AsDoubleReader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<double>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual(0, testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetDouble(ColumnIndex)).Returns(65.3).Verifiable();
            Assert.AreEqual(65.3, testing.Read());
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsDoubleNullable()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(_testing.AsDoubleNullable);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetDouble(ColumnIndex)).Returns(87.4).Verifiable();
            Assert.AreEqual(87.4, _testing.AsDouble);
            _dataReader.Verify();
        }

        [TestMethod]
        public void DoubleNullableReader()
        {
            var testing = _testing.AsDoubleNullableReader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<Double?>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetDouble(ColumnIndex)).Returns(65.6).Verifiable();
            Assert.AreEqual(65.6, testing.Read());
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsInt16()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual(0, _testing.AsInt16);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetInt16(ColumnIndex)).Returns(87).Verifiable();
            Assert.AreEqual((short)87, _testing.AsInt16);
            _dataReader.Verify();
        }

        [TestMethod]
        public void Int16ReadValue()
        {
            var testing = _testing.AsInt16Reader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<short>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual(0, testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetInt16(ColumnIndex)).Returns(65).Verifiable();
            Assert.AreEqual((short)65, testing.Read());
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsShort()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual(0, _testing.AsShort);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetInt16(ColumnIndex)).Returns(87).Verifiable();
            Assert.AreEqual((short)87, _testing.AsShort);
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsInt16Nullable()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(_testing.AsInt16Nullable);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetInt16(ColumnIndex)).Returns(87).Verifiable();
            Assert.AreEqual((short)87, _testing.AsInt16Nullable);
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsShortNullable()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(_testing.AsShortNullable);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetInt16(ColumnIndex)).Returns(87).Verifiable();
            Assert.AreEqual((short)87, _testing.AsShortNullable);
            _dataReader.Verify();
        }

        [TestMethod]
        public void Int16NullableReader()
        {
            var testing = _testing.AsInt16NullableReader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<short?>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetInt16(ColumnIndex)).Returns(65).Verifiable();
            Assert.AreEqual((short)65, testing.Read());
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsInt32()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual(0, _testing.AsInt32);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetInt32(ColumnIndex)).Returns(87).Verifiable();
            Assert.AreEqual(87, _testing.AsInt32);
            _dataReader.Verify();
        }

        public void AsInt()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual(0, _testing.AsInt);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetInt32(ColumnIndex)).Returns(87).Verifiable();
            Assert.AreEqual(87, _testing.AsInt);
            _dataReader.Verify();
        }

        [TestMethod]
        public void Int32ReadValue()
        {
            var testing = _testing.AsInt32Reader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<int>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual(0, testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetInt32(ColumnIndex)).Returns(65).Verifiable();
            Assert.AreEqual(65, testing.Read());
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsInt32Nullable()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(_testing.AsInt32Nullable);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetInt32(ColumnIndex)).Returns(87).Verifiable();
            Assert.AreEqual((int)87, _testing.AsInt32Nullable);
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsIntNullable()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(_testing.AsIntNullable);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetInt32(ColumnIndex)).Returns(87).Verifiable();
            Assert.AreEqual((int)87, _testing.AsIntNullable);
            _dataReader.Verify();
        }

        [TestMethod]
        public void Int32NullableReader()
        {
            var testing = _testing.AsInt32NullableReader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<Int32?>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetInt32(ColumnIndex)).Returns(65).Verifiable();
            Assert.AreEqual(65, testing.Read());
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsInt64()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual(0, _testing.AsInt64);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetInt64(ColumnIndex)).Returns(87).Verifiable();
            Assert.AreEqual((long)87, _testing.AsInt64);
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsLong()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual(0, _testing.AsLong);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetInt64(ColumnIndex)).Returns(87).Verifiable();
            Assert.AreEqual((long)87, _testing.AsLong);
            _dataReader.Verify();
        }

        [TestMethod]
        public void Int64ReadValue()
        {
            var testing = _testing.AsInt64Reader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<long>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual(0, testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetInt64(ColumnIndex)).Returns(65).Verifiable();
            Assert.AreEqual((long)65, testing.Read());
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsInt64Nullable()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(_testing.AsInt64Nullable);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetInt64(ColumnIndex)).Returns(87).Verifiable();
            Assert.AreEqual((long)87, _testing.AsInt64Nullable);
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsLongNullable()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(_testing.AsLongNullable);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetInt64(ColumnIndex)).Returns(87).Verifiable();
            Assert.AreEqual((long)87, _testing.AsLongNullable);
            _dataReader.Verify();
        }

        [TestMethod]
        public void Int64NullableReader()
        {
            var testing = _testing.AsInt64NullableReader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<Int64?>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetInt64(ColumnIndex)).Returns(65).Verifiable();
            Assert.AreEqual((long)65, testing.Read());
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsUInt16()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual(0, _testing.AsUInt16);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetInt32(ColumnIndex)).Returns(87).Verifiable();
            Assert.AreEqual((ushort)87, _testing.AsUInt16);
            _dataReader.Verify();
        }

        [TestMethod]
        public void UInt16ReadValue()
        {
            var testing = _testing.AsUInt16Reader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<UInt16>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual(0, testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetInt32(ColumnIndex)).Returns(65).Verifiable();
            Assert.AreEqual((ushort)65, testing.Read());
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsUInt16Nullable()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(_testing.AsUInt16Nullable);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetInt32(ColumnIndex)).Returns(87).Verifiable();
            Assert.AreEqual((ushort)87, _testing.AsUInt16);
            _dataReader.Verify();
        }

        [TestMethod]
        public void UInt16NullableReader()
        {
            var testing = _testing.AsUInt16NullableReader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<UInt16?>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetInt32(ColumnIndex)).Returns(65).Verifiable();
            Assert.AreEqual((ushort)65, testing.Read());
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsUInt32()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual((uint)0, _testing.AsUInt32);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetInt64(ColumnIndex)).Returns(87).Verifiable();
            Assert.AreEqual((uint)87, _testing.AsUInt32);
            _dataReader.Verify();
        }

        [TestMethod]
        public void UInt32ReadValue()
        {
            var testing = _testing.AsUInt32Reader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<UInt32>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual((uint)0, testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetInt64(ColumnIndex)).Returns(65).Verifiable();
            Assert.AreEqual((uint)65, testing.Read());
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsUInt32Nullable()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(_testing.AsUInt32Nullable);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetInt64(ColumnIndex)).Returns(87).Verifiable();
            Assert.AreEqual((uint)87, _testing.AsUInt32);
            _dataReader.Verify();
        }

        [TestMethod]
        public void UInt32NullableReader()
        {
            var testing = _testing.AsUInt32NullableReader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<uint?>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetInt64(ColumnIndex)).Returns(65).Verifiable();
            Assert.AreEqual((uint)65, testing.Read());
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsUInt64()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual((ulong)0, _testing.AsUInt64);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetValue(ColumnIndex)).Returns(87).Verifiable();
            Assert.AreEqual((ulong)87, _testing.AsUInt64);
            _dataReader.Verify();
        }

        [TestMethod]
        public void UInt64ReadValue()
        {
            var testing = _testing.AsUInt64Reader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<UInt64>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual((ulong)0, testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetValue(ColumnIndex)).Returns(65).Verifiable();
            Assert.AreEqual((ulong)65, testing.Read());
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsUInt64Nullable()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(_testing.AsUInt64Nullable);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetValue(ColumnIndex)).Returns(87).Verifiable();
            Assert.AreEqual((ulong)87, _testing.AsUInt64);
            _dataReader.Verify();
        }

        [TestMethod]
        public void UInt64NullableReader()
        {
            var testing = _testing.AsUInt64NullableReader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<UInt64?>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetValue(ColumnIndex)).Returns(65).Verifiable();
            Assert.AreEqual((ulong)65, testing.Read());
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsBinary()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(_testing.AsBinary);
            _dataReader.Verify();
            var value = new byte[] {0, 1, 2, 3, 4, 5};
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h[ColumnIndex]).Returns(value).Verifiable();
            CollectionAssert.AreEqual(value, _testing.AsBinary);
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsBinaryReaderReader()
        {
            var testing = _testing.AsBinaryReader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<byte[]>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(testing.Value);
            _dataReader.Verify();
            var value = new byte[] { 0, 1, 2, 3, 4, 5 };
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h[ColumnIndex]).Returns(value).Verifiable();
            CollectionAssert.AreEqual(value, testing.Value);
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsSByte()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual(0, _testing.AsSByte);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetInt16(ColumnIndex)).Returns(87).Verifiable();
            Assert.AreEqual((sbyte)87, _testing.AsSByte);
            _dataReader.Verify();
        }

        [TestMethod]
        public void ByteSReadValue()
        {
            var testing = _testing.AsSByteReader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<sbyte>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual(0, testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetInt16(ColumnIndex)).Returns(65).Verifiable();
            Assert.AreEqual((sbyte)65, testing.Read());
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsSByteNullable()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(_testing.AsSByteNullable);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetInt16(ColumnIndex)).Returns(87).Verifiable();
            Assert.AreEqual((sbyte)87, _testing.AsSByte);
            _dataReader.Verify();
        }

        [TestMethod]
        public void SByteNullableReader()
        {
            var testing = _testing.AsSByteNullableReader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<sbyte?>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetInt16(ColumnIndex)).Returns(65).Verifiable();
            Assert.AreEqual((sbyte)65, testing.Read());
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsSingle()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual((float)0, _testing.AsSingle);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetFloat(ColumnIndex)).Returns((float)87.1).Verifiable();
            Assert.AreEqual((float)87.1, _testing.AsSingle);
            _dataReader.Verify();
        }

        [TestMethod]
        public void SingleReadValue()
        {
            var testing = _testing.AsSingleReader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<Single>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.AreEqual((float)0, testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetFloat(ColumnIndex)).Returns((float)65.2).Verifiable();
            Assert.AreEqual((float)65.2, testing.Read());
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsSingleNullable()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(_testing.AsSingleNullable);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetFloat(ColumnIndex)).Returns((float)87.4).Verifiable();
            Assert.AreEqual((float)87.4, _testing.AsSingle);
            _dataReader.Verify();
        }

        [TestMethod]
        public void SingleNullableReader()
        {
            var testing = _testing.AsSingleNullableReader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<Single?>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetFloat(ColumnIndex)).Returns((float)65.6).Verifiable();
            Assert.AreEqual((float)65.6, testing.Read());
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsString()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(_testing.AsString);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetString(ColumnIndex)).Returns("Hello Out Three").Verifiable();
            Assert.AreEqual("Hello Out Three", _testing.AsString);
            _dataReader.Verify();
        }

        [TestMethod]
        public void StringReader()
        {
            var testing = _testing.AsStringReader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<string>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetString(ColumnIndex)).Returns("Hello Out Three").Verifiable();
            Assert.AreEqual("Hello Out Three", testing.Value);
            _dataReader.Verify();
        }

        [TestMethod]
        public void AsAnsiString()
        {
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(_testing.AsAnsiString);
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetString(ColumnIndex)).Returns("Hello Out Three").Verifiable();
            Assert.AreEqual("Hello Out Three", _testing.AsAnsiString);
            _dataReader.Verify();
        }

        [TestMethod]
        public void StringAnsiReader()
        {
            var testing = _testing.AsAnsiStringReader;
            Assert.IsInstanceOfType(testing, typeof(ColumnReadTypedValue<string>));
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(true).Verifiable();
            Assert.IsNull(testing.Read());
            _dataReader.Verify();
            _dataReader.Setup(h => h.IsDBNull(ColumnIndex)).Returns(false).Verifiable();
            _dataReader.Setup(h => h.GetString(ColumnIndex)).Returns("Hello Out Three").Verifiable();
            Assert.AreEqual("Hello Out Three", testing.Value);
            _dataReader.Verify();
        }

    }
}
