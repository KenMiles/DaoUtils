using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DaoUtilsCore.def;
using DaoUtilsCore.def.utils;

namespace DaoUtilsCore.utils
{
    internal class DataTableBuilderColumn<T> : IDataTableBuilderColumn<T>, IColumnHolder
    {
        public DataTableBuilderColumn(string columnName)
        {
            ColumnName = columnName;
        }

        public string ColumnName { get; }
        public int NoValues => Values?.Length ?? 0;

        public object Value(int index)
        {
            return Values[index];
        }

        public DataColumn CreateColumn()
        {
          return new DataColumn(ColumnName, typeof(T));
        } 

        public T[] Values { get; set; }
        public IDataTableBuilderColumn<T> SetValues(IEnumerable<T> values)
        {
            Values = values?.ToArray() ?? new T[0];
            return this;
        }
    }

}
