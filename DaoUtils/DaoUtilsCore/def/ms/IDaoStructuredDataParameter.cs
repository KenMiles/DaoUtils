using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using DaoUtils.Standard;
using DaoUtilsCore.def;
using DaoUtilsCore.def.utils;

namespace DaoUtilsCore.def.ms
{
    public interface IDaoStructuredDataParameterInput : IDaoInputParameter<DataTable>
    {
        IParameterDataTableBuilder TableBuilder(string tableName = null);
    }
}
