using System;
using System.Collections.Generic;
using System.Data;
using DaoUtilsCore.log;
using DaoUtils.Standard;
using DaoUtilsCore.core;

namespace DaoUtils.core
{
    abstract internal class DataReaderHelperAbstract<T> : IReadHelper<T> where T : IReadValue
    {
        private static readonly ILog StaticLog = LogManager.GetLogger(typeof(DataReaderHelperAbstract<T>));
        private ILog Log { get; }

        private Dictionary<string, T> _columnNames;
        private readonly IDataReader _reader;
        private Dictionary<string, T> BuildColumnNames()
        {
            var result = new Dictionary<string, T>(StringComparer.CurrentCultureIgnoreCase);
            for (var idx = _reader.FieldCount - 1; idx >= 0; idx--)
            {
                result[_reader.GetName(idx)] = CreateColumnReader(_reader, idx);
            }
            return result;
        }

        private Dictionary<string, T> ColumnNames
        {
            get { return _columnNames = _columnNames ?? BuildColumnNames(); }
        }
        protected abstract T CreateColumnReader(IDataReader reader, int columnIndex);
        protected DataReaderHelperAbstract(IDataReader reader, ILog log = null)
        {
            _reader = reader;
            Log = log ?? StaticLog;
        }

        public T Named(string name)
        {
            var key = name?.ToLower() ?? "<NULL>";
            if (!ColumnNames.ContainsKey(key))
            {
                Log.Error($"Unknown Column '{name}'");
                throw new DaoUtilsException($"Unknown Column '{name}'");
            }
            return ColumnNames[key];
        }

        public T this[string name] => Named(name);

        private Dictionary<int, T> _columnByIndex;

        T IReadHelper<T>.this[int columIndex]
        {
            get
            {
                _columnByIndex = _columnByIndex ?? new Dictionary<int, T>();
                if (_columnByIndex.ContainsKey(columIndex)) return _columnByIndex[columIndex];
                return _columnByIndex[columIndex] = CreateColumnReader(_reader, columIndex);
            }
        }
    }
         
}