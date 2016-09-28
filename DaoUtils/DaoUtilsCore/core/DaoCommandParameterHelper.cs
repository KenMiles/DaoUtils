using System;
using System.Collections.Generic;
using System.Linq;
using DaoUtilsCore.log;
using DaoUtils.Standard;
using DaoUtilsCore.core;
using DaoUtilsCore.def;

namespace DaoUtils.core
{
    class DaoCommandParameterHelper: IDaoCommandParameterHelper
    {
        protected readonly List<IDaoParameterInternal> Parameters;
        private static readonly ILog LogStatic = LogManager.GetLogger(typeof(DaoCommandParameterHelper));
        private readonly ILog _log;

        internal DaoCommandParameterHelper(List<IDaoParameterInternal> parameters, ILog log)
        {
            Parameters = parameters;
            _log = log ?? LogStatic;
        }

        public DaoCommandParameterHelper(List<IDaoParameterInternal> parameters) : this(parameters, LogStatic)
        {
        }

        private List<IDaoParameterInternal> _changingInputs;

        private List<IDaoParameterInternal> GetChangingInputs()
        {
            return Parameters.Where(p => p.IsInput && p.InputParamArraySize > 0).ToList();
        }

        private List<IDaoParameterInternal> ChangingInputs => _changingInputs = _changingInputs ?? GetChangingInputs(); 

        private List<IDaoParameterInternal> _changingOutputs;
        private List<IDaoParameterInternal> ChangingOutputs => _changingOutputs = _changingOutputs ?? Parameters.Where(p => p.IsOutput).ToList(); 

        private int Row { get; set; }

        internal void PreCall(int index)
        {
            Row = 0;
            ChangingInputs.ForEach(p => p.PreCall(index));
        }

        internal void PostCall()
        {
            ChangingOutputs.ForEach(p => p.PostCall());
        }

        internal void SetReadParameterIndex(List<IDaoParameterInternal> parameters, int index)
        {
            parameters.ForEach(param => param.SetReadIndex(index));
        }

        internal static string NoOfCallsStr(int call, int noOfCalls, int row = 0)
        {
            var desc = new [] {row < 1 ? "" : $"row {row}", noOfCalls <= 1 ? "" : $"{call} of {noOfCalls} calls" };
            return string.Join("; ", desc.Where(s => !string.IsNullOrWhiteSpace(s)));
        }

        internal string ParamValues(bool readReturnValuesStage)
        {
            return string.Join("; ", Parameters.Select(p => p.ForLog(readReturnValuesStage)));
        }

        public void ReadReturnedParams(CommandActionDelegate onRead)
        {
            var arrayParams = Parameters.Where(p => p.OutputParamArraySize > 0).ToList();
            int noCallsMade = arrayParams.Count == 0 ? 1 : Math.Max(arrayParams.Max(p => p.OutputParamArraySize), 1);
            for (int idx = 0; idx < noCallsMade; idx++)
            {
                try
                {
                    SetReadParameterIndex(arrayParams, idx);
                    onRead(idx);
                }
                catch (Exception e)
                {
                    var message = $"{e.Message} on {NoOfCallsStr(idx, noCallsMade)}, params = {ParamValues(true)}";
                    _log.Error(message, e);
                    throw new DaoUtilsException(message, e);
                }
            }
        }

        private void ExecuteCall(CommandActionDelegate onExecute, bool isQuery, int call)
        {
            try
            {
                PreCall(call);
                onExecute(call);
                if (!isQuery) PostCall();
            }
            catch (Exception e)
            {
                var message = $"{e.Message} on {NoOfCallsStr(call, _noCallsRequired, Row)}, params = {ParamValues(false)}";
                _log.Error(message, e);
                throw new DaoUtilsException(message, e);
            }
        }

        protected virtual void PreOnExecute(int noCallsRequired, bool isQuery)
        {
        }

        protected virtual int NoCallsRequired(bool queryMode)
        {
            if (Parameters.Count == 0) return 1;
            var max = Math.Max(Parameters.Max(p => p.InputParamArraySize), 1);
            var errorParams = Parameters.Where(p => p.InputParamArraySize > 0).Where(p => p.InputParamArraySize != max);
            if (!errorParams.Any()) return max;
            var paramsAndLength = Parameters.Where(p => p.InputParamArraySize > 0)
                    .Select(p => $"{p.Name}: {p.InputParamArraySize} values");
            var message = $"Mismatch in input array parameter lengths - {string.Join(",", paramsAndLength)}";
            _log.Error(message);
            throw new DaoUtilsException(message);
        }


        private int _noCallsRequired;
        public void Execute(CommandExecuteMode mode, CommandActionDelegate onExecute)
        {
            var query = mode == CommandExecuteMode.Query;
            _noCallsRequired = NoCallsRequired(query);
            _changingInputs = null;
            _changingOutputs = null;
            Parameters.ForEach(p => p.PreOnExecute(query, _noCallsRequired));
            PreOnExecute(_noCallsRequired, query);
            for (int iCall = 0; iCall < _noCallsRequired; iCall++)
            {
                ExecuteCall(onExecute, query, iCall);
            }
        }

        public void RecordRow()
        {
            Row++;
        }

        private static void AddErrors(List<string> errorList, string errorDescription, string[] parameterNames)
        {
            if (!parameterNames.Any()) return;
            errorList.Add($"{errorDescription}: {string.Join(", ", parameterNames)}");
        }
        
        public void ValidateParameters(IEnumerable<string> queryParameterNames, IEnumerable<string> ignoreParamNames, bool ignoreQueryParamsIsses)
        {
            var errors = new List<string>();
            var ignoreParams = new HashSet<string>(ignoreParamNames, StringComparer.InvariantCultureIgnoreCase);
            var queryParameters = queryParameterNames?.Except(ignoreParams).Select(name => name.ToLower()).ToList()??new List<string>();
            var createdParameters = Parameters.Select(p => p.Name.ToLower()).ToList();
            if (!ignoreQueryParamsIsses)
            {
                AddErrors(errors, "Missing Parameters", queryParameters.Except(createdParameters).ToArray());
                AddErrors(errors, "Unknown Parameters", createdParameters.Except(queryParameters).ToArray());
                AddErrors(errors, "Duplicated Query Parameters", queryParameters.GroupBy(name => name).Where(dup => dup.Count() > 1).Select(dup => dup.Key).ToArray());
            }
            AddErrors(errors, "Duplicated Added Parameters", createdParameters.GroupBy(name => name).Where(dup => dup.Count() > 1).Select(dup => dup.Key).ToArray());
            var paramList = Parameters.Where(p => p.InputParamArraySize > 0).GroupBy(p => p.InputParamArraySize).ToArray();
            if (paramList.Count() > 1)
            {
                var paramaterNames =
                    Parameters.Where(p => p.InputParamArraySize > 0)
                        .Select(p => $"{p.Name} - {p.InputParamArraySize}")
                        .ToArray();
                AddErrors(errors, "Different Number Parameter Values in Arrays", paramaterNames);
            }
            if (errors.Count > 0)
            {
                _log.Error(string.Join("\r\n", errors));
                throw new DaoUtilsException(string.Join("\r\n", errors));
            }
        }

        public Dictionary<string, IDaoParameterInternal> ParamertersByName()
        {
            return Parameters.ToDictionary(p => p.Name, p => p, StringComparer.InvariantCultureIgnoreCase);
        }

    }
}