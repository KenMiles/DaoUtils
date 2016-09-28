using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using DaoUtils.Standard;
using DaoUtilsCore.log;
using DaoUtilsCore.core;

namespace DaoUtils.core
{
    internal delegate string GetConnectionDescription(IDbConnection connection);
    abstract class DaoHelperAbstract<TI, TIO, TO, TR, TCmd> : IDaoConnectionInfo, IDaoHelper<TI, TIO, TO, TR, TCmd>
        where TI : IDaoParametersBuilderInput
        where TIO : IDaoParametersBuilderInputOutput
        where TO : IDaoParametersBuilderOutput
        where TR : IReadValue
        where TCmd : IDbCommand
    {
        protected readonly IDbConnection Connection;
        private Task _openTask;

        private static readonly ILog StaticLog = LogManager.GetLogger(typeof(DaoHelperAbstract<TI, TIO, TO, TR, TCmd>));
        private readonly ILog _log;

        protected virtual string GetConnectionDescription(IDbConnection connection)
        {
            if (connection == null) return "Connection is null";
            return string.IsNullOrWhiteSpace(connection.Database) ? "unknown database" : connection.Database;
        }

        private static string DoGetConnectionDescription(IDbConnection connection,
            GetConnectionDescription getConnectionDescription)
        {
            if (connection == null) return "Connection is null";
            try
            {
                return $"{getConnectionDescription(connection)} - {connection.GetType().FullName}";
            }
            catch (Exception ex)
            {
                StaticLog.Error("Looking up Connection details Threw Error", ex);
                return $"Looking up Connection Threw Exception {ex.Message}";
            }
        }

        private static void DoOpenConnection(IDbConnection connection, string openingType, GetConnectionDescription connectionDescription)
        {
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                StaticLog.Error($"Opening {openingType} connection {DoGetConnectionDescription(connection, connectionDescription)}", ex);
                throw;
            }
        }

        private static Task DoOpenConnection(IDbConnection connection, OpenConnection openConnection, GetConnectionDescription connectionDescription)
        {
            switch (openConnection)
            {
                case OpenConnection.Background:
                    return Task.Run(() =>
                    {
                        DoOpenConnection(connection, "In Background", connectionDescription);
                    });
                case OpenConnection.Immediate:
                    DoOpenConnection(connection, "Immediately", connectionDescription);
                    break;
            }
            return null;
        }

        protected DaoHelperAbstract(IDbConnection connection, OpenConnection openConnection, ILog log = null)
        {
            _log = log ?? StaticLog;
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            Connection = connection;
            _openTask = DoOpenConnection(connection, openConnection, GetConnectionDescription);
        }

        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed || !disposing) return;
            _disposed = true;
            Connection.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private readonly object _lockOpenConnection = new object();

        private void WaitOpenTask()
        {
            lock (_lockOpenConnection)
            {
                if (_openTask == null) return;
                var task = _openTask;
                _openTask = null;
                try
                {
                    task.Wait();
                }
                catch (Exception e)
                {
                    throw new DaoUtilsException("Opening Connection Threw Error", e);
                }
                if (task.IsFaulted)
                {
                    throw new DaoUtilsException("Opening Connection Threw Error", task.Exception);
                }
            }
        }

        private bool IsOpen(ConnectionState state)
        {
            switch (Connection.State)
            {
                case ConnectionState.Open:
                case ConnectionState.Executing:
                case ConnectionState.Fetching:
                    return true;
            }
            return false;
        }

        public void WaitOpen()
        {
            if (_disposed) throw new DaoUtilsException("Disposed Called - No longer able to call database");
            WaitOpenTask();
            if(IsOpen(Connection.State)) return;
            lock (_lockOpenConnection)
            {
                if (Connection.State == ConnectionState.Broken) Connection.Close();
                Connection.Open();
            }
        }

        private string _paramPrefix;
        protected virtual string GetParamPrefix()
        {
            WaitOpen();
            var connection = Connection as DbConnection;
            // Don't know - return SQL server
            if (connection == null) return "@";
            //http://stackoverflow.com/questions/6904430/get-the-parameter-prefix-in-ado-net
            string paramFormat =
                connection.GetSchema(DbMetaDataCollectionNames.DataSourceInformation).Rows[0][
                    DbMetaDataColumnNames.ParameterMarkerFormat].ToString();
            var paramPrefix = paramFormat.Replace("{0}", "");
            return string.IsNullOrWhiteSpace(paramPrefix) ? "@" : paramPrefix;
        }

        public string ParamPrefix => _paramPrefix = _paramPrefix ?? GetParamPrefix();

        protected abstract IDaoCommand<TR, TCmd> CreateCommand(string commandText, DaoSetupParameters<TI, TIO, TO>  setupParameters, bool storedProc);

        public IDaoCommand<TR, TCmd> NewCommand(string commandText, DaoSetupParameters<TI, TIO, TO> setupParameters)
        {
            WaitOpen();
            return CreateCommand(commandText, setupParameters, false);
        }

        public IDaoCommand<TR, TCmd> NewCommand(string commandText)
        {
            return NewCommand(commandText, helper => { });
        }

        public IDaoCommand<TR, TCmd> NewStoredProc(string procedureName, DaoSetupParameters<TI, TIO, TO> setupParameters)
        {
            WaitOpen();
            return CreateCommand(procedureName, setupParameters, true);
        }

        public IDaoCommand<TR, TCmd> NewStoredProc(string procedureName)
        {
            return NewStoredProc(procedureName, helper => { });
        }


        protected abstract IDaoQuery<TR, TCmd> CreateQuery(string querySql, DaoSetupParameters<TI> setupParameters);

        public IDaoQuery<TR, TCmd> NewQuery(string querySql, DaoSetupParameters<TI> setupParameters)
        {
            WaitOpen();
            return CreateQuery(querySql, setupParameters);
        }

        public IDaoQuery<TR, TCmd> NewQuery(string querySql)
        {
            return NewQuery(querySql, helper => { });
        }

        private readonly ThreadLocal<IDbTransaction> _threadTransactions = new ThreadLocal<IDbTransaction>(() => null);
        public IDbTransaction ActiveTransaction => _threadTransactions.Value;

        public void DoInTransaction(string description, Action daoInTransaction)
        {
            if (ActiveTransaction != null) throw new DaoUtilsException("Already in Transaction");
            WaitOpen();
            try
            {
                using (var trans = _threadTransactions.Value = Connection.BeginTransaction())
                    try
                    {
                        daoInTransaction();
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        _log.Error(e);
                        trans.Rollback();
                        throw new DaoUtilsException($"{description} errored with '{e.Message}'", e);
                    }
            }
            finally
            {
                _threadTransactions.Value = null;
            }

        }

        public List<T> ReadQuery<T>(IDaoQuery<TR, TCmd> command, DaoReadRow<T, TR> readRow)
        {
            return command.ReadQuery(readRow);
        }

        public List<T> ReadQuery<T>(string sql, DaoSetupParameters<TI> setupParameters, DaoReadRowAndParams<T, TR> readRow)
        {
            using (var cmd = NewQuery(sql, setupParameters))
            {
                return cmd.ReadQuery(readRow);
            }
        }

        public List<T> ReadQuery<T>(string sql, DaoReadRow<T, TR> readRow)
        {
            return ReadQuery(sql, s => { }, readRow);
        }

        public List<T> ReadQuery<T>(string sql, DaoSetupParameters<TI> setupParameters, DaoReadRow<T, TR> readRow)
        {
            using (var cmd = NewQuery(sql, setupParameters))
            {
                return ReadQuery(cmd, readRow);
            }
        }

        public Dictionary<TK, TD> ReadQuery<TK, TD>(IDaoQuery<TR, TCmd> command, DaoReadRow<TK, TR> rowKey, DaoReadRow<TD, TR> readRow)
        {
            return command.ReadQuery(rowKey, readRow);
        }

        public Dictionary<TK, TD> ReadQuery<TK, TD>(string sql, DaoSetupParameters<TI> setupParameters, DaoReadRow<TK, TR> rowKey, DaoReadRow<TD, TR> readRow)
        {
            using (var cmd = NewQuery(sql, setupParameters))
            {
                return ReadQuery(cmd, rowKey, readRow);
            }
        }

        public Dictionary<TK, TD> ReadQuery<TK, TD>(string sql, DaoReadRow<TK, TR> rowKey, DaoReadRow<TD, TR> readRow)
        {
            return ReadQuery(sql, s => { }, rowKey, readRow);
        }

        public Dictionary<TK, TD> ReadQuery<TK, TD>(string sql, DaoSetupParameters<TI> setupParameters, DaoReadRowAndParams<TK, TR> rowKey, DaoReadRowAndParams<TD, TR> readRow)
        {
            using (var cmd = NewQuery(sql, setupParameters))
            {
                return cmd.ReadQuery(rowKey, readRow);
            }
        }

        public T ReadSingleRow<T>(string sql, DaoSetupParameters<TI> setupParameters, DaoReadRowAndParams<T, TR> readRow, T defaultValue = default(T))
        {
            using (var cmd = NewQuery(sql, setupParameters))
            {
                return cmd.ReadSingleRow(readRow, defaultValue);
            }
        }

        public T ReadSingleRow<T>(string sql, DaoSetupParameters<TI> setupParameters, DaoReadRow<T, TR> readRow, T defaultValue = default(T))
        {
            using (var cmd = NewQuery(sql, setupParameters))
            {
                return cmd.ReadSingleRow(readRow, defaultValue);
            }
        }

        public T ReadSingleRow<T>(string sql, DaoReadRow<T, TR> readRow, T defaultValue = default(T))
        {
            return ReadSingleRow(sql, h => { }, readRow, defaultValue);
        }

        public string ReadSingleString(string sql, DaoSetupParameters<TI> setupParameters, string defaultValue = null)
        {
            return ReadSingleRow(sql, setupParameters, h => h[0].AsString, defaultValue);
        }

        public int? ReadSingleIntNullable(string sql, DaoSetupParameters<TI> setupParameters)
        {
            return ReadSingleRow(sql, setupParameters, h => h[0].AsIntNullable, null);
        }

        public DateTime? ReadSingleDateNullable(string sql, DaoSetupParameters<TI> setupParameters)
        {
            return ReadSingleRow(sql, setupParameters, h => h[0].AsDateNullable, null);
        }

        public int ReadSingleInt(string sql, DaoSetupParameters<TI> setupParameters, int defaultValue = 0)
        {
            return ReadSingleRow(sql, setupParameters, h => h[0].AsInt, defaultValue);
        }

        public DateTime ReadSingleDate(string sql, DaoSetupParameters<TI> setupParameters, DateTime defaultValue = new DateTime())
        {
            return ReadSingleRow(sql, setupParameters, h => h[0].AsDate, defaultValue);
        }

        public int? ReadSingleIntNullable(string sql)
        {
            return ReadSingleIntNullable(sql, h => { });
        }

        public DateTime? ReadSingleDateNullable(string sql)
        {
            return ReadSingleDateNullable(sql, h => { });
        }

        public int ReadSingleInt(string sql, int defaultValue = 0)
        {
            return ReadSingleInt(sql, h => { }, defaultValue);
        }

        public DateTime ReadSingleDate(string sql, DateTime defaultValue = new DateTime())
        {
            return ReadSingleDate(sql, h => { }, defaultValue);
        }

        public string ReadSingleString(string sql, string defaultValue = null)
        {
            return ReadSingleString(sql, h => { }, defaultValue);
        }

        public int[] ExecuteNonQuery(IDaoCommand<TR, TCmd> command)
        {
            return command.ExecuteNonQuery();
        }

        /*public int[] ExecuteNonQuery(TCmd command)
        {
            return new DaoCommand(command, this).ExecuteNonQuery();
        }*/

        private int[] ExecuteNonQuery(string sql, DaoSetupParameters<TI, TIO, TO> setupParameters, bool isStoredProc)
        {
            using (var cmd =  isStoredProc? NewStoredProc(sql, setupParameters) : NewCommand(sql, setupParameters))
            {
                return cmd.ExecuteNonQuery();
            }
        }

        public int[] ExecuteNonQuery(string sql)
        {
            return ExecuteNonQuery(sql, helper => { }, false);
        }

        public int[] ExecuteNonQuery(string sql, DaoSetupParameters<TI, TIO, TO> setupParameters)
        {
            return ExecuteNonQuery(sql, setupParameters, false);
        }

        private List<T> ExecuteNonQuery<T>(string sql, DaoSetupParameters<TI, TIO, TO> setupParameters, DaoOnExecute<T, TR> onExecute, bool isStoredProc)
        {
            using (var cmd = NewCommand(sql, setupParameters))
            {
                return cmd.ExecuteNonQuery(onExecute);
            }
        }

        public List<T> ExecuteNonQuery<T>(string sql, DaoSetupParameters<TI, TIO, TO> setupParameters, DaoOnExecute<T, TR> onExecute)
        {
            return ExecuteNonQuery(sql, setupParameters, onExecute, false);
        }

        public List<T> ExecuteNonQuery<T>(string sql, DaoOnExecute<T, TR> onExecute)
        {
            return ExecuteNonQuery(sql, helper => { }, onExecute);
        }

        public List<T> ExecuteNonQuery<T>(IDaoCommand<TR, TCmd> command, DaoOnExecute<T, TR> onExecute)
        {
            return command.ExecuteNonQuery(onExecute);
        }

        public int[] ExecuteStoredProc(string storedProcName, DaoSetupParameters<TI, TIO, TO> setupParameters)
        {
            return ExecuteNonQuery(storedProcName, setupParameters, true);
        }

        public int[] ExecuteStoredProc(string storedProcName)
        {
            return ExecuteNonQuery(storedProcName, helper => { }, true);
        }

        public List<T> ExecuteStoredProc<T>(string storedProcName, DaoSetupParameters<TI, TIO, TO> setupParameters, DaoOnExecute<T, TR> onExecute)
        {
            return ExecuteNonQuery(storedProcName, setupParameters, onExecute, true);
        }

        public List<T> ExecuteStoredProc<T>(string storedProcName, DaoOnExecute<T, TR> onExecute)
        {
            return ExecuteNonQuery(storedProcName, h => { }, onExecute, true);
        }
    }
}
