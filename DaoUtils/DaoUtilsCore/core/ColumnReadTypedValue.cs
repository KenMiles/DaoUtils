using System;
using System.Collections.Generic;
using System.Text;
using DaoUtils.Standard;

namespace DaoUtilsCore.core
{
    internal delegate T ReadColumnValue<out T>();
    internal delegate bool IsDbNull();
    internal class ColumnReadTypedValue<T> : IReadValue<T>
    {
        private readonly IsDbNull _isNull;
        private readonly ReadColumnValue<T> _readColumnValue;
        public ColumnReadTypedValue(IsDbNull isNull, ReadColumnValue<T> readColumnValue)
        {
            _isNull = isNull;
            _readColumnValue = readColumnValue;
        }

        public T Read(T defaultValue)
        {
            return _isNull() ? defaultValue : _readColumnValue();
        }

        public T Read()
        {
            return Read(default(T));
        }

        public T Value => Read();
    }
}
