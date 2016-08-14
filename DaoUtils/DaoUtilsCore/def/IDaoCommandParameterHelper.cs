using System;
using System.Collections.Generic;
using System.Text;
using DaoUtils.Standard;

namespace DaoUtilsCore.def
{
    internal delegate void CommandActionDelegate(int callIdx);
    internal enum CommandExecuteMode {NonQuery, Query};
    internal interface IDaoCommandParameterHelper
    {
        void ReadReturnedParams(CommandActionDelegate onRead);
        void Execute(CommandExecuteMode mode, CommandActionDelegate onExecute);
        void RecordRow();
        void ValidateParameters(IEnumerable<string> queryParameterNames);
        Dictionary<string, IDaoParameterInternal> ParamertersByName();
    }
}
