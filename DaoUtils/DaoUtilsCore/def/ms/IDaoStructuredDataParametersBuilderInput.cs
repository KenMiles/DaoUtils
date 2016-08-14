using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using DaoUtils.Standard;
using DaoUtilsCore.def;

namespace DaoUtilsCore.def.ms
{
    public interface IDaoStructuredDataParametersBuilderInput : IDaoParametersBuilderInput
    {
        IDaoStructuredDataParameterInput AsTableParameter(string userDefinedTypeName);
        IDaoStructuredDataParameterInput Value(DataTable value, string userDefinedTypeName);
        IDaoStructuredDataParameterInput Values(IEnumerable<DataTable> values, string userDefinedTypeName);
    }
}
