using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using DaoUtils.core;
using DaoUtils.Standard;
using DaoUtilsCore.def.ms;
using System.Text.RegularExpressions;

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

        private static readonly Regex VariableNameFinder = new Regex(@"(?<=\bDeclare[ \t\r\n]+@)\b[_a-zA-Z0-9]+\b", RegexOptions.IgnoreCase);
        override protected string[] SqlVariableNames(string sql)
        {
            return (from Match match in VariableNameFinder.Matches(RemoveCommentsAndStringsFrom(sql)) select match.Value).ToArray();
        }
    }
}
