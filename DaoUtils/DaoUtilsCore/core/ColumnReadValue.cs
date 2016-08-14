using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaoUtils.Standard;
using DaoUtilsCore.core;

namespace DaoUtils.core
{
    internal class ColumnReadValue : IReadValue
    {
        private readonly IDataReader _reader;
        private readonly int _columnIndex;

        public ColumnReadValue(IDataReader reader, int columnIndex)
        {
            _reader = reader;
            _columnIndex = columnIndex;
        }

        private bool IsDbNull() { return _reader.IsDBNull(_columnIndex); }
        public bool DbNull => IsDbNull();

        protected IReadValue<T> NewColumnReadValue<T>(ReadColumnValue<T> readColumnValue)
        {
            return new ColumnReadTypedValue<T>(IsDbNull, readColumnValue);
        }

        protected IReadValue<bool> BooleanReadValue() { return NewColumnReadValue<bool>(() => _reader.GetBoolean(_columnIndex)); }
        protected IReadValue<byte> ByteReadValue() { return NewColumnReadValue<byte>(() => _reader.GetByte(_columnIndex)); }
        protected IReadValue<byte[]> BytesReadValue() { return NewColumnReadValue<byte[]>(()=> (byte[])_reader[_columnIndex]); }
        protected IReadValue<char> CharReadValue() { return NewColumnReadValue<char>(() => _reader.GetChar(_columnIndex)); }
        //protected ColumnReadValue<char[]> CharsReadValue() { return NewColumnReadValue<char[]>(()=> _reader.GetChars(_columnIndex)); }
        protected IReadValue<string> StringReadValue() { return NewColumnReadValue<string>(() => _reader.GetString(_columnIndex)); }
        protected IReadValue<object> ObjectReadValue() { return NewColumnReadValue<object>(() => _reader.GetValue(_columnIndex)); }
        protected IReadValue<DateTime> DateTimeReadValue() { return NewColumnReadValue<DateTime>(() => _reader.GetDateTime(_columnIndex)); }
        protected IReadValue<decimal> DecimalReadValue() { return NewColumnReadValue<decimal>(() => _reader.GetDecimal(_columnIndex)); }
        protected IReadValue<double> DoubleReadValue() { return NewColumnReadValue<double>(() => _reader.GetDouble(_columnIndex)); }
        protected IReadValue<float> FloatReadValue() { return NewColumnReadValue<float>(() => _reader.GetFloat(_columnIndex)); }
        protected IReadValue<Guid> GuidReadValue() { return NewColumnReadValue<Guid>(() => _reader.GetGuid(_columnIndex)); }
        protected IReadValue<short> Int16ReadValue() { return NewColumnReadValue<short>(() => _reader.GetInt16(_columnIndex)); }
        protected IReadValue<int> Int32ReadValue() { return NewColumnReadValue<int>(() => _reader.GetInt32(_columnIndex)); }
        protected IReadValue<long> Int64ReadValue() { return NewColumnReadValue<long>(() => _reader.GetInt64(_columnIndex)); }
        protected IReadValue<T> GenericReadValue<T>() { return NewColumnReadValue<T>(() => (T)_reader.GetValue(_columnIndex)); }
        protected IReadValue<ushort> UInt16ReadValue() { return NewColumnReadValue<ushort>(() => (ushort)_reader.GetInt32(_columnIndex)); }
        protected IReadValue<uint> UInt32ReadValue() { return NewColumnReadValue<uint>(() => (uint)_reader.GetInt64(_columnIndex)); }
        protected IReadValue<ulong> UInt64ReadValue() { return NewColumnReadValue<ulong>(() => Convert.ToUInt64(_reader.GetValue(_columnIndex))); }
        protected IReadValue<sbyte> SbyteReadValue() { return NewColumnReadValue<sbyte>(() => (sbyte)_reader.GetInt16(_columnIndex)); }

        protected IReadValue<bool?> BooleanNulllableReadValue() { return NewColumnReadValue<bool?>(() => _reader.GetBoolean(_columnIndex)); }
        protected IReadValue<byte?> ByteNulllableReadValue() { return NewColumnReadValue<byte?>(() => _reader.GetByte(_columnIndex)); }
        protected IReadValue<char?> CharNulllableReadValue() { return NewColumnReadValue<char?>(() => _reader.GetChar(_columnIndex)); }
        //protected ColumnReadValue<char[]?> CharsNulllableReadValue() { return NewColumnReadValue<char[]?>(()=> _reader.GetChars(_columnIndex)); }
        protected IReadValue<DateTime?> DateTimeNulllableReadValue() { return NewColumnReadValue<DateTime?>(() => _reader.GetDateTime(_columnIndex)); }
        protected IReadValue<decimal?> DecimalNulllableReadValue() { return NewColumnReadValue<decimal?>(() => _reader.GetDecimal(_columnIndex)); }
        protected IReadValue<double?> DoubleNulllableReadValue() { return NewColumnReadValue<double?>(() => _reader.GetDouble(_columnIndex)); }
        protected IReadValue<float?> FloatNulllableReadValue() { return NewColumnReadValue<float?>(() => _reader.GetFloat(_columnIndex)); }
        protected IReadValue<short?> Int16NulllableReadValue() { return NewColumnReadValue<short?>(() => _reader.GetInt16(_columnIndex)); }
        protected IReadValue<int?> Int32NulllableReadValue() { return NewColumnReadValue<int?>(() => _reader.GetInt32(_columnIndex)); }
        protected IReadValue<long?> Int64NulllableReadValue() { return NewColumnReadValue<long?>(() => _reader.GetInt64(_columnIndex)); }
        protected IReadValue<ushort?> UInt16NullableReadValue() { return NewColumnReadValue<ushort?>(() => (ushort)_reader.GetInt32(_columnIndex)); }
        protected IReadValue<uint?> UInt32NullableReadValue() { return NewColumnReadValue<uint?>(() => (uint)_reader.GetInt64(_columnIndex)); }
        protected IReadValue<ulong?> UInt64NullableReadValue() { return NewColumnReadValue<ulong?>(() => Convert.ToUInt64(_reader.GetValue(_columnIndex))); }
        protected IReadValue<sbyte?> SByteNullableReadValue() { return NewColumnReadValue<sbyte?>(() => (sbyte)_reader.GetInt16(_columnIndex)); }

        private IReadValue<bool> _asBoolean;
        private IReadValue<bool?> _asBooleanNullable;
        private IReadValue<byte> _asByte;
        private IReadValue<byte[]> _asBinary;
        private IReadValue<byte?> _asByteNullable;
        private IReadValue<DateTime> _asDateTime;
        private IReadValue<DateTime> _asDate;
        private IReadValue<DateTime> _asTime;
        private IReadValue<DateTime?> _asDateTimeNullable;
        private IReadValue<DateTime?> _asDateNullable;
        private IReadValue<DateTime?> _asTimeNullable;
        private IReadValue<decimal> _asDecimal;
        private IReadValue<decimal?> _asDecimalNullable;
        private IReadValue<double> _asDouble;
        private IReadValue<double?> _asDoubleNullable;
        private IReadValue<short> _asInt16;
        private IReadValue<short?> _asInt16Nullable;
        private IReadValue<int> _asInt32;
        private IReadValue<int?> _asInt32Nullable;
        private IReadValue<long> _asInt64;
        private IReadValue<long?> _asInt64Nullable;
        private IReadValue<sbyte> _asSByte;
        private IReadValue<sbyte?> _asSByteNullable;
        private IReadValue<float> _asSingle;
        private IReadValue<float?> _asSingleNullable;
        private IReadValue<string> _asString;
        private IReadValue<string> _asAnsiString;
        private IReadValue<ushort> _asUInt16;
        private IReadValue<ushort?> _asUInt16Nullable;
        private IReadValue<uint> _asUInt32;
        private IReadValue<uint?> _asUInt32Nullable;
        private IReadValue<ulong> _asUInt64;
        private IReadValue<ulong?> _asUInt64Nullable;

        public bool AsBoolean => AsBooleanReader.Value;
        public bool? AsBooleanNullable => AsBooleanNullableReader.Value;
        public byte AsByte => AsByteReader.Value;
        public byte[] AsBinary => AsBinaryReader.Value; 
        public byte? AsByteNullable => AsByteNullableReader.Value; 
        public DateTime AsDateTime => AsDateTimeReader.Value; 
        public DateTime AsDate => AsDateReader.Value; 
        public DateTime AsTime => AsTimeReader.Value; 
        public DateTime? AsDateTimeNullable => AsDateTimeNullableReader.Value; 
        public DateTime? AsDateNullable => AsDateNullableReader.Value; 
        public DateTime? AsTimeNullable => AsTimeNullableReader.Value; 
        public decimal AsDecimal => AsDecimalReader.Value; 
        public decimal? AsDecimalNullable => AsDecimalNullableReader.Value; 
        public double AsDouble => AsDoubleReader.Value; 
        public double? AsDoubleNullable => AsDoubleNullableReader.Value; 
        public short AsInt16 => AsInt16Reader.Value; 
        public short? AsInt16Nullable => AsInt16NullableReader.Value; 
        public short AsShort => AsInt16Reader.Value; 
        public short? AsShortNullable => AsInt16NullableReader.Value; 
        public int AsInt32 => AsInt32Reader.Value; 
        public int? AsInt32Nullable => AsInt32NullableReader.Value; 
        public int AsInt => AsIntReader.Value; 
        public int? AsIntNullable => AsIntNullableReader.Value; 
        public long AsInt64 => AsInt64Reader.Value; 
        public long? AsInt64Nullable => AsInt64NullableReader.Value; 
        public long AsLong => AsLongReader.Value; 
        public long? AsLongNullable => AsLongNullableReader.Value; 
        public sbyte AsSByte => AsSByteReader.Value; 
        public sbyte? AsSByteNullable => AsSByteNullableReader.Value; 
        public float AsSingle => AsSingleReader.Value; 
        public float? AsSingleNullable => AsSingleNullableReader.Value; 
        public string AsString => AsStringReader.Value; 
        public string AsAnsiString => AsAnsiStringReader.Value; 
        public ushort AsUInt16 => AsUInt16Reader.Value; 
        public ushort? AsUInt16Nullable => AsUInt16NullableReader.Value; 
        public ushort AsUshort => AsUInt16Reader.Value; 
        public ushort? AsUshortNullable => AsUInt16NullableReader.Value; 
        public uint AsUInt32 => AsUInt32Reader.Value; 
        public uint? AsUInt32Nullable => AsUInt32NullableReader.Value; 
        public uint AsUint => AsUInt32Reader.Value; 
        public uint? AsUintNullable => AsUInt32NullableReader.Value; 
        public ulong AsUInt64 => AsUInt64Reader.Value; 
        public ulong? AsUInt64Nullable => AsUInt64NullableReader.Value; 
        public ulong AsUlong => AsUInt64Reader.Value; 
        public ulong? AsUlongNullable => AsUInt64NullableReader.Value;

        public IReadValue<bool> AsBooleanReader { get { return _asBoolean = _asBoolean ?? BooleanReadValue(); } }
        public IReadValue<bool?> AsBooleanNullableReader { get { return _asBooleanNullable = _asBooleanNullable ?? BooleanNulllableReadValue(); } }
        public IReadValue<byte> AsByteReader { get { return _asByte = _asByte ?? ByteReadValue(); } }
        public IReadValue<byte[]> AsBinaryReader { get { return _asBinary = _asBinary ?? BytesReadValue(); } } 
        public IReadValue<byte?> AsByteNullableReader { get { return _asByteNullable = _asByteNullable ?? ByteNulllableReadValue(); } }
        public IReadValue<DateTime> AsDateTimeReader { get { return _asDateTime = _asDateTime ?? DateTimeReadValue(); } }
        public IReadValue<DateTime> AsDateReader { get { return _asDate = _asDate ?? DateTimeReadValue(); } }
        public IReadValue<DateTime> AsTimeReader { get { return _asTime = _asTime ?? DateTimeReadValue(); } }
        public IReadValue<DateTime?> AsDateTimeNullableReader { get { return _asDateTimeNullable = _asDateTimeNullable ?? DateTimeNulllableReadValue(); } }
        public IReadValue<DateTime?> AsDateNullableReader { get { return _asDateNullable = _asDateNullable ?? DateTimeNulllableReadValue(); } }
        public IReadValue<DateTime?> AsTimeNullableReader { get { return _asTimeNullable = _asTimeNullable ?? DateTimeNulllableReadValue(); } }
        public IReadValue<decimal> AsDecimalReader { get { return _asDecimal = _asDecimal ?? DecimalReadValue(); } }
        public IReadValue<decimal?> AsDecimalNullableReader { get { return _asDecimalNullable = _asDecimalNullable ?? DecimalNulllableReadValue(); } }
        public IReadValue<double> AsDoubleReader { get { return _asDouble = _asDouble ?? DoubleReadValue(); } }
        public IReadValue<double?> AsDoubleNullableReader { get { return _asDoubleNullable = _asDoubleNullable ?? DoubleNulllableReadValue(); } }
        public IReadValue<short> AsInt16Reader { get { return _asInt16 = _asInt16 ?? Int16ReadValue(); } }
        public IReadValue<short?> AsInt16NullableReader { get { return _asInt16Nullable = _asInt16Nullable ?? Int16NulllableReadValue(); } }
        public IReadValue<int> AsInt32Reader { get { return _asInt32 = _asInt32 ?? Int32ReadValue(); } }
        public IReadValue<int?> AsInt32NullableReader { get { return _asInt32Nullable = _asInt32Nullable ?? Int32NulllableReadValue(); } }
        public IReadValue<int> AsIntReader => AsInt32Reader;
        public IReadValue<int?> AsIntNullableReader => AsInt32NullableReader;
        public IReadValue<long> AsInt64Reader { get { return _asInt64 = _asInt64 ?? Int64ReadValue(); } }
        public IReadValue<long?> AsInt64NullableReader { get { return _asInt64Nullable = _asInt64Nullable ?? Int64NulllableReadValue(); } }
        public IReadValue<long> AsLongReader => AsInt64Reader;
        public IReadValue<long?> AsLongNullableReader => AsInt64NullableReader;
        public IReadValue<sbyte> AsSByteReader { get { return _asSByte = _asSByte ?? SbyteReadValue(); } }
        public IReadValue<sbyte?> AsSByteNullableReader { get { return _asSByteNullable = _asSByteNullable ?? SByteNullableReadValue(); } }
        public IReadValue<float> AsSingleReader { get { return _asSingle = _asSingle ?? FloatReadValue(); } }
        public IReadValue<float?> AsSingleNullableReader { get { return _asSingleNullable = _asSingleNullable ?? FloatNulllableReadValue(); } }
        public IReadValue<string> AsStringReader { get { return _asString = _asString ?? StringReadValue(); } }
        public IReadValue<string> AsAnsiStringReader { get { return _asAnsiString = _asAnsiString ?? StringReadValue(); } }
        public IReadValue<ushort> AsUInt16Reader { get { return _asUInt16 = _asUInt16 ?? UInt16ReadValue(); } }
        public IReadValue<ushort?> AsUInt16NullableReader { get { return _asUInt16Nullable = _asUInt16Nullable ?? UInt16NullableReadValue(); } }
        public IReadValue<uint> AsUInt32Reader { get { return _asUInt32 = _asUInt32 ?? UInt32ReadValue(); } }
        public IReadValue<uint?> AsUInt32NullableReader { get { return _asUInt32Nullable = _asUInt32Nullable ?? UInt32NullableReadValue(); } }
        public IReadValue<ulong> AsUInt64Reader { get { return _asUInt64 = _asUInt64 ?? UInt64ReadValue(); } }
        public IReadValue<ulong?> AsUInt64NullableReader { get { return _asUInt64Nullable = _asUInt64Nullable ?? UInt64NullableReadValue(); } }
    }
}
