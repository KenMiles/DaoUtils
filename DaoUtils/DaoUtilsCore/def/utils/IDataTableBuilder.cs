using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DaoUtilsCore.def.utils
{
    public interface IDataTableBuilderColumn<T>
    {
        T[] Values { get; set; }
        IDataTableBuilderColumn<T> SetValues(IEnumerable<T> values);
    }

    public interface IDataTableBuilderRoot<out T>
    {
        IDataTableBuilderColumn<bool> BooleanColumn(string columnName);
        IDataTableBuilderColumn<byte> ByteColumn(string columnName);
        IDataTableBuilderColumn<char> CharColumn(string columnName);
        IDataTableBuilderColumn<DateTime> DateTimeColumn(string columnName);
        IDataTableBuilderColumn<decimal> DecimalColumn(string columnName);
        IDataTableBuilderColumn<double> DoubleColumn(string columnName);
        IDataTableBuilderColumn<Guid> GuidColumn(string columnName);
        IDataTableBuilderColumn<short> Int16Column(string columnName);
        IDataTableBuilderColumn<int> Int32Column(string columnName);
        IDataTableBuilderColumn<long> Int64Column(string columnName);
        IDataTableBuilderColumn<sbyte> SByteColumn(string columnName);
        IDataTableBuilderColumn<float> SingleColumn(string columnName);
        IDataTableBuilderColumn<string> StringColumn(string columnName);
        IDataTableBuilderColumn<TimeSpan> TimeSpanColumn(string columnName);
        IDataTableBuilderColumn<ushort> UInt16Column(string columnName);
        IDataTableBuilderColumn<uint> UInt32Column(string columnName);
        IDataTableBuilderColumn<ulong> UInt64Column(string columnName);
        IDataTableBuilderColumn<byte[]> BinaryColumn(string columnName);

        T Column(string columnName, IEnumerable<bool> values);
        T Column(string columnName, IEnumerable<byte> values);
        T Column(string columnName, IEnumerable<char> values);
        T Column(string columnName, IEnumerable<DateTime> values);
        T Column(string columnName, IEnumerable<decimal> values);
        T Column(string columnName, IEnumerable<double> values);
        T Column(string columnName, IEnumerable<Guid> values);
        T Column(string columnName, IEnumerable<short> values);
        T Column(string columnName, IEnumerable<int> values);
        T Column(string columnName, IEnumerable<long> values);
        T Column(string columnName, IEnumerable<sbyte> values);
        T Column(string columnName, IEnumerable<float> values);
        T Column(string columnName, IEnumerable<string> values);
        T Column(string columnName, IEnumerable<TimeSpan> values);
        T Column(string columnName, IEnumerable<ushort> values);
        T Column(string columnName, IEnumerable<uint> values);
        T Column(string columnName, IEnumerable<ulong> values);
        T Column(string columnName, IEnumerable<byte[]> values);
    }

    public interface IParameterDataTableBuilder: IDataTableBuilderRoot<IParameterDataTableBuilder>
    {
        void BuildAndSetAsValue();
        void BuildAndSetAsValue(int batchSize);
        IParameterDataTableBuilder BuildAndAddTable();
    }


    public interface IDataTableBuilder : IDataTableBuilderRoot<IDataTableBuilder>
    {
        DataTable BuildTable();
    }
}
