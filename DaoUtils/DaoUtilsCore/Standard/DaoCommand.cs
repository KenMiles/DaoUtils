using System.Collections.Generic;
using System.Data;
using DaoUtils.core;
using DaoUtils.Standard;

namespace DaoUtils.Standard
{
    class DaoCommand : DaoCommandAbstract<IDaoParametersBuilderInput, IDaoParametersBuilderInputOutput, IDaoParametersBuilderOutput, IReadValue, IDbCommand>
    {
        public DaoCommand(IDbCommand command, IDaoConnectionInfo connectionInfo)
            : base(command, connectionInfo)
        {
        }

        public override IDaoParametersBuilderDirection<IDaoParametersBuilderInput, IDaoParametersBuilderInputOutput, IDaoParametersBuilderOutput> Name(string parameterName)
        {
            return new DaoParameterBuilder(Command, Parameters, parameterName);
        }

        protected override IReadHelper<IReadValue> ReadHelper(List<IDaoParameterInternal> parameters)
        {
            return new ParametersReadHelper(parameters);
        }

        protected override IReadHelper<IReadValue> ReadHelper(IDataReader dataReader)
        {
            return new DataReaderHelper(dataReader);
        }
    }
}
