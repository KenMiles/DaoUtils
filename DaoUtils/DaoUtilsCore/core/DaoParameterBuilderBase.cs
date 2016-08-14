using System;
using System.Collections.Generic;
using System.Data;
using DaoUtils.Standard;
using DaoUtilsCore.core;

namespace DaoUtils.core
{
    /* TODO rethink this - also see IDaoParametersBuilder<Direction>
     * 
     * Could do all the As<Type>, Value, Values by Generics, e.g. IDaoInputParameter<T> As<T>(); but that would allow any class or long list of contraints 
     * and wouldn't handle using same type for slightly different types (e.g. DateTime, Date, & Time) - note setting by value picks one (e.g. DateTime always DateTime)
     * Could possibly use reflection to implement
     * 
     * Using the Interfaces to hide methods not appropriate
     * 
     * So used some macros to generate the code as this looks best for IDE code validation
     */

    internal delegate TP CreateParameter<out TP>(string name, ParameterDirection direction, int size) where TP : IDaoParameterInternal;

    class DaoParameterBuilderBase<TI, TIO, TO, TCmd> : 
        IDaoParametersBuilderInput, 
        IDaoParametersBuilderInputOutput, 
        IDaoParametersBuilderOutput, 
        IDaoParametersBuilderDirection<TI, TIO, TO> 
        where TI: class, IDaoParametersBuilderInput
        where TIO : class, IDaoParametersBuilderInputOutput
        where TO : class, IDaoParametersBuilderOutput
        where TCmd : IDbCommand
    {
        public string Name { get; }
        public ParameterDirection Direction { get; private set; }
        public int ParamSize { get; private set; }
        protected TCmd Command { get; }
        private readonly List<IDaoParameterInternal> _parameters;

        protected DaoParameterBuilderBase(TCmd command, List<IDaoParameterInternal> parameters, string name)
        {
            Name = name;
            Command = command;
            _parameters = parameters;
            ParamSize = 0;
        }

        public IDaoParametersBuilderDirection<TI, TIO, TO> Size(int size)
        {
            ParamSize = Math.Max(0, size);
            return this;
        }

        private T ThisAs<T>()
            where T: class 
        {
            T result = this as T;
            if (result == null) throw new DaoUtilsException($"{GetType().FullName} does not implement ${typeof(T).FullName}");
            return result;
        }

        public TI Input
        {
            get
            {
                Direction = ParameterDirection.Input;
                return ThisAs<TI>();
            }
        }

        public TIO InputOutput
        {
            get
            {
                Direction = ParameterDirection.InputOutput;
                return ThisAs<TIO>();
            }
        }

        public TO Output
        {
            get
            {
                Direction = ParameterDirection.Output;
                return ThisAs<TO>();
            }
        }

        public TO ReturnValue
        {
            get
            {
                Direction = ParameterDirection.ReturnValue;
                return ThisAs<TO>();
            }
        }


        protected void SetCommonParameterValues(IDbDataParameter parameter)
        {
            parameter.ParameterName = Name;
            parameter.Direction = Direction;
            parameter.Size = ParamSize;
        }

        protected IDbDataParameter Parameter(DbType type)
        {
            var parameter = Command.CreateParameter();
            parameter.DbType = type;
            SetCommonParameterValues(parameter);
            return parameter;
        }

        private DaoParameter<TP> DoCreateParameter<TP>(DbType type)
        {
            return new DaoParameter<TP>(Parameter(type), Name, Direction, ParamSize);
        }

        protected virtual DaoParameter<TP> DaoParameter<TP>(DbType type)
        {
            return CreateParameter((name, direction, size) => DoCreateParameter<TP>(type)); 
        }

        protected TP CreateParameter<TP>(CreateParameter<TP>  createParameter)
            where TP: IDaoParameterInternal
        {
            var result = createParameter(Name, Direction, ParamSize);
            _parameters.Add(result);
            return result;
        }


        #region Raw Parameters

        protected virtual DaoParameter<byte> AsByteRaw() { return DaoParameter<byte>(DbType.Byte); }
        protected virtual DaoParameter<byte[]> AsBinaryRaw() { return DaoParameter<byte[]>(DbType.Binary); }
        protected virtual DaoParameter<byte?> AsByteNullableRaw() { return DaoParameter<byte?>(DbType.Byte); }
        protected virtual DaoParameter<DateTime> AsDateTimeRaw() { return DaoParameter<DateTime>(DbType.DateTime); }
        protected virtual DaoParameter<DateTime> AsDateRaw() { return DaoParameter<DateTime>(DbType.Date); }
        protected virtual DaoParameter<DateTime> AsTimeRaw() { return DaoParameter<DateTime>(DbType.Time); }
        protected virtual DaoParameter<DateTime?> AsDateTimeNullableRaw() { return DaoParameter<DateTime?>(DbType.DateTime); }
        protected virtual DaoParameter<DateTime?> AsDateNullableRaw() { return DaoParameter<DateTime?>(DbType.Date); }
        protected virtual DaoParameter<DateTime?> AsTimeNullableRaw() { return DaoParameter<DateTime?>(DbType.Time); }
        protected virtual DaoParameter<decimal> AsDecimalRaw() { return DaoParameter<decimal>(DbType.Decimal); }
        protected virtual DaoParameter<decimal?> AsDecimalNullableRaw() { return DaoParameter<decimal?>(DbType.Decimal); }
        protected virtual DaoParameter<double> AsDoubleRaw() { return DaoParameter<double>(DbType.Double); }
        protected virtual DaoParameter<double?> AsDoubleNullableRaw() { return DaoParameter<double?>(DbType.Double); }
        protected virtual DaoParameter<short> AsInt16Raw() { return DaoParameter<short>(DbType.Int16); }
        protected virtual DaoParameter<short?> AsInt16NullableRaw() { return DaoParameter<short?>(DbType.Int16); }
        protected virtual DaoParameter<int> AsInt32Raw() { return DaoParameter<int>(DbType.Int32); }
        protected virtual DaoParameter<int?> AsInt32NullableRaw() { return DaoParameter<int?>(DbType.Int32); }
        protected virtual DaoParameter<long> AsInt64Raw() { return DaoParameter<long>(DbType.Int64); }
        protected virtual DaoParameter<long?> AsInt64NullableRaw() { return DaoParameter<long?>(DbType.Int64); }
        protected virtual DaoParameter<sbyte> AsSByteRaw() { return DaoParameter<sbyte>(DbType.SByte); }
        protected virtual DaoParameter<sbyte?> AsSByteNullableRaw() { return DaoParameter<sbyte?>(DbType.SByte); }
        protected virtual DaoParameter<float> AsSingleRaw() { return DaoParameter<float>(DbType.Single); }
        protected virtual DaoParameter<float?> AsSingleNullableRaw() { return DaoParameter<float?>(DbType.Single); }
        protected virtual DaoParameter<string> AsStringRaw() { return DaoParameter<string>(DbType.String); }
        protected virtual DaoParameter<string> AsAnsiStringRaw() { return DaoParameter<string>(DbType.AnsiString); }
        protected virtual DaoParameter<ushort> AsUInt16Raw() { return DaoParameter<ushort>(DbType.UInt16); }
        protected virtual DaoParameter<ushort?> AsUInt16NullableRaw() { return DaoParameter<ushort?>(DbType.UInt16); }
        protected virtual DaoParameter<uint> AsUInt32Raw() { return DaoParameter<uint>(DbType.UInt32); }
        protected virtual DaoParameter<uint?> AsUInt32NullableRaw() { return DaoParameter<uint?>(DbType.UInt32); }
        protected virtual DaoParameter<ulong> AsUInt64Raw() { return DaoParameter<ulong>(DbType.UInt64); }
        protected virtual DaoParameter<ulong?> AsUInt64NullableRaw() { return DaoParameter<ulong?>(DbType.UInt64); }

        DaoParameter<int> AsIntRaw() { return AsInt32Raw(); }
        DaoParameter<int?> AsIntNullableRaw() { return AsInt32NullableRaw(); }
        DaoParameter<long> AsLongRaw() { return AsInt64Raw(); }
        DaoParameter<long?> AsLongNullableRaw() { return AsInt64NullableRaw(); }

        DaoParameter<byte> ValueRaw(byte value) { return AsByteRaw().SetValue(value); }
        DaoParameter<byte[]> ValueRaw(byte[] value) { return AsBinaryRaw().SetValue(value); }
        DaoParameter<byte?> ValueRaw(byte? value) { return AsByteNullableRaw().SetValue(value); }
        DaoParameter<DateTime> ValueRaw(DateTime value) { return AsDateTimeRaw().SetValue(value); }
        DaoParameter<DateTime?> ValueRaw(DateTime? value) { return AsDateTimeNullableRaw().SetValue(value); }
        DaoParameter<decimal> ValueRaw(decimal value) { return AsDecimalRaw().SetValue(value); }
        DaoParameter<decimal?> ValueRaw(decimal? value) { return AsDecimalNullableRaw().SetValue(value); }
        DaoParameter<double> ValueRaw(double value) { return AsDoubleRaw().SetValue(value); }
        DaoParameter<double?> ValueRaw(double? value) { return AsDoubleNullableRaw().SetValue(value); }
        DaoParameter<short> ValueRaw(short value) { return AsInt16Raw().SetValue(value); }
        DaoParameter<short?> ValueRaw(short? value) { return AsInt16NullableRaw().SetValue(value); }
        DaoParameter<int> ValueRaw(int value) { return AsInt32Raw().SetValue(value); }
        DaoParameter<int?> ValueRaw(int? value) { return AsInt32NullableRaw().SetValue(value); }
        DaoParameter<long> ValueRaw(long value) { return AsInt64Raw().SetValue(value); }
        DaoParameter<long?> ValueRaw(long? value) { return AsInt64NullableRaw().SetValue(value); }
        DaoParameter<sbyte> ValueRaw(sbyte value) { return AsSByteRaw().SetValue(value); }
        DaoParameter<sbyte?> ValueRaw(sbyte? value) { return AsSByteNullableRaw().SetValue(value); }
        DaoParameter<float> ValueRaw(float value) { return AsSingleRaw().SetValue(value); }
        DaoParameter<float?> ValueRaw(float? value) { return AsSingleNullableRaw().SetValue(value); }
        DaoParameter<string> ValueRaw(string value) { return AsStringRaw().SetValue(value); }
        DaoParameter<ushort> ValueRaw(ushort value) { return AsUInt16Raw().SetValue(value); }
        DaoParameter<ushort?> ValueRaw(ushort? value) { return AsUInt16NullableRaw().SetValue(value); }
        DaoParameter<uint> ValueRaw(uint value) { return AsUInt32Raw().SetValue(value); }
        DaoParameter<uint?> ValueRaw(uint? value) { return AsUInt32NullableRaw().SetValue(value); }
        DaoParameter<ulong> ValueRaw(ulong value) { return AsUInt64Raw().SetValue(value); }
        DaoParameter<ulong?> ValueRaw(ulong? value) { return AsUInt64NullableRaw().SetValue(value); }

        DaoParameter<byte> ValuesRaw(IEnumerable<byte> values) { return AsByteRaw().SetValues(values); }
        DaoParameter<byte[]> ValuesRaw(IEnumerable<byte[]> values) { return AsBinaryRaw().SetValues(values); }
        DaoParameter<byte?> ValuesRaw(IEnumerable<byte?> values) { return AsByteNullableRaw().SetValues(values); }
        DaoParameter<DateTime> ValuesRaw(IEnumerable<DateTime> values) { return AsDateTimeRaw().SetValues(values); }
        DaoParameter<DateTime?> ValuesRaw(IEnumerable<DateTime?> values) { return AsDateTimeNullableRaw().SetValues(values); }
        DaoParameter<decimal> ValuesRaw(IEnumerable<decimal> values) { return AsDecimalRaw().SetValues(values); }
        DaoParameter<decimal?> ValuesRaw(IEnumerable<decimal?> values) { return AsDecimalNullableRaw().SetValues(values); }
        DaoParameter<double> ValuesRaw(IEnumerable<double> values) { return AsDoubleRaw().SetValues(values); }
        DaoParameter<double?> ValuesRaw(IEnumerable<double?> values) { return AsDoubleNullableRaw().SetValues(values); }
        DaoParameter<short> ValuesRaw(IEnumerable<short> values) { return AsInt16Raw().SetValues(values); }
        DaoParameter<short?> ValuesRaw(IEnumerable<short?> values) { return AsInt16NullableRaw().SetValues(values); }
        DaoParameter<int> ValuesRaw(IEnumerable<int> values) { return AsInt32Raw().SetValues(values); }
        DaoParameter<int?> ValuesRaw(IEnumerable<int?> values) { return AsInt32NullableRaw().SetValues(values); }
        DaoParameter<long> ValuesRaw(IEnumerable<long> values) { return AsInt64Raw().SetValues(values); }
        DaoParameter<long?> ValuesRaw(IEnumerable<long?> values) { return AsInt64NullableRaw().SetValues(values); }
        DaoParameter<sbyte> ValuesRaw(IEnumerable<sbyte> values) { return AsSByteRaw().SetValues(values); }
        DaoParameter<sbyte?> ValuesRaw(IEnumerable<sbyte?> values) { return AsSByteNullableRaw().SetValues(values); }
        DaoParameter<float> ValuesRaw(IEnumerable<float> values) { return AsSingleRaw().SetValues(values); }
        DaoParameter<float?> ValuesRaw(IEnumerable<float?> values) { return AsSingleNullableRaw().SetValues(values); }
        DaoParameter<string> ValuesRaw(IEnumerable<string> values) { return AsStringRaw().SetValues(values); }
        DaoParameter<ushort> ValuesRaw(IEnumerable<ushort> values) { return AsUInt16Raw().SetValues(values); }
        DaoParameter<ushort?> ValuesRaw(IEnumerable<ushort?> values) { return AsUInt16NullableRaw().SetValues(values); }
        DaoParameter<uint> ValuesRaw(IEnumerable<uint> values) { return AsUInt32Raw().SetValues(values); }
        DaoParameter<uint?> ValuesRaw(IEnumerable<uint?> values) { return AsUInt32NullableRaw().SetValues(values); }
        DaoParameter<ulong> ValuesRaw(IEnumerable<ulong> values) { return AsUInt64Raw().SetValues(values); }
        DaoParameter<ulong?> ValuesRaw(IEnumerable<ulong?> values) { return AsUInt64NullableRaw().SetValues(values); }
#endregion

#region Input Parameters

        IDaoInputParameter<byte> IDaoParametersBuilderInput.AsByteParameter() { return AsByteRaw(); }
        IDaoInputParameter<byte[]> IDaoParametersBuilderInput.AsBinaryParameter() { return AsBinaryRaw(); }
        IDaoInputParameter<byte?> IDaoParametersBuilderInput.AsByteNullableParameter() { return AsByteNullableRaw(); }
        IDaoInputParameter<DateTime> IDaoParametersBuilderInput.AsDateTimeParameter() { return AsDateTimeRaw(); }
        IDaoInputParameter<DateTime> IDaoParametersBuilderInput.AsDateReaderParameter() { return AsDateRaw(); }
        IDaoInputParameter<DateTime> IDaoParametersBuilderInput.AsTimeReaderParameter() { return AsTimeRaw(); }
        IDaoInputParameter<DateTime?> IDaoParametersBuilderInput.AsDateTimeNullableParameter() { return AsDateTimeNullableRaw(); }
        IDaoInputParameter<DateTime?> IDaoParametersBuilderInput.AsDateNullableParameter() { return AsDateNullableRaw(); }
        IDaoInputParameter<DateTime?> IDaoParametersBuilderInput.AsTimeNullableParameter() { return AsTimeNullableRaw(); }
        IDaoInputParameter<decimal> IDaoParametersBuilderInput.AsDecimalParameter() { return AsDecimalRaw(); }
        IDaoInputParameter<decimal?> IDaoParametersBuilderInput.AsDecimalNullableParameter() { return AsDecimalNullableRaw(); }
        IDaoInputParameter<double> IDaoParametersBuilderInput.AsDoubleParameter() { return AsDoubleRaw(); }
        IDaoInputParameter<double?> IDaoParametersBuilderInput.AsDoubleNullableParameter() { return AsDoubleNullableRaw(); }
        IDaoInputParameter<short> IDaoParametersBuilderInput.AsInt16Parameter() { return AsInt16Raw(); }
        IDaoInputParameter<short?> IDaoParametersBuilderInput.AsInt16NullableParameter() { return AsInt16NullableRaw(); }
        IDaoInputParameter<short> IDaoParametersBuilderInput.AsShortParameter() { return AsInt16Raw(); }
        IDaoInputParameter<short?> IDaoParametersBuilderInput.AsShortNullableParameter() { return AsInt16NullableRaw(); }
        IDaoInputParameter<int> IDaoParametersBuilderInput.AsInt32Parameter() { return AsInt32Raw(); }
        IDaoInputParameter<int?> IDaoParametersBuilderInput.AsInt32NullableParameter() { return AsInt32NullableRaw(); }
        IDaoInputParameter<int> IDaoParametersBuilderInput.AsIntParameter() { return AsIntRaw(); }
        IDaoInputParameter<int?> IDaoParametersBuilderInput.AsIntNullableParameter() { return AsIntNullableRaw(); }
        IDaoInputParameter<long> IDaoParametersBuilderInput.AsInt64Parameter() { return AsInt64Raw(); }
        IDaoInputParameter<long?> IDaoParametersBuilderInput.AsInt64NullableParameter() { return AsInt64NullableRaw(); }
        IDaoInputParameter<long> IDaoParametersBuilderInput.AsLongParameter() { return AsLongRaw(); }
        IDaoInputParameter<long?> IDaoParametersBuilderInput.AsLongNullableParameter() { return AsLongNullableRaw(); }
        IDaoInputParameter<sbyte> IDaoParametersBuilderInput.AsSByteParameter() { return AsSByteRaw(); }
        IDaoInputParameter<sbyte?> IDaoParametersBuilderInput.AsSByteNullableParameter() { return AsSByteNullableRaw(); }
        IDaoInputParameter<float> IDaoParametersBuilderInput.AsSingleParameter() { return AsSingleRaw(); }
        IDaoInputParameter<float?> IDaoParametersBuilderInput.AsSingleNullableParameter() { return AsSingleNullableRaw(); }
        IDaoInputParameter<string> IDaoParametersBuilderInput.AsStringParameter() { return AsStringRaw(); }
        IDaoInputParameter<string> IDaoParametersBuilderInput.AsAnsiStringParameter() { return AsAnsiStringRaw(); }
        IDaoInputParameter<ushort> IDaoParametersBuilderInput.AsUInt16Parameter() { return AsUInt16Raw(); }
        IDaoInputParameter<ushort?> IDaoParametersBuilderInput.AsUInt16NullableParameter() { return AsUInt16NullableRaw(); }
        IDaoInputParameter<ushort> IDaoParametersBuilderInput.AsUshortParameter() { return AsUInt16Raw(); }
        IDaoInputParameter<ushort?> IDaoParametersBuilderInput.AsUshortNullableParameter() { return AsUInt16NullableRaw(); }
        IDaoInputParameter<uint> IDaoParametersBuilderInput.AsUInt32Parameter() { return AsUInt32Raw(); }
        IDaoInputParameter<uint?> IDaoParametersBuilderInput.AsUInt32NullableParameter() { return AsUInt32NullableRaw(); }
        IDaoInputParameter<uint> IDaoParametersBuilderInput.AsUintParameter() { return AsUInt32Raw(); }
        IDaoInputParameter<uint?> IDaoParametersBuilderInput.AsUintNullableParameter() { return AsUInt32NullableRaw(); }
        IDaoInputParameter<ulong> IDaoParametersBuilderInput.AsUInt64Parameter() { return AsUInt64Raw(); }
        IDaoInputParameter<ulong?> IDaoParametersBuilderInput.AsUInt64NullableParameter() { return AsUInt64NullableRaw(); }
        IDaoInputParameter<ulong> IDaoParametersBuilderInput.AsUlongParameter() { return AsUInt64Raw(); }
        IDaoInputParameter<ulong?> IDaoParametersBuilderInput.AsUlongNullableParameter() { return AsUInt64NullableRaw(); }

        IDaoInputParameter<byte> IDaoParametersBuilderInput.Value(byte value) { return ValueRaw(value); }
        IDaoInputParameter<byte[]> IDaoParametersBuilderInput.Value(byte[] value) { return ValueRaw(value); }
        IDaoInputParameter<byte?> IDaoParametersBuilderInput.Value(byte? value) { return ValueRaw(value); }
        IDaoInputParameter<DateTime> IDaoParametersBuilderInput.Value(DateTime value) { return ValueRaw(value); }
        IDaoInputParameter<DateTime?> IDaoParametersBuilderInput.Value(DateTime? value) { return ValueRaw(value); }
        IDaoInputParameter<decimal> IDaoParametersBuilderInput.Value(decimal value) { return ValueRaw(value); }
        IDaoInputParameter<decimal?> IDaoParametersBuilderInput.Value(decimal? value) { return ValueRaw(value); }
        IDaoInputParameter<double> IDaoParametersBuilderInput.Value(double value) { return ValueRaw(value); }
        IDaoInputParameter<double?> IDaoParametersBuilderInput.Value(double? value) { return ValueRaw(value); }
        IDaoInputParameter<short> IDaoParametersBuilderInput.Value(short value) { return ValueRaw(value); }
        IDaoInputParameter<short?> IDaoParametersBuilderInput.Value(short? value) { return ValueRaw(value); }
        IDaoInputParameter<int> IDaoParametersBuilderInput.Value(int value) { return ValueRaw(value); }
        IDaoInputParameter<int?> IDaoParametersBuilderInput.Value(int? value) { return ValueRaw(value); }
        IDaoInputParameter<long> IDaoParametersBuilderInput.Value(long value) { return ValueRaw(value); }
        IDaoInputParameter<long?> IDaoParametersBuilderInput.Value(long? value) { return ValueRaw(value); }
        IDaoInputParameter<sbyte> IDaoParametersBuilderInput.Value(sbyte value) { return ValueRaw(value); }
        IDaoInputParameter<sbyte?> IDaoParametersBuilderInput.Value(sbyte? value) { return ValueRaw(value); }
        IDaoInputParameter<float> IDaoParametersBuilderInput.Value(float value) { return ValueRaw(value); }
        IDaoInputParameter<float?> IDaoParametersBuilderInput.Value(float? value) { return ValueRaw(value); }
        IDaoInputParameter<string> IDaoParametersBuilderInput.Value(string value) { return ValueRaw(value); }
        IDaoInputParameter<ushort> IDaoParametersBuilderInput.Value(ushort value) { return ValueRaw(value); }
        IDaoInputParameter<ushort?> IDaoParametersBuilderInput.Value(ushort? value) { return ValueRaw(value); }
        IDaoInputParameter<uint> IDaoParametersBuilderInput.Value(uint value) { return ValueRaw(value); }
        IDaoInputParameter<uint?> IDaoParametersBuilderInput.Value(uint? value) { return ValueRaw(value); }
        IDaoInputParameter<ulong> IDaoParametersBuilderInput.Value(ulong value) { return ValueRaw(value); }
        IDaoInputParameter<ulong?> IDaoParametersBuilderInput.Value(ulong? value) { return ValueRaw(value); }

        IDaoInputParameter<byte> IDaoParametersBuilderInput.Values(IEnumerable<byte> values) { return ValuesRaw(values); }
        IDaoInputParameter<byte[]> IDaoParametersBuilderInput.Values(IEnumerable<byte[]> values) { return ValuesRaw(values); }
        IDaoInputParameter<byte?> IDaoParametersBuilderInput.Values(IEnumerable<byte?> values) { return ValuesRaw(values); }
        IDaoInputParameter<DateTime> IDaoParametersBuilderInput.Values(IEnumerable<DateTime> values) { return ValuesRaw(values); }
        IDaoInputParameter<DateTime?> IDaoParametersBuilderInput.Values(IEnumerable<DateTime?> values) { return ValuesRaw(values); }
        IDaoInputParameter<decimal> IDaoParametersBuilderInput.Values(IEnumerable<decimal> values) { return ValuesRaw(values); }
        IDaoInputParameter<decimal?> IDaoParametersBuilderInput.Values(IEnumerable<decimal?> values) { return ValuesRaw(values); }
        IDaoInputParameter<double> IDaoParametersBuilderInput.Values(IEnumerable<double> values) { return ValuesRaw(values); }
        IDaoInputParameter<double?> IDaoParametersBuilderInput.Values(IEnumerable<double?> values) { return ValuesRaw(values); }
        IDaoInputParameter<short> IDaoParametersBuilderInput.Values(IEnumerable<short> values) { return ValuesRaw(values); }
        IDaoInputParameter<short?> IDaoParametersBuilderInput.Values(IEnumerable<short?> values) { return ValuesRaw(values); }
        IDaoInputParameter<int> IDaoParametersBuilderInput.Values(IEnumerable<int> values) { return ValuesRaw(values); }
        IDaoInputParameter<int?> IDaoParametersBuilderInput.Values(IEnumerable<int?> values) { return ValuesRaw(values); }
        IDaoInputParameter<long> IDaoParametersBuilderInput.Values(IEnumerable<long> values) { return ValuesRaw(values); }
        IDaoInputParameter<long?> IDaoParametersBuilderInput.Values(IEnumerable<long?> values) { return ValuesRaw(values); }
        IDaoInputParameter<sbyte> IDaoParametersBuilderInput.Values(IEnumerable<sbyte> values) { return ValuesRaw(values); }
        IDaoInputParameter<sbyte?> IDaoParametersBuilderInput.Values(IEnumerable<sbyte?> values) { return ValuesRaw(values); }
        IDaoInputParameter<float> IDaoParametersBuilderInput.Values(IEnumerable<float> values) { return ValuesRaw(values); }
        IDaoInputParameter<float?> IDaoParametersBuilderInput.Values(IEnumerable<float?> values) { return ValuesRaw(values); }
        IDaoInputParameter<string> IDaoParametersBuilderInput.Values(IEnumerable<string> values) { return ValuesRaw(values); }
        IDaoInputParameter<ushort> IDaoParametersBuilderInput.Values(IEnumerable<ushort> values) { return ValuesRaw(values); }
        IDaoInputParameter<ushort?> IDaoParametersBuilderInput.Values(IEnumerable<ushort?> values) { return ValuesRaw(values); }
        IDaoInputParameter<uint> IDaoParametersBuilderInput.Values(IEnumerable<uint> values) { return ValuesRaw(values); }
        IDaoInputParameter<uint?> IDaoParametersBuilderInput.Values(IEnumerable<uint?> values) { return ValuesRaw(values); }
        IDaoInputParameter<ulong> IDaoParametersBuilderInput.Values(IEnumerable<ulong> values) { return ValuesRaw(values); }
        IDaoInputParameter<ulong?> IDaoParametersBuilderInput.Values(IEnumerable<ulong?> values) { return ValuesRaw(values); }
#endregion

#region InputOutput Parameters
        IDaoInputOutputParameter<byte> IDaoParametersBuilderInputOutput.AsByteParameter() { return AsByteRaw(); }
        IDaoInputOutputParameter<byte[]> IDaoParametersBuilderInputOutput.AsBinaryParameter() { return AsBinaryRaw(); }
        IDaoInputOutputParameter<byte?> IDaoParametersBuilderInputOutput.AsByteNullableParameter() { return AsByteNullableRaw(); }
        IDaoInputOutputParameter<DateTime> IDaoParametersBuilderInputOutput.AsDateTimeParameter() { return AsDateTimeRaw(); }
        IDaoInputOutputParameter<DateTime> IDaoParametersBuilderInputOutput.AsDateParameter() { return AsDateRaw(); }
        IDaoInputOutputParameter<DateTime> IDaoParametersBuilderInputOutput.AsTimeParameter() { return AsTimeRaw(); }
        IDaoInputOutputParameter<DateTime?> IDaoParametersBuilderInputOutput.AsDateTimeNullableParameter() { return AsDateTimeNullableRaw(); }
        IDaoInputOutputParameter<DateTime?> IDaoParametersBuilderInputOutput.AsDateNullableParameter() { return AsDateNullableRaw(); }
        IDaoInputOutputParameter<DateTime?> IDaoParametersBuilderInputOutput.AsTimeNullableParameter() { return AsTimeNullableRaw(); }
        IDaoInputOutputParameter<decimal> IDaoParametersBuilderInputOutput.AsDecimalParameter() { return AsDecimalRaw(); }
        IDaoInputOutputParameter<decimal?> IDaoParametersBuilderInputOutput.AsDecimalNullableParameter() { return AsDecimalNullableRaw(); }
        IDaoInputOutputParameter<double> IDaoParametersBuilderInputOutput.AsDoubleParameter() { return AsDoubleRaw(); }
        IDaoInputOutputParameter<double?> IDaoParametersBuilderInputOutput.AsDoubleNullableParameter() { return AsDoubleNullableRaw(); }
        IDaoInputOutputParameter<short> IDaoParametersBuilderInputOutput.AsInt16Parameter() { return AsInt16Raw(); }
        IDaoInputOutputParameter<short?> IDaoParametersBuilderInputOutput.AsInt16NullableParameter() { return AsInt16NullableRaw(); }
        IDaoInputOutputParameter<int> IDaoParametersBuilderInputOutput.AsInt32Parameter() { return AsInt32Raw(); }
        IDaoInputOutputParameter<int?> IDaoParametersBuilderInputOutput.AsInt32NullableParameter() { return AsInt32NullableRaw(); }
        IDaoInputOutputParameter<int> IDaoParametersBuilderInputOutput.AsIntParameter() { return AsIntRaw(); }
        IDaoInputOutputParameter<int?> IDaoParametersBuilderInputOutput.AsIntNullableParameter() { return AsIntNullableRaw(); }
        IDaoInputOutputParameter<long> IDaoParametersBuilderInputOutput.AsInt64Parameter() { return AsInt64Raw(); }
        IDaoInputOutputParameter<long?> IDaoParametersBuilderInputOutput.AsInt64NullableParameter() { return AsInt64NullableRaw(); }
        IDaoInputOutputParameter<long> IDaoParametersBuilderInputOutput.AsLongParameter() { return AsLongRaw(); }
        IDaoInputOutputParameter<long?> IDaoParametersBuilderInputOutput.AsLongNullableParameter() { return AsLongNullableRaw(); }
        IDaoInputOutputParameter<sbyte> IDaoParametersBuilderInputOutput.AsSByteParameter() { return AsSByteRaw(); }
        IDaoInputOutputParameter<sbyte?> IDaoParametersBuilderInputOutput.AsSByteNullableParameter() { return AsSByteNullableRaw(); }
        IDaoInputOutputParameter<float> IDaoParametersBuilderInputOutput.AsSingleParameter() { return AsSingleRaw(); }
        IDaoInputOutputParameter<float?> IDaoParametersBuilderInputOutput.AsSingleNullableParameter() { return AsSingleNullableRaw(); }
        IDaoInputOutputParameter<string> IDaoParametersBuilderInputOutput.AsStringParameter() { return AsStringRaw(); }
        IDaoInputOutputParameter<string> IDaoParametersBuilderInputOutput.AsAnsiStringParameter() { return AsAnsiStringRaw(); }
        IDaoInputOutputParameter<ushort> IDaoParametersBuilderInputOutput.AsUInt16Parameter() { return AsUInt16Raw(); }
        IDaoInputOutputParameter<ushort?> IDaoParametersBuilderInputOutput.AsUInt16NullableParameter() { return AsUInt16NullableRaw(); }
        IDaoInputOutputParameter<uint> IDaoParametersBuilderInputOutput.AsUInt32Parameter() { return AsUInt32Raw(); }
        IDaoInputOutputParameter<uint?> IDaoParametersBuilderInputOutput.AsUInt32NullableParameter() { return AsUInt32NullableRaw(); }
        IDaoInputOutputParameter<ulong> IDaoParametersBuilderInputOutput.AsUInt64Parameter() { return AsUInt64Raw(); }
        IDaoInputOutputParameter<ulong?> IDaoParametersBuilderInputOutput.AsUInt64NullableParameter() { return AsUInt64NullableRaw(); }

        IDaoInputOutputParameter<byte> IDaoParametersBuilderInputOutput.Value(byte value) { return ValueRaw(value); }
        IDaoInputOutputParameter<byte[]> IDaoParametersBuilderInputOutput.Value(byte[] value) { return ValueRaw(value); }
        IDaoInputOutputParameter<byte?> IDaoParametersBuilderInputOutput.Value(byte? value) { return ValueRaw(value); }
        IDaoInputOutputParameter<DateTime> IDaoParametersBuilderInputOutput.Value(DateTime value) { return ValueRaw(value); }
        IDaoInputOutputParameter<DateTime?> IDaoParametersBuilderInputOutput.Value(DateTime? value) { return ValueRaw(value); }
        IDaoInputOutputParameter<decimal> IDaoParametersBuilderInputOutput.Value(decimal value) { return ValueRaw(value); }
        IDaoInputOutputParameter<decimal?> IDaoParametersBuilderInputOutput.Value(decimal? value) { return ValueRaw(value); }
        IDaoInputOutputParameter<double> IDaoParametersBuilderInputOutput.Value(double value) { return ValueRaw(value); }
        IDaoInputOutputParameter<double?> IDaoParametersBuilderInputOutput.Value(double? value) { return ValueRaw(value); }
        IDaoInputOutputParameter<short> IDaoParametersBuilderInputOutput.Value(short value) { return ValueRaw(value); }
        IDaoInputOutputParameter<short?> IDaoParametersBuilderInputOutput.Value(short? value) { return ValueRaw(value); }
        IDaoInputOutputParameter<int> IDaoParametersBuilderInputOutput.Value(int value) { return ValueRaw(value); }
        IDaoInputOutputParameter<int?> IDaoParametersBuilderInputOutput.Value(int? value) { return ValueRaw(value); }
        IDaoInputOutputParameter<long> IDaoParametersBuilderInputOutput.Value(long value) { return ValueRaw(value); }
        IDaoInputOutputParameter<long?> IDaoParametersBuilderInputOutput.Value(long? value) { return ValueRaw(value); }
        IDaoInputOutputParameter<sbyte> IDaoParametersBuilderInputOutput.Value(sbyte value) { return ValueRaw(value); }
        IDaoInputOutputParameter<sbyte?> IDaoParametersBuilderInputOutput.Value(sbyte? value) { return ValueRaw(value); }
        IDaoInputOutputParameter<float> IDaoParametersBuilderInputOutput.Value(float value) { return ValueRaw(value); }
        IDaoInputOutputParameter<float?> IDaoParametersBuilderInputOutput.Value(float? value) { return ValueRaw(value); }
        IDaoInputOutputParameter<string> IDaoParametersBuilderInputOutput.Value(string value) { return ValueRaw(value); }
        IDaoInputOutputParameter<ushort> IDaoParametersBuilderInputOutput.Value(ushort value) { return ValueRaw(value); }
        IDaoInputOutputParameter<ushort?> IDaoParametersBuilderInputOutput.Value(ushort? value) { return ValueRaw(value); }
        IDaoInputOutputParameter<uint> IDaoParametersBuilderInputOutput.Value(uint value) { return ValueRaw(value); }
        IDaoInputOutputParameter<uint?> IDaoParametersBuilderInputOutput.Value(uint? value) { return ValueRaw(value); }
        IDaoInputOutputParameter<ulong> IDaoParametersBuilderInputOutput.Value(ulong value) { return ValueRaw(value); }
        IDaoInputOutputParameter<ulong?> IDaoParametersBuilderInputOutput.Value(ulong? value) { return ValueRaw(value); }

        IDaoInputOutputParameter<byte> IDaoParametersBuilderInputOutput.Values(IEnumerable<byte> values) { return ValuesRaw(values); }
        IDaoInputOutputParameter<byte[]> IDaoParametersBuilderInputOutput.Values(IEnumerable<byte[]> values) { return ValuesRaw(values); }
        IDaoInputOutputParameter<byte?> IDaoParametersBuilderInputOutput.Values(IEnumerable<byte?> values) { return ValuesRaw(values); }
        IDaoInputOutputParameter<DateTime> IDaoParametersBuilderInputOutput.Values(IEnumerable<DateTime> values) { return ValuesRaw(values); }
        IDaoInputOutputParameter<DateTime?> IDaoParametersBuilderInputOutput.Values(IEnumerable<DateTime?> values) { return ValuesRaw(values); }
        IDaoInputOutputParameter<decimal> IDaoParametersBuilderInputOutput.Values(IEnumerable<decimal> values) { return ValuesRaw(values); }
        IDaoInputOutputParameter<decimal?> IDaoParametersBuilderInputOutput.Values(IEnumerable<decimal?> values) { return ValuesRaw(values); }
        IDaoInputOutputParameter<double> IDaoParametersBuilderInputOutput.Values(IEnumerable<double> values) { return ValuesRaw(values); }
        IDaoInputOutputParameter<double?> IDaoParametersBuilderInputOutput.Values(IEnumerable<double?> values) { return ValuesRaw(values); }
        IDaoInputOutputParameter<short> IDaoParametersBuilderInputOutput.Values(IEnumerable<short> values) { return ValuesRaw(values); }
        IDaoInputOutputParameter<short?> IDaoParametersBuilderInputOutput.Values(IEnumerable<short?> values) { return ValuesRaw(values); }
        IDaoInputOutputParameter<int> IDaoParametersBuilderInputOutput.Values(IEnumerable<int> values) { return ValuesRaw(values); }
        IDaoInputOutputParameter<int?> IDaoParametersBuilderInputOutput.Values(IEnumerable<int?> values) { return ValuesRaw(values); }
        IDaoInputOutputParameter<long> IDaoParametersBuilderInputOutput.Values(IEnumerable<long> values) { return ValuesRaw(values); }
        IDaoInputOutputParameter<long?> IDaoParametersBuilderInputOutput.Values(IEnumerable<long?> values) { return ValuesRaw(values); }
        IDaoInputOutputParameter<sbyte> IDaoParametersBuilderInputOutput.Values(IEnumerable<sbyte> values) { return ValuesRaw(values); }
        IDaoInputOutputParameter<sbyte?> IDaoParametersBuilderInputOutput.Values(IEnumerable<sbyte?> values) { return ValuesRaw(values); }
        IDaoInputOutputParameter<float> IDaoParametersBuilderInputOutput.Values(IEnumerable<float> values) { return ValuesRaw(values); }
        IDaoInputOutputParameter<float?> IDaoParametersBuilderInputOutput.Values(IEnumerable<float?> values) { return ValuesRaw(values); }
        IDaoInputOutputParameter<string> IDaoParametersBuilderInputOutput.Values(IEnumerable<string> values) { return ValuesRaw(values); }
        IDaoInputOutputParameter<ushort> IDaoParametersBuilderInputOutput.Values(IEnumerable<ushort> values) { return ValuesRaw(values); }
        IDaoInputOutputParameter<ushort?> IDaoParametersBuilderInputOutput.Values(IEnumerable<ushort?> values) { return ValuesRaw(values); }
        IDaoInputOutputParameter<uint> IDaoParametersBuilderInputOutput.Values(IEnumerable<uint> values) { return ValuesRaw(values); }
        IDaoInputOutputParameter<uint?> IDaoParametersBuilderInputOutput.Values(IEnumerable<uint?> values) { return ValuesRaw(values); }
        IDaoInputOutputParameter<ulong> IDaoParametersBuilderInputOutput.Values(IEnumerable<ulong> values) { return ValuesRaw(values); }
        IDaoInputOutputParameter<ulong?> IDaoParametersBuilderInputOutput.Values(IEnumerable<ulong?> values) { return ValuesRaw(values); }

#endregion

#region Output Parameters
        IDaoOutputParameter<byte> IDaoParametersBuilderOutput.AsByteParameter() { return AsByteRaw(); }
        IDaoOutputParameter<byte[]> IDaoParametersBuilderOutput.AsBinaryParameter() { return AsBinaryRaw(); }
        IDaoOutputParameter<byte?> IDaoParametersBuilderOutput.AsByteNullableParameter() { return AsByteNullableRaw(); }
        IDaoOutputParameter<DateTime> IDaoParametersBuilderOutput.AsDateTimeParameter() { return AsDateTimeRaw(); }
        IDaoOutputParameter<DateTime> IDaoParametersBuilderOutput.AsDateParameter() { return AsDateRaw(); }
        IDaoOutputParameter<DateTime> IDaoParametersBuilderOutput.AsTimeParameter() { return AsTimeRaw(); }
        IDaoOutputParameter<DateTime?> IDaoParametersBuilderOutput.AsDateTimeNullableParameter() { return AsDateTimeNullableRaw(); }
        IDaoOutputParameter<DateTime?> IDaoParametersBuilderOutput.AsDateNullableParameter() { return AsDateNullableRaw(); }
        IDaoOutputParameter<DateTime?> IDaoParametersBuilderOutput.AsTimeNullableParameter() { return AsTimeNullableRaw(); }
        IDaoOutputParameter<decimal> IDaoParametersBuilderOutput.AsDecimalParameter() { return AsDecimalRaw(); }
        IDaoOutputParameter<decimal?> IDaoParametersBuilderOutput.AsDecimalNullableParameter() { return AsDecimalNullableRaw(); }
        IDaoOutputParameter<double> IDaoParametersBuilderOutput.AsDoubleParameter() { return AsDoubleRaw(); }
        IDaoOutputParameter<double?> IDaoParametersBuilderOutput.AsDoubleNullableParameter() { return AsDoubleNullableRaw(); }
        IDaoOutputParameter<short> IDaoParametersBuilderOutput.AsInt16Parameter() { return AsInt16Raw(); }
        IDaoOutputParameter<short?> IDaoParametersBuilderOutput.AsInt16NullableParameter() { return AsInt16NullableRaw(); }
        IDaoOutputParameter<int> IDaoParametersBuilderOutput.AsInt32Parameter() { return AsInt32Raw(); }
        IDaoOutputParameter<int?> IDaoParametersBuilderOutput.AsInt32NullableParameter() { return AsInt32NullableRaw(); }
        IDaoOutputParameter<int> IDaoParametersBuilderOutput.AsIntParameter() { return AsIntRaw(); }
        IDaoOutputParameter<int?> IDaoParametersBuilderOutput.AsIntNullableParameter() { return AsIntNullableRaw(); }
        IDaoOutputParameter<long> IDaoParametersBuilderOutput.AsInt64Parameter() { return AsInt64Raw(); }
        IDaoOutputParameter<long?> IDaoParametersBuilderOutput.AsInt64NullableParameter() { return AsInt64NullableRaw(); }
        IDaoOutputParameter<long> IDaoParametersBuilderOutput.AsLongParameter() { return AsLongRaw(); }
        IDaoOutputParameter<long?> IDaoParametersBuilderOutput.AsLongNullableParameter() { return AsLongNullableRaw(); }
        IDaoOutputParameter<sbyte> IDaoParametersBuilderOutput.AsSByteParameter() { return AsSByteRaw(); }
        IDaoOutputParameter<sbyte?> IDaoParametersBuilderOutput.AsSByteNullableParameter() { return AsSByteNullableRaw(); }
        IDaoOutputParameter<float> IDaoParametersBuilderOutput.AsSingleParameter() { return AsSingleRaw(); }
        IDaoOutputParameter<float?> IDaoParametersBuilderOutput.AsSingleNullableParameter() { return AsSingleNullableRaw(); }
        IDaoOutputParameter<string> IDaoParametersBuilderOutput.AsStringParameter() { return AsStringRaw(); }
        IDaoOutputParameter<string> IDaoParametersBuilderOutput.AsAnsiStringParameter() { return AsAnsiStringRaw(); }
        IDaoOutputParameter<ushort> IDaoParametersBuilderOutput.AsUInt16Parameter() { return AsUInt16Raw(); }
        IDaoOutputParameter<ushort?> IDaoParametersBuilderOutput.AsUInt16NullableParameter() { return AsUInt16NullableRaw(); }
        IDaoOutputParameter<uint> IDaoParametersBuilderOutput.AsUInt32Parameter() { return AsUInt32Raw(); }
        IDaoOutputParameter<uint?> IDaoParametersBuilderOutput.AsUInt32NullableParameter() { return AsUInt32NullableRaw(); }
        IDaoOutputParameter<ulong> IDaoParametersBuilderOutput.AsUInt64Parameter() { return AsUInt64Raw(); }
        IDaoOutputParameter<ulong?> IDaoParametersBuilderOutput.AsUInt64NullableParameter() { return AsUInt64NullableRaw(); }
#endregion
    }

}
