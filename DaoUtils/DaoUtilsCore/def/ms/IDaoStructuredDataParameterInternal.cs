using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using DaoUtils.core;

namespace DaoUtilsCore.def.ms
{
    internal interface IDaoStructuredDataParameterInternal
    {
        void AddValue(DataTable table);
        DaoParameter<DataTable> SetValue(DataTable value);
        DaoParameter<DataTable> SetValues(IEnumerable<DataTable> value);

    }

}
