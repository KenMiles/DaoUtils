using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaoUtils.Standard
{

    public interface IDaoParametersBuilderOutput
    {
        //IDaoOutputParameter<T> As<T>();
        IDaoOutputParameter<byte> AsByteParameter();
        IDaoOutputParameter<byte[]> AsBinaryParameter();
        IDaoOutputParameter<byte?> AsByteNullableParameter();
        IDaoOutputParameter<DateTime> AsDateTimeParameter();
        IDaoOutputParameter<DateTime> AsDateParameter();
        IDaoOutputParameter<DateTime> AsTimeParameter();
        IDaoOutputParameter<DateTime?> AsDateTimeNullableParameter();
        IDaoOutputParameter<DateTime?> AsDateNullableParameter();
        IDaoOutputParameter<DateTime?> AsTimeNullableParameter();
        IDaoOutputParameter<decimal> AsDecimalParameter();
        IDaoOutputParameter<decimal?> AsDecimalNullableParameter();
        IDaoOutputParameter<double> AsDoubleParameter();
        IDaoOutputParameter<double?> AsDoubleNullableParameter();
        IDaoOutputParameter<short> AsInt16Parameter();
        IDaoOutputParameter<short?> AsInt16NullableParameter();
        IDaoOutputParameter<int> AsInt32Parameter();
        IDaoOutputParameter<int?> AsInt32NullableParameter();
        IDaoOutputParameter<int> AsIntParameter();
        IDaoOutputParameter<int?> AsIntNullableParameter();
        IDaoOutputParameter<long> AsInt64Parameter();
        IDaoOutputParameter<long?> AsInt64NullableParameter();
        IDaoOutputParameter<long> AsLongParameter();
        IDaoOutputParameter<long?> AsLongNullableParameter();
        IDaoOutputParameter<sbyte> AsSByteParameter();
        IDaoOutputParameter<sbyte?> AsSByteNullableParameter();
        IDaoOutputParameter<float> AsSingleParameter();
        IDaoOutputParameter<float?> AsSingleNullableParameter();
        IDaoOutputParameter<string> AsStringParameter();
        IDaoOutputParameter<string> AsAnsiStringParameter();
        IDaoOutputParameter<ushort> AsUInt16Parameter();
        IDaoOutputParameter<ushort?> AsUInt16NullableParameter();
        IDaoOutputParameter<uint> AsUInt32Parameter();
        IDaoOutputParameter<uint?> AsUInt32NullableParameter();
        IDaoOutputParameter<ulong> AsUInt64Parameter();
        IDaoOutputParameter<ulong?> AsUInt64NullableParameter();
    }
}
