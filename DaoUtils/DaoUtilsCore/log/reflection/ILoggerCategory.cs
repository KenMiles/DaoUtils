using System;
using System.Collections.Generic;
using System.Text;

namespace DaoUtilsCore.log.reflection
{
    interface ILoggerCategory
    {
        bool IsEnabled(object logger);

        void Log(object logger, object message);
        void Log(object logger, object message, Exception exception);
        void LogFormat(object logger, string format, params object[] args);
        void LogFormat(object logger, IFormatProvider provider, string format, params object[] args);
    }
}
