﻿using System;
using System.Data;
using DaoUtilsCore.log;
using DaoUtils.core;
using DaoUtils.def;

namespace DaoUtils.Standard
{

    internal class DaoHelper :
        DaoHelperAbstract<IDaoParametersBuilderInput, IDaoParametersBuilderInputOutput, IDaoParametersBuilderOutput, IReadValue, IDbCommand>,
        IStandardDaoHelper {

        private static readonly ILog StaticLog = LogManager.GetLogger(typeof(DaoHelper));
        private ILog Log { get; }

        public DaoHelper(IDbConnection connection, OpenConnection openConnection, ILog log = null)
            : base(connection, openConnection, log??StaticLog)
        {
            Log = log ?? StaticLog;
        }


        private IDbCommand DbCommand(string commandText, bool storedProc)
        {
            try
            {
                var result = Connection.CreateCommand();
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

        protected override IDaoCommand<IReadValue, IDbCommand> CreateCommand(string commandText, DaoSetupParameters<IDaoParametersBuilderInput, IDaoParametersBuilderInputOutput, IDaoParametersBuilderOutput> setupParameters, bool storedProc)
        {
            return new DaoCommand(DbCommand(commandText, storedProc), this).SetupParameters(setupParameters);
        }

        protected override IDaoQuery<IReadValue, IDbCommand> CreateQuery(string querySql, DaoSetupParameters<IDaoParametersBuilderInput> setupParameters)
        {
            return new DaoCommand(DbCommand(querySql, false), this).SetupParameters(setupParameters);
        }

      }
}
