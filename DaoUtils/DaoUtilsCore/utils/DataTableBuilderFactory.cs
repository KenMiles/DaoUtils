using System;
using System.Collections.Generic;
using System.Text;
using DaoUtilsCore.def;
using DaoUtilsCore.def.utils;
using DaoUtilsCore.ms;
using DaoUtilsCore.utils;

namespace DaoUtilsCore.utils
{
    internal class DataTableBuilder : TableBuilderBase<IDataTableBuilder>, IDataTableBuilder
    {
        internal DataTableBuilder(string tableName) : base(tableName)
        {
        }
    }

    public class DataTableBuilderFactory
    {
        private DataTableBuilderFactory()
        {
        }

        public static IDataTableBuilder TableBuilder(string tableName)
        {
            return new DataTableBuilder(tableName);
        }
    }
}
