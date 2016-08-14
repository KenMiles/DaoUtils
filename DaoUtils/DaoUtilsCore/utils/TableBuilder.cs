using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaoUtilsCore.core;
using DaoUtilsCore.def;
using DaoUtilsCore.ms;
using DaoUtilsCore.def.utils;

namespace DaoUtilsCore.utils
{

    internal class TableBuilderBase<T>
        where T : class 
    {
        private readonly List<IColumnHolder> _columnHolders = new List<IColumnHolder>();
        private readonly string _tableName;
        private readonly T _builderReturn;
        private bool _clearDefined;

        public TableBuilderBase(string tableName)
        {
            _tableName = tableName;
            _builderReturn = this as T;
            if (_builderReturn == null) throw  new Exception($"{GetType().FullName} does not implement {typeof(T).FullName}");
        }

        private DataTable CreateEmptyTable()
        {
            DataTable dt = new DataTable(_tableName);
            dt.Columns.AddRange(_columnHolders.Select(c => c.CreateColumn()).ToArray());
            return dt;
        }

        public DataTable[] BuildTables(int batchSize)
        {
            if (batchSize <= 0) batchSize = int.MaxValue;
            _clearDefined = true;
            var grouped = _columnHolders.GroupBy(c => c.NoValues).ToArray();
            if (grouped.Count() > 1)
            {
                var colLengths = grouped.Select(g => $"{string.Join(", ", g.Select(c => c.ColumnName))} have {g.Key} values");
                throw new Exception("Columns have different Lengths: " + string.Join(" - ", colLengths));
            }
            var result = new List<DataTable>();
            int noRows = _columnHolders.FirstOrDefault()?.NoValues ?? 0;
            for (var batchStart = 0; batchStart < noRows; batchStart += batchSize)
            {
                var dt = CreateEmptyTable();
                for (var idx = batchStart; idx < Math.Min(noRows, batchStart + batchSize) ; idx++)
                {
                    var row = idx;
                    dt.Rows.Add(_columnHolders.Select(c => c.Value(row)).ToArray());
                }
                result.Add(dt);
            }
            return result.ToArray();
        }

        public DataTable BuildTable()
        {
            return BuildTables(int.MaxValue).FirstOrDefault()??CreateEmptyTable();
        }

        private DataTableBuilderColumn<TCt> CreateColumn<TCt>(string columName)
        {
            var result = new DataTableBuilderColumn<TCt>(columName);
            _columnHolders.Add(result);
            return result;
        }

        private T CreateColumn<TCt>(string columName, IEnumerable<TCt> values)
        {
            if (_clearDefined) _columnHolders.Clear();
            _clearDefined = false;
            CreateColumn<TCt>(columName).SetValues(values);
            return _builderReturn;
        }

        public T Column(string columName, IEnumerable<bool> values)
        {
            return CreateColumn(columName, values);
        }

        public T Column(string columName, IEnumerable<byte> values)
        {
            return CreateColumn(columName, values);
        }

        public T Column(string columName, IEnumerable<char> values)
        {
            return CreateColumn(columName, values);
        }

        public T Column(string columName, IEnumerable<DateTime> values)
        {
            return CreateColumn(columName, values);
        }

        public T Column(string columName, IEnumerable<decimal> values)
        {
            return CreateColumn(columName, values);
        }

        public T Column(string columName, IEnumerable<double> values)
        {
            return CreateColumn(columName, values);
        }

        public T Column(string columName, IEnumerable<Guid> values)
        {
            return CreateColumn(columName, values);
        }

        public T Column(string columName, IEnumerable<short> values)
        {
            return CreateColumn(columName, values);
        }

        public T Column(string columName, IEnumerable<int> values)
        {
            return CreateColumn(columName, values);
        }

        public T Column(string columName, IEnumerable<long> values)
        {
            return CreateColumn(columName, values);
        }

        public T Column(string columName, IEnumerable<sbyte> values)
        {
            return CreateColumn(columName, values);
        }

        public T Column(string columName, IEnumerable<float> values)
        {
            return CreateColumn(columName, values);
        }

        public T Column(string columName, IEnumerable<string> values)
        {
            return CreateColumn(columName, values);
        }

        public T Column(string columName, IEnumerable<TimeSpan> values)
        {
            return CreateColumn(columName, values);
        }

        public T Column(string columName, IEnumerable<ushort> values)
        {
            return CreateColumn(columName, values);
        }

        public T Column(string columName, IEnumerable<uint> values)
        {
            return CreateColumn(columName, values);
        }

        public T Column(string columName, IEnumerable<ulong> values)
        {
            return CreateColumn(columName, values);
        }

        public T Column(string columName, IEnumerable<byte[]> values)
        {
            return CreateColumn(columName, values);
        }

        public IDataTableBuilderColumn<bool> BooleanColumn(string columnName)
        {
            return CreateColumn<bool>(columnName);
        }

        public IDataTableBuilderColumn<byte> ByteColumn(string columnName)
        {
            return CreateColumn<byte>(columnName);
        }

        public IDataTableBuilderColumn<char> CharColumn(string columnName)
        {
            return CreateColumn<char>(columnName);
        }

        public IDataTableBuilderColumn<DateTime> DateTimeColumn(string columnName)
        {
            return CreateColumn<DateTime>(columnName);
        }

        public IDataTableBuilderColumn<decimal> DecimalColumn(string columnName)
        {
            return CreateColumn<Decimal>(columnName);
        }

        public IDataTableBuilderColumn<double> DoubleColumn(string columnName)
        {
            return CreateColumn<double>(columnName);
        }

        public IDataTableBuilderColumn<Guid> GuidColumn(string columnName)
        {
            return CreateColumn<Guid>(columnName);
        }

        public IDataTableBuilderColumn<short> Int16Column(string columnName)
        {
            return CreateColumn<short>(columnName);
        }

        public IDataTableBuilderColumn<int> Int32Column(string columnName)
        {
            return CreateColumn<int>(columnName);
        }

        public IDataTableBuilderColumn<long> Int64Column(string columnName)
        {
            return CreateColumn<long>(columnName);
        }

        public IDataTableBuilderColumn<sbyte> SByteColumn(string columnName)
        {
            return CreateColumn<sbyte>(columnName);
        }

        public IDataTableBuilderColumn<float> SingleColumn(string columnName)
        {
            return CreateColumn<float>(columnName);
        }

        public IDataTableBuilderColumn<string> StringColumn(string columnName)
        {
            return CreateColumn<string>(columnName);
        }

        public IDataTableBuilderColumn<TimeSpan> TimeSpanColumn(string columnName)
        {
            return CreateColumn<TimeSpan>(columnName);
        }

        public IDataTableBuilderColumn<ushort> UInt16Column(string columnName)
        {
            return CreateColumn<ushort>(columnName);
        }

        public IDataTableBuilderColumn<uint> UInt32Column(string columnName)
        {
            return CreateColumn<uint>(columnName);
        }

        public IDataTableBuilderColumn<ulong> UInt64Column(string columnName)
        {
            return CreateColumn<ulong>(columnName);
        }

        public IDataTableBuilderColumn<byte[]> BinaryColumn(string columnName)
        {
            return CreateColumn<byte[]>(columnName);
        }
    }



}
