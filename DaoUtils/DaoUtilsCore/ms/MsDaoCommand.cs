using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using DaoUtils.core;
using DaoUtils.Standard;
using DaoUtilsCore.def.ms;

namespace DaoUtilsCore.ms
{
    class MsDaoCommand : DaoCommandAbstract<IDaoStructuredDataParametersBuilderInput, IDaoParametersBuilderInputOutput, IDaoParametersBuilderOutput, IReadValue, SqlCommand>
    {
        public MsDaoCommand(SqlCommand command, IDaoConnectionInfo connectionInfo)
            : base(command, connectionInfo)
        {
        }

        public override IDaoParametersBuilderDirection<IDaoStructuredDataParametersBuilderInput, IDaoParametersBuilderInputOutput, IDaoParametersBuilderOutput> Name(string parameterName)
        {
            return new DaoStructuredParameterBuilder(Command, Parameters, parameterName);
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
