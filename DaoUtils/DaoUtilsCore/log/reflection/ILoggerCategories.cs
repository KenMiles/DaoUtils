using System;
using System.Collections.Generic;
using System.Text;

namespace DaoUtilsCore.log.reflection
{
    interface ILoggerCategories
    {
        ILoggerCategory Debug { get; }
        ILoggerCategory Info { get; }
        ILoggerCategory Warn { get; }
        ILoggerCategory Error { get; }
        ILoggerCategory Fatal { get; }
    }
}
