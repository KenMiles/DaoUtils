using System;
using System.Collections.Generic;
using System.Text;
using DaoUtilsCore.log.reflection;

namespace DaoUtilsCore.log.console
{
    class LogCategoryConsole: ILoggerCategory
    {
        private readonly string _category;
        private readonly bool _enabled;
        private readonly bool _errorStream;
        private readonly IConsolWrapper _writer;

        public LogCategoryConsole(string category, bool enabled, bool errorStream): this(category, enabled, errorStream, new ConsolWrapper())
        {
        }

        internal LogCategoryConsole(string category, bool enabled, bool errorStream, IConsolWrapper writer)
        {
            _category = category;
            _enabled = enabled;
            _errorStream = errorStream;
            _writer = writer ?? new ConsolWrapper();
        }

        public bool IsEnabled(object logger)
        {
            return _enabled;
        }

        public void Log(object logger, object message)
        {
            Log(logger, message, null);
        }

        public void Log(object logger, object message, Exception exception)
        {
            var logEntry = $"{logger}: {_category} {message}" + (exception != null ? $"{Environment.NewLine}{exception}":"");
            _writer.WriteLnToDebug(logEntry);
            if (_enabled)_writer.WriteLn($"{DateTime.Now:G}\t{logEntry}", _errorStream);
        }

        public void LogFormat(object logger, string format, params object[] args)
        {
            Log(logger, string.Format(format, args), null);
        }

        public void LogFormat(object logger, IFormatProvider provider, string format, params object[] args)
        {
            Log(logger, string.Format(provider, format, args), null);
        }
    }
}
