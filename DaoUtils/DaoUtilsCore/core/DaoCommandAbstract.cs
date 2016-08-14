using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using DaoUtils.Standard;
using Common.Logging;
using DaoUtilsCore.core;
using DaoUtilsCore.def;

namespace DaoUtils.core
{

    internal delegate void OnParameters(IDaoParameterInternal parameter);
    class DaoCommandStatics
    {
        private static readonly string[] RemoveCommentsAndStringsRegexExpressions = {
            @"/\*([\r\n]|[^\r\n])*?\*/" // block comments
            ,@"[-]{2}.*?$" // sql line comments
            ,@"[/]{2}.*?$" // line comments
            ,@"'([^\r\n']*?)'" // sql string
            ,@"""([^\r\n""]*?)""" // double quoted string
            ,@"`([^\r\n`]*?)`" // mysql string
        };
        protected static readonly Regex RemoveCommentsAndStringsRegex = new Regex(string.Join("|", RemoveCommentsAndStringsRegexExpressions), RegexOptions.Multiline);
    }

    abstract class DaoCommandAbstract<TI, TIO, TO, TR, TCmd> : DaoCommandStatics, 
        IDaoCommand<TR, TCmd>,
        IDaoQuery<TR, TCmd>,
        IDaoSetupParametersHelper<TI, TIO, TO>,
        IDaoSetupParametersHelper<TI>
        where TI : IDaoParametersBuilderInput
        where TIO : IDaoParametersBuilderInputOutput
        where TO : IDaoParametersBuilderOutput
        where TR : IReadValue
        where TCmd : IDbCommand
    {
        private bool _disposed = false;
        private static readonly ILog LogStatic = LogManager.GetLogger(typeof(DaoCommandAbstract<TI, TIO, TO, TR, TCmd>));
        private ILog Log { get; }
        private readonly IDaoConnectionInfo _connectionInfo;
        protected readonly List<IDaoParameterInternal> Parameters = new List<IDaoParameterInternal>();
        protected virtual IDaoCommandParameterHelper CreateHelper() { return new DaoCommandParameterHelper(Parameters);}
        private IDaoCommandParameterHelper _helper;
        protected IDaoCommandParameterHelper Helper { get { return _helper = _helper ?? CreateHelper(); }}

        private void ValidateStatus()
        {
            if (_disposed) throw new CommandDisposedOfException("Command has been disposed of");
            if (Equals(Command, default(TCmd))) throw new DaoUtilsException("Command Is Null");
        }

        internal DaoCommandAbstract(TCmd command, IDaoConnectionInfo connectionInfo, IDaoCommandParameterHelper paramHelper, ILog log)
        {
            if (connectionInfo == null) throw new ArgumentNullException(nameof(connectionInfo));
            if (Equals(command, default(TCmd))) throw new ArgumentNullException(nameof(command));
            _helper = paramHelper;
            Log = log??LogStatic;
            Command = command;
            _connectionInfo = connectionInfo;
            ValidateStatus();
        }

        internal DaoCommandAbstract(TCmd command, IDaoConnectionInfo connectionInfo):this(command, connectionInfo, null, null)
        {
        }

        public TCmd Command { get; private set; }


        private const string FindParametersRegexFmt = @"(?<={0})\b[_a-zA-Z0-9]+\b";
        private string _findParametersRegex = null;
        protected static string RemoveCommentsAndStringsFrom(string sql)
        {
            return RemoveCommentsAndStringsRegex.Replace(sql, " ");
        }

        protected string[] SqlParameterNames(string sql)
        {
            _findParametersRegex = _findParametersRegex ?? string.Format(FindParametersRegexFmt, _connectionInfo.ParamPrefix);
            return (from Match match in Regex.Matches(RemoveCommentsAndStringsFrom(sql), _findParametersRegex) select match.Value).ToArray();
        }


        protected void AttachParameters()
        {
            var sqlParameterNames = SqlParameterNames(Command.CommandText);
            Helper.ValidateParameters(sqlParameterNames);
            var dictionaryCreated = Helper.ParamertersByName();
            Command.Parameters.Clear();
            /* Had an issue with Oracle 11 where it seem to bind by index even though names were used in sql & parameters
               only found out because kept getting ORA-06502: PL/SQL: numeric or value error: character to number conversion error
               when introducing new parameter :s 
             * so just add parameters in same order as in sql/command text
             */
            foreach (var sqlParameterName in sqlParameterNames)
            {
                var paramter = dictionaryCreated[sqlParameterName.ToLower()].Parameter;
                paramter.ParameterName = sqlParameterName;
                Command.Parameters.Add(paramter);
            }
        }

        private DaoCommandAbstract<TI, TIO, TO, TR, TCmd> SetupParameters(Action setupParameters)
        {
            ValidateStatus();
            Parameters.Clear();
            setupParameters();
            AttachParameters();
            return this;
        }

        public DaoCommandAbstract<TI, TIO, TO, TR, TCmd> SetupParameters(DaoSetupParameters<TI, TIO, TO> setupParameters)
        {
            return SetupParameters(() => setupParameters(this));
        }

        public DaoCommandAbstract<TI, TIO, TO, TR, TCmd> SetupParameters(DaoSetupParameters<TI> setupParameters)
        {
            return SetupParameters(() => setupParameters(this));
        }

        public abstract IDaoParametersBuilderDirection<TI, TIO, TO> Name(string parameterName);

        TI IDaoSetupParametersHelper<TI>.Name(string parameterName)
        {
            return Name(parameterName).Input;
        }

        protected void LogExection(Exception e, string action)
        {
            var msg = $"\"{e?.GetType().Name}\" - \"{e?.Message}\" thrown during {action} \"{Command.CommandText}\"";
            Log.Error(msg.Trim(), e);
        }

        virtual public int[] ExecuteNonQuery()
        {
            ValidateStatus();
            List<int> result = new List<int>();
            try
            {
                _connectionInfo.WaitOpen();
                Command.Transaction = _connectionInfo.ActiveTransaction;
                Helper.Execute(CommandExecuteMode.NonQuery, i => result.Add(Command.ExecuteNonQuery()));
            }
            catch (Exception e)
            {
                LogExection(e, "executing non query");
                throw;
            }
            finally
            {
                Command.Transaction = null;
            }
            return result.ToArray();
        }

        abstract protected IReadHelper<TR> ReadHelper(List<IDaoParameterInternal> parameters);

        public List<T> ExecuteNonQuery<T>(DaoOnExecute<T, TR> onExecute)
        {
            var helper =  ReadHelper(Parameters);
            var result = new List<T>();
            var calls = ExecuteNonQuery();
            Helper.ReadReturnedParams(idx => result.Add(onExecute(helper, calls.Length > idx ? calls[idx] : -1)));
            return result;
        }

        abstract protected IReadHelper<TR> ReadHelper(IDataReader dataReader);

        public List<T> ReadQuery<T>(DaoReadRowAndParams<T, TR> readRow)
        {
            if (readRow == null) throw new ArgumentNullException(nameof(readRow));
            ValidateStatus();
            var result = new List<T>();
            var parameterHelper = ReadHelper(Parameters);
            try
            {
                _connectionInfo.WaitOpen();
                Command.Transaction = _connectionInfo.ActiveTransaction;
                Helper.Execute(CommandExecuteMode.Query, i =>
                {
                    using (var dr = Command.ExecuteReader())
                    {
                        var drHelper = ReadHelper(dr);
                        while (dr.Read())
                        {
                            Helper.RecordRow();
                            result.Add(readRow(drHelper, parameterHelper));
                        }
                    }
                });
            }
            catch (Exception e)
            {
                LogExection(e, "Reading Query");
                throw;
            }
            finally
            {
                Command.Transaction = null;
            }
            return result;
        }

        public List<T> ReadQuery<T>(DaoReadRow<T, TR> readRow)
        {
            if (readRow == null) throw new ArgumentNullException(nameof(readRow));
            return ReadQuery<T>((drHelper, parameterHelper) => readRow(drHelper));
        }

        public Dictionary<TK, TD> ReadQuery<TK, TD>(DaoReadRow<TK, TR> rowKey, DaoReadRow<TD, TR> readRow)
        {
            if (readRow == null) throw new ArgumentNullException(nameof(readRow));
            if (rowKey == null) throw new ArgumentNullException(nameof(rowKey));
            return ReadQuery((drHelper, parameterHelper) => rowKey(drHelper), (drHelper, parameterHelper) => readRow(drHelper));
        }

        public Dictionary<TK, TD> ReadQuery<TK, TD>(DaoReadRowAndParams<TK, TR> rowKey, DaoReadRowAndParams<TD, TR> readRow)
        {
            if (readRow == null) throw new ArgumentNullException(nameof(readRow));
            if (rowKey == null) throw new ArgumentNullException(nameof(rowKey));
            ValidateStatus();
            var parameterHelper = ReadHelper(Parameters);
            _connectionInfo.WaitOpen();
            Command.Transaction = _connectionInfo.ActiveTransaction;
            var result = new Dictionary<TK, TD>();
            try
            {
                Helper.Execute(CommandExecuteMode.Query, i =>
                {
                    using (var dr = Command.ExecuteReader())
                    {
                        var drHelper = ReadHelper(dr);
                        while (dr.Read())
                        {
                            Helper.RecordRow();
                            var key = rowKey(drHelper, parameterHelper);
                            try
                            {
                                result.Add(key, readRow(drHelper, parameterHelper));
                            }
                            catch (Exception e)
                            {
                                throw new DaoUtilsException($"Error add/reading Key '{key}'",e);
                            }
                        }
                    }
                });
            }
            catch (Exception e)
            {
                LogExection(e, "Reading Query");
                throw;
            }
            finally
            {
                Command.Transaction = null;
            }
            return result;
        }

        public T ReadSingleRow<T>(DaoReadRow<T, TR> readRow, T defaultValue = default(T))
        {
            if (readRow == null) throw new ArgumentNullException(nameof(readRow));
            return ReadSingleRow((drHelper, parameterHelper) => readRow(drHelper), defaultValue);
        }

        public T ReadSingleRow<T>(DaoReadRowAndParams<T, TR> readRow, T defaultValue = default(T))
        {
            if (readRow == null) throw new ArgumentNullException(nameof(readRow));
            ValidateStatus();
            var parameterHelper = ReadHelper(Parameters);
            T result = defaultValue;
            _connectionInfo.WaitOpen();
            Command.Transaction = _connectionInfo.ActiveTransaction;
            try
            {
                var found = false;
                Helper.Execute(CommandExecuteMode.Query, i =>
                {
                    if (found) return;
                    using (var dr = Command.ExecuteReader())
                    {
                        var drHelper = ReadHelper(dr);
                        if (!dr.Read()) return;
                        Helper.RecordRow();
                        result = readRow(drHelper, parameterHelper);
                        found = true;
                    }
                });
                return result;
            }
            catch (Exception e)
            {
                LogExection(e, "Reading Single Row");
                throw;
            }
            finally
            {
                Command.Transaction = null;
            }
        }

        public object ExecuteScalar()
        {
            ValidateStatus();
            try
            {
                return Command.ExecuteScalar();
            }
            catch (Exception e)
            {
                LogExection(e, "ExecuteScalar");
                throw;
            }
        }

        public T ExecuteScalar<T>(T defaultValue = default(T))
            where T: IConvertible
        {
            var value = ExecuteScalar();
            try
            {
                return DataTypeAdaptor.ConvertValue(value, defaultValue);
            }
            catch (Exception e)
            {
                LogExection(e, $"converting to {typeof(T)} value \"{value}\" returned by ExecuteScalar");
                throw;
            }
        }


        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing && !Equals(Command, default(TCmd)))
            {
                Command.Dispose();
            }
            Command = default(TCmd);
            _disposed = true;
        }

    }

}
