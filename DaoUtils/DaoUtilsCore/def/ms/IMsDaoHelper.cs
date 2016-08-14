using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using DaoUtils.Standard;

namespace DaoUtilsCore.def.ms
{
    public interface IMsDaoHelper : IDaoHelper<IDaoStructuredDataParametersBuilderInput, IDaoParametersBuilderInputOutput, IDaoParametersBuilderOutput, IReadValue, SqlCommand>
    {
    }
}
