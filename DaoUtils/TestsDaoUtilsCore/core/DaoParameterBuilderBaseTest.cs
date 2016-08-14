using System;
using System.Collections.Generic;
using System.Data;
using DaoUtils.core;
using DaoUtils.Standard;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TestsDaoUtilsCore.core
{
    internal class TestableDaoParameterBuilderBase: DaoParameterBuilderBase<
        IDaoParametersBuilderInput, 
        IDaoParametersBuilderInputOutput, 
        IDaoParametersBuilderOutput, 
        IDbCommand
    >
    {
        private readonly List<IDaoParameterInternal> _parameters;
        public TestableDaoParameterBuilderBase(IDbCommand command, List<IDaoParameterInternal> parameters, string name) : base(command, parameters, name)
        {
            _parameters = parameters;
        }

        private readonly List<IDaoParameterInternal> _createdParameters = new List<IDaoParameterInternal>();
        protected override DaoParameter<TP> DaoParameter<TP>(DbType type)
        {
            var result = base.DaoParameter<TP>(type);
            _createdParameters.Add(result);
            return result;

        }

        public IDbDataParameter AccessParameter(DbType type)
        {
            return Parameter(type);
        }

        public DaoParameter<TP> AccessDaoParameter<TP>(DbType type)
        {
            return DaoParameter<TP>(type);
        }

        public void CheckParametersCreated(int createdParams = 0)
        {
            Assert.AreEqual($"Created Params = {createdParams}", $"Created Params = {_createdParameters.Count}");
            CollectionAssert.AreEqual(_createdParameters, _parameters);
        }
    }

    [TestClass]
    public class DaoParameterBuilderBaseTest
    {
        private readonly Mock<IDbCommand> _command = new Mock<IDbCommand>(MockBehavior.Strict);
        private readonly List<IDaoParameterInternal> _parameters = new List<IDaoParameterInternal>();
        private const string ParameterName = "A Parameter";

        private TestableDaoParameterBuilderBase Uut(string parameterName = ParameterName)
        {
            return new TestableDaoParameterBuilderBase(_command.Object, _parameters, parameterName);
        }


        [TestMethod]
        public void TestConstructor()
        {
            var uut = Uut();
            Assert.AreEqual(ParameterName, uut.Name);
            Assert.AreEqual(0, uut.ParamSize);
            uut.CheckParametersCreated();

            Assert.AreEqual("Hello", Uut("Hello").Name);
        }

        [TestMethod]
        public void Size()
        {
            var uut = Uut();
            Assert.AreEqual(uut, uut.Size(10));
            Assert.AreEqual(10, uut.ParamSize);
            uut.CheckParametersCreated();

            var uut2 = Uut();
            Assert.AreEqual(uut2, uut2.Size(23));
            Assert.AreEqual(23, uut2.ParamSize);
            uut2.CheckParametersCreated();
        }

        private Mock<IDbDataParameter> MockParameter(string name, ParameterDirection direction, int paramSize, DbType type)
        {
            var result = new Mock<IDbDataParameter>(MockBehavior.Strict);
            _command.Setup(s => s.CreateParameter()).Returns(result.Object);
            result.SetupSet(r => r.Size = paramSize).Verifiable();
            result.SetupSet(r => r.ParameterName = name).Verifiable();
            result.SetupSet(r => r.Direction = direction).Verifiable();
            result.SetupSet(r => r.DbType = type).Verifiable();
            return result;
        }

        [TestMethod]
        public void Parameter()
        {
            var uut = Uut();
            var mockParam = MockParameter(ParameterName, ParameterDirection.ReturnValue, 1991, DbType.Currency);
            IDaoParametersBuilderOutput returnBuilder = uut.ReturnValue;
            uut.Size(1991);
            Assert.AreSame(mockParam.Object, uut.AccessParameter(DbType.Currency));
            mockParam.Verify();
            uut.CheckParametersCreated(0);
            Assert.AreEqual(returnBuilder, uut);
        }

        [TestMethod]
        public void DaoParameter()
        {
            var uut = Uut();
            var mockParam = MockParameter(ParameterName, ParameterDirection.InputOutput, 2345, DbType.Double);
            IDaoParametersBuilderInputOutput returnBuilder = uut.InputOutput;
            uut.Size(2345);
            DaoParameter<double> param = uut.AccessDaoParameter<double>(DbType.Double);
            Assert.AreSame(mockParam.Object, param.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            Assert.AreEqual(returnBuilder, uut);
        }

        #region Input Params Test

        [TestMethod]
        public void Input()
        {
            var uut = Uut();
            IDaoParametersBuilderInput input = uut.Input;
            Assert.AreEqual(ParameterDirection.Input, uut.Direction);
            uut.CheckParametersCreated(0);
            Assert.AreEqual(input, uut);
        }

        [TestMethod]
        public void InputAsBinaryParameter()
        {
            const int size = 3702;
            const string name = "Input,AsBinaryParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Binary);
            IDaoInputParameter<byte[]> builtParam = uut.Size(size).Input.AsBinaryParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsByteNullableParameter()
        {
            const int size = 3716;
            const string name = "Input,AsByteNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Byte);
            IDaoInputParameter<byte?> builtParam = uut.Size(size).Input.AsByteNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsDateTimeParameter()
        {
            const int size = 4098;
            const string name = "Input,AsDateTimeParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.DateTime);
            IDaoInputParameter<DateTime> builtParam = uut.Size(size).Input.AsDateTimeParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsDateReaderParameter()
        {
            const int size = 9084;
            const string name = "Input,AsDateReaderParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Date);
            IDaoInputParameter<DateTime> builtParam = uut.Size(size).Input.AsDateReaderParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsTimeReaderParameter()
        {
            const int size = 64;
            const string name = "Input,AsTimeReaderParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Time);
            IDaoInputParameter<DateTime> builtParam = uut.Size(size).Input.AsTimeReaderParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsDateTimeNullableParameter()
        {
            const int size = 7179;
            const string name = "Input,AsDateTimeNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.DateTime);
            IDaoInputParameter<DateTime?> builtParam = uut.Size(size).Input.AsDateTimeNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsDateNullableParameter()
        {
            const int size = 4430;
            const string name = "Input,AsDateNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Date);
            IDaoInputParameter<DateTime?> builtParam = uut.Size(size).Input.AsDateNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsTimeNullableParameter()
        {
            const int size = 2772;
            const string name = "Input,AsTimeNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Time);
            IDaoInputParameter<DateTime?> builtParam = uut.Size(size).Input.AsTimeNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsDecimalParameter()
        {
            const int size = 2520;
            const string name = "Input,AsDecimalParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Decimal);
            IDaoInputParameter<decimal> builtParam = uut.Size(size).Input.AsDecimalParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsDecimalNullableParameter()
        {
            const int size = 8474;
            const string name = "Input,AsDecimalNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Decimal);
            IDaoInputParameter<decimal?> builtParam = uut.Size(size).Input.AsDecimalNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsDoubleParameter()
        {
            const int size = 2502;
            const string name = "Input,AsDoubleParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Double);
            IDaoInputParameter<double> builtParam = uut.Size(size).Input.AsDoubleParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsDoubleNullableParameter()
        {
            const int size = 2563;
            const string name = "Input,AsDoubleNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Double);
            IDaoInputParameter<double?> builtParam = uut.Size(size).Input.AsDoubleNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsInt16Parameter()
        {
            const int size = 4686;
            const string name = "Input,AsInt16Parameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int16);
            IDaoInputParameter<short> builtParam = uut.Size(size).Input.AsInt16Parameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsInt16NullableParameter()
        {
            const int size = 6363;
            const string name = "Input,AsInt16NullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int16);
            IDaoInputParameter<short?> builtParam = uut.Size(size).Input.AsInt16NullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsShortParameter()
        {
            const int size = 7118;
            const string name = "Input,AsShortParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int16);
            IDaoInputParameter<short> builtParam = uut.Size(size).Input.AsShortParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsShortNullableParameter()
        {
            const int size = 786;
            const string name = "Input,AsShortNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int16);
            IDaoInputParameter<short?> builtParam = uut.Size(size).Input.AsShortNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsInt32Parameter()
        {
            const int size = 7593;
            const string name = "Input,AsInt32Parameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int32);
            IDaoInputParameter<int> builtParam = uut.Size(size).Input.AsInt32Parameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsInt32NullableParameter()
        {
            const int size = 2250;
            const string name = "Input,AsInt32NullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int32);
            IDaoInputParameter<int?> builtParam = uut.Size(size).Input.AsInt32NullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsIntParameter()
        {
            const int size = 4018;
            const string name = "Input,AsIntParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int32);
            IDaoInputParameter<int> builtParam = uut.Size(size).Input.AsIntParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsIntNullableParameter()
        {
            const int size = 269;
            const string name = "Input,AsIntNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int32);
            IDaoInputParameter<int?> builtParam = uut.Size(size).Input.AsIntNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsInt64Parameter()
        {
            const int size = 594;
            const string name = "Input,AsInt64Parameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int64);
            IDaoInputParameter<long> builtParam = uut.Size(size).Input.AsInt64Parameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsInt64NullableParameter()
        {
            const int size = 441;
            const string name = "Input,AsInt64NullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int64);
            IDaoInputParameter<long?> builtParam = uut.Size(size).Input.AsInt64NullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsLongParameter()
        {
            const int size = 7350;
            const string name = "Input,AsLongParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int64);
            IDaoInputParameter<long> builtParam = uut.Size(size).Input.AsLongParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsLongNullableParameter()
        {
            const int size = 5038;
            const string name = "Input,AsLongNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int64);
            IDaoInputParameter<long?> builtParam = uut.Size(size).Input.AsLongNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsSByteParameter()
        {
            const int size = 7336;
            const string name = "Input,AsSByteParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.SByte);
            IDaoInputParameter<sbyte> builtParam = uut.Size(size).Input.AsSByteParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsSByteNullableParameter()
        {
            const int size = 340;
            const string name = "Input,AsSByteNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.SByte);
            IDaoInputParameter<sbyte?> builtParam = uut.Size(size).Input.AsSByteNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsSingleParameter()
        {
            const int size = 3165;
            const string name = "Input,AsSingleParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Single);
            IDaoInputParameter<float> builtParam = uut.Size(size).Input.AsSingleParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsSingleNullableParameter()
        {
            const int size = 4629;
            const string name = "Input,AsSingleNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Single);
            IDaoInputParameter<float?> builtParam = uut.Size(size).Input.AsSingleNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsStringParameter()
        {
            const int size = 2669;
            const string name = "Input,AsStringParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.String);
            IDaoInputParameter<string> builtParam = uut.Size(size).Input.AsStringParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsAnsiStringParameter()
        {
            const int size = 1400;
            const string name = "Input,AsAnsiStringParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.AnsiString);
            IDaoInputParameter<string> builtParam = uut.Size(size).Input.AsAnsiStringParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsUInt16Parameter()
        {
            const int size = 3803;
            const string name = "Input,AsUInt16Parameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt16);
            IDaoInputParameter<ushort> builtParam = uut.Size(size).Input.AsUInt16Parameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsUInt16NullableParameter()
        {
            const int size = 2250;
            const string name = "Input,AsUInt16NullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt16);
            IDaoInputParameter<ushort?> builtParam = uut.Size(size).Input.AsUInt16NullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsUshortParameter()
        {
            const int size = 2985;
            const string name = "Input,AsUshortParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt16);
            IDaoInputParameter<ushort> builtParam = uut.Size(size).Input.AsUshortParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsUshortNullableParameter()
        {
            const int size = 6914;
            const string name = "Input,AsUshortNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt16);
            IDaoInputParameter<ushort?> builtParam = uut.Size(size).Input.AsUshortNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsUInt32Parameter()
        {
            const int size = 7150;
            const string name = "Input,AsUInt32Parameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt32);
            IDaoInputParameter<uint> builtParam = uut.Size(size).Input.AsUInt32Parameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsUInt32NullableParameter()
        {
            const int size = 6775;
            const string name = "Input,AsUInt32NullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt32);
            IDaoInputParameter<uint?> builtParam = uut.Size(size).Input.AsUInt32NullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsUintParameter()
        {
            const int size = 2432;
            const string name = "Input,AsUintParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt32);
            IDaoInputParameter<uint> builtParam = uut.Size(size).Input.AsUintParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsUintNullableParameter()
        {
            const int size = 7695;
            const string name = "Input,AsUintNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt32);
            IDaoInputParameter<uint?> builtParam = uut.Size(size).Input.AsUintNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsUInt64Parameter()
        {
            const int size = 3214;
            const string name = "Input,AsUInt64Parameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt64);
            IDaoInputParameter<ulong> builtParam = uut.Size(size).Input.AsUInt64Parameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsUInt64NullableParameter()
        {
            const int size = 6834;
            const string name = "Input,AsUInt64NullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt64);
            IDaoInputParameter<ulong?> builtParam = uut.Size(size).Input.AsUInt64NullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsUlongParameter()
        {
            const int size = 8218;
            const string name = "Input,AsUlongParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt64);
            IDaoInputParameter<ulong> builtParam = uut.Size(size).Input.AsUlongParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputAsUlongNullableParameter()
        {
            const int size = 9817;
            const string name = "Input,AsUlongNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt64);
            IDaoInputParameter<ulong?> builtParam = uut.Size(size).Input.AsUlongNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueByte()
        {
            const int size = 2990;
            const string name = "Input,byte";
            byte value = 0;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Byte);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<byte> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueByteArray()
        {
            const int size = 5353;
            const string name = "Input,byte[]";
            byte[] value = new byte[] { 0, 1, 2, 3, 4, 5, 69 }; ;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Binary);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<byte[]> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueByteArrayNull()
        {
            const int size = 674;
            const string name = "Input,byte[]";
            byte[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Binary);
            mockParam.SetupSet(p => p.Value = DBNull.Value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<byte[]> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueByteNullable()
        {
            const int size = 3967;
            const string name = "Input,byte?";
            byte? value = 61;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Byte);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<byte?> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueByteNullableNull()
        {
            const int size = 8064;
            const string name = "Input,byte?";
            byte? value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Byte);
            mockParam.SetupSet(p => p.Value = DBNull.Value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<byte?> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueDatetime()
        {
            const int size = 7934;
            const string name = "Input,DateTime";
            DateTime value = DateTime.Now;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.DateTime);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<DateTime> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueDatetimeNullable()
        {
            const int size = 2096;
            const string name = "Input,DateTime?";
            DateTime? value = DateTime.Now;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.DateTime);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<DateTime?> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueDatetimeNullableNull()
        {
            const int size = 4176;
            const string name = "Input,DateTime?";
            DateTime? value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.DateTime);
            mockParam.SetupSet(p => p.Value = DBNull.Value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<DateTime?> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueDecimal()
        {
            const int size = 3587;
            const string name = "Input,decimal";
            decimal value = 929;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Decimal);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<decimal> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueDecimalNullable()
        {
            const int size = 8278;
            const string name = "Input,decimal?";
            decimal? value = 345;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Decimal);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<decimal?> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueDecimalNullableNull()
        {
            const int size = 6658;
            const string name = "Input,decimal?";
            decimal? value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Decimal);
            mockParam.SetupSet(p => p.Value = DBNull.Value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<decimal?> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueDouble()
        {
            const int size = 3868;
            const string name = "Input,double";
            double value = 417.122;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Double);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<double> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueDoubleNullable()
        {
            const int size = 9291;
            const string name = "Input,double?";
            double? value = 566.16;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Double);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<double?> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueDoubleNullableNull()
        {
            const int size = 823;
            const string name = "Input,double?";
            double? value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Double);
            mockParam.SetupSet(p => p.Value = DBNull.Value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<double?> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueShort()
        {
            const int size = 6574;
            const string name = "Input,short";
            short value = 311;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int16);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<short> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueShortNullable()
        {
            const int size = 1959;
            const string name = "Input,short?";
            short? value = 310;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int16);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<short?> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueShortNullableNull()
        {
            const int size = 6906;
            const string name = "Input,short?";
            short? value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int16);
            mockParam.SetupSet(p => p.Value = DBNull.Value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<short?> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueInt()
        {
            const int size = 2734;
            const string name = "Input,int";
            int value = -331;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int32);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<int> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueIntNullable()
        {
            const int size = 1980;
            const string name = "Input,int?";
            int? value = -945;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int32);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<int?> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueIntNullableNull()
        {
            const int size = 2400;
            const string name = "Input,int?";
            int? value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int32);
            mockParam.SetupSet(p => p.Value = DBNull.Value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<int?> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueLong()
        {
            const int size = 7852;
            const string name = "Input,long";
            long value = -816;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int64);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<long> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueLongNullable()
        {
            const int size = 3987;
            const string name = "Input,long?";
            long? value = -541;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int64);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<long?> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueLongNullableNull()
        {
            const int size = 2607;
            const string name = "Input,long?";
            long? value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int64);
            mockParam.SetupSet(p => p.Value = DBNull.Value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<long?> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueSbyte()
        {
            const int size = 6840;
            const string name = "Input,sbyte";
            sbyte value = -51;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.SByte);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<sbyte> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueSbyteNullable()
        {
            const int size = 8937;
            const string name = "Input,sbyte?";
            sbyte? value = -44;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.SByte);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<sbyte?> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueSbyteNullableNull()
        {
            const int size = 2459;
            const string name = "Input,sbyte?";
            sbyte? value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.SByte);
            mockParam.SetupSet(p => p.Value = DBNull.Value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<sbyte?> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueFloat()
        {
            const int size = 3607;
            const string name = "Input,float";
            float value = 680.14f;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Single);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<float> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueFloatNullable()
        {
            const int size = 7242;
            const string name = "Input,float?";
            float? value = 467.31f;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Single);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<float?> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueFloatNullableNull()
        {
            const int size = 1455;
            const string name = "Input,float?";
            float? value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Single);
            mockParam.SetupSet(p => p.Value = DBNull.Value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<float?> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueString()
        {
            const int size = 6498;
            const string name = "Input,string";
            string value = "A String 113";
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.String);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<string> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueStringNull()
        {
            const int size = 2527;
            const string name = "Input,string";
            string value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.String);
            mockParam.SetupSet(p => p.Value = DBNull.Value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<string> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueUshort()
        {
            const int size = 8762;
            const string name = "Input,ushort";
            ushort value = 727;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt16);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<ushort> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueUshortNullable()
        {
            const int size = 4501;
            const string name = "Input,ushort?";
            ushort? value = 85;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt16);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<ushort?> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueUshortNullableNull()
        {
            const int size = 4009;
            const string name = "Input,ushort?";
            ushort? value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt16);
            mockParam.SetupSet(p => p.Value = DBNull.Value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<ushort?> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueUint()
        {
            const int size = 8128;
            const string name = "Input,uint";
            uint value = 425;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt32);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<uint> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueUintNullable()
        {
            const int size = 9936;
            const string name = "Input,uint?";
            uint? value = 601;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt32);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<uint?> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueUintNullableNull()
        {
            const int size = 242;
            const string name = "Input,uint?";
            uint? value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt32);
            mockParam.SetupSet(p => p.Value = DBNull.Value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<uint?> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueUlong()
        {
            const int size = 6577;
            const string name = "Input,ulong";
            ulong value = 821;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt64);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<ulong> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueUlongNullable()
        {
            const int size = 6837;
            const string name = "Input,ulong?";
            ulong? value = 973;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt64);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<ulong?> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValueUlongNullableNull()
        {
            const int size = 8390;
            const string name = "Input,ulong?";
            ulong? value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt64);
            mockParam.SetupSet(p => p.Value = DBNull.Value).Verifiable();
            var uut = Uut(name);
            IDaoInputParameter<ulong?> builtParam = uut.Size(size).Input.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputValuesByte()
        {
            const int size = 8510;
            const string name = "Input,byte";
            byte[] value = new byte[] { 52, 76, 39, 12, 82 };
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Byte);
            var uut = Uut(name);
            IDaoInputParameter<byte> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesByteNull()
        {
            const int size = 4932;
            const string name = "Input,byte";
            byte[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Byte);
            var uut = Uut(name);
            IDaoInputParameter<byte> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesByteArray()
        {
            const int size = 9984;
            const string name = "Input,byte[]";
            byte[][] value = new byte[][] { new byte[] { 0, 1, 2, 3, 4, 5, 115 }, new byte[] { 0, 1, 2, 3, 4, 5, 14 }, new byte[] { 0, 1, 2, 3, 4, 5, 73 }, new byte[] { 0, 1, 2, 3, 4, 5, 53 }, new byte[] { 0, 1, 2, 3, 4, 5, 41 } };
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Binary);
            var uut = Uut(name);
            IDaoInputParameter<byte[]> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesByteArrayNull()
        {
            const int size = 8640;
            const string name = "Input,byte[]";
            byte[][] value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Binary);
            var uut = Uut(name);
            IDaoInputParameter<byte[]> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesByteNullable()
        {
            const int size = 94;
            const string name = "Input,byte?";
            byte?[] value = new byte?[] { 88, 124, 20, 1, 41 };
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Byte);
            var uut = Uut(name);
            IDaoInputParameter<byte?> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesByteNullableNull()
        {
            const int size = 6899;
            const string name = "Input,byte?";
            byte?[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Byte);
            var uut = Uut(name);
            IDaoInputParameter<byte?> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesDatetime()
        {
            const int size = 2019;
            const string name = "Input,DateTime";
            DateTime[] value = new DateTime[] { DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now };
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.DateTime);
            var uut = Uut(name);
            IDaoInputParameter<DateTime> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesDatetimeNull()
        {
            const int size = 2108;
            const string name = "Input,DateTime";
            DateTime[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.DateTime);
            var uut = Uut(name);
            IDaoInputParameter<DateTime> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesDatetimeNullable()
        {
            const int size = 7843;
            const string name = "Input,DateTime?";
            DateTime?[] value = new DateTime?[] { DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now };
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.DateTime);
            var uut = Uut(name);
            IDaoInputParameter<DateTime?> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesDatetimeNullableNull()
        {
            const int size = 5680;
            const string name = "Input,DateTime?";
            DateTime?[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.DateTime);
            var uut = Uut(name);
            IDaoInputParameter<DateTime?> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesDecimal()
        {
            const int size = 3913;
            const string name = "Input,decimal";
            decimal[] value = new decimal[] { 171, 12, 355, 542, 423 };
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Decimal);
            var uut = Uut(name);
            IDaoInputParameter<decimal> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesDecimalNull()
        {
            const int size = 5735;
            const string name = "Input,decimal";
            decimal[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Decimal);
            var uut = Uut(name);
            IDaoInputParameter<decimal> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesDecimalNullable()
        {
            const int size = 4132;
            const string name = "Input,decimal?";
            decimal?[] value = new decimal?[] { 719, 28, 67, 296, 785 };
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Decimal);
            var uut = Uut(name);
            IDaoInputParameter<decimal?> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesDecimalNullableNull()
        {
            const int size = 9979;
            const string name = "Input,decimal?";
            decimal?[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Decimal);
            var uut = Uut(name);
            IDaoInputParameter<decimal?> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesDouble()
        {
            const int size = 7479;
            const string name = "Input,double";
            double[] value = new double[] { 183.40, 711.35, 849.112, 85.16, 838.28 };
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Double);
            var uut = Uut(name);
            IDaoInputParameter<double> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesDoubleNull()
        {
            const int size = 9197;
            const string name = "Input,double";
            double[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Double);
            var uut = Uut(name);
            IDaoInputParameter<double> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesDoubleNullable()
        {
            const int size = 7144;
            const string name = "Input,double?";
            double?[] value = new double?[] { 136.69, 343.101, 659.31, 90.36, 747.97 };
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Double);
            var uut = Uut(name);
            IDaoInputParameter<double?> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesDoubleNullableNull()
        {
            const int size = 4918;
            const string name = "Input,double?";
            double?[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Double);
            var uut = Uut(name);
            IDaoInputParameter<double?> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesShort()
        {
            const int size = 6881;
            const string name = "Input,short";
            short[] value = new short[] { 452, 491, 487, 178, 754 };
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int16);
            var uut = Uut(name);
            IDaoInputParameter<short> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesShortNull()
        {
            const int size = 9361;
            const string name = "Input,short";
            short[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int16);
            var uut = Uut(name);
            IDaoInputParameter<short> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesShortNullable()
        {
            const int size = 4745;
            const string name = "Input,short?";
            short?[] value = new short?[] { 447, 67, 306, 274, 976 };
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int16);
            var uut = Uut(name);
            IDaoInputParameter<short?> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesShortNullableNull()
        {
            const int size = 9711;
            const string name = "Input,short?";
            short?[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int16);
            var uut = Uut(name);
            IDaoInputParameter<short?> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesInt()
        {
            const int size = 9842;
            const string name = "Input,int";
            int[] value = new int[] { -820, -888, -322, -576, -792 };
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int32);
            var uut = Uut(name);
            IDaoInputParameter<int> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesIntNull()
        {
            const int size = 4486;
            const string name = "Input,int";
            int[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int32);
            var uut = Uut(name);
            IDaoInputParameter<int> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesIntNullable()
        {
            const int size = 2720;
            const string name = "Input,int?";
            int?[] value = new int?[] { -640, -742, -569, -799, -874 };
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int32);
            var uut = Uut(name);
            IDaoInputParameter<int?> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesIntNullableNull()
        {
            const int size = 8025;
            const string name = "Input,int?";
            int?[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int32);
            var uut = Uut(name);
            IDaoInputParameter<int?> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesLong()
        {
            const int size = 2347;
            const string name = "Input,long";
            long[] value = new long[] { -227, -470, -440, -750, -81 };
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int64);
            var uut = Uut(name);
            IDaoInputParameter<long> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesLongNull()
        {
            const int size = 7597;
            const string name = "Input,long";
            long[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int64);
            var uut = Uut(name);
            IDaoInputParameter<long> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesLongNullable()
        {
            const int size = 2704;
            const string name = "Input,long?";
            long?[] value = new long?[] { -130, -435, -111, -279, -192 };
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int64);
            var uut = Uut(name);
            IDaoInputParameter<long?> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesLongNullableNull()
        {
            const int size = 6522;
            const string name = "Input,long?";
            long?[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Int64);
            var uut = Uut(name);
            IDaoInputParameter<long?> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesSbyte()
        {
            const int size = 8704;
            const string name = "Input,sbyte";
            sbyte[] value = new sbyte[] { -100, -56, -101, -65, -68 };
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.SByte);
            var uut = Uut(name);
            IDaoInputParameter<sbyte> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesSbyteNull()
        {
            const int size = 2634;
            const string name = "Input,sbyte";
            sbyte[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.SByte);
            var uut = Uut(name);
            IDaoInputParameter<sbyte> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesSbyteNullable()
        {
            const int size = 3017;
            const string name = "Input,sbyte?";
            sbyte?[] value = new sbyte?[] { -65, -21, -34, -58, -50 };
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.SByte);
            var uut = Uut(name);
            IDaoInputParameter<sbyte?> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesSbyteNullableNull()
        {
            const int size = 6883;
            const string name = "Input,sbyte?";
            sbyte?[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.SByte);
            var uut = Uut(name);
            IDaoInputParameter<sbyte?> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesFloat()
        {
            const int size = 6695;
            const string name = "Input,float";
            float[] value = new float[] { 726.110f, 337.33f, 992.51f, 826.42f, 145.119f };
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Single);
            var uut = Uut(name);
            IDaoInputParameter<float> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesFloatNull()
        {
            const int size = 8038;
            const string name = "Input,float";
            float[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Single);
            var uut = Uut(name);
            IDaoInputParameter<float> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesFloatNullable()
        {
            const int size = 9102;
            const string name = "Input,float?";
            float?[] value = new float?[] { 118.123f, 303.116f, 853.69f, 196.26f, 423.32f };
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Single);
            var uut = Uut(name);
            IDaoInputParameter<float?> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesFloatNullableNull()
        {
            const int size = 7833;
            const string name = "Input,float?";
            float?[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.Single);
            var uut = Uut(name);
            IDaoInputParameter<float?> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesString()
        {
            const int size = 5193;
            const string name = "Input,string";
            string[] value = new string[] { "A String 117", "A String 0", "A String 90", "A String 30", "A String 37" };
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.String);
            var uut = Uut(name);
            IDaoInputParameter<string> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesStringNull()
        {
            const int size = 884;
            const string name = "Input,string";
            string[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.String);
            var uut = Uut(name);
            IDaoInputParameter<string> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesUshort()
        {
            const int size = 438;
            const string name = "Input,ushort";
            ushort[] value = new ushort[] { 185, 556, 44, 379, 368 };
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt16);
            var uut = Uut(name);
            IDaoInputParameter<ushort> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesUshortNull()
        {
            const int size = 1539;
            const string name = "Input,ushort";
            ushort[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt16);
            var uut = Uut(name);
            IDaoInputParameter<ushort> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesUshortNullable()
        {
            const int size = 3983;
            const string name = "Input,ushort?";
            ushort?[] value = new ushort?[] { 167, 552, 105, 458, 482 };
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt16);
            var uut = Uut(name);
            IDaoInputParameter<ushort?> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesUshortNullableNull()
        {
            const int size = 9707;
            const string name = "Input,ushort?";
            ushort?[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt16);
            var uut = Uut(name);
            IDaoInputParameter<ushort?> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesUint()
        {
            const int size = 8939;
            const string name = "Input,uint";
            uint[] value = new uint[] { 110, 402, 613, 218, 846 };
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt32);
            var uut = Uut(name);
            IDaoInputParameter<uint> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesUintNull()
        {
            const int size = 3736;
            const string name = "Input,uint";
            uint[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt32);
            var uut = Uut(name);
            IDaoInputParameter<uint> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesUintNullable()
        {
            const int size = 9297;
            const string name = "Input,uint?";
            uint?[] value = new uint?[] { 654, 216, 531, 570, 853 };
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt32);
            var uut = Uut(name);
            IDaoInputParameter<uint?> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesUintNullableNull()
        {
            const int size = 5547;
            const string name = "Input,uint?";
            uint?[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt32);
            var uut = Uut(name);
            IDaoInputParameter<uint?> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesUlong()
        {
            const int size = 5719;
            const string name = "Input,ulong";
            ulong[] value = new ulong[] { 693, 193, 60, 926, 2 };
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt64);
            var uut = Uut(name);
            IDaoInputParameter<ulong> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesUlongNull()
        {
            const int size = 7941;
            const string name = "Input,ulong";
            ulong[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt64);
            var uut = Uut(name);
            IDaoInputParameter<ulong> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesUlongNullable()
        {
            const int size = 6730;
            const string name = "Input,ulong?";
            ulong?[] value = new ulong?[] { 903, 467, 824, 452, 783 };
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt64);
            var uut = Uut(name);
            IDaoInputParameter<ulong?> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputValuesUlongNullableNull()
        {
            const int size = 8798;
            const string name = "Input,ulong?";
            ulong?[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.Input, size, DbType.UInt64);
            var uut = Uut(name);
            IDaoInputParameter<ulong?> builtParam = uut.Size(size).Input.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }
        #endregion
        #region InputOutput Params Test

        [TestMethod]
        public void InputOutput()
        {
            var uut = Uut();
            IDaoParametersBuilderInputOutput inputOutput = uut.InputOutput;
            Assert.AreEqual(ParameterDirection.InputOutput, uut.Direction);
            uut.CheckParametersCreated(0);
            Assert.AreEqual(inputOutput, uut);
        }

        [TestMethod]
        public void InputOutputAsBinaryParameter()
        {
            const int size = 4083;
            const string name = "Input,AsBinaryParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Binary);
            IDaoInputOutputParameter<byte[]> builtParam = uut.Size(size).InputOutput.AsBinaryParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsByteParameter()
        {
            const int size = 9117;
            const string name = "Input,AsByteParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Byte);
            IDaoInputOutputParameter<byte> builtParam = uut.Size(size).InputOutput.AsByteParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }


        [TestMethod]
        public void InputOutputAsByteNullableParameter()
        {
            const int size = 6734;
            const string name = "Input,AsByteNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Byte);
            IDaoInputOutputParameter<byte?> builtParam = uut.Size(size).InputOutput.AsByteNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsDateTimeParameter()
        {
            const int size = 821;
            const string name = "Input,AsDateTimeParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.DateTime);
            IDaoInputOutputParameter<DateTime> builtParam = uut.Size(size).InputOutput.AsDateTimeParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsDateParameter()
        {
            const int size = 1505;
            const string name = "Input,AsDateParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Date);
            IDaoInputOutputParameter<DateTime> builtParam = uut.Size(size).InputOutput.AsDateParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsTimeParameter()
        {
            const int size = 2578;
            const string name = "Input,AsTimeParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Time);
            IDaoInputOutputParameter<DateTime> builtParam = uut.Size(size).InputOutput.AsTimeParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsDateTimeNullableParameter()
        {
            const int size = 6642;
            const string name = "Input,AsDateTimeNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.DateTime);
            IDaoInputOutputParameter<DateTime?> builtParam = uut.Size(size).InputOutput.AsDateTimeNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsDateNullableParameter()
        {
            const int size = 9833;
            const string name = "Input,AsDateNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Date);
            IDaoInputOutputParameter<DateTime?> builtParam = uut.Size(size).InputOutput.AsDateNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsTimeNullableParameter()
        {
            const int size = 4985;
            const string name = "Input,AsTimeNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Time);
            IDaoInputOutputParameter<DateTime?> builtParam = uut.Size(size).InputOutput.AsTimeNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsDecimalParameter()
        {
            const int size = 3293;
            const string name = "Input,AsDecimalParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Decimal);
            IDaoInputOutputParameter<decimal> builtParam = uut.Size(size).InputOutput.AsDecimalParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsDecimalNullableParameter()
        {
            const int size = 6598;
            const string name = "Input,AsDecimalNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Decimal);
            IDaoInputOutputParameter<decimal?> builtParam = uut.Size(size).InputOutput.AsDecimalNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsDoubleParameter()
        {
            const int size = 8867;
            const string name = "Input,AsDoubleParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Double);
            IDaoInputOutputParameter<double> builtParam = uut.Size(size).InputOutput.AsDoubleParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsDoubleNullableParameter()
        {
            const int size = 2554;
            const string name = "Input,AsDoubleNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Double);
            IDaoInputOutputParameter<double?> builtParam = uut.Size(size).InputOutput.AsDoubleNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsInt16Parameter()
        {
            const int size = 218;
            const string name = "Input,AsInt16Parameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Int16);
            IDaoInputOutputParameter<short> builtParam = uut.Size(size).InputOutput.AsInt16Parameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsInt16NullableParameter()
        {
            const int size = 7568;
            const string name = "Input,AsInt16NullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Int16);
            IDaoInputOutputParameter<short?> builtParam = uut.Size(size).InputOutput.AsInt16NullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsInt32Parameter()
        {
            const int size = 4357;
            const string name = "Input,AsInt32Parameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Int32);
            IDaoInputOutputParameter<int> builtParam = uut.Size(size).InputOutput.AsInt32Parameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsInt32NullableParameter()
        {
            const int size = 6528;
            const string name = "Input,AsInt32NullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Int32);
            IDaoInputOutputParameter<int?> builtParam = uut.Size(size).InputOutput.AsInt32NullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsIntParameter()
        {
            const int size = 9106;
            const string name = "Input,AsIntParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Int32);
            IDaoInputOutputParameter<int> builtParam = uut.Size(size).InputOutput.AsIntParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsIntNullableParameter()
        {
            const int size = 1300;
            const string name = "Input,AsIntNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Int32);
            IDaoInputOutputParameter<int?> builtParam = uut.Size(size).InputOutput.AsIntNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsInt64Parameter()
        {
            const int size = 1794;
            const string name = "Input,AsInt64Parameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Int64);
            IDaoInputOutputParameter<long> builtParam = uut.Size(size).InputOutput.AsInt64Parameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsInt64NullableParameter()
        {
            const int size = 7801;
            const string name = "Input,AsInt64NullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Int64);
            IDaoInputOutputParameter<long?> builtParam = uut.Size(size).InputOutput.AsInt64NullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsLongParameter()
        {
            const int size = 7677;
            const string name = "Input,AsLongParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Int64);
            IDaoInputOutputParameter<long> builtParam = uut.Size(size).InputOutput.AsLongParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsLongNullableParameter()
        {
            const int size = 1859;
            const string name = "Input,AsLongNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Int64);
            IDaoInputOutputParameter<long?> builtParam = uut.Size(size).InputOutput.AsLongNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsSByteParameter()
        {
            const int size = 9527;
            const string name = "Input,AsSByteParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.SByte);
            IDaoInputOutputParameter<sbyte> builtParam = uut.Size(size).InputOutput.AsSByteParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsSByteNullableParameter()
        {
            const int size = 7787;
            const string name = "Input,AsSByteNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.SByte);
            IDaoInputOutputParameter<sbyte?> builtParam = uut.Size(size).InputOutput.AsSByteNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsSingleParameter()
        {
            const int size = 4571;
            const string name = "Input,AsSingleParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Single);
            IDaoInputOutputParameter<float> builtParam = uut.Size(size).InputOutput.AsSingleParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsSingleNullableParameter()
        {
            const int size = 9364;
            const string name = "Input,AsSingleNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Single);
            IDaoInputOutputParameter<float?> builtParam = uut.Size(size).InputOutput.AsSingleNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsStringParameter()
        {
            const int size = 8000;
            const string name = "Input,AsStringParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.String);
            IDaoInputOutputParameter<string> builtParam = uut.Size(size).InputOutput.AsStringParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsAnsiStringParameter()
        {
            const int size = 652;
            const string name = "Input,AsAnsiStringParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.AnsiString);
            IDaoInputOutputParameter<string> builtParam = uut.Size(size).InputOutput.AsAnsiStringParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsUInt16Parameter()
        {
            const int size = 6078;
            const string name = "Input,AsUInt16Parameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.UInt16);
            IDaoInputOutputParameter<ushort> builtParam = uut.Size(size).InputOutput.AsUInt16Parameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsUInt16NullableParameter()
        {
            const int size = 9892;
            const string name = "Input,AsUInt16NullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.UInt16);
            IDaoInputOutputParameter<ushort?> builtParam = uut.Size(size).InputOutput.AsUInt16NullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsUInt32Parameter()
        {
            const int size = 9380;
            const string name = "Input,AsUInt32Parameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.UInt32);
            IDaoInputOutputParameter<uint> builtParam = uut.Size(size).InputOutput.AsUInt32Parameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsUInt32NullableParameter()
        {
            const int size = 3476;
            const string name = "Input,AsUInt32NullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.UInt32);
            IDaoInputOutputParameter<uint?> builtParam = uut.Size(size).InputOutput.AsUInt32NullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsUInt64Parameter()
        {
            const int size = 5731;
            const string name = "Input,AsUInt64Parameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.UInt64);
            IDaoInputOutputParameter<ulong> builtParam = uut.Size(size).InputOutput.AsUInt64Parameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputAsUInt64NullableParameter()
        {
            const int size = 2988;
            const string name = "Input,AsUInt64NullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.UInt64);
            IDaoInputOutputParameter<ulong?> builtParam = uut.Size(size).InputOutput.AsUInt64NullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueByte()
        {
            const int size = 4864;
            const string name = "Input,byte";
            byte value = 68;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Byte);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<byte> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueByteArray()
        {
            const int size = 3244;
            const string name = "Input,byte[]";
            byte[] value = new byte[] { 0, 1, 2, 3, 4, 5, 33 }; ;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Binary);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<byte[]> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueByteNullable()
        {
            const int size = 7578;
            const string name = "Input,byte?";
            byte? value = 16;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Byte);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<byte?> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueByteNullableNull()
        {
            const int size = 2871;
            const string name = "Input,byte?";
            byte? value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Byte);
            mockParam.SetupSet(p => p.Value = DBNull.Value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<byte?> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueDatetime()
        {
            const int size = 4946;
            const string name = "Input,DateTime";
            DateTime value = DateTime.Now;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.DateTime);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<DateTime> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueDatetimeNullable()
        {
            const int size = 3679;
            const string name = "Input,DateTime?";
            DateTime? value = DateTime.Now;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.DateTime);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<DateTime?> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueDatetimeNullableNull()
        {
            const int size = 5895;
            const string name = "Input,DateTime?";
            DateTime? value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.DateTime);
            mockParam.SetupSet(p => p.Value = DBNull.Value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<DateTime?> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueDecimal()
        {
            const int size = 522;
            const string name = "Input,decimal";
            decimal value = 889;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Decimal);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<decimal> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueDecimalNullable()
        {
            const int size = 530;
            const string name = "Input,decimal?";
            decimal? value = 350;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Decimal);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<decimal?> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueDecimalNullableNull()
        {
            const int size = 9285;
            const string name = "Input,decimal?";
            decimal? value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Decimal);
            mockParam.SetupSet(p => p.Value = DBNull.Value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<decimal?> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueDouble()
        {
            const int size = 5796;
            const string name = "Input,double";
            double value = 740.109;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Double);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<double> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueDoubleNullable()
        {
            const int size = 1553;
            const string name = "Input,double?";
            double? value = 190.96;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Double);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<double?> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueDoubleNullableNull()
        {
            const int size = 7037;
            const string name = "Input,double?";
            double? value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Double);
            mockParam.SetupSet(p => p.Value = DBNull.Value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<double?> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueShort()
        {
            const int size = 5752;
            const string name = "Input,short";
            short value = 502;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Int16);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<short> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueShortNullable()
        {
            const int size = 7525;
            const string name = "Input,short?";
            short? value = 363;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Int16);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<short?> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueShortNullableNull()
        {
            const int size = 2459;
            const string name = "Input,short?";
            short? value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Int16);
            mockParam.SetupSet(p => p.Value = DBNull.Value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<short?> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueInt()
        {
            const int size = 1562;
            const string name = "Input,int";
            int value = -383;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Int32);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<int> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueIntNullable()
        {
            const int size = 8209;
            const string name = "Input,int?";
            int? value = -394;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Int32);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<int?> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueIntNullableNull()
        {
            const int size = 9458;
            const string name = "Input,int?";
            int? value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Int32);
            mockParam.SetupSet(p => p.Value = DBNull.Value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<int?> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueLong()
        {
            const int size = 2392;
            const string name = "Input,long";
            long value = -888;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Int64);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<long> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueLongNullable()
        {
            const int size = 5418;
            const string name = "Input,long?";
            long? value = -145;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Int64);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<long?> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueLongNullableNull()
        {
            const int size = 6496;
            const string name = "Input,long?";
            long? value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Int64);
            mockParam.SetupSet(p => p.Value = DBNull.Value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<long?> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueSbyte()
        {
            const int size = 3917;
            const string name = "Input,sbyte";
            sbyte value = -111;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.SByte);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<sbyte> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueSbyteNullable()
        {
            const int size = 5579;
            const string name = "Input,sbyte?";
            sbyte? value = -24;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.SByte);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<sbyte?> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueSbyteNullableNull()
        {
            const int size = 3880;
            const string name = "Input,sbyte?";
            sbyte? value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.SByte);
            mockParam.SetupSet(p => p.Value = DBNull.Value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<sbyte?> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueFloat()
        {
            const int size = 3552;
            const string name = "Input,float";
            float value = 744.11f;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Single);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<float> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueFloatNullable()
        {
            const int size = 5833;
            const string name = "Input,float?";
            float? value = 602.25f;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Single);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<float?> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueFloatNullableNull()
        {
            const int size = 6630;
            const string name = "Input,float?";
            float? value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Single);
            mockParam.SetupSet(p => p.Value = DBNull.Value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<float?> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueString()
        {
            const int size = 615;
            const string name = "Input,string";
            string value = "A String 64";
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.String);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<string> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueStringNull()
        {
            const int size = 2382;
            const string name = "Input,string";
            string value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.String);
            mockParam.SetupSet(p => p.Value = DBNull.Value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<string> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueUshort()
        {
            const int size = 5907;
            const string name = "Input,ushort";
            ushort value = 343;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.UInt16);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<ushort> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueUshortNullable()
        {
            const int size = 8533;
            const string name = "Input,ushort?";
            ushort? value = 895;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.UInt16);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<ushort?> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueUshortNullableNull()
        {
            const int size = 9565;
            const string name = "Input,ushort?";
            ushort? value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.UInt16);
            mockParam.SetupSet(p => p.Value = DBNull.Value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<ushort?> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueUint()
        {
            const int size = 6809;
            const string name = "Input,uint";
            uint value = 982;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.UInt32);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<uint> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueUintNullable()
        {
            const int size = 1958;
            const string name = "Input,uint?";
            uint? value = 340;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.UInt32);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<uint?> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueUintNullableNull()
        {
            const int size = 448;
            const string name = "Input,uint?";
            uint? value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.UInt32);
            mockParam.SetupSet(p => p.Value = DBNull.Value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<uint?> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueUlong()
        {
            const int size = 3670;
            const string name = "Input,ulong";
            ulong value = 54;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.UInt64);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<ulong> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueUlongNullable()
        {
            const int size = 9767;
            const string name = "Input,ulong?";
            ulong? value = 183;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.UInt64);
            mockParam.SetupSet(p => p.Value = value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<ulong?> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputValueUlongNullableNull()
        {
            const int size = 1416;
            const string name = "Input,ulong?";
            ulong? value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.UInt64);
            mockParam.SetupSet(p => p.Value = DBNull.Value).Verifiable();
            var uut = Uut(name);
            IDaoInputOutputParameter<ulong?> builtParam = uut.Size(size).InputOutput.Value(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void InputOutputMultipleValueUlongNullable()
        {
            const int size = 9767;
            const string name = "Input,ulong?";
            ulong?[] value = new ulong?[] { 183, 182 };
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.UInt64);
            var uut = Uut(name);
            IDaoInputOutputParameter<ulong?> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesByte()
        {
            const int size = 3365;
            const string name = "Input,byte";
            byte[] value = new byte[] { 56, 117, 43, 67, 103 };
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Byte);
            var uut = Uut(name);
            IDaoInputOutputParameter<byte> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesByteNull()
        {
            const int size = 960;
            const string name = "Input,byte";
            byte[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Byte);
            var uut = Uut(name);
            IDaoInputOutputParameter<byte> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesByteArray()
        {
            const int size = 1984;
            const string name = "Input,byte[]";
            byte[][] value = new byte[][] { new byte[] { 0, 1, 2, 3, 4, 5, 104 }, new byte[] { 0, 1, 2, 3, 4, 5, 9 }, new byte[] { 0, 1, 2, 3, 4, 5, 14 }, new byte[] { 0, 1, 2, 3, 4, 5, 70 }, new byte[] { 0, 1, 2, 3, 4, 5, 61 } };
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Binary);
            var uut = Uut(name);
            IDaoInputOutputParameter<byte[]> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesByteArrayNull()
        {
            const int size = 5579;
            const string name = "Input,byte[]";
            byte[][] value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Binary);
            var uut = Uut(name);
            IDaoInputOutputParameter<byte[]> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesByteNullable()
        {
            const int size = 6951;
            const string name = "Input,byte?";
            byte?[] value = new byte?[] { 103, 88, 22, 42, 122 };
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Byte);
            var uut = Uut(name);
            IDaoInputOutputParameter<byte?> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesByteNullableNull()
        {
            const int size = 3848;
            const string name = "Input,byte?";
            byte?[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Byte);
            var uut = Uut(name);
            IDaoInputOutputParameter<byte?> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesDatetime()
        {
            const int size = 827;
            const string name = "Input,DateTime";
            DateTime[] value = new DateTime[] { DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now };
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.DateTime);
            var uut = Uut(name);
            IDaoInputOutputParameter<DateTime> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesDatetimeNull()
        {
            const int size = 521;
            const string name = "Input,DateTime";
            DateTime[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.DateTime);
            var uut = Uut(name);
            IDaoInputOutputParameter<DateTime> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesDatetimeNullable()
        {
            const int size = 1368;
            const string name = "Input,DateTime?";
            DateTime?[] value = new DateTime?[] { DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now };
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.DateTime);
            var uut = Uut(name);
            IDaoInputOutputParameter<DateTime?> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesDatetimeNullableNull()
        {
            const int size = 5693;
            const string name = "Input,DateTime?";
            DateTime?[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.DateTime);
            var uut = Uut(name);
            IDaoInputOutputParameter<DateTime?> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesDecimal()
        {
            const int size = 5009;
            const string name = "Input,decimal";
            decimal[] value = new decimal[] { 573, 56, 346, 116, 687 };
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Decimal);
            var uut = Uut(name);
            IDaoInputOutputParameter<decimal> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesDecimalNull()
        {
            const int size = 5606;
            const string name = "Input,decimal";
            decimal[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Decimal);
            var uut = Uut(name);
            IDaoInputOutputParameter<decimal> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesDecimalNullable()
        {
            const int size = 5878;
            const string name = "Input,decimal?";
            decimal?[] value = new decimal?[] { 650, 701, 964, 247, 657 };
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Decimal);
            var uut = Uut(name);
            IDaoInputOutputParameter<decimal?> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesDecimalNullableNull()
        {
            const int size = 3957;
            const string name = "Input,decimal?";
            decimal?[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Decimal);
            var uut = Uut(name);
            IDaoInputOutputParameter<decimal?> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesDouble()
        {
            const int size = 7132;
            const string name = "Input,double";
            double[] value = new double[] { 410.14, 802.38, 649.47, 34.15, 222.52 };
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Double);
            var uut = Uut(name);
            IDaoInputOutputParameter<double> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesDoubleNull()
        {
            const int size = 968;
            const string name = "Input,double";
            double[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Double);
            var uut = Uut(name);
            IDaoInputOutputParameter<double> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesDoubleNullable()
        {
            const int size = 4071;
            const string name = "Input,double?";
            double?[] value = new double?[] { 677.92, 883.81, 966.57, 687.111, 97.35 };
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Double);
            var uut = Uut(name);
            IDaoInputOutputParameter<double?> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesDoubleNullableNull()
        {
            const int size = 316;
            const string name = "Input,double?";
            double?[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Double);
            var uut = Uut(name);
            IDaoInputOutputParameter<double?> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesShort()
        {
            const int size = 9391;
            const string name = "Input,short";
            short[] value = new short[] { 639, 298, 889, 544, 658 };
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Int16);
            var uut = Uut(name);
            IDaoInputOutputParameter<short> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesShortNull()
        {
            const int size = 1719;
            const string name = "Input,short";
            short[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Int16);
            var uut = Uut(name);
            IDaoInputOutputParameter<short> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesShortNullable()
        {
            const int size = 9648;
            const string name = "Input,short?";
            short?[] value = new short?[] { 314, 510, 466, 302, 920 };
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Int16);
            var uut = Uut(name);
            IDaoInputOutputParameter<short?> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesShortNullableNull()
        {
            const int size = 9923;
            const string name = "Input,short?";
            short?[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Int16);
            var uut = Uut(name);
            IDaoInputOutputParameter<short?> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesInt()
        {
            const int size = 6438;
            const string name = "Input,int";
            int[] value = new int[] { -443, -732, -900, -20, -275 };
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Int32);
            var uut = Uut(name);
            IDaoInputOutputParameter<int> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesIntNull()
        {
            const int size = 7388;
            const string name = "Input,int";
            int[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Int32);
            var uut = Uut(name);
            IDaoInputOutputParameter<int> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesIntNullable()
        {
            const int size = 8443;
            const string name = "Input,int?";
            int?[] value = new int?[] { -823, -532, -168, -77, -479 };
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Int32);
            var uut = Uut(name);
            IDaoInputOutputParameter<int?> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesIntNullableNull()
        {
            const int size = 1492;
            const string name = "Input,int?";
            int?[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Int32);
            var uut = Uut(name);
            IDaoInputOutputParameter<int?> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesLong()
        {
            const int size = 9851;
            const string name = "Input,long";
            long[] value = new long[] { -382, -819, -104, -238, -382 };
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Int64);
            var uut = Uut(name);
            IDaoInputOutputParameter<long> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesLongNull()
        {
            const int size = 6079;
            const string name = "Input,long";
            long[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Int64);
            var uut = Uut(name);
            IDaoInputOutputParameter<long> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesLongNullable()
        {
            const int size = 2354;
            const string name = "Input,long?";
            long?[] value = new long?[] { -138, -85, -919, -763, -999 };
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Int64);
            var uut = Uut(name);
            IDaoInputOutputParameter<long?> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesLongNullableNull()
        {
            const int size = 7431;
            const string name = "Input,long?";
            long?[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Int64);
            var uut = Uut(name);
            IDaoInputOutputParameter<long?> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesSbyte()
        {
            const int size = 5043;
            const string name = "Input,sbyte";
            sbyte[] value = new sbyte[] { -60, -118, -101, -31, -74 };
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.SByte);
            var uut = Uut(name);
            IDaoInputOutputParameter<sbyte> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesSbyteNull()
        {
            const int size = 3835;
            const string name = "Input,sbyte";
            sbyte[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.SByte);
            var uut = Uut(name);
            IDaoInputOutputParameter<sbyte> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesSbyteNullable()
        {
            const int size = 4271;
            const string name = "Input,sbyte?";
            sbyte?[] value = new sbyte?[] { -97, -7, -22, -8, -104 };
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.SByte);
            var uut = Uut(name);
            IDaoInputOutputParameter<sbyte?> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesSbyteNullableNull()
        {
            const int size = 2136;
            const string name = "Input,sbyte?";
            sbyte?[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.SByte);
            var uut = Uut(name);
            IDaoInputOutputParameter<sbyte?> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesFloat()
        {
            const int size = 3358;
            const string name = "Input,float";
            float[] value = new float[] { 25.48f, 819.66f, 80.38f, 228.59f, 377.18f };
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Single);
            var uut = Uut(name);
            IDaoInputOutputParameter<float> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesFloatNull()
        {
            const int size = 3511;
            const string name = "Input,float";
            float[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Single);
            var uut = Uut(name);
            IDaoInputOutputParameter<float> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesFloatNullable()
        {
            const int size = 3061;
            const string name = "Input,float?";
            float?[] value = new float?[] { 325.19f, 787.53f, 151.16f, 470.1f, 395.79f };
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Single);
            var uut = Uut(name);
            IDaoInputOutputParameter<float?> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesFloatNullableNull()
        {
            const int size = 2593;
            const string name = "Input,float?";
            float?[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.Single);
            var uut = Uut(name);
            IDaoInputOutputParameter<float?> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesString()
        {
            const int size = 3788;
            const string name = "Input,string";
            string[] value = new string[] { "A String 107", "A String 72", "A String 116", "A String 113", "A String 22" };
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.String);
            var uut = Uut(name);
            IDaoInputOutputParameter<string> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesStringNull()
        {
            const int size = 7843;
            const string name = "Input,string";
            string[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.String);
            var uut = Uut(name);
            IDaoInputOutputParameter<string> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesUshort()
        {
            const int size = 7449;
            const string name = "Input,ushort";
            ushort[] value = new ushort[] { 434, 518, 275, 401, 710 };
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.UInt16);
            var uut = Uut(name);
            IDaoInputOutputParameter<ushort> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesUshortNull()
        {
            const int size = 8563;
            const string name = "Input,ushort";
            ushort[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.UInt16);
            var uut = Uut(name);
            IDaoInputOutputParameter<ushort> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesUshortNullable()
        {
            const int size = 9622;
            const string name = "Input,ushort?";
            ushort?[] value = new ushort?[] { 277, 686, 956, 994, 269 };
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.UInt16);
            var uut = Uut(name);
            IDaoInputOutputParameter<ushort?> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesUshortNullableNull()
        {
            const int size = 423;
            const string name = "Input,ushort?";
            ushort?[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.UInt16);
            var uut = Uut(name);
            IDaoInputOutputParameter<ushort?> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesUint()
        {
            const int size = 4293;
            const string name = "Input,uint";
            uint[] value = new uint[] { 213, 803, 226, 417, 644 };
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.UInt32);
            var uut = Uut(name);
            IDaoInputOutputParameter<uint> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesUintNull()
        {
            const int size = 6943;
            const string name = "Input,uint";
            uint[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.UInt32);
            var uut = Uut(name);
            IDaoInputOutputParameter<uint> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesUintNullable()
        {
            const int size = 2884;
            const string name = "Input,uint?";
            uint?[] value = new uint?[] { 489, 358, 904, 146, 725 };
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.UInt32);
            var uut = Uut(name);
            IDaoInputOutputParameter<uint?> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesUintNullableNull()
        {
            const int size = 6577;
            const string name = "Input,uint?";
            uint?[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.UInt32);
            var uut = Uut(name);
            IDaoInputOutputParameter<uint?> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesUlong()
        {
            const int size = 200;
            const string name = "Input,ulong";
            ulong[] value = new ulong[] { 218, 223, 384, 88, 480 };
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.UInt64);
            var uut = Uut(name);
            IDaoInputOutputParameter<ulong> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesUlongNull()
        {
            const int size = 9430;
            const string name = "Input,ulong";
            ulong[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.UInt64);
            var uut = Uut(name);
            IDaoInputOutputParameter<ulong> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesUlongNullable()
        {
            const int size = 588;
            const string name = "Input,ulong?";
            ulong?[] value = new ulong?[] { 433, 727, 327, 332, 68 };
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.UInt64);
            var uut = Uut(name);
            IDaoInputOutputParameter<ulong?> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(value.Length, p?.InputParamArraySize);
        }

        [TestMethod]
        public void InputOutputValuesUlongNullableNull()
        {
            const int size = 8767;
            const string name = "Input,ulong?";
            ulong?[] value = null;
            var mockParam = MockParameter(name, ParameterDirection.InputOutput, size, DbType.UInt64);
            var uut = Uut(name);
            IDaoInputOutputParameter<ulong?> builtParam = uut.Size(size).InputOutput.Values(value);
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
            IDaoParameterInternal p = builtParam as IDaoParameterInternal;
            Assert.AreEqual(0, p?.InputParamArraySize);
        }
        #endregion
        #region Output Params Test
        [TestMethod]
        public void Output()
        {
            var uut = Uut();
            IDaoParametersBuilderOutput output = uut.Output;
            Assert.AreEqual(ParameterDirection.Output, uut.Direction);
            uut.CheckParametersCreated(0);
            Assert.AreEqual(output, uut);
        }
        [TestMethod]
        public void OutputAsByteParameter()
        {
            const int size = 8003;
            const string name = "Input,AsByteParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.Byte);
            IDaoOutputParameter<byte> builtParam = uut.Size(size).Output.AsByteParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsBinaryParameter()
        {
            const int size = 4204;
            const string name = "Input,AsBinaryParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.Binary);
            IDaoOutputParameter<byte[]> builtParam = uut.Size(size).Output.AsBinaryParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsByteNullableParameter()
        {
            const int size = 9670;
            const string name = "Input,AsByteNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.Byte);
            IDaoOutputParameter<byte?> builtParam = uut.Size(size).Output.AsByteNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsDateTimeParameter()
        {
            const int size = 2836;
            const string name = "Input,AsDateTimeParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.DateTime);
            IDaoOutputParameter<DateTime> builtParam = uut.Size(size).Output.AsDateTimeParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsDateParameter()
        {
            const int size = 2187;
            const string name = "Input,AsDateParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.Date);
            IDaoOutputParameter<DateTime> builtParam = uut.Size(size).Output.AsDateParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsTimeParameter()
        {
            const int size = 9148;
            const string name = "Input,AsTimeParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.Time);
            IDaoOutputParameter<DateTime> builtParam = uut.Size(size).Output.AsTimeParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsDateTimeNullableParameter()
        {
            const int size = 8287;
            const string name = "Input,AsDateTimeNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.DateTime);
            IDaoOutputParameter<DateTime?> builtParam = uut.Size(size).Output.AsDateTimeNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsDateNullableParameter()
        {
            const int size = 2085;
            const string name = "Input,AsDateNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.Date);
            IDaoOutputParameter<DateTime?> builtParam = uut.Size(size).Output.AsDateNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsTimeNullableParameter()
        {
            const int size = 5108;
            const string name = "Input,AsTimeNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.Time);
            IDaoOutputParameter<DateTime?> builtParam = uut.Size(size).Output.AsTimeNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsDecimalParameter()
        {
            const int size = 4717;
            const string name = "Input,AsDecimalParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.Decimal);
            IDaoOutputParameter<decimal> builtParam = uut.Size(size).Output.AsDecimalParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsDecimalNullableParameter()
        {
            const int size = 6798;
            const string name = "Input,AsDecimalNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.Decimal);
            IDaoOutputParameter<decimal?> builtParam = uut.Size(size).Output.AsDecimalNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsDoubleParameter()
        {
            const int size = 8230;
            const string name = "Input,AsDoubleParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.Double);
            IDaoOutputParameter<double> builtParam = uut.Size(size).Output.AsDoubleParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsDoubleNullableParameter()
        {
            const int size = 1868;
            const string name = "Input,AsDoubleNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.Double);
            IDaoOutputParameter<double?> builtParam = uut.Size(size).Output.AsDoubleNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsInt16Parameter()
        {
            const int size = 2592;
            const string name = "Input,AsInt16Parameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.Int16);
            IDaoOutputParameter<short> builtParam = uut.Size(size).Output.AsInt16Parameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsInt16NullableParameter()
        {
            const int size = 3624;
            const string name = "Input,AsInt16NullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.Int16);
            IDaoOutputParameter<short?> builtParam = uut.Size(size).Output.AsInt16NullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsInt32Parameter()
        {
            const int size = 9580;
            const string name = "Input,AsInt32Parameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.Int32);
            IDaoOutputParameter<int> builtParam = uut.Size(size).Output.AsInt32Parameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsInt32NullableParameter()
        {
            const int size = 1473;
            const string name = "Input,AsInt32NullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.Int32);
            IDaoOutputParameter<int?> builtParam = uut.Size(size).Output.AsInt32NullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsIntParameter()
        {
            const int size = 5381;
            const string name = "Input,AsIntParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.Int32);
            IDaoOutputParameter<int> builtParam = uut.Size(size).Output.AsIntParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsIntNullableParameter()
        {
            const int size = 4991;
            const string name = "Input,AsIntNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.Int32);
            IDaoOutputParameter<int?> builtParam = uut.Size(size).Output.AsIntNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsInt64Parameter()
        {
            const int size = 859;
            const string name = "Input,AsInt64Parameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.Int64);
            IDaoOutputParameter<long> builtParam = uut.Size(size).Output.AsInt64Parameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsInt64NullableParameter()
        {
            const int size = 9173;
            const string name = "Input,AsInt64NullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.Int64);
            IDaoOutputParameter<long?> builtParam = uut.Size(size).Output.AsInt64NullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsLongParameter()
        {
            const int size = 7682;
            const string name = "Input,AsLongParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.Int64);
            IDaoOutputParameter<long> builtParam = uut.Size(size).Output.AsLongParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsLongNullableParameter()
        {
            const int size = 6738;
            const string name = "Input,AsLongNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.Int64);
            IDaoOutputParameter<long?> builtParam = uut.Size(size).Output.AsLongNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsSByteParameter()
        {
            const int size = 2821;
            const string name = "Input,AsSByteParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.SByte);
            IDaoOutputParameter<sbyte> builtParam = uut.Size(size).Output.AsSByteParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsSByteNullableParameter()
        {
            const int size = 1988;
            const string name = "Input,AsSByteNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.SByte);
            IDaoOutputParameter<sbyte?> builtParam = uut.Size(size).Output.AsSByteNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsSingleParameter()
        {
            const int size = 2806;
            const string name = "Input,AsSingleParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.Single);
            IDaoOutputParameter<float> builtParam = uut.Size(size).Output.AsSingleParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsSingleNullableParameter()
        {
            const int size = 3207;
            const string name = "Input,AsSingleNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.Single);
            IDaoOutputParameter<float?> builtParam = uut.Size(size).Output.AsSingleNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsStringParameter()
        {
            const int size = 7450;
            const string name = "Input,AsStringParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.String);
            IDaoOutputParameter<string> builtParam = uut.Size(size).Output.AsStringParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsAnsiStringParameter()
        {
            const int size = 9787;
            const string name = "Input,AsAnsiStringParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.AnsiString);
            IDaoOutputParameter<string> builtParam = uut.Size(size).Output.AsAnsiStringParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsUInt16Parameter()
        {
            const int size = 5150;
            const string name = "Input,AsUInt16Parameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.UInt16);
            IDaoOutputParameter<ushort> builtParam = uut.Size(size).Output.AsUInt16Parameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsUInt16NullableParameter()
        {
            const int size = 6222;
            const string name = "Input,AsUInt16NullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.UInt16);
            IDaoOutputParameter<ushort?> builtParam = uut.Size(size).Output.AsUInt16NullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsUInt32Parameter()
        {
            const int size = 9679;
            const string name = "Input,AsUInt32Parameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.UInt32);
            IDaoOutputParameter<uint> builtParam = uut.Size(size).Output.AsUInt32Parameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsUInt32NullableParameter()
        {
            const int size = 4417;
            const string name = "Input,AsUInt32NullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.UInt32);
            IDaoOutputParameter<uint?> builtParam = uut.Size(size).Output.AsUInt32NullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsUInt64Parameter()
        {
            const int size = 5113;
            const string name = "Input,AsUInt64Parameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.UInt64);
            IDaoOutputParameter<ulong> builtParam = uut.Size(size).Output.AsUInt64Parameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void OutputAsUInt64NullableParameter()
        {
            const int size = 1317;
            const string name = "Input,AsUInt64NullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.Output, size, DbType.UInt64);
            IDaoOutputParameter<ulong?> builtParam = uut.Size(size).Output.AsUInt64NullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }
        #endregion
        #region Return Value Params Test
        [TestMethod]
        public void ReturnValue()
        {
            var uut = Uut();
            IDaoParametersBuilderOutput returnBuilder = uut.ReturnValue;
            Assert.AreEqual(ParameterDirection.ReturnValue, uut.Direction);
            uut.CheckParametersCreated(0);
            Assert.AreEqual(returnBuilder, uut);
        }

        [TestMethod]
        public void ReturnValueAsByteParameter()
        {
            const int size = 9875;
            const string name = "Input,AsByteParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.Byte);
            IDaoOutputParameter<byte> builtParam = uut.Size(size).ReturnValue.AsByteParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsBinaryParameter()
        {
            const int size = 4100;
            const string name = "Input,AsBinaryParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.Binary);
            IDaoOutputParameter<byte[]> builtParam = uut.Size(size).ReturnValue.AsBinaryParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsByteNullableParameter()
        {
            const int size = 3757;
            const string name = "Input,AsByteNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.Byte);
            IDaoOutputParameter<byte?> builtParam = uut.Size(size).ReturnValue.AsByteNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsDateTimeParameter()
        {
            const int size = 8912;
            const string name = "Input,AsDateTimeParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.DateTime);
            IDaoOutputParameter<DateTime> builtParam = uut.Size(size).ReturnValue.AsDateTimeParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsDateParameter()
        {
            const int size = 2719;
            const string name = "Input,AsDateParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.Date);
            IDaoOutputParameter<DateTime> builtParam = uut.Size(size).ReturnValue.AsDateParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsTimeParameter()
        {
            const int size = 1033;
            const string name = "Input,AsTimeParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.Time);
            IDaoOutputParameter<DateTime> builtParam = uut.Size(size).ReturnValue.AsTimeParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsDateTimeNullableParameter()
        {
            const int size = 6914;
            const string name = "Input,AsDateTimeNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.DateTime);
            IDaoOutputParameter<DateTime?> builtParam = uut.Size(size).ReturnValue.AsDateTimeNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsDateNullableParameter()
        {
            const int size = 7269;
            const string name = "Input,AsDateNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.Date);
            IDaoOutputParameter<DateTime?> builtParam = uut.Size(size).ReturnValue.AsDateNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsTimeNullableParameter()
        {
            const int size = 5994;
            const string name = "Input,AsTimeNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.Time);
            IDaoOutputParameter<DateTime?> builtParam = uut.Size(size).ReturnValue.AsTimeNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsDecimalParameter()
        {
            const int size = 2697;
            const string name = "Input,AsDecimalParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.Decimal);
            IDaoOutputParameter<decimal> builtParam = uut.Size(size).ReturnValue.AsDecimalParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsDecimalNullableParameter()
        {
            const int size = 2862;
            const string name = "Input,AsDecimalNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.Decimal);
            IDaoOutputParameter<decimal?> builtParam = uut.Size(size).ReturnValue.AsDecimalNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsDoubleParameter()
        {
            const int size = 4376;
            const string name = "Input,AsDoubleParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.Double);
            IDaoOutputParameter<double> builtParam = uut.Size(size).ReturnValue.AsDoubleParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsDoubleNullableParameter()
        {
            const int size = 1250;
            const string name = "Input,AsDoubleNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.Double);
            IDaoOutputParameter<double?> builtParam = uut.Size(size).ReturnValue.AsDoubleNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsInt16Parameter()
        {
            const int size = 3831;
            const string name = "Input,AsInt16Parameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.Int16);
            IDaoOutputParameter<short> builtParam = uut.Size(size).ReturnValue.AsInt16Parameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsInt16NullableParameter()
        {
            const int size = 2824;
            const string name = "Input,AsInt16NullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.Int16);
            IDaoOutputParameter<short?> builtParam = uut.Size(size).ReturnValue.AsInt16NullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsInt32Parameter()
        {
            const int size = 4593;
            const string name = "Input,AsInt32Parameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.Int32);
            IDaoOutputParameter<int> builtParam = uut.Size(size).ReturnValue.AsInt32Parameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsInt32NullableParameter()
        {
            const int size = 3234;
            const string name = "Input,AsInt32NullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.Int32);
            IDaoOutputParameter<int?> builtParam = uut.Size(size).ReturnValue.AsInt32NullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsIntParameter()
        {
            const int size = 1818;
            const string name = "Input,AsIntParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.Int32);
            IDaoOutputParameter<int> builtParam = uut.Size(size).ReturnValue.AsIntParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsIntNullableParameter()
        {
            const int size = 9149;
            const string name = "Input,AsIntNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.Int32);
            IDaoOutputParameter<int?> builtParam = uut.Size(size).ReturnValue.AsIntNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsInt64Parameter()
        {
            const int size = 3732;
            const string name = "Input,AsInt64Parameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.Int64);
            IDaoOutputParameter<long> builtParam = uut.Size(size).ReturnValue.AsInt64Parameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsInt64NullableParameter()
        {
            const int size = 5957;
            const string name = "Input,AsInt64NullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.Int64);
            IDaoOutputParameter<long?> builtParam = uut.Size(size).ReturnValue.AsInt64NullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsLongParameter()
        {
            const int size = 6377;
            const string name = "Input,AsLongParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.Int64);
            IDaoOutputParameter<long> builtParam = uut.Size(size).ReturnValue.AsLongParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsLongNullableParameter()
        {
            const int size = 8393;
            const string name = "Input,AsLongNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.Int64);
            IDaoOutputParameter<long?> builtParam = uut.Size(size).ReturnValue.AsLongNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsSByteParameter()
        {
            const int size = 4719;
            const string name = "Input,AsSByteParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.SByte);
            IDaoOutputParameter<sbyte> builtParam = uut.Size(size).ReturnValue.AsSByteParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsSByteNullableParameter()
        {
            const int size = 2572;
            const string name = "Input,AsSByteNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.SByte);
            IDaoOutputParameter<sbyte?> builtParam = uut.Size(size).ReturnValue.AsSByteNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsSingleParameter()
        {
            const int size = 4311;
            const string name = "Input,AsSingleParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.Single);
            IDaoOutputParameter<float> builtParam = uut.Size(size).ReturnValue.AsSingleParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsSingleNullableParameter()
        {
            const int size = 4221;
            const string name = "Input,AsSingleNullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.Single);
            IDaoOutputParameter<float?> builtParam = uut.Size(size).ReturnValue.AsSingleNullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsStringParameter()
        {
            const int size = 9506;
            const string name = "Input,AsStringParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.String);
            IDaoOutputParameter<string> builtParam = uut.Size(size).ReturnValue.AsStringParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsAnsiStringParameter()
        {
            const int size = 8014;
            const string name = "Input,AsAnsiStringParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.AnsiString);
            IDaoOutputParameter<string> builtParam = uut.Size(size).ReturnValue.AsAnsiStringParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsUInt16Parameter()
        {
            const int size = 8638;
            const string name = "Input,AsUInt16Parameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.UInt16);
            IDaoOutputParameter<ushort> builtParam = uut.Size(size).ReturnValue.AsUInt16Parameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsUInt16NullableParameter()
        {
            const int size = 2464;
            const string name = "Input,AsUInt16NullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.UInt16);
            IDaoOutputParameter<ushort?> builtParam = uut.Size(size).ReturnValue.AsUInt16NullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsUInt32Parameter()
        {
            const int size = 5953;
            const string name = "Input,AsUInt32Parameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.UInt32);
            IDaoOutputParameter<uint> builtParam = uut.Size(size).ReturnValue.AsUInt32Parameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsUInt32NullableParameter()
        {
            const int size = 9235;
            const string name = "Input,AsUInt32NullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.UInt32);
            IDaoOutputParameter<uint?> builtParam = uut.Size(size).ReturnValue.AsUInt32NullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsUInt64Parameter()
        {
            const int size = 6625;
            const string name = "Input,AsUInt64Parameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.UInt64);
            IDaoOutputParameter<ulong> builtParam = uut.Size(size).ReturnValue.AsUInt64Parameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }

        [TestMethod]
        public void ReturnValueAsUInt64NullableParameter()
        {
            const int size = 5856;
            const string name = "Input,AsUInt64NullableParameter";
            var uut = Uut(name);
            var mockParam = MockParameter(name, ParameterDirection.ReturnValue, size, DbType.UInt64);
            IDaoOutputParameter<ulong?> builtParam = uut.Size(size).ReturnValue.AsUInt64NullableParameter();
            Assert.AreSame(mockParam.Object, builtParam.Parameter);
            mockParam.Verify();
            uut.CheckParametersCreated(1);
        }
        #endregion


    }
}
