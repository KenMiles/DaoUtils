using System.Collections.Generic;
using System.Data;
using DaoUtils.core;
using DaoUtils.Standard;

namespace DaoUtils.Standard
{
    internal class DaoParameterBuilder :
        DaoParameterBuilderBase
            <IDaoParametersBuilderInput, IDaoParametersBuilderInputOutput, IDaoParametersBuilderOutput, IDbCommand>
    {
        public DaoParameterBuilder(IDbCommand command, List<IDaoParameterInternal> parameters, string name)
            : base(command, parameters, name)
        {
        }

    }
}
