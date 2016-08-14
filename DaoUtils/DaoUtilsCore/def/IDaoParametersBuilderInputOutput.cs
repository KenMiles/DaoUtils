using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaoUtils.Standard
{
    /* TODO rethink this
     * 
     * Could do all the As<Type>, Value, Values by Generics, e.g. IDaoInputParameter<T> As<T>(); but that would allow any class or long list of contraints 
     * and wouldn't handle using same type (e.g. DateTime, Date, & Time) - note setting by value picks one (e.g. DateTime always DateTime)
     * Could possibly use reflection to implement
     * 
     * So used some macros to generate the code as this looks best for IDE code validation
     */

    public interface IDaoParametersBuilderInputOutput
    {
        //IDaoInputOutputParameter<T> As<T>();
        IDaoInputOutputParameter<byte> AsByteParameter();
        IDaoInputOutputParameter<byte[]> AsBinaryParameter();
        IDaoInputOutputParameter<byte?> AsByteNullableParameter();
        IDaoInputOutputParameter<DateTime> AsDateTimeParameter();
        IDaoInputOutputParameter<DateTime> AsDateParameter();
        IDaoInputOutputParameter<DateTime> AsTimeParameter();
        IDaoInputOutputParameter<DateTime?> AsDateTimeNullableParameter();
        IDaoInputOutputParameter<DateTime?> AsDateNullableParameter();
        IDaoInputOutputParameter<DateTime?> AsTimeNullableParameter();
        IDaoInputOutputParameter<decimal> AsDecimalParameter();
        IDaoInputOutputParameter<decimal?> AsDecimalNullableParameter();
        IDaoInputOutputParameter<double> AsDoubleParameter();
        IDaoInputOutputParameter<double?> AsDoubleNullableParameter();
        IDaoInputOutputParameter<short> AsInt16Parameter();
        IDaoInputOutputParameter<short?> AsInt16NullableParameter();
        IDaoInputOutputParameter<int> AsInt32Parameter();
        IDaoInputOutputParameter<int?> AsInt32NullableParameter();
        IDaoInputOutputParameter<int> AsIntParameter();
        IDaoInputOutputParameter<int?> AsIntNullableParameter();
        IDaoInputOutputParameter<long> AsInt64Parameter();
        IDaoInputOutputParameter<long?> AsInt64NullableParameter();
        IDaoInputOutputParameter<long> AsLongParameter();
        IDaoInputOutputParameter<long?> AsLongNullableParameter();
        IDaoInputOutputParameter<sbyte> AsSByteParameter();
        IDaoInputOutputParameter<sbyte?> AsSByteNullableParameter();
        IDaoInputOutputParameter<float> AsSingleParameter();
        IDaoInputOutputParameter<float?> AsSingleNullableParameter();
        IDaoInputOutputParameter<string> AsStringParameter();
        IDaoInputOutputParameter<string> AsAnsiStringParameter();
        IDaoInputOutputParameter<ushort> AsUInt16Parameter();
        IDaoInputOutputParameter<ushort?> AsUInt16NullableParameter();
        IDaoInputOutputParameter<uint> AsUInt32Parameter();
        IDaoInputOutputParameter<uint?> AsUInt32NullableParameter();
        IDaoInputOutputParameter<ulong> AsUInt64Parameter();
        IDaoInputOutputParameter<ulong?> AsUInt64NullableParameter();

        IDaoInputOutputParameter<byte> Value(byte value);
        IDaoInputOutputParameter<byte[]> Value(byte[] value);
        IDaoInputOutputParameter<byte?> Value(byte? value);
        IDaoInputOutputParameter<DateTime> Value(DateTime value);
        IDaoInputOutputParameter<DateTime?> Value(DateTime? value);
        IDaoInputOutputParameter<decimal> Value(decimal value);
        IDaoInputOutputParameter<decimal?> Value(decimal? value);
        IDaoInputOutputParameter<double> Value(double value);
        IDaoInputOutputParameter<double?> Value(double? value);
        IDaoInputOutputParameter<short> Value(short value);
        IDaoInputOutputParameter<short?> Value(short? value);
        IDaoInputOutputParameter<int> Value(int value);
        IDaoInputOutputParameter<int?> Value(int? value);
        IDaoInputOutputParameter<long> Value(long value);
        IDaoInputOutputParameter<long?> Value(long? value);
        IDaoInputOutputParameter<sbyte> Value(sbyte value);
        IDaoInputOutputParameter<sbyte?> Value(sbyte? value);
        IDaoInputOutputParameter<float> Value(float value);
        IDaoInputOutputParameter<float?> Value(float? value);
        IDaoInputOutputParameter<string> Value(string value);
        IDaoInputOutputParameter<ushort> Value(ushort value);
        IDaoInputOutputParameter<ushort?> Value(ushort? value);
        IDaoInputOutputParameter<uint> Value(uint value);
        IDaoInputOutputParameter<uint?> Value(uint? value);
        IDaoInputOutputParameter<ulong> Value(ulong value);
        IDaoInputOutputParameter<ulong?> Value(ulong? value);

        IDaoInputOutputParameter<byte> Values(IEnumerable<byte> values);
        IDaoInputOutputParameter<byte[]> Values(IEnumerable<byte[]> values);
        IDaoInputOutputParameter<byte?> Values(IEnumerable<byte?> values);
        IDaoInputOutputParameter<DateTime> Values(IEnumerable<DateTime> values);
        IDaoInputOutputParameter<DateTime?> Values(IEnumerable<DateTime?> values);
        IDaoInputOutputParameter<decimal> Values(IEnumerable<decimal> values);
        IDaoInputOutputParameter<decimal?> Values(IEnumerable<decimal?> values);
        IDaoInputOutputParameter<double> Values(IEnumerable<double> values);
        IDaoInputOutputParameter<double?> Values(IEnumerable<double?> values);
        IDaoInputOutputParameter<short> Values(IEnumerable<short> values);
        IDaoInputOutputParameter<short?> Values(IEnumerable<short?> values);
        IDaoInputOutputParameter<int> Values(IEnumerable<int> values);
        IDaoInputOutputParameter<int?> Values(IEnumerable<int?> values);
        IDaoInputOutputParameter<long> Values(IEnumerable<long> values);
        IDaoInputOutputParameter<long?> Values(IEnumerable<long?> values);
        IDaoInputOutputParameter<sbyte> Values(IEnumerable<sbyte> values);
        IDaoInputOutputParameter<sbyte?> Values(IEnumerable<sbyte?> values);
        IDaoInputOutputParameter<float> Values(IEnumerable<float> values);
        IDaoInputOutputParameter<float?> Values(IEnumerable<float?> values);
        IDaoInputOutputParameter<string> Values(IEnumerable<string> values);
        IDaoInputOutputParameter<ushort> Values(IEnumerable<ushort> values);
        IDaoInputOutputParameter<ushort?> Values(IEnumerable<ushort?> values);
        IDaoInputOutputParameter<uint> Values(IEnumerable<uint> values);
        IDaoInputOutputParameter<uint?> Values(IEnumerable<uint?> values);
        IDaoInputOutputParameter<ulong> Values(IEnumerable<ulong> values);
        IDaoInputOutputParameter<ulong?> Values(IEnumerable<ulong?> values);
    }
}
