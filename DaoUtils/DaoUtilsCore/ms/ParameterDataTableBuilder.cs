using System;
using System.Collections.Generic;
using System.Text;
using DaoUtilsCore.def.utils;
using DaoUtilsCore.def.ms;
using DaoUtilsCore.utils;

namespace DaoUtilsCore.ms
{
    internal class ParameterDataTableBuilder : TableBuilderBase<IParameterDataTableBuilder>, IParameterDataTableBuilder
    {
        readonly private IDaoStructuredDataParameterInternal _parameter;
        internal ParameterDataTableBuilder(string tableName, IDaoStructuredDataParameterInternal parameter) : base(tableName)
        {
            _parameter = parameter;
        }

        public void BuildAndSetAsValue()
        {
            _parameter.SetValue(BuildTable());
        }

        public void BuildAndSetAsValue(int batchSize)
        {
            _parameter.SetValues(BuildTables(batchSize));
        }

        public IParameterDataTableBuilder BuildAndAddTable()
        {
            _parameter.AddValue(BuildTable());
            return this;
        }
    }
}
