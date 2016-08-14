using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaoUtils.Standard
{
    public interface IReadValue<T>
    {
        T Read();
        T Read(T defaultValue);

        T Value { get; }
    }

    public interface IReadValue
    {
        bool DbNull { get; }

        byte AsByte { get; }
        byte[] AsBinary { get; }
        byte? AsByteNullable { get; }
        DateTime AsDateTime { get; }
        DateTime AsDate { get; }
        DateTime AsTime { get; }
        DateTime? AsDateTimeNullable { get; }
        DateTime? AsDateNullable { get; }
        DateTime? AsTimeNullable { get; }
        decimal AsDecimal { get; }
        decimal? AsDecimalNullable { get; }
        double AsDouble { get; }
        double? AsDoubleNullable { get; }
        short AsInt16 { get; }
        short? AsInt16Nullable { get; }
        short AsShort { get; }
        short? AsShortNullable { get; }
        int AsInt32 { get; }
        int? AsInt32Nullable { get; }
        int AsInt { get; }
        int? AsIntNullable { get; }
        long AsInt64 { get; }
        long? AsInt64Nullable { get; }
        long AsLong { get; }
        long? AsLongNullable { get; }
        sbyte AsSByte { get; }
        sbyte? AsSByteNullable { get; }
        float AsSingle { get; }
        float? AsSingleNullable { get; }
        string AsString { get; }
        string AsAnsiString { get; }
        ushort AsUInt16 { get; }
        ushort? AsUInt16Nullable { get; }
        ushort AsUshort{ get; }
        ushort? AsUshortNullable { get; }
        uint AsUInt32 { get; }
        uint? AsUInt32Nullable { get; }
        uint AsUint { get; }
        uint? AsUintNullable { get; }
        ulong AsUInt64 { get; }
        ulong? AsUInt64Nullable { get; }
        ulong AsUlong { get; }
        ulong? AsUlongNullable { get; }

        //TODO Do we need these?
        IReadValue<byte> AsByteReader { get; }
        IReadValue<byte[]> AsBinaryReader { get; }
        IReadValue<byte?> AsByteNullableReader { get; }
        IReadValue<DateTime> AsDateTimeReader { get; }
        IReadValue<DateTime> AsDateReader { get; }
        IReadValue<DateTime> AsTimeReader { get; }
        IReadValue<DateTime?> AsDateTimeNullableReader { get; }
        IReadValue<DateTime?> AsDateNullableReader { get; }
        IReadValue<DateTime?> AsTimeNullableReader { get; }
        IReadValue<decimal> AsDecimalReader { get; }
        IReadValue<decimal?> AsDecimalNullableReader { get; }
        IReadValue<double> AsDoubleReader { get; }
        IReadValue<double?> AsDoubleNullableReader { get; }
        IReadValue<short> AsInt16Reader { get; }
        IReadValue<short?> AsInt16NullableReader { get; }
        IReadValue<int> AsInt32Reader { get; }
        IReadValue<int?> AsInt32NullableReader { get; }
        IReadValue<int> AsIntReader { get; }
        IReadValue<int?> AsIntNullableReader { get; }
        IReadValue<long> AsInt64Reader { get; }
        IReadValue<long?> AsInt64NullableReader { get; }
        IReadValue<long> AsLongReader { get; }
        IReadValue<long?> AsLongNullableReader { get; }
        IReadValue<sbyte> AsSByteReader { get; }
        IReadValue<sbyte?> AsSByteNullableReader { get; }
        IReadValue<float> AsSingleReader { get; }
        IReadValue<float?> AsSingleNullableReader { get; }
        IReadValue<string> AsStringReader { get; }
        IReadValue<string> AsAnsiStringReader { get; }
        IReadValue<ushort> AsUInt16Reader { get; }
        IReadValue<ushort?> AsUInt16NullableReader { get; }
        IReadValue<uint> AsUInt32Reader { get; }
        IReadValue<uint?> AsUInt32NullableReader { get; }
        IReadValue<ulong> AsUInt64Reader { get; }
        IReadValue<ulong?> AsUInt64NullableReader { get; }
    }
}
