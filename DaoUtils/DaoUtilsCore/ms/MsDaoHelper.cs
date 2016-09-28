using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using DaoUtilsCore.log;
using DaoUtils.core;
using DaoUtils.Standard;
using DaoUtilsCore.def.ms;

namespace DaoUtilsCore.ms
{
    class MsDaoHelper: DaoHelperAbstract<IDaoStructuredDataParametersBuilderInput, IDaoParametersBuilderInputOutput, IDaoParametersBuilderOutput, IReadValue, SqlCommand>, 
        IMsDaoHelper
    {
        private static readonly ILog StaticLog = LogManager.GetLogger(typeof(MsDaoHelper));
        private ILog Log { get; }

        private readonly SqlConnection _connection;
        public MsDaoHelper(SqlConnection connection, OpenConnection openConnection, ILog log = null) 
            : base(connection, openConnection, log??StaticLog)
        {
            _connection = connection;
            Log = log ?? StaticLog;
        }

        private SqlCommand DbCommand(string commandText, bool storedProc)
        {
            try
            {
                var result = _connection.CreateCommand();
                result.CommandText = commandText;
                result.CommandType = storedProc ? CommandType.StoredProcedure : CommandType.Text;
                return result;
            }
            catch (Exception e)
            {
                Log.Error($"Error Creating command: {commandText}", e);
                throw;
            }
        }

        protected override IDaoCommand<IReadValue, SqlCommand> CreateCommand(string commandText, DaoSetupParameters<IDaoStructuredDataParametersBuilderInput, IDaoParametersBuilderInputOutput, IDaoParametersBuilderOutput> setupParameters, bool storedProc)
        {
            return  new MsDaoCommand(DbCommand(commandText, storedProc), this).SetupParameters(setupParameters);
        }

        protected override IDaoQuery<IReadValue, SqlCommand> CreateQuery(string querySql, DaoSetupParameters<IDaoStructuredDataParametersBuilderInput> setupParameters)
        {
            return new MsDaoCommand(DbCommand(querySql, false), this).SetupParameters(setupParameters);
        }
    }
}
