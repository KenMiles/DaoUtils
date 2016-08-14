using System.Collections.Generic;
using DaoUtils.core;
using DaoUtils.Standard;

namespace DaoUtils.Standard
{


    internal class ParametersReadHelper : AbstractParametersReadHelper<IReadValue>
    {
        public ParametersReadHelper(IEnumerable<IDaoParameterInternal> parameters) : base(parameters)
        {
        }

        protected override IReadValue ParamReader(IDaoParameterInternal parameter)
        {
            return new ParameterReadValue(parameter);
        }
    }

}
