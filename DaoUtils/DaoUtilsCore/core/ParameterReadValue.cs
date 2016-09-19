using System;
using DaoUtilsCore.log;
using DaoUtils.Standard;
using DaoUtilsCore.core;

namespace DaoUtils.core
{
    internal class ParameterReadValue<T> : IReadValue<T>
    {
        private static readonly ILog StaticLog = LogManager.GetLogger(typeof(ParameterReadValue<T>));
        private ILog Log { get; }
        private readonly IDaoParameterInternal _parameter;
        public ParameterReadValue(IDaoParameterInternal parameter, ILog log = null)
        {
            _parameter = parameter;
            Log = log ?? StaticLog;
        }

        public T Read()
        {
            return Read(default(T));
        }

        public T Read(T defaultValue)
        {
            try
            {
                var val = _parameter.GetValueAsObject();
                return (val == DBNull.Value || val == null) ? defaultValue : DataTypeAdaptor.ConvertValue<T>(val) ;
            }
            catch (Exception e)
            {
                Log.Error($"Reading paramater '{_parameter?.Name ?? "<Unknown>"}' as {typeof(T).Name}", e);
                throw;
            }
        }

        public T Value => Read();
    }

    class ParameterReadValue : IReadValue
    {
        private readonly IDaoParameterInternal _parameter;

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

        public bool DbNull => _parameter.GetValueAsObject() == DBNull.Value;

        protected IReadValue<T> NewParameterReadValue<T>()
        {
            return new ParameterReadValue<T>(_parameter);
        }

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

        public IReadValue<byte> AsByteReader => _asByte = _asByte ?? NewParameterReadValue<byte>();
        public IReadValue<byte[]> AsBinaryReader => _asBinary = _asBinary ?? NewParameterReadValue<byte[]>();
        public IReadValue<byte?> AsByteNullableReader => _asByteNullable = _asByteNullable ?? NewParameterReadValue<byte?>();
        public IReadValue<DateTime> AsDateTimeReader => _asDateTime = _asDateTime ?? NewParameterReadValue<DateTime>();
        public IReadValue<DateTime> AsDateReader => _asDate = _asDate ?? NewParameterReadValue<DateTime>();
        public IReadValue<DateTime> AsTimeReader => _asTime = _asTime ?? NewParameterReadValue<DateTime>();
        public IReadValue<DateTime?> AsDateTimeNullableReader => _asDateTimeNullable = _asDateTimeNullable ?? NewParameterReadValue<DateTime?>();
        public IReadValue<DateTime?> AsDateNullableReader => _asDateNullable = _asDateNullable ?? NewParameterReadValue<DateTime?>();
        public IReadValue<DateTime?> AsTimeNullableReader => _asTimeNullable = _asTimeNullable ?? NewParameterReadValue<DateTime?>();
        public IReadValue<decimal> AsDecimalReader => _asDecimal = _asDecimal ?? NewParameterReadValue<decimal>();
        public IReadValue<decimal?> AsDecimalNullableReader => _asDecimalNullable = _asDecimalNullable ?? NewParameterReadValue<decimal?>();
        public IReadValue<double> AsDoubleReader => _asDouble = _asDouble ?? NewParameterReadValue<double>();
        public IReadValue<double?> AsDoubleNullableReader => _asDoubleNullable = _asDoubleNullable ?? NewParameterReadValue<double?>();
        public IReadValue<short> AsInt16Reader => _asInt16 = _asInt16 ?? NewParameterReadValue<short>();
        public IReadValue<short?> AsInt16NullableReader => _asInt16Nullable = _asInt16Nullable ?? NewParameterReadValue<short?>();
        public IReadValue<int> AsInt32Reader => _asInt32 = _asInt32 ?? NewParameterReadValue<int>();
        public IReadValue<int?> AsInt32NullableReader => _asInt32Nullable = _asInt32Nullable ?? NewParameterReadValue<int?>();
        public IReadValue<int> AsIntReader => AsInt32Reader;
        public IReadValue<int?> AsIntNullableReader => _asInt32Nullable = _asInt32Nullable ?? NewParameterReadValue<int?>();
        public IReadValue<long> AsInt64Reader => _asInt64 = _asInt64 ?? NewParameterReadValue<long>();
        public IReadValue<long?> AsInt64NullableReader => _asInt64Nullable = _asInt64Nullable ?? NewParameterReadValue<long?>();
        public IReadValue<long> AsLongReader => AsInt64Reader;
        public IReadValue<long?> AsLongNullableReader => AsInt64NullableReader;
        public IReadValue<sbyte> AsSByteReader => _asSByte = _asSByte ?? NewParameterReadValue<sbyte>();
        public IReadValue<sbyte?> AsSByteNullableReader => _asSByteNullable = _asSByteNullable ?? NewParameterReadValue<sbyte?>();
        public IReadValue<float> AsSingleReader => _asSingle = _asSingle ?? NewParameterReadValue<float>();
        public IReadValue<float?> AsSingleNullableReader => _asSingleNullable = _asSingleNullable ?? NewParameterReadValue<float?>();
        public IReadValue<string> AsStringReader => _asString = _asString ?? NewParameterReadValue<string>();
        public IReadValue<string> AsAnsiStringReader => _asAnsiString = _asAnsiString ?? NewParameterReadValue<string>();
        public IReadValue<ushort> AsUInt16Reader => _asUInt16 = _asUInt16 ?? NewParameterReadValue<ushort>();
        public IReadValue<ushort?> AsUInt16NullableReader => _asUInt16Nullable = _asUInt16Nullable ?? NewParameterReadValue<ushort?>();
        public IReadValue<uint> AsUInt32Reader => _asUInt32 = _asUInt32 ?? NewParameterReadValue<uint>();
        public IReadValue<uint?> AsUInt32NullableReader => _asUInt32Nullable = _asUInt32Nullable ?? NewParameterReadValue<uint?>();
        public IReadValue<ulong> AsUInt64Reader => _asUInt64 = _asUInt64 ?? NewParameterReadValue<ulong>();
        public IReadValue<ulong?> AsUInt64NullableReader => _asUInt64Nullable = _asUInt64Nullable ?? NewParameterReadValue<ulong?>();



        public ParameterReadValue(IDaoParameterInternal parameter)
        {
            _parameter = parameter;
        }

    }
}
