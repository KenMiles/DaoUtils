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
    class DaoStructuredParameterBuilder :
        DaoParameterBuilderBase
            <IDaoStructuredDataParametersBuilderInput, IDaoParametersBuilderInputOutput, IDaoParametersBuilderOutput, SqlCommand>,
        IDaoStructuredDataParametersBuilderInput

    {
        public DaoStructuredParameterBuilder(SqlCommand command, List<IDaoParameterInternal> parameters, string name)
            : base(command, parameters, name)
        {
        }

        private SqlParameter Parameter(SqlDbType type, string userDefinedTypeName)
        {
            var parameter = Command.CreateParameter();
            SetCommonParameterValues(parameter);
            parameter.SqlDbType = type;
            parameter.TypeName = userDefinedTypeName;
            return parameter;
        }

        protected DaoStructuredDataParameter AsStructuredRaw(string userDefinedTypeName)
        {
            return CreateParameter((name, direction, size) => new DaoStructuredDataParameter(Parameter(SqlDbType.Structured, userDefinedTypeName), name, direction, size));
        }

        IDaoStructuredDataParameterInput IDaoStructuredDataParametersBuilderInput.AsTableParameter(string userDefinedTypeName)
        {
            return AsStructuredRaw(userDefinedTypeName);
        }

        IDaoStructuredDataParameterInput IDaoStructuredDataParametersBuilderInput.Value(DataTable value, string userDefinedTypeName)
        {
            return AsStructuredRaw(userDefinedTypeName).SetValue(value);
        }

        IDaoStructuredDataParameterInput IDaoStructuredDataParametersBuilderInput.Values(IEnumerable<DataTable> values, string userDefinedTypeName)
        {
            return AsStructuredRaw(userDefinedTypeName).SetValues(values);
        }

    }
}
